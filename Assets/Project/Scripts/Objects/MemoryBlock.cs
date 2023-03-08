using JetBrains.Annotations;
using PaintIn3D;
using UnityEngine;

public class MemoryBlock : MonoBehaviour
{
    public RenderTexture MemoryTexture => m_PaintableTexture.Current;

    [SerializeField] private P3dPaintableTexture m_PaintableTexture;
    [SerializeField] private P3dPaintFill        m_PaintFill;

    private void OnEnable()
    {
        setScale();
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