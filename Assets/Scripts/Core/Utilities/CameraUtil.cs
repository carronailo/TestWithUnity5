using UnityEngine;

public static class CameraUtil
{
	private static Camera mainCamera = null;
	public static Camera MainCamera
	{
		get
		{
			if (mainCamera == null)
			{
				GameObject mainCameraGO = GameObject.FindWithTag("MainCamera");
				if (mainCameraGO != null)
					mainCamera = mainCameraGO.GetComponent<Camera>();
			}
			return mainCamera;
		}
	}

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
}
