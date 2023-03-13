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

        private float m_JumpTimer;
        private float m_DashTimer;

        [field: SerializeField] public InputAction PlayerMovement { get; set; }
        [field: SerializeField] public InputAction PlayerAction   { get; set; }

        private InputVariables m_InputVars => GameConfig.Instance.Input;

        [SerializeField]            private bool       logScreen;
        [ShowInInspector, ReadOnly] public  ScreenData ScreenData;

    #region Unity Methods

        private void OnEnable()
        {
            OnLevelStarted();

            PlayerMovement.Enable();
            PlayerAction.Enable();

            PlayerAction.performed += OnActionInput;
        }

        private void OnDisable()
        {
            PlayerMovement.Disable();
            PlayerAction.Disable();

            PlayerAction.performed -= OnActionInput;
        }

        private void Update()
        {
            m_JumpTimer += Time.deltaTime;
            m_DashTimer += Time.deltaTime;
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
            if (context.control == PlayerAction.controls[0] && m_JumpTimer > 0) //Jump
            {
                m_JumpTimer = -m_InputVars.JumpInputInterval;
                OnJumpInput?.Invoke();
            }

            if (context.control == PlayerAction.controls[1] && m_DashTimer > 0) //Dash
            {
                m_DashTimer = -m_InputVars.DashInputInterval;
                OnDashInput?.Invoke();
            }
        }

    #endregion
    }
}