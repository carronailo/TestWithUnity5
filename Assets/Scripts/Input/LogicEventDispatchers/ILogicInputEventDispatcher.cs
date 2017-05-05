
public interface ILogicInputEventDispatcher
{
	void RegisterLogicInputEventHandler(ILogicInputEventProvider provider);
	void UnregisterLogicInputEventHandler(ILogicInputEventProvider provider);
	object AquireLogicInputEvent(ELogicInputEventType eventType);
}
