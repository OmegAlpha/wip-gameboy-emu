using System;
using System.Collections.Generic;
using Drongo.GameboyEmulator.Utils;
using UnityEngine;

namespace Drongo.GameboyEmulator
{
    /// <summary>
    /// Central Processing Unit
    /// </summary>
    [Serializable]
    public class CPU
    {
        public bool showDebugOperations;
        
        public CPU_Registers registers;

        // more about HALT here https://gbdev.io/pandocs/halt.html
        public bool isHalted;
        // false if there is an interruption in progress. 
        // which means "master" is the execution of the "common" code and not
        // interruption served code
        public bool isMasterEnabled;
        public bool stopCounter;
        
        // nice explanation about haltBug in the comments
        // https://www.reddit.com/r/EmuDev/comments/5ie3k7/infinite_loop_trying_to_pass_blarggs_interrupt/
        // and also in pan docs https://gbdev.io/pandocs/Interrupts.html
        public bool haltBug;

        public AddressBus bus;

        public OpDefinition currentOp;
        public ushort opPC;

        public InstructionProcessor processor;

        private GameBoy _gb;

        public InstructionDebug[] debugOperationsFuture;
        public InstructionDebug[] debugOperations;
        public long qtyInstructions;
        
        public CPU(GameBoy gb)
        {
            _gb = gb;
            bus = gb.bus;

            // important to ensure correct interruptions
            isMasterEnabled = true;
            
            registers = new CPU_Registers();
            
            processor = new InstructionProcessor();
            processor.SetContext(this);
            
            OperationsMap.Initialize();

            debugOperations = new InstructionDebug[10];
            for (int i = 0; i < 10; i++)
            {
                debugOperations[i] = DebugInstructions.Get(_gb, 0x00);
            }

            debugOperationsFuture = new InstructionDebug[5];
            for (int i = 0; i < 5; i++)
            {
                debugOperationsFuture[i] = DebugInstructions.Get(_gb, 0x00);
            }

        }

        // how to handle interrupts
        // https://gbdev.io/pandocs/Interrupts.html#interrupt-handling
        public int ExecuteInterrupt(byte flag)
        {
            if (!isHalted && !isMasterEnabled)
                return 0;

            isHalted = false;
            
            // no nested interrupts
            if (!isMasterEnabled)
                return 0;

            isMasterEnabled = false;
            PushStack16(registers.pc);
            // 0x40, 0x48, 0x50, 0x58, 0x60
            registers.pc = (ushort) (0x40 + 8 * flag);
            bus.IF = Tools.ClearBit(bus.IF, flag);

            // In theory, these operations should take 5 cycles
            // I saw a mention about this in pandocs and I didn't see other
            // emulators using this, but I'll keep it to see how it goes
            return 5;
        }

        public int DoStep()
        {
            int cycles = 0;
            
            // Serve Interrupts
            // only if is currently in master execution. This is the default
            // console behavior, programmers could override the registers to 
            // do nested interruptions, but it's not a default  hardware feature
            // if IE & IF != 0 then it means an interrupt was requested
            // there are 5 interrupt bit flags
            // read here: https://gbdev.io/pandocs/Interrupts.html
            for (byte i = 0; i <= 4; i++)
            {
                // if it's enabled AND requested
                // if multiple interrupts are requested and enabled, 
                // priority goes from bit 0 to bit 5, in that order
                bool shouldServe = (byte) (((bus.IE & bus.IF) >> i) & 0x1) != 0;
            
                if (shouldServe)
                {
                    cycles += ExecuteInterrupt(i);
                }
            }
  
            // fetch next instruction
            opPC = registers.pc;

            // NOP by default (for instance if it's halted)
            byte currentOperationCode = 0x00;

            if (!isHalted)
            {
                // could use GetD8 but I'm trying to optimize code
                // in fact, I'll delete GetD8 and Get16 once the code is working as expected
                currentOperationCode = bus.Read8(registers.pc);

                if (haltBug)
                {
                    registers.pc--;
                    haltBug = false;
                }
                
                // halt bug
                if (stopCounter)
                    stopCounter = false;
                else
                    registers.pc++;
            }
            
            qtyInstructions++;

            // if (currentOperationCode == _gb.breakCode && _gb.shouldBreakOnCode)
            //     _gb.useManualSteps = true;

            // set debug operations
            // these are pretty slow, so we only want them to execute if we set this
            // boolean as true in the Editor Inspector
            if (showDebugOperations)
            {
                for (int i = debugOperations.Length - 1; i > 0 ; i--)
                {
                    debugOperations[i] = DebugInstructions.Get(_gb, debugOperations[i - 1].address);
                }
                debugOperations[0] = DebugInstructions.Get(_gb, opPC);
            
                int futureBytes = debugOperations[0].length;
            
                for (int i = debugOperationsFuture.Length - 1; i >= 0 ; i--)
                {
                    debugOperationsFuture[i] = DebugInstructions.Get(_gb, (ushort) (debugOperations[0].address + futureBytes ));
            
                    futureBytes += debugOperationsFuture[i].length;
                }

            }

            currentOp = OperationsMap.map[currentOperationCode];

            if (currentOp == null)
                throw new Exception($"Op not Found: {Tools.HexString(currentOperationCode, 2)}");
            
            // execute instruction
            cycles = processor.Execute(currentOp.code);
            return cycles;
        }
        
        // -------------------------------- CPU OPERATIONS ------------------

        // read the immediate 8 bits value in the next pc address
        // this is the "hardcoded" number that comes with an instruction
        public byte GetD8()
        {
            byte val = bus.Read8(registers.pc);
            registers.pc++;
            return val;
        }
        
        // read the immediate 16 bits value in the next pc addresses
        // this is the "hardcoded" number that comes with an instruction
        public ushort GetD16()
        {
            ushort val = bus.Read16(registers.pc);
            registers.pc += 2;
            return val;
        }
        
        public void PushStack8(byte value)
        {
            bus.Write8(--registers.sp, value);
        }
        
        public byte PopStack8()
        {
            return bus.Read8(registers.sp++);
        }
        
        public void PushStack16(ushort value)
        {
            byte low = (byte)(value & 0xFF);
            byte hi  = (byte)(value >> 8 & 0xFF);

            bus.Write8(--registers.sp, hi);
            bus.Write8(--registers.sp, low);
        }
        
        public ushort PopStack16()
        {
            byte lo = bus.Read8(registers.sp++);
            byte hi = bus.Read8(registers.sp++);
            
            return (ushort) ( (hi << 8) | lo );
        }

        public void DoHalt()
        {
            isHalted = true;
        }
        
        // arithmetics (alu) happens in the A register, 
        // so that's why you will see a value being added to A
        public void DoADC(ushort value)
        {
            byte carryBit = (byte) (registers.flagCarry ? 1 : 0);
            
            byte a = registers.a;
            byte sum = (byte) (value + a + carryBit);

            registers.flagZero = sum == 0;
            registers.flagSubtraction = false;
            registers.flagHalfCarry = ((a & 0xF) + (value & 0xF) + carryBit) > 0xF;
            // don't use sum here, because we need to overflow
            registers.flagCarry = (a + value + carryBit) > 0xFF; 

            registers.a = sum;
        }
        
        // arithmetics (alu) happens in the A register, 
        // so that's why you will see a value being added to A
        public void DoAdd(byte b)
        {
            int sum = registers.a + b;
            // Z 0 H C
            registers.flagZero = (sum & 0xFF) == 0;
            registers.flagSubtraction = false;
            registers.flagHalfCarry = (registers.a & 0xF) + (b & 0xF) >= 0x10;
            registers.flagCarry = sum >= 0x100;

            registers.a = (byte)(sum & 0xFF);
        }
        
        // while A is used for arithmetics, HL is used in similar way
        // when the Add is 16 bits
        public void DoAddToHL(ushort b)
        {
            int sum = registers.hl + b;
            
            registers.flagSubtraction = false;
            registers.flagHalfCarry = ( registers.hl & 0xFFF) + (b & 0xFFF) >= 0x1000;
            registers.flagCarry = sum >> 16  != 0;
            
            registers.hl = (ushort)(sum & 0xFFFF);
        }
        
        public void DoAnd(byte other)
        {
            registers.a = (byte)(registers.a & other);
            registers.flagZero = registers.a == 0;
            registers.flagSubtraction = false;
            registers.flagHalfCarry = true;
            registers.flagCarry = false;
        }
        
        public void DoCP(byte value)
        {
            int n = registers.a - value;

            registers.flagZero = n == 0;
            registers.flagSubtraction = true;
            registers.flagHalfCarry = (registers.a & 0x0F) < (value & 0x0F) ;
            registers.flagCarry = n < 0;
        }
        
        public byte DoDEC(ushort value)
        {
            int result = value - 1;
            
            registers.flagZero = result == 0;
            registers.flagSubtraction = true;
            registers.flagHalfCarry = (value & 0xF) < 1;

            return  (byte) result;
        }
        
        public void CheckFlags_INC(ushort value)
        {
            registers.flagZero = value == 0;
            registers.flagSubtraction = false;
            registers.flagHalfCarry = ((value - 1) & 0xF) + (1 & 0xF) > 0xF;
        }
        
        public bool DoJR(bool condition)
        {
            int r8 = GetD8();
            
            if (condition)
            {
                // needs to be casted to signed byte, because the relative displacement
                // can be negative
                registers.pc = (ushort) (registers.pc + (sbyte) r8);
                
                return true;
            }
            
            return false;
        }
        
        public void DoOR(byte other)
        {
            registers.a = (byte)(registers.a | other);
            registers.flagZero = registers.a == 0;
            registers.flagSubtraction = false;
            registers.flagHalfCarry = false;
            registers.flagCarry = false;
        }
        
        // sub with carry
        public void DoSBC(byte value)
        {
            byte carryBit = (byte) (registers.flagCarry ? 0 : 1);
            value = (byte) ~value;

            byte result = (byte)(registers.a + carryBit + value);

            registers.flagZero = result == 0;
            registers.flagSubtraction = true;
            registers.flagHalfCarry = ! (((registers.a & 0xF) + (value & 0xF) + carryBit) > 0xF);
            registers.flagCarry = ! ((registers.a + value + carryBit) > 0xFF);

            registers.a = (byte) (result & 0xFF);
        }
        
        // arithmetics (alu) happens in the A register, 
        // so that's why you will see a value being sub to A
        public void DoSub(byte value)
        {
            int result = registers.a - value;

            registers.flagZero = result == 0;
            registers.flagSubtraction = true;
            registers.flagHalfCarry =  (value & 0x0F) > (registers.a & 0x0F);
            registers.flagCarry = value > registers.a;

            registers.a = (byte) result;
        }
        
        public void DoXOR(byte fromValue)
        {
            registers.a = (byte)(registers.a ^ fromValue);

            registers.flagZero = registers.a == 0;
            registers.flagSubtraction = false;
            registers.flagHalfCarry = false;
            registers.flagCarry = false;
        }

        // if condition applies, jump to the address 
        // specified in the immediate data, interpreted as 16 bit address
        public bool DoJumpToA16(bool condition)
        {
            ushort address = GetD16();
            
            // we can't call DoJump because we would need to move the 
            // pc -2 due the Get16() above. I mean.. I could do it differently 
            // but this works :p
            if (condition)
            {
                registers.pc = address;
                return true;
            }

            return false;
        }

        // this is basically invoking a Function, so you save the 
        // return address in the stack
        public bool DoCall(bool condition)
        {
            ushort address =  GetD16();
            
            if (condition)
            {
                PushStack16(registers.pc );
                registers.pc = address;
                return true;
            }

            return false;
        }

        public bool DoReturn(bool condition)
        {
            if (condition)
            {
                registers.pc = PopStack16();
                
                return true;

            }

            return false;
        }
        

        // executes a reset in the program, forcing a jump to a specific position
        // The stack seems to be saved however, I'd need to read more about this one
        public void DoRst(byte value)
        {
            PushStack16(registers.pc);
            registers.pc = value;
        }

        public ushort DoDisplaceR8()
        {
            ushort r8 = GetD8();
            ushort sp = registers.sp;

            registers.flagZero = false;
            registers.flagSubtraction = false;
            registers.flagHalfCarry = (sp & 0xF) + (r8 & 0xF) > 0xF;
            registers.flagCarry = (sp & 0xFF) + (r8 & 0xFF) > 0xFF;

            // IMPORTANT!
            // r8 is a displacement so it should be interpreted
            // as a signed byte
            return (ushort)(sp + (sbyte)r8);
        }
        
        // cb operations
        
        public byte RLC(byte value) 
        {
            byte result = (byte)((value << 1) | (value >> 7));

            registers.flagZero = result == 0;
            registers.flagSubtraction = false;
            registers.flagHalfCarry = false;
            registers.flagCarry = (value & 0x80) != 0;

            return result;
        }
        
        public byte RRC(byte value) 
        {
            byte result = (byte)((value >> 1) | (value << 7));
            
            registers.flagZero = result == 0;
            registers.flagSubtraction = false;
            registers.flagHalfCarry = false;
            registers.flagCarry = (value & 0x1) != 0;

            return result;
        }
        
        public byte SET(byte value, byte reg) 
        {
            return (byte)(reg | value);
        }

        public byte RES(int value, byte reg) 
        {
            return (byte)(reg & ~value);
        }

        public void BIT(byte value, byte reg) 
        {
            registers.flagZero = (reg & value) == 0;
            registers.flagSubtraction = false;
            registers.flagHalfCarry = true;
        }

        public byte SRL(byte value) 
        {
            byte result = (byte)(value >> 1);
            registers.flagZero = result == 0;
            registers.flagSubtraction = false;
            registers.flagHalfCarry = false;
            registers.flagCarry = (value & 0x1) != 0;
            return result;
        }

        public byte SWAP(byte value) 
        {
            byte result = (byte)((value & 0xF0) >> 4 | (value & 0x0F) << 4);
            
            registers.flagZero = result == 0;
            registers.flagSubtraction = false;
            registers.flagHalfCarry = false;
            registers.flagCarry = false;

            return result;
        }

        public byte SRA(byte value) 
        {
            byte result = (byte)((value >> 1) | ( value & 0x80));
            
            registers.flagZero = result == 0;
            registers.flagSubtraction = false;
            registers.flagHalfCarry = false;
            registers.flagCarry = (value & 0x1) != 0;
            
            return result;
        }

        public byte SLA(byte value)
        {
            byte result = (byte)(value << 1);
            
            registers.flagZero = result == 0;
            registers.flagSubtraction = false;
            registers.flagHalfCarry = false;
            registers.flagCarry = (value & 0x80) != 0;

            return result;
        }

        public byte RR(byte value) 
        {
            byte result = (byte)((value >> 1) | (registers.flagCarry ? 0x80 : 0));
            
            registers.flagZero = result == 0;
            registers.flagSubtraction = false;
            registers.flagHalfCarry = false;
            registers.flagCarry = (value & 0x1) != 0;
            
            return result;
        }

        public byte RL(byte value) 
        {
            byte result = (byte)((value << 1) | (registers.flagCarry ? 1 : 0));
            
            registers.flagZero = result == 0;
            registers.flagSubtraction = false;
            registers.flagHalfCarry = false;
            registers.flagCarry = (value & 0x80) != 0;
            
            return result;
        }
        
        
    }
}