using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public event Action OnJumpCompleted;
    public event Action OnFireAnimEvent;

#region Refs

    [FoldoutGroup("Refs")] [SerializeField]
    private Animator m_Animator;

    [FoldoutGroup("Refs")] [SerializeField]
    private AnimationEventSender m_AnimEventSender;

#endregion

#region Unity Methods

    private void OnEnable()
    {
        m_AnimEventSender.OnJumpAnimCompleted += OnJumpAnimCompleted;
        m_AnimEventSender.OnFireAnimEvent     += OnFireAnimInvoked;
        m_AnimEventSender.OnFireAnimCompleted += OnFireAnimCompleted;
    }

    private void OnDisable()
    {
        m_AnimEventSender.OnJumpAnimCompleted -= OnJumpAnimCompleted;
        m_AnimEventSender.OnFireAnimEvent     -= OnFireAnimInvoked;
        m_AnimEventSender.OnFireAnimCompleted -= OnFireAnimCompleted;
    }

#endregion

#region Events

    private void OnJumpAnimCompleted()
    {
        OnJumpCompleted?.Invoke();
    }

    private void OnFireAnimInvoked()
    {
        OnFireAnimEvent?.Invoke();
    }

    private void OnFireAnimCompleted() { }

#endregion

    public void SetStateAnimation(eCharacterState i_State)
    {
        m_Animator.SetBool(AnimationHashes.S_Dash,  i_State == eCharacterState.Dash);
        m_Animator.SetBool(AnimationHashes.S_Hang,  i_State == eCharacterState.Hang);
        m_Animator.SetBool(AnimationHashes.S_OnAir, i_State is eCharacterState.OnAir or eCharacterState.Jump);
        m_Animator.SetBool(AnimationHashes.S_Walk,  i_State == eCharacterState.Walk);
        m_Animator.SetBool(AnimationHashes.s_Idle,  i_State == eCharacterState.Idle);
    }

    public void SetWalkAnimSpeed(float i_Speed)
    {
        m_Animator.SetFloat(AnimationHashes.S_WalkSpeed, i_Speed);
    }

    public void Hang(float i_Dur)
    {
        m_Animator.CrossFade(AnimationHashes.S_Hang, i_Dur);
    }

    public void Jump(eCharacterState i_State)
    {
        SetStateAnimation(i_State);
        m_Animator.SetTrigger(AnimationHashes.S_Jump);
    }

    public void Fire(eCharacterState i_State)
    {
        SetStateAnimation(i_State);
        m_Animator.SetTrigger(AnimationHashes.S_Throw);
    }
}