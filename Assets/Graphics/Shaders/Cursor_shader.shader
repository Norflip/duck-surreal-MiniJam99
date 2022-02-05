Shader "Unlit/Cursor_shader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Radius ("Radius", Float) = 0.4
        _Thickness ("Thickness", Float) = 0.0
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
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 localPos : TEXCOORD0;
            };

            float4 _Color;
            float _Radius;
            float _Thickness;
  
            float sdCircle (float2 p, float radius, float thc)
            {
                return abs(length(p) - radius) - thc;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 pos = i.localPos.xy;
                
                float dst = sdCircle(pos, _Radius, _Thickness);

                float distanceChange = fwidth(dst) * 0.5f;
                float aa = smoothstep(distanceChange, -distanceChange, dst);

                clip(-dst);

                return float4(dst, dst, dst, 1.0);
            }
            ENDCG
        }
    }
}
