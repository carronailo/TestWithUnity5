// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ARPG Project/Particles/Alpha Blended (Soft Edge)" {
	Properties {
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_Multiplier ("Color Multiplier", Float) = 1
		_SoftEdge ("Soft Edge", Float) = 0.8
	}

	Category {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off Lighting Off ZWrite Off ZTest LEqual Fog { Color(0,0,0,0) }

		SubShader {
			Pass {
				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				sampler2D _CameraDepthTexture;

				fixed4 _TintColor;
				fixed _Multiplier;
				fixed _SoftEdge;

				struct appdata_t {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					float4 screenPos : TEXCOORD1;
				};

				float4 _MainTex_ST;

				v2f vert(appdata_t v) 
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.screenPos = ComputeScreenPos(o.vertex);
					COMPUTE_EYEDEPTH(o.screenPos.z);
					return o;
				}

				fixed4 frag(v2f i) : COLOR
				{
					fixed4 col;
					fixed4 tex = tex2D(_MainTex, i.texcoord);
					col.rgb = 2.0f * tex.rgb * i.color.rgb * _TintColor.rgb * _Multiplier;
					col.a = 2.0f * tex.a * i.color.a * _TintColor.a;

					// Soft Edges
					half depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, i.screenPos.xyzw);
					depth = LinearEyeDepth(depth);
					fixed diff = saturate((depth - i.screenPos.z) * _SoftEdge);				
					col.a *= diff;

					return col;
				}

				ENDCG
			}
		} 	
	
	}

	Fallback "ARPG Project/Particles/Alpha Blended"
}
