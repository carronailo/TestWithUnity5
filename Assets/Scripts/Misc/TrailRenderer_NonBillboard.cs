using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class TrailKeyData
{
	public Vector3 point;
	public Vector3 upDir;
	public float time;
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TrailRenderer_NonBillboard : MonoBehaviour
{
	public bool alwaysRender;		// 总是渲染，没有渐隐渐显，也不需要接收开始渲染和结束渲染的消息

	public float fadeTime;			// 渐隐和渐显的持续时间
	public int sampleRate;			// 一秒钟之内采样的次数，数值约合为一秒钟生成的模型三角面数量的二分之一大小
	public float startSize;			// 刀光片的高度，或者说刀光的宽度（起始尺寸）
	public float endSize;			// 刀光片的高度，或者说刀光的宽度（终止尺寸）
	public Color[] colorsSequence;	// 刀光片颜色渐变序列
	public float trailDuration;		// 刀光片持续存在时间
	public float minVectorDistance;	// 刀光片上相邻两个顶点的最小距离，小于此距离时不作采样
	public float durationDamping;	// 刀光片持续时间的衰减速度，单位为（秒每秒），即每秒钟持续时间衰减N秒

	public bool usedForCollider;
	public bool autoDestroy;		// 刀光消失后自动销毁

	private Color defaultStartColor = new Color(1f, 1f, 1f, 1f);
	private Color defaultEndColor = new Color(1f, 1f, 1f, 0f);
	//private float durationThreshold = 0.0167f;

	//private float time = 0f;
	private MeshFilter meshFilter = null;
	private Mesh mesh;
	private LinkedList<TrailKeyData> trailKeys = new LinkedList<TrailKeyData>();
	private Vector3 lastPosition;
	private Vector3 lastUpDir;
	public float desireDuration;
	public float duration;

	//private bool recordTrail = false;

	void Awake()
	{
		meshFilter = GetComponent<MeshFilter>();
		mesh = new Mesh();
		meshFilter.mesh = mesh;
	}

	// Use this for initialization
	void Start()
	{
		CheckColors();
		duration = 0f;
		if (alwaysRender)
		{
			desireDuration = trailDuration;
			lastPosition = transform.position;
			lastUpDir = transform.TransformDirection(Vector3.up);
		}
	}

	// Update is called once per frame
	void Update()
	{
		RefreshDuration();
		//if (alwaysRender || recordTrail)
		//{
		if (duration > 0f)
			RecordTrail();
		else
			ClearTrail();
		//}
		UpdateTrail();
	}

	void CheckColors()
	{
		if (colorsSequence.Length < 2)
		{
			colorsSequence = new Color[2];
			colorsSequence[0] = defaultStartColor;
			colorsSequence[1] = defaultEndColor;
		}
	}

	void RecordTrail()
	{
		Vector3 position = transform.position;
		Vector3 upDir = transform.TransformDirection(Vector3.up);
		float now = Time.time;
		float sampleTimeDelta = 1f / sampleRate;
		float deltaTime = Time.deltaTime;
		float lastFrameTime = now - deltaTime;
		float time = 0f;
		int Count = 0;
		while (time < deltaTime)
		{
			time = Mathf.Min(time + sampleTimeDelta, deltaTime);
			if (trailKeys.Count == 0 || (trailKeys.First.Value.point - position).sqrMagnitude > minVectorDistance * minVectorDistance)
			{
				TrailKeyData key = new TrailKeyData();
				float f = time / deltaTime;
				key.point = Vector3.Slerp(lastPosition, position, f);
				key.upDir = Vector3.Slerp(lastUpDir, upDir, f);
				key.time = lastFrameTime + time;
				trailKeys.AddFirst(key);
			}
			++Count;
		}
		while (trailKeys.Count > 0 && now > trailKeys.Last.Value.time + duration)
			trailKeys.RemoveLast();
		//Debug.Log(Count + "::" + trailKeys.Count + "=====deltaTime" + deltaTime + "sampleTimeDelta" + sampleTimeDelta + "duration" + duration);

		lastPosition = position;
		lastUpDir = upDir;
	}

	void UpdateTrail()
	{
		mesh.Clear();
		if (trailKeys.Count < 2)
			return;

		float now = Time.time;
		float colorLerpDelta = 1f / (colorsSequence.Length - 1);

		Vector3[] vertices = new Vector3[trailKeys.Count * 2];
		Color[] colors = new Color[trailKeys.Count * 2];
		Vector2[] uvs = new Vector2[trailKeys.Count * 2];

		//TrailKeyData currentKey = trailKeys.First.Value;
		//
		// Use matrix instead of transform.TransformPoint for performance reasons
		//Matrix4x4 localSpaceTransform = transform.worldToLocalMatrix;

		int index = 0;
		// Generate vertex, uv and colors
		foreach (TrailKeyData key in trailKeys)
		{
			// Calculate u for texture uv and color interpolation
			float u  = 0f;
			if(index > 0) 
				u = Mathf.Clamp01((now - key.time) / duration);
			//
			// Calculate upwards direction
			Vector3 upDir = key.upDir;

			// Generate vertices
			//vertices[index * 2 + 0] = localSpaceTransform.MultiplyPoint(key.point);
			//vertices[index * 2 + 1] = localSpaceTransform.MultiplyPoint(key.point + upDir * Mathf.Lerp(startSize, endSize, u));
			vertices[index * 2 + 0] = transform.InverseTransformPoint(key.point);
			vertices[index * 2 + 1] = transform.InverseTransformPoint(key.point + upDir * Mathf.Lerp(startSize, endSize, u));

			uvs[index * 2 + 0] = new Vector2(u, 0);
			uvs[index * 2 + 1] = new Vector2(u, 1);

			// fade colors out over time
			float colorLerpFactor = u / colorLerpDelta;
			int startColorIndex = Mathf.FloorToInt(colorLerpFactor);
			int endColorIndex = Mathf.CeilToInt(colorLerpFactor);
			colorLerpFactor = colorLerpFactor - startColorIndex;
			Color interpolatedColor = Color.Lerp(colorsSequence[startColorIndex], colorsSequence[endColorIndex], colorLerpFactor);
			colors[index * 2 + 0] = interpolatedColor;
			colors[index * 2 + 1] = interpolatedColor;

			++index;
		}
		// Generate triangles indices
		int[] triangles = new int[(trailKeys.Count - 1) * 2 * 3];
		for (int i = 0; i < triangles.Length / 6; i++)
		{
			triangles[i * 6 + 0] = i * 2;
			triangles[i * 6 + 1] = i * 2 + 1;
			triangles[i * 6 + 2] = i * 2 + 2;

			triangles[i * 6 + 3] = i * 2 + 2;
			triangles[i * 6 + 4] = i * 2 + 1;
			triangles[i * 6 + 5] = i * 2 + 3;
		}

		// Assign to mesh	
		//mesh.MarkDynamic();
		mesh.vertices = vertices;
		mesh.colors = colors;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		;
		if (usedForCollider)
		{
			//(collider as MeshCollider).sharedMesh = null;
			(GetComponent<Collider>() as MeshCollider).sharedMesh = mesh;
		}
	}

	void RefreshDuration()
	{
		if (duration >= desireDuration)
		{
			// duration往下减
			duration = Mathf.Max(duration - Time.deltaTime * durationDamping, desireDuration);
		}
		else
		{
			// duration往上加
			duration = Mathf.Min(duration + Time.deltaTime * durationDamping, desireDuration);
		}
	}

	void ClearTrail()
	{
		duration = 0f;
		if (mesh != null)
		{
			mesh.Clear();
			trailKeys.Clear();
		}
		if (autoDestroy)
			Destroy(gameObject);
	}

	public void StartRender()
	{
		desireDuration = trailDuration;
		lastPosition = transform.position;
		lastUpDir = transform.TransformDirection(Vector3.up);
		//recordTrail = true;
	}

	public void StopRender()
	{
		desireDuration = 0f;
		//recordTrail = false;
	}
}
