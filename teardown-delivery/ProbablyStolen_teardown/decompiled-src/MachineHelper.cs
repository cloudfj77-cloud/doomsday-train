using System;
using System.Collections.Generic;
using UnityEngine;

public class MachineHelper
{
	public const string POWERED = "POWERED";

	public const string MACHINE_PERFORMANCE_RATIO_INT = "MACHINE_PERFORMANCE_RATIO_INT";

	public const string MACHINE_EFFICIENCY_RATIO_INT = "MACHINE_EFFICIENCY_RATIO_INT";

	public const string MACHIME_QUALITY_RATIO_INT = "MACHINE_QUALITY_RATIO_INT";

	public const string MACHINE_PERFORMANCE_REDUCES_POWER_TAG = "MACHINE_PERFORMANCE_REDUCES_POWER_TAG";

	public const string MACHINE_RECHARGABLE_TAG = "MACHINE_RECHARGABLE_TAG";

	public const string MODULE_UPGRADE_STAGE_INT = "MODULE_UPGRADE_STAGE_INT";

	public const string LOCKED_WORKING_TAG = "LOCKED_WORKING_TAG";

	public const string STANDARD_MACHINE_TAG = "STANDARD_MACHINE_TAG";

	public const string DEFAULT_MODULE_BAY_CONFIG = "10000010001000001110000010001000001";

	public const int DEFAULT_MODULE_BAY_WIDTH = 7;

	public const string MODULE_BAY_1_CONFIG = "10000010000000000000000000001000001";

	public const int MODULE_BAY_1_WIDTH = 7;

	public const string MODULE_BAY_2_CONFIG = "00000000000000000000000000000000000";

	public const int MODULE_BAY_2_WIDTH = 7;

	public const string MANUAL_TAG = "MACHINE_MANUAL_TAG";

	public const float MANUAL_TOOLTIP_WIDTH_MULT = 2f;

	public static void SetupBatterySlot(GameSlotInventory itemInventoryBattery, GameItem machine)
	{
		machine.EnableTag("MACHINE_RECHARGABLE_TAG");
		itemInventoryBattery.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(item) && item.IsTag("power_source_item");
		itemInventoryBattery.SetBackgroundFadeSprite("Items/items_tool", "battery100");
		itemInventoryBattery.onSlotAddItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(itemInventoryBattery.onSlotAddItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
		{
			AudioManager.Instance.Play3("battery_insert");
		});
		itemInventoryBattery.onSlotRemoveItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(itemInventoryBattery.onSlotRemoveItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
		{
			AudioManager.Instance.Play3("battery_removed");
		});
	}

	public static PixelWindow CreateMachineryWindow(bool canMove, params MachineryPanel[] panels)
	{
		PixelWindow pixelWindow = new PixelWindow(canMove, null, WindowStyle.Machinery);
		pixelWindow.Attach(new MachineryPanelRow(panels));
		return pixelWindow;
	}

	public static PixelWindow CreateMachineryWindow(bool canMove, int[] displayOrder, params MachineryPanel[] panels)
	{
		PixelWindow pixelWindow = new PixelWindow(canMove, null, WindowStyle.Machinery);
		pixelWindow.Attach(new MachineryPanelRow(panels).SetDisplayOrder(displayOrder));
		return pixelWindow;
	}

	public static MachineryPanel Panel(string label, PixelElement content, string role = null, bool styleInventories = true)
	{
		return new MachineryPanel(label, WrapInIntake(content, styleInventories), role);
	}

	public static MachineryPanel IconPanel(Sprite icon, PixelElement content, string role = null)
	{
		return new MachineryPanel(icon, WrapInIntake(content), role);
	}

	public static MachineryPanel PowerPanel(GameSlotInventory batterySlot)
	{
		return IconPanel(MachineryStyle.IconPower, batterySlot, "power");
	}

	public static MachineryPanel NotePanel(GameSlotInventory noteSlot)
	{
		return IconPanel(MachineryStyle.IconNote, noteSlot, "note");
	}

	public static PixelElement WrapInIntake(PixelElement content, bool styleInventories = true)
	{
		if (content is GameInventory || content is GridPixelElement)
		{
			return new MachineryIntake(content, vents: true, styleInventories);
		}
		return content;
	}

	public static bool CanPower(GameItem machine, GameSlotInventory batterySlot)
	{
		int machinePowerUsage = MachineryHelper.GetMachinePowerUsage(machine);
		if (batterySlot.childItem == null || !PowerHelper.CanDrawPowerSource(batterySlot.childItem, machinePowerUsage))
		{
			return false;
		}
		return true;
	}

	public static bool CanPower(GameItem machine)
	{
		GameSlotInventory batterySlot = GetBatterySlot(machine);
		if (batterySlot == null)
		{
			return false;
		}
		return CanPower(machine, batterySlot);
	}

	public static GraphNodeStorage GetBodyStorage(GameItem machine, int index)
	{
		PixelElement pixelElement = machine?.contentWindow?.childElement;
		if (pixelElement is MachineryPanelRow machineryPanelRow)
		{
			return machineryPanelRow.GetStorage(index);
		}
		if (pixelElement is GridPixelElement gridPixelElement)
		{
			return gridPixelElement.GetElement(index, 1) as GraphNodeStorage;
		}
		return null;
	}

	public static GameSlotInventory GetBatterySlot(GameItem machine)
	{
		return GetBodyStorage(machine, 0) as GameSlotInventory;
	}

	public static bool HasBattery(GameItem machine)
	{
		GameSlotInventory batterySlot = GetBatterySlot(machine);
		if (batterySlot == null || batterySlot.childItem == null)
		{
			return false;
		}
		return true;
	}

	public static GameInventory GetSlot(GameItem machine, int index)
	{
		return (GameInventory)GetBodyStorage(machine, index);
	}

	public static List<GameItem> GetModules(GameItem machine)
	{
		GameGridInventory obj = (GameGridInventory)GetBodyStorage(machine, 1);
		List<GameItem> modules = new List<GameItem>();
		obj.childItems.ForEach(delegate(GameItem item)
		{
			modules.Add(item);
		});
		return modules;
	}

	public static GameGridInventory GetModuleInv(GameItem machine)
	{
		return (GameGridInventory)GetBodyStorage(machine, 1);
	}

	public static bool IsModuleBay(GameInventory inventory)
	{
		GameItem gameItem = inventory?.GetParentItem();
		if (gameItem != null)
		{
			return GetBodyStorage(gameItem, 1) == inventory;
		}
		return false;
	}

	public static void LockModuleInv(GameItem machine)
	{
		machine.EnableTag("LOCKED_WORKING_TAG");
		GetModuleInv(machine)?.LockInv();
	}

	public static void UnlockModuleInv(GameItem machine)
	{
		machine.DisableTag("LOCKED_WORKING_TAG");
		GetModuleInv(machine)?.UnlockInv();
	}

	public static void OnLoad(GameItem machine)
	{
		if (machine.IsTag("LOCKED_WORKING_TAG"))
		{
			LockModuleInv(machine);
		}
	}

	public static GameItem GetLockedModuleBayMachine(GameItem dragged, GameItem hoveredItem, GameInventory hoveredInventory)
	{
		if (dragged == null)
		{
			return null;
		}
		GameItem gameItem = null;
		if (hoveredItem != null && hoveredItem.IsTag("LOCKED_WORKING_TAG"))
		{
			gameItem = hoveredItem;
		}
		else if (hoveredItem != null && hoveredItem.parentInventory != null && hoveredItem.parentInventory.IsInsertLocked())
		{
			gameItem = hoveredItem.parentInventory.GetParentItem();
		}
		else if (hoveredItem == null && hoveredInventory != null && hoveredInventory.IsInsertLocked())
		{
			gameItem = hoveredInventory.GetParentItem();
		}
		if (gameItem == null || !gameItem.IsTag("LOCKED_WORKING_TAG"))
		{
			return null;
		}
		GameGridInventory moduleGrid = GetModuleGrid(gameItem);
		if (moduleGrid == null || moduleGrid.mayInventoryAddItemFunc == null)
		{
			return null;
		}
		if (!moduleGrid.mayInventoryAddItemFunc.InvokeAllReduce(dragged, moduleGrid, InvokeFuncExtensions.BoolReduceOR))
		{
			return null;
		}
		return gameItem;
	}

	public static RichText CreateLockedModuleBayDragTooltip(RichTextBuilder builder, GameItem module, GameItem machine)
	{
		builder.Clear();
		builder.Add(LocHelper.GetLocalizedMechanic("mech_drag_module_bay_locked", module.GetDisplayName(preferCustomName: true), "<u>" + machine.GetDisplayName(preferCustomName: true) + "</u>"), RenderHandler.ColorPalette.Red);
		return builder.Build();
	}

	private static GameGridInventory GetModuleGrid(GameItem machine)
	{
		return GetBodyStorage(machine, 1) as GameGridInventory;
	}

	public static bool CanExpand(GameItem item)
	{
		if (item.GetTagReadonly("MODULE_UPGRADE_STAGE_INT").GetInt() < 2)
		{
			return true;
		}
		return false;
	}

	public static void ExpandModule(GameGridInventory moduleGrid, GameItem item)
	{
		int moduleUpgradeStage = item.GetTagReadonly("MODULE_UPGRADE_STAGE_INT").GetInt();
		if (moduleUpgradeStage == 0)
		{
			Debug.Log("Expand to Module Bay Upgrade 1");
			moduleGrid.SetShape("10000010000000000000000000001000001", 7);
		}
		else if (moduleUpgradeStage == 1)
		{
			Debug.Log("Expand to Module Bay Upgrade 2");
			moduleGrid.SetShape("00000000000000000000000000000000000", 7);
		}
		moduleUpgradeStage++;
		item.ModifyTag("MODULE_UPGRADE_STAGE_INT", delegate(TagState state)
		{
			state.SetInt(moduleUpgradeStage);
		});
		EmporiumAudioPlayer.Instance.ratchet.Play();
		item.contentWindow.Validate();
	}

	public static void LoadUpgrade(GameItem item)
	{
		if (item.IsTag("STANDARD_MACHINE_TAG"))
		{
			int @int = item.GetTagReadonly("MODULE_UPGRADE_STAGE_INT").GetInt();
			Debug.Log("Loaded Module Bay Upgrade " + @int);
			switch (@int)
			{
			case 0:
				return;
			case 1:
				Debug.Log("Load upgrade stage 1");
				GetModuleInv(item).SetShape("10000010000000000000000000001000001", 7);
				break;
			case 2:
				GetModuleInv(item).SetShape("00000000000000000000000000000000000", 7);
				break;
			}
			item.contentWindow.Validate();
		}
	}

	public static bool IsMachineRechargable(GameItem gameItem)
	{
		_ = gameItem.contentWindow;
		return false;
	}

	public static void RechargeMachineBattery(GameItem gameItem, int amount = 1, bool ignoreBlackout = false)
	{
		if (gameItem.contentWindow != null)
		{
			GameSlotInventory batterySlot = GetBatterySlot(gameItem);
			if (batterySlot != null && batterySlot.childItem != null)
			{
				PowerHelper.ChargeBattery(batterySlot.childItem, amount, ignoreBlackout);
			}
		}
	}

	public static void SetupModuleBay(GameInventory moduleGrid, GameItem machine, Action onChanged = null, string[] allowedModuleTypes = null)
	{
		if (allowedModuleTypes == null)
		{
			allowedModuleTypes = new string[1] { "MODULE_TYPE_UNIVERSAL" };
		}
		ContainerHelper.AllowOnlyTaggedItemsOr(moduleGrid, allowedModuleTypes, ownedOnly: true);
		GameInventory gameInventory = moduleGrid;
		gameInventory.onSlotAddItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameInventory.onSlotAddItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			ModuleEffectHelper.OnAddedModuleToGrid(addedItem, machine, moduleGrid.childItems);
			if (addedItem.IsTag("MODULE_TAG"))
			{
				ModuleHelper.ComputeModuleEffect(moduleGrid, addedItem, machine);
				ModuleHelper.ApplyBasicModuleEffect(moduleGrid, addedItem, machine);
			}
			MachineryHelper.UpdatePowerUsageProcessTypeMachine(machine);
			onChanged?.Invoke();
		});
		GameInventory gameInventory2 = moduleGrid;
		gameInventory2.onSlotRemoveItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameInventory2.onSlotRemoveItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem removedItem, GameInventory parentInventory, SlotMarker slot)
		{
			ModuleEffectHelper.OnRemovedModuleFromGrid(removedItem, machine, moduleGrid.childItems);
			if (removedItem.IsTag("MODULE_TAG"))
			{
				ModuleHelper.ComputeModuleEffect(moduleGrid, removedItem, machine);
				ModuleHelper.ApplyBasicModuleEffect(moduleGrid, removedItem, machine);
			}
			MachineryHelper.UpdatePowerUsageProcessTypeMachine(machine);
			onChanged?.Invoke();
		});
	}

	public static void SetupManualSlot(GameSlotInventory manualSlot)
	{
		manualSlot.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => item.identifier.Equals("postit");
		manualSlot.SetBackgroundFadeSprite("Items/items_tool", "postit");
	}

	public static GameItem CreateManual(string nameLocKey, string flavorLocKey)
	{
		GameItem gameItem = DirectoryMaster.Item("postit");
		gameItem.name = LocHelper.GetLocalizedItem(nameLocKey);
		gameItem.flavorText = LocHelper.GetLocalizedItem(flavorLocKey);
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("MACHINE_MANUAL_TAG");
		gameItem.unitValue = 0L;
		return gameItem;
	}

	public static bool TryDrawCyclePower(GameItem machine, GameSlotInventory batterySlot)
	{
		if (batterySlot == null || batterySlot.childItem == null)
		{
			return false;
		}
		int machinePowerUsage = MachineryHelper.GetMachinePowerUsage(machine);
		return PowerHelper.DrawPowerSource(batterySlot.childItem, machinePowerUsage);
	}

	public static void OnMachineActioned(GameItem machine, GameInventory moduleGrid)
	{
		Debug.Log("Machine actioned, activating module effects.");
		foreach (GameItem childItem in moduleGrid.childItems)
		{
			ModuleEffectHelper.ActivateOnUseAction(childItem, moduleGrid, machine);
		}
		ModuleEffectHelper.ComputeAllModuleTempStat(moduleGrid.childItems);
		ModuleHelper.ComputeModuleEffect(moduleGrid, null, machine);
		ModuleHelper.ApplyBasicModuleEffect(moduleGrid, null, machine);
	}
}
