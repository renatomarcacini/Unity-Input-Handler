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

public class InputHandler : Singleton<InputHandler>
{
    private bool isDestroyed = false;

    public static Dictionary<INPUT, bool> keyHeldInputs = new Dictionary<INPUT, bool>();
    public static Dictionary<INPUT, bool> keyDownInputs = new Dictionary<INPUT, bool>();
    public static Dictionary<INPUT, bool> keyUpInputs = new Dictionary<INPUT, bool>();

    [Header("Analog Settings")]
    [Space(10)]
    [SerializeField] private string[] actions;
    [SerializeField, Range(0, 1)] private float leftDeadzoneThreshold = 0.5f;
    [SerializeField, Range(0, 1)] private float rightDeadzoneThreshold = 0.5f;

    [SerializeField] private int numAngles = 8;
    private float angleIncrement;
    private float[] angleDivisions;

    private PlayerAction playerAction;

    [Header("Analog Previews")]
    [Space(10)]
    //Left Stick Analog
    [SerializeField] private float leftStickHorizontal;
    [SerializeField] private float leftStickVertical;
    [SerializeField] private int leftStickHorizontalRaw;
    [SerializeField] private int leftStickVerticalRaw;

    //Right Stick Analog
    [SerializeField] private float rightStickHorizontal;
    [SerializeField] private float rightStickVertical;
    [SerializeField] private int rightStickHorizontalRaw;
    [SerializeField] private int rightStickVerticalRaw;

    public static Vector2 LeftAnalog { get; private set; }
    public static Vector2 LeftAnalogRaw { get; private set; }
    public static Vector2 LeftAnalogNormalized { get; private set; }
    public static Vector2 LeftAnalogWithDeadzone { get; private set; }
    public static Vector2 LeftAnalogAngled { get; private set; }

    public static Vector2 RightAnalog { get; private set; }
    public static Vector2 RightAnalogRaw { get; private set; }
    public static Vector2 RightAnalogNormalized { get; private set; }
    public static Vector2 RightAnalogWithDeadzone { get; private set; }
    public static Vector2 RightAnalogAngled { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }


    private void Start()
    {
        angleIncrement = 360f / numAngles;
        angleDivisions = new float[numAngles + 1];

        for (int i = 0; i < numAngles; i++)
        {
            angleDivisions[i] = i * angleIncrement;
        }
        angleDivisions[angleDivisions.Length - 1] = 360f;

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
            };

        //playerAction = LoadActions.LoadAllActions(actions);
        playerAction = new PlayerAction();
        #region PLAYER_ACTION_MAP
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
        #endregion

        #region UI_ACTION_MAP
        //Add Started Events
        playerAction.UI.ButtonNorth.started += ctx => ButtonAction(ctx, INPUT.UI_KeyButtonNorth);
        playerAction.UI.ButtonSouth.started += ctx => ButtonAction(ctx, INPUT.UI_KeyButtonSouth);
        playerAction.UI.ButtonEast.started += ctx => ButtonAction(ctx, INPUT.UI_KeyButtonEast);
        playerAction.UI.ButtonWest.started += ctx => ButtonAction(ctx, INPUT.UI_KeyButtonWest);
        playerAction.UI.RightShoulder.started += ctx => ButtonAction(ctx, INPUT.UI_ButtonRightShoulder);
        playerAction.UI.LeftShoulder.started += ctx => ButtonAction(ctx, INPUT.UI_ButtonLeftShoulder);
        playerAction.UI.RightTrigger.started += ctx => ButtonAction(ctx, INPUT.UI_ButtonRightTrigger);
        playerAction.UI.LeftTrigger.started += ctx => ButtonAction(ctx, INPUT.UI_ButtonLeftTrigger);
        playerAction.UI.Start.started += ctx => ButtonAction(ctx, INPUT.UI_Start);
        playerAction.UI.Select.started += ctx => ButtonAction(ctx, INPUT.UI_Select);

        //Add Canceled Events
        playerAction.UI.ButtonNorth.canceled += ctx => ButtonAction(ctx, INPUT.UI_KeyButtonNorth);
        playerAction.UI.ButtonSouth.canceled += ctx => ButtonAction(ctx, INPUT.UI_KeyButtonSouth);
        playerAction.UI.ButtonEast.canceled += ctx => ButtonAction(ctx, INPUT.UI_KeyButtonEast);
        playerAction.UI.ButtonWest.canceled += ctx => ButtonAction(ctx, INPUT.UI_KeyButtonWest);
        playerAction.UI.RightShoulder.canceled += ctx => ButtonAction(ctx, INPUT.UI_ButtonRightShoulder);
        playerAction.UI.LeftShoulder.canceled += ctx => ButtonAction(ctx, INPUT.UI_ButtonLeftShoulder);
        playerAction.UI.RightTrigger.canceled += ctx => ButtonAction(ctx, INPUT.UI_ButtonRightTrigger);
        playerAction.UI.LeftTrigger.canceled += ctx => ButtonAction(ctx, INPUT.UI_ButtonLeftTrigger);
        playerAction.UI.Start.canceled += ctx => ButtonAction(ctx, INPUT.UI_Start);
        playerAction.UI.Select.canceled += ctx => ButtonAction(ctx, INPUT.UI_Select);
        #endregion

        playerAction.UI.Enable(); // Enable action with new inputs
        playerAction.PlayerInput.Enable(); // Enable action with new inputs
    }

    private void OnDestroy()
    {
        #region PLAYER_ACTION_MAP
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

        //Add Performed Events
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
        #endregion

        #region UI_ACTION_MAP
        //Add Started Events
        playerAction.UI.ButtonNorth.started -= ctx => ButtonAction(ctx, INPUT.UI_KeyButtonNorth);
        playerAction.UI.ButtonSouth.started -= ctx => ButtonAction(ctx, INPUT.UI_KeyButtonSouth);
        playerAction.UI.ButtonEast.started -= ctx => ButtonAction(ctx, INPUT.UI_KeyButtonEast);
        playerAction.UI.ButtonWest.started -= ctx => ButtonAction(ctx, INPUT.UI_KeyButtonWest);
        playerAction.UI.RightShoulder.started -= ctx => ButtonAction(ctx, INPUT.UI_ButtonRightShoulder);
        playerAction.UI.LeftShoulder.started -= ctx => ButtonAction(ctx, INPUT.UI_ButtonLeftShoulder);
        playerAction.UI.RightTrigger.started -= ctx => ButtonAction(ctx, INPUT.UI_ButtonRightTrigger);
        playerAction.UI.LeftTrigger.started -= ctx => ButtonAction(ctx, INPUT.UI_ButtonLeftTrigger);
        playerAction.UI.Start.started -= ctx => ButtonAction(ctx, INPUT.UI_Start);
        playerAction.UI.Select.started -= ctx => ButtonAction(ctx, INPUT.UI_Select);

        //Add Canceled Events
        playerAction.UI.ButtonNorth.canceled -= ctx => ButtonAction(ctx, INPUT.UI_KeyButtonNorth);
        playerAction.UI.ButtonSouth.canceled -= ctx => ButtonAction(ctx, INPUT.UI_KeyButtonSouth);
        playerAction.UI.ButtonEast.canceled -= ctx => ButtonAction(ctx, INPUT.UI_KeyButtonEast);
        playerAction.UI.ButtonWest.canceled -= ctx => ButtonAction(ctx, INPUT.UI_KeyButtonWest);
        playerAction.UI.RightShoulder.canceled -= ctx => ButtonAction(ctx, INPUT.UI_ButtonRightShoulder);
        playerAction.UI.LeftShoulder.canceled -= ctx => ButtonAction(ctx, INPUT.UI_ButtonLeftShoulder);
        playerAction.UI.RightTrigger.canceled -= ctx => ButtonAction(ctx, INPUT.UI_ButtonRightTrigger);
        playerAction.UI.LeftTrigger.canceled -= ctx => ButtonAction(ctx, INPUT.UI_ButtonLeftTrigger);
        playerAction.UI.Start.canceled -= ctx => ButtonAction(ctx, INPUT.UI_Start);
        playerAction.UI.Select.canceled -= ctx => ButtonAction(ctx, INPUT.UI_Select);
        #endregion

        playerAction.UI.Disable(); // Disable action with new inputs
        playerAction.PlayerInput.Disable(); // Disable action with new inputs
        StopAllCoroutines();
        isDestroyed = true;
    }

    private void HandleRebindCompeleted()
    {
        playerAction.Disable();
        playerAction = LoadActions.LoadAllActions(actions);
        playerAction.Enable();
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
            return;


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
}
