using System;
using System.Collections.Generic;
using UnityEngine;

public class ManaObject : MonoBehaviour
{
    public static            List<ManaObject> ActiveManaObjectList = new();
    [SerializeField] private GameObject       m_LightObject;
    [SerializeField] private Collider2D       m_Col;
    [SerializeField] private Rigidbody2D      m_Rb;

    private void OnEnable()
    {
        ActiveManaObjectList.Add(this);
    }

    private void OnDisable()
    {
        ActiveManaObjectList.Remove(this);
    }

    public void Placed()
    {
        
    }
    
    public void Thrown(Vector2 i_Force)
    {
        m_LightObject.SetActive(true);
        m_Rb.simulated = true;
        m_Col.enabled  = true;
        m_Rb.AddForce(i_Force,ForceMode2D.Impulse);
    }
}