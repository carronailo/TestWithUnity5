using UnityEngine;

public class ThisIsPersistantObject : MonoBehaviour
{
	GameObject myGameObject = null;

	void Awake()
	{
		myGameObject = gameObject;
		DontDestroyOnLoad(myGameObject);
	}
}
