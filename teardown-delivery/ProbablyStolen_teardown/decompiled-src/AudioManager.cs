using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	private static AudioManager mInstance;

	public AudioSource audioSource;

	public AudioSource audioSource2;

	public AudioSource audioSource3;

	public AudioSource audioSourceAmbient;

	public AudioSource audioSourceMusic;

	public AudioDirectory audioDirectory;

	public Coroutine audioCoroutine;

	public static bool Exists => mInstance != null;

	public static AudioManager Instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = new GameObject
				{
					name = "AudioManagerContainer"
				}.AddComponent<AudioManager>();
				mInstance.audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
				mInstance.audioSource2 = GameObject.Find("AudioSource2").GetComponent<AudioSource>();
				mInstance.audioSource3 = GameObject.Find("AudioSource3").GetComponent<AudioSource>();
				mInstance.audioSourceAmbient = GameObject.Find("AudioSourceAmbient").GetComponent<AudioSource>();
				mInstance.audioSourceMusic = GameObject.Find("AudioSourceMusic").GetComponent<AudioSource>();
				mInstance.audioDirectory = GameObject.Find("AudioDirectory").GetComponent<AudioDirectory>();
			}
			return mInstance;
		}
	}

	private void Start()
	{
	}

	public void PlayAndCallback(Action onComplete)
	{
		audioCoroutine = StartCoroutine(WaitForSound(onComplete));
	}

	private IEnumerator WaitForSound(Action onComplete)
	{
		yield return new WaitUntil(() => audioSourceMusic.isPlaying);
		yield return new WaitWhile(() => audioSourceMusic.isPlaying);
		onComplete?.Invoke();
	}

	public static void StopAudioCoroutine()
	{
		if (Instance.audioCoroutine != null)
		{
			Instance.StopCoroutine(Instance.audioCoroutine);
			Instance.audioCoroutine = null;
		}
	}

	public static void OnTrackDone()
	{
		Debug.Log("Audio finished, function executed!");
		MachineCassettePlayer.OnNextButtonPressed(EmporiumEntry.Instance.cassettePlayer, MachineCassettePlayer.GetCassetteInventory());
	}

	public void PauseAllSource()
	{
		AudioListener.pause = true;
	}

	public void ResumeAllSource()
	{
		AudioListener.pause = false;
	}

	public bool Play(string identifier)
	{
		if (!CanPlay(identifier))
		{
			return false;
		}
		AudioClip audioClip = audioDirectory.GetAudioClip(identifier);
		Instance.audioSource.clip = audioClip;
		Instance.audioSource.Play();
		return true;
	}

	public bool Play2(string identifier)
	{
		if (!CanPlay(identifier))
		{
			return false;
		}
		AudioClip audioClip = audioDirectory.GetAudioClip(identifier);
		Instance.audioSource2.clip = audioClip;
		Instance.audioSource2.Play();
		return true;
	}

	public bool Play3(string identifier)
	{
		if (!CanPlay(identifier))
		{
			return false;
		}
		AudioClip audioClip = audioDirectory.GetAudioClip(identifier);
		Instance.audioSource3.clip = audioClip;
		Instance.audioSource3.Play();
		return true;
	}

	public void StopPlay3()
	{
		Instance.audioSource3.Stop();
	}

	public void PlayAmbient(string identifier)
	{
		AudioClip audioClip = audioDirectory.GetAudioClip(identifier);
		Instance.audioSourceAmbient.clip = audioClip;
		Instance.audioSourceAmbient.Play();
	}

	public void PlayMusic(string identifier)
	{
		AudioClip audioClip = audioDirectory.GetAudioClip(identifier);
		Instance.audioSourceMusic.clip = audioClip;
		Instance.audioSourceMusic.Play();
	}

	public void PauseMusic()
	{
		Instance.audioSourceMusic.Pause();
	}

	public void UnpauseMusic()
	{
		Instance.audioSourceMusic.UnPause();
	}

	public void StopMusic()
	{
		Instance.audioSourceMusic.Stop();
	}

	public bool CanPlay(string identifier)
	{
		if (identifier == null)
		{
			return false;
		}
		bool num = audioDirectory.HasAudioClip(identifier);
		if (!num)
		{
			Debug.LogWarning("Audio identifier " + identifier + " was not found! No sound will play.");
		}
		return num;
	}

	public bool TryPlayOne(params string[] list)
	{
		foreach (string identifier in list)
		{
			if (Instance.CanPlay(identifier))
			{
				Instance.Play(identifier);
				return true;
			}
		}
		return false;
	}
}
