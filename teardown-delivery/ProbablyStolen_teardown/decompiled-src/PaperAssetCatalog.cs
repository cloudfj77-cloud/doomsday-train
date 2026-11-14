using System;
using System.Collections.Generic;
using UnityEngine;

public class PaperAssetCatalog : ScriptableObject
{
	public enum Kind
	{
		Background,
		Button,
		Illustration
	}

	[Serializable]
	public class Entry
	{
		public string key;

		public Sprite sprite;

		public Kind kind;

		public Vector2 displaySize;

		public string sourcePanel;
	}

	public const string RESOURCE_PATH = "PaperUI/PaperAssetCatalog";

	public List<Entry> entries = new List<Entry>();

	private Dictionary<string, Entry> byKey;

	private Dictionary<Sprite, Entry> bySprite;

	public void ResetLookup()
	{
		byKey = null;
		bySprite = null;
	}

	private void BuildLookup()
	{
		if (byKey != null)
		{
			return;
		}
		byKey = new Dictionary<string, Entry>();
		bySprite = new Dictionary<Sprite, Entry>();
		foreach (Entry entry in entries)
		{
			if (entry != null && !(entry.sprite == null) && !string.IsNullOrEmpty(entry.key))
			{
				if (!byKey.ContainsKey(entry.key))
				{
					byKey[entry.key] = entry;
				}
				if (!bySprite.ContainsKey(entry.sprite))
				{
					bySprite[entry.sprite] = entry;
				}
			}
		}
	}

	public Entry Find(string key)
	{
		BuildLookup();
		if (string.IsNullOrEmpty(key) || !byKey.TryGetValue(key, out var value))
		{
			return null;
		}
		return value;
	}

	public Entry FindBySprite(Sprite sprite)
	{
		BuildLookup();
		if (!(sprite != null) || !bySprite.TryGetValue(sprite, out var value))
		{
			return null;
		}
		return value;
	}

	public IEnumerable<Entry> OfKind(Kind kind)
	{
		foreach (Entry entry in entries)
		{
			if (entry != null && entry.sprite != null && entry.kind == kind)
			{
				yield return entry;
			}
		}
	}
}
