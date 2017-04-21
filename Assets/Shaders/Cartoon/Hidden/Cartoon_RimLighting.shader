// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Cartoon_RimLightting" 
{
	Properties 
	{
		_MainTex("Base (RGB) Gloss (A) ", 2D) = "white" { }
		
		// RIM LIGHTING
		_RimSwitch("Use Rim", Range(-1, 1)) = -1
		_RimColor("Rim Color", Color) = (0, 0, 0, 1)
		_RimPower("Rim Power", Range(0.1, 2.0)) = 1.0
		_RimBase("Rim Base", Range(0, 1)) = 0.1
		_RimFrequency("Frequency", Float) = 1.0

		//DISSOLVE
		_DissolveTex("Dissolve Texture(A)", 2D) = "white" { }
		_DissolveFactor("Dissolve Factor", Range(0, 1.01)) = 0

		_AlphaCutoutSwitch("Use CutOut", Range(0, 1)) = 0
		_AlphaCutout("Alpha CutOut", Range(0, 1)) = 0

	}
	SubShader 
	{
		Tags { "Queue"="Geometry" "RenderType"="Opaque" }
		LOD 200
		
		Pass 
		{
			Name "RIMLIGHTING"
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

			sampler2D _MainTex;
			sampler2D _DissolveTex;

			uniform fixed _RimSwitch;
			uniform fixed4 _RimColor;
			uniform fixed _RimBase;
			uniform fixed _RimPower;
			uniform fixed _RimFrequency;
			uniform fixed _DissolveFactor;
			uniform fixed _AlphaCutoutSwitch;
			uniform fixed _AlphaCutout;

			uniform float _Clip;// = 0;
			uniform float4 _ClipRange0;// = float4(0.0, 0.0, 1.0, 1.0);
			uniform float2 _ClipArgs0;// = float2(1000.0, 1000.0);

			struct v2f 
			{
				float4 pos : SV_POSITION;
				half4 uv : TEXCOORD0;
				fixed3 normal : TEXCOORD1;
				fixed3 viewDir : TEXCOORD2;
				float2 worldPos : TEXCOORD3;
			};

			uniform float4 _MainTex_ST;
			uniform float4 _DissolveTex_ST;

			v2f vert (appdata_full v) 
			{
				v2f o = (v2f)0;

				if(_RimSwitch <= 0.0)
				{
					return o;
				}
				else
				{
					o.pos = UnityObjectToClipPos (v.vertex);
					o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.uv.zw = TRANSFORM_TEX(v.texcoord, _DissolveTex);
					float3 worldN = normalize(mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL));
					o.normal = worldN;
					o.viewDir = normalize(WorldSpaceViewDir( v.vertex ));
					o.worldPos = mul(unity_ObjectToWorld, v.vertex).xy;
					if (_Clip > 0.5)
						o.worldPos = o.worldPos * _ClipRange0.zw + _ClipRange0.xy;
					return o;
				}
			}

			fixed4 frag (v2f IN) : SV_Target 
			{
				if(_RimSwitch <= 0.0)
				{
					return fixed4(0, 0, 0, 0);
				}
				else
				{
					// Rim
					half rim = 1.0 - saturate(dot (IN.viewDir, IN.normal));
					fixed scale = (1.0 - _RimBase) * 0.5;
					half3 combinedColor = _RimColor.rgb * pow (rim, _RimPower) * (sin(_Time * _RimFrequency).w * scale + scale + _RimBase);
					// 计算时间的变换公式：time factor = _RimBase + (1-_RimBase) * (0~1之间的随时间变化参数)
					// sin(_Time * _RimFrequency).w 是用_RimFrequency进行缩放调整的时间变化参数，但是取值范围是-1~1，所以需要用(sin(_Time * _RimFrequency).w + 1) * 0.5来变换到0～1之间
					// 最后得出 time factor = (sin(_Time * _RimFrequency).w + 1) * 0.5 * (1-_RimBase) + _RimBase
					// 取 scale = 0.5 * (1-_RimBase)，转换为 time factor = sin(_Time * _RimFrequency).w * scale + scale + _RimBase

					fixed4 c = tex2D(_MainTex, IN.uv.xy);
					fixed4 dis = tex2D(_DissolveTex, IN.uv.zw);
					// ui clipping
					if (_Clip > 0.5)
					{
						float2 factor = (float2(1.0, 1.0) - abs(IN.worldPos)) * _ClipArgs0;
						float f = min(factor.x, factor.y);
						_RimColor.a *= clamp(f, 0.0, 1.0);
						if (_RimColor.a <= 0.1 || dis.a < _DissolveFactor || (_AlphaCutoutSwitch > 0 && c.a < _AlphaCutout))
							discard;
					}
					else
					{
						if (dis.a < _DissolveFactor || (_AlphaCutoutSwitch > 0 && c.a < _AlphaCutout))
							discard;
					}
					return fixed4(combinedColor, _RimColor.a);
				}
			}

			ENDCG

		}

	} 
	//FallBack "Diffuse"
}
