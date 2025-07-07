Shader "Unlit/CodeRain"
{
    Properties
    {
        _MainTex ("Digit Atlas (0–9)", 2D) = "white" {}
        _GlowColour ("Digit Glow Colour", Color) = (0, 1, 0, 1)
        _FlashColour ("Flash Colour", Color) = (1, 1, 1, 1)
        _Speed ("Scroll Speed", Float) = 1.0
        _Scale ("Digit Size", Float) = 1.0
        _ColumnSpacing ("Column Spacing", Float) = 1.2
        _LayerCount ("Depth Layers", Int) = 4
        _ScanlineStrength ("Scanline Strength", Float) = 0.1
        _ScanlineSpeed ("Scanline Speed", Float) = 2.0

        _GlitchPeriod ("Glitch Period (s)", Float) = 5.0
        _GlitchDuration ("Glitch Duration (s)", Float) = 0.3
        _GlitchIntensity ("Glitch Intensity", Float) = 0.05

        _UVInset ("UV Inset", Float) = 0.005
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            sampler2D _MainTex;
            float4 _GlowColour;
            float4 _FlashColour;
            float _Speed;
            float _Scale;
            float _ColumnSpacing;
            int _LayerCount;
            float _ScanlineStrength;
            float _ScanlineSpeed;

            float _GlitchPeriod;
            float _GlitchDuration;
            float _GlitchIntensity;

            float _UVInset;

            //Vertex Input Structure
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            //Vertex to Fragment Output Structure
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            //Hash For Random Number Generation Based on 2D Position
            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

             //Vertex shader -> Pass UV & Transform Vertex
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            //Fragment Shader -> Render Code Rain
            float4 frag(v2f i) : SV_Target
            {
                float phase = frac(_Time.y / _GlitchPeriod);
                bool glitchActive = phase < (_GlitchDuration / _GlitchPeriod);

                float2 globalJitter = float2(0, 0);
                if (glitchActive)
                {
                    float seed = hash21(float2(phase, phase * 1.37));
                    float angle = seed * 6.2831853;
                    globalJitter = float2(cos(angle), sin(angle)) * _GlitchIntensity;
                }

                //Background Colour
                float4 finalColour = float4(0, 0, 0, 1);

                float2 baseUV = float2(i.uv.y - 1.0, 1.0 - i.uv.x) + globalJitter;

                //Loop Through Depth Layers
                int layers = clamp(_LayerCount, 1, 10);
                for (int layer = 0; layer < layers; layer++)
                {
                    //Depth Based Variation For Parallax Effect
                    float depthFactor = float(layer) / float(layers);
                    float scrollSpeed = lerp(_Speed * 0.5, _Speed * 2.0, depthFactor);
                    float brightness = lerp(1.0, 0.2, depthFactor);

                    //Apply Horizontal Scanline Distortion
                    float scanDistort = sin((baseUV.y + _Time.y * _ScanlineSpeed + layer) * 50.0) * _ScanlineStrength;
                    float2 uv = baseUV * _Scale + float2(scanDistort, 0);

                    //Scroll Digits Downward Over Time
                    uv.y -= _Time.y * scrollSpeed + layer * 10.0;

                    //Convert UV to A Grid Cell ID
                    float2 gridUV = uv * float2(20 / _ColumnSpacing, 40);
                    float2 cellID = floor(gridUV);
                    float colFrac = frac(gridUV.x);

                    //Create Gaps Between Columns
                    if (colFrac < 0.3 || colFrac > 0.7)
                        continue;

                    //Select Only Digit 1/ 0 Based on Randomness
                    float digitRand = hash21(cellID + floor(_Time.y));
                     //1st/ 10th Digit (1/ 0)
                    float digitIndex = (digitRand < 0.5) ? 0.0 : 9.0;

                    //Digit Flickering
                    float flicker = lerp(0.8, 1.2, hash21(cellID + _Time.y * 5.0));

                    //Occasional Darker Spots
                    float flashTime = floor(_Time.y * 2.0);
                    float flashChance = step(0.92, hash21(cellID + flashTime));
                    float flashStr = lerp(1.0, 2.5, flashChance);
                    float4 flashColour = lerp(_GlowColour, _FlashColour, flashChance);

                    //Get Local UV Within Digit Cell
                    float fracX = frac(gridUV.x);
                    float fracY = frac(gridUV.y);
                    float cellWidth = 1.0 / 10.0;

                    //Inset to Prevent Overlap Between Digits
                    float inset = _UVInset;
                    float localX = inset + fracX * (1.0 - 2.0 * inset);

                    float2 sampleUV;

                    //Flip UV''s Within Digit to Rotate Glyph 180 Degrees
                    float flippedX = 1.0 - localX;
                    float flippedY = 1.0 - fracY;

                    sampleUV.x = digitIndex * cellWidth + flippedX * cellWidth;
                    sampleUV.y = flippedY;

                    //Sample The Digit Texture
                    float4 sampled = tex2D(_MainTex, sampleUV);
                    clip(sampled.a - 0.01);

                    finalColour.rgb += sampled.rgb * flashColour.rgb * flicker * flashStr * brightness;
                }

                //Apply Final Scanline Flicker to The Entire Colour
                float scanline = sin((baseUV.y + _Time.y * _ScanlineSpeed) * 200.0) * 0.5 + 0.5;
                finalColour.rgb *= lerp(0.9, 1.1, scanline);

                return finalColour;
            }

            ENDHLSL
        }
    }
}