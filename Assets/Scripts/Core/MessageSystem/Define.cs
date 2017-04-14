using System;

namespace MessageSystem
{
	public enum 处理方式 : uint
	{
		立即处理 = 0,
		延缓到Update中处理,
		延缓到FixedUpdate中处理,
		延缓到LateUpdate中处理,
	}

	public enum 投递选项 : uint
	{
		指定目标为投递对象 = 0,
		指定目标为排除对象,
	}

	[Serializable]
	public enum 延迟处理消息 : int
	{
		初始化完就开工 = 0,
		等待唤醒 = 1,
		彻底罢工 = 2,
	}
}
