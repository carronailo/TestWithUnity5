using UnityEngine;

public class AnimationSpriteSheet : MonoBehaviour
{
	public int uvX;
	public int xRepeat = 1;
	public int uvY;
	public int yRepeat = 1;
	public int fps;
	public int circle;
	public float delay;
	public int desireFrame;

	private int spriteCount;
	private int circleCount;
	private float timePast = 0f;

	private bool stop = false;

	private bool hasStarted = false;

	void Start()
	{
		hasStarted = true;
		OnStartAndEnable();
	}

	void OnEnable()
	{
		if (hasStarted)
			OnStartAndEnable();
	}

	void OnStartAndEnable()
	{
		stop = true;
		DelayedFunctionCaller.DelayedCall(delay, StartAnimation);
	}

	void Update()
	{
		if (stop)
			return;
		int index = (int)(timePast * fps);
		//if (index >= spriteCount)
		//	circleCount--;
		if (index >= desireFrame)
			circleCount--;
		if (circleCount <= 0)
		{
			stop = true;
			return;
		}

		index = index % desireFrame;
		Vector2 size = new Vector2(xRepeat * 1f / uvX, yRepeat * 1f / uvY);
		int uIndex = index % uvX;
		int vIndex = index / uvX;
		Vector2 offset = new Vector2(uIndex * size.x, 1.0f - size.y - vIndex * size.y);
		GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
		GetComponent<Renderer>().material.SetTextureScale("_MainTex", size);

		timePast += Time.deltaTime;
	}

	void StartAnimation()
	{
		spriteCount = uvX * uvY;
		desireFrame = desireFrame > 0 ? Mathf.Min(spriteCount, desireFrame) : spriteCount;
		circleCount = circle <= 0 ? int.MaxValue : circle;
		timePast = 0f;
		stop = false;
	}
}
