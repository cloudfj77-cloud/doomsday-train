using UnityEngine;

public class StoreLamp : MonoBehaviour, IClickable
{
	public GameObject light;

	public GameObject low;

	public GameObject high;

	private int currentState;

	private int clickCount;

	private bool isBroken;

	private float clickCooldownTimer;

	private const float clickCooldownInterval = 0.25f;

	public void OnClick()
	{
		if (isBroken)
		{
			return;
		}
		clickCount++;
		if (clickCount >= 4)
		{
			BreakLamp();
			clickCount = 0;
			return;
		}
		if (currentState == 0)
		{
			currentState = 1;
			light.SetActive(value: true);
			low.SetActive(value: true);
			high.SetActive(value: false);
		}
		else
		{
			TurnOffLamp();
		}
		EmporiumAudioPlayer.Instance.audioLampClick.Play();
	}

	public void TurnOffLamp()
	{
		light.SetActive(value: false);
		low.SetActive(value: false);
		high.SetActive(value: true);
		currentState = 0;
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (clickCooldownTimer > 0f)
		{
			clickCooldownTimer -= Time.deltaTime;
		}
		else if (clickCount > 0)
		{
			clickCount--;
			clickCooldownTimer = 0.25f;
		}
	}

	public void BreakLamp()
	{
		EmporiumAudioPlayer.Instance.audioLampSnap.Play();
		isBroken = true;
		TurnOffLamp();
		high.SetActive(value: false);
	}
}
