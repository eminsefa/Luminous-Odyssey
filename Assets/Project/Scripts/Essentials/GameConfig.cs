using System;
using DG.Tweening;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GameConfig")]
public class GameConfig : SingletonScriptableObject<GameConfig>
{
    public UIVariables       UI       = new();
    public InputVariables    Input    = new();
    public MovementVariables Movement = new();
    public ManaVariables     Mana = new();
}

[Serializable]
public class UIVariables
{
    
}

[Serializable]
public class InputVariables
{
    [HideIf(nameof(KeyboardInput))][OnValueChanged(nameof(OnInputReversed))]
    public bool ReverseInput;

    public bool  KeyboardInput;
    public float KeyboardInputSensitivity;

    [HideIf(nameof(KeyboardInput))] public JoystickData Joystick;

    [Serializable]
    public class JoystickData
    {
        public bool IsShowVisuals;

        public bool IsStatic;

        public bool IsResetDirection;

        public float Radius;
        public float HandleRadiusMultiplier;
    }

    public void OnInputReversed()
    {
        if(!KeyboardInput) StorageManager.Instance.IsInputReverse = ReverseInput;
    }
}

[Serializable]
public class MovementVariables
{
    [BoxGroup("Movement")] public float MoveSpeed;
    [BoxGroup("Movement")] public float JumpSpeed;
    [BoxGroup("Movement")] public float JumpThreshold;
    [BoxGroup("Movement")] public float MaxSpeed;
    [BoxGroup("Movement")] public float GravityScale;
    [BoxGroup("Movement")] public float AnimMaxWalkSpeed;

    [BoxGroup("Hanging")] public float HangingStartDuration;
    [BoxGroup("Hanging")] public float HangingSpeed;
    [BoxGroup("Hanging")] public float HangingCheckMinSpeed;

    [BoxGroup("Dash")] public int                DashManaCost;
    [BoxGroup("Dash")] public DashDataDictionary DashDataDictionary;
    

    [Space(20)] public LightVariables LightVars;
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
    public                float          DarkenSpeed;
    public                float          BrightenSpeed;
    public                AnimationCurve BrightnessCurve;
    [Range(0, 25)] public float          VisibilityFalloff = 1;
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

[Serializable] public class DashDataDictionary : UnitySerializedDictionary<InputManager.eSwipeDirections, DashData> { }