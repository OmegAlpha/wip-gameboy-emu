using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Drongo.GameboyEmulator
{
    
    /// <summary>
    /// CartridgeRom
    /// </summary>
    public abstract class CartridgeRom
    {
        private const string ROMS_FOLDER = "Content/ROMS";
        
        public RomHeader header;

        protected byte[] _loadedRom;

        protected bool _testMode;

        public void Load(string romName)
        {
            if (romName == "TEST_RUNNER")
            {
                _testMode = true;
                _loadedRom = new byte[0Xffff];
                return;
            }
            
            string romPath = Path.Combine(Application.dataPath, ROMS_FOLDER);
            romPath = Path.Combine(romPath, romName + ".gb");

            _loadedRom = File.ReadAllBytes(romPath);

            header = new RomHeader();
            header.Read(ref _loadedRom);
        }

        public abstract byte ReadLowRom(ushort address);
        public abstract byte ReadHighRom(ushort address);
        public abstract void WriteRom(ushort address, byte value);
        public abstract byte ReadERam(ushort address);
        public abstract void WriteERam(ushort address, byte value);
    }
}