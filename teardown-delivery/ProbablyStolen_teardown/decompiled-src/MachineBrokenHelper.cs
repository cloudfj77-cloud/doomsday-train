using System.Text.RegularExpressions;

public class MachineBrokenHelper
{
	public const string BROKEN_MACHINE_TAG = "BROKEN_MACHINE_TAG";

	public const string BROKEN_MACHINE_WIRE_COUNT = "BROKEN_MACHINE_WIRE_COUNT";

	public const string BROKEN_MACHINE_SCREW_COUNT = "BROKEN_MACHINE_SCREW_COUNT";

	public const string BROKEN_MACHINE_WELD_COUNT = "BROKEN_MACHINE_WELD_COUNT";

	public static void LoadBrokenMachine(GameItem gameItem)
	{
		if (gameItem.IsTag("BROKEN_MACHINE_TAG"))
		{
			if (gameItem.identifier.Equals("security_alarm"))
			{
				gameItem.SetSpriteAndShape("Items/items_broken_machine", "broken_alarm");
			}
			else if (gameItem.identifier.Equals("furnace"))
			{
				gameItem.SetSpriteAndShape("Items/items_broken_machine", "broken_furnace");
			}
			else if (gameItem.identifier.Equals("moisture_farm"))
			{
				gameItem.SetSpriteAndShape("Items/items_broken_machine", "broken_moisture_farm");
			}
			MachineHelper.HasBattery(gameItem);
			gameItem.onCycleEndSlotItemFunc = delegate
			{
			};
			gameItem.onCycleEndEarlySlotItemFunc = delegate
			{
			};
			MachineHelper.GetBatterySlot(gameItem).onSlotAddItemEndFunc = delegate
			{
			};
			MachineHelper.GetBatterySlot(gameItem).onSlotAddItemFunc = delegate
			{
			};
			MachineHelper.GetBatterySlot(gameItem).onSlotRemoveItemEndFunc = delegate
			{
			};
			MachineHelper.GetBatterySlot(gameItem).onSlotRemoveItemFunc = delegate
			{
			};
		}
	}

	public static void InitBrokenMachine(GameItem gameItem)
	{
		gameItem.onCycleEndSlotItemFunc = delegate
		{
		};
		gameItem.onCycleEndEarlySlotItemFunc = delegate
		{
		};
		MachineHelper.GetBatterySlot(gameItem).onSlotAddItemEndFunc = delegate
		{
		};
		MachineHelper.GetBatterySlot(gameItem).onSlotAddItemFunc = delegate
		{
		};
		MachineHelper.GetBatterySlot(gameItem).onSlotRemoveItemEndFunc = delegate
		{
		};
		MachineHelper.GetBatterySlot(gameItem).onSlotRemoveItemFunc = delegate
		{
			gameItem.SetSpriteAndShape(gameItem.spriteAtlasPath, Regex.Replace(gameItem.spritePath, "_b$", ""));
		};
		gameItem.EnableTag("BROKEN_MACHINE_TAG");
		gameItem.ModifyTag("BROKEN_MACHINE_WIRE_COUNT", delegate(TagState state)
		{
			state.Enable().SetInt(3);
		});
		gameItem.ModifyTag("BROKEN_MACHINE_SCREW_COUNT", delegate(TagState state)
		{
			state.Enable().SetInt(3);
		});
		gameItem.ModifyTag("BROKEN_MACHINE_WELD_COUNT", delegate(TagState state)
		{
			state.Enable().SetInt(4);
		});
	}

	public static void AddBrokenMachineItem(GameItem gameItem)
	{
		if (RNG.Roll(50))
		{
			GraphUtils.TryAcceptAll(gameItem, PreBuiltItemHelper.EnergyCredit(RNG.GetRandomInt(1, 25)));
		}
		else if (RNG.Roll(25))
		{
			GraphUtils.TryAcceptAll(gameItem, PreBuiltItemHelper.EnergyCredit(RNG.GetRandomInt(25, 50)));
		}
		ModuleHelper.AddStuckCorruptedModule(gameItem);
		if (RNG.Roll(50))
		{
			ModuleHelper.AddStuckCorruptedModule(gameItem);
		}
		if (RNG.Roll(25))
		{
			ModuleHelper.AddStuckCorruptedModule(gameItem);
		}
		if (RNG.Roll(75))
		{
			ModuleHelper.AddRandomStuckNormalModule(gameItem);
		}
		if (RNG.Roll(50))
		{
			ModuleHelper.AddRandomStuckNormalModule(gameItem);
		}
		if (RNG.Roll(25))
		{
			ModuleHelper.AddRandomStuckNormalModule(gameItem);
		}
		if (RNG.Roll(50))
		{
			ModuleHelper.AddStuckRuinedModule(gameItem);
		}
		if (RNG.Roll(50))
		{
			ModuleHelper.AddStuckRuinedModule(gameItem);
		}
	}

	public static void OnScrewdriverUsed(GameItem machine, GameItem tool)
	{
		if (machine.IsTag("BROKEN_MACHINE_TAG"))
		{
			int @int = machine.GetTagReadonly("BROKEN_MACHINE_SCREW_COUNT").GetInt();
			if (@int != 0)
			{
				machine.unitValue -= RNG.MultiplyAndRound((int)machine.backupUnitValue, 0.33);
			}
			for (int i = 0; i < @int; i++)
			{
				GraphUtils.EmporiumTryAdd(machine.parentInventory, DirectoryMaster.Item("nuts_metal"));
			}
			machine.ModifyTag("BROKEN_MACHINE_SCREW_COUNT", delegate(TagState state)
			{
				state.Enable().SetInt(0);
			});
			EmporiumAudioPlayer.Instance.ratchet.Play();
		}
	}

	public static void OnWireCutterUsed(GameItem machine, GameItem tool)
	{
		if (machine.IsTag("BROKEN_MACHINE_TAG"))
		{
			int @int = machine.GetTagReadonly("BROKEN_MACHINE_WIRE_COUNT").GetInt();
			if (@int != 0)
			{
				machine.unitValue -= RNG.MultiplyAndRound((int)machine.backupUnitValue, 0.33);
			}
			for (int i = 0; i < @int; i++)
			{
				GraphUtils.EmporiumTryAdd(machine.parentInventory, DirectoryMaster.Item("wire"));
			}
			machine.ModifyTag("BROKEN_MACHINE_WIRE_COUNT", delegate(TagState state)
			{
				state.Enable().SetInt(0);
			});
			EmporiumAudioPlayer.Instance.wirecutter.Play();
		}
	}

	public static void OnWelderUsed(GameItem machine, GameItem tool)
	{
		if (machine.IsTag("BROKEN_MACHINE_TAG"))
		{
			int @int = machine.GetTagReadonly("BROKEN_MACHINE_WELD_COUNT").GetInt();
			for (int i = 0; i < @int; i++)
			{
				GraphUtils.EmporiumTryAdd(machine.parentInventory, DirectoryMaster.Item("scrap_metal"));
			}
			machine.ModifyTag("BROKEN_MACHINE_WELD_COUNT", delegate(TagState state)
			{
				state.Enable().SetInt(0);
			});
			EmporiumAudioPlayer.Instance.welder.Play();
			machine.Destroy();
		}
	}
}
