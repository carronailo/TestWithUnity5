
internal interface IKeyboardInputEventHandler : IRawInputEventHandler
{
	void HandleKeyPressed(object eventData);
	void HandleKeyHolden(object eventData);
	void HandleKeyReleased(object eventData);
}

