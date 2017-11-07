// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Toony Gooch Pro+Mobile Shaders
// (c) 2013, Jean Moreno

Shader "Hidden/Cartoon_Outline"
{
	Properties
	{
		//OUTLINE
		_OutlineSwitch ("Use Outline", Range(-1, 1)) = -1
		_Outline ("Outline Width", Float) = 1.0
		_OutlineColor ("Outline Color", Color) = (0.2, 0.2, 0.2, 1)
		
		//Z CORRECT
		_ZSmooth ("Z Correction", Range(-3.0,3.0)) = -0.5
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		//Outline default
		Pass
		{
			Name "OUTLINE"
			
			Cull Front
			Lighting Off
			ZWrite Off
			Tags { "LightMode"="ForwardBase" }
			
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
			};
			
			uniform fixed _OutlineSwitch;
			uniform fixed _Outline;
			uniform sampler2D _MainTex;
			uniform fixed4 _OutlineColor;
			uniform float4 _MainTex_ST;
			
			v2f vert (a2v v)
			{
				v2f o = (v2f)0;

				if(_OutlineSwitch <= 0.0)
				{
					return o;
				}
				else
				{
				#ifdef SMOOTH_Z_ARTEFACTS
					//Correct Z artefacts
					float4 pos = mul( UNITY_MATRIX_MV, v.vertex);
					float3 normal = mul( (float3x3)UNITY_MATRIX_IT_MV, v.normal);
					normal.z = -0.6;
				
					//Camera-independent size
					#ifdef OUTLINE_CONST_SIZE
						float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
						pos = pos + float4(normalize(normal),0) * _Outline * dist;
					#else
						pos = pos + float4(normalize(normal),0) * _Outline;
					#endif
				
				#else
					#ifdef OUTLINE_CONST_SIZE
						float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
						float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(v.normal,0) * _Outline * dist);
					#else
						float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(v.normal,0) * _Outline);
					#endif
				#endif
				
					o.pos = mul(UNITY_MATRIX_P, pos);
					return o;
				}
			}
			
			fixed4 frag (v2f IN) : COLOR
			{
				if(_OutlineSwitch <= 0.0)
				{
					return fixed4(0, 0, 0, 0);
				}
				else
				{
					return _OutlineColor;
				}
			}
		ENDCG
		}
		
		//Outline Const Size
		Pass
		{
			Name "OUTLINE_CONST"
			
			Cull Front
			Lighting Off
			ZWrite Off
			Tags { "LightMode"="ForwardBase" }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#define OUTLINE_CONST_SIZE
			
			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			}; 
			
			struct v2f
			{
				float4 pos : POSITION;
			};
			
			uniform fixed _OutlineSwitch;
			uniform fixed _Outline;
			uniform sampler2D _MainTex;
			uniform fixed4 _OutlineColor;
			uniform float4 _MainTex_ST;
			
			v2f vert (a2v v)
			{
				v2f o = (v2f)0;
				
				if(_OutlineSwitch <= 0.0)
				{
					return o;
				}
				else
				{
				#ifdef SMOOTH_Z_ARTEFACTS
					//Correct Z artefacts
					float4 pos = mul( UNITY_MATRIX_MV, v.vertex);
					float3 normal = mul( (float3x3)UNITY_MATRIX_IT_MV, v.normal);
					normal.z = -1;
				
					//Camera-independent size
					#ifdef OUTLINE_CONST_SIZE
						float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
						pos = pos + float4(normalize(normal),0) * _Outline * dist;
					#else
						pos = pos + float4(normalize(normal),0) * _Outline;
					#endif
				
				#else
					#ifdef OUTLINE_CONST_SIZE
						float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
						float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(v.normal,0) * _Outline * dist);
					#else
						float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(v.normal,0) * _Outline);
					#endif
				#endif
				
					o.pos = mul(UNITY_MATRIX_P, pos);
					return o;
				}
			}
			
			fixed4 frag (v2f IN) : COLOR
			{
				if(_OutlineSwitch <= 0.0)
				{
					return fixed4(0, 0, 0, 0);
				}
				else
				{
					return _OutlineColor;
				}
			}
		ENDCG
		}
		
		//Outline - Z Correct
		Pass
		{
			Name "OUTLINE_Z"
			
			Cull Front
			Lighting Off
			ZWrite Off
			Tags { "LightMode"="ForwardBase" }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#define SMOOTH_Z_ARTEFACTS
			
			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			}; 
			
			struct v2f
			{
				float4 pos : POSITION;
			};
			
			uniform fixed _OutlineSwitch;
			uniform fixed _Outline;
			uniform sampler2D _MainTex;
			uniform fixed4 _OutlineColor;
			uniform float4 _MainTex_ST;
			
			#ifdef SMOOTH_Z_ARTEFACTS
				fixed _ZSmooth;
			#endif
			
			v2f vert (a2v v)
			{
				v2f o = (v2f)0;
				
				if(_OutlineSwitch <= 0.0)
				{
					return o;
				}
				else
				{
				#ifdef SMOOTH_Z_ARTEFACTS
					//Correct Z artefacts
					float4 pos = mul( UNITY_MATRIX_MV, v.vertex);
					float3 normal = mul( (float3x3)UNITY_MATRIX_IT_MV, v.normal);
					normal.z = _ZSmooth;
				
					//Camera-independent size
					#ifdef OUTLINE_CONST_SIZE
						float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
						pos = pos + float4(normalize(normal),0) * _Outline * dist;
					#else
						pos = pos + float4(normalize(normal),0) * _Outline;
					#endif
				
				#else
					#ifdef OUTLINE_CONST_SIZE
						float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
						float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(v.normal,0) * _Outline * dist);
					#else
						float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(v.normal,0) * _Outline);
					#endif
				#endif
				
					o.pos = mul(UNITY_MATRIX_P, pos);
					return o;
				}
			}
			
			fixed4 frag (v2f IN) : COLOR
			{
				if(_OutlineSwitch <= 0.0)
				{
					return fixed4(0, 0, 0, 0);
				}
				else
				{
					return _OutlineColor;
				}
			}
		ENDCG
		}
		
		//Outline - Z Correct
		Pass
		{
			Name "OUTLINE_Z_REVERSE"
			
			Cull Back
			Lighting Off
			ZWrite Off
			Tags { "LightMode"="ForwardBase" }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#define SMOOTH_Z_ARTEFACTS
			
			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			}; 
			
			struct v2f
			{
				float4 pos : POSITION;
			};
			
			uniform fixed _OutlineSwitch;
			uniform fixed _Outline;
			uniform sampler2D _MainTex;
			uniform fixed4 _OutlineColor;
			uniform float4 _MainTex_ST;
			
			#ifdef SMOOTH_Z_ARTEFACTS
				fixed _ZSmooth;
			#endif
			
			v2f vert (a2v v)
			{
				v2f o = (v2f)0;
				
				if(_OutlineSwitch <= 0.0)
				{
					return o;
				}
				else
				{
				#ifdef SMOOTH_Z_ARTEFACTS
					//Correct Z artefacts
					float4 pos = mul( UNITY_MATRIX_MV, v.vertex);
					float3 normal = mul( (float3x3)UNITY_MATRIX_IT_MV, v.normal);
					normal.z = _ZSmooth;
				
					//Camera-independent size
					#ifdef OUTLINE_CONST_SIZE
						float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
						pos = pos + float4(normalize(normal),0) * _Outline * dist;
					#else
						pos = pos + float4(normalize(normal),0) * _Outline;
					#endif
				
				#else
					#ifdef OUTLINE_CONST_SIZE
						float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
						float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(v.normal,0) * _Outline * dist);
					#else
						float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(v.normal,0) * _Outline);
					#endif
				#endif
				
					o.pos = mul(UNITY_MATRIX_P, pos);
					return o;
				}
			}
			
			fixed4 frag (v2f IN) : COLOR
			{
				if(_OutlineSwitch <= 0.0)
				{
					return fixed4(0, 0, 0, 0);
				}
				else
				{
					return _OutlineColor;
				}
			}
		ENDCG
		}

		//Outline Const Size - Z Correct
		Pass
		{
			Name "OUTLINE_CONST_Z"
			
			Cull Front
			Lighting Off
			ZWrite Off
			Tags { "LightMode"="ForwardBase" }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#define OUTLINE_CONST_SIZE
			#define SMOOTH_Z_ARTEFACTS
			
			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			
			struct v2f
			{
				float4 pos : POSITION;
			};
			
			uniform fixed _OutlineSwitch;
			uniform fixed _Outline;
			uniform sampler2D _MainTex;
			uniform fixed4 _OutlineColor;
			uniform float4 _MainTex_ST;
			
			#ifdef SMOOTH_Z_ARTEFACTS
				fixed _ZSmooth;
			#endif
			
			v2f vert (a2v v)
			{
				v2f o = (v2f)0;
				
				if(_OutlineSwitch <= 0.0)
				{
					return o;
				}
				else
				{
				#ifdef SMOOTH_Z_ARTEFACTS
					//Correct Z artefacts
					float4 pos = mul( UNITY_MATRIX_MV, v.vertex);
					float3 normal = mul( (float3x3)UNITY_MATRIX_IT_MV, v.normal);
					normal.z = _ZSmooth;
				
					//Camera-independent size
					#ifdef OUTLINE_CONST_SIZE
						float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
						pos = pos + float4(normalize(normal),0) * _Outline * dist;
					#else
						pos = pos + float4(normalize(normal),0) * _Outline;
					#endif
				
				#else
					#ifdef OUTLINE_CONST_SIZE
						float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
						float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(v.normal,0) * _Outline * dist);
					#else
						float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(v.normal,0) * _Outline);
					#endif
				#endif
				
					o.pos = mul(UNITY_MATRIX_P, pos);
					return o;
				}
			}
			
			fixed4 frag (v2f IN) : COLOR
			{
				if(_OutlineSwitch <= 0.0)
				{
					return fixed4(0, 0, 0, 0);
				}
				else
				{
					return _OutlineColor;
				}
			}
		ENDCG
		}
	}
}
