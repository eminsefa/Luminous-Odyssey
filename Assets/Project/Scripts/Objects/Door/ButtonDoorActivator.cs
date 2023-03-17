using System;
using DG.Tweening;
using UnityEngine;

public class ButtonDoorActivator : DoorActivator
{
    [SerializeField] private float[]   m_ButtonAnimationDurations;
    [SerializeField] private float[]   m_ButtonUpYPositions;

    private void OnCollisionEnter2D(Collision2D i_Col)
    {
        if (!IsInteractable) return;
        buttonPressed();
    }

    public override void OpenDoorFailed()
    {
        transform.DOLocalMoveY(m_ButtonUpYPositions[0], m_ButtonAnimationDurations[0])
                  .SetEase(Ease.OutBounce)
                  .OnComplete(() => IsInteractable = true);
    }

    private void buttonPressed()
    {
        IsInteractable = false;
        Interact();
        transform.DOLocalMoveY(m_ButtonUpYPositions[1], m_ButtonAnimationDurations[1])
                 .SetEase(Ease.OutCubic);
    }
}