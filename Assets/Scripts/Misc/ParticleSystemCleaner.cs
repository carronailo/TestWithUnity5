using UnityEngine;
using System.Collections;

public class ParticleSystemCleaner : MonoBehaviour
{
	private ParticleSystem[] pss = null;

	void Awake()
	{
		pss = GetComponentsInChildren<ParticleSystem>(true);
	}

	public void Clear()
	{
		if (pss != null)
		{
			foreach (ParticleSystem ps in pss)
			{
				ps.Clear();
				ps.Stop();
				//ps.gameObject.SetActive(false);
			}
		}
	}
}
