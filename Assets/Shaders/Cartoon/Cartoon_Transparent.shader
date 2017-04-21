// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Cartoon/Transparent" {
	Properties {
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.15)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_ColorMultiplier ("Color Multiplier", Float) = 1
		_AlphaCutout("Alpha Cut Out", Range(0, 1)) = 0
		_RimColor("Rim Color", Color) = (1, 1, 1, 1)
		_RimMultiplier("Rim Multiplier", Float) = 1.1
		_RimPower("Rim Power", Range(0.1, 3.0)) = 1.3

		_GlowTex("Glow", 2D) = "" {}
		_GlowColor("Glow Color", Color) = (1,1,1,1)
		_GlowStrength("Glow Strength", Float) = 0.0
	}

	Category {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True"  "RenderType" = "Glow11Transparent" "RenderEffect" = "Glow11Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back Lighting Off ZWrite on Fog { Mode Off }

		SubShader {
			Pass {
				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				fixed4 _TintColor;
				fixed _ColorMultiplier;
				fixed _RimMultiplier;
				fixed _AlphaCutout;
				

				uniform half4 _GlowColor;
				uniform fixed4 _RimColor;
				uniform fixed _RimBase;
				uniform fixed _RimPower;

				struct appdata_t {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					//float4 pos : SV_POSITION;
				    fixed3 normal : TEXCOORD1;
				    fixed3 viewDir : TEXCOORD2;
				};

				float4 _MainTex_ST;

				v2f vert(appdata_full v) 
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
			 		//o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
					o.color = v.color;
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				    float3 worldN = normalize(mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL));
					o.normal = worldN;
					o.viewDir = normalize(WorldSpaceViewDir( v.vertex ));
					return o;
				}

				fixed4 frag(v2f i) : COLOR
				{
					fixed4 col;
					fixed4 tex = tex2D(_MainTex, i.texcoord);
					half rim = 1.0 - saturate(dot (i.viewDir, i.normal));
					half4 combinedColor = _RimColor  * tex * pow (rim, _RimPower) *_RimMultiplier ;
					col = 2.0f * tex * i.color * _TintColor * _ColorMultiplier + combinedColor;
					//col.rgb = 2.0f * tex.rgb * i.color.rgb * _TintColor.rgb * _ColorMultiplier + combinedColor;
					//col.a = 2.0f * tex.a * i.color.a * _TintColor.a + _RimColor.a;
					//col.a = col.a - (_AlphaCutout *(1 - col.a));
					return col ;
				}
				
				ENDCG
			}
		} 	
	
	}
	CustomEditor "CustomQueueAndGlowMatInspector"
}