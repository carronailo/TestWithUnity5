// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Per pixel bumped refraction.
// Uses a normal map to distort the image behind, and
// an additional texture to tint the color.

Shader "ARPG Project/Particles/Distortion"
{
	Properties 
	{
		_MainTex ("Tint Color (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_BumpAmt  ("Distortion", range (0,256)) = 10
		//_MMultiplier ("Layer Multiplier", Float) = 1
		_InvFade ("Soft Particles Factor", Range(0,10)) = 1.0
	}

	Category 
	{
		Tags { "Queue"="Transparent+10" "IgnoreProjector"="True" "RenderType"="Opaque" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off 
		Lighting Off 
		ZWrite Off 
		Fog { Mode Off }

		SubShader 
		{
			Pass 
			{
				Name "BASE"
				Tags { "LightMode" = "Always" }
			
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma multi_compile_particles
				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex : POSITION;
					fixed4 color : Color;
					float2 texcoord: TEXCOORD0;
				};

				struct v2f 
				{
					float4 vertex : POSITION;
					fixed4 color : Color;
					float4 uvgrab : TEXCOORD0;
					float2 uvbump : TEXCOORD1;
					float2 uvmain : TEXCOORD2;
					#ifdef SOFTPARTICLES_ON
						float4 projPos : TEXCOORD3;
					#endif
				};

				float _InvFade;
				float _BumpAmt;
				float4 _BumpMap_ST;
				float4 _MainTex_ST;

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
//					#if UNITY_UV_STARTS_AT_TOP
//						float scale = -1.0;
//					#else
						float scale = 1.0;
//					#endif
					o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
					o.uvgrab.zw = o.vertex.zw;
					o.uvbump = TRANSFORM_TEX( v.texcoord, _BumpMap );
					o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex );
					o.color = v.color;
					return o;
				}

				sampler2D _GrabTextureMobile;
				float4 _GrabTextureMobile_TexelSize;
				sampler2D _BumpMap;
				sampler2D _MainTex;
				sampler2D _CameraDepthTexture;

				half4 frag( v2f i ) : COLOR
				{
					#ifdef SOFTPARTICLES_ON
						if(_InvFade > 0.0001)
						{
							float sceneZ = LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
							float partZ = i.projPos.z;
							float fade = saturate (_InvFade * (sceneZ-partZ));
							i.color.a *= fade;
						}
					#endif

					// calculate perturbed coordinates
					half2 bump = UnpackNormal(tex2D( _BumpMap, i.uvbump )).rg; // we could optimize this by just reading the x & y without reconstructing the Z
					float2 offset = bump.xy * _BumpAmt * i.color.a * _GrabTextureMobile_TexelSize.xy;
					i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
					half4 col = tex2Dproj( _GrabTextureMobile, UNITY_PROJ_COORD(i.uvgrab));
					col.a = i.color.a * _BumpAmt;
					return col;
				}
				ENDCG
			}
		}

		// ------------------------------------------------------------------
		// Fallback for older cards and Unity non-Pro
	
		SubShader 
		{
			Blend DstColor Zero
			Pass 
			{
				Name "BASE"
				SetTexture [_MainTex] {	combine texture }
			}
		}

		CustomEditor "CustomQueueMatInspector"

	}
}
