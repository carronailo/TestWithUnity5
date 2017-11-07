using UnityEngine;
using System.Collections;

namespace ProjectARPG
{
	public class Offset : MonoBehaviour
	{
		public Vector3 offset;
		//public bool resetOnDisable = true;

		public Vector3 oriPos;
		private Transform myTransform;

		void Awake()
		{
			myTransform = transform;
			oriPos = myTransform.localPosition;
		}

		void Update()
		{
			myTransform.localPosition = oriPos;
			myTransform.Translate(offset, Space.Self);
		}
	}
}
