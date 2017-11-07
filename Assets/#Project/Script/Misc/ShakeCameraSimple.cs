using UnityEngine;

public class ShakeCameraSimple : MonoBehaviour
{
	public float delay = 0f;
	public float shakeIntensity = 0.3f;
	public float shakeDecay = 0.002f;
	public bool shakePos = true;
	public bool shakeRot = true;

	private Vector3 oriPosition;
	private Quaternion oriRotation;
	private Transform myTransform;

	private bool shake = false;

	void OnEnable()
	{
		myTransform = transform;
		InitializeShake();
	}

	void Start()
	{
		oriPosition = myTransform.localPosition;
		oriRotation = myTransform.localRotation;
	}

	void Update()
	{
		if (shake)
		{
			if (shakeIntensity > 0)
			{
				if (shakePos)
				{
					Vector3 tmp = Random.onUnitSphere * shakeIntensity;
					myTransform.localPosition = oriPosition + tmp;
				}
				if (shakeRot)
				{
					myTransform.localRotation = new Quaternion(
									oriRotation.x + Random.Range(-shakeIntensity, shakeIntensity) * .2f,
									oriRotation.y + Random.Range(-shakeIntensity, shakeIntensity) * .2f,
									oriRotation.z + Random.Range(-shakeIntensity, shakeIntensity) * .2f,
									oriRotation.w + Random.Range(-shakeIntensity, shakeIntensity) * .2f);
				}
				shakeIntensity -= shakeDecay;
			}
			else
			{
				myTransform.localPosition = oriPosition;
				myTransform.localRotation = oriRotation;
				shake = false;
				//Destroy(this);
			}
		}
	}

	[ContextMenu("重置")]
	public void Reset()
	{
		delay = 0f;
		shakeIntensity = 0f;
		shakeDecay = 0f;
		shakePos = true;
		shakeRot = true;
		InitializeShake();
	}

	void InitializeShake()
	{
		// 只有标识了“ShakeAnchor”的gameObject上的ShakeCameraSimple脚本会执行实际的震屏，其他的脚本都只是向这个脚本传递震屏参数
		if (!tag.Equals("ShakeAnchor"))
		{
			GameObject[] shakeAnchorGOs = GameObject.FindGameObjectsWithTag("ShakeAnchor");
			if (shakeAnchorGOs != null)
			{
				for(int i = 0; i < shakeAnchorGOs.Length; ++i)
				{
					ShakeCameraSimple shakeComp = shakeAnchorGOs[i].GetComponent<ShakeCameraSimple>();
					if (shakeComp != null && shakeComp.enabled)
					{
						shakeComp.delay = delay;
						shakeComp.shakeIntensity = shakeIntensity;
						shakeComp.shakeDecay = shakeDecay;
						shakeComp.shakePos = shakePos;
						shakeComp.shakeRot = shakeRot;
						shakeComp.Restart();
					}
				}
			}
			enabled = false;
		}
	}

	void Restart()
	{
		shake = false;
		if (delay > 0f)
			DelayedFunctionCaller.DelayedCall(delay, StartShake);
		else
			StartShake();
	}

	void StartShake()
	{
		shake = true;
	}


}
