using UnityEngine;
using System.Collections;

public class TunableEffect : MonoBehaviour
{
	public float speed;

	public bool IsPaused { get; private set; }
	public float CurrentSpeed { get; private set; }

	private float slowFactor = 1f;

	private ParticleSystem[] pss;
	private Animator[] animators;
	private UVAnimation[] uvAnims;
	private AutoDestructParticleSystem[] autoDestroyComps = null;

	void Awake()
	{
		pss = GetComponentsInChildren<ParticleSystem>();
		animators = GetComponentsInChildren<Animator>();
		uvAnims = GetComponentsInChildren<UVAnimation>();
		autoDestroyComps = GetComponentsInChildren<AutoDestructParticleSystem>();
	}

	void Start()
	{
		if (!IsPaused)
			UpdateSpeed(speed * slowFactor);
	}

	void Update()
	{
		if(!IsPaused)
			UpdateSpeed(speed * slowFactor);
	}

	void UpdateSpeed(float newSpeed)
	{
		if(CurrentSpeed != newSpeed)
		{
			CurrentSpeed = newSpeed;
			for(int i = 0; i < pss.Length; ++i)
			{
				pss[i].playbackSpeed = newSpeed;
			}
			for (int i = 0; i < animators.Length; ++i)
			{
				animators[i].speed = newSpeed;
			}
			for (int i = 0; i < uvAnims.Length; ++i)
			{
				uvAnims[i].TuneSpeed(newSpeed);
			}
			for (int i = 0; i < autoDestroyComps.Length; ++i)
			{
				if (autoDestroyComps[i].useFixedDuration)
					autoDestroyComps[i].TuneSpeed(newSpeed);
			}
		}
	}

	public void SetSpeed(float newSpeed)
	{
		speed = newSpeed;
	}

	public void Slow(float factor)
	{
		slowFactor = factor;
	}

	public void RecoverSlow()
	{
		slowFactor = 1f;
	}

	public void Pause()
	{
		for (int i = 0; i < pss.Length; ++i)
		{
			if (!pss[i].isPaused)
				pss[i].Pause(true);
		}
		for (int i = 0; i < animators.Length; ++i)
		{
			if (animators[i].speed > 0f)
				animators[i].speed = 0f;
		}
		for (int i = 0; i < uvAnims.Length; ++i)
		{
			uvAnims[i].Pause();
		}
		for (int i = 0; i < autoDestroyComps.Length; ++i)
		{
			autoDestroyComps[i].Pause();
		}
		IsPaused = true;
	}

	public void Resume()
	{
		for (int i = 0; i < pss.Length; ++i)
		{
			if (pss[i].isPaused)
				pss[i].Play(true);
		}
		for (int i = 0; i < animators.Length; ++i)
		{
			if (animators[i].speed <= 0f)
				animators[i].speed = 1f;
		}
		for (int i = 0; i < uvAnims.Length; ++i)
		{
			uvAnims[i].Resume();
		}
		for (int i = 0; i < autoDestroyComps.Length; ++i)
		{
			autoDestroyComps[i].Resume();
		}
		IsPaused = false;
	}
}
