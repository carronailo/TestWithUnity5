using UnityEngine;

public static class CameraUtil
{
	// 使用的前提是摄像机的Tag不能被修改
	private static Camera mainCamera = null;
	private static Transform mainCameraTrans = null;
	public static Camera MainCamera
	{
		get
		{
			if (mainCamera == null)
			{
				GameObject mainCameraGO = GameObject.FindWithTag("MainCamera");
				if (mainCameraGO != null)
				{
					mainCameraTrans = mainCameraGO.transform;
					mainCamera = mainCameraGO.GetComponent<Camera>();
				}
			}
			return mainCamera;
		}
	}
	public static Transform MainCameraTransfrom
	{
		get
		{
			if (mainCameraTrans == null && MainCamera != null)
				mainCameraTrans = MainCamera.transform;
			return mainCameraTrans;
		}
	}

	// 使用的前提是摄像机的Tag不能被修改
	private static Camera specialCamera = null;
	public static Camera SpecialCamera
	{
		get
		{
			if(specialCamera == null)
			{
				GameObject specialCameraGO = GameObject.FindWithTag("SpecialCamera");
				if (specialCameraGO != null)
					specialCamera = specialCameraGO.GetComponent<Camera>();
			}
			return specialCamera;
		}
	}

	// 使用的前提是摄像机的Tag不能被修改
	private static Camera uiCamera = null;
	public static Camera UICamera
	{
		get
		{
			if (uiCamera == null)
			{
				GameObject uiCameraGO = GameObject.FindWithTag("UICamera");
				if (uiCameraGO != null)
					uiCamera = uiCameraGO.GetComponent<Camera>();
			}
			return uiCamera;
		}
	}

	// 使用的前提是摄像机的Tag不能被修改
	private static Camera uiCamera3D = null;
	public static Camera UICamera3D
	{
		get
		{
			if (uiCamera3D == null)
			{
				GameObject uiCamera3DGO = GameObject.FindWithTag("3DUICamera");
				if (uiCamera3DGO != null)
					uiCamera3D = uiCamera3DGO.GetComponent<Camera>();
			}
			return uiCamera3D;
		}
	}

	// 使用的前提是摄像机的Tag不能被修改
	private static Camera minimapCamera = null;
	public static Camera MiniMapCamera
	{
		get
		{
			if(minimapCamera == null)
			{
				GameObject minimapCameraGO = GameObject.FindWithTag("MiniMapCamera");
				if (minimapCameraGO != null)
					minimapCamera = minimapCameraGO.GetComponent<Camera>();
			}
			return minimapCamera;
		}
	}

	public static bool IsOffScreen(this Camera cam, Vector3 screenPos)
	{
		return screenPos.x < 0 || screenPos.x > cam.pixelWidth || screenPos.y < 0 || screenPos.y > cam.pixelHeight || screenPos.z < 0f;
	}

	public static Vector3 GetMainCameraForward()
	{
		if(MainCameraTransfrom != null)
		{
			return Vector3.Scale(MainCameraTransfrom.eulerAngles, new Vector3(0f, 1f, 1f));
		}
		return Vector3.forward;
	}
}
