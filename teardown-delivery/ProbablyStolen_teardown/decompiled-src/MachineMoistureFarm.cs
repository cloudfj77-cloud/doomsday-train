using System;
using System.Collections.Generic;
using UnityEngine;

public class MachineMoistureFarm
{
	public const float SHARED_AIR_PENALTY = 0.25f;

	private static int runningCountDay = -1;

	private static int runningCountTonight = 1;

	private const float TOOLTIP_COUNT_CACHE_SECONDS = 0.5f;

	private static float tooltipCountTime = -1f;

	private static int tooltipCount = 1;

	private const float NIGHT_COUNT_WINDOW_SECONDS = 10f;

	private static float runningCountTime = -1f;

	public const int BASE_OUTPUT_ML = 850;

	public static GameItem MoistureFarm()
	{
		var (contentWindow, powerInv, itemInventoryModule, bottleInv, gameSlotInventory) = CreateMachineInventoryWindow();
		bottleInv.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(item) && item.IsTag("LIQUID_CONTAINER_TAG");
		bottleInv.SetBackgroundFadeSprite("Items/water_container4", "xl");
		gameSlotInventory.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => item.identifier.Equals("postit");
		gameSlotInventory.SetBackgroundFadeSprite("Items/items_tool", "postit");
		GameItem machine = ItemDirectory.CreateEmptyItem().SetContentWindow(contentWindow).SetSpriteAndShape("Items/items_tool", "moisture_farm");
		machine.name = LocHelper.GetLocalizedItem("item_moisture_farm_name");
		machine.flavorText = LocHelper.GetLocalizedItem("item_moisture_farm_flavor");
		machine.shortDescription = LocHelper.GetLocalizedItem("item_moisture_farm_desc") + "\n" + LocHelper.GetLocalizedItem("item_moisture_farm_desc_shared_air");
		machine.unitValue = 350L;
		MachineHelper.SetupBatterySlot(powerInv, machine);
		machine.EnableTag("CONTAINER_TAG");
		machine.EnableTag("manufacture");
		machine.EnableTag("STANDARD_MACHINE_TAG");
		machine.SetGameItemType("MACHINE");
		MachineryHelper.InitMachinery(machine, 6);
		string[] tags = new string[1] { "MODULE_TYPE_UNIVERSAL" };
		ContainerHelper.AllowOnlyTaggedItemsOr(itemInventoryModule, tags, ownedOnly: true);
		itemInventoryModule.onSlotAddItemEndFunc = delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			ModuleEffectHelper.OnAddedModuleToGrid(addedItem, machine, itemInventoryModule.childItems);
			if (addedItem.IsTag("MODULE_TAG"))
			{
				ModuleHelper.ComputeModuleEffect(itemInventoryModule, addedItem, machine);
				ModuleHelper.ApplyBasicModuleEffect(itemInventoryModule, addedItem, machine);
			}
			MachineryHelper.UpdatePowerUsageProcessTypeMachine(machine);
		};
		itemInventoryModule.onSlotRemoveItemEndFunc = delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			ModuleEffectHelper.OnRemovedModuleFromGrid(addedItem, machine, itemInventoryModule.childItems);
			if (addedItem.IsTag("MODULE_TAG"))
			{
				ModuleHelper.ComputeModuleEffect(itemInventoryModule, addedItem, machine);
				ModuleHelper.ApplyBasicModuleEffect(itemInventoryModule, addedItem, machine);
			}
			MachineryHelper.UpdatePowerUsageProcessTypeMachine(machine);
		};
		machine.onCycleEndSlotItemFunc = delegate(GameItem item, GameInventory parentInventory, SlotMarker slot)
		{
			if (bottleInv.childItem != null && WaterHelper.GetFreeCapacity(bottleInv.childItem) != 0)
			{
				int machinePowerUsage = MachineryHelper.GetMachinePowerUsage(item);
				if (powerInv.childItem != null && PowerHelper.CanDrawPowerSource(powerInv.childItem, machinePowerUsage))
				{
					int runningFarmCountTonight = GetRunningFarmCountTonight();
					PowerHelper.DrawPowerSource(powerInv.childItem, machinePowerUsage);
					int volume = ApplySharedAir(GetOutputVolume(machine), runningFarmCountTonight);
					Fill(GetOutputQuality(item), bottleInv.childItem, volume);
					MachineHelper.OnMachineActioned(machine, itemInventoryModule);
				}
			}
		};
		powerInv.onSlotAddItemEndFunc = delegate
		{
			machine.SetSpriteAndShape("Items/items_tool", "moisture_farm");
		};
		powerInv.onSlotRemoveItemEndFunc = delegate
		{
			machine.SetSpriteAndShape("Items/items_tool", "moisture_farm");
		};
		bottleInv.onSlotAddItemEndFunc = delegate
		{
			if (powerInv.childItem != null)
			{
				machine.SetSpriteAndShape("Items/items_tool", "moisture_farm_w");
			}
			else
			{
				machine.SetSpriteAndShape("Items/items_tool", "moisture_farm");
			}
		};
		bottleInv.onSlotRemoveItemEndFunc = delegate
		{
			if (powerInv.childItem != null)
			{
				machine.SetSpriteAndShape("Items/items_tool", "moisture_farm_w");
			}
			else
			{
				machine.SetSpriteAndShape("Items/items_tool", "moisture_farm");
			}
		};
		return machine;
	}

	public static int GetSharedAirPercent(int runningFarms)
	{
		int num = Math.Max(1, runningFarms);
		return Mathf.RoundToInt(100f / (1f + 0.25f * (float)(num - 1)));
	}

	public static int ApplySharedAir(int volume, int runningFarms)
	{
		int num = Math.Max(1, runningFarms);
		return Mathf.RoundToInt((float)volume / (1f + 0.25f * (float)(num - 1)));
	}

	public static bool CanRunTonight(GameItem farm)
	{
		if (farm == null || !farm.identifier.Equals("moisture_farm"))
		{
			return false;
		}
		if (farm.IsTag("BROKEN_MACHINE_TAG"))
		{
			return false;
		}
		if (!GeneralHelper.IsItemOwned(farm) || farm.contentWindow == null)
		{
			return false;
		}
		GameItem gameItem = MachineHelper.GetBatterySlot(farm)?.childItem;
		if (gameItem == null || !PowerHelper.CanDrawPowerSource(gameItem, MachineryHelper.GetMachinePowerUsage(farm)))
		{
			return false;
		}
		GameItem gameItem2 = (MachineHelper.GetSlot(farm, 2) as GameSlotInventory)?.childItem;
		if (gameItem2 != null)
		{
			return WaterHelper.GetFreeCapacity(gameItem2) > 0;
		}
		return false;
	}

	public static int CountRunningFarms()
	{
		HashSet<GameItem> hashSet = new HashSet<GameItem>();
		int num = 0;
		foreach (GameItem allOwnedItem in EmporiumEntry.Instance.GetAllOwnedItems())
		{
			if (hashSet.Add(allOwnedItem) && CanRunTonight(allOwnedItem))
			{
				num++;
			}
		}
		return num;
	}

	private static int GetRunningFarmCountTonight()
	{
		int dayCounter = StoreStation.Instance.dayCounter;
		if (runningCountDay != dayCounter || runningCountTime < 0f || Time.unscaledTime - runningCountTime > 10f)
		{
			runningCountDay = dayCounter;
			runningCountTime = Time.unscaledTime;
			runningCountTonight = Math.Max(1, CountRunningFarms());
		}
		return runningCountTonight;
	}

	public static int GetSharingFarmCount(GameItem farm)
	{
		if (Time.unscaledTime - tooltipCountTime > 0.5f || tooltipCountTime < 0f)
		{
			tooltipCount = CountRunningFarms();
			tooltipCountTime = Time.unscaledTime;
		}
		if (!CanRunTonight(farm))
		{
			return tooltipCount + 1;
		}
		return Math.Max(1, tooltipCount);
	}

	public static int GetOutputVolume(GameItem gameItem)
	{
		int currentPerformanceBonus = MachineryHelper.GetCurrentPerformanceBonus(gameItem);
		return Math.Max(850000 * (100 + currentPerformanceBonus) / 100, 0);
	}

	public static string GetOutputQuality(GameItem gameItem)
	{
		int num = MachineryHelper.GetCurrentQualityBonus(gameItem) + 50;
		if (num < 0)
		{
			return "Gutterflow";
		}
		if (num < 50)
		{
			return "Rustwater";
		}
		if (num < 100)
		{
			return "Ghostwater";
		}
		if (num < 150)
		{
			return "Basewater";
		}
		if (num < 200)
		{
			return "High Quality Water";
		}
		return "Pure Water";
	}

	public static string GetOutputQualityDisplay(GameItem gameItem)
	{
		return GetOutputQuality(gameItem) switch
		{
			"Gutterflow" => LocHelper.GetLocalizedMechanic("mech_condition_gutterflow"), 
			"Rustwater" => LocHelper.GetLocalizedMechanic("mech_condition_rustwater"), 
			"Ghostwater" => LocHelper.GetLocalizedMechanic("mech_condition_ghostwater"), 
			"Basewater" => LocHelper.GetLocalizedMechanic("mech_condition_basewater"), 
			"High Quality Water" => LocHelper.GetLocalizedMechanic("mech_condition_high_quality_water"), 
			_ => LocHelper.GetLocalizedMechanic("mech_condition_pure_water"), 
		};
	}

	public static void Fill(string quality, GameItem container, int volume)
	{
		GameItem gameItem = ItemSpawner.Spawn("water_jug");
		if (volume > gameItem.GetTagReadonly("LIQUID_CONTAINER_CAPACITY").GetInt())
		{
			gameItem.ModifyTag("LIQUID_CONTAINER_CAPACITY", delegate(TagState state)
			{
				state.Enable().SetInt(volume);
			});
		}
		if (quality.Equals("Gutterflow"))
		{
			WaterHelper.AddGutterflow(gameItem, volume);
		}
		else if (quality.Equals("Rustwater"))
		{
			WaterHelper.AddRustWater(gameItem, volume);
		}
		else if (quality.Equals("Ghostwater"))
		{
			WaterHelper.AddGhostWater(gameItem, volume);
		}
		else if (quality.Equals("Basewater"))
		{
			WaterHelper.AddBaseWater(gameItem, volume);
		}
		else if (quality.Equals("High Quality Water"))
		{
			WaterHelper.AddHighQualityWater(gameItem, volume);
		}
		else
		{
			if (!quality.Equals("Pure Water"))
			{
				throw new ArgumentException("Invalid water quality specified.");
			}
			WaterHelper.AddPureWater(gameItem, volume);
		}
		WaterHelper.TransferLiquid(gameItem, container);
		gameItem.Destroy();
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.identifier.Equals("moisture_farm") && !item.IsTag("BROKEN_MACHINE_TAG"))
		{
			int sharingFarmCount = GetSharingFarmCount(item);
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_moisture_farm_tooltip", ApplySharedAir(GetOutputVolume(item), sharingFarmCount) / 1000, GetOutputQualityDisplay(item)), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			if (sharingFarmCount > 1)
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_moisture_farm_tooltip_running", sharingFarmCount, GetSharedAirPercent(sharingFarmCount)), ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
			}
		}
	}

	public static (PixelWindow, GameSlotInventory, GameInventory, GameSlotInventory, GameSlotInventory) CreateMachineInventoryWindow(bool isDraggable = true, bool isCentered = true, int minWidth = 0)
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameGridInventory gameGridInventory = new GameGridInventory("10000010001000001110000010001000001", 7);
		GameSlotInventory gameSlotInventory2 = new GameSlotInventory();
		GameSlotInventory gameSlotInventory3 = new GameSlotInventory();
		return (MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.PowerPanel(gameSlotInventory), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_moisture_farm_label_module"), gameGridInventory, "module"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_moisture_farm_label_container"), gameSlotInventory2, "container"), MachineHelper.NotePanel(gameSlotInventory3)), gameSlotInventory, gameGridInventory, gameSlotInventory2, gameSlotInventory3);
	}

	public static GameItem CreateNote()
	{
		GameItem gameItem = DirectoryMaster.Item("postit");
		gameItem.name = LocHelper.GetLocalizedItem("item_moisture_farm_instruction_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_moisture_farm_instruction_short");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_moisture_farm_instruction_flavor");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.unitValue = 0L;
		return gameItem;
	}
}
