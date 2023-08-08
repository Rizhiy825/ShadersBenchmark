Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        light ("Light position", Vector) = (1, 1, 1, 1)
        lightRadius ("Light Radius", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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
                float3 vertexNormal : NORMAL;
            };

            float4 light;
            float lightRadius;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.vertexNormal = v.normal;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 normalizedLight = normalize(light);
                float3 normalizedNormal = normalize(i.vertexNormal);
                float dotValue = max(dot(normalizedLight, normalizedNormal), 0.0);
                float distanceToLight = length(i.vertex - light);
                float attenuation = saturate(1.0 - distanceToLight / lightRadius);
                // sample the texture tex2D(_MainTex, i.uv);

                
                fixed4 col = fixed4(dotValue * attenuation, dotValue * attenuation, dotValue * attenuation, 1);
                return col;
            }
            ENDCG
        }
    }
}
