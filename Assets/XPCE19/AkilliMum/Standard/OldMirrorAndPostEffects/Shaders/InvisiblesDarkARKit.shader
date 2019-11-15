
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UB/InvisiblesDarkARKit" 
{
    Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader 
    {
        Pass
        {
            Tags{ "LightMode" = "ForwardBase" }
            //Cull Front
        
            CGPROGRAM
            #pragma vertex vert  
            #pragma fragment frag 

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            //#pragma surface surf Mine fullforwardshadows
            #pragma multi_compile PORTRAIT_ON PORTRAIT_OFF
            #pragma multi_compile_fwdbase
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                //float4 vertex : SV_POSITION;
                float4 pos : SV_POSITION;
                float4 screenPos : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                SHADOW_COORDS(3) //put to TEXCOORD3
                //LIGHTING_COORDS(0,1);
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex); 
                //o.vertex = UnityObjectToClipPos(v.vertex);
                //o.screenPos = ComputeScreenPos(o.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.screenPos = ComputeScreenPos(o.pos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                TRANSFER_SHADOW(o); // pass shadow coordinates to pixel shader
                return o;
            }
            
            #define FLT_MAX 3.402823466e+38
            #define FLT_MIN 1.175494351e-38
            #define DBL_MAX 1.7976931348623158e+308
            #define DBL_MIN 2.2250738585072014e-308

            fixed4 frag(v2f IN) : COLOR
            {
                UNITY_LIGHT_ATTENUATION(atten, IN, IN.worldPos) //atten is builtIn :)
               
               fixed2 screenUV = (IN.screenPos.xy) / (IN.screenPos.w+FLT_MIN);

                // Get the video color given the screen's coordinates
                half4 video = tex2D (_MainTex, screenUV);

                // Uses the texture from the video and the transparency computed above
                return video*atten*_LightColor0;//.rgb;
            }
            ENDCG
        }
        Pass
        {
            Blend One One
            Tags {"LightMode" = "ForwardAdd"}
            
             CGPROGRAM
            #pragma vertex vert  
            #pragma fragment frag 

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            //#pragma surface surf Mine fullforwardshadows
            #pragma multi_compile PORTRAIT_ON PORTRAIT_OFF
            #pragma multi_compile_fwdadd_fullshadows
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                //float4 vertex : SV_POSITION;
                float4 pos : SV_POSITION;
                float4 screenPos : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                SHADOW_COORDS(3) //put to TEXCOORD3
                //LIGHTING_COORDS(0,1);
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex); 
                //o.vertex = UnityObjectToClipPos(v.vertex);
                //o.screenPos = ComputeScreenPos(o.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.screenPos = ComputeScreenPos(o.pos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                TRANSFER_SHADOW(o); // pass shadow coordinates to pixel shader
                return o;
            }
            
            #define FLT_MAX 3.402823466e+38
            #define FLT_MIN 1.175494351e-38
            #define DBL_MAX 1.7976931348623158e+308
            #define DBL_MIN 2.2250738585072014e-308

            fixed4 frag(v2f IN) : COLOR
            {
                UNITY_LIGHT_ATTENUATION(atten, IN, IN.worldPos) //atten is builtIn :)
               
                fixed2 screenUV = (IN.screenPos.xy) / (IN.screenPos.w+FLT_MIN);

                // Get the video color given the screen's coordinates
                half4 video = tex2D (_MainTex, screenUV);


                // Uses the texture from the video and the transparency computed above
                return video*atten*_LightColor0;//.rgb;
            }
            ENDCG
        }
        //shadow casting support
        //UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    } 
    FallBack "Diffuse"
}

