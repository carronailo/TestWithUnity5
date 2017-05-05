using System.Collections.Generic;

internal class DefaultRawInputEventDispatcher : IRawInputEventDispatcher
{
	// 一种类型的原始输入事件可以存在多个处理者
	private Dictionary<ERawInputMetaType, List<IRawInputEventHandler>> handlerTable = new Dictionary<ERawInputMetaType, List<IRawInputEventHandler>>();

	public void DispatchRawInputEvent(ERawInputEventType eventType, object eventData)
	{
		ERawInputMetaType metaType = InputUtil.GetMetaTypeOfRawEvent(eventType);
		switch (metaType)
		{
			case ERawInputMetaType.Touch:
				_DispatchTouchRawInputEvent(eventType, eventData);
				break;
			case ERawInputMetaType.Axis:
				_DispatchAxisRawInputEvent(eventType, eventData);
				break;
			case ERawInputMetaType.Keyboard:
				_DispatchKeyboardRawInputEvent(eventType, eventData);
				break;
			case ERawInputMetaType.Mouse:
				_DispatchMouseRawInputEvent(eventType, eventData);
				break;
			default:
				break;
		}
	}

	public void RegisterRawInputEventHandler(IRawInputEventHandler handler)
	{
		ERawInputMetaType metaType = InputUtil.GetMetaTypeOfRawEventHandler(handler);
		if (metaType != ERawInputMetaType.Unknown)
		{
			if((metaType & ERawInputMetaType.Touch) != 0)
				CollectionUtil.AddIntoTable(ERawInputMetaType.Touch, handler, handlerTable, true);
			if ((metaType & ERawInputMetaType.Axis) != 0)
				CollectionUtil.AddIntoTable(ERawInputMetaType.Axis, handler, handlerTable, true);
			if ((metaType & ERawInputMetaType.Keyboard) != 0)
				CollectionUtil.AddIntoTable(ERawInputMetaType.Keyboard, handler, handlerTable, true);
			if ((metaType & ERawInputMetaType.Mouse) != 0)
				CollectionUtil.AddIntoTable(ERawInputMetaType.Mouse, handler, handlerTable, true);
		}
	}

	public void UnregisterRawInputEventHandler(IRawInputEventHandler handler)
	{
		ERawInputMetaType metaType = InputUtil.GetMetaTypeOfRawEventHandler(handler);
		if (metaType != ERawInputMetaType.Unknown)
		{
			if ((metaType & ERawInputMetaType.Touch) != 0)
				CollectionUtil.RemoveFromTable(ERawInputMetaType.Touch, handler, handlerTable);
			if ((metaType & ERawInputMetaType.Axis) != 0)
				CollectionUtil.RemoveFromTable(ERawInputMetaType.Axis, handler, handlerTable);
			if ((metaType & ERawInputMetaType.Keyboard) != 0)
				CollectionUtil.RemoveFromTable(ERawInputMetaType.Keyboard, handler, handlerTable);
			if ((metaType & ERawInputMetaType.Mouse) != 0)
				CollectionUtil.RemoveFromTable(ERawInputMetaType.Mouse, handler, handlerTable);
		}
	}

	private void _DispatchTouchRawInputEvent(ERawInputEventType eventType, object eventData)
	{
		List<IRawInputEventHandler> handlerList = null;
		handlerTable.TryGetValue(ERawInputMetaType.Touch, out handlerList);
		if (handlerList == null)
			return;
		for (int i = 0; i < handlerList.Count; ++i)
		{
			ITouchInputEventHandler handler = handlerList[i] as ITouchInputEventHandler;
			if (handler == null)
				continue;
			switch (eventType)
			{
				case ERawInputEventType.TouchBegun:
					handler.HandleTouchBegun(eventData);
					break;
				case ERawInputEventType.TouchMoved:
					handler.HandleTouchMoved(eventData);
					break;
				case ERawInputEventType.TouchHolden:
					handler.HandleTouchHolden(eventData);
					break;
				case ERawInputEventType.TouchEnded:
					handler.HandleTouchEnded(eventData);
					break;
				case ERawInputEventType.TouchCanceled:
					handler.HandleTouchCanceled(eventData);
					break;
				default:
					break;
			}
		}
	}

	private void _DispatchAxisRawInputEvent(ERawInputEventType eventType, object eventData)
	{
		List<IRawInputEventHandler> handlerList = null;
		handlerTable.TryGetValue(ERawInputMetaType.Axis, out handlerList);
		if (handlerList == null)
			return;
		for (int i = 0; i < handlerList.Count; ++i)
		{
			IAxisInputEventHandler handler = handlerList[i] as IAxisInputEventHandler;
			if (handler == null)
				continue;
			switch (eventType)
			{
				case ERawInputEventType.AxisMainHorizontal:
					handler.HandleAxisMainHorizontal(eventData);
					break;
				case ERawInputEventType.AxisMainVertical:
					handler.HandleAxisMainVertical(eventData);
					break;
				case ERawInputEventType.AxisMainVector:
					handler.HandleAxisMainVector(eventData);
					break;
				case ERawInputEventType.AxisSecondaryHorizontal:
					handler.HandleAxisSecondaryHorizontal(eventData);
					break;
				case ERawInputEventType.AxisSecondaryVertical:
					handler.HandleAxisSecondaryVertical(eventData);
					break;
				case ERawInputEventType.AxisSecondaryVector:
					handler.HandleAxisSecondaryVector(eventData);
					break;
				case ERawInputEventType.AxisButtonPressed:
					handler.HandleAxisButtonPressed(eventData);
					break;
				case ERawInputEventType.AxisButtonHolden:
					handler.HandleAxisButtonHolden(eventData);
					break;
				case ERawInputEventType.AxisButtonReleased:
					handler.HandleAxisButtonReleased(eventData);
					break;
				default:
					break;
			}
		}
	}

	private void _DispatchKeyboardRawInputEvent(ERawInputEventType eventType, object eventData)
	{
		List<IRawInputEventHandler> handlerList = null;
		handlerTable.TryGetValue(ERawInputMetaType.Keyboard, out handlerList);
		if (handlerList == null)
			return;
		for (int i = 0; i < handlerList.Count; ++i)
		{
			IKeyboardInputEventHandler handler = handlerList[i] as IKeyboardInputEventHandler;
			if (handler == null)
				continue;
			switch (eventType)
			{
				case ERawInputEventType.KeyPressed:
					handler.HandleKeyPressed(eventData);
					break;
				case ERawInputEventType.KeyHolden:
					handler.HandleKeyHolden(eventData);
					break;
				case ERawInputEventType.KeyReleased:
					handler.HandleKeyReleased(eventData);
					break;
				default:
					break;
			}
		}
	}

	private void _DispatchMouseRawInputEvent(ERawInputEventType eventType, object eventData)
	{
		List<IRawInputEventHandler> handlerList = null;
		handlerTable.TryGetValue(ERawInputMetaType.Mouse, out handlerList);
		if (handlerList == null)
			return;
		for (int i = 0; i < handlerList.Count; ++i)
		{
			IMouseInputEventHandler handler = handlerList[i] as IMouseInputEventHandler;
			if (handler == null)
				continue;
			switch (eventType)
			{
				case ERawInputEventType.MouseButtonPressed:
					handler.HandleMouseButtonPressed(eventData);
					break;
				case ERawInputEventType.MouseButtonHolden:
					handler.HandleMouseButtonHolden(eventData);
					break;
				case ERawInputEventType.MouseButtonReleased:
					handler.HandleMouseButtonReleased(eventData);
					break;
				default:
					break;
			}
		}
	}
}
