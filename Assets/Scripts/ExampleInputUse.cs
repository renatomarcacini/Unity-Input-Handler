using UnityEngine;

public class ExampleInputUse : MonoBehaviour
{
    [SerializeField] private Vector2 leftAnalog;
    [SerializeField] private Vector2 rightAnalog;

    void Update()
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
