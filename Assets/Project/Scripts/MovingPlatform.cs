using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private static readonly int s_LightPos   = Shader.PropertyToID("_LightPos");
    private static readonly int s_LightRange = Shader.PropertyToID("_LightRange");

    private float   m_RealDur;
    private Vector3 m_ClosestLightPos = Vector3.one * 999;
    
    [BoxGroup("Animation")][SerializeField]  private bool  m_MoveX;
    [BoxGroup("Animation")] [SerializeField] private bool  m_IsRelative;
    [BoxGroup("Animation")][SerializeField]  private float m_Delay;
    [BoxGroup("Animation")][SerializeField]  private float m_Speed = 10;
    [BoxGroup("Animation")][SerializeField]  private float m_Dur   = 2;
    
    [BoxGroup("Refs")][SerializeField] private Rigidbody2D      m_Rb;
    [BoxGroup("Refs")][SerializeField] private Material         m_Mat;
    [BoxGroup("Refs")][SerializeField] private SpriteRenderer[] m_Renderers;

    private void Awake()
    {
        var newMat = new Material(m_Mat);
        m_Mat = newMat;
        for (var i = 0; i < m_Renderers.Length; i++)
        {
            m_Renderers[i].material = m_Mat;
        }

        m_Mat.SetFloat(s_LightRange, LightSetter.Instance.BrightnessFactor * GameConfig.Instance.LightVars.LightRange);
    }

    private void OnEnable()
    {
        m_RealDur = m_Dur / 2f;
        movePlatform();
    }

    [Button]
    private void movePlatform()
    {
        if (m_MoveX)
        {
            DOTween.To(() => m_Rb.velocity, x => m_Rb.velocity = x, Vector2.right * m_Speed, m_RealDur)
                   .SetRelative(m_IsRelative)
                   .SetDelay(m_Delay)
                   .OnComplete(() =>
                                   {
                                       m_Speed   *= -1;
                                       m_RealDur =  m_Dur;
                                       movePlatform();
                                   })
                   .SetUpdate(UpdateType.Fixed);
        }
        else
        {
            DOTween.To(() => m_Rb.velocity, x => m_Rb.velocity = x, Vector2.up * m_Speed, m_RealDur)
                   .SetRelative(m_IsRelative)
                   .SetDelay(m_Delay)
                   .OnComplete(() =>
                                   {
                                       m_Speed   *= -1;
                                       m_RealDur =  m_Dur;
                                       movePlatform();
                                   })
                   .SetUpdate(UpdateType.Fixed);
        }
    }

    private void Update()
    {
        var relLightPos = transform.InverseTransformPoint(LightSetter.Instance.transform.position);
        if (relLightPos.sqrMagnitude < m_ClosestLightPos.sqrMagnitude)
        {
            var oldRange = m_Mat.GetFloat(s_LightRange);
            var newRange = LightSetter.Instance.BrightnessFactor * GameConfig.Instance.LightVars.LightRange;
            if (relLightPos.magnitude - newRange < m_ClosestLightPos.magnitude - oldRange)
            {
                m_ClosestLightPos = relLightPos;
                m_Mat.SetFloat(s_LightRange, newRange);
            }
        }

        m_Mat.SetVector(s_LightPos, transform.TransformPoint(m_ClosestLightPos));
    }
}