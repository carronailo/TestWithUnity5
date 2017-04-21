// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ARPG Project/FlowLight (Based On Main UV)" {
	Properties {
		_FlowLightTex ("Flow Light (RGB)", 2D) = "white" {}
		_FlowSpeed("Flow Speed", Vector) = (1,1,0,0)
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

			sampler2D _FlowLightTex;
			float4 _FlowLightTex_ST;

			float2 _FlowSpeed;

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD0;
			};
			
			v2f vert (appdata_full v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_FlowLightTex);
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				fixed2 ruv = i.texcoord;
				ruv += _Time.xx * _FlowSpeed;
				fixed4 flowLight = tex2D(_FlowLightTex, ruv.xy);
				return flowLight;
			}

			ENDCG
		}
	} 
	FallBack "Diffuse"
}
