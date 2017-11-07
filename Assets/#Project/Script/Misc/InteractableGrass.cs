using UnityEngine;

public class InteractableGrass : MonoBehaviour
{
	public Vector4 desiredWind;
	//public float desiredFactor;
	//public float desiredFreq;
	public float duration;
	public float attenTime;
	Vector4 oriWind;
	//float oriFactor;
	//float oriFreq;

	void Awake()
	{
		oriWind = GetComponent<Renderer>().material.GetVector("_Wind");
		//oriFactor = renderer.material.GetFloat("_WindEdgeFlutter");
		//oriFreq = renderer.material.GetFloat("_WindEdgeFlutterFreqScale");
	}

	void OnTriggerEnter(Collider col)
	{
		if (!col.CompareTag("Player"))
			return;
		StopAllCoroutines();
		GetComponent<Renderer>().material.SetVector("_Wind", desiredWind);
		//renderer.material.SetFloat("_WindEdgeFlutter", desiredFactor);
		//renderer.material.SetFloat("_WindEdgeFlutterFreqScale", desiredFreq);
		StartCoroutine(Coroutines.ChangeValue(0f, 1f, duration, attenTime, ChangeWind, null));
		//StartCoroutine(Reset(duration, attenTime));
	}

	void OnTriggerExit(Collider col)
	{
		if (!col.CompareTag("Player"))
			return;
		StopAllCoroutines();
		GetComponent<Renderer>().material.SetVector("_Wind", desiredWind);
		//renderer.material.SetFloat("_WindEdgeFlutter", desiredFactor);
		//renderer.material.SetFloat("_WindEdgeFlutterFreqScale", desiredFreq);
		StartCoroutine(Coroutines.ChangeValue(0f, 1f, duration, attenTime, ChangeWind, null));
		//StartCoroutine(Reset(duration, attenTime));
	}

	void ChangeWind(float value)
	{
		GetComponent<Renderer>().material.SetVector("_Wind", Vector4.Lerp(desiredWind, oriWind, value));
		//renderer.material.SetFloat("_WindEdgeFlutter", Mathf.Lerp(desiredFactor, oriFactor, f));
		//renderer.material.SetFloat("_WindEdgeFlutterFreqScale", Mathf.Lerp(desiredFreq, oriFreq, f));
	}
}
