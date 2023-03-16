using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public delegate void JumpAnimCompletedEvent();
    public event JumpAnimCompletedEvent JumpAnimCompleted;

#region Refs

    [FoldoutGroup("Refs")][SerializeField]
    private Transform m_Model;
    
    [FoldoutGroup("Refs")][SerializeField]
    private Animator m_Animator;

    [FoldoutGroup("Refs")][SerializeField]
    private AnimationEventSender m_AnimEventSender;

#endregion
    
    private void OnEnable()
    {
        m_AnimEventSender.OnJumpAnimCompleted += OnJumpAnimCompleted;
    }

    private void OnDisable()
    {
        m_AnimEventSender.OnJumpAnimCompleted -= OnJumpAnimCompleted;
    }
    
    private void OnJumpAnimCompleted()
    {
        JumpAnimCompleted?.Invoke();
    }
    
    public void SetAnimation(eCharacterState i_State, Vector2 i_MoveDir)
    {
        m_Animator.SetBool(AnimationHashes.S_Jump,  i_State == eCharacterState.Jump);
        m_Animator.SetBool(AnimationHashes.S_Dash,  i_State == eCharacterState.Dash);
        m_Animator.SetBool(AnimationHashes.S_Hang,  i_State == eCharacterState.Hang);
        m_Animator.SetBool(AnimationHashes.S_OnAir, i_State == eCharacterState.OnAir);
        m_Animator.SetBool(AnimationHashes.S_Walk,  i_State == eCharacterState.Walk);
        
        rotate(i_MoveDir);
    }
    
    private void rotate(Vector2 i_MoveDir)
    {
        var rot = m_Model.localRotation;
        rot.y                 = i_MoveDir.x < 0 ? 180 : i_MoveDir.x > 0 ? 0 : rot.y;
        m_Model.localRotation = rot;
    }

    public void SetWalkAnimSpeed(float i_Speed)
    {
        m_Animator.SetFloat(AnimationHashes.S_WalkSpeed, i_Speed);

    }
    
    public void CrossFadeAnim(float i_Dur)
    {
        m_Animator.CrossFade(AnimationHashes.S_Hang, i_Dur);

    }
}
