using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig")]
public class GameConfig : SingletonScriptableObject<GameConfig>
{
    public DebugVariables    Debug     = new();
    public InputVariables    Input     = new();
    public MovementVariables Movement  = new();
    public ManaVariables     Mana      = new();
    public LightVariables    LightVars = new();
}

[Serializable]
public class DebugVariables
{
    public bool InfiniteMana;
    public bool GodMode;
}

[Serializable]
public class InputVariables
{
    public float JumpInputInterval = 0.05f;
    public float DashInputInterval = 0.05f;
}

[Serializable]
public class MovementVariables
{
    [BoxGroup("Movement")] public float WalkSpeed;
    [BoxGroup("Movement")] public float AirMoveSpeed;
    [BoxGroup("Movement")] public float JumpSpeed;
    [BoxGroup("Movement")] public float MaxSpeed;
    [BoxGroup("Movement")] public float RotationSpeed;
    [BoxGroup("Movement")] public float Friction            = 0.4f;
    [BoxGroup("Movement")] public float GroundCheckDistance = 0.1f;
    [BoxGroup("Movement")] public float GravityScale;
    [BoxGroup("Movement")] public float AnimMaxWalkSpeed;
    [BoxGroup("Movement")] public float WalkThreshold = 0.1f;

    [BoxGroup("Hanging")] public float HangingStartDuration;
    [BoxGroup("Hanging")] public float HangingSpeed;
    [BoxGroup("Hanging")] public float CayoteTime;

    [BoxGroup("Dash")] public DashData DashData;
}

[Serializable]
public class ManaVariables
{
    public float ManaFillAmount;
    public float ManaDrainAmount;
    public float ManaDrainMinIdleDuration;
    public float ManaFillMinVelocity;

    [BoxGroup("Visual")] public float ManaStackedClearSpeed = 0.25f;
    [BoxGroup("Visual")] public float ManaStackedClearDelay = 0.1f;
}

[Serializable]
public class LightVariables
{
    public               float          DarkenSpeed;
    public               float          BrightenSpeed;
    public               float          LightRange     = 7;
    public               float          PaintRangeMult = 1;
    public               float          MaskRangeMult  = 60;
    public               AnimationCurve BrightnessCurve;
    [Range(0, 2)] public float          VisibilityFalloff = 1;
}

[Serializable]
public class DashData
{
    public float          DashSpeed;
    public float          DashAmount;
    public float          RotateSpeed;
    public float          RotateAmount;
    public AnimationCurve DashCurve;
}