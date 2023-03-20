using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private List<Interactable> m_InteractablesInRange = new();
    
    [SerializeField] private GameObject m_ManaPrefab;

#region Unity Events

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

    public void Interact()
    {
        foreach (var i in m_InteractablesInRange)
        {
            if (!i.IsInteractable) continue;
            if (!ManaManager.Instance.TryToUseMana()) return;
            switch (i.InteractableType)
            {
                case eInteractableType.DoorActivator:
                    var mana = Instantiate(m_ManaPrefab, LightSetter.Instance.transform.position, m_ManaPrefab.transform.rotation);
                    ((ManaDoorActivator) i).PlaceMana(mana);
                    break;
            }
        }
    }

    public void Fire()
    {
        var mana = Instantiate(m_ManaPrefab, LightSetter.Instance.transform.position, m_ManaPrefab.transform.rotation);
    }
}