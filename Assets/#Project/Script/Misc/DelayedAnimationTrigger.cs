using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class DelayedAnimationTrigger : MonoBehaviour
{
	public string triggerName = "";
	public float delay;

	private Animator anim = null;
	private float life = 0f;
	private bool end = false;

	void OnEnable()
	{
		if (anim == null)
			anim = GetComponent<Animator>();
		life = 0f;
		end = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (life >= delay && !end)
		{
			anim.SetTrigger(triggerName);
			end = true;
		}
		life += Time.deltaTime;
	}
}
