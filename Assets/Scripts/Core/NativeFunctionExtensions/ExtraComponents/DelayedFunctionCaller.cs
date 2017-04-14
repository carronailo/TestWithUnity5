using System.Collections;
using UnityEngine;
using System;

public class DelayedFunctionCaller : MonoBehaviour
{
	public delegate void Action<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
	public delegate void Action<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
	public delegate void Action<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

	public static DelayedFunctionCaller Instance { get; private set; }

	public static void DelayedCall(MonoBehaviour caller, float delaySeconds, Action delayedFunction)
	{
		if (Instance == null)
			return;
		Instance._DelayedCall(caller, delaySeconds, delayedFunction);
	}

	public static void DelayedCall(float delaySeconds, Action delayedFunction)
	{
		DelayedCall(Instance, delaySeconds, delayedFunction);
	}

	public static void DelayedCall<T>(MonoBehaviour caller, float delaySeconds, Action<T> delayedFunction, T arg)
	{
		if (Instance == null)
			return;
		Instance._DelayedCall(caller, delaySeconds, delayedFunction, arg);
	}

	public static void DelayedCall<T>(float delaySeconds, Action<T> delayedFunction, T arg)
	{
		DelayedCall(Instance, delaySeconds, delayedFunction, arg);
	}

	public static void DelayedCall<T1, T2>(MonoBehaviour caller, float delaySeconds, Action<T1, T2> delayedFunction, T1 arg1, T2 arg2)
	{
		if (Instance == null)
			return;
		Instance._DelayedCall(caller, delaySeconds, delayedFunction, arg1, arg2);
	}

	public static void DelayedCall<T1, T2>(float delaySeconds, Action<T1, T2> delayedFunction, T1 arg1, T2 arg2)
	{
		DelayedCall(Instance, delaySeconds, delayedFunction, arg1, arg2);
	}

	public static void DelayedCall<T1, T2, T3>(MonoBehaviour caller, float delaySeconds, Action<T1, T2, T3> delayedFunction, T1 arg1, T2 arg2, T3 arg3)
	{
		if (Instance == null)
			return;
		Instance._DelayedCall(caller, delaySeconds, delayedFunction, arg1, arg2, arg3);
	}

	public static void DelayedCall<T1, T2, T3>(float delaySeconds, Action<T1, T2, T3> delayedFunction, T1 arg1, T2 arg2, T3 arg3)
	{
		DelayedCall(Instance, delaySeconds, delayedFunction, arg1, arg2, arg3);
	}

	public static void DelayedCall<T1, T2, T3, T4>(MonoBehaviour caller, float delaySeconds, Action<T1, T2, T3, T4> delayedFunction, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if (Instance == null)
			return;
		Instance._DelayedCall(caller, delaySeconds, delayedFunction, arg1, arg2, arg3, arg4);
	}

	public static void DelayedCall<T1, T2, T3, T4>(float delaySeconds, Action<T1, T2, T3, T4> delayedFunction, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		DelayedCall(Instance, delaySeconds, delayedFunction, arg1, arg2, arg3, arg4);
	}

	public static void DelayedCall<T1, T2, T3, T4, T5>(MonoBehaviour caller, float delaySeconds, Action<T1, T2, T3, T4, T5> delayedFunction, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		if (Instance == null)
			return;
		Instance._DelayedCall(caller, delaySeconds, delayedFunction, arg1, arg2, arg3, arg4, arg5);
	}

	public static void DelayedCall<T1, T2, T3, T4, T5>(float delaySeconds, Action<T1, T2, T3, T4, T5> delayedFunction, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		DelayedCall(Instance, delaySeconds, delayedFunction, arg1, arg2, arg3, arg4, arg5);
	}

	public static void DelayedCall<T1, T2, T3, T4, T5, T6>(MonoBehaviour caller, float delaySeconds, Action<T1, T2, T3, T4, T5, T6> delayedFunction, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
	{
		if (Instance == null)
			return;
		Instance._DelayedCall(caller, delaySeconds, delayedFunction, arg1, arg2, arg3, arg4, arg5, arg6);
	}

	public static void DelayedCall<T1, T2, T3, T4, T5, T6>(float delaySeconds, Action<T1, T2, T3, T4, T5, T6> delayedFunction, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
	{
		DelayedCall(Instance, delaySeconds, delayedFunction, arg1, arg2, arg3, arg4, arg5, arg6);
	}

	public static void DelayedCall<T1, T2, T3, T4, T5, T6, T7>(MonoBehaviour caller, float delaySeconds, Action<T1, T2, T3, T4, T5, T6, T7> delayedFunction, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
	{
		if (Instance == null)
			return;
		Instance._DelayedCall(caller, delaySeconds, delayedFunction, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
	}

	public static void DelayedCall<T1, T2, T3, T4, T5, T6, T7>(float delaySeconds, Action<T1, T2, T3, T4, T5, T6, T7> delayedFunction, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
	{
		DelayedCall(Instance, delaySeconds, delayedFunction, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
	}

	void Awake()
	{
		Instance = this;
	}

	void _DelayedCall(MonoBehaviour caller, float delaySeconds, Action delayedFunction)
	{
		if (caller == null || delayedFunction == null)
			return;
		//if (delaySeconds <= 0f)
		//	delayedFunction();
		//else
		caller.StartCoroutine(DelayedCallCoroutine(delaySeconds, delayedFunction));
		//Coroutines.StartMyCoroutine(string.Format("{0}.{1}", caller.GetInstanceID(), DateTime.Now.Ticks), DelayedCallCoroutine(delaySeconds, delayedFunction));
	}

	void _DelayedCall<T>(MonoBehaviour caller, float delaySeconds, Action<T> delayedFunction, T arg)
	{
		if (caller == null || delayedFunction == null)
			return;
		//if (delaySeconds <= 0f)
		//	delayedFunction(arg);
		//else
		caller.StartCoroutine(DelayedCallCoroutine(delaySeconds, delayedFunction, arg));
		//Coroutines.StartMyCoroutine(string.Format("{0}.{1}", caller.GetInstanceID(), DateTime.Now.Ticks), DelayedCallCoroutine(delaySeconds, delayedFunction, arg));
	}

	void _DelayedCall<T1, T2>(MonoBehaviour caller, float delaySeconds, Action<T1, T2> delayedFunction, T1 arg1, T2 arg2)
	{
		if (caller == null || delayedFunction == null)
			return;
		//if (delaySeconds <= 0f)
		//	delayedFunction(arg1, arg2);
		//else
		caller.StartCoroutine(DelayedCallCoroutine(delaySeconds, delayedFunction, arg1, arg2));
		//Coroutines.StartMyCoroutine(string.Format("{0}.{1}", caller.GetInstanceID(), DateTime.Now.Ticks), DelayedCallCoroutine(delaySeconds, delayedFunction, arg1, arg2));
	}

	void _DelayedCall<T1, T2, T3>(MonoBehaviour caller, float delaySeconds, Action<T1, T2, T3> delayedFunction, T1 arg1, T2 arg2, T3 arg3)
	{
		if (caller == null || delayedFunction == null)
			return;
		//if (delaySeconds <= 0f)
		//	delayedFunction(arg1, arg2, arg3);
		//else
		caller.StartCoroutine(DelayedCallCoroutine(delaySeconds, delayedFunction, arg1, arg2, arg3));
		//Coroutines.StartMyCoroutine(string.Format("{0}.{1}", caller.GetInstanceID(), DateTime.Now.Ticks), DelayedCallCoroutine(delaySeconds, delayedFunction, arg1, arg2, arg3));
	}

	void _DelayedCall<T1, T2, T3, T4>(MonoBehaviour caller, float delaySeconds, Action<T1, T2, T3, T4> delayedFunction, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if (caller == null || delayedFunction == null)
			return;
		//if (delaySeconds <= 0f)
		//	delayedFunction(arg1, arg2, arg3, arg4);
		//else
		caller.StartCoroutine(DelayedCallCoroutine(delaySeconds, delayedFunction, arg1, arg2, arg3, arg4));
		//Coroutines.StartMyCoroutine(string.Format("{0}.{1}", caller.GetInstanceID(), DateTime.Now.Ticks), DelayedCallCoroutine(delaySeconds, delayedFunction, arg1, arg2, arg3, arg4));
	}

	void _DelayedCall<T1, T2, T3, T4, T5>(MonoBehaviour caller, float delaySeconds, Action<T1, T2, T3, T4, T5> delayedFunction, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		if (caller == null || delayedFunction == null)
			return;
		//if (delaySeconds <= 0f)
		//	delayedFunction(arg1, arg2, arg3, arg4);
		//else
		caller.StartCoroutine(DelayedCallCoroutine(delaySeconds, delayedFunction, arg1, arg2, arg3, arg4, arg5));
		//Coroutines.StartMyCoroutine(string.Format("{0}.{1}", caller.GetInstanceID(), DateTime.Now.Ticks), DelayedCallCoroutine(delaySeconds, delayedFunction, arg1, arg2, arg3, arg4, arg5));
	}

	void _DelayedCall<T1, T2, T3, T4, T5, T6>(MonoBehaviour caller, float delaySeconds, Action<T1, T2, T3, T4, T5, T6> delayedFunction, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
	{
		if (caller == null || delayedFunction == null)
			return;
		//if (delaySeconds <= 0f)
		//	delayedFunction(arg1, arg2, arg3, arg4);
		//else
		caller.StartCoroutine(DelayedCallCoroutine(delaySeconds, delayedFunction, arg1, arg2, arg3, arg4, arg5, arg6));
		//Coroutines.StartMyCoroutine(string.Format("{0}.{1}", caller.GetInstanceID(), DateTime.Now.Ticks), DelayedCallCoroutine(delaySeconds, delayedFunction, arg1, arg2, arg3, arg4, arg5, arg6));
	}

	void _DelayedCall<T1, T2, T3, T4, T5, T6, T7>(MonoBehaviour caller, float delaySeconds, Action<T1, T2, T3, T4, T5, T6, T7> delayedFunction, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
	{
		if (caller == null || delayedFunction == null)
			return;
		//if (delaySeconds <= 0f)
		//	delayedFunction(arg1, arg2, arg3, arg4);
		//else
		caller.StartCoroutine(DelayedCallCoroutine(delaySeconds, delayedFunction, arg1, arg2, arg3, arg4, arg5, arg6, arg7));
		//Coroutines.StartMyCoroutine(string.Format("{0}.{1}", caller.GetInstanceID(), DateTime.Now.Ticks), DelayedCallCoroutine(delaySeconds, delayedFunction, arg1, arg2, arg3, arg4, arg5, arg6, arg7));
	}

	IEnumerator DelayedCallCoroutine(float delaySeconds, Action delayedFunction)
	{
		if (delaySeconds <= 0f)
			yield return new WaitForEndOfFrame();
		else
			yield return new WaitForSeconds(delaySeconds);
		delayedFunction();
	}

	IEnumerator DelayedCallCoroutine<T>(float delaySeconds, Action<T> delayedFunction, T arg)
	{
		if (delaySeconds <= 0f)
			yield return new WaitForEndOfFrame();
		else
			yield return new WaitForSeconds(delaySeconds);
		delayedFunction(arg);
	}

	IEnumerator DelayedCallCoroutine<T1, T2>(float delaySeconds, Action<T1, T2> delayedFunction, T1 arg1, T2 arg2)
	{
		if (delaySeconds <= 0f)
			yield return new WaitForEndOfFrame();
		else
			yield return new WaitForSeconds(delaySeconds);
		delayedFunction(arg1, arg2);
	}

	IEnumerator DelayedCallCoroutine<T1, T2, T3>(float delaySeconds, Action<T1, T2, T3> delayedFunction, T1 arg1, T2 arg2, T3 arg3)
	{
		if (delaySeconds <= 0f)
			yield return new WaitForEndOfFrame();
		else
			yield return new WaitForSeconds(delaySeconds);
		delayedFunction(arg1, arg2, arg3);
	}

	IEnumerator DelayedCallCoroutine<T1, T2, T3, T4>(float delaySeconds, Action<T1, T2, T3, T4> delayedFunction, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if (delaySeconds <= 0f)
			yield return new WaitForEndOfFrame();
		else
			yield return new WaitForSeconds(delaySeconds);
		delayedFunction(arg1, arg2, arg3, arg4);
	}

	IEnumerator DelayedCallCoroutine<T1, T2, T3, T4, T5>(float delaySeconds, Action<T1, T2, T3, T4, T5> delayedFunction, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		if (delaySeconds <= 0f)
			yield return new WaitForEndOfFrame();
		else
			yield return new WaitForSeconds(delaySeconds);
		delayedFunction(arg1, arg2, arg3, arg4, arg5);
	}

	IEnumerator DelayedCallCoroutine<T1, T2, T3, T4, T5, T6>(float delaySeconds, Action<T1, T2, T3, T4, T5, T6> delayedFunction, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
	{
		if (delaySeconds <= 0f)
			yield return new WaitForEndOfFrame();
		else
			yield return new WaitForSeconds(delaySeconds);
		delayedFunction(arg1, arg2, arg3, arg4, arg5, arg6);
	}

	IEnumerator DelayedCallCoroutine<T1, T2, T3, T4, T5, T6, T7>(float delaySeconds, Action<T1, T2, T3, T4, T5, T6, T7> delayedFunction, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
	{
		if (delaySeconds <= 0f)
			yield return new WaitForEndOfFrame();
		else
			yield return new WaitForSeconds(delaySeconds);
		delayedFunction(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
	}

}
