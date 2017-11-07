using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(TrailRenderer))]
public class ModifyMaterialForTrailRenderer : MonoBehaviour 
{
	private static int mainColorProperty = Shader.PropertyToID("_Color");
	private static int tintColorProperty = Shader.PropertyToID("_TintColor");

	[SerializeField]
	Color color1;
	[SerializeField]
	Color color2;
	[SerializeField]
	Color color3;
	[SerializeField]
	Color color4;
	[SerializeField]
	Color color5;

	Material[] materials;
	TrailRenderer trailRenderer = null;

	private void Awake()
	{
		trailRenderer = GetComponent<TrailRenderer>();
	}

	private void Update()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
			materials = trailRenderer.materials;
		else
			materials = trailRenderer.sharedMaterials;
#else
		materials = trailRenderer.materials;
#endif
		if (materials.Length >= 5)
		{
			if (materials[4].HasProperty(mainColorProperty))
				materials[4].SetColor(mainColorProperty, color5);
			else if (materials[4].HasProperty(tintColorProperty))
				materials[4].SetColor(tintColorProperty, color5);
		}
		if (materials.Length >= 4)
		{
			if (materials[3].HasProperty(mainColorProperty))
				materials[3].SetColor(mainColorProperty, color4);
			else if (materials[3].HasProperty(tintColorProperty))
				materials[3].SetColor(tintColorProperty, color4);
		}
		if (materials.Length >= 3)
		{
			if (materials[2].HasProperty(mainColorProperty))
				materials[2].SetColor(mainColorProperty, color3);
			else if (materials[2].HasProperty(tintColorProperty))
				materials[2].SetColor(tintColorProperty, color3);
		}
		if (materials.Length >= 2)
		{
			if (materials[1].HasProperty(mainColorProperty))
				materials[1].SetColor(mainColorProperty, color2);
			else if (materials[1].HasProperty(tintColorProperty))
				materials[1].SetColor(tintColorProperty, color2);
		}
		if (materials.Length >= 1)
		{
			if (materials[0].HasProperty(mainColorProperty))
				materials[0].SetColor(mainColorProperty, color1);
			else if (materials[0].HasProperty(tintColorProperty))
				materials[0].SetColor(tintColorProperty, color1);
		}
	}
}
