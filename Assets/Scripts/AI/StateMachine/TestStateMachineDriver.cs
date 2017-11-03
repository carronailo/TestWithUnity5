using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class TestStateMachineDriver : AbstractStateMachineDriver
{
	private bool started = false;
	private IState currentState;

	private void Start()
	{
		// 初始化，至少需要获得StateMachine的起始State
		currentState = initialState;
	}

	private void Update()
	{
		if (!started)
			return;
		if(currentState != null)
		{
			currentState.Update();
		}
	}

	public override void StartMachine()
	{
		started = true;
	}

#if UNITY_EDITOR
	[ContextMenu("Test")]
	public void Test()
	{
		StateMachineTemplate template = ScriptableObject.CreateInstance<StateMachineTemplate>();
		template.allStates = new List<BaseState>();
		AdvancedState advancedState = new AdvancedState();
		AnotherState anotherState = new AnotherState();
		AnotherAdvancedState anotherAdvancedState = new AnotherAdvancedState();
		BaseState baseState = new BaseState();
		template.allStates.Add(advancedState);
		template.allStates.Add(anotherState);
		template.allStates.Add(anotherAdvancedState);
		template.allStates.Add(baseState);
		AssetDatabase.CreateAsset(template, "Assets/State.asset");
	}
#endif
}
