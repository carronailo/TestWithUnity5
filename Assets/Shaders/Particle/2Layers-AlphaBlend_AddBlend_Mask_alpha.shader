// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "ARPG Project/Particles/2LayersAdd-AlphaBlend_Mask" {
Properties {
	_MainTex ("Base layer (RGB)", 2D) = "white" {}
	_DetailTex ("2nd layer (RGB)", 2D) = "white" {}
	_MaskTex ("Mask layer (RGB)", 2D) = "white" {}
	
	_ScrollX ("Base layer Scroll speed X", Float) = 0
	_ScrollY ("Base layer Scroll speed Y", Float) = 0.0
	_Scroll2X ("2nd layer Scroll speed X", Float) = 0.0
	_Scroll2Y ("2nd layer Scroll speed Y", Float) = 0.0
	_SineAmplX ("Base layer sine amplitude X",Float) = 0.0 
	_SineAmplY ("Base layer sine amplitude Y",Float) = 0.0
	_SineFreqX ("Base layer sine freq X",Float) = 0 
	_SineFreqY ("Base layer sine freq Y",Float) = 0
	_SineAmplX2 ("2nd layer sine amplitude X",Float) = 0.0 
	_SineAmplY2 ("2nd layer sine amplitude Y",Float) = 0.0
	_SineFreqX2 ("2nd layer sine freq X",Float) = 0 
	_SineFreqY2 ("2nd layer sine freq Y",Float) = 0
	_ColorA("ColorA", Color) = (1,1,1,1)
	_ColorB("ColorB", Color) = (1,1,1,0.5)
	//_ColorM("ColorM", Color) = (0.5,0.5,0.5,0)
	_AMultiplier("mask Multiplier", Float) = 1.0
	_MMultiplier ("Layer Multiplier", Float) = 2.0
}

	
SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	
	Blend SrcAlpha OneMinusSrcAlpha
	//Blend DstColor SrcAlpha
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	
	LOD 100
	
	
	
	CGINCLUDE
	#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
	#include "UnityCG.cginc"
	sampler2D _MainTex;
	sampler2D _DetailTex;
	sampler2D _MaskTex;

	float4 _MainTex_ST;
	float4 _DetailTex_ST;
	float4 _MaskTex_ST;
	
	float _ScrollX;
	float _ScrollY;
	float _Scroll2X;
	float _Scroll2Y;
	float _Scroll3X;
	
	float _MMultiplier;
	float _AMultiplier;
	
	float _SineAmplX;
	float _SineAmplY;
	float _SineFreqX;
	float _SineFreqY;

	float _SineAmplX2;
	float _SineAmplY2;
	float _SineFreqX2;
	float _SineFreqY2;
	
	float4 _ColorA;
	float4 _ColorB;
	float4 _ColorM;

	uniform float _Clip = 0;
	uniform float4 _ClipRange0;// = float4(0.0, 0.0, 1.0, 1.0);
	uniform float2 _ClipArgs0;// = float2(1000.0, 1000.0);

	struct v2f {
		float4 pos : SV_POSITION;
		float4 uv : TEXCOORD0;
		fixed4 color : TEXCOORD1;
		float2 mask : TEXCOORD2;
		float2 worldPos : TEXCOORD3;
	};

	
	v2f vert (appdata_full v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex) + frac(float2(_ScrollX, _ScrollY) * _Time);
		o.uv.zw = TRANSFORM_TEX(v.texcoord.xy,_DetailTex) + frac(float2(_Scroll2X, _Scroll2Y) * _Time);
		
		o.uv.x += sin(_Time * _SineFreqX) * _SineAmplX;
		o.uv.y += sin(_Time * _SineFreqY) * _SineAmplY;
		
		o.uv.z += sin(_Time * _SineFreqX2) * _SineAmplX2;
		o.uv.w += sin(_Time * _SineFreqY2) * _SineAmplY2;
		
		o.color = v.color * _MMultiplier;
		
		o.mask = TRANSFORM_TEX(v.texcoord.xy,_MaskTex);
	
		o.worldPos = mul(unity_ObjectToWorld, v.vertex).xy;
		if (_Clip > 0.5)
			o.worldPos = o.worldPos * _ClipRange0.zw + _ClipRange0.xy;
		return o;
	}
	ENDCG


	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
//		#pragma fragmentoption ARB_precision_hint_fastest		
		fixed4 frag (v2f i) : COLOR
		{
			fixed4 o;
			fixed4 tex = tex2D (_MainTex, i.uv.xy);
			fixed4 tex2 = tex2D (_DetailTex, i.uv.zw);
			fixed4 mask = tex2D(_MaskTex,i.mask);
					
			//o = ( tex * _ColorA  + tex2 * _ColorB );
			o =  tex * _ColorA  + tex2 * _ColorB ;
			 
			o = o * i.color;
			
			//o.a = o.a -( mask.a * mask * _ColorM.a * _AMultiplier);
			//o.a = (  mask * _ColorM * _AMultiplier) - o.a;
			//o.a =  (mask.a - o.a) * _AMultiplier;
			o.a =  (mask.a * o.a)* _AMultiplier;
			// ui clipping
			if (_Clip > 0.5)
			{
				float2 factor = (float2(1.0, 1.0) - abs(i.worldPos)) * _ClipArgs0;
				o.a *= clamp(min(factor.x, factor.y), 0.0, 1.0);
			}
			return o;
		}
		ENDCG 
	}	
}
CustomEditor "CustomQueueMatInspector"

}
