using System;

public class AsyncResourceRequester
{
	public float progress = 0f;
	public bool done = false;
	public bool fail = false;
	public bool released = false;
	//public EResourceType resourceType;
	public string resourceName;
	public ResourceHolder resourceHolder = null;
	public Action<AsyncResourceRequester, object> resourceHolderGenerateCallback = null;
	public object callbackExtraParam = null;

	public void Init(string resourceName, ResourceHolder resourceHolder, Action<AsyncResourceRequester, object> resourceHolderGenerateCallback, object callbackExtraParam)
	{
		progress = 0f;
		done = false;
		fail = false;
		released = false;
		this.resourceName = resourceName;
		this.resourceHolder = resourceHolder;
		this.resourceHolderGenerateCallback = resourceHolderGenerateCallback;
		this.callbackExtraParam = callbackExtraParam;
	}
}

