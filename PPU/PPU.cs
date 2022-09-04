using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Tools = Drongo.GameboyEmulator.Utils.Tools;

namespace Drongo.GameboyEmulator
{
    /// <summary>
    /// some good documentation about how to implement the PPU
    /// https://hacktix.github.io/GBEDG/ppu/
    /// </summary>
    public class PPU
    {
        public const int SCREEN_WIDTH = 160;
        public const int SCREEN_HEIGHT = 144;
        private const int SCREEN_VBLANK_HEIGHT = 154;
        
        
        private const int MODE_2_OAM_CYCLES = 80;
        private const int MODE_3_DRAWING_CYCLES = 172;
        private const int MODE_0_HBLANK_CYCLES = 204;
        private const int MODE1_VBLANK_CYCLES = 456;
        

        private const int VBLANK_INTERRUPT = 0;
        private const int LCD_INTERRUPT = 1;
        
        //PPU IO Regs

        //FF48 - OBP0 - Object Palette 0 Data (R/W) - Non CGB Mode Only
        //FF49 - OBP1 - Object Palette 1 Data (R/W) - Non CGB Mode Only
        public byte OBP0 => _bus.Read8(0xFF48);
        public byte OBP1 => _bus.Read8(0xFF49);

        public readonly int[,] pixels;

        private int _scanlineCounter;

        private AddressBus _bus;

        public PPU(GameBoy gb)
        {
            _bus = gb.bus;
            
            pixels = new int[SCREEN_WIDTH, SCREEN_HEIGHT];
            
            // the initial scanline mode is 0 (h blank)
            // in this way the first time it reaches the end of mode 0 cycle,
            // it comes back to the beginning and ly = 1
            SetScanMode(0);
        }
        
        public void Update(int cycles, AddressBus bus)
        {
            _scanlineCounter += cycles;

            // last 3 bits of that address value will give you
            // the Mask Mode
            byte scanMode = (byte)(bus.lcdStat & 0x3);
            
            bool isLCDEnabled = Tools.GetBIT(bus.lcdControl, 7) == 1;
            
            // TODO: assume always on?
            if (isLCDEnabled || true)
            {
                switch (scanMode)
                {
                    // Scan Part 1: MODE 2
                    // OAM Access (80 cycles)
                    
                    // it's the first mode used in a scanline, it searches for 
                    // objects in the OAM and saved them in a buffer (the FIFO registers) 
                    // Since we dont' really need to buffer to show sprites on screen,
                    // we only simulate the timing it takes to do this process
                    // this is because we can just grab them from the right place in the mode 3
                    // but in the actual device, they will be fetched in mode 2 into the FIFO
                    // and the next mode would grab them and draw them
                    case 2:
                        
                        if (_scanlineCounter >= MODE_2_OAM_CYCLES) 
                        {
                            // when object search finishes after 80 cycles
                            // we should go to the next mode (drawing)
                            SetScanMode(3);
                            _scanlineCounter -= MODE_2_OAM_CYCLES;
                        }
                        
                        break;
                    
                    // Scan Part 2: DRAWING
                    // VRAM Access (172/289 cycles)
                    
                    // Second part of a scan line. It takes the sprites from the buffer and 
                    // draws them on the screen. It takes from 172 to 289 cycles, 
                    // however, we don't really need to emulate that difference
                    case 3:
                        if (_scanlineCounter >= MODE_3_DRAWING_CYCLES) 
                        {
                            // when the drawing finishes after 172/289 cycles
                            // we should go to the next mode (h-blank)
                            DrawScanLine();
                            SetScanMode(0);
                            _scanlineCounter -= MODE_3_DRAWING_CYCLES;
                        }
                        
                        break;
                    
                    // Scan Part 3: H-BLANK
                    // Horizontal BLANK (87/204 cycles)
                    
                    // Last part of an "actually rendered" line (see mode 1 to understand this).
                    // This part does not do anything, it's a padding of time without logic that
                    // ensure the whole scan-line takes a total of 456 cycles.
                    case 0:
                        
                        if (_scanlineCounter >= MODE_0_HBLANK_CYCLES)
                        {
                            // when the scan-line finishes, we increment the line index
                            // if the line index reached the screen height limit, then 
                            // we enter into the v-blank mode and also we render the frame data
                            
                            // otherwise, we go back to mode 2 and draw a new line 
                            
                            _bus.ly++;
                            _scanlineCounter -= MODE_0_HBLANK_CYCLES;
                            
                            if (_bus.ly >= SCREEN_HEIGHT) 
                            {
                                SetScanMode(1);
                                _bus.RequestInterrupt(VBLANK_INTERRUPT);
                                RenderFrame();
                            } 
                            else 
                            { 
                                SetScanMode(2);
                            }
                        }

                        break;
                    
                    // Scan Part 4 (vertical padding)
                    // V-BLANK (4560 cycles - 10 lines) 
                    // To ensure the render of a whole frame takes a total of 70224 cycles,
                    // 10 blank scan-lines are executed that don't do anything. 
                    // As mentioned above, each line takes 456 cycles
                    case 1:
                        
                        if (_scanlineCounter >= MODE1_VBLANK_CYCLES)
                        {
                            _bus.ly++;
                            _scanlineCounter -= MODE1_VBLANK_CYCLES;
                            
                            if (_bus.ly >= SCREEN_VBLANK_HEIGHT)
                            { 
                                // go back to the first line and restart 
                                // drawing the new frame
                                _bus.ly = 0;
                                SetScanMode(2);
                            }
                        }
                        
                        break;
                }

                // TODO: not critical, probably not even used.....
                // if (bus.ly == bus.lyCompare)
                // {
                //    
                //     _bus.lcdStat = Tools.SetBit(_bus.lcdStat, 2);
                //     if (Tools.GetBIT(_bus.lcdStat, 6) != 0)
                //     {
                //         _bus.RequestInterrupt(LCD_INTERRUPT);
                //     }
                // }
                // else
                // {
                //     _bus.lcdStat = Tools.ClearBit(_bus.lcdStat, 2);
                // }
            }
            // if LCD is disabled 
            else
            {
                _scanlineCounter = 0;
                // reset ly
                bus.ly = 0;
                // set mask mode to 3
                bus.lcdStat = (byte)(bus.lcdStat & ~0x3);
            }
        }
        
        private void SetScanMode(int mode) 
        {
            byte maskMode = (byte)(_bus.lcdStat & ~0x3);
            _bus.lcdStat = (byte)(maskMode | mode);
            
            //HBLANK 
            if (mode == 0 && Tools.GetBIT(maskMode, 3) != 0)
            {
                // Bit 3 - Mode 0 H-Blank Interrupt     (1=Enable) (Read/Write)
                _bus.RequestInterrupt(LCD_INTERRUPT);
            }
            //VBLANK 
            else if (mode == 1 && Tools.GetBIT(maskMode, 4) != 0) 
            { // Bit 4 - Mode 1 V-Blank Interrupt     (1=Enable) (Read/Write)
                _bus.RequestInterrupt(LCD_INTERRUPT);
            }
            //Accessing OAM 
            else if (mode == 2 && Tools.GetBIT(maskMode, 5) != 0)
            { // Bit 5 - Mode 2 OAM Interrupt         (1=Enable) (Read/Write)
                _bus.RequestInterrupt(LCD_INTERRUPT);
            }
        }

        private void RenderFrame()
        {
        }

        private void DrawScanLine()
        {
            bool isBGDisplayOn = Tools.GetBIT(_bus.lcdControl, 0) == 1;
            if (isBGDisplayOn)
            {
                // background rendering has 2 layer: window and background
                bool isWindow = Tools.GetBIT(_bus.lcdControl, 5) != 0 && (_bus.wy <= _bus.ly);

                if (isWindow)
                {
                    RenderWindow();
                }
                else
                {
                    RenderBG();    
                }
            }
            
            bool isOBJSpriteOn = Tools.GetBIT(_bus.lcdControl, 1) == 1;
            if (isOBJSpriteOn)
            {
                RenderSprites();
            }
        }

        private void RenderWindow()
        {
            // caching some getters for performance
            byte wx = (byte)(_bus.wx - 7);
             
            byte scx = _bus.scx;
            byte lcdControl = _bus.lcdControl;
            byte bgPalette = _bus.bgPalette;

            byte y = (byte) (_bus.ly - _bus.wy);
            byte tileLine = (byte)((y & 7) * 2);
            
            ushort tileRow = (ushort)(y / 8 * 32);
            //Bit 3 - BG Tile Map Display Select     (0=9800-9BFF, 1=9C00-9FFF)
            ushort tileMap = Tools.GetBIT(lcdControl, 6) != 0 ? (ushort)0x9C00 : (ushort)0x9800;
                
            byte hi = 0;
            byte lo = 0;
            
            for (int p = 0; p < SCREEN_WIDTH; p++)
            {
                byte x = p >= wx ? (byte)(p - wx) : (byte) (p + scx);

                if ((p & 0x7) == 0 || ((p + scx) & 0x7) == 0) 
                {
                    ushort tileCol = (ushort)(x / 8);
                    ushort tileAddress = (ushort)(tileMap + tileRow + tileCol);

                    //Bit 4 - BG & Window Tile Data Select   (0=8800-97FF, 1=8000-8FFF)
                    ushort tileLoc = Tools.GetBIT(lcdControl, 4) != 0 ? (ushort)0x8000 : (ushort)0x8800; //0x8800 signed area 
                    byte vramData = _bus.Read8(tileAddress);
                    // if it's signed address
                    // Bit 4 - BG & Window Tile Data Select   (0=8800-97FF, 1=8000-8FFF)
                    if (Tools.GetBIT(lcdControl, 4) != 0) 
                    {
                        tileLoc += (ushort)(vramData * 16);
                    } else 
                    {
                        tileLoc = (ushort)(((sbyte)vramData + 128) * 16);
                    }

                    lo = _bus.Read8((ushort)(tileLoc + tileLine));
                    hi = _bus.Read8((ushort)(tileLoc + tileLine + 1));
                }

                int colorBit = 7 - (x & 7); //inversed
                
                // annoying I can't create functions for things like this that are 
                // repeated, but Unity doesn't support inlining booooo
                int colHi = (hi >> colorBit) & 0x1 ;
                int colLo = (lo >> colorBit) & 0x1;
                int colorId = (colHi << 1 | colLo);

                pixels[p, _bus.ly] = (bgPalette >> colorId * 2) & 0x3;
            }
            
        }

        private void RenderBG()
        {
            // caching some getters for performance
            byte scx = _bus.scx;
            byte lcdControl = _bus.lcdControl;
            byte bgPalette = _bus.bgPalette;
            
            
            byte y = (byte) (_bus.ly + _bus.scy);
            byte tileLine = (byte)((y & 7) * 2);
            
            ushort tileRow = (ushort)(y / 8 * 32);
            //Bit 3 - BG Tile Map Display Select     (0=9800-9BFF, 1=9C00-9FFF)
            ushort tileMap = Tools.GetBIT(lcdControl, 3) != 0 ? (ushort)0x9C00 : (ushort)0x9800;
                
            byte hi = 0;
            byte lo = 0;
            
            for (int p = 0; p < SCREEN_WIDTH; p++)
            {
                byte x = (byte)(p + scx);

                if ((p & 0x7) == 0 || ((p + scx) & 0x7) == 0) 
                {
                    ushort tileCol = (ushort)(x / 8);
                    ushort tileAddress = (ushort)(tileMap + tileRow + tileCol);

                    //Bit 4 - BG & Window Tile Data Select   (0=8800-97FF, 1=8000-8FFF)
                    ushort tileLoc = Tools.GetBIT(lcdControl, 4) != 0 ? (ushort)0x8000 : (ushort)0x8800; //0x8800 signed area 
                    byte vramData = _bus.Read8(tileAddress);
                    // if it's signed address
                    // Bit 4 - BG & Window Tile Data Select   (0=8800-97FF, 1=8000-8FFF)
                    if (Tools.GetBIT(lcdControl, 4) != 0) 
                    {
                        tileLoc += (ushort)(vramData * 16);
                    } else 
                    {
                        tileLoc = (ushort)(((sbyte)vramData + 128) * 16);
                    }

                    lo = _bus.Read8((ushort)(tileLoc + tileLine));
                    hi = _bus.Read8((ushort)(tileLoc + tileLine + 1));
                }

                int colorBit = 7 - (x & 7); //inversed
                
                // annoying I can't create functions for things like this that are 
                // repeated, but Unity doesn't support inlining booooo
                int colHi = (hi >> colorBit) & 0x1 ;
                int colLo = (lo >> colorBit) & 0x1;
                int colorId = (colHi << 1 | colLo);

                pixels[p, _bus.ly] = (bgPalette >> colorId * 2) & 0x3;
            }
        }

        private void RenderSprites()
        {
            byte ly = _bus.ly;
            byte bgPalette = _bus.bgPalette;

            // sprites can be height 8 or 16
            // but width only 8
            int spriteSize = Tools.GetBIT( _bus.lcdControl, 2) != 0 ? 16 : 8;
            
            for (int i = 0x9C; i >= 0; i -= 4)  
            { 
                //0x9F OAM Size, 40 Sprites x 4 bytes:
                int y = _bus.oam[i] - 16;    //Byte0 - Y Position //needs 16 offset
                int x = _bus.oam[i + 1] - 8; //Byte1 - X Position //needs 8 offset
                byte tile = _bus.oam[i + 2]; //Byte2 - Tile/Pattern Number
                byte attr = _bus.oam[i + 3]; //Byte3 - Attributes/Flags
                
                if (ly >= y && ly < y + spriteSize) 
                {
                    // Bit 6 is Y flipped?  (0: normal 1: flipped)                
                    bool isYFlipped = Tools.GetBIT(attr, 6) == 1;
                    int tileRow = isYFlipped ? spriteSize - 1 - (ly - y) : (ly - y);

                    ushort tileAddress = (ushort)(0x8000 + (tile * 16) + (tileRow * 2));
                    byte lo = _bus.vRam[tileAddress & 0x1FFF];
                    byte hi = _bus.vRam[(ushort)(tileAddress + 1) & 0x1FFF];

                    for (int p = 0; p < 8; p++) 
                    {
                        if (x + p >= 0 && x + p < SCREEN_WIDTH)
                        {
                            // Bit 5 is X flipped?  (0: normal 1: flipped)                
                            bool isXFlipped = Tools.GetBIT(attr, 5) == 1;
                            int colorBit = isXFlipped ? p : 7 - p;
                            // annoying I can't create functions for things like this that are 
                            // repeated, but Unity doesn't support inlining booooo
                            int colHi = (hi >> colorBit) & 0x1 ;
                            int colLo = (lo >> colorBit) & 0x1;
                            int colorId = (colHi << 1 | colLo);
                            
                            bool isPixelVisible = colorId != 0;
                            // Bit7 (0 = OBJ Above BG, 1 = OBJ Behind BG color 1 - 3)
                            bool isPixelAboveBG = attr >> 7 == 0;
                            
                            // get current BG color
                            byte whiteId = (byte) (bgPalette & 0x3);

                            bool isWhiteBG = pixels[x + p, _bus.ly] == whiteId;
                            
                            if (isPixelVisible && isPixelAboveBG || isWhiteBG) 
                            {
                                // Bit 4
                                //Palette number  **Non CGB Mode Only** (0=OBP0, 1=OBP1)
                                byte palette = Tools.GetBIT(attr, 4) == 1 ? _bus.objPalette0 : _bus.objPalette1; 
                                
                                pixels[x + p, _bus.ly] = (palette >> colorId * 2) & 0x3;
                            }
                        }
                    }
                }
            }   
        }
    }
}