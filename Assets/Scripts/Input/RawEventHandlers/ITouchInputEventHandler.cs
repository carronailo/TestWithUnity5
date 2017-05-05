
internal interface ITouchInputEventHandler : IRawInputEventHandler
{
	void HandleTouchBegun(object eventData);
	void HandleTouchMoved(object eventData);
	void HandleTouchHolden(object eventData);
	void HandleTouchEnded(object eventData);
	void HandleTouchCanceled(object eventData);
}

