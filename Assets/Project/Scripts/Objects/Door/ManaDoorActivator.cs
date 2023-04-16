using System;
using DG.Tweening;
using UnityEngine;

public class ManaDoorActivator : DoorActivator
{
    [SerializeField] private Transform m_ManaPlacePoint;

    private bool       m_IsFailed;
    private ManaObject m_ActiveMana;

    public void PlaceMana(ManaObject i_Mana)
    {
        IsInteractable = false;
        m_ActiveMana   = i_Mana;
        var interactionVars = GameConfig.Instance.Action;
        i_Mana.transform.DOJump(m_ManaPlacePoint.position, interactionVars.ManaPlaceJumpPower, 1, interactionVars.ManaPlaceDuration)
              .SetDelay(interactionVars.ManaPlaceDelay)
              .SetEase(interactionVars.ManaPlaceEase)
              .OnComplete(Interact);
        i_Mana.transform.DORotate(m_ManaPlacePoint.transform.rotation.eulerAngles, interactionVars.ManaPlaceDuration)
              .SetDelay(interactionVars.ManaPlaceDelay);
        i_Mana.transform.DOScale(0.75f, interactionVars.ManaPlaceDuration)
              .SetDelay(interactionVars.ManaPlaceDelay);
    }

    public void ThrowToPlaceMana(ManaObject i_Mana)
    {
        IsInteractable = false;
        m_ActiveMana   = i_Mana;
        i_Mana.transform.DOMove(m_ManaPlacePoint.position, i_Mana.Velocity.magnitude)
              .SetSpeedBased()
              .OnComplete(Interact);
        i_Mana.transform.DORotate(m_ManaPlacePoint.transform.rotation.eulerAngles, i_Mana.Velocity.magnitude)
              .SetSpeedBased();
        i_Mana.transform.DOScale(0.75f, i_Mana.Velocity.magnitude)
              .SetSpeedBased();
    }

    public override void OpenDoorFailed()
    {
        m_ActiveMana.MoveToPlayer();
        IsInteractable = true;
    }
}