Shader "Hanadayori/AlwaysOnTop"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)    // 色と透明度を設定
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }  // 透明物を描画するQueueに設定
        LOD 200

        Pass
        {
            ZTest Always       // 常にレンダリング
            ZWrite Off         // 深度バッファへの書き込みを無効化
            Blend SrcAlpha OneMinusSrcAlpha  // アルファブレンディングを有効化

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // テクスチャと色を掛け合わせ、透明度を反映
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 finalColor = texColor * _Color;
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Transparent"
}