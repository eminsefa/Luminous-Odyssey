using System;
using Attributes;
using Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers
{
    [ExecutionOrder(eExecutionOrder.InputManager)]
    public class InputManager : Singleton<InputManager>
    {
        public static event Action<eSwipeDirections> OnInputActionSwiped;
        
        public static event Action<Vector2>          OnInputActionDown;
        public static event Action<Vector2>          OnInputActionUp;
        public static event Action<Vector2>          OnInputJoystickDown;
        public static event Action<Vector2>          OnInputJoystickUp;
        
        private InputVariables inputVars  => GameConfig.Instance.Input;

        [SerializeField]            private bool       logScreen;
        [ShowInInspector, ReadOnly] public  ScreenData ScreenData;

        public enum eSwipeDirections
        {
            Up,
            Down,
            Right,
            Left,
        }
        
    #region Joystick

        private Vector2 joystickPositionFromMousePosAndDirection => HUDManager.Instance.JoystickInputPos - JoystickDirection * inputVars.Joystick.Radius * ScreenData.FinalDPI;

        public bool    IsJoystickDown              { get; private set; }
        public Vector2 JoystickPosition            { get; private set; } = Vector2.zero;
        public Vector2 JoystickDirection           { get; private set; } = Vector2.zero;
        public Vector2 JoystickHandleLocalPosition { get; private set; } = Vector2.zero;

    #endregion

        public override void Awake()
        {
            base.Awake();
            ScreenData.CalculateData(logScreen);
        }

        private void OnEnable()
        {
            GameManager.OnLevelStarted += OnLevelStarted;
            OnLevelStarted();
        }

        private void OnDisable()
        {
            GameManager.OnLevelStarted -= OnLevelStarted;
        }

        private void OnLevelStarted()
        {
            JoystickDirection           = Vector2.zero;
            JoystickPosition            = Vector2.zero;
            JoystickHandleLocalPosition = Vector2.zero;
        }

        private void Update()
        {
            keyboardInput();
            if (!IsJoystickDown) return;

            computeJoystick();
        }

        private void keyboardInput()
        {
            if(Input.GetKeyDown(KeyCode.W)) OnInputActionSwiped?.Invoke(eSwipeDirections.Up);
            else if(Input.GetKeyDown(KeyCode.S)) OnInputActionSwiped?.Invoke(eSwipeDirections.Down);
            else if(Input.GetKeyDown(KeyCode.D)) OnInputActionSwiped?.Invoke(eSwipeDirections.Right);
            else if(Input.GetKeyDown(KeyCode.A)) OnInputActionSwiped?.Invoke(eSwipeDirections.Left);
            else if(Input.GetKeyDown(KeyCode.Q)) OnInputActionDown?.Invoke(Vector2.zero);
        }

        public void InputActionDown()
        {
            OnInputActionDown?.Invoke(HUDManager.Instance.ActionInputPos);
        }

        public void InputActionUp()
        {
            OnInputActionUp?.Invoke(HUDManager.Instance.ActionInputPos);
        }

        public void InputJoystickDown()
        {
            IsJoystickDown = true;
            resetJoystick();
            OnInputJoystickDown?.Invoke(HUDManager.Instance.JoystickInputPos);
        }

        public void InputJoystickUp()
        {
            IsJoystickDown = false;
            resetJoystick();

            OnInputJoystickUp?.Invoke(HUDManager.Instance.JoystickInputPos);
        }


    #region Joystick

        private void resetJoystick()
        {
            if (inputVars.Joystick.IsResetDirection)
            {
                if (JoystickDirection != Vector2.zero)
                {
                    JoystickDirection = JoystickDirection.normalized * 0.05f;
                    JoystickPosition  = JoystickHandleLocalPosition = HUDManager.Instance.JoystickInputPos - JoystickDirection;
                }
                else
                {
                    JoystickDirection = Vector2.zero;
                    JoystickPosition  = JoystickHandleLocalPosition = HUDManager.Instance.JoystickInputPos;
                }
            }
            else
            {
                JoystickPosition            = joystickPositionFromMousePosAndDirection;
                JoystickHandleLocalPosition = JoystickDirection;
            }

            computeJoystick();
        }

        private void computeJoystick()
        {
            JoystickDirection = (HUDManager.Instance.JoystickInputPos - JoystickPosition) / (inputVars.Joystick.Radius * ScreenData.FinalDPI);

            if (inputVars.Joystick.IsStatic)
            {
                if (JoystickDirection.magnitude > 1)
                {
                    JoystickDirection = JoystickDirection.normalized;
                }
            }
            else
            {
                if (JoystickDirection.magnitude > 1)
                {
                    JoystickDirection = JoystickDirection.normalized;
                    JoystickPosition  = joystickPositionFromMousePosAndDirection;
                }
            }

            JoystickHandleLocalPosition = JoystickDirection;
        }

    #endregion
    }
}