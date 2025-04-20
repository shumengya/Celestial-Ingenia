Shader "Custom/2DGoldenOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1, 0.8, 0, 1)
        _OutlineWidth ("Outline Width", Range(0, 5)) = 1
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 1
        _GlowSpeed ("Glow Speed", Range(0, 10)) = 1
        _GradientSteps ("Gradient Steps", Range(1, 10)) = 3
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        // 绘制描边的 Pass
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

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
            float4 _MainTex_ST;
            fixed4 _OutlineColor;
            float _OutlineWidth;
            float _GlowIntensity;
            float _GlowSpeed;
            float _GradientSteps;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 计算描边
                float2 uvOffset = float2(_OutlineWidth / _ScreenParams.x, _OutlineWidth / _ScreenParams.y);
                fixed4 col = tex2D(_MainTex, i.uv);

                // 检查周围像素是否透明
                bool isOutline = false;
                float minDistance = 1000;
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x == 0 && y == 0) continue;
                        float2 sampleUV = i.uv + float2(x * uvOffset.x, y * uvOffset.y);
                        fixed4 sampleCol = tex2D(_MainTex, sampleUV);
                        if (sampleCol.a > 0 && col.a == 0)
                        {
                            isOutline = true;
                            float distance = length(float2(x, y));
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                            }
                        }
                    }
                }

                // 计算发光效果
                float glowFactor = sin(_Time.y * _GlowSpeed) * 0.5 + 0.5;
                glowFactor *= _GlowIntensity;

                // 如果是描边区域，应用发光颜色和渐变
                if (isOutline)
                {
                    float gradientFactor = minDistance / _GradientSteps;
                    col = _OutlineColor * glowFactor * (1 - gradientFactor);
                }

                return col;
            }
            ENDCG
        }

        // 绘制主纹理的 Pass
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

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
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}    