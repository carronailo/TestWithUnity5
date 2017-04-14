using System.Collections.Generic;
using UnityEngine;

public static class Collider_Belonging_Extension
{
	static Dictionary<Collider, KeyValuePair<Transform, Collider>> colliderBelongingTable = new Dictionary<Collider, KeyValuePair<Transform, Collider>>();

	public static bool RegsiterThisBelonging(this Collider col, Transform supervisor)
	{
		if (supervisor == null)
			return false;
		Collider supervisorCol = supervisor.GetComponent<Collider>();
		if (supervisorCol == null)
			return false;
		colliderBelongingTable[col] = new KeyValuePair<Transform, Collider>(supervisor, supervisorCol);
		return true;
	}

	public static void UnregisterBelonging(this Collider col)
	{
		colliderBelongingTable.Remove(col);
	}

	public static bool BelongsTo(this Collider col, Transform supervisor)
	{
		// two layers top, one for weapon, another for co-collider created by weapon, such as dash collider
		KeyValuePair<Transform, Collider> tmpKV;
		if (colliderBelongingTable.TryGetValue(col, out tmpKV))
		{
			if (tmpKV.Key == supervisor)
				return true;
			else
			{
				KeyValuePair<Transform, Collider> tmpKV2;
				if (colliderBelongingTable.TryGetValue(tmpKV.Value, out tmpKV2) && tmpKV2.Key == supervisor)
					return true;
			}
		}
		return false;
	}

	public static bool BelongsTo(this Collider col, Collider supervisor)
	{
		// two layers top, one for weapon, another for co-collider created by weapon, such as dash collider
		KeyValuePair<Transform, Collider> tmpKV;
		if (colliderBelongingTable.TryGetValue(col, out tmpKV))
		{
			if (tmpKV.Value == supervisor)
				return true;
			else
			{
				KeyValuePair<Transform, Collider> tmpKV2;
				if (colliderBelongingTable.TryGetValue(tmpKV.Value, out tmpKV2) && tmpKV2.Value == supervisor)
					return true;
			}
		}
		return false;
	}

	public static Collider DirectSupervisor(this Collider col)
	{
		KeyValuePair<Transform, Collider> tmpKV;
		if (colliderBelongingTable.TryGetValue(col, out tmpKV))
		{
			return tmpKV.Value;
		}
		return col;
	}

	public static Transform DirectSupervisorTransform(this Collider col)
	{
		KeyValuePair<Transform, Collider> tmpKV;
		if (colliderBelongingTable.TryGetValue(col, out tmpKV))
		{
			return tmpKV.Key;
		}
		return col.transform;
	}

	public static Collider SupermeSupervisor(this Collider col)
	{
		KeyValuePair<Transform, Collider> tmpKV;
		if (colliderBelongingTable.TryGetValue(col, out tmpKV))
		{
			KeyValuePair<Transform, Collider> tmpKV2;
			if (colliderBelongingTable.TryGetValue(tmpKV.Value, out tmpKV2))
				return tmpKV2.Value;
			else
				return tmpKV.Value;
		}
		return col;
	}

	public static Transform SupremeSupervisorTransform(this Collider col)
	{
		KeyValuePair<Transform, Collider> tmpKV;
		if (colliderBelongingTable.TryGetValue(col, out tmpKV))
		{
			KeyValuePair<Transform, Collider> tmpKV2;
			if (colliderBelongingTable.TryGetValue(tmpKV.Value, out tmpKV2))
				return tmpKV2.Key;
			else
				return tmpKV.Key;
		}
		return col.transform;
	}

	public static T FindHost<T>(this Collider col) where T : Component
	{
		return col.GetComponentInParent<T>();
	}
}

public static class Collider_Echo_Extension
{
	// TODO abstract an Echo class, each collider can have multiple sensor, but can only have one echo
	static Dictionary<Collider, FeedbackReceiverBase> colliderFeedbackReceiverTable = new Dictionary<Collider, FeedbackReceiverBase>();

	public static bool RegisterThisFeedbackReceiver(this Collider col, FeedbackReceiverBase feedbackReceiver)
	{
		if (feedbackReceiver == null)
			return false;
		colliderFeedbackReceiverTable[col] = feedbackReceiver;
		return true;
	}

	public static void UnregisterEcho(this Collider col)
	{
		colliderFeedbackReceiverTable.Remove(col);
	}

	public static FeedbackReceiverBase FeedbackReceiver(this Collider col)
	{
		FeedbackReceiverBase feedbackReceiver;
		if (colliderFeedbackReceiverTable.TryGetValue(col, out feedbackReceiver))
		{
			return feedbackReceiver;
		}
		return null;
	}
}

public static class Collider_Is_Extension
{
	private const string terrainTag = "Terrain";
	private const string weaponTag = "Weapon";

	public static bool IsTerrainCollider(this Collider col)
	{
		return col.CompareTag(terrainTag);
	}

	public static bool IsWeaponCollider(this Collider col)
	{
		return col.CompareTag(weaponTag);
	}
}
