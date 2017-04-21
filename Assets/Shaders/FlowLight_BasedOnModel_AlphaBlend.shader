// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ARPG Project/FlowLight (Based On Model) (AlphaBlend)" {
	Properties {
		_FlowLightColor1 ("Flow Light 1 Color", Color) = (1,1,1,1)
		_FlowLightTex1 ("Flow Light 1 (RGB)", 2D) = "white" {}
		_FlowSpeed1("Flow Speed 1", Vector) = (1,1,0,0)
		_FlowLightColor2 ("Flow Light 2 Color", Color) = (1,1,1,1)
		_FlowLightTex2 ("Flow Light 2 (RGB)", 2D) = "white" {}
		_FlowSpeed2("Flow Speed 2", Vector) = (1,0,0,0)
		_Multiplier ("Multiplier", Float) = 1
	}
	SubShader {
		Tags { "Queue"="Geometry+2" "RenderType"="Opaque" }
		Blend SrcAlpha One
		LOD 100
		
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			fixed4 _FlowLightColor1;
			fixed4 _FlowLightColor2;
			sampler2D _FlowLightTex1;
			sampler2D _FlowLightTex2;
			fixed2 _FlowSpeed1;
			fixed2 _FlowSpeed2;
			fixed _Multiplier;

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				fixed3 objvertex : TEXCOORD1;
			};
			
			v2f vert (appdata_full v)
			{
				v2f o;
				o.objvertex = v.vertex;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				fixed2 ruv1 = i.objvertex.xy;
				ruv1 += _Time.xx * _FlowSpeed1;
				fixed4 flowLight1 = tex2D(_FlowLightTex1, ruv1.xy);
				fixed2 ruv2 = i.objvertex.xy;
				ruv2 += _Time.xx * _FlowSpeed2;
				fixed4 flowLight2 = tex2D(_FlowLightTex2, ruv2.xy);

				return i.color * (flowLight1 * _FlowLightColor1 * flowLight2 * _FlowLightColor2) * _Multiplier;
			}

			ENDCG
		}
	} 
	FallBack "Diffuse"
}
