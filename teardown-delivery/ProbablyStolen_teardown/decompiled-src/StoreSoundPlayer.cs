using System.Collections.Generic;
using UnityEngine;

public class StoreSoundPlayer : MonoBehaviour
{
	public AudioSource musicAudioSource;

	public AudioSource soundAudioSource;

	public AudioSource ambianceAudioSource;

	public AudioSource gameOverAudioSource;

	public AudioSource typewriterAudioSource;

	public AudioClip storeAmbient;

	public AudioClip dumpingGroundAmbient;

	public AudioClip commissaryAmbient;

	public AudioClip inventorAmbient;

	private readonly Dictionary<string, AudioClip> ambientById = new Dictionary<string, AudioClip>();

	private bool vanillaAmbientRegistered;

	public static StoreSoundPlayer Instance { get; private set; }

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

	public void StartGameOverMusic()
	{
		gameOverAudioSource.Play();
	}

	public void StopGameOverMusic()
	{
		gameOverAudioSource.Stop();
	}

	public void PlayInventorAmbient()
	{
		PlayAmbient("dr_jackson");
	}

	public void PlayCommissaryAmbient()
	{
		PlayAmbient("commissary");
	}

	public void PlayDumpingGroundAmbient()
	{
		PlayAmbient("dumping_ground");
	}

	public void PlayStoreAmbient()
	{
		ambianceAudioSource.clip = storeAmbient;
		ambianceAudioSource.Play();
	}

	public void RegisterAmbient(string id, AudioClip clip)
	{
		if (!string.IsNullOrEmpty(id) && !(clip == null))
		{
			ambientById[id] = clip;
		}
	}

	private void EnsureVanillaAmbient()
	{
		if (!vanillaAmbientRegistered)
		{
			vanillaAmbientRegistered = true;
			if (!ambientById.ContainsKey("dumping_ground"))
			{
				RegisterAmbient("dumping_ground", dumpingGroundAmbient);
			}
			if (!ambientById.ContainsKey("commissary"))
			{
				RegisterAmbient("commissary", commissaryAmbient);
			}
			if (!ambientById.ContainsKey("dr_jackson"))
			{
				RegisterAmbient("dr_jackson", inventorAmbient);
			}
		}
	}

	public AudioClip GetAmbient(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		EnsureVanillaAmbient();
		if (!ambientById.TryGetValue(id, out var value))
		{
			return null;
		}
		return value;
	}

	public void PlayAmbient(string id)
	{
		AudioClip ambient = GetAmbient(id);
		if (ambient == null)
		{
			Debug.LogWarning("[StoreSoundPlayer] No ambience registered for '" + id + "'");
			return;
		}
		ambianceAudioSource.clip = ambient;
		ambianceAudioSource.Play();
	}
}
