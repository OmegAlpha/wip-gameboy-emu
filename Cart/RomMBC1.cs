using Drongo.GameboyEmulator;
using UnityEngine;

namespace Emulator.Cart
{
    // documentation
    // https://niwanetwork.org/wiki/MBC1_(Game_Boy_mapper)
    // https://b13rg.github.io/Gameboy-MBC-Analysis/
    // https://b13rg.github.io/Gameboy-Bank-Switching/#:~:text=The%20MBC1%20is%20only%20able,%2460%20to%20%242000%2D%243FFF%20.
    
    public class RomMBC1 : CartridgeRom
    {
        private const int ROM_OFFSET = 0x4000;
        private const int ERAM_OFFSET = 0x2000;
        
        // external ram
        // When MBC1 is set to maximize the eram space,
        // the max value is 0x8000
        private byte[] _eram = new byte[0x8000];  

        private int _romBank;

        private int _ramBank;

        private int _bankingMode; // 0 rom - 1 ram

        private bool _isEramEnabled;
        
        
        public override byte ReadLowRom(ushort address)
        {
            return _loadedRom[address];
        }

        public override byte ReadHighRom(ushort address)
        {
            int offset = (ROM_OFFSET * _romBank);
            int addr = (address & 0x3FFF);

            int target = addr + offset;
            
            if(target >= _loadedRom.Length)
                Debug.LogError("lalalala");
            
            
            return _loadedRom[target];
        }

        public override byte ReadERam(ushort address)
        {
            if (_isEramEnabled)
                return _eram[(ERAM_OFFSET * _ramBank) + (address & 0x1FFF)];
            
            return 0xFF;
        }

        public override void WriteERam(ushort address, byte value)
        {
            if (_isEramEnabled)
            {
                _eram[(ERAM_OFFSET * _ramBank) + address & 0x1FFF] = value;
            }
        }
        
        // bank switching
        // https://b13rg.github.io/Gameboy-Bank-Switching/#:~:text=The%20MBC1%20is%20only%20able,%2460%20to%20%242000%2D%243FFF%20.
        public override void WriteRom(ushort address, byte value)
        {
            // 0x0000 - 0x1FFF
            if (address < 0x2000)
            {
                // <<Explanation>>
                // this address space is read only (from 0 to 4000) and it's part of the ROM Bank 00
                // however, if you try to write from 0 to 1FFF, it's going to be intercepted 
                // and used as "flag setter" for ram enabled/disabled
                // if you write specifically 0x000A, then the device it's gonna interpret 
                // that you want to enable the ram. Any other value sent to this this range
                // of addresses, will mean that you want to disable the ram

                // activate / deactivate eram flag
                _isEramEnabled = (value == 0x0A);
            }
            // 0x2000 - 0x3FFF
            else if (address < 0x4000)
            {
                // <<Explanation>>
                // this address space is read only (from 0 to 4000) and it's part of the ROM Bank 00
                // however, if you try to write from 0x2000 to 0x3FFF, it's going to be intercepted
                // and used for rom bank switching. 
                // The device will take the lower 5 bits and use them as "bank number" you want to swap in.
                // (5 bits it's b1111 or x1F, and you would think that means a total of 31 possible bank roms,
                // however, these 5 bits are combined with the 2 bits from 0x4000 - 0x5FFF when ROM mode, and that
                // gives a total of 7 bits, which is 127 values. So 127 roms banks are available to use)
                // You can't choose x00, because that's already present at 0x0000 - x1FFF, so if you choose
                // this number, it's going to be converted to x01
                // Also, it looks like if you send x20, x40, x60 they also are interpreted with value + 1.
                // Different documentations refer to this as a hardware design flaw, and it was corrected
                // in the next MBC models. 
                // So, if you take this into account, the number of possible bank roms is not the total mentioned above
                // but that number minus these "bugged" addresses
                
                // swapping selected rom bank in ->
                _romBank = value & 0x1F;
                
                // fixing the mentioned hardware flaw
                if (_romBank == 0x00 || _romBank == 0x20 || _romBank == 0x40 || _romBank == 0x60)
                    _romBank++;
            }
            // 0x4000 - 0x5FFF
            else if (address < 0x6000)
            {
                // <<Explanation>>
                // similar to previous cases, this sector is rom read only, but writing to it will
                // be intercepted and have a used for setting the ROM/RAM modes.
                // only the first 2  bits are used for the following logic
                // if bank mode is ram: 
                //   it specifies which RAM bank should be loaded into 0xA000 - 0xBFFF. 
                //   if in this mode, only ROM banks 0 and 1 may be used
                // if bank mode is rom:
                //   it will specify the upper 2 bits of the ROM bank number. 
                //   if in this mode, only RAM bank 0 may be used

                if (_bankingMode == 0)
                {
                    _romBank |= value & 0x3; // last 2 bits
                    if (_romBank == 0x00 || _romBank == 0x20 || _romBank == 0x40 || _romBank == 0x60)
                        _romBank++;
                }
                else
                {
                    _ramBank = value & 0x3; // last 2 bits
                }
            }
            // 0x6000 - 0x7FFF
            else if (address <= 0x8000)
            {
                // <<Explanation>>
                // Again a read only area that has special functionality if you try to write there
                // this simply select the banking mode
                // mode 0:   16mb ROM / 8kb RAM
                // mode 1:   4mb ROM / 32kb RAM
                _bankingMode = value & 1;
            }
        }
    }
}