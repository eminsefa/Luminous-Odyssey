using System;
using DG.Tweening;
using UnityEngine;

public class ManaDoorActivator : DoorActivator
{
    public static event Action OnManaReturned;

    [SerializeField] private Transform m_ManaPlacePoint;

    private bool       m_IsFailed;
    private GameObject m_ActiveMana;

    public void PlaceMana(GameObject i_Mana)
    {
        IsInteractable = false;
        m_ActiveMana   = i_Mana;
        var interactionVars = GameConfig.Instance.Action;
        i_Mana.transform.DOJump(m_ManaPlacePoint.position, interactionVars.ManaPlaceJumpPower, 1, interactionVars.ManaPlaceDuration)
              .SetDelay(interactionVars.ManaPlaceDelay)
              .SetEase(interactionVars.ManaPlaceEase)
              .OnComplete(Interact);
    }

    public override void OpenDoorFailed()
    {
        m_IsFailed = true;
    }

    private void Update()
    {
        if (!m_IsFailed) return;

        var lightPos = LightSetter.Instance.transform.position;
        var manaPos  = m_ActiveMana.transform.position;
        var speed    = GameConfig.Instance.Action.ManaPlaceReturnSpeed;
        m_ActiveMana.transform.position = Vector2.MoveTowards(manaPos, lightPos, speed * Time.deltaTime);

        if ((lightPos - manaPos).sqrMagnitude < 1)
        {
            m_IsFailed     = false;
            OnManaReturned?.Invoke();
            
            IsInteractable = true;
            Destroy(m_ActiveMana);
        }
    }
}