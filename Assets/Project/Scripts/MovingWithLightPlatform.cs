using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWithLightPlatform : MonoBehaviour
{
    private bool m_IsOnLight;

    [SerializeField] private Vector3        m_TargetPoint;
    [SerializeField] private Rigidbody2D    m_Rb;
    [SerializeField] private float          m_Speed;
    [SerializeField] private ParticleSystem m_MoveParticle;

    private void FixedUpdate()
    {
        if (!m_IsOnLight) return;
        var dir = m_TargetPoint - transform.position;
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
        m_Rb.velocity = Vector2.zero;
        m_MoveParticle.Stop();
    }
}