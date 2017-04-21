Shader "Cartoon/Outline_Only" 
{
	Properties 
	{
		//OUTLINE
		_Outline ("Outline Width", Range(0,0.05)) = 0.005
		_OutlineColor ("Outline Color", Color) = (0.2, 0.2, 0.2, 1)
		_ZSmooth ("Z Correction", Range(-3.0,3.0)) = -0.5
	}
	SubShader 
	{
		Tags { "Queue"="Geometry" "RenderType"="Opaque" }
		LOD 200
		UsePass "Hidden/Cartoon_Outline/OUTLINE_Z_REVERSE"
	} 
}
