Shader "Unlit/Moving With Light Object"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Intensity ("Intensity",float)=1
        _ColorChangeSpeed ("Color Change Speed", Range(0, 10)) = 1
        _ColorStrength ("Color Strength", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "RenderType"="Transparent"
        }
        LOD 100

        PASS
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
            float _ColorChangeSpeed;
            float _ColorStrength;
            float _Intensity;

            float3 rgb2hsv(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float3 hsv2rgb(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }

            float random(float seed)
            {
                return frac(sin(seed * 12345.6789) * 98765.4321);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 hsv = rgb2hsv(col.rgb);
                hsv.x += _Time.y * _ColorChangeSpeed;
                float3 gradientColor = hsv2rgb(hsv);

                col.rgb = lerp(col.rgb, gradientColor, _ColorStrength) *_Intensity+ col.rgb ;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}