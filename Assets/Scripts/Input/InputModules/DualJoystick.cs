using System;
using UnityEngine;

public class DualJoystick : InputModule, IAxisInputEventHandler, IKeyboardInputEventHandler, IMainJoystickInputEventProvider, ISecondaryJoystickInputEventProvider
{
	private JoystickInputData currentMainData;
	private JoystickInputData prevMainData;
	private JoystickInputData currentSecondaryData;
	private JoystickInputData prevSecondaryData;

	private void Awake()
	{
		currentMainData = new JoystickInputData { swayVector = Vector2.zero, swayDelta = Vector2.zero, status = EJoystickStatus.Invalid };
		prevMainData = currentMainData;
		currentSecondaryData = new JoystickInputData { swayVector = Vector2.zero, swayDelta = Vector2.zero, status = EJoystickStatus.Invalid };
		prevSecondaryData = currentSecondaryData;
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
		prevMainData = currentMainData;
		currentMainData.swayVector = Vector2.zero;
		currentMainData.swayDelta = Vector2.zero;
		currentMainData.status = EJoystickStatus.Invalid;
		prevSecondaryData = currentSecondaryData;
		currentSecondaryData.swayVector = Vector2.zero;
		currentSecondaryData.swayDelta = Vector2.zero;
		currentSecondaryData.status = EJoystickStatus.Invalid;
	}

	public object AquireLogicInputEvent(ELogicInputEventType eventType)
	{
		switch (eventType)
		{
			case ELogicInputEventType.MainJoystickSway:
				return currentMainData;
			case ELogicInputEventType.SecondaryJoystickSway:
				return currentSecondaryData;
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
	}

	public void HandleAxisMainVector(object eventData)
	{
		currentMainData.swayVector = ((Vector2)eventData);
		float mag = currentMainData.swayVector.magnitude;
		if (mag > 1f)
			currentMainData.swayVector /= mag;  // 不使用Unity提供的normalize方法，以节省开方运算次数，因为normalize方法里会再次计算magnitude
		currentMainData.swayDelta = currentMainData.swayVector - prevMainData.swayVector;
		if (currentMainData.swayVector == Vector2.zero)
			currentMainData.status = EJoystickStatus.Released;
		else if (currentMainData.swayDelta == Vector2.zero)
			currentMainData.status = EJoystickStatus.Holden;
		else
			currentMainData.status = EJoystickStatus.Swayed;
	}

	public void HandleAxisMainVertical(object eventData)
	{
	}

	public void HandleAxisSecondaryHorizontal(object eventData)
	{
	}

	public void HandleAxisSecondaryVector(object eventData)
	{
		currentSecondaryData.swayVector = ((Vector2)eventData);
		float mag = currentSecondaryData.swayVector.magnitude;
		if (mag > 1f)
			currentSecondaryData.swayVector /= mag;  // 不使用Unity提供的normalize方法，以节省开方运算次数，因为normalize方法里会再次计算magnitude
		currentSecondaryData.swayDelta = currentSecondaryData.swayVector - prevMainData.swayVector;
		if (currentSecondaryData.swayVector == Vector2.zero)
			currentSecondaryData.status = EJoystickStatus.Released;
		else if (currentSecondaryData.swayDelta == Vector2.zero)
			currentSecondaryData.status = EJoystickStatus.Holden;
		else
			currentSecondaryData.status = EJoystickStatus.Swayed;
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
