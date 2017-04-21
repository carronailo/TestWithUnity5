// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ARPG Project/Particles/Alpha Blended New" {
	Properties {
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_quse ("quse", Range(0, 1)) = 0
		[MaterialToggle] _RampK ("Ramp I/O", Float ) = 0
		_Ramp ("Ramp Texer", 2D) = "white" {}
		
		_Multiplier ("Color Multiplier", Float) = 1
		_AlphaCutout("Alpha Cut Out", Range(0, 1)) = 0

		//DISSOLVE
		_DissolveTex("Dissolve Texture(A)", 2D) = "white" { }
		_DissolveFactor("Dissolve Factor", Range(0, 1)) = 0
		_DissolveRange("Dissolve Range", Range(0, 1)) = 1

		_GlowTex ("Glow", 2D) = "" {}
		_GlowColor ("Glow Color", Color)  = (1,1,1,1)
		_GlowStrength ("Glow Strength", Float) = 0.0
	}

	Category {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Glow11Transparent" "RenderEffect"="Glow11Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off Lighting Off ZWrite Off Fog { Color(0,0,0,0) }

		ColorMask RGBA

		SubShader {
			Pass {
				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				sampler2D _Ramp;
				sampler2D _DissolveTex;

				fixed4 _TintColor;
				float _RampM;
				
				
				fixed _Multiplier;
				fixed _AlphaCutout;

				uniform half4 _GlowColor;
				uniform fixed _RampK;
				uniform float _quse;

				uniform fixed _DissolveFactor;
				uniform fixed _DissolveRange;

				uniform float _Clip = 0;
				uniform float4 _ClipRange0;// = float4(0.0, 0.0, 1.0, 1.0);
				uniform float2 _ClipArgs0;// = float2(1000.0, 1000.0);

				struct appdata_t {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					half2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					half4 texcoord : TEXCOORD0;
					float2 worldPos : TEXCOORD1;
				};

				float4 _MainTex_ST;
				float4 _Ramp_ST;
				uniform float4 _DissolveTex_ST;

				v2f vert(appdata_t v) 
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.texcoord.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.texcoord.zw = TRANSFORM_TEX(v.texcoord, _DissolveTex);
					o.worldPos = mul(unity_ObjectToWorld, v.vertex).xy;
					if (_Clip > 0.5)
						o.worldPos = o.worldPos * _ClipRange0.zw + _ClipRange0.xy;
					return o;
				}

				fixed4 frag(v2f i) : COLOR
				{
					fixed4 col;
					fixed4 tex = tex2D(_MainTex, i.texcoord.xy);
					fixed4 dis = tex2D(_DissolveTex, i.texcoord.zw);
					float3 quse = lerp(tex.rgb,dot(tex.rgb,float3(0.3,0.59,0.11)),_quse);
					
					fixed4 ramp = tex2D(_Ramp,TRANSFORM_TEX(quse,_Ramp));
					
					col.rgb = 2.0f * lerp(quse, quse.rgb * ramp * 3.0f,_RampK).rgb * i.color.rgb * _TintColor.rgb * _Multiplier;
					col.a = 2.0f * tex.a * i.color.a * _TintColor.a;
					//col.a = col.a > _AlphaCutout ? col.a : 0;
					fixed disDelta = dis.a - _DissolveFactor;
					if (disDelta <= 0)
						col.a = 0;
					else if (disDelta <= _DissolveRange)
						col.a *= disDelta / _DissolveRange;
					if (col.a < _AlphaCutout)
						col.a = 0;
					// ui clipping
					if (_Clip > 0.5)
					{
						float2 factor = (float2(1.0, 1.0) - abs(i.worldPos)) * _ClipArgs0;
						col.a *= clamp(min(factor.x, factor.y), 0.0, 1.0);
					}
					return col;
				}

				ENDCG
			}
		} 	
	
	}

	CustomEditor "CustomQueueAndGlowMatInspector"

}
