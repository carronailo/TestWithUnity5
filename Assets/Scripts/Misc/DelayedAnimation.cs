using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class DelayedAnimation : MonoBehaviour
{
	public float delay;
	[Header("随机延迟不为0时，delay取值[lower,higher]")]
	public float randomDelayLower;
	public float randomDelayHigher;

	private Animator anim = null;
	private Vector3 oriLocalPos;
	private float life = 0f;
	private bool end = false;

	void OnEnable()
	{
		if (anim == null)
		{
			anim = GetComponent<Animator>();
			oriLocalPos = transform.localPosition;
		}
		else
			transform.localPosition = oriLocalPos;
		life = 0f;
		end = false;
		anim.enabled = false;
		if (randomDelayHigher > 0f)
			delay = Random.Range(randomDelayLower, randomDelayHigher);
	}

	void Update()
	{
		if (life >= delay && !end)
		{
			anim.enabled = true;
			end = true;
		}
		life += Time.deltaTime;
	}

}
