using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class StoreUISounds
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Boot()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		AudioDirectory directory = UnityEngine.Object.FindObjectOfType<AudioDirectory>(includeInactive: true);
		if (!(directory == null))
		{
			Apply(directory, delegate(EmporiumAudioPlayer p)
			{
				Use(directory, p.audioButton1, "effect_button1");
				Use(directory, p.audioButton2, "effect_button2");
				Use(directory, p.audioButton3, "effect_button3");
				Use(directory, p.audioButton4, "effect_button4");
			});
			Apply(directory, delegate(RecorderUIManager r)
			{
				Use(directory, r.openAudio, "effect_button3");
				Use(directory, r.closeAudio, "effect_button4");
			});
			Apply(directory, delegate(PaperUIManager p)
			{
				Use(directory, p.audioSourcePickup, "effect_paper_pickup");
				Use(directory, p.audioSourceDrop, "effect_paper_drop");
			});
			Apply(directory, delegate(NewsUIManager n)
			{
				Use(directory, n.audioSourcePickup, "effect_paper_pickup");
				Use(directory, n.audioSourceDrop, "effect_paper_drop");
			});
			Apply(directory, delegate(LogUIManager l)
			{
				Use(directory, l.openAudio, "effect_paper_pickup");
				Use(directory, l.closeAudio, "effect_paper_drop");
			});
			Apply(directory, delegate(NotebookUIManager n)
			{
				Use(directory, n.audioSourcePickup, "effect_book_pickup");
				Use(directory, n.audioSourceDrop, "effect_book_drop");
			});
			Apply(directory, delegate(StoreDismissButton b)
			{
				Use(directory, b.audioSource, "effect_light_switch");
			});
			Apply(directory, delegate(StoreSecButton b)
			{
				Use(directory, b.audioSource, "effect_light_switch");
			});
			Apply(directory, delegate(StoreShutterButton b)
			{
				Use(directory, b.audioSource, "effect_light_switch");
			});
			Apply(directory, delegate(StoreTransferButton b)
			{
				Use(directory, b.audioSource, "effect_light_switch");
			});
			Apply(directory, delegate(EscapeUIManager e)
			{
				Use(directory, e.audioSource, "effect_light_switch");
			});
		}
	}

	private static void Apply<T>(AudioDirectory directory, Action<T> assign) where T : UnityEngine.Object
	{
		T[] array = UnityEngine.Object.FindObjectsOfType<T>(includeInactive: true);
		foreach (T obj in array)
		{
			assign(obj);
		}
	}

	private static void Use(AudioDirectory directory, AudioSource source, string identifier)
	{
		if (!(source == null))
		{
			if (directory.HasAudioClip(identifier))
			{
				source.clip = directory.GetAudioClip(identifier);
				return;
			}
			Debug.LogWarning("[StoreUISounds] No audio identifier '" + identifier + "' in the AudioDirectory; " + source.name + " keeps its scene clip.");
		}
	}
}
