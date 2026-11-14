using System;
using System.Collections.Generic;

public class MachineEvaporator
{
	public static GameItem Evaporator()
	{
		(PixelWindow, GameSlotInventory, GameGridInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory) tuple = CreateMachineInventoryWindow(8, 8);
		PixelWindow item2 = tuple.Item1;
		GameSlotInventory itemInventoryBattery = tuple.Item2;
		GameGridInventory itemInventoryModule = tuple.Item3;
		GameSlotInventory itemInventoryContainer = tuple.Item4;
		GameSlotInventory distillateSlot = tuple.Item5;
		GameSlotInventory item3 = tuple.Item6;
		GameItem machine = ItemDirectory.CreateEmptyItem().SetContentWindow(item2).SetSpriteAndShape("Items/items_tool2", "evaporator");
		machine.name = LocHelper.GetLocalizedItem("item_evaporator_name");
		machine.shortDescription = LocHelper.GetLocalizedItem("item_evaporator_desc");
		machine.unitValue = 300L;
		machine.SetGameItemType("MACHINE");
		machine.EnableTag("CONTAINER_TAG");
		machine.EnableTag("TURBO_BOOST_ELIGIBLE_TAG");
		AudioHelper.InitMetalMachine(machine);
		SetupWaterSlot(itemInventoryContainer);
		SetupWaterSlot(distillateSlot);
		distillateSlot.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		itemInventoryContainer.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 3;
		itemInventoryContainer.onSlotAddItemEndFunc = delegate
		{
			machine.SetSpriteAndShape("Items/items_tool2", "evaporator_b");
		};
		itemInventoryContainer.onSlotRemoveItemEndFunc = delegate
		{
			machine.SetSpriteAndShape("Items/items_tool2", "evaporator");
		};
		MachineHelper.SetupBatterySlot(itemInventoryBattery, machine);
		string[] tags = new string[2] { "MODULE_TYPE_UNIVERSAL", "MODULE_TYPE_EVAPORATOR" };
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
		item3.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => item.identifier.Equals("postit");
		item3.SetBackgroundFadeSprite("Items/items_tool", "postit");
		MachineryHelper.InitMachinery(machine, 6);
		machine.onCycleEndSlotItemFunc = delegate(GameItem item, GameInventory parentInventory, SlotMarker slot)
		{
			int machinePowerUsage = MachineryHelper.GetMachinePowerUsage(item);
			if (itemInventoryBattery.childItems.Count != 0 && CanEvaporateContainer(itemInventoryContainer.childItem) && PowerHelper.DrawPowerSource(itemInventoryBattery.childItem, machinePowerUsage))
			{
				int num = 300000;
				int currentPerformanceBonus = MachineryHelper.GetCurrentPerformanceBonus(machine);
				int currentQualityBonus = MachineryHelper.GetCurrentQualityBonus(machine);
				currentQualityBonus = Math.Clamp(currentQualityBonus, 0, 100);
				float num2 = 1f + (float)currentPerformanceBonus / 100f;
				if (num2 < 0f)
				{
					num2 = 0f;
				}
				num = (int)Math.Round((float)num * num2);
				int num3 = 0;
				foreach (Liquid liquid in Liquid.Liquids)
				{
					if (liquid.isEvaporative)
					{
						num3 += itemInventoryContainer.childItem.GetTagReadonly(Liquid.GetCurrentPartFromId(liquid.identifier)).GetInt();
					}
				}
				if (num3 <= 0)
				{
					Debug.Log("DSERROR: CanEvaporateContainer() should not let this log happen.");
				}
				else
				{
					if (num3 < num)
					{
						num = num3;
					}
					List<GameItem> modules = MachineHelper.GetModules(machine);
					bool flag = false;
					foreach (GameItem item4 in modules)
					{
						if (item4.identifier.Equals("evaporator_module_solvent_recovery"))
						{
							flag = true;
						}
					}
					int num4 = 0;
					int num5 = 0;
					foreach (Liquid liquid2 in Liquid.Liquids)
					{
						if (liquid2.isEvaporative && itemInventoryContainer.childItem.GetTagReadonly(Liquid.GetCurrentPartFromId(liquid2.identifier)).GetInt() > 0)
						{
							num5++;
						}
					}
					foreach (Liquid liquid3 in Liquid.Liquids)
					{
						if (liquid3.isEvaporative)
						{
							int @int = itemInventoryContainer.childItem.GetTagReadonly(Liquid.GetCurrentPartFromId(liquid3.identifier)).GetInt();
							if (@int > 0)
							{
								num5--;
								int value = (int)((num5 == 0) ? (num - num4) : ((long)num * (long)@int / num3));
								value = Math.Clamp(value, 0, @int);
								num4 += value;
								int newAmount = @int - value;
								itemInventoryContainer.childItem.ModifyTag(Liquid.GetCurrentPartFromId(liquid3.identifier), delegate(TagState state)
								{
									state.Enable().SetInt(newAmount);
								});
								int num6 = (int)Math.Round((double)value * ((double)currentQualityBonus / 100.0));
								if (flag && !liquid3.IsLiquid("alcohol"))
								{
									num6 = 0;
								}
								if (num6 > 0 && distillateSlot.childItem != null)
								{
									distillateSlot.childItem.GetTagReadonly(Liquid.GetCurrentPartFromId(liquid3.identifier)).GetInt();
									WaterHelper.AddLiquid(distillateSlot.childItem, liquid3.identifier, num6);
									WaterFeatureHelper.OnLiquidChanged(distillateSlot.childItem, exposeFeature: true);
								}
							}
						}
					}
					WaterFeatureHelper.OnLiquidContainerPourOutOf(itemInventoryContainer.childItem);
					SpriteHelper.UpdateWaterContainerSprite(itemInventoryContainer.childItem);
					MachineHelper.OnMachineActioned(machine, itemInventoryModule);
				}
			}
		};
		return machine;
	}

	private static void SetupWaterSlot(GameSlotInventory waterSlot)
	{
		waterSlot.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(item) && item.IsTag("LIQUID_CONTAINER_TAG");
		waterSlot.SetBackgroundFadeSprite("Items/water_container4", "xl");
	}

	public static GameItem CreateNote()
	{
		GameItem gameItem = DirectoryMaster.Item("postit");
		gameItem.name = LocHelper.GetLocalizedItem("item_evaporator_instruction_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_evaporator_instruction_flavor");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.unitValue = 0L;
		return gameItem;
	}

	public static bool CanEvaporateContainer(GameItem container)
	{
		if (container == null)
		{
			return false;
		}
		if (WaterHelper.IsEmpty(container))
		{
			return false;
		}
		int num = 0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			if (liquid.isEvaporative)
			{
				num += container.GetTagReadonly(Liquid.GetCurrentPartFromId(liquid.identifier)).GetInt();
			}
		}
		return num > 0;
	}

	public static (PixelWindow, GameSlotInventory, GameGridInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory) CreateMachineInventoryWindow(int width, int height, bool isDraggable = true, bool isCentered = true, int minWidth = 5)
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameGridInventory gameGridInventory = new GameGridInventory(4, 4);
		GameSlotInventory gameSlotInventory2 = new GameSlotInventory();
		GameSlotInventory gameSlotInventory3 = new GameSlotInventory();
		GameSlotInventory gameSlotInventory4 = new GameSlotInventory();
		return (MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.PowerPanel(gameSlotInventory), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_evaporator_label_module"), gameGridInventory, "module"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_evaporator_label_container"), gameSlotInventory2, "container"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_evaporator_label_distillate"), gameSlotInventory3, "distillate"), MachineHelper.NotePanel(gameSlotInventory4)), gameSlotInventory, gameGridInventory, gameSlotInventory2, gameSlotInventory3, gameSlotInventory4);
	}
}
