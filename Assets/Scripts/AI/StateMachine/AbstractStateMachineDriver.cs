using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractStateMachineDriver : MonoBehaviour, IStateMachineDriver
{
	protected IState initialState;
	protected List<IState> allStates = new List<IState>();

	public virtual List<IState> GetAllStates()
	{
		return allStates;
	}

	public virtual IState GetInitialState()
	{
		return initialState;
	}

	public virtual void StartMachine()
	{
	}
}
