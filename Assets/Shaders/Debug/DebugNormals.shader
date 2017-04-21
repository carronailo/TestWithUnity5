// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Debug/Normals" {
SubShader {
    Pass {
        Fog { Mode Off }
        CGPROGRAM

        #pragma vertex vert
        #pragma fragment frag
		#include "UnityCG.cginc"

        // vertex input: position, normal
        struct appdata {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
        };

        struct v2f {
            float4 pos : SV_POSITION;
            fixed4 color : COLOR;
        };
        
        v2f vert (appdata v) {
            v2f o;
            o.pos = UnityObjectToClipPos( v.vertex );
            //o.color.xyz = v.normal * 0.5 + 0.5;	// Normal’s X,Y,Z components are visualized as R,G,B colors. Because the normal components are in –1..1 range, we scale and bias them so that the output colors are in displayable 0..1 range.
            o.color.xyz = (mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL)) * 0.5 + 0.5;
			o.color.w = 1.0;
            return o;
        }
        
        fixed4 frag (v2f i) : COLOR0 { return i.color; }
        ENDCG
    }
}
}