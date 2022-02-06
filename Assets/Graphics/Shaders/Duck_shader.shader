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
        Tags { "RenderType"="Opaque" }
        Cull off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.localPos = v.vertex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));
                col *= _Color;
                
                fixed4 paint = tex2D(_PaintTex, TRANSFORM_TEX(i.uv, _PaintTex));
                float v = snoise(normalize(i.localPos) * _NoiseScale);
                v = smoothstep(0.1,0.7,v);
                
                return float4(lerp(col.rgb, _SelectedColor.rgb, max(v, paint.r) * _UsePaintTexture), 1.0);
            }
            ENDCG
        }
    }
}
