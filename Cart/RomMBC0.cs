using Drongo.GameboyEmulator;

namespace Emulator.Cart
{
    public class RomMBC0 : CartridgeRom
    {
        public override byte ReadLowRom(ushort address)
        {
            return _loadedRom[address];
        }

        public override byte ReadHighRom(ushort address)
        {
            return _loadedRom[address];
        }

        public override void WriteRom(ushort address, byte value)
        {
            // MBC 0 doesn't support writing to ROM 
            // Debug.LogError("fail");
            if(_testMode)
                _loadedRom[address] = value;
        }

        public override byte ReadERam(ushort address)
        {
            // Debug.LogError("fail");
            // MBC 0 doesn't support ERam
            return _testMode ? _loadedRom[address] : (byte)0xFF;
        }

        public override void WriteERam(ushort address, byte value)
        {
            // Debug.LogError("fail");
            // MBC 0 doesn't support ERam

            if (_testMode)
            {
                _loadedRom[address] = value;    
            }
        }
    }
}