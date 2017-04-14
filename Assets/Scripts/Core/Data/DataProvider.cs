using System;
using System.Collections.Generic;
using System.Reflection;

namespace Data
{
	public abstract class DataProvider
	{
		private Dictionary<Type, Dictionary<string, FieldInfo>> dataTypeAliasTable = null;
		private Dictionary<Type, Dictionary<string, object>> dataCacheTable = null;

		protected DataProvider()
		{
			FieldInfo[] fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			Type dataStructBaseType = typeof(DataStructBase);
			foreach (FieldInfo field in fields)
			{
				Type dataFieldType = field.FieldType;
#if UNITY_EDITOR
				if (dataFieldType.IsPrimitive)
				{
					if (LogUtil.ShowError != null)
						LogUtil.ShowError(string.Format("数据集合[{0:s}]中的成员[{1}]是基础类型，不允许直接对外提供基础数据类型的成员！", GetType().Name, field.Name));
					continue;
				}
				else if (!dataFieldType.IsSubclassOf(dataStructBaseType))
				{
					if (LogUtil.ShowError != null)
						LogUtil.ShowError(string.Format("数据集合[{0:s}]中的成员[{1}]没有从DataStructBase派生，对外提供的数据成员必须从DataStructBase派生！", GetType().Name, field.Name));
					continue;
				}
#endif
				object[] attributesOnField = field.GetCustomAttributes(typeof(ProvideThisDataAttribute), false);
				if (attributesOnField == null || attributesOnField.Length <= 0)
					continue;       // 这个域不提供外部查询，跳过
#if UNITY_EDITOR
				else if (attributesOnField.Length > 1)
					if (LogUtil.ShowWarning != null)
						LogUtil.ShowWarning(string.Format("数据集合[{0:s}]中的成员[{1}]定义了多个ProvideThisData属性，只有第一条会生效", GetType().Name, field.Name));
#endif
				string alias = (attributesOnField[0] as ProvideThisDataAttribute).alias;

				if (dataTypeAliasTable == null)
					dataTypeAliasTable = new Dictionary<Type, Dictionary<string, FieldInfo>>();
#if UNITY_EDITOR
				if (CollectionUtil.GetFromTable(field.FieldType, alias, dataTypeAliasTable) != null)
				{
					if (LogUtil.ShowError != null)
					{
						if (string.IsNullOrEmpty(alias))
							LogUtil.ShowError(string.Format("数据集合[{0:s}]中已经注册有[{1}]类型的成员，请添加别名", GetType().Name, dataFieldType.Name));
						else
							LogUtil.ShowError(string.Format("数据集合[{0:s}]中的成员[{1}]定义的别名[{2}]已经被使用，请确保别名的唯一性", GetType().Name, field.Name, alias));
					}
				}
#endif
				CollectionUtil.AddIntoTable(dataFieldType, alias, field, dataTypeAliasTable);
				RegisterToDataCenter(dataFieldType, alias);
			}
		}

		/// <summary>
		/// 需要显示调用Release来释放DataProvider占用的资源和在DataCenter中注册的引用
		/// </summary>
		public void Release()
		{
			UnregisterAllFromDataCenter();
			if (dataTypeAliasTable != null)
			{
				dataTypeAliasTable.Clear();
				dataTypeAliasTable = null;
			}
		}

		void RegisterToDataCenter(Type dataType, string alias)
		{
			DataCenter.Instance.RegisterProvider(dataType, alias, this);
		}

		void UnregisterFromDataCenter(Type dataType, string alias)
		{
			DataCenter.Instance.UnregisterProvider(dataType, alias, this);
		}

		void UnregisterAllFromDataCenter()
		{
			if(dataTypeAliasTable != null)
				foreach(KeyValuePair<Type, Dictionary<string, FieldInfo>> kv1 in dataTypeAliasTable)
					foreach(KeyValuePair<string, FieldInfo> kv2 in kv1.Value)
						DataCenter.Instance.UnregisterProvider(kv1.Key, kv2.Key, this);
		}

		public T GetData<T>(string alias = "") where T : DataStructBase
		{
			T res = null;
			Type dataType = typeof(T);
			if (dataType != null)
			{
				object cachedRef = null;
				if(dataCacheTable != null)
					cachedRef = CollectionUtil.GetFromTable(dataType, alias, dataCacheTable);
				if(cachedRef != null)
				{
#if UNITY_EDITOR
					if(LogUtil.ShowDebug != null)
						LogUtil.ShowDebug(string.Format("[数据中心]从缓存中读取数据引用: 数据集合[{0}] 类型[{1}](别名[{2}])", GetType().Name, dataType.Name, alias));
#endif
					res = cachedRef as T;
				}
				else
				{
					FieldInfo field = CollectionUtil.GetFromTable(dataType, alias, dataTypeAliasTable);
					if (field != null)
					{
						res = field.GetValue(this) as T;
						if (dataCacheTable == null)
							dataCacheTable = new Dictionary<Type, Dictionary<string, object>>();
						CollectionUtil.AddIntoTable(dataType, alias, res, dataCacheTable);
					}
#if UNITY_EDITOR
					else
					{
						if (LogUtil.ShowError != null)
							LogUtil.ShowError(string.Format("数据集合[{0}]中没有注册类型为[{1}](别名[{2}])的成员!", GetType().Name, dataType.Name, alias));
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

		public abstract void ClearData();
	}
}

