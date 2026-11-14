using System;
using System.Collections;
using UnityEngine;

public class Arm : MonoBehaviour
{
	[Header("Muzzle Flash")]
	[SerializeField]
	private SpriteRenderer muzzleFlash;

	[SerializeField]
	private float flashDuration = 0.24f;

	[SerializeField]
	private float flashPeakScale = 0.5f;

	[SerializeField]
	private float flashRiseFraction = 0.2f;

	[Header("Breathing")]
	[SerializeField]
	private float breathingAmplitudeX = 0.02f;

	[SerializeField]
	private float breathingAmplitudeY = 0.04f;

	[SerializeField]
	private float breathingFrequency = 0.25f;

	[Header("Heartbeat")]
	[SerializeField]
	private float heartbeatAmplitude = 0.005f;

	[SerializeField]
	private float heartbeatFrequency = 1.1f;

	[SerializeField]
	private float heartbeatDubDelay = 0.15f;

	[SerializeField]
	private float heartbeatPulseWidth = 0.08f;

	[Header("Recoil")]
	[SerializeField]
	private float recoilX = 0.25f;

	[SerializeField]
	private float recoilY = 0.15f;

	[SerializeField]
	private float recoilRiseDuration = 0.05f;

	[SerializeField]
	private float recoilFallDuration = 0.25f;

	private Vector3 _restPosition;

	private Coroutine _flashCoroutine;

	private Coroutine _recoilCoroutine;

	private Vector3 _recoilOffset;

	private void Start()
	{
		_restPosition = base.transform.localPosition;
	}

	private void OnEnable()
	{
		if (muzzleFlash != null)
		{
			muzzleFlash.gameObject.SetActive(value: false);
		}
	}

	public void TriggerRecoil()
	{
		if (_recoilCoroutine != null)
		{
			StopCoroutine(_recoilCoroutine);
		}
		_recoilCoroutine = StartCoroutine(RecoilRoutine());
	}

	private IEnumerator RecoilRoutine()
	{
		Vector3 target = new Vector3(recoilX, recoilY, 0f);
		float elapsed2 = 0f;
		while (elapsed2 < recoilRiseDuration)
		{
			elapsed2 += Time.deltaTime;
			_recoilOffset = Vector3.Lerp(Vector3.zero, target, elapsed2 / recoilRiseDuration);
			yield return null;
		}
		elapsed2 = 0f;
		while (elapsed2 < recoilFallDuration)
		{
			elapsed2 += Time.deltaTime;
			_recoilOffset = Vector3.Lerp(target, Vector3.zero, elapsed2 / recoilFallDuration);
			yield return null;
		}
		_recoilOffset = Vector3.zero;
		_recoilCoroutine = null;
	}

	public void TriggerMuzzleFlash()
	{
		if (!(muzzleFlash == null))
		{
			if (_flashCoroutine != null)
			{
				StopCoroutine(_flashCoroutine);
			}
			_flashCoroutine = StartCoroutine(FlashRoutine());
		}
	}

	private IEnumerator FlashRoutine()
	{
		Transform flashTransform = muzzleFlash.transform;
		Vector3 baseScale = Vector3.one;
		muzzleFlash.gameObject.SetActive(value: true);
		float riseTime = flashDuration * flashRiseFraction;
		float fallTime = flashDuration * (1f - flashRiseFraction);
		float elapsed2 = 0f;
		while (elapsed2 < riseTime)
		{
			float num = elapsed2 / riseTime;
			flashTransform.localScale = baseScale * Mathf.Lerp(0.25f, flashPeakScale, num);
			SetFlashAlpha(num);
			elapsed2 += Time.deltaTime;
			yield return null;
		}
		elapsed2 = 0f;
		while (elapsed2 < fallTime)
		{
			float num2 = elapsed2 / fallTime;
			flashTransform.localScale = baseScale * Mathf.Lerp(flashPeakScale, 0.25f, num2);
			SetFlashAlpha(1f - num2);
			elapsed2 += Time.deltaTime;
			yield return null;
		}
		flashTransform.localScale = baseScale;
		muzzleFlash.gameObject.SetActive(value: false);
		_flashCoroutine = null;
	}

	private void SetFlashAlpha(float alpha)
	{
		Color color = muzzleFlash.color;
		color.a = alpha;
		muzzleFlash.color = color;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(1))
		{
			StoreUIManager.Instance.ExitShootingMode();
		}
		if (Input.GetMouseButtonDown(0) && StoreUIManager.Instance.CanShoot())
		{
			StoreUIManager.Instance.Shoot();
		}
		float time = Time.time;
		float num = time * breathingFrequency * MathF.PI * 2f;
		float num2 = Mathf.Sin(num * 0.7f) * breathingAmplitudeX;
		float num3 = Mathf.Sin(num) * breathingAmplitudeY;
		float phase = time * heartbeatFrequency % 1f;
		float num4 = GaussianPulse(phase, 0f, heartbeatPulseWidth);
		float num5 = GaussianPulse(phase, heartbeatDubDelay, heartbeatPulseWidth * 0.6f);
		float num6 = (num4 + num5 * 0.5f) * heartbeatAmplitude;
		base.transform.localPosition = _restPosition + new Vector3(num2 + _recoilOffset.x, num3 + num6 + _recoilOffset.y, 0f);
	}

	private static float GaussianPulse(float phase, float centre, float width)
	{
		float num = phase - centre;
		if (num > 0.5f)
		{
			num -= 1f;
		}
		if (num < -0.5f)
		{
			num += 1f;
		}
		return Mathf.Exp((0f - num * num) / (2f * width * width));
	}
}
