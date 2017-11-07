using UnityEngine;
using System.Collections;

public class UVAnimation : MonoBehaviour
{
	public Material uvMaterial;

	public float xOffsetSpeed;
	public float yOffsetSpeed;

	public int circle = 0;
	public float delay;

	public Vector2 initialOffset4MainTex = Vector2.zero;
	public Vector2 initialOffset4DetailTex = Vector2.zero;
	public bool resetTexOffsetEvertyTime = false;

	private bool paused = false;

	private float originalXOffsetSpeed;
	private float originalYOffsetSpeed;

	private Material mat;
	private int xCircle = 0;
	private int yCircle = 0;
	private float time = 0f;

	void Awake()
	{
		originalXOffsetSpeed = xOffsetSpeed;
		originalYOffsetSpeed = yOffsetSpeed;
	}

	void OnEnable()
	{
		Reset();
	}

	void Start()
	{
		if (uvMaterial == null)
			mat = GetComponent<Renderer>().material;
		else
			mat = uvMaterial;
		Reset();
		//if (circle > 0)
		//{
		//	// 如果需要按特定循环圈数进行走UV，那么在起始的时候要把UV偏移归零
		//	xCircle = circle;
		//	yCircle = circle;
		//	mat.SetTextureOffset("_MainTex", Vector2.zero);
		//	if (mat.HasProperty("_DetailTex"))
		//		mat.SetTextureOffset("_DetailTex", Vector2.zero);
		//}
	}

	// Update is called once per frame
	void Update()
	{
		if (paused)
			return;
		time += Time.deltaTime;
		if (time <= delay)
			return;
		Vector2 currOffset = mat.GetTextureOffset("_MainTex");
		Vector2 offset = new Vector2(currOffset.x + (xCircle > 0f ? Time.deltaTime * xOffsetSpeed : 0f), currOffset.y + (yCircle > 0f ? Time.deltaTime * yOffsetSpeed : 0f));
		if (offset.x > 1f)
		{
			offset.x = offset.x - 1f;
			--xCircle;
		}
		else if (offset.x < -1f)
		{
			offset.x = offset.x + 1f;
			--xCircle;
		}
		if (offset.y > 1f)
		{
			offset.y = offset.y - 1f;
			--yCircle;
		}
		else if (offset.y < -1f)
		{
			offset.y = offset.y + 1f;
			--yCircle;
		}
		//offset.x = offset.x > 1f ? (offset.x - 1f) : (offset.x < -1f ? (offset.x + 1f) : offset.x);
		//offset.y = offset.y > 1f ? (offset.y - 1f) : (offset.y < -1f ? (offset.y + 1f) : offset.y);
		mat.SetTextureOffset("_MainTex", offset);
		if (mat.HasProperty("_DetailTex"))
			mat.SetTextureOffset("_DetailTex", offset);
	}

	void OnDestroy()
	{
		if (uvMaterial == null)
			Destroy(mat);
	}

	public void Reset()
	{
		time = 0f;
		if (circle > 0)
		{
			// 如果需要按特定循环圈数进行走UV，那么在起始的时候要把UV偏移归零
			xCircle = circle;
			yCircle = circle;
			if (mat != null)
			{
				mat.SetTextureOffset("_MainTex", initialOffset4MainTex);
				if (mat.HasProperty("_DetailTex"))
					mat.SetTextureOffset("_DetailTex", initialOffset4DetailTex);
			}
		}
		else
		{
			xCircle = int.MaxValue;
			yCircle = int.MaxValue;
			if (resetTexOffsetEvertyTime) 
			{
				if (mat != null)
				{
					mat.SetTextureOffset("_MainTex", initialOffset4MainTex);
					if (mat.HasProperty("_DetailTex"))
						mat.SetTextureOffset("_DetailTex", initialOffset4DetailTex);
				}
			}
		}
		paused = false;
	}

	public void Pause()
	{
		paused = true;
	}

	public void Resume()
	{
		paused = false;
	}

	public void TuneSpeed(float factor)
	{
		xOffsetSpeed = originalXOffsetSpeed * factor;
		yOffsetSpeed = originalYOffsetSpeed * factor;
	}
}
