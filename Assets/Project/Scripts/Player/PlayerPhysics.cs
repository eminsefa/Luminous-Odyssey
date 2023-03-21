using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    public event Action<float> OnHangingStarted;

    public  bool              CanCayoteJump => m_CayoteJumpTimer > 0f;
    public  Vector2           Velocity      => m_Rb.velocity;
    public  Vector2           LookDir       => m_Col.transform.right;
    private MovementVariables m_Movement    => GameConfig.Instance.Movement;

    private float          m_HangTimer;
    private float          m_CayoteJumpTimer;
    private Vector2        m_LastDashDir;
    private Tweener        m_TwDash;
    private Tweener        m_TwDashRotate;
    private RaycastHit2D[] m_MoveCheckCast = new RaycastHit2D[2];
    private RaycastHit2D[] m_HangCheckCast = new RaycastHit2D[4];

#region Refs

    [FoldoutGroup("Refs")] [SerializeField]
    private Transform m_Model;

    [FoldoutGroup("Refs")] [SerializeField]
    private CapsuleCollider2D m_Col;

    [FoldoutGroup("Refs")] [SerializeField]
    private Rigidbody2D m_Rb;

    [FoldoutGroup("Refs")] [SerializeField]
    private LayerMask m_GroundLayer;

#endregion

    private void Update()
    {
        m_CayoteJumpTimer -= Time.deltaTime;
    }

    public void MoveHorizontal(eCharacterState i_State, Vector2 i_MoveDir, ref float i_MoveSpeed)
    {
        if (i_State == eCharacterState.Dash) return;

        var vel = m_Rb.velocity;
        if (i_State != eCharacterState.Hang)
        {
            var speed   = i_State == eCharacterState.OnAir ? m_Movement.AirMoveSpeed : m_Movement.WalkSpeed;
            var newVelX = vel.x + i_MoveDir.x * speed * Time.fixedDeltaTime;
            vel.x         = Mathf.Clamp(newVelX, -m_Movement.MaxSpeed, m_Movement.MaxSpeed);
            m_Rb.velocity = vel;

            var moving = i_MoveDir.sqrMagnitude > m_Movement.MoveSpeedThreshold;
            i_MoveSpeed = moving ? Mathf.Lerp(0, m_Movement.AnimMaxWalkSpeed, (Mathf.Abs(vel.x) - m_Movement.MoveSpeedThreshold) / m_Movement.MaxSpeed) : 0;

            rotateBody(i_State, i_MoveDir);
        }
        else
        {
            var check = Mathf.Lerp(0.05f, m_Movement.HangingStartDuration, Mathf.InverseLerp(m_Movement.HangingSpeed, m_Movement.HangingSpeed * 2, Mathf.Abs(m_Rb.velocity.y)));
            if (m_HangTimer >= check)
            {
                vel.y         = Mathf.Max(vel.y, -m_Movement.HangingSpeed);
                m_Rb.velocity = vel;
                i_MoveSpeed   = 0;
            }
        }
    }

    public void Jump()
    {
        m_CayoteJumpTimer = 0f;
        m_Rb.velocity     = new Vector2(m_Rb.velocity.x, 0f);
        m_Rb.AddForce(Vector2.up * m_Movement.JumpPower, ForceMode2D.Impulse);
    }

#region Hang

    private bool checkHang()
    {
        Array.Clear(m_HangCheckCast, 0, m_HangCheckCast.Length);

        var angle           = Quaternion.AngleAxis(m_Col.transform.eulerAngles.z, Vector3.forward).eulerAngles.z;
        var raycastDistance = Mathf.Abs(m_Col.size.x / Mathf.Cos(Mathf.Deg2Rad * angle));
        var dir             = m_Col.transform.right;

        var hitUp = Physics2D.RaycastNonAlloc(m_Col.bounds.center,
                                              dir, m_HangCheckCast,
                                              raycastDistance,
                                              m_GroundLayer) > 0;

        if (!hitUp) return false;
        var hitDown = Physics2D.RaycastNonAlloc((Vector2) m_Col.bounds.center - (Vector2) m_Col.transform.up * (m_Col.size.y * 0.25f),
                                                dir, m_HangCheckCast,
                                                raycastDistance,
                                                m_GroundLayer) > 0;
        return hitDown;
    }

    private eCharacterState updateHangState(eCharacterState i_State)
    {
        m_HangTimer += Time.fixedDeltaTime;

        if (i_State == eCharacterState.Hang)
        {
            m_CayoteJumpTimer = m_Movement.CayoteTime;
            return i_State;
        }
        else
        {
            var dur = Mathf.Lerp(0.05f, m_Movement.HangingStartDuration,
                                 Mathf.InverseLerp(m_Movement.HangingSpeed, m_Movement.HangingSpeed * 2, Mathf.Abs(m_Rb.velocity.y)));
            OnHangingStarted?.Invoke(dur);

            m_CayoteJumpTimer = m_Movement.CayoteTime + dur;
            return eCharacterState.Hang;
        }
    }

#endregion

#region Dash

    public void Dash(Vector2 i_DashDir)
    {
        m_LastDashDir = i_DashDir;
        var dashData = m_Movement.DashData;
        m_TwDash = m_Rb.DOMove(i_DashDir * dashData.DashAmount, dashData.DashSpeed)
                       .SetSpeedBased()
                       .SetUpdate(UpdateType.Fixed)
                       .SetRelative(true)
                       .SetEase(dashData.DashCurve)
                       .OnStart(() => dashStarted(i_DashDir, dashData))
                       .OnKill(dashCompleted);
    }

    private void dashStarted(Vector3 i_DashDir, DashData i_DashData)
    {
        m_Rb.gravityScale = 0;
        m_Rb.velocity     = Vector2.zero;

        if (i_DashDir.x != 0 && i_DashDir.x * i_DashDir.y != 0)
        {
            var endRot = i_DashData.RotateAmount * Mathf.Sign(i_DashDir.y);
            m_TwDashRotate = m_Col.transform.DOLocalRotate(new Vector3(0f, 0, endRot), i_DashData.RotateSpeed)
                                  .SetSpeedBased()
                                  .SetUpdate(UpdateType.Fixed);
        }
    }

    private void dashCompleted()
    {
        m_TwDash = null;

        m_Rb.gravityScale = m_Movement.GravityScale;

        m_TwDashRotate?.Kill();
        m_TwDashRotate = m_Col.transform.DOLocalRotate(Vector3.zero, m_Movement.DashData.RotateSpeed * 1.5f)
                              .SetSpeedBased()
                              .SetUpdate(UpdateType.Fixed);

        var movementVel = Vector2.right * Mathf.Lerp(0, Mathf.Sign(m_LastDashDir.x), Mathf.Abs(m_LastDashDir.x));
        m_Rb.velocity = (m_LastDashDir + movementVel) * m_Movement.MaxSpeed;
    }

#endregion

#region State Machine

    public eCharacterState CheckState(eCharacterState i_State, Vector2 i_MoveDir)
    {
        if (i_State != eCharacterState.Dash) flipModel(i_MoveDir);
        if (i_State is (eCharacterState.Jump or eCharacterState.Throw)) return i_State;

        if (m_TwDash != null && m_TwDash.IsPlaying())
        {
             return eCharacterState.Dash;
        }
        int count = checkGround();
        if (count > 0) // Touching Ground
        {
            m_HangTimer = 0;
            return updateGroundedState(i_State, i_MoveDir);
        }
        if (checkHang()) return updateHangState(i_State); // Hanging
        
        m_HangTimer = 0;
        return eCharacterState.OnAir;
    }

    private int checkGround()
    {
        Array.Clear(m_MoveCheckCast, 0, m_MoveCheckCast.Length);
        var onGround = Physics2D.RaycastNonAlloc(m_Col.bounds.center, -m_Col.transform.up, m_MoveCheckCast, m_Movement.GroundCheckDistance + m_Col.size.y / 2f, m_GroundLayer);
        return onGround;
    }

    private eCharacterState updateGroundedState(eCharacterState i_State, Vector2 i_MoveDir)
    {
        var onMovingPlatform = m_MoveCheckCast[0].transform.CompareTag(ObjectTags.S_MovingPlatform);

        var vel = m_Rb.velocity;
        if (onMovingPlatform)
        {
            vel.y = m_MoveCheckCast[0].rigidbody.velocity.y;
            if (Mathf.Abs(i_MoveDir.x) <= 0f) vel.x = m_MoveCheckCast[0].rigidbody.velocity.x;
        }
        else if (vel.sqrMagnitude > m_Movement.MoveSpeedThreshold) //Add friction
        {
            vel -= vel.normalized * m_Movement.Friction;
        }
        else vel = Vector2.zero;

        m_Rb.velocity = vel;

        m_CayoteJumpTimer = m_Movement.CayoteTime;

        if (Mathf.Abs(i_MoveDir.x) > 0 || (!onMovingPlatform && Mathf.Abs(m_Rb.velocity.x) > m_Movement.MoveSpeedThreshold))
        {
            return eCharacterState.Walk;
        }
        else
        {
            return eCharacterState.Idle;
        }
    }

#endregion

    private void rotateBody(eCharacterState i_State, Vector2 i_MoveDir)
    {
        var targetRotation = Quaternion.identity;

        if (i_State is not (eCharacterState.Idle or eCharacterState.Walk))
        {
            if (Quaternion.Angle(targetRotation, m_Rb.transform.rotation) > 1)
            {
                var newRotation = Quaternion.Lerp(m_Rb.transform.rotation, targetRotation, m_Movement.RotationSpeed * Time.fixedDeltaTime);
                m_Rb.transform.rotation = newRotation;
            }
        }
        else
        {
            if (Vector2.Angle(m_Rb.transform.up, m_MoveCheckCast[0].normal) > 1)
            {
                targetRotation          = Quaternion.FromToRotation(Vector3.Lerp(Vector3.up, m_MoveCheckCast[0].normal, 0.5f), m_MoveCheckCast[0].normal);
                m_Rb.transform.rotation = Quaternion.Lerp(m_Rb.transform.rotation, targetRotation, m_Movement.RotationSpeed * Time.fixedDeltaTime);
            }
        }

        if (i_State is not (eCharacterState.Hang or eCharacterState.Throw or eCharacterState.Dash)) flipModel(i_MoveDir);
    }

    private void flipModel(Vector2 i_MoveDir)
    {
        var rot = m_Model.localRotation.eulerAngles;
        rot.y                 = i_MoveDir.x < 0 ? 180 : i_MoveDir.x > 0 ? 0 : rot.y;
        m_Model.localRotation = Quaternion.Euler(rot);
    }

    public eCharacterState Hit(eCharacterState i_State, Collision2D i_Col, Vector2 i_MoveDir)
    {
        switch (i_State)
        {
            case eCharacterState.Dash:
                var angle = Vector2.Angle(m_LastDashDir, i_Col.GetContact(0).normal);
                if (angle > 90)
                {
                    m_TwDash?.Kill();
                    return eCharacterState.Idle;
                }
                break;
            case eCharacterState.Jump:
                return eCharacterState.Idle;
        }
        return i_State;
    }

    public void SetGravityScale()
    {
        m_Rb.gravityScale = m_Movement.GravityScale;
    }
}