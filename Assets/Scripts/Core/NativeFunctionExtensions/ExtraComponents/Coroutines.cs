using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Coroutines : MonoBehaviour
{
	public class MyCoroutine
	{
		public string name;
		public IEnumerator coroutineBody;
		public Action<object> updateCallback;
		public Action<object, object> updateCallbackWithExtraParam;
		public object updateCallbackExtraParam;

		public MyCoroutine(string coroutineName, IEnumerator coroutine, Action<object, object> onUpdate, object onUpdateParam)
		{
			name = coroutineName;
			coroutineBody = coroutine;
			updateCallback = null;
			updateCallbackWithExtraParam = onUpdate;
			updateCallbackExtraParam = onUpdateParam;
		}

		public MyCoroutine(string coroutineName, IEnumerator coroutine, Action<object> onUpdate)
		{
			name = coroutineName;
			coroutineBody = coroutine;
			updateCallback = onUpdate;
			updateCallbackWithExtraParam = null;
			updateCallbackExtraParam = null;
		}
	}

	public static Coroutines Instance { get; private set; }

	private static Queue<MyCoroutine> startQueue = null;
	private static Dictionary<string, MyCoroutine> coroutinesTable = null;

	void Awake()
	{
		Instance = this;
	}

	void Update()
	{
		if (startQueue != null)
		{
			while (startQueue.Count > 0)
			{
				MyCoroutine coroutine = startQueue.Dequeue();
				if (!coroutinesTable.ContainsKey(coroutine.name))
					coroutinesTable[coroutine.name] = coroutine;
			}
		}

		if (coroutinesTable == null || coroutinesTable.Count <= 0)
			return;

		List<string> removeList = null;
		foreach(MyCoroutine coroutine in coroutinesTable.Values)
		{
			IEnumerator it = coroutine.coroutineBody;
			if (it.MoveNext())
			{
				if (coroutine.updateCallback != null)
					coroutine.updateCallback(it.Current);
				if (coroutine.updateCallbackWithExtraParam != null)
					coroutine.updateCallbackWithExtraParam(it.Current, coroutine.updateCallbackExtraParam);
			}
			else
			{
				if (removeList == null)
					removeList = new List<string>();
				removeList.Add(coroutine.name);
			}
		}
		if (removeList != null)
		{
			for(int i = 0; i < removeList.Count; ++i)
				coroutinesTable.Remove(removeList[i]);
		}
	}

	public static void StartMyCoroutine(string coroutineName, IEnumerator coroutine, Action<object, object> onUpdate, object onUpdateParam)
	{
		if (coroutinesTable == null)
			coroutinesTable = new Dictionary<string, MyCoroutine>();
		if (!coroutinesTable.ContainsKey(coroutineName))
		{
#if UNITY_EDITOR
			Debug.logger.Log("创建携程：" + coroutineName);
#endif
			if (startQueue == null)
				startQueue = new Queue<MyCoroutine>();
			startQueue.Enqueue(new MyCoroutine(coroutineName, coroutine, onUpdate, onUpdateParam));
		}
	}

	public static void StartMyCoroutine(string coroutineName, IEnumerator coroutine, Action<object> onUpdate = null)
	{
		if (coroutinesTable == null)
			coroutinesTable = new Dictionary<string, MyCoroutine>();
		if (!coroutinesTable.ContainsKey(coroutineName))
		{
#if UNITY_EDITOR
			Debug.logger.Log("创建携程：" + coroutineName);
#endif
			if (startQueue == null)
				startQueue = new Queue<MyCoroutine>();
			startQueue.Enqueue(new MyCoroutine(coroutineName, coroutine, onUpdate));
		}
	}

	/// <summary>
	/// 通用数值随时间变化的协程体
	/// </summary>
	/// <param name="fromValue">起始值</param>
	/// <param name="toValue">目标值</param>
	/// <param name="delay">延迟x秒后开始变化数值</param>
	/// <param name="time">数值开始变化后，经历x秒变化到目标值</param>
	/// <param name="onValueChanged">每一次数值发生变化时调用的回调</param>
	/// <param name="onFinished">结束后的回调</param>
	/// <returns></returns>
	public static IEnumerator ChangeValue(float fromValue, float toValue, float delay, float time, Action<float> onValueChanged, Action onFinished)
	{
		float timePast = 0f;
		if (delay > 0f)
		{
			while (timePast < delay)
			{
				timePast += Time.deltaTime;
				yield return null;
			}
		}
		float value = fromValue;
		float step = (toValue - fromValue) / time;
		timePast = 0f;
		while (timePast < time)
		{
			if(onValueChanged != null)
				onValueChanged(value);
			value += step * Time.deltaTime;
			timePast += Time.deltaTime;
			yield return null;
		}
		if (onValueChanged != null)
			onValueChanged(toValue);
		if (onFinished != null)
			onFinished();
	}
}
