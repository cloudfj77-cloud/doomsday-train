using System;

public class MachineDesequencer
{
	public const string DESEQUENCER_STATE_TAG = "DESEQUENCER_STATE_TAG";

	public const string STATE_DESEQUENCING = "STATE_DESEQUENCING";

	public const string STATE_READY = "STATE_READY";

	public const string DESEQUENCER_PROGRESS_TAG = "DESEQUQUENCER_PROGRESS_TAG";

	public const string DESEQUENCER_TARGET_ID_TAG = "DESEQUENCER_TARGET_ID_TAG";

	public static GameItem Desequencer()
	{
		var (contentWindow, powerInv, itemInventoryModule, chipSlot, inputSlot, outputSlot) = CreateMachineInventoryWindow();
		chipSlot.mayInventoryAddItemFunc = delegate(GameItem item, GameInventory _)
		{
			if (!GeneralHelper.IsItemOwned(item))
			{
				return false;
			}
			return !item.IsGameItemType("MACHINE") && item.IsTag("DESEQUENCER_TARGET_ID_TAG");
		};
		chipSlot.SetBackgroundFadeSprite("Items/items_tool", "sec_module");
		inputSlot.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(item) && item.IsGameItemType("ACCESS_CARD");
		inputSlot.SetBackgroundFadeSprite("Items/items_misc", "blank_card");
		outputSlot.mayInventoryAddItemFunc = delegate(GameItem item, GameInventory _)
		{
			if (!GeneralHelper.IsItemOwned(item))
			{
				return false;
			}
			return !item.identifier.Equals("blank_keycard") && item.IsGameItemType("ACCESS_CARD");
		};
		outputSlot.SetBackgroundFadeSprite("Items/items_misc", "sec_keycard");
		GameSlotInventory gameSlotInventory = chipSlot;
		gameSlotInventory.onSlotAddItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameSlotInventory.onSlotAddItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
		{
			AudioManager.Instance.Play3("module_insert");
		});
		GameSlotInventory gameSlotInventory2 = chipSlot;
		gameSlotInventory2.onSlotRemoveItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameSlotInventory2.onSlotRemoveItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
		{
			AudioManager.Instance.Play3("module_remove");
		});
		GameItem machine = ItemDirectory.CreateEmptyItem().SetContentWindow(contentWindow).SetSpriteAndShape("Items/items_tool", "desequencer");
		machine.name = LocHelper.GetLocalizedItem("item_desequencer_name");
		machine.flavorText = LocHelper.GetLocalizedItem("item_desequencer_flavor");
		machine.shortDescription = LocHelper.GetLocalizedItem("item_desequencer_desc");
		machine.unitValue = 350L;
		MachineHelper.SetupBatterySlot(powerInv, machine);
		machine.SetGameItemType("MACHINE");
		machine.EnableTag("CONTAINER_TAG");
		machine.EnableTag("manufacture");
		machine.EnableTag("STANDARD_MACHINE_TAG");
		ContrabandHelper.InitContrabandItem(machine, 2);
		machine.ModifyTag("DESEQUENCER_STATE_TAG", delegate(TagState state)
		{
			state.Enable().SetString("STATE_READY");
		});
		machine.ModifyTag("DESEQUQUENCER_PROGRESS_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		MachineryHelper.InitMachinery(machine, 10);
		MachineProgressHelper.InitProgressTypeMachine(machine, 33, useDefaultTooltip: false);
		machine.onCycleEndSlotItemFunc = delegate(GameItem item, GameInventory parentInventory, SlotMarker slot)
		{
			int machinePowerUsage = MachineryHelper.GetMachinePowerUsage(item);
			if (powerInv.childItem != null && PowerHelper.CanDrawPowerSource(powerInv.childItem, machinePowerUsage))
			{
				if (item.GetTagReadonly("DESEQUENCER_STATE_TAG").GetString().Equals("STATE_DESEQUENCING"))
				{
					if (powerInv.childItem != null && outputSlot.childItem == null)
					{
						PowerHelper.DrawPowerSource(powerInv.childItem, machinePowerUsage);
						MachineHelper.OnMachineActioned(machine, itemInventoryModule);
						MachineProgressHelper.ContinueProgressTypeMachine(item);
						if (MachineProgressHelper.IsProgressTypeMachineFinished(item))
						{
							FinishDesequencer(item, inputSlot, outputSlot);
						}
						else
						{
							var (currentProgress2, num) = MachineProgressHelper.GetProgress(item);
							item.ModifyTag("DESEQUQUENCER_PROGRESS_TAG", delegate(TagState state)
							{
								state.Enable().SetInt(currentProgress2);
							});
							inputSlot.childItem.shortDescription = LocHelper.GetLocalizedMechanic("mech_desequencer_progress", currentProgress2, num);
						}
					}
				}
				else if (item.GetTagReadonly("DESEQUENCER_STATE_TAG").GetString().Equals("STATE_READY") && inputSlot.childItem != null && chipSlot.childItem != null && inputSlot.childItem.IsGameItemType("ACCESS_CARD") && powerInv.childItem != null && outputSlot.childItem == null)
				{
					PowerHelper.DrawPowerSource(powerInv.childItem, machinePowerUsage);
					MachineHelper.OnMachineActioned(machine, itemInventoryModule);
					item.ModifyTag("DESEQUENCER_STATE_TAG", delegate(TagState state)
					{
						state.Enable().SetString("STATE_DESEQUENCING");
					});
					string targetItemId = chipSlot.childItem.GetTagReadonly("DESEQUENCER_TARGET_ID_TAG").GetString();
					item.ModifyTag("DESEQUENCER_TARGET_ID_TAG", delegate(TagState state)
					{
						state.Enable().SetString(targetItemId);
					});
					MachineProgressHelper.StartProgressTypeMachine(item, chipSlot.childItem);
					MachineProgressHelper.ContinueProgressTypeMachine(item);
					var (currentProgress, num2) = MachineProgressHelper.GetProgress(item);
					Debug.Log("Current progress: " + currentProgress + " Target progress: " + num2);
					item.ModifyTag("DESEQUQUENCER_PROGRESS_TAG", delegate(TagState state)
					{
						state.Enable().SetInt(currentProgress);
					});
					inputSlot.childItem.SetSpriteAndShape("Items/items_misc", "blank_card_wip");
					GeneralHelper.LockItem(inputSlot.childItem);
					inputSlot.childItem.shortDescription = LocHelper.GetLocalizedMechanic("mech_desequencer_progress", currentProgress, num2);
					inputSlot.childItem.flavorText = "";
					inputSlot.childItem.name = LocHelper.GetLocalizedItem("item_modified_access_card_name");
					if (MachineProgressHelper.IsProgressTypeMachineFinished(item))
					{
						FinishDesequencer(item, inputSlot, outputSlot);
					}
				}
			}
		};
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
		powerInv.onSlotAddItemEndFunc = delegate
		{
			if (powerInv.childItem != null && PowerHelper.CanDrawPowerSource(powerInv.childItem, 10))
			{
				machine.SetSpriteAndShape("Items/items_tool", "desequencer_r");
			}
			else
			{
				machine.SetSpriteAndShape("Items/items_tool", "desequencer");
			}
		};
		powerInv.onSlotRemoveItemEndFunc = delegate
		{
			machine.SetSpriteAndShape("Items/items_tool", "desequencer");
		};
		return machine;
	}

	public static void FinishDesequencer(GameItem item, GameSlotInventory inputSlot, GameSlotInventory outputSlot)
	{
		item.ModifyTag("DESEQUENCER_STATE_TAG", delegate(TagState state)
		{
			state.Enable().SetString("STATE_READY");
		});
		item.ModifyTag("DESEQUQUENCER_PROGRESS_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		string @string = item.GetTagReadonly("DESEQUENCER_TARGET_ID_TAG").GetString();
		GraphUtils.EmporiumTryAdd(outputSlot, DirectoryMaster.Item(@string));
		inputSlot.childItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => true;
		inputSlot.childItem.Destroy();
	}

	public static void CreateMachineDesequencerTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("DESEQUENCER_STATE_TAG"))
		{
			string @string = item.GetTagReadonly("DESEQUENCER_STATE_TAG").GetString();
			if (@string.Equals("STATE_DESEQUENCING"))
			{
				var (num, num2) = MachineProgressHelper.GetProgress(item);
				builder.Add(LocHelper.GetLocalizedMechanic("mech_desequencer_status_working"), RenderHandler.ColorPalette.LightCyan);
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_desequencer_status_progress", num, num2), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
			else if (@string.Equals("STATE_READY"))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_desequencer_status_ready"), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
		}
	}

	public static (PixelWindow, GameSlotInventory, GameGridInventory, GameSlotInventory, GameSlotInventory, GameSlotInventory) CreateMachineInventoryWindow(bool isDraggable = true, bool isCentered = true, int minWidth = 50)
	{
		GameSlotInventory gameSlotInventory = new GameSlotInventory();
		GameGridInventory gameGridInventory = new GameGridInventory("10000010001000001110000010001000001", 7);
		GameSlotInventory gameSlotInventory2 = new GameSlotInventory();
		GameSlotInventory gameSlotInventory3 = new GameSlotInventory();
		GameSlotInventory gameSlotInventory4 = new GameSlotInventory();
		PixelWindow item = MachineHelper.CreateMachineryWindow(isDraggable, MachineHelper.PowerPanel(gameSlotInventory), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_desequencer_label_module"), gameGridInventory, "module"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_desequencer_label_chip"), gameSlotInventory2, "chip"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_desequencer_label_input"), gameSlotInventory3, "input"), MachineHelper.Panel(LocHelper.GetLocalizedMechanic("mech_desequencer_label_output"), gameSlotInventory4, "output"));
		gameSlotInventory3.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 2;
		gameSlotInventory4.slotPriorityFunc = (GameItem _, GameInventory _, SlotMarker _) => 1;
		return (item, gameSlotInventory, gameGridInventory, gameSlotInventory2, gameSlotInventory3, gameSlotInventory4);
	}

	public static GameItem CreateNote()
	{
		GameItem gameItem = DirectoryMaster.Item("postit");
		gameItem.name = LocHelper.GetLocalizedItem("item_desequencer_instruction_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_desequencer_instruction_flavor");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.unitValue = 0L;
		return gameItem;
	}
}
