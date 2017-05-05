using UnityEngine;

[ExecuteInEditMode]
public class LookAtTarget : MonoBehaviour
{
	public Transform target;
	public Vector3 offset = Vector3.zero;
	public bool lockZ = true;
	public EUpdateType updateType = EUpdateType.Update;

	private Transform myTransform = null;

	void Awake()
	{
		myTransform = transform;
	}

	void Start()
	{
		if (target != null)
			Apply();
	}

	void Update()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
			_Update();
		else
#endif
		if (updateType == EUpdateType.Update)
			_Update();
	}

	void LateUpdate()
	{
		if (updateType == EUpdateType.LateUpdate)
			_Update();
	}

	void FixedUpdate()
	{
		if (updateType == EUpdateType.FixedUpdate)
			_Update();
	}

	void _Update()
	{
		if (target != null)
			Apply();
	}

	[ContextMenu("Look At Target")]
	public void Apply()
	{
		if (target != null)
		{
			if (lockZ)
				myTransform.rotation = Quaternion.LookRotation(target.position + offset - myTransform.position);
			else
				myTransform.rotation = Quaternion.LookRotation(target.position + offset - myTransform.position, myTransform.TransformDirection(Vector3.up));
		}
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		Color gizmosColor = Gizmos.color;
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(myTransform.position, target.position + offset);
		Gizmos.DrawWireSphere(target.position + offset, 0.1f);
		Gizmos.color = gizmosColor;
	}
#endif
}
