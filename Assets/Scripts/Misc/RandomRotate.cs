using UnityEngine;

public class RandomRotate : MonoBehaviour
{
	public int angleStep = 30;
	public bool x;
	public bool y;
	public bool z;
	public bool useBaseAngle;

	private void Start()
	{
		RotateIt();
	}

	private void OnDisable()
	{
		RotateIt();
	}

	void RotateIt()
	{
		int randomRange = Mathf.CeilToInt(360f / angleStep);
		int rand = Random.Range(0, randomRange);
		if (x)
		{
			Vector3 angle = transform.rotation.eulerAngles;
			if (useBaseAngle)
				transform.rotation = Quaternion.Euler(new Vector3(angle.x + angleStep * rand, angle.y, angle.z));
			else
				transform.rotation = Quaternion.Euler(new Vector3(angleStep * rand, angle.y, angle.z));
		}
		if (y)
		{
			Vector3 angle = transform.rotation.eulerAngles;
			if (useBaseAngle)
				transform.rotation = Quaternion.Euler(new Vector3(angle.x, angle.y + angleStep * rand, angle.z));
			else
				transform.rotation = Quaternion.Euler(new Vector3(angle.x, angleStep * rand, angle.z));
		}
		if (z)
		{
			Vector3 angle = transform.rotation.eulerAngles;
			if (useBaseAngle)
				transform.rotation = Quaternion.Euler(new Vector3(angle.x, angle.y, angle.z + angleStep * rand));
			else
				transform.rotation = Quaternion.Euler(new Vector3(angle.x, angle.y, angleStep * rand));
		}
	}
}
