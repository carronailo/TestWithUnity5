// Toony Gooch Pro+Mobile Shaders
// (c) 2013, Jean Moreno

Shader "Cartoon/Outline_Specular"
{
	Properties
	{
		_MainTex ("Base (RGB) Gloss (A) ", 2D) = "white" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		_EnvLightFactor("Environment Light Factor", Range(0, 1)) = 0

		//SPECULAR
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_SpecTex ("Specular / Reflection Mask", 2D) = "white" {}
		_SpecularPower ("Specular Power", Float) = 20
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_ReflectTex ("Reflect Map", CUBE) = "" {}

		//GOOCH
		_Color ("Highlight Color", Color) = (0.8,0.8,0.8,1)
		_SColor ("Shadow Color", Color) = (0.0,0.0,0.0,1)
		
		//OUTLINE
		_Outline ("Outline Width", Range(0,0.05)) = 0.005
		_OutlineColor ("Outline Color", Color) = (0.2, 0.2, 0.2, 1)
		_ZSmooth ("Z Correction", Range(-3.0,3.0)) = -0.5
	}
	
	SubShader
	{
		Tags { "Queue"="Geometry" "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		
		//#include "TGP_Include.cginc"
		
		//nolightmap nodirlightmap		LIGHTMAP
		//approxview halfasview			SPECULAR/VIEW DIR
		#pragma surface surf ToonyGoochSpecWithTex nolightmap nodirlightmap noforwardadd approxview halfasview exclude_path:prepass
		
		sampler2D _MainTex;
		sampler2D _Ramp;
		sampler2D _SpecTex;
		samplerCUBE _ReflectTex;

		fixed4 _Color;
		fixed4 _SColor;

		fixed _EnvLightFactor;
		fixed _Shininess;
		fixed _SpecularPower;
	
		struct Input
		{
			half2 uv_MainTex;
		};
		
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

		half4 LightingToonyGoochSpecWithTex (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			#ifndef USING_DIRECTIONAL_LIGHT
				lightDir = normalize(lightDir);
			#endif
	
			fixed3 combinedColor;

			//Ramp shading
			fixed ndl = dot(s.Normal, lightDir)*0.5 + 0.5;
			fixed3 ramp = tex2D(_Ramp, fixed2(ndl,ndl));
	
			//Gooch shading
			ramp = lerp(_SColor,_Color,ramp);

			fixed3 envLightColor = saturate(_LightColor0 + _EnvLightFactor);
			combinedColor = s.Albedo * envLightColor * ramp * (atten * 2);
			//combinedColor = s.Albedo * ramp * (atten * 2);

			//Specular
			// viewDir as halfDir, because of #pragma halfasview
			fixed ndh = max (0, dot (s.Normal, viewDir));
			// Use the eye vector as the light vector
			//float4_t reflectionMaskColor = tex2D( _SpecTex, i.uv.xy );
			fixed4 lighting = lit( ndl, ndh, s.Specular );
			fixed3 specularColor = saturate( lighting.z ) * _SpecColor.rgb * s.Gloss;
			//fixed3 specularColor = saturate( lighting.z ) * _SpecColor.rgb * s.Albedo.rgb * s.Gloss;
			//float3_t specularColor = saturate( lighting.z ) * diffSamplerColor.rgb;
			//combinedColor += specularColor;
	
			combinedColor += specularColor;

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
			//float opacity = s.Alpha * _LightColor0.a;
	
			return fixed4(combinedColor, 1);
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			
			//Specular
			half4 sp = tex2D(_SpecTex, IN.uv_MainTex);
			o.Gloss = sp.a;
			o.Specular = _Shininess;
		}
		ENDCG

		UsePass "Hidden/Cartoon_Outline/OUTLINE_Z"

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
