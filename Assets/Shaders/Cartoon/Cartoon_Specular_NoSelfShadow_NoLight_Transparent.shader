// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Toony Gooch Pro+Mobile Shaders
// (c) 2013, Jean Moreno

Shader "Cartoon/Specular_NoSelfShadow_NoLight_Transparent"
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

		//BE HIT LIGHTING
		_BeHitLight("Be Hit Light Color", Color) = (1,1,1,0.5)
		_BeHitLightSequence("Be Hit Light Sequence", Float) = 0.1
		_BeHitSwitch("Be Hit Switch", Float) = 0
		//_GameTime("Game Time", Float) = 0
	}
	
	SubShader
	{
		Tags { "Queue"="Geometry" "RenderType"="Opaque" }
		LOD 200

		Blend SrcAlpha One
		
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

			uniform fixed4 _Color;
			uniform fixed4 _BeHitLight;

			uniform fixed _Multiply;
			uniform fixed _Saturation;
			uniform fixed _Shininess;
			uniform fixed _SpecularPower;

			uniform float _GameTime;
			uniform fixed _BeHitLightSequence;
			uniform fixed _BeHitSwitch;
			uniform fixed _LastBeHitTime;
	
			struct v2f {
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
				fixed3 normal : TEXCOORD1;
				fixed3 halfDir : TEXCOORD2;
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
				//float3 worldLightDir = normalize(WorldSpaceLightDir(v.vertex));
				// use viewdir as lightdir
				float3 worldLightDir = viewDir;
				o.halfDir = normalize (worldLightDir + viewDir);
				return o;
			}

			fixed4 frag (v2f IN) : SV_Target {
				half4 c = tex2D(_MainTex, IN.uv);
				half4 sp = tex2D(_SpecTex, IN.uv);

				fixed3 combinedColor;

				combinedColor = c.rgb * _Color * _Multiply;

				//Specular
				fixed ndh = max (0, dot (IN.normal, IN.halfDir));
				fixed3 specularColor = _SpecColor.rgb * pow(ndh, _Shininess) * sp.a;
				combinedColor += specularColor;

				// Saturation
				fixed lum = Luminance(combinedColor.rgb);
				combinedColor.rgb = lerp(fixed3(lum,lum,lum), combinedColor.rgb, _Saturation);

				// Be Hit Lighting
				fixed factor = max(0, _BeHitLightSequence - (_GameTime - _LastBeHitTime));
				if(factor > 0 && _BeHitSwitch > 0)
					combinedColor.rgb += _BeHitLight.rgb * _BeHitLight.a;

				return fixed4(combinedColor, c.a);
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
