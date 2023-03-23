Shader "Unlit/MemoryMaskObject"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FalloffRadius ("Falloff Radius", Range(0, 1)) = 0.8
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Zwrite Off
        Cull Off

        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        float _FalloffRadius;

        struct Input
        {
            float2 uv_MainTex;
        };

               void surf (Input IN, inout SurfaceOutput o)
        {
            // Sample the main texture
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

            // Calculate the distance from the center
            float dist = distance(IN.uv_MainTex, float2(0.5, 0.5));

            // Apply the falloff effect
            float falloff = smoothstep(_FalloffRadius, 1.0, dist);

            // Apply the falloff to the alpha channel
            c.a *= falloff;

            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

