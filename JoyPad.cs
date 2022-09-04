using Drongo.GameboyEmulator.Utils;

namespace Drongo.GameboyEmulator
{
    // the old good pandocs for this
    // https://gbdev.io/pandocs/Joypad_Input.html
    public class JoyPad
    {
        private const int JOYPAD_INTERRUPT = 4;
        private const byte PAD_MASK = 0x10;
        private const byte BUTTON_MASK = 0x20;
        private byte pad = 0xF;
        private byte buttons = 0xF;

        private AddressBus _bus;
        
        public JoyPad(AddressBus bus)
        {
            _bus = bus;
            PlayerInputComponent.onKeyPressed += OnKeyPressed;
            PlayerInputComponent.onKeyReleased += OnKeyReleased;
        }

        private void OnKeyPressed(byte bit)
        {
            if ((bit & PAD_MASK) == PAD_MASK) 
            {
                pad = (byte)(pad & ~(bit & 0xF));
            } 
            else if((bit & BUTTON_MASK) == BUTTON_MASK) 
            {
                buttons = (byte)(buttons & ~(bit & 0xF));
            }
        }

        private void OnKeyReleased(byte bit)
        {
            if ((bit & PAD_MASK) == PAD_MASK) 
            {
                pad = (byte)(pad | (bit & 0xF));
            } 
            else if ((bit & BUTTON_MASK) == BUTTON_MASK)
            {
                buttons = (byte)(buttons | (bit & 0xF));
            }
        }

        public void Update()
        {
            if(Tools.GetBIT(_bus.joyPad, 4) == 0)
            {
                _bus.joyPad = (byte)((_bus.joyPad & 0xF0) | pad);
                if(pad != 0xF) 
                    _bus.RequestInterrupt(JOYPAD_INTERRUPT);
            }
            
            if (Tools.GetBIT(_bus.joyPad, 5) == 0) 
            {
                _bus.joyPad = (byte)((_bus.joyPad & 0xF0) | buttons);
                if (buttons != 0xF) 
                    _bus.RequestInterrupt(JOYPAD_INTERRUPT);
            }
            
            if ((_bus.joyPad & 0b00110000) == 0b00110000) 
                _bus.joyPad = 0xFF;
        }
    }
}