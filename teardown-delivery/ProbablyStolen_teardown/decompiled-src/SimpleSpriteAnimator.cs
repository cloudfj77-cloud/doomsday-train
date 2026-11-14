using System.Collections;
using UnityEngine;

public class SimpleSpriteAnimator : MonoBehaviour
{
	public Sprite[] frames;

	public float frameRate = 0.15f;

	private SpriteRenderer sr;

	private int index;

	private Coroutine animationRoutine;

	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
	}

	private void OnEnable()
	{
		index = 0;
		animationRoutine = StartCoroutine(Animate());
	}

	private void OnDisable()
	{
		if (animationRoutine != null)
		{
			StopCoroutine(animationRoutine);
			animationRoutine = null;
		}
	}

	private IEnumerator Animate()
	{
		while (true)
		{
			sr.sprite = frames[index];
			index = (index + 1) % frames.Length;
			yield return new WaitForSeconds(frameRate);
		}
	}
}
