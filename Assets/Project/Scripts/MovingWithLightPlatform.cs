using System;
using UnityEngine;

public class MovingWithLightPlatform : MonoBehaviour
{
    private bool  m_IsOnLight;
    private int   m_DirCounter;
    private float m_Speed;

    [SerializeField] private Vector3[]      m_MovePoints;
    [SerializeField] private Rigidbody2D    m_Rb;
    [SerializeField] private float          m_MaxSpeed;
    [SerializeField] private float          m_SpeedAcceleration;
    [SerializeField] private ParticleSystem m_MoveParticle;

    private void FixedUpdate()
    {
        var delta = m_SpeedAcceleration * m_SpeedAcceleration;
        m_Speed += delta * (m_IsOnLight ? 1 : -1) ;
        m_Speed =  Mathf.Clamp(m_Speed, 0, m_MaxSpeed);
        var dir = m_MovePoints[m_DirCounter % m_MovePoints.Length] - transform.position;
        if (dir.sqrMagnitude < 1) m_DirCounter++;
        m_Rb.velocity = dir.normalized * m_Speed;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!m_IsOnLight) m_MoveParticle.Play();
        m_IsOnLight = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        m_IsOnLight   = false;
        m_MoveParticle.Stop();
    }
}