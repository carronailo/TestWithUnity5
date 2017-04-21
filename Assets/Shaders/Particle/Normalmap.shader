﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Per pixel bumped refraction.
// Uses a normal map to distort the image behind, and
// an additional texture to tint the color.

Shader "ARPG Project/Particles/Normal" {
Properties {
	_MainTex ("Tint Color (RGB)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_BumpAmt  ("Distortion", range (0,256)) = 10
	//_MMultiplier ("Layer Multiplier", Float) = 1
}

Category {

	// We must be transparent, so other objects are drawn before this one.
	Tags { "Queue"="Transparent+10" "RenderType"="Opaque" }
	//Blend SrcAlpha OneMinusSrcAlpha
	Cull Off Lighting Off ZWrite On Fog { Mode Off }


	SubShader {

		// This pass grabs the screen behind the object into a texture.
		// We can access the result in the next pass as _GrabTexture
		GrabPass {	"GrabTextureTest"					
			Name "BASE"
			Tags { "LightMode" = "Always" }
 		}
 		
 		// Main pass: Take the texture grabbed above and use the bumpmap to perturb it
 		// on to the screen
		Pass {
			Name "BASE"
			Tags { "LightMode" = "Always" }
			
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct appdata_t {
	float4 vertex : POSITION;
	fixed4 color : Color;
	float2 texcoord: TEXCOORD0;
};

struct v2f {
	float4 vertex : POSITION;
	fixed4 color : Color;
	float4 uvgrab : TEXCOORD0;
	float2 uvbump : TEXCOORD1;
	float2 uvmain : TEXCOORD2;
};

float _BumpAmt;
float4 _BumpMap_ST;
float4 _MainTex_ST;

v2f vert (appdata_t v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	#if UNITY_UV_STARTS_AT_TOP
	float scale = -1.0;
	#else
	float scale = 1.0;
	#endif
	o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
	o.uvgrab.zw = o.vertex.zw;
	o.uvbump = TRANSFORM_TEX( v.texcoord, _BumpMap );
	o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex );
	o.color = v.color;
	return o;
}

sampler2D GrabTextureTest;
float4 GrabTextureTest_TexelSize;
sampler2D _BumpMap;
sampler2D _MainTex;

half4 frag( v2f i ) : COLOR
{
	// calculate perturbed coordinates
	half3 bump = UnpackNormal(tex2D( _BumpMap, i.uvbump )).rgb; // we could optimize this by just reading the x & y without reconstructing the Z
	float2 offset = bump.xy * _BumpAmt * i.color.a * GrabTextureTest_TexelSize.xy;
	i.uvgrab.xy = (offset * i.uvgrab.z + i.uvgrab.xy);
	half4 col = tex2Dproj( GrabTextureTest, UNITY_PROJ_COORD(i.uvgrab));
//	col.a = dot(bump.xyz - 0.5f);
//	half4 tint = tex2D( _MainTex, i.uvmain );
	return col;
}
ENDCG
		}
	}

	// ------------------------------------------------------------------
	// Fallback for older cards and Unity non-Pro
	
	SubShader {
		Blend DstColor Zero
		Pass {
			Name "BASE"
			SetTexture [_MainTex] {	combine texture }
		}
	}
}

}