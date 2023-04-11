using System;
using System.Collections.Generic;
using UnityEngine;

public class ManaObject : LightObject
{
    public static event Action<ManaObject> OnManaReturned;

    private Camera    m_MainCamera;
    private bool      m_IsThrown;
    private bool      m_MoveToPlayer;
    private Transform m_Tr;

    [SerializeField] private GameObject  m_LightObject;
    [SerializeField] private Collider2D  m_Col;
    [SerializeField] private Rigidbody2D m_Rb;

    private void Awake()
    {
        m_Tr = transform;
    }

    public void Placed()
    {
        m_Tr.SetParent(null);
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
        m_Tr.SetParent(PlayerController.Instance.ManaObjectsHolder);
        m_MoveToPlayer = true;
    }

    public void ResetObject()
    {
        m_IsThrown           = false;
        m_MoveToPlayer       = false;
        
        m_Rb.velocity        = Vector2.zero;
        m_Rb.angularVelocity = 0;
        m_Rb.simulated       = false;
        m_Col.enabled        = false;
        
        m_Tr.SetParent(PlayerController.Instance.ManaObjectsHolder);
        m_Tr.localPosition = Vector3.zero;
        m_Tr.localRotation = Quaternion.identity;
    }
    
    private void Update()
    {
        if (m_IsThrown)
        {
            Vector3 viewPos = m_MainCamera.WorldToViewportPoint(m_Tr.position);

            if (viewPos.x < -0.1f || viewPos.x > 1.1f || viewPos.y < -0.1f || viewPos.y > 1.1f)
            {
                disappeared();
            }
        }

        if (m_MoveToPlayer)
        {
            var lightPos = PlayerController.Instance.LightPos; //Maybe combine with mana holder later
            var manaPos  = (Vector2)m_Tr.position;
            var speed    = GameConfig.Instance.Action.ManaPlaceReturnSpeed;
            m_Tr.position = Vector2.MoveTowards(manaPos, lightPos, speed * Time.deltaTime);

            if ((lightPos - manaPos).sqrMagnitude < 1)
            {
                manaReturned();
            }
        }
    }

    private void manaReturned()
    {
        ResetObject();
        OnManaReturned?.Invoke(this);
    }

    private void disappeared()
    {
        m_IsThrown = false;
        OnManaReturned?.Invoke(this);
    }
}