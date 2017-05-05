using UnityEngine;

public enum EUpdateType
{
	Update,
	LateUpdate,
	FixedUpdate,
}

[ExecuteInEditMode]
public class AnchorWithTarget : MonoBehaviour
{
	public Transform target;
	public bool SyncPosition;
	public Vector3 offset;
	public Space offsetSpace = Space.World;
	public bool syncRotate;
	public bool syncAxisX;
	public bool syncAxisY;
	public bool syncAxisZ;
	public Vector3 offsetRotation;
	public Space offsetRotationSpace = Space.World;
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

	// Update is called once per frame
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

	[ContextMenu("Anchor With Target")]
	void Apply()
	{
		if (SyncPosition)
		{
			myTransform.position = target.position;
			if (offsetSpace == Space.World)
				myTransform.position += offset;
			else if (offsetSpace == Space.Self)
				myTransform.localPosition += offset;
		}
		if (syncRotate)
		{
			Vector3 myRot = myTransform.eulerAngles;
			Vector3 targetRot = target.eulerAngles;
			if (syncAxisX)
				myRot.x = targetRot.x;
			if (syncAxisY)
				myRot.y = targetRot.y;
			if (syncAxisZ)
				myRot.z = targetRot.z;
			myTransform.eulerAngles = myRot;
			if(offsetRotationSpace == Space.World)
				myTransform.rotation *= Quaternion.Euler(offsetRotation);
			else if(offsetRotationSpace == Space.Self)
				myTransform.localRotation *= Quaternion.Euler(offsetRotation);
		}
	}
}
