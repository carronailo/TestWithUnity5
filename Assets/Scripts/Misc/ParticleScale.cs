using UnityEngine;

public class ParticleScale : MonoBehaviour 
{
	ParticleSystem[] pss = null;
	TunableEffect tune = null;
	Vector3 lastScale = Vector3.one;

	Transform myTransform = null;

	void Awake()
	{
		myTransform = transform;
		lastScale = myTransform.lossyScale;
	}

	// Use this for initialization
	void Start () 
	{
		pss = GetComponentsInChildren<ParticleSystem>(true);
		tune = GetComponent<TunableEffect>();
	}

	// Update is called once per frame
	void Update () 
	{
		if(lastScale != myTransform.lossyScale)
		{
			ScaleParticleSystems();
			ScaleTunableEffect();
			lastScale = myTransform.lossyScale;
		}
	}

	void ScaleParticleSystems()
	{
		if(pss != null)
		{
			float lastFactor = (lastScale.x + lastScale.y + lastScale.z) / 3f;
			if(lastFactor > 0f)
			{
				float newFactor = (myTransform.lossyScale.x + myTransform.lossyScale.y + myTransform.lossyScale.z) / 3f;
				for (int i = 0; i < pss.Length; ++i)
				{
					ParticleSystem ps = pss[i];
					ps.startSize = ps.startSize * newFactor / lastFactor;
				}
			}
		}
	}

	void ScaleTunableEffect()
	{
		if(tune != null)
		{
			float lastFactor = (lastScale.x + lastScale.y + lastScale.z) / 3f;
			if (lastFactor > 0f)
			{
				float newFactor = (myTransform.lossyScale.x + myTransform.lossyScale.y + myTransform.lossyScale.z) / 3f;
				tune.speed = tune.speed * newFactor / lastFactor;
			}
		}
	}
}
