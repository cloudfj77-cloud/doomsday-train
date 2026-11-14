using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimationHelper
{
	private class AnimationEntry
	{
		public GameItemElement item;

		public Sprite[] frames;

		public string framesKey;

		public int frameMs;

		public bool playing;

		public int lastFrameIndex = -1;
	}

	public const string SPRITE_ANIMATION_TAG = "SPRITE_ANIMATION_TAG";

	public const string SPRITE_ANIMATION_FRAME_MS_INT = "SPRITE_ANIMATION_FRAME_MS_INT";

	public const string SPRITE_ANIMATION_FRAME_COUNT_INT = "SPRITE_ANIMATION_FRAME_COUNT_INT";

	public const string SPRITE_ANIMATION_FRAMES_STRING = "SPRITE_ANIMATION_FRAMES_STRING";

	public const string SPRITE_ANIMATION_PLAYING_BOOL = "SPRITE_ANIMATION_PLAYING_BOOL";

	private static readonly List<AnimationEntry> entries = new List<AnimationEntry>();

	public static void InitSpriteAnimation(GameItem item, int frameCount, int frameMs)
	{
		item.ModifyTag("SPRITE_ANIMATION_FRAME_COUNT_INT", delegate(TagState s)
		{
			s.Enable().SetInt(frameCount);
		}, skipValidate: true);
		FinishInit(item, frameMs);
	}

	public static void InitSpriteAnimation(GameItem item, List<string> frameNames, int frameMs)
	{
		item.ModifyTag("SPRITE_ANIMATION_FRAMES_STRING", delegate(TagState s)
		{
			s.Enable().SetString(string.Join(",", frameNames ?? new List<string>()));
		}, skipValidate: true);
		FinishInit(item, frameMs);
	}

	private static void FinishInit(GameItem item, int frameMs)
	{
		item.ModifyTag("SPRITE_ANIMATION_FRAME_MS_INT", delegate(TagState s)
		{
			s.Enable().SetInt(frameMs);
		}, skipValidate: true);
		item.ModifyTag("SPRITE_ANIMATION_PLAYING_BOOL", delegate(TagState s)
		{
			s.Enable().SetBool(val: true);
		}, skipValidate: true);
		item.ModifyTag("SPRITE_ANIMATION_TAG", delegate(TagState s)
		{
			s.Enable();
		});
	}

	public static void RemoveSpriteAnimation(GameItem item)
	{
		item.ModifyTag("SPRITE_ANIMATION_FRAME_COUNT_INT", delegate(TagState s)
		{
			s.Destroy();
		}, skipValidate: true);
		item.ModifyTag("SPRITE_ANIMATION_FRAMES_STRING", delegate(TagState s)
		{
			s.Destroy();
		}, skipValidate: true);
		item.ModifyTag("SPRITE_ANIMATION_FRAME_MS_INT", delegate(TagState s)
		{
			s.Destroy();
		}, skipValidate: true);
		item.ModifyTag("SPRITE_ANIMATION_PLAYING_BOOL", delegate(TagState s)
		{
			s.Destroy();
		}, skipValidate: true);
		item.ModifyTag("SPRITE_ANIMATION_TAG", delegate(TagState s)
		{
			s.Destroy();
		});
	}

	public static bool IsSpriteAnimated(GameItem item)
	{
		return item.IsTag("SPRITE_ANIMATION_TAG");
	}

	public static void SetSpriteAnimationPlaying(GameItem item, bool playing)
	{
		if (item.IsTag("SPRITE_ANIMATION_TAG"))
		{
			item.ModifyTag("SPRITE_ANIMATION_PLAYING_BOOL", delegate(TagState s)
			{
				s.Enable().SetBool(playing);
			});
		}
	}

	public static void SetSpriteAnimationSpeed(GameItem item, int frameMs)
	{
		if (item.IsTag("SPRITE_ANIMATION_TAG"))
		{
			item.ModifyTag("SPRITE_ANIMATION_FRAME_MS_INT", delegate(TagState s)
			{
				s.Enable().SetInt(frameMs);
			});
		}
	}

	public static void SetSpriteAnimationFrames(GameItem item, List<string> frameNames)
	{
		if (item.IsTag("SPRITE_ANIMATION_TAG"))
		{
			item.ModifyTag("SPRITE_ANIMATION_FRAMES_STRING", delegate(TagState s)
			{
				s.Enable().SetString(string.Join(",", frameNames ?? new List<string>()));
			});
		}
	}

	public static void SyncSpriteAnimation(GameItemElement item)
	{
		if (!item.IsTag("SPRITE_ANIMATION_TAG"))
		{
			if (entries.Count > 0)
			{
				RemoveEntry(item);
			}
			return;
		}
		AnimationEntry animationEntry = FindEntry(item);
		if (animationEntry == null)
		{
			animationEntry = new AnimationEntry
			{
				item = item
			};
			entries.Add(animationEntry);
		}
		animationEntry.frameMs = Mathf.Max(1, item.GetTagReadonly("SPRITE_ANIMATION_FRAME_MS_INT").GetInt());
		animationEntry.playing = item.GetTagReadonly("SPRITE_ANIMATION_PLAYING_BOOL").GetBool();
		string text = item.GetTagReadonly("SPRITE_ANIMATION_FRAMES_STRING").GetString() + "|" + item.GetTagReadonly("SPRITE_ANIMATION_FRAME_COUNT_INT").GetInt();
		if (animationEntry.framesKey != text)
		{
			animationEntry.framesKey = text;
			animationEntry.frames = ResolveFrames(item);
			animationEntry.lastFrameIndex = -1;
		}
	}

	public static void Tick()
	{
		if (entries.Count == 0)
		{
			return;
		}
		double num = Time.unscaledTimeAsDouble * 1000.0;
		for (int num2 = entries.Count - 1; num2 >= 0; num2--)
		{
			AnimationEntry animationEntry = entries[num2];
			if (animationEntry.item.IsDestroyed())
			{
				entries.RemoveAt(num2);
			}
			else if (animationEntry.playing && animationEntry.frames != null && animationEntry.frames.Length >= 2 && animationEntry.item.handler.gameObject.activeInHierarchy)
			{
				int num3 = (int)(num / (double)animationEntry.frameMs) % animationEntry.frames.Length;
				if (num3 != animationEntry.lastFrameIndex)
				{
					animationEntry.lastFrameIndex = num3;
					animationEntry.item.ApplyAnimationFrame(animationEntry.frames[num3]);
				}
			}
		}
	}

	private static AnimationEntry FindEntry(GameItemElement item)
	{
		for (int i = 0; i < entries.Count; i++)
		{
			if (entries[i].item == item)
			{
				return entries[i];
			}
		}
		return null;
	}

	private static void RemoveEntry(GameItemElement item)
	{
		for (int num = entries.Count - 1; num >= 0; num--)
		{
			if (entries[num].item == item)
			{
				entries.RemoveAt(num);
			}
		}
	}

	private static Sprite[] ResolveFrames(GameItemElement item)
	{
		List<Sprite> list = new List<Sprite>();
		string @string = item.GetTagReadonly("SPRITE_ANIMATION_FRAMES_STRING").GetString();
		if (@string != null && @string.Length > 0)
		{
			string[] array = @string.Split(',');
			foreach (string text in array)
			{
				list.Add(item.ResolveSpriteByName(text.Trim()));
			}
		}
		else
		{
			int @int = item.GetTagReadonly("SPRITE_ANIMATION_FRAME_COUNT_INT").GetInt();
			for (int j = 0; j < @int; j++)
			{
				list.Add(item.ResolveSpriteByName(item.spritePath + "_" + j));
			}
		}
		for (int k = 1; k < list.Count; k++)
		{
			if ((object)list[0] != null && (object)list[k] != null && list[k].rect.size != list[0].rect.size)
			{
				Debug.LogWarning("Sprite animation frame " + k + " of item '" + item.identifier + "' has a different pixel size than frame 0. All frames must share the same size.");
			}
		}
		return list.ToArray();
	}
}
