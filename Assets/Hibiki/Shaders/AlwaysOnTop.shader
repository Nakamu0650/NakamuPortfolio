Shader "Hanadayori/AlwaysOnTop"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Overlay" } // オーバーレイとして描画
        LOD 100

        Pass
        {
            Name "AlwaysOnTopPass"
            Tags { "LightMode" = "UniversalForward" }

            ZTest Always     // 常に描画
            ZWrite Off       // Zバッファに書き込まない
            Cull Off         // カリングを無効化
            Blend SrcAlpha OneMinusSrcAlpha // 透過処理を有効化

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
                half2 uv : TEXCOORD0;
            };

            sampler2D _BaseMap;
            float4 _BaseColor;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 baseColor = tex2D(_BaseMap, IN.uv) * _BaseColor;
                return baseColor;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}