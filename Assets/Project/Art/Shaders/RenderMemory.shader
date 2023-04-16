Shader "Unlit/RenderMemory"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _MaskTexLight ("Mask Texture Visible", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _LightRange ("Light Range", Range(0,5000)) = 7.5
        _VisibilityFalloff ("Visibility Falloff", Range(0, 25)) = 1
        _BlindShineColor ("Blind Shine Color",Color)=(1,1,1,1)
        _BlindShineIntensity ("Blind Shine Intensity",float)=1
        _BlindShineTex ("Blind Shine Texture", 2D) = "white" {}
        _BlindShineSpeed ("Blind Shine Speed", Range(0, 10)) = 1
        _WaveFrequency ("Wave Frequency", Range(0, 10)) = 1
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

        Pass
        {

            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
                UNITY_FOG_COORDS(1)
            };

            sampler2D _MainTex;
            sampler2D _MaskTex;
            sampler2D _MaskTexLight;
            sampler2D _LightTexture;
            float4 _MainTex_ST;
            float4 _MaskTex_ST;
            float4 _MaskTexLight_ST;
            float4 _Color;
            float _LightCount;
            float _LightRange;
            float _VisibilityFalloff;

            sampler2D _BlindShineTex;
            float4 _BlindShineTex_ST;;
            float4 _BlindShineColor;;
            float _BlindShineIntensity;
            float _BlindShineSpeed;
            float _BlindFactor;
            float _WaveFrequency;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 maskCol = tex2D(_MaskTex, TRANSFORM_TEX(i.uv, _MaskTex));
                fixed4 maskLightCol = tex2D(_MaskTexLight, TRANSFORM_TEX(i.uv, _MaskTexLight));
                fixed4 col = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex)) * _Color;

                float maskAlpha = maskCol.a;
                float maskLightAlpha = 1 - pow(maskLightCol.a, _VisibilityFalloff);

                float waveX = _WaveFrequency * sin(_Time.y * _BlindShineSpeed );
                float waveY = _WaveFrequency * cos(_Time.y * _BlindShineSpeed );
                float2 noiseUV = TRANSFORM_TEX(i.uv, _BlindShineTex) + float2(waveX, waveY);
                fixed4 noise = tex2D(_BlindShineTex, noiseUV);

                float intensity=lerp(0,_BlindShineIntensity,_BlindFactor);
                col.rgb += noise.r * _BlindShineColor * intensity ;

                col.a *= lerp(maskAlpha, maskLightAlpha, maskLightCol.a);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}