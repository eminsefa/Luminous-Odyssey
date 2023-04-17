using System;
using PaintIn3D;
using Sirenix.OdinInspector;
using UnityEngine;

public class LightManager : Singleton<LightManager>
{
    private LightVariables m_LightVars => GameConfig.Instance.LightVars;

    private Camera    m_MainCam;
    private Texture2D m_LightTexture;

    [FoldoutGroup("Refs")] [SerializeField]
    private Material m_RenderVisibleMat;

    [FoldoutGroup("Refs")] [SerializeField]
    private Material m_RenderMemoryMat;

    protected override void OnAwakeEvent()
    {
        base.OnAwakeEvent();
        m_LightTexture = new Texture2D(6, 1, TextureFormat.RGBAFloat, false)
                         {
                             filterMode = FilterMode.Point
                         };
        m_MainCam = Camera.main;
        setStartLight();
    }

    private void LateUpdate()
    {
        setLight();
    }

    private void setStartLight()
    {
        m_RenderVisibleMat.SetFloat(ShaderIDs.S_ManaObjectLightRange, m_LightVars.ThrowManaLightRange);
        m_RenderVisibleMat.SetFloat(ShaderIDs.S_VisibilityFalloff,    m_LightVars.VisibilityFalloff); 
        m_RenderMemoryMat.SetFloat(ShaderIDs.S_VisibilityFalloff, m_LightVars.VisibilityFalloff);     
    }

    private void setLight()
    {
        setLightPositions();

        m_RenderVisibleMat.SetFloat(ShaderIDs.S_LightRange, PlayerLight.BrightnessFactor * m_LightVars.LightRange * m_LightVars.MaskRangeMult);
        m_RenderMemoryMat.SetFloat(ShaderIDs.S_LightRange,  PlayerLight.BrightnessFactor * m_LightVars.LightRange * m_LightVars.MaskRangeMult);
        
        m_RenderMemoryMat.SetFloat(ShaderIDs.S_BlindFactor,  PlayerController.Instance.BlindFactor);
    }
    
    private void setLightPositions()
    {
        var objects = LightObject.ActiveLightObjects;
        var count   = objects.Count;
        m_RenderVisibleMat.SetFloat(ShaderIDs.S_LightCount, count);
        m_RenderMemoryMat.SetFloat(ShaderIDs.S_LightCount, count);

        m_LightTexture.Reinitialize(count, 1, TextureFormat.RGBAFloat, false);
        for (int i = 0; i < count; i++)
        {
            var     manaPos   = objects[i ].transform.position;
            var     screenPos = m_MainCam.WorldToScreenPoint(new Vector3(manaPos.x, manaPos.y, m_MainCam.transform.position.z));
            var lightPos = new Vector2(screenPos.x - Screen.width / 2f, screenPos.y - Screen.height / 2f);

            m_LightTexture.SetPixel(i, 0, new Color(lightPos.x / Screen.width, lightPos.y / Screen.height, 0, 0));
        }

        m_LightTexture.Apply();
        m_RenderVisibleMat.SetTexture(ShaderIDs.S_LightTexture, m_LightTexture);
        m_RenderMemoryMat.SetTexture(ShaderIDs.S_LightTexture, m_LightTexture);
    }
    
    private void OnApplicationQuit()
    {
        m_RenderVisibleMat.SetVector(ShaderIDs.S_LightPos, Vector3.up                    * -264);
        m_RenderVisibleMat.SetFloat(ShaderIDs.S_LightRange, PlayerLight.BrightnessFactor * m_LightVars.LightRange * m_LightVars.MaskRangeMult);
    }
}
