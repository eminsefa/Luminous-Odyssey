using System;
using System.Collections.Generic;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    private eCharacterState m_CharacterState = eCharacterState.Idle;

    public Vector2 Velocity => m_Physics.Velocity;

    private Vector2 m_MoveDir => InputManager.Instance.PlayerMovement.ReadValue<Vector2>().normalized;

#region Refs

    [FoldoutGroup("Refs")] [SerializeField]
    private PlayerInput m_Input;

    [FoldoutGroup("Refs")] [SerializeField]
    private PlayerAnimation m_Animation;

    [FoldoutGroup("Refs")] [SerializeField]
    private PlayerPhysics m_Physics;

    [FoldoutGroup("Refs")] [SerializeField]
    private PlayerAction m_Action;

#endregion


#region Unity Methods

    private void OnEnable()
    {
        m_Input.OnJumpInputEvent        += OnJumpInput;
        m_Input.OnDashInputEvent        += OnDashInput;
        m_Input.OnInteractionInputEvent += OnInteractionInput;
        m_Input.OnThrowInputEvent       += OnThrowInput;

        m_Animation.OnJumpCompleted           += OnJumpCompleted;
        m_Animation.OnThrowCreateEvent        += OnThrowCreateEvent;
        m_Animation.OnThrowAnimEvent          += OnThrowAnimEvent;
        m_Animation.OnThrowAnimCompletedEvent += OnThrowAnimCompletedEvent;

        m_Physics.OnHangingStarted += OnHangingStarted;
        m_Physics.SetGravityScale();
    }

    private void OnDisable()
    {
        m_Input.OnJumpInputEvent        -= OnJumpInput;
        m_Input.OnDashInputEvent        -= OnDashInput;
        m_Input.OnInteractionInputEvent -= OnInteractionInput;
        m_Input.OnThrowInputEvent       -= OnThrowInput;

        m_Animation.OnJumpCompleted           -= OnJumpCompleted;
        m_Animation.OnThrowCreateEvent        -= OnThrowCreateEvent;
        m_Animation.OnThrowAnimCompletedEvent -= OnThrowAnimCompletedEvent;

        m_Physics.OnHangingStarted -= OnHangingStarted;
    }

    private void OnCollisionEnter2D(Collision2D i_Col)
    {
        m_CharacterState = m_Physics.Hit(m_CharacterState, i_Col, m_MoveDir);
    }

    private void FixedUpdate()
    {
        m_CharacterState = m_Physics.CheckState(m_CharacterState, m_MoveDir);

        m_Animation.SetStateAnimation(m_CharacterState);

        var walkSpeed = 0f;
        m_Physics.MoveHorizontal(m_CharacterState, m_MoveDir, ref walkSpeed);
        m_Animation.SetWalkAnimSpeed(walkSpeed);
    }

#endregion

#region Events

    private void OnJumpInput()
    {
        var cayoteJump = m_CharacterState == eCharacterState.OnAir && m_Physics.CanCayoteJump;
        if (m_CharacterState is not (eCharacterState.Idle or eCharacterState.Walk) && !cayoteJump) return;

        m_CharacterState = eCharacterState.Jump;
        m_Animation.Jump(m_CharacterState);
        m_Physics.Jump();
    }

    private void OnDashInput()
    {
        if (m_CharacterState == eCharacterState.Dash) return;
        if (!ManaManager.Instance.TryToUseMana()) return;

        m_CharacterState = eCharacterState.Dash;
        m_Animation.SetStateAnimation(m_CharacterState);
        m_Physics.Dash(m_MoveDir);
    }

    private void OnInteractionInput()
    {
        if (m_CharacterState is not (eCharacterState.Idle or eCharacterState.Walk)) return;
        m_Action.Interact();
    }

    private void OnThrowInput()
    {
        if (!ManaManager.Instance.TryToUseMana()) return;

        m_CharacterState = eCharacterState.Throw;
        m_Animation.Throw(m_CharacterState);
    }

    private void OnJumpCompleted()
    {
        m_CharacterState = eCharacterState.Idle;
    }

    private void OnThrowCreateEvent()
    {
        if (m_CharacterState is eCharacterState.Dash) return;
        m_CharacterState = eCharacterState.Throw;
        m_Action.ThrowCreateMana();
    }

    private void OnThrowAnimEvent()
    {
        if (m_CharacterState is eCharacterState.Dash) return;
        m_CharacterState = eCharacterState.Throw;
        var throwDir = m_MoveDir.sqrMagnitude > 0.1f ? m_MoveDir.normalized : m_Physics.LookDir;
        m_Action.ThrowMana(throwDir);
    }
    
    private void OnThrowAnimCompletedEvent()
    {
        m_CharacterState = eCharacterState.Idle;
    }
    
    private void OnHangingStarted(float i_Dur)
    {
        if (m_CharacterState is eCharacterState.Dash) return;
        m_Animation.Hang(i_Dur);
    }

#endregion
}