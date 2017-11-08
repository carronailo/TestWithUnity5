using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class KeyFrameForSize
{
	public float duration;
	public Vector3 StartValue;
	public Vector3 EndValue;
}

public class SizeChangeCtrl : MonoBehaviour
{
	public bool playAtAwake = false;
	public bool loop = false;

	public float circleTime;         //  变化的总时间
	public List<Vector3> keyFrameList = new List<Vector3>();

	private List<KeyFrameForSize> keyFrames = new List<KeyFrameForSize>();

	private float startTime;
	private Transform myTransform;

	void Awake()
	{
		myTransform = transform;
		Init();
	}

	void Start()
	{
		if (playAtAwake)
			Play();
	}

	void Update()
	{
		float passedTime = Time.time - startTime;
		if (passedTime >= circleTime)
		{
			if (loop)
				Play();
		}
		else
			ChangeSize(GetSizeAtTime(passedTime));
	}

	void Init()
	{
		keyFrames.Clear();

		int frameTimeGapCount = keyFrameList.Count - 1;
		if (frameTimeGapCount > 1)
		{
			float gapTime = circleTime / frameTimeGapCount;
			for (int i = 0; i < frameTimeGapCount; ++i)
			{
				KeyFrameForSize keyFrame = new KeyFrameForSize();
				keyFrame.duration = gapTime;
				keyFrame.StartValue = keyFrameList[i];
				keyFrame.EndValue = keyFrameList[i + 1];
				keyFrames.Add(keyFrame);
			}
		}
	}

	void Play()
	{
#if UNITY_EDITOR
		Init();
#endif
		startTime = Time.time;
	}

	Vector3 GetSizeAtTime(float time)
	{
		if (keyFrames.Count == 0)
			return Vector3.zero;

		foreach (KeyFrameForSize keyFrame in keyFrames)
		{
			if (time < keyFrame.duration)
			{
				return Vector3.Lerp(keyFrame.StartValue, keyFrame.EndValue, time / keyFrame.duration);
			}
			time -= keyFrame.duration;
		}

		return keyFrames[keyFrames.Count - 1].EndValue;
	}

	void ChangeSize(Vector3 size)
	{
		myTransform.localScale = size;
	}
}
