using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private List<Interactable> m_InteractablesInRange = new();

    private InteractionVariables m_InteractionVars => GameConfig.Instance.Interaction;

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
}