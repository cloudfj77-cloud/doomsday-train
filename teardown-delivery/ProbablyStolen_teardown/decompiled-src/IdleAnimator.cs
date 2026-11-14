using System;
using UnityEngine;

public class IdleAnimator : MonoBehaviour
{
	public static bool GloballyEnabled = true;

	[Tooltip("How far up and down to bob, in local units.")]
	public float amplitude = 0.25f;

	[Tooltip("Bobs per second.")]
	public float frequency = 1f;

	[Tooltip("Direction to bob along, in local space.")]
	public Vector3 axis = Vector3.up;

	[Tooltip("Use unscaled time so it keeps bobbing while the game is paused.")]
	public bool useUnscaledTime;

	[Tooltip("Start bobbing automatically on enable.")]
	public bool playOnEnable = true;

	private Vector3 originalPosition;

	private bool hasOriginal;

	private float timer;

	private bool playing;

	private void OnEnable()
	{
		if (playOnEnable)
		{
			Play();
		}
	}

	private void OnDisable()
	{
		Stop();
	}

	private void Update()
	{
		if (playing)
		{
			timer += (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
			float num = Mathf.Sin(timer * frequency * 2f * MathF.PI) * amplitude;
			base.transform.localPosition = originalPosition + axis.normalized * num;
		}
	}

	public void Play()
	{
		originalPosition = base.transform.localPosition;
		hasOriginal = true;
		timer = 0f;
		playing = GloballyEnabled;
	}

	public static void SetGloballyEnabled(bool enabled)
	{
		GloballyEnabled = enabled;
		IdleAnimator[] array = UnityEngine.Object.FindObjectsOfType<IdleAnimator>();
		foreach (IdleAnimator idleAnimator in array)
		{
			if (enabled)
			{
				idleAnimator.Play();
			}
			else
			{
				idleAnimator.Freeze();
			}
		}
	}

	public void Stop()
	{
		playing = false;
		if (hasOriginal)
		{
			base.transform.localPosition = originalPosition;
		}
	}

	public void Freeze()
	{
		originalPosition = base.transform.localPosition;
		hasOriginal = true;
		playing = false;
	}
}
