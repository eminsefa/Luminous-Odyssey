using PaintIn3D;
using Sirenix.OdinInspector;
using UnityEngine;

public class LightSetter : Singleton<LightSetter>
{
    private static readonly int s_LightPos          = Shader.PropertyToID("_LightPos");
    private static readonly int s_LightRange        = Shader.PropertyToID("_LightRange");
    private static readonly int s_VisibilityFalloff = Shader.PropertyToID("_VisibilityFalloff");

    private LightVariables m_LightVars => GameConfig.Instance.LightVars;

    private float m_CamY;
    private float m_Brightness = 1;

    public float BrightnessFactor => m_LightVars.BrightnessCurve.Evaluate(m_Brightness);

    [FoldoutGroup("Refs")] [SerializeField]
    private Transform m_LightCircleMask;
    
    [FoldoutGroup("Refs")] [SerializeField]
    private Material m_LightCircleMat;
    
    [FoldoutGroup("Refs")] [SerializeField]
    private P3dPaintSphere m_PaintSphere;


    protected override void OnAwakeEvent()
    {
        base.OnAwakeEvent();
        m_CamY = CameraManager.Instance.MainCam.transform.position.y;
        setLight();
    }

    private void Update()
    {
        calculateBrightness();
        setLight();
        setPaint();
    }

    private void calculateBrightness()
    {
        var delta = PlayerController.Instance.Velocity.sqrMagnitude * m_LightVars.BrightenSpeed - m_LightVars.DarkenSpeed;
        m_Brightness += delta * Time.deltaTime;
        m_Brightness =  Mathf.Clamp01(m_Brightness);
    }

    private void setLight()
    {
        m_LightCircleMat.SetVector(s_LightPos, Vector3.up               *(m_CamY-Screen.height /4f));
        m_LightCircleMat.SetFloat(s_LightRange,        BrightnessFactor * m_LightVars.LightRange * m_LightVars.MaskRangeMult);
        m_LightCircleMat.SetFloat(s_VisibilityFalloff, m_LightVars.VisibilityFalloff); //Later call this once

        m_LightCircleMask.localScale = Vector3.one * Mathf.Lerp(0, 14f, BrightnessFactor);
    }

    private void setPaint()
    {
        m_PaintSphere.Radius = Mathf.Max(BrightnessFactor * m_LightVars.LightRange * m_LightVars.PaintRangeMult, 0.01f); //paints all when it gets to 0
    }

    private void OnApplicationQuit()
    {
        m_Brightness                               = 1;
        m_LightCircleMat.SetVector(s_LightPos, Vector3.up       *-264);
        m_LightCircleMat.SetFloat(s_LightRange, BrightnessFactor * m_LightVars.LightRange * m_LightVars.MaskRangeMult);
    }
}