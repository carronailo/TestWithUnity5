using UnityEngine;
using System.Collections;

public class RotateAroundAxis : MonoBehaviour
{
	public float rotateSpeed = 10f;
	public Space reletiveSpace = Space.Self;
	public bool x = false;
	public bool y = false;
	public bool z = false;

	// Update is called once per frame
	void Update()
	{
		if (x)
		{
			transform.Rotate(Vector3.right, rotateSpeed * Time.deltaTime, reletiveSpace);
		}
		if (y)
		{
			transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, reletiveSpace);
		}
		if (z)
		{
			transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime, reletiveSpace);
		}
	}
}
