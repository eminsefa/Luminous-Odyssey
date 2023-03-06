using Managers;
using Sirenix.OdinInspector;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public float   OrthSize  => m_MainCam.orthographicSize;
    public Vector2 BlockSize => m_BlockSize;

    private Vector2 m_BlockSize = new (53.333f, 30f); //Maybe later make it dynamic

    [SerializeField] private Camera m_MainCam;

    [ShowInInspector, ReadOnly] public ScreenData ScreenData;

    protected override void OnAwakeEvent()
    {
        base.OnAwakeEvent();
        ScreenData.CalculateData();

        m_BlockSize.y = OrthSize      * 2;
        m_BlockSize.x = m_BlockSize.y * m_MainCam.aspect;

        m_MainCam.targetTexture.width  = (int) ScreenData.OriginalRes.x;
        m_MainCam.targetTexture.height = (int) ScreenData.OriginalRes.y;
    }
}