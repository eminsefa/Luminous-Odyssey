using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class BasicAnimation : MonoBehaviour
{
    private enum eAnimType
    {
        Move,
        Rotate,
        Scale,
    }

    [SerializeField] private eAnimType m_AnimType;
    [SerializeField] private float     m_Duration;
    [SerializeField] private float     m_StartDelay;
    [SerializeField] private Ease     m_Ease;
    
    [ShowIf(nameof(m_AnimType), eAnimType.Move)] [SerializeField]
    private Vector3 m_FinalPosition;

    [ShowIf(nameof(m_AnimType), eAnimType.Rotate)] [SerializeField]
    private Vector3 m_FinalRotation;

    [ShowIf(nameof(m_AnimType), eAnimType.Scale)] [SerializeField]
    private Vector3 m_FinalScale;

    [SerializeField] int  m_LoopCount = 0;
    private          bool m_IsLooping => m_LoopCount is > 0 or -1;

    [ShowIf(nameof(m_IsLooping))] [SerializeField]
    private LoopType m_LoopType;

    [SerializeField] private bool m_AnimateSelf;

    [HideIf(nameof(m_AnimateSelf))] [SerializeField]
    private Transform m_TargetTransform;

    //Convert to Local
    public void Animate()
    {
        var tr = m_AnimateSelf ? transform : m_TargetTransform;
        switch (m_AnimType)
        {
            case eAnimType.Move:
                tr.DOMove(m_FinalPosition, m_Duration)
                  .SetDelay(m_StartDelay)
                  .SetEase(m_Ease)
                  .SetLoops(m_LoopCount, m_LoopType);
                break;
            case eAnimType.Rotate:
                tr.DORotate(m_FinalRotation, m_Duration)
                  .SetDelay(m_StartDelay)
                  .SetEase(m_Ease)
                  .SetLoops(m_LoopCount, m_LoopType);
                break;
            case eAnimType.Scale:
                tr.DOScale(m_FinalScale, m_Duration)
                  .SetDelay(m_StartDelay)
                  .SetEase(m_Ease)
                  .SetLoops(m_LoopCount, m_LoopType);
                break;
        }
    }
}