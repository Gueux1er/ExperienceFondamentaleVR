// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UB/BlursLOD"
{
	Properties
	{
		_CustomFloatParam1("Iteration", float) = 1
		_CustomFloatParam2("Neighbour", float) = 1
		_MainTex("Base (RGB)", 2D) = "" {}
        _Lod("Lod",float) = 0
        _AR("AR Mode",float) = 0
	}

	SubShader
	{
		Pass
		{
			ZTest Always 
			Cull Off
			Fog{ Mode off }

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
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
            float _Lod;
			float _CustomFloatParam1;
            float _CustomFloatParam2;
			float _AR;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

            #define FLT_MAX 3.402823466e+38
            #define FLT_MIN 1.175494351e-38
            #define DBL_MAX 1.7976931348623158e+308
            #define DBL_MIN 2.2250738585072014e-308

			fixed4 frag(v2f i) : COLOR
			{
				float stepX = (1.0 / (_ScreenParams.xy / _ScreenParams.w + FLT_MIN).x) * _CustomFloatParam2;
                //float stepX = (1.0 / (_ScreenParams.xy / _ScreenParams.w+FLT_MIN).x)*(_NeighbourPixels + (_Iteration % 2));
                float stepY = (1.0 / (_ScreenParams.xy / _ScreenParams.w + FLT_MIN).y) * _CustomFloatParam2;
                //float stepY = (1.0 / (_ScreenParams.xy / _ScreenParams.w+FLT_MIN).y)*(_NeighbourPixels + (_Iteration % 2));

                if (_AR > 0)
                {
                    if (_CustomFloatParam1 == 1) //clear the texture
                    {
                        half3 check = float3(0, 0, 0);

                        float onePixelX = (1.0 / (_ScreenParams.xy / _ScreenParams.w + FLT_MIN).x);
                        float onePixelY = (1.0 / (_ScreenParams.xy / _ScreenParams.w + FLT_MIN).y);

                        half4 up = tex2Dlod(_MainTex, float4(i.uv + float2(0, onePixelY), 0, _Lod));
                        half4 down = tex2Dlod(_MainTex, float4(i.uv + float2(0, -onePixelY), 0, _Lod));
                        half4 left = tex2Dlod(_MainTex, float4(i.uv + float2(-onePixelX, 0), 0, _Lod));
                        half4 right = tex2Dlod(_MainTex, float4(i.uv + float2(onePixelX, 0), 0, _Lod));
                        
                        if (all(check == up.rgb)
                            &&
                            all(check == right.rgb))
                            return tex2Dlod(_MainTex, float4(i.uv + float2(-onePixelX, -onePixelY), 0, _Lod));
                        if(all(check == right.rgb)
                            &&
                            all(check == down.rgb))
                            return tex2Dlod(_MainTex, float4(i.uv + float2(-onePixelX, onePixelY), 0, _Lod));
                        if (all(check == down.rgb)
                            &&
                            all(check == left.rgb))
                            return tex2Dlod(_MainTex, float4(i.uv + float2(onePixelX, onePixelY), 0, _Lod));
                        if (all(check == left.rgb)
                            &&
                            all(check == up.rgb))
                            return tex2Dlod(_MainTex, float4(i.uv + float2(onePixelX, -onePixelY), 0, _Lod));

                        if (all(check == up.rgb))
                            return tex2Dlod(_MainTex, float4(i.uv + float2(0, -onePixelY), 0, _Lod));
                        if (all(check == right.rgb))
                            return tex2Dlod(_MainTex, float4(i.uv + float2(-onePixelX, 0), 0, _Lod));
                        if (all(check == down.rgb))
                            return tex2Dlod(_MainTex, float4(i.uv + float2(0, onePixelY), 0, _Lod));
                        if (all(check == left.rgb))
                            return tex2Dlod(_MainTex, float4(i.uv + float2(onePixelX, 0), 0, _Lod));

                        return tex2Dlod(_MainTex, float4(i.uv, 0, _Lod));
                    }
                    else //jump black pixel blur
                    {
                        half3 check = float3(0, 0, 0);

                        half4 up = tex2Dlod(_MainTex, float4(i.uv + float2(0, stepY), 0, _Lod));
                        half4 down = tex2Dlod(_MainTex, float4(i.uv + float2(0, -stepY), 0, _Lod));
                        half4 left = tex2Dlod(_MainTex, float4(i.uv + float2(-stepX, 0), 0, _Lod));
                        half4 right = tex2Dlod(_MainTex, float4(i.uv + float2(stepX, 0), 0, _Lod));

                        if (all(check == up.rgb)
                            ||
                            all(check == right.rgb)
                            ||
                            all(check == down.rgb)
                            ||
                            all(check == left.rgb))
                            return tex2Dlod(_MainTex, float4(i.uv, 0, _Lod));
                    }
                }

				half4 color = float4 (0,0,0,0);

				fixed2 copyUV;

				copyUV.x = i.uv.x - stepX;
				copyUV.y = i.uv.y - stepY;
				color += tex2Dlod (_MainTex, float4(copyUV,0,_Lod))*0.077847;
				copyUV.x = i.uv.x;
				copyUV.y = i.uv.y - stepY;
				color += tex2Dlod (_MainTex, float4(copyUV,0,_Lod))*0.123317;
				copyUV.x = i.uv.x + stepX;
				copyUV.y = i.uv.y - stepY;
				color += tex2Dlod (_MainTex, float4(copyUV,0,_Lod))*0.077847;

				copyUV.x = i.uv.x - stepX;
				copyUV.y = i.uv.y;
				color += tex2Dlod (_MainTex, float4(copyUV,0,_Lod))*0.123317;
				copyUV.x = i.uv.x;
				copyUV.y = i.uv.y;
				color += tex2Dlod (_MainTex, float4(copyUV,0,_Lod))*0.195346;
				copyUV.x = i.uv.x + stepX;
				copyUV.y = i.uv.y;
				color += tex2Dlod (_MainTex, float4(copyUV,0,_Lod))*0.123317;

				copyUV.x = i.uv.x - stepX;
				copyUV.y = i.uv.y + stepY;
				color += tex2Dlod (_MainTex, float4(copyUV,0,_Lod))*0.077847;
				copyUV.x = i.uv.x;
				copyUV.y = i.uv.y + stepY;
				color += tex2Dlod (_MainTex, float4(copyUV,0,_Lod))*0.123317;
				copyUV.x = i.uv.x + stepX;
				copyUV.y = i.uv.y + stepY;
				color += tex2Dlod (_MainTex, float4(copyUV,0,_Lod))*0.077847;
    
				return color;
			}

			ENDCG
		}
	}
}

