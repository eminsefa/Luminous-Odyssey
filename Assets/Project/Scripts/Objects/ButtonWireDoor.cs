using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ButtonWireDoor : MonoBehaviour
{
    [SerializeField] private float            m_PointLightInterval;
    [SerializeField] private DoorButton       m_Button;
    [SerializeField] private SpriteRenderer   m_Door;
    [SerializeField] private SpriteRenderer[] m_WirePoints;

    private void OnCollisionEnter2D(Collision2D col)
    {
        StartCoroutine(tryToOpenDoor());
    }

    private void OnEnable()
    {
        m_Button.OnButtonPressed += OnButtonPressed;
    }

    private void OnDisable()
    {
        m_Button.OnButtonPressed -= OnButtonPressed;
    }

    private void OnButtonPressed()
    {
        StartCoroutine(tryToOpenDoor());
    }

    private IEnumerator tryToOpenDoor()
    {
        for (var i = 0; i < m_WirePoints.Length; i++)
        {
            var p = m_WirePoints[i];
            yield return new WaitForSeconds(m_PointLightInterval);
            var painted = MemoryBlockManager.Instance.IsPointPainted(p.transform.position);
            if (!painted)
            {
                StartCoroutine(openFailed(i));
                yield break;
            }

            p.color = Color.green;
        }

        openCompleted();
    }

    private IEnumerator openFailed(int i_LastTry)
    {
        for (var i = i_LastTry; i >= 0; i--)
        {
            yield return new WaitForSeconds(m_PointLightInterval / 2f);
            m_WirePoints[i].color = Color.black;
        }

        m_Button.OpenDoorFailed();
    }

    private void openCompleted()
    {
        m_Door.color = Color.green;
        m_Door.transform.DOLocalMoveY(2, 2);
    }
}