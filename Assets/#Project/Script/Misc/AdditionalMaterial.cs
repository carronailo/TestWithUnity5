using System.Collections;
using UnityEngine;

public enum EMaterialParameterType
{
	数值,
	颜色,
	无,
}

public class AdditionalMaterial : MonoBehaviour
{
	public Material mat;
	public string paramName;
	public EMaterialParameterType paramType;
	public float paramFloatValue;
	public Color paramColorValue;

	public System.Action onMaterialApplyed = null;

	private Renderer rdr = null;
	private Material tempMat = null;

	IEnumerator Start()
	{
		rdr = GetComponentInParent<Renderer>();
		while (rdr == null)
		{
			yield return null;
			rdr = GetComponentInParent<Renderer>();
		}
		if (rdr != null && mat != null)
		{
			mat.shader = Shader.Find(mat.shader.name);
			Material[] mats = rdr.materials;
			System.Array.Resize(ref mats, mats.Length + 1);
			mats[mats.Length - 1] = mat;
			rdr.materials = mats;
			tempMat = rdr.materials[mats.Length - 1];
			if (onMaterialApplyed != null)
				onMaterialApplyed();
		}
	}

	void Update()
	{
		if (rdr != null && tempMat != null)
		{
			switch (paramType)
			{
				case EMaterialParameterType.数值:
					tempMat.SetFloat(paramName, paramFloatValue);
					break;
				case EMaterialParameterType.颜色:
					tempMat.SetColor(paramName, paramColorValue);
					break;
			}
		}
	}

	void OnDisable()
	{
		if (rdr != null && tempMat != null)
		{
			Material[] mats = rdr.materials;
			if(mats.Length > 0)
			{
				int index = -1;
				for(int i = 0;i < mats.Length; ++i)
				{
					if(mats[i] == tempMat)
					{
						index = i;
						break;
					}
				}
				if(index >= 0)
				{
					for(int i = index; i < mats.Length - 1; ++i)
					{
						mats[i] = mats[i + 1];
					}
					System.Array.Resize(ref mats, mats.Length - 1);
				}
			}
			rdr.materials = mats;
			Destroy(tempMat);
		}
	}

}
