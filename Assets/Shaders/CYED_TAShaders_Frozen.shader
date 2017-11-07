//2015_07_17//
//CYED_TAShader_Frozen By KK/

Shader "TAShaders/Frozen" 
{
    Properties 
    {
        _Transparent ("透明度" , range(0.0 , 1.0 )) = 0.5 

        _MainTex ("固有色(RGB) 冰(A)", 2D) = "white" {}
        
        _ReflColor ("反光颜色" , Color) = (1,1,1,1) 
        _ReflPower ("反光强度", range(0, 3.0)) = 0.5

        _NoiseTex ("纹理贴图(RGB) 透明度(A)", 2D) = "white" {}
        _NoiseType ("纹理位移", range (0,0.5)) = 0.25
        _NoisePower ("纹理扰动", range (0,0.5)) = 0.1

        _RimLevel ("边缘光强度",Range(0,5)) = 1
        _RimPower ("边缘光范围",Range(0,5)) = 1    
    }

    SubShader 
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Back
        LOD 400
			
        CGPROGRAM
        #pragma surface surf WrapPhong vertex:CameraViewReflect approxview halfasview noforwardadd 

        sampler2D _NoiseTex; 
        sampler2D _MainTex;

        fixed _NoisePower;
        fixed _NoiseType;   

        fixed4 _ReflColor;
        fixed _ReflPower;
        fixed _Transparent;
        fixed _RimLevel;
        fixed _RimPower;

        inline fixed4 LightingWrapPhong (SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten) 
        {
            lightDir = normalize(lightDir);
            fixed diff = max (0, dot (s.Normal, lightDir) * 0.5 + 0.5);           
            fixed4 c;
            c.rgb = (s.Albedo * _LightColor0.rgb * diff) * (atten * 2);
            c.a = s.Alpha * atten;
            return c;
        }

        struct Input 
        {
            half2 uv_NoiseTex;
            half2 uv_MainTex;
            half4 nv : TEXCOORD1;
            half3 viewDir;
        };

        void CameraViewReflect (inout appdata_full v, out Input o) 
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            half4 pos = mul(UNITY_MATRIX_MV,v.vertex);

            half3 i = half3(0,0,0) - pos.xyz;
            half3 x = mul((float3x3)UNITY_MATRIX_MV, v.normal);
            half3 n = normalize(reflect(i, x));

            o.nv = float4(n, abs(dot(n, x)));
            //o.uv_MainTex = v.texcoord;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {            
            fixed4 MainTexCol = tex2D(_MainTex, IN.uv_MainTex);

            fixed4 offsetColor1 = tex2D(_NoiseTex, IN.uv_NoiseTex + _NoiseType);
            //half4 offsetColor2 = tex2D(_NoiseTex, IN.uv_NoiseTex - _NoiseType);

            IN.uv_NoiseTex.x += ((offsetColor1.r + offsetColor1.g) - 1) * _NoisePower;
            IN.uv_NoiseTex.y += ((offsetColor1.g - offsetColor1.b) - 1) * _NoisePower;
                      
            fixed4 NoiseTexCol = tex2Dproj( _NoiseTex, UNITY_PROJ_COORD(half4(IN.uv_NoiseTex.xy, IN.nv.xw)));
            NoiseTexCol.xyz *= _ReflColor;

            fixed rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
            fixed3 rimCol = pow(rim,_RimPower) * _RimLevel * MainTexCol.xyz * _ReflColor;            

            fixed3 FrozenCol = lerp(NoiseTexCol.xyz + rimCol, 0, saturate(pow(MainTexCol.a, _ReflPower))) * _ReflPower;
            
            o.Albedo = MainTexCol.xyz + FrozenCol;
            o.Alpha = lerp (((MainTexCol.a + NoiseTexCol.a) * _Transparent ), 1.0, MainTexCol.a);          
        }
    ENDCG    
    }

Fallback "VertexLit"

}