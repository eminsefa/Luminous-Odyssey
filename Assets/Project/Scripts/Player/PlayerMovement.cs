using System;
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

    [ShowInInspector] private eCharacterState m_CharacterState;
    private                   float           m_HangTimer;
    private                   Tweener         m_TwDash;
    private                   RaycastHit[]    m_MoveCheckCast = new RaycastHit[1];

    public bool  IsOnManaFillSpeed => Velocity > GameConfig.Instance.Mana.ManaFillMinVelocity;
    public float Velocity          => m_Rb.velocity.sqrMagnitude;

    private MovementVariables m_Movement => GameConfig.Instance.Movement;

    // private Vector2           m_MoveDir  => InputManager.Instance.JoystickDirection;
    private Vector2 m_MoveDir => InputManager.Instance.PlayerMovement.ReadValue<Vector2>();

    private enum eCharacterState
    {
        Idle,
        Walk,
        OnAir,
        Dash,
        Hang
    }

#region Refs

    [FoldoutGroup("Refs")] [SerializeField]
    private SpriteRenderer m_Renderer;

    [FoldoutGroup("Refs")] [SerializeField]
    private Animator m_Animator;

    [FoldoutGroup("Refs")] [SerializeField]
    private Rigidbody2D m_Rb;

    [FoldoutGroup("Refs")] [SerializeField]
    private Collider2D m_Col;

    [FoldoutGroup("Refs")] [SerializeField]
    private LayerMask m_GroundLayer;

#endregion

#region Unity Methods

    private void OnEnable()
    {
        // InputManager.OnInputActionDown   += OnInputActionDown;
        // InputManager.OnInputActionSwiped += OnInputActionSwiped;

        InputManager.OnJumpInput += OnJumpInput;
        InputManager.OnDashInput += OnDashInput;
        
        m_Rb.gravityScale        =  m_Movement.GravityScale;
        m_CharacterState         =  eCharacterState.Idle;
    }

    private void OnDisable()
    {
        // InputManager.OnInputActionDown   -= OnInputActionDown;
        // InputManager.OnInputActionSwiped -= OnInputActionSwiped;
        
        InputManager.OnJumpInput -= OnJumpInput;
        InputManager.OnDashInput -= OnDashInput;
    }

    private void FixedUpdate()
    {
        checkState();
        move();
    }

#endregion

#region Events

    private void OnJumpInput()
    {
        // if (Mathf.Abs(m_Rb.velocity.y) < m_Movement.JumpThreshold && m_CharacterState != eCharacterState.Dash)
        if (m_CharacterState != eCharacterState.Dash && m_CharacterState != eCharacterState.OnAir)
        {
            m_Animator.SetTrigger(s_Jump);
            m_Rb.AddForce(Vector2.up * m_Movement.JumpSpeed);
        }
    }

    private void OnDashInput()
    {
        dash();
    }

#endregion

    private void dash()
    {
        if (m_CharacterState == eCharacterState.Dash) return;

        if (!ManaManager.Instance.IsManaEnough()) return;

        var dashData = m_Movement.DashData;

        var dashAmount = dashData.DashAmount;
        var count      = Physics.RaycastNonAlloc(m_Rb.position, m_MoveDir, m_MoveCheckCast, dashAmount, m_GroundLayer);
        if (count > 0)
        {
            dashAmount = (m_MoveCheckCast[0].point - transform.position).magnitude;
        }

        m_TwDash = m_Rb.DOMove(m_MoveDir * dashAmount, dashData.DashSpeed)
                       .SetSpeedBased()
                       .SetUpdate(UpdateType.Fixed)
                       .SetRelative(true)
                       .SetEase(dashData.DashCurve)
                       .OnStart(() =>
                                {
                                    m_Rb.gravityScale = 0;
                                    m_Rb.velocity     = Vector2.zero;
                                    
                                    var scale = m_Renderer.transform.localScale;
                                    scale.x = m_MoveDir.x < 0 ? -1 : m_MoveDir.x > 0 ? 1 : scale.x;

                                    m_Renderer.transform.localScale = scale;
                                })
                       .OnKill(() =>
                               {
                                   m_TwDash          = null;
                                   m_Rb.gravityScale = m_Movement.GravityScale;
                                   var movementVel = Vector2.right * Mathf.Lerp(0, Mathf.Sign(m_MoveDir.x), Mathf.Abs(m_MoveDir.x));
                                   m_Rb.velocity = (m_MoveDir + movementVel) * m_Movement.MaxSpeed;
                               });
    }

    private void move()
    {
        if (m_CharacterState is eCharacterState.Idle or eCharacterState.Walk or eCharacterState.OnAir)
        {
            m_Rb.AddForce(Vector2.right * (m_MoveDir.x * m_Movement.MoveSpeed));
            var vel = m_Rb.velocity;
            vel.x         = Mathf.Clamp(vel.x, -m_Movement.MaxSpeed, m_Movement.MaxSpeed);
            m_Rb.velocity = vel;
        }

        var velX  = m_Rb.velocity.x;
        var scale = m_Renderer.transform.localScale;
        scale.x = velX < 0 ? -1 : velX > 0 ? 1 : scale.x;

        m_Renderer.transform.localScale = scale;

        m_Animator.SetFloat(s_WalkSpeed, Mathf.Lerp(0, m_Movement.AnimMaxWalkSpeed, (Mathf.Abs(velX) - m_Movement.WalkThreshold) / m_Movement.MaxSpeed));

        if (m_CharacterState == eCharacterState.Hang) m_Rb.velocity = Vector2.ClampMagnitude(m_Rb.velocity, m_Movement.HangingSpeed);
    }

    private void checkState()
    {
        if (m_TwDash != null && m_TwDash.IsPlaying())
        {
            m_CharacterState = eCharacterState.Dash;
        }
        else
        {
            if (m_Col.IsTouchingLayers(m_GroundLayer)) //Touching Wall
            {
                if (m_Rb.velocity.y < -m_Movement.HangingCheckMinSpeed) //Touching Side Wall
                {
                    m_HangTimer += Time.fixedDeltaTime;
                    if (m_HangTimer > m_Movement.HangingStartDuration)
                    {
                        if (m_CharacterState == eCharacterState.OnAir) m_Animator.CrossFade(s_Hang, 0.1f);
                        m_CharacterState = eCharacterState.Hang;
                    }
                }
                else //Touching Ground
                {
                    if (m_MoveDir.sqrMagnitude > 0)
                    {
                        if (m_CharacterState == eCharacterState.OnAir) m_Animator.CrossFade(s_Walk, 0.1f);
                        m_CharacterState = eCharacterState.Walk;
                    }

                    if (Mathf.Abs(m_Rb.velocity.x) < m_Movement.WalkThreshold)
                    {
                        m_CharacterState = eCharacterState.Idle;
                    }
                }
            }
            else //On Air
            {
                m_HangTimer      = 0;
                m_CharacterState = eCharacterState.OnAir;
            }
        }

        m_Animator.SetBool(s_Dash,  m_CharacterState == eCharacterState.Dash);
        m_Animator.SetBool(s_Hang,  m_CharacterState == eCharacterState.Hang);
        m_Animator.SetBool(s_OnAir, m_CharacterState == eCharacterState.OnAir);
        m_Animator.SetBool(s_Walk,  m_CharacterState == eCharacterState.Walk);
    }
}