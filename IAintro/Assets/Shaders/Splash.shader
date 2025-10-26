Shader "Unlit/Splash"
{
   Properties
    {
        _Center ("Center (uv)", Vector) = (0.5,0.5,0,0)
        _Radius ("Radius", Float) = 0.05
        _Intensity ("Intensity", Float) = 1.0
        _Softness ("Softness", Float) = 0.02
    }
    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _Center;
            float _Radius;
            float _Intensity;
            float _Softness;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 pos : SV_POSITION; };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float d = distance(i.uv, _Center.xy);
                float edge = smoothstep(_Radius, _Radius - _Softness, d);
                float val = (1.0 - edge) * _Intensity;
                return fixed4(val, val, val, 1.0);
            }
            ENDCG
        }
    }
}