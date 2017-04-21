// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_LightmapInd', a built-in variable
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D
// Upgrade NOTE: replaced tex2D unity_LightmapInd with UNITY_SAMPLE_TEX2D_SAMPLER

Shader "ARPG Project/Terrain/T4M Modify AddPass(9~12)" 
{
	Properties 
	{
		_Splat0 ("Layer 1", 2D) = "white" {}
		_Splat1 ("Layer 2", 2D) = "white" {}
		_Splat2 ("Layer 3", 2D) = "white" {}
		_Splat3 ("Layer 4", 2D) = "white" {}
		_Control ("Control (RGBA)", 2D) = "white" {}
	}
	SubShader 
	{
		Tags 
		{
			"SplatCount" = "4"
			"Queue" = "Geometry-98"
			"IgnoreProjector"="True"
			"RenderType" = "Opaque"
		}
		
		Pass 
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }
			Blend One One ZWrite Off Fog { Color (0,0,0,0) }

			CGPROGRAM
			// compile directives
			#pragma vertex vert_surf
			#pragma fragment frag_surf
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fwdbase
			#include "HLSLSupport.cginc"
			#include "UnityShaderVariables.cginc"
			#define UNITY_PASS_FORWARDBASE
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			#define INTERNAL_DATA
			#define WorldReflectionVector(data,normal) data.worldRefl
			#define WorldNormalVector(data,normal) normal

			sampler2D _Control;
			sampler2D _Splat0,_Splat1,_Splat2,_Splat3;

			uniform fixed4 _Splat0TillingOffset;

#ifdef LIGHTMAP_OFF
			struct v2f_surf
			{
				float4 pos : SV_POSITION;
				float4 pack0 : TEXCOORD0;
				float4 pack1 : TEXCOORD1;
				float2 pack2 : TEXCOORD2;
				fixed3 normal : TEXCOORD3;
				fixed3 vlight : TEXCOORD4;
				LIGHTING_COORDS(5,6)
			};
#else
			struct v2f_surf 
			{
				float4 pos : SV_POSITION;
				float4 pack0 : TEXCOORD0;
				float4 pack1 : TEXCOORD1;
				float2 pack2 : TEXCOORD2;
				float2 lmap : TEXCOORD3;
				LIGHTING_COORDS(4,5)
			};
			// float4 unity_LightmapST;
#endif

			float4 _Control_ST;
			float4 _Splat0_ST;
			float4 _Splat1_ST;
			float4 _Splat2_ST;
			float4 _Splat3_ST;

			// vertex shader
			v2f_surf vert_surf (appdata_full v) 
			{
				v2f_surf o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.pack0.xy = TRANSFORM_TEX(v.texcoord, _Control);
				o.pack0.zw = TRANSFORM_TEX(v.texcoord, _Splat0);
				o.pack1.xy = TRANSFORM_TEX(v.texcoord, _Splat1);
				o.pack1.zw = TRANSFORM_TEX(v.texcoord, _Splat2);
				o.pack2.xy = TRANSFORM_TEX(v.texcoord, _Splat3);
#ifndef LIGHTMAP_OFF
				o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif
				float3 worldN = mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL);
#ifdef LIGHTMAP_OFF
				o.normal = worldN;
#endif

				// SH/ambient and vertex lights
#ifdef LIGHTMAP_OFF
				float3 shlight = ShadeSH9 (float4(worldN,1.0));
				o.vlight = shlight;
	#ifdef VERTEXLIGHT_ON
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.vlight += Shade4PointLights (
				unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
				unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
				unity_4LightAtten0, worldPos, worldN );
	#endif // VERTEXLIGHT_ON
#endif // LIGHTMAP_OFF

				// pass lighting information to pixel shader
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}

#ifndef LIGHTMAP_OFF
			// sampler2D unity_Lightmap;
	#ifndef DIRLIGHTMAP_OFF
			// sampler2D unity_LightmapInd;
	#endif
#endif

			// fragment shader
			fixed4 frag_surf (v2f_surf IN) : SV_Target 
			{
				fixed4 splat_control = tex2D (_Control, IN.pack0.xy).rgba;
				fixed3 lay1 = tex2D (_Splat0, IN.pack0.zw);
				fixed3 lay2 = tex2D (_Splat1, IN.pack1.xy);
				fixed3 lay3 = tex2D (_Splat2, IN.pack1.zw);
				fixed3 lay4 = tex2D (_Splat3, IN.pack2.xy);
				SurfaceOutput o;
				o.Albedo.rgb = (lay1 * splat_control.r + lay2 * splat_control.g + lay3 * splat_control.b + lay4 * splat_control.a);
				o.Emission = 0.0;
				o.Specular = 0.0;
				o.Alpha = 0.0;
				o.Gloss = 0.0;
#ifdef LIGHTMAP_OFF
				o.Normal = IN.normal;
#endif

				// compute lighting & shadowing factor
				fixed atten = LIGHT_ATTENUATION(IN);
				fixed4 c = 0;

				// realtime lighting: call lighting function
#ifdef LIGHTMAP_OFF
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
				gi.indirect.diffuse = 0;
				gi.indirect.specular = 0;
				gi.light.color = _LightColor0.rgb;
				gi.light.dir = _WorldSpaceLightPos0.xyz;
				c = LightingLambert(o, gi);
				//c = LightingLambert (o, _WorldSpaceLightPos0.xyz, atten);
#endif // LIGHTMAP_OFF || DIRLIGHTMAP_OFF
#ifdef LIGHTMAP_OFF
				c.rgb += o.Albedo * IN.vlight;
#endif // LIGHTMAP_OFF

				// lightmaps:
#ifndef LIGHTMAP_OFF
	#ifndef DIRLIGHTMAP_OFF
				// directional lightmaps
				fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy);
				fixed4 lmIndTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd,unity_Lightmap, IN.lmap.xy);
				half3 lm = LightingLambert_DirLightmap(o, lmtex, lmIndTex, 0).rgb;
	#else // !DIRLIGHTMAP_OFF
				// single lightmap
				fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy);
				fixed3 lm = DecodeLightmap (lmtex);
	#endif // !DIRLIGHTMAP_OFF

				// combine lightmaps with realtime shadows
	#ifdef SHADOWS_SCREEN
		#if defined(UNITY_NO_RGBM)
				c.rgb += o.Albedo * min(lm, atten*2);
		#else
				c.rgb += o.Albedo * max(min(lm,(atten*2)*lmtex.rgb), lm*atten);
		#endif
	#else // SHADOWS_SCREEN
				c.rgb += o.Albedo * lm;
	#endif // SHADOWS_SCREEN
				c.a = o.Alpha;
#endif // LIGHTMAP_OFF

				return c;
			}
			ENDCG

		}

		Pass 
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardAdd" }
			ZWrite Off Blend One One Fog { Color (0,0,0,0) }
			Blend One One ZWrite Off Fog { Color (0,0,0,0) }

			CGPROGRAM
			// compile directives
			#pragma vertex vert_surf
			#pragma fragment frag_surf
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fwdadd
			#include "HLSLSupport.cginc"
			#include "UnityShaderVariables.cginc"
			#define UNITY_PASS_FORWARDADD
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			#define INTERNAL_DATA
			#define WorldReflectionVector(data,normal) data.worldRefl
			#define WorldNormalVector(data,normal) normal

			sampler2D _Control;
			sampler2D _Splat0,_Splat1,_Splat2,_Splat3;

			// vertex-to-fragment interpolation data
			struct v2f_surf 
			{
				float4 pos : SV_POSITION;
				float4 pack0 : TEXCOORD0;
				float4 pack1 : TEXCOORD1;
				float2 pack2 : TEXCOORD2;
				fixed3 normal : TEXCOORD3;
				half3 lightDir : TEXCOORD4;
				LIGHTING_COORDS(5,6)
			};

			float4 _Control_ST;
			float4 _Splat0_ST;
			float4 _Splat1_ST;
			float4 _Splat2_ST;
			float4 _Splat3_ST;

			// vertex shader
			v2f_surf vert_surf (appdata_full v) 
			{
				v2f_surf o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.pack0.xy = TRANSFORM_TEX(v.texcoord, _Control);
				o.pack0.zw = TRANSFORM_TEX(v.texcoord, _Splat0);
				o.pack1.xy = TRANSFORM_TEX(v.texcoord, _Splat1);
				o.pack1.zw = TRANSFORM_TEX(v.texcoord, _Splat2);
				o.pack2.xy = TRANSFORM_TEX(v.texcoord, _Splat3);
				o.normal = mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL);
				float3 lightDir = WorldSpaceLightDir( v.vertex );
				o.lightDir = lightDir;

				// pass lighting information to pixel shader
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}

			// fragment shader
			fixed4 frag_surf (v2f_surf IN) : SV_Target 
			{
				fixed4 splat_control = tex2D (_Control, IN.pack0.xy).rgba;
				fixed3 lay1 = tex2D (_Splat0, IN.pack0.zw);
				fixed3 lay2 = tex2D (_Splat1, IN.pack1.xy);
				fixed3 lay3 = tex2D (_Splat2, IN.pack1.zw);
				fixed3 lay4 = tex2D (_Splat3, IN.pack2.xy);
				SurfaceOutput o;
				o.Albedo.rgb = (lay1 * splat_control.r + lay2 * splat_control.g + lay3 * splat_control.b + lay4 * splat_control.a);
				o.Emission = 0.0;
				o.Specular = 0.0;
				o.Alpha = 0.0;
				o.Gloss = 0.0;
				o.Normal = IN.normal;

#ifndef USING_DIRECTIONAL_LIGHT
				fixed3 lightDir = normalize(IN.lightDir);
#else
				fixed3 lightDir = IN.lightDir;
#endif
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
				gi.indirect.diffuse = 0;
				gi.indirect.specular = 0;
				gi.light.color = _LightColor0.rgb;
				gi.light.dir = lightDir;
				fixed4 c = LightingLambert(o, gi);
				//fixed4 c = LightingLambert (o, lightDir, LIGHT_ATTENUATION(IN));
				c.a = 0.0;
				return c;
			}
			ENDCG

		}
	} 
	FallBack "Diffuse"
}
