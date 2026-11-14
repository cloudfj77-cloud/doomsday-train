using System;
using Duckov;
using UnityEngine;

public class Radar : MonoBehaviour
{
	[Min(0.1f)]
	[SerializeField]
	private float nearestDist = 0.5f;

	[Min(1f)]
	[SerializeField]
	private float maxFreq = 10f;

	public Transform overrideTarget;

	private static Transform _sTarget;

	private float buffer;

	private Vector3 CenterPosition => base.transform.position;

	public Vector3 TargetPosition { get; set; }

	public static Transform Target
	{
		get
		{
			return _sTarget;
		}
		set
		{
			_sTarget = value;
			Radar.OnSetTarget?.Invoke(_sTarget);
		}
	}

	public static event Action<Transform> OnSetTarget;

	private Transform GetTarget()
	{
		if (overrideTarget != null)
		{
			return overrideTarget;
		}
		return Target;
	}

	private Vector3 GetDelta()
	{
		return TargetPosition - CenterPosition;
	}

	private float CalculateFrequency()
	{
		float magnitude = GetDelta().magnitude;
		if (Mathf.Abs(magnitude) < nearestDist)
		{
			return maxFreq;
		}
		return maxFreq / (magnitude + 1f - nearestDist);
	}

	private void OnEnable()
	{
		buffer = 1f;
	}

	private void Update()
	{
		Transform target = GetTarget();
		if (target != null)
		{
			TargetPosition = target.position;
		}
		float num = CalculateFrequency();
		buffer += num * Time.deltaTime;
		if (buffer > 1f)
		{
			buffer = 0f;
			Beep();
		}
	}

	private void Beep()
	{
		AudioManager.Post("SFX/Special/Radar/Beep", base.gameObject);
	}
}
