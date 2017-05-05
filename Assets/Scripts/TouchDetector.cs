using UnityEngine;

public class TouchDetector : MonoBehaviour
{
	public InputModule hostModule;

	private int fingerCount;

	// Use this for initialization
	void Start()
	{
		if (hostModule == null)
			hostModule = GetComponentInParent<InputModule>();
	}

	// Update is called once per frame
	void Update()
	{
		fingerCount = 0;
		foreach (Touch touch in Input.touches)
		{
			if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
				fingerCount++;

		}
		if (fingerCount > 0)
			print("User has " + fingerCount + " finger(s) touching the screen");

	}
}
