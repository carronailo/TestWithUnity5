using System.Collections.Generic;

public static class CollectionUtil
{
	public static void IncreaseIntoTable<K>(K key, int value, Dictionary<K, int> table)
	{
		int tmp;
		if (table.TryGetValue(key, out tmp))
			table[key] = tmp + value;
		else
			table.Add(key, value);
	}

	public static void DecreaseFromTable<K>(K key, int value, Dictionary<K, int> table)
	{
		int tmp;
		if (table.TryGetValue(key, out tmp))
		{
			if (tmp <= value)
				table.Remove(key);
			else
				table[key] = tmp - value;
		}
	}

	public static void AddIntoTable<K, V>(K key, V value, Dictionary<K, List<V>> table, bool exclusive = false)
	{
		List<V> tmpList;
		if (!table.TryGetValue(key, out tmpList))
		{
			tmpList = new List<V>();
			table.Add(key, tmpList);
		}
		if(!exclusive || !tmpList.Contains(value))
			tmpList.Add(value);
	}

	public static void AddIntoTable<K, V>(K key, V value, Dictionary<K, List<V>> table, int capacity, bool exclusive = false)
	{
		List<V> tmpList;
		if (!table.TryGetValue(key, out tmpList))
		{
			tmpList = new List<V>(capacity);
			table.Add(key, tmpList);
		}
		if (!exclusive || !tmpList.Contains(value))
			tmpList.Add(value);
	}

	public static void AddIntoTable<K, V>(K key, IEnumerable<V> values, Dictionary<K, List<V>> table)
	{
		List<V> tmpList;
		if (!table.TryGetValue(key, out tmpList))
		{
			tmpList = new List<V>();
			table.Add(key, tmpList);
		}
		tmpList.AddRange(values);
	}

	public static void AddIntoTable<K, V>(K key, V value, SortedDictionary<K, List<V>> table)
	{
		List<V> tmpList;
		if (!table.TryGetValue(key, out tmpList))
		{
			tmpList = new List<V>();
			table.Add(key, tmpList);
		}
		tmpList.Add(value);
	}

	public static void AddIntoTable<K, V>(K key, V value, Dictionary<K, HashSet<V>> table)
	{
		HashSet<V> tmpSet;
		if (!table.TryGetValue(key, out tmpSet))
		{
			tmpSet = new HashSet<V>();
			table.Add(key, tmpSet);
		}
		tmpSet.Add(value);
	}

	public static void AddIntoTable<K, V, T>(K key, V value, Dictionary<K, Dictionary<V, T>> table)
	{
		Dictionary<V, T> tmpDict;
		if (!table.TryGetValue(key, out tmpDict))
		{
			tmpDict = new Dictionary<V, T>();
			table.Add(key, tmpDict);
		}
		tmpDict[value] = default(T);
	}

	public static void AddIntoTable<K, V, T>(K key, IEnumerable<V> values, Dictionary<K, Dictionary<V, T>> table)
	{
		Dictionary<V, T> tmpDict;
		if (!table.TryGetValue(key, out tmpDict))
		{
			tmpDict = new Dictionary<V, T>();
			table.Add(key, tmpDict);
		}
		foreach (V v in values)
			tmpDict[v] = default(T);
	}

	public static void AddIntoTable<K, V, T>(K key, IEnumerable<KeyValuePair<V, T>> values, Dictionary<K, Dictionary<V, T>> table)
	{
		Dictionary<V, T> tmpDict;
		if (!table.TryGetValue(key, out tmpDict))
		{
			tmpDict = new Dictionary<V, T>();
			table.Add(key, tmpDict);
		}
		foreach (KeyValuePair<V, T> kv in values)
			tmpDict[kv.Key] = kv.Value;
	}

	public static void AddIntoTable<K, V, T>(K keyL1, V keyL2, T value, Dictionary<K, Dictionary<V, T>> table)
	{
		Dictionary<V, T> tmpDict;
		if (!table.TryGetValue(keyL1, out tmpDict))
		{
			tmpDict = new Dictionary<V, T>();
			table.Add(keyL1, tmpDict);
		}
		tmpDict[keyL2] = value;
	}

	//public static void AddIntoTable<K, V, T>(K keyL1, V keyL2, T value, Dictionary<K, Dictionary<V, List<T>>> table)
	//{
	//	Dictionary<V, List<T>> tmpDict;
	//	List<T> tmpList;
	//	if (!table.TryGetValue(keyL1, out tmpDict))
	//	{
	//		tmpList = new List<T>();
	//		tmpDict = new Dictionary<V, List<T>>();
	//		tmpDict.Add(keyL2, tmpList);
	//		table.Add(keyL1, tmpDict);
	//	}
	//	else if (!tmpDict.TryGetValue(keyL2, out tmpList))
	//	{
	//		tmpList = new List<T>();
	//		tmpDict[keyL2] = tmpList;
	//	}
	//	tmpList.Add(value);
	//}

	public static void AddIntoTable<K, V, T>(K keyL1, V keyL2, T value, Dictionary<K, Dictionary<V, List<T>>> table, bool overwriteIfExist = false)
	{
		Dictionary<V, List<T>> tmpDict;
		List<T> tmpList;
		if (!table.TryGetValue(keyL1, out tmpDict))
		{
			tmpList = new List<T>();
			tmpDict = new Dictionary<V, List<T>>();
			tmpDict.Add(keyL2, tmpList);
			table.Add(keyL1, tmpDict);
		}
		else if (!tmpDict.TryGetValue(keyL2, out tmpList))
		{
			tmpList = new List<T>();
			tmpDict[keyL2] = tmpList;
		}
		int existIndex = tmpList.IndexOf(value);
		if (existIndex >= 0 && overwriteIfExist)
			tmpList[existIndex] = value;
		else
			tmpList.Add(value);
	}

	public static void AddIntoTable<K, V, U, T>(K keyL1, V keyL2, U keyL3, T value, Dictionary<K, Dictionary<V, Dictionary<U, T>>> table)
	{
		Dictionary<V, Dictionary<U, T>> tmpDict1;
		Dictionary<U, T> tmpDict2;
		if (!table.TryGetValue(keyL1, out tmpDict1))
		{
			tmpDict2 = new Dictionary<U, T>();
			tmpDict1 = new Dictionary<V, Dictionary<U, T>>();
			tmpDict1.Add(keyL2, tmpDict2);
			table.Add(keyL1, tmpDict1);
		}
		else if (!tmpDict1.TryGetValue(keyL2, out tmpDict2))
		{
			tmpDict2 = new Dictionary<U, T>();
			tmpDict1[keyL2] = tmpDict2;
		}
		tmpDict2[keyL3] = value;
	}

	public static void AddIntoTable<K, V, U, W, T>(K keyL1, V keyL2, U keyL3, W keyL4, T value, Dictionary<K, Dictionary<V, Dictionary<U, Dictionary<W, T>>>> table)
	{
		Dictionary<V, Dictionary<U, Dictionary<W, T>>> tmpDict1;
		Dictionary<U, Dictionary<W, T>> tmpDict2;
		Dictionary<W, T> tmpDict3;
		if (!table.TryGetValue(keyL1, out tmpDict1))
		{
			tmpDict3 = new Dictionary<W, T>();
			tmpDict2 = new Dictionary<U, Dictionary<W, T>>();
			tmpDict1 = new Dictionary<V, Dictionary<U, Dictionary<W, T>>>();
			tmpDict2.Add(keyL3, tmpDict3);
			tmpDict1.Add(keyL2, tmpDict2);
			table.Add(keyL1, tmpDict1);
		}
		else if (!tmpDict1.TryGetValue(keyL2, out tmpDict2))
		{
			tmpDict3 = new Dictionary<W, T>();
			tmpDict2 = new Dictionary<U, Dictionary<W, T>>();
			tmpDict2.Add(keyL3, tmpDict3);
			tmpDict1[keyL2] = tmpDict2;
		}
		else if (!tmpDict2.TryGetValue(keyL3, out tmpDict3))
		{
			tmpDict3 = new Dictionary<W, T>();
			tmpDict2[keyL3] = tmpDict3;
		}
		tmpDict3[keyL4] = value;
	}

	public static void AddIntoTable<K1, K2, V>(K1 keyL1, K2 keyL2, V value, Dictionary<K1, SortedDictionary<K2, List<V>>> table, bool overwriteIfExist = false)
	{
		SortedDictionary<K2, List<V>> tmpDict;
		List<V> tmpList;
		if (!table.TryGetValue(keyL1, out tmpDict))
		{
			tmpList = new List<V>();
			tmpDict = new SortedDictionary<K2, List<V>>();
			tmpDict.Add(keyL2, tmpList);
			table.Add(keyL1, tmpDict);
		}
		else if (!tmpDict.TryGetValue(keyL2, out tmpList))
		{
			tmpList = new List<V>();
			tmpDict[keyL2] = tmpList;
		}
		int existIndex = tmpList.IndexOf(value);
		if (existIndex >= 0 && overwriteIfExist)
			tmpList[existIndex] = value;
		else
			tmpList.Add(value);
	}

	public static T GetFromTable<K, V, T>(K keyL1, V keyL2, Dictionary<K, Dictionary<V, T>> table)
	{
		T res = default(T);
		Dictionary<V, T> tmpDict;
		if (table.TryGetValue(keyL1, out tmpDict))
			tmpDict.TryGetValue(keyL2, out res);
		return res;
	}

	public static List<T> GetFromTable<K, V, T>(K keyL1, V keyL2, Dictionary<K, Dictionary<V, List<T>>> table)
	{
		List<T> res = default(List<T>);
		Dictionary<V, List<T>> tmpDict;
		if (table.TryGetValue(keyL1, out tmpDict))
			tmpDict.TryGetValue(keyL2, out res);
		return res;
	}

	public static T GetFromTable<K, V, U, T>(K keyL1, V keyL2, U keyL3, Dictionary<K, Dictionary<V, Dictionary<U, T>>> table)
	{
		T res = default(T);
		Dictionary<V, Dictionary<U, T>> tmpDict1;
		Dictionary<U, T> tmpDict2;
		if (table.TryGetValue(keyL1, out tmpDict1))
			if (tmpDict1.TryGetValue(keyL2, out tmpDict2))
				tmpDict2.TryGetValue(keyL3, out res);
		return res;
	}

	public static List<T> GetFromTable<K, V, U, T>(K keyL1, U keyL3, Dictionary<K, Dictionary<V, Dictionary<U, T>>> table)
	{
		List<T> res = new List<T>();
		Dictionary<V, Dictionary<U, T>> tmpDict;
		if (table.TryGetValue(keyL1, out tmpDict))
		{
			T tmp;
			foreach (Dictionary<U, T> dict in tmpDict.Values)
			{
				if (dict.TryGetValue(keyL3, out tmp))
					res.Add(tmp);
			}
		}
		return res;
	}

	public static T GetFromTable<K, V, U, W, T>(K keyL1, V keyL2, U keyL3, W keyL4, Dictionary<K, Dictionary<V, Dictionary<U, Dictionary<W, T>>>> table)
	{
		T res = default(T);
		Dictionary<V, Dictionary<U, Dictionary<W, T>>> tmpDict1;
		Dictionary<U, Dictionary<W, T>> tmpDict2;
		Dictionary<W, T> tmpDict3;
		if (table.TryGetValue(keyL1, out tmpDict1))
			if (tmpDict1.TryGetValue(keyL2, out tmpDict2))
				if (tmpDict2.TryGetValue(keyL3, out tmpDict3))
					tmpDict3.TryGetValue(keyL4, out res);
		return res;
	}

	public static List<T> GetFromTable<K, V, T>(K keyL1, V keyL2, Dictionary<K, SortedDictionary<V, List<T>>> table)
	{
		List<T> res = default(List<T>);
		SortedDictionary<V, List<T>> tmpDict;
		if (table.TryGetValue(keyL1, out tmpDict))
			tmpDict.TryGetValue(keyL2, out res);
		return res;
	}

	public static void RemoveFromTable<K, V>(K key, V value, Dictionary<K, List<V>> table)
	{
		List<V> tmpList;
		if (!table.TryGetValue(key, out tmpList))
			return;
		tmpList.Remove(value);
	}

	public static void RemoveFromTable<K, V>(K key, V value, SortedDictionary<K, List<V>> table)
	{
		List<V> tmpList;
		if (!table.TryGetValue(key, out tmpList))
			return;
		tmpList.Remove(value);
	}

	public static void RemoveFromTable<K, V>(K key, V value, Dictionary<K, HashSet<V>> table)
	{
		HashSet<V> tmpSet;
		if (!table.TryGetValue(key, out tmpSet))
			return;
		tmpSet.Remove(value);
	}

	public static void RemoveFromTable<K, V, T>(K keyL1, V keyL2, Dictionary<K, Dictionary<V, T>> table)
	{
		Dictionary<V, T> tmpTable;
		if (!table.TryGetValue(keyL1, out tmpTable))
			return;
		tmpTable.Remove(keyL2);
	}

	public static void RemoveFromTable<K, V, T>(K keyL1, V keyL2, T value, Dictionary<K, Dictionary<V, List<T>>> table)
	{
		List<T> tmpList = default(List<T>);
		Dictionary<V, List<T>> tmpDict;
		if (table.TryGetValue(keyL1, out tmpDict))
			tmpDict.TryGetValue(keyL2, out tmpList);
		tmpList.Remove(value);
	}

	public static void RemoveFromTable<K, V, T>(K keyL1, V keyL2, Dictionary<K, Dictionary<V, List<T>>> table)
	{
		Dictionary<V, List<T>> tmpDict;
		if (table.TryGetValue(keyL1, out tmpDict))
			tmpDict.Remove(keyL2);
	}

	public static void RemoveFromTable<K, V, U, T>(K keyL1, V keyL2, U keyL3, Dictionary<K, Dictionary<V, Dictionary<U, T>>> table)
	{
		Dictionary<V, Dictionary<U, T>> tmpDict1;
		if (table.TryGetValue(keyL1, out tmpDict1))
		{
			Dictionary<U, T> tmpDict2;
			if (tmpDict1.TryGetValue(keyL2, out tmpDict2))
				tmpDict2.Remove(keyL3);
		}
	}

	public static void RemoveFromTable<K, V, U, T>(K keyL1, V keyL2, Dictionary<K, Dictionary<V, Dictionary<U, T>>> table)
	{
		Dictionary<V, Dictionary<U, T>> tmpDict;
		if (table.TryGetValue(keyL1, out tmpDict))
			tmpDict.Remove(keyL2);
	}

	public static void RemoveFromTable<K, V, U, W, T>(K keyL1, V keyL2, U keyL3, W keyL4, Dictionary<K, Dictionary<V, Dictionary<U, Dictionary<W, T>>>> table)
	{
		Dictionary<V, Dictionary<U, Dictionary<W, T>>> tmpDict1;
		if (table.TryGetValue(keyL1, out tmpDict1))
		{
			Dictionary<U, Dictionary<W, T>> tmpDict2;
			if (tmpDict1.TryGetValue(keyL2, out tmpDict2))
			{
				Dictionary<W, T> tmpDict3;
				if (tmpDict2.TryGetValue(keyL3, out tmpDict3))
					tmpDict3.Remove(keyL4);
			}
		}
	}

	public static void RemoveFromTable<K, V, U, W, T>(K keyL1, V keyL2, U keyL3, Dictionary<K, Dictionary<V, Dictionary<U, Dictionary<W, T>>>> table)
	{
		Dictionary<V, Dictionary<U, Dictionary<W, T>>> tmpDict1;
		if (table.TryGetValue(keyL1, out tmpDict1))
		{
			Dictionary<U, Dictionary<W, T>> tmpDict2;
			if (tmpDict1.TryGetValue(keyL2, out tmpDict2))
				tmpDict2.Remove(keyL3);
		}
	}

	public static void RemoveFromTable<K, V, U, W, T>(K keyL1, V keyL2, Dictionary<K, Dictionary<V, Dictionary<U, Dictionary<W, T>>>> table)
	{
		Dictionary<V, Dictionary<U, Dictionary<W, T>>> tmpDict;
		if (table.TryGetValue(keyL1, out tmpDict))
			tmpDict.Remove(keyL2);
	}

	public static void RemoveFromTable<K, V, T>(K keyL1, V keyL2, T value, Dictionary<K, SortedDictionary<V, List<T>>> table)
	{
		List<T> tmpList = default(List<T>);
		SortedDictionary<V, List<T>> tmpDict;
		if (table.TryGetValue(keyL1, out tmpDict))
			tmpDict.TryGetValue(keyL2, out tmpList);
		tmpList.Remove(value);
	}

	public static void RemoveFromTable<K, V, T>(K keyL1, V keyL2, Dictionary<K, SortedDictionary<V, List<T>>> table)
	{
		SortedDictionary<V, List<T>> tmpDict;
		if (table.TryGetValue(keyL1, out tmpDict))
			tmpDict.Remove(keyL2);
	}

	public static bool IsContain<K, V>(K key, V value, Dictionary<K, List<V>> table)
	{
		List<V> tmpList;
		if (!table.TryGetValue(key, out tmpList))
			return false;
		return tmpList.Contains(value);
	}

	public static bool IsContain<K1, K2, V>(K1 keyL1, K2 keyL2, Dictionary<K1, SortedDictionary<K2, List<V>>> table)
	{
		SortedDictionary<K2, List<V>> tmpDict;
		if (!table.TryGetValue(keyL1, out tmpDict))
			return false;
		return tmpDict.ContainsKey(keyL2);
	}

	public static bool IsContain<K1, K2, V>(K1 keyL1, K2 keyL2, V value, Dictionary<K1, SortedDictionary<K2, List<V>>> table)
	{
		SortedDictionary<K2, List<V>> tmpDict;
		List<V> tmpList;
		if (!table.TryGetValue(keyL1, out tmpDict))
			return false;
		if (!tmpDict.TryGetValue(keyL2, out tmpList))
			return false;
		return tmpList.Contains(value);
	}
}
