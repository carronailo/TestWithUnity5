using UnityEngine;

public class MoveControl : MonoBehaviour
{
	private Animator anim;

	private void Awake()
	{
		anim = GetComponent<Animator>();
	}

	// Use this for initialization
	//void Start ()
	//{
	//	
	//}

	// Update is called once per frame
	void Update()
	{
		if(anim != null)
		{
			object data = InputSystem.GetLogicInputEvent(ELogicInputEventType.MainJoystickSway);
			if(data != null)
			{
				JoystickInputData inputData = (JoystickInputData)data;
				if (inputData.status == EJoystickStatus.Released)
					anim.SetFloat("Speed", 0f);
				else if (inputData.status == EJoystickStatus.Holden || inputData.status == EJoystickStatus.Swayed)
				{
					anim.SetFloat("Speed", inputData.swayVector.magnitude);
					// calculate actual running direction from controller parameters
					float releAngle = Vector2.Angle(Vector2.up, inputData.swayVector);
					// if angle from Vector.up to direction greater than 180 degree, make it negative
					if (inputData.swayVector.x < 0f)
						releAngle = -releAngle;
					anim.SetFloat("Turn", releAngle);
				}
			}
		}
	}
}
