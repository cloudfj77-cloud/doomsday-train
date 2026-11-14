using System;
using System.Collections.Generic;
using UnityEngine;

public static class BlackLampHelper
{
	public const string BLACK_LAMP_ID = "black_lamp";

	public const string LAMP_TARGET_TAG = "LAMP_TARGET_TAG";

	public static readonly string[] TARGETS = new string[3] { "foodThief", "pettyThief", "thief" };

	private const string LAMP_ATLAS = "Items/items_sign";

	private const string LAMP_SPRITE_OFF = "black_lamp";

	private static readonly string[] LAMP_SPRITE_ON = new string[3] { "black_lamp_on_alt_2", "black_lamp_on_alt_3", "black_lamp_on_alt_1" };

	private static readonly Dictionary<string, int> BASE_CHANCE_BY_TARGET = new Dictionary<string, int>
	{
		{ "foodThief", 30 },
		{ "pettyThief", 20 },
		{ "thief", 10 }
	};

	private const int DEFAULT_BASE_CHANCE = 15;

	private const int REP_CHANCE_DIVISOR = 2;

	private const int CHANCE_CAP = 95;

	public static void InitBlackLamp(GameItem item)
	{
		item.mayStackItemFunc = (GameItem _, GameItem _) => false;
		item.ModifyTag("LAMP_TARGET_TAG", delegate(TagState state)
		{
			state.Enable().SetString(TARGETS[0]);
		});
		item.mayActivateSlotItemFunc = (GameItem lamp, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(lamp);
		item.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.onActivateSlotItemFunc = delegate(GameItem lamp, GameInventory _, SlotMarker _)
		{
			CycleTarget(lamp);
		};
	}

	public static string GetTarget(GameItem item)
	{
		string @string = item.GetTagReadonly("LAMP_TARGET_TAG").GetString();
		if (!string.IsNullOrEmpty(@string))
		{
			return @string;
		}
		return TARGETS[0];
	}

	public static void CycleTarget(GameItem item)
	{
		int num = Array.IndexOf(TARGETS, GetTarget(item));
		int next = (num + 1) % TARGETS.Length;
		item.ModifyTag("LAMP_TARGET_TAG", delegate(TagState state)
		{
			state.Enable().SetString(TARGETS[next]);
		});
		RefreshLampSprite(item);
		AudioManager.Instance.Play2("click_vintage");
	}

	public static void RefreshLampSprite(GameItem item)
	{
		if (item == null || !item.identifier.Equals("black_lamp"))
		{
			return;
		}
		if (IsInDisplayCase(item))
		{
			int num = Array.IndexOf(TARGETS, GetTarget(item));
			if (num < 0)
			{
				num = 0;
			}
			item.SetSpriteQuick("Items/items_sign", LAMP_SPRITE_ON[num]);
		}
		else
		{
			item.SetSpriteQuick("Items/items_sign", "black_lamp");
		}
	}

	public static int GetLampCountForTarget(string targetId)
	{
		int num = 0;
		foreach (GameItem childItem in EmporiumEntry.Instance.showcaseElement.childItems)
		{
			if (childItem.identifier.Equals("black_lamp") && GetTarget(childItem).Equals(targetId))
			{
				num++;
			}
		}
		return num;
	}

	public static int GetBaseChance(string targetId)
	{
		if (!BASE_CHANCE_BY_TARGET.TryGetValue(targetId, out var value))
		{
			return 15;
		}
		return value;
	}

	public static int GetChanceForTarget(string targetId)
	{
		int lampCountForTarget = GetLampCountForTarget(targetId);
		if (lampCountForTarget <= 0)
		{
			return 0;
		}
		int bMReputation = StoreReputation.GetBMReputation();
		return Mathf.Clamp(lampCountForTarget * GetBaseChance(targetId) + bMReputation / 2, 0, 95);
	}

	public static List<(string targetId, int chance)> GetActiveInvitations()
	{
		List<(string, int)> list = new List<(string, int)>();
		HashSet<string> hashSet = new HashSet<string>();
		foreach (GameItem childItem in EmporiumEntry.Instance.showcaseElement.childItems)
		{
			if (childItem.identifier.Equals("black_lamp"))
			{
				string target = GetTarget(childItem);
				if (hashSet.Add(target))
				{
					list.Add((target, GetChanceForTarget(target)));
				}
			}
		}
		return list;
	}

	public static void CreateBlackLampTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.identifier.Equals("black_lamp"))
		{
			string clientDisplayName = StoreClientListDict.GetClientDisplayName(GetTarget(item));
			builder.Add(LocHelper.GetLocalizedMechanic("mech_black_lamp_signaling", clientDisplayName), RenderHandler.ColorPalette.LightRed, "", bold: true);
			if (IsInDisplayCase(item))
			{
				builder.AddLine(" (" + GetChanceForTarget(GetTarget(item)) + "%)", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed, bold: true);
			}
			else
			{
				builder.AddLine("");
			}
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_black_lamp_hint"), ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue, bold: false, italic: true);
		}
	}

	private static bool IsInDisplayCase(GameItem item)
	{
		foreach (GameItem childItem in EmporiumEntry.Instance.showcaseElement.childItems)
		{
			if (childItem == item)
			{
				return true;
			}
		}
		return false;
	}
}
