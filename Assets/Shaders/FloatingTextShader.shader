Shader "Custom/FloatingTextShader" {
    Properties {
        _MainTex ("Font Texture", 2D) = "white" {}
        _Color ("Text Color", Color) = (1,1,1,1)
        _Alpha ("Alpha", Range(0, 1)) = 1
        _OffsetX ("Offset X", Float) = 0
        _OffsetY ("Offset Y", Float) = 0
        _JumpHeight ("Jump Height", Float) = 1.0
        _JumpSpeed ("Jump Speed", Float) = 3.0
        _FloatAmplitude ("Float Amplitude", Float) = 0.3
        _FloatSpeed ("Float Speed", Float) = 2.0
        _RandomSeed ("Random Seed", Float) = 0
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Alpha;
            float _OffsetX;
            float _OffsetY;
            float _JumpHeight;
            float _JumpSpeed;
            float _FloatAmplitude;
            float _FloatSpeed;
            float _RandomSeed;

            // Simple hash function for pseudo-randomness
            float hash(float2 p) {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            v2f vert (appdata v) {
                v2f o;
                float4 worldPos = v.vertex;
                
                // Create a random offset for this vertex
                float randomValue = hash(worldPos.xy + _RandomSeed);
                float randomX = hash(worldPos.yx + _RandomSeed * 2.1) * 2.0 - 1.0;
                
                // Time-based animation
                float t = _Time.y;
                
                // Initial jump phase (0-1 seconds)
                float jumpPhase = min(t, 1.0) * _JumpSpeed;
                float jumpOffset = sin(jumpPhase * 3.14159) * _JumpHeight;
                
                // Floating phase (after 1 second)
                float floatPhase = max(0, t - 1.0);
                float fallOffset = -floatPhase * 0.5 * randomValue;
                float wobbleX = sin(floatPhase * _FloatSpeed + randomValue * 6.28) * _FloatAmplitude * randomX;
                float wobbleY = sin(floatPhase * _FloatSpeed * 0.7 + randomValue * 6.28) * _FloatAmplitude * 0.5;
                
                // Combine all offsets
                worldPos.x += wobbleX + _OffsetX;
                worldPos.y += jumpOffset + fallOffset + wobbleY + _OffsetY;
                
                o.vertex = UnityObjectToClipPos(worldPos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                col.a *= _Alpha;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}    