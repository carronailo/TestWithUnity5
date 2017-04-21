// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "ARPG Project/Particles/Additive 2 Textures" {
	Properties{
		_MainTex("Base layer (RGB)", 2D) = "white" {}
		_DetailTex("2nd layer (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,0.5)

		_quse("quse", Range(0, 1)) = 0
		_ColorC("Color", Color) = (1,1,1,0.5)
		_MMultiplier("Layer Multiplier", Float) = 1
		[MaterialToggle] _RampK("Ramp I/O", Float) = 0
		_Ramp("Ramp Texer", 2D) = "white" {}

		_AlphaCutout("Alpha Cut Out", Range(0, 1)) = 0
		_GlowTex("Glow", 2D) = "" {}
		_GlowColor("Glow Color", Color) = (1,1,1,1)
		_GlowStrength("Glow Strength", Float) = 0.0
	}


	SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Glow11Transparent" "RenderEffect" = "Glow11Transparent" }

		Blend SrcAlpha One
		Cull Off Lighting Off ZWrite Off Fog{ Color(0,0,0,0) }

		LOD 100



		CGINCLUDE
		#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
		//#pragma exclude_renderers molehill    
		#include "UnityCG.cginc"
		sampler2D _MainTex;
		sampler2D _DetailTex;
		sampler2D _Ramp;

		float4 _MainTex_ST;
		float4 _DetailTex_ST;
		float4 _Ramp_ST;

		float _MMultiplier;
		float4 _Color;
		float4 _ColorC;

		fixed _AlphaCutout;

		uniform half4 _GlowColor;
		uniform fixed _RampK;
		uniform float _quse;

		uniform float _Clip = 0;
		uniform float4 _ClipRange0;// = float4(0.0, 0.0, 1.0, 1.0);
		uniform float2 _ClipArgs0;// = float2(1000.0, 1000.0);

		struct v2f {
			float4 pos : SV_POSITION;
			float4 uv : TEXCOORD0;
			fixed4 color : TEXCOORD1;
			float2 worldPos : TEXCOORD2;
		};


		v2f vert(appdata_full v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex);
			o.uv.zw = TRANSFORM_TEX(v.texcoord.xy,_DetailTex);
			o.color = v.color * _MMultiplier;

			o.worldPos = mul(unity_ObjectToWorld, v.vertex).xy;
			if (_Clip > 0.5)
				o.worldPos = o.worldPos * _ClipRange0.zw + _ClipRange0.xy;
			return o;
		}
		ENDCG


		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest		
			fixed4 frag(v2f i) : COLOR
			{
				fixed4 col;
				fixed4 tex = tex2D(_MainTex, i.uv.xy);
				fixed4 tex2 = tex2D(_DetailTex, i.uv.zw);
				fixed4 tex3 = (tex *  tex2 * _Color);
				float3 quse = lerp(tex3.rgb, dot(tex3.rgb, float3(0.3, 0.59, 0.11)), _quse);

				fixed4 ramp = tex2D(_Ramp,TRANSFORM_TEX(quse,_Ramp));

				col.rgb = 2.0f * lerp(quse * _ColorC, quse.rgb * ramp * 3.0f, _RampK).rgb * i.color.rgb  * _MMultiplier;
				col.a = 2.0f * tex3.a * i.color.a;
				col.a = col.a > _AlphaCutout ? col.a : 0;
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
	CustomEditor "CustomQueueAndGlowMatInspector"
}
