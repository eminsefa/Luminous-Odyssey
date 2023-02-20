Shader "Custom/LightVisibility"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LightPos ("Light Position", Vector) = (0.5, 0.5, 0, 0)
        _LightRange ("Light Range", Range(0,10000)) = 0.5
        _VisibilityFalloff ("Visibility Falloff", Range(0, 25)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "Queue"="Geometry"
        }
        Blend SrcAlpha OneMinusSrcAlpha

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
            float4 _MainTex_ST;
            float4 _LightPos;
            float _LightRange;
            float _VisibilityFalloff;

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
                float3 diff = i.worldPos - float3(_LightPos.xy, 0);
                float dist = length(diff);
                float visRange = _LightRange * _LightRange;
                float visibility = saturate(1 - dot(diff,diff) / visRange);
                visibility = pow(visibility, _VisibilityFalloff);
                
                fixed4 col = tex2D(_MainTex, i.uv);
                col.a = lerp(1, visibility, dist / _LightRange);
                
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}