using System.Collections.Generic;
using UnityEngine;

public static class PaperSpriteEffects
{
	private static readonly Dictionary<(Sprite, Color32), Sprite> silhouettes = new Dictionary<(Sprite, Color32), Sprite>();

	private static readonly Dictionary<(Sprite, Color32), Sprite> fills = new Dictionary<(Sprite, Color32), Sprite>();

	public static Sprite FillEnclosed(Sprite source, Color color)
	{
		if (source == null || source.texture == null)
		{
			return source;
		}
		Color32 color2 = color;
		(Sprite, Color32) key = (source, color2);
		if (fills.TryGetValue(key, out var value) && value != null)
		{
			return value;
		}
		int w;
		int h;
		Color32[] pixels = ReadPixels(source, out w, out h);
		if (pixels == null)
		{
			return source;
		}
		bool[] outside = new bool[w * h];
		Queue<int> queue = new Queue<int>();
		for (int i = 0; i < w; i++)
		{
			Seed(i, 0);
			Seed(i, h - 1);
		}
		for (int j = 0; j < h; j++)
		{
			Seed(0, j);
			Seed(w - 1, j);
		}
		while (queue.Count > 0)
		{
			int num = queue.Dequeue();
			int num2 = num % w;
			int num3 = num / w;
			if (num2 > 0)
			{
				Seed(num2 - 1, num3);
			}
			if (num2 < w - 1)
			{
				Seed(num2 + 1, num3);
			}
			if (num3 > 0)
			{
				Seed(num2, num3 - 1);
			}
			if (num3 < h - 1)
			{
				Seed(num2, num3 + 1);
			}
		}
		bool flag = false;
		for (int k = 0; k < pixels.Length; k++)
		{
			if (pixels[k].a < 128 && !outside[k])
			{
				pixels[k] = color2;
				flag = true;
			}
		}
		if (!flag)
		{
			fills[key] = source;
			return source;
		}
		Sprite sprite = CreateSprite(pixels, w, h, source, "_filled");
		fills[key] = sprite;
		return sprite;
		void Seed(int x, int y)
		{
			int num4 = y * w + x;
			if (!outside[num4] && pixels[num4].a < 128)
			{
				outside[num4] = true;
				queue.Enqueue(num4);
			}
		}
	}

	private static Sprite CreateSprite(Color32[] pixels, int w, int h, Sprite source, string suffix)
	{
		Texture2D texture2D = new Texture2D(w, h, TextureFormat.RGBA32, mipChain: false);
		texture2D.name = source.name + suffix;
		texture2D.filterMode = source.texture.filterMode;
		texture2D.wrapMode = TextureWrapMode.Clamp;
		texture2D.SetPixels32(pixels);
		texture2D.Apply(updateMipmaps: false, makeNoLongerReadable: true);
		Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, w, h), new Vector2(0.5f, 0.5f), source.pixelsPerUnit, 0u, SpriteMeshType.FullRect, source.border);
		sprite.name = source.name + suffix;
		return sprite;
	}

	public static Sprite Silhouette(Sprite source, Color color)
	{
		if (source == null || source.texture == null)
		{
			return null;
		}
		Color32 item = color;
		(Sprite, Color32) key = (source, item);
		if (silhouettes.TryGetValue(key, out var value) && value != null)
		{
			return value;
		}
		int w;
		int h;
		Color32[] array = ReadPixels(source, out w, out h);
		if (array == null)
		{
			return null;
		}
		Color32 color2 = new Color32(0, 0, 0, 0);
		bool flag = false;
		for (int i = 0; i < array.Length; i++)
		{
			byte a = array[i].a;
			array[i] = ((a > 0) ? new Color32(item.r, item.g, item.b, a) : color2);
			flag = flag || a > 0;
		}
		if (!flag)
		{
			return null;
		}
		Sprite sprite = CreateSprite(array, w, h, source, "_silhouette");
		silhouettes[key] = sprite;
		return sprite;
	}

	private static Color32[] ReadPixels(Sprite source, out int w, out int h)
	{
		Rect textureRect = source.textureRect;
		int num = Mathf.RoundToInt(textureRect.x);
		int num2 = Mathf.RoundToInt(textureRect.y);
		w = Mathf.RoundToInt(textureRect.width);
		h = Mathf.RoundToInt(textureRect.height);
		if (w <= 0 || h <= 0)
		{
			return null;
		}
		Texture2D texture = source.texture;
		RenderTexture temporary = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.ARGB32);
		RenderTexture active = RenderTexture.active;
		try
		{
			Graphics.Blit(texture, temporary);
			RenderTexture.active = temporary;
			Texture2D texture2D = new Texture2D(w, h, TextureFormat.RGBA32, mipChain: false);
			texture2D.ReadPixels(new Rect(num, num2, w, h), 0, 0);
			texture2D.Apply();
			Color32[] pixels = texture2D.GetPixels32();
			Object.Destroy(texture2D);
			return pixels;
		}
		finally
		{
			RenderTexture.active = active;
			RenderTexture.ReleaseTemporary(temporary);
		}
	}
}
