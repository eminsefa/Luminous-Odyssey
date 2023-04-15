using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig")]
public class GameConfig : SingletonScriptableObject<GameConfig>
{
    [BoxGroup("Player"),Space(5)] public MovementVariables Movement  = new();
    [BoxGroup("Player"),Space(5)] public ActionVariables   Action    = new();
    [BoxGroup("Player"),Space(5)] public ManaVariables     Mana      = new();
    [BoxGroup("Player"),Space(5)] public LightVariables    LightVars = new();
    [BoxGroup("Others"),Space(5)] public InputVariables    Input     = new();
    [BoxGroup("Others"),Space(5)] public DebugVariables    Debug     = new();
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
    public InputInterval[] InputIntervals;
    
    [Serializable]
    public class InputInterval
    {
        public string Header;
        public float  Value ;
    }
}

[Serializable]
public class MovementVariables
{
    [BoxGroup("Movement")] public float MoveSpeedThreshold = 0.05f;
    [BoxGroup("Movement")] public float WalkSpeed;
    [BoxGroup("Movement")] public float MaxSpeed;
    [BoxGroup("Movement")] public float SlowWalkMaxSpeed;
    [BoxGroup("Movement")] public float SlowWalkSpeed;
    [BoxGroup("Movement")] public float AirMoveSpeed;
    [BoxGroup("Movement")] public float JumpPower;
    [BoxGroup("Movement")] public float RotationSpeed;
    [BoxGroup("Movement")] public float Friction            = 0.4f;
    [BoxGroup("Movement")] public float GroundCheckDistance = 0.1f;
    [BoxGroup("Movement")] public float GravityScale;

    [BoxGroup("Hang")] public float HangingStartDuration;
    [BoxGroup("Hang")] public float HangingSpeed;
    [BoxGroup("Hang")] public float CayoteTime;

    [BoxGroup("Dash")] public DashData DashData;
}

[Serializable]
public class ActionVariables
{
    [BoxGroup("Place Mana")] public float ManaPlaceDuration;
    [BoxGroup("Place Mana")] public float ManaPlaceJumpPower;
    [BoxGroup("Place Mana")] public float ManaPlaceDelay;
    [BoxGroup("Place Mana")] public Ease  ManaPlaceEase;
    [BoxGroup("Place Mana")] public float ManaPlaceReturnSpeed;
    
    [BoxGroup("Throw Mana")] public float ThrowSpeed;
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
    public                          float          DarkenSpeed;
    public                          float          BrightenSpeed;
    public                          float          LightRange     = 7;
    public                          float          PaintRangeMult = 1;
    public                          float          MaskRangeMult  = 60;
    public                          AnimationCurve BrightnessCurve;
    [Range(0, 2)]            public float          VisibilityFalloff       = 1;

    public float MoveLightThreshold      = 0.1f;
    public float ThrowManaLightRangeMult = 0.5f;
    public float SlowWalkDarkenSpeed     = 0.5f;
    public float ThrowManaLightRange => LightRange * MaskRangeMult *ThrowManaLightRangeMult;
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