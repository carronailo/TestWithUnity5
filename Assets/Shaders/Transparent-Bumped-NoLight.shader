// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ARPG Project/Transparent/Bumped Diffuse NoLight" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {} 
	_LightColor("Light Color", Color) = (1,1,1,1)
	_LightIntensity("Light Intensity", Float) = 1
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 300
	ZWrite Off ColorMask RGB

	Pass {
		Name "FORWARD"
		Tags { "LightMode" = "ForwardBase" }
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		// compile directives
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma multi_compile_fwdbase nolightmap nodirlightmap
		#pragma multi_compile_fwdbasealpha
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "AutoLight.cginc"

		sampler2D _MainTex;
		sampler2D _BumpMap;
		fixed4 _Color;
		fixed4 _LightColor;
		fixed _LightIntensity;

		// vertex-to-fragment interpolation data
		struct v2f {
			float4 pos : SV_POSITION;
			float4 pack0 : TEXCOORD0;
			fixed3 lightDir : TEXCOORD1;
			fixed3 vlight : TEXCOORD2;
			LIGHTING_COORDS(3,4)
		};
		float4 _MainTex_ST;
		float4 _BumpMap_ST;

		// vertex shader
		v2f vert (appdata_full v) {
			v2f o;
			o.pos = UnityObjectToClipPos (v.vertex);
			o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.pack0.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);
			float3 worldN = mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL);
			TANGENT_SPACE_ROTATION;
			float3 lightDir = mul (rotation, ObjSpaceLightDir(v.vertex));
			o.lightDir = lightDir;
			// pass lighting information to pixel shader
			return o;
		}

		// fragment shader
		fixed4 frag (v2f IN) : SV_Target {
			fixed4 mainColor = tex2D(_MainTex, IN.pack0.xy) * _Color;
			fixed3 normal = UnpackNormal(tex2D(_BumpMap, IN.pack0.zw));
			// compute lighting & shadowing factor
			// Lambert
			fixed diff = max (0, dot (normal, IN.lightDir));
			fixed4 c;
			c.rgb = mainColor.rgb * _LightColor.rgb * _LightIntensity * (diff /** atten * 2*/);
			c.a = mainColor.a;
			return c;
		}

		ENDCG

	}

	/*
	// ---- forward rendering additive lights pass:
	Pass {
		Name "FORWARD"
		Tags { "LightMode" = "ForwardAdd" }
		ZWrite Off Blend One One Fog { Color (0,0,0,0) }
		Blend SrcAlpha One

		CGPROGRAM
		// compile directives
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_fwdadd
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "AutoLight.cginc"

		sampler2D _MainTex;
		sampler2D _BumpMap;
		fixed4 _Color;
		fixed4 _LightColor;
		fixed _LightIntensity;

		// vertex-to-fragment interpolation data
		struct v2f {
			float4 pos : SV_POSITION;
			float4 pack0 : TEXCOORD0;
			half3 lightDir : TEXCOORD1;
			LIGHTING_COORDS(2,3)
		};
		float4 _MainTex_ST;
		float4 _BumpMap_ST;

		// vertex shader
		v2f vert (appdata_full v) {
			v2f o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.pack0.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);
			TANGENT_SPACE_ROTATION;
			float3 lightDir = mul (rotation, ObjSpaceLightDir(v.vertex));
			o.lightDir = lightDir;
			// pass lighting information to pixel shader
		  return o;
		}

		// fragment shader
		fixed4 frag (v2f IN) : SV_Target {
			fixed4 mainColor = tex2D(_MainTex, IN.pack0.xy) * _Color;
			fixed3 normal = UnpackNormal(tex2D(_BumpMap, IN.pack0.zw));
			#ifndef USING_DIRECTIONAL_LIGHT
				fixed3 lightDir = normalize(IN.lightDir);
			#else
				fixed3 lightDir = IN.lightDir;
			#endif
			// Lambert
			fixed diff = max (0, dot (normal, IN.lightDir));
			fixed4 c;
			c.rgb = mainColor.rgb * _LightColor.rgb * _LightIntensity * (diff /** atten * 2*/);
			c.a = mainColor.a;
			return c;
		}

		ENDCG

	}
	*/
}

FallBack "Transparent/Diffuse"
}