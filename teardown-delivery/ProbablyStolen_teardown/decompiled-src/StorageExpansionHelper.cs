using System;
using System.Collections.Generic;

public static class StorageExpansionHelper
{
	public const string IDENTIFIER = "storage_bay_expansion_kit";

	public const string STORAGE_EXPANSION_KIT_TAG = "STORAGE_EXPANSION_KIT_TAG";

	public const string STORAGE_EXPANSION_TABS_INT = "STORAGE_EXPANSION_TABS_INT";

	public const string STORAGE_TAB_NAMES_STRING = "STORAGE_TAB_NAMES_STRING";

	private const char TAB_NAME_SEPARATOR = '\n';

	public const int TAB_NAME_MAX_LENGTH = 12;

	public const int MAX_TABS = 5;

	public const int STORAGE_BAY_KIT_TABS = 2;

	public const int STORAGE_BAY_LARGE_KIT_TABS = 1;

	public const int KIT_VALUE = 1063;

	public static GameItem CreateKit()
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetSpriteAndShape("Items/items_tool2", "storage_bay_expansion_kit");
		gameItem.name = LocHelper.GetLocalizedItem("item_storage_bay_expansion_kit_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_storage_bay_expansion_kit_desc");
		gameItem.unitValue = 1063L;
		gameItem.EnableTag("STORAGE_EXPANSION_KIT_TAG");
		gameItem.SetGameItemType("TOOL");
		AudioHelper.InitSolidMetal(gameItem);
		gameItem.maySelectSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s);
		gameItem.canSelectSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s);
		gameItem.mayThisTargetItemFunc = (GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s, t) && CanExpand(t);
		gameItem.canThisTargetItemFunc = (GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s, t) && CanExpand(t);
		gameItem.onThisTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			Expand(t, s);
		};
		return gameItem;
	}

	private static int GetKitTabs(GameItem bay)
	{
		if (bay == null)
		{
			return 0;
		}
		if (bay.identifier == "storage_bay")
		{
			return 2;
		}
		if (bay.identifier == "storage_bay_gun")
		{
			return 2;
		}
		if (bay.identifier == "storage_bay_chem")
		{
			return 2;
		}
		if (bay.identifier == "storage_bay_large")
		{
			return 1;
		}
		return 0;
	}

	public static int GetExtraTabs(GameItem bay)
	{
		if (bay == null || !bay.IsTag("STORAGE_EXPANSION_TABS_INT"))
		{
			return 0;
		}
		return bay.GetTagReadonly("STORAGE_EXPANSION_TABS_INT").GetInt();
	}

	public static bool CanExpand(GameItem bay)
	{
		if (GetKitTabs(bay) == 0 || 1 + GetExtraTabs(bay) >= 5)
		{
			return false;
		}
		if (GetExtraTabs(bay) != 0)
		{
			return GetTabs(bay) != null;
		}
		return bay.contentWindow?.childElement is GameGridInventory;
	}

	public static void Expand(GameItem bay, GameItem kit)
	{
		if (CanExpand(bay))
		{
			int extraTabs = GetExtraTabs(bay);
			int added = Math.Min(GetKitTabs(bay), 4 - extraTabs);
			bay.ModifyTag("STORAGE_EXPANSION_TABS_INT", delegate(TagState state)
			{
				state.Enable().SetInt(extraTabs + added);
			});
			AddTabs(bay, added);
			bay.unitValue += kit.unitValue;
			kit.Destroy();
			EmporiumAudioPlayer.Instance.ratchet.Play();
		}
	}

	public static void RestoreTabs(GameItem item)
	{
		int num = Math.Min(GetExtraTabs(item), 4);
		if (num > 0 && GetKitTabs(item) != 0 && item.contentWindow?.childElement is GameGridInventory)
		{
			AddTabs(item, num);
		}
		InitTabNames(item);
	}

	private static MachineryTabs GetTabs(GameItem bay)
	{
		return (bay?.contentWindow?.childElement as MachineryPanelRow)?.GetPanel("storage")?.content as MachineryTabs;
	}

	private static void AddTabs(GameItem bay, int count)
	{
		PixelWindow contentWindow = bay.contentWindow;
		MachineryTabs machineryTabs = GetTabs(bay);
		if (machineryTabs == null)
		{
			if (!(contentWindow.childElement is GameGridInventory gameGridInventory))
			{
				return;
			}
			contentWindow.Detach(gameGridInventory);
			machineryTabs = new MachineryTabs();
			AddTab(machineryTabs, gameGridInventory);
			contentWindow.Attach(new MachineryPanelRow(MachineHelper.Panel(LocHelper.GetLocalizedMechanic("type_storage"), machineryTabs, "storage")));
		}
		if ((machineryTabs.GetPage(0) as MachineryIntake)?.content is GameGridInventory gameGridInventory2)
		{
			int width = gameGridInventory2.inventoryShape.width;
			int height = gameGridInventory2.inventoryShape.height;
			for (int i = 0; i < count; i++)
			{
				GameGridInventory gameGridInventory3 = new GameGridInventory(width, height).SetMachineryStyle();
				gameGridInventory3.mayInventoryAddItemFunc = gameGridInventory2.mayInventoryAddItemFunc;
				AddTab(machineryTabs, gameGridInventory3);
			}
			InitTabNames(bay);
			(contentWindow.childElement as MachineryPanelRow)?.Validate();
			contentWindow.Validate();
		}
	}

	private static void AddTab(MachineryTabs tabs, GameGridInventory grid)
	{
		tabs.AddTab(DefaultTabName(tabs.count), MachineHelper.WrapInIntake(grid));
	}

	public static void InitTabNames(GameItem bay)
	{
		MachineryTabs tabs = GetTabs(bay);
		if (tabs != null)
		{
			tabs.onRenameRequest = delegate(int index)
			{
				OpenRename(bay, index);
			};
			ApplyTabNames(bay, tabs);
		}
	}

	private static string DefaultTabName(int index)
	{
		return LocHelper.GetLocalizedMechanic("mech_storage_bay_tab", index + 1);
	}

	private static List<string> GetTabNames(GameItem bay)
	{
		List<string> list = new List<string>();
		if (bay != null && bay.IsTag("STORAGE_TAB_NAMES_STRING"))
		{
			list.AddRange((bay.GetTagReadonly("STORAGE_TAB_NAMES_STRING").GetString() ?? "").Split('\n'));
		}
		return list;
	}

	public static string GetTabName(GameItem bay, int index)
	{
		List<string> tabNames = GetTabNames(bay);
		if (index < 0 || index >= tabNames.Count || string.IsNullOrWhiteSpace(tabNames[index]))
		{
			return null;
		}
		return tabNames[index];
	}

	public static void SetTabName(GameItem bay, int index, string name)
	{
		MachineryTabs tabs = GetTabs(bay);
		if (tabs == null || index < 0 || index >= tabs.count)
		{
			return;
		}
		name = (name ?? "").Replace('\n', ' ').Trim();
		if (name.Length > 12)
		{
			name = name.Substring(0, 12);
		}
		List<string> tabNames = GetTabNames(bay);
		while (tabNames.Count < tabs.count)
		{
			tabNames.Add("");
		}
		tabNames[index] = name;
		if (tabNames.TrueForAll(string.IsNullOrEmpty))
		{
			bay.DisableTag("STORAGE_TAB_NAMES_STRING");
		}
		else
		{
			string joined = string.Join('\n'.ToString(), tabNames);
			bay.ModifyTag("STORAGE_TAB_NAMES_STRING", delegate(TagState state)
			{
				state.Enable().SetString(joined);
			});
		}
		ApplyTabNames(bay, tabs);
		(bay.contentWindow?.childElement as MachineryPanelRow)?.Validate();
		bay.contentWindow?.Validate();
	}

	private static void ApplyTabNames(GameItem bay, MachineryTabs tabs)
	{
		for (int i = 0; i < tabs.count; i++)
		{
			tabs.SetCaption(i, GetTabName(bay, i) ?? DefaultTabName(i));
		}
	}

	private static void OpenRename(GameItem bay, int index)
	{
		if (!(LabelerBlueUIManager.Instance == null))
		{
			LabelerBlueUIManager.Instance.OpenUI(GetTabName(bay, index) ?? "", delegate(string text)
			{
				SetTabName(bay, index, text);
			}, delegate
			{
				SetTabName(bay, index, null);
			}, 12);
		}
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (GetKitTabs(item) != 0)
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_storage_bay_tabs", 1 + GetExtraTabs(item), 5), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
	}
}
