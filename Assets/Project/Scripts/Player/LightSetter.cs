using PaintIn3D;
using Sirenix.OdinInspector;
using UnityEngine;

public class LightSetter : Singleton<LightSetter>
{
    private static readonly int s_LightPos          = Shader.PropertyToID("_LightPos");
    private static readonly int s_LightRange        = Shader.PropertyToID("_LightRange");
    private static readonly int s_VisibilityFalloff = Shader.PropertyToID("_VisibilityFalloff");

    private LightVariables m_LightVars => GameConfig.Instance.Movement.LightVars;

    private float    m_Brightness = 1;
    public  Material LightReverseEffectedMat { get; set; }

    public float BrightnessFactor => m_LightVars.BrightnessCurve.Evaluate(m_Brightness);

    [FoldoutGroup("Refs")][SerializeField] private Transform      m_LightCenter;
    [FoldoutGroup("Refs")][SerializeField] private P3dPaintSphere m_PaintSphere;
    [FoldoutGroup("Refs")][SerializeField] private Material       m_LightEffectedMat;

    
    private void Update()
    {
        calculateBrightness();
        setLight();
        setPaint();
    }

    private void calculateBrightness()
    {
        var delta = PlayerMovement.Instance.Velocity * m_LightVars.BrightenSpeed - m_LightVars.DarkenSpeed;
        m_Brightness += delta * Time.deltaTime;
        m_Brightness =  Mathf.Clamp01(m_Brightness);
    }

    private void setLight()
    {
        m_LightEffectedMat.SetVector(s_LightPos, m_LightCenter.position);
        m_LightEffectedMat.SetFloat(s_LightRange,        BrightnessFactor * m_LightVars.LightRange);
        m_LightEffectedMat.SetFloat(s_VisibilityFalloff, m_LightVars.VisibilityFalloff);

        if (LightReverseEffectedMat != null)
        {
            LightReverseEffectedMat.SetVector(s_LightPos, m_LightCenter.position);
            LightReverseEffectedMat.SetFloat(s_LightRange, BrightnessFactor * m_LightVars.LightRange);
        }
    }

    private void setPaint()
    {
        m_PaintSphere.Radius = Mathf.Max(BrightnessFactor * m_LightVars.LightRange*m_LightVars.PaintRangeMult, 0.01f); //paints all when it gets to 0
    }

    private void OnApplicationQuit()
    {
        m_Brightness       = 1;
        transform.position = Vector3.zero;
        setLight();
    }
}