using UnityEngine;
using System.Collections;

public class RandomShake : MonoBehaviour
{
	public float radius;
	public int shakeIntervalFrame;
	public bool for3D = false;

	private int frameSinceLastShake = 0;
	private Vector3 oriPosition;
	private Transform myTransform = null;

	void Awake()
	{
		myTransform = transform;
	}

	// Use this for initialization
	void Start()
	{
		oriPosition = transform.localPosition;
	}

	// Update is called once per frame
	void Update()
	{
		if (frameSinceLastShake >= shakeIntervalFrame)
		{
			if (for3D)
			{
				Vector3 tmp = Random.insideUnitSphere * radius;
				myTransform.localPosition = oriPosition + tmp;
			}
			else
			{
				Vector2 tmp = Random.insideUnitCircle * radius;
				myTransform.localPosition = oriPosition + new Vector3(tmp.x, tmp.y);
			}
			frameSinceLastShake = 0;
		}
		else
			frameSinceLastShake++;
	}
}
