using DG.Tweening;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private static readonly int s_LightPos   = Shader.PropertyToID("_LightPos");
    private static readonly int s_LightRange = Shader.PropertyToID("_LightRange");

    private Vector3 m_ClosestLightPos = Vector3.one * 999;

    [SerializeField] private bool             m_MoveX;
    [SerializeField] private float            m_Speed = 10;
    [SerializeField] private float            m_Dur   = 2;
    [SerializeField] private Rigidbody2D      m_Rb;
    [SerializeField] private Material         m_Mat;
    [SerializeField] private SpriteRenderer[] m_Renderers;

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
        if (m_MoveX)
        {
            DOTween.To(() => m_Rb.velocity, x => m_Rb.velocity = x, Vector2.right * m_Speed, m_Dur)
                   .OnStepComplete(() =>
                                   {
                                       m_Speed *= -1;
                                       OnEnable();
                                   })
                   .SetUpdate(UpdateType.Fixed);
        }
        else
        {
            DOTween.To(() => m_Rb.velocity, x => m_Rb.velocity = x, Vector2.up * m_Speed, m_Dur)
                   .OnStepComplete(() =>
                                   {
                                       m_Speed *= -1;
                                       OnEnable();
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