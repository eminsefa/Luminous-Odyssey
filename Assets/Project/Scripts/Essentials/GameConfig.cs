using System;
using DG.Tweening;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GameConfig")]
public class GameConfig : SingletonScriptableObject<GameConfig>
{
    public InputVariables    Input    = new ();
    public MovementVariables Movement = new ();
}

[Serializable]
public class InputVariables
{
    [OnValueChanged(nameof(OnInputReversed))]
    public bool ReverseInput;

    public JoystickData Joystick;

    [Serializable]
    public class JoystickData
    {
        public bool IsShowVisuals;

        public bool IsStatic;

        public bool IsResetDirection;

        public float Radius;
        public float HandleRadiusMultiplier;
    }

    public void OnInputReversed() => StorageManager.Instance.IsInputReverse = ReverseInput;
}

[Serializable]
public class MovementVariables
{
    public float              MoveSpeed;
    public float              JumpSpeed;
    public float              JumpThreshold;
    public float              MaxSpeed;
    public float              GravityScale;
    public float              HangingStartDuration;
    public float              HangingSpeed;
    public float              HangingCheckMinSpeed;
    public DashDataDictionary DashDataDictionary;
    public ColorVariables     ColorVars;
}

[Serializable]
public class ColorVariables
{
    public float          DarkenSpeed;
    public float          BrightenSpeed;
    public AnimationCurve BrightnessCurve;
}

[Serializable]
public class DashData
{
    public Vector2        Direction;
    public float          DashSpeed;
    public float          DashAmount;
    public Ease           DashEase;
    public AnimationCurve DashCurve;
}

[Serializable] public class DashDataDictionary : UnitySerializedDictionary<InputManager.eSwipeDirections, DashData> {}