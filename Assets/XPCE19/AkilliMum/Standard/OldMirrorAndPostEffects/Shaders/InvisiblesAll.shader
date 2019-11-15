// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UB/InvisiblesAll" 
{
    Properties 
    {
        _Color("Directional Shadow Color", Color) = (0,0,0,0.5)
    }
    SubShader 
    {
        Pass
        {
            Tags{
                "RenderType" = "Transparent"
                "IgnoreProjector" = "True"
                "Queue" = "Transparent"
                "LightMode" = "ForwardBase"
            }
            LOD 100
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert  
            #pragma fragment frag 

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase
            
            fixed4 _Color;
            
            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                SHADOW_COORDS(1) //put to TEXCOORD1
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex); 
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
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
                
                float4 color = _Color;
                color.a=(1-atten)*_Color.a;
                return color;//.rgb;
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
            #pragma multi_compile_fwdadd_fullshadows
            
            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                SHADOW_COORDS(1) //put to TEXCOORD1
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex); 
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
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
                
                return atten*_LightColor0;
            }
            ENDCG
        }
        //shadow casting support
        //UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    } 
    FallBack "Diffuse"
}
