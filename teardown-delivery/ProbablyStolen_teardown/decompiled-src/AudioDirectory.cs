using System.Collections.Generic;
using UnityEngine;

public class AudioDirectory : MonoBehaviour
{
	public AudioAsset[] audioAsset;

	public AudioAsset[] audioAsset2;

	public AudioAsset[] audioAsset3;

	public AudioAsset[] music;

	public AudioAsset[] music2;

	public AudioAsset[] item1;

	public AudioAsset[] item2;

	public AudioAsset[] effect1;

	public Dictionary<string, AudioClip> audioAssetDict;

	private static readonly Dictionary<string, AudioClip> modClips = new Dictionary<string, AudioClip>();

	private static AudioDirectory activeDirectory;

	private bool initialized;

	public static void RegisterModClip(string identifier, AudioClip clip)
	{
		modClips[identifier] = clip;
		if (activeDirectory != null && activeDirectory.audioAssetDict != null)
		{
			activeDirectory.audioAssetDict[identifier] = clip;
		}
	}

	private void Start()
	{
		EnsureInit();
	}

	private void EnsureInit()
	{
		if (initialized)
		{
			return;
		}
		initialized = true;
		int num = 0;
		audioAssetDict = new Dictionary<string, AudioClip>();
		for (int i = 0; i < audioAsset.Length; i++)
		{
			audioAssetDict.Add(audioAsset[i].identifier, audioAsset[i].sound);
			num++;
		}
		for (int j = 0; j < audioAsset2.Length; j++)
		{
			audioAssetDict.Add(audioAsset2[j].identifier, audioAsset2[j].sound);
			num++;
		}
		for (int k = 0; k < audioAsset3.Length; k++)
		{
			audioAssetDict.Add(audioAsset3[k].identifier, audioAsset3[k].sound);
			num++;
		}
		for (int l = 0; l < music.Length; l++)
		{
			audioAssetDict.Add(music[l].identifier, music[l].sound);
			num++;
		}
		for (int m = 0; m < music2.Length; m++)
		{
			audioAssetDict.Add(music2[m].identifier, music2[m].sound);
			num++;
		}
		for (int n = 0; n < item1.Length; n++)
		{
			audioAssetDict.Add(item1[n].identifier, item1[n].sound);
			num++;
		}
		for (int num2 = 0; num2 < item2.Length; num2++)
		{
			audioAssetDict.Add(item2[num2].identifier, item2[num2].sound);
			num++;
		}
		if (effect1 != null)
		{
			for (int num3 = 0; num3 < effect1.Length; num3++)
			{
				audioAssetDict.Add(effect1[num3].identifier, effect1[num3].sound);
				num++;
			}
		}
		activeDirectory = this;
		foreach (KeyValuePair<string, AudioClip> modClip in modClips)
		{
			audioAssetDict[modClip.Key] = modClip.Value;
			num++;
		}
		Debug.Log(num + " audio clip was loaded successfully for AudioManager");
	}

	private void Update()
	{
	}

	public AudioClip GetAudioClip(string identifier)
	{
		EnsureInit();
		return audioAssetDict[identifier];
	}

	public bool HasAudioClip(string identifier)
	{
		if (identifier == null)
		{
			return false;
		}
		EnsureInit();
		return audioAssetDict.ContainsKey(identifier);
	}
}
