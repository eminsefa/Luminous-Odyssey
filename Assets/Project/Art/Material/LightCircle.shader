Shader "Custom/LightCircle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LightPos ("Light Position", Vector) = (0.5, 0.5, 0, 0)
        _LightRange ("Light Range", Range(0,5000)) = 7.5
        _VisibilityFalloff ("Visibility Falloff", Range(0, 25)) = 1


        _NumberOfHalos ("Number of Halos", int) = 4
        _RotationSpeed ("Rotation Speed", Range(0, 10)) = 1
        _HaloThickness ("Halo Thickness", Range(0, 1)) = 0.1
        _SizeChangeSpeed ("Size Change Speed", Range(0, 10)) = 1
        _HaloBendRange ("Halo Bend Range", Range(0, 1)) = 0.2
        _HaloRandomSeed ("Halo Random Seed", Range(0, 100)) = 42
        _HaloMinMaxSize ("Halo Min Max Length", Vector) = (0.25, 0.5,0,0)
        _HaloSizeMultiplier ("Halo Size Multiplier", float) =1
        _HaloThicknessMultiplier ("Halo Thickness Multiplier", float) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "Queue"="Geometry"
        }

        Blend One OneMinusSrcAlpha
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
            float4 _MainTex_ST;
            float4 _LightPos;
            float _LightRange;
            float _VisibilityFalloff;
            int _NumberOfHalos;
            float _RotationSpeed;
            float _HaloThickness;
            float _SizeChangeSpeed;
            float _HaloBendRange;
            float _HaloRandomSeed;
            float2 _HaloMinMaxSize;
            float _HaloSizeMultiplier;
            float _HaloThicknessMultiplier;

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
                float visibility = saturate(1 - dot(diff, diff) / visRange);
                visibility = pow(visibility, _VisibilityFalloff);

                fixed4 col = tex2D(_MainTex, i.uv) * lerp(1, visibility, dist / _LightRange);

                float2 normDiff = normalize(diff.xy);

                for (int n = 0; n < _NumberOfHalos; n++)
                {
                    float randomStartAngle = frac(sin(float(n) * 1.618 + _HaloRandomSeed) * 43758.5453) * 2.0 * 3.14159265f;
                    float directionMultiplier = (n % 2 == 0) ? 1.0 : -1.0;
                    float currentAngle = _Time.y * _RotationSpeed * directionMultiplier + randomStartAngle;

                    float2 haloDirection = float2(cos(currentAngle), sin(currentAngle));
                    float haloRadius = lerp(_LightRange, _LightRange * (1 - _HaloBendRange), float(n) / float(_NumberOfHalos));
                    float haloAngle = acos(dot(normDiff, haloDirection));

                    float randomScale = (sin(float(n) * 12.9898 + 78.233 + _Time.y * _SizeChangeSpeed) + 1) * 0.5;
                    float haloSize = lerp(_HaloMinMaxSize.x, _HaloMinMaxSize.y, randomScale) * 3.14159265f * pow(_HaloSizeMultiplier, n);

                    float haloVisibility = saturate(1.0 - abs(haloAngle) / haloSize);

                    float edgeDist = abs(dist - haloRadius);
                    float currentHaloThickness = _HaloThickness * _LightRange * pow(_HaloThicknessMultiplier, n);
                    haloVisibility *= saturate((currentHaloThickness - edgeDist) / currentHaloThickness);

                    haloVisibility *= currentHaloThickness;

                    col.rgb += haloVisibility * visibility;
                }

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}