Shader "Custom/Lambert"
{
    Properties
    {
        light ("Light position", Vector) = (1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard 

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
        #pragma enable_d3d11_debug_symbols

        float3 light;

        struct Input
        {
            float3 lightPosition;
            float3 vertexNormal;
        };

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        // Transfer the vertex normal to the Input structure
        void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.vertexNormal = abs(v.normal);
        }
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            //fixed4 c = tex2D (_MainTex, IN.uv_MainTex);

            float3 normalizedLight = normalize(light);
            float3 normalizedNormal = normalize(IN.vertexNormal);
            float dotValue = max(dot(normalizedLight, normalizedNormal), 0.0);
            
            o.Albedo = float3(dotValue, dotValue, dotValue);
            // Metallic and smoothness come from slider variables
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
