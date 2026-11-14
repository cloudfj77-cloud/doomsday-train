using UnityEngine;

public class MachineAlarm
{
	public const string TRANSMITTER_TARGET_TAG = "TRANSMITTER_TARGET_TAG";

	public static GameItem Alarme()
	{
		(PixelWindow, GameSlotInventory, GameInventory, GameSlotInventory) tuple = CreateMachineInventoryWindow();
		PixelWindow item2 = tuple.Item1;
		GameSlotInventory itemInventory = tuple.Item2;
		GameInventory itemInventoryModule = tuple.Item3;
		GameSlotInventory item3 = tuple.Item4;
		item3.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => item.identifier.Equals("postit");
		item3.SetBackgroundFadeSprite("Items/items_tool", "postit");
		GameItem machine = ItemDirectory.CreateEmptyItem().SetContentWindow(item2).SetSpriteAndShape("Items/items_tool", "alarm_system");
		machine.name = LocHelper.GetLocalizedItem("item_security_alarm_name");
		machine.flavorText = LocHelper.GetLocalizedItem("item_security_alarm_flavor");
		machine.shortDescription = LocHelper.GetLocalizedItem("item_security_alarm_desc");
		machine.unitValue = 150L;
		MachineHelper.SetupBatterySlot(itemInventory, machine);
		machine.EnableTag("CONTAINER_TAG");
		machine.EnableTag("manufacture");
		machine.EnableTag("STANDARD_MACHINE_TAG");
		machine.SetGameItemType("MACHINE");
		MachineryHelper.InitMachinery(machine, 4);
		string[] tags = new string[2] { "MODULE_TYPE_UNIVERSAL", "MODULE_TYPE_ALARM" };
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
		machine.onCycleEndEarlySlotItemFunc = delegate(GameItem item, GameInventory parentInventory, SlotMarker slot)
		{
			machine.DisableTag("POWERED");
			int machinePowerUsage2 = MachineryHelper.GetMachinePowerUsage(item);
			if (itemInventory.childItems.Count != 0 && PowerHelper.DrawPowerSource(itemInventory.childItem, machinePowerUsage2))
			{
				PlayerStore.Instance.isSecurityAlarmActive = true;
				machine.EnableTag("POWERED");
				if (PowerHelper.CanDrawPowerSource(itemInventory.childItem, machinePowerUsage2))
				{
					machine.SetSpriteAndShape("Items/items_tool", "alarm_system_r");
				}
				else
				{
					machine.SetSpriteAndShape("Items/items_tool", "alarm_system");
				}
			}
			else
			{
				machine.SetSpriteAndShape("Items/items_tool", "alarm_system");
				if (CurrentConverterHelper.isNightCycle)
				{
					NightLog.AlarmNoPower(machinePowerUsage2);
				}
			}
			MachineHelper.OnMachineActioned(machine, itemInventoryModule);
		};
		machine.onCycleEndSlotItemFunc = delegate
		{
		};
		itemInventory.onSlotAddItemEndFunc = delegate(GameItem item, GameInventory parentInventory, SlotMarker slot)
		{
			int machinePowerUsage = MachineryHelper.GetMachinePowerUsage(item);
			if (PowerHelper.CanDrawPowerSource(itemInventory.childItem, machinePowerUsage))
			{
				machine.SetSpriteAndShape("Items/items_tool", "alarm_system_r");
			}
			else
			{
				machine.SetSpriteAndShape("Items/items_tool", "alarm_system");
			}
		};
		itemInventory.onSlotRemoveItemEndFunc = delegate
		{
			machine.SetSpriteAndShape("Items/items_tool", "alarm_system");
		};
		return machine;
	}

	public static (PixelWindow, GameSlotInventory, GameInventory, GameSlotInventory) CreateMachineInventoryWindow(bool isDraggable = true, bool isCentered = true, int minWidth = 150)
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameGridInventory gameGridInventory = new GameGridInventory("10000010001000001110000010001000001", 7);
		GameSlotInventory gameSlotInventory2 = new GameSlotInventory();
		return (MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.PowerPanel(gameSlotInventory), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_alarm_label_module"), gameGridInventory, "module"), MachineHelper.NotePanel(gameSlotInventory2)), gameSlotInventory, gameGridInventory, gameSlotInventory2);
	}

	public static GameItem FindAlarmeItem()
	{
		foreach (GameItem item in PlayerStore.Instance.FindAllItem())
		{
			if (item.identifier.Equals("security_alarm") && !item.IsTag("BROKEN_MACHINE_TAG"))
			{
				return item;
			}
		}
		return null;
	}

	public static int GetAlarmePerformance(GameItem gameItem = null)
	{
		if (gameItem == null)
		{
			GameItem gameItem2 = FindAlarmeItem();
			if (gameItem2 == null)
			{
				return 0;
			}
			return MachineryHelper.GetCurrentPerformanceBonus(gameItem2);
		}
		return MachineryHelper.GetCurrentPerformanceBonus(gameItem);
	}

	public static int GetAlarmeStopRate(GameItem gameItem = null)
	{
		if (gameItem == null)
		{
			GameItem gameItem2 = FindAlarmeItem();
			if (gameItem2 == null)
			{
				Debug.Log("<b>Alarm not found in inventory.</b>");
				return 0;
			}
			Debug.Log((GetAlarmePerformance(gameItem2) + 50).ToString() ?? "");
			return Mathf.Max(0, GetAlarmePerformance(gameItem2) + 50);
		}
		return Mathf.Max(0, GetAlarmePerformance(gameItem) + 50);
	}

	public static void CreateAlarmTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.identifier.Equals("security_alarm") && !item.IsTag("BROKEN_MACHINE_TAG"))
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_alarm_theft_prevention_rate", GetAlarmeStopRate(item)), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
	}

	public static GameItem CreateNote()
	{
		GameItem gameItem = DirectoryMaster.Item("postit");
		gameItem.name = LocHelper.GetLocalizedItem("item_alarm_instruction_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_alarm_instruction_flavor");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.unitValue = 0L;
		return gameItem;
	}

	public static void ToggleTransmitter(GameItem transmitterModule)
	{
		string @string = transmitterModule.GetTagReadonly("TRANSMITTER_TARGET_TAG").GetString();
		string newTarget = "";
		string localizedMechanic = LocHelper.GetLocalizedMechanic("mech_transmitter_postfix");
		if (@string.Equals("FACTION_REVOLUTION"))
		{
			newTarget = "FACTION_SECURITY";
			transmitterModule.shortDescription = LocHelper.GetLocalizedMechanic("mech_transmitter_desc_security") + localizedMechanic;
		}
		else if (@string.Equals("FACTION_SECURITY"))
		{
			newTarget = "FACTION_BLACK_MARKET";
			transmitterModule.shortDescription = LocHelper.GetLocalizedMechanic("mech_transmitter_desc_blackmarket") + localizedMechanic;
		}
		else if (@string.Equals("FACTION_BLACK_MARKET"))
		{
			newTarget = "FACTION_UPPER_LEVEL";
			transmitterModule.shortDescription = LocHelper.GetLocalizedMechanic("mech_transmitter_desc_upperlevel") + localizedMechanic;
		}
		else if (@string.Equals("FACTION_UPPER_LEVEL"))
		{
			newTarget = "FACTION_REVOLUTION";
			transmitterModule.shortDescription = LocHelper.GetLocalizedMechanic("mech_transmitter_desc_revolution") + localizedMechanic;
		}
		transmitterModule.ModifyTag("TRANSMITTER_TARGET_TAG", delegate(TagState state)
		{
			state.SetString(newTarget);
		});
	}

	public static void Operate(GameItem alarm)
	{
		if (alarm == null)
		{
			return;
		}
		GameGridInventory moduleInv = MachineHelper.GetModuleInv(alarm);
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		foreach (GameItem childItem in moduleInv.childItems)
		{
			if (childItem.identifier.Equals("alarm_module_transmitter"))
			{
				string @string = childItem.GetTagReadonly("TRANSMITTER_TARGET_TAG").GetString();
				if (@string.Equals("FACTION_REVOLUTION"))
				{
					flag2 = true;
				}
				else if (@string.Equals("FACTION_SECURITY"))
				{
					flag = true;
				}
				else if (@string.Equals("FACTION_BLACK_MARKET"))
				{
					flag4 = true;
				}
				else if (@string.Equals("FACTION_UPPER_LEVEL"))
				{
					flag3 = true;
				}
			}
		}
		if (flag2)
		{
			StoreReputation.GetRevFaction().ModReputation(1.0);
		}
		if (flag)
		{
			StoreReputation.GetSecFaction().ModReputation(1.0);
		}
		if (flag4)
		{
			StoreReputation.GetBMFaction().ModReputation(1.0);
		}
		if (flag3)
		{
			StoreReputation.GetULFaction().ModReputation(1.0);
		}
	}
}
