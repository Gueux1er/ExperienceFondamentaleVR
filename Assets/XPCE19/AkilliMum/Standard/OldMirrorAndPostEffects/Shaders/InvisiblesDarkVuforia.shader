// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UB/InvisiblesDarkVuforia" 
{
    Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _TextureRatioX ("TextureRatioX", Float) = 0.0
        _TextureRatioY ("TextureRatioY", Float) = 0.0
        _ViewportSizeX ("ViewportSizeX", Float) = 0.0
        _ViewportSizeY ("ViewportSizeY", Float) = 0.0
        _ViewportOrigX ("ViewportOrigX", Float) = 0.0
        _ViewportOrigY ("ViewportOrigY", Float) = 0.0
        _ScreenWidth ("ScreenWidth", Float) = 0.0
        _ScreenHeight ("ScreenHeight", Float) = 0.0
        _PrefixX ("PrefixX", Float) = 0.0
        _PrefixY ("PrefixY", Float) = 0.0
        _InversionMultiplierX ("InversionMultiplierX", Float) = 0.0
        _InversionMultiplierY ("InversionMultiplierY", Float) = 0.0
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
                float4 _LightCoord: TEXCOORD4;
                //LIGHTING_COORDS(0,1);
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _TextureRatioX;
            float _TextureRatioY;
            float _ViewportSizeX;
            float _ViewportSizeY;
            float _ViewportOrigX;
            float _ViewportOrigY;
            float _ScreenWidth;
            float _ScreenHeight;
            float _PrefixX;
            float _PrefixY;
            float _InversionMultiplierX;
            float _InversionMultiplierY;

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
                
                float2 currentFragCoord;
                float2 screenCoord;

                // Convert from unity coordinates to viewport coordinates
                currentFragCoord.xy = float2(_ScreenWidth, _ScreenHeight) * (IN.screenPos.xy/IN.screenPos.w+FLT_MIN);     

                float normalized_coordinates[2];

                // The following equations calculate the appropriate UV coordinates
                // to take from the video sampler. They consider whether the screen
                // is in landscape or portrait mode and whether it uses the front (reflected)
                // or back camera. The actual coefficients are passed by BoxSetUpShader.cs.
                
                normalized_coordinates[0] = (currentFragCoord.x-_ViewportOrigX)/_ViewportSizeX;
                normalized_coordinates[1] = (currentFragCoord.y-_ViewportOrigY)/_ViewportSizeY; 

                // convert from viewport coordinates to screen_texture coordinates
                #ifdef PORTRAIT_ON
                    screenCoord.x = (_PrefixX + (_InversionMultiplierX * normalized_coordinates[1])) * _TextureRatioX;
                    screenCoord.y = (_PrefixY + (_InversionMultiplierY * normalized_coordinates[0])) * _TextureRatioY;
                #else
                    screenCoord.x = (_PrefixX + (_InversionMultiplierX * normalized_coordinates[0])) * _TextureRatioX;
                    screenCoord.y = (_PrefixY + (_InversionMultiplierY * normalized_coordinates[1])) * _TextureRatioY;
                #endif

                // Get the video color given the screen's coordinates
                half4 video = tex2D (_MainTex, screenCoord.xy);

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
                float4 _LightCoord: TEXCOORD4; //!!!!! needed for android!!!
                //LIGHTING_COORDS(0,1);
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _TextureRatioX;
            float _TextureRatioY;
            float _ViewportSizeX;
            float _ViewportSizeY;
            float _ViewportOrigX;
            float _ViewportOrigY;
            float _ScreenWidth;
            float _ScreenHeight;
            float _PrefixX;
            float _PrefixY;
            float _InversionMultiplierX;
            float _InversionMultiplierY;

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
                
                float2 currentFragCoord;
                float2 screenCoord;

                // Convert from unity coordinates to viewport coordinates
                currentFragCoord.xy = float2(_ScreenWidth, _ScreenHeight) * (IN.screenPos.xy/IN.screenPos.w+FLT_MIN);     

                float normalized_coordinates[2];

                // The following equations calculate the appropriate UV coordinates
                // to take from the video sampler. They consider whether the screen
                // is in landscape or portrait mode and whether it uses the front (reflected)
                // or back camera. The actual coefficients are passed by BoxSetUpShader.cs.
                
                normalized_coordinates[0] = (currentFragCoord.x-_ViewportOrigX)/_ViewportSizeX;
                normalized_coordinates[1] = (currentFragCoord.y-_ViewportOrigY)/_ViewportSizeY; 

                // convert from viewport coordinates to screen_texture coordinates
                #ifdef PORTRAIT_ON
                    screenCoord.x = (_PrefixX + (_InversionMultiplierX * normalized_coordinates[1])) * _TextureRatioX;
                    screenCoord.y = (_PrefixY + (_InversionMultiplierY * normalized_coordinates[0])) * _TextureRatioY;
                #else
                    screenCoord.x = (_PrefixX + (_InversionMultiplierX * normalized_coordinates[0])) * _TextureRatioX;
                    screenCoord.y = (_PrefixY + (_InversionMultiplierY * normalized_coordinates[1])) * _TextureRatioY;
                #endif

                // Get the video color given the screen's coordinates
                half4 video = tex2D (_MainTex, screenCoord.xy);

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
