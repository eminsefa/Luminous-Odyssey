using System;
using DG.Tweening;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerMovement : Singleton<PlayerMovement>
{
    private static readonly int          s_IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int          s_WalkSpeed = Animator.StringToHash("WalkSpeed");
    
    private                 bool         m_IsDashing;
    private                 bool         m_IsHanging;
    private                 float        m_SideWallTimer;
    private                 RaycastHit[] m_MoveCheckCast = new RaycastHit[1];

    public bool  IsOnManaFillSpeed => Velocity > GameConfig.Instance.Mana.ManaFillMinVelocity;
    public float Velocity     => m_Rb.velocity.sqrMagnitude;

    private MovementVariables m_Movement => GameConfig.Instance.Movement;
    private Vector2           m_MoveDir  => InputManager.Instance.JoystickDirection;

#region Refs

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
        InputManager.OnInputActionDown   += OnInputActionDown;
        InputManager.OnInputActionSwiped += OnInputActionSwiped;

        m_Rb.gravityScale = m_Movement.GravityScale;
    }

    private void OnDisable()
    {
        InputManager.OnInputActionDown   -= OnInputActionDown;
        InputManager.OnInputActionSwiped -= OnInputActionSwiped;
    }
    
    private void FixedUpdate()
    {
        move();
        checkToHang();
    }

#endregion

#region Events

    private void OnInputActionDown(Vector2 vec)
    {
        if (Mathf.Abs(m_Rb.velocity.y) < m_Movement.JumpThreshold && !m_IsDashing) m_Rb.AddForce(Vector2.up * m_Movement.JumpSpeed);
    }

    private void OnInputActionSwiped(InputManager.eSwipeDirections direction)
    {
        dashDirection(direction);
    }

#endregion
    
    private void dashDirection(InputManager.eSwipeDirections direction)
    {
        if (m_IsDashing) return;

        if(!ManaManager.Instance.IsManaEnough()) return;
        
        var dashData = m_Movement.DashDataDictionary[direction];
        m_IsDashing = true;
        m_IsHanging = false;

        var dashAmount = dashData.DashAmount;
        var count      = Physics.RaycastNonAlloc(m_Rb.position, dashData.Direction, m_MoveCheckCast, dashAmount, m_GroundLayer);
        if (count > 0)
        {
            dashAmount = (m_MoveCheckCast[0].point - transform.position).magnitude;
        }

        m_Rb.DOMove(dashData.Direction * dashAmount, dashData.DashSpeed)
            .SetSpeedBased()
            .SetUpdate(UpdateType.Fixed)
            .SetRelative(true)
            .SetEase(dashData.DashCurve)
            .OnStart(() =>
                     {
                         m_Rb.gravityScale = 0;
                         m_Rb.velocity     = Vector2.zero;
                     })
            .OnComplete(() =>
                        {
                            m_IsDashing = false;

                            m_Rb.gravityScale = m_Movement.GravityScale;
                            var movementVel = Vector2.right * Mathf.Lerp(0, Mathf.Sign(m_MoveDir.x), Mathf.Abs(m_MoveDir.x));
                            m_Rb.velocity = (dashData.Direction + movementVel) * m_Movement.MaxSpeed;
                        });
    }

    private void move()
    {
        if (InputManager.Instance.IsJoystickDown && !m_IsDashing && !m_IsHanging)
        {
            m_Rb.AddForce(Vector2.right * (m_MoveDir.x * m_Movement.MoveSpeed));
            var vel = m_Rb.velocity;
            vel.x         = Mathf.Clamp(vel.x, -m_Movement.MaxSpeed, m_Movement.MaxSpeed);
            m_Rb.velocity = vel;
            m_Animator.SetBool(s_IsWalking,true);
            m_Animator.SetFloat(s_WalkSpeed, Mathf.Lerp(0, m_Movement.AnimMaxWalkSpeed,vel.x/m_Movement.MaxSpeed));
        }
        else m_Animator.SetBool(s_IsWalking, false);
    }

    private void checkToHang()
    {
        if (m_Col.IsTouchingLayers(m_GroundLayer) && m_Rb.velocity.y < -m_Movement.HangingCheckMinSpeed)
        {
            m_SideWallTimer += Time.fixedDeltaTime;
            if (m_SideWallTimer > m_Movement.HangingStartDuration && !m_IsDashing) m_IsHanging = true;
        }
        else
        {
            m_SideWallTimer = 0;
            m_IsHanging     = false;
        }

        if (m_IsHanging) m_Rb.velocity = Vector2.ClampMagnitude(m_Rb.velocity, m_Movement.HangingSpeed);
    }
}