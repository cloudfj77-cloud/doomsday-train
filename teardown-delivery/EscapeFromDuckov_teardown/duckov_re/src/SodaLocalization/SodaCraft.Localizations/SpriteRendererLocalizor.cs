using System;
using System.Collections.Generic;
using UnityEngine;

namespace SodaCraft.Localizations;

public class SpriteRendererLocalizor : MonoBehaviour
{
	[Serializable]
	private struct LanguageSpritePair
	{
		public SystemLanguage language;

		public Sprite sprite;
	}

	[SerializeField]
	private List<LanguageSpritePair> entries;

	[SerializeField]
	private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		if (spriteRenderer == null)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
		}
		LocalizationManager.OnSetLanguage += OnSetLanguage;
	}

	private void OnDestroy()
	{
		LocalizationManager.OnSetLanguage -= OnSetLanguage;
	}

	private void Start()
	{
		Refresh();
	}

	private void OnSetLanguage(SystemLanguage language)
	{
		Refresh();
	}

	private void Refresh()
	{
		if (entries != null)
		{
			Sprite sprite = entries.Find((LanguageSpritePair e) => e.language == LocalizationManager.CurrentLanguage).sprite;
			if (sprite == null && entries.Count > 0)
			{
				sprite = entries[0].sprite;
			}
			spriteRenderer.sprite = sprite;
		}
	}

	private void OnValidate()
	{
		Refresh();
	}
}
