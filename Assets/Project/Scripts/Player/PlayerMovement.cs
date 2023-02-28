using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerMovement : Singleton<PlayerMovement>
{
    private static readonly int s_WalkSpeed = Animator.StringToHash("WalkSpeed");
    private static readonly int s_Walk      = Animator.StringToHash("Walk");
    private static readonly int s_OnAir     = Animator.StringToHash("OnAir");
    private static readonly int s_Hang      = Animator.StringToHash("Hang");
    private static readonly int s_Jump      = Animator.StringToHash("Jump");
    private static readonly int s_Dash      = Animator.StringToHash("Dash");
    private static readonly int s_Idle      = Animator.StringToHash("Idle");

    [ShowInInspector] private eCharacterState m_CharacterState;
    private                   float           m_HangTimer;
    private                   Vector2         m_LastDashDir;
    private                   Tweener         m_TwDash;
    private                   RaycastHit2D[]  m_MoveCheckCast = new RaycastHit2D[4];

    public bool  IsOnManaFillSpeed => Velocity > GameConfig.Instance.Mana.ManaFillMinVelocity;
    public float Velocity          => m_Rb.velocity.sqrMagnitude;

    private MovementVariables m_Movement => GameConfig.Instance.Movement;

    private Vector2 m_MoveDir => InputManager.Instance.PlayerMovement.ReadValue<Vector2>();

    private enum eCharacterState
    {
        Idle,
        Walk,
        Jump,
        OnAir,
        Dash,
        Hang
    }

#region Refs

    [FoldoutGroup("Refs")] [SerializeField]
    private Transform m_Model;

    [FoldoutGroup("Refs")] [SerializeField]
    private Animator m_Animator;

    [FoldoutGroup("Refs")] [SerializeField]
    private Rigidbody2D m_Rb;

    [FoldoutGroup("Refs")] [SerializeField]
    private CapsuleCollider2D m_Col;

    [FoldoutGroup("Refs")] [SerializeField]
    private LayerMask m_GroundLayer;

    [FoldoutGroup("Refs")] [SerializeField]
    private AnimationEventSender m_AnimEventSender;

#endregion

#region Unity Methods

    private void OnEnable()
    {
        InputManager.OnJumpInput            += OnJumpInput;
        InputManager.OnDashInput            += OnDashInput;
        m_AnimEventSender.JumpAnimCompleted += OnJumpCompleted;

        m_Rb.gravityScale = m_Movement.GravityScale;
        m_CharacterState  = eCharacterState.Idle;
    }

    private void OnDisable()
    {
        InputManager.OnJumpInput            -= OnJumpInput;
        InputManager.OnDashInput            -= OnDashInput;
        m_AnimEventSender.JumpAnimCompleted -= OnJumpCompleted;
    }

    private void FixedUpdate()
    {
        checkState();
        setAnimation();
        move();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        switch (m_CharacterState)
        {
            case eCharacterState.Dash:
                var angle = Vector2.Angle(m_LastDashDir, col.GetContact(0).normal);
                if (angle > 90)
                {
                    m_TwDash.Kill();
                    m_Animator.CrossFade(s_Idle, 0.05f);
                }

                break;
            case eCharacterState.Jump:
                m_CharacterState = eCharacterState.Idle;
                m_Animator.CrossFade(s_Idle, 0.05f);
                break;
        }

        checkState();
        setAnimation();
    }

#endregion

#region Events

    private void OnJumpInput()
    {
        var wallJump = m_CharacterState == eCharacterState.Hang || m_CharacterState == eCharacterState.OnAir && m_HangTimer > m_Movement.WallJumpMinHangTime;
        if (m_CharacterState is eCharacterState.Idle or eCharacterState.Walk || wallJump)
        {
            m_Animator.SetBool(s_Jump, true);
            m_CharacterState = eCharacterState.Jump;

            var force = Vector2.up * m_Movement.JumpSpeed;
            if (wallJump)
            {
                var scale = m_Model.transform.localScale;
                force += Vector2.right * Mathf.Sign(scale.x * -1) * m_Movement.JumpSpeed;
            }

            m_Rb.AddForce(force);
        }
    }

    private void OnDashInput()
    {
        dash();
    }

    private void OnJumpCompleted()
    {
        m_CharacterState = eCharacterState.Idle;
        checkState();
    }

#endregion

    private void dash()
    {
        if (m_CharacterState == eCharacterState.Dash) return;

        if (!ManaManager.Instance.IsManaEnough()) return;

        m_LastDashDir = m_MoveDir;
        var dashData = m_Movement.DashData;

        m_TwDash = m_Rb.DOMove(m_LastDashDir * dashData.DashAmount, dashData.DashSpeed)
                       .SetSpeedBased()
                       .SetUpdate(UpdateType.Fixed)
                       .SetRelative(true)
                       .SetEase(dashData.DashCurve)
                       .OnStart(() =>
                                {
                                    m_Rb.gravityScale = 0;
                                    m_Rb.velocity     = Vector2.zero;

                                    var scale = m_Model.transform.localScale;
                                    scale.x = m_LastDashDir.x < 0 ? -1.5f : m_LastDashDir.x > 0 ? 1.5f : scale.x;

                                    m_Model.transform.localScale = scale;
                                })
                       .OnKill(() =>
                               {
                                   m_TwDash          = null;
                                   m_Rb.gravityScale = m_Movement.GravityScale;
                                   var movementVel = Vector2.right * Mathf.Lerp(0, Mathf.Sign(m_LastDashDir.x), Mathf.Abs(m_LastDashDir.x));
                                   m_Rb.velocity = (m_LastDashDir + movementVel) * m_Movement.MaxSpeed;
                               });
    }

    private void move()
    {
        var vel = m_Rb.velocity;
        if (m_CharacterState != eCharacterState.Dash)
        {
            if (m_CharacterState != eCharacterState.Hang)
            {
                var newVelX = vel.x + m_MoveDir.x * m_Movement.MoveSpeed * Time.fixedDeltaTime;
                vel.x         = Mathf.Clamp(newVelX, -m_Movement.MaxSpeed, m_Movement.MaxSpeed);
                m_Rb.velocity = vel;


                m_Animator.SetFloat(s_WalkSpeed, Mathf.Lerp(0, m_Movement.AnimMaxWalkSpeed, (Mathf.Abs(vel.x) - m_Movement.WalkThreshold) / m_Movement.MaxSpeed));
            }
            else
            {
                vel.y         = Mathf.Max(vel.y, -m_Movement.HangingSpeed);
                m_Rb.velocity = vel;
            }
        }

        var scale = m_Model.transform.localScale;
        scale.x = m_MoveDir.x < 0 ? -1.5f : m_MoveDir.x > 0 ? 1.5f : scale.x;

        m_Model.transform.localScale = scale;
    }

    private void checkState()
    {
        if (m_TwDash != null && m_TwDash.IsPlaying())
        {
            m_CharacterState = eCharacterState.Dash;
        }
        else if (m_CharacterState != eCharacterState.Jump)
        {
            if (m_Col.IsTouchingLayers(m_GroundLayer)) //Touching Wall
            {
                var count = Physics2D.RaycastNonAlloc(m_Rb.position, Vector2.down, m_MoveCheckCast, 0.5f, m_GroundLayer);
                if (count > 0) //Touching Ground
                {
                    m_HangTimer = 0;
                    if (m_MoveDir.sqrMagnitude > 0)
                    {
                        if (m_CharacterState == eCharacterState.OnAir) m_Animator.CrossFade(s_Walk, 0.05f);
                        m_CharacterState = eCharacterState.Walk;
                    }

                    if (Mathf.Abs(m_Rb.velocity.x) < m_Movement.WalkThreshold && Mathf.Abs(m_MoveDir.x) <= 0)
                    {
                        m_CharacterState = eCharacterState.Idle;
                    }
                }
                else
                {
                    var countHang = Physics2D.RaycastNonAlloc(m_Col.bounds.center, Vector2.right * Mathf.Sign(m_Model.localScale.x), m_MoveCheckCast, m_Col.size.x - 0.1f, m_GroundLayer);
                    if (countHang > 0)
                    {
                        m_HangTimer += Time.fixedDeltaTime;
                        if (m_HangTimer > m_Movement.HangingStartDuration)
                        {
                            if (m_CharacterState == eCharacterState.OnAir) m_Animator.CrossFade(s_Hang, 0.05f);
                            m_CharacterState = eCharacterState.Hang;
                        }
                        else m_CharacterState = eCharacterState.OnAir;
                    }
                }
            }
            else //On Air
            {
                m_HangTimer      = 0;
                m_CharacterState = eCharacterState.OnAir;
            }
        }
    }

    private void setAnimation()
    {
        m_Animator.SetBool(s_Jump,  m_CharacterState == eCharacterState.Jump);
        m_Animator.SetBool(s_Dash,  m_CharacterState == eCharacterState.Dash);
        m_Animator.SetBool(s_Hang,  m_CharacterState == eCharacterState.Hang);
        m_Animator.SetBool(s_OnAir, m_CharacterState == eCharacterState.OnAir);
        m_Animator.SetBool(s_Walk,  m_CharacterState == eCharacterState.Walk);
    }

#region Joystick

    // private Vector2           m_MoveDir  => InputManager.Instance.JoystickDirection;
    // InputManager.OnInputActionDown   += OnInputActionDown;
    // InputManager.OnInputActionSwiped += OnInputActionSwiped;
    // InputManager.OnInputActionDown   -= OnInputActionDown;
    // InputManager.OnInputActionSwiped -= OnInputActionSwiped;

#endregion
}