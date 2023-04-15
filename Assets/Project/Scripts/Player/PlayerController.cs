using System;
using System.Collections.Generic;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    [ShowInInspector] private eCharacterState m_CharacterState = eCharacterState.Null;

    public  bool      IsBlind           => Input.GetKey(KeyCode.LeftShift) && m_Light.CloseEyeIter > 0.95f;
    public  Transform ManaObjectsHolder => m_Action.ManaObjectsHolder;
    public  Vector2   LightPos          => m_Light.transform.position;
    public  Vector2   Velocity          => m_Physics.Velocity;
    private Vector2   m_MoveVector      => InputManager.Instance.PlayerMovement.ReadValue<Vector2>();

#region Refs

    [FoldoutGroup("Refs")] [SerializeField]
    private PlayerInput m_Input;

    [FoldoutGroup("Refs")] [SerializeField]
    private PlayerAnimation m_Animation;

    [FoldoutGroup("Refs")] [SerializeField]
    private PlayerPhysics m_Physics;

    [FoldoutGroup("Refs")] [SerializeField]
    private PlayerAction m_Action;

    [FoldoutGroup("Refs")] [SerializeField]
    private PlayerLight m_Light;

#endregion


#region Unity Methods

    private void OnEnable()
    {
        m_Input.OnJumpInputEvent        += OnJumpInput;
        m_Input.OnDashInputEvent        += OnDashInput;
        m_Input.OnInteractionInputEvent += OnInteractionInput;
        m_Input.OnThrowInputEvent       += OnThrowInput;
        m_Input.OnSlowWalkInputEvent    += OnSlowWalkInput;

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
        m_Input.OnSlowWalkInputEvent    -= OnSlowWalkInput;

        m_Animation.OnJumpCompleted           -= OnJumpCompleted;
        m_Animation.OnThrowCreateEvent        -= OnThrowCreateEvent;
        m_Animation.OnThrowAnimCompletedEvent -= OnThrowAnimCompletedEvent;

        m_Physics.OnHangingStarted -= OnHangingStarted;
    }

    private void OnCollisionEnter2D(Collision2D i_Col)
    {
        m_CharacterState = m_Physics.Hit(m_CharacterState, i_Col, m_MoveVector.normalized);
    }

    private void FixedUpdate()
    {
        m_CharacterState = m_Physics.CheckState(m_CharacterState, m_MoveVector);

        m_Animation.SetStateAnimation(m_CharacterState);

        m_Physics.MoveHorizontal(m_CharacterState, m_MoveVector);
    }

    private void Update()
    {
        var isBlind = Input.GetKey(KeyCode.LeftShift);
        var walking   = m_CharacterState is eCharacterState.Walk;
        m_Physics.SetSlowWalk(isBlind && walking);

        m_Light.SetMoveSpeed(m_Physics.Velocity.x, isBlind);
        m_Animation.SetWalkAnimSpeed(m_Physics.VelAnimSpeedIter, m_Physics.SlowWalkBlendIter);
    }

#endregion

#region Events

    private void OnJumpInput()
    {
        var cayoteJump = m_CharacterState == eCharacterState.OnAir && m_Physics.CanCayoteJump;
        if (m_CharacterState is not (eCharacterState.Idle or eCharacterState.Walk) && !cayoteJump) return;

        InputManager.Instance.SetInputTimer(0);

        m_Animation.Jump(m_CharacterState);
        m_CharacterState = eCharacterState.Jump;

        m_Physics.Jump();
    }

    private void OnDashInput()
    {
        if (m_CharacterState is eCharacterState.Dash or eCharacterState.Throw) return;
        if (!ManaManager.Instance.TryToUseMana()) return;
        InputManager.Instance.SetInputTimer(1);
        m_CharacterState = eCharacterState.Dash;
        m_Animation.SetStateAnimation(m_CharacterState);

        var dashDir = m_MoveVector.sqrMagnitude > 0.1f ? m_MoveVector : m_Physics.LookDir;
        m_Physics.Dash(dashDir);
    }

    private void OnInteractionInput()
    {
        if (m_CharacterState is not (eCharacterState.Idle or eCharacterState.Walk)) return;
        InputManager.Instance.SetInputTimer(2);
        m_Action.Interact();
    }

    private void OnThrowInput()
    {
        if (m_CharacterState is eCharacterState.Throw) return;
        if (!ManaManager.Instance.TryToUseMana()) return;

        InputManager.Instance.SetInputTimer(3);
        m_Animation.Throw(m_CharacterState);
        m_CharacterState = eCharacterState.Throw;
    }

    private void OnSlowWalkInput(bool i_Started)
    {
        // if(m_CharacterState!=eCharacterState.Walk) return;
        // m_Physics.GetSlowWalkVel(i_Started);
        // m_Animation.SetSlowWalk(i_Started);
    }

    private void OnJumpCompleted()
    {
        if (m_CharacterState == eCharacterState.Jump)
        {
            m_CharacterState = eCharacterState.Null;
            m_Animation.SetStateAnimation(m_CharacterState);
        }
    }

    private void OnThrowCreateEvent()
    {
        m_Action.ThrowCreateMana();
    }

    private void OnThrowAnimEvent()
    {
        var throwDir = m_MoveVector.sqrMagnitude > 0.1f ? m_MoveVector.normalized : m_Physics.LookDir;
        m_Action.ThrowMana(throwDir);
    }

    private void OnThrowAnimCompletedEvent()
    {
        if (m_CharacterState == eCharacterState.Throw)
        {
            m_CharacterState = eCharacterState.Null;
            m_Animation.SetStateAnimation(m_CharacterState);
        }
    }

    private void OnHangingStarted(float i_Dur)
    {
        // if (m_CharacterState is eCharacterState.Dash) return;
        m_Animation.Hang(m_CharacterState, i_Dur);
        m_CharacterState = eCharacterState.Hang;
    }

#endregion
}