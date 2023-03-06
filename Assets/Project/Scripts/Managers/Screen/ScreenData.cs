namespace Managers
{
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public struct ScreenData
{
    public Vector2 ScreenSizeInch => screenSizeInch;

    [field:ShowInInspector, ReadOnly] public Vector2 OriginalRes;
    [ShowInInspector, ReadOnly] private float originalDPI;

    [ShowInInspector, ReadOnly] private Vector2 finalRes;
    [ShowInInspector, ReadOnly] public  float   FinalDPI { get; private set; }

    [ShowInInspector, ReadOnly] private Vector2 screenSizeInch;
    [ShowInInspector, ReadOnly] private Vector2 screenSizeCm;
    
    [ShowInInspector, ReadOnly] private Vector2 scalingVector;
    [ShowInInspector, ReadOnly] public  float   Scaling;
    
    
    public void CalculateIfScreenChanges()
    {
        if (OriginalRes != new Vector2(Screen.width, Screen.height))
            CalculateData();
    }

    public void CalculateData(bool debugLog=false)
    {
        OriginalRes = new Vector2(Screen.width, Screen.height);
        originalDPI = Screen.dpi;

#if UNITY_EDITOR
        scalingVector = GameViewEditorRes.GetGameViewScale();
#else
        scalingVector = Vector3.one;
#endif
        Scaling = (scalingVector.x + scalingVector.y) * .5f;

        finalRes = new Vector2(Screen.width * scalingVector.x, Screen.height * scalingVector.y);

        if (Screen.dpi == 0)
        {
            FinalDPI = 320;
        }
        else
        {
            FinalDPI = Screen.dpi * Scaling;
        }

        screenSizeInch = new Vector2((finalRes.x / FinalDPI) * scalingVector.x, (finalRes.y / FinalDPI) * scalingVector.y);
        screenSizeCm   = screenSizeInch * 2.54f;

        if(debugLog)
        {
            Debug.LogError("Original Resolution - " + OriginalRes);
            Debug.LogError("Original DPI - "        + originalDPI);
            Debug.LogError("Resolution - "          + finalRes);
            Debug.LogError("DPI - "                 + FinalDPI);
            Debug.LogError("Size Inches - "         + screenSizeInch);
            Debug.LogError("Size Cm - "             + screenSizeCm);

            Debug.LogError("Scaling Vector - " + scalingVector);
            Debug.LogError("Scaling - "        + Scaling);
        }
    }
}

}