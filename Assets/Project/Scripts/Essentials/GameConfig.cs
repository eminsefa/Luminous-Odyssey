using System;
using DG.Tweening;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GameConfig")]
public class GameConfig : SingletonScriptableObject<GameConfig>
{
    public InputVariables    Input    = new();
    public MovementVariables Movement = new();
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

    [BoxGroup("Hanging")] public float HangingStartDuration;
    [BoxGroup("Hanging")] public float HangingSpeed;
    [BoxGroup("Hanging")] public float HangingCheckMinSpeed;

    [BoxGroup("Mana")] public float DashManaCost;
    [BoxGroup("Mana")] public float DashManaDrainSpeed;
    [BoxGroup("Mana")] public Ease  DashManaDrainEase;
    [BoxGroup("Mana")] public float ManaFillAmount;
    [BoxGroup("Mana")] public float ManaDrainAmount;
    [BoxGroup("Mana")] public float ManaFillMinVelocity;
    [BoxGroup("Mana")] public float ManaDrainMinIdleDuration;

    [BoxGroup("Dash")] public DashDataDictionary DashDataDictionary;

    [Space(20)] public LightVariables LightVars;
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