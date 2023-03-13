using System;
using DG.Tweening;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public event Action OnButtonPressed;

    private bool m_IsActive = true;

    [SerializeField] private float[]   m_ButtonAnimationDurations;
    [SerializeField] private float[]   m_ButtonUpYPositions;

    private void OnCollisionEnter2D(Collision2D i_Col)
    {
        if (!m_IsActive) return;
        m_IsActive = false;
        buttonPressed();
    }

    public void OpenDoorFailed()
    {
        transform.DOLocalMoveY(m_ButtonUpYPositions[0], m_ButtonAnimationDurations[0])
                  .SetEase(Ease.OutBounce)
                  .OnComplete(() => m_IsActive = true);
    }

    private void buttonPressed()
    {
        transform.DOLocalMoveY(m_ButtonUpYPositions[1], m_ButtonAnimationDurations[1])
                 .SetEase(Ease.OutCubic);
        OnButtonPressed?.Invoke();
    }
}