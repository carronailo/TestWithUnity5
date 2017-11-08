using UnityEngine;

[System.Serializable]
public class MaterialParameter
{
	public string paramName;
	public EMaterialParameterType paramType;
	public float paramFloatValue;
	public Color paramColorValue;
}

public class AdditionalSkinMaterial : MonoBehaviour
{
	public Material mat;
	public MaterialParameter[] parameters;

	private SkinnedMeshRenderer[] rdrs = null;
	private Material[] tempMats = null;

	void Start()
	{
		if(transform.parent != null)
		{
			rdrs = transform.parent.GetComponentsInChildren<SkinnedMeshRenderer>();
			if (rdrs == null || rdrs.Length <= 0)
				rdrs = transform.root.GetComponentsInChildren<SkinnedMeshRenderer>();
			if (rdrs != null && mat != null)
			{
				tempMats = new Material[rdrs.Length];
				for (int i = 0; i < rdrs.Length; ++i)
				{
					Renderer rdr = rdrs[i];
					mat.shader = Shader.Find(mat.shader.name);
					Material[] mats = rdr.materials;
					System.Array.Resize(ref mats, mats.Length + 1);
					mats[mats.Length - 1] = mat;
					rdr.materials = mats;
					tempMats[i] = rdr.materials[mats.Length - 1];
				}
			}
		}
	}

	void Update()
	{
		if (rdrs != null && tempMats != null && parameters != null)
		{
			for(int i = 0; i < tempMats.Length; ++i)
			{
				for(int j = 0; j < parameters.Length; ++j)
				{
					MaterialParameter param = parameters[j];
					switch (param.paramType)
					{
						case EMaterialParameterType.数值:
							tempMats[i].SetFloat(param.paramName, param.paramFloatValue);
							break;
						case EMaterialParameterType.颜色:
							tempMats[i].SetColor(param.paramName, param.paramColorValue);
							break;
					}
				}
			}
		}
	}

	void OnDestroy()
	{
		if (rdrs != null && tempMats != null)
		{
			for(int i = 0; i < rdrs.Length; ++i)
			{
				Renderer rdr = rdrs[i];
				Material[] mats = rdr.materials;
				if (mats.Length > 0)
				{
					Material m = mats[mats.Length - 1];
					System.Array.Resize(ref mats, mats.Length - 1);
					Destroy(m);
				}
				rdr.materials = mats;
			}
		}
	}

}
