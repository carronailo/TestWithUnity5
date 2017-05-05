using UnityEngine;
using UnityEngine.EventSystems;

public class TestUIInteractive : MonoBehaviour
{

	// Use this for initialization
	//void Start ()
	//{
	//	
	//}

	public void OnPress(BaseEventData data)
	{
		LogConsole.Log("TestUIInteractive", Time.frameCount + "OnPress " + data.GetType(), data.selectedObject);
	}

	public void OnRelease(BaseEventData data)
	{
		LogConsole.Log("TestUIInteractive", Time.frameCount + "OnRelease" + data.GetType(), data.selectedObject);
	}

	public void OnClick(BaseEventData data)
	{
		LogConsole.Log("TestUIInteractive", Time.frameCount + "OnClick" + data.GetType(), data.selectedObject);
	}

	public void OnInitialDrag(BaseEventData data)
	{
		LogConsole.Log("TestUIInteractive", Time.frameCount + "OnInitialDrag " + data.GetType(), data.selectedObject);
	}

	public void OnBeginDrag(BaseEventData data)
	{
		LogConsole.Log("TestUIInteractive", Time.frameCount + "OnBeginDrag " + data.GetType(), data.selectedObject);
	}

	public void OnDrag(BaseEventData data)
	{
		LogConsole.Log("TestUIInteractive", Time.frameCount + "OnDrag " + data.GetType(), data.selectedObject);
	}

	public void OnEndDrag(BaseEventData data)
	{
		LogConsole.Log("TestUIInteractive", Time.frameCount + "OnEndDrag " + data.GetType(), data.selectedObject);
	}
}
