using System;
using Enums;
using Managers;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class HUDManager : Singleton<HUDManager>
{
    private bool           isInputReverse;
    private Vector2        dummyVector2;
    public  Vector2        JoystickInputPos => isInputReverse ? leftInputButton.InputPosition : rghtInputButton.InputPosition;
    public  Vector2        ActionInputPos   => isInputReverse ? rghtInputButton.InputPosition : leftInputButton.InputPosition;
    private InputVariables inputVars        => GameConfig.Instance.Input;

#region Refs
    
    [SerializeField, ReadOnly] private InputButton leftInputButton;
    [SerializeField, ReadOnly] private InputButton rghtInputButton;

#if UNITY_EDITOR

    [Button]
    public void SetRefs()
    {
        leftInputButton = transform.FindDeepChild<InputButton>("Left Button");
        rghtInputButton = transform.FindDeepChild<InputButton>("Right Button");
    }
#endif

#endregion

#region Init

    private void OnEnable()
    {
        StorageManager.OnInputReverseChanged += OnInputReverseChanged;
        leftInputButton.OnButtonDown         += OnInputDown;
        leftInputButton.OnButtonUp           += OnInputUp;
        rghtInputButton.OnButtonDown         += OnInputDown;
        rghtInputButton.OnButtonUp           += OnInputUp;
        Init();
    }

    private void OnDisable()
    {
        StorageManager.OnInputReverseChanged -= OnInputReverseChanged;
        leftInputButton.OnButtonDown         -= OnInputDown;
        leftInputButton.OnButtonUp           -= OnInputUp;
        rghtInputButton.OnButtonDown         -= OnInputDown;
        rghtInputButton.OnButtonUp           -= OnInputUp;
    }

#endregion

#region Callbacks

#region Input

    private void OnInputDown(InputButton button)
    {
        bool right = button.ButtonType == eButtonType.InputMove;
        if (right)
        {
            if (!isInputReverse) joystickInputDown(button.InputPosition);
            else actionInputDown(button.InputPosition);
        }
        else
        {
            if (isInputReverse) joystickInputDown(button.InputPosition);
            else actionInputDown(button.InputPosition);
        }
    }

    private void OnInputUp(InputButton button)
    {
        bool right = button.ButtonType == eButtonType.InputMove;
        if (right)
        {
            if (!isInputReverse) joystickInputUp(button.InputPosition);
            else actionInputUp(button.InputPosition);
        }
        else
        {
            if (isInputReverse) joystickInputUp(button.InputPosition);
            else actionInputUp(button.InputPosition);
        }
    }

    private void OnInputReverseChanged(bool reversed)
    {
        isInputReverse = reversed;
    }

#endregion

#endregion

    private void Init()
    {
        isInputReverse = StorageManager.Instance.IsInputReverse;
    }
    
#region Input

    private void actionInputDown(Vector2 inputPos)
    {
        InputManager.Instance.InputActionDown();
    }

    private void actionInputUp(Vector2 inputPos)
    {
        InputManager.Instance.InputActionUp();
    }

    private void joystickInputDown(Vector2 inputPos)
    {
        InputManager.Instance.InputJoystickDown();
    }

    private void joystickInputUp(Vector2 inputPos)
    {
        InputManager.Instance.InputJoystickUp();
    }

#endregion
}