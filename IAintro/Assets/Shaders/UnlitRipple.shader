Shader "Custom/UnlitRipple"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
        _RippleTex ("Ripple Tex", 2D) = "black" {}
        _Strength ("Displacement Strength", Range(0,0.1)) = 0.03
        _Tint ("Tint Color", Color) = (1,1,1,0.5)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _RippleTex;
            float4 _MainTex_ST;
            float _Strength;
            float4 _Tint;

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
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // leer textura base
                fixed4 col = tex2D(_MainTex, i.uv);

                // leer mapa de onda
                float r = tex2D(_RippleTex, i.uv).r;
                float disp = (r * 2.0 - 1.0) * _Strength;

                // aplicar desplazamiento UV
                col = tex2D(_MainTex, i.uv + disp);

                // multiplicar por color y alfa del tint
                col *= _Tint;

                return col;
            }
            ENDCG
        }
    }
}