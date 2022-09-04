using System;
using System.Collections;
using Emulator.Cart;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Drongo.GameboyEmulator
{
    
    public class GameBoy : MonoBehaviour
    {
        [SerializeField]
        private string _romNameToLoad;

        [BoxGroup("Debug")]
        public bool shouldBreakOnCode;
        
        [BoxGroup("Debug")]
        public string breakOnCode;
        
        [BoxGroup("Debug")]
        public bool useManualSteps;

        [ShowInInspector]
        public CPU cpu;
        public CartridgeRom cart;
        public AddressBus bus;
        public PPU ppu;
        public Timer timer;
        public JoyPad _joypad;

        private long _cycles;

        public bool buttonStepRequested;

        public int timeMultiplier = 10;

        [Button]
        private void RequestStep()
        {
            buttonStepRequested = true;
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            
            Initialize(_romNameToLoad);
            
            StartCoroutine(EmulationLoop());
        }

        public void Initialize(string romName)
        {
            bus = new AddressBus(this);
            ppu = new PPU(this);
            timer = new Timer();
            cart = new RomMBC1();
            cart.Load(romName);

            cpu = new CPU(this);
            _joypad = new JoyPad(bus);
            
            StartUp();
        }
        
        private void StartUp()
        {
            // grabbed from here
            https://bgb.bircd.org/pandocs.htm#powerupsequence
            
            cpu.registers.af = 0x01B0;
            cpu.registers.bc = 0x0013;
            cpu.registers.de = 0x00D8;
            cpu.registers.hl = 0x014D;
            
            cpu.registers.sp = 0xFFFE;
            cpu.registers.pc = 0x0100;

            bus.Write8(0xFF05, 0x00); // TIMA
            bus.Write8(0xFF06, 0x00); // TMA
            bus.Write8(0xFF07, 0x00); // TAC
            bus.Write8(0xFF10, 0x80); // NR10
            bus.Write8(0xFF11, 0xBF); // NR11
            bus.Write8(0xFF12, 0xF3); // NR12
            bus.Write8(0xFF14, 0xBF); // NR14
            bus.Write8(0xFF16, 0x3F); // NR21
            bus.Write8(0xFF17, 0x00); // NR22
            bus.Write8(0xFF19, 0xBF); // NR24
            bus.Write8(0xFF1A, 0x7F); // NR30
            bus.Write8(0xFF1B, 0xFF); // NR31
            bus.Write8(0xFF1C, 0x9F); // NR32
            bus.Write8(0xFF1E, 0xBF); // NR33
            bus.Write8(0xFF20, 0xFF); // NR41
            bus.Write8(0xFF21, 0x00); // NR42
            bus.Write8(0xFF22, 0x00); // NR43
            bus.Write8(0xFF23, 0xBF); // NR30
            bus.Write8(0xFF24, 0x77); // NR50
            bus.Write8(0xFF25, 0xF3); // NR51
            bus.Write8(0xFF26, 0xF1); // NR52
            bus.Write8(0xFF40, 0x91); // lcdControl
            bus.Write8(0xFF41, 0x85); // lcdStat
            bus.Write8(0xFF42, 0x00); // scy
            bus.Write8(0xFF43, 0x00); // scx
            bus.Write8(0xFF44, 0x00); // LY
            bus.Write8(0xFF45, 0x00); // LYC
            bus.Write8(0xFF47, 0xFC); // BGP
            bus.Write8(0xFF48, 0xFF); // OBP0
            bus.Write8(0xFF49, 0xFF); // OBP1
            bus.Write8(0xFF4A, 0x00); // WY
            bus.Write8(0xFF4B, 0x00); // WX
            bus.Write8(0xFF0F, 0xE1); // IF
            bus.Write8(0xFFFF, 0x00); // IE
        }
        

        private IEnumerator EmulationLoop()
        {
            int cycles = 0;
            
            while (true)
            {
                // a frame lasts 70224 _cycles 
                // (154 scan-lines, 456 _cycles each)
                
                // STEP:
                while (_cycles < 70224 * timeMultiplier)
                {
                    if (useManualSteps)
                    {
                        yield return new WaitUntil(() => buttonStepRequested);
                        buttonStepRequested = false;
                    }
                    
                    int stepCycles = cpu.DoStep();
                    
                    // something went wrong or stop was called
                    if(stepCycles < 0)
                        yield break;

                    if (stepCycles == 0)
                    {
                        continue;
                    }

                    timer.Update(stepCycles, bus); 
                    _joypad.Update();
                    ppu.Update(stepCycles, bus);

                    _cycles += stepCycles;
                }
                
                // not 0 because _cycles may be bigger instead of equal
                _cycles -= 70224 * timeMultiplier;

                yield return null;
            }
        }
    }
}