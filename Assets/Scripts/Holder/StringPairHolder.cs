using UnityEngine;

public class StringPairHolder : ScriptableObject
{
	public StringPair[] content;
}

[System.Serializable]
public class StringPair
{
	public string key;
	public string value;
}

