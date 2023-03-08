using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
    private static readonly int s_LightPos   = Shader.PropertyToID("_LightPos");
    private static readonly int s_LightRange = Shader.PropertyToID("_LightRange");

    private Vector3               m_ClosestLightPos = Vector3.forward * -10;
    private MaterialPropertyBlock m_MatPropBlock;

    public bool             moveX;
    public float            speed;
    public float            dur;
    public Rigidbody2D      rb;
    public Material         mat;
    public SpriteRenderer[] renderers;

    private void Awake()
    {
        var newMat = new Material(mat);
        mat = newMat;
        for (var i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = mat;
        }
        mat.SetFloat(s_LightRange, LightSetter.Instance.BrightnessFactor * GameConfig.Instance.LightVars.LightRange);
    }

    private void OnEnable()
    {
        if (moveX)
        {
            DOTween.To(() => rb.velocity, x => rb.velocity = x, Vector2.right * speed, dur)
                   .OnStepComplete(() =>
                                   {
                                       speed *= -1;
                                       OnEnable();
                                   })
                   .SetUpdate(UpdateType.Fixed);
        }
        else
        {
            DOTween.To(() => rb.velocity, x => rb.velocity = x, Vector2.up * speed, dur)
                   .OnStepComplete(() =>
                                   {
                                       speed *= -1;
                                       OnEnable();
                                   })
                   .SetUpdate(UpdateType.Fixed);
        }
    }

    private void FixedUpdate()
    {
        var relLightPos = transform.InverseTransformPoint(LightSetter.Instance.transform.position);
        if (relLightPos.sqrMagnitude < m_ClosestLightPos.sqrMagnitude)
        {
            m_ClosestLightPos = relLightPos;
            mat.SetFloat(s_LightRange, LightSetter.Instance.BrightnessFactor * GameConfig.Instance.LightVars.LightRange);
        }

        mat.SetVector(s_LightPos, transform.TransformPoint(m_ClosestLightPos));
    }
}