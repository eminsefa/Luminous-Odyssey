Shader "Custom/SpaceStars"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("Texture", 2D) = "white" {}
        _MaskTexLight ("Texture", 2D) = "white" {}
        _StarAmount ("Star Amount", Range(0, 1000)) = 100
        _StarSize ("Star Size", float) = 1
        _FlickerFrequency("Flicker Frequency",float)=2
        _FlickerIntensity("Flicker Intensity",float)=0.5
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "RenderType"="Transparent"
        }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Zwrite Off
        Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _MaskTex;
            sampler2D _MaskTexLight;
            float4 _MaskTex_ST;
            float4 _MaskTexLight_ST;
            float _StarAmount;
            float _StarSize;
            float _FlickerFrequency;
            float _FlickerIntensity;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float rndStar = frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
                float rndFlicker = frac(sin(dot(uv, float2(12.9898 * 3.33, 78.233 * 3.33))) * 43758.5453);

                float starThreshold = 1.0 - _StarAmount / 1000.0;

                float timeOffset = rndFlicker * 1000.0; 
                float timeBasedFlicker = sin((_Time.y + timeOffset) * _FlickerFrequency) * _FlickerIntensity + (1.0 - _FlickerIntensity);

                float4 col = 0;
                if (rndStar > starThreshold)
                {
                    col = float4(1.0, 1.0, 1.0, 1.0) * 1 * timeBasedFlicker;
                }
                fixed4 maskLightCol = tex2D(_MaskTexLight, TRANSFORM_TEX(i.uv, _MaskTexLight));
                float maskLightAlpha = 1 - maskLightCol.a;

                // fixed4 maskCol = tex2D(_MaskTex, TRANSFORM_TEX(i.uv, _MaskTex));
                // float maskAlpha = maskCol.a;

                // col.a *= lerp(maskAlpha, maskLightAlpha, maskLightCol.a);
                col.a *= maskLightAlpha;

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}