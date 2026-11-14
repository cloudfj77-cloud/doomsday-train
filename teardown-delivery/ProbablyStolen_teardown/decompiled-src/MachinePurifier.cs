using System;
using System.Collections.Generic;

public class MachinePurifier
{
	public const string TOTAL_CONTAMINANT_REMOVED_TAG = "TOTAL_CONTAMINANT_REMOVED_TAG";

	public const int BUDGET_VOLUME_PERMILLE = 3;

	public const int BUDGET_FLAT_UL = 25000;

	public const int BUDGET_VOLUME_CAP_UL = 10000000;

	public const double REMOVAL_MULTIPLIER = 1.25;

	public const int BASE_MAX_PURITY = 9901;

	private static readonly (int purity, double bonus)[] DIRT_BONUS_CURVE = new(int, double)[6]
	{
		(9800, 1.0),
		(9400, 2.0),
		(8850, 5.0),
		(8000, 10.0),
		(6250, 15.0),
		(3750, 20.0)
	};

	private static readonly HashSet<string> REMOVED_LAST = new HashSet<string> { "mineral", "chemical_contaminant", "heavy_metal" };

	public static GameItem WaterPurifier()
	{
		(PixelWindow, GameSlotInventory, GameInventory, GameSlotInventory, GameSlotInventory) tuple = CreateMachineInventoryWindow();
		PixelWindow item2 = tuple.Item1;
		GameSlotInventory powerInv = tuple.Item2;
		GameInventory itemInventoryModule = tuple.Item3;
		GameSlotInventory bottleInv = tuple.Item4;
		GameSlotInventory item3 = tuple.Item5;
		item3.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => item.identifier.Equals("postit");
		item3.SetBackgroundFadeSprite("Items/items_tool", "postit");
		bottleInv.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(item) && item.IsTag("LIQUID_CONTAINER_TAG");
		bottleInv.SetBackgroundFadeSprite("Items/water_container4", "xl");
		GameItem machine = ItemDirectory.CreateEmptyItem().SetContentWindow(item2).SetSpriteAndShape("Items/items_tool2", "purifier");
		machine.name = LocHelper.GetLocalizedItem("item_water_purifier_name");
		machine.flavorText = LocHelper.GetLocalizedItem("item_water_purifier_flavor");
		machine.shortDescription = LocHelper.GetLocalizedItem("item_water_purifier_desc");
		machine.unitValue = 250L;
		MachineHelper.SetupBatterySlot(powerInv, machine);
		machine.EnableTag("CONTAINER_TAG");
		machine.EnableTag("manufacture");
		machine.EnableTag("STANDARD_MACHINE_TAG");
		machine.SetGameItemType("MACHINE");
		MachineryHelper.InitMachinery(machine, 4);
		machine.ModifyTag("MACHINE_QUALITY_RATIO_INT", delegate(TagState state)
		{
			state.Enable().SetInt(2);
		});
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
			item.ModifyTag("TOTAL_CONTAMINANT_REMOVED_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(0);
			});
			if (bottleInv.childItem != null && !WaterHelper.IsEmpty(bottleInv.childItem))
			{
				int machinePowerUsage = MachineryHelper.GetMachinePowerUsage(item);
				if (powerInv.childItem != null && PowerHelper.CanDrawPowerSource(powerInv.childItem, machinePowerUsage))
				{
					int removedAmount = PurifyContainer(item, bottleInv.childItem);
					if (removedAmount > 0)
					{
						PowerHelper.DrawPowerSource(powerInv.childItem, machinePowerUsage);
						item.ModifyTag("TOTAL_CONTAMINANT_REMOVED_TAG", delegate(TagState state)
						{
							state.Enable().SetInt(removedAmount);
						});
						MachineHelper.OnMachineActioned(machine, itemInventoryModule);
					}
				}
			}
		};
		bottleInv.onSlotAddItemEndFunc = delegate
		{
			machine.SetSpriteAndShape("Items/items_tool2", "purifier_w");
		};
		bottleInv.onSlotRemoveItemEndFunc = delegate
		{
			machine.SetSpriteAndShape("Items/items_tool2", "purifier");
		};
		return machine;
	}

	public static int PurifyContainer(GameItem machine, GameItem waterContainer, bool ignoreBonus = false)
	{
		if (!waterContainer.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return 0;
		}
		Debug.Log("Purifying water container");
		int num = MachineryHelper.GetCurrentPerformanceBonus(machine);
		int bonusQuality = MachineryHelper.GetCurrentQualityBonus(machine);
		int totalVolume = WaterHelper.GetTotalVolume(waterContainer);
		if (ignoreBonus)
		{
			num = 0;
			bonusQuality = 0;
		}
		double dirtBonus = GetDirtBonus(WaterHelper.GetWaterPurity(waterContainer));
		dirtBonus += (double)num / 100.0;
		if (dirtBonus < 0.0)
		{
			dirtBonus = 0.0;
		}
		dirtBonus *= 1.25;
		Debug.Log("Amount multiplier: " + dirtBonus);
		long budget = (long)((double)((long)Math.Min(totalVolume, 10000000) * 3L / 1000 + 25000) * dirtBonus);
		int result = RemoveLargestFirst(waterContainer, GetMaxPurityCap(bonusQuality), budget);
		WaterFeatureHelper.UpdateWaterFeature(waterContainer, isExpose: false);
		return result;
	}

	public static (PixelWindow, GameSlotInventory, GameInventory, GameSlotInventory, GameSlotInventory) CreateMachineInventoryWindow(bool isDraggable = true, bool isCentered = true, int minWidth = 5)
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameSlotInventory gameSlotInventory2 = new GameSlotInventory();
		GameGridInventory gameGridInventory = new GameGridInventory("10000010001000001110000010001000001", 7);
		GameSlotInventory gameSlotInventory3 = new GameSlotInventory();
		return (MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.PowerPanel(gameSlotInventory2), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_purifier_label_module"), gameGridInventory, "module"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_purifier_label_container"), gameSlotInventory3, "container"), MachineHelper.NotePanel(gameSlotInventory)), gameSlotInventory2, gameGridInventory, gameSlotInventory3, gameSlotInventory);
	}

	public static GameItem CreateNote()
	{
		GameItem gameItem = DirectoryMaster.Item("postit");
		gameItem.name = LocHelper.GetLocalizedItem("item_water_purifier_instruction_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_water_purifier_instruction_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_water_purifier_instruction_short");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.unitValue = 0L;
		return gameItem;
	}

	public static double GetPurityMult(int bonusQuality)
	{
		if (bonusQuality >= 200)
		{
			return 0.0;
		}
		if (bonusQuality >= 100)
		{
			return 0.1;
		}
		if (bonusQuality >= 75)
		{
			return 0.2;
		}
		if (bonusQuality >= 50)
		{
			return 0.35;
		}
		if (bonusQuality >= 25)
		{
			return 0.65;
		}
		if (bonusQuality >= 15)
		{
			return 0.85;
		}
		return 1.0;
	}

	public static double GetDirtBonus(int purity)
	{
		if (purity >= DIRT_BONUS_CURVE[0].purity)
		{
			return DIRT_BONUS_CURVE[0].bonus;
		}
		for (int i = 1; i < DIRT_BONUS_CURVE.Length; i++)
		{
			var (num, num2) = DIRT_BONUS_CURVE[i - 1];
			var (num3, num4) = DIRT_BONUS_CURVE[i];
			if (purity >= num3)
			{
				double num5 = (double)(num - purity) / (double)(num - num3);
				return num2 + num5 * (num4 - num2);
			}
		}
		return DIRT_BONUS_CURVE[DIRT_BONUS_CURVE.Length - 1].bonus;
	}

	public static int GetMaxPurityCap(int bonusQuality)
	{
		return 10000 - (int)Math.Round(99.0 * GetPurityMult(bonusQuality));
	}

	private static int RemoveLargestFirst(GameItem container, int cap, long budget)
	{
		int totalVolume = WaterHelper.GetTotalVolume(container);
		if (totalVolume <= 0)
		{
			return 0;
		}
		string currentPartFromId = Liquid.GetCurrentPartFromId("water");
		int water = container.GetTagReadonly(currentPartFromId).GetInt();
		if (water <= 0)
		{
			return 0;
		}
		long num = (long)totalVolume * (long)(10000 - cap) / 10000;
		long num2 = Math.Min(budget, totalVolume - water - num);
		if (num2 <= 0)
		{
			return 0;
		}
		SortedDictionary<int, List<Liquid>> sortedDictionary = new SortedDictionary<int, List<Liquid>>();
		foreach (Liquid liquid in Liquid.Liquids)
		{
			if (!(liquid.identifier == "water") && liquid.canBeRemovedByPurifier && container.GetTagReadonly(liquid.currentPart).GetInt() > 0)
			{
				int key = (REMOVED_LAST.Contains(liquid.identifier) ? 1000000 : 0) - liquid.particleSize;
				if (!sortedDictionary.TryGetValue(key, out var value))
				{
					value = (sortedDictionary[key] = new List<Liquid>());
				}
				value.Add(liquid);
			}
		}
		long removed = 0L;
		foreach (List<Liquid> value2 in sortedDictionary.Values)
		{
			if (num2 <= 0)
			{
				break;
			}
			long num3 = 0L;
			foreach (Liquid item in value2)
			{
				num3 += container.GetTagReadonly(item.currentPart).GetInt();
			}
			long num4 = Math.Min(num3, num2);
			long num5 = 0L;
			for (int i = 0; i < value2.Count; i++)
			{
				int @int = container.GetTagReadonly(value2[i].currentPart).GetInt();
				long val = ((i == value2.Count - 1) ? (num4 - num5) : (@int * num4 / num3));
				val = Math.Min(val, @int);
				num5 += val;
				int left = @int - (int)val;
				container.ModifyTag(value2[i].currentPart, delegate(TagState state)
				{
					state.Enable().SetInt(left);
				});
			}
			removed += num5;
			num2 -= num5;
		}
		container.ModifyTag(currentPartFromId, delegate(TagState state)
		{
			state.Enable().SetInt(water + (int)removed);
		});
		return (int)removed;
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.identifier.Equals("water_purifier") && !item.IsTag("BROKEN_MACHINE_TAG"))
		{
			int @int = item.GetTagReadonly("TOTAL_CONTAMINANT_REMOVED_TAG").GetInt();
			if (@int != 0)
			{
				float num = (float)@int / 1000f;
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_purifier_tooltip_removed", num.ToString("0.#")), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
			string text = ((double)GetMaxPurityCap(MachineryHelper.GetCurrentQualityBonus(item)) / 100.0).ToString("0.##");
			builder.AddLine(LocHelper.GetLocalizedMechanicOrDefault("mech_purifier_tooltip_max_purity", "Max Purity: {0}%", text), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
	}
}
