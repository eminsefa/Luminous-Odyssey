using System;
using System.Collections.Generic;
using UnityEngine;

public class ManaObject : LightObject
{
    public static event Action<ManaObject> OnManaReturned;

    private Camera m_MainCamera;
    private bool   m_IsThrown;
    private bool   m_MoveToPlayer;

    [SerializeField] private GameObject  m_LightObject;
    [SerializeField] private Collider2D  m_Col;
    [SerializeField] private Rigidbody2D m_Rb;

    public void Placed()
    {
        m_IsThrown = false;
        m_LightObject.SetActive(false);
    }

    public void Thrown(Vector2 i_Force)
    {
        m_LightObject.SetActive(true);
        m_Rb.simulated = true;
        m_Col.enabled  = true;
        m_Rb.AddForce(i_Force, ForceMode2D.Impulse);

        m_IsThrown   = true;
        m_MainCamera = CameraManager.Instance.MainCam;
    }

    public void MoveToPlayer()
    {
        m_MoveToPlayer = true;
    }

    public void ResetObject(Transform i_Parent)
    {
        m_Rb.velocity        = Vector2.zero;
        m_Rb.angularVelocity = 0;
        m_Rb.simulated       = false;
        m_Col.enabled        = false;
        
        transform.SetParent(i_Parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
    
    private void Update()
    {
        if (m_IsThrown)
        {
            Vector3 viewPos = m_MainCamera.WorldToViewportPoint(transform.position);

            if (viewPos.x < -0.1f || viewPos.x > 1.1f || viewPos.y < -0.1f || viewPos.y > 1.1f)
            {
                disappeared();
            }
        }

        if (m_MoveToPlayer)
        {
            var lightPos = LightSetter.Instance.transform.position;
            var manaPos  = transform.position;
            var speed    = GameConfig.Instance.Action.ManaPlaceReturnSpeed;
            transform.position = Vector2.MoveTowards(manaPos, lightPos, speed * Time.deltaTime);

            if ((lightPos - manaPos).sqrMagnitude < 1)
            {
                m_IsThrown = false;
                OnManaReturned?.Invoke(this);
            }
        }
    }

    private void disappeared()
    {
        m_IsThrown = false;
        OnManaReturned?.Invoke(this);
    }
}