using UnityEngine;
using System.Collections.Generic;

public static class ComparerUtil
{
	public class ObjectComparer : IEqualityComparer<Object>
	{
		public bool Equals(Object x, Object y)
		{
			return x.GetInstanceID() == y.GetInstanceID();
		}

		public int GetHashCode(Object obj)
		{
			return obj.GetInstanceID();
		}
	}

	public class ComponentComparer : IEqualityComparer<Component>
	{
		public bool Equals(Component x, Component y)
		{
			return x.GetInstanceID() == y.GetInstanceID();
		}

		public int GetHashCode(Component obj)
		{
			return obj.GetInstanceID();
		}
	}

	public class MonoBehaviourComparer : IEqualityComparer<MonoBehaviour>
	{
		public bool Equals(MonoBehaviour x, MonoBehaviour y)
		{
			return x.GetInstanceID() == y.GetInstanceID();
		}

		public int GetHashCode(MonoBehaviour obj)
		{
			return obj.GetInstanceID();
		}
	}

	public class ColliderComparer : IEqualityComparer<Collider>
	{
		public bool Equals(Collider x, Collider y)
		{
			return x.GetInstanceID() == y.GetInstanceID();
		}

		public int GetHashCode(Collider obj)
		{
			return obj.GetInstanceID();
		}
	}

	public class GameObjectComparer : IEqualityComparer<GameObject>
	{
		public bool Equals(GameObject x, GameObject y)
		{
			return x.GetInstanceID() == y.GetInstanceID();
		}

		public int GetHashCode(GameObject obj)
		{
			return obj.GetInstanceID();
		}
	}

	public class TransformComparer : IEqualityComparer<Transform>
	{
		public bool Equals(Transform x, Transform y)
		{
			return x.GetInstanceID() == y.GetInstanceID();
		}

		public int GetHashCode(Transform obj)
		{
			return obj.GetInstanceID();
		}
	}

}
