using System;
using UnityEngine;

public class AutoDestructParticleSystem : MonoBehaviour
{
	public bool useFixedDuration = false;
	public float FixedDuration
	{
		get { return fixedDuration; }
		set
		{
			lifeTime = 0f;
			fixedDuration = value;
			originalFixedDuration = value;
		}
	}


	public bool disableInsteadOfDestroy = false;
	public bool naturallyEnd = false;

	[NonSerialized]
	public Action<GameObject, object> OnEnd = null;
	[NonSerialized]
	public object onEndExtraParam = null;

	[SerializeField]
	private float fixedDuration = 0f;

	private bool originalUseFixedDuration = false;
	private float originalFixedDuration = 0f;

	private float lifeTime = 0f;
	private bool pause = false;
	private ParticleSystem[] psList = null;
	private bool[] psLoop = null;
	private ParticleSystemCleaner psCleaner = null;

	void Awake()
	{
		psList = GetComponentsInChildren<ParticleSystem>();
		psLoop = new bool[psList.Length];
		psCleaner = GetComponent<ParticleSystemCleaner>();
		originalFixedDuration = fixedDuration;
		originalUseFixedDuration = useFixedDuration;
	}

	private void Start()
	{
		for(int i = 0; i < psList.Length; ++i)
		{
			ParticleSystem ps = psList[i];
			if (ps != null)
				psLoop[i] = ps.loop;
		}
	}

	void Update()
	{
		if (!pause)
		{
			lifeTime += Time.deltaTime;
		}
	}

	void LateUpdate()
	{
		if (useFixedDuration)
		{
			if (fixedDuration < lifeTime)
			{
				if (naturallyEnd)
				{
					useFixedDuration = false;
					for(int i = 0; i < psList.Length; ++i)
					{
						ParticleSystem ps = psList[i];
						if(ps != null)
						{
							ps.enableEmission = false;
							ps.loop = false;
						}
					}
				}
				else
					End();
			}
		}
		else
		{
			bool isLive = false;
			for (int i = 0; i < psList.Length; ++i)
			{
				ParticleSystem ps = psList[i];
				if (ps != null && ps.IsAlive(false))
				{
					isLive = true;
					break;
				}
			}
			if (!isLive)
				End();
		}
	}

	void OnDisable()
	{
		lifeTime = 0f;
	}

	public void Pause()
	{
		pause = true;
	}

	public void Resume()
	{
		pause = false;
	}

	public void TuneSpeed(float factor)
	{
		if (factor > 0f)
			fixedDuration = originalFixedDuration / factor;
		else
			fixedDuration = float.MaxValue;
	}

	public void End()
	{
		if (disableInsteadOfDestroy)
		{
			// 如果采用复用的方式使用SFX实例，那么在结束的时候需要做以下几件事情：清理粒子，将自动销毁时长和状态位重置回来，将粒子发射器的发射相关参数重置回来
			if (psCleaner != null)
				psCleaner.Clear();
			for (int i = 0; i < psList.Length; ++i)
			{
				ParticleSystem ps = psList[i];
				if (ps != null)
				{
					ps.enableEmission = true;
					ps.loop = psLoop[i];
				}
			}
			useFixedDuration = originalUseFixedDuration;
			fixedDuration = originalFixedDuration;
			if (OnEnd != null)
				OnEnd(gameObject, onEndExtraParam);
			// 由于Unity内核机制不再允许在OnDisable/OnEnable中改变子物体的层级，所以只能在发生之前先通知
			SendMessage("AboutToDisable", SendMessageOptions.DontRequireReceiver);
			gameObject.SetActive(false);
		}
		else
		{
			// 由于Unity内核机制不再允许在OnDisable/OnEnable中改变子物体的层级，所以只能在发生之前先通知
			SendMessage("AboutToDisable", SendMessageOptions.DontRequireReceiver);
			Destroy(gameObject);
		}
	}
}
