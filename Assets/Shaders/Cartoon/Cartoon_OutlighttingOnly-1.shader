Shader "Cartoon/Outlightting_Only-1"
{
	Properties 
	{
		//OUTLIGHTTING
		_Outlightting ("Outlightting Size", Range(0,0.1)) = 0.005
		_OutlighttingColor ("Outlightting Color", Color) = (0.2, 0.2, 0.2, 1)
        _Falloff("Falloff", Float) = 5
		_Transparency("Transparency", Float) = 15
	}
	SubShader
	{
		Tags { "Queue"="Geometry-1" "RenderType"="Opaque" }
		LOD 200
		UsePass "Hidden/Cartoon_Outlightting/OUTLIGHTTING_REVERSE"
	}

	//Fallback "Mobile/VertexLit"
}
