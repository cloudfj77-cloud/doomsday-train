using UnityEngine;

public class TimeScaleManager : MonoBehaviour
{
	public static float devCambulletTimeScale = 1f;

	private float bulletTimeScale = 0.05f;

	private float bulletTimeTimer;

	private void Awake()
	{
		devCambulletTimeScale = 1f;
	}

	private void Update()
	{
		float a = 1f;
		a = Mathf.Min(a, devCambulletTimeScale);
		if (bulletTimeTimer > 0f)
		{
			bulletTimeTimer -= Time.unscaledDeltaTime;
			a = Mathf.Min(a, bulletTimeScale);
		}
		if (GameManager.Paused)
		{
			a = 0f;
		}
		if (CameraMode.Active)
		{
			a = 0f;
		}
		Time.timeScale = a;
		Time.fixedDeltaTime = Mathf.Max(0.0005f, Time.timeScale * 0.02f);
	}

	public void EnterBulletTime(float time)
	{
		bulletTimeTimer = Mathf.Max(bulletTimeTimer, time);
	}
}
