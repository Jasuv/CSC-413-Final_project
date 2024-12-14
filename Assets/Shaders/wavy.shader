Shader "Custom/wavy"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _Distortion ("Distortion", Float) = 0
        _Speed ("Speed", Float) = 0
        _Frequency ("Frequency", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Distortion;
            float _Speed;
            float _Frequency;
            
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
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float waveX = sin(i.uv.x * _Frequency + _Time.y * _Speed) * cos(i.uv.y * _Frequency * 0.5 + _Time.y * _Speed * 0.8) * _Distortion / 100;
                float waveY = cos(i.uv.y * _Frequency + _Time.y * _Speed) * sin(i.uv.x * _Frequency * 0.5 + _Time.y * _Speed * 0.8) * _Distortion / 100;
                float2 distortedUV = i.uv + float2(waveX, waveY);
                return tex2D(_MainTex, distortedUV);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}