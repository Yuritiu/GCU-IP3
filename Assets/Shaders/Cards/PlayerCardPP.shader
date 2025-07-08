Shader "Unlit/PlayerCardPP"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _TintColour("Tint Color", Color) = (1,1,1,1)

        _ChromaticAberrationIntensity("Chromatic Aberration Intensity", Range(0, 0.1)) = 0.02
        _BloomIntensity("Bloom Intensity", Range(0, 5)) = 1.0
        _BloomThreshold("Bloom Threshold", Range(0,1)) = 0.5
        _BloomTint("Bloom Tint", Color) = (1,1,1,1)

        _GlitchIntensity("Glitch Intensity", Range(0, 0.05)) = 0.01
        _GlitchFrequency("Glitch Frequency", Range(0, 5)) = 1.5

        _HoverEffect("Hover Effect Enabled", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            sampler2D _MainTex;
            float4 _TintColour;

            float _ChromaticAberrationIntensity;
            float _BloomIntensity;
            float _BloomThreshold;
            float4 _BloomTint;

            float _GlitchIntensity;
            float _GlitchFrequency;

            float _HoverEffect;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            //Random Based on UV & Time For Glitch Pulse Timing
            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898,78.233))) * 43758.5453);
            }

            float4 frag(v2f i) : SV_Target
            {
                //Clamp Hover Between 0 - 1
                float hover = saturate(_HoverEffect);

                //Base Colour Sample
                float4 col = tex2D(_MainTex, i.uv) * _TintColour;

                //--- Chromatic Aberration (rgb offset) ---
                float caIntensity = _ChromaticAberrationIntensity * hover;

                float2 offsetR = float2(caIntensity, 0);
                float2 offsetB = float2(-caIntensity, 0);

                float4 colR = tex2D(_MainTex, i.uv + offsetR);
                float4 colG = tex2D(_MainTex, i.uv);
                float4 colB = tex2D(_MainTex, i.uv + offsetB);

                float4 caColour = float4(colR.r, colG.g, colB.b, col.a);

                //--- Bloom ---
                //Brightness Mask
                float brightness = dot(col.rgb, float3(0.299, 0.587, 0.114));
                float bloomMask = smoothstep(_BloomThreshold, 1.0, brightness);

                float4 bloomColour = col * _BloomTint * _BloomIntensity * bloomMask * hover;

                //--- Glitch effect ---
                //Random Glitch Pulses on X Offset
                float glitchPulse = step(0.99, rand(float2(_Time.y * _GlitchFrequency, i.uv.y)));
                float glitchOffset = glitchPulse * _GlitchIntensity * hover;

                float2 glitchUV = i.uv + float2(glitchOffset, 0);

                float4 glitchSample = tex2D(_MainTex, glitchUV);

                //Subtly Mix Glitch Effect
                float4 finalColour = lerp(caColour, glitchSample, glitchPulse * hover);

                //Add Bloom on Top
                finalColour.rgb += bloomColour.rgb;

                //Multiply by Base Alpha
                finalColour.a = col.a;

                return finalColour;
            }

            ENDHLSL
        }
    }
}