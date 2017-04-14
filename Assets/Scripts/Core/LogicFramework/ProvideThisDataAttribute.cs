using System;

[AttributeUsage(AttributeTargets.Field)]
public class ProvideThisDataAttribute : Attribute
{
	public string alias = "";
}
