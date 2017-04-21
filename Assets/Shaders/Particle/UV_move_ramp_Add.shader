// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge Beta 0.35 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge

Shader "ARPG Project/Particles/UV_movie_Ramp_AddBlend" {
    Properties {
        [MaterialToggle] _AddMulti ("Add/Multi", Float ) = 2
        _Tex1 ("Tex1", 2D) = "white" {}
        _Tex2 ("Tex2", 2D) = "white" {}
        _S ("S", Range(0, 1)) = 0
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Multiply ("Multiply", Float ) = 1
        _XUSpeYURotZVSpeWVRot ("X=U Spe       Y=U Rot       Z=V Spe       W=V Rot", Vector) = (0,0,0,0)
        [MaterialToggle] _Ramp01 ("Ramp(0/1)", Float ) = 1
        _RampTex ("Ramp Tex", 2D) = "white" {}
        _NorPow ("Nor Pow", Range(-1, 1)) = 0
        _Nor ("Nor", 2D) = "white" {}
        _NorXUSpeYURotZVSpeWVRot ("Nor  X=U Spe       Y=U Rot       Z=V Spe       W=V Rot", Vector) = (0,0,0,0)
        [MaterialToggle] _AlphaAddMutiply ("Alpha(Add/Mutiply)", Float ) = 2
        _Mask ("Mask", 2D) = "white" {}
 		_GlowTex ("Glow", 2D) = "" {}
		_GlowColor ("Glow Color", Color)  = (1,1,1,1)
		_GlowStrength ("Glow Strength", Float) = 0.0
   }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
			"RenderType"="Glow11Transparent" 
			"RenderEffect"="Glow11Transparent"
        }
        LOD 200
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase

            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform sampler2D _Tex1;
			uniform float4 _Tex1_ST;
            uniform float4 _XUSpeYURotZVSpeWVRot;
            uniform sampler2D _Tex2;
			uniform float4 _Tex2_ST;
            uniform fixed _AddMulti;
            uniform sampler2D _RampTex;
			uniform float4 _RampTex_ST;
            uniform float _S;
            uniform fixed _Ramp01;
            uniform float _Multiply;
            uniform float _NorPow;
            uniform sampler2D _Nor; 
			uniform float4 _Nor_ST;
            uniform float4 _NorXUSpeYURotZVSpeWVRot;
            uniform fixed _AlphaAddMutiply;
            uniform sampler2D _Mask;
			uniform float4 _Mask_ST;

            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
				float4 norUV : TEXCOORD1;
				float4 rotUV : TEXCOORD2;
				float timeFactor : TEXCOORD3;
                float4 vertexColor : COLOR;
            };

            VertexOutput vert (VertexInput v) 
			{
				VertexOutput o;
				o.uv.xy = v.texcoord0;
				o.uv.zw = TRANSFORM_TEX(v.texcoord0, _Mask);
				o.vertexColor = v.vertexColor;
				o.pos = UnityObjectToClipPos(v.vertex);

				float timeFactor = (_Time + _TimeEditor).y;
                float2 halfVector = float2(0.5,0.5);
				float2 alteredUV = v.texcoord0 - halfVector;

				float2 rotNorUVCos = cos(_NorXUSpeYURotZVSpeWVRot.ga);
				float2 rotNorUVSin = sin(_NorXUSpeYURotZVSpeWVRot.ga);
				float2 alteredNorVertex1 = mul(alteredUV, float2x2( rotNorUVCos.x, -rotNorUVSin.x, rotNorUVSin.x, rotNorUVCos.x)) + halfVector + (timeFactor * _NorXUSpeYURotZVSpeWVRot.r) * float2(0, 0.25);
				float2 norUV1 = TRANSFORM_TEX(alteredNorVertex1, _Nor);
				float2 alteredNorVertex2 = mul(alteredUV, float2x2( rotNorUVCos.y, -rotNorUVSin.y, rotNorUVSin.y, rotNorUVCos.y)) + halfVector + (timeFactor * _NorXUSpeYURotZVSpeWVRot.b) * float2(0, 0.25);
				float2 norUV2 = TRANSFORM_TEX(alteredNorVertex2, _Nor);

				o.norUV = float4(norUV1, norUV2);

				float2 rotUVCos = cos(_XUSpeYURotZVSpeWVRot.ga);
				float2 rotUVSin = sin(_XUSpeYURotZVSpeWVRot.ga);
				o.rotUV = float4(rotUVCos, rotUVSin);
				o.timeFactor = timeFactor;

				return o;
            }

            fixed4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float2 halfVector = fixed2(0.5,0.5);

                float4 normal1 = tex2D(_Nor, i.norUV.xy);
                float4 normal2 = tex2D(_Nor, i.norUV.zw);

                float2 poweredNormal = pow((normal1.xy + normal2.xy), _NorPow) * i.uv.xy - halfVector;

				float2 tmp = i.timeFactor * float2(0, 0.5);

                float2 alteredTex1Vertex = mul(poweredNormal, float2x2( i.rotUV.x, -i.rotUV.z, i.rotUV.z, i.rotUV.x)) + halfVector + (_XUSpeYURotZVSpeWVRot.r * tmp);
                float4 tex1Color = tex2D(_Tex1, TRANSFORM_TEX(alteredTex1Vertex, _Tex1));

                float2 alteredTex2Vertex = mul(poweredNormal, float2x2( i.rotUV.y, -i.rotUV.w, i.rotUV.w, i.rotUV.y)) + halfVector + (_XUSpeYURotZVSpeWVRot.b * tmp);
                float4 tex2Color = tex2D(_Tex2, TRANSFORM_TEX(alteredTex2Vertex, _Tex2));

				float3 temp1 = (tex1Color.rgb + tex2Color.rgb);
				if(_AddMulti > 0)
					temp1 = saturate(tex1Color.rgb * tex2Color.rgb);
				float temp2 = (tex1Color.a + tex2Color.a);
				if(_AlphaAddMutiply > 0)
					temp2 = saturate(tex1Color.a * tex2Color.a);
				float4 texColor = fixed4(temp1, temp2);
				float texLumi = dot(texColor.rgb, float3(0.3,0.59,0.11));
                float3 texGrayscale = lerp(texColor.rgb, float3(texLumi, texLumi, texLumi), _S);

                float4 maskColor = tex2D(_Mask,i.uv.zw);
				float maskLumi = dot(maskColor.rgb, float3(0.3,0.59,0.11));
				float alteredMask = maskColor.a * maskLumi * texColor.a;

                float3 emissive = _Color.rgb * texGrayscale;
				if(_Ramp01 > 0)
				{
				    float2 rampUV = float2(texGrayscale.r,1.0);
					float4 rampTexColor = tex2D(_RampTex, TRANSFORM_TEX(rampUV, _RampTex));
					emissive = texGrayscale * rampTexColor.rgb;
				}
                float3 finalColor = emissive * _Multiply * i.vertexColor.rgb * i.vertexColor.a * alteredMask;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    //CustomEditor "ShaderForgeMaterialInspector"
	CustomEditor "CustomQueueAndGlowMatInspector"
}

