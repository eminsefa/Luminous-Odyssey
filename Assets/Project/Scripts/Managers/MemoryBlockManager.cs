using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sirenix.OdinInspector;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MemoryBlockManager : MonoBehaviour
{
    private Vector2 m_PlayerActiveBlockPos;
    private Camera  m_MainCam;
    private float   m_CamY;

    private Vector2 m_BlockSize => CameraManager.Instance.BlockSize;

    [ShowInInspector] private Dictionary<Vector2, Texture> m_PermanentMemory = new();

    [SerializeField] private MemoryBlock[] m_MemoryBlocks;

    private void Start()
    {
        m_MainCam = Camera.main;
        m_CamY    = m_MainCam.transform.position.y;
        for (var i = 0; i < m_MemoryBlocks.Length; i++)
        {
            var xPos = ((i % 3) - 1) * m_BlockSize.x;
            var yPos = -((i / 3) - 1) * m_BlockSize.y + m_CamY;
            var pos  = new Vector3(xPos, yPos, 0);
            m_MemoryBlocks[i].transform.position = pos;

            m_MemoryBlocks[i].gameObject.SetActive(true);
            m_PermanentMemory.Add(pos,m_MemoryBlocks[i].MemoryTexture);
        }
    }

    private void Update()
    {
        var playerPos = PlayerMovement.Instance.transform.position;

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


    private void setMemoryBlocks()
    {
        foreach (var m in m_MemoryBlocks)
        {
            var dist = m_PlayerActiveBlockPos - (Vector2) m.transform.position;

            var moveX = Mathf.Abs(dist.x) > m_BlockSize.x * 1.75f;
            var moveY = Mathf.Abs(dist.y) > m_BlockSize.y * 1.75f;
            if (moveX || moveY)
            {
                var movePos = new Vector2(
                                          moveX ? m_BlockSize.x * Mathf.Sign(dist.x): m.transform.position.x,
                                          moveY ? m_BlockSize.y * Mathf.Sign(dist.y) + m_CamY: m.transform.position.y
                                         );
                MoveBlockPosition(m,  m_PlayerActiveBlockPos + movePos);
            }
        }
    }


    private void MoveBlockPosition(MemoryBlock i_MemoryBlock, Vector2 i_MovePos)
    {
        copyTexture(i_MemoryBlock);

        Texture tex = null;
        if (!m_PermanentMemory.ContainsKey(i_MovePos))
        {
            m_PermanentMemory.Add(i_MovePos, i_MemoryBlock.MemoryCamTexture);
        }
        else tex = m_PermanentMemory[i_MovePos];
        
        i_MemoryBlock.Clear();
        i_MemoryBlock.MoveMap(tex, i_MovePos);
    }
    
    private void copyTexture(MemoryBlock i_MemoryBlock)
    {
        var rt = i_MemoryBlock.MemoryTexture;
        var newTexture = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
        RenderTexture.active = rt;
        
        newTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        newTexture.Apply();
        
        RenderTexture.active = null;
        m_PermanentMemory[i_MemoryBlock.transform.position] = newTexture;
    }
}