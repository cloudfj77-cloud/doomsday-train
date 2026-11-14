using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ToolboxHelper
{
	public const string TOOLBOX_TAG = "TOOLBOX_TAG";

	public const string TOOLBOX_ITEM_TAG = "TOOLBOX_ITEM_TAG";

	public const string TOOLBOX_UNLOCKED_SLOTS_INT = "TOOLBOX_UNLOCKED_SLOTS_INT";

	public const string TOOLBOX_SLOT_TAG = "TOOLBOX_SLOT_TAG";

	public const string TOOLBOX_SLOT_ID_PREFIX = "toolbox_slot_";

	public const int TOOLBOX_START_SLOTS = 3;

	public const int TOOLBOX_MAX_SLOTS = 9;

	private const string SLOT_BG_ATLAS = "Items/items_tool";

	private const string SLOT_BG_SPRITE = "screwdriver_s";

	public static (PixelWindow, List<GameSlotInventory>, List<MachineryLabel>) CreateToolboxWindow()
	{
		List<GameSlotInventory> list = new List<GameSlotInventory>();
		List<MachineryLabel> list2 = new List<MachineryLabel>();
		List<MachineryPanel> list3 = new List<MachineryPanel>();
		for (int i = 0; i < 9; i++)
		{
			GameSlotInventory gameSlotInventory = new GameSlotInventory(2, 2);
			gameSlotInventory.SetBackgroundFadeSprite("Items/items_tool", "screwdriver_s");
			MachineryPanel machineryPanel = MachineHelper.Panel(SlotLabel(i), gameSlotInventory, "slot" + i);
			list.Add(gameSlotInventory);
			list2.Add(machineryPanel.label);
			list3.Add(machineryPanel);
		}
		PixelWindow pixelWindow = MachineHelper.CreateMachineryWindow(canMove: true, list3.ToArray());
		((MachineryPanelRow)pixelWindow.childElement).SetColumns(3);
		pixelWindow.SetTitle(LocHelper.GetLocalizedItem("item_toolbox_name"));
		pixelWindow.Validate();
		return (pixelWindow, list, list2);
	}

	public static void InitToolbox(GameItem box, PixelWindow window, List<GameSlotInventory> slots, List<MachineryLabel> labels)
	{
		box.EnableTag("TOOLBOX_TAG");
		box.EnableTag("CONTAINER_TAG");
		box.EnableTag("NESTABLE_DEVICE_TAG");
		box.EnableTag("IMPORTANT_TAG");
		ContainerHelper.AddContainerTooltipTag(box);
		box.ModifyTag("TOOLBOX_UNLOCKED_SLOTS_INT", delegate(TagState s)
		{
			s.Enable().SetInt(3);
		});
		for (int i = 0; i < slots.Count; i++)
		{
			int slotIndex = i;
			GameSlotInventory gameSlotInventory = slots[i];
			gameSlotInventory.EnableTag("TOOLBOX_SLOT_TAG");
			gameSlotInventory.identifier = "toolbox_slot_" + slotIndex;
			gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 5 + (8 - slotIndex);
			gameSlotInventory.mayInventoryAddItemFunc = (Func<GameItem, GameInventory, bool>)Delegate.Combine(gameSlotInventory.mayInventoryAddItemFunc, (Func<GameItem, GameInventory, bool>)delegate(GameItem item, GameInventory _)
			{
				if (slotIndex >= GetUnlockedSlots(box))
				{
					return false;
				}
				return GeneralHelper.IsItemOwned(item) && IsToolboxCompatible(item);
			});
		}
		GameItem gameItem = box;
		gameItem.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem.onLoaded, (Action<GameItem>)delegate
		{
			RefreshSlots(box, slots, labels, window);
		});
		PixelWindow pixelWindow = window;
		pixelWindow.openCallback = (Action)Delegate.Combine(pixelWindow.openCallback, (Action)delegate
		{
			RefreshSlots(box, slots, labels, window);
		});
		RefreshSlots(box, slots, labels, window);
		GameItem gameItem2 = box;
		gameItem2.onItemTargetThisFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem2.onItemTargetThisFunc, (Action<GameItem, GameItem>)delegate(GameItem source, GameItem target)
		{
			if (MayUpgrade(source, box))
			{
				int unlocked = GetUnlockedSlots(box) + 1;
				box.ModifyTag("TOOLBOX_UNLOCKED_SLOTS_INT", delegate(TagState s)
				{
					s.Enable().SetInt(unlocked);
				});
				box.unitValue += source.unitValue;
				StartingPerkContent.ConsumeUpgradeMaterial(source);
				RefreshSlots(box, slots, labels, window);
				AudioManager.Instance.Play2("scrap_metal_drop");
			}
		});
	}

	public static bool IsToolboxCompatible(GameItem item)
	{
		if (!item.IsGameItemType("TOOL") && !item.IsGameItemType("MELEE_WEAPON") && !item.IsTag("CAN_USE_ON_NON_OWNED_TAG") && !item.IsTag("TOOLBOX_ITEM_TAG") && !item.IsTag("LIQUID_CONTAINER_DROPPER_TAG") && !item.identifier.Equals("labeler") && !item.identifier.Equals("labeler_blue") && !item.identifier.Equals("surgery_tool"))
		{
			return item.identifier.Equals("uv_filter");
		}
		return true;
	}

	public static int GetUnlockedSlots(GameItem box)
	{
		return Math.Clamp(box.GetTagReadonly("TOOLBOX_UNLOCKED_SLOTS_INT").GetInt(), 3, 9);
	}

	public static bool CanUpgradeToolbox(GameItem box)
	{
		if (box.IsTag("TOOLBOX_TAG"))
		{
			return GetUnlockedSlots(box) < 9;
		}
		return false;
	}

	private static bool MayUpgrade(GameItem source, GameItem box)
	{
		if (!source.identifier.Equals("scrap_metal"))
		{
			return false;
		}
		if (!GeneralHelper.IsItemOwned(source) || !GeneralHelper.IsItemOwned(box))
		{
			return false;
		}
		return CanUpgradeToolbox(box);
	}

	private static void UpdateToolboxSprite(GameItem box)
	{
		bool flag = GetUnlockedSlots(box) >= 9;
		box.SetSpriteAndShape("Items/items_tool2", flag ? "toolbox_new" : "toolbox_old");
	}

	private static void RefreshSlots(GameItem box, List<GameSlotInventory> slots, List<MachineryLabel> labels, PixelWindow window)
	{
		int unlockedSlots = GetUnlockedSlots(box);
		UpdateToolboxSprite(box);
		for (int i = 0; i < slots.Count; i++)
		{
			bool flag = i < unlockedSlots;
			labels[i].SetText(flag ? SlotLabel(i) : LocHelper.GetLocalizedMechanic("mech_toolbox_label_locked"));
			labels[i].SetTextColor(flag ? MachineryStyle.LABEL_TEXT : MachineryStyle.BUTTON_TEXT_DISABLED);
			if (flag)
			{
				slots[i].SetBackgroundFadeSprite("Items/items_tool", "screwdriver_s");
			}
			else
			{
				slots[i].SetBackgroundFadeSprite("Items/items_tool", "screwdriver_s", 0.3f, 0.25f);
			}
		}
		window.Validate();
	}

	public static void CreateToolboxTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("TOOLBOX_TAG"))
		{
			int unlockedSlots = GetUnlockedSlots(item);
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_toolbox_tooltip_slots", unlockedSlots, 9), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			if (unlockedSlots < 9)
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_toolbox_tooltip_upgrade"), ignoreEmpty: true, RenderHandler.ColorPalette.LightGray);
			}
			builder.AddEmptyLine(14);
		}
	}

	private static string SlotLabel(int slotIndex)
	{
		string localizedMechanic = LocHelper.GetLocalizedMechanic("mech_toolbox_label_slot", slotIndex + 1);
		string id = KeyBindHelper.ToolboxSlot(slotIndex);
		KeyCode keyCode = KeyBindHelper.Get(id);
		if (keyCode == KeyBindHelper.GetDefault(id))
		{
			return localizedMechanic;
		}
		return localizedMechanic + " (" + KeyBindHelper.KeyName(keyCode) + ")";
	}

	public static void HandleHotkeys()
	{
		if (DialogUIManager.Instance != null && DialogUIManager.Instance.IsChoice())
		{
			return;
		}
		for (int i = 0; i < 9; i++)
		{
			if (KeyBindHelper.GetKeyDown(KeyBindHelper.ToolboxSlot(i)))
			{
				TryToggleEquipSlot(i);
				break;
			}
		}
	}

	public static void TryToggleEquipSlot(int slotIndex)
	{
		GameItem gameItem = FindToolboxInReach();
		if (gameItem == null)
		{
			return;
		}
		List<GameSlotInventory> toolboxSlots = GetToolboxSlots(gameItem);
		if (slotIndex < 0 || slotIndex >= toolboxSlots.Count)
		{
			return;
		}
		GameItem childItem = toolboxSlots[slotIndex].childItem;
		if (childItem != null)
		{
			if (ItemSelectHandler.current.GetEquippedItem() == childItem)
			{
				ItemSelectHandler.current.EndSelect();
			}
			else
			{
				ItemSelectHandler.current.TryEquipItem(childItem);
			}
		}
	}

	public static GameItem FindToolboxInReach()
	{
		if (MapUIManager.Instance != null && MapUIManager.Instance.isOutside)
		{
			return FindToolboxIn(EmporiumEntry.Instance.afterhourWindow);
		}
		foreach (PixelWindow visibleWindow in WindowsHandler.current.visibleWindows)
		{
			GameItem gameItem = FindToolboxIn(visibleWindow);
			if (gameItem != null)
			{
				return gameItem;
			}
		}
		return null;
	}

	private static GameItem FindToolboxIn(GraphNodeStorage root)
	{
		List<GameItem> list = GraphUtils.FindAllChildrenType(root, (GameItem i) => i.IsTag("TOOLBOX_TAG") && GeneralHelper.IsItemOwned(i));
		if (list.Count <= 0)
		{
			return null;
		}
		return list[0];
	}

	public static List<GameSlotInventory> GetToolboxSlots(GameItem box)
	{
		return (from s in GraphUtils.FindAllChildrenType(box, (GameSlotInventory s) => s.IsTag("TOOLBOX_SLOT_TAG"))
			orderby GetSlotIndex(s)
			select s).ToList();
	}

	private static int GetSlotIndex(GameSlotInventory slot)
	{
		string identifier = slot.identifier;
		if (identifier != null && identifier.StartsWith("toolbox_slot_") && int.TryParse(identifier.Substring("toolbox_slot_".Length), out var result))
		{
			return result;
		}
		return int.MaxValue;
	}
}
