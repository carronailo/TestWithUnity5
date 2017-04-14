using Data;
using System;
using System.Reflection;
using UnityEngine;

namespace LogicFramework
{
	public abstract class LogicModule : MonoBehaviour
	{
		private static Type messageProcessUnitType = typeof(LogicMessageProcessUnit);
		private static Type messageProcessUnitAttributeType = typeof(MessageProcessUnitAttribute);
		private static Type networkMessageProcessUnitType = typeof(LogicNetworkMessageProcessUnit);
		private static Type networkMessageProcessUnitAttributeType = typeof(NetworkMessageProcessUnitAttribute);

		//TODO 编辑器脚本让模块可以指定是否要实例化下面的子组件
		protected DataProvider dataProvider = null;
		protected ConfigureData configureData = null;

		// 对外隐藏两个MessageProcessUnit的引用，只做自动资源分配和销毁之用
		private LogicMessageProcessUnit localMessageProcessor = null;
		private LogicNetworkMessageProcessUnit networkMessageProcessor = null;

		[SerializeField]
		[HideInInspector]
		private bool disableMessageProcessor = false;
		[SerializeField]
		[HideInInspector]
		private bool disableNetworkMessageProcessor = false;
		[SerializeField]
		[HideInInspector]
		private bool disableDataProvider = false;
		[SerializeField]
		[HideInInspector]
		private bool disableConfigData = false;

#if UNITY_EDITOR
		public LogicModule()
		{
			_CheckReservedMethodName();
		}

		void _CheckReservedMethodName()
		{
			Type myType = GetType();
			MethodInfo method;
			method = myType.GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy, null, Type.EmptyTypes, null);
			if (method != null)
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("{0} 类继承自 LogicModule 类，不允许定义 Awake() 方法，请用 OnAwaken() 方法代替 Awake() 方法。", method.DeclaringType.ToString()));
			method = myType.GetMethod("OnDestroy", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy, null, Type.EmptyTypes, null);
			if (method != null)
				if (LogUtil.ShowError != null)
					LogUtil.ShowError(string.Format("{0} 类继承自 LogicModule 类，不允许定义 OnDestroy() 方法，请用 OnDestroyed() 方法代替 OnDestroy() 方法。", method.DeclaringType.ToString()));
			//method = myType.GetMethod("OnEnable", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy, null, Type.EmptyTypes, null);
			//if (method != null)
			//	if (LogUtil.ShowError != null)
			//		LogUtil.ShowError(string.Format("{0} 类继承自 LogicModule 类，不允许定义 OnEnable() 方法，请用 OnEnabled() 方法代替 OnEnable() 方法。", method.DeclaringType.ToString()));
			//method = myType.GetMethod("OnDisable", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy, null, Type.EmptyTypes, null);
			//if (method != null)
			//	if (LogUtil.ShowError != null)
			//		LogUtil.ShowError(string.Format("{0} 类继承自 LogicModule 类，不允许定义 OnDisable() 方法，请用 OnDisabled() 方法代替 OnDisable() 方法。", method.DeclaringType.ToString()));
		}
#endif

		private void Awake()
		{
			if(!disableMessageProcessor)
				GenerateMyLocalMessageHandler();
			if (!disableNetworkMessageProcessor)
				GenerateMyNetworkMessageHandler();
			if (!disableDataProvider)
				GenerateMyDataProvider();
			if (!disableConfigData)
				GenerateMyConfigureData();
			// if derive class defined method "OnAwaken", invoke it
			MethodInfo method = GetType().GetMethod("OnAwaken",
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null);
			if (method != null)
			{
				try
				{
					method.Invoke(this, null);
				}
				catch (Exception ex)
				{
					if (LogUtil.ShowException != null)
						LogUtil.ShowException(ex);
				}
			}
		}

		private void OnDestroy()
		{
			// if derive class defined method "OnDestroyed", invoke it
			MethodInfo method = GetType().GetMethod("OnDestroyed",
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null);
			if (method != null)
			{
				try
				{
					method.Invoke(this, null);
				}
				catch (Exception ex)
				{
					if (LogUtil.ShowException != null)
						LogUtil.ShowException(ex);
				}
			}
			DisposeMyLocalMessageHandler();
			DisposeMyNetworkMessageHandler();
			DisposeMyDataProvider();
			DisposeMyConfigureData();
		}

		void GenerateMyLocalMessageHandler()
		{
			object[] attributes = GetType().GetCustomAttributes(messageProcessUnitAttributeType, true);
			if (attributes.Length >= 1)
			{
#if UNITY_EDITOR
				if (attributes.Length > 1)
				{
					if (LogUtil.ShowError != null)
						LogUtil.ShowError(string.Format("模块[{0}]注册了多个消息处理器，只有第一个会生效", GetType().Name));
				}
#endif
				MessageProcessUnitAttribute attribute = attributes[0] as MessageProcessUnitAttribute;
				Type handlerType = attribute.define;
				if (handlerType != null && handlerType.IsSubclassOf(messageProcessUnitType))
				{
					localMessageProcessor = (LogicMessageProcessUnit)Activator.CreateInstance(handlerType);
					//localMessageProcessor = handlerType.GetConstructor(Type.EmptyTypes).Invoke(null) as LogicMessageProcessUnit;
					FieldInfo fieldHost = messageProcessUnitType.GetField("host", BindingFlags.NonPublic | BindingFlags.Instance);
					fieldHost.SetValue(localMessageProcessor, this);
					MethodInfo method = messageProcessUnitType.GetMethod("OnMessageProcessUnitInitialized", BindingFlags.NonPublic | BindingFlags.Instance);
					method.Invoke(localMessageProcessor, null);
				}
#if UNITY_EDITOR
				else
				{
					if (LogUtil.ShowError != null)
						LogUtil.ShowError(string.Format("模块[{0}]注册的消息处理器类型定义错误，消息处理器类型需要从MessageProcessUnit派生", GetType().Name));
				}
#endif
			}
		}

		void GenerateMyNetworkMessageHandler()
		{
			object[] attributes = GetType().GetCustomAttributes(networkMessageProcessUnitAttributeType, true);
			if (attributes.Length >= 1)
			{
#if UNITY_EDITOR
				if (attributes.Length > 1)
				{
					if (LogUtil.ShowError != null)
						LogUtil.ShowError(string.Format("模块[{0}]注册了多个远程消息处理器，只有第一个会生效", GetType().Name));
				}
#endif
				NetworkMessageProcessUnitAttribute attribute = attributes[0] as NetworkMessageProcessUnitAttribute;
				Type handlerType = attribute.define;
				if (handlerType != null && handlerType.IsSubclassOf(networkMessageProcessUnitType))
				{
					networkMessageProcessor = (LogicNetworkMessageProcessUnit)Activator.CreateInstance(handlerType);
					//networkMessageProcessor = handlerType.GetConstructor(Type.EmptyTypes).Invoke(null) as LogicNetworkMessageProcessUnit;
					FieldInfo fieldHost = networkMessageProcessUnitType.GetField("host", BindingFlags.NonPublic | BindingFlags.Instance);
					fieldHost.SetValue(networkMessageProcessor, this);
					MethodInfo method = networkMessageProcessUnitType.GetMethod("OnNetworkMessageProcessUnitInitialized", BindingFlags.NonPublic | BindingFlags.Instance);
					method.Invoke(networkMessageProcessor, null);
				}
#if UNITY_EDITOR
				else
				{
					if (LogUtil.ShowError != null)
						LogUtil.ShowError(string.Format("模块[{0}]注册的远程消息处理器类型定义错误，远程消息处理器类型需要从NetworkMessageProcessUnit派生", GetType().Name));
				}
#endif
			}
		}

		void GenerateMyDataProvider()
		{
			object[] attributes = GetType().GetCustomAttributes(typeof(DataProviderAttribute), true);
			if (attributes.Length >= 1)
			{
#if UNITY_EDITOR
				if (attributes.Length > 1)
				{
					if (LogUtil.ShowError != null)
						LogUtil.ShowError(string.Format("模块[{0}]注册了多个数据源，只有第一个会生效", GetType().Name));
				}
#endif
				DataProviderAttribute attribute = attributes[0] as DataProviderAttribute;
				Type dataProviderType = attribute.define;
				if (dataProviderType != null && dataProviderType.IsSubclassOf(typeof(DataProvider)))
				{
					dataProvider = (DataProvider)Activator.CreateInstance(dataProviderType);
					//dataProvider = dataProviderType.GetConstructor(Type.EmptyTypes).Invoke(null) as DataProvider;
				}
#if UNITY_EDITOR
				else
				{
					if (LogUtil.ShowError != null)
						LogUtil.ShowError(string.Format("模块[{0}]注册的数据源类型定义错误，数据源类型需要从DataProvider派生", GetType().Name));
				}
#endif
			}
		}

		void GenerateMyConfigureData()
		{
			object[] attributes = GetType().GetCustomAttributes(typeof(ConfigureDataAttribute), true);
			if (attributes.Length >= 1)
			{
#if UNITY_EDITOR
				if (attributes.Length > 1)
				{
					if (LogUtil.ShowError != null)
						LogUtil.ShowError(string.Format("模块[{0}]注册了多个配置数据模块，只有第一个会生效", GetType().Name));
				}
#endif
				ConfigureDataAttribute attribute = attributes[0] as ConfigureDataAttribute;
				Type configureDataType = attribute.define;
				if (configureDataType != null && configureDataType.IsSubclassOf(typeof(ConfigureData)))
				{
					configureData = (ConfigureData)Activator.CreateInstance(configureDataType);
					//configureData = configureDataType.GetConstructor(Type.EmptyTypes).Invoke(null) as ConfigureData;
				}
#if UNITY_EDITOR
				else
				{
					if (LogUtil.ShowError != null)
						LogUtil.ShowError(string.Format("模块[{0}]注册的配置数据模块类型定义错误，配置数据模块类型需要从ConfigureData派生", GetType().Name));
				}
#endif
			}
		}

		void DisposeMyLocalMessageHandler()
		{
			if (localMessageProcessor != null)
			{
				localMessageProcessor.Release();
				localMessageProcessor = null;
			}
		}

		void DisposeMyNetworkMessageHandler()
		{
			if (networkMessageProcessor != null)
			{
				networkMessageProcessor.Release();
				networkMessageProcessor = null;
			}
		}

		void DisposeMyDataProvider()
		{
			if(dataProvider != null)
			{
				dataProvider.Release();
				dataProvider = null;
			}
		}

		void DisposeMyConfigureData()
		{
			if (configureData != null)
				configureData = null;
		}
	}
}

