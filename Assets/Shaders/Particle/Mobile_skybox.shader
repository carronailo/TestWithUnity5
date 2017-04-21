// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "ARPG Project/Particles/Skybox" {
	Properties {
		_MainTex ("Base layer (RGB)", 2D) = "white" {}
		_DetailTex ("2nd layer (RGB)", 2D) = "white" {}
		_ColorA("ColorA", Color) = (1,1,1,0.5)
		_ColorB("ColorB", Color) = (0,0,0,0)
	
		_MMultiplier ("Layer Multiplier", Float) = 1
		_AlphaCutout("Alpha Cut Out", Range(0, 1)) = 0
		_GlowColor ("Glow Color", Color) = (1, 1, 1, 1)
	}

	
	SubShader {
		Tags { "Queue"="Background" "IgnoreProjector"="True" "RenderType"="Background" }
	
		//Blend SrcAlpha OneMinusSrcAlpha
		Lighting Off ZWrite Off Fog {Mode Off}
	
		LOD 100
	
		CGINCLUDE
		#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
		//#pragma exclude_renderers molehill    
		#include "UnityCG.cginc"
		sampler2D _MainTex;
		sampler2D _DetailTex;

		float4 _MainTex_ST;
		float4 _DetailTex_ST;
	
		float _MMultiplier;
		float4 _ColorA;
		float4 _ColorB;
		fixed _AlphaCutout;

		uniform half4 _GlowColor;

		struct v2f {
			float4 pos : SV_POSITION;
			float4 uv : TEXCOORD0;
			fixed4 color : TEXCOORD1;
		};

	
		v2f vert (appdata_full v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex);
			o.uv.zw = TRANSFORM_TEX(v.texcoord.xy,_DetailTex);
			o.color = v.color * _MMultiplier;
	
			return o;
		}
		ENDCG


		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest		
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 o;
				fixed4 tex = tex2D (_MainTex, i.uv.xy);
				fixed4 tex2 = tex2D (_DetailTex, i.uv.zw);
					
				o =  tex * _ColorA + tex2 * _ColorB ;
				o = o * i.color;
				o.a = o.a > _AlphaCutout ? o.a : 0;
			
				return o;
			}
			ENDCG 
		}	
	}
}
