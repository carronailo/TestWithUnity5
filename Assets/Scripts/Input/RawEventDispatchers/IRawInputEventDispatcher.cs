
public interface IRawInputEventDispatcher
{
	void RegisterRawInputEventHandler(IRawInputEventHandler handler);
	void UnregisterRawInputEventHandler(IRawInputEventHandler handler);
	void DispatchRawInputEvent(ERawInputEventType eventType, object eventData);
}

