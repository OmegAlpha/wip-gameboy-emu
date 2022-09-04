using Drongo.GameboyEmulator.Utils;

namespace Drongo.GameboyEmulator
{
    
    public class CPU_Registers
    {
        public byte a;
        public byte b;
        public byte c;
        public byte d;
        public byte e;
        public byte h;
        public byte l;
        public ushort pc; // program counter
        public ushort sp; // stack pointer
        public ushort af
        {
            get
            {
                ushort val = (ushort)((a << 8) | f);

                // last 4 bits of flags are always 0, this is important
                // I found that even if they're not used in theory, if they are not set to 0
                // some code will fail to execute properly
                return (ushort) (val & 0xFFF0);
            }
            set 
            { 
                a = (byte)(value >> 8 & 0xFF);
                f = (byte)(value & 0xF0);
            }
        }

        private byte f
        {
            get
            {
                byte fVal = 0;
                fVal  = (byte)((flagZero ? fVal | (1 << 7) : fVal & ~(1 << 7)));
                fVal = (byte)((flagSubtraction ? fVal | (1 << 6) : fVal & ~(1 << 6)));
                fVal = (byte)((flagHalfCarry ? fVal | (1 << 5) : fVal & ~(1 << 5)));
                fVal = (byte)((flagCarry ? fVal | (1 << 4) : fVal & ~(1 << 4)));

                return fVal;
            }

            set
            {
                flagZero = (value & (1 << 7)) != 0;
                flagSubtraction = (value & (1 << 6)) != 0;
                flagHalfCarry = (value & (1 << 5)) != 0;
                flagCarry = (value & (1 << 4)) != 0;
            }
        } 
        

        public ushort bc
        {
            get => (ushort) ( b << 8 | c);
            set { b = (byte)(value >> 8 & 0xFF); c = (byte)(value & 0xFF); }
        }
        
        public ushort de
        {
            get => (ushort) ( d << 8 | e);
            set { d = (byte)(value >> 8 & 0xFF); e = (byte)(value & 0xFF); }
        }
        
        public ushort hl 
        {
            get => (ushort) ( h << 8 | l);
            set { h = (byte)(value >> 8 & 0xFF); l = (byte)(value & 0xFF); }
        }

        public bool flagZero;
        // {
        //     get => (f & (1 << 7)) != 0;
        //     set => f = (byte) ( (value ? f | (1 << 7) : f & ~(1 << 7))  & 0xF0);    
        // }

        public bool flagSubtraction;
        // {
        //     get => (f & (1 << 6)) != 0;
        //     set => f = (byte)((value ? f | (1 << 6) : f & ~(1 << 6))  & 0xF0);    
        // }

        public bool flagHalfCarry;
        // {
        //     get => (f & (1 << 5)) != 0;
        //     set => f = (byte)((value ? f | (1 << 5) : f & ~(1 << 5))  & 0xF0);    
        // }

        public bool flagCarry;
        // {
        //     get => (f & (1 << 4)) != 0;
        //     set => f = (byte)((value ? f | (1 << 4) : f & ~(1 << 4))  & 0xF0);    
        // }
        
        public CPU_Registers()
        {
            pc = 0x100;
            sp = 0xFFFE;
            af = 0x0000;
            bc = 0x0000;
            de = 0x0000;
            hl = 0x0000;

            flagZero = false;
            flagSubtraction = false;
            flagHalfCarry = false;
            flagCarry = false;
        }
        
    }
}