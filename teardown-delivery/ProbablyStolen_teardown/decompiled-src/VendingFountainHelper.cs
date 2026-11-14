using TMPro;

public class VendingFountainHelper
{
	public const string VENDING_FOUNTAIN_TAG = "VENDING_FOUNTAIN_TAG";

	public const int PARTS_PER_POWER_STEP = 2;

	public const int POWER_STEP = 1;

	public static void UnlockVendingFountain()
	{
		EmporiumEntry.Instance.vendingFountainWindow.Validate();
		PlayerStore.Instance.isVendingFountainUnlocked = true;
		NetworkUpgrade.MarkUnlockedSilently("VENDING_FOUNTAIN");
		StoreVendingFountain.Instance.gameObject.SetActive(value: true);
		GameItem gameItem = ItemSpawner.Spawn("vending_fountain");
		GameItem gameItem2 = ItemSpawner.Spawn("vending_fountain_core");
		GeneralHelper.SetParentItem(gameItem2, gameItem);
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => true;
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.vendingFountainElement, gameItem);
		GraphUtils.TryAcceptAll(gameItem, gameItem2);
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		EmporiumEntry.Instance.Validate();
	}

	public static GameItem VendingFountainCore()
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetSpriteAndShape("Items/items_module", "core");
		gameItem.name = LocHelper.GetLocalizedItem("item_vending_fountain_core_name");
		return gameItem;
	}

	public static GameItem VendingFountain()
	{
		(PixelWindow, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, MachineryText, MachineryButton, GameSlotInventory) tuple = CreateMachineInventoryWindow();
		PixelWindow item2 = tuple.Item1;
		GameSlotInventory powerInv = tuple.Item2;
		GameSlotInventory baseWaterInv = tuple.Item3;
		GameSlotInventory highQualityWaterInv = tuple.Item4;
		GameSlotInventory pureWaterInv = tuple.Item5;
		MachineryText cashLabel = tuple.Item6;
		MachineryButton item3 = tuple.Item7;
		GameSlotInventory item4 = tuple.Rest.Item1;
		GameItem machine = ItemDirectory.CreateEmptyItem().SetContentWindow(item2).SetSpriteAndShape("Items/items_vending", "fountain0");
		machine.name = LocHelper.GetLocalizedItem("item_vending_fountain_name");
		machine.flavorText = LocHelper.GetLocalizedItem("item_vending_fountain_flavor");
		machine.shortDescription = LocHelper.GetLocalizedItem("item_vending_fountain_desc");
		MachineryHelper.InitMachinery(machine, 2);
		baseWaterInv.SetBackgroundFadeSprite("Items/water_container4", "xl");
		highQualityWaterInv.SetBackgroundFadeSprite("Items/water_container4", "xl");
		pureWaterInv.SetBackgroundFadeSprite("Items/water_container4", "xl");
		machine.EnableTag("IMPORTANT_TAG");
		machine.ModifyTag("VENDING_CASH", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		machine.ModifyTag("VENDING_ATTRACTIVENESS", delegate(TagState state)
		{
			state.SetInt(15);
		});
		machine.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		baseWaterInv.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => MayAcceptWaterSource(item);
		highQualityWaterInv.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => MayAcceptWaterSource(item);
		pureWaterInv.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => MayAcceptWaterSource(item);
		item4.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 3;
		item4.mayInventoryRemoveItemFunc = (GameItem item, GameInventory _) => false;
		item2.openCallback = delegate
		{
			cashLabel.SetText(LocHelper.GetLocalizedMechanic("mech_vending_cash_label", machine.GetTagReadonly("VENDING_CASH").GetInt()));
			UpdateSprite(machine, "fountain", baseWaterInv, highQualityWaterInv, pureWaterInv);
		};
		item3.SetCallback(delegate
		{
			VendingMachineHelper.TransferCashToPlayer(machine);
			cashLabel.SetText(LocHelper.GetLocalizedMechanic("mech_vending_cash_label", machine.GetTagReadonly("VENDING_CASH").GetInt()));
		});
		baseWaterInv.onSlotAddItemEndFunc = delegate
		{
			UpdateSprite(machine, "fountain", baseWaterInv, highQualityWaterInv, pureWaterInv);
		};
		baseWaterInv.onSlotRemoveItemEndFunc = delegate
		{
			UpdateSprite(machine, "fountain", baseWaterInv, highQualityWaterInv, pureWaterInv);
		};
		highQualityWaterInv.onSlotAddItemEndFunc = delegate
		{
			UpdateSprite(machine, "fountain", baseWaterInv, highQualityWaterInv, pureWaterInv);
		};
		highQualityWaterInv.onSlotRemoveItemEndFunc = delegate
		{
			UpdateSprite(machine, "fountain", baseWaterInv, highQualityWaterInv, pureWaterInv);
		};
		pureWaterInv.onSlotAddItemEndFunc = delegate
		{
			UpdateSprite(machine, "fountain", baseWaterInv, highQualityWaterInv, pureWaterInv);
		};
		pureWaterInv.onSlotRemoveItemEndFunc = delegate
		{
			UpdateSprite(machine, "fountain", baseWaterInv, highQualityWaterInv, pureWaterInv);
		};
		MachineHelper.SetupBatterySlot(powerInv, machine);
		machine.onCycleEndLateSlotItemFunc = delegate
		{
			if (powerInv.childItem != null && PowerHelper.CanDrawPowerSource(powerInv.childItem, MachineryHelper.GetMachinePowerUsage(machine)))
			{
				bool num = IsSourceEmpty(baseWaterInv);
				bool flag = IsSourceEmpty(highQualityWaterInv);
				bool flag2 = IsSourceEmpty(pureWaterInv);
				if (!(num && flag && flag2))
				{
					HandleNight(machine, baseWaterInv, highQualityWaterInv, pureWaterInv);
					PowerHelper.DrawPowerSource(powerInv.childItem, MachineryHelper.GetMachinePowerUsage(machine));
					cashLabel.SetText(LocHelper.GetLocalizedMechanic("mech_vending_cash_label", machine.GetTagReadonly("VENDING_CASH").GetInt()));
				}
			}
		};
		machine.onLoaded = delegate
		{
			UpdateSprite(machine, "fountain", baseWaterInv, highQualityWaterInv, pureWaterInv);
			UpdatePowerUsage(machine);
		};
		return machine;
	}

	public static (PixelWindow, GameSlotInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory, MachineryText, MachineryButton, GameSlotInventory) CreateMachineInventoryWindow(bool isDraggable = true, bool isCentered = true, int minWidth = 5)
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameSlotInventory gameSlotInventory2 = new GameSlotInventory();
		GameSlotInventory gameSlotInventory3 = new GameSlotInventory();
		GameSlotInventory gameSlotInventory4 = new GameSlotInventory();
		GameSlotInventory gameSlotInventory5 = new GameSlotInventory();
		MachineryText machineryText = new MachineryText(LocHelper.GetLocalizedMechanic("mech_vending_cash_label", 0), 6, HorizontalAlignmentOptions.Center, uppercase: false);
		MachineryButton machineryButton = new MachineryButton(LocHelper.GetLocalizedMechanic("mech_vending_btn_transfer"));
		GridPixelElement gridPixelElement = new GridPixelElement(1, 2, isCentered: true);
		gridPixelElement.SetSpacing(0, 4);
		gridPixelElement.Attach(machineryText, 0, 0, 60, 10);
		gridPixelElement.Attach(machineryButton, 0, 1, 60, 5);
		gameSlotInventory.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory2.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 1;
		return (MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.PowerPanel(gameSlotInventory), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_fountain_label_basewater"), gameSlotInventory2, "base_water"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_fountain_label_hq_water"), gameSlotInventory3, "hq_water"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_fountain_label_pure_water"), gameSlotInventory4, "pure_water"), new MachineryPanel(LocHelper.GetLocalizedMechanic("mech_fountain_label_control"), gridPixelElement, "control"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_fountain_label_core"), gameSlotInventory5, "core")), gameSlotInventory, gameSlotInventory2, gameSlotInventory3, gameSlotInventory4, machineryText, machineryButton, gameSlotInventory5);
	}

	public static bool IsWaterSourceMachine(GameItem item)
	{
		if (item != null)
		{
			if (!item.IsID("water_purifier"))
			{
				return item.IsID("moisture_farm");
			}
			return true;
		}
		return false;
	}

	public static bool MayAcceptWaterSource(GameItem item)
	{
		bool flag = item.IsGameItemType("WATER") && item.IsTag("LIQUID_CONTAINER_TAG");
		if (GeneralHelper.IsItemOwned(item))
		{
			if (!flag)
			{
				return IsWaterSourceMachine(item);
			}
			return true;
		}
		return false;
	}

	public static GameItem GetWaterSource(GameSlotInventory slot)
	{
		GameItem gameItem = slot?.childItem;
		if (gameItem == null)
		{
			return null;
		}
		if (!IsWaterSourceMachine(gameItem))
		{
			return gameItem;
		}
		return ((gameItem.contentWindow != null) ? (MachineHelper.GetSlot(gameItem, 2) as GameSlotInventory) : null)?.childItem;
	}

	public static bool IsSourceEmpty(GameSlotInventory slot)
	{
		GameItem waterSource = GetWaterSource(slot);
		if (waterSource != null)
		{
			return WaterHelper.IsEmpty(waterSource);
		}
		return true;
	}

	public static void HandleNight(GameItem vendingMachine, GameSlotInventory baseWaterInv, GameSlotInventory highQualityWaterInv, GameSlotInventory pureWaterInv)
	{
		Debug.Log("Vending Fountain selling water...");
		int @int = vendingMachine.GetTagReadonly("VENDING_ATTRACTIVENESS").GetInt();
		SimulateWaterSale(2, "bottled_water", baseWaterInv, vendingMachine);
		SimulateWaterSale(1, (@int >= 85) ? "bottled_water" : "small_bottled_water", highQualityWaterInv, vendingMachine);
		if (@int >= 20)
		{
			string containerID = "mini_bottle";
			if (@int >= 50)
			{
				containerID = "small_bottled_water";
			}
			if (@int >= 75)
			{
				containerID = "bottled_water";
			}
			if (@int >= 100)
			{
				containerID = "large_bottled_water";
			}
			SimulateWaterSale(0, containerID, pureWaterInv, vendingMachine);
		}
		if (@int >= 40)
		{
			SimulateWaterSale(2, "large_bottled_water", baseWaterInv, vendingMachine);
		}
		else if (@int >= 35)
		{
			SimulateWaterSale(2, "bottled_water", baseWaterInv, vendingMachine);
		}
		else if (@int >= 30)
		{
			SimulateWaterSale(2, "small_bottled_water", baseWaterInv, vendingMachine);
		}
		else if (@int >= 20)
		{
			SimulateWaterSale(2, "mini_bottle", baseWaterInv, vendingMachine);
		}
		if (@int >= 80)
		{
			SimulateWaterSale(2, "large_bottled_water", baseWaterInv, vendingMachine);
		}
		else if (@int >= 75)
		{
			SimulateWaterSale(2, "bottled_water", baseWaterInv, vendingMachine);
		}
		else if (@int >= 70)
		{
			SimulateWaterSale(2, "small_bottled_water", baseWaterInv, vendingMachine);
		}
		else if (@int >= 65)
		{
			SimulateWaterSale(2, "mini_bottle", baseWaterInv, vendingMachine);
		}
		if (@int >= 60)
		{
			SimulateWaterSale(1, "large_bottled_water", highQualityWaterInv, vendingMachine);
		}
		else if (@int >= 55)
		{
			SimulateWaterSale(1, "bottled_water", highQualityWaterInv, vendingMachine);
		}
		else if (@int >= 45)
		{
			SimulateWaterSale(1, "small_bottled_water", highQualityWaterInv, vendingMachine);
		}
		else if (@int >= 25)
		{
			SimulateWaterSale(1, "mini_bottle", highQualityWaterInv, vendingMachine);
		}
		UpdateSprite(vendingMachine, "fountain", baseWaterInv, highQualityWaterInv, pureWaterInv);
	}

	public static void SimulateWaterSale(int labelPurity, string containerID, GameSlotInventory waterInv, GameItem vendingMachine)
	{
		GameItem waterSource = GetWaterSource(waterInv);
		if (waterSource == null || WaterHelper.IsEmpty(waterSource))
		{
			return;
		}
		string identifier = WaterFeatureHelper.GetConditionFromPurity(WaterHelper.GetWaterPurity(waterSource)).identifier;
		if (!identifier.Equals(WaterFeatureHelper.GUTTERFLOW_WATER_DISPLAY) && !identifier.Equals(WaterFeatureHelper.RUST_WATER_DISPLAY))
		{
			GameItem gameItem = ItemSpawner.Spawn(containerID);
			gameItem.unitValue = 0L;
			WaterHelper.TransferLiquid(waterSource, gameItem);
			ItemFeature itemFeature = gameItem.FindItemFeatureByCategory("CATEGORY_WATER_PURITY");
			itemFeature.fakeCondition = ItemConditionList.conditionWaterPurity[labelPurity];
			itemFeature.SetExposed(value: false);
			WaterFeatureHelper.OnWaterLabelUpdated(itemFeature, labelPurity);
			gameItem.AddItemFeature(ItemFeatureList.VendingMachineBonus());
			int value = (int)gameItem.GetCurrentValue(useRetailMarkup: true, withChild: true, includeEvents: false, forceMarkup: true, 0L);
			bool isGhostwater;
			float mislabeledWaterWeight = SecData.GetMislabeledWaterWeight(itemFeature, out isGhostwater);
			if (mislabeledWaterWeight > 0f)
			{
				VendingMachineHelper.TrackMislabelledFountainSale(labelPurity, (float)value * mislabeledWaterWeight);
			}
			PlayerStore.Instance.PerformOnSoldCheck(gameItem, null);
			gameItem.Destroy();
			int currentCash = vendingMachine.GetTagReadonly("VENDING_CASH").GetInt();
			vendingMachine.ModifyTag("VENDING_CASH", delegate(TagState state)
			{
				state.SetInt(currentCash + value);
			});
			RunStats.OnFountainSale(value);
		}
	}

	public static void UpdatePowerUsage(GameItem fountain)
	{
		int num = VendingMachineHelper.GetAttractivenessUpgradeCount(fountain) / 2;
		int total = 2 + num;
		fountain.ModifyTag("machinery_drawn_power", delegate(TagState state)
		{
			state.Enable().SetInt(total);
		});
		fountain.ModifyTag("MACHINERY_DRAWN_POWER_BASE", delegate(TagState state)
		{
			state.Enable().SetInt(2);
		});
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.identifier.Equals("vending_fountain_core"))
		{
			GameItem parentItem = GeneralHelper.GetParentItem(item);
			if (parentItem != null)
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_vending_tooltip_markup"), ignoreEmpty: true, RenderHandler.ColorPalette.LightTurquoise);
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_vending_tooltip_attractiveness", parentItem.GetTagReadonly("VENDING_ATTRACTIVENESS").GetInt()), ignoreEmpty: true, RenderHandler.ColorPalette.LightTurquoise);
				VendingMachineHelper.AddUpgradeHeader(builder);
				VendingMachineHelper.AddAttractivenessUpgradeLine(builder, parentItem);
				builder.AddLine(LocHelper.GetLocalizedMechanicOrDefault("mech_fountain_tooltip_upgrade_power", "Every {1} {0} upgrades raise the nightly energy use by {2}.", LocHelper.GetLocalizedItem("item_common_electronic_name"), 2, 1), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
				MachineryHelper.CreateMachineryTooltip(builder, parentItem);
			}
		}
	}

	public static int GetTotalCapacityPercentage(GameItem item)
	{
		int num = 0;
		int num2 = 0;
		GameSlotInventory gameSlotInventory = (GameSlotInventory)MachineHelper.GetSlot(item, 1);
		GameSlotInventory gameSlotInventory2 = (GameSlotInventory)MachineHelper.GetSlot(item, 2);
		GameSlotInventory gameSlotInventory3 = (GameSlotInventory)MachineHelper.GetSlot(item, 3);
		GameSlotInventory[] array = new GameSlotInventory[3] { gameSlotInventory, gameSlotInventory2, gameSlotInventory3 };
		for (int i = 0; i < array.Length; i++)
		{
			GameItem waterSource = GetWaterSource(array[i]);
			if (waterSource != null)
			{
				num += WaterHelper.GetCurrentCapacityML(waterSource);
				num2 += WaterHelper.GetTotalCapacityML(waterSource);
			}
		}
		if (num2 > 0)
		{
			return (int)((float)num / (float)num2 * 100f);
		}
		return 0;
	}

	public static string GetSpriteNameFromCapacity(string prefix, int currentCapacity)
	{
		if (currentCapacity == 0)
		{
			return prefix + "0";
		}
		if (currentCapacity <= 14)
		{
			return prefix + "10";
		}
		if (currentCapacity <= 35)
		{
			return prefix + "25";
		}
		if (currentCapacity <= 65)
		{
			return prefix + "50";
		}
		if (currentCapacity <= 80)
		{
			return prefix + "75";
		}
		if (currentCapacity <= 97)
		{
			return prefix + "90";
		}
		return prefix + "100";
	}

	public static void UpdateSprite(GameItem gameItem, string prefix, GameSlotInventory baseWaterInv, GameSlotInventory highQualityWaterInv, GameSlotInventory pureWaterInv)
	{
		int totalCapacityPercentage = GetTotalCapacityPercentage(gameItem);
		string spriteNameFromCapacity = GetSpriteNameFromCapacity(prefix, totalCapacityPercentage);
		gameItem.SetSpriteAndShape("Items/items_vending", spriteNameFromCapacity);
		StoreVendingFountain.Instance.UpdateSprite();
	}
}
