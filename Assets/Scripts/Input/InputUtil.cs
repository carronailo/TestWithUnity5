
public static class InputUtil
{
	public static ERawInputMetaType GetMetaTypeOfRawEventHandler(IRawInputEventHandler handler)
	{
		ERawInputMetaType result = ERawInputMetaType.Unknown;
		if (handler is ITouchInputEventHandler)
			result |= ERawInputMetaType.Touch;
		if (handler is IAxisInputEventHandler)
			result |= ERawInputMetaType.Axis;
		if (handler is IKeyboardInputEventHandler)
			result |= ERawInputMetaType.Keyboard;
		if (handler is IMouseInputEventHandler)
			result |= ERawInputMetaType.Mouse;
		return result;
	}

	public static ERawInputMetaType GetMetaTypeOfRawEvent(ERawInputEventType eventType)
	{
		if (eventType > ERawInputEventType.TOUCH_EVENT_BEGIN && eventType < ERawInputEventType.TOUCH_EVENT_END)
			return ERawInputMetaType.Touch;
		else if (eventType > ERawInputEventType.AXIS_EVENT_BEGIN && eventType < ERawInputEventType.AXIS_EVENT_END)
			return ERawInputMetaType.Axis;
		else if (eventType > ERawInputEventType.KEY_EVENT_BEGIN && eventType < ERawInputEventType.KEY_EVENT_END)
			return ERawInputMetaType.Keyboard;
		else if (eventType > ERawInputEventType.MOUSE_EVENT_BEGIN && eventType < ERawInputEventType.MOUSE_EVENT_END)
			return ERawInputMetaType.Mouse;
		else
			return ERawInputMetaType.Unknown;
	}

	public static ELogicInputMetaType GetMetaTypeOfLogicEventProvider(ILogicInputEventProvider provider)
	{
		ELogicInputMetaType result = ELogicInputMetaType.Unknown;
		if (provider is IMainJoystickInputEventProvider)
			result |= ELogicInputMetaType.MainJoytick;
		if (provider is ISecondaryJoystickInputEventProvider)
			result |= ELogicInputMetaType.SecondaryJoystick;
		return result;
	}

	public static ELogicInputMetaType GetMetaTypeOfLogicEvent(ELogicInputEventType eventType)
	{
		if (eventType > ELogicInputEventType.MAIN_JOYSTICK_EVENT_BEGIN && eventType < ELogicInputEventType.MAIN_JOYSTICK_EVENT_END)
			return ELogicInputMetaType.MainJoytick;
		else if (eventType > ELogicInputEventType.SECONDARY_JOYSTICK_EVENT_BEGIN && eventType < ELogicInputEventType.SECONDARY_JOYSTICK_EVENT_END)
			return ELogicInputMetaType.SecondaryJoystick;
		else
			return ELogicInputMetaType.Unknown;
	}

}
