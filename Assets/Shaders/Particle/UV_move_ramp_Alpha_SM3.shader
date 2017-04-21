// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge Beta 0.35 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.35;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:14476,x:31284,y:32222|emission-14670-OUT,alpha-15472-OUT;n:type:ShaderForge.SFN_Color,id:14477,x:32494,y:31981,ptlb:Color,ptin:_Color,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2d,id:14478,x:33246,y:32710,ptlb:Tex1,ptin:_Tex1,ntxv:0,isnm:False|UVIN-14493-UVOUT;n:type:ShaderForge.SFN_Multiply,id:14479,x:32239,y:32099|A-14477-RGB,B-14585-OUT;n:type:ShaderForge.SFN_Time,id:14486,x:34198,y:32681;n:type:ShaderForge.SFN_Multiply,id:14487,x:33787,y:32757|A-14488-X,B-14486-T;n:type:ShaderForge.SFN_Vector4Property,id:14488,x:34198,y:33011,ptlb:X=U Spe       Y=U Rot       Z=V Spe       W=V Rot,ptin:_XUSpeYURotZVSpeWVRot,glob:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Panner,id:14493,x:33552,y:32666,spu:0,spv:0.5|UVIN-14562-UVOUT,DIST-14487-OUT;n:type:ShaderForge.SFN_Rotator,id:14562,x:33876,y:32606|UVIN-14678-OUT,ANG-14488-Y;n:type:ShaderForge.SFN_TexCoord,id:14563,x:34397,y:32950,uv:0;n:type:ShaderForge.SFN_Panner,id:14564,x:33555,y:32942,spu:0,spv:0.5|UVIN-14566-UVOUT,DIST-14565-OUT;n:type:ShaderForge.SFN_Multiply,id:14565,x:33787,y:32942|A-14488-Z,B-14486-T;n:type:ShaderForge.SFN_Rotator,id:14566,x:33881,y:33132|UVIN-14678-OUT,ANG-14488-W;n:type:ShaderForge.SFN_Tex2d,id:14568,x:33262,y:32986,ptlb:Tex2,ptin:_Tex2,ntxv:0,isnm:False|UVIN-14564-UVOUT;n:type:ShaderForge.SFN_Blend,id:14569,x:32918,y:32963,blmd:1,clmp:True|SRC-14478-RGB,DST-14568-RGB;n:type:ShaderForge.SFN_Add,id:14570,x:32901,y:32787|A-14478-RGB,B-14568-RGB;n:type:ShaderForge.SFN_SwitchProperty,id:14571,x:32723,y:32875,ptlb:A/M,ptin:_AM,on:False|A-14570-OUT,B-14569-OUT;n:type:ShaderForge.SFN_Desaturate,id:14585,x:32494,y:32215|COL-14571-OUT,DES-14668-OUT;n:type:ShaderForge.SFN_Vector1,id:14586,x:32820,y:32486,v1:1;n:type:ShaderForge.SFN_Tex2d,id:14588,x:32024,y:32493,ptlb:Ramp Tex,ptin:_RampTex,ntxv:0,isnm:False|UVIN-14653-OUT;n:type:ShaderForge.SFN_Multiply,id:14616,x:32073,y:32257|A-14585-OUT,B-14588-RGB;n:type:ShaderForge.SFN_ComponentMask,id:14652,x:32508,y:32491,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-14664-OUT;n:type:ShaderForge.SFN_Append,id:14653,x:32198,y:32585|A-14652-OUT,B-14586-OUT;n:type:ShaderForge.SFN_Clamp01,id:14664,x:32368,y:32354|IN-14585-OUT;n:type:ShaderForge.SFN_Slider,id:14668,x:32740,y:32356,ptlb:S,ptin:_S,min:0,cur:0,max:1;n:type:ShaderForge.SFN_SwitchProperty,id:14669,x:31873,y:32150,ptlb:Ramp(0/1),ptin:_Ramp01,on:False|A-14479-OUT,B-14616-OUT;n:type:ShaderForge.SFN_Multiply,id:14670,x:31608,y:32092|A-14669-OUT,B-14671-OUT,C-15482-RGB;n:type:ShaderForge.SFN_ValueProperty,id:14671,x:31743,y:32433,ptlb:Multiply,ptin:_Multiply,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:14678,x:34198,y:32834|A-14681-OUT,B-14563-UVOUT;n:type:ShaderForge.SFN_Append,id:14679,x:34889,y:32667|A-14759-R,B-14759-G;n:type:ShaderForge.SFN_Slider,id:14680,x:34565,y:32938,ptlb:Nor Pow,ptin:_NorPow,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Power,id:14681,x:34397,y:32783|VAL-14774-OUT,EXP-14680-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:14757,x:35360,y:32868,ptlb:Nor,ptin:_Nor,glob:False;n:type:ShaderForge.SFN_Tex2d,id:14759,x:35157,y:32770,ntxv:0,isnm:False|UVIN-14763-UVOUT,TEX-14757-TEX;n:type:ShaderForge.SFN_Multiply,id:14761,x:35698,y:32817|A-14891-T,B-14892-X;n:type:ShaderForge.SFN_Panner,id:14763,x:35463,y:32726,spu:0,spv:0.25|UVIN-14765-UVOUT,DIST-14761-OUT;n:type:ShaderForge.SFN_Rotator,id:14765,x:35787,y:32665|UVIN-14943-UVOUT,ANG-14892-Y;n:type:ShaderForge.SFN_Panner,id:14767,x:35466,y:33002,spu:0,spv:0.25|UVIN-14771-UVOUT,DIST-14769-OUT;n:type:ShaderForge.SFN_Multiply,id:14769,x:35698,y:33002|A-14891-T,B-14892-Z;n:type:ShaderForge.SFN_Rotator,id:14771,x:35792,y:33192|UVIN-14943-UVOUT,ANG-14892-W;n:type:ShaderForge.SFN_Tex2d,id:14773,x:35157,y:32992,ntxv:0,isnm:False|UVIN-14767-UVOUT,TEX-14757-TEX;n:type:ShaderForge.SFN_Add,id:14774,x:34621,y:32773|A-14679-OUT,B-14788-OUT;n:type:ShaderForge.SFN_Append,id:14788,x:34935,y:32960|A-14773-R,B-14773-G;n:type:ShaderForge.SFN_Vector1,id:14886,x:34463,y:34061,v1:1;n:type:ShaderForge.SFN_Vector1,id:14888,x:34527,y:34125,v1:1;n:type:ShaderForge.SFN_Vector1,id:14890,x:34591,y:34189,v1:1;n:type:ShaderForge.SFN_Time,id:14891,x:36166,y:32815;n:type:ShaderForge.SFN_Vector4Property,id:14892,x:36207,y:33011,ptlb:Nor  X=U Spe       Y=U Rot       Z=V Spe       W=V Rot,ptin:_NorXUSpeYURotZVSpeWVRot,glob:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_TexCoord,id:14943,x:36256,y:32660,uv:0;n:type:ShaderForge.SFN_SwitchProperty,id:15467,x:32200,y:32896,ptlb:Alpha(Add/Mutiply),ptin:_AlphaAddMutiply,on:False|A-15469-OUT,B-15471-OUT;n:type:ShaderForge.SFN_Add,id:15469,x:32461,y:32768|A-14478-A,B-14568-A;n:type:ShaderForge.SFN_Blend,id:15471,x:32450,y:32954,blmd:1,clmp:True|SRC-14478-A,DST-14568-A;n:type:ShaderForge.SFN_Multiply,id:15472,x:31676,y:32617|A-14477-A,B-15467-OUT,C-15474-OUT,D-15482-A;n:type:ShaderForge.SFN_Tex2d,id:15473,x:32021,y:32908,ptlb:Mask,ptin:_Mask,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:15474,x:31720,y:32809|A-15473-A,B-15475-OUT;n:type:ShaderForge.SFN_Desaturate,id:15475,x:31739,y:32978|COL-15473-RGB,DES-15476-OUT;n:type:ShaderForge.SFN_Vector1,id:15476,x:31940,y:33176,v1:1;n:type:ShaderForge.SFN_VertexColor,id:15482,x:31951,y:31926;proporder:14571-14478-14568-14668-14477-14671-14488-14669-14588-14680-14757-14892-15467-15473;pass:END;sub:END;*/

Shader "ARPG Project/Particles/UV_movie_Ramp_AlphaBlend_SM3" {
    Properties {
        [MaterialToggle] _AM ("A/M", Float ) = 2
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
            #pragma exclude_renderers d3d11 opengl xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform sampler2D _Tex1; uniform float4 _Tex1_ST;
            uniform float4 _XUSpeYURotZVSpeWVRot;
            uniform sampler2D _Tex2; uniform float4 _Tex2_ST;
            uniform fixed _AM;
            uniform sampler2D _RampTex; uniform float4 _RampTex_ST;
            uniform float _S;
            uniform fixed _Ramp01;
            uniform float _Multiply;
            uniform float _NorPow;
            uniform sampler2D _Nor; uniform float4 _Nor_ST;
            uniform float4 _NorXUSpeYURotZVSpeWVRot;
            uniform fixed _AlphaAddMutiply;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_14486 = _Time + _TimeEditor;
                float node_14562_ang = _XUSpeYURotZVSpeWVRot.g;
                float node_14562_spd = 1.0;
                float node_14562_cos = cos(node_14562_spd*node_14562_ang);
                float node_14562_sin = sin(node_14562_spd*node_14562_ang);
                float2 node_14562_piv = float2(0.5,0.5);
                float4 node_14891 = _Time + _TimeEditor;
                float node_14765_ang = _NorXUSpeYURotZVSpeWVRot.g;
                float node_14765_spd = 1.0;
                float node_14765_cos = cos(node_14765_spd*node_14765_ang);
                float node_14765_sin = sin(node_14765_spd*node_14765_ang);
                float2 node_14765_piv = float2(0.5,0.5);
                float2 node_14943 = i.uv0;
                float2 node_14765 = (mul(node_14943.rg-node_14765_piv,float2x2( node_14765_cos, -node_14765_sin, node_14765_sin, node_14765_cos))+node_14765_piv);
                float2 node_14763 = (node_14765+(node_14891.g*_NorXUSpeYURotZVSpeWVRot.r)*float2(0,0.25));
                float4 node_14759 = tex2D(_Nor,TRANSFORM_TEX(node_14763, _Nor));
                float node_14771_ang = _NorXUSpeYURotZVSpeWVRot.a;
                float node_14771_spd = 1.0;
                float node_14771_cos = cos(node_14771_spd*node_14771_ang);
                float node_14771_sin = sin(node_14771_spd*node_14771_ang);
                float2 node_14771_piv = float2(0.5,0.5);
                float2 node_14771 = (mul(node_14943.rg-node_14771_piv,float2x2( node_14771_cos, -node_14771_sin, node_14771_sin, node_14771_cos))+node_14771_piv);
                float2 node_14767 = (node_14771+(node_14891.g*_NorXUSpeYURotZVSpeWVRot.b)*float2(0,0.25));
                float4 node_14773 = tex2D(_Nor,TRANSFORM_TEX(node_14767, _Nor));
                float2 node_14678 = (pow((float2(node_14759.r,node_14759.g)+float2(node_14773.r,node_14773.g)),_NorPow)*i.uv0.rg);
                float2 node_14562 = (mul(node_14678-node_14562_piv,float2x2( node_14562_cos, -node_14562_sin, node_14562_sin, node_14562_cos))+node_14562_piv);
                float2 node_14493 = (node_14562+(_XUSpeYURotZVSpeWVRot.r*node_14486.g)*float2(0,0.5));
                float4 node_14478 = tex2D(_Tex1,TRANSFORM_TEX(node_14493, _Tex1));
                float node_14566_ang = _XUSpeYURotZVSpeWVRot.a;
                float node_14566_spd = 1.0;
                float node_14566_cos = cos(node_14566_spd*node_14566_ang);
                float node_14566_sin = sin(node_14566_spd*node_14566_ang);
                float2 node_14566_piv = float2(0.5,0.5);
                float2 node_14566 = (mul(node_14678-node_14566_piv,float2x2( node_14566_cos, -node_14566_sin, node_14566_sin, node_14566_cos))+node_14566_piv);
                float2 node_14564 = (node_14566+(_XUSpeYURotZVSpeWVRot.b*node_14486.g)*float2(0,0.5));
                float4 node_14568 = tex2D(_Tex2,TRANSFORM_TEX(node_14564, _Tex2));
                float3 node_14585 = lerp(lerp( (node_14478.rgb+node_14568.rgb), saturate((node_14478.rgb*node_14568.rgb)), _AM ),dot(lerp( (node_14478.rgb+node_14568.rgb), saturate((node_14478.rgb*node_14568.rgb)), _AM ),float3(0.3,0.59,0.11)),_S);
                float2 node_14653 = float2(saturate(node_14585).r,1.0);
                float4 node_15482 = i.vertexColor;
                float3 emissive = (lerp( (_Color.rgb*node_14585), (node_14585*tex2D(_RampTex,TRANSFORM_TEX(node_14653, _RampTex)).rgb), _Ramp01 )*_Multiply*node_15482.rgb);
                float3 finalColor = emissive;
                float2 node_15488 = i.uv0;
                float4 node_15473 = tex2D(_Mask,TRANSFORM_TEX(node_15488.rg, _Mask));
/// Final Color:
                return fixed4(finalColor,(_Color.a*lerp( (node_14478.a+node_14568.a), saturate((node_14478.a*node_14568.a)), _AlphaAddMutiply )*(node_15473.a*lerp(node_15473.rgb,dot(node_15473.rgb,float3(0.3,0.59,0.11)),1.0))*node_15482.a));
            }
            ENDCG
        }
    }
    FallBack "ARPG Project/Particles/UV_movie_Ramp_AlphaBlend"
    CustomEditor "ShaderForgeMaterialInspector"
	CustomEditor "CustomQueueAndGlowMatInspector"
}
