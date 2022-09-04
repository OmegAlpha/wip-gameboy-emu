namespace Drongo.GameboyEmulator
{
    /// <summary>
    /// you can find the instructions set here:
    /// https://www.pastraiser.com/cpu/gameboy/gameboy_opcodes.html
    /// </summary>
    
    public class InstructionProcessor
    {
        private CPU _cpu;
        private AddressBus _bus;
        private CPU_Registers _registers;

        private int _valueForFlags;

        public void SetContext(CPU cpu)
        {
            _cpu = cpu;
            _bus = cpu.bus;
            _registers = _cpu.registers;
        }

        public int Execute(byte opCode)
        {
            switch (opCode)
            {
                // NOP
                case 0x00: 
                    return 4;

                // LD BC,d16
                // 3  12
                // - - - -
                case 0x01:
                    _registers.bc = _cpu.GetD16();
                    return 12;
                 
                // LD (BC),A
                // 1  8
                // - - - -
                case 0x02:
                    _bus.Write8(_registers.bc, _registers.a);
                    return 8;

                // INC BC
                // 1  8
                // - - - -
                case 0x03:
                    _registers.bc++;
                    return 8;

                // INC B
                // 1  4
                // Z 0 H -
                case 0x04:
                    _registers.b++;
                    _cpu.CheckFlags_INC(_registers.b);
                    return 4;

                // DEC B
                // 1  4
                // Z 1 H -
                case 0x05:
                    _registers.b = _cpu.DoDEC(_registers.b);
                    return 4;

                // LD B,d8
                // 2  8
                // - - - -
                case 0x06:
                    _registers.b = _cpu.GetD8();
                    return 8;

                // RLCA
                // 1  4
                // 0 0 0 C
                case 0x07:
                {
                    byte val = _registers.a;
                    byte c = (byte)((val >> 7) & 1);
                    val = (byte)((val << 1) | c);

                    _registers.a = val;

                    _registers.flagZero = false;
                    _registers.flagSubtraction = false;
                    _registers.flagHalfCarry = false;
                    _registers.flagCarry = c != 0;
                    return 4;
                }
               
                // LD a16,SP
                // 3  20
                // - - - -
                case 0x08:
                    _bus.Write16(_cpu.GetD16(), _registers.sp);
                    return 20;

                // ADD HL,BC
                // 1  8
                // - 0 H C        
                case 0x09:
                    _cpu.DoAddToHL(_registers.bc);
                    return 8;

                // LD A,(BC)
                // 1  8
                // - - - -
                case 0x0A:
                    _registers.a = _bus.Read8(_registers.bc);
                    return 8;

                // DEC BC
                // 1  8
                // - - - -
                case 0x0B:
                    _registers.bc--;
                    return 8;

                // INC C
                // 1  4
                // Z 0 H -
                case 0x0C:
                    _registers.c++;
                    _cpu.CheckFlags_INC(_registers.c);
                    return 4;

                // DEC C
                // 1  4
                // Z 1 H -
                case 0x0D:
                    _registers.c = _cpu.DoDEC(_registers.c);
                    return 4;

                // LD C,d8
                // 2  8
                // - - - -
                case 0x0E:
                    _registers.c = _cpu.GetD8();
                    return 8;

                // RRCA
                // 1  4
                // 0 0 0 C
                case 0x0F:
                {
                    byte val = _registers.a;
                    _registers.a = (byte)((val >> 1) | (val << 7));;

                    _registers.flagZero = false;
                    _registers.flagSubtraction = false;
                    _registers.flagHalfCarry = false;
                    _registers.flagCarry = (val & 0x1) != 0;
                    return 4;
                }
               
                // STOP 0
                // 2  4
                // - - - -
                case 0x10:
                    // _registers.pc++;
                    return 4;

                // LD DE,d16
                // 3  12
                // - - - -
                case 0x11:
                    _registers.de = _cpu.GetD16();
                    return 12;

                // LD (DE),A
                // 1  8
                // - - - -
                case 0x12:
                    _bus.Write8(_registers.de, _registers.a); 
                    return 8;

                // INC DE
                // 1  8
                // - - - -
                case 0x13:
                    _registers.de++;
                    return 8;

                // INC D
                // 1  4
                // Z 0 H -
                case 0x14:
                    _registers.d++;
                    _cpu.CheckFlags_INC(_registers.d);
                    return 4;

                // DEC D
                // 1  4
                // Z 1 H -
                case 0x15:
                    _registers.d = _cpu.DoDEC(_registers.d);
                    return 4;

                // LD D,d8
                // 2  8
                // - - - -
                case 0x16:
                    _registers.d = _cpu.GetD8();
                    return 8;

                // RLA
                // 1  4
                // 0 0 0 C
                case 0x17:
                {
                    byte flagCBit  = (byte) (_registers.flagCarry ? 1 : 0);

                    _registers.flagZero = false;
                    _registers.flagSubtraction = false;
                    _registers.flagHalfCarry = false;
                    _registers.flagCarry = (_registers.a  & 0x80) != 0;

                    _registers.a = (byte)((_registers.a << 1) | flagCBit);
                    
                    return 4;
                }

                // JR r8
                // 2  12
                // - - - -
                case 0x18:
                    _cpu.DoJR(true);
                    return 12;

                // ADD HL,DE
                // 1  8
                // - 0 H C
                case 0x19:
                    _cpu.DoAddToHL(_registers.de);
                    return 8;

                // LD A,(DE)
                // 1  8
                // - - - -
                case 0x1A:
                    _registers.a = _bus.Read8(_registers.de);
                    return 8;

                // DEC DE
                // 1  8
                // - - - -
                case 0x1B:
                    _registers.de--;
                    return 8;

                // INC E
                // 1  4
                // Z 0 H -
                case 0x1C:
                    _registers.e++;
                    _cpu.CheckFlags_INC(_registers.e);
                    return 4;

                // DEC E
                // 1  4
                // Z 1 H -
                case 0x1D:
                    _registers.e = _cpu.DoDEC(_registers.e);
                    return 4;

                // LD E,d8
                // 2  8
                // - - - -
                case 0x1E:
                    _registers.e = _cpu.GetD8();
                    return 8;

                // RRA
                // 1  4
                // 0 0 0 C
                case 0x1F:
                {
                    byte flagCBit  = (byte) (_registers.flagCarry ? 0x80 : 0);

                    _registers.flagZero = false;
                    _registers.flagSubtraction = false;
                    _registers.flagHalfCarry = false;
                    _registers.flagCarry = (_registers.a & 0x1) != 0;
                    
                    _registers.a = (byte)((_registers.a >> 1) | flagCBit);
                    
                    return 4;
                }
               
                // JR NZ,r8
                // 2  12/8
                // - - - -
                case 0x20:

                    if (_cpu.DoJR(!_registers.flagZero))
                        return 12;
                        
                    return 8;

                // LD HL,d16
                // 3  12
                // - - - -
                case 0x21:
                    _registers.hl = _cpu.GetD16();
                    return 12;

                // LD (HL+),A
                // 1  8
                // - - - -
                case 0x22:
                {
                    _bus.Write8(_registers.hl, _registers.a);
                    _registers.hl++;
                    return 8;
                }

                // INC HL
                // 1  8
                // - - - -
                case 0x23:
                    _registers.hl++;
                    return 8;
                 
                // INC H
                // 1  4
                // Z 0 H -
                case 0x24:
                    _registers.h++;
                    _cpu.CheckFlags_INC(_registers.h);
                    return 4;

                // DEC H
                // 1  4
                // Z 1 H -
                case 0x25:
                    _registers.h = _cpu.DoDEC(_registers.h);
                    return 4;

                // LD H,d8
                // 2  8
                // - - - -
                case 0x26:
                    _registers.h = _cpu.GetD8();
                    return 8;

                // DAA
                // 1  4
                // Z - 0 C
                case 0x27:
                    // not much idea of what this one does....
                    // I looked for other implementations and tried to mimic it
                    // but I definitely need to take a look later
                    // I think it must be related to BCD? 
                    
                    if (_registers.flagSubtraction) 
                    {
                        if (_registers.flagCarry)
                        {
                            _registers.a -= 0x60;
                        }

                        if (_registers.flagHalfCarry)
                        {
                            _registers.a -= 0x6;
                        }
                    } 
                    else 
                    {
                        if (_registers.flagCarry || (_registers.a > 0x99))
                        {
                            _registers.a += 0x60; 
                            _registers.flagCarry = true;
                        }

                        if (_registers.flagHalfCarry || (_registers.a & 0xF) > 0x9)
                        {
                            _registers.a += 0x6;
                        }
                    }

                    _registers.flagZero = _registers.a == 0;
                    _registers.flagHalfCarry = false;
                    
                    return 4;               

                // JR Z,r8
                // 2  12/8
                // - - - -
                case 0x28:
                    if (_cpu.DoJR(_registers.flagZero))
                        return 12;
                        
                    return 8;
               
                // 0x29 ADD HL,HL
                // 1  8
                // - 0 H C
                case 0x29:
                    _cpu.DoAddToHL(_registers.hl);
                    return 8;

                // LD A,(HL+)
                // 1  8
                // - - - -
                case 0x2A:
                {
                    _registers.a = _bus.Read8(_registers.hl++);
                    return 8;
                }
               
                // DEC HL
                // 1  8
                // - - - -
                case 0x2B:
                    _registers.hl--;
                    return 8;

                // INC L
                // 1  4
                // Z 0 H -
                case 0x2C:
                    _registers.l++;
                    _cpu.CheckFlags_INC(_registers.l);
                    return 4;
               
                // DEC L
                // 1  4
                // Z 1 H -
                case 0x2D:
                    _registers.l = _cpu.DoDEC(_registers.l);
                    return 4;

                // LD L,d8
                // 2  8
                // - - - -
                case 0x2E:
                    _registers.l = _cpu.GetD8();
                    return 8;
               
                // CPL
                // 1  4
                // - 1 1 -
                case 0x2F:
                    _registers.a = (byte) ~ _registers.a;

                    _registers.flagSubtraction = true;
                    _registers.flagHalfCarry = true;
                    return 4;

                // JR NC,r8
                // 2  12/8
                // - - - -
                case 0x30:
                    if (_cpu.DoJR(!_registers.flagCarry))
                        return 12;
                        
                    return 8;
               
                // LD SP,d16
                // 3  12
                // - - - -
                case 0x31:
                    _registers.sp = _cpu.GetD16();
                    return 12;

                // LD (HL-),A
                // 1  8
                // - - - -
                case 0x32:
                {
                    _bus.Write8(_registers.hl--, _registers.a);
                    return 8;
                }

                // INC SP
                // 1  8
                // - - - -
                case 0x33:
                    _registers.sp++;
                    return 8;
                 
                // INC (HL)
                // 1  12
                // Z 0 H -
                case 0x34:
                {
                    byte val = (byte)(_bus.Read8(_registers.hl) + 1);
                    _bus.Write8(_registers.hl, val);
                    _cpu.CheckFlags_INC(val);
                    return 12;
                }
               
                // DEC (HL)
                // 1  12
                // Z 1 H -
                case 0x35:
                {
                    _bus.Write8(_registers.hl, _cpu.DoDEC(_bus.Read8(_registers.hl)));

                    return 12;
                }

                // LD (HL),d8
                // 2  12
                // - - - -
                case 0x36:
                    _bus.Write8(_registers.hl, _cpu.GetD8());
                    return 12;

                // SCF
                // 1  4
                // - 0 0 1
                case 0x37:
                    _registers.flagSubtraction = false;
                    _registers.flagHalfCarry = false;
                    _registers.flagCarry = true;
                    return 4;
               
                // JR C,r8
                // 2  12/8
                // - - - -
                case 0x38:
                    if (_cpu.DoJR(_registers.flagCarry))
                        return 12;
                        
                    return 8;

                // ADD HL,SP
                // 1  8
                // - 0 H C
                case 0x39:
                    _cpu.DoAddToHL(_registers.sp);
                    return 8;

                // LD A,(HL-)
                // 1  8
                // - - - -
                case 0x3A:
                {
                    ushort address = _registers.hl--;
                    _registers.a = _bus.Read8(address);
                    return 8;
                }
               
                // DEC SP
                // 1  8
                // - - - -
                case 0x3B:
                    _registers.sp--;
                    return 8;
               
                // INC A
                // 1  4
                // Z 0 H -
                case 0x3C:
                    _registers.a++;
                    _cpu.CheckFlags_INC(_registers.a);
                    return 4;

                // DEC A
                // 1  4
                // Z 1 H -
                case 0x3D:
                    _registers.a = _cpu.DoDEC(_registers.a);
                    return 4;

                // LD A,d8
                // 2  8
                // - - - -
                case 0x3E:
                    _registers.a = _cpu.GetD8();
                    return 8;

                // CCF
                // 1  4
                // - 0 0 C
                case 0x3F:
                    _registers.flagSubtraction = false;
                    _registers.flagHalfCarry = false;
                    _registers.flagCarry = !_registers.flagCarry;
                    return 4;
               
                // LD B,B
                // 1  4
                // - - - -
                case 0x40:
                    // this is basically the same than doing nothing,
                    // but yeah, this is how it is
                    _registers.b = _registers.b;
                    return 4;

                // LD B,C
                // 1  4
                // - - - -
                case 0x41:
                    _registers.b = _registers.c; 
                    return 4;

                // LD B,D
                // 1  4
                // - - - -
                case 0x42:
                    _registers.b = _registers.d; 
                    return 4;

                // LD B,E
                // 1  4
                // - - - -
                case 0x43:
                    _registers.b = _registers.e; 
                    return 4;

                // LD B,H
                // 1  4
                // - - - -
                case 0x44:
                    _registers.b = _registers.h; 
                    return 4;

                // LD B,L
                // 1  4
                // - - - -
                case 0x45:
                    _registers.b = _registers.l; 
                    return 4;

                // LD B,(HL)
                // 1  8
                // - - - -
                case 0x46:
                    _registers.b = _bus.Read8(_registers.hl);
                    return 8;

                // LD B,A
                // 1  4
                // - - - -
                case 0x47:
                    _registers.b = _registers.a; 
                    return 4;

                // LD C,B
                // 1  4
                // - - - -
                case 0x48:
                    _registers.c = _registers.b; 
                    return 4;

                // LD C,C
                // 1  4
                // - - - -
                case 0x49:
                    // this is basically the same than doing nothing,
                    // but yeah, this is how it is
                    _registers.c = _registers.c; 
                    return 4;

                // LD C,D
                // 1  4
                // - - - -
                case 0x4A:
                    _registers.c = _registers.d; 
                    return 4;

                // LD C,E
                // 1  4
                // - - - -
                case 0x4B:
                    _registers.c = _registers.e; 
                    return 4;
               
                // LD C,H
                // 1  4
                // - - - -
                case 0x4C:
                    _registers.c = _registers.h; 
                    return 4;

                // LD C,L
                // 1  4
                // - - - -
                case 0x4D:
                    _registers.c = _registers.l; 
                    return 4;

                // LD C,(HL)
                // 1  8
                // - - - -
                case 0x4E:
                    _registers.c = _bus.Read8(_registers.hl);
                    return 8;

                // LD C,A
                // 1  4
                // - - - -
                case 0x4F:
                    _registers.c = _registers.a; 
                    return 4;

                // LD D,B
                // 1  4
                // - - - -
                case 0x50:
                    _registers.d = _registers.b; 
                    return 4;

                // LD D,C
                // 1  4
                // - - - -
                case 0x51:
                    _registers.d = _registers.c; 
                    return 4;

                // LD D,D
                // 1  4
                // - - - -
                case 0x52:
                    // this is basically the same than doing nothing,
                    // but yeah, this is how it is
                    _registers.d = _registers.d; 
                    return 4;

                // LD D,E
                // 1  4
                // - - - -
                case 0x53:
                    _registers.d = _registers.e; 
                    return 4;

                // LD D,H
                // 1  4
                // - - - -
                case 0x54:
                    _registers.d = _registers.h; 
                    return 4;

                // LD D,L
                // 1  4
                // - - - -
                case 0x55:
                    _registers.d = _registers.l;  
                    return 4;

                // LD D,(HL)
                // 1  8
                // - - - -
                case 0x56:
                    _registers.d = _bus.Read8(_registers.hl);
                    return 8;

                // LD D,A
                // 1  4
                // - - - -
                case 0x57:
                    _registers.d = _registers.a;
                    return 4;

                // LD E,B
                // 1  4
                // - - - -
                case 0x58:
                    _registers.e = _registers.b; 
                    return 4;

                // LD E,C
                // 1  4
                // - - - -
                case 0x59:
                    _registers.e = _registers.c; 
                    return 4;

                // LD E,D
                // 1  4
                // - - - -
                case 0x5A:
                    _registers.e = _registers.d; 
                    return 4;

                // LD E,E
                // 1  4
                // - - - -
                case 0x5B:
                    // this is basically the same than doing nothing,
                    // but yeah, this is how it is
                    _registers.e = _registers.e; 
                    return 4;

                // LD E,H
                // 1  4
                // - - - -
                case 0x5C:
                    _registers.e = _registers.h; 
                    return 4;

                // LD E,L
                // 1  4
                // - - - -
                case 0x5D:
                    _registers.e = _registers.l; 
                    return 4;

                // LD E,(HL)
                // 1  8
                // - - - -
                case 0x5E:
                    _registers.e = _bus.Read8(_registers.hl);
                    return 8;

                // LD E,A
                // 1  4
                // - - - -
                case 0x5F:
                    _registers.e = _registers.a;  
                    return 4;

                // LD H,B
                // 1  4
                // - - - -
                case 0x60:
                    _registers.h = _registers.b;
                    return 4;

                // LD H,C
                // 1  4
                // - - - -
                case 0x61:
                    _registers.h = _registers.c; 
                    return 4;

                // LD H,D
                // 1  4
                // - - - -
                case 0x62:
                    _registers.h = _registers.d; 
                    return 4;

                // LD H,E
                // 1  4
                // - - - -
                case 0x63:
                    _registers.h = _registers.e; 
                    return 4;

                // LD H,H
                // 1  4
                // - - - -
                case 0x64:
                    // this is basically the same than doing nothing,
                    // but yeah, this is how it is
                    _registers.h = _registers.h; 
                    return 4;

                // LD H,L
                // 1  4
                // - - - -
                case 0x65:
                    _registers.h = _registers.l; 
                    return 4;

                // LD H,(HL)
                // 1  8
                // - - - -
                case 0x66:
                    _registers.h = _bus.Read8(_registers.hl);
                    return 8;

                // LD H,A
                // 1  4
                // - - - -
                case 0x67:
                    _registers.h = _registers.a;  
                    return 4;

                // LD L,B
                // 1  4
                // - - - -
                case 0x68:
                    _registers.l = _registers.b; 
                    return 4;

                // LD L,C
                // 1  4
                // - - - -
                case 0x69:
                    _registers.l = _registers.c; 
                    return 4;

                // LD L,D
                // 1  4
                // - - - -
                case 0x6A:
                    _registers.l = _registers.d; 
                    return 4;

                // LD L,E
                // 1  4
                // - - - -
                case 0x6B:
                    _registers.l = _registers.e;  
                    return 4;

                // LD L,H
                // 1  4
                // - - - -
                case 0x6C:
                    _registers.l = _registers.h; 
                    return 4;

                // LD L,L
                // 1  4
                // - - - -
                case 0x6D:
                    // this is basically the same than doing nothing,
                    // but yeah, this is how it is
                    _registers.l = _registers.l; 
                    return 4;

                // LD L,(HL)
                // 1  8
                // - - - -
                case 0x6E:
                    _registers.l = _bus.Read8(_registers.hl);
                    return 8;

                // LD L,A
                // 1  4
                // - - - -
                case 0x6F:
                    _registers.l = _registers.a; 
                    return 4;

                // LD (HL),B
                // 1  8
                // - - - -
                case 0x70:
                    _bus.Write8(_registers.hl, _registers.b); 
                    return 8;

                // LD (HL),C
                // 1  8
                // - - - -
                case 0x71:
                    _bus.Write8(_registers.hl, _registers.c); 
                    return 8;

                // LD (HL),D
                // 1  8
                // - - - -
                case 0x72:
                    _bus.Write8(_registers.hl, _registers.d); 
                    return 8;

                // LD (HL),E
                // 1  8
                // - - - -
                case 0x73:
                    _bus.Write8(_registers.hl, _registers.e);  
                    return 8;

                // LD (HL),H
                // 1  8
                // - - - -
                case 0x74:
                    _bus.Write8(_registers.hl, _registers.h);  
                    return 8;

                // LD (HL),L
                // 1  8
                // - - - -
                case 0x75:
                    _bus.Write8(_registers.hl, _registers.l);  
                    return 8;
                // HALT
                // 1  4
                // - - - -
                case 0x76:

                    _cpu.DoHalt();
                    return 4;

                // LD (HL),A
                // 1  8
                // - - - -
                case 0x77:
                    _bus.Write8(_registers.hl, _registers.a); 
                    return 8;

                // LD A,B
                // 1  4
                // - - - -
                case 0x78:
                    _registers.a = _registers.b;
                    return 4;

                // LD A,C
                // 1  4
                // - - - -
                case 0x79:
                    _registers.a = _registers.c; 
                    return 4;

                // LD A,D
                // 1  4
                // - - - -
                case 0x7A:
                    _registers.a = _registers.d; 
                    return 4;

                // LD A,E
                // 1  4
                // - - - -
                case 0x7B:
                    _registers.a = _registers.e; 
                    return 4;

                // LD A,H
                // 1  4
                // - - - -
                case 0x7C:
                    _registers.a = _registers.h;  
                    return 4;

                // LD A,L
                // 1  4
                // - - - -
                case 0x7D:
                    _registers.a = _registers.l; 
                    return 4;

                // LD A,(HL)
                // 1  8
                // - - - -
                case 0x7E:
                    _registers.a = _bus.Read8(_registers.hl);
                    return 8;

                // LD A,A
                // 1  4
                // - - - -
                case 0x7F:
                    // this is basically the same than doing nothing,
                    // but yeah, this is how it is
                    _registers.a = _registers.a;  
                    return 4;
               
                //0x80 ADD A,B
                // 1  4
                // Z 0 H C
                case 0x80:
                    _cpu.DoAdd(_registers.b);
                    return 4;
                     
                // 0x81 ADD A,C
                // 1  4
                // Z 0 H C
                case 0x81:
                    _cpu.DoAdd(_registers.c); 
                    return 4;
                 
                // 0x82 ADD A,D
                // 1  4
                // Z 0 H C
                case 0x82:
                    _cpu.DoAdd(_registers.d); 
                    return 4;
                 
                // 0x83 ADD A,E
                // 1  4
                // Z 0 H C
                case 0x83:
                    _cpu.DoAdd(_registers.e); 
                    return 4;
                 
                // 0x84 ADD A,H
                // 1  4
                // Z 0 H C
                case 0x84:
                    _cpu.DoAdd(_registers.h); 
                    return 4;
                 
                // 0x85 ADD A,L
                // 1  4
                // Z 0 H C
                case 0x85:
                    _cpu.DoAdd(_registers.l); 
                    return 4;
                 
                // 0x86 ADD A,(HL)
                // 1  8
                // Z 0 H C
                case 0x86:
                    _cpu.DoAdd(_bus.Read8(_registers.hl));
                    return 8;
                 
                // 0x87 ADD A,A
                // 1  4
                // Z 0 H C
                case 0x87:
                    _cpu.DoAdd(_registers.a);
                    return 4;
               
                // ADC A,B
                // 1  4
                // Z 0 H C
                case 0x88:
                    _cpu.DoADC(_registers.b);
                    return 4;
                 
                // ADC A,C
                // 1  4
                // Z 0 H C
                case 0x89:
                    _cpu.DoADC(_registers.c);
                    return 4;
                 
                // ADC A,D
                // 1  4
                // Z 0 H C
                case 0x8A:
                    _cpu.DoADC(_registers.d);
                    return 4;
                 
                // ADC A,E
                // 1  4
                // Z 0 H C
                case 0x8B:
                    _cpu.DoADC(_registers.e);
                    return 4;
                 
                // ADC A,H
                // 1  4
                // Z 0 H C
                case 0x8C:
                    _cpu.DoADC(_registers.h);
                    return 4;
                 
                // ADC A,L
                // 1  4
                // Z 0 H C
                case 0x8D:
                    _cpu.DoADC(_registers.l);
                    return 4;
                 
                // ADC A,(HL)
                // 1  8
                // Z 0 H C
                case 0x8E:
                    _cpu.DoADC(_bus.Read8(_registers.hl));
                    return 8;
                 
                // ADC A,A
                // 1  4
                // Z 0 H C
                case 0x8F:
                    _cpu.DoADC(_registers.a);
                    return 4;
               
                // SUB B
                // 1  4
                // Z 1 H C
                case 0x90:
                    _cpu.DoSub(_registers.b);
                    return 4;

                // SUB C
                // 1  4
                // Z 1 H C
                case 0x91:
                    _cpu.DoSub(_registers.c);
                    return 4;
                 
                // SUB D
                // 1  4
                // Z 1 H C
                case 0x92:
                    _cpu.DoSub(_registers.d);
                    return 4;

                // SUB E
                // 1  4
                // Z 1 H C
                case 0x93:
                    _cpu.DoSub(_registers.e);
                    return 4;
                 
                // SUB H
                // 1  4
                // Z 1 H C
                case 0x94:
                    _cpu.DoSub(_registers.h);
                    return 4;
               
                // SUB L
                // 1  4
                // Z 1 H C
                case 0x95:
                    _cpu.DoSub(_registers.l);
                    return 4;      
               
                // SUB (HL)
                // 1  8
                // Z 1 H C
                case 0x96:
                    _cpu.DoSub(_bus.Read8(_registers.hl));
                    return 8;
                 
                // SUB A
                // 1  4
                // Z 1 H C
                case 0x97:
                    _cpu.DoSub(_registers.a);
                    return 4;
               
                // SBC A,B
                // 1  4
                // Z 1 H C
                case 0x98:
                    _cpu.DoSBC(_registers.b);
                    return 4;
                 
                // SBC A,C
                // 1  4
                // Z 1 H C
                case 0x99:
                    _cpu.DoSBC(_registers.c);
                    return 4;
                 
                // SBC A,D
                // 1  4
                // Z 1 H C
                case 0x9A:
                    _cpu.DoSBC(_registers.d);
                    return 4;
                 
                // SBC A,E
                // 1  4
                // Z 1 H C
                case 0x9B:
                    _cpu.DoSBC(_registers.e);
                    return 4;
                 
                // SBC A,H
                // 1  4
                // Z 1 H C
                case 0x9C:
                    _cpu.DoSBC(_registers.h);
                    return 4;
                 
                // SBC A,L
                // 1  4
                // Z 1 H C
                case 0x9D:
                    _cpu.DoSBC(_registers.l);
                    return 4;
                 
                // SBC A,(HL)
                // 1  8
                // Z 1 H C
                case 0x9E:
                    _cpu.DoSBC(_bus.Read8(_registers.hl));
                    return 8;
                 
                // SBC A,A
                // 1  4
                // Z 1 H C
                case 0x9F:
                    _cpu.DoSBC(_registers.a);
                    return 4;
               
                // AND B
                // 1  4
                // Z 0 1 0
                case 0xA0:
                    _cpu.DoAnd(_registers.b);
                    return 4;
                 
                // AND C
                // 1  4
                // Z 0 1 0
                case 0xA1:
                    _cpu.DoAnd(_registers.c);
                    return 4;
                 
                // AND D
                // 1  4
                // Z 0 1 0
                case 0xA2:
                    _cpu.DoAnd(_registers.d);
                    return 4;
                 
                // AND E
                // 1  4
                // Z 0 1 0
                case 0xA3:
                    _cpu.DoAnd(_registers.e);
                    return 4;
                 
                // AND H
                // 1  4
                // Z 0 1 0
                case 0xA4:
                    _cpu.DoAnd(_registers.h);
                    return 4;
                 
                // AND L
                // 1  4
                // Z 0 1 0
                case 0xA5:
                    _cpu.DoAnd(_registers.l);
                    return 4;
                 
                // AND (HL)
                // 1  8
                // Z 0 1 0
                case 0xA6:
                    _cpu.DoAnd(_bus.Read8(_registers.hl));
                    return 8;
                 
                // AND A
                // 1  4
                // Z 0 1 0
                case 0xA7:
                    _cpu.DoAnd(_registers.a);
                    return 4;
               
                // XOR B
                // 1  4
                // Z 0 0 0
                case 0xA8:
                    _cpu.DoXOR(_registers.b);
                    return 4;
                 
                // XOR C
                // 1  4
                // Z 0 0 0
                case 0xA9:
                    _cpu.DoXOR(_registers.c);
                    return 4;
                 
                // XOR D
                // 1  4
                // Z 0 0 0
                case 0xAA:
                    _cpu.DoXOR(_registers.d);
                    return 4;
                 
                // XOR E
                // 1  4
                // Z 0 0 0
                case 0xAB:
                    _cpu.DoXOR(_registers.e);
                    return 4;
                 
                // XOR H
                // 1  4
                // Z 0 0 0
                case 0xAC:
                    _cpu.DoXOR(_registers.h);
                    return 4;
                 
                // XOR L
                // 1  4
                // Z 0 0 0
                case 0xAD:
                    _cpu.DoXOR(_registers.l);
                    return 4;
                 
                // XOR (HL)
                // 1  8
                // Z 0 0 0
                case 0xAE:
                    _cpu.DoXOR(_bus.Read8(_registers.hl));
                    return 8;
                 
                // XOR A
                // 1  4
                // Z 0 0 0
                case 0xAF:
                    _cpu.DoXOR(_registers.a);
                    return 4;
               
                // OR B
                // 1  4
                // Z 0 0 0
                case 0xB0:
                    _cpu.DoOR(_registers.b);
                    return 4;
                 
                // OR C
                // 1  4
                // Z 0 0 0
                case 0xB1:
                    _cpu.DoOR(_registers.c);
                    return 4;
                 
                // OR D
                // 1  4
                // Z 0 0 0
                case 0xB2:
                    _cpu.DoOR(_registers.d);
                    return 4;
                 
                // OR E
                // 1  4
                // Z 0 0 0
                case 0xB3:
                    _cpu.DoOR(_registers.e);
                    return 4;
                 
                // OR H
                // 1  4
                // Z 0 0 0
                case 0xB4:
                    _cpu.DoOR(_registers.h);
                    return 4;
                 
                // OR L
                // 1  4
                // Z 0 0 0
                case 0xB5:
                    _cpu.DoOR(_registers.l);
                    return 4;
                 
                // OR (HL)
                // 1  8
                // Z 0 0 0
                case 0xB6:
                    _cpu.DoOR(_bus.Read8(_registers.hl));
                    return 8;
                 
                // OR A
                // 1  4
                // Z 0 0 0
                case 0xB7:
                    _cpu.DoOR(_registers.a);
                    return 4;
               
                // CP B
                // 1  4
                // Z 1 H C
                case 0xB8:
                    _cpu.DoCP(_registers.b);
                    return 4;
                 
                // CP C
                // 1  4
                // Z 1 H C
                case 0xB9:
                    _cpu.DoCP(_registers.c);
                    return 4;
                 
                // CP D
                // 1  4
                // Z 1 H C
                case 0xBA:
                    _cpu.DoCP(_registers.d);
                    return 4;
                 
                // CP E
                // 1  4
                // Z 1 H C
                case 0xBB:
                    _cpu.DoCP(_registers.e);
                    return 4;
                 
                // CP H
                // 1  4
                // Z 1 H C
                case 0xBC:
                    _cpu.DoCP(_registers.h);
                    return 4;
                 
                // CP L
                // 1  4
                // Z 1 H C
                case 0xBD:
                    _cpu.DoCP(_registers.l);
                    return 4;
                 
                // CP (HL)
                // 1  8
                // Z 1 H C
                case 0xBE:
                    _cpu.DoCP(_bus.Read8(_registers.hl));
                    return 8;
                 
                // CP A
                // 1  4
                // Z 1 H C
                case 0xBF:
                    _cpu.DoCP(_registers.a);
                    return 4;
               
                // RET NZ
                // 1  20/8
                // - - - -
                case 0xC0:
                    if (_cpu.DoReturn(!_registers.flagZero))
                        return 20;
                        
                    return 8;
               
                // POP BC
                // 1  12
                // - - - -
                case 0xC1:
                    _registers.bc = _cpu.PopStack16();
                    return 12;
               
                // JP NZ,a16
                // 3  16/12
                // - - - -
                case 0xC2:
                {
                    if (_cpu.DoJumpToA16(!_registers.flagZero))
                        return 16;

                    return 12;
                }
                // JP a16
                // 3  16
                // - - - -
                case 0xC3:
                {
                    _cpu.DoJumpToA16(true);
                    return 16;
                }
                // CALL NZ,a16
                // 3  24/12
                // - - - -
                case 0xC4:
                    if (_cpu.DoCall(!_registers.flagZero))
                        return 24;

                    return 12;

                // PUSH BC
                // 1  16
                // - - - -
                case 0xC5:
                    _cpu.PushStack16(_registers.bc);
                    return 16;

                // 0xC6 ADD A,d8
                // 2  8
                // Z 0 H C
                case 0xC6:
                    _cpu.DoAdd(_cpu.GetD8());
                    return 8;

                // RST 00H
                // 1  16
                // - - - -
                case 0xC7:
                    _cpu.DoRst(0x00);
                    return 16;

                // RET Z
                // 1  20/8
                // - - - -
                case 0xC8:
                    if (_cpu.DoReturn(_registers.flagZero))
                        return 20;
                        
                    return 8;
              
                // RET
                // 1  16
                // - - - -
                case 0xC9:
                    _cpu.DoReturn(true);
                    return 16;

                // JP Z,a16
                // 3  16/12
                // - - - -
                case 0xCA:
                {
                    if (_cpu.DoJumpToA16(_registers.flagZero))
                        return 16;

                    return 12;
                }
                case 0xCB:
                {
                    ExecuteCBOperation(_cpu.GetD8());
                    return 4;
                }

                // CALL Z,a16
                // 3  24/12
                // - - - -
                case 0xCC:
                {
                    if (_cpu.DoCall(_registers.flagZero))
                        return 24;
                    
                    return 12;
                }
                // CALL a16
                // 3  24
                // - - - -
                case 0xCD:
                {
                    _cpu.DoCall(true);
                    return 24;
                }
                // ADC A,d8
                // 2  8
                // Z 0 H C
                case 0xCE:
                    _cpu.DoADC(_cpu.GetD8());
                    return 8;

                // RST 08H
                // 1  16
                // - - - -
                case 0xCF:
                    _cpu.DoRst(0x08);
                    return 16;

                // RET NC
                // 1  20/8
                // - - - -
                case 0xD0:

                    if (_cpu.DoReturn(!_registers.flagCarry))
                        return 20;
                        
                    return 8;

                // POP DE
                // 1  12
                // - - - -
                case 0xD1:
                    _registers.de = _cpu.PopStack16();
                    return 12;

                // JP NC,a16
                // 3  16/12
                // - - - - 
                case 0xD2:
                {
                    if (_cpu.DoJumpToA16(!_registers.flagCarry))
                        return 16;
                    
                    return 12;
                }

                // CALL NC,a16
                // 3  24/12
                // - - - -
                case 0xD4:
                    if (_cpu.DoCall(!_registers.flagCarry))
                        return 24;
                    
                    return 12;

                // PUSH DE
                // 1  16
                // - - - -
                case 0xD5:
                    _cpu.PushStack16(_registers.de);
                    return 16;

                // SUB d8
                // 2  8
                // Z 1 H C
                case 0xD6:
                    _cpu.DoSub(_cpu.GetD8());
                    return 8;

                // RST 10H
                // 1  16
                // - - - -
                case 0xD7:
                    _cpu.DoRst(0x10);
                    return 16;

                // RET C
                // 1  20/8
                // - - - -
                case 0xD8:
                    if (_cpu.DoReturn(_registers.flagCarry))
                        return 20;
                        
                    return 8;

                // RETI
                // 1  16
                // - - - -
                case 0xD9:
                    // return from interruption
                    _cpu.DoReturn(true);
                    _cpu.isMasterEnabled = true;
                    return 16;

                // JP C,a16
                // 3  16/12
                // - - - -	
                case 0xDA:
                    if (_cpu.DoJumpToA16(_registers.flagCarry))
                        return 16;

                    return 12;

                // CALL C,a16
                // 3  24/12
                // - - - -
                case 0xDC:
                    if (_cpu.DoCall(_registers.flagCarry))
                        return 24;
                    
                    return 12;

                // SBC A,d8
                // 2  8
                // Z 1 H C
                case 0xDE:
                    _cpu.DoSBC(_cpu.GetD8());
                    return 8;

                // RST 18H
                // 1  16
                // - - - -
                case 0xDF:
                    _cpu.DoRst(0x18);
                    return 16;

                // LDH (a8),A
                // 2  12
                // - - - -
                case 0xE0:
                {
                    ushort highRamAddress = (ushort)(0xFF00 | _cpu.GetD8());
                    _cpu.bus.Write8(highRamAddress, _registers.a);
                    return 12;
                }
               
                // POP HL
                // 1  12
                // - - - -
                case 0xE1:
                    _registers.hl = _cpu.PopStack16();
                    return 12;

                // LD (C),A
                // 2  8
                // - - - -
                case 0xE2:
                    _bus.Write8((ushort)(0xFF00 + _registers.c), _registers.a);
                    return 8;

                // PUSH HL
                // 1  16
                // - - - -
                case 0xE5:
                    _cpu.PushStack16(_registers.hl);
                    return 16;

                // AND d8
                // 2  8
                // Z 0 1 0
                case 0xE6:
                    _cpu.DoAnd(_cpu.GetD8());
                    return 8;

                // RST 20H
                // 1  16
                // - - - -
                case 0xE7:
                    _cpu.DoRst(0x20);
                    return 16;

                // ADD SP,r8
                // 2  16
                // 0 0 H C
                case 0xE8:

                    _registers.sp = _cpu.DoDisplaceR8();
                    return 16;

                // JP (HL)
                // 1  4
                // - - - -
                case 0xE9:
                    _registers.pc = _registers.hl;
                    return 4;

                // LD (a16),A
                // 3  16
                // - - - -
                case 0xEA:
                    _bus.Write8(_cpu.GetD16(), _registers.a);
                    return 16;

                // XOR d8
                // 2  8
                // Z 0 0 0
                case 0xEE:
                    _cpu.DoXOR(_cpu.GetD8());   
                    return 8;

                // RST 28H
                // 1  16
                // - - - -
                case 0xEF:
                    _cpu.DoRst(0x28);
                    return 16;

                // LDH A,(a8)
                // 2  12
                // - - - -
                case 0xF0:
                {
                    ushort highRamAddress = (ushort)(0xFF00 | _cpu.GetD8());
                    _registers.a = _cpu.bus.Read8(highRamAddress);
                    return 12;
                }

                // POP AF
                // 1  12
                // Z N H C
                case 0xF1:
                    _registers.af = _cpu.PopStack16();
                    return 12;

                // LD A,(C)
                // 2  8
                // - - - -
                case 0xF2:
                    _registers.a = _bus.Read8((ushort)(0xFF00  + _registers.c));
                    return 8;

                // DI
                // 1  4
                // - - - -
                case 0xF3:
                    // disables master execution
                    _cpu.isMasterEnabled = false;
                    return 4;

                // PUSH AF
                // 1  16
                // - - - -
                case 0xF5:
                    _cpu.PushStack16(_registers.af);
                    return 16;

                // OR d8
                // 2  8
                // Z 0 0 0
                case 0xF6:
                    _cpu.DoOR(_cpu.GetD8());
                    return 8;

                // RST 30H
                // 1  16
                // - - - -
                case 0xF7:
                    _cpu.DoRst(0x30);
                    return 16;

                // LD HL,SP+r8
                // 2  12
                // 0 0 H C
                case 0xF8:
                    _registers.hl = _cpu.DoDisplaceR8();
                    return 12;

                // LD SP,HL
                // 1  8
                // - - - -
                case 0xF9:
                    _registers.sp = _registers.hl;
                    return 8;

                // LD A,(a16)
                // 3  16
                // - - - -
                case 0xFA:
                    _registers.a = _bus.Read8(_cpu.GetD16());
                    return 16;

                // EI
                // 1  4
                // - - - -
                case 0xFB:
                    _cpu.isMasterEnabled = true;
                    return 4;

                // CP d8
                // 2  8
                // Z 1 H C
                case 0xFE:
                    _cpu.DoCP(_cpu.GetD8());
                    return 8;

                // RST 38H
                // 1  16
                // - - - -
                case 0xFF:
                    _cpu.DoRst(0x38);
                    return 16;

            }

            return -1;
        }

        private int ExecuteCBOperation(byte cbCode)
        {
            switch (cbCode)
            {
                // RLC B
                // 2  8
                // Z 0 0 C
                case 0x00:
                    _registers.b = _cpu.RLC(_registers.b);
                   return 8;

               // RLC C
               // 2  8
               // Z 0 0 C
                case 0x01:
                    _registers.c = _cpu.RLC(_registers.c);
                   return 8;

               // RLC D
               // 2  8
               // Z 0 0 C
                case 0x02:
                    _registers.d = _cpu.RLC(_registers.d);
                   return 8;

               // RLC E
               // 2  8
               // Z 0 0 C
                case 0x03:
                    _registers.e = _cpu.RLC(_registers.e);
                   return 8;

               // RLC H
               // 2  8
               // Z 0 0 C
                case 0x04:
                    _registers.h = _cpu.RLC(_registers.h);
                   return 8;

               // RLC L
               // 2  8
               // Z 0 0 C
                case 0x05:
                    _registers.l = _cpu.RLC(_registers.l);
                   return 8;

               // RLC (HL)
               // 2  16
               // Z 0 0 C
                case 0x06:
                {
                    byte val  = _cpu.RLC(_bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                // RLC A
               // 2  8
               // Z 0 0 C
                case 0x07:
                    _registers.a = _cpu.RLC(_registers.a);
                   return 8;

               // RRC B
               // 2  8
               // Z 0 0 C
                case 0x08:
                    _registers.b = _cpu.RRC(_registers.b);
                   return 8;

               // RRC C
               // 2  8
               // Z 0 0 C
                case 0x09:
                    _registers.c = _cpu.RRC(_registers.c);
                   return 8;

               // RRC D
               // 2  8
               // Z 0 0 C
                case 0x0A:
                    _registers.d = _cpu.RRC(_registers.d);
                   return 8;

               // RRC E
               // 2  8
               // Z 0 0 C
                case 0x0B:
                    _registers.e = _cpu.RRC(_registers.e);
                   return 8;

               // RRC H
               // 2  8
               // Z 0 0 C
                case 0x0C:
                    _registers.h = _cpu.RRC(_registers.h);
                   return 8;

               // RRC L
               // 2  8
               // Z 0 0 C
                case 0x0D:
                    _registers.l = _cpu.RRC(_registers.l);
                   return 8;

               // RRC (HL)
               // 2  16
               // Z 0 0 C
                case 0x0E:
                {
                    byte val  = _cpu.RRC(_bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                // RRC A
               // 2  8
               // Z 0 0 C
                case 0x0F:
                    _registers.a = _cpu.RRC(_registers.a);
                   return 8;

               // RL B
               // 2  8
               // Z 0 0 C
                case 0x10:
                    _registers.b = _cpu.RL(_registers.b);
                   return 8;

                   // RL C
                   // 2  8
                   // Z 0 0 C
                case 0x11:
                    _registers.c = _cpu.RL(_registers.c);
                   return 8;

                   // RL D
                   // 2  8
                   // Z 0 0 C
                case 0x12:
                    _registers.d = _cpu.RL(_registers.d);
                   return 8;

                   // RL E
                   // 2  8
                   // Z 0 0 C
                case 0x13:
                    _registers.e = _cpu.RL(_registers.e);
                   return 8;

                   // RL H
                   // 2  8
                   // Z 0 0 C
                case 0x14:
                    _registers.h = _cpu.RL(_registers.h);
                   return 8;

                   // RL L
                   // 2  8
                   // Z 0 0 C
                case 0x15:
                    _registers.l = _cpu.RL(_registers.l);
                   return 8;

                   // RL (HL)
                   // 2  16
                   // Z 0 0 C
                case 0x16:
                {
                    byte val  = _cpu.RL(_bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                   // RL A
                   // 2  8
                   // Z 0 0 C
                case 0x17:
                    _registers.a = _cpu.RL(_registers.a);
                   return 8;

                   // RR B
                   // 2  8
                   // Z 0 0 C
                case 0x18:
                    _registers.b = _cpu.RR(_registers.b);
                   return 8;

                   // RR C
                   // 2  8
                   // Z 0 0 C
                case 0x19:
                    _registers.c = _cpu.RR(_registers.c);
                   return 8;

                   // RR D
                   // 2  8
                   // Z 0 0 C
                case 0x1A:
                    _registers.d = _cpu.RR(_registers.d);
                   return 8;

                   // RR E
                   // 2  8
                   // Z 0 0 C
                case 0x1B:
                    _registers.e = _cpu.RR(_registers.e);
                   return 8;

                   // RR H
                   // 2  8
                   // Z 0 0 C
                case 0x1C:
                    _registers.h = _cpu.RR(_registers.h);
                   return 8;

                   // RR L
                   // 2  8
                   // Z 0 0 C
                case 0x1D:
                    _registers.l = _cpu.RR(_registers.l);
                   return 8;

                   // RR (HL)
                   // 2  16
                   // Z 0 0 C
                case 0x1E:
                {
                    byte val  = _cpu.RR(_bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                   // RR A
                   // 2  8
                   // Z 0 0 C
                case 0x1F:
                    _registers.a = _cpu.RR(_registers.a);
                   return 8;

                   // SLA B
                   // 2  8
                   // Z 0 0 C
                case 0x20:
                    _registers.b = _cpu.SLA(_registers.b);
                   return 8;
                    
                   // SLA C
                   // 2  8
                   // Z 0 0 C
                case 0x21:
                    _registers.c = _cpu.SLA(_registers.c);
                   return 8;

                   // SLA D
                   // 2  8
                   // Z 0 0 C
                case 0x22:
                    _registers.d = _cpu.SLA(_registers.d);
                   return 8;

                   // SLA E
                   // 2  8
                   // Z 0 0 C
                case 0x23:
                    _registers.e = _cpu.SLA(_registers.e);
                   return 8;

                   // SLA H
                   // 2  8
                   // Z 0 0 C
                case 0x24:
                    _registers.h = _cpu.SLA(_registers.h);
                   return 8;

                   // SLA L
                   // 2  8
                   // Z 0 0 C
                case 0x25:
                    _registers.l = _cpu.SLA(_registers.l);
                   return 8;

                   // SLA (HL)
                   // 2  16
                   // Z 0 0 C
                case 0x26:
                {
                    byte val  = _cpu.SLA(_bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                   // SLA A
                   // 2  8
                   // Z 0 0 C
                case 0x27:
                    _registers.a = _cpu.SLA(_registers.a);
                   return 8;

                   // SRA B
                   // 2  8
                   // Z 0 0 0
                case 0x28:
                    _registers.b = _cpu.SRA(_registers.b);
                   return 8;

                   // SRA C
                   // 2  8
                   // Z 0 0 0
                case 0x29:
                    _registers.c = _cpu.SRA(_registers.c);
                   return 8;

                   // SRA D
                   // 2  8
                   // Z 0 0 0
                case 0x2A:
                    _registers.d = _cpu.SRA(_registers.d);
                   return 8;

                   // SRA E
                   // 2  8
                   // Z 0 0 0
                case 0x2B:
                    _registers.e = _cpu.SRA(_registers.e);
                   return 8;

                   // SRA H
                   // 2  8
                   // Z 0 0 0
                case 0x2C:
                    _registers.h = _cpu.SRA(_registers.h);
                   return 8;

                   // SRA L
                   // 2  8
                   // Z 0 0 0
                case 0x2D:
                    _registers.l = _cpu.SRA(_registers.l);
                   return 8;

                   // SRA (HL)
                   // 2  16
                   // Z 0 0 0
                case 0x2E:
                {
                    byte val  = _cpu.SRA(_bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                    // SRA A
                   // 2  8
                   // Z 0 0 0
                case 0x2F:
                    _registers.a = _cpu.SRA(_registers.a);
                   return 8;

                   // SWAP B
                   // 2  8
                   // Z 0 0 0
                case 0x30:
                    _registers.b = _cpu.SWAP(_registers.b);
                   return 8;

                   // SWAP C
                   // 2  8
                   // Z 0 0 0
                case 0x31:
                    _registers.c = _cpu.SWAP(_registers.c);
                   return 8;

                   // SWAP D
                   // 2  8
                   // Z 0 0 0
                case 0x32:
                    _registers.d = _cpu.SWAP(_registers.d);
                   return 8;

                   // SWAP E
                   // 2  8
                   // Z 0 0 0
                case 0x33:
                    _registers.e = _cpu.SWAP(_registers.e);
                   return 8;

                   // SWAP H
                   // 2  8
                   // Z 0 0 0
                case 0x34:
                    _registers.h = _cpu.SWAP(_registers.h);
                   return 8;

                   // SWAP L
                   // 2  8
                   // Z 0 0 0
                case 0x35:
                    _registers.l = _cpu.SWAP(_registers.l);
                   return 8;

                   // SWAP (HL)
                   // 2  16
                   // Z 0 0 0
                case 0x36:
                {
                    byte val  = _cpu.SWAP(_bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }
                
                   // SWAP A
                   // 2  8
                   // Z 0 0 0
                case 0x37:
                    _registers.a = _cpu.SWAP(_registers.a);
                   return 8;

                   // SRL B
                   // 2  8
                   // Z 0 0 C
                case 0x38:
                    _registers.b = _cpu.SRL(_registers.b);
                   return 8;

                   // SRL C
                   // 2  8
                   // Z 0 0 C
                case 0x39:
                    _registers.c = _cpu.SRL(_registers.c);
                   return 8;

                   // SRL D
                   // 2  8
                   // Z 0 0 C
                case 0x3A:
                    _registers.d = _cpu.SRL(_registers.d);
                   return 8;

                   // SRL E
                   // 2  8
                   // Z 0 0 C
                case 0x3B:
                    _registers.e = _cpu.SRL(_registers.e);
                   return 8;

                   // SRL H
                   // 2  8
                   // Z 0 0 C
                case 0x3C:
                    _registers.h = _cpu.SRL(_registers.h);
                   return 8;

                   // SRL L
                   // 2  8
                   // Z 0 0 C
                case 0x3D:
                    _registers.l = _cpu.SRL(_registers.l);
                   return 8;

                   // SRL (HL)
                   // 2  16
                   // Z 0 0 C
                case 0x3E:
                {
                    byte val  = _cpu.SRL(_bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                   // SRL A
                   // 2  8
                   // Z 0 0 C
                case 0x3F:
                    _registers.a = _cpu.SRL(_registers.a);
                   return 8;

                   // BIT 0,B
                   // 2  8
                   // Z 0 1 -
                case 0x40:
                    _cpu.BIT(0x1, _registers.b);
                   return 8;

                   // BIT 0,C
                   // 2  8
                   // Z 0 1 -
                case 0x41:
                    _cpu.BIT(0x1, _registers.c);
                   return 8;

                   // BIT 0,D
                   // 2  8
                   // Z 0 1 -
                case 0x42:
                    _cpu.BIT(0x1, _registers.d);
                   return 8;

                   // BIT 0,E
                   // 2  8
                   // Z 0 1 -
                case 0x43:
                    _cpu.BIT(0x1, _registers.e);
                   return 8;

                   // BIT 0,H
                   // 2  8
                   // Z 0 1 -
                case 0x44:
                    _cpu.BIT(0x1, _registers.h);
                   return 8;

                   // BIT 0,L
                   // 2  8
                   // Z 0 1 -
                case 0x45:
                    _cpu.BIT(0x1, _registers.l);
                   return 8;

                   // BIT 0,(HL)
                   // 2  16
                   // Z 0 1 -
                case 0x46:
                    _cpu.BIT(0x1, _bus.Read8(_registers.hl));
                   return 8;

                   // BIT 0,A
                   // 2  8
                   // Z 0 1 -
                case 0x47:
                    _cpu.BIT(0x1, _registers.a);
                   return 8;

                   // BIT 1,B
                   // 2  8
                   // Z 0 1 -
                case 0x48:
                    _cpu.BIT(0x2, _registers.b);
                   return 8;

                   // BIT 1,C
                   // 2  8
                   // Z 0 1 -
                case 0x49:
                    _cpu.BIT(0x2, _registers.c);
                   return 8;

                   // BIT 1,D
                   // 2  8
                   // Z 0 1 -
                case 0x4A:
                    _cpu.BIT(0x2, _registers.d);
                   return 8;

                   // BIT 1,E
                   // 2  8
                   // Z 0 1 -
                case 0x4B:
                    _cpu.BIT(0x2, _registers.e);
                   return 8;

                   // BIT 1,H
                   // 2  8
                   // Z 0 1 -
                case 0x4C:
                    _cpu.BIT(0x2, _registers.h);
                   return 8;

                   // BIT 1,L
                   // 2  8
                   // Z 0 1 -
                case 0x4D:
                    _cpu.BIT(0x2, _registers.l);
                   return 8;

                   // BIT 1,(HL)
                   // 2  16
                   // Z 0 1 -
                case 0x4E:
                    _cpu.BIT(0x2, _bus.Read8(_registers.hl));
                   return 8;

                   // BIT 1,A
                   // 2  8
                   // Z 0 1 -
                case 0x4F:
                    _cpu.BIT(0x2, _registers.a);
                   return 8;

                   // BIT 2,B
                   // 2  8
                   // Z 0 1 -
                case 0x50:
                    _cpu.BIT(0x4, _registers.b);
                   return 8;

                   // BIT 2,C
                   // 2  8
                   // Z 0 1 -
                case 0x51:
                    _cpu.BIT(0x4, _registers.c);
                   return 8;

                   // BIT 2,D
                   // 2  8
                   // Z 0 1 -
                case 0x52:
                    _cpu.BIT(0x4, _registers.d);
                   return 8;

                   // BIT 2,E
                   // 2  8
                   // Z 0 1 -
                case 0x53:
                    _cpu.BIT(0x4, _registers.e);
                   return 8;

                   // BIT 2,H
                   // 2  8
                   // Z 0 1 -
                case 0x54:
                    _cpu.BIT(0x4, _registers.h);
                   return 8;

                   // BIT 2,L
                   // 2  8
                   // Z 0 1 -
                case 0x55:
                    _cpu.BIT(0x4, _registers.l);
                   return 8;

                   // BIT 2,(HL)
                   // 2  16
                   // Z 0 1 -
                case 0x56:
                    _cpu.BIT(0x4, _bus.Read8(_registers.hl));
                   return 8;

                   // BIT 2,A
                   // 2  8
                   // Z 0 1 -
                case 0x57:
                    _cpu.BIT(0x4, _registers.a);
                   return 8;

                   // BIT 3,B
                   // 2  8
                   // Z 0 1 -
                case 0x58:
                    _cpu.BIT(0x8, _registers.b);
                   return 8;

                   // BIT 3,C
                   // 2  8
                   // Z 0 1 -
                case 0x59:
                    _cpu.BIT(0x8, _registers.c);
                   return 8;

                   // BIT 3,D
                   // 2  8
                   // Z 0 1 -
                case 0x5A:
                    _cpu.BIT(0x8, _registers.d);
                   return 8;

                   // BIT 3,E
                   // 2  8
                   // Z 0 1 -
                case 0x5B:
                    _cpu.BIT(0x8, _registers.e);
                   return 8;

                   // BIT 3,H
                   // 2  8
                   // Z 0 1 -
                case 0x5C:
                    _cpu.BIT(0x8, _registers.h);
                   return 8;

                   // BIT 3,L
                   // 2  8
                   // Z 0 1 -
                case 0x5D:
                    _cpu.BIT(0x8, _registers.l);
                   return 8;

                   // BIT 3,(HL)
                   // 2  16
                   // Z 0 1 -
                case 0x5E:
                    _cpu.BIT(0x8, _bus.Read8(_registers.hl));
                   return 8;

                   // BIT 3,A
                   // 2  8
                   // Z 0 1 -
                case 0x5F:
                    _cpu.BIT(0x8, _registers.a);
                   return 8;

                   // BIT 4,B
                   // 2  8
                   // Z 0 1 -
                case 0x60:
                    _cpu.BIT(0x10, _registers.b);
                   return 8;

                   // BIT 4,C
                   // 2  8
                   // Z 0 1 -
                case 0x61:
                    _cpu.BIT(0x10, _registers.c);
                   return 8;

                   // BIT 4,D
                   // 2  8
                   // Z 0 1 -
                case 0x62:
                    _cpu.BIT(0x10, _registers.d);
                   return 8;

                   // BIT 4,E
                   // 2  8
                   // Z 0 1 -
                case 0x63:
                    _cpu.BIT(0x10, _registers.e);
                   return 8;

                   // BIT 4,H
                   // 2  8
                   // Z 0 1 -
                case 0x64:
                    _cpu.BIT(0x10, _registers.h);
                   return 8;

                   // BIT 4,L
                   // 2  8
                   // Z 0 1 -
                case 0x65:
                    _cpu.BIT(0x10, _registers.l);
                   return 8;

                   // BIT 4,(HL)
                   // 2  16
                   // Z 0 1 -
                case 0x66:
                    _cpu.BIT(0x10, _bus.Read8(_registers.hl));
                   return 8;

                   // BIT 4,A
                   // 2  8
                   // Z 0 1 -
                case 0x67:
                    _cpu.BIT(0x10, _registers.a);
                   return 8;

                   // BIT 5,B
                   // 2  8
                   // Z 0 1 -
                case 0x68:
                    _cpu.BIT(0x20, _registers.b);
                   return 8;

                   // BIT 5,C
                   // 2  8
                   // Z 0 1 -
                case 0x69:
                    _cpu.BIT(0x20, _registers.c);
                   return 8;

                   // BIT 5,D
                   // 2  8
                   // Z 0 1 -
                case 0x6A:
                    _cpu.BIT(0x20, _registers.d);
                   return 8;

                   // BIT 5,E
                   // 2  8
                   // Z 0 1 -
                case 0x6B:
                    _cpu.BIT(0x20, _registers.e);
                   return 8;

                   // BIT 5,H
                   // 2  8
                   // Z 0 1 -
                case 0x6C:
                    _cpu.BIT(0x20, _registers.h);
                   return 8;

                   // BIT 5,L
                   // 2  8
                   // Z 0 1 -
                case 0x6D:
                    _cpu.BIT(0x20, _registers.l);
                   return 8;

                   // BIT 5,(HL)
                   // 2  16
                   // Z 0 1 -
                case 0x6E:
                    _cpu.BIT(0x20, _bus.Read8(_registers.hl));
                   return 8;

                   // BIT 5,A
                   // 2  8
                   // Z 0 1 -
                case 0x6F:
                    _cpu.BIT(0x20, _registers.a);
                   return 8;

                   // BIT 6,B
                   // 2  8
                   // Z 0 1 -
                case 0x70:
                    _cpu.BIT(0x40, _registers.b);
                   return 8;

                   // BIT 6,C
                   // 2  8
                   // Z 0 1 -
                case 0x71:
                    _cpu.BIT(0x40, _registers.c);
                   return 8;

                   // BIT 6,D
                   // 2  8
                   // Z 0 1 -
                case 0x72:
                    _cpu.BIT(0x40, _registers.d);
                   return 8;

                   // BIT 6,E
                   // 2  8
                   // Z 0 1 -
                case 0x73:
                    _cpu.BIT(0x40, _registers.e);
                   return 8;

                   // BIT 6,H
                   // 2  8
                   // Z 0 1 -
                case 0x74:
                    _cpu.BIT(0x40, _registers.h);
                   return 8;

                   // BIT 6,L
                   // 2  8
                   // Z 0 1 -
                case 0x75:
                    _cpu.BIT(0x40, _registers.l);
                   return 8;

                   // BIT 6,(HL)
                   // 2  16
                   // Z 0 1 -
                case 0x76:
                    _cpu.BIT(0x40, _bus.Read8(_registers.hl));
                   return 8;

                   // BIT 6,A
                   // 2  8
                   // Z 0 1 -
                case 0x77:
                    _cpu.BIT(0x40, _registers.a);
                   return 8;

                   // BIT 7,B
                   // 2  8
                   // Z 0 1 -
                case 0x78:
                    _cpu.BIT(0x80, _registers.b);
                   return 8;

                   // BIT 7,C
                   // 2  8
                   // Z 0 1 -
                case 0x79:
                    _cpu.BIT(0x80, _registers.c);
                   return 8;

                   // BIT 7,D
                   // 2  8
                   // Z 0 1 -
                case 0x7A:
                    _cpu.BIT(0x80, _registers.d);
                   return 8;

                   // BIT 7,E
                   // 2  8
                   // Z 0 1 -
                case 0x7B:
                    _cpu.BIT(0x80, _registers.e);
                   return 8;

                   // BIT 7,H
                   // 2  8
                   // Z 0 1 -
                case 0x7C:
                    _cpu.BIT(0x80, _registers.h);
                   return 8;

                   // BIT 7,L
                   // 2  8
                   // Z 0 1 -
                case 0x7D:
                    _cpu.BIT(0x80, _registers.l);
                   return 8;

                   // BIT 7,(HL)
                   // 2  16
                   // Z 0 1 -
                case 0x7E:
                    _cpu.BIT(0x80, _bus.Read8(_registers.hl));
                   return 8;

                   // BIT 7,A
                   // 2  8
                   // Z 0 1 -
                case 0x7F:
                    _cpu.BIT(0x80, _registers.a);
                   return 8;

                   // RES 0,B
                   // 2  8 
                   // - - - -
                case 0x80:
                    _registers.b = _cpu.RES(0x1, _registers.b);
                   return 8;

                   // RES 0,C
                   // 2  8
                   // - - - -
                case 0x81:
                    _registers.c = _cpu.RES(0x1, _registers.c);
                   return 8;

                   // RES 0,D
                   // 2  8
                   // - - - -
                case 0x82:
                    _registers.d = _cpu.RES(0x1, _registers.d);
                   return 8;

                   // RES 0,E
                   // 2  8
                   //     - - - -
                case 0x83:
                    _registers.e = _cpu.RES(0x1, _registers.e);
                   return 8;

                   // RES 0,H
                   // 2  8
                   //     - - - -
                case 0x84:
                    _registers.h = _cpu.RES(0x1, _registers.h);
                   return 8;

                   // RES 0,L
                   // 2  8
                   //     - - - -
                case 0x85:
                    _registers.l = _cpu.RES(0x1, _registers.l);
                   return 8;

                   // RES 0,(HL)
                   //     2  16
                   //     - - - -
                case 0x86:
                {
                    byte val = _cpu.RES(0x1, _bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }
                   // RES 0,A
                   // 2  8
                   //     - - - -
                case 0x87:
                    _registers.a = _cpu.RES(0x1, _registers.a);
                   return 8;

                   // RES 1,B
                   // 2  8
                   //     - - - -
                case 0x88:
                    _registers.b = _cpu.RES(0x2, _registers.b);
                   return 8;

                   // RES 1,C
                   // 2  8
                   //     - - - -
                case 0x89:
                    _registers.c = _cpu.RES(0x2, _registers.c);
                   return 8;

                   // RES 1,D
                   // 2  8
                   //     - - - -
                case 0x8A:
                    _registers.d = _cpu.RES(0x2, _registers.d);
                   return 8;

                   // RES 1,E
                   // 2  8
                   //     - - - -
                case 0x8B:
                    _registers.e = _cpu.RES(0x2, _registers.e);
                   return 8;

                   // RES 1,H
                   // 2  8
                   //     - - - -
                case 0x8C:
                    _registers.h = _cpu.RES(0x2, _registers.h);
                   return 8;

                   // RES 1,L
                   // 2  8
                   //     - - - -
                case 0x8D:
                    _registers.l = _cpu.RES(0x2, _registers.l);
                   return 8;

                   // RES 1,(HL)
                   //     2  16
                   //     - - - -
                case 0x8E:
                {
                    byte val = _cpu.RES(0x2, _bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                    // RES 1,A
                   // 2  8
                   //     - - - -
                case 0x8F:
                    _registers.a = _cpu.RES(0x2, _registers.a);
                   return 8;

                   // RES 2,B
                   // 2  8
                   //     - - - -
                case 0x90:
                    _registers.b = _cpu.RES(0x4, _registers.b);
                   return 8;

                   // RES 2,C
                   // 2  8
                   //     - - - -
                case 0x91:
                    _registers.c = _cpu.RES(0x4, _registers.c);
                   return 8;

                   // RES 2,D
                   // 2  8
                   //     - - - -
                case 0x92:
                    _registers.d = _cpu.RES(0x4, _registers.d);
                   return 8;

                   // RES 2,E
                   // 2  8
                   //     - - - -
                case 0x93:
                    _registers.e = _cpu.RES(0x4, _registers.e);
                   return 8;

                   // RES 2,H
                   // 2  8
                   //     - - - -
                case 0x94:
                    _registers.h = _cpu.RES(0x4, _registers.h);
                   return 8;

                   // RES 2,L
                   // 2  8
                   //     - - - -
                case 0x95:
                    _registers.l = _cpu.RES(0x4, _registers.l);
                   return 8;

                   // RES 2,(HL)
                   //     2  16
                   //     - - - -
                case 0x96:
                {
                    byte val = _cpu.RES(0x4, _bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                    // RES 2,A
                   // 2  8
                   //     - - - -
                case 0x97:
                    _registers.a = _cpu.RES(0x4, _registers.a);
                   return 8;

                   // RES 3,B
                   // 2  8
                   //     - - - -
                case 0x98:
                    _registers.b = _cpu.RES(0x8, _registers.b);
                   return 8;

                   // RES 3,C
                   // 2  8
                   //     - - - -
                case 0x99:
                    _registers.c = _cpu.RES(0x8, _registers.c);
                   return 8;

                   // RES 3,D
                   // 2  8
                   //     - - - -
                case 0x9A:
                    _registers.d = _cpu.RES(0x8, _registers.d);
                   return 8;

                   // RES 3,E
                   // 2  8
                   //     - - - -
                case 0x9B:
                    _registers.e = _cpu.RES(0x8, _registers.e);
                   return 8;

                   // RES 3,H
                   // 2  8
                   //     - - - -
                case 0x9C:
                    _registers.h = _cpu.RES(0x8, _registers.h);
                   return 8;

                   // RES 3,L
                   // 2  8
                   //     - - - -
                case 0x9D:
                    _registers.l = _cpu.RES(0x8, _registers.l);
                   return 8;

                   // RES 3,(HL)
                   //     2  16
                   //     - - - -
                case 0x9E:
                {
                    byte val = _cpu.RES(0x8, _bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                   // RES 3,A
                   // 2  8
                   //     - - - -
                case 0x9F:
                    _registers.a = _cpu.RES(0x8, _registers.a);
                   return 8;

                   // RES 4,B
                   // 2  8
                   //     - - - -
                case 0xA0:
                    _registers.b = _cpu.RES(0x10, _registers.b);
                   return 8;

                   // RES 4,C
                   // 2  8
                   //     - - - -
                case 0xA1:
                    _registers.c = _cpu.RES(0x10, _registers.c);
                   return 8;

                   // RES 4,D
                   // 2  8
                   //     - - - -
                case 0xA2:
                    _registers.d = _cpu.RES(0x10, _registers.d);
                   return 8;

                   // RES 4,E
                   // 2  8
                   //     - - - -
                case 0xA3:
                    _registers.e = _cpu.RES(0x10, _registers.e);
                   return 8;

                   // RES 4,H
                   // 2  8
                   //     - - - -
                case 0xA4:
                    _registers.h = _cpu.RES(0x10, _registers.h);
                   return 8;

                   // RES 4,L
                   // 2  8
                   //     - - - -
                case 0xA5:
                    _registers.l = _cpu.RES(0x10, _registers.l);
                   return 8;

                   // RES 4,(HL)
                   //     2  16
                   //     - - - -
                case 0xA6:
                {
                    byte val = _cpu.RES(0x10, _bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                   // RES 4,A
                   // 2  8
                   //     - - - -
                case 0xA7:
                    _registers.a = _cpu.RES(0x10, _registers.a);
                   return 8;

                   // RES 5,B
                   // 2  8
                   //     - - - -
                case 0xA8:
                    _registers.b = _cpu.RES(0x20, _registers.b);
                   return 8;

                   // RES 5,C
                   // 2  8
                   //     - - - -
                case 0xA9:
                    _registers.c = _cpu.RES(0x20, _registers.c);
                   return 8;

                   // RES 5,D
                   // 2  8
                   //     - - - -
                case 0xAA:
                    _registers.d = _cpu.RES(0x20, _registers.d);
                   return 8;

                   // RES 5,E
                   // 2  8
                   //     - - - -
                case 0xAB:
                    _registers.e = _cpu.RES(0x20, _registers.e);
                   return 8;

                   // RES 5,H
                   // 2  8
                   //     - - - -
                case 0xAC:
                    _registers.h = _cpu.RES(0x20, _registers.h);
                   return 8;

                   // RES 5,L
                   // 2  8
                   //     - - - -
                case 0xAD:
                    _registers.l = _cpu.RES(0x20, _registers.l);
                   return 8;

                   // RES 5,(HL)
                   //     2  16
                   //     - - - -
                case 0xAE:
                {
                    byte val = _cpu.RES(0x20, _bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                   // RES 5,A
                   // 2  8
                   //     - - - -
                case 0xAF:
                    _registers.a = _cpu.RES(0x20, _registers.a);
                   return 8;

                   // RES 6,B
                   // 2  8
                   //     - - - -
                case 0xB0:
                    _registers.b = _cpu.RES(0x40, _registers.b);
                   return 8;

                   // RES 6,C
                   // 2  8
                   //     - - - -
                case 0xB1:
                    _registers.c = _cpu.RES(0x40, _registers.c);
                   return 8;

                   // RES 6,D
                   // 2  8
                   //     - - - -
                case 0xB2:
                    _registers.d = _cpu.RES(0x40, _registers.d);
                   return 8;

                   // RES 6,E
                   // 2  8
                   //     - - - -
                case 0xB3:
                    _registers.e = _cpu.RES(0x40, _registers.e);
                   return 8;

                   // RES 6,H
                   // 2  8
                   //     - - - -
                case 0xB4:
                    _registers.h = _cpu.RES(0x40, _registers.h);
                   return 8;

                   // RES 6,L
                   // 2  8
                   //     - - - -
                case 0xB5:
                    _registers.l = _cpu.RES(0x40, _registers.l);
                   return 8;

                   // RES 6,(HL)
                   //     2  16
                   //     - - - -
                case 0xB6:
                {
                    byte val = _cpu.RES(0x40, _bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                   // RES 6,A
                   // 2  8
                   //     - - - -
                case 0xB7:
                    _registers.a = _cpu.RES(0x40, _registers.a);
                   return 8;

                   // RES 7,B
                   // 2  8
                   //     - - - -
                case 0xB8:
                    _registers.b = _cpu.RES(0x80, _registers.b);
                   return 8;

                   // RES 7,C
                   // 2  8
                   //     - - - -
                case 0xB9:
                    _registers.c = _cpu.RES(0x80, _registers.c);
                   return 8;

                   // RES 7,D
                   // 2  8
                   //     - - - -
                case 0xBA:
                    _registers.d = _cpu.RES(0x80, _registers.d);
                   return 8;

                   // RES 7,E
                   // 2  8
                   //     - - - -
                case 0xBB:
                    _registers.e = _cpu.RES(0x80, _registers.e);
                   return 8;

                   // RES 7,H
                   // 2  8
                   //     - - - -
                case 0xBC:
                    _registers.h = _cpu.RES(0x80, _registers.h);
                   return 8;

                   // RES 7,L
                   // 2  8
                   //     - - - -
                case 0xBD:
                    _registers.l = _cpu.RES(0x80, _registers.l);
                   return 8;

                   // RES 7,(HL)
                   //     2  16
                   //     - - - -
                case 0xBE:
                {
                    byte val = _cpu.RES(0x80, _bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                   // RES 7,A
                   // 2  8
                   //     - - - -
                case 0xBF:
                    _registers.a = _cpu.RES(0x80, _registers.a);
                   return 8;

                   // SET 0,B
                   // 2  8
                   //     - - - -
                case 0xC0:
                    _registers.b = _cpu.SET(0x1, _registers.b);
                   return 8;

                   // SET 0,C
                   // 2  8
                   //     - - - -
                case 0xC1:
                    _registers.c = _cpu.SET(0x1, _registers.c);
                   return 8;

                   // SET 0,D
                   // 2  8
                   //     - - - -
                case 0xC2:
                    _registers.d = _cpu.SET(0x1, _registers.d);
                   return 8;

                   // SET 0,E
                   // 2  8
                   //     - - - -
                case 0xC3:
                    _registers.e = _cpu.SET(0x1, _registers.e);
                   return 8;

                   // SET 0,H
                   // 2  8
                   //     - - - -
                case 0xC4:
                    _registers.h = _cpu.SET(0x1, _registers.h);
                   return 8;

                   // SET 0,L
                   // 2  8
                   //     - - - -
                case 0xC5:
                    _registers.l = _cpu.SET(0x1, _registers.l);
                   return 8;

                   // SET 0,(HL)
                   //     2  16
                   //     - - - -
                case 0xC6:
                {
                    byte val = _cpu.SET(0x1, _bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                   // SET 0,A
                   // 2  8
                   //     - - - -
                case 0xC7:
                    _registers.a = _cpu.SET(0x1, _registers.a);
                   return 8;

                   // SET 1,B
                   // 2  8
                   //     - - - -
                case 0xC8:
                    _registers.b = _cpu.SET(0x2, _registers.b);
                   return 8;

                   // SET 1,C
                   // 2  8
                   //     - - - -
                case 0xC9:
                    _registers.c = _cpu.SET(0x2, _registers.c);
                   return 8;

                   // SET 1,D
                   // 2  8
                   //     - - - -
                case 0xCA:
                    _registers.d = _cpu.SET(0x2, _registers.d);
                   return 8;

                   // SET 1,E
                   // 2  8
                   //     - - - -
                case 0xCB:
                    _registers.e = _cpu.SET(0x2, _registers.e);
                   return 8;

                   // SET 1,H
                   // 2  8
                   //     - - - -
                case 0xCC:
                    _registers.h = _cpu.SET(0x2, _registers.h);
                   return 8;

                   // SET 1,L
                   // 2  8
                   //     - - - -
                case 0xCD:
                    _registers.l = _cpu.SET(0x2, _registers.l);
                   return 8;

                   // SET 1,(HL)
                   //     2  16
                   //     - - - -
                case 0xCE:
                {
                    byte val = _cpu.SET(0x2, _bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                   // SET 1,A
                   // 2  8
                   //     - - - -
                case 0xCF:
                    _registers.a = _cpu.SET(0x2, _registers.a);
                   return 8;

                   // SET 2,B
                   // 2  8
                   //     - - - -
                case 0xD0:
                    _registers.b = _cpu.SET(0x4, _registers.b);
                   return 8;

                   // SET 2,C
                   // 2  8
                   //     - - - -
                case 0xD1:
                    _registers.c = _cpu.SET(0x4, _registers.c);
                   return 8;

                   // SET 2,D
                   // 2  8
                   //     - - - -
                case 0xD2:
                    _registers.d = _cpu.SET(0x4, _registers.d);
                   return 8;

                   // SET 2,E
                   // 2  8
                   //     - - - -
                case 0xD3:
                    _registers.e = _cpu.SET(0x4, _registers.e);
                   return 8;

                   // SET 2,H
                   // 2  8
                   //     - - - -
                case 0xD4:
                    _registers.h = _cpu.SET(0x4, _registers.h);
                   return 8;

                   // SET 2,L
                   // 2  8
                   //     - - - -
                case 0xD5:
                    _registers.l = _cpu.SET(0x4, _registers.l);
                   return 8;

                   // SET 2,(HL)
                   //     2  16
                   //     - - - -
                case 0xD6:
                {
                    byte val = _cpu.SET(0x4, _bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                   // SET 2,A
                   // 2  8
                   //     - - - -
                case 0xD7:
                    _registers.a = _cpu.SET(0x4, _registers.a);
                   return 8;

                   // SET 3,B
                   // 2  8
                   //     - - - -
                case 0xD8:
                    _registers.b = _cpu.SET(0x8, _registers.b);
                   return 8;

                   // SET 3,C
                   // 2  8
                   //     - - - -
                case 0xD9:
                    _registers.c = _cpu.SET(0x8, _registers.c);
                   return 8;

                   // SET 3,D
                   // 2  8
                   //     - - - -
                case 0xDA:
                    _registers.d = _cpu.SET(0x8, _registers.d);
                   return 8;

                   // SET 3,E
                   // 2  8
                   //     - - - -
                case 0xDB:
                    _registers.e = _cpu.SET(0x8, _registers.e);
                   return 8;
                
                   // SET 3,H
                   // 2  8
                   //     - - - -
                case 0xDC:
                    _registers.h = _cpu.SET(0x8, _registers.h);
                   return 8;

                   // SET 3,L
                   // 2  8
                   //     - - - -
                case 0xDD:
                    _registers.l = _cpu.SET(0x8, _registers.l);
                   return 8;

                   // SET 3,(HL)
                   //     2  16
                   //     - - - -
                case 0xDE:
                {
                    byte val = _cpu.SET(0x8, _bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                   // SET 3,A
                   // 2  8
                   //     - - - -
                case 0xDF:
                    _registers.a = _cpu.SET(0x8, _registers.a);
                   return 8;

                   // SET 4,B
                   // 2  8
                   //     - - - -
                case 0xE0:
                    _registers.b = _cpu.SET(0x10, _registers.b);
                   return 8;

                   // SET 4,C
                   // 2  8
                   //     - - - -
                case 0xE1:
                    _registers.c = _cpu.SET(0x10, _registers.c);
                   return 8;

                   // SET 4,D
                   // 2  8
                   //     - - - -
                case 0xE2:
                    _registers.d = _cpu.SET(0x10, _registers.d);
                   return 8;

                   // SET 4,E
                   // 2  8
                   //     - - - -
                case 0xE3:
                    _registers.e = _cpu.SET(0x10, _registers.e);
                   return 8;

                   // SET 4,H
                   // 2  8
                   //     - - - -
                case 0xE4:
                    _registers.h = _cpu.SET(0x10, _registers.h);
                   return 8;

                   // SET 4,L
                   // 2  8
                   //     - - - -
                case 0xE5:
                    _registers.l = _cpu.SET(0x10, _registers.l);
                   return 8;

                   // SET 4,(HL)
                   //     2  16
                   //     - - - -
                case 0xE6:
                {
                    byte val = _cpu.SET(0x10, _bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                   // SET 4,A
                   // 2  8
                   //     - - - -
                case 0xE7:
                    _registers.a = _cpu.SET(0x10, _registers.a);
                   return 8;

                   // SET 5,B
                   // 2  8
                   //     - - - -
                case 0xE8:
                    _registers.b = _cpu.SET(0x20, _registers.b);
                   return 8;

                   // SET 5,C
                   // 2  8
                   //     - - - -
                case 0xE9:
                    _registers.c = _cpu.SET(0x20, _registers.c);
                   return 8;

                   // SET 5,D
                   // 2  8
                   //     - - - -
                case 0xEA:
                    _registers.d = _cpu.SET(0x20, _registers.d);
                   return 8;

                   // SET 5,E
                   // 2  8
                   //     - - - -
                case 0xEB:
                    _registers.e = _cpu.SET(0x20, _registers.e);
                   return 8;

                   // SET 5,H
                   // 2  8
                   //     - - - -
                case 0xEC:
                    _registers.h = _cpu.SET(0x20, _registers.h);
                   return 8;

                   // SET 5,L
                   // 2  8
                   //     - - - -
                case 0xED:
                    _registers.l = _cpu.SET(0x20, _registers.l);
                   return 8;

                   // SET 5,(HL)
                   //     2  16
                   //     - - - -
                case 0xEE:
                {
                    byte val = _cpu.SET(0x20, _bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                   // SET 5,A
                   // 2  8
                   //     - - - -
                case 0xEF:
                    _registers.a = _cpu.SET(0x20, _registers.a);
                   return 8;

                   // SET 6,B
                   // 2  8
                   //     - - - -
                case 0xF0:
                    _registers.b = _cpu.SET(0x40, _registers.b);
                   return 8;

                   // SET 6,C
                   // 2  8
                   //     - - - -
                case 0xF1:
                    _registers.c = _cpu.SET(0x40, _registers.c);
                   return 8;

                   // SET 6,D
                   // 2  8
                   //     - - - -
                case 0xF2:
                    _registers.d = _cpu.SET(0x40, _registers.d);
                   return 8;

                   // SET 6,E
                   // 2  8
                   //     - - - -
                case 0xF3:
                    _registers.e = _cpu.SET(0x40, _registers.e);
                   return 8;

                   // SET 6,H
                   // 2  8
                   //     - - - -
                case 0xF4:
                    _registers.h = _cpu.SET(0x40, _registers.h);
                   return 8;

                   // SET 6,L
                   // 2  8
                   //     - - - -
                case 0xF5:
                    _registers.l = _cpu.SET(0x40, _registers.l);
                   return 8;

                   // SET 6,(HL)
                   //     2  16
                   //     - - - -
                case 0xF6:
                {
                    byte val = _cpu.SET(0x40, _bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                   // SET 6,A
                   // 2  8
                   //     - - - -
                case 0xF7:
                    _registers.a = _cpu.SET(0x40, _registers.a);
                   return 8;

                   // SET 7,B
                   // 2  8
                   //     - - - -
                case 0xF8:
                    _registers.b = _cpu.SET(0x80, _registers.b);
                   return 8;

                   // SET 7,C
                   // 2  8
                   //     - - - -
                case 0xF9:
                    _registers.c = _cpu.SET(0x80, _registers.c);
                   return 8;

                   // SET 7,D
                   // 2  8
                   //     - - - -
                case 0xFA:
                    _registers.d = _cpu.SET(0x80, _registers.d);
                   return 8;

                   // SET 7,E
                   // 2  8
                   //     - - - -
                case 0xFB:
                    _registers.e = _cpu.SET(0x80, _registers.e);
                   return 8;

                   // SET 7,H
                   // 2  8
                   //     - - - -
                case 0xFC:
                    _registers.h = _cpu.SET(0x80, _registers.h);
                   return 8;

                   // SET 7,L
                   // 2  8
                   //     - - - -
                case 0xFD:
                    _registers.l = _cpu.SET(0x80, _registers.l);
                   return 8;

                   // SET 7,(HL)
                   //     2  16
                   //     - - - -
                case 0xFE:
                {
                    byte val = _cpu.SET(0x80, _bus.Read8(_registers.hl));
                    _bus.Write8(_registers.hl, val);
                    return 8;
                }

                   // SET 7,A
                   // 2  8
                   //     - - - -
                case 0xFF:
                    _registers.a = _cpu.SET(0x80, _registers.a);
                   return 8;
            }
            
            return -1;
        }
        
    }
}