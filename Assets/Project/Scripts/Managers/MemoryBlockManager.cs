using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class MemoryBlockManager : Singleton<MemoryBlockManager>
{
    private Vector2 m_PlayerActiveBlockPos;
    private Camera  m_MainCam;
    private float   m_CamY;

    private Vector2   m_BlockSize => CameraManager.Instance.BlockSize;

    [ShowInInspector] private Dictionary<Vector2, Texture2D>         m_PermanentMemory = new();
    [ShowInInspector] private Dictionary<MemoryBlock, RenderTexture> m_TemporaryMemory = new();

    [SerializeField] private MemoryBlock[] m_MemoryBlocks;

#region Unity Events
    
    private void Start()
    {
        m_MainCam = CameraManager.Instance.MainCam;
        m_CamY    = m_MainCam.transform.position.y;
        for (var i = 0; i < m_MemoryBlocks.Length; i++)
        {
            var xPos = ((i % 3) - 1) * m_BlockSize.x;
            var yPos = -((i / 3) - 1) * m_BlockSize.y + m_CamY;
            var pos  = new Vector3(xPos, yPos, 0);
            m_MemoryBlocks[i].transform.position = pos;

            m_MemoryBlocks[i].gameObject.SetActive(true);
            m_PermanentMemory.Add(pos, convertRenderTextureToTexture2D(m_MemoryBlocks[i].TempMemoryTexture));
            m_TemporaryMemory.Add(m_MemoryBlocks[i], m_MemoryBlocks[i].TempMemoryTexture);
        }
    }

    private void Update()
    {
        var playerPos = PlayerController.Instance.transform.position;

        Vector2 activeBlockCenter = new Vector2(
                                                Mathf.RoundToInt(playerPos.x / m_BlockSize.x) * m_BlockSize.x,
                                                Mathf.RoundToInt(playerPos.y / m_BlockSize.y) * m_BlockSize.y
                                               );

        if (activeBlockCenter != m_PlayerActiveBlockPos)
        {
            m_PlayerActiveBlockPos = activeBlockCenter;
            setMemoryBlocks();
        }
    }

#endregion

    private void setMemoryBlocks()
    {
        foreach (var m in m_MemoryBlocks)
        {
            var mPos = m.transform.position;
            var dif  = m_PlayerActiveBlockPos - new Vector2(mPos.x, mPos.y - m_CamY);

            var moveX = Mathf.Abs(dif.x) > m_BlockSize.x * Random.Range(1.55f, 1.75f);
            var moveY = Mathf.Abs(dif.y) > m_BlockSize.y * Random.Range(1.55f, 1.75f);
            if (moveX || moveY)
            {
                var movePos = new Vector2(
                                          moveX ? m_PlayerActiveBlockPos.x + m_BlockSize.x * Mathf.Sign(dif.x) : m.transform.position.x,
                                          moveY ? m_PlayerActiveBlockPos.y + m_BlockSize.y * Mathf.Sign(dif.y) + m_CamY : m.transform.position.y
                                         );
                MoveBlockPosition(m, movePos);
            }
        }
    }
    
    private void MoveBlockPosition(MemoryBlock i_MemoryBlock, Vector2 i_MovePos)
    {
        var copyTex = convertRenderTextureToTexture2D(i_MemoryBlock.TempMemoryTexture);
        m_PermanentMemory[i_MemoryBlock.transform.position] = copyTex;

        i_MemoryBlock.Clear();
        Texture2D tex = null;
        if (!m_PermanentMemory.ContainsKey(i_MovePos))
        {
            m_PermanentMemory.Add(i_MovePos, convertRenderTextureToTexture2D(i_MemoryBlock.TempMemoryTexture));
        }
        else tex = m_PermanentMemory[i_MovePos];

        i_MemoryBlock.MoveMap(tex, i_MovePos);
    }

    private Texture2D convertRenderTextureToTexture2D(RenderTexture i_RenderTexture)
    {
        var texture = new Texture2D(i_RenderTexture.width, i_RenderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = i_RenderTexture;

        texture.ReadPixels(new Rect(0, 0, i_RenderTexture.width, i_RenderTexture.height), 0, 0);
        texture.Apply();

        RenderTexture.active = null;

        return texture;
    }

    public bool IsPointPainted(Vector3 i_Point)
    {
        var x = Mathf.RoundToInt((i_Point.x           / m_BlockSize.x)) * m_BlockSize.x;
        var y = Mathf.RoundToInt((i_Point.y - m_CamY) / m_BlockSize.y) * m_BlockSize.y + m_CamY;

        var point = new Vector2(x, y);
        var tex   = m_PermanentMemory[point];

        foreach (var m in m_MemoryBlocks)
        {
            if (((Vector2)m.transform.position - point).sqrMagnitude < 1)
            {
                tex = convertRenderTextureToTexture2D(m.TempMemoryTexture);
                break;
            }
        }

        var screenX = ((i_Point.x - x) / m_BlockSize.x + 0.5f) * tex.width;
        var screenY = ((i_Point.y - y) / m_BlockSize.y + 0.5f) * tex.height;

        return tex.GetPixel((int) screenX, (int) screenY).a > 0.25f;
    }
}