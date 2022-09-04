using UnityEngine;

// the old good pandocs for this
// https://gbdev.io/pandocs/Joypad_Input.html
public class PlayerInputComponent : MonoBehaviour
{
    public delegate void keyChanged(byte bit);

    public static keyChanged onKeyPressed = delegate(byte bit) {  };
    public static keyChanged onKeyReleased = delegate(byte bit) {  };

    private void Update()
    {
        CheckPressDown();
        CheckPressUp();
    }

    private void CheckPressDown()
    {
        // directions pad
        if (Input.GetKeyDown(KeyCode.D))
            onKeyPressed(0b00010001);
        
        if (Input.GetKeyDown(KeyCode.A))
            onKeyPressed(0b00010010);
        
        if (Input.GetKeyDown(KeyCode.W))
            onKeyPressed(0b00010100);
        
        if (Input.GetKeyDown(KeyCode.S))
            onKeyPressed(0b00011000);
        
        // action buttons
        if (Input.GetKeyDown(KeyCode.K))
            onKeyPressed(0b00100001);
        
        if (Input.GetKeyDown(KeyCode.J))
            onKeyPressed(0b00100010);
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
            onKeyPressed(0b00100100);
        
        if (Input.GetKeyDown(KeyCode.Return))
            onKeyPressed(0b00101000);
    }
    
    private void CheckPressUp()
    {
        // directions pad
        if (Input.GetKeyUp(KeyCode.D))
            onKeyReleased(0b00010001);
        
        if (Input.GetKeyUp(KeyCode.A))
            onKeyReleased(0b00010010);
        
        if (Input.GetKeyUp(KeyCode.W))
            onKeyReleased(0b00010100);
        
        if (Input.GetKeyUp(KeyCode.S))
            onKeyReleased(0b00011000);
        
        // action buttons
        if (Input.GetKeyUp(KeyCode.K))
            onKeyReleased(0b00100001);
        
        if (Input.GetKeyUp(KeyCode.J))
            onKeyReleased(0b00100010);
        
        if (Input.GetKeyUp(KeyCode.LeftShift))
            onKeyReleased(0b00100100);
        
        if (Input.GetKeyUp(KeyCode.Return))
            onKeyReleased(0b00101000);
    }
}
