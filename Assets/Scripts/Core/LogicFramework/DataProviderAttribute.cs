using System;

namespace LogicFramework
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class DataProviderAttribute : Attribute
	{
		public Type define;
	}
}

