using System.Collections.Generic;
using UnityEngine;

public delegate bool CheckCondition(Transform trans);

public static class Transform_Utility_Extension
{
	public static void ResetLocalParameters(this Transform trans)
	{
		trans.localPosition = Vector3.zero;
		trans.localRotation = Quaternion.identity;
		trans.localScale = Vector3.one;
	}

	public static Transform[] GetDirectChild(this Transform trans)
	{
		Transform[] res = new Transform[trans.childCount];
		for(int i = 0; i < trans.childCount; ++i)
			res[i] = trans.GetChild(i);
		return res;
	}
}

public static class Transform_Action_Extension
{
	public static Vector2 PickDirectionPureRandom(this Transform trans)
	{
		// take random Vector as controller direction
		return Random.insideUnitCircle;
	}

	public static float PickDirectionAnglePureRandom(this Transform trans)
	{
		return Random.Range(0f, 360f);
	}

	public static Vector2 PickDirectionNotTooFarAwayOriginalPoint(this Transform trans, Vector3 originalPoint, float radius)
	{
		Vector3 delta = originalPoint - trans.position;
		if (delta.magnitude > radius)
		{
			// convert to controller direction
			Quaternion r = Quaternion.Euler(new Vector3(0, -225, 0));
			Vector3 dir = r * delta;
			return new Vector2(dir.x, dir.z).normalized;
		}
		else
			return trans.PickDirectionPureRandom();
	}

	public static float PickDirectionAngleNotTooFarAwayOriginalPoint(this Transform trans, Vector3 originalPoint, float radius)
	{
		Vector3 delta = originalPoint - trans.position;
		if (delta.magnitude > radius)
		{
			float deg = Vector3.Angle(Vector3.forward, delta);
			if (delta.x < 0f)
				deg = -deg;
			return deg;
		}
		else
			return trans.PickDirectionAnglePureRandom();
	}

	public static Vector2 PickDirectionTowardsTarget(this Transform trans, Transform target)
	{
		if (target == null)
			return Vector2.zero;
		// convert to controller direction
		Quaternion r = Quaternion.Euler(new Vector3(0, -225, 0));
		Vector3 dir = r * (target.position - trans.position);
		return new Vector2(dir.x, dir.z).normalized;
	}

	public static float PickDirectionAngleTowardsTarget(this Transform trans, Transform target)
	{
		if (target == null)
			return 0f;
		Vector3 dir = target.position - trans.position;
		float deg = Vector3.Angle(Vector3.forward, dir);
		if (dir.x < 0f)
			deg = -deg;
		return deg;
	}

	public static bool IsTargetInRange(this Transform trans, Transform target, float rangeSquare, bool ignoreY = false)
	{
		if (target == null)
			return false;
		Vector3 targetPos = target.position;
		Vector3 selfPos = trans.position;
		if(ignoreY)
		{
			targetPos.y = 0;
			selfPos.y = 0;
		}
		float distanceSquare = (targetPos - selfPos).sqrMagnitude;
		return distanceSquare < rangeSquare;
	}

	public static Transform TheNearest(this Transform trans, IEnumerable<Transform> targets, float rangeLimitSquare, CheckCondition condition)
	{
		float nearestDistanceSquare = float.MaxValue;
		Transform nearestTarget = null;
		foreach (Transform target in targets)
		{
			if (condition != null && !condition(target))
				continue;
			float distanceSquare = (target.position - trans.position).sqrMagnitude;
			if (distanceSquare >= rangeLimitSquare)
				continue;
			if (distanceSquare < nearestDistanceSquare)
			{
				nearestTarget = target;
				nearestDistanceSquare = distanceSquare;
			}
		}
		return nearestTarget;
	}

	public static Transform TheNearest(this Transform trans, IEnumerable<GameObject> targetGOs, float rangeLimitSquare, CheckCondition condition)
	{
		float nearestDistanceSquare = float.MaxValue;
		Transform nearestTarget = null;
		foreach (GameObject targetGO in targetGOs)
		{
			Transform target = targetGO.transform;
			if (condition != null && !condition(target))
				continue;
			float distanceSquare = (target.position - trans.position).sqrMagnitude;
			if (distanceSquare >= rangeLimitSquare)
				continue;
			if (distanceSquare < nearestDistanceSquare)
			{
				nearestTarget = target;
				nearestDistanceSquare = distanceSquare;
			}
		}
		return nearestTarget;
	}

	public static Transform Faced(this Transform trans, IEnumerable<Transform> targets, float rangeLimitSquare, CheckCondition condition)
	{
		float nearestAngle = float.MaxValue;
		Transform facedTarget = null;
		Vector3 myDir = trans.TransformDirection(Vector3.forward);
		foreach (Transform target in targets)
		{
			if (condition != null && !condition(target))
				continue;
			float distanceSquare = (target.position - trans.position).sqrMagnitude;
			if (distanceSquare >= rangeLimitSquare)
				continue;
			float angle = Vector3.Angle(target.position - trans.position, myDir);
			if (angle < nearestAngle)
			{
				facedTarget = target;
				nearestAngle = angle;
			}
		}
		return facedTarget;
	}

	public static Transform Faced(this Transform trans, IEnumerable<GameObject> targetGOs, float rangeLimitSquare, CheckCondition condition)
	{
		float nearestAngle = float.MaxValue;
		Transform facedTarget = null;
		Vector3 myDir = trans.TransformDirection(Vector3.forward);
		foreach (GameObject targetGO in targetGOs)
		{
			Transform target = targetGO.transform;
			if (condition != null && !condition(target))
				continue;
			float distanceSquare = (target.position - trans.position).sqrMagnitude;
			if (distanceSquare >= rangeLimitSquare)
				continue;
			float angle = Vector3.Angle(target.position - trans.position, myDir);
			if (angle < nearestAngle)
			{
				facedTarget = target;
				nearestAngle = angle;
			}
		}
		return facedTarget;
	}

	public static Transform InSector(this Transform trans, IEnumerable<Transform> targets, float rangeLimitSquare, float sectorPivotAngle, float sectorAngleRange, CheckCondition condition)
	{
		Transform sectorTarget = null;
		Vector3 myDir = Quaternion.Euler(0f, sectorPivotAngle, 0f) * Vector3.forward;
		float nearestAngle = sectorAngleRange * 0.5f;
		foreach (Transform target in targets)
		{
			if (condition != null && !condition(target))
				continue;
			float distanceSquare = (target.position - trans.position).sqrMagnitude;
			if (distanceSquare >= rangeLimitSquare)
				continue;
			float angle = Vector3.Angle(target.position - trans.position, myDir);
			if (angle < nearestAngle)
			{
				sectorTarget = target;
				nearestAngle = angle;
			}
		}
		return sectorTarget;
	}

	public static Transform InSector(this Transform trans, IEnumerable<GameObject> targetGOs, float rangeLimitSquare, float sectorPivotAngle, float sectorAngleRange, CheckCondition condition)
	{
		Transform sectorTarget = null;
		Vector3 myDir = Quaternion.Euler(0f, sectorPivotAngle, 0f) * Vector3.forward;
		float nearestAngle = sectorAngleRange * 0.5f;
		foreach (GameObject targetGO in targetGOs)
		{
			Transform target = targetGO.transform;
			if (condition != null && !condition(target))
				continue;
			float distanceSquare = (target.position - trans.position).sqrMagnitude;
			if (distanceSquare >= rangeLimitSquare)
				continue;
			float angle = Vector3.Angle(target.position - trans.position, myDir);
			if (angle < nearestAngle)
			{
				sectorTarget = target;
				nearestAngle = angle;
			}
		}
		return sectorTarget;
	}

	public static Transform[] AllInRange(this Transform trans, IEnumerable<Transform> targetTranses, float rangeLimitSquare, CheckCondition condition)
	{
		List<Transform> targets = new List<Transform>();
		foreach (Transform targetTrans in targetTranses)
		{
			if (condition != null && !condition(targetTrans))
				continue;
			float distanceSquare = (targetTrans.position - trans.position).sqrMagnitude;
			if (distanceSquare >= rangeLimitSquare)
				continue;
			targets.Add(targetTrans);
		}
		return targets.ToArray();
	}

	public static Transform[] AllInRange(this Transform trans, IEnumerable<GameObject> targetGOs, float rangeLimitSquare, CheckCondition condition)
	{
		List<Transform> targets = new List<Transform>();
		foreach (GameObject targetGO in targetGOs)
		{
			Transform target = targetGO.transform;
			if (condition != null && !condition(target))
				continue;
			float distanceSquare = (target.position - trans.position).sqrMagnitude;
			if (distanceSquare >= rangeLimitSquare)
				continue;
			targets.Add(target);
		}
		return targets.ToArray();
	}

	public static void LookAtTarget(this Transform trans, Transform target, float turnSpeed)
	{
		trans.localRotation = Quaternion.Lerp(trans.rotation, Quaternion.LookRotation(target.position - trans.position), Time.deltaTime * turnSpeed);
	}
}
