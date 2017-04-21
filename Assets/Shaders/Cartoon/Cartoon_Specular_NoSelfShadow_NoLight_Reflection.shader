// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Toony Gooch Pro+Mobile Shaders
// (c) 2013, Jean Moreno

Shader "Cartoon/Specular_NoSelfShadow_NoLight_Reflection"
{
	Properties
	{
		_Color ("Main Color", Color) = (0.8,0.8,0.8,1)
		_MainTex ("Base (RGB) Gloss (A) ", 2D) = "white" {}
		_Multiply("Multiply", Range(0, 4)) = 1
		_Saturation("Saturation", Range(0, 5)) = 1

		//SPECULAR
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_SpecTex ("Specular / Reflection Mask", 2D) = "white" {}
		_SpecularPower ("Specular Power", Float) = 20
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125

		//REFLECT
		_ReflectCubeMap ("Reflect Map (CUBE)", CUBE) = "" {}
		_ReflectPower ("Reflect Power", Range(0.1, 1)) = 0.1
		_Reflect2DMap ("Reflect Map (2D)", 2D) = "white" {}
		_Blur ("Blur Factor", Range(0,10)) = 0.1
	}
	
	SubShader
	{
		Tags { "Queue"="Geometry" "RenderType"="Opaque" }
		LOD 200
		ColorMask RGB
		//Blend SrcAlpha One
		
		// ---- forward rendering base pass:
		Pass {
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }

			Lighting Off

			CGPROGRAM
		
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fwdbase nolightmap nodirlightmap

			#define UNITY_PASS_FORWARDBASE
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
		
			sampler2D _MainTex;
			sampler2D _SpecTex;
			samplerCUBE _ReflectCubeMap;
			sampler2D _Reflect2DMap;

			uniform fixed4 _Color;

			uniform fixed _Multiply;
			uniform fixed _Saturation;
			uniform fixed _Shininess;
			uniform fixed _SpecularPower;
			uniform fixed _ReflectPower;
			uniform fixed _Blur;

			struct v2f {
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
				fixed3 normal : TEXCOORD1;
				fixed3 halfDir : TEXCOORD2;
				fixed3 reflectDir : TEXCOORD3;
			};

			uniform float4 _MainTex_ST;
			uniform half4 _ReflectCubeMap_TexelSize;

			v2f vert (appdata_full v) {
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				float3 worldN = normalize(mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL));
				o.normal = worldN;
				float3 viewDir = normalize(WorldSpaceViewDir( v.vertex ));
				// use viewdir as lightdir
				//float3 worldLightDir = normalize(WorldSpaceLightDir(v.vertex));
				float3 worldLightDir = viewDir;
				o.halfDir = normalize (worldLightDir + viewDir);
				o.reflectDir = reflect(-viewDir, worldN);
				return o;
			}

			fixed4 frag (v2f IN) : SV_Target {
				half4 c = tex2D(_MainTex, IN.uv);
				half4 sp = tex2D(_SpecTex, IN.uv);

				fixed3 combinedColor;

				combinedColor = c.rgb * _Color * _Multiply;

				//Specular
				// viewDir as halfDir
				fixed ndh = max (0, dot (IN.normal, IN.halfDir));
				// Use the eye vector as the light vector
				// Use Cube map
				fixed4 reflectColor = texCUBE (_ReflectCubeMap, IN.reflectDir);
				//reflectColor += texCUBE (_ReflectCubeMap, IN.reflectDir + _ReflectCubeMap_TexelSize.xyz * half3(-_Blur, -_Blur, -_Blur));
				//reflectColor += texCUBE (_ReflectCubeMap, IN.reflectDir + _ReflectCubeMap_TexelSize.xyz * half3(_Blur, -_Blur, _Blur));
				//reflectColor += texCUBE (_ReflectCubeMap, IN.reflectDir + _ReflectCubeMap_TexelSize.xyz * half3(-_Blur, _Blur, -_Blur));
				//reflectColor /= 4;
				// Use 2D map
				//fixed4 reflectColor = tex2D(_Reflect2DMap, (IN.reflectDir.xy + fixed2(1.0, 1.0)) * 0.5);
				//fixed3 specularColor = _SpecColor.rgb * pow(ndh, _Shininess) * sp.a * reflectColor.rgb * _SpecColor.a;
				fixed4 specularColor = _SpecColor * pow(ndh, _Shininess) * sp.a;
				combinedColor += specularColor.rgb;

				//reflectColor *= _SpecColor.a;

				//combinedColor *= reflectColor.rgb;
				combinedColor += reflectColor.rgb * specularColor.a * _ReflectPower;

	/*			// Reflection
				fixed3 reflectVector = reflect(-viewDir, s.Normal);
				//float3_t reflectVector = reflect( -i.eyeDir.xyz, normalVec ).xzy;
				//float2_t sphereMapCoords = 0.5 * ( float2_t( 1.0, 1.0 ) + reflectVector.xy );
				//float3_t reflectColor = tex2D( _EnvMapSampler, sphereMapCoords ).rgb;
				fixed3 reflectColor = texCUBE(_ReflectTex, reflectVector).rgb;
				//reflectColor *= combinedColor;
				//reflectColor = lerp(combinedColor, reflectColor, s.Gloss);
				reflectColor = GetOverlayColor( reflectColor, combinedColor );

				//combinedColor = reflectColor;
				combinedColor = lerp( combinedColor, reflectColor, saturate(s.Gloss * 2) );
		*/
				
/*				fixed4 reflectColor = texCUBE (_ReflectCubeMap, IN.reflectDir);
				reflectColor += texCUBE (_ReflectCubeMap, IN.reflectDir + _ReflectCubeMap_TexelSize.xyz * half3(-10h, -10h, 0h));
				reflectColor += texCUBE (_ReflectCubeMap, IN.reflectDir + _ReflectCubeMap_TexelSize.xyz * half3(10h, -10h, 0h));
				reflectColor += texCUBE (_ReflectCubeMap, IN.reflectDir + _ReflectCubeMap_TexelSize.xyz * half3(-10h, 10h, 0h));
				reflectColor /= 4;
				combinedColor = lerp( combinedColor, reflectColor, saturate(sp.a * 2) );
				*/

				// Saturation
				fixed lum = Luminance(combinedColor.rgb);
				combinedColor.rgb = lerp(fixed3(lum,lum,lum), combinedColor.rgb, _Saturation);

				return fixed4(combinedColor, 1);
			}

			// Overlay blend
			fixed3 GetOverlayColor( fixed3 inUpper, fixed3 inLower )
			{
				fixed lum = Luminance(inLower);
				fixed3 result = 0;
				//if(lum < 0.5)
					//result = 2.0 * inUpper * inLower;
				//else
					result = 1.0 - 2.0 * (1.0 - inUpper) * (1.0 - inLower);
				return result;
			}

			ENDCG
		}

//		UsePass "Hidden/Cartoon_Outlightting/OUTLIGHTTING_REVERSE"

//		UsePass "Hidden/Cartoon_Outline/OUTLINE_Z"

		Pass 
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
		
			Fog {Mode Off}
			ZWrite On ZTest LEqual Cull Off
			Offset 1, 1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"

			struct v2f { 
				V2F_SHADOW_CASTER;
			};

			v2f vert( appdata_base v )
			{
				v2f o;
				TRANSFER_SHADOW_CASTER(o)
				return o;
			}

			float4 frag( v2f i ) : SV_Target
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	

	}
	
	//Fallback "Mobile/VertexLit"
}
