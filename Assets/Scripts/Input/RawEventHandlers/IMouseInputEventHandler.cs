
internal interface IMouseInputEventHandler : IRawInputEventHandler
{
	void HandleMouseButtonPressed(object eventData);
	void HandleMouseButtonHolden(object eventData);
	void HandleMouseButtonReleased(object eventData);
}
