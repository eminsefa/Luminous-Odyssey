using UnityEngine;

public struct ShaderIDs
{
    public static readonly int S_LightPos             = Shader.PropertyToID("_LightPos");
    public static readonly int S_LightRange           = Shader.PropertyToID("_LightRange");
    public static readonly int S_LightCount           = Shader.PropertyToID("_LightCount");
    public static readonly int S_VisibilityFalloff    = Shader.PropertyToID("_VisibilityFalloff");
    public static readonly int S_LightTexture         = Shader.PropertyToID("_LightTexture");
    public static readonly int S_ManaObjectLightRange = Shader.PropertyToID("_ManaObjectLightRange");
}