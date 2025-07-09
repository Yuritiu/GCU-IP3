Shader "Unlit/CRTWhiteFlicker"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _FlickerSpeed ("Flicker Speed", Float) = 20.0
        _FlickerMin ("Flicker Min", Float) = 0.8
        _FlickerMax ("Flicker Max", Float) = 1.0
        [HDR] _EmissionColor ("Emission Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Name "CRT Flicker"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // Shader properties
            float4 _BaseColor;
            float _FlickerSpeed;
            float _FlickerMin;
            float _FlickerMax;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float remap(float value, float inMin, float inMax, float outMin, float outMax)
            {
                return outMin + (value - inMin) * (outMax - outMin) / (inMax - inMin);
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float flicker = sin(_Time.y * _FlickerSpeed);
                float brightness = remap(flicker, -1.0, 1.0, _FlickerMin, _FlickerMax);
                float4 finalColor = _BaseColor * brightness;
                return finalColor;
            }

            ENDHLSL
        }

        // Optional: Emission pass for bloom
        Pass
        {
            Name "CRT Emission"
            Tags { "LightMode"="UniversalForwardOnly" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragEmission
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _EmissionColor;
            float _FlickerSpeed;
            float _FlickerMin;
            float _FlickerMax;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float remap(float value, float inMin, float inMax, float outMin, float outMax)
            {
                return outMin + (value - inMin) * (outMax - outMin) / (inMax - inMin);
            }

            half4 fragEmission(Varyings IN) : SV_Target
            {
                float flicker = sin(_Time.y * _FlickerSpeed);
                float brightness = remap(flicker, -1.0, 1.0, _FlickerMin, _FlickerMax);
                return _EmissionColor * brightness;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/InternalErrorShader"
}