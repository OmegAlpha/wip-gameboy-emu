using System.Collections.Generic;
using Drongo.GameboyEmulator.Utils;
using UnityEngine;

namespace Drongo.GameboyEmulator
{
    public static class DebugInstructions
    {
        private static InstructionDebug[] s_cache = new InstructionDebug[0x10000];

        public static InstructionDebug Get(GameBoy gb, ushort address)
        {
            if (s_cache[address] == null)
            {
                InstructionDebug inst = null;
                inst = new InstructionDebug(gb, address);
                s_cache[address] = inst;
            }

            return s_cache[address];
        }
    }
    
    public class InstructionDebug
    {
        public int length;
        public byte[] bytes;
        public string bytesString;
        public ushort address;
        public byte code;
        
        private GameBoy _gb;

        public InstructionDebug(GameBoy gb, ushort address)
        {
            _gb = gb;

            this.address = address;

            byte instruction = _gb.bus.Read8(address);
            
            if (OperationsMap.map[instruction] == null)
            {
                Debug.LogError(Tools.HexString(instruction));
                return;
            }

            length = OperationsMap.map[instruction].length;
            
            bytes = new byte[length];
            
            for (int i = 0; i < length; i++)
            {
                bytes[i] = _gb.bus.Read8((ushort) (address + i));
            }

            code = bytes[0];
            
            bytesString = "";

            string addressStr =  Tools.HexString(address, 2); 
            string codeStr = Tools.HexString(code, 2);
            
            if(length == 2)
                bytesString += "  " + Tools.HexString(bytes[1], 2);
            else if(length == 3)
                bytesString += "  " + Tools.HexString(bytes[1] | bytes[2] << 8  , 4);
        }
    }

}