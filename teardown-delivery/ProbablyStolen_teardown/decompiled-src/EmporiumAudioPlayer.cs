using UnityEngine;

public class EmporiumAudioPlayer : MonoBehaviour
{
	public AudioSource audioButton1;

	public AudioSource audioButton2;

	public AudioSource audioButton3;

	public AudioSource audioButton4;

	public AudioSource audioBeatVictory;

	public AudioSource audioRegister;

	public AudioSource audioStep1;

	public AudioSource audioStep2;

	public AudioSource audioClickClick;

	public AudioSource audioNegativeTwoBeep;

	public AudioSource audioExposeScam;

	public AudioSource audioLabeler;

	public AudioSource audioFail;

	public AudioSource audioBedroll;

	public AudioSource audioLampClick;

	public AudioSource audioLampSnap;

	public AudioSource audioWriting;

	public AudioSource flip1;

	public AudioSource alarm2;

	public AudioSource airlock_ext_open;

	public AudioSource airlock_ext_close;

	public AudioSource cross;

	public AudioSource dumpWater;

	public AudioSource wirecutter;

	public AudioSource ratchet;

	public AudioSource ratchet2;

	public AudioSource welder;

	public int stepPlaying;

	public static EmporiumAudioPlayer Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Play()
	{
		audioButton1.Play();
	}

	public void PlayMatched()
	{
		audioButton3.Play();
	}

	public void PlayUnMatched()
	{
		audioButton4.Play();
	}

	public void PlayBeatVictory()
	{
		audioBeatVictory.Play();
	}

	public void PlayWalking()
	{
		audioStep1.Play();
		stepPlaying = 0;
		for (int i = 0; i < 4; i++)
		{
			StartCoroutine(MetaHelper.WaitAndDo((float)(i + 1) * 0.55f, delegate
			{
				stepPlaying++;
				if (stepPlaying % 2 == 1)
				{
					audioStep1.Play();
				}
				else
				{
					audioStep2.Play();
				}
			}));
		}
	}

	public void PlayRegister()
	{
		audioRegister.Play();
	}
}
