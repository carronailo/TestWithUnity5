using System;

public interface IResourceLoader
{
	/// <summary>
	/// 加载资源
	/// </summary>
	/// <param name="resourceName">资源名</param>
	/// <param name="onResourceLoadedCallback">资源加载完成时的回调</param>
	/// <returns>保持资源异步加载进度的请求器实例</returns>
	AsyncResourceRequester LoadResource(string resourceName, Action<AsyncResourceRequester, object> onResourceLoadedCallback, object callbackExtraParam);

	/// <summary>
	/// 卸载资源
	/// </summary>
	/// <param name="requester">请求资源时获得的请求器</param>
	void UnloadResource(AsyncResourceRequester requester);
}
