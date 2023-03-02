using PaintIn3D;
using UnityEngine;

public class MemoryMap : MonoBehaviour
{
    private Camera  m_MainCamera;

    [SerializeField] private P3dMaterialCloner m_MaterialCloner;

    private void Awake()
    {
        m_MainCamera = Camera.main;
        setScale();
    }

    private void setScale()
    {
        float screenHeight = m_MainCamera.orthographicSize * 2.0f;
        float screenWidth  = screenHeight                  * m_MainCamera.aspect;

        transform.localScale = new Vector3(screenWidth, screenHeight, 1.0f);
    }

    public void OnActivated()
    {
        LightSetter.Instance.LightReverseEffectedMat = m_MaterialCloner.Current;
    }
}