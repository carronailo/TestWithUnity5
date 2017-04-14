using System;

namespace LogicFramework
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class NetworkMessageProcessUnitAttribute : Attribute
	{
		public Type define;
	}
}
