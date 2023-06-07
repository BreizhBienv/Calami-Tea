Shader "Custom/CanonBallShadow"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _ShadowColor("Shadow Color (RGB)", Color) = (0,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _ShadowWidth("EchoWidth", Float) = 15
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard fullforwardshadows

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

            sampler2D _MainTex;

            struct Input
            {
                float2 uv_MainTex;
                float3 worldPos;
            };

            half _Glossiness;
            half _Metallic;
            fixed4 _Color;
            fixed4 _ShadowColor;
            float _ShadowWidth;

            int _PointsSize;
            fixed4 _Points[1000];
            float _ShadowRange[1000];

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color


            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            fixed emissive = 0;

            float dist = 0;
            float halfWidth = 0;
            float ringStrength = 0;

            for (int i = 1; i < _PointsSize; ++i)
            {
                dist = length(IN.worldPos.xyz - _Points[i].xyz) - _ShadowRange[i] * _Points[i].w;
            }
            if (dist == 0)
            {
                o.Albedo = c.rgb;
            }
            else
            {
                o.Albedo = ((dist > _Color) /* fixed4(1.f, 1.f, 0.f, 0.f)*/) * c.rgb;
            }
            //o.Albedo = ((dist < _Color) /* fixed4(1.f, 1.f, 0.f, 0.f)*/) * c.rgb;
            //o.Albedo = ((dist < _ShadowColor) * fixed4(0.f, 0.f, 0.f, 0.f))* tex2D(_MainTex, IN.uv_MainTex)
            o.Occlusion = 1;

            //o.Emission = c.rgb;
            //o.Emission = ((dist < _ShadowColor)) * tex2D(_MainTex, IN.uv_MainTex);
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
