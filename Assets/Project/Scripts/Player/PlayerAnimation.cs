using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public event Action OnJumpCompleted;
    
    public event Action OnThrowCreateEvent;
    public event Action OnThrowAnimEvent;
    public event Action OnThrowAnimCompletedEvent;

#region Refs

    [FoldoutGroup("Refs")] [SerializeField]
    private Animator m_Animator;

    [FoldoutGroup("Refs")] [SerializeField]
    private AnimationEventSender m_AnimEventSender;

#endregion

#region Unity Methods

    private void OnEnable()
    {
        m_AnimEventSender.OnJumpAnimCompleted  += OnJumpAnimCompleted;
        m_AnimEventSender.OnThrowCreateInvoked += OnThrowCreateInvoked;
        m_AnimEventSender.OnThrowAnimInvoked     += OnThrowAnimInvoked;
        m_AnimEventSender.OnThrowAnimCompleted += OnThrowAnimCompleted;
    }

    private void OnDisable()
    {
        m_AnimEventSender.OnJumpAnimCompleted  -= OnJumpAnimCompleted;
        m_AnimEventSender.OnThrowCreateInvoked -= OnThrowCreateInvoked;
        m_AnimEventSender.OnThrowAnimInvoked     -= OnThrowAnimInvoked;
        m_AnimEventSender.OnThrowAnimCompleted -= OnThrowAnimCompleted;
    }

#endregion

#region Events

    private void OnJumpAnimCompleted()
    {
        OnJumpCompleted?.Invoke();
    }
    
    private void OnThrowCreateInvoked()
    {
        OnThrowCreateEvent?.Invoke();
    }
    
    private void OnThrowAnimInvoked()
    {
        OnThrowAnimEvent?.Invoke();
    }
    
    private void OnThrowAnimCompleted()
    {
        OnThrowAnimCompletedEvent?.Invoke();
    }

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
        m_Animator.CrossFade(AnimationHashes.S_Jump, 0.25f,0);
        m_Animator.CrossFade(AnimationHashes.S_Jump, 0.1f,1);
    }

    public void Throw(eCharacterState i_State)
    {
        SetStateAnimation(i_State);
        m_Animator.CrossFade(AnimationHashes.S_Throw, 0.25f);
    }
}