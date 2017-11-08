using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class TillingLineRenderer : MonoBehaviour
{
	public enum ERatioDirection
	{
		X,
		Y,
	}

	public Vector3 startPos;
	public Vector3 endPos;
	public float ratio;
	public ERatioDirection rationDirection;

	private LineRenderer lineRenderer;

	void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();
	}

	void Update()
	{
		lineRenderer.SetPosition(0, startPos);
		lineRenderer.SetPosition(1, endPos);
		float length = Vector3.Distance(startPos, endPos);
		switch (rationDirection)
		{
			case ERatioDirection.X:
				lineRenderer.material.SetTextureScale("_MainTex", new Vector2(ratio * length, 1f));
				break;
			case ERatioDirection.Y:
				lineRenderer.material.SetTextureScale("_MainTex", new Vector2(1f, ratio * length));
				break;
		}
	}
}
