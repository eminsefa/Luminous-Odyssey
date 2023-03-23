using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private List<Interactable> m_InteractablesInRange = new();

    private ActionVariables m_ActionVars => GameConfig.Instance.Action;

    private ManaObject       m_CurrentThrowMana;
    private List<ManaObject> m_ActiveManaObjects;

    [SerializeField] private Transform        m_ManaObjectsHolder;
    [SerializeField] private Transform        m_HandThrowPoint;
    [SerializeField] private List<ManaObject> m_AllManaObjects;

#region Unity Events

    private void OnEnable()
    {
        m_ActiveManaObjects          =  new List<ManaObject>(m_AllManaObjects);
        ManaObject.OnManaReturned += OnManaReturned;
    }

    private void OnDisable()
    {
        ManaObject.OnManaReturned -= OnManaReturned;
    }

    private void OnTriggerEnter2D(Collider2D i_Other)
    {
        i_Other.TryGetComponent(out Interactable interactable);
        if (interactable != null)
        {
            m_InteractablesInRange.Add(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D i_Other)
    {
        i_Other.TryGetComponent(out Interactable interactable);
        if (interactable != null)
        {
            m_InteractablesInRange.Remove(interactable);
        }
    }

#endregion

    private void OnManaReturned(ManaObject i_ManaObject)
    {
        if(!m_ActiveManaObjects.Contains(i_ManaObject)) queueMana(i_ManaObject);
    }
    
    public void Interact()
    {
        foreach (var i in m_InteractablesInRange)
        {
            if (!i.IsInteractable) continue;
            if (!ManaManager.Instance.TryToUseMana()) return;
            switch (i.InteractableType)
            {
                case eInteractableType.DoorActivator:
                    var mana = dequeueMana();
                    mana.Placed();
                    ((ManaDoorActivator) i).PlaceMana(mana);
                    break;
            }
        }
    }

    public void ThrowCreateMana()
    {
        var mana = dequeueMana();
        m_CurrentThrowMana = mana;
        m_CurrentThrowMana.transform.SetParent(m_HandThrowPoint);
    }

    public void ThrowMana(Vector2 i_ThrowDir)
    {
        m_CurrentThrowMana.transform.SetParent(null);
        m_CurrentThrowMana.Thrown(i_ThrowDir * m_ActionVars.ThrowSpeed);
        m_CurrentThrowMana = null;
    }

    private void queueMana(ManaObject i_ManaObject)
    {
        i_ManaObject.ResetObject(m_ManaObjectsHolder);
        i_ManaObject.gameObject.SetActive(false);
        m_ActiveManaObjects.Add(i_ManaObject);
    }
    
    private ManaObject dequeueMana()
    {
        var mana = m_ActiveManaObjects[0];
        mana.transform.position = m_HandThrowPoint.position;
        mana.gameObject.SetActive(true);
        m_ActiveManaObjects.RemoveAt(0);
        return mana;
    }
}