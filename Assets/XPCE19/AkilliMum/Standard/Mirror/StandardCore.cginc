// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

#ifndef UNITY_STANDARD_CORE_INCLUDED
#define UNITY_STANDARD_CORE_INCLUDED

#include "UnityCG.cginc"
#include "UnityShaderVariables.cginc"
#if !UNITY_2018_1_OR_NEWER
    #include "UnityInstancing.cginc"
#endif
#include "UnityStandardConfig.cginc"
#include "../Mirror/StandardInput.cginc"
#include "UnityPBSLighting.cginc"
#include "UnityStandardUtils.cginc"
#include "UnityGBuffer.cginc"
#include "UnityStandardBRDF.cginc"

#include "AutoLight.cginc"

//-------------------------------------------------------------------------------------
// counterpart for NormalizePerPixelNormal
// skips normalization per-vertex and expects normalization to happen per-pixel
half3 NormalizePerVertexNormal (float3 n) // takes float to avoid overflow
{
    #if (SHADER_TARGET < 30) || UNITY_STANDARD_SIMPLE
        return normalize(n);
    #else
        return n; // will normalize per-pixel instead
    #endif
}

half3 NormalizePerPixelNormal (half3 n)
{
    #if (SHADER_TARGET < 30) || UNITY_STANDARD_SIMPLE
        return n;
    #else
        return normalize((float3)n); // takes float to avoid overflow //old one-> return normalize(n);
    #endif
}

//-------------------------------------------------------------------------------------
UnityLight MainLight ()
{
    UnityLight l;

    l.color = _LightColor0.rgb;
    l.dir = _WorldSpaceLightPos0.xyz;
    return l;
}

UnityLight AdditiveLight (half3 lightDir, half atten)
{
    UnityLight l;

    l.color = _LightColor0.rgb;
    l.dir = lightDir;
    #ifndef USING_DIRECTIONAL_LIGHT
        l.dir = NormalizePerPixelNormal(l.dir);
    #endif

    // shadow the light
    l.color *= atten;
    return l;
}

UnityLight DummyLight ()
{
    UnityLight l;
    l.color = 0;
    l.dir = half3 (0,1,0);
    return l;
}

UnityIndirect ZeroIndirect ()
{
    UnityIndirect ind;
    ind.diffuse = 0;
    ind.specular = 0;
    return ind;
}

//-------------------------------------------------------------------------------------
// Common fragment setup

// deprecated
half3 WorldNormal(half4 tan2world[3])
{
    return normalize(tan2world[2].xyz);
}

// deprecated
#ifdef _TANGENT_TO_WORLD
    half3x3 ExtractTangentToWorldPerPixel(half4 tan2world[3])
    {
        half3 t = tan2world[0].xyz;
        half3 b = tan2world[1].xyz;
        half3 n = tan2world[2].xyz;

    #if UNITY_TANGENT_ORTHONORMALIZE
        n = NormalizePerPixelNormal(n);

        // ortho-normalize Tangent
        t = normalize (t - n * dot(t, n));

        // recalculate Binormal
        half3 newB = cross(n, t);
        b = newB * sign (dot (newB, b));
    #endif

        return half3x3(t, b, n);
    }
#else
    half3x3 ExtractTangentToWorldPerPixel(half4 tan2world[3])
    {
        return half3x3(0,0,0,0,0,0,0,0,0);
    }
#endif

half3 PerPixelWorldNormal(float4 i_tex, half4 tangentToWorld[3])
{
#ifdef _NORMALMAP
    half3 tangent = tangentToWorld[0].xyz;
    half3 binormal = tangentToWorld[1].xyz;
    half3 normal = tangentToWorld[2].xyz;

    #if UNITY_TANGENT_ORTHONORMALIZE
        normal = NormalizePerPixelNormal(normal);

        // ortho-normalize Tangent
        tangent = normalize (tangent - normal * dot(tangent, normal));

        // recalculate Binormal
        half3 newB = cross(normal, tangent);
        binormal = newB * sign (dot (newB, binormal));
    #endif

    half3 normalTangent = NormalInTangentSpace(i_tex);
    half3 normalWorld = NormalizePerPixelNormal(tangent * normalTangent.x + binormal * normalTangent.y + normal * normalTangent.z); // @TODO: see if we can squeeze this normalize on SM2.0 as well
#else
    half3 normalWorld = normalize(tangentToWorld[2].xyz);
#endif
    return normalWorld;
}

#ifdef _PARALLAXMAP
    #define IN_VIEWDIR4PARALLAX(i) NormalizePerPixelNormal(half3(i.tangentToWorldAndPackedData[0].w,i.tangentToWorldAndPackedData[1].w,i.tangentToWorldAndPackedData[2].w))
    #define IN_VIEWDIR4PARALLAX_FWDADD(i) NormalizePerPixelNormal(i.viewDirForParallax.xyz)
#else
    #define IN_VIEWDIR4PARALLAX(i) half3(0,0,0)
    #define IN_VIEWDIR4PARALLAX_FWDADD(i) half3(0,0,0)
#endif

#if UNITY_REQUIRE_FRAG_WORLDPOS
    #if UNITY_PACK_WORLDPOS_WITH_TANGENT
        #define IN_WORLDPOS(i) half3(i.tangentToWorldAndPackedData[0].w,i.tangentToWorldAndPackedData[1].w,i.tangentToWorldAndPackedData[2].w)
    #else
        #define IN_WORLDPOS(i) i.posWorld
    #endif
    #define IN_WORLDPOS_FWDADD(i) i.posWorld
#else
    #define IN_WORLDPOS(i) half3(0,0,0)
    #define IN_WORLDPOS_FWDADD(i) half3(0,0,0)
#endif

#define IN_LIGHTDIR_FWDADD(i) half3(i.tangentToWorldAndLightDir[0].w, i.tangentToWorldAndLightDir[1].w, i.tangentToWorldAndLightDir[2].w)

#define FRAGMENT_SETUP(x) FragmentCommonData x = \
    FragmentSetup(i.tex, i.eyeVec, IN_VIEWDIR4PARALLAX(i), i.tangentToWorldAndPackedData, IN_WORLDPOS(i));

#define FRAGMENT_SETUP_FWDADD(x) FragmentCommonData x = \
    FragmentSetup(i.tex, i.eyeVec, IN_VIEWDIR4PARALLAX_FWDADD(i), i.tangentToWorldAndLightDir, IN_WORLDPOS_FWDADD(i));

struct FragmentCommonData
{
    half3 diffColor, specColor;
    // Note: smoothness & oneMinusReflectivity for optimization purposes, mostly for DX9 SM2.0 level.
    // Most of the math is being done on these (1-x) values, and that saves a few precious ALU slots.
    half oneMinusReflectivity, smoothness;
    half3 normalWorld, eyeVec, posWorld;
    half alpha;

#if UNITY_STANDARD_SIMPLE
    half3 reflUVW;
#endif

#if UNITY_STANDARD_SIMPLE
    half3 tangentSpaceNormal;
#endif
};

#ifndef UNITY_SETUP_BRDF_INPUT
    #define UNITY_SETUP_BRDF_INPUT SpecularSetup
#endif


float mod(float a, float b)
{
    return a - floor(a / b) * b;
}
float2 mod(float2 a, float2 b)
{
    return a - floor(a / b) * b;
}
float3 mod(float3 a, float3 b)
{
    return a - floor(a / b) * b;
}
float4 mod(float4 a, float4 b)
{
    return a - floor(a / b) * b;
} 

#define MAX_RADIUS 2
#define HASHSCALE1 .1031
#define HASHSCALE3 float3(.1031, .1030, .0973)

float hash12(float2 p)
{
    float3 p3  = frac(float3(p.xyx) * HASHSCALE1);
    p3 += dot(p3, p3.yzx + 19.19);
    return frac((p3.x + p3.y) * p3.z);
}

float2 hash22(float2 p)
{
    float3 p3 = frac(float3(p.xyx) * HASHSCALE3);
    p3 += dot(p3, p3.yzx+19.19);
    return frac((p3.xx+p3.yz)*p3.zy);
}

float4 GetWithLOD(sampler2D tex, float2 uv){
    if (_LODLevel>0) {
        return tex2Dlod (tex, float4(uv,0,_LODLevel));
    } else {
        return tex2D(tex, uv);
    }
}

float4 BlurWithLOD(sampler2D tex, float2 uv, float stepX, float stepY){
    half4 color = float4 (0, 0, 0, 1);
    fixed2 copyUV;
    
    copyUV.x = uv.x - stepX;
    copyUV.y = uv.y - stepY;
    color += GetWithLOD(tex, copyUV) * 0.077847;
    copyUV.x = uv.x;
    copyUV.y = uv.y - stepY;
    color += GetWithLOD(tex, copyUV) * 0.123317;
    copyUV.x = uv.x + stepX;
    copyUV.y = uv.y - stepY;
    color += GetWithLOD(tex, copyUV) * 0.077847;

    copyUV.x = uv.x - stepX;
    copyUV.y = uv.y;
    color += GetWithLOD(tex, copyUV) * 0.123317;
    copyUV.x = uv.x;
    copyUV.y = uv.y;
    color += GetWithLOD(tex, copyUV) * 0.195346;
    copyUV.x = uv.x + stepX;
    copyUV.y = uv.y;
    color += GetWithLOD(tex, copyUV) * 0.123317;

    copyUV.x = uv.x - stepX;
    copyUV.y = uv.y + stepY;
    color += GetWithLOD(tex, copyUV) * 0.077847;
    copyUV.x = uv.x;
    copyUV.y = uv.y + stepY;
    color += GetWithLOD(tex, copyUV) * 0.123317;
    copyUV.x = uv.x + stepX;
    copyUV.y = uv.y + stepY;
    color += GetWithLOD(tex, copyUV) * 0.077847;
    
    return color;
}

half TestNeighboursForAR(float2 screenUV, float eyeIndex, half alpha) {
    float onePixelX = (1.0 / (_ScreenParams.xy / _ScreenParams.w + FLT_MIN).x);
    float onePixelY = (1.0 / (_ScreenParams.xy / _ScreenParams.w + FLT_MIN).y);
    
    float3 up, down, left, right = float3(0, 0, 0);

    for (int i = 1; i <= 3; i++)
    {
        float stepX = onePixelX * i;
        float stepY = onePixelY * i;

        if (eyeIndex > 0)
        {
            up = GetWithLOD(_ReflectionTexOther, screenUV + float2(0, stepY)).rgb;
        }
        else
        {
            up = GetWithLOD(_ReflectionTex, screenUV + float2(0, stepY)).rgb;
        }
        if (eyeIndex > 0)
        {
            down = GetWithLOD(_ReflectionTexOther, screenUV + float2(0, -stepY)).rgb;
        }
        else
        {
            down = GetWithLOD(_ReflectionTex, screenUV + float2(0, -stepY)).rgb;
        }
        if (eyeIndex > 0)
        {
            left = GetWithLOD(_ReflectionTexOther, screenUV + float2(-stepX, 0)).rgb;
        }
        else
        {
            left = GetWithLOD(_ReflectionTex, screenUV + float2(-stepX, 0)).rgb;
        }
        if (eyeIndex > 0)
        {
            right = GetWithLOD(_ReflectionTexOther, screenUV + float2(stepX, 0)).rgb;
        }
        else
        {
            right = GetWithLOD(_ReflectionTex, screenUV + float2(stepX, 0)).rgb;
        }
        /*half4 up = tex2Dlod(_MainTex, float4(i.uv + float2(0, onePixelY), 0, _Lod));
        half4 down = tex2Dlod(_MainTex, float4(i.uv + float2(0, -onePixelY), 0, _Lod));
        half4 left = tex2Dlod(_MainTex, float4(i.uv + float2(-onePixelX, 0), 0, _Lod));
        half4 right = tex2Dlod(_MainTex, float4(i.uv + float2(onePixelX, 0), 0, _Lod));*/
        half3 check = float3(0, 0, 0);

        if (all(check == up.rgb)
            ||
            all(check == right.rgb)
            ||
            all(check == down.rgb)
            ||
            all(check == left.rgb))
            return 0.25 * i; //so 3 pixels comming to the edges of the full transparency will fade
    }

    return alpha;
}

//void DoWetProcess(inout float3 diffuse, inout float gloss)
//{
//    // Water influence on material BRDF
//    diffuse *= lerp(1.0, 0.3, _WetLevel);
//    // Not the same boost factor than the paper
//    gloss = min(gloss * lerp(1.0, 2.5, _WetLevel), 1.0);
//}

inline void CalculateReflection (float4 screenPos, float texdistance, float eyeIndex, float4 eyePos,inout half smoothness,inout half3 specColor,
    float2 texuv, inout float3 normal, inout half3 diffColor,
    bool isForward, inout half3 emissiveColor, inout half texAlpha)
{
    //!!important eyeIndex is calculated inside related shader code, so do not check single pass etc here!! only check eyeIndex

    ////wetness
    //DoWetProcess(diffColor, smoothness);

    //half maskAlpha = 1;
    half mask = 1;
    if(_EnableMask > 0){
        mask = tex2D(_MaskTex, texuv/half2(_MaskTiling.r,_MaskTiling.g)).a;
        //maskAlpha = SampleAlbedoAlpha(texuv/half2(_MaskTiling.r,_MaskTiling.g), TEXTURE2D_PARAM(_MaskTex, sampler_MaskTex));
        mask = smoothstep(mask, 0, _MaskCutoff);
    }
    
    ////recalculate according to mask
    //smoothness = lerp(smoothness, 1.0, mask);
    //// Water F0 specular is 0.02 (based on IOR of 1.33)
    //specColor = lerp(specColor, 0.02, mask);

    float2 screenUV = (screenPos.xy) / (screenPos.w+FLT_MIN);
    
    #if UNITY_SINGLE_PASS_STEREO
        // If Single-Pass Stereo mode is active, transform the
        // coordinates to get the correct output UV for the current eye.
        float4 scaleOffset = unity_StereoScaleOffset[unity_StereoEyeIndex];
        screenUV = (screenUV - scaleOffset.zw) / scaleOffset.xy;
    #endif
    
    half3 nor = float3(1,1,1);
    if(_ReflectionRefraction > 0){
        nor = UnpackNormal(tex2D(_RefractionTex, texuv));
        //nor = SampleNormal(texuv, TEXTURE2D_PARAM(_RefractionTex, sampler_RefractionTex), _BumpScale);
        screenUV.xy += (nor * (_ReflectionRefraction / texdistance));
    }
    
    float4 reflection = float4(1,1,1,1);
    //float4 reflection1 = float4(1,1,1,1);
    //float4 reflection2 = float4(1,1,1,1);
    
    half3 col_orig1 = half3(1,1,1);
    half3 col_orig2 = half3(1,1,1);
    half2 input1 = half2(1,1);
    //half2 input2 = half2(1,1);
    half3 rippleUV = half3(1,1,1);
    if(_EnableWave > 0)
    {
        col_orig1 = UnpackNormal(tex2D(_WaveNoiseTex, texuv/_WaveSize + _WaveSpeed*_Time.y));
        col_orig2 = UnpackNormal(tex2D(_WaveNoiseTex, texuv/_WaveSize - _WaveSpeed*_Time.y));

        _WaveDistortion /= texdistance;

        half2 screenUV1 = screenUV + (col_orig1 * _WaveDistortion) * mask;
        half2 screenUV2 = screenUV - (col_orig2 * _WaveDistortion) * mask;
        //half2 screenUV1 = screenUV + (col_orig1.r * _WaveDistortion - _WaveDistortion / 2) * mask;
        //half2 screenUV2 = screenUV - (col_orig1.r * _WaveDistortion - _WaveDistortion / 2) * mask;
        
        //input1 = texuv + (col_orig1.r * _WaveDistortion - _WaveDistortion / 2) * _WaveSize * mask;
        //input2 = texuv - (col_orig1.g * _WaveDistortion - _WaveDistortion / 2) * _WaveSize * mask;
        
        //texuv = (input1 + input2)/2;
        
        if(eyeIndex > 0)
            reflection = GetWithLOD(_ReflectionTexOther, screenUV1) / 2 +
                         GetWithLOD(_ReflectionTexOther, screenUV2) / 2;
        else
            reflection = GetWithLOD(_ReflectionTex, screenUV1) / 2 +
                         GetWithLOD(_ReflectionTex, screenUV2) / 2;
    }
    else if (_EnableRipple > 0){
       
        //float2 temp_cast_0 = (_RainDrops_Tile).xx;                                                //RAIN
        float2 temp_cast_0 = (_RippleSize).xx;                                              //RAIN
        float2 uv_TexCoord53 = texuv * temp_cast_0;                                     //RAIN
        float2 appendResult57 = (float2(frac(uv_TexCoord53.x), frac(uv_TexCoord53.y))); //RAIN
        // *** BEGIN Flipbook UV Animation vars ***
        // Total tiles of Flipbook Texture
        float fbtotaltiles58 = 8.0 * 8.0;                                                       //RAIN
        // Offsets for cols and rows of Flipbook Texture
        float fbcolsoffset58 = 1.0f / 8.0;                                                      //RAIN
        float fbrowsoffset58 = 1.0f / 8.0;                                                      //RAIN
        // Speed of animation
        //float fbspeed58 = _Time[1] * _RainSpeed;                                              //RAIN
        float fbspeed58 = _Time[1] * _RippleSpeed;                                              //RAIN
        // UV Tiling (col and row offset)
        float2 fbtiling58 = float2(fbcolsoffset58, fbrowsoffset58);                             //RAIN
        // UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)
        // Calculate current tile linear index
        float fbcurrenttileindex58 = round(fmod(fbspeed58 + 0.0, fbtotaltiles58));          //RAIN
        fbcurrenttileindex58 += (fbcurrenttileindex58 < 0) ? fbtotaltiles58 : 0;                //RAIN
        // Obtain Offset X coordinate from current tile linear index
        float fblinearindextox58 = round(fmod(fbcurrenttileindex58, 8.0));              //RAIN
        // Multiply Offset X by coloffset
        float fboffsetx58 = fblinearindextox58 * fbcolsoffset58;                                //RAIN
        // Obtain Offset Y coordinate from current tile linear index
        float fblinearindextoy58 = round(fmod((fbcurrenttileindex58 - fblinearindextox58) / 8.0, 8.0));//RAIN
        // Reverse Y to get tiles from Top to Bottom
        fblinearindextoy58 = (int)(8.0 - 1) - fblinearindextoy58;                                   //RAIN
        // Multiply Offset Y by rowoffset
        float fboffsety58 = fblinearindextoy58 * fbrowsoffset58;                                //RAIN
        // UV Offset
        float2 fboffset58 = float2(fboffsetx58, fboffsety58);                                   //RAIN
        // Flipbook UV
        half2 fbuv58 = appendResult57 * fbtiling58 + fboffset58;                                //RAIN
        // *** END Flipbook UV Animation vars ***
        //float4 temp_output_63_0 = (tex2D(_Mask, customUVs39, float2(0, 0), float2(0, 0)) * i.vertexColor);
        if(_EnableMask > 0)
        {
            float3 lerpResult61 = lerp(                                                             //RAIN
            //UnpackScaleNormal(tex2D(_Normal, customUVs39, temp_output_40_0, temp_output_41_0), _NormalScale),
            //o.Normal,
            normal,
            //UnpackScaleNormal(tex2D(_RippleTex, fbuv58), _RainDrops_Power),
            UnpackScaleNormal(tex2D(_RippleTex, fbuv58), _RippleDensity),
            //temp_output_63_0.r);
            //wetAlpha);
            mask);
            //o.Normal = lerpResult61;                                                              //RAIN
            normal = lerpResult61;                                                              //RAIN
            //o.Normal = lerp(o.Normal, float3(0, 0, 1), wetAlpha);

            //normal = UnpackScaleNormal(tex2D(_RippleTex, fbuv58), _RippleDensity).mask;
        }
        else
        {
            normal = UnpackScaleNormal(tex2D(_RippleTex, fbuv58), _RippleDensity);
        }

        screenUV.xy -= (normal * (_RippleRefraction / texdistance)); //so far away pixels will not be refracted very much
        
        if(eyeIndex > 0)
            reflection = GetWithLOD(_ReflectionTexOther, screenUV);
        else
            reflection = GetWithLOD(_ReflectionTex, screenUV);
    }
    else
    {
        if(eyeIndex > 0)
            reflection = GetWithLOD(_ReflectionTexOther, screenUV);
        else
            reflection = GetWithLOD(_ReflectionTex, screenUV);
    }
    
    
    //update normals
    //if(_ReflectionRefraction > 0){
    //    half3 bump = UnpackNormal(tex2D(_RefractionTex,uv/_ReflectionRefraction)).rgb;
    //    normal = 
    //        _ReflectionIntensity > 0 ?
    //            (
    //                mask > 0 ?
    //                    normal * bump * mask //nor *_ReflectionRefraction 
    //                    :
    //                    normal
    //            )
    //            :
    //            normal;
    //}
    
    if(_EnableWave > 0)
    {
        normal = 
            _ReflectionIntensity > 0 ?
                (
                        normal
                        //+ col_orig1.rgg * col_orig2.gbb * mask
                        //+ col_orig1.gbb * col_orig2.rgg * mask
                        + col_orig1.rgb * mask
                        - col_orig2.rgb * mask
                    //mask > 0 ?
                        //half3(0,1,0) 
                        ////+ col_orig1.rgg * col_orig2.gbb * mask
                        ////+ col_orig1.gbb * col_orig2.rgg * mask
                        //+ col_orig1.rgb * mask
                        //:
                        ////normal
                        //normal
                        ////+ col_orig1.rgg * col_orig2.gbb * mask
                        ////+ col_orig1.gbb * col_orig2.rgg * mask
                        //+ col_orig1.rgg * mask
                )
                :
                normal;           
        //recalculate albedo with similar waves:)
        //if(_ReflectionIntensity > 0){
            //surfaceData.albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input1) / 2 +
                                 //SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input2) / 2;
        //}
    }
    //else if (_EnableRipple > 0){
    //    normal = 
    //        _ReflectionIntensity > 0 ? 
    //            (
    //                mask > 0 ?
    //                    half3(0,1,0) 
    //                    + rippleUV.rbb * rippleUV.brr *mask //rippleUV * mask //col_orig1.rgg * col_orig1.gbb * mask//normal * half3(input1.x,input1.y,input1.x+input1.y) * mask //normal// * rippleUV * mask
    //                    :
    //                    normal
    //            )
    //            :
    //            normal;
    //                             
    //    //recalculate albedo with similar ripples:)
    //    //if(_ReflectionIntensity > 0){
    //    //    surfaceData.albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input1);
    //    //}
    //}
    //else{
        //normal =
            //_ReflectionIntensity > 0 ? 
                //(
                //    mask > 0 ?
                //        normal * mask //rippleUV * mask //col_orig1.rgg * col_orig1.gbb * mask//normal * half3(input1.x,input1.y,input1.x+input1.y) * mask //normal// * rippleUV * mask
                //        :
                //        normal
                //)
                //:
                //normal;
    //}
    
    //lerp reflection
    //reflection = lerp(reflection1, reflection2, eyeIndex);
    
    
    if(_EnableDepth>0)
    {

        float sceneDepthAtFrag = 0;
        
        if(eyeIndex > 0) 
            sceneDepthAtFrag = tex2Dproj(_ReflectionTexOtherDepth, UNITY_PROJ_COORD(screenPos)).r;
        else
            sceneDepthAtFrag = tex2Dproj(_ReflectionTexDepth, UNITY_PROJ_COORD(screenPos)).r;
        
        //sceneDepthAtFrag = LinearEyeDepth(sceneDepthAtFrag);
#if UNITY_REVERSED_Z
        sceneDepthAtFrag = 1-LinearEyeDepth(sceneDepthAtFrag);
#else
        sceneDepthAtFrag = LinearEyeDepth(sceneDepthAtFrag);
#endif

        float x, y, z, w;
        //float _NearClip = 0.3; //pass camera clipping planes to shader
        //float FarClip = 1000;
#if UNITY_REVERSED_Z //SHADER_API_GLES3 // insted of UNITY_REVERSED_Z
        x = -1.0 + _NearClip / _FarClip;
        y = 1;
        z = x / _NearClip;
        w = 1 / _NearClip;
#else
        x = 1.0 - _NearClip / _FarClip;
        y = _NearClip / _FarClip;
        z = x / _NearClip;
        w = y / _NearClip;
#endif

        //sceneDepthAtFrag = 1.0 / (z * sceneDepthAtFrag + w);
        //float fragDepth = eyePos.z * -1;
        //float depth = sceneDepthAtFrag;
        //depth = pow(depth, _DepthCutoff*fragDepth);
        //_ReflectionIntensity = depth; //change reflection intensity!!
        
        sceneDepthAtFrag = 1.0 / (z * sceneDepthAtFrag + w);
        
        float depth = sceneDepthAtFrag;
        
        depth = clamp(pow(depth, _DepthCutoff * texdistance), 0., 1.);
        
        _ReflectionIntensity = depth; //change reflection intensity!!
        
        texAlpha = depth;

        // blur
        float stepX = -(1.0 / (_ScreenParams.xy / _ScreenParams.w + FLT_MIN).x) * _DepthBlur * (1 - depth);
        float stepY = -(1.0 / (_ScreenParams.xy / _ScreenParams.w + FLT_MIN).y) * _DepthBlur * (1 - depth);
        //change steps according to distance, so distant objects will not blur very much
        stepX = stepX / texdistance;
        stepY = stepY / texdistance;
        //change steps with depth, so far away (much deeper) objects will not mix very much
        //stepX = stepX * (depth;
        //stepY = stepY * (depth / 10);
        
        if(eyeIndex > 0)
            reflection = BlurWithLOD(_ReflectionTexOther, screenUV, stepX, stepY);
        else
            reflection = BlurWithLOD(_ReflectionTex, screenUV, stepX, stepY);
    }
    
    if(_Mode == 0){ //manipulate if surface is not transparent
        if (_EnableMask>0) {
            if (isForward) {
                diffColor = diffColor * (1-mask) +
                    diffColor * mask * (1-_ReflectionIntensity);
                emissiveColor = reflection.rgb * _ReflectionIntensity * pow(mask,_MaskEdgeDarkness);
            } else { 
                diffColor = diffColor * (1-mask) +
                    diffColor * mask * (1-_ReflectionIntensity);
                emissiveColor = emissiveColor * (1-mask) +
                    emissiveColor * mask * (1-_ReflectionIntensity)
                    + reflection.rgb * _ReflectionIntensity * pow(mask,_MaskEdgeDarkness);
            }
        } else {
            if (isForward) {
                diffColor = diffColor*(1-_ReflectionIntensity);
                emissiveColor = reflection.rgb * _ReflectionIntensity;
            } else {
                diffColor = diffColor*(1-_ReflectionIntensity);
                emissiveColor = emissiveColor*(1-_ReflectionIntensity)
                    + reflection.rgb * _ReflectionIntensity;
            }
        }
    }
    else //transparent!
    {
        //no need masking because it already alpha enabled :)
        if(_WorkType == 3.) //My transparency
        {
            texAlpha = _ReflectionIntensity * mask;
            float3 _check = float3(0, 0, 0);
            if (any(reflection.rgb == _check)) {
                texAlpha = 0;
            }
            else
            {
                //test the neighbours
                texAlpha = TestNeighboursForAR(screenUV, eyeIndex, texAlpha);
            }
            diffColor = 0;//reflection * texAlpha;// + //(1-mask) +
                //reflection * (1-texAlpha) * _ReflectionIntensity;
                //diffColor * mask * (1-_ReflectionIntensity);
            emissiveColor = reflection * texAlpha; //reflection.rgb * _ReflectionIntensity * (1 - texAlpha);
        }
        else{
            if (isForward) {
                diffColor = diffColor * texAlpha; //diffColor*(1-_ReflectionIntensity);
                #ifndef _FULLMIRROR
                    emissiveColor = reflection.rgb * _ReflectionIntensity * (1 - texAlpha) * mask;
                #else
                    emissiveColor = reflection.rgb * _ReflectionIntensity * mask;
                #endif
                
            } else {
                diffColor = diffColor * texAlpha; //diffColor*(1-_ReflectionIntensity);
                #ifndef _FULLMIRROR
                    emissiveColor = emissiveColor*(1-_ReflectionIntensity) * (1 - texAlpha) * mask
                        + reflection.rgb * _ReflectionIntensity * (1 - texAlpha) * mask;
                #else
                    emissiveColor = emissiveColor*(1-_ReflectionIntensity) * mask
                        + reflection.rgb * _ReflectionIntensity * mask;
                #endif
                
            }
            texAlpha = max(_ReflectionIntensity, texAlpha); //so alpha value will be not color, but reflection :)
        }
    }
    //if(_Surface != 1){ //manipulate if surface is not transparent
    //    if(_EnableMask>0){
    //        surfaceData.albedo = surfaceData.albedo * (1-mask.a) +
    //            surfaceData.albedo * mask.a * (1-_ReflectionIntensity);
                
    //        surfaceData.emission = reflection * _ReflectionIntensity * pow(mask.a,_MaskEdgeDarkness);
    //    }
    //    else{
    //        surfaceData.albedo   = (1-_ReflectionIntensity) * surfaceData.albedo;
    //        surfaceData.emission = _ReflectionIntensity     * reflection;
    //    }
    //}
    
    //half4 color = LightweightFragmentPBR(inputData, surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.occlusion, surfaceData.emission, surfaceData.alpha);
    
    //if(_Surface == 1){ //transparent mix of reflection for glass like objects
    //    color = color*(1-_ReflectionIntensity+surfaceData.alpha) + 
    //            half4(reflection,1)*(_ReflectionIntensity-surfaceData.alpha);
    //}
    
    //color.rgb = MixFog(color.rgb, inputData.fogCoord);
    
    //return color;
}

inline FragmentCommonData SpecularSetup (float4 i_tex)
{
    half4 specGloss = SpecularGloss(i_tex.xy);
    half3 specColor = specGloss.rgb;
    half smoothness = specGloss.a;

    half oneMinusReflectivity;
    half3 diffColor = EnergyConservationBetweenDiffuseAndSpecular (Albedo(i_tex), specColor, /*out*/ oneMinusReflectivity);

    FragmentCommonData o = (FragmentCommonData)0;
    o.diffColor = diffColor;
    o.specColor = specColor;
    o.oneMinusReflectivity = oneMinusReflectivity;
    o.smoothness = smoothness;
    return o;
}

#if  UNITY_2018_1_OR_NEWER
    inline FragmentCommonData RoughnessSetup(float4 i_tex)
    {
        half2 metallicGloss = MetallicRough(i_tex.xy);
        half metallic = metallicGloss.x;
        half smoothness = metallicGloss.y; // this is 1 minus the square root of real roughness m.

        half oneMinusReflectivity;
        half3 specColor;
        half3 diffColor = DiffuseAndSpecularFromMetallic(Albedo(i_tex), metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);

        FragmentCommonData o = (FragmentCommonData)0;
        o.diffColor = diffColor;
        o.specColor = specColor;
        o.oneMinusReflectivity = oneMinusReflectivity;
        o.smoothness = smoothness;
        return o;
    }
#endif

inline FragmentCommonData MetallicSetup (float4 i_tex)
{
    half2 metallicGloss = MetallicGloss(i_tex.xy);
    half metallic = metallicGloss.x;
    half smoothness = metallicGloss.y; // this is 1 minus the square root of real roughness m.

    half oneMinusReflectivity;
    half3 specColor;
    half3 diffColor = DiffuseAndSpecularFromMetallic (Albedo(i_tex), metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);

    FragmentCommonData o = (FragmentCommonData)0;
    o.diffColor = diffColor;
    o.specColor = specColor;
    o.oneMinusReflectivity = oneMinusReflectivity;
    o.smoothness = smoothness;
    return o;
}

// parallax transformed texcoord is used to sample occlusion
inline FragmentCommonData FragmentSetup (inout float4 i_tex, half3 i_eyeVec, half3 i_viewDirForParallax, half4 tangentToWorld[3], half3 i_posWorld)
{
    i_tex = Parallax(i_tex, i_viewDirForParallax);

    half alpha = Alpha(i_tex.xy);
    #if defined(_ALPHATEST_ON)
        clip (alpha - _Cutoff);
    #endif

    FragmentCommonData o = UNITY_SETUP_BRDF_INPUT (i_tex);
    o.normalWorld = PerPixelWorldNormal(i_tex, tangentToWorld);
    o.eyeVec = NormalizePerPixelNormal(i_eyeVec);
    o.posWorld = i_posWorld;
    
    // NOTE: shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
    o.diffColor = PreMultiplyAlpha (o.diffColor, alpha, o.oneMinusReflectivity, /*out*/ o.alpha);
    return o;
}

inline UnityGI FragmentGI (FragmentCommonData s, half occlusion, half4 i_ambientOrLightmapUV, half atten, UnityLight light, bool reflections)
{
    UnityGIInput d;
    d.light = light;
    d.worldPos = s.posWorld;
    d.worldViewDir = -s.eyeVec;
    d.atten = atten;
    #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
        d.ambient = 0;
        d.lightmapUV = i_ambientOrLightmapUV;
    #else
        d.ambient = i_ambientOrLightmapUV.rgb;
        d.lightmapUV = 0;
    #endif
    
    d.probeHDR[0] = unity_SpecCube0_HDR;
    d.probeHDR[1] = unity_SpecCube1_HDR;
    #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
      d.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
    #endif
    #ifdef UNITY_SPECCUBE_BOX_PROJECTION
      d.boxMax[0] = unity_SpecCube0_BoxMax;
      d.probePosition[0] = unity_SpecCube0_ProbePosition;
      d.boxMax[1] = unity_SpecCube1_BoxMax;
      d.boxMin[1] = unity_SpecCube1_BoxMin;
      d.probePosition[1] = unity_SpecCube1_ProbePosition;
    #endif

    if(reflections)
    {
        Unity_GlossyEnvironmentData g = UnityGlossyEnvironmentSetup(s.smoothness, -s.eyeVec, s.normalWorld, s.specColor);
        // Replace the reflUVW if it has been compute in Vertex shader. Note: the compiler will optimize the calcul in UnityGlossyEnvironmentSetup itself
        #if UNITY_STANDARD_SIMPLE
            g.reflUVW = s.reflUVW;
        #endif

        return UnityGlobalIllumination (d, occlusion, s.normalWorld, g);
    }
    else
    {
        return UnityGlobalIllumination (d, occlusion, s.normalWorld);
    }
}

inline UnityGI FragmentGI (FragmentCommonData s, half occlusion, half4 i_ambientOrLightmapUV, half atten, UnityLight light)
{
    return FragmentGI(s, occlusion, i_ambientOrLightmapUV, atten, light, true);
}


//-------------------------------------------------------------------------------------
half4 OutputForward (half4 output, half alphaFromSurface)
{
    #if defined(_ALPHABLEND_ON) || defined(_ALPHAPREMULTIPLY_ON)
        output.a = alphaFromSurface;
    #else
        UNITY_OPAQUE_ALPHA(output.a);
    #endif
    return output;
}

inline half4 VertexGIForward(VertexInput v, float3 posWorld, half3 normalWorld)
{
    half4 ambientOrLightmapUV = 0;
    // Static lightmaps
    #ifdef LIGHTMAP_ON
        ambientOrLightmapUV.xy = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
        ambientOrLightmapUV.zw = 0;
    // Sample light probe for Dynamic objects only (no static or dynamic lightmaps)
    #elif UNITY_SHOULD_SAMPLE_SH
        #ifdef VERTEXLIGHT_ON
            // Approximated illumination from non-important point lights
            ambientOrLightmapUV.rgb = Shade4PointLights (
                unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                unity_4LightAtten0, posWorld, normalWorld);
        #endif

        ambientOrLightmapUV.rgb = ShadeSHPerVertex (normalWorld, ambientOrLightmapUV.rgb);
    #endif

    #ifdef DYNAMICLIGHTMAP_ON
        ambientOrLightmapUV.zw = v.uv2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
    #endif

    return ambientOrLightmapUV;
}

// ------------------------------------------------------------------
//  Base forward pass (directional light, emission, lightmaps, ...)

struct VertexOutputForwardBase
{
    UNITY_POSITION(pos);
    float4 tex                          : TEXCOORD0;
    half3 eyeVec                        : TEXCOORD1;
    half4 tangentToWorldAndPackedData[3]    : TEXCOORD2;    // [3x3:tangentToWorld | 1x3:viewDirForParallax or worldPos]
    half4 ambientOrLightmapUV           : TEXCOORD5;    // SH or Lightmap UV
    #if  UNITY_2018_1_OR_NEWER
    UNITY_LIGHTING_COORDS(6,7)
    #else
    UNITY_SHADOW_COORDS(6)
    UNITY_FOG_COORDS(7)
    #endif
    
    // next ones would not fit into SM2.0 limits, but they are always for SM3.0+
    #if UNITY_REQUIRE_FRAG_WORLDPOS && !UNITY_PACK_WORLDPOS_WITH_TANGENT
        float3 posWorld                 : TEXCOORD8;
    #endif
    
    float4 screenPos                    : TEXCOORD9;
    float distance                      : TEXCOORD10;
    float eyeIndex                      : TEXCOORD11;
    float4 eyePos                       : TEXCOORD12;

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

VertexOutputForwardBase vertForwardBase (VertexInput v)
{
    UNITY_SETUP_INSTANCE_ID(v);
    VertexOutputForwardBase o;
    UNITY_INITIALIZE_OUTPUT(VertexOutputForwardBase, o);
    UNITY_TRANSFER_INSTANCE_ID(v, o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
    #if UNITY_REQUIRE_FRAG_WORLDPOS
        #if UNITY_PACK_WORLDPOS_WITH_TANGENT
            o.tangentToWorldAndPackedData[0].w = posWorld.x;
            o.tangentToWorldAndPackedData[1].w = posWorld.y;
            o.tangentToWorldAndPackedData[2].w = posWorld.z;
        #else
            o.posWorld = posWorld.xyz;
        #endif
    #endif
    o.pos = UnityObjectToClipPos(v.vertex);
    o.screenPos = ComputeScreenPos(o.pos); //!!
    //COMPUTE_EYEDEPTH(o.eyeDepth);
    //o.depthColor = _DepthPlaneOrigin*_DepthPlaneNormal-posWorld*_DepthPlaneNormal;
    o.distance = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, o.pos));
    o.eyePos = mul(UNITY_MATRIX_MV, v.vertex);
    
    #ifdef UNITY_SINGLE_PASS_STEREO
        o.eyeIndex = unity_StereoEyeIndex;
    #else
    // When not using single pass stereo rendering, eye index must be determined by testing the
    // sign of the horizontal skew of the projection matrix.
    if (unity_CameraProjection[0][2] > 0) {
        o.eyeIndex = 1.0;
    } else {
        o.eyeIndex = 0.0;
    }
    #endif
    
    o.tex = TexCoords(v);
    o.eyeVec = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
    float3 normalWorld = UnityObjectToWorldNormal(v.normal);
    #ifdef _TANGENT_TO_WORLD
        float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);

        float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
        o.tangentToWorldAndPackedData[0].xyz = tangentToWorld[0];
        o.tangentToWorldAndPackedData[1].xyz = tangentToWorld[1];
        o.tangentToWorldAndPackedData[2].xyz = tangentToWorld[2];
    #else
        o.tangentToWorldAndPackedData[0].xyz = 0;
        o.tangentToWorldAndPackedData[1].xyz = 0;
        o.tangentToWorldAndPackedData[2].xyz = normalWorld;
    #endif

    //We need this for shadow receving
    #if  UNITY_2018_1_OR_NEWER
    UNITY_TRANSFER_LIGHTING(o, v.uv1);
    #else
    UNITY_TRANSFER_SHADOW(o, v.uv1);
    #endif
    
    o.ambientOrLightmapUV = VertexGIForward(v, posWorld, normalWorld);

    #ifdef _PARALLAXMAP
        TANGENT_SPACE_ROTATION;
        half3 viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
        o.tangentToWorldAndPackedData[0].w = viewDirForParallax.x;
        o.tangentToWorldAndPackedData[1].w = viewDirForParallax.y;
        o.tangentToWorldAndPackedData[2].w = viewDirForParallax.z;
    #endif
    
    #if  UNITY_2018_1_OR_NEWER
    UNITY_TRANSFER_FOG_COMBINED_WITH_EYE_VEC(o,o.pos);
    #else
    UNITY_TRANSFER_FOG(o,o.pos);
    #endif
    return o;
}

half4 fragForwardBaseInternal (VertexOutputForwardBase i)
{
    UNITY_APPLY_DITHER_CROSSFADE(i.pos.xy);

    FRAGMENT_SETUP(s)

    UNITY_SETUP_INSTANCE_ID(i);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

    UnityLight mainLight = MainLight ();
    
    UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld);
    
    half occlusion = Occlusion(i.tex.xy);
    
    half3 emissiveColor;
    CalculateReflection(i.screenPos, i.distance, i.eyeIndex, i.eyePos, s.smoothness, s.specColor,
        i.tex.xy, s.normalWorld, s.diffColor, true, emissiveColor, s.alpha);

    #ifndef _FULLMIRROR
        UnityGI gi = FragmentGI (s, occlusion, i.ambientOrLightmapUV, atten, mainLight);
    #else
        UnityGI gi = FragmentGI (s, 0, i.ambientOrLightmapUV, 0, mainLight);
    #endif

    #ifdef _DISABLEPROBES
    half4 c = UNITY_BRDF_PBS (s.diffColor,
        0, 1, 0, s.normalWorld, 
        -s.eyeVec, gi.light, gi.indirect);
    #else
    half4 c = UNITY_BRDF_PBS (s.diffColor,
        s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, 
        -s.eyeVec, gi.light, gi.indirect);
    #endif
    
    c.rgb += Emission(i.tex.xy)+emissiveColor;
    
    #if  UNITY_2018_1_OR_NEWER
    UNITY_EXTRACT_FOG_FROM_EYE_VEC(i);
    UNITY_APPLY_FOG(_unity_fogCoord, c.rgb);
    #else
    UNITY_APPLY_FOG(i.fogCoord, c.rgb);
    #endif
    return OutputForward (c, s.alpha);
}

half4 fragForwardBase (VertexOutputForwardBase i) : SV_Target   // backward compatibility (this used to be the fragment entry function)
{
    return fragForwardBaseInternal(i);
}

// ------------------------------------------------------------------
//  Additive forward pass (one light per pass)

struct VertexOutputForwardAdd
{
    UNITY_POSITION(pos);
    float4 tex                          : TEXCOORD0;
    half3 eyeVec                        : TEXCOORD1; //float3 on 2017??? todo:
    half4 tangentToWorldAndLightDir[3]  : TEXCOORD2;    // [3x3:tangentToWorld | 1x3:lightDir]
    float3 posWorld                     : TEXCOORD5;
    #if  UNITY_2018_1_OR_NEWER
    UNITY_LIGHTING_COORDS(6, 7)
    #else
    UNITY_SHADOW_COORDS(6)
    UNITY_FOG_COORDS(7)
    #endif
    
    // next ones would not fit into SM2.0 limits, but they are always for SM3.0+
#if defined(_PARALLAXMAP)
    half3 viewDirForParallax            : TEXCOORD8;
#endif

    float4 screenPos                    : TEXCOORD9;
    float4 distance                     : TEXCOORD10;
    float eyeIndex                      : TEXCOORD11;
    //float4 depthColor                   : TEXCOORD12;
    //float eyeDepth                      : TEXCOORD12;
    float4 eyePos                       : TEXCOORD12;
    
    UNITY_VERTEX_OUTPUT_STEREO
};

VertexOutputForwardAdd vertForwardAdd (VertexInput v)
{
    UNITY_SETUP_INSTANCE_ID(v);
    VertexOutputForwardAdd o;
    UNITY_INITIALIZE_OUTPUT(VertexOutputForwardAdd, o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
    o.pos = UnityObjectToClipPos(v.vertex);
    o.screenPos = ComputeScreenPos(o.pos); //!!
    //COMPUTE_EYEDEPTH(o.eyeDepth);
    //o.depthColor = _DepthPlaneOrigin*_DepthPlaneNormal-posWorld*_DepthPlaneNormal;
    o.distance = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, o.pos));
    o.eyePos = mul(UNITY_MATRIX_MV, v.vertex);
    
    #ifdef UNITY_SINGLE_PASS_STEREO
        o.eyeIndex = unity_StereoEyeIndex;
    #else
    // When not using single pass stereo rendering, eye index must be determined by testing the
    // sign of the horizontal skew of the projection matrix.
    if (unity_CameraProjection[0][2] > 0) {
        o.eyeIndex = 1.0;
    } else {
        o.eyeIndex = 0.0;
    }
    #endif
    
    o.tex = TexCoords(v);
    o.eyeVec.xyz = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
    o.posWorld = posWorld.xyz;
    float3 normalWorld = UnityObjectToWorldNormal(v.normal);
    #ifdef _TANGENT_TO_WORLD
        float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);

        float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
        o.tangentToWorldAndLightDir[0].xyz = tangentToWorld[0];
        o.tangentToWorldAndLightDir[1].xyz = tangentToWorld[1];
        o.tangentToWorldAndLightDir[2].xyz = tangentToWorld[2];
    #else
        o.tangentToWorldAndLightDir[0].xyz = 0;
        o.tangentToWorldAndLightDir[1].xyz = 0;
        o.tangentToWorldAndLightDir[2].xyz = normalWorld;
    #endif
    //We need this for shadow receiving
    #if  UNITY_2018_1_OR_NEWER
    UNITY_TRANSFER_LIGHTING(o, v.uv1);
    #else
    UNITY_TRANSFER_SHADOW(o, v.uv1);
    #endif
    
    float3 lightDir = _WorldSpaceLightPos0.xyz - posWorld.xyz * _WorldSpaceLightPos0.w;
    #ifndef USING_DIRECTIONAL_LIGHT
        lightDir = NormalizePerVertexNormal(lightDir);
    #endif
    o.tangentToWorldAndLightDir[0].w = lightDir.x;
    o.tangentToWorldAndLightDir[1].w = lightDir.y;
    o.tangentToWorldAndLightDir[2].w = lightDir.z;

    #ifdef _PARALLAXMAP
        TANGENT_SPACE_ROTATION;
        o.viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
    #endif
    
    #if  UNITY_2018_1_OR_NEWER
    UNITY_TRANSFER_FOG_COMBINED_WITH_EYE_VEC(o, o.pos);
    #else
    UNITY_TRANSFER_FOG(o,o.pos);
    #endif
    return o;
}

half4 fragForwardAddInternal (VertexOutputForwardAdd i)
{
    UNITY_APPLY_DITHER_CROSSFADE(i.pos.xy);
    #if  UNITY_2018_1_OR_NEWER
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
    #endif
    FRAGMENT_SETUP_FWDADD(s)
    
    UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld); 
    
    UnityLight light = AdditiveLight (IN_LIGHTDIR_FWDADD(i), atten);
    UnityIndirect noIndirect = ZeroIndirect ();
    
    half3 emissiveColor;
    CalculateReflection(i.screenPos, i.distance, i.eyeIndex, i.eyePos, s.smoothness, s.specColor,
        i.tex.xy, s.normalWorld, s.diffColor, true, emissiveColor, s.alpha);
    
    #ifdef _DISABLEPROBES
    half4 c = UNITY_BRDF_PBS (s.diffColor,
        0, 1, 0, s.normalWorld, 
        -s.eyeVec, light, noIndirect);
    #else
    half4 c = UNITY_BRDF_PBS (s.diffColor,
        s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, 
        -s.eyeVec, light, noIndirect);
    #endif
    
    #if  UNITY_2018_1_OR_NEWER
    UNITY_EXTRACT_FOG_FROM_EYE_VEC(i);
    UNITY_APPLY_FOG_COLOR(_unity_fogCoord, c.rgb, half4(0,0,0,0)); // fog towards black in additive pass
    #else
    UNITY_APPLY_FOG_COLOR(i.fogCoord, c.rgb, half4(0,0,0,0)); // fog towards black in additive pass
    #endif
    return OutputForward (c, s.alpha);
}

half4 fragForwardAdd (VertexOutputForwardAdd i) : SV_Target     // backward compatibility (this used to be the fragment entry function)
{
    return fragForwardAddInternal(i);
}

// ------------------------------------------------------------------
//  Deferred pass

struct VertexOutputDeferred
{
    UNITY_POSITION(pos);
    float4 tex                          : TEXCOORD0;
    half3 eyeVec                        : TEXCOORD1;
    half4 tangentToWorldAndPackedData[3]: TEXCOORD2;    // [3x3:tangentToWorld | 1x3:viewDirForParallax or worldPos]
    half4 ambientOrLightmapUV           : TEXCOORD5;    // SH or Lightmap UVs

    #if UNITY_REQUIRE_FRAG_WORLDPOS && !UNITY_PACK_WORLDPOS_WITH_TANGENT
        float3 posWorld                     : TEXCOORD6;
    #endif
    
    float4 screenPos                     :TEXCOORD7;
    float4 distance                      :TEXCOORD8;
    float eyeIndex                       :TEXCOORD9;
    //float4 depthColor                    :TEXCOORD10;
    //float eyeDepth                       :TEXCOORD10;
    float4 eyePos                        :TEXCOORD10;
    
    #if  UNITY_2018_1_OR_NEWER
    UNITY_VERTEX_INPUT_INSTANCE_ID
    #endif
    UNITY_VERTEX_OUTPUT_STEREO
};


VertexOutputDeferred vertDeferred (VertexInput v)
{
    UNITY_SETUP_INSTANCE_ID(v);
    VertexOutputDeferred o;
    UNITY_INITIALIZE_OUTPUT(VertexOutputDeferred, o);
    #if  UNITY_2018_1_OR_NEWER
    UNITY_TRANSFER_INSTANCE_ID(v, o);
    #endif
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    
    float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
    #if UNITY_REQUIRE_FRAG_WORLDPOS
        #if UNITY_PACK_WORLDPOS_WITH_TANGENT
            o.tangentToWorldAndPackedData[0].w = posWorld.x;
            o.tangentToWorldAndPackedData[1].w = posWorld.y;
            o.tangentToWorldAndPackedData[2].w = posWorld.z;
        #else
            o.posWorld = posWorld.xyz;
        #endif
    #endif
    o.pos = UnityObjectToClipPos(v.vertex);
    o.screenPos = ComputeScreenPos(o.pos); //!!
    //COMPUTE_EYEDEPTH(o.eyeDepth);
    //o.depthColor = _DepthPlaneOrigin*_DepthPlaneNormal-posWorld*_DepthPlaneNormal;
    o.distance = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, o.pos));
    o.eyePos = mul(UNITY_MATRIX_MV, v.vertex);

    #ifdef UNITY_SINGLE_PASS_STEREO
        o.eyeIndex = unity_StereoEyeIndex;
    #else
    // When not using single pass stereo rendering, eye index must be determined by testing the
    // sign of the horizontal skew of the projection matrix.
    if (unity_CameraProjection[0][2] > 0) {
        o.eyeIndex = 1.0;
    } else {
        o.eyeIndex = 0.0;
    }
    #endif
    
    o.tex = TexCoords(v);
    o.eyeVec = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
    float3 normalWorld = UnityObjectToWorldNormal(v.normal);
    #ifdef _TANGENT_TO_WORLD
        float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);

        float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
        o.tangentToWorldAndPackedData[0].xyz = tangentToWorld[0];
        o.tangentToWorldAndPackedData[1].xyz = tangentToWorld[1];
        o.tangentToWorldAndPackedData[2].xyz = tangentToWorld[2];
    #else
        o.tangentToWorldAndPackedData[0].xyz = 0;
        o.tangentToWorldAndPackedData[1].xyz = 0;
        o.tangentToWorldAndPackedData[2].xyz = normalWorld;
    #endif

    o.ambientOrLightmapUV = 0;
    #ifdef LIGHTMAP_ON
        o.ambientOrLightmapUV.xy = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    #elif UNITY_SHOULD_SAMPLE_SH
        o.ambientOrLightmapUV.rgb = ShadeSHPerVertex (normalWorld, o.ambientOrLightmapUV.rgb);
    #endif
    #ifdef DYNAMICLIGHTMAP_ON
        o.ambientOrLightmapUV.zw = v.uv2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
    #endif

    #ifdef _PARALLAXMAP
        TANGENT_SPACE_ROTATION;
        half3 viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
        o.tangentToWorldAndPackedData[0].w = viewDirForParallax.x;
        o.tangentToWorldAndPackedData[1].w = viewDirForParallax.y;
        o.tangentToWorldAndPackedData[2].w = viewDirForParallax.z;
    #endif
    
    return o;
}

void fragDeferred (
    VertexOutputDeferred i,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3          // RT3: emission (rgb), --unused-- (a)
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    ,out half4 outShadowMask : SV_Target4       // RT4: shadowmask (rgba)
#endif
)
{
    #if (SHADER_TARGET < 30)
        outGBuffer0 = 1;
        outGBuffer1 = 1;
        outGBuffer2 = 0;
        outEmission = 0;
        #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
            outShadowMask = 1;
        #endif
        return;
    #endif

    UNITY_APPLY_DITHER_CROSSFADE(i.pos.xy);

    FRAGMENT_SETUP(s)
    #if  UNITY_2018_1_OR_NEWER
    UNITY_SETUP_INSTANCE_ID(i);
    #endif
    
    // no analytic lights in this pass
    UnityLight dummyLight = DummyLight ();
    half atten = 1;
    
    // only GI
    half occlusion = Occlusion(i.tex.xy);
#if UNITY_ENABLE_REFLECTION_BUFFERS
    bool sampleReflectionsInDeferred = false;
#else
    bool sampleReflectionsInDeferred = true;
#endif

    UnityGI gi = FragmentGI (s, occlusion, i.ambientOrLightmapUV, atten, dummyLight, sampleReflectionsInDeferred);
    
    half3 emissiveColor = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect).rgb;
    
    #ifdef _EMISSION
        emissiveColor += Emission (i.tex.xy);
    #endif

    #ifndef UNITY_HDR_ON
        emissiveColor.rgb = exp2(-emissiveColor.rgb);
    #endif

    UnityStandardData data;
    
    CalculateReflection(i.screenPos, i.distance, i.eyeIndex, i.eyePos, s.smoothness, s.specColor,
        i.tex.xy, s.normalWorld, s.diffColor, false, emissiveColor, s.alpha);

#ifdef _DISABLEPROBES  
    data.occlusion      = 0; //occlusion; // do not calculate occ for UnityStandardDataToGbuffer - to disable probe reflections
#else
    data.occlusion      = occlusion;
#endif
    data.specularColor  = s.specColor;
    data.smoothness     = s.smoothness;
    data.diffuseColor   = s.diffColor;
    data.normalWorld    = s.normalWorld;
    
    #ifndef _FULLMIRROR
        UnityStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);
    #endif
    
    // Emissive lighting buffer
    outEmission = half4(emissiveColor, 1);

#ifdef _DISABLEPROBES  
    data.occlusion      = occlusion; //set correct occ for UnityGetRawBakedOcclusions
#endif
    
    // Baked direct lighting occlusion if any
    #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
        outShadowMask = UnityGetRawBakedOcclusions(i.ambientOrLightmapUV.xy, IN_WORLDPOS(i));
    #endif
}


//
// Old FragmentGI signature. Kept only for backward compatibility and will be removed soon
//

inline UnityGI FragmentGI(
    float3 posWorld,
    half occlusion, half4 i_ambientOrLightmapUV, half atten, half smoothness, half3 normalWorld, half3 eyeVec,
    UnityLight light,
    bool reflections)
{
    // we init only fields actually used
    FragmentCommonData s = (FragmentCommonData)0;
    s.smoothness = smoothness;
    s.normalWorld = normalWorld;
    s.eyeVec = eyeVec;
    s.posWorld = posWorld;
    return FragmentGI(s, occlusion, i_ambientOrLightmapUV, atten, light, reflections);
}
inline UnityGI FragmentGI (
    float3 posWorld,
    half occlusion, half4 i_ambientOrLightmapUV, half atten, half smoothness, half3 normalWorld, half3 eyeVec,
    UnityLight light)
{
    return FragmentGI (posWorld, occlusion, i_ambientOrLightmapUV, atten, smoothness, normalWorld, eyeVec, light, true);
}

#endif // UNITY_STANDARD_CORE_INCLUDED
