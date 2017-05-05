using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenResolutionLocker : MonoBehaviour
{
	private static float ratioThreshold16by9 = 1.777778f;
	private static float rationThreshold4by3 = 1.333333f;
	private static float rationThreshold3by2 = 1.5f;

	public bool useFrameRateLimit = true;
	public bool useResolutionLimit = true;
	[Header("显示刷新帧率将会强制置为此值，实际刷新帧率将不会高于此值：")]
	public int targetFrameRate = 60;
	[Header("是否强制物理帧率跟显示帧率保持同步：")]
	public bool forceSyncPhysicsFrameRate = false;
	public bool debug = false;

	private void Awake()
	{
		SceneManager.sceneLoaded += OnSceneWasLoaded;
	}

	void OnEnable()
	{
		LimitResolution();
		LimitFrameRate();
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneWasLoaded;
	}

#if UNITY_EDITOR
	void Update()
	{
		if (debug)
		{
			LimitResolution();
			LimitFrameRate();
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
		}
	}
#endif

	void OnSceneWasLoaded(Scene scene, LoadSceneMode mode)
	{
		LimitResolution();
		LimitFrameRate();
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	void LimitFrameRate()
	{
		if (!useFrameRateLimit && Application.targetFrameRate != -1)
		{
			Application.targetFrameRate = -1;
			if (forceSyncPhysicsFrameRate)
				Time.fixedDeltaTime = 1f / 60;
		}
		else if(useFrameRateLimit && Application.targetFrameRate != targetFrameRate)
		{
			Application.targetFrameRate = targetFrameRate;
			if (forceSyncPhysicsFrameRate)
				Time.fixedDeltaTime = 1f / targetFrameRate;
		}
	}

	void LimitResolution()
	{
		if (Application.isEditor)
			return;
		if (!useResolutionLimit)
			return;
		float ration = Screen.width / (float)Screen.height;
		if (Mathf.Abs(ration - rationThreshold3by2) < 0.01f)
			SetResolution(960, 640);
		else if (Mathf.Abs(ration - rationThreshold4by3) < 0.01f)
		{
			if (Screen.height <= 768)
				SetResolution(1024, 768);
			else
				SetResolution(1536, 1152);
		}
		else if (Mathf.Abs(ration - ratioThreshold16by9) < 0.01f)
		{
			//if (Screen.height >= 1080)
			//	SetResolution(1920, 1080);
			//else 
			if (Screen.height >= 720)
				SetResolution(1280, 720);
			else
				SetResolution(Screen.width, Screen.height);
		}
		else
			SetResolution(1280, 720);

		Debug.Log("SetResolution " + transform.name + "->" + transform.root.name);
	}

	void SetResolution(int width, int height)
	{
		if(Screen.width != width || Screen.height != height)
		{
			if (useFrameRateLimit)
			{
				Screen.SetResolution(width, height, Screen.fullScreen, targetFrameRate);
				if(forceSyncPhysicsFrameRate)
					Time.fixedDeltaTime = 1f / targetFrameRate;
			}
			else
			{
				Screen.SetResolution(width, height, Screen.fullScreen);
				if (forceSyncPhysicsFrameRate)
					Time.fixedDeltaTime = 1f / 60;
			}
			//Message.Send(new ScreenResolutionChangedMessage { screenWidth = width, screenHeight = height }).Broadcast();
		}
	}
}
