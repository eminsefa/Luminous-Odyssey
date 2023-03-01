using System;
using Attributes;
using Enums;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    [ExecutionOrder(eExecutionOrder.InputManager)]
    public class InputManager : Singleton<InputManager>
    {
        public static event Action OnJumpInput;
        public static event Action OnDashInput;

        [field: SerializeField] public InputAction PlayerMovement { get; set; }
        [field: SerializeField] public InputAction PlayerAction   { get; set; }

        [SerializeField]            private bool       logScreen;
        [ShowInInspector, ReadOnly] public  ScreenData ScreenData;
        
    #region Unity Methods

        private void OnEnable()
        {
            GameManager.OnLevelStarted += OnLevelStarted;
            OnLevelStarted();

            PlayerMovement.Enable();
            PlayerAction.Enable();

            PlayerAction.performed += OnActionInput;
        }

        private void OnDisable()
        {
            GameManager.OnLevelStarted -= OnLevelStarted;

            PlayerMovement.Disable();
            PlayerAction.Disable();

            PlayerAction.performed -= OnActionInput;
        }

    #endregion
        
    #region Events

        protected override void OnAwakeEvent()
        {
            ScreenData.CalculateData(logScreen);
        }
        
        private void OnLevelStarted() { }
        
        private void OnActionInput(InputAction.CallbackContext context)
        {
            if (context.control == PlayerAction.controls[0]) //Jump
            {
                OnJumpInput?.Invoke();
            }

            if (context.control == PlayerAction.controls[1]) //Dash
            {
                OnDashInput?.Invoke();
            }
        }

    #endregion

    #region Joystick

        // private InputVariables inputVars => GameConfig.Instance.Input;

        // public static event Action<eSwipeDirections> OnInputActionSwiped;
        // public static event Action<Vector2> OnInputActionDown;
        // public static event Action<Vector2> OnInputActionUp;
        // public static event Action<Vector2> OnInputJoystickDown;
        // public static event Action<Vector2> OnInputJoystickUp;

        // private Vector2 joystickPositionFromMousePosAndDirection => HUDManager.Instance.JoystickInputPos - JoystickDirection * inputVars.Joystick.Radius * ScreenData.FinalDPI;
        //
        // public bool    IsJoystickDown              { get; private set; }
        // public Vector2 JoystickPosition            { get; private set; } = Vector2.zero;
        // public Vector2 JoystickDirection           { get; private set; } = Vector2.zero;
        // public Vector2 JoystickHandleLocalPosition { get; private set; } = Vector2.zero;
        //
        // public enum eSwipeDirections
        // {
        //     Up,
        //     Down,
        //     Right,
        //     Left,
        //     UpRight,
        //     UpLeft,
        //     DownRight,
        //     DownLeft,
        // }

        // private void OnLevelStarted()
        // {
        //     JoystickDirection           = Vector2.zero;
        //     JoystickPosition            = Vector2.zero;
        //     JoystickHandleLocalPosition = Vector2.zero;
        // }

        // private void Update()
        // {
        //     if (!IsJoystickDown) return;
        //
        //     computeJoystick();
        // }

        // public void InputActionDown()
        // {
        //     OnInputActionDown?.Invoke(HUDManager.Instance.ActionInputPos);
        // }
        //
        // public void InputActionUp()
        // {
        //     OnInputActionUp?.Invoke(HUDManager.Instance.ActionInputPos);
        // }
        //
        // public void InputJoystickDown()
        // {
        //     IsJoystickDown = true;
        //     resetJoystick();
        //     OnInputJoystickDown?.Invoke(HUDManager.Instance.JoystickInputPos);
        // }
        //
        // public void InputJoystickUp()
        // {
        //     IsJoystickDown = false;
        //     resetJoystick();
        //
        //     OnInputJoystickUp?.Invoke(HUDManager.Instance.JoystickInputPos);
        // }

        // private void resetJoystick()
        // {
        //     if (inputVars.Joystick.IsResetDirection)
        //     {
        //         if (JoystickDirection != Vector2.zero)
        //         {
        //             JoystickDirection = JoystickDirection.normalized * 0.05f;
        //             JoystickPosition  = JoystickHandleLocalPosition = HUDManager.Instance.JoystickInputPos - JoystickDirection;
        //         }
        //         else
        //         {
        //             JoystickDirection = Vector2.zero;
        //             JoystickPosition  = JoystickHandleLocalPosition = HUDManager.Instance.JoystickInputPos;
        //         }
        //     }
        //     else
        //     {
        //         JoystickPosition            = joystickPositionFromMousePosAndDirection;
        //         JoystickHandleLocalPosition = JoystickDirection;
        //     }
        //
        //     computeJoystick();
        // }
        //
        // private void computeJoystick()
        // {
        //     JoystickDirection = (HUDManager.Instance.JoystickInputPos - JoystickPosition) / (inputVars.Joystick.Radius * ScreenData.FinalDPI);
        //
        //     if (inputVars.Joystick.IsStatic)
        //     {
        //         if (JoystickDirection.magnitude > 1)
        //         {
        //             JoystickDirection = JoystickDirection.normalized;
        //         }
        //     }
        //     else
        //     {
        //         if (JoystickDirection.magnitude > 1)
        //         {
        //             JoystickDirection = JoystickDirection.normalized;
        //             JoystickPosition  = joystickPositionFromMousePosAndDirection;
        //         }
        //     }
        //
        //     JoystickHandleLocalPosition = JoystickDirection;
        // }

    #endregion
    }
}