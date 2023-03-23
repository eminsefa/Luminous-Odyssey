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
        public static event Action OnInteractionInput;
        public static event Action OnFireInput;

        private float[] m_InputTimers = new float[4];

        [field: SerializeField] public InputAction PlayerMovement { get; set; }
        [field: SerializeField] public InputAction PlayerAction   { get; set; }

        private InputVariables m_InputVars => GameConfig.Instance.Input;

        [SerializeField]            private bool       logScreen;
        [ShowInInspector, ReadOnly] public  ScreenData ScreenData;

    #region Unity Methods

        private void OnEnable()
        {
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
            for (var i = 0; i < m_InputTimers.Length; i++)
            {
                m_InputTimers[i] += Time.deltaTime;
            }
        }

    #endregion

    #region Events

        protected override void OnAwakeEvent()
        {
            ScreenData.CalculateData(logScreen);
        }

        private void OnActionInput(InputAction.CallbackContext context)
        {
            var jumpOrDash = false;
            if (context.control == PlayerAction.controls[0] && m_InputTimers[0] > 0.1f) //Jump
            {
                m_InputTimers[0] = 0;
                jumpOrDash       = true;
                OnJumpInput?.Invoke();
            }

            if (context.control == PlayerAction.controls[1] && m_InputTimers[1] > 0.1f) //Dash
            {
                m_InputTimers[1] = 0;
                jumpOrDash       = true;
                OnDashInput?.Invoke();
            }

            if (context.control == PlayerAction.controls[2] && m_InputTimers[2] > 0.1f && !jumpOrDash) //Interaction
            {
                m_InputTimers[2] = 0;
                OnInteractionInput?.Invoke();
            }

            if (context.control == PlayerAction.controls[3] && m_InputTimers[3] > 0.1f) //Fire
            {
                m_InputTimers[3] = 0;
                OnFireInput?.Invoke();
            }
        }

        public void SetInputTimer(int i_Index)
        {
            m_InputTimers[i_Index] = -m_InputVars.InputIntervals[i_Index].Value;
        }

    #endregion
    }
}