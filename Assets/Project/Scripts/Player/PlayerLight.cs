using System;
using PaintIn3D;
using Sirenix.OdinInspector;
using UnityEngine;
using Util;

public class PlayerLight : LightObject
{
    public static float BrightnessFactor { get; private set; }
    public        float BlindFactor      { get; private set; }

    private float m_Brightness = 1;
    private float m_LightSpeed;
    private float m_SlowWalkTimer;

    private LightVariables m_LightVars => GameConfig.Instance.LightVars;

#region Refs

    [FoldoutGroup("Refs")] [SerializeField]
    private Transform m_EyeClosers;

    [FoldoutGroup("Refs")] [SerializeField]
    private Transform m_LightCircleMask;

    [FoldoutGroup("Refs")] [SerializeField]
    private P3dPaintSphere m_PaintSphere;

#endregion

    private void Update()
    {
        calculateBrightness();
        setMaskScale();
        setPaint();
    }

    private void calculateBrightness()
    {
        var deltaMove = Mathf.Abs(m_LightSpeed) > m_LightVars.MoveLightThreshold ? m_LightSpeed : 0;

        if (Input.GetKey(KeyCode.A)) deltaMove = m_LightVars.BrightenSpeed; //Test

        var delta = deltaMove * m_LightVars.BrightenSubtleSpeed - m_LightVars.DarkenSubtleSpeed;

        m_Brightness += delta * Time.deltaTime;
        m_Brightness =  Mathf.Clamp(m_Brightness, 0, 1);

        BrightnessFactor = m_LightVars.BrightnessCurve.Evaluate(m_Brightness);
    }

    private void setMaskScale()
    {
        m_LightCircleMask.localScale = Vector3.one * Mathf.Lerp(0, 16f, BrightnessFactor);
    }

    private void setPaint()
    {
        m_PaintSphere.Radius = Mathf.Max(BrightnessFactor * m_LightVars.LightRange * m_LightVars.PaintRangeMult, 0.01f); //paints all when it gets to 0
    }

    public void SetMoveSpeed(float i_Speed, bool i_IsBlind)
    {
        if (Mathf.Abs(i_Speed) > 0.1f) i_Speed = m_LightVars.BrightenSpeed;
        var blindDarkenSpeed                   = m_LightVars.SlowWalkDarkenSpeed;
        m_LightSpeed = i_IsBlind ? -blindDarkenSpeed : i_Speed;

        var delta = (i_IsBlind ? 1 : -1) * blindDarkenSpeed * 0.5f * Time.deltaTime;

        BlindFactor += delta;
        BlindFactor =  Mathf.Clamp(BlindFactor, 0, 1);
        m_EyeClosers.SetLocalScale(ExtensionMethods.Axis.x, BlindFactor);
    }

    private void OnApplicationQuit()
    {
        m_Brightness = 1;
    }
}