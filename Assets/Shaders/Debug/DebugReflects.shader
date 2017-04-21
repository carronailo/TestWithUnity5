// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Debug/Reflects" {
Properties
{
	_SampleColor ("Sample Color", Color) = (1,1,1,1)
	_ReflectMap ("Reflect Texture (RGB)", 2D) = "white" {}
	_ReflectCubeMap ("Reflect Texture (Cube)", CUBE) = "" {}
	_Blur ("Blur Factor", Range(0,10)) = 0.1
}
SubShader {
    Pass {
        Fog { Mode Off }
        CGPROGRAM

        #pragma vertex vert
        #pragma fragment frag
		#pragma target 3.0
		#include "UnityCG.cginc"

		sampler2D _ReflectMap;
		sampler2D _BumpMap;
		samplerCUBE _ReflectCubeMap;

		uniform half4 _ReflectCubeMap_TexelSize;
		uniform fixed _Blur;

		float4 _BumpMap_ST;

        // vertex input: position, normal
        struct appdata {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
			float2 texcoord: TEXCOORD0;
        };

        struct v2f {
            float4 pos : SV_POSITION;
            fixed4 color : COLOR;
			float3 reflectDir : TEXCOORD0;
       };
        
        v2f vert (appdata v) {
            v2f o;
            o.pos = UnityObjectToClipPos( v.vertex );
			float3 viewDir = WorldSpaceViewDir(v.vertex);
			half3 reflectDir = reflect(-viewDir, normalize((mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL))));
            //o.color.xyz = v.normal * 0.5 + 0.5;	// Normal’s X,Y,Z components are visualized as R,G,B colors. Because the normal components are in –1..1 range, we scale and bias them so that the output colors are in displayable 0..1 range.
            //o.color.xyz = reflectDir * 0.5 + 0.5;
			//o.color.w = 1.0;
			//o.reflectDir = normalize((mul((float3x3)_Object2World, SCALED_NORMAL)));
			o.reflectDir = reflectDir;
            return o;
        }
        
        fixed4 frag (v2f i) : COLOR0 {
			//return tex2D(_ReflectMap, (i.reflectDir.xy + fixed2(1.0, 1.0)) * 0.5);
			fixed delta = 0.0001;
			fixed4 color = texCUBE (_ReflectCubeMap, i.reflectDir);
			color += texCUBE (_ReflectCubeMap, i.reflectDir + _ReflectCubeMap_TexelSize.xyz * half3(-_Blur, -_Blur, delta * -_Blur));
			color += texCUBE (_ReflectCubeMap, i.reflectDir + _ReflectCubeMap_TexelSize.xyz * half3(-_Blur, -_Blur, delta * _Blur));
			color += texCUBE (_ReflectCubeMap, i.reflectDir + _ReflectCubeMap_TexelSize.xyz * half3(_Blur, _Blur, delta * -_Blur));
			color += texCUBE (_ReflectCubeMap, i.reflectDir + _ReflectCubeMap_TexelSize.xyz * half3(_Blur, _Blur, delta * _Blur));
			color += texCUBE (_ReflectCubeMap, i.reflectDir + _ReflectCubeMap_TexelSize.xyz * half3(_Blur, -_Blur, delta * _Blur));
			color += texCUBE (_ReflectCubeMap, i.reflectDir + _ReflectCubeMap_TexelSize.xyz * half3(_Blur, -_Blur, delta * -_Blur));
			color += texCUBE (_ReflectCubeMap, i.reflectDir + _ReflectCubeMap_TexelSize.xyz * half3(-_Blur, _Blur, delta * -_Blur));
			color += texCUBE (_ReflectCubeMap, i.reflectDir + _ReflectCubeMap_TexelSize.xyz * half3(-_Blur, _Blur, delta * _Blur));
			return color / 9;
		}
        ENDCG
    }
}
}