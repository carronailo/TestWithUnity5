using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class ProjectorMaterialMainColor : MonoBehaviour
{
	public Color color;

	private Projector proj = null;

	void Awake()
	{
		proj = GetComponent<Projector>();
	}

	// Use this for initialization
	void Start()
	{
		if (proj != null && proj.material != null)
			proj.material.color = color;
	}

	// Update is called once per frame
	void Update()
	{
		if (proj != null && proj.material != null)
			proj.material.color = color;
	}

	// projector好像不需要手动删除材质球
	//void OnDestroy()
	//{
	//	if (proj != null)
	//		DestroyImmediate(proj.material);
	//}
}
