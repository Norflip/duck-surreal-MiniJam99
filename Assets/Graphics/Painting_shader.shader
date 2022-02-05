Shader "Custom/Painting_shader"
{
    Properties
    {
        _Background("Background", 2D) = "white" {}
        _MainTex ("Texture", 2D) = "white" {}
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 localPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _Background;
            float4 _Background_ST;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex.xyz;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 bguv = frac(i.localPos.xy);
                bguv = TRANSFORM_TEX(bguv, _Background);
                fixed4 bg_col = tex2D(_Background, bguv);
  
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return lerp(bg_col, col, col.a);
            }
            ENDCG
        }
    }
}
