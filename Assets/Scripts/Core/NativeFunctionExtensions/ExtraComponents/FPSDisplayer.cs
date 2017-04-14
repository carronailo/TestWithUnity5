using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FPSDisplayer : MonoBehaviour
{
	public float updateInterval = 0.5f;
	public int fps;
	public Text textFPS;
	public Text textRES;

	private int frames = 0;
	private float lastInterval = 0f;
	private string resolution;
	private int lastFPS = 0;

	private GUIText guiTextComp = null;

	void Awake()
	{
		guiTextComp = GetComponent<GUIText>();
		SceneManager.sceneLoaded += OnSceneWasLoaded;
	}

	void Start()
	{
		lastInterval = Time.realtimeSinceStartup;
		frames = 0;
		resolution = "(" + Screen.width.ToString() + "X" + Screen.height.ToString() + ") ";
		if (textRES != null)
			textRES.text = resolution;
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneWasLoaded;
	}

	void Update()
	{
		++frames;
		float timeNow = Time.realtimeSinceStartup;
		if (timeNow > lastInterval + updateInterval)
		{
			fps = (int)(frames / (timeNow - lastInterval));
			frames = 0;
			lastInterval = timeNow;
			if(lastFPS != fps)
			{
				if (guiTextComp != null)
					guiTextComp.text = resolution + fps.ToString();
				if (textFPS != null)
					textFPS.text = fps.ToString();
				lastFPS = fps;
			}
		}
	}

	void OnSceneWasLoaded(Scene scene, LoadSceneMode mode)
	{
		resolution = "(" + Screen.width.ToString() + "X" + Screen.height.ToString() + ") ";
		if (textRES != null)
			textRES.text = resolution;
	}

#if UNITY_EDITOR
	//void OnGUI()
	//{
	//	GUI.Box(new Rect(Screen.width - 40, 10, 30, 20), fps.ToString());
	//	GUI.Box(new Rect(Screen.width - 100, 40, 90, 20), Screen.width.ToString() + "X" + Screen.height.ToString());
	//	GUI.Box(new Rect(Screen.width - 100, 70, 90, 20), Time.realtimeSinceStartup + "s");
	//	GUI.Box(new Rect(Screen.width - 100, 100, 90, 20), Time.frameCount + "f");
	//}
#endif
}
