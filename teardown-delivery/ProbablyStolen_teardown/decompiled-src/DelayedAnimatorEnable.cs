using System.Collections;
using UnityEngine;

public class DelayedAnimatorEnable : MonoBehaviour
{
	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		if (animator != null)
		{
			animator.enabled = false;
			StartCoroutine(EnableAnimatorWithDelay());
		}
	}

	private IEnumerator EnableAnimatorWithDelay()
	{
		float seconds = Random.Range(0f, 0.5f);
		yield return new WaitForSeconds(seconds);
		animator.enabled = true;
	}
}
