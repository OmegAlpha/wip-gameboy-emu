using System;
using Drongo.GameboyEmulator.Utils;
using UnityEngine;

namespace Drongo.GameboyEmulator
{
    // Memory map
    // More info at https://rylev.github.io/DMG-01/public/book/memory_map.html

    /// <summary>
    /// Bus of Addresses
    /// </summary>
    public class AddressBus
    {
         // quick accessors to specific memory locations
        
         //FF00 - joyPad
         public byte joyPad { get => _io[0x00]; set => _io[0x00] = value; }

         // FF04 - DIV - Divider Register 
         // it gets incremented automatically by the clock  at rate of 16384Hz (~16779Hz on SGB)
         // but if you write to it manually, it gets reset to 0.
         public byte div => _io[0x04];

         // FF40 - lcdControl - LCD Control (R/W)
         // controls the LCD with 8 different flags
         // see here: https://i.gyazo.com/cd92a3544cf882e2c1516254a0b2c640.png
         public byte lcdControl => _io[0x40];
         
         // FF41 - STAT - lcdControl Status (R/W)
         // series of status bits
         // see here: https://i.gyazo.com/51e518a67556e6324b70a88ad664b98b.png
         public byte lcdStat { get => _io[0x41];  set => _io[0x41] = value; }

         // for Background Layer
         // FF42 - scy - Scroll Y (R/W)
         // FF43 - scx - Scroll X (R/W)
         public byte scy => _io[0x42];
         public byte scx => _io[0x43];

         // FF44 - LY - lcdControl Y-Coordinate (R) bypasses on write always 0
         // current scanline number
         // instead of going from 0 to 144 (screen width), 
         // it goes from 0 to 154. The remaining 10 scan-lines are used
         // for padding the timer and nothing is drawn in the lcd
         public byte ly { get => _io[0x44];  set => _io[0x44] = value; }
         
         //FF45 - LYC - LY Compare(R/W)
         // if this matches LY, and interrupt is triggered
         // allowing to produce mid-frame effects
         public byte lyCompare => _io[0x45];

         //FF47 - BG Palette - Non CGB Mode Only
         public byte bgPalette => _io[0x47];
         
         //FF48 - Objects Palette0 - Non CGB Mode Only
         //FF49 - Objects Palette1  - Non CGB Mode Only
         public byte objPalette0 => _io[0x48];
         public byte objPalette1 => _io[0x49];

         // for Windows Layer
         //FF4A - wy - Window Y Position (R/W)
         //FF4B - wx - Window X Position minus 7 (R/W)
         // todo: the minus 7 was in a doc file I read but can't remember why it was important
         public byte wy => _io[0x4A];
         public byte wx => _io[0x4B];

         // IE - Interrupt Enable: is this interrupt enabled or not? 
         // IF - Interrupt Requested: is this interrupt requested?
         // the mask tells you for each bit what type of interrupt
         // was requested: 
         // bit 0: V-Blank
         // bit 1: LCD Stat
         // bit 2: Timer 
         // bit 3: Serial
         // bit 4: Joypad

         // FF0F
         public byte IF { get => _io[0x0F];  set => _io[0x0F] = value; }

         // FFFF
        public byte IE { get => _hRam[0x7F];  set => _hRam[0x7F] = value; }

        // memory map
        
        private byte[] _wRam0 = new byte[0x1000];
        private byte[] _wRam1 = new byte[0x1000];
        private byte[] _io = new byte[0x80];
        private byte[] _hRam = new byte[0x80];
        
        private byte[] _wRam0Echo = new byte[0x1000];
        private byte[] _wRam1Echo = new byte[0x1000];
        
        // public for quick ppu accessing
        public byte[] oam = new byte[0xA0];
        public byte[] vRam = new byte[0x2000];
        
        private byte[] _reserved = new byte[0x80];
        
        private GameBoy _gb;
        
        
        public AddressBus(GameBoy gb)
        {
            _gb = gb;
            
            Tools.ZeroFill(ref vRam);
            Tools.ZeroFill(ref _wRam0);
            Tools.ZeroFill(ref _wRam1);
            Tools.ZeroFill(ref oam);
            Tools.ZeroFill(ref _io);
            Tools.ZeroFill(ref _hRam);
        }
        
        public byte Read8(ushort address)
        {
            int readValue = -1;
            
            // 0x0000 - 0x3FFF = ROM Bank 0 
            if (address <= 0x3FFF)
            {
                readValue = _gb.cart.ReadLowRom(address);
            }
            // 0x4000 - 0x7FFF = ROM Bank 1..n (switcheable banks)
            else if (address <= 0x7FFF)
            {
                readValue = _gb.cart.ReadHighRom(address);
            }
            // 0x8000 - 0x9FFF Video Ram
            // -- tiles (16 bytes each)
            // (from 0x8000 + 128 * 16 are the tiles for 8000 method)
            // (from 0x9000 +/- 128 *16 are the tiles for 8800 method)
            // -- bg maps (32x32 bytes each - 1kb)
            // (from 0x9800 to x9BFF and 0x9C00 to 0x9FFF the background / window grids are stored)
            else if (address <= 0x9FFF)
            {
                readValue = vRam[address & 0x1FFF];
            }
            // 0xA000 - 0xBFFF : Cartridge RAM - External Ram
            else if (address <= 0xBFFF)
            {
                readValue = _gb.cart.ReadERam(address);
            }
            // 0xC000 - 0xCFFF : Work RAM Bank 0
            else if (address <= 0xCFFF)
            {
                readValue = _wRam0[address & 0xFFF];
            }
            // 0xD000 - 0xDFFF : RAM Bank 1-7 - switchable - Color only
            else if (address <=  0xDFFF)
            {
                int target = address & 0xFFF;
                readValue = _wRam1[target];
            }
            // Echo Ram 0
            else if (address <= 0xEFFF)
            {
                readValue = _wRam0Echo[address & 0xFFF];
            }
            // Echo Ram 1
            else if (address <= 0xFDFF)
            {
                readValue = _wRam1Echo[address & 0xFFF];
            }
            // 0xFE00 - 0xFE9F : Object Attribute Memory
            // contains data used to display Sprites/Objects on screen.
            // each Sprite takes up to 4 bytes, so it's a max of 40 sprites.
            // 4 bytes of each sprite:
            // 0: y position (16 would be top. this is so it can "enter" the frame )
            // 1: x position (7 would be left. this is so it can "enter" the frame )
            // 2: Tile Number to grab from vram the sprite data. Sprites always use 8000 method.
            // 3: Flags: a set of effects that can be applied to a sprite in this byte
            // ------ 7: 0=sprite above bg, 1= overlays with bg (sort of)
            // ------ 6: y flip
            // ------ 5: x flip
            // ------ 4 palette number (obp0 or obp1)
            // ------ 3-0: for the game boy color only (some day I'll upgrade this emu :P) 
            else if (address <= 0xFE9F)
            {
                readValue = oam[address & 0x9F];
            }
            // unused reserved
            // 0xFE80 - 0xFEFF
            else if (address <= 0xFEFF)
            {
                readValue = _reserved[address & 0x7F];
            }
            // FF00-FF7F IO Ports
            else if (address <= 0xFF7F)
            {
                int add = address & 0x7F;
                
                readValue = _io[add];
            }
            // FF80-FFFE High RAM(HRAM)
            else if (address <= 0xFFFF)
            {
                readValue = _hRam[address & 0x7F];
            }
            
            return (byte) readValue;
        }

        public void Write8(ushort address, byte value)
        {
            // 0x0000 - 0x3FFF = ROM Bank 0 
            if (address <= 0x3FFF)
            {
                _gb.cart.WriteRom(address, value);
            }
            // 0x4000 - 0x7FFF = ROM Bank 1..n (switcheable banks)
            else if (address <= 0x7FFF)
            {
                _gb.cart.WriteRom(address, value);
            }
            // 0x8000 - 0x9FFF Video Ram
            else if (address <= 0x9FFF)
            {
                vRam[address & 0x1FFF] = value;
            }
            // 0xA000 - 0xBFFF : Cartridge RAM - external ram
            else if (address <= 0xBFFF)
            {
                _gb.cart.WriteERam(address, value);
            }
            // 0xC000 - 0xCFFF : Work RAM Bank 0
            else if (address <= 0xCFFF)
            {
                _wRam0[address & 0xFFF] = value;
            }
            // 0xD000 - 0xDFFF : RAM Bank 1-7 - switchable - Color only
            else if (address <= 0xDFFF)
            {
                int target = address & 0xFFF;
                _wRam1[target] = value;
            }
            // Echo Ram 0
            else if (address <= 0xEFFF)
            {
                _wRam0Echo[address & 0xFFF] = value;
            }
            // Echo Ram 1
            else if (address <= 0xFDFF)
            {
                _wRam1Echo[address & 0xFFF] = value;
            }
            // 0xFE00 - 0xFE9F : Object Attribute Memory
            else if (address <= 0xFE9F)
            {
                oam[address & 0x9F] = value;
            }
            // unused reserved
            else if ( address <= 0xFEFF)
            {
                _reserved[address & 0x7F] = value;
            }
            // IO 0xFF00 to 0xFF4B
            else if ( address <= 0xFF7F)
            {
                // dma transfer
                if (address == 0xFF46)
                {
                    DMATransfer(value);
                }
                else
                {
                    int add = address & 0x7F;
                    _io[add] = value;                    
                }
            }
            // High Ram
            else if (address <= 0xFFFF)
            {
                _hRam[address & 0x7F] = value;
            }
        }

        public ushort Read16(ushort address)
        {
            // little endian, the less significant first
            byte low = Read8(address);
            byte high = Read8((ushort)(address + 1));

            ushort value = (ushort)(high << 8 | low);
            return value;
        }
        
        public void Write16(ushort address, ushort value)
        {
            byte low = (byte) (value & 0xFF);
            byte high = (byte) (value >> 8);
            
            // little endian, the less significant first
            Write8(address, low);
            Write8( (ushort) (address + 1), high);
        }
        
        public void RequestInterrupt(byte bit)
        {
            IF = Tools.SetBit(IF, bit);
        }

        private void DMATransfer(ushort value)
        {
            ushort address = (ushort)(value << 8);
            for (byte i = 0; i < oam.Length; i++) 
            {
                oam[i] = Read8((ushort)(address + i));
            }
        }
    }
}