using System;
using PaintIn3D;
using Sirenix.OdinInspector;
using UnityEngine;
using Util;

public class PlayerLight : LightObject
{
    public static            float BrightnessFactor { get; private set; }
    public float CloseEyeIter     { get; private set; }

    private float m_Brightness = 1;
    private float m_LightSpeed;
    private float m_SlowWalkTimer;

    private LightVariables m_LightVars => GameConfig.Instance.LightVars;

#region Refs

    [FoldoutGroup("Refs")] [SerializeField]
    private GameObject m_EyeClosers;

    [FoldoutGroup("Refs")] [SerializeField]
    private Transform m_LightCircleMask;

    [FoldoutGroup("Refs")] [SerializeField]
    private P3dPaintSphere m_PaintSphere;

#endregion

    private void LateUpdate()
    {
        calculateBrightness();
        setPaint();
    }

    private void calculateBrightness()
    {
        var deltaMove = Mathf.Abs(m_LightSpeed) > m_LightVars.MoveLightThreshold ? m_LightSpeed : 0;

        var delta = deltaMove * m_LightVars.BrightenSpeed - m_LightVars.DarkenSpeed;
        m_Brightness += delta * Time.deltaTime;
        m_Brightness =  Mathf.Clamp01(m_Brightness);

        BrightnessFactor = m_LightVars.BrightnessCurve.Evaluate(m_Brightness);

        var scale = Mathf.Lerp(m_LightCircleMask.localScale.x, Mathf.Lerp(0, 16f, BrightnessFactor), 0.5f);
        m_LightCircleMask.localScale = Vector3.one * scale;
    }

    private void setPaint()
    {
        m_PaintSphere.Radius = Mathf.Max(BrightnessFactor * m_LightVars.LightRange * m_LightVars.PaintRangeMult, 0.01f); //paints all when it gets to 0
    }

    private void OnApplicationQuit()
    {
        m_Brightness = 1;
    }

    public void SetMoveSpeed(float i_Speed, bool i_IsBlind)
    {
        m_LightSpeed = i_IsBlind ? -(m_LightVars.SlowWalkDarkenSpeed * m_LightVars.SlowWalkDarkenSpeed) : i_Speed * i_Speed;

        m_EyeClosers.SetActive(i_IsBlind);
        CloseEyeIter = i_IsBlind ? (1 - BrightnessFactor) : CloseEyeIter - Time.deltaTime;
        CloseEyeIter = Mathf.Clamp(CloseEyeIter, 0, 1);
        m_EyeClosers.transform.SetLocalScale(ExtensionMethods.Axis.x, CloseEyeIter);
    }
}