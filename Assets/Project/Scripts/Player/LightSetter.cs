using PaintIn3D;
using Sirenix.OdinInspector;
using UnityEngine;

public class LightSetter : Singleton<LightSetter>
{
    private LightVariables m_LightVars => GameConfig.Instance.LightVars;

    private float     m_Brightness = 1;
    private Texture2D m_LightTexture;

    public float BrightnessFactor => m_LightVars.BrightnessCurve.Evaluate(m_Brightness);

    [FoldoutGroup("Refs")] [SerializeField]
    private Transform m_LightCircleMask;
    
    [FoldoutGroup("Refs")] [SerializeField]
    private Material m_RenderVisibleMat;
    
    [FoldoutGroup("Refs")] [SerializeField]
    private P3dPaintSphere m_PaintSphere;

    private static readonly int s_LightTexture = Shader.PropertyToID("_LightTexture");


    protected override void OnAwakeEvent()
    {
        base.OnAwakeEvent();
        m_LightTexture = new Texture2D(6 , 1, TextureFormat.RGBAFloat, false)
                         {
                             filterMode = FilterMode.Point
                         };
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
        setLightPositions();

        m_RenderVisibleMat.SetFloat(ShaderIDs.S_LightRange,        BrightnessFactor * m_LightVars.LightRange * m_LightVars.MaskRangeMult);
        m_RenderVisibleMat.SetFloat(ShaderIDs.S_VisibilityFalloff, m_LightVars.VisibilityFalloff); //Later call this once

        m_LightCircleMask.localScale = Vector3.one * Mathf.Lerp(0, 14f, BrightnessFactor);
    }

    private void setPaint()
    {
        m_PaintSphere.Radius = Mathf.Max(BrightnessFactor * m_LightVars.LightRange * m_LightVars.PaintRangeMult, 0.01f); //paints all when it gets to 0
    }

    private void OnApplicationQuit()
    {
        m_Brightness                               = 1;
        m_RenderVisibleMat.SetVector(ShaderIDs.S_LightPos, Vector3.up       *-264);
        m_RenderVisibleMat.SetFloat(ShaderIDs.S_LightRange, BrightnessFactor * m_LightVars.LightRange * m_LightVars.MaskRangeMult);
    }
    
    private void setLightPositions()
    {
        var objects = ManaObject.ActiveManaObjectList;
        var count   = 1 + objects.Count;
        m_RenderVisibleMat.SetFloat(ShaderIDs.S_LightCount, count);

        m_LightTexture.Reinitialize(count, 1, TextureFormat.RGBAFloat, false);
        var mainCam = CameraManager.Instance.MainCam;
        for (int i = 0; i < count; i++)
        {
            Vector2 lightPos = Vector3.forward * 100;
            if (i == 0)
            {
                var screenPos =mainCam.WorldToScreenPoint(new Vector3(transform.position.x,transform.position.y, mainCam.transform.position.z));
                lightPos = new Vector2(screenPos.x - Screen.width / 2f, screenPos.y - Screen.height / 2f);
            }
            else if (i < count)
            {
                var manaPos = objects[i - 1].transform.position;
                var screenPos    =mainCam.WorldToScreenPoint(new Vector3(manaPos.x, manaPos.y, mainCam.transform.position.z));
                lightPos = new Vector2(screenPos.x - Screen.width / 2f, screenPos.y - Screen.height / 2f);
            }
            m_LightTexture.SetPixel(i, 0, new Color(lightPos.x, lightPos.y, 0, 0));
        }
        m_LightTexture.Apply();
        m_RenderVisibleMat.SetTexture(s_LightTexture, m_LightTexture);
    }

}