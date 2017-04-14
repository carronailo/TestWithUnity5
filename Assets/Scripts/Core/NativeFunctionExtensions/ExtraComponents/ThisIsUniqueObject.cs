using UnityEngine;

public class ThisIsUniqueObject : MonoBehaviour
{
	public static ThisIsUniqueObject Instance
	{
		get { return instance; }
	}
	private static ThisIsUniqueObject instance = null;

	//public float timeStamp = 0L;
	GameObject myGameObject = null;

	void Awake()
	{
		//timeStamp = Time.realtimeSinceStartup;
		//name += timeStamp.ToString();
		myGameObject = gameObject;
		if (CheckUnique())
		{
			foreach (MonoBehaviour comp in GetComponents<MonoBehaviour>())
				comp.enabled = true;
			DontDestroyOnLoad(myGameObject);
			instance = this;
		}
		else
		{
			myGameObject.SetActive(false);
			Destroy(myGameObject);
		}
	}

	bool CheckUnique()
	{
		if (instance == null)
			return true;
		return false;
		//GameObject[] uniqueObjects = GameObject.FindGameObjectsWithTag("UniqueComponent");
		//foreach (GameObject uniqueObject in uniqueObjects)
		//{
		//	ThisIsUniqueObject comp = uniqueObject.GetComponent<ThisIsUniqueObject>();
		//	if (comp.timeStamp < timeStamp)
		//		return false;
		//}
		//return true;
	}

}
