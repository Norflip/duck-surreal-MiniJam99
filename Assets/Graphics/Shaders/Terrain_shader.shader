Shader "Unlit/Terrain_shader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
            [Header(Dithering)]
        _DitherPattern ("Dithering Pattern", 2D) = "white" {}
        _MinDistance ("Minimum Fade Distance", Float) = 0
        _MaxDistance ("Maximum Fade Distance", Float) = 1

        _TextureXZ ("Texture XZ", 2D) = "white" {}
        _TextureY ("Texture Y", 2D) = "white" {}

        _TriplanarBlendSharpness ("Blend Sharpness",float) = 1
         _MapScale("Scale", Float) = 1
    }
    SubShader
    {   
        Tags {"RenderType" = "Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On
        //cull Off
		LOD 100

        Pass
        {
            Tags {"LightMode" = "Vertex" }

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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 worldposXZ : TEXCOORD3;
                float3 localCoord : TEXCOORD1;
                float3 localNormal : NORMAL;

                float2 uv : TEXCOORD0;
                float4 color : COLOR0;
                float4 screenPos : TEXCOORD2;
            };

            float4 _Color;

            sampler2D _DitherPattern;
            float4 _DitherPattern_TexelSize;
        
            sampler2D _TextureXZ;
            sampler2D _TextureY;
            float _MapScale;
            float _TriplanarBlendSharpness;

            //remapping of distance
            float _MinDistance;
            float _MaxDistance;

            float g_PlayerWorldPositionZ;
            float g_PlayerFade;
            float g_PlayerRadius;

            float sdcircle (float2 samplePoint, float2 centre)
            {
                return length(samplePoint - centre) - g_PlayerRadius;
            }

            

            v2f vert (appdata v)
            {
                float4 vert = v.vertex;
                float3 worldPos = mul(unity_ObjectToWorld, float4(vert.xyz, 1.0));

                float dst = sdcircle(worldPos.xz, float2(0, g_PlayerWorldPositionZ));
                dst = smoothstep(-g_PlayerFade, g_PlayerFade, dst);

                //vert.y = lerp(vert.y, 0.0f, saturate(dst));
                
                v2f o;
                o.vertex = UnityObjectToClipPos(vert);
                o.worldposXZ = worldPos.xz;
                o.localCoord = vert;
                o.localNormal = v.normal;
                o.uv = v.uv;
                
                o.screenPos = ComputeScreenPos(o.vertex);
                o.color = float4(ShadeVertexLights(vert, v.normal), 1.0);
                return o;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                float dst = -sdcircle(i.worldposXZ, float2(0, g_PlayerWorldPositionZ));
                dst = smoothstep(-g_PlayerFade, g_PlayerFade, dst);

                float d = dot(i.localNormal, _WorldSpaceLightPos0);

                
                //clip(dst - 0.5f);

                return float4(_Color.rgb * d, dst);



                //value from the dither pattern
                float2 screenPos = i.screenPos.xy / i.screenPos.w;
                float2 ditherCoordinate = screenPos * _ScreenParams.xy * _DitherPattern_TexelSize.xy;
                float ditherValue = tex2D(_DitherPattern, ditherCoordinate).r;
                    
                 //get relative distance from the camera
                float relDistance = i.screenPos.w;
                relDistance = relDistance - _MinDistance;
                relDistance = relDistance / (_MaxDistance - _MinDistance);
                
                //discard pixels accordingly
                clip(relDistance - ditherValue);

                return i.color;
                
                /*
                // Triplanar mapping
                float2 tx = i.localCoord.zy / _MapScale;
                float2 ty = i.localCoord.xz / _MapScale;
                float2 tz = i.localCoord.xy / _MapScale;
                half3 yDiff = tex2D (_TextureY, ty);
                half3 xDiff = tex2D (_TextureXZ, tx);
                half3 zDiff = tex2D (_TextureXZ, tz);
                

                 // Get the absolute value of the world normal.
                // Put the blend weights to the power of BlendSharpness, the higher the value, 
                // the sharper the transition between the planar maps will be.
                
                half3 blendWeights = pow (abs(i.localNormal), _TriplanarBlendSharpness);
                // Divide our blend mask by the sum of it's components, this will make x+y+z=1
                blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);

                // Finally, blend together all three samples based on the blend mask.
                return float4(xDiff * blendWeights.x + yDiff * blendWeights.y + zDiff * blendWeights.z, 1.0) * i.color;
                */
            }
            ENDCG
        }
    }
}
