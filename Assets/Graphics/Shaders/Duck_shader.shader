Shader "Unlit/Duck_shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PaintTex ("Paint Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        [MaterialToggle] _UsePaintTexture ("Use Paint Texture", Float) = 0.0
        _SelectedColor("Selected Color", Color) = (1,1,1,1)
        _NoiseScale ("Noise Scale", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType" = "Geometry" }
        Cull Off
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

            sampler2D _PaintTex;
            float4 _PaintTex_ST;
            
            float4 _Color;
            float _UsePaintTexture;
            float4 _SelectedColor;

            float _NoiseScale;

            
			float g_PlayerWorldPositionZ;
			float g_PlayerFade;
			float g_PlayerRadius;

			float sdcircle (float2 samplePoint, float2 centre)
			{
				return length(samplePoint - centre) - g_PlayerRadius;
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
                float dst = -sdcircle(i.worldPos.xz, float2(0, g_PlayerWorldPositionZ));
                dst = smoothstep(-g_PlayerFade, g_PlayerFade, dst);
				clip(dst - 0.5f);
                
                fixed4 col = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));
                col *= _Color;
                
                fixed4 paint = tex2D(_PaintTex, TRANSFORM_TEX(i.uv, _PaintTex));
                float v = snoise(normalize(i.localPos) * _NoiseScale);
                v = smoothstep(0.1,0.7,v);
                
                return float4(lerp(col.rgb, _SelectedColor.rgb, (max(v, paint.r) * _UsePaintTexture)) * paint.a, 1.0);
            }
            ENDCG
        }
    }

    Fallback "VertexLit"
}
