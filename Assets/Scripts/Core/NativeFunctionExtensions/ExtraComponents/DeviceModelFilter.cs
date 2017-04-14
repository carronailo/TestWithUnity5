using UnityEngine;

public enum EIPhoneDeviceFilterFactor
{
	None,
	iPhone5cAndAbove,
	iPhone5sAndAbove,
	iPhone6AndAbove,
}

public enum EIPadDeviceFilterFactor
{
	None,
	iPadMini2AndAbove,
	iPadMini4AndAbove,
}

public class DeviceModelFilter : MonoBehaviour
{
	public EIPhoneDeviceFilterFactor iPhoneFilterFactor;
	public EIPadDeviceFilterFactor iPadFilterFactor;

	public GameObject dialog;

	void Start()
	{
		// 检查机型是否符合机型过滤条件
		if (!CheckDevice())
		{
			dialog.SetActive(true);
			// 阻止游戏模块加载
			ThisIsPersistantObject systemObj = FindObjectOfType<ThisIsPersistantObject>();
			if (systemObj != null)
				Destroy(systemObj.gameObject);
			//Destroy(ThisIsUniqueObject.Instance.gameObject);
		}
	}

	bool CheckDevice()
	{
		bool result = false;
		string deviceModel = SystemInfo.deviceModel;
		//string graphicDeviceName = SystemInfo.graphicsDeviceName;
		bool isIPhone = deviceModel.Contains("iPhone");
		bool isIPad = deviceModel.Contains("iPad");
		if (isIPhone)
		{
			EIPhoneDeviceFilterFactor currentDeviceFactor = EIPhoneDeviceFilterFactor.None;
			if (deviceModel.Contains("iPhone5"))    // 5c
				currentDeviceFactor = EIPhoneDeviceFilterFactor.iPhone5cAndAbove;
			else if (deviceModel.Contains("iPhone6"))   // 5s
				currentDeviceFactor = EIPhoneDeviceFilterFactor.iPhone5sAndAbove;
			else if (deviceModel.Contains("iPhone7"))   // 6 and 6plus
				currentDeviceFactor = EIPhoneDeviceFilterFactor.iPhone6AndAbove;
			else if (deviceModel.Contains("iPhone8"))
				currentDeviceFactor = EIPhoneDeviceFilterFactor.iPhone6AndAbove;
			else if (deviceModel.Contains("iPhone9"))
				currentDeviceFactor = EIPhoneDeviceFilterFactor.iPhone6AndAbove;
			if (currentDeviceFactor >= iPhoneFilterFactor)
				result = true;
		}
		else if (isIPad)
		{
			EIPadDeviceFilterFactor currentDeviceFactor = EIPadDeviceFilterFactor.None;
			if (deviceModel.Contains("iPad4"))  // mini2
				currentDeviceFactor = EIPadDeviceFilterFactor.iPadMini2AndAbove;
			else if (deviceModel.Contains("iPad5")) // mini4
				currentDeviceFactor = EIPadDeviceFilterFactor.iPadMini4AndAbove;
			else if (deviceModel.Contains("iPad6"))
				currentDeviceFactor = EIPadDeviceFilterFactor.iPadMini4AndAbove;
			else if (deviceModel.Contains("iPad7"))
				currentDeviceFactor = EIPadDeviceFilterFactor.iPadMini4AndAbove;
			if (currentDeviceFactor >= iPadFilterFactor)
				result = true;
		}
		else if(Application.platform == RuntimePlatform.Android)
		{
			result = true;
		}
#if UNITY_EDITOR || UNITY_STANDALONE
		result = true;
#endif
		return result;
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
