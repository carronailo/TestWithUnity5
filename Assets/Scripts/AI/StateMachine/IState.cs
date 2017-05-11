using System.Collections.Generic;

public interface IState
{
	// 获取这个State上的所有Transition
	List<ITransition> GetAllTransitions();
	// State心跳
	void Update();
}
