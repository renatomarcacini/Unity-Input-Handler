# Unity Input Handler

The new Input System in Unity is an excellent solution for managing inputs across various platforms (keyboard, touch, joysticks). However, it is quite generic, which is both an advantage and a drawback. Often, it requires creating a new mapping for each project. Additionally, it does not easily capture events like KeyDown, KeyUp, and KeyHeld as the old Input System did. To address this, I have adapted the new Input System to work more like the old format, initially focusing on joystick inputs.

## Inputs Available
Here are the currently available inputs in this solution:

```c#
public enum INPUT
{
    KeyButtonNorth,
    KeyButtonSouth,
    KeyButtonWest,
    KeyButtonEast,
    ButtonRightShoulder,
    ButtonLeftShoulder,
    ButtonRightTrigger,
    ButtonLeftTrigger,
    Start,
    Select,
    UI_KeyButtonNorth,
    UI_KeyButtonSouth,
    UI_KeyButtonWest,
    UI_KeyButtonEast,
    UI_ButtonRightShoulder,
    UI_ButtonLeftShoulder,
    UI_ButtonRightTrigger,
    UI_ButtonLeftTrigger,
    UI_Start,
    UI_Select
}


[field: SerializeField] public static Vector2 LeftAnalog { get; private set; }
[field: SerializeField] public static Vector2 LeftAnalogRaw { get; private set; }
[field: SerializeField] public static Vector2 LeftAnalogNormalized { get; private set; }
[field: SerializeField] public static Vector2 LeftAnalogWithDeadzone { get; private set; }
[field: SerializeField] public static Vector2 LeftAnalogAngled { get; private set; }

[field: SerializeField] public static Vector2 RightAnalog { get; private set; }
[field: SerializeField] public static Vector2 RightAnalogRaw { get; private set; }
[field: SerializeField] public static Vector2 RightAnalogNormalized { get; private set; }
[field: SerializeField] public static Vector2 RightAnalogWithDeadzone { get; private set; }
[field: SerializeField] public static Vector2 RightAnalogAngled { get; private set; }
```
## Example Usage
Below is an example of how to use this adapted Input System:

```c#
public class ExampleInputUse : MonoBehaviour
{
    [SerializeField] private Vector2 leftAnalog;
    [SerializeField] private Vector2 rightAnalog;

    private void Update()
    {
        #region PLAYER_EXAMPLE
        if (InputHandler.KeyDown(INPUT.KeyButtonSouth))
            Debug.Log("PLAYER BUTTON SOUTH KEY DOWN");

        if (InputHandler.KeyUp(INPUT.KeyButtonSouth))
            Debug.Log("PLAYER BUTTON SOUTH KEY UP");

        if (InputHandler.KeyHeld(INPUT.KeyButtonSouth))
            Debug.Log("PLAYER BUTTON SOUTH KEY HELD");

        if (JumpInputAction())
            Debug.Log("PLAYER JUMP");
        #endregion

        #region UI_EXAMPLE
        if (InputHandler.KeyDown(INPUT.UI_KeyButtonSouth))
            Debug.Log("UI BUTTON SOUTH KEY DOWN");

        if (InputHandler.KeyUp(INPUT.UI_KeyButtonSouth))
            Debug.Log("UI BUTTON SOUTH KEY UP");

        if (InputHandler.KeyHeld(INPUT.UI_KeyButtonSouth))
            Debug.Log("UI BUTTON SOUTH KEY HELD");
        #endregion

        leftAnalog = InputHandler.LeftAnalogNormalized;
        rightAnalog = InputHandler.RightAnalog;
    }

    //Example of method 
    public bool JumpInputAction()
    {
        return InputHandler.KeyDown(INPUT.KeyButtonSouth);
    }
}
```
This example demonstrates how to handle key events and analog inputs using the new system. The InputHandler class is designed to provide easy access to these inputs, maintaining consistency with the old Input System's event handling.
