// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Cartoon_Outlightting" 
{
	Properties
	{
		//OUTLIGHTTING
		_OutlighttingSwitch("Use Outlightting", Range(-1, 1)) = -1
		_Outlightting ("Outlightting Size", Range(0,0.1)) = 0.005
		_OutlighttingColor ("Outlightting Color", Color) = (0.2, 0.2, 0.2, 1)
        //_Falloff("Falloff", Float) = 5
		//_Transparency("Transparency", Float) = 15
		//Z CORRECT
		_ZSmooth("Z Correction", Range(-3.0,3.0)) = -0.5
	}
	
	SubShader
	{
		Tags { "Queue"="Geometry" "RenderType"="Opaque" }
		LOD 200
		
		//Outline default
		Pass
		{
			Name "OUTLIGHTTING"
			Tags { "LightMode"="ForwardBase" }
			Cull Front
			Lighting Off
			ZWrite Off
            //Blend One Zero
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
//			#define SMOOTH_Z_ARTEFACTS
			
			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			}; 
			
			struct v2f
			{
				float4 pos : POSITION;
                float3 normal : TEXCOORD0;
                float3 worldvertpos : TEXCOORD1;
			};
			
			uniform fixed _OutlighttingSwitch;
			uniform float _Outlightting;
			uniform fixed4 _OutlighttingColor;
            //uniform float _Falloff;
            //uniform float _Transparency;
			uniform fixed _ZSmooth;

			v2f vert (a2v v)
			{
				v2f o = (v2f)0;

				if (_OutlighttingSwitch <= 0.0)
				{
					return o;
				}
				else
				{
					o.normal = v.normal;
					o.normal.z += _ZSmooth;
					v.vertex.xyz += o.normal * _Outlightting;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.normal = mul((float3x3)unity_ObjectToWorld, v.normal);
					//o.normal = v.normal;
					o.worldvertpos = mul(unity_ObjectToWorld, v.vertex);
					return o;
				}
			}
			
			float4 frag (v2f IN) : COLOR
			{
				if (_OutlighttingSwitch <= 0.0)
				{
					return fixed4(0, 0, 0, 0);
				}
				else
				{
					return _OutlighttingColor;
				}
				//IN.normal = normalize(IN.normal);
                //float3 viewdir = normalize(IN.worldvertpos-_WorldSpaceCameraPos);
                //float3 viewdir = normalize(_WorldSpaceCameraPos - IN.worldvertpos);
                   
                //float4 color = _OutlighttingColor;
                //color.a = pow(saturate(dot(viewdir, IN.normal)), _Falloff);
                //color.a *= _Transparency*_Color*dot(normalize(i.worldvertpos-_WorldSpaceLightPos0), i.normal);
                //color.a *= _Transparency*_Color;
				//color.a *= _Transparency;
                //return color;
			}
		ENDCG
		}

		Pass
		{
			Name "OUTLIGHTTING_REVERSE"
			Tags { "LightMode"="ForwardBase" }
			Cull Back
			Lighting Off
			ZWrite Off
            //Blend One Zero
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
//			#define SMOOTH_Z_ARTEFACTS
			
			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			}; 
			
			struct v2f
			{
				float4 pos : POSITION;
                float3 normal : TEXCOORD0;
                float3 worldvertpos : TEXCOORD1;
			};
			
			uniform fixed _OutlighttingSwitch;
			uniform float _Outlightting;
			uniform fixed4 _OutlighttingColor;
            //uniform float _Falloff;
            //uniform float _Transparency;
			uniform fixed _ZSmooth;

			v2f vert (a2v v)
			{
				v2f o = (v2f)0;

				if (_OutlighttingSwitch <= 0.0)
				{
					return o;
				}
				else
				{
					v.vertex.xyz += v.normal * _Outlightting;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.normal = mul((float3x3)unity_ObjectToWorld, v.normal);
					//o.normal = v.normal;
					o.worldvertpos = mul(unity_ObjectToWorld, v.vertex);
					return o;
				}
			}
			
			float4 frag (v2f IN) : COLOR
			{
				if (_OutlighttingSwitch <= 0.0)
				{
					return fixed4(0, 0, 0, 0);
				}
				else
				{
					return _OutlighttingColor;
				}
				//IN.normal = normalize(IN.normal);
                //float3 viewdir = normalize(IN.worldvertpos-_WorldSpaceCameraPos);
                //float3 viewdir = normalize(_WorldSpaceCameraPos - IN.worldvertpos);
                   
                //float4 color = _OutlighttingColor;
                //color.a = pow(saturate(dot(viewdir, IN.normal)), _Falloff);
                //color.a *= _Transparency*_Color*dot(normalize(i.worldvertpos-_WorldSpaceLightPos0), i.normal);
                //color.a *= _Transparency*_Color;
				//color.a *= _Transparency;
                //return color;
			}
		ENDCG
		}
	}
}
