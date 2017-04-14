using UnityEngine;

public static class PhysicsUtil
{
	public static Collider HitTest(Camera detectCamera, Vector2 touchPosition, int ignoreLayer)
	{
		if (!detectCamera.enabled)
			return null;
		Ray ray = detectCamera.ScreenPointToRay(new Vector3(touchPosition.x, touchPosition.y));
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 100f, (detectCamera.cullingMask | ignoreLayer) ^ ignoreLayer))
			return hitInfo.collider;
		return null;
	}

	public static bool HitTest(Collider tester, Vector2 touchPosition, int ignoreLayer)
	{
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(touchPosition.x, touchPosition.y));
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 100f, (Camera.main.cullingMask | ignoreLayer) ^ ignoreLayer))
		{
			Debug.Log(hitInfo.collider.name);
			if (hitInfo.collider == tester)
				return true;
		}
		return false;
	}

	public static bool HitTest_TesterOnly(Collider tester, Camera detectCamera, Vector2 touchPosition)
	{
		if (!detectCamera.enabled)
			return false;
		Ray ray = detectCamera.ScreenPointToRay(new Vector3(touchPosition.x, touchPosition.y));
		RaycastHit hitInfo;
		bool result = tester.Raycast(ray, out hitInfo, 100f);
		return result;
	}
}
