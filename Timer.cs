using System.IO;
using UnityEngine;

namespace Drongo.GameboyEmulator
{
    /// <summary>
    /// More info about the Timer and Divider Registers can
    /// be found here
    /// https://gbdev.io/pandocs/Timer_and_Divider_Registers.html
    /// </summary>
    public class Timer
    {
        private const int DIV_FREQ = 256; //  16384Hz;
        
        private const byte TIMER_INTERRUPT = 2;
        // 0: cpu clock / 1024 (4096 hz)
        // 1: cpu clock / 16 (262144 hz)
        // 2: cpu clock / 64 (65536 hz)
        // 3: cpu clock / 256 (16384 hz)
        private static int[] TAC_FREQ = { 1024, 16, 64, 256 };

        private const ushort REG_DIV = 0xFF04;
        private const ushort REG_TIMA = 0xFF05;
        private const ushort REG_TMA = 0xFF06;
        private const ushort REG_TAC = 0xFF07;
        
        private int _divCounter;
        private int _timerCounter;
        
        public void Update(int cycles, AddressBus bus)
        {
            // increment divider
            // <explanation>
            // each cpu cycle will increment also the divider counter
            // each time the div counter reaches its frequency value, 
            // it gets reset back and counts again (with mod values, not to 0)
            // each time the frequency is reached (256 cycles at 16384hz), then
            // the register 0xFF04 (io DIV) is incremented
            _divCounter += cycles;
            while (_divCounter >= DIV_FREQ)
            {
                // increase io DIV register at FF04
                byte divVal = (byte) (bus.Read8(REG_DIV) + 1);
                bus.Write8(REG_DIV, divVal); 
                
                _divCounter -= DIV_FREQ;
            }

            // handle timer

            // if byte 2 of 0XFF07 is 1, then tac is enabled
            // TAC Register (Timer Control) as 0xFF07
            // if enabled, then TIMA counting is also enabled.
            // DIV counting is always enabled though
            bool tacEnabled = (bus.Read8(REG_TAC) & 0x4) != 0;

            if (tacEnabled)
            {
                // FF05: TIMA Timer Counter
                //       Increments the clock at the frequency specified by TAC
                //       When this value overflows it gets reset to the value 
                //       specified in TMA 
                    
                // FF06: TMA Timer Module
                //       value used as reset value for the TIMA register (read above).
                //       when this happens, an interrupt is requested at the clock
                //       frequency specified by TAC
                
                _timerCounter += cycles;

                // bytes 0 and 1 returns value from 0 to 3
                // used to map the taq frequency
                int taqFreq = (bus.Read8(REG_TAC) & 0x3);
                while (_timerCounter >= TAC_FREQ[taqFreq])
                {
                    byte divVal = (byte) (bus.Read8(REG_TIMA) + 1);
                    bus.Write8(REG_TIMA, divVal);
                    _timerCounter -= TAC_FREQ[taqFreq];
                }

                if (bus.Read8(REG_TIMA) == 0xFF)
                {
                    bus.RequestInterrupt(TIMER_INTERRUPT);
                    bus.Write8(REG_TIMA, bus.Read8(REG_TMA));
                }
            }
        }
    }
}