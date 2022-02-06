Shader "Unlit/BigDuck"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Radius("Radius", Float) = 10
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On
        Lod 200
    
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            #include "noiseSimplex.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD2;
                float3 localPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Radius;
            float4 _Color;

			float g_PlayerWorldPositionZ;
			float g_PlayerFade;
			float g_PlayerRadius;

			float sdcircle (float2 samplePoint, float2 centre, float radius)
			{
				return length(samplePoint - centre) - radius;
			}

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.localPos = v.vertex;
                o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dst = -sdcircle(i.worldPos.xz, float2(0, g_PlayerWorldPositionZ), g_PlayerRadius + _Radius);
                dst = smoothstep(-g_PlayerFade, g_PlayerFade, dst);
                
                fixed4 col = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));
                col *= _Color;
                
                return float4(col.rgb, dst);
            }
            ENDCG
        }
    }

    Fallback "VertexLit"
}
