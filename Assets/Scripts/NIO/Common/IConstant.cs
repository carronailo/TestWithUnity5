
namespace NIO
{
	using System;

	public interface IConstant<T> : IComparable<T> where T : IConstant<T>
	{
		int ID { get; }
		string Name { get; }
	}
}
