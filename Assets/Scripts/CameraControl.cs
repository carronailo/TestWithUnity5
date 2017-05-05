using UnityEngine;

public class CameraControl : MonoBehaviour
{
	[Range(1f, 12f)]
	public float cameraXSensitivity = 6f;
	public bool reverseX = false;
	[Range(1f, 12f)]
	public float cameraYSensitivity = 6f;
	public bool reverseY = true;

	// Use this for initialization
	//void Start ()
	//{
	//	
	//}

	// Update is called once per frame
	void LateUpdate()
	{
		object data = InputSystem.GetLogicInputEvent(ELogicInputEventType.SecondaryJoystickSway);
		if(data != null)
		{
			JoystickInputData inputData = (JoystickInputData)data;
			if (inputData.status == EJoystickStatus.Holden || inputData.status == EJoystickStatus.Swayed)
			{
				Vector3 temp =
					new Vector3(
						transform.eulerAngles.x + inputData.swayVector.y * cameraYSensitivity * (reverseY ? -1f : 1f),
						transform.eulerAngles.y + inputData.swayVector.x * cameraXSensitivity * (reverseX ? -1f : 1f),
						transform.eulerAngles.z
						);
				transform.eulerAngles = temp;
			}
		}
		//if (inputData.status == EJoystickStatus.Released)
		//	anim.SetFloat("Speed", 0f);
		//else if (inputData.status == EJoystickStatus.Holden || inputData.status == EJoystickStatus.Swayed)
		//{
		//	anim.SetFloat("Speed", inputData.swayVector.magnitude);
		//	// calculate actual running direction from controller parameters
		//	float releAngle = Vector2.Angle(Vector2.up, inputData.swayVector);
		//	// if angle from Vector.up to direction greater than 180 degree, make it negative
		//	if (inputData.swayVector.x < 0f)
		//		releAngle = -releAngle;
		//	anim.SetFloat("Turn", releAngle);
		//}
	}
}
