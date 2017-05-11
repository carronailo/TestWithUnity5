using System.Collections.Generic;

public interface IStateMachineDriver
{
	// 获取StateMachine上的所有State
	List<IState> GetAllStates();
	// 获取StateMachine上的起始State
	IState GetInitialState();
	// 开始驱动StateMachine
	void StartMachine();
}
