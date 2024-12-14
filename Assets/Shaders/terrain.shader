Shader "Custom/terrain"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf Lambert
        #include "UnityCG.cginc"

        struct appdata
        {
            float4 vertex : POSITION;
            float4 color : COLOR;
        };

        struct v2f
        {
            float4 pos : POSITION;
            float4 color : COLOR;
        };

        v2f vert(appdata v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.color = v.color;
            return o;
        }

        struct Input
        {
            float4 color : COLOR;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            o.Albedo = IN.color.rgb;
            o.Normal = float3(0, 0, 1);
            o.Specular = 0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}