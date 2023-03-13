using Managers;
using Sirenix.OdinInspector;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public Camera  MainCam   => m_MainCam;
    public Vector2 BlockSize => m_BlockSize;

    private Vector2 m_BlockSize;

    [SerializeField] private Camera          m_MainCam;
    [SerializeField] private RenderTexture[] m_RenderTextures;
    [SerializeField] private RenderTexture[] m_MaskRenderTextures;

    [ShowInInspector, ReadOnly] public ScreenData ScreenData;

    protected override void OnAwakeEvent()
    {
        base.OnAwakeEvent();
        ScreenData.CalculateData();

        m_BlockSize.y = m_MainCam.orthographicSize * 2;
        m_BlockSize.x = m_BlockSize.y              * m_MainCam.aspect;

        var width  = (int) ScreenData.OriginalRes.x;
        var height = (int) ScreenData.OriginalRes.y;
        for (var i = 0; i < m_RenderTextures.Length; i++)
        {
            m_RenderTextures[i].width  = width;
            m_RenderTextures[i].height = height;
        }

        for (var i = 0; i < m_RenderTextures.Length; i++)
        {
            m_MaskRenderTextures[i].width  = width  /2;
            m_MaskRenderTextures[i].height = height /2;
        }
    }
}