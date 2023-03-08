using Managers;
using Sirenix.OdinInspector;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public float   OrthSize  => m_MainCam.orthographicSize;
    public Vector2 BlockSize => m_BlockSize;

    private Vector2 m_BlockSize = new (53.333f, 30f); //Maybe later make it dynamic

    [SerializeField] private Camera          m_MainCam;
    [SerializeField] private RenderTexture[] m_RenderTextures;

    [ShowInInspector, ReadOnly] public ScreenData ScreenData;

    protected override void OnAwakeEvent()
    {
        base.OnAwakeEvent();
        ScreenData.CalculateData();

        m_BlockSize.y = OrthSize      * 2;
        m_BlockSize.x = m_BlockSize.y * m_MainCam.aspect;
        
        var width  =(int) ScreenData.OriginalRes.x;
        var height =(int) ScreenData.OriginalRes.y;
        for (var i = 0; i < m_RenderTextures.Length; i++)
        {
            m_RenderTextures[i].width  = width;
            m_RenderTextures[i].height = height;
        }
    }
}