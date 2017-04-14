using System.Collections.Generic;
using System;

namespace Data
{
	public class DataCenter
	{
		public static DataCenter Instance
		{
			get
			{
				if (_instance == null)
					_instance = new DataCenter();
				return _instance;
			}
		}
		private static DataCenter _instance = null;

		private Dictionary<Type, Dictionary<string, DataProvider>> dataProviderTable = null;

		public void RegisterProvider(Type dataType, string alias, DataProvider dataProvider)
		{
			if (dataType != null && dataProvider != null)
			{
				if (dataProviderTable == null)
					dataProviderTable = new Dictionary<Type, Dictionary<string, DataProvider>>();
#if UNITY_EDITOR
				if (CollectionUtil.GetFromTable(dataType, alias, dataProviderTable) != null)
				{
					if (LogUtil.ShowError != null)
					{
						if (string.IsNullOrEmpty(alias))
							LogUtil.ShowError(string.Format("数据中心已经注册有[{0}]类型的成员，请为来自[{1}]的该类型数据添加别名", dataType.Name, dataProvider.GetType().Name));
						else
							LogUtil.ShowError(string.Format("数据中心已经注册有[{0}]类型且别名为[{1}]的成员，来自[{2}]的数据需要修改别名", dataType.Name, alias, dataProvider.GetType().Name));
					}
				}
#endif
				CollectionUtil.AddIntoTable(dataType, alias, dataProvider, dataProviderTable);
#if UNITY_EDITOR
				if (LogUtil.ShowDebug != null)
					LogUtil.ShowDebug(string.Format("[数据中心]1.注册数据：来自[{0}] 数据类型[{1}] 别名[{2}]", dataProvider.GetType().Name, dataType.Name, alias));
#endif
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("注册DataProvider时提供的参数有null值!");
			}
#endif
		}

		public void UnregisterProvider(Type dataType, string alias, DataProvider dataProvider)
		{
			if (dataType != null && dataProvider != null)
			{
				if (dataProviderTable != null)
				{
#if UNITY_EDITOR
					DataProvider oldProvider = CollectionUtil.GetFromTable(dataType, alias, dataProviderTable);
					if (oldProvider != dataProvider && LogUtil.ShowError != null)
						LogUtil.ShowError(string.Format("数据中心里为类型[{0}](别名{1})注册的Provider与当前要注销的Provider不匹配，或者数据中心里没有注册这样的Provider"));
#endif
					CollectionUtil.RemoveFromTable(dataType, alias, dataProviderTable);
#if UNITY_EDITOR
					if (LogUtil.ShowDebug != null)
						LogUtil.ShowDebug(string.Format("[数据中心]2.注销数据：来自[{0}] 数据类型[{1}] 别名[{2}]", dataProvider.GetType().Name, dataType.Name, alias));
#endif
				}
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("注销DataProvider时提供的参数有null值!");
			}
#endif
		}

		public T GetData<T>(string alias = "") where T : DataStructBase
		{
			T res = null;
			Type dataType = typeof(T);
			if (dataType != null)
			{
				if(dataProviderTable != null)
				{
					DataProvider provider = CollectionUtil.GetFromTable(dataType, alias, dataProviderTable);
					if (provider != null)
						res = provider.GetData<T>(alias);
#if UNITY_EDITOR
					else
					{
						if (LogUtil.ShowWarning != null)
							LogUtil.ShowWarning(string.Format("数据中心没有注册类型为[{0}](别名[{1}])的成员!", dataType.Name, alias));
					}
#endif
				}
			}
#if UNITY_EDITOR
			else
			{
				if (LogUtil.ShowError != null)
					LogUtil.ShowError("获取数据时提供的参数有null值!");
			}
#endif
			return res;
		}

		public void ClearAllData()
		{
			foreach(Dictionary<string, DataProvider> tempDict in dataProviderTable.Values)
			{
				foreach(DataProvider provider in tempDict.Values)
				{
					provider.ClearData();
				}
			}
		}
	}
}

