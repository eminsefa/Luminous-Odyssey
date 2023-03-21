using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private List<Interactable> m_InteractablesInRange = new();

    private ActionVariables m_ActionVars => GameConfig.Instance.Action;

    private                  ManaObject m_CurrentThrowMana;

    [SerializeField] private Transform  m_HandThrowPoint;
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

    public void ThrowCreateMana()
    {
        Instantiate(m_ManaPrefab, LightSetter.Instance.transform.position, m_ManaPrefab.transform.rotation).TryGetComponent(out ManaObject mana);
        m_CurrentThrowMana = mana;
        m_CurrentThrowMana.transform.SetParent(m_HandThrowPoint);
        m_CurrentThrowMana.transform.localPosition=Vector3.zero;
    }
    
    public void ThrowMana(Vector2 i_ThrowDir)
    {
        m_CurrentThrowMana.transform.SetParent(null);
        m_CurrentThrowMana.Thrown(i_ThrowDir * m_ActionVars.ThrowSpeed);
        m_CurrentThrowMana = null;
    }
}