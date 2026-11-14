using System.Collections.Generic;
using UnityEngine;

public static class PaperAssets
{
	public const string SPRITE_CLOSE = "paper_buttons_close1";

	public const string SPRITE_ZOOM = "paper_buttons_screen_0";

	public const string SPRITE_FLIP_LEFT = "paper_buttons_l_1";

	public const string SPRITE_FLIP_RIGHT = "paper_buttons1";

	public const string SPRITE_TEXT_BUTTON = "blue_button_2x";

	public const string SPRITE_GUIDE_LOGO = "guide_logo";

	private static PaperAssetCatalog catalog;

	private static bool catalogLoaded;

	private static readonly Dictionary<string, Sprite> slicedCache = new Dictionary<string, Sprite>();

	private static readonly HashSet<string> warnedKeys = new HashSet<string>();

	private const string ITEM_ATLAS_ROOT = "Items";

	private static List<(string key, Sprite sprite, string group)> itemAtlasSprites;

	public static PaperAssetCatalog Catalog
	{
		get
		{
			if (!catalogLoaded)
			{
				catalogLoaded = true;
				catalog = Resources.Load<PaperAssetCatalog>("PaperUI/PaperAssetCatalog");
				if (catalog == null)
				{
					Debug.LogWarning("[PaperUI] No paper asset catalog in Resources/PaperUI; run Build > Collect Paper UI Assets. Vanilla sprite names will not resolve.");
				}
			}
			return catalog;
		}
	}

	public static PaperAssetCatalog.Entry FindInCatalog(string key)
	{
		if (!catalogLoaded)
		{
			PaperAssetCatalog paperAssetCatalog = Resources.Load<PaperAssetCatalog>("PaperUI/PaperAssetCatalog");
			if (paperAssetCatalog == null)
			{
				return null;
			}
			catalog = paperAssetCatalog;
			catalogLoaded = true;
		}
		if (!(catalog != null))
		{
			return null;
		}
		return catalog.Find(key);
	}

	public static void ReloadCatalog()
	{
		catalogLoaded = false;
		catalog = null;
	}

	public static bool TryResolve(string key, string modId, out Sprite sprite)
	{
		sprite = Resolve(key, modId, quiet: true);
		return sprite != null;
	}

	public static Sprite Resolve(string key, string modId, bool quiet = false)
	{
		if (string.IsNullOrEmpty(key))
		{
			return null;
		}
		int num = key.IndexOf(':');
		if (num > 0)
		{
			Sprite sprite = FromMod(key.Substring(0, num), key.Substring(num + 1));
			if (sprite == null && !quiet)
			{
				WarnOnce(key, modId);
			}
			return sprite;
		}
		if (key.IndexOf('/') >= 0)
		{
			Sprite sprite2 = FromItemAtlas(key);
			if (sprite2 == null && !quiet)
			{
				WarnOnce(key, modId);
			}
			return sprite2;
		}
		PaperAssetCatalog.Entry entry = ((Catalog != null) ? Catalog.Find(key) : null);
		if (entry != null)
		{
			return entry.sprite;
		}
		if (CustomUIManager.Instance != null)
		{
			Sprite sprite3 = CustomUIManager.Instance.GetSprite(key, quiet: true);
			if (sprite3 != null)
			{
				return sprite3;
			}
		}
		if (!string.IsNullOrEmpty(modId))
		{
			Sprite sprite4 = FromMod(modId, key);
			if (sprite4 != null)
			{
				return sprite4;
			}
		}
		if (!quiet)
		{
			WarnOnce(key, modId);
		}
		return null;
	}

	private static void WarnOnce(string key, string modId)
	{
		if (warnedKeys.Add(modId + "|" + key))
		{
			Debug.LogWarning("[PaperUI] No sprite found for key '" + key + "'" + (string.IsNullOrEmpty(modId) ? "" : (" (mod '" + modId + "')")));
		}
	}

	public static Sprite FromItemAtlas(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return null;
		}
		string text = (key.StartsWith("Items/") ? key.Substring("Items".Length + 1) : key);
		int num = text.LastIndexOf('/');
		if (num <= 0 || num == text.Length - 1)
		{
			return null;
		}
		return RenderHandler.TryLoadFromAtlas("Items/" + text.Substring(0, num), text.Substring(num + 1));
	}

	public static List<(string key, Sprite sprite, string group)> ItemAtlasSprites()
	{
		if (itemAtlasSprites != null)
		{
			return itemAtlasSprites;
		}
		itemAtlasSprites = new List<(string, Sprite, string)>();
		Sprite[] array = Resources.LoadAll<Sprite>("Items");
		foreach (Sprite sprite in array)
		{
			if (!(sprite == null) && !(sprite.texture == null))
			{
				string name = sprite.texture.name;
				itemAtlasSprites.Add((name + "/" + sprite.name, sprite, "Items " + name));
			}
		}
		return itemAtlasSprites;
	}

	public static Sprite FromMod(string modId, string name)
	{
		LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == modId);
		if (loadedMod == null)
		{
			return null;
		}
		if (loadedMod.PaperSprites.TryGetValue(name, out var value))
		{
			return value;
		}
		if (loadedMod.Sprites.TryGetValue(name, out var value2))
		{
			return value2;
		}
		return null;
	}

	public static Sprite ResolveBackground(PaperBookData data, string modId)
	{
		Sprite sprite = Resolve(data.background, modId);
		if (sprite == null)
		{
			return null;
		}
		if (data.border == null || data.border.Length != 4)
		{
			return sprite;
		}
		Vector4 vector = new Vector4(data.border[0], data.border[1], data.border[2], data.border[3]);
		if (vector == Vector4.zero)
		{
			return sprite;
		}
		if (sprite.border == vector)
		{
			return sprite;
		}
		string key = $"{modId}|{data.background}|{vector}";
		if (slicedCache.TryGetValue(key, out var value) && value != null)
		{
			return value;
		}
		Sprite sprite2 = Sprite.Create(sprite.texture, sprite.rect, new Vector2(0.5f, 0.5f), sprite.pixelsPerUnit, 0u, SpriteMeshType.FullRect, vector);
		sprite2.name = sprite.name + "_sliced";
		slicedCache[key] = sprite2;
		return sprite2;
	}

	public static string KeyOf(Sprite sprite)
	{
		return ((Catalog != null) ? Catalog.FindBySprite(sprite) : null)?.key;
	}

	public static Vector2 DisplaySizeOf(string backgroundKey)
	{
		PaperAssetCatalog.Entry entry = ((Catalog != null) ? Catalog.Find(backgroundKey) : null);
		if (entry == null || entry.kind != 0)
		{
			return Vector2.zero;
		}
		return entry.displaySize;
	}

	public static List<(string key, Sprite sprite, string group)> ListAvailable(string modId, bool includeItemAtlases = true)
	{
		List<(string, Sprite, string)> list = new List<(string, Sprite, string)>();
		if (Catalog != null)
		{
			foreach (PaperAssetCatalog.Entry entry in Catalog.entries)
			{
				if (entry != null && !(entry.sprite == null))
				{
					list.Add((entry.key, entry.sprite, entry.kind.ToString()));
				}
			}
		}
		if (CustomUIManager.Instance != null)
		{
			foreach (KeyValuePair<string, Sprite> metaSprite in CustomUIManager.Instance.MetaSprites)
			{
				list.Add((metaSprite.Key, metaSprite.Value, metaSprite.Key.StartsWith("icons_") ? "Icons" : "Meta"));
			}
		}
		if (includeItemAtlases)
		{
			list.AddRange(ItemAtlasSprites());
		}
		if (!string.IsNullOrEmpty(modId))
		{
			LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == modId);
			if (loadedMod != null)
			{
				foreach (KeyValuePair<string, Sprite> paperSprite in loadedMod.PaperSprites)
				{
					list.Add((paperSprite.Key, paperSprite.Value, "Mod paper"));
				}
				foreach (KeyValuePair<string, Sprite> sprite in loadedMod.Sprites)
				{
					list.Add((sprite.Key, sprite.Value, "Mod items"));
				}
			}
		}
		return list;
	}
}
