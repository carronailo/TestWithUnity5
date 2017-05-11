using System.Collections.Generic;

public interface ITransition
{
	// 获取这个Transition上所有的Condition
	List<ICondition> GetAllConditions();
	// 检查Transition上的所有Condition是否都已经满足
	bool CheckConditions();
	// 沿此Transition做状态迁移，并返回迁移后的新的状态
	IState Transit();
}
