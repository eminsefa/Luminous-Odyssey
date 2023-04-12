Shader "Unlit/Moving With Light Object"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _ColorChangeSpeed ("Color Change Speed", Range(0, 10)) = 1
        _ColorStrength ("Color Strength", Range(0, 1)) = 0.5
        _NumCirclesRange ("Number of Circles Range", Vector) = (1, 5,0,0)
        _CircleSizeRange ("Circle Size Range", Vector) = (0.1, 0.3,0,0)
        _SpawnIntervalRange ("Spawn Interval Range", Vector) = (0.5, 2,0,0)
        _MaxBrightnessPlus ("Max Brightness Plus", Range(0, 5)) = 1
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
            float2 _NumCirclesRange;
            float2 _CircleSizeRange;
            float2 _SpawnIntervalRange;
            float _MaxBrightnessPlus;

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

            float smoothcircle(float2 uv, float2 center, float radius)
            {
                float dist = length(uv - center);
                float edgeDist = max(0.0, 1.0 - (dist / radius));
                return _MaxBrightnessPlus * pow(edgeDist, 3.0);
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

                float shine = 0.0;
                float time = _Time.y;
                
                int numCircles = int(lerp(_NumCirclesRange.x, _NumCirclesRange.y, random(0.0)));

                float interval = lerp(_SpawnIntervalRange.x, _SpawnIntervalRange.y, random(0.0));
                float t = fmod(time, interval) / interval;
                if (t < 1.0 && shine==0)
                {
                    for (int j = 0; j < numCircles; j++)
                    {
                        float seed = float(j) * 47.0;
                        float2 randomPos = float2(random(seed + 10.0 * t), random(seed + 1.0 + 10.0 * t));
                        float circleSize = lerp(_CircleSizeRange.x, _CircleSizeRange.y, random(seed + 2.0 + 10.0 * t));
                        float fadeInOut = (1.0 - abs(2.0 * t - 1.0));
                        shine += fadeInOut * smoothcircle(i.uv, randomPos, circleSize);
                    }
                }


                shine /= float(numCircles);

                col.rgb = lerp(col.rgb, gradientColor, _ColorStrength) + col.rgb * shine;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}