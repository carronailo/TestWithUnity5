#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "ARPG Project/Particles/Water Sine Vertex Wave" {
	Properties {
		_MainTex ("Base layer (RGB)", 2D) = "white" {}
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
		_Color("Color", Color) = (1,1,1,1)
	
		_MMultiplier ("Layer Multiplier", Float) = 2.0

		// vertex wave
		_GerstnerIntensity("Per vertex displacement", Float) = 1.0
		_GAmplitude ("Wave Amplitude", Vector) = (0.3 ,0.35, 0.25, 0.25)
		_GFrequency ("Wave Frequency", Vector) = (1.3, 1.35, 1.25, 1.25)
		_GSteepness ("Wave Steepness", Vector) = (1.0, 1.0, 1.0, 1.0)
		_GSpeed ("Wave Speed", Vector) = (1.2, 1.375, 1.1, 1.5)
		_GDirectionAB ("Wave Direction", Vector) = (0.3 ,0.85, 0.85, 0.25)
		_GDirectionCD ("Wave Direction", Vector) = (0.1 ,0.9, 0.5, 0.5)	

		_BumpMap ("Normals ", 2D) = "bump" {}
		_BumpTiling ("Bump Tiling", Vector) = (1.0 ,1.0, -2.0, 3.0)
		_BumpDirection ("Bump Direction & Speed", Vector) = (1.0 ,1.0, -1.0, 1.0)
		_DistortParams ("Distortions (Bump waves, Reflection, Fresnel power, Fresnel bias)", Vector) = (1.0 ,1.0, 2.0, 1.15)
		_FresnelScale ("FresnelScale", Range (0.15, 4.0)) = 0.75	
		_ReflectionColor ("Reflection color", COLOR)  = ( .54, .95, .99, 0.5)	
		_SpecularColor ("Specular color", COLOR)  = ( .72, .72, .72, 1)
	}

	
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	
		Blend SrcAlpha One
		Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	
		LOD 100
	
		CGINCLUDE
		#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
		//#pragma exclude_renderers molehill    
		#include "UnityCG.cginc"
		sampler2D _MainTex;
		sampler2D _DetailTex;

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

		uniform half _GerstnerIntensity;
		uniform float4 _GAmplitude;
		uniform float4 _GFrequency;
		uniform float4 _GSteepness; 									
		uniform float4 _GSpeed;					
		uniform float4 _GDirectionAB;		
		uniform float4 _GDirectionCD;

		sampler2D _BumpMap;

		uniform float4 _DistortParams;
		uniform float _FresnelScale;	
		uniform float4 _BumpTiling;
		uniform float4 _BumpDirection;
		uniform float4 _ReflectionColor;
		uniform float4 _SpecularColor;

		struct v2f {
			float4 pos : SV_POSITION;
			float4 uv : TEXCOORD0;
			//fixed4 color : TEXCOORD4;
			float4 normalInterpolator : TEXCOORD1;
			float4 viewInterpolator : TEXCOORD2; 	
			float4 bumpCoords : TEXCOORD3;
		};
	
		half3 GerstnerOffset4 (half2 xzVtx, half4 steepness, half4 amp, half4 freq, half4 speed, half4 dirAB, half4 dirCD) 
		{
			half3 offsets;
		
			half4 AB = steepness.xxyy * amp.xxyy * dirAB.xyzw;
			half4 CD = steepness.zzww * amp.zzww * dirCD.xyzw;
		
			half4 dotABCD = freq.xyzw * half4(dot(dirAB.xy, xzVtx), dot(dirAB.zw, xzVtx), dot(dirCD.xy, xzVtx), dot(dirCD.zw, xzVtx));
			half4 TIME = _Time.yyyy * speed;
		
			half4 COS = cos (dotABCD + TIME);
			half4 SIN = sin (dotABCD + TIME);
		
			offsets.x = dot(COS, half4(AB.xz, CD.xz));
			offsets.z = dot(COS, half4(AB.yw, CD.yw));
			offsets.y = dot(SIN, amp);

			return offsets;			
		}	

		half3 GerstnerNormal4 (half2 xzVtx, half4 amp, half4 freq, half4 speed, half4 dirAB, half4 dirCD) 
		{
			half3 nrml = half3(0,2.0,0);
		
			half4 AB = freq.xxyy * amp.xxyy * dirAB.xyzw;
			half4 CD = freq.zzww * amp.zzww * dirCD.xyzw;
		
			half4 dotABCD = freq.xyzw * half4(dot(dirAB.xy, xzVtx), dot(dirAB.zw, xzVtx), dot(dirCD.xy, xzVtx), dot(dirCD.zw, xzVtx));
			half4 TIME = _Time.yyyy * speed;
		
			half4 COS = cos (dotABCD + TIME);
		
			nrml.x -= dot(COS, half4(AB.xz, CD.xz));
			nrml.z -= dot(COS, half4(AB.yw, CD.yw));
		
			nrml.xz *= _GerstnerIntensity;
			nrml = normalize (nrml);

			return nrml;			
		}	

		void Gerstner (	out half3 offs, out half3 nrml,
					half3 vtx, half3 tileableVtx, 
					half4 amplitude, half4 frequency, half4 steepness, 
					half4 speed, half4 directionAB, half4 directionCD ) 
		{
			offs = GerstnerOffset4(tileableVtx.xz, steepness, amplitude, frequency, speed, directionAB, directionCD);
			nrml = GerstnerNormal4(tileableVtx.xz + offs.xz, amplitude, frequency, speed, directionAB, directionCD);		
		}

		v2f vert (appdata_full v)
		{
			half3 worldSpaceVertex = mul(unity_ObjectToWorld,(v.vertex)).xyz;
			half3 vtxForAni = (worldSpaceVertex).xzz * 1.0; 		
			half3 nrml;
			half3 offsets;
			Gerstner (
				offsets, nrml, v.vertex.xyz, vtxForAni, 					// offsets, nrml will be written
				_GAmplitude,					 							// amplitude
				_GFrequency,				 								// frequency
				_GSteepness, 												// steepness
				_GSpeed,													// speed
				_GDirectionAB,												// direction # 1, 2
				_GDirectionCD												// direction # 3, 4
			);
			v.vertex.xyz += offsets;		

			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);

			o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex);// + frac(float2(_ScrollX, _ScrollY) * _Time);
			o.uv.zw = TRANSFORM_TEX(v.texcoord.xy,_DetailTex);// + frac(float2(_Scroll2X, _Scroll2Y) * _Time);
		
//			o.uv.x += sin(_Time * _SineFreqX) * _SineAmplX;
//			o.uv.y += sin(_Time * _SineFreqY) * _SineAmplY;
		
//			o.uv.z += sin(_Time * _SineFreqX2) * _SineAmplX2;
//			o.uv.w += sin(_Time * _SineFreqY2) * _SineAmplY2;
		
			//o.color = _MMultiplier * _Color * v.color;

			half2 tileableUv = worldSpaceVertex.xz;					
			o.bumpCoords.xyzw = (tileableUv.xyxy + _Time.xxxx * _BumpDirection.xyzw) * _BumpTiling.xyzw;	
			o.viewInterpolator.xyz = worldSpaceVertex - _WorldSpaceCameraPos;
			o.normalInterpolator.xyz = nrml;
			o.normalInterpolator.w = 1;//GetDistanceFadeout(o.screenPos.w, DISTANCE_SCALE); 

			return o;
		}

		ENDCG


		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest		

			// shortcuts
			#define PER_PIXEL_DISPLACE _DistortParams.x
			#define REALTIME_DISTORTION _DistortParams.y
			#define FRESNEL_POWER _DistortParams.z
			#define VERTEX_WORLD_NORMAL i.normalInterpolator.xyz
			#define FRESNEL_BIAS _DistortParams.w
			#define NORMAL_DISPLACEMENT_PER_VERTEX _InvFadeParemeter.z

			half3 PerPixelNormal(sampler2D bumpMap, half4 coords, half3 vertexNormal, half bumpStrength) 
			{
				half4 bump = tex2D(bumpMap, coords.xy) + tex2D(bumpMap, coords.zw);
				bump.xy = bump.wy - half2(1.0, 1.0);
				half3 worldNormal = /*vertexNormal + */bump.xxy * bumpStrength * half3(1,0,1);
				return normalize(worldNormal);
			} 

			half3 PerPixelNormalUnpacked(sampler2D bumpMap, half4 coords, half bumpStrength) 
			{
				half4 bump = tex2D(bumpMap, coords.xy) + tex2D(bumpMap, coords.zw);
				bump = bump * 0.5;
				half3 normal = UnpackNormal(bump);
				normal.xy *= bumpStrength;
				return normalize(normal);
			} 

			half Fresnel(half3 viewVector, half3 worldNormal, half bias, half power)
			{
				half facing =  clamp(1.0-max(dot(-viewVector, worldNormal), 0.0), 0.0,1.0);	
				half refl2Refr = saturate(bias+(1.0-bias) * pow(facing,power));	
				return refl2Refr;	
			}

			inline fixed3 UnpackNormalaaa(fixed4 packednormal)
			{
				return packednormal.xyz * 2 - 1;
			}

			fixed4 frag (v2f i) : COLOR
			{
				half3 worldNormal = normalize(UnpackNormalaaa((tex2D(_BumpMap, i.bumpCoords.xy) + tex2D(_BumpMap, i.bumpCoords.zw)) * 0.5f));//PerPixelNormal(_BumpMap, i.bumpCoords, normalize(VERTEX_WORLD_NORMAL), PER_PIXEL_DISPLACE);
				half3 viewVector = normalize(i.viewInterpolator.xyz);
				//worldNormal.xz *= _FresnelScale;		
				half refl2Refr = Fresnel(viewVector, worldNormal, FRESNEL_BIAS, FRESNEL_POWER);

				fixed4 o;
				fixed4 tex = tex2D (_MainTex, i.uv.xy);
				fixed4 tex2 = tex2D (_DetailTex, i.uv.zw);
			
				//o = tex * tex2 * i.color;
				o = tex * tex2 * _MMultiplier * _Color;
						
				half4 baseColor = o;
				baseColor = lerp (baseColor, _ReflectionColor, saturate(refl2Refr * 2.0));		
				return baseColor;
			}
			ENDCG 
		}	
	}
}
