using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DebrisHelper
{
	public const string DEBRIS_TAG = "DEBRIS_PILE_TAG";

	public const string DEBRIS_PROGRESS_INT = "DEBRIS_PROGRESS_INT";

	public const string DEBRIS_TOOLS_USED_STRING = "DEBRIS_TOOLS_USED_STRING";

	public const string REST_PILE_ID = "debris_pile1";

	public const string TOOL_PILE_ID = "debris_pile2";

	public const string SERVICE_PILE_ID = "debris_pile3";

	public const int REST_NIGHTS = 10;

	public const int SERVICE_NIGHTS = 10;

	public const int SERVICE_COST = 50;

	public const int SERVICE_PULLS_PER_NIGHT = 5;

	public const int CLEAR_PULLS = 10;

	public const int CLEAR_PULLS_NO_JUNK = 5;

	public const int TOOL_REWARD_COUNT = 3;

	private const string SPRITE_ATLAS = "Items/items_ship";

	private static readonly Dictionary<string, string> TOOL_REWARDS = new Dictionary<string, string>
	{
		{ "crowbar", "common_electronic" },
		{ "wire_cutter", "wire" },
		{ "welder", "scrap_metal" }
	};

	public static GameItem CreateRestPile()
	{
		GameItem gameItem = CreatePile("debris_pile1");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_debris_pile_rest_desc");
		gameItem.ModifyTag("DEBRIS_PROGRESS_INT", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		return gameItem;
	}

	public static GameItem CreateToolPile()
	{
		GameItem gameItem = CreatePile("debris_pile2");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_debris_pile_tool_desc");
		gameItem.ModifyTag("DEBRIS_TOOLS_USED_STRING", delegate(TagState state)
		{
			state.Enable().SetString("");
		});
		return gameItem;
	}

	public static GameItem CreateServicePile()
	{
		GameItem gameItem = CreatePile("debris_pile3");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_debris_pile_service_desc");
		gameItem.ModifyTag("DEBRIS_PROGRESS_INT", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		return gameItem;
	}

	private static GameItem CreatePile(string sprite)
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetSpriteAndShape("Items/items_ship", sprite);
		gameItem.name = LocHelper.GetLocalizedItem("item_debris_pile_name");
		gameItem.unitValue = 0L;
		gameItem.EnableTag("DEBRIS_PILE_TAG");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		AudioHelper.InitScrapMetal(gameItem);
		return gameItem;
	}

	public static bool IsDebris(GameItem item)
	{
		return item?.IsTag("DEBRIS_PILE_TAG") ?? false;
	}

	private static List<GameItem> FindPiles(string pileId)
	{
		return (from item in GraphUtils.FindAllChildrenType<GameItem>(EmporiumEntry.Instance.invElement)
			where IsDebris(item) && item.identifier == pileId
			select item).ToList();
	}

	public static bool HasServicePile()
	{
		return FindPiles("debris_pile3").Count > 0;
	}

	private static int GetProgress(GameItem pile)
	{
		return pile.GetTagReadonly("DEBRIS_PROGRESS_INT").GetInt();
	}

	public static void OnNight()
	{
		if (PlayerStore.Instance.lastAfterhourOutingDay != StoreStation.Instance.dayCounter)
		{
			foreach (GameItem item in FindPiles("debris_pile1"))
			{
				int num = Advance(item);
				GiveToBackCounter(DirectoryMaster.Item("scrap_metal"));
				GiveToBackCounter(PullDumpingGround(1, skipJunk: false));
				PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_debris_rest_progress", num, 10));
				if (num >= 10)
				{
					ClearPile(item, EmporiumEntry.Instance.backInvinvElement, nightLog: true);
				}
			}
		}
		StoreService debrisRemovalService = StoreService.GetDebrisRemovalService();
		if (debrisRemovalService == null || !debrisRemovalService.IsActive())
		{
			return;
		}
		GameItem gameItem = FindPiles("debris_pile3").FirstOrDefault();
		if (gameItem != null)
		{
			int num2 = Advance(gameItem);
			GiveToBackCounter(PullDumpingGround(5, skipJunk: false));
			PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_debris_crew_progress", num2, 10));
			if (num2 >= 10)
			{
				ClearPile(gameItem, EmporiumEntry.Instance.backInvinvElement, nightLog: true);
			}
		}
	}

	private static int Advance(GameItem pile)
	{
		int progress = GetProgress(pile) + 1;
		pile.ModifyTag("DEBRIS_PROGRESS_INT", delegate(TagState state)
		{
			state.Enable().SetInt(progress);
		});
		return progress;
	}

	public static void InitClearingTool(GameItem tool)
	{
		tool.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(tool.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => CanClearWithTool(s, t)));
		tool.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(tool.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => CanClearWithTool(s, t)));
		tool.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(tool.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			ClearWithTool(s, t);
		});
	}

	public static bool CanClearWithTool(GameItem tool, GameItem pile)
	{
		if (tool == null || pile == null)
		{
			return false;
		}
		if (pile.identifier != "debris_pile2" || !IsDebris(pile))
		{
			return false;
		}
		if (!TOOL_REWARDS.ContainsKey(tool.identifier))
		{
			return false;
		}
		if (!GeneralHelper.IsItemOwned(tool) || !GeneralHelper.IsItemOwned(pile))
		{
			return false;
		}
		return !GetUsedTools(pile).Contains(tool.identifier);
	}

	public static void ClearWithTool(GameItem tool, GameItem pile)
	{
		if (CanClearWithTool(tool, pile))
		{
			string identifier = tool.identifier;
			List<string> used = GetUsedTools(pile);
			used.Add(identifier);
			pile.ModifyTag("DEBRIS_TOOLS_USED_STRING", delegate(TagState state)
			{
				state.Enable().SetString(string.Join(",", used));
			});
			tool.Destroy();
			for (int i = 0; i < 3; i++)
			{
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, DirectoryMaster.Item(TOOL_REWARDS[identifier]));
			}
			AudioManager.Instance.Play2("debris");
			if (used.Count >= TOOL_REWARDS.Count)
			{
				ClearPile(pile, EmporiumEntry.Instance.invElement, nightLog: false);
			}
		}
	}

	private static List<string> GetUsedTools(GameItem pile)
	{
		string @string = pile.GetTagReadonly("DEBRIS_TOOLS_USED_STRING").GetString();
		if (!string.IsNullOrEmpty(@string))
		{
			return @string.Split(',').ToList();
		}
		return new List<string>();
	}

	private static void ClearPile(GameItem pile, GameInventory lootInventory, bool nightLog)
	{
		pile.Destroy();
		List<GameItem> list = PullDumpingGround(5, skipJunk: false);
		list.AddRange(PullDumpingGround(5, skipJunk: true));
		foreach (GameItem item in list)
		{
			GraphUtils.EmporiumTryAdd(lootInventory, item);
		}
		if (nightLog)
		{
			PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_debris_cleared"), "5c6f46");
		}
	}

	private static List<GameItem> PullDumpingGround(int count, bool skipJunk)
	{
		List<GameItem> list = new List<GameItem>();
		for (int i = 0; i < count; i++)
		{
			string text = (skipJunk ? LootRegistry.RollGroup("dumpingGroundTG", (string tableId) => (!(tableId == "junkTable")) ? 1f : 0f) : LootRegistry.RollGroup("dumpingGroundTG"));
			if (!string.IsNullOrEmpty(text))
			{
				GameItem gameItem = ItemSpawner.Spawn(text);
				if (gameItem != null)
				{
					list.Add(gameItem);
				}
			}
		}
		return list;
	}

	private static void GiveToBackCounter(GameItem item)
	{
		if (item != null)
		{
			GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, item);
		}
	}

	private static void GiveToBackCounter(List<GameItem> items)
	{
		foreach (GameItem item in items)
		{
			GiveToBackCounter(item);
		}
	}

	public static void PlaceHoarderPiles()
	{
		GameGridInventory invElement = EmporiumEntry.Instance.invElement;
		List<GameItem> list = new List<GameItem>
		{
			DirectoryMaster.Item("debris_pile1"),
			DirectoryMaster.Item("debris_pile2"),
			DirectoryMaster.Item("debris_pile3")
		};
		int width = invElement.inventoryShape.width;
		int height = invElement.inventoryShape.height;
		int[] array = list.Select((GameItem p) => p.modifiedShape.globalWidth).ToArray();
		int num = width - array[2];
		int[] array2 = new int[3]
		{
			0,
			array[0] + Mathf.Max(0, (num - array[0] - array[1]) / 2),
			num
		};
		List<GameItem> list2 = new List<GameItem>();
		for (int i = 0; i < list.Count; i++)
		{
			GameItem gameItem = list[i];
			GridShape gridShape = new GridShapeBuilder(gameItem.modifiedShape).SetPosition(array2[i], height - gameItem.modifiedShape.globalHeight).Build();
			foreach (GameItem childItem in invElement.childItems)
			{
				if (!list.Contains(childItem) && gridShape.Intersects(childItem.modifiedShape))
				{
					invElement.Expel(childItem);
					list2.Add(childItem);
				}
			}
			if (invElement.CheckShapePlacement(gameItem, gridShape, new List<GameItem>()))
			{
				gameItem.modifiedShape = new GridShapeBuilder(gameItem.modifiedShape).SetTransform(gridShape);
				invElement.UncheckedAccept(gameItem);
			}
			else
			{
				GraphUtils.EmporiumTryAdd(invElement, gameItem);
			}
		}
		foreach (GameItem item in list2)
		{
			GraphUtils.EmporiumTryAdd(invElement, item);
		}
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!IsDebris(item))
		{
			return;
		}
		builder.AddLine(" ");
		if (item.identifier == "debris_pile1")
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_machinery_progress") + " " + GetProgress(item) + "/" + 10, ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		else if (item.identifier == "debris_pile2")
		{
			List<string> usedTools = GetUsedTools(item);
			foreach (string key in TOOL_REWARDS.Keys)
			{
				bool flag = usedTools.Contains(key);
				builder.AddLine(GeneralHelper.GetDisplayNameFromID(key) + " " + (flag ? 1 : 0) + "/1", ignoreEmpty: true, flag ? RenderHandler.ColorPalette.LightGreen : RenderHandler.ColorPalette.LightCyan);
			}
		}
		else if (item.identifier == "debris_pile3")
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_machinery_progress") + " " + GetProgress(item) + "/" + 10, ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		builder.AddLine(LocHelper.GetLocalizedMechanic("mech_debris_tooltip_fixed"), ignoreEmpty: true, RenderHandler.ColorPalette.Gray);
	}
}
