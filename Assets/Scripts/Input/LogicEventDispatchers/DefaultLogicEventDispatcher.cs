using System.Collections.Generic;

public class DefaultLogicEventDispatcher : ILogicInputEventDispatcher
{
	// 一种类型的逻辑输入事件只能存在一个提供者
	private Dictionary<ELogicInputMetaType, ILogicInputEventProvider> logicEventProviders = new Dictionary<ELogicInputMetaType, ILogicInputEventProvider>();

	public object AquireLogicInputEvent(ELogicInputEventType eventType)
	{
		ELogicInputMetaType metaType = InputUtil.GetMetaTypeOfLogicEvent(eventType);
		ILogicInputEventProvider provider = null;
		if (logicEventProviders.TryGetValue(metaType, out provider))
			return provider.AquireLogicInputEvent(eventType);
		return null;
	}

	public void RegisterLogicInputEventHandler(ILogicInputEventProvider provider)
	{
		ELogicInputMetaType metaType = InputUtil.GetMetaTypeOfLogicEventProvider(provider);
		if (metaType != ELogicInputMetaType.Unknown)
		{
			if ((metaType & ELogicInputMetaType.MainJoytick) != 0)
				logicEventProviders[ELogicInputMetaType.MainJoytick] = provider;
			if ((metaType & ELogicInputMetaType.SecondaryJoystick) != 0)
				logicEventProviders[ELogicInputMetaType.SecondaryJoystick] = provider;
		}
	}

	public void UnregisterLogicInputEventHandler(ILogicInputEventProvider provider)
	{
		ELogicInputMetaType metaType = InputUtil.GetMetaTypeOfLogicEventProvider(provider);
		if (metaType != ELogicInputMetaType.Unknown)
		{
			if ((metaType & ELogicInputMetaType.MainJoytick) != 0)
			{
				ILogicInputEventProvider temp = null;
				logicEventProviders.TryGetValue(ELogicInputMetaType.MainJoytick, out temp);
				if (temp == provider)
					logicEventProviders.Remove(ELogicInputMetaType.MainJoytick);
			}
			if ((metaType & ELogicInputMetaType.SecondaryJoystick) != 0)
			{
				ILogicInputEventProvider temp = null;
				logicEventProviders.TryGetValue(ELogicInputMetaType.SecondaryJoystick, out temp);
				if (temp == provider)
					logicEventProviders.Remove(ELogicInputMetaType.SecondaryJoystick);
			}
		}
	}
}
