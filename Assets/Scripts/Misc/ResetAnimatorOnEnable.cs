using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class ResetAnimatorOnEnable : MonoBehaviour
{
	private Animator anim = null;

	void Awake()
	{
		anim = GetComponent<Animator>();
		if(anim != null)
			anim.logWarnings = false;
	}

	void OnEnable()
	{
		if (anim != null)
			anim.SetTrigger("Reset");
    }
}
