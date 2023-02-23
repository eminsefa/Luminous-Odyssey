using Enums;
using Managers;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using Util;

public class HUDManager : Singleton<HUDManager>
{
    
#region Input Joystick
    
    // private bool    m_IsInputReverse;
    // public  Vector2 JoystickInputPos => m_IsInputReverse ? m_LeftInputButton.InputPosition : m_RightInputButton.InputPosition;
    // public  Vector2 ActionInputPos   => m_IsInputReverse ? m_RightInputButton.InputPosition : m_LeftInputButton.InputPosition;
    
    // [SerializeField, ReadOnly] private InputButton m_LeftInputButton;
    // [SerializeField, ReadOnly] private InputButton m_RightInputButton;
    
    // [Button]
    // public void SetRefs()
    // {
    //     m_LeftInputButton = transform.FindDeepChild<InputButton>("Left Button");
    //     m_RightInputButton = transform.FindDeepChild<InputButton>("Right Button");
    // }
    
    // private void OnEnable()
    // {
    //     StorageManager.OnInputReverseChanged += OnInputReverseChanged;
    //     m_LeftInputButton.OnButtonDown       += OnInputDown;
    //     m_LeftInputButton.OnButtonUp         += OnInputUp;
    //     m_RightInputButton.OnButtonDown      += OnInputDown;
    //     m_RightInputButton.OnButtonUp        += OnInputUp;
    //     Init();
    // }
    //
    // private void OnDisable()
    // {
    //     StorageManager.OnInputReverseChanged -= OnInputReverseChanged;
    //     m_LeftInputButton.OnButtonDown       -= OnInputDown;
    //     m_LeftInputButton.OnButtonUp         -= OnInputUp;
    //     m_RightInputButton.OnButtonDown      -= OnInputDown;
    //     m_RightInputButton.OnButtonUp        -= OnInputUp;
    // }
    
    // private void Init()
    // {
    //     m_IsInputReverse = StorageManager.Instance.IsInputReverse;
    // }
    
    // private void OnInputDown(InputButton button)
    // {
    //     bool right = button.ButtonType == eButtonType.InputMove;
    //     if (right)
    //     {
    //         if (!m_IsInputReverse) joystickInputDown(button.InputPosition);
    //         else actionInputDown(button.InputPosition);
    //     }
    //     else
    //     {
    //         if (m_IsInputReverse) joystickInputDown(button.InputPosition);
    //         else actionInputDown(button.InputPosition);
    //     }
    // }
    //
    // private void OnInputUp(InputButton button)
    // {
    //     bool right = button.ButtonType == eButtonType.InputMove;
    //     if (right)
    //     {
    //         if (!m_IsInputReverse) joystickInputUp(button.InputPosition);
    //         else actionInputUp(button.InputPosition);
    //     }
    //     else
    //     {
    //         if (m_IsInputReverse) joystickInputUp(button.InputPosition);
    //         else actionInputUp(button.InputPosition);
    //     }
    // }
    //
    // private void OnInputReverseChanged(bool reversed)
    // {
    //     m_IsInputReverse = reversed;
    // }
    
    // private void actionInputDown(Vector2 inputPos)
    // {
    //     InputManager.Instance.InputActionDown();
    // }
    //
    // private void actionInputUp(Vector2 inputPos)
    // {
    //     InputManager.Instance.InputActionUp();
    // }
    //
    // private void joystickInputDown(Vector2 inputPos)
    // {
    //     InputManager.Instance.InputJoystickDown();
    // }
    //
    // private void joystickInputUp(Vector2 inputPos)
    // {
    //     InputManager.Instance.InputJoystickUp();
    // }

#endregion
    
}