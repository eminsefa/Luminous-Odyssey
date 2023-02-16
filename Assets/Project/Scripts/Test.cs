using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private Material mat;

    [Button]
    public void TestButton()
    {
        Debug.LogError(mat.GetFloat("_Col"));
    }
    [Button]
    public void TestButton2()
    {
        Debug.LogError(mat.GetFloat("_Radius"));
    }
}
