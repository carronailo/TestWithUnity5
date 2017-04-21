// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Cartoon_Base" 
{
	Properties 
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Gloss (A) ", 2D) = "white" { }
		_Multiply("Multiply", Range(0, 4)) = 1
		_Saturation("Saturation", Range(0, 5)) = 1

		//SPECULAR
		_SpecColor("Specular Color", Color) = (0, 0, 0, 1)
		_SpecTex("Specular / Reflection Mask", 2D) = "white" { }
		_SpecularPower("Specular Power", Float) = 20
		_Shininess("Shininess", Range(0.01, 1)) = 0.078125

		//BE HIT LIGHTING
		_BeHitColor("Be Hit Color", Color) = (0,0,0,0.5)

		//DISSOLVE
		_DissolveTex("Dissolve Texture(A)", 2D) = "white" { }
		_DissolveFactor("Dissolve Factor", Range(0, 1.01)) = 0

		//TRANSPARENT
		//_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.15)
		//_ColorMultiplier ("Color Multiplier", Float) = 1
		_AlphaCutoutSwitch("Use CutOut", Range(0, 1)) = 0
		_AlphaCutout("Alpha CutOut", Range(0, 1)) = 0
	}

	SubShader 
	{
		Tags{ "Queue" = "Geometry+2" "RenderType" = "Opaque" }
		LOD 300
		ColorMask RGB
		//Blend SrcAlpha OneMinusSrcAlpha

		// ---- forward rendering base pass:
		Pass
		{
			Name "FORWARD"
			Tags{"LightMode" = "ForwardBase" }

			//ZWrite On
			Lighting Off
			//Blend SrcAlpha OneMinusSrcAlpha

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
			sampler2D _DissolveTex;
			uniform fixed4 _Color;
			uniform fixed4 _BeHitColor;
			uniform fixed _Multiply;
			uniform fixed _Saturation;
			uniform fixed _Shininess;
			uniform fixed _SpecularPower;
			uniform fixed _DissolveFactor;
			uniform fixed _AlphaCutoutSwitch;
			uniform fixed _AlphaCutout;

			uniform float _Clip = 0;
			uniform float4 _ClipRange0;// = float4(0.0, 0.0, 1.0, 1.0);
			uniform float2 _ClipArgs0;// = float2(1000.0, 1000.0);

			struct v2f {
				float4 pos : SV_POSITION;
				half4 uv : TEXCOORD0;
				fixed3 normal : TEXCOORD1;
				fixed3 halfDir : TEXCOORD2;
				float2 worldPos : TEXCOORD3;
			};

			uniform float4 _MainTex_ST;
			uniform float4 _DissolveTex_ST;
			uniform half4 _ReflectCubeMap_TexelSize;

			v2f vert(appdata_full v) 
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.texcoord, _DissolveTex);
				fixed3 worldN = normalize(mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL));
				o.normal = worldN;
				fixed3 viewDir = normalize(WorldSpaceViewDir(v.vertex));
				//float3 worldLightDir = normalize(WorldSpaceLightDir(v.vertex));
				// use viewdir as lightdir
				fixed3 worldLightDir = viewDir;
				o.halfDir = normalize(worldLightDir + viewDir);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xy;
				if (_Clip > 0.5)
					o.worldPos = o.worldPos * _ClipRange0.zw + _ClipRange0.xy;
				return o;
			}


			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, IN.uv.xy);
				fixed4 sp = tex2D(_SpecTex, IN.uv.xy);
				fixed4 dis = tex2D(_DissolveTex, IN.uv.zw);
				fixed3 combinedColor;
				//combinedColor = c.rgb * _Color * _Multiply;
				// 主颜色暂时不参与运算
				combinedColor = c.rgb * _Multiply;
				//Specular
				fixed ndh = max(0, dot(IN.normal, IN.halfDir));
				fixed3 specularColor = _SpecColor.rgb * pow(ndh, _Shininess) * sp.a;
				combinedColor += specularColor;
				// Saturation
				fixed lum = Luminance(combinedColor.rgb);
				combinedColor.rgb = lerp(fixed3(lum,lum,lum), combinedColor.rgb, _Saturation);
				// Be Hit Lighting
				combinedColor.rgb += _BeHitColor.rgb * _BeHitColor.a;
				fixed a = 1;

				// ui clipping
				if (_Clip > 0.5)
				{
					float2 factor = (float2(1.0, 1.0) - abs(IN.worldPos)) * _ClipArgs0;
					float f = min(factor.x, factor.y);
					a *= clamp(f, 0.0, 1.0);
					if (a <= 0.1 || dis.a < _DissolveFactor || (_AlphaCutoutSwitch > 0 && c.a < _AlphaCutout))
						discard;
				}
				else
				{
					if (dis.a < _DissolveFactor || (_AlphaCutoutSwitch > 0 && c.a < _AlphaCutout))
						//a = 0;
						discard;
				}

				return fixed4(combinedColor, a);
			}

			// Overlay blend
			fixed3 GetOverlayColor(fixed3 inUpper, fixed3 inLower)
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

		/*		Pass
		{
		Name "ShadowCaster"
		Tags {
		"LightMode" = "ShadowCaster" }

		Fog {
		Mode Off}
		ZWrite On ZTest LEqual Cull Off
		Offset 1, 1

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_shadowcaster
		#include "UnityCG.cginc"

		struct v2f {
		V2F_SHADOW_CASTER;
		}
		;
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
		}*/

		/*		Pass
		{
		Name "FORWARD RIM"
		Tags { "LightMode" = "ForwardBase" }

		Lighting Off

		Blend SrcAlpha One

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma multi_compile_fwdbase nolightmap nodirlightmap

		#define UNITY_PASS_FORWARDBASE
		#include "UnityCG.cginc"

		uniform fixed4 _RimColor;
		uniform fixed _RimBase;
		uniform fixed _RimPower;
		uniform fixed _RimFrequency;

		struct v2f
		{
		float4 pos : SV_POSITION;
		fixed3 normal : TEXCOORD1;
		fixed3 viewDir : TEXCOORD2;
		};

		v2f vert (appdata_full v)
		{
		v2f o;
		o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		float3 worldN = normalize(mul((float3x3)_Object2World, SCALED_NORMAL));
		o.normal = worldN;
		o.viewDir = normalize(WorldSpaceViewDir( v.vertex ));
		return o;
		}

		fixed4 frag (v2f IN) : SV_Target
		{
		// Rim
		half rim = 1.0 - saturate(dot (IN.viewDir, IN.normal));
		fixed scale = (1.0 - _RimBase) * 0.5;
		half3 combinedColor = _RimColor.rgb * pow (rim, _RimPower) * (sin(_Time * _RimFrequency).w * scale + scale + _RimBase);

		return fixed4(combinedColor, _RimColor.a);
		}

		ENDCG
		}

		}*/

		/*	SubShader
		{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back Lighting Off ZWrite on Fog {Mode Off}

		Pass
		{
		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		fixed4 _TintColor;
		fixed _ColorMultiplier;
		fixed _RimMultiplier;
		//fixed _AlphaCutout;


		//uniform half4 _GlowColor;
		uniform fixed4 _RimColor;
		uniform fixed _RimBase;
		uniform fixed _RimPower;

		struct appdata_t {
		float4 vertex : POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		};

		struct v2f {
		float4 vertex : POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		//float4 pos : SV_POSITION;
		fixed3 normal : TEXCOORD1;
		fixed3 viewDir : TEXCOORD2;
		};

		float4 _MainTex_ST;

		v2f vert(appdata_full v)
		{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		//o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		o.color = v.color;
		o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
		float3 worldN = normalize(mul((float3x3)_Object2World, SCALED_NORMAL));
		o.normal = worldN;
		o.viewDir = normalize(WorldSpaceViewDir( v.vertex ));
		return o;
		}

		fixed4 frag(v2f i) : COLOR
		{
		fixed4 col;
		fixed4 tex = tex2D(_MainTex, i.texcoord);
		half rim = 1.0 - saturate(dot (i.viewDir, i.normal));
		half4 combinedColor = _RimColor  * tex * pow (rim, _RimPower) *_RimMultiplier ;
		col = 2.0f * tex * i.color * _TintColor * _ColorMultiplier + combinedColor;
		//col.rgb = 2.0f * tex.rgb * i.color.rgb * _TintColor.rgb * _ColorMultiplier + combinedColor;
		//col.a = 2.0f * tex.a * i.color.a * _TintColor.a + _RimColor.a;
		//col.a = col.a - (_AlphaCutout *(1 - col.a));
		return col ;
		}

		ENDCG
		}*/
	}
}
