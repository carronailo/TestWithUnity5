// Toony Gooch Pro+Mobile Shaders
// (c) 2013, Jean Moreno

Shader "Cartoon/Specular_NoSelfShadow_NoLight+RimLight"
{
    Properties
	{
        _Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Gloss (A) ", 2D) = "white" {
            }
		_Multiply("Multiply", Range(0, 4)) = 1
		_Saturation("Saturation", Range(0, 5)) = 1

		//SPECULAR
		_SpecColor ("Specular Color", Color) = (0, 0, 0, 1)
		_SpecTex ("Specular / Reflection Mask", 2D) = "white" {
            }
		_SpecularPower ("Specular Power", Float) = 20
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125

		//BE HIT LIGHTING
		_BeHitColor("Be Hit Color", Color) = (0,0,0,0.5)

		//DISSOLVE
		_DissolveTex("Dissolve Texture(A)", 2D) = "white" {
            }
		_DissolveFactor("Dissolve Factor", Range(0, 1.01)) = 0

		//TRANSPARENT
		//_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.15)
		//_ColorMultiplier ("Color Multiplier", Float) = 1
		_AlphaCutoutSwitch("Use CutOut", Range(0, 1)) = 0
		_AlphaCutout("Alpha Cut Out", Range(0, 1)) = 0

		// RIM LIGHTING
		_RimSwitch("Use Rim", Range(-1, 1)) = -1
		_RimColor("Rim Color", Color) = (1, 0, 0, 1)
		_RimPower("Rim Power", Range(0.1, 2.0)) = 1.0
		_RimBase("Rim Base", Range(0, 1)) = 0.1
		_RimFrequency("Frequency", Float) = 1.0

		////OUTLINE
		//_OutlineSwitch("Use Outline", Range(-1, 1)) = -1
		//_Outline("Outline Width", Float) = 0.005
		//_OutlineColor("Outline Color", Color) = (1, 0, 0, 1)
		////Z CORRECT
		//_ZSmooth("Z Correction", Range(-3.0,3.0)) = -0.5

		_GlowTex("Glow", 2D) = "" {}
		_GlowColor("Glow Color", Color) = (1,1,1,1)
		_GlowStrength("Glow Strength", Float) = 0.0

	}
	
	SubShader
	{
        Tags {
            "Queue"="Geometry" "RenderType"="Glow11" "RenderEffect" = "Glow11" }
		LOD 300

		ColorMask RGB

		//Blend SrcAlpha OneMinusSrcAlpha
		
		UsePass "Hidden/Cartoon_Base/FORWARD"
		
		//UsePass "Hidden/Cartoon_Outlightting/OUTLIGHTTiNG_REVERSE"

		//UsePass "Hidden/Cartoon_Outline/OUTLINE_Z"

		UsePass "Hidden/Cartoon_RimLightting/RIMLIGHTING"

	} 	

	Fallback "Cartoon/Specular_NoSelfShadow_NoLight"

	CustomEditor "CustomQueueAndGlowMatInspector"

}
