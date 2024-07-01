using Nato.Singleton;
using NatoGames.JoystickSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    UICancel,
    UISubmit,
}

public class InputHandler : Singleton<InputHandler>
{
    private bool isDestroyed = false;

    public static Dictionary<INPUT, bool> keyHeldInputs = new Dictionary<INPUT, bool>();
    public static Dictionary<INPUT, bool> keyDownInputs = new Dictionary<INPUT, bool>();
    public static Dictionary<INPUT, bool> keyUpInputs = new Dictionary<INPUT, bool>();

    [Header("Analog Settings")]
    [SerializeField] private float leftDeadzoneThreshold = 0.5f;
    [SerializeField] private float rightDeadzoneThreshold = 0.5f;

    [SerializeField] private int numAngles = 32;
    private float angleIncrement;
    private float[] angleDivisions;

    private PlayerAction playerAction;
    [SerializeField] private string[] actions;

    //Left Stick Analog
    private float leftStickHorizontal;
    private float leftStickVertical;
    private int leftStickHorizontalRaw;
    private int leftStickVerticalRaw;

    //Right Stick Analog
    private float rightStickHorizontal;
    private float rightStickVertical;
    private int rightStickHorizontalRaw;
    private int rightStickVerticalRaw;

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


    //Button North
    private static bool keyHeldButtonNorth;
    private static bool keyDownButtonNorth;
    private static bool keyUpButtonNorth;

    //Button South
    private static bool keyHeldButtonSouth;
    private static bool keyDownButtonSouth;
    private static bool keyUpButtonSouth;


    //Button West
    private static bool keyHeldButtonWest;
    private static bool keyDownButtonWest;
    private static bool keyUpButtonWest;


    //Button East
    private static bool keyHeldButtonEast;
    private static bool keyDownButtonEast;
    private static bool keyUpButtonEast;


    //Button Right Shoulder
    private static bool keyHeldRightShoulder;
    private static bool keyDownRightShoulder;
    private static bool keyUpRightShoulder;


    //Button Left Shoulder
    private static bool keyHeldLeftShoulder;
    private static bool keyDownLeftShoulder;
    private static bool keyUpLeftShoulder;


    //Button Right Trigger
    private static bool keyHeldRightTrigger;
    private static bool keyDownRightTrigger;
    private static bool keyUpRightTrigger;


    //Button LeftTrigger
    private static bool keyHeldLeftTrigger;
    private static bool keyDownLeftTrigger;
    private static bool keyUpLeftTrigger;


    //Button Start
    private static bool keyHeldStart;
    private static bool keyDownStart;
    private static bool keyUpStart;


    //Button Select
    private static bool keyHeldSelect;
    private static bool keyDownSelect;
    private static bool keyUpSelect;


    //Button Cancel
    private static bool keyHeldCancel;
    private static bool keyDownCancel;
    private static bool keyUpCancel;


    //Button Submit
    private static bool keyHeldSubmit;
    private static bool keyDownSubmit;
    private static bool keyUpSubmit;


    private void Start()
    {
        angleIncrement = 360f / numAngles;
        angleDivisions = new float[numAngles + 1];

        for (int i = 0; i < numAngles; i++)
        {
            angleDivisions[i] = i * angleIncrement;
        }
        angleDivisions[angleDivisions.Length - 1] = 360f;
    }

    private void OnEnable()
    {
        foreach (INPUT inputType in System.Enum.GetValues(typeof(INPUT)))
        {
            keyHeldInputs[inputType] = false;
            keyDownInputs[inputType] = false;
            keyUpInputs[inputType] = false;
        }

        actions = new string[]{
            "ButtonNorth",
            "ButtonSouth",
            "ButtonEast",
            "ButtonWest",
            "RightShoulder",
            "LeftShoulder",
            "RightTrigger",
            "LeftTrigger",
            "Start",
            "Select",
            "Cancel",
            "Submit",
        };

        playerAction = LoadActions.LoadAllActions(actions);

        #region AUTOMATIC INPUT
        //Add Started Events
        playerAction.PlayerInput.ButtonNorth.started += ctx => ButtonAction(ctx, INPUT.KeyButtonNorth);
        playerAction.PlayerInput.ButtonSouth.started += ctx => ButtonAction(ctx, INPUT.KeyButtonSouth);
        playerAction.PlayerInput.ButtonEast.started += ctx => ButtonAction(ctx, INPUT.KeyButtonEast);
        playerAction.PlayerInput.ButtonWest.started += ctx => ButtonAction(ctx, INPUT.KeyButtonWest);
        playerAction.PlayerInput.RightShoulder.started += ctx => ButtonAction(ctx, INPUT.ButtonRightShoulder);
        playerAction.PlayerInput.LeftShoulder.started += ctx => ButtonAction(ctx, INPUT.ButtonLeftShoulder);
        playerAction.PlayerInput.RightTrigger.started += ctx => ButtonAction(ctx, INPUT.ButtonRightTrigger);
        playerAction.PlayerInput.LeftTrigger.started += ctx => ButtonAction(ctx, INPUT.ButtonLeftTrigger);
        playerAction.PlayerInput.Start.started += ctx => ButtonAction(ctx, INPUT.Start);
        playerAction.PlayerInput.Select.started += ctx => ButtonAction(ctx, INPUT.Select);
        playerAction.PlayerInput.LeftHorizontalMove.started += LeftStickHorizontalMoveAction;
        playerAction.PlayerInput.LeftVerticalMove.started += LeftStickVerticalMoveAction;
        playerAction.PlayerInput.RightHorizontalMove.started += RightStickHorizontalMoveAction;
        playerAction.PlayerInput.RightVerticalMove.started += RightStickVerticalMoveAction;

        //Add Started Events UI
        playerAction.UI.Cancel.started += ctx => ButtonAction(ctx, INPUT.UICancel);
        playerAction.UI.Submit.started += ctx => ButtonAction(ctx, INPUT.UISubmit);

        //Add Performed Events
        playerAction.PlayerInput.RightTrigger.performed += ctx => ButtonAction(ctx, INPUT.ButtonRightTrigger);
        playerAction.PlayerInput.LeftTrigger.performed += ctx => ButtonAction(ctx, INPUT.ButtonLeftTrigger);
        playerAction.PlayerInput.LeftHorizontalMove.performed += LeftStickHorizontalMoveAction;
        playerAction.PlayerInput.LeftVerticalMove.performed += LeftStickVerticalMoveAction;
        playerAction.PlayerInput.RightHorizontalMove.performed += RightStickHorizontalMoveAction;
        playerAction.PlayerInput.RightVerticalMove.performed += RightStickVerticalMoveAction;

        //Add Canceled Events
        playerAction.PlayerInput.ButtonNorth.canceled += ctx => ButtonAction(ctx, INPUT.KeyButtonNorth);
        playerAction.PlayerInput.ButtonSouth.canceled += ctx => ButtonAction(ctx, INPUT.KeyButtonSouth);
        playerAction.PlayerInput.ButtonEast.canceled += ctx => ButtonAction(ctx, INPUT.KeyButtonEast);
        playerAction.PlayerInput.ButtonWest.canceled += ctx => ButtonAction(ctx, INPUT.KeyButtonWest);
        playerAction.PlayerInput.RightShoulder.canceled += ctx => ButtonAction(ctx, INPUT.ButtonRightShoulder);
        playerAction.PlayerInput.LeftShoulder.canceled += ctx => ButtonAction(ctx, INPUT.ButtonLeftShoulder);
        playerAction.PlayerInput.RightTrigger.canceled += ctx => ButtonAction(ctx, INPUT.ButtonRightTrigger);
        playerAction.PlayerInput.LeftTrigger.canceled += ctx => ButtonAction(ctx, INPUT.ButtonLeftTrigger);
        playerAction.PlayerInput.Start.canceled += ctx => ButtonAction(ctx, INPUT.Start);
        playerAction.PlayerInput.Select.canceled += ctx => ButtonAction(ctx, INPUT.Select);
        playerAction.PlayerInput.LeftHorizontalMove.canceled += LeftStickHorizontalMoveAction;
        playerAction.PlayerInput.LeftVerticalMove.canceled += LeftStickVerticalMoveAction;
        playerAction.PlayerInput.RightHorizontalMove.canceled += RightStickHorizontalMoveAction;
        playerAction.PlayerInput.RightVerticalMove.canceled += RightStickVerticalMoveAction;

        //Add Canceled Events UI
        playerAction.UI.Cancel.canceled += ctx => ButtonAction(ctx, INPUT.UICancel);
        playerAction.UI.Submit.canceled += ctx => ButtonAction(ctx, INPUT.UICancel);
        #endregion


        #region PER INPUT
        //Add Started Events
        playerAction.PlayerInput.ButtonNorth.started += ButtonNorthAction;
        playerAction.PlayerInput.ButtonSouth.started += ButtonSouthAction;
        playerAction.PlayerInput.ButtonEast.started += ButtonEastAction;
        playerAction.PlayerInput.ButtonWest.started += ButtonWestAction;
        playerAction.PlayerInput.RightShoulder.started += RightShoulderAction;
        playerAction.PlayerInput.LeftShoulder.started += LeftShoulderAction;
        playerAction.PlayerInput.RightTrigger.started += RightTriggerAction;
        playerAction.PlayerInput.LeftTrigger.started += LeftTriggerAction;
        playerAction.PlayerInput.Start.started += StartAction;
        playerAction.PlayerInput.Select.started += SelectAction;

        playerAction.UI.Cancel.started += CancelAction;
        playerAction.UI.Submit.started += SubmitAction;

        playerAction.PlayerInput.LeftTrigger.performed += LeftTriggerAction;
        playerAction.PlayerInput.RightTrigger.performed += RightTriggerAction;

        //Add Canceled Events
        playerAction.PlayerInput.ButtonNorth.canceled += ButtonNorthAction;
        playerAction.PlayerInput.ButtonSouth.canceled += ButtonSouthAction;
        playerAction.PlayerInput.ButtonEast.canceled += ButtonEastAction;
        playerAction.PlayerInput.ButtonWest.canceled += ButtonWestAction;
        playerAction.PlayerInput.RightShoulder.canceled += RightShoulderAction;
        playerAction.PlayerInput.LeftShoulder.canceled += LeftShoulderAction;
        playerAction.PlayerInput.RightTrigger.canceled += RightTriggerAction;
        playerAction.PlayerInput.LeftTrigger.canceled += LeftTriggerAction;
        playerAction.PlayerInput.Start.canceled += StartAction;
        playerAction.PlayerInput.Select.canceled += SelectAction;


        playerAction.UI.Cancel.canceled += CancelAction;
        playerAction.UI.Submit.canceled += SubmitAction;

        #endregion

        playerAction.UI.Enable(); // Enable action with new inputs
        playerAction.PlayerInput.Enable(); // Enable action with new inputs
    }



    private void OnDisable()
    {
        #region AUTOMATIC INPUT
        //Remove Started Events
        playerAction.PlayerInput.ButtonNorth.started -= ctx => ButtonAction(ctx, INPUT.KeyButtonNorth);
        playerAction.PlayerInput.ButtonSouth.started -= ctx => ButtonAction(ctx, INPUT.KeyButtonSouth);
        playerAction.PlayerInput.ButtonEast.started -= ctx => ButtonAction(ctx, INPUT.KeyButtonEast);
        playerAction.PlayerInput.ButtonWest.started -= ctx => ButtonAction(ctx, INPUT.KeyButtonWest);
        playerAction.PlayerInput.RightShoulder.started -= ctx => ButtonAction(ctx, INPUT.ButtonRightShoulder);
        playerAction.PlayerInput.LeftShoulder.started -= ctx => ButtonAction(ctx, INPUT.ButtonLeftShoulder);
        playerAction.PlayerInput.RightTrigger.started -= ctx => ButtonAction(ctx, INPUT.ButtonRightTrigger);
        playerAction.PlayerInput.LeftTrigger.started -= ctx => ButtonAction(ctx, INPUT.ButtonLeftTrigger);
        playerAction.PlayerInput.Start.started -= ctx => ButtonAction(ctx, INPUT.Start);
        playerAction.PlayerInput.Select.started -= ctx => ButtonAction(ctx, INPUT.Select);
        playerAction.PlayerInput.LeftHorizontalMove.started -= LeftStickHorizontalMoveAction;
        playerAction.PlayerInput.LeftVerticalMove.started -= LeftStickVerticalMoveAction;
        playerAction.PlayerInput.RightHorizontalMove.started -= RightStickHorizontalMoveAction;
        playerAction.PlayerInput.RightVerticalMove.started -= RightStickVerticalMoveAction;


        //Remove Started Events UI
        playerAction.UI.Cancel.started -= ctx => ButtonAction(ctx, INPUT.UICancel);
        playerAction.UI.Submit.started -= ctx => ButtonAction(ctx, INPUT.UISubmit);

        //Remove Performed Events
        playerAction.PlayerInput.RightTrigger.performed -= ctx => ButtonAction(ctx, INPUT.ButtonRightTrigger);
        playerAction.PlayerInput.LeftTrigger.performed -= ctx => ButtonAction(ctx, INPUT.ButtonLeftTrigger);
        playerAction.PlayerInput.LeftHorizontalMove.performed -= LeftStickHorizontalMoveAction;
        playerAction.PlayerInput.LeftVerticalMove.performed -= LeftStickVerticalMoveAction;
        playerAction.PlayerInput.RightHorizontalMove.performed -= RightStickHorizontalMoveAction;
        playerAction.PlayerInput.RightVerticalMove.performed -= RightStickVerticalMoveAction;

        //Remove Canceled Events
        playerAction.PlayerInput.ButtonNorth.canceled -= ctx => ButtonAction(ctx, INPUT.KeyButtonNorth);
        playerAction.PlayerInput.ButtonSouth.canceled -= ctx => ButtonAction(ctx, INPUT.KeyButtonSouth);
        playerAction.PlayerInput.ButtonEast.canceled -= ctx => ButtonAction(ctx, INPUT.KeyButtonEast);
        playerAction.PlayerInput.ButtonWest.canceled -= ctx => ButtonAction(ctx, INPUT.KeyButtonWest);
        playerAction.PlayerInput.RightShoulder.canceled -= ctx => ButtonAction(ctx, INPUT.ButtonRightShoulder);
        playerAction.PlayerInput.LeftShoulder.canceled -= ctx => ButtonAction(ctx, INPUT.ButtonLeftShoulder);
        playerAction.PlayerInput.RightTrigger.canceled -= ctx => ButtonAction(ctx, INPUT.ButtonRightTrigger);
        playerAction.PlayerInput.LeftTrigger.canceled -= ctx => ButtonAction(ctx, INPUT.ButtonLeftTrigger);
        playerAction.PlayerInput.Start.canceled -= ctx => ButtonAction(ctx, INPUT.Start);
        playerAction.PlayerInput.Select.canceled -= ctx => ButtonAction(ctx, INPUT.Select);
        playerAction.PlayerInput.LeftHorizontalMove.canceled -= LeftStickHorizontalMoveAction;
        playerAction.PlayerInput.LeftVerticalMove.canceled -= LeftStickVerticalMoveAction;
        playerAction.PlayerInput.RightHorizontalMove.canceled -= RightStickHorizontalMoveAction;
        playerAction.PlayerInput.RightVerticalMove.canceled -= RightStickVerticalMoveAction;

        //Remove Canceled Events UI
        playerAction.UI.Cancel.canceled -= ctx => ButtonAction(ctx, INPUT.UICancel);
        playerAction.UI.Submit.canceled -= ctx => ButtonAction(ctx, INPUT.UISubmit);
        #endregion


        #region PER INPUT
        //Remove Started Events
        playerAction.PlayerInput.ButtonNorth.started -= ButtonNorthAction;
        playerAction.PlayerInput.ButtonSouth.started -= ButtonSouthAction;
        playerAction.PlayerInput.ButtonEast.started -= ButtonEastAction;
        playerAction.PlayerInput.ButtonWest.started -= ButtonWestAction;
        playerAction.PlayerInput.RightShoulder.started -= RightShoulderAction;
        playerAction.PlayerInput.LeftShoulder.started -= LeftShoulderAction;
        playerAction.PlayerInput.RightTrigger.started -= RightTriggerAction;
        playerAction.PlayerInput.LeftTrigger.started -= LeftTriggerAction;
        playerAction.PlayerInput.Start.started -= StartAction;
        playerAction.PlayerInput.Select.started -= SelectAction;

        playerAction.UI.Cancel.started -= CancelAction;
        playerAction.UI.Submit.started -= SubmitAction;

        playerAction.PlayerInput.LeftTrigger.performed -= LeftTriggerAction;
        playerAction.PlayerInput.RightTrigger.performed -= RightTriggerAction;

        //Remove Canceled Events
        playerAction.PlayerInput.ButtonNorth.canceled -= ButtonNorthAction;
        playerAction.PlayerInput.ButtonSouth.canceled -= ButtonSouthAction;
        playerAction.PlayerInput.ButtonEast.canceled -= ButtonEastAction;
        playerAction.PlayerInput.ButtonWest.canceled -= ButtonWestAction;
        playerAction.PlayerInput.RightShoulder.canceled -= RightShoulderAction;
        playerAction.PlayerInput.LeftShoulder.canceled -= LeftShoulderAction;
        playerAction.PlayerInput.RightTrigger.canceled -= RightTriggerAction;
        playerAction.PlayerInput.LeftTrigger.canceled -= LeftTriggerAction;
        playerAction.PlayerInput.Start.canceled -= StartAction;
        playerAction.PlayerInput.Select.canceled -= SelectAction;

        playerAction.UI.Cancel.canceled -= CancelAction;
        playerAction.UI.Submit.canceled -= SubmitAction;
        #endregion

        playerAction.UI.Disable(); // Disable action with new inputs
        playerAction.PlayerInput.Disable(); // Disable action with new inputs
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        isDestroyed = true;
    }

    private void Update()
    {
        LeftAnalog = new Vector2(leftStickHorizontal, leftStickVertical);
        LeftAnalogRaw = new Vector2(leftStickHorizontalRaw, leftStickVerticalRaw);
        LeftAnalogNormalized = new Vector2(leftStickHorizontal, leftStickVertical);
        LeftAnalogNormalized = LeftAnalogNormalized.normalized;
        LeftAnalogWithDeadzone = new Vector2(leftStickHorizontal, leftStickVertical);
        if (LeftAnalogWithDeadzone.magnitude >= leftDeadzoneThreshold)
            LeftAnalogWithDeadzone = LeftAnalogNormalized;
        else
            LeftAnalogWithDeadzone = Vector2.zero;
        LeftAnalogAngled = AnalogAngled(new Vector2(leftStickHorizontal, leftStickVertical), leftDeadzoneThreshold);


        RightAnalog = new Vector2(rightStickHorizontal, rightStickVertical);
        RightAnalogRaw = new Vector2(rightStickHorizontalRaw, rightStickVerticalRaw);
        RightAnalogNormalized = new Vector2(rightStickHorizontal, rightStickVertical);
        RightAnalogNormalized = RightAnalogNormalized.normalized;
        RightAnalogWithDeadzone = new Vector2(rightStickHorizontal, rightStickVertical);
        if (RightAnalogWithDeadzone.magnitude >= rightDeadzoneThreshold)
            RightAnalogWithDeadzone = RightAnalogNormalized;
        else
            RightAnalogWithDeadzone = Vector2.zero;
        RightAnalogAngled = AnalogAngled(new Vector2(rightStickHorizontal, rightStickVertical), rightDeadzoneThreshold);
    }

    private Vector2 AnalogAngled(Vector2 analogDirection, float deadzoneThreshold)
    {
        Vector2 direction = analogDirection;
        if (direction.magnitude >= deadzoneThreshold)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (angle < 0)
                angle += 360f;
            float closestAngle = FindClosestAngle(angle);
            direction = AngleToVector(closestAngle);
        }
        else
            direction = Vector2.zero;

        return direction;
    }

    public void ButtonAction(InputAction.CallbackContext context, INPUT keyInput)
    {
        if (isDestroyed)
        {
            return;
        }

        if (context.started)
        {
            keyHeldInputs[keyInput] = true;
            keyDownInputs[keyInput] = true;
            StartCoroutine(ResetKeysCoroutine());
        }
        if (context.canceled)
        {
            keyHeldInputs[keyInput] = false;
            keyUpInputs[keyInput] = true;
            StartCoroutine(ResetKeysCoroutine());
        }
    }

    private IEnumerator ResetKeysCoroutine()
    {
        yield return new WaitForEndOfFrame();
        foreach (var key in new List<INPUT>(keyDownInputs.Keys))
        {
            if (keyDownInputs.ContainsKey(key) && keyDownInputs[key])
            {
                keyDownInputs[key] = false;
            }
            if (keyUpInputs.ContainsKey(key) && keyUpInputs[key])
            {
                keyUpInputs[key] = false;
            }
        }
    }

    public void LeftStickHorizontalMoveAction(InputAction.CallbackContext context)
    {
        // Axys Raw
        float horizontalValue = context.ReadValue<float>();
        if (horizontalValue > 0)
            leftStickHorizontalRaw = 1;
        else if (horizontalValue < 0)
            leftStickHorizontalRaw = -1;
        else
            leftStickHorizontalRaw = 0;

        // Axys Normal
        leftStickHorizontal = horizontalValue;
    }

    public void LeftStickVerticalMoveAction(InputAction.CallbackContext context)
    {
        // Axys Raw
        float verticalValue = context.ReadValue<float>();
        if (verticalValue > 0)
            leftStickVerticalRaw = 1;
        else if (verticalValue < 0)
            leftStickVerticalRaw = -1;
        else
            leftStickVerticalRaw = 0;

        // Axys Normal
        leftStickVertical = verticalValue;
    }

    public void RightStickHorizontalMoveAction(InputAction.CallbackContext context)
    {
        float horizontalValue = context.ReadValue<float>();
        // Axys Raw
        if (horizontalValue > 0)
            rightStickHorizontalRaw = 1;
        else if (horizontalValue < 0)
            rightStickHorizontalRaw = -1;
        else
            rightStickHorizontalRaw = 0;

        // Axys Normal
        rightStickHorizontal = horizontalValue;
    }

    public void RightStickVerticalMoveAction(InputAction.CallbackContext context)
    {
        // Axys Raw
        float verticalValue = context.ReadValue<float>();
        if (verticalValue > 0)
            rightStickVerticalRaw = 1;
        else if (verticalValue < 0)
            rightStickVerticalRaw = -1;
        else
            rightStickVerticalRaw = 0;

        // Axys Normal
        rightStickVertical = verticalValue;
    }

    private static bool CheckKeyInput(Dictionary<INPUT, bool> keyInputs, INPUT inputType) => keyInputs.ContainsKey(inputType) && keyInputs[inputType];
    public static bool KeyDown(INPUT inputType) => CheckKeyInput(keyDownInputs, inputType);
    public static bool KeyUp(INPUT inputType) => CheckKeyInput(keyUpInputs, inputType);
    public static bool KeyHeld(INPUT inputType) => CheckKeyInput(keyHeldInputs, inputType);

    private float FindClosestAngle(float targetAngle)
    {
        float minDifference = float.MaxValue;
        float closestAngle = 0f;

        for (int i = angleDivisions.Length - 1; i >= 0; i--)
        {
            float division = angleDivisions[i];
            float difference = Mathf.Abs(targetAngle - division);
            if (difference < minDifference)
            {
                minDifference = difference;
                closestAngle = division;
            }
        }
        return closestAngle;
    }
    private Vector2 AngleToVector(float angle)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }


    private static bool UseInput(ref bool keyInput)
    {
        if (keyInput)
        {
            keyInput = false;
            return true;
        }
        return false;
    }

    public void ButtonNorthAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            keyHeldButtonNorth = true;
            keyDownButtonNorth = true;
        }
        if (context.canceled)
        {
            keyHeldButtonNorth = false;
            keyUpButtonNorth = true;
        }
        StartCoroutine(ResetAllPerKeysCoroutine());
    }
    public static bool KeyHeldButtonNorth() => keyHeldButtonNorth;
    public static bool KeyDownButtonNorth() => UseInput(ref keyDownButtonNorth);
    public static bool KeyUpButtonNorth() => UseInput(ref keyUpButtonNorth);

    public void ButtonSouthAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            keyHeldButtonSouth = true;
            keyDownButtonSouth = true;
        }
        if (context.canceled)
        {
            keyHeldButtonSouth = false;
            keyUpButtonSouth = true;
        }
        StartCoroutine(ResetAllPerKeysCoroutine());
    }

    public static bool KeyHeldButtonSouth() => keyHeldButtonSouth;
    public static bool KeyDownButtonSouth() => UseInput(ref keyDownButtonSouth);
    public static bool KeyUpButtonSouth() => UseInput(ref keyUpButtonSouth);

    public void ButtonWestAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            keyHeldButtonWest = true;
            keyDownButtonWest = true;
        }
        if (context.canceled)
        {
            keyHeldButtonWest = false;
            keyUpButtonWest = true;
        }
        StartCoroutine(ResetAllPerKeysCoroutine());
    }
    public static bool KeyHeldButtonWest() => keyHeldButtonWest;
    public static bool KeyDownButtonWest() => UseInput(ref keyDownButtonWest);
    public static bool KeyUpButtonWest() => UseInput(ref keyUpButtonWest);

    public void ButtonEastAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            keyHeldButtonEast = true;
            keyDownButtonEast = true;
        }
        if (context.canceled)
        {
            keyHeldButtonEast = false;
            keyUpButtonEast = true;
        }
        StartCoroutine(ResetAllPerKeysCoroutine());
    }

    public static bool KeyHeldButtonEast() => keyHeldButtonEast;
    public static bool KeyDownButtonEast() => UseInput(ref keyDownButtonEast);
    public static bool KeyUpButtonEast() => UseInput(ref keyUpButtonEast);

    public void RightShoulderAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            keyHeldRightShoulder = true;
            keyDownRightShoulder = true;
        }
        if (context.canceled)
        {
            keyHeldRightShoulder = false;
            keyUpRightShoulder = true;
        }
        StartCoroutine(ResetAllPerKeysCoroutine());
    }

    public static bool KeyHeldRightShoulder() => keyHeldRightShoulder;
    public static bool KeyDownRightShoulder() => UseInput(ref keyDownRightShoulder);
    public static bool KeyUpRightShoulder() => UseInput(ref keyUpRightShoulder);

    public void LeftShoulderAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            keyHeldLeftShoulder = true;
            keyDownLeftShoulder = true;
        }
        if (context.canceled)
        {
            keyHeldLeftShoulder = false;
            keyUpLeftShoulder = true;
        }
        StartCoroutine(ResetAllPerKeysCoroutine());
    }
    public static bool KeyHeldLeftShoulder() => keyHeldLeftShoulder;
    public static bool KeyDownLeftShoulder() => UseInput(ref keyDownLeftShoulder);
    public static bool KeyUpLeftShoulder() => UseInput(ref keyUpLeftShoulder);

    public void RightTriggerAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            keyHeldRightTrigger = true;
            keyDownRightTrigger = true;
            StartCoroutine(ResetAllPerKeysCoroutine());
        }
        else if (context.canceled)
        {
            keyHeldRightTrigger = false;
            keyUpRightTrigger = true;
            StartCoroutine(ResetAllPerKeysCoroutine());
        }
    }

    public static bool KeyHeldRightTrigger() => keyHeldRightTrigger;
    public static bool KeyDownRightTrigger() => UseInput(ref keyDownRightTrigger);
    public static bool KeyUpRightTrigger() => UseInput(ref keyUpRightTrigger);

    public void LeftTriggerAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            keyHeldLeftTrigger = true;
            keyDownLeftTrigger = true;
            StartCoroutine(ResetAllPerKeysCoroutine());
        }
        if (context.canceled)
        {
            keyHeldLeftTrigger = false;
            keyUpLeftTrigger = true;
            StartCoroutine(ResetAllPerKeysCoroutine());
        }
    }
    public static bool KeyHeldLeftTrigger() => keyHeldLeftTrigger;
    public static bool KeyDownLeftTrigger() => UseInput(ref keyDownLeftTrigger);
    public static bool KeyUpLeftTrigger() => UseInput(ref keyUpLeftTrigger);


    public void StartAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            keyHeldStart = true;
            keyDownStart = true;
        }
        if (context.canceled)
        {
            keyHeldStart = false;
            keyUpStart = true;
        }
        StartCoroutine(ResetAllPerKeysCoroutine());
    }

    public static bool KeyHeldStart() => keyHeldStart;
    public static bool KeyDownStart() => UseInput(ref keyDownStart);
    public static bool KeyUpStart() => UseInput(ref keyUpStart);
    public void SelectAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            keyHeldSelect = true;
            keyDownSelect = true;
        }
        if (context.canceled)
        {
            keyHeldSelect = false;
            keyUpSelect = true;
        }
        StartCoroutine(ResetAllPerKeysCoroutine());
    }
    public static bool KeyHeldSelect() => keyHeldSelect;
    public static bool KeyDownSelect() => UseInput(ref keyDownSelect);
    public static bool KeyUpSelect() => UseInput(ref keyUpSelect);

    public void CancelAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            keyHeldCancel = true;
            keyDownCancel = true;
        }
        if (context.canceled)
        {
            keyHeldCancel = false;
            keyUpCancel = true;
        }
        StartCoroutine(ResetAllPerKeysCoroutine());
    }

    public static bool KeyHeldCancel() => keyHeldCancel;
    public static bool KeyDownCancel() => UseInput(ref keyDownCancel);
    public static bool KeyUpCancel() => UseInput(ref keyDownCancel);
    public void SubmitAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            keyHeldSubmit = true;
            keyDownSubmit = true;
        }
        if (context.canceled)
        {
            keyHeldSubmit = false;
            keyUpSubmit = true;
        }
        StartCoroutine(ResetAllPerKeysCoroutine());
    }
    public static bool KeyHeldSubmit() => keyHeldSubmit;
    public static bool KeyDownSubmit() => UseInput(ref keyDownSubmit);
    public static bool KeyUpSubmit() => UseInput(ref keyDownSubmit);


    public void UseAllInputs()
    {
        UseInput(ref keyDownButtonNorth);
        UseInput(ref keyUpButtonNorth);

        UseInput(ref keyDownButtonSouth);
        UseInput(ref keyUpButtonSouth);

        UseInput(ref keyDownButtonWest);
        UseInput(ref keyUpButtonWest);

        UseInput(ref keyDownButtonEast);
        UseInput(ref keyUpButtonEast);

        UseInput(ref keyDownLeftShoulder);
        UseInput(ref keyUpLeftShoulder);

        UseInput(ref keyDownRightShoulder);
        UseInput(ref keyUpRightShoulder);

        UseInput(ref keyDownLeftTrigger);
        UseInput(ref keyUpLeftTrigger);

        UseInput(ref keyDownRightTrigger);
        UseInput(ref keyUpRightTrigger);

        UseInput(ref keyDownStart);
        UseInput(ref keyUpStart);

        UseInput(ref keyDownSelect);
        UseInput(ref keyUpSelect);

        UseInput(ref keyDownCancel);
        UseInput(ref keyUpCancel);

        UseInput(ref keyDownSubmit);
        UseInput(ref keyUpSubmit);
    }

    private IEnumerator ResetAllPerKeysCoroutine()
    {
        yield return new WaitForEndOfFrame();
        UseAllInputs();
    }
}