using System.Collections;
using UnityEngine;

public abstract class InputModule : MonoBehaviour
{
	public virtual bool Enable
	{
		get
		{
			return isEnable;
		}
		set
		{
			isEnable = value;
		}
	}
	protected bool isEnable = true;

	public virtual bool Visible
	{
		get
		{
			return isVisible;
		}
		set
		{
			isVisible = value;
			isEnable = value;
		}
	}
	protected bool isVisible = true;

	protected virtual void Start()
	{
		StartCoroutine(ResetInputFlagsEveryEngineTick(new WaitForEndOfFrame()));
	}

	protected virtual void OnDestroy()
	{
		StopAllCoroutines();
	}

	protected IEnumerator ResetInputFlagsEveryEngineTick(WaitForEndOfFrame it)
	{
		while (true)
		{
			yield return it;
			ResetAllFlags();
		}
	}

	protected virtual void ResetAllFlags()
	{

	}

}
