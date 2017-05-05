using System;
using System.Collections.Generic;

public class InputSystem
{
	private static InputSystem Instance
	{
		get
		{
			if (_instance == null)
				_instance = new InputSystem();
			return _instance;
		}
	}
	private static InputSystem _instance = null;

	public static void SetRawInputEventDispatcher(IRawInputEventDispatcher dispatcher)
	{
		Instance._SetRawInputEventDispatcher(dispatcher);
	}

	public static void RegisterRawInputEventHandler(IRawInputEventHandler handler)
	{
		Instance._RegisterRawInputEventHandler(handler);
	}

	public static void UnregisterRawInputEventHandler(IRawInputEventHandler handler)
	{
		Instance._UnregisterRawInputEventHandler(handler);
	}

	public static void RegisterLogicInputEventProvider(ILogicInputEventProvider provider)
	{
		Instance._RegisterLogicInputEventProvider(provider);
	}

	public static void UnregisterLogicInputEventProvider(ILogicInputEventProvider provider)
	{
		Instance._UnregisterLogicInputEventProvider(provider);
	}

	public static void ProcessRawInputEvent(ERawInputEventType eventType, object eventData)
	{
		Instance._ProcessRawInputEvent(eventType, eventData);
	}

	public static object GetLogicInputEvent(ELogicInputEventType eventType)
	{
		return Instance._GetLogicInputEvent(eventType);
	}

	private IRawInputEventDispatcher rawEventDispatcher = null;
	private ILogicInputEventDispatcher logicEventDispatcher = null;

	private List<IRawInputEventHandler> rawEventHandlerCacheList = new List<IRawInputEventHandler>();
	private List<ILogicInputEventProvider> logicEventProviderCacheList = new List<ILogicInputEventProvider>();

	private InputSystem()
	{
		_SetRawInputEventDispatcher(new DefaultRawInputEventDispatcher());
		_SetLogicInputEventDispatcher(new DefaultLogicEventDispatcher());
	}

	private void _SetRawInputEventDispatcher(IRawInputEventDispatcher dispatcher)
	{
		rawEventDispatcher = dispatcher;
		for (int i = 0; i < rawEventHandlerCacheList.Count; ++i)
			dispatcher.RegisterRawInputEventHandler(rawEventHandlerCacheList[i]);
	}

	private void _SetLogicInputEventDispatcher(ILogicInputEventDispatcher dispatcher)
	{
		logicEventDispatcher = dispatcher;
		for (int i = 0; i < logicEventProviderCacheList.Count; ++i)
			dispatcher.RegisterLogicInputEventHandler(logicEventProviderCacheList[i]);
	}

	private void _RegisterRawInputEventHandler(IRawInputEventHandler handler)
	{
		if (!rawEventHandlerCacheList.Contains(handler))
			rawEventHandlerCacheList.Add(handler);
		if(rawEventDispatcher != null)
			rawEventDispatcher.RegisterRawInputEventHandler(handler);
	}

	private void _UnregisterRawInputEventHandler(IRawInputEventHandler handler)
	{
		rawEventHandlerCacheList.Remove(handler);
		if (rawEventDispatcher != null)
			rawEventDispatcher.UnregisterRawInputEventHandler(handler);
	}

	public void _RegisterLogicInputEventProvider(ILogicInputEventProvider provider)
	{
		if (!logicEventProviderCacheList.Contains(provider))
			logicEventProviderCacheList.Add(provider);
		if (logicEventDispatcher != null)
			logicEventDispatcher.RegisterLogicInputEventHandler(provider);
	}

	public void _UnregisterLogicInputEventProvider(ILogicInputEventProvider provider)
	{
		logicEventProviderCacheList.Remove(provider);
		if (logicEventDispatcher != null)
			logicEventDispatcher.UnregisterLogicInputEventHandler(provider);
	}

	private void _ProcessRawInputEvent(ERawInputEventType eventType, object eventData)
	{
		if (rawEventDispatcher != null)
			rawEventDispatcher.DispatchRawInputEvent(eventType, eventData);
	}

	private object _GetLogicInputEvent(ELogicInputEventType eventType)
	{
		if (logicEventDispatcher != null)
			return logicEventDispatcher.AquireLogicInputEvent(eventType);
		return null;
	}

}
