using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
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

        m_Mat.SetFloat(ShaderIDs.S_LightRange, PlayerLight.BrightnessFactor * GameConfig.Instance.LightVars.LightRange);
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
        var lightObjects = LightObject.ActiveLightObjects;
        var oldRange     = m_Mat.GetFloat(ShaderIDs.S_LightRange);
        for (var i = 0; i < lightObjects.Count; i++)
        {
            var lightObject      = lightObjects[i];
            var relLightPos      = transform.InverseTransformPoint(lightObject.position);
            var newRange         = PlayerLight.BrightnessFactor * GameConfig.Instance.LightVars.LightRange;
            if (i != 0) newRange /= 2;
            if (relLightPos.magnitude - newRange < m_ClosestLightPos.magnitude - oldRange)
            {
                m_ClosestLightPos = relLightPos;
                m_Mat.SetFloat(ShaderIDs.S_LightRange, newRange);
            }
        }
        m_Mat.SetVector(ShaderIDs.S_LightPos, transform.TransformPoint(m_ClosestLightPos));
    }
}