// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// 2015.1.15 luoyinan
// 两张贴图实现的简单河水


Shader "luoyinan/water_river_simple" {
	
Properties {
	_NormalMap("Normal Map", 2D) = "bump" {}
	_ReflectionMap("Reflection Map",2D) = "white"{}

	_MainColor ("Base Color", Color) = (0.1137,0.4,0.31,1)

	_ReflectionDistortion("Reflection Distortion", float) = 0.5
	_ReflectionAlpha("Reflection Alpha", Range(0,1)) = 1.0

	_BumpDirection("Bump Direction & Speed", Vector) = (8.0, 8.0, 0, 0)
	_BumpTiling("Bump Tiling", Vector) = (10.0, 8.0, 0, 0)
}

CGINCLUDE	

#include "UnityCG.cginc"

sampler2D _NormalMap;
sampler2D _ReflectionMap;

fixed4 _MainColor;

half _ReflectionDistortion;
half _ReflectionAlpha;

half4 _BumpDirection;
half4 _BumpTiling;

struct v2f_full
{
	half4 pos : SV_POSITION;
	half4 uv : TEXCOORD0;
	half4 bumpCoords : TEXCOORD1;
};

ENDCG 

SubShader {
	Tags {"RenderType"="Transparent" "Queue"="Transparent"} 
	 
	LOD 200

	Pass {
		Blend SrcAlpha OneMinusSrcAlpha
		ZTest LEqual
		ZWrite Off
		//Cull Off

		CGPROGRAM
		
		v2f_full vert (appdata_full v) 
		{
			v2f_full o;
			
			o.pos = UnityObjectToClipPos (v.vertex);
			o.bumpCoords.xyzw = v.texcoord.xyxy * _BumpTiling + _Time.xxxx * _BumpDirection;
			o.uv = v.texcoord;
			
			return o; 
		}
				
		fixed4 frag (v2f_full i) : COLOR0
		{	
			// normal
			fixed4 normal_1 = tex2D(_NormalMap, i.bumpCoords.xy);
			//fixed4 normal_2 = tex2D(_NormalMap, i.bumpCoords.zw);
			//fixed4 bump = (normal_1 + normal_2) - 1.0; 
			fixed4 bump = normal_1 - 0.5; 

			// reflection
			fixed4 offset = bump * _ReflectionDistortion;
			fixed4 reflection = tex2D(_ReflectionMap ,i.uv.xy + offset.xy);
		
			// final color
			fixed4 baseColor = _MainColor + (reflection * _ReflectionAlpha);
			baseColor.a =  _MainColor.a;
			
			return baseColor;
		}	
		
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
	
		ENDCG
	}	
	
} 

FallBack "Diffuse"
}

