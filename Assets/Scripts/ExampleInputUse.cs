using UnityEngine;

public class ExampleInputUse : MonoBehaviour
{

    void Update()
    {
        if (InputHandler.KeyDown(INPUT.KeyButtonSouth))
            Debug.Log("BUTTON SOUTH KEY DOWN");

        if (InputHandler.KeyUp(INPUT.KeyButtonSouth))
            Debug.Log("BUTTON SOUTH KEY UP");

        if (InputHandler.KeyHeld(INPUT.KeyButtonSouth))
            Debug.Log("BUTTON SOUTH KEY HELD");
    }
}
