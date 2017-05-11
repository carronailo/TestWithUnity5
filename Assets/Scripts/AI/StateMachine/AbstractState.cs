using System.Collections.Generic;

public abstract class AbstractState
{
	protected List<ITransition> allTransitions = new List<ITransition>();
}
