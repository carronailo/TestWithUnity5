// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_LightmapInd', a built-in variable
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D
// Upgrade NOTE: replaced tex2D unity_LightmapInd with UNITY_SAMPLE_TEX2D_SAMPLER

Shader "ARPG Project/Diffuse (Soft Edge)" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SoftEdge ("Soft Edge", Float) = 0.8
	}
	SubShader {
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType"="Transparent" }
		Pass {
			Tags { "LightMode" = "ForwardBase" }

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
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

			sampler2D _MainTex;
			fixed4 _Color;

			struct Input {
				float2 uv_MainTex;
			};

			void surf (Input IN, inout SurfaceOutput o) {
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Alpha = c.a;
			}

			// vertex-to-fragment interpolation data
			#ifdef LIGHTMAP_OFF
			struct v2f {
				float4 pos : SV_POSITION;
				float2 pack0 : TEXCOORD0;
				fixed3 normal : TEXCOORD1;
				fixed3 vlight : TEXCOORD2;
				LIGHTING_COORDS(3,4)
				float4 screenPos : TEXCOORD5;
			};
			#endif
			#ifndef LIGHTMAP_OFF
			struct v2f {
				float4 pos : SV_POSITION;
				float2 pack0 : TEXCOORD0;
				float2 lmap : TEXCOORD1;
				LIGHTING_COORDS(2,3)
				float4 screenPos : TEXCOORD4;
			};
			#endif
			#ifndef LIGHTMAP_OFF
			// float4 unity_LightmapST;
			#endif
			float4 _MainTex_ST;

			sampler2D _CameraDepthTexture;
			fixed _SoftEdge;

			// vertex shader
			v2f vert (appdata_full v) {
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
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

				o.screenPos = ComputeScreenPos(o.pos);
				COMPUTE_EYEDEPTH(o.screenPos.z);

				return o;
			}

			#ifndef LIGHTMAP_OFF
			// sampler2D unity_Lightmap;
			#ifndef DIRLIGHTMAP_OFF
			// sampler2D unity_LightmapInd;
			#endif
			#endif

			// fragment shader
			fixed4 frag (v2f IN) : SV_Target {
				// prepare and unpack data
				#ifdef UNITY_COMPILER_HLSL
				Input surfIN = (Input)0;
				#else
				Input surfIN;
				#endif
				surfIN.uv_MainTex = IN.pack0.xy;
				#ifdef UNITY_COMPILER_HLSL
				SurfaceOutput o = (SurfaceOutput)0;
				#else
				SurfaceOutput o;
				#endif
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Specular = 0.0;
				o.Alpha = 0.0;
				o.Gloss = 0.0;
				#ifdef LIGHTMAP_OFF
				o.Normal = IN.normal;
				#endif

				// call surface function
				surf (surfIN, o);

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
					#if (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)) && defined(SHADER_API_MOBILE)
					c.rgb += o.Albedo * min(lm, atten*2);
					#else
					c.rgb += o.Albedo * max(min(lm,(atten*2)*lmtex.rgb), lm*atten);
					#endif
				#else // SHADOWS_SCREEN
					c.rgb += o.Albedo * lm;
				#endif // SHADOWS_SCREEN
				c.a = o.Alpha;
				#endif // LIGHTMAP_OFF

				// Soft Edges
				half depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, IN.screenPos.xyzw);
				depth = LinearEyeDepth(depth);
				fixed diff = saturate((depth - IN.screenPos.z) * _SoftEdge);				
				c.a *= diff;

				return c;
			}

			ENDCG
		}
	}

	//Fallback "Mobile/Diffuse"
}
