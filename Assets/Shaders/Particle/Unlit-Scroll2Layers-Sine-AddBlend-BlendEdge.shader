// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "ARPG Project/Particles/Scroll 2 Layers Sine AddBlended (Soft Edge)" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DetailTex ("2nd layer (RGB)", 2D) = "white" {}
		_ScrollX ("Base layer Scroll speed X", Float) = 0
		_ScrollY ("Base layer Scroll speed Y", Float) = 0.0
		_Scroll2X ("2nd layer Scroll speed X", Float) = 0.0
		_Scroll2Y ("2nd layer Scroll speed Y", Float) = 0.0
		_SineAmplX ("Base layer sine amplitude X",Float) = 0
		_SineAmplY ("Base layer sine amplitude Y",Float) = 0
		_SineFreqX ("Base layer sine freq X",Float) = 0 
		_SineFreqY ("Base layer sine freq Y",Float) = 0
		_SineAmplX2 ("2nd layer sine amplitude X",Float) = 0 
		_SineAmplY2 ("2nd layer sine amplitude Y",Float) = 0
		_SineFreqX2 ("2nd layer sine freq X",Float) = 0 
		_SineFreqY2 ("2nd layer sine freq Y",Float) = 0
		_Color ("Color", Color) = (1,1,1,1)

		_MMultiplier ("Color Multiplier", Float) = 1

		_SoftEdge ("Soft Edge", Float) = 0.8
	}
	SubShader {
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType"="Transparent" }
		Pass {
			Cull Off
			Blend SrcAlpha One
			ZWrite Off
			ZTest LEqual
			Lighting Off
			Fog { Color (0,0,0,0) }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			sampler2D _DetailTex;
			sampler2D _CameraDepthTexture;

			float4 _MainTex_ST;
			float4 _DetailTex_ST;
	
			float _ScrollX;
			float _ScrollY;
			float _Scroll2X;
			float _Scroll2Y;
			float _MMultiplier;
	
			float _SineAmplX;
			float _SineAmplY;
			float _SineFreqX;
			float _SineFreqY;

			float _SineAmplX2;
			float _SineAmplY2;
			float _SineFreqX2;
			float _SineFreqY2;
			float4 _Color;

			fixed _SoftEdge;

			struct v2f 
			{
			    float4 pos : SV_POSITION;
			    float4 uv : TEXCOORD0;
			    float4 screenPos : TEXCOORD1;
				float4 color : TEXCOORD2;
			};
			
			v2f vert (appdata_full v)
			{
			    v2f o;			    		
			    o.pos = UnityObjectToClipPos( v.vertex );

				o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex) + frac(float2(_ScrollX, _ScrollY) * _Time);
				o.uv.zw = TRANSFORM_TEX(v.texcoord.xy,_DetailTex) + frac(float2(_Scroll2X, _Scroll2Y) * _Time);
		
				o.uv.x += sin(_Time * _SineFreqX) * _SineAmplX;
				o.uv.y += sin(_Time * _SineFreqY) * _SineAmplY;
		
				o.uv.z += sin(_Time * _SineFreqX2) * _SineAmplX2;
				o.uv.w += sin(_Time * _SineFreqY2) * _SineAmplY2;
								
				o.screenPos = ComputeScreenPos(o.pos);
				COMPUTE_EYEDEPTH(o.screenPos.z);
								
				o.color = _MMultiplier * _Color * v.color;

			    return o;
			}
			
			
			fixed4 frag( v2f i ) : COLOR
			{
				fixed4 c;
				fixed4 tex = tex2D (_MainTex, i.uv.xy);
				fixed4 tex2 = tex2D (_DetailTex, i.uv.zw);
				
				c = tex * tex2 * i.color;

				// Soft Edges
				half depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos));
				depth = LinearEyeDepth(depth);
				fixed diff = saturate((depth - i.screenPos.z) * _SoftEdge);				
				c.a *= diff;

			    return c;
			}
			
			ENDCG
		}
	} 
	Fallback "ARPG Project/Particles/Scroll 2 Layers Sine AddBlended"
}
