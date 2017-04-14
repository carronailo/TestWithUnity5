//#define USE_METADATATOKEN   // Unity5.3之前的版本IL2CPP转64位以后不支持MetadataToken
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class ReflectionUtil
{
	private static Queue<List<FieldInfo>> tempFieldListPool = new Queue<List<FieldInfo>>();
	private static Type byteType = typeof(byte);
	private static Type boolType = typeof(bool);
	private static Type intType = typeof(int);
	private static Type shortType = typeof(short);
	private static Type longType = typeof(long);
	private static Type floatType = typeof(float);
	private static Type doubleType = typeof(double);
	private static Type stringType = typeof(string);

	public static bool FindAllFields(Type structDefine, ref List<FieldInfo> columnList)
	{
		if (columnList == null || columnList.Count > 0)
		{
			if(LogUtil.ShowError != null)
				LogUtil.ShowError("提供的columnList是空的，或者没有清空");
			return false;
		}
#if USE_METADATATOKEN
		FieldInfo[] infos = structDefine.GetFields(BindingFlags.Instance | BindingFlags.Public);
		// 将所有找到的public的类实例变量按照定义先后顺序排序
		IEnumerable<FieldInfo> sortedInfos = infos.OrderBy(f => f.MetadataToken);
		columnList.AddRange(sortedInfos);
#else
		if (structDefine == typeof(object))
			return true;
		Type baseType = structDefine.BaseType;
		FindAllFields(baseType, ref columnList);
		FieldInfo[] infos = structDefine.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
		columnList.AddRange(infos);
#endif
		return true;
	}

	public static object Deserialize(object obj, BitConverterEx converter, bool useBigEnum)
	{
		List<FieldInfo> fields = null;
		if (tempFieldListPool.Count > 0)
			fields = tempFieldListPool.Dequeue();
		else
			fields = new List<FieldInfo>();
		object ret = obj;
		if (FindAllFields(obj.GetType(), ref fields))
			ret = DeserializeObject(converter, obj, fields, useBigEnum);
		fields.Clear();
		tempFieldListPool.Enqueue(fields);
		return ret;
	}

	private static object DeserializeObject(BitConverterEx converter, object instance, List<FieldInfo> fields, bool useBigEnum)
	{
		try
		{
			foreach (FieldInfo field in fields)
			{
				Type fieldType = field.FieldType;
				if (fieldType == byteType)
					field.SetValue(instance, converter.PopByte());
				else if (fieldType == boolType)
					field.SetValue(instance, converter.PopBoolean());
				else if (fieldType == shortType)
					field.SetValue(instance, converter.PopShort());
				else if (fieldType == intType)
					field.SetValue(instance, converter.PopInteger());
				else if (fieldType == longType)
					field.SetValue(instance, converter.PopLong());
				else if (fieldType == floatType)
					field.SetValue(instance, converter.PopFloat());
				else if (fieldType == doubleType)
					field.SetValue(instance, converter.PopDouble());
				else if (fieldType == stringType)
					field.SetValue(instance, converter.PopString());
				else if (fieldType.IsEnum)
				{
					if(useBigEnum)
						field.SetValue(instance, converter.PopInteger());
					else
						field.SetValue(instance, converter.PopByte());
				}
				else if (fieldType.IsArray)
				{
					Type elemType = fieldType.GetElementType();
					if (elemType == byteType)
						field.SetValue(instance, converter.PopByteArray());
					else if (elemType == boolType)
						field.SetValue(instance, converter.PopBooleanArray());
					else if (elemType == shortType)
						field.SetValue(instance, converter.PopShortArray());
					else if (elemType == intType)
						field.SetValue(instance, converter.PopIntegerArray());
					else if (elemType == longType)
						field.SetValue(instance, converter.PopLongArray());
					else if (elemType == floatType)
						field.SetValue(instance, converter.PopFloatArray());
					else if (elemType == doubleType)
						field.SetValue(instance, converter.PopDoubleArray());
					else if (elemType == stringType)
						field.SetValue(instance, converter.PopStringArray());
					else if (elemType.IsEnum)
					{
						int arraySize = (ushort)converter.PopShort();
						Array array = Array.CreateInstance(elemType, arraySize);
						for (int i = 0; i < arraySize; ++i)
						{
							if (useBigEnum)
								array.SetValue(converter.PopInteger(), i);
							else
								array.SetValue(converter.PopByte(), i);
						}
						field.SetValue(instance, array);
					}
					else if (elemType.IsClass || elemType.IsValueType)
					{
						int arraySize = (ushort)converter.PopShort();
						Array array = Array.CreateInstance(elemType, arraySize);
						List<FieldInfo> subFields = null;
						if (tempFieldListPool.Count > 0)
							subFields = tempFieldListPool.Dequeue();
						else
							subFields = new List<FieldInfo>();
						if (FindAllFields(elemType, ref subFields))
						{
							for (int i = 0; i < arraySize; ++i)
							{
								object o = Activator.CreateInstance(elemType);
								DeserializeObject(converter, o, subFields, useBigEnum);
								array.SetValue(o, i);
							}
						}
						subFields.Clear();
						tempFieldListPool.Enqueue(subFields);
						field.SetValue(instance, array);
					}
				}
				else if (fieldType.IsClass || fieldType.IsValueType)
				{
					object o = Activator.CreateInstance(fieldType);
					List<FieldInfo> subFields = null;
					if (tempFieldListPool.Count > 0)
						subFields = tempFieldListPool.Dequeue();
					else
						subFields = new List<FieldInfo>();
					if (FindAllFields(fieldType, ref subFields))
						DeserializeObject(converter, o, subFields, useBigEnum);
					subFields.Clear();
					tempFieldListPool.Enqueue(subFields);
					field.SetValue(instance, o);
				}
			}
		}
		catch (Exception ex)
		{
			if (LogUtil.ShowError != null)
				LogUtil.ShowError(string.Format("DeserializeObject Exception at {0}\n{1}", instance.GetType().Name, ex.Message));
		}
		return instance;
	}

	public static byte[] Serialize(object obj, bool useBigEndian, bool useBigEnum)
	{
		byte[] bytes = null;
		List<FieldInfo> fields = new List<FieldInfo>();
		if (FindAllFields(obj.GetType(), ref fields))
		{
			List<byte> buffer = new List<byte>();
			SerializeObject(obj, fields, ref buffer, useBigEndian, useBigEnum);
			bytes = buffer.ToArray();
		}
		return bytes;
	}

	private static void SerializeObject(object instance, List<FieldInfo> fields, ref List<byte> buffer, bool useBigEndian, bool useBigEnum)
	{
		foreach (FieldInfo field in fields)
		{
			if (field.FieldType == typeof(byte))
				buffer.Add((byte)field.GetValue(instance));
			else if (field.FieldType == typeof(bool))
				buffer.AddRange(BitConverterEx.GetBytes((bool)field.GetValue(instance)));
			else if (field.FieldType == typeof(short))
				buffer.AddRange(BitConverterEx.GetBytes((short)field.GetValue(instance), useBigEndian));
			else if (field.FieldType == typeof(int))
				buffer.AddRange(BitConverterEx.GetBytes((int)field.GetValue(instance), useBigEndian));
			else if (field.FieldType == typeof(long))
				buffer.AddRange(BitConverterEx.GetBytes((long)field.GetValue(instance), useBigEndian));
			else if (field.FieldType == typeof(float))
				buffer.AddRange(BitConverterEx.GetBytes((float)field.GetValue(instance), useBigEndian));
			else if (field.FieldType == typeof(double))
				buffer.AddRange(BitConverterEx.GetBytes((double)field.GetValue(instance), useBigEndian));
			else if (field.FieldType == typeof(string))
				buffer.AddRange(BitConverterEx.GetBytes((string)field.GetValue(instance), useBigEndian));
			else if (field.FieldType.IsEnum)
			{
				if (useBigEnum) 
					// 枚举用int存储
					buffer.AddRange(BitConverterEx.GetBytes((int)field.GetValue(instance), useBigEndian));
				else
					// 枚举用byte存储，直接enum转byte会有问题，所以用int中转
					buffer.Add((byte)((int)field.GetValue(instance)));
			}
			else if (field.FieldType.IsArray)
			{
				Type elemType = field.FieldType.GetElementType();
				if (elemType == typeof(byte))
				{
					byte[] bytes = (byte[])field.GetValue(instance);
					buffer.AddRange(BitConverterEx.GetBytes((short)bytes.Length, useBigEndian));
					buffer.AddRange(bytes);
				}
				else if (elemType == typeof(bool))
					buffer.AddRange(BitConverterEx.GetBytes((bool[])field.GetValue(instance), useBigEndian));
				else if (elemType == typeof(short))
					buffer.AddRange(BitConverterEx.GetBytes((short[])field.GetValue(instance), useBigEndian));
				else if (elemType == typeof(int))
					buffer.AddRange(BitConverterEx.GetBytes((int[])field.GetValue(instance), useBigEndian));
				else if (elemType == typeof(long))
					buffer.AddRange(BitConverterEx.GetBytes((long[])field.GetValue(instance), useBigEndian));
				else if (elemType == typeof(float))
					buffer.AddRange(BitConverterEx.GetBytes((float[])field.GetValue(instance), useBigEndian));
				else if (elemType == typeof(double))
					buffer.AddRange(BitConverterEx.GetBytes((double[])field.GetValue(instance), useBigEndian));
				else if (elemType == typeof(string))
					buffer.AddRange(BitConverterEx.GetBytes((string[])field.GetValue(instance), useBigEndian));
				else if (elemType.IsEnum)
				{
					object[] arr = field.GetValue(instance) as object[];
					buffer.AddRange(BitConverterEx.GetBytes((short)arr.Length, useBigEndian));
					for (int i = 0; i < arr.Length; ++i)
					{
						if (useBigEnum)
							// 枚举用int存储
							buffer.AddRange(BitConverterEx.GetBytes((int)arr[i], useBigEndian));
						else
							// 枚举用byte存储，直接enum转byte会有问题，所以用int中转
							buffer.Add((byte)((int)arr[i]));
					}
				}
				else if (elemType.IsClass || elemType.IsValueType)
				{
					List<FieldInfo> subFields = new List<FieldInfo>();
					if (FindAllFields(elemType, ref subFields))
					{
						object[] arr = field.GetValue(instance) as object[];
						buffer.AddRange(BitConverterEx.GetBytes((short)arr.Length, useBigEndian));
						for (int i = 0; i < arr.Length; i++)
							SerializeObject(arr[i], subFields, ref buffer, useBigEndian, useBigEnum);
					}
				}
			}
			else if (field.FieldType.IsClass || field.FieldType.IsValueType)
			{
				List<FieldInfo> subFields = new List<FieldInfo>();
				if (FindAllFields(field.FieldType, ref subFields))
				{
					object o = field.GetValue(instance);
					SerializeObject(o, subFields, ref buffer, useBigEndian, useBigEnum);
				}
			}
		}
	}
}
