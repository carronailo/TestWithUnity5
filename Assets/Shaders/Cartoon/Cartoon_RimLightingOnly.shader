Shader "Cartoon/RimLighting_Only" 
{
	Properties 
	{
		// RIM LIGHTING
		_RimSwitch("Use Rim", Range(-1, 1)) = -1
		_RimColor("Rim Color", Color) = (1, 0, 0, 1)
		_RimPower("Rim Power", Range(0.1, 2.0)) = 1.0
		_RimBase("Rim Base", Range(0, 1)) = 0.1
		_RimFrequency("Frequency", Float) = 1.0
	}
	SubShader 
	{
		Tags { "Queue"="Geometry" "RenderType"="Opaque" }
		LOD 200
		
		UsePass "Hidden/Cartoon_RimLightting/RIMLIGHTING"
	} 
	//FallBack "Diffuse"
}
