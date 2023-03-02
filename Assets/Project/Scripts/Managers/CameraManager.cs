using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField]              public  Camera MainCam;
    [SerializeField]              public  CinemachineVirtualCamera MemoryCam;
    
    [SerializeField]            private RenderTexture[]          m_RenderTextures;
    [ShowInInspector, ReadOnly] public  ScreenData               ScreenData;

    private void Awake()
    {
        ScreenData.CalculateData();
        for (var i = 0; i < m_RenderTextures.Length; i++)
        {
            m_RenderTextures[i].width  = (int) ScreenData.OriginalRes.x;
            m_RenderTextures[i].height = (int) ScreenData.OriginalRes.y;
        }
    }
}