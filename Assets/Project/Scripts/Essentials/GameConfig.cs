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
    
    [BoxGroup("Movement")] public float              MoveSpeed;
    [BoxGroup("Movement")] public float              JumpSpeed;
    [BoxGroup("Movement")] public float              JumpThreshold;
    [BoxGroup("Movement")] public float              MaxSpeed;
    [BoxGroup("Movement")] public float              GravityScale;
    
    [BoxGroup("Hanging")]  public float              HangingStartDuration;
    [BoxGroup("Hanging")]  public float              HangingSpeed;
    [BoxGroup("Hanging")]  public float              HangingCheckMinSpeed;
    
    [BoxGroup("Dash")] public float              DashManaCost;
    [BoxGroup("Dash")] public float              DashManaDecreaseSpeed;
    [BoxGroup("Dash")] public Ease              DashManaDecreaseEase;
    [BoxGroup("Dash")] public float              ManaFillAmount;
    [BoxGroup("Dash")] public DashDataDictionary DashDataDictionary;
    
    [Space(20)]
    
    public                        ColorVariables     ColorVars;
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