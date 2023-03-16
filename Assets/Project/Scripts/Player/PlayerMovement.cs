using System;
using DG.Tweening;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerMovement : Singleton<PlayerMovement>
{
    private eCharacterState m_CharacterState;
    private float           m_HangTimer;
    private float           m_CayoteJumpTimer;
    private bool            m_IsMirrored;
    private Vector2         m_LastDashDir;
    private Tweener         m_TwDash;
    private Tweener         m_TwDashRotate;
    private RaycastHit2D[]  m_MoveCheckCast = new RaycastHit2D[2];
    private RaycastHit2D[]  m_HangCheckCast = new RaycastHit2D[4];

    public bool  IsOnManaFillSpeed => Velocity > GameConfig.Instance.Mana.ManaFillMinVelocity;
    public float Velocity          => m_Rb.velocity.sqrMagnitude;

    private MovementVariables m_Movement => GameConfig.Instance.Movement;

    private Vector2 m_MoveDir => InputManager.Instance.PlayerMovement.ReadValue<Vector2>().normalized;

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
        InputManager.OnInteractionInput     += OnInteractionInput;
        m_AnimEventSender.JumpAnimCompleted += OnJumpCompleted;

        m_Rb.gravityScale = m_Movement.GravityScale;
        m_CharacterState  = eCharacterState.Idle;
    }

    private void OnDisable()
    {
        InputManager.OnJumpInput            -= OnJumpInput;
        InputManager.OnDashInput            -= OnDashInput;
        InputManager.OnInteractionInput     -= OnInteractionInput;
        m_AnimEventSender.JumpAnimCompleted -= OnJumpCompleted;
    }

    private void FixedUpdate()
    {
        m_CayoteJumpTimer -= Time.fixedDeltaTime;

        checkState();
        setAnimation();
        move();
        rotate();
    }

    private void OnCollisionEnter2D(Collision2D i_Col)
    {
        switch (m_CharacterState)
        {
            case eCharacterState.Dash:
                var angle = Vector2.Angle(m_LastDashDir, i_Col.GetContact(0).normal);
                if (angle > 90) m_TwDash?.Kill();
                break;
            case eCharacterState.Jump:
                m_CharacterState = eCharacterState.Idle;
                break;
        }
    }

#endregion

#region Event Callbacks

    private void OnJumpInput()
    {
        var cayoteJump = m_CharacterState == eCharacterState.OnAir && m_CayoteJumpTimer > 0;
        if (m_CharacterState is eCharacterState.Idle or eCharacterState.Walk || cayoteJump)
        {
            m_CayoteJumpTimer = 0;
            jump();
        }
    }

    private void OnDashInput()
    {
        if (m_CharacterState == eCharacterState.Dash) return;
        if (!ManaManager.Instance.TryToUseMana()) return;
        
        dash();
    }

    private void OnInteractionInput()
    {
        if(m_CharacterState is not (eCharacterState.Idle or eCharacterState.Walk)) return;
        if (!ManaManager.Instance.TryToUseMana()) return;
        
    }

    private void OnJumpCompleted()
    {
        m_CharacterState = eCharacterState.Idle;
    }

#endregion

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
                m_Animator.SetFloat(AnimationHashes.S_WalkSpeed, Mathf.Lerp(0, m_Movement.AnimMaxWalkSpeed, (Mathf.Abs(vel.x) - m_Movement.WalkThreshold) / m_Movement.MaxSpeed));
            }
            else
            {
                var check = Mathf.Lerp(0.05f, m_Movement.HangingStartDuration, Mathf.InverseLerp(m_Movement.HangingSpeed, m_Movement.HangingSpeed * 2, Mathf.Abs(m_Rb.velocity.y)));
                if (m_HangTimer >= check)
                {
                    vel.y         = Mathf.Max(vel.y, -m_Movement.HangingSpeed);
                    m_Rb.velocity = vel;
                }
            }
        }

        var rot = m_Model.localRotation;
        rot.y                 = m_MoveDir.x < 0 ? 180 : m_MoveDir.x > 0 ? 0 : rot.y;
        m_Model.localRotation = rot;
    }

    private void rotate()
    {
        var targetRotation = Quaternion.identity;

        if (m_CharacterState is not (eCharacterState.Idle or eCharacterState.Walk))
        {
            var newRotation = Quaternion.Lerp(m_Rb.transform.rotation, targetRotation, m_Movement.RotationSpeed * Time.fixedDeltaTime);
            m_Rb.transform.rotation = newRotation;
        }
    }

    private void jump()
    {
        m_Animator.SetBool(AnimationHashes.S_Jump, true);
        m_CharacterState = eCharacterState.Jump;

        var vel = m_Rb.velocity;
        vel.y         = 0;
        m_Rb.velocity = vel;

        var force = Vector2.up * m_Movement.JumpSpeed;
        m_Rb.AddForce(force);
    }

    private void dash()
    {
        m_LastDashDir = m_MoveDir.sqrMagnitude > 0.05f ? m_MoveDir.normalized : m_Col.transform.right;
        var dashData = m_Movement.DashData;
        
        m_TwDash = m_Rb.DOMove(m_LastDashDir * dashData.DashAmount, dashData.DashSpeed)
                       .SetSpeedBased()
                       .SetUpdate(UpdateType.Fixed)
                       .SetRelative(true)
                       .SetEase(dashData.DashCurve)
                       .OnStart(() =>
                                {
                                    m_CharacterState  = eCharacterState.Dash;
                                    m_Rb.gravityScale = 0;
                                    m_Rb.velocity     = Vector2.zero;

                                    var rot = m_Model.localRotation;
                                    rot.y                 = m_LastDashDir.x < 0 ? 180 : m_LastDashDir.x > 0 ? 0 : rot.y;
                                    m_Model.localRotation = rot;

                                    if (m_LastDashDir.x != 0 && m_LastDashDir.x * m_LastDashDir.y != 0)
                                    {
                                        var endRot = dashData.RotateAmount * Mathf.Sign(m_LastDashDir.y);
                                        m_TwDashRotate = m_Col.transform.DOLocalRotate(new Vector3(0f, 0, endRot), dashData.RotateSpeed)
                                                              .SetSpeedBased()
                                                              .SetUpdate(UpdateType.Fixed);
                                    }
                                })
                       .OnKill(dashCompleted);
        
        m_CayoteJumpTimer = 0;
    }

    private void dashCompleted()
    {
        m_TwDash = null;

        m_Rb.gravityScale = m_Movement.GravityScale;

        m_TwDashRotate?.Kill();
        m_TwDashRotate = m_Col.transform.DOLocalRotate(Vector3.zero, m_Movement.DashData.RotateSpeed * 1.5f)
                              .SetSpeedBased()
                              .SetUpdate(UpdateType.Fixed);

        checkState();
        if (m_CharacterState != eCharacterState.Dash)
        {
            var movementVel = Vector2.right * Mathf.Lerp(0, Mathf.Sign(m_LastDashDir.x), Mathf.Abs(m_LastDashDir.x));
            m_Rb.velocity = (m_LastDashDir + movementVel) * m_Movement.MaxSpeed;
        }
    }

    private void checkState()
    {
        if (m_TwDash != null && m_TwDash.IsPlaying())
        {
            m_CharacterState = eCharacterState.Dash;
        }
        else if (m_CharacterState != eCharacterState.Jump)
        {
            bool onAir = false;

            Array.Clear(m_MoveCheckCast, 0, m_MoveCheckCast.Length);

            var count = Physics2D.RaycastNonAlloc(m_Col.bounds.center, -m_Col.transform.up, m_MoveCheckCast, m_Movement.GroundCheckDistance + m_Col.size.y / 2f, m_GroundLayer);
            if (count > 0) //Touching Ground
            {
                var onMovingPlatform = m_MoveCheckCast[0].transform.CompareTag(ObjectTags.S_MovingPlatform);

                var vel = m_Rb.velocity;
                if (onMovingPlatform)
                {
                    vel.y = m_MoveCheckCast[0].rigidbody.velocity.y;
                    if (Mathf.Abs(m_MoveDir.x) <= 0f) vel.x = m_MoveCheckCast[0].rigidbody.velocity.x;
                }
                else if (vel.sqrMagnitude > 0.1f) //Add friction
                {
                    vel -= vel.normalized * m_Movement.Friction;
                }
                else vel = Vector2.zero;

                m_Rb.velocity = vel;

                m_HangTimer       = 0;
                m_CayoteJumpTimer = m_Movement.CayoteJumpThreshold;

                if (Mathf.Abs(m_MoveDir.x) > 0 || (!onMovingPlatform && Mathf.Abs(m_Rb.velocity.x) > m_Movement.WalkThreshold))
                {
                    m_CharacterState = eCharacterState.Walk;
                }
                else
                {
                    m_CharacterState = eCharacterState.Idle;
                }

                var newRot = Quaternion.FromToRotation(Vector3.Lerp(Vector3.up, m_MoveCheckCast[0].normal, 0.5f), m_MoveCheckCast[0].normal);
                m_Rb.transform.rotation = Quaternion.Lerp(m_Rb.transform.rotation, newRot, m_Movement.RotationSpeed * Time.fixedDeltaTime);
            }
            else //Check if hang
            {
                Array.Clear(m_HangCheckCast, 0, m_HangCheckCast.Length);

                var angle           = Quaternion.AngleAxis(m_Col.transform.eulerAngles.z, Vector3.forward).eulerAngles.z;
                var raycastDistance = Mathf.Abs(m_Col.size.x / Mathf.Cos(Mathf.Deg2Rad * angle));

                var hitUp = Physics2D.RaycastNonAlloc(m_Col.bounds.center,
                                                      m_Col.transform.right, m_HangCheckCast,
                                                      raycastDistance,
                                                      m_GroundLayer) > 0;

                var hitDown = Physics2D.RaycastNonAlloc((Vector2) m_Col.bounds.center - (Vector2) m_Col.transform.up * (m_Col.size.y * 0.25f),
                                                        m_Col.transform.right, m_HangCheckCast,
                                                        raycastDistance,
                                                        m_GroundLayer) > 0;

                if (hitUp && hitDown) //Hanging
                {
                    m_HangTimer += Time.fixedDeltaTime;

                    if(m_CharacterState!=eCharacterState.Hang)
                    {
                        var dur = Mathf.Lerp(0.05f, m_Movement.HangingStartDuration,
                                             Mathf.InverseLerp(m_Movement.HangingSpeed, m_Movement.HangingSpeed * 2, Mathf.Abs(m_Rb.velocity.y)));
                        m_Animator.CrossFade(AnimationHashes.S_Hang, dur);
                        
                        m_CharacterState  = eCharacterState.Hang;
                        m_CayoteJumpTimer = m_Movement.CayoteJumpThreshold + dur;
                    }
                    else
                    {
                        m_CayoteJumpTimer = m_Movement.CayoteJumpThreshold;
                    }

                }
                else onAir = true;
            }

            if (onAir)
            {
                m_HangTimer      = 0;
                m_CharacterState = eCharacterState.OnAir;
            }
        }
    }

    private void setAnimation()
    {
        m_Animator.SetBool(AnimationHashes.S_Jump,  m_CharacterState == eCharacterState.Jump);
        m_Animator.SetBool(AnimationHashes.S_Dash,  m_CharacterState == eCharacterState.Dash);
        m_Animator.SetBool(AnimationHashes.S_Hang,  m_CharacterState == eCharacterState.Hang);
        m_Animator.SetBool(AnimationHashes.S_OnAir, m_CharacterState == eCharacterState.OnAir);
        m_Animator.SetBool(AnimationHashes.S_Walk,   m_CharacterState == eCharacterState.Walk);
    }
}