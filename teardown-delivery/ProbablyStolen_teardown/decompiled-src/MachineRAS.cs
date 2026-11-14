using System;
using System.Collections.Generic;

public class MachineRAS
{
	private const string SPRITE_DRY = "ras_empty";

	private const string SPRITE_WATER = "ras";

	private const string SPRITE_FULL = "ras_full";

	private const string SPRITE_TANK = "ras_tank";

	public const string RAS_TAG = "RAS_TAG";

	public const string RAS_TANK_GRID_TAG = "RAS_TANK_GRID_TAG";

	public const string RAS_FILTER_SLOT_TAG = "RAS_FILTER_SLOT_TAG";

	public const string RAS_WATER_TANK_TAG = "RAS_WATER_TANK_TAG";

	public const int POWER_USE = 5;

	public const int FILTER_SLOT_COUNT = 4;

	public const int TANK_WIDTH = 8;

	public const int TANK_HEIGHT = 4;

	public const string RAS_WIDTH_UPGRADE_COUNT_TAG = "RAS_WIDTH_UPGRADE_COUNT_TAG";

	public const string RAS_HEIGHT_UPGRADE_COUNT_TAG = "RAS_HEIGHT_UPGRADE_COUNT_TAG";

	public const string RAS_POWER_UPGRADE_COUNT_TAG = "RAS_POWER_UPGRADE_COUNT_TAG";

	public const int MAX_SIZE_UPGRADES = 4;

	public const int MAX_POWER_UPGRADES = 12;

	public const int WIDTH_UPGRADE_POWER_INCREASE = 2;

	public const int HEIGHT_UPGRADE_POWER_INCREASE = 3;

	public const int POWER_UPGRADE_REDUCTION = 2;

	public const int MIN_POWER_USE = 1;

	public const int WATER_TANK_CAPACITY_ML = 12000;

	private const int COL_BATTERY = 0;

	private const int COL_WATER = 1;

	private const int COL_FILTERS = 2;

	private const int COL_DISPENSER = 3;

	private const int COL_TANK = 4;

	private const int COL_BUTCHER = 5;

	private const int FILTER_GRID_COLUMNS = 4;

	public static GameItem CreateRAS()
	{
		GameSlotInventory batterySlot = new GameSlotInventory();
		GameSlotInventory waterSlot = new GameSlotInventory();
		List<GameSlotInventory> filterSlots = new List<GameSlotInventory>();
		for (int i = 0; i < 4; i++)
		{
			filterSlots.Add(new GameSlotInventory());
		}
		GameSlotInventory dispenserSlot = new GameSlotInventory();
		GameGridInventory tankGrid = new GameGridInventory(8, 4);
		batterySlot.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		waterSlot.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		dispenserSlot.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		foreach (GameSlotInventory item in filterSlots)
		{
			item.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		}
		tankGrid.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 1;
		GridPixelElement gridPixelElement = new GridPixelElement(4, 1);
		for (int j = 0; j < 4; j++)
		{
			gridPixelElement.Attach(filterSlots[j], j % 4, j / 4, 5, 5);
		}
		MachineryButton blendButton = new MachineryButton(LocHelper.GetLocalizedMechanic("mech_ras_btn_blend"));
		GridPixelElement gridPixelElement2 = new GridPixelElement(1, 2, isCentered: true);
		gridPixelElement2.SetSpacing(0, 4);
		gridPixelElement2.Attach(new MachineryIntake(dispenserSlot), 0, 0, 5, 5);
		gridPixelElement2.Attach(blendButton, 0, 1, 5, 5);
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		MachineryButton machineryButton = new MachineryButton(LocHelper.GetLocalizedMechanic("mech_auto_butcher_btn_butcher_now"));
		GridPixelElement gridPixelElement3 = new GridPixelElement(1, 2, isCentered: true);
		gridPixelElement3.SetSpacing(0, 4);
		gridPixelElement3.Attach(new MachineryIntake(gameSlotInventory), 0, 0, 5, 5);
		gridPixelElement3.Attach(machineryButton, 0, 1, 5, 5);
		PixelWindow contentWindow = MachineHelper.CreateMachineryWindow(true, MachineHelper.PowerPanel(batterySlot), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_hydroponic_label_water"), waterSlot, "water"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_ras_label_filters"), gridPixelElement, "filters"), new MachineryPanel(LocHelper.GetLocalizedMechanic("mech_ras_label_dispenser"), gridPixelElement2, "dispenser"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_ras_label_tank"), tankGrid, "tank"), new MachineryPanel(LocHelper.GetLocalizedMechanic("mech_auto_butcher_label_slot"), gridPixelElement3, "butcher"));
		GameItem machine = ItemDirectory.CreateEmptyItem().SetContentWindow(contentWindow).SetSpriteAndShape("Items/items_aquaculture", "ras_empty");
		machine.name = LocHelper.GetLocalizedItem("item_ras_name");
		machine.shortDescription = LocHelper.GetLocalizedItem("item_ras_desc");
		machine.unitValue = 300L;
		machine.EnableTag("RAS_TAG");
		machine.ModifyTag("RAS_WIDTH_UPGRADE_COUNT_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		machine.ModifyTag("RAS_HEIGHT_UPGRADE_COUNT_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		machine.ModifyTag("RAS_POWER_UPGRADE_COUNT_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		MachineryHelper.InitMachinery(machine, 5);
		MachineHelper.SetupBatterySlot(batterySlot, machine);
		waterSlot.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(machine, item) && item.IsTag("RAS_WATER_TANK_TAG");
		waterSlot.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		waterSlot.SetBackgroundFadeSprite("Items/items_aquaculture", "ras_tank");
		foreach (GameSlotInventory item2 in filterSlots)
		{
			item2.EnableTag("RAS_FILTER_SLOT_TAG");
			item2.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(machine, item) && item.IsTag("WATER_FILTER");
			item2.SetBackgroundFadeSprite("Items/items_amenities", "water_filter");
		}
		dispenserSlot.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(machine, item) && item.IsTag("FEED_DISPENSER_TAG");
		dispenserSlot.SetBackgroundFadeSprite("Items/items_husbandry", "animal_feeder");
		blendButton.SetCallback(delegate
		{
			GameItem gameItem4 = SlottedDispenser();
			if (gameItem4 != null)
			{
				MachineFeedDispenser.Blend(gameItem4, MachineFeedDispenser.GetStorageGrid(gameItem4));
			}
		});
		dispenserSlot.onSlotAddItemEndFunc = delegate
		{
			RefreshBlendButton();
		};
		dispenserSlot.onSlotRemoveItemEndFunc = delegate
		{
			RefreshBlendButton();
		};
		GameItem gameItem = machine;
		gameItem.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem.onLoaded, (Action<GameItem>)delegate
		{
			RefreshBlendButton();
		});
		RefreshBlendButton();
		MachineAutoButcher.SetupHostSlot(machine, gameSlotInventory, machineryButton);
		tankGrid.EnableTag("RAS_TANK_GRID_TAG");
		tankGrid.onSlotAddItemEndFunc = delegate
		{
			UpdateSprite(machine);
		};
		tankGrid.onSlotRemoveItemEndFunc = delegate
		{
			UpdateSprite(machine);
		};
		waterSlot.onSlotAddItemEndFunc = delegate
		{
			UpdateSprite(machine);
		};
		GameItem gameItem2 = machine;
		gameItem2.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem2.onLoaded, (Action<GameItem>)delegate(GameItem loaded)
		{
			UpdateSprite(loaded);
		});
		GameItem gameItem3 = machine;
		gameItem3.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem3.onLoaded, (Action<GameItem>)delegate(GameItem loaded)
		{
			ApplyTankSize(loaded);
			UpdatePowerUsage(loaded);
		});
		tankGrid.mayInventoryAddItemFunc = delegate(GameItem item, GameInventory _)
		{
			if (!GeneralHelper.IsItemOwned(machine))
			{
				return false;
			}
			if (!GeneralHelper.IsItemOwned(item))
			{
				return false;
			}
			if (item.IsTag("CONTAINER_TAG"))
			{
				return false;
			}
			if (item.IsGameItemType("MACHINE"))
			{
				return false;
			}
			return item.IsTag("FISH_ITEM_TAG") || item.IsTag("FISH_GROUP_TAG") || item.IsTag("CALORIE_VALUE_TAG");
		};
		machine.onCycleEndEarlySlotItemFunc = delegate(GameItem curItem, GameInventory _, SlotMarker _)
		{
			AquacultureHelper.GeneratePower(curItem, tankGrid);
		};
		machine.onCycleEndSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			bool isPowered = IsWorthRunning(curItem) && MachineHelper.TryDrawCyclePower(curItem, batterySlot);
			AquacultureHelper.TickTank(curItem, tankGrid, waterSlot.childItem, filterSlots, dispenserSlot, isPowered);
			UpdateSprite(curItem);
		};
		return machine;
		void RefreshBlendButton()
		{
			blendButton.SetHidden(SlottedDispenser() == null);
		}
		GameItem SlottedDispenser()
		{
			if (dispenserSlot.childItem == null || !dispenserSlot.childItem.IsTag("FEED_DISPENSER_TAG"))
			{
				return null;
			}
			return dispenserSlot.childItem;
		}
	}

	public static GameItem SpawnRASWithEmptyTank()
	{
		GameItem gameItem = ItemSpawner.Spawn("ras");
		if (gameItem == null)
		{
			return null;
		}
		GameItem gameItem2 = ItemSpawner.Spawn("ras_water_tank");
		if (gameItem2 != null)
		{
			GraphUtils.TryAcceptAllOrDestroy(gameItem, gameItem2, "ras sold with tank");
		}
		return gameItem;
	}

	public static GameItem CreateWaterTank()
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetName(LocHelper.GetLocalizedItem("item_ras_water_tank_name")).SetSpriteAndShape("Items/items_aquaculture", "ras_tank");
		gameItem.identifier = "ras_water_tank";
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_ras_water_tank_desc");
		WaterHelper.InitLiquidContainerItem(gameItem, 12000, 0, isFilterContainer: false, isGraduated: false, isDropper: false, isTransparentContainer: false, 4);
		gameItem.unitValue = 0L;
		gameItem.EnableTag("RAS_WATER_TANK_TAG");
		gameItem.EnableTag("IMPORTANT_TAG");
		AudioHelper.InitPlasticContainer(gameItem);
		gameItem.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem.onLoaded, (Action<GameItem>)delegate(GameItem loaded)
		{
			loaded.EnableTag("RAS_WATER_TANK_TAG");
			loaded.EnableTag("IMPORTANT_TAG");
		});
		return gameItem;
	}

	public static bool CanUpgradeWidth(GameItem ras)
	{
		if (ras == null || !ras.IsTag("RAS_TAG"))
		{
			return false;
		}
		return ras.GetTagReadonly("RAS_WIDTH_UPGRADE_COUNT_TAG").GetInt() < 4;
	}

	public static bool CanUpgradeHeight(GameItem ras)
	{
		if (ras == null || !ras.IsTag("RAS_TAG"))
		{
			return false;
		}
		return ras.GetTagReadonly("RAS_HEIGHT_UPGRADE_COUNT_TAG").GetInt() < 4;
	}

	public static bool CanUpgradePowerEfficiency(GameItem ras)
	{
		if (ras == null || !ras.IsTag("RAS_TAG"))
		{
			return false;
		}
		return ras.GetTagReadonly("RAS_POWER_UPGRADE_COUNT_TAG").GetInt() < 12;
	}

	public static void UpgradeWidth(GameItem ras, GameItem upgradeItem)
	{
		if (CanUpgradeWidth(ras))
		{
			BumpCount(ras, "RAS_WIDTH_UPGRADE_COUNT_TAG");
			ApplyTankSize(ras);
			ConsumeUpgradeItem(ras, upgradeItem);
		}
	}

	public static void UpgradeHeight(GameItem ras, GameItem upgradeItem)
	{
		if (CanUpgradeHeight(ras))
		{
			BumpCount(ras, "RAS_HEIGHT_UPGRADE_COUNT_TAG");
			ApplyTankSize(ras);
			ConsumeUpgradeItem(ras, upgradeItem);
		}
	}

	public static void UpgradePowerEfficiency(GameItem ras, GameItem upgradeItem)
	{
		if (CanUpgradePowerEfficiency(ras))
		{
			BumpCount(ras, "RAS_POWER_UPGRADE_COUNT_TAG");
			ConsumeUpgradeItem(ras, upgradeItem);
		}
	}

	private static void BumpCount(GameItem ras, string tag)
	{
		int count = ras.GetTagReadonly(tag).GetInt();
		ras.ModifyTag(tag, delegate(TagState state)
		{
			state.SetInt(count + 1);
		});
	}

	private static void ConsumeUpgradeItem(GameItem ras, GameItem upgradeItem)
	{
		UpdatePowerUsage(ras);
		ras.unitValue += upgradeItem.unitValue;
		StartingPerkContent.ConsumeUpgradeMaterial(upgradeItem);
		EmporiumAudioPlayer.Instance.ratchet.Play();
	}

	public static void ApplyTankSize(GameItem ras)
	{
		GameGridInventory tankGrid = GetTankGrid(ras);
		if (tankGrid != null)
		{
			int @int = ras.GetTagReadonly("RAS_WIDTH_UPGRADE_COUNT_TAG").GetInt();
			int int2 = ras.GetTagReadonly("RAS_HEIGHT_UPGRADE_COUNT_TAG").GetInt();
			tankGrid.SetShape(8 + @int, 4 + int2);
			tankGrid.Validate();
			ras.contentWindow.Validate();
			ras.Validate();
		}
	}

	public static void UpdatePowerUsage(GameItem ras)
	{
		int @int = ras.GetTagReadonly("RAS_WIDTH_UPGRADE_COUNT_TAG").GetInt();
		int int2 = ras.GetTagReadonly("RAS_HEIGHT_UPGRADE_COUNT_TAG").GetInt();
		int int3 = ras.GetTagReadonly("RAS_POWER_UPGRADE_COUNT_TAG").GetInt();
		int total = 5 + @int * 2 + int2 * 3 - int3 * 2;
		if (total < 1)
		{
			total = 1;
		}
		ras.ModifyTag("machinery_drawn_power", delegate(TagState state)
		{
			state.Enable().SetInt(total);
		});
	}

	public static GameGridInventory GetTankGrid(GameItem machine)
	{
		return MachineHelper.GetBodyStorage(machine, 4) as GameGridInventory;
	}

	public static GameSlotInventory GetWaterSlot(GameItem machine)
	{
		return MachineHelper.GetBodyStorage(machine, 1) as GameSlotInventory;
	}

	public static GameSlotInventory GetDispenserSlot(GameItem machine)
	{
		return (MachineHelper.GetBodyStorage(machine, 3) as GridPixelElement)?.children.Find((GraphNodeStorage n) => n is GameSlotInventory) as GameSlotInventory;
	}

	public static GameSlotInventory GetButcherSlot(GameItem machine)
	{
		return (MachineHelper.GetBodyStorage(machine, 5) as GridPixelElement)?.children.Find((GraphNodeStorage n) => n is GameSlotInventory) as GameSlotInventory;
	}

	public static GridPixelElement GetFilterGrid(GameItem machine)
	{
		return MachineHelper.GetBodyStorage(machine, 2) as GridPixelElement;
	}

	public static bool HasWater(GameItem machine)
	{
		GameItem gameItem = GetWaterSlot(machine)?.childItem;
		if (gameItem != null)
		{
			return WaterHelper.GetTotalVolume(gameItem) > 0;
		}
		return false;
	}

	public static bool HasLiveStock(GameItem machine)
	{
		return GetTankGrid(machine)?.childItems.Exists(AquacultureHelper.IsLiveStock) ?? false;
	}

	public static bool IsWorthRunning(GameItem machine)
	{
		if (HasWater(machine))
		{
			return HasLiveStock(machine);
		}
		return false;
	}

	public static bool HasFish(GameItem machine)
	{
		return GetTankGrid(machine)?.childItems.Exists((GameItem f) => AquacultureHelper.IsFish(f) || AquacultureHelper.IsGroup(f)) ?? false;
	}

	public static void OnWaterTankLiquidChanged(GameItem tank)
	{
		if (tank != null && tank.IsTag("RAS_WATER_TANK_TAG"))
		{
			GameItem gameItem = tank.parentInventory?.GetParentItem();
			if (gameItem != null && gameItem.IsTag("RAS_TAG") && GetWaterSlot(gameItem)?.childItem == tank)
			{
				UpdateSprite(gameItem);
			}
		}
	}

	public static void UpdateSprite(GameItem machine)
	{
		string text = ((!HasWater(machine)) ? "ras_empty" : (HasFish(machine) ? "ras_full" : "ras"));
		if (!(machine.spritePath == text) || !(machine.spriteAtlasPath == "Items/items_aquaculture"))
		{
			machine.SetSpriteAndShape("Items/items_aquaculture", text);
		}
	}

	public static int GetInstalledFilterCount(GameItem machine)
	{
		GridPixelElement filterGrid = GetFilterGrid(machine);
		if (filterGrid == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			if (filterGrid.GetElement(i % 4, i / 4) is GameSlotInventory { childItem: not null })
			{
				num++;
			}
		}
		return num;
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!item.IsTag("RAS_TAG"))
		{
			return;
		}
		GameGridInventory tankGrid = GetTankGrid(item);
		GameItem gameItem = GetWaterSlot(item)?.childItem;
		builder.AddLine(" ");
		if (gameItem == null)
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_ras_no_water_tank"), ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
		}
		else
		{
			int waterMl = AquacultureHelper.GetWaterMl(gameItem);
			int totalWaterNeedMl = AquacultureHelper.GetTotalWaterNeedMl(tankGrid);
			float wastePercent = AquacultureHelper.GetWastePercent(gameItem);
			RenderHandler.ColorPalette color = ((totalWaterNeedMl == 0 || waterMl >= totalWaterNeedMl) ? RenderHandler.ColorPalette.LightCyan : (((float)waterMl >= (float)totalWaterNeedMl * 0.5f) ? RenderHandler.ColorPalette.Yellow : RenderHandler.ColorPalette.LightRed));
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_ras_water", waterMl, totalWaterNeedMl), ignoreEmpty: true, color);
			RenderHandler.ColorPalette color2 = ((wastePercent < 3f) ? RenderHandler.ColorPalette.LightCyan : ((wastePercent < 8f) ? RenderHandler.ColorPalette.Yellow : RenderHandler.ColorPalette.LightRed));
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_ras_waste", wastePercent.ToString("0.0")), ignoreEmpty: true, color2);
		}
		if (tankGrid != null)
		{
			int num = 0;
			int num2 = 0;
			foreach (GameItem childItem in tankGrid.childItems)
			{
				if (AquacultureHelper.IsLiveStock(childItem))
				{
					num++;
				}
				else if (AquacultureHelper.IsCarcass(childItem))
				{
					num2++;
				}
			}
			builder.AddLine((num2 > 0) ? LocHelper.GetLocalizedMechanic("mech_ras_stock_dead", num, num2) : LocHelper.GetLocalizedMechanic("mech_ras_stock", num), ignoreEmpty: true, (num2 > 0) ? RenderHandler.ColorPalette.Yellow : RenderHandler.ColorPalette.LightCyan);
			int tankPowerOutput = AquacultureHelper.GetTankPowerOutput(tankGrid);
			if (tankPowerOutput > 0)
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_ras_eel_power", tankPowerOutput), ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue);
			}
		}
		MachineFeedDispenser.CreateSlottedTooltip(builder, GetDispenserSlot(item)?.childItem);
		int @int = item.GetTagReadonly("RAS_WIDTH_UPGRADE_COUNT_TAG").GetInt();
		int int2 = item.GetTagReadonly("RAS_HEIGHT_UPGRADE_COUNT_TAG").GetInt();
		int int3 = item.GetTagReadonly("RAS_POWER_UPGRADE_COUNT_TAG").GetInt();
		builder.AddLine(" ");
		builder.AddLine(LocHelper.GetLocalizedMechanic("mech_fridge_tooltip_upgrade_header"), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		builder.AddLine(LocHelper.GetLocalizedMechanic("mech_fridge_tooltip_upgrade_storage", LocHelper.GetLocalizedItem("item_scrap_metal_name"), @int, 4), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		builder.AddLine(LocHelper.GetLocalizedMechanic("mech_fridge_tooltip_upgrade_storage", LocHelper.GetLocalizedItem("item_metal_ingot_name"), int2, 4), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		builder.AddLine(LocHelper.GetLocalizedMechanic("mech_fridge_tooltip_upgrade_efficiency", LocHelper.GetLocalizedItem("item_common_electronic_name"), int3, 12), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
	}
}
