using System;
using JetBrains.Annotations;
using PaintIn3D;
using UnityEngine;

public class MemoryBlock : MonoBehaviour
{
    public RenderTexture MemoryTexture => m_PaintableTexture.Current;
    public RenderTexture MemoryCamTexture => m_MemoryCam.targetTexture;

    [SerializeField] private P3dPaintableTexture m_PaintableTexture;
    [SerializeField] private Camera              m_MemoryCam;
    [SerializeField] private P3dPaintFill        m_PaintFill;


    private void OnEnable()
    {
        setCam();
        setScale();
    }

    private void setCam()
    {
        m_MemoryCam.orthographicSize = CameraManager.Instance.OrthSize;

        var rT = m_MemoryCam.targetTexture;
        rT.width  = (int) CameraManager.Instance.ScreenData.OriginalRes.x;
        rT.height = (int) CameraManager.Instance.ScreenData.OriginalRes.y;
    }

    private void setScale()
    {
        var blockSize = CameraManager.Instance.BlockSize;
        m_PaintableTexture.transform.localScale = new Vector3(blockSize.x, blockSize.y, 1.0f);
    }

    public void MoveMap([CanBeNull] Texture i_Texture, Vector2 i_MovePos)
    {
        if (i_Texture)
        {
            var m = m_PaintFill.BlendMode;
            m.Texture             = i_Texture;
            m_PaintFill.BlendMode = m;
            m_PaintFill.HandleHitPoint(false, 0, 1, 0, m_PaintableTexture.Paintable);
        }
        transform.position = i_MovePos;
    }

    public void Clear()
    {
        m_PaintableTexture.Clear();
    }
}