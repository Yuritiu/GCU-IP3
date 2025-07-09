Shader "Unlit/EmissiveShader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0, 0, 0, 1)
        _EmissionColor ("Emission Color", Color) = (0, 1, 0, 1)
        _EmissionIntensity ("Emission Intensity", Float) = 1.0
        _MainTex ("Base Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            sampler2D _MainTex;
            float4 _BaseColor;
            float4 _EmissionColor;
            float _EmissionIntensity;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 baseCol = tex2D(_MainTex, i.uv) * _BaseColor;
                float4 emissive = _EmissionColor * _EmissionIntensity;

                return baseCol + emissive;
            }

            ENDHLSL
        }
    }
    FallBack "Unlit/Color"
}