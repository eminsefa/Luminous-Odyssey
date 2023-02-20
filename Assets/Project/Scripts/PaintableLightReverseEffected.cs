using PaintIn3D;
using UnityEngine;

public class PaintableLightReverseEffected : MonoBehaviour
{
    [SerializeField] private P3dMaterialCloner m_MaterialCloner;
    
    public void OnActivated()
    {
        LightSetter.Instance.LightReverseEffectedMat = m_MaterialCloner.Current;
    }
}