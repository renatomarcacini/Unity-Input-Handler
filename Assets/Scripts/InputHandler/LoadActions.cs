using UnityEngine;
using UnityEngine.InputSystem;

namespace NatoGames.JoystickSystem
{
    public class LoadActions
    {
        public static PlayerAction inputActions;

        public static PlayerAction LoadAllActions(string[] actions)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                LoadBindingOverride(actions[i]);
            }

            return inputActions;
        }

        private static void LoadBindingOverride(string actionName)
        {
            if (inputActions == null)
                inputActions = new PlayerAction();

            InputAction action = inputActions.asset.FindAction(actionName);

            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (!string.IsNullOrEmpty(PlayerPrefs.GetString(action.actionMap + action.name + i)))
                {
                    //Debug.Log($"ACTION:{PlayerPrefs.GetString(action.actionMap + action.name + i)}");
                    action.ApplyBindingOverride(i, PlayerPrefs.GetString(action.actionMap + action.name + i));
                }
            }
        }
    }
}

