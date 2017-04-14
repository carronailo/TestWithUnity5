using System;

namespace LogicFramework
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class ConfigureDataAttribute : Attribute
	{
		public Type define;
	}
}

