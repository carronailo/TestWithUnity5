// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge Beta 0.35 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.35;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:14476,x:31284,y:32222|emission-14670-OUT,alpha-15472-OUT;n:type:ShaderForge.SFN_Color,id:14477,x:32494,y:31981,ptlb:Color,ptin:_Color,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2d,id:14478,x:33246,y:32710,ptlb:Tex1,ptin:_Tex1,ntxv:0,isnm:False|UVIN-14493-UVOUT;n:type:ShaderForge.SFN_Multiply,id:14479,x:32239,y:32099|A-14477-RGB,B-14585-OUT;n:type:ShaderForge.SFN_Time,id:14486,x:34198,y:32681;n:type:ShaderForge.SFN_Multiply,id:14487,x:33787,y:32757|A-14488-X,B-14486-T;n:type:ShaderForge.SFN_Vector4Property,id:14488,x:34198,y:33011,ptlb:X=U Spe       Y=U Rot       Z=V Spe       W=V Rot,ptin:_XUSpeYURotZVSpeWVRot,glob:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Panner,id:14493,x:33552,y:32666,spu:0,spv:0.5|UVIN-14562-UVOUT,DIST-14487-OUT;n:type:ShaderForge.SFN_Rotator,id:14562,x:33876,y:32606|UVIN-14678-OUT,ANG-14488-Y;n:type:ShaderForge.SFN_TexCoord,id:14563,x:34397,y:32950,uv:0;n:type:ShaderForge.SFN_Panner,id:14564,x:33555,y:32942,spu:0,spv:0.5|UVIN-14566-UVOUT,DIST-14565-OUT;n:type:ShaderForge.SFN_Multiply,id:14565,x:33787,y:32942|A-14488-Z,B-14486-T;n:type:ShaderForge.SFN_Rotator,id:14566,x:33881,y:33132|UVIN-14678-OUT,ANG-14488-W;n:type:ShaderForge.SFN_Tex2d,id:14568,x:33262,y:32986,ptlb:Tex2,ptin:_Tex2,ntxv:0,isnm:False|UVIN-14564-UVOUT;n:type:ShaderForge.SFN_Blend,id:14569,x:32918,y:32963,blmd:1,clmp:True|SRC-14478-RGB,DST-14568-RGB;n:type:ShaderForge.SFN_Add,id:14570,x:32901,y:32787|A-14478-RGB,B-14568-RGB;n:type:ShaderForge.SFN_SwitchProperty,id:14571,x:32723,y:32875,ptlb:A/M,ptin:_AM,on:False|A-14570-OUT,B-14569-OUT;n:type:ShaderForge.SFN_Desaturate,id:14585,x:32494,y:32215|COL-14571-OUT,DES-14668-OUT;n:type:ShaderForge.SFN_Vector1,id:14586,x:32820,y:32486,v1:1;n:type:ShaderForge.SFN_Tex2d,id:14588,x:32024,y:32493,ptlb:Ramp Tex,ptin:_RampTex,ntxv:0,isnm:False|UVIN-14653-OUT;n:type:ShaderForge.SFN_Multiply,id:14616,x:32073,y:32257|A-14585-OUT,B-14588-RGB;n:type:ShaderForge.SFN_ComponentMask,id:14652,x:32508,y:32491,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-14664-OUT;n:type:ShaderForge.SFN_Append,id:14653,x:32198,y:32585|A-14652-OUT,B-14586-OUT;n:type:ShaderForge.SFN_Clamp01,id:14664,x:32368,y:32354|IN-14585-OUT;n:type:ShaderForge.SFN_Slider,id:14668,x:32740,y:32356,ptlb:S,ptin:_S,min:0,cur:0,max:1;n:type:ShaderForge.SFN_SwitchProperty,id:14669,x:31873,y:32150,ptlb:Ramp(0/1),ptin:_Ramp01,on:False|A-14479-OUT,B-14616-OUT;n:type:ShaderForge.SFN_Multiply,id:14670,x:31608,y:32092|A-14669-OUT,B-14671-OUT,C-15482-RGB;n:type:ShaderForge.SFN_ValueProperty,id:14671,x:31743,y:32433,ptlb:Multiply,ptin:_Multiply,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:14678,x:34198,y:32834|A-14681-OUT,B-14563-UVOUT;n:type:ShaderForge.SFN_Append,id:14679,x:34889,y:32667|A-14759-R,B-14759-G;n:type:ShaderForge.SFN_Slider,id:14680,x:34565,y:32938,ptlb:Nor Pow,ptin:_NorPow,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Power,id:14681,x:34397,y:32783|VAL-14774-OUT,EXP-14680-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:14757,x:35360,y:32868,ptlb:Nor,ptin:_Nor,glob:False;n:type:ShaderForge.SFN_Tex2d,id:14759,x:35157,y:32770,ntxv:0,isnm:False|UVIN-14763-UVOUT,TEX-14757-TEX;n:type:ShaderForge.SFN_Multiply,id:14761,x:35698,y:32817|A-14891-T,B-14892-X;n:type:ShaderForge.SFN_Panner,id:14763,x:35463,y:32726,spu:0,spv:0.25|UVIN-14765-UVOUT,DIST-14761-OUT;n:type:ShaderForge.SFN_Rotator,id:14765,x:35787,y:32665|UVIN-14943-UVOUT,ANG-14892-Y;n:type:ShaderForge.SFN_Panner,id:14767,x:35466,y:33002,spu:0,spv:0.25|UVIN-14771-UVOUT,DIST-14769-OUT;n:type:ShaderForge.SFN_Multiply,id:14769,x:35698,y:33002|A-14891-T,B-14892-Z;n:type:ShaderForge.SFN_Rotator,id:14771,x:35792,y:33192|UVIN-14943-UVOUT,ANG-14892-W;n:type:ShaderForge.SFN_Tex2d,id:14773,x:35157,y:32992,ntxv:0,isnm:False|UVIN-14767-UVOUT,TEX-14757-TEX;n:type:ShaderForge.SFN_Add,id:14774,x:34621,y:32773|A-14679-OUT,B-14788-OUT;n:type:ShaderForge.SFN_Append,id:14788,x:34935,y:32960|A-14773-R,B-14773-G;n:type:ShaderForge.SFN_Vector1,id:14886,x:34463,y:34061,v1:1;n:type:ShaderForge.SFN_Vector1,id:14888,x:34527,y:34125,v1:1;n:type:ShaderForge.SFN_Vector1,id:14890,x:34591,y:34189,v1:1;n:type:ShaderForge.SFN_Time,id:14891,x:36166,y:32815;n:type:ShaderForge.SFN_Vector4Property,id:14892,x:36207,y:33011,ptlb:Nor  X=U Spe       Y=U Rot       Z=V Spe       W=V Rot,ptin:_NorXUSpeYURotZVSpeWVRot,glob:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_TexCoord,id:14943,x:36256,y:32660,uv:0;n:type:ShaderForge.SFN_SwitchProperty,id:15467,x:32200,y:32896,ptlb:Alpha(Add/Mutiply),ptin:_AlphaAddMutiply,on:False|A-15469-OUT,B-15471-OUT;n:type:ShaderForge.SFN_Add,id:15469,x:32461,y:32768|A-14478-A,B-14568-A;n:type:ShaderForge.SFN_Blend,id:15471,x:32450,y:32954,blmd:1,clmp:True|SRC-14478-A,DST-14568-A;n:type:ShaderForge.SFN_Multiply,id:15472,x:31676,y:32617|A-14477-A,B-15467-OUT,C-15474-OUT,D-15482-A;n:type:ShaderForge.SFN_Tex2d,id:15473,x:32021,y:32908,ptlb:Mask,ptin:_Mask,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:15474,x:31720,y:32809|A-15473-A,B-15475-OUT;n:type:ShaderForge.SFN_Desaturate,id:15475,x:31739,y:32978|COL-15473-RGB,DES-15476-OUT;n:type:ShaderForge.SFN_Vector1,id:15476,x:31940,y:33176,v1:1;n:type:ShaderForge.SFN_VertexColor,id:15482,x:31951,y:31926;proporder:14571-14478-14568-14668-14477-14671-14488-14669-14588-14680-14757-14892-15467-15473;pass:END;sub:END;*/

Shader "ARPG Project/Particles/UV_movie_Ramp_AlphaBlend" {
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
		[HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
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
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Cull Off
            
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

			fixed4 frag(VertexOutput i) : COLOR 
			{
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
				float alteredMask = maskColor.a * maskLumi * i.vertexColor.a;

                float3 emissive = _Color.rgb * texGrayscale;
				if(_Ramp01 > 0)
				{
					float2 rampUV = float2(texGrayscale.r,1.0);
					float4 rampTexColor = tex2D(_RampTex, TRANSFORM_TEX(rampUV, _RampTex));
					emissive = texGrayscale * rampTexColor.rgb;
				}
                float3 finalColor = emissive * _Multiply * i.vertexColor.rgb;
				float finalAlpha = _Color.a * texColor.a * alteredMask;
/// Final Color:
				return fixed4(finalColor,finalAlpha);
			}
			ENDCG
		}
	}
    FallBack "Diffuse"
	CustomEditor "CustomQueueAndGlowMatInspector"
}
