using UnityEngine;

public class RendererMaterialColor : MonoBehaviour
{
	public Color color;
	public string propertyName;

	private Renderer[] rdrs = null;
	private int propertyID;

	void Awake()
	{
		rdrs = GetComponentsInChildren<Renderer>();
		propertyID = Shader.PropertyToID(propertyName);
	}

	// Use this for initialization
	void Start()
	{
		if (rdrs != null)
		{
			for (int i = 0; i < rdrs.Length; ++i)
			{
				if (rdrs[i] != null && rdrs[i].material != null)
					rdrs[i].material.SetColor(propertyID, color);
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (rdrs != null)
		{
			for (int i = 0; i < rdrs.Length; ++i)
			{
				if (rdrs[i] != null && rdrs[i].material != null)
					rdrs[i].material.SetColor(propertyID, color);
			}
		}
	}

	void OnDestroy()
	{
		if (rdrs != null)
		{
			for (int i = 0; i < rdrs.Length; ++i)
			{
				if (rdrs[i] != null)
					DestroyImmediate(rdrs[i].material);
			}
		}
	}
}
