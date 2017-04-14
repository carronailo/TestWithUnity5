using System;

namespace LogicFramework
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class MessageProcessUnitAttribute : Attribute
	{
		public Type define;
	}
}
