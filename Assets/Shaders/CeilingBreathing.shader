Shader "Unlit/CeilingBreathing"
{
    Properties
    {
        _BaseColour ("Base Colour", Color) = (0.0, 0.1, 0.05, 1)
        _GlowColour ("Glow Colour", Color) = (0.0, 1.0, 0.3, 1)
        _PulseSpeed ("Pulse Speed", Float) = 1.5
        _PulseIntensity ("Pulse Intensity", Float) = 0.5
        _EdgeGlowStrength ("Edge Glow Strength", Float) = 1.2
        _EdgeSharpness ("Edge Sharpness", Float) = 5.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            float4 _BaseColour;
            float4 _GlowColour;
            float _PulseSpeed;
            float _PulseIntensity;
            float _EdgeGlowStrength;
            float _EdgeSharpness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                o.worldPos = TransformObjectToWorld(v.vertex.xyz);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float pulse = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5;
                float intensity = lerp(1.0, 1.0 + _PulseIntensity, pulse);

                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float facing = dot(normalize(i.worldNormal), viewDir);
                float edge = pow(1.0 - saturate(facing), _EdgeSharpness);

                float3 colour = _GlowColour.rgb * edge * pulse;

                float edgeNoise = sin(i.worldPos.x * 5.0 + _Time.y * 2.0) * 0.5 + 0.5;
    float edgeHighlight = pow(edgeNoise, 4.0) * pulse;
    colour += _GlowColour.rgb * edgeHighlight * 0.3;

                return float4(colour, 1.0);
            }
            ENDHLSL
        }
    }
}