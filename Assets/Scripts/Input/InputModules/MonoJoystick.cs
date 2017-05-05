using UnityEngine;

public class MonoJoystick : InputModule, IAxisInputEventHandler, IKeyboardInputEventHandler, IMainJoystickInputEventProvider
{
	private JoystickInputData currentData;
	private JoystickInputData prevData;

	private void Awake()
	{
		currentData = new JoystickInputData { swayVector = Vector2.zero, swayDelta = Vector2.zero, status = EJoystickStatus.Invalid };
		prevData = currentData;
	}

	private void OnEnable()
	{
		InputSystem.RegisterRawInputEventHandler(this);
		InputSystem.RegisterLogicInputEventProvider(this);
	}

	private void OnDisable()
	{
		InputSystem.UnregisterRawInputEventHandler(this);
		InputSystem.UnregisterLogicInputEventProvider(this);
	}

	protected override void ResetAllFlags()
	{
		prevData = currentData;
		currentData.swayVector = Vector2.zero;
		currentData.swayDelta = Vector2.zero;
		currentData.status = EJoystickStatus.Invalid;
	}

	public object AquireLogicInputEvent(ELogicInputEventType eventType)
	{
		switch (eventType)
		{
			case ELogicInputEventType.MainJoystickSway:
				return currentData;
			default:
				return null;
		}
	}

	public void HandleAxisButtonHolden(object eventData)
	{
	}

	public void HandleAxisButtonPressed(object eventData)
	{
	}

	public void HandleAxisButtonReleased(object eventData)
	{
	}

	public void HandleAxisMainHorizontal(object eventData)
	{
		//currentData.swayVector = new Vector2((float)eventData, currentData.swayVector.y);
	}

	public void HandleAxisMainVector(object eventData)
	{
		currentData.swayVector = ((Vector2)eventData);
		float mag = currentData.swayVector.magnitude;
		if (mag > 1f)
			currentData.swayVector /= mag;	// 不使用Unity提供的normalize方法，以节省开方运算次数，因为normalize方法里会再次计算magnitude
		currentData.swayDelta = currentData.swayVector - prevData.swayVector;
		if (currentData.swayVector == Vector2.zero)
			currentData.status = EJoystickStatus.Released;
		else if (currentData.swayDelta == Vector2.zero)
			currentData.status = EJoystickStatus.Holden;
		else
			currentData.status = EJoystickStatus.Swayed;
	}

	public void HandleAxisMainVertical(object eventData)
	{
		//currentData.swayVector = new Vector2(currentData.swayVector.x, (float)eventData);
	}

	public void HandleAxisSecondaryHorizontal(object eventData)
	{
	}

	public void HandleAxisSecondaryVector(object eventData)
	{
	}

	public void HandleAxisSecondaryVertical(object eventData)
	{
	}

	public void HandleKeyHolden(object eventData)
	{
	}

	public void HandleKeyPressed(object eventData)
	{
	}

	public void HandleKeyReleased(object eventData)
	{
	}
}
