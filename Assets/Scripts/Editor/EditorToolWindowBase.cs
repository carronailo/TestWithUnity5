using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorToolWindowBase : EditorWindow
{
	public class MyCoroutine
	{
		public string name;
		public Stack<IEnumerator> coroutineBodyStack;
		public System.Action<object> updateCallback;
		public System.Action<object> finishCallback;
		public object finishCallbackParam;

		public void SetStartCoroutineBody(IEnumerator coroutineBody)
		{
			if (coroutineBodyStack == null)
				coroutineBodyStack = new Stack<IEnumerator>();
			coroutineBodyStack.Push(coroutineBody);
		}
	}

	protected static Vector2 defaultWindowSize = new Vector2(500f, 500f);

	protected static void InitWindow()
	{
		CreatewScriptObjectAssetWindow myWindow = GetWindow<CreatewScriptObjectAssetWindow>();
		myWindow.minSize = defaultWindowSize;
		myWindow.Init();
	}

	protected Dictionary<string, MyCoroutine> coroutinesTable = new Dictionary<string, MyCoroutine>();

	protected virtual void Init()
	{
		// Do some initialization works
	}

	protected virtual void Update()
	{
		List<string> coroutines = new List<string>(coroutinesTable.Keys);
		foreach (string coroutine in coroutines)
		{
			MyCoroutine myCoroutine = coroutinesTable[coroutine];
			if (myCoroutine == null || myCoroutine.coroutineBodyStack == null || myCoroutine.coroutineBodyStack.Count <= 0)
			{
				coroutinesTable.Remove(coroutine);
				continue;
			}
			IEnumerator it = myCoroutine.coroutineBodyStack.Peek();
			if (it.MoveNext())
			{
				if (it.Current is IEnumerator)
				{
					myCoroutine.coroutineBodyStack.Push(it.Current as IEnumerator);
				}
				else
				{
					if (myCoroutine.updateCallback != null)
						myCoroutine.updateCallback(it.Current);
				}
			}
			else
			{
				myCoroutine.coroutineBodyStack.Pop();
				if (myCoroutine.coroutineBodyStack.Count <= 0)
				{
					coroutinesTable.Remove(coroutine);
					if (myCoroutine.finishCallback != null)
						myCoroutine.finishCallback(myCoroutine.finishCallbackParam);
				}
			}
		}
		Repaint();
	}

	protected void StartCoroutine(string coroutineName, IEnumerator coroutine, System.Action<object> updateCallback, System.Action<object> finishCallback, object finishCallbackParam)
	{
		MyCoroutine myCoroutine = new MyCoroutine { name = coroutineName, updateCallback = updateCallback, finishCallback = finishCallback, finishCallbackParam = finishCallbackParam };
		myCoroutine.SetStartCoroutineBody(coroutine);
		coroutinesTable[coroutineName] = myCoroutine;
	}

}

