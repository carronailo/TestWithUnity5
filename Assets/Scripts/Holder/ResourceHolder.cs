using System;

public class ResourceHolder
{
	public readonly string resourceName;
	public readonly object resource = null;
	public bool Released { get; private set; }
	protected readonly Func<ResourceHolder, bool> onRelease = null;

	public ResourceHolder(string resourceName, object resource, Func<ResourceHolder, bool> onRelease)
	{
		this.resourceName = resourceName;
		this.resource = resource;
		this.onRelease = onRelease;
	}

	/// <summary>
	/// 需要显示调用Release来释放ResourceHolder占用的资源
	/// </summary>
	public void Release()
	{
		if (onRelease != null && !Released)
			Released = onRelease(this);
	}
}
