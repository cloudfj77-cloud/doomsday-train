using System;
using System.Collections.Generic;
using UnityEngine;

public class MachineWineRack
{
	public const int AGE_WELL_BASE_DAYS = 1;

	public const int AGE_WELL_QUALITY_PER_EXTRA_DAY = 125;

	private static readonly Vector2[] WINE_RACK_BOTTLE_OFFSET = new Vector2[6]
	{
		new Vector2(15f, -19f),
		new Vector2(27f, -19f),
		new Vector2(15f, -27f),
		new Vector2(27f, -27f),
		new Vector2(15f, -36f),
		new Vector2(27f, -36f)
	};

	public static GameItem AgeWell()
	{
		(PixelWindow, GameSlotInventory, GameGridInventory, GameGridInventory, GameSlotInventory) tuple = CreateMachineInventoryWindow();
		var (contentWindow, itemInventoryBattery, itemInventoryModule, wineSlot, _) = tuple;
		MachineHelper.SetupManualSlot(tuple.Item5);
		SetupWineSlot(wineSlot);
		GameItem machine = ItemDirectory.CreateEmptyItem().SetContentWindow(contentWindow).SetSpriteAndShape("Items/items_winemaking", "wine_rack");
		machine.name = LocHelper.GetLocalizedItem("item_age_well_name");
		machine.shortDescription = LocHelper.GetLocalizedItem("item_age_well_desc");
		machine.unitValue = 350L;
		machine.SetGameItemType("MACHINE");
		machine.EnableTag("CONTAINER_TAG");
		machine.EnableTag("STANDARD_MACHINE_TAG");
		AudioHelper.InitMetalMachine(machine);
		MachineHelper.SetupBatterySlot(itemInventoryBattery, machine);
		machine.ModifyTag("MACHINE_EFFICIENCY_RATIO_INT", delegate(TagState state)
		{
			state.Enable().SetInt(2);
		});
		machine.ModifyTag("MACHINE_PERFORMANCE_RATIO_INT", delegate(TagState state)
		{
			state.Enable().SetInt(2);
		});
		machine.EnableTag("MACHINE_PERFORMANCE_REDUCES_POWER_TAG");
		MachineryHelper.InitMachinery(machine, 24);
		MachineHelper.SetupModuleBay(itemInventoryModule, machine, delegate
		{
			machine.Validate();
		});
		wineSlot.onSlotAddItemLateFunc = delegate
		{
			machine.Validate();
		};
		wineSlot.onSlotRemoveItemLateFunc = delegate
		{
			machine.Validate();
		};
		machine.onCycleEndSlotItemFunc = delegate
		{
			List<GameItem> list2 = GraphUtils.FindAllChildrenType(wineSlot, (GameItem w) => CanAgeWine(w));
			if (list2.Count != 0 && MachineHelper.TryDrawCyclePower(machine, itemInventoryBattery))
			{
				int agingDays = GetAgingDays(machine);
				foreach (GameItem item in list2)
				{
					for (int i = 0; i < agingDays; i++)
					{
						WineHelper.OnAgeWine(item);
					}
				}
				MachineHelper.OnMachineActioned(machine, itemInventoryModule);
			}
		};
		machine.getItemSpriteModifierForItemFunc = delegate
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			int num = 0;
			foreach (GameItem childItem in wineSlot.childItems)
			{
				_ = childItem;
				if (num >= WINE_RACK_BOTTLE_OFFSET.Length)
				{
					break;
				}
				list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_winemaking", "wine_bottom"), WINE_RACK_BOTTLE_OFFSET[num], 0, isFlipped: false, 0, haveMask: false, Color.white));
				num++;
			}
			return list;
		};
		return machine;
	}

	public static int GetAgingDays(GameItem machine)
	{
		int currentQualityBonus = MachineryHelper.GetCurrentQualityBonus(machine);
		return 1 + Math.Max(0, currentQualityBonus) / 125;
	}

	public static bool CanAgeWine(GameItem wine)
	{
		if (wine == null)
		{
			return false;
		}
		if (!wine.IsTag("WINE_CONTAINER_TAG"))
		{
			return false;
		}
		if (wine.IsTag("WINE_COUNTERFEIT_TAG"))
		{
			return false;
		}
		return wine.GetTagReadonly("WINE_CONTAINER_STATE").GetInt() == 2;
	}

	private static void SetupWineSlot(GameGridInventory wineSlot)
	{
		wineSlot.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(item) && CanAgeWine(item);
	}

	public static (PixelWindow, GameSlotInventory, GameGridInventory, GameGridInventory, GameSlotInventory) CreateMachineInventoryWindow(bool isDraggable = true, bool isCentered = true, int minWidth = 20)
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameGridInventory gameGridInventory = new GameGridInventory("10000010001000001110000010001000001", 7);
		GameGridInventory gameGridInventory2 = new GameGridInventory(6, 4);
		GameSlotInventory gameSlotInventory2 = new GameSlotInventory();
		return (MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.PowerPanel(gameSlotInventory), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_purifier_label_module"), gameGridInventory, "module"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_evaporator_label_container"), gameGridInventory2, "wine"), MachineHelper.NotePanel(gameSlotInventory2)), gameSlotInventory, gameGridInventory, gameGridInventory2, gameSlotInventory2);
	}

	public static GameItem CreateNote()
	{
		return MachineHelper.CreateManual("item_age_well_instruction_name", "item_age_well_instruction_flavor");
	}
}
