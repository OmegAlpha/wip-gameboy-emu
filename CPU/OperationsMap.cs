using System.Collections.Generic;

namespace Drongo.GameboyEmulator
{
    public class OpDefinition
    {
        public byte code;
        public string fullInstructionCode;
        public int length;
    }

    public class OperationsMap
    {
        // not Dictionary since it's pretty slow
        public static readonly OpDefinition[] map = new OpDefinition[0x100];

        public static void Initialize()
        {
            CreateBaseOperations();
        }

        private static void AddOp(byte opCode, string instructionText, int length)
        {
            OpDefinition opDefinition = new OpDefinition
            {
                code = opCode,
                fullInstructionCode = instructionText,
                length = length
            };

            map[opCode] = opDefinition;
        }

        private static void CreateBaseOperations()
        {
            AddOp(0x00, "NOP", 1);
            AddOp(0x01, "LD BC,d16", 3);
            AddOp(0x02, "LD (BC),A", 1);
            AddOp(0x03, "INC BC", 1);
            AddOp(0x04, "INC B", 1);
            AddOp(0x05, "DEC B", 1);
            AddOp(0x06, "LD B,d8", 2);
            AddOp(0x07, "RLCA", 1);
            AddOp(0x08, "LD (a16),SP", 3);
            AddOp(0x09, "ADD HL,BC", 1);
            AddOp(0x0A, "LD A,(BC)", 1);
            AddOp(0x0B, "DEC BC", 1);
            AddOp(0x0C, "INC C", 1);
            AddOp(0x0D, "DEC C", 1);
            AddOp(0x0E, "LD C,d8", 2);
            AddOp(0x0F, "RRCA", 1);
            AddOp(0x10, "STOP 0", 2);
            AddOp(0x11, "LD DE,d16", 3);
            AddOp(0x12, "LD (DE),A", 1);
            AddOp(0x13, "INC DE", 1);
            AddOp(0x14, "INC D", 1);
            AddOp(0x15, "DEC D", 1);
            AddOp(0x16, "LD D,d8", 2);
            AddOp(0x17, "RLA", 1);
            AddOp(0x18, "JR r8", 2);
            AddOp(0x19, "ADD HL,DE", 1);
            AddOp(0x1A, "LD A,(DE)", 1);
            AddOp(0x1B, "DEC DE", 1);
            AddOp(0x1C, "INC E", 1);
            AddOp(0x1D, "DEC E", 1);
            AddOp(0x1E, "LD E,d8", 2);
            AddOp(0x1F, "RRA", 1);
            AddOp(0x20, "JR NZ,r8", 2);
            AddOp(0x21, "LD HL,d16", 3);
            AddOp(0x22, "LD (HL+),A", 1);
            AddOp(0x23, "INC HL", 1);
            AddOp(0x24, "INC H", 1);
            AddOp(0x25, "DEC H", 1);
            AddOp(0x26, "LD H,d8", 2);
            AddOp(0x27, "DAA", 1);
            AddOp(0x28, "JR Z,r8", 2);
            AddOp(0x29, "ADD HL,HL", 1);
            AddOp(0x2A, "LD A,(HL+)", 1);
            AddOp(0x2B, "DEC HL", 1);
            AddOp(0x2C, "INC L", 1);
            AddOp(0x2D, "DEC L", 1);
            AddOp(0x2E, "LD L,d8", 2);
            AddOp(0x2F, "CPL", 1);
            AddOp(0x30, "JR NC,r8", 2);
            AddOp(0x31, "LD SP,d16", 3);
            AddOp(0x32, "LD (HL-),A", 1);
            AddOp(0x33, "INC SP", 1);
            AddOp(0x34, "INC (HL)", 1);
            AddOp(0x35, "DEC (HL)", 1);
            AddOp(0x36, "LD (HL),d8", 2);
            AddOp(0x37, "SCF", 1);
            AddOp(0x38, "JR C,r8", 2);
            AddOp(0x39, "ADD HL,SP", 1);
            AddOp(0x3A, "LD A,(HL-)", 1);
            AddOp(0x3B, "DEC SP", 1);
            AddOp(0x3C, "INC A", 1);
            AddOp(0x3D, "DEC A", 1);
            AddOp(0x3E, "LD A,d8", 2);
            AddOp(0x3F, "CCF", 1);
            AddOp(0x40, "LD B,B", 1);
            AddOp(0x41, "LD B,C", 1);
            AddOp(0x42, "LD B,D", 1);
            AddOp(0x43, "LD B,E", 1);
            AddOp(0x44, "LD B,H", 1);
            AddOp(0x45, "LD B,L", 1);
            AddOp(0x46, "LD B,(HL)", 1);
            AddOp(0x47, "LD B,A", 1);
            AddOp(0x48, "LD C,B", 1);
            AddOp(0x49, "LD C,C", 1);
            AddOp(0x4A, "LD C,D", 1);
            AddOp(0x4B, "LD C,E", 1);
            AddOp(0x4C, "LD C,H", 1);
            AddOp(0x4D, "LD C,L", 1);
            AddOp(0x4E, "LD C,(HL)", 1);
            AddOp(0x4F, "LD C,A", 1);
            AddOp(0x50, "LD D,B", 1);
            AddOp(0x51, "LD D,C", 1);
            AddOp(0x52, "LD D,D", 1);
            AddOp(0x53, "LD D,E", 1);
            AddOp(0x54, "LD D,H", 1);
            AddOp(0x55, "LD D,L", 1);
            AddOp(0x56, "LD D,(HL)", 1);
            AddOp(0x57, "LD D,A", 1);
            AddOp(0x58, "LD E,B", 1);
            AddOp(0x59, "LD E,C", 1);
            AddOp(0x5A, "LD E,D", 1);
            AddOp(0x5B, "LD E,E", 1);
            AddOp(0x5C, "LD E,H", 1);
            AddOp(0x5D, "LD E,L", 1);
            AddOp(0x5E, "LD E,(HL)", 1);
            AddOp(0x5F, "LD E,A", 1);
            AddOp(0x60, "LD H,B", 1);
            AddOp(0x61, "LD H,C", 1);
            AddOp(0x62, "LD H,D", 1);
            AddOp(0x63, "LD H,E", 1);
            AddOp(0x64, "LD H,H", 1);
            AddOp(0x65, "LD H,L", 1);
            AddOp(0x66, "LD H,(HL)", 1);
            AddOp(0x67, "LD H,A", 1);
            AddOp(0x68, "LD L,B", 1);
            AddOp(0x69, "LD L,C", 1);
            AddOp(0x6A, "LD L,D", 1);
            AddOp(0x6B, "LD L,E", 1);
            AddOp(0x6C, "LD L,H", 1);
            AddOp(0x6D, "LD L,L", 1);
            AddOp(0x6E, "LD L,(HL)", 1);
            AddOp(0x6F, "LD L,A", 1);
            AddOp(0x70, "LD (HL),B", 1);
            AddOp(0x71, "LD (HL),C", 1);
            AddOp(0x72, "LD (HL),D", 1);
            AddOp(0x73, "LD (HL),E", 1);
            AddOp(0x74, "LD (HL),H", 1);
            AddOp(0x75, "LD (HL),L", 1);
            AddOp(0x76, "HALT", 1);
            AddOp(0x77, "LD (HL),A", 1);
            AddOp(0x78, "LD A,B", 1);
            AddOp(0x79, "LD A,C", 1);
            AddOp(0x7A, "LD A,D", 1);
            AddOp(0x7B, "LD A,E", 1);
            AddOp(0x7C, "LD A,H", 1);
            AddOp(0x7D, "LD A,L", 1);
            AddOp(0x7E, "LD A,(HL)", 1);
            AddOp(0x7F, "LD A,A", 1);
            AddOp(0x80, "ADD A,B", 1);
            AddOp(0x81, "ADD A,C", 1);
            AddOp(0x82, "ADD A,D", 1);
            AddOp(0x83, "ADD A,E", 1);
            AddOp(0x84, "ADD A,H", 1);
            AddOp(0x85, "ADD A,L", 1);
            AddOp(0x86, "ADD A,(HL)", 1);
            AddOp(0x87, "ADD A,A", 1);
            AddOp(0x88, "ADC A,B", 1);
            AddOp(0x89, "ADC A,C", 1);
            AddOp(0x8A, "ADC A,D", 1);
            AddOp(0x8B, "ADC A,E", 1);
            AddOp(0x8C, "ADC A,H", 1);
            AddOp(0x8D, "ADC A,L", 1);
            AddOp(0x8E, "ADC A,(HL)", 1);
            AddOp(0x8F, "ADC A,A", 1);
            AddOp(0x90, "SUB B", 1);
            AddOp(0x91, "SUB C", 1);
            AddOp(0x92, "SUB D", 1);
            AddOp(0x93, "SUB E", 1);
            AddOp(0x94, "SUB H", 1);
            AddOp(0x95, "SUB L", 1);
            AddOp(0x96, "SUB (HL)", 1);
            AddOp(0x97, "SUB A", 1);
            AddOp(0x98, "SBC A,B", 1);
            AddOp(0x99, "SBC A,C", 1);
            AddOp(0x9A, "SBC A,D", 1);
            AddOp(0x9B, "SBC A,E", 1);
            AddOp(0x9C, "SBC A,H", 1);
            AddOp(0x9D, "SBC A,L", 1);
            AddOp(0x9E, "SBC A,(HL)", 1);
            AddOp(0x9F, "SBC A,A", 1);
            AddOp(0xA0, "AND B", 1);
            AddOp(0xA1, "AND C", 1);
            AddOp(0xA2, "AND D", 1);
            AddOp(0xA3, "AND E", 1);
            AddOp(0xA4, "AND H", 1);
            AddOp(0xA5, "AND L", 1);
            AddOp(0xA6, "AND (HL)", 1);
            AddOp(0xA7, "AND A", 1);
            AddOp(0xA8, "XOR B", 1);
            AddOp(0xA9, "XOR C", 1);
            AddOp(0xAA, "XOR D", 1);
            AddOp(0xAB, "XOR E", 1);
            AddOp(0xAC, "XOR H", 1);
            AddOp(0xAD, "XOR L", 1);
            AddOp(0xAE, "XOR (HL)", 1);
            AddOp(0xAF, "XOR A", 1);
            AddOp(0xB0, "OR B", 1);
            AddOp(0xB1, "OR C", 1);
            AddOp(0xB2, "OR D", 1);
            AddOp(0xB3, "OR E", 1);
            AddOp(0xB4, "OR H", 1);
            AddOp(0xB5, "OR L", 1);
            AddOp(0xB6, "OR (HL)", 1);
            AddOp(0xB7, "OR A", 1);
            AddOp(0xB8, "CP B", 1);
            AddOp(0xB9, "CP C", 1);
            AddOp(0xBA, "CP D", 1);
            AddOp(0xBB, "CP E", 1);
            AddOp(0xBC, "CP H", 1);
            AddOp(0xBD, "CP L", 1);
            AddOp(0xBE, "CP (HL)", 1);
            AddOp(0xBF, "CP A", 1);
            AddOp(0xC0, "RET NZ", 1);
            AddOp(0xC1, "POP BC", 1);
            AddOp(0xC2, "JP NZ,a16", 3);
            AddOp(0xC3, "JP a16", 3);
            AddOp(0xC4, "CALL NZ,a16", 3);
            AddOp(0xC5, "PUSH BC", 1);
            AddOp(0xC6, "ADD A,d8", 2);
            AddOp(0xC7, "RST 00H", 1);
            AddOp(0xC8, "RET Z", 1);
            AddOp(0xC9, "RET", 1);
            AddOp(0xCA, "JP Z,a16", 3);
            AddOp(0xCB, "CB", 1);
            AddOp(0xCC, "CALL Z,a16", 3);
            AddOp(0xCD, "CALL a16", 3);
            AddOp(0xCE, "ADC A,d8", 2);
            AddOp(0xCF, "RST 08H", 1);
            AddOp(0xD0, "RET NC", 1);
            AddOp(0xD1, "POP DE", 1);
            AddOp(0xD2, "JP NC,a16", 3);
            AddOp(0xD4, "CALL NC,a16", 3);
            AddOp(0xD5, "PUSH DE", 1);
            AddOp(0xD6, "SUB d8", 2);
            AddOp(0xD7, "RST 10H", 1);
            AddOp(0xD8, "RET C", 1);
            AddOp(0xD9, "RETI", 1);
            AddOp(0xDA, "JP C,a16", 3);
            AddOp(0xDC, "CALL C,a16", 3);
            AddOp(0xDE, "SBC A,d8", 2);
            AddOp(0xDF, "RST 18H", 1);
            AddOp(0xE0, "LDH (a8),A", 2);
            AddOp(0xE1, "POP HL", 1);
            AddOp(0xE2, "LD (C),A", 1);
            AddOp(0xE5, "PUSH HL", 1);
            AddOp(0xE6, "AND d8", 2);
            AddOp(0xE7, "RST 20H", 1);
            AddOp(0xE8, "ADD SP,r8", 2);
            AddOp(0xE9, "JP (HL)", 1);
            AddOp(0xEA, "LD (a16),A", 3);
            AddOp(0xEE, "XOR d8", 2);
            AddOp(0xEF, "RST 28H", 1);
            AddOp(0xF0, "LDH A,(a8)", 2);
            AddOp(0xF1, "POP AF", 1);
            AddOp(0xF2, "LD A,(C)", 1);
            AddOp(0xF3, "DI", 1);
            AddOp(0xF5, "PUSH AF", 1);
            AddOp(0xF6, "OR d8", 2);
            AddOp(0xF7, "RST 30H", 1);
            AddOp(0xF8, "LD HL,SP+r8", 2);
            AddOp(0xF9, "LD SP,HL", 1);
            AddOp(0xFA, "LD A,(a16)", 3);
            AddOp(0xFB, "EI", 1);
            AddOp(0xFE, "CP d8", 2);
            AddOp(0xFF, "RST 38H", 1);

        }
    }
}