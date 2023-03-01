using System;
using DG.Tweening;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GameConfig")]
public class GameConfig : SingletonScriptableObject<GameConfig>
{
    public InputVariables    Input     = new();
    public MovementVariables Movement  = new();
    public ManaVariables     Mana      = new();
    public LightVariables    LightVars = new();
}

[Serializable] public class UIVariables { }

[Serializable]
public class InputVariables
{
#region Joystick

    // [OnValueChanged(nameof(OnInputReversed))]
    // public bool ReverseInput;

    // public JoystickData Joystick;
    //
    // [Serializable]
    // public class JoystickData
    // {
    //     public bool IsShowVisuals;
    //
    //     public bool IsStatic;
    //
    //     public bool IsResetDirection;
    //
    //     public float Radius;
    //     public float HandleRadiusMultiplier;
    // }

    // public void OnInputReversed()
    // {
    //     if(!KeyboardInput) StorageManager.Instance.IsInputReverse = ReverseInput;
    // }

#endregion
}

[Serializable]
public class MovementVariables
{
    [BoxGroup("Movement")] public float MoveSpeed;
    [BoxGroup("Movement")] public float JumpSpeed;
    [BoxGroup("Movement")] public float MaxSpeed;
    [BoxGroup("Movement")] public float GravityScale;
    [BoxGroup("Movement")] public float AnimMaxWalkSpeed;
    [BoxGroup("Movement")] public float WalkThreshold = 0.1f;

    [BoxGroup("Hanging")] public float HangingStartDuration;
    [BoxGroup("Hanging")] public float HangingSpeed;
    [BoxGroup("Hanging")] public float WalkingColMinDist;
    [BoxGroup("Hanging")] public float WallJumpMinHangTime;

    [BoxGroup("Dash")] public int      DashManaCost;
    [BoxGroup("Dash")] public DashData DashData;
}

[Serializable]
public class ManaVariables
{
    public float ManaFillAmount;
    public float ManaDrainAmount;
    public float ManaDrainMinIdleDuration;
    public float ManaStackedClearSpeed = 0.25f;
    public float ManaStackedClearDelay = 0.1f;
    public float ManaFillMinVelocity;
}

[Serializable]
public class LightVariables
{
    public               float          DarkenSpeed;
    public               float          BrightenSpeed;
    public               float          LightRange            = 10;
    public               float          PaintRangeMult        = 0.8f;
    public               float          ReverseLightRangeMult = 0.9f;
    public               AnimationCurve BrightnessCurve;
    [Range(0, 2)] public float          VisibilityFalloff = 0.8f;
}

[Serializable]
public class DashData
{
    public float          DashSpeed;
    public float          DashAmount;
    public float          RotateDuration;
    public float          RotateAmount;
    public AnimationCurve DashCurve;
}

// [Serializable] public class DashDataDictionary : UnitySerializedDictionary<InputManager.eSwipeDirections, DashData> { }