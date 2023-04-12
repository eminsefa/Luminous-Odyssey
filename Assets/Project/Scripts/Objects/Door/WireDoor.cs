using System.Collections;
using DG.Tweening;
using UnityEngine;

public class WireDoor : MonoBehaviour
{
    [SerializeField] private BasicAnimation   m_DoorAnimation;
    [SerializeField] private float            m_PointLightInterval;
    [SerializeField] private DoorActivator       m_Activator;
    [SerializeField] private SpriteRenderer[] m_WirePoints;

    private void OnCollisionEnter2D(Collision2D i_Col)
    {
        StartCoroutine(tryToOpenDoor());
    }

    private void OnEnable()
    {
        m_Activator.OnTryOpenDoor += OnButtonPressed;
    }

    private void OnDisable()
    {
        m_Activator.OnTryOpenDoor -= OnButtonPressed;
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
            m_WirePoints[i].color = Color.white;
        }

        m_Activator.OpenDoorFailed();
    }

    private void openCompleted()
    {
        m_DoorAnimation.transform.DOShakePosition(0.5f,0.25f)
                       .OnComplete(() => m_DoorAnimation.Animate());
    }
}