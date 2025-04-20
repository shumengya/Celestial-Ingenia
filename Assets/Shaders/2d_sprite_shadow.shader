Shader "Custom/2DSpriteShadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ShadowColor ("Shadow Color", Color) = (0, 0, 0, 0.5)
        _ShadowOffsetX ("Shadow Offset X", Range(-1, 1)) = 0.1
        _ShadowOffsetY ("Shadow Offset Y", Range(-1, 1)) = -0.1
        _ShadowFlattenFactor ("Shadow Flatten Factor", Range(0, 1)) = 0.5
        _ShadowStretchX ("Shadow Stretch X", Range(0, 5)) = 1
        _ShadowStretchY ("Shadow Stretch Y", Range(0, 5)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        // 绘制影子的 Pass
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
            fixed4 _ShadowColor;
            float _ShadowOffsetX;
            float _ShadowOffsetY;
            float _ShadowFlattenFactor;
            float _ShadowStretchX;
            float _ShadowStretchY;

            v2f vert (appdata v)
            {
                v2f o;
                // 扁平化处理
                v.vertex.y *= _ShadowFlattenFactor;
                // 拉伸处理
                v.vertex.x *= _ShadowStretchX;
                v.vertex.y *= _ShadowStretchY;
                // 应用影子偏移
                v.vertex.xy += float2(_ShadowOffsetX, _ShadowOffsetY);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 采样主纹理
                fixed4 col = tex2D(_MainTex, i.uv);
                // 应用影子颜色
                col.rgb = _ShadowColor.rgb;
                return col;
            }
            ENDCG
        }

        // 绘制主精灵的 Pass
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