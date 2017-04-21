// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// - Unlit
// - Per-vertex (virtual) camera space specular light
// - SUPPORTS lightmap

Shader "MADFINGER/Environment/Lightmap + WindEX"
{
	Properties 
	{
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_Wind("Wind params",Vector) = (1,1,1,1)
		_WindEdgeFlutter("Wind edge fultter factor", float) = 0.5
		_WindEdgeFlutterFreqScale("Wind edge fultter freq scale",float) = 0.5
		_Color("Emissive Color", Color) = (1,1,1,1)

	}

    SubShader 
    {
        Lighting On

	    Tags { "Queue"="Transparent" }  
	    
	    Cull Off 
	    
	    //ZWrite On
	    
 	    CGINCLUDE
	    #include "UnityCG.cginc"
		#include "TerrainEngine.cginc"
	    
	    float _WindEdgeFlutter;
		float _WindEdgeFlutterFreqScale;
		
	    inline float4 AnimateVertex2(float4 pos, float3 normal, float4 animParams,float4 wind,float2 time)
		{	
			// animParams stored in color
			// animParams.x = branch phase
			// animParams.y = edge flutter factor
			// animParams.z = primary factor
			// animParams.w = secondary factor
		
			float fDetailAmp = 0.1f;
			float fBranchAmp = 0.3f;
			
			// Phases (object, vertex, branch)
			float fObjPhase = dot(unity_ObjectToWorld[3].xyz, 1);
			float fBranchPhase = fObjPhase + animParams.x;
			
			float fVtxPhase = dot(pos.xyz, animParams.y + fBranchPhase);
			
			// x is used for edges; y is used for branches
			float2 vWavesIn = time  + float2(fVtxPhase, fBranchPhase );
			
			// 1.975, 0.793, 0.375, 0.193 are good frequencies
			float4 vWaves = (frac( vWavesIn.xxyy * float4(1.975, 0.793, 0.375, 0.193) ) * 2.0 - 1.0);
			
			vWaves = SmoothTriangleWave( vWaves );
			float2 vWavesSum = vWaves.xz + vWaves.yw;
		
			// Edge (xz) and branch bending (y)
			float3 bend = animParams.y * fDetailAmp * normal.xyz;
			bend.y = animParams.w * fBranchAmp;
			pos.xyz += ((vWavesSum.xyx * bend) + (wind.xyz * vWavesSum.y * animParams.w)) * wind.w; 
		
			// Primary bending
			// Displace position
			pos.xyz += animParams.z * wind.xyz;
			
			return pos;
		}  
		ENDCG
	    
	    CGPROGRAM
	    #pragma surface surf Lambert vertex:vert alpha 
	       
	    struct Input 
	    {
	        float2 uv_MainTex;
	    };  
	    
	    sampler2D _MainTex;
	    sampler2D _BumpMap;
	    samplerCUBE _Cube;
	    float4	_Color;
	    
	    void vert(inout appdata_full v)
	    { 
	    	float4	wind;
			
			float	bendingFact	= v.color.a;
			
			wind.xyz	= mul((float3x3)unity_WorldToObject,_Wind.xyz);
			wind.w		= _Wind.w  * bendingFact;
			
			float4	windParams	= float4(0,_WindEdgeFlutter,bendingFact.xx);
			float 	windTime 	= _Time.y * float2(_WindEdgeFlutterFreqScale,1);
			float4	mdlPos		= AnimateVertex2(v.vertex,v.normal,windParams,wind,windTime);
			
			v.vertex =  mdlPos;
			
			v.color.a = 1;

	    }
	      
	    void surf (Input IN, inout SurfaceOutput o) 
	    {
	    	float4 tex = tex2D (_MainTex, IN.uv_MainTex);
	        o.Albedo = tex.rgb;
	        o.Albedo = o.Albedo + _Color.rgb - float3(0.5,0.5,0.5);
	        o.Alpha = tex.a;
	    } 
	    
	    ENDCG
    } 
    //Fallback "Diffuse"

}


