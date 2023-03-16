using UnityEngine;

public struct AnimationHashes
{
    public static readonly int S_WalkSpeed = Animator.StringToHash("WalkSpeed");
    public static readonly int S_Walk      = Animator.StringToHash("Walk");
    public static readonly int S_OnAir     = Animator.StringToHash("OnAir");
    public static readonly int S_Hang      = Animator.StringToHash("Hang");
    public static readonly int S_Jump      = Animator.StringToHash("Jump");
    public static readonly int S_Dash      = Animator.StringToHash("Dash");
    public static readonly int s_Idle      = Animator.StringToHash("Idle");
}