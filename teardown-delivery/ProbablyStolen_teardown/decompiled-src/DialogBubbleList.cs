using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogBubbleList : MonoBehaviour
{
	[Serializable]
	public class SpriteEntry
	{
		public string key;

		public Sprite sprite;
	}

	[SerializeField]
	private List<SpriteEntry> spriteEntries = new List<SpriteEntry>();

	[HideInInspector]
	public Dictionary<string, Sprite> spriteDictionary;

	private void Awake()
	{
		InitializeDictionary();
	}

	private void InitializeDictionary()
	{
		spriteDictionary = new Dictionary<string, Sprite>();
		foreach (SpriteEntry spriteEntry in spriteEntries)
		{
			if (!string.IsNullOrEmpty(spriteEntry.key) && spriteEntry.sprite != null)
			{
				if (!spriteDictionary.ContainsKey(spriteEntry.key))
				{
					spriteDictionary.Add(spriteEntry.key, spriteEntry.sprite);
				}
				else
				{
					Debug.LogWarning("Duplicate key detected: " + spriteEntry.key + ". Entry skipped.");
				}
			}
			else
			{
				Debug.LogWarning("Invalid sprite entry. Ensure all keys and sprites are assigned.");
			}
		}
	}

	public Sprite GetSprite(string key)
	{
		if (spriteDictionary.TryGetValue(key, out var value))
		{
			return value;
		}
		Debug.LogWarning("Sprite Value not found for key in Dict: " + key);
		return spriteDictionary["default"];
	}

	public bool IsContainKey(string key)
	{
		return spriteDictionary.ContainsKey(key);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
