using UnityEngine;

public static class GUITexture_Position_Extension
{
	public static void SetPosition(this GUITexture gui, Vector2 newPos)
	{
		gui.SetPosition(newPos.x, newPos.y);
	}

	public static void SetPosition(this GUITexture gui, float x, float y)
	{
		gui.pixelInset = new Rect(x - gui.transform.position.x * Screen.width,
								  y - gui.transform.position.y * Screen.height,
								  gui.pixelInset.width,
								  gui.pixelInset.height);
	}

	public static Vector2 GetBottomLeft(this GUITexture gui)
	{
		return new Vector2(gui.pixelInset.x + gui.transform.position.x * Screen.width,
						   gui.pixelInset.y + gui.transform.position.y * Screen.height);
	}

	public static Vector2 GetCenter(this GUITexture gui)
	{
		return new Vector2(gui.pixelInset.center.x + gui.transform.position.x * Screen.width,
						   gui.pixelInset.center.y + gui.transform.position.y * Screen.height);
	}
}
