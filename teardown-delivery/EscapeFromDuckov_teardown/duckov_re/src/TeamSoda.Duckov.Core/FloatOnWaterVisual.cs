using System;
using UnityEngine;

public class FloatOnWaterVisual : MonoBehaviour
{
	[Header("Position")]
	[SerializeField]
	private float floatHeight = 0.12f;

	[SerializeField]
	private float floatSpeed = 1f;

	[SerializeField]
	private AnimationCurve floatCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.25f, 1f), new Keyframe(0.5f, 0f), new Keyframe(0.75f, -1f), new Keyframe(1f, 0f));

	[Header("Rotation")]
	[SerializeField]
	private Vector3 rotationAmount = new Vector3(4f, 0f, 4f);

	[SerializeField]
	private Vector3 rotationSpeed = new Vector3(0.8f, 0.35f, 1.1f);

	[SerializeField]
	private Vector3 rotationPhaseOffset = new Vector3(0f, 1.7f, 3.4f);

	[Header("Timing")]
	[SerializeField]
	private float phaseOffset;

	[SerializeField]
	private bool randomizePhaseOnAwake = true;

	[SerializeField]
	private bool useUnscaledTime;

	private Vector3 initialLocalPosition;

	private Quaternion initialLocalRotation;

	private float startTime;

	private float runtimePhaseOffset;

	private bool initialPoseCaptured;

	private float CurrentTime
	{
		get
		{
			if (!useUnscaledTime)
			{
				return Time.time;
			}
			return Time.unscaledTime;
		}
	}

	private void Awake()
	{
		CaptureInitialPose();
		runtimePhaseOffset = phaseOffset;
		if (randomizePhaseOnAwake)
		{
			runtimePhaseOffset += UnityEngine.Random.value;
		}
		startTime = CurrentTime;
	}

	private void OnDisable()
	{
		ResetToInitialPose();
	}

	private void Update()
	{
		float time = CurrentTime - startTime + runtimePhaseOffset;
		ApplyFloat(time);
	}

	public void CaptureInitialPose()
	{
		initialLocalPosition = base.transform.localPosition;
		initialLocalRotation = base.transform.localRotation;
		initialPoseCaptured = true;
	}

	public void ResetToInitialPose()
	{
		if (initialPoseCaptured)
		{
			base.transform.localPosition = initialLocalPosition;
			base.transform.localRotation = initialLocalRotation;
		}
	}

	private void ApplyFloat(float time)
	{
		Vector3 localPosition = initialLocalPosition;
		localPosition.y += EvaluateFloat(time) * floatHeight;
		base.transform.localPosition = localPosition;
		Vector3 euler = new Vector3(Mathf.Sin(time * rotationSpeed.x + rotationPhaseOffset.x) * rotationAmount.x, Mathf.Sin(time * rotationSpeed.y + rotationPhaseOffset.y) * rotationAmount.y, Mathf.Sin(time * rotationSpeed.z + rotationPhaseOffset.z) * rotationAmount.z);
		base.transform.localRotation = initialLocalRotation * Quaternion.Euler(euler);
	}

	private float EvaluateFloat(float time)
	{
		if (floatCurve == null || floatCurve.length == 0)
		{
			return Mathf.Sin(time * floatSpeed * MathF.PI * 2f);
		}
		float time2 = Mathf.Repeat(time * floatSpeed, 1f);
		return floatCurve.Evaluate(time2);
	}

	private void OnValidate()
	{
		floatHeight = Mathf.Max(0f, floatHeight);
		floatSpeed = Mathf.Max(0f, floatSpeed);
		rotationSpeed.x = Mathf.Max(0f, rotationSpeed.x);
		rotationSpeed.y = Mathf.Max(0f, rotationSpeed.y);
		rotationSpeed.z = Mathf.Max(0f, rotationSpeed.z);
	}
}
