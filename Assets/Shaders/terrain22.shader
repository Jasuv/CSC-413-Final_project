Shader "Custom/terrain22"
{
    Properties
    {
        _MainTex1 ("Texture 1", 2D) = "white" {}
        _MainTex2 ("Texture 2", 2D) = "white" {}
        _MainTex3 ("Texture 3", 2D) = "white" {}
        _HeightThreshold1 ("Height Threshold 1", Float) = 0.33
        _HeightThreshold2 ("Height Threshold 2", Float) = 0.66
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex1;
        sampler2D _MainTex2;
        sampler2D _MainTex3;
        float _HeightThreshold1;
        float _HeightThreshold2;

        struct appdata
        {
            float4 vertex : POSITION;
        };

        struct v2f
        {
            float4 pos : SV_POSITION;
            float3 worldPos : TEXCOORD0; // Pass world position to fragment shader
        };

        v2f vert(appdata v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // Transform vertex to world space
            o.worldPos = worldPos; // Pass full world position
            return o;
        }

        struct Input
        {
            float2 uv_MainTex; // UV coordinates
            float worldPosY;   // World-space Y position
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            // Normalize height to range [0, 1]
            float height = saturate(IN.worldPosY*0.33f); // Scale 10 based on your terrain height
/*
            // Blend weights
            float blend1 = saturate(1.0 - height / _HeightThreshold1);
            float blend2 = saturate((height - _HeightThreshold1) / (_HeightThreshold2 - _HeightThreshold1));
            float blend3 = saturate((height - _HeightThreshold2) / (1.0 - _HeightThreshold2));

            // Sample textures
            float3 tex1 = tex2D(_MainTex1, IN.uv_MainTex).rgb;
            float3 tex2 = tex2D(_MainTex2, IN.uv_MainTex).rgb;
            float3 tex3 = tex2D(_MainTex3, IN.uv_MainTex).rgb;
            */

            // Combine textures
            //o.Albedo = tex1 * blend1 + tex2 * blend2 + tex3 * blend3;
            o.Normal = float3(0, 0, 1);
            o.Specular = 0;

            if (height < _HeightThreshold1)
            {
                o.Albedo = float3(1, 0, 0); // Red
            }
            else if (height < _HeightThreshold2)
            {
                o.Albedo = float3(0, 1, 0); // Green
            }
            else
            {
                o.Albedo = float3(0, 0, 1); // Blue
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}