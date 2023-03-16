using Managers;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    private eCharacterState m_CharacterState = eCharacterState.Idle;
    
    public float VelocityMag => m_Physics.Velocity.sqrMagnitude;
    
    private Vector2 m_MoveDir => InputManager.Instance.PlayerMovement.ReadValue<Vector2>().normalized;

#region Refs

    [FoldoutGroup("Refs")][SerializeField]
    private PlayerAnimation m_Animation;

    [FoldoutGroup("Refs")][SerializeField]
    private PlayerPhysics m_Physics;

#endregion
 

#region Unity Methods

    private void OnEnable()
    {
        InputManager.OnJumpInput        += OnJumpInput;
        InputManager.OnDashInput        += OnDashInput;
        InputManager.OnInteractionInput += OnInteractionInput;
        m_Physics.OnHangingStarted      += OnHangingStarted;
        m_Animation.JumpAnimCompleted   += OnJumpCompleted;

        m_Physics.SetGravityScale();
    }

    private void OnDisable()
    {
        InputManager.OnJumpInput        -= OnJumpInput;
        InputManager.OnDashInput        -= OnDashInput;
        InputManager.OnInteractionInput -= OnInteractionInput;
        m_Physics.OnHangingStarted      -= OnHangingStarted;
        m_Animation.JumpAnimCompleted   -= OnJumpCompleted;
    }
    
    private void OnCollisionEnter2D(Collision2D i_Col)
    {
        switch (m_CharacterState)
        {
            case eCharacterState.Dash:
                m_Physics.HitOnDash(ref m_CharacterState, i_Col.GetContact(0).normal, m_MoveDir);
                break;
            case eCharacterState.Jump:
                m_CharacterState = eCharacterState.Idle;
                break;
        }
    }
    
    private void FixedUpdate()
    {
        m_Physics.CheckState(ref m_CharacterState, m_MoveDir);
        m_Animation.SetAnimation(m_CharacterState, m_MoveDir);

        var walkSpeed = 0f;
        m_Physics.Move(m_CharacterState, m_MoveDir, ref walkSpeed);
        m_Animation.SetWalkAnimSpeed(walkSpeed);
    }
    
#endregion

#region Events

    private void OnHangingStarted(float i_Dur)
    {
        m_Animation.CrossFadeAnim(i_Dur);
    }

    private void OnJumpInput()
    {
        var cayoteJump = m_CharacterState == eCharacterState.OnAir && m_Physics.CanCayoteJump;
        if (m_CharacterState is eCharacterState.Idle or eCharacterState.Walk || cayoteJump)
        {
            m_CharacterState = eCharacterState.Jump;
            m_Physics.Jump();
        }
    }

    private void OnDashInput()
    {
        if (m_CharacterState == eCharacterState.Dash) return;
        if (!ManaManager.Instance.TryToUseMana()) return;

        m_CharacterState = eCharacterState.Dash;
        m_Physics.Dash(m_MoveDir);
    }
    
    private void OnInteractionInput()
    {
        if (m_CharacterState is not (eCharacterState.Idle or eCharacterState.Walk)) return;
        if (!ManaManager.Instance.TryToUseMana()) return;

    }
    
    private void OnJumpCompleted()
    {
        m_CharacterState = eCharacterState.Idle;
    }
    
#endregion

}
