using System;
using UnityEngine;

public class MovingWhileBlindPlatform : MonoBehaviour
{
    private bool  m_IsPlayerOn;
    private bool  m_IsPlayerBlind;
    private float m_Speed;

    [SerializeField] private BoxCollider2D  m_Col;
    [SerializeField] private Vector3        m_MovePoint;
    [SerializeField] private Rigidbody2D    m_Rb;
    [SerializeField] private float          m_MaxSpeed;
    [SerializeField] private float          m_SpeedAcceleration;
    [SerializeField] private ParticleSystem m_MoveParticle;

    private void FixedUpdate()
    {
        var delta = m_SpeedAcceleration * m_SpeedAcceleration;
        m_Speed += delta * ((m_IsPlayerOn && m_IsPlayerBlind) ? 1 : -1);
        m_Speed =  Mathf.Clamp(m_Speed, 0, m_MaxSpeed);
        var dir = m_MovePoint - transform.position;
        m_Rb.velocity = dir.normalized * m_Speed;
    }

    private void Update()
    {
        m_IsPlayerBlind = PlayerController.Instance.IsBlind;

        if (m_IsPlayerBlind)
        {
            if (!m_MoveParticle.isPlaying) m_MoveParticle.Play();
        }
        else if (m_MoveParticle.isPlaying) m_MoveParticle.Stop();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        collision.rigidbody.TryGetComponent(out PlayerController player);
        var isPlayerOn    = player                         != null;
        var isObjectAbove =collision.GetContact(0).point.y > m_Col.bounds.center.y;
        m_IsPlayerOn = isPlayerOn && isObjectAbove;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        m_IsPlayerOn = false;
    }
}