
internal interface IAxisInputEventHandler : IRawInputEventHandler
{
	void HandleAxisMainHorizontal(object eventData);
	void HandleAxisMainVertical(object eventData);
	void HandleAxisMainVector(object eventData);
	void HandleAxisSecondaryHorizontal(object eventData);
	void HandleAxisSecondaryVertical(object eventData);
	void HandleAxisSecondaryVector(object eventData);
	void HandleAxisButtonPressed(object eventData);
	void HandleAxisButtonHolden(object eventData);
	void HandleAxisButtonReleased(object eventData);
}

