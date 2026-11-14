using System;
using System.Collections.Generic;

public class PowerHelper
{
	public const string POWER_SOURCE_FURNITURE_TAG = "power_source_furniture";

	public const string POWER_SOURCE_ITEM_TAG = "power_source_item";

	public const string POWER_SOURCE_ITEM_BANDWIDTH = "power_source_item_bandwidth";

	public const string POWER_SOURCE_ITEM_ENERGY = "power_source_item_energy";

	public const string POWER_SOURCE_ITEM_MAX_ENERGY = "power_source_item_max_energy";

	public const string POWER_ANALYSER_TAG = "power_analyser";

	public const string POWER_UNIT = "kW";

	public const string ENERGY_UNIT = "kJ";

	public const int BREEDER_RECHARGE_RATE = 4;

	public const string BREEDER_POWER_SOURCE_ITEM_TAG = "BREEDER_POWER_SOURCE_ITEM_TAG";

	public const string POWER_SOURCE_ITEM_RECHARGE = "power_source_item_recharge";

	public const string POWER_SOURCE_ITEM_DISPOSABLE_TAG = "POWER_SOURCE_ITEM_DISPOSABLE_TAG";

	public const string POWER_SOURCE_ITEM_VALUE_PERCENT_INT = "POWER_SOURCE_ITEM_VALUE_PERCENT_INT";

	public static void SimulatePower(bool isFromShip = true)
	{
		int num = CheckAvailablePower(isFromShip);
		int num2 = CheckCurrentPowerUsage(isFromShip);
		List<GameItem> allPoweredOnMachinery = MachineryHelper.GetAllPoweredOnMachinery(isFromShip);
		if (num >= num2)
		{
			foreach (GameItem item in allPoweredOnMachinery)
			{
				item.ModifyTag("WILL_MACHINERY_RECEIVED_POWER", delegate(TagState state)
				{
					state.Enable().SetBool(val: true);
				});
			}
			return;
		}
		foreach (GameItem item2 in allPoweredOnMachinery)
		{
			num = TryToPowerMachinerySimulation(item2, num);
		}
	}

	public static void HandlePower(bool isFromShip = true)
	{
		List<GameItem> allPowerSourceItem = GetAllPowerSourceItem(GetAllPowerSourceFourniture(isFromShip));
		List<GameItem> allPoweredOnMachinery = MachineryHelper.GetAllPoweredOnMachinery(isFromShip);
		int num = CheckAvailablePower(isFromShip);
		int num2 = CheckCurrentPowerUsage(isFromShip);
		ChargeAllRechargable(allPowerSourceItem);
		if (num >= num2)
		{
			foreach (GameItem item in allPoweredOnMachinery)
			{
				item.ModifyTag("machinery_received_power", delegate(TagState state)
				{
					state.Enable().SetBool(val: true);
				});
			}
			foreach (GameItem item2 in allPowerSourceItem)
			{
				int num3 = TryRemoveEnergy(item2, num2);
				num2 -= num3;
			}
			_ = 0;
			return;
		}
		foreach (GameItem item3 in allPowerSourceItem)
		{
			RemoveMaxEnergy(item3);
		}
		foreach (GameItem item4 in allPoweredOnMachinery)
		{
			num = TryToPowerMachinery(item4, num);
		}
	}

	public static int TryToPowerMachinery(GameItem machinery, int availableEnergy)
	{
		int @int = machinery.GetTagReadonly("machinery_drawn_power").GetInt();
		if (machinery.GetTagReadonly("machinery_drawn_power").GetInt() <= availableEnergy)
		{
			machinery.ModifyTag("machinery_received_power", delegate(TagState state)
			{
				state.Enable().SetBool(val: true);
			});
			return availableEnergy - @int;
		}
		return availableEnergy;
	}

	public static int TryToPowerMachinerySimulation(GameItem machinery, int availableEnergy)
	{
		int @int = machinery.GetTagReadonly("machinery_drawn_power").GetInt();
		if (machinery.GetTagReadonly("machinery_drawn_power").GetInt() <= availableEnergy)
		{
			machinery.ModifyTag("WILL_MACHINERY_RECEIVED_POWER", delegate(TagState state)
			{
				state.Enable().SetBool(val: true);
			});
			return availableEnergy - @int;
		}
		machinery.ModifyTag("WILL_MACHINERY_RECEIVED_POWER", delegate(TagState state)
		{
			state.Enable().SetBool(val: false);
		});
		return availableEnergy;
	}

	public static void RemoveMaxEnergy(GameItem powerSourceItem)
	{
		int availableEnergy = powerSourceItem.GetTagReadonly("power_source_item_energy").GetInt();
		int @int = powerSourceItem.GetTagReadonly("power_source_item_bandwidth").GetInt();
		int bottleNeck = 0;
		if (availableEnergy < @int)
		{
			bottleNeck = availableEnergy;
		}
		powerSourceItem.ModifyTag("power_source_item_energy", delegate(TagState state)
		{
			state.SetInt(availableEnergy - bottleNeck);
		});
	}

	public static int TryRemoveEnergy(GameItem powerSourceItem, int desiredEnergy, bool useBandwidth = true)
	{
		int availableEnergy = powerSourceItem.GetTagReadonly("power_source_item_energy").GetInt();
		int num = powerSourceItem.GetTagReadonly("power_source_item_bandwidth").GetInt();
		int bottleNeck = 0;
		if (!useBandwidth)
		{
			num = availableEnergy;
		}
		if (availableEnergy < num)
		{
			bottleNeck = availableEnergy;
		}
		else
		{
			bottleNeck = num;
		}
		if (desiredEnergy <= num && desiredEnergy <= availableEnergy)
		{
			powerSourceItem.ModifyTag("power_source_item_energy", delegate(TagState state)
			{
				state.SetInt(availableEnergy - desiredEnergy);
			});
			return desiredEnergy;
		}
		powerSourceItem.ModifyTag("power_source_item_energy", delegate(TagState state)
		{
			state.SetInt(availableEnergy - bottleNeck);
		});
		return bottleNeck;
	}

	public static int CheckAvailablePower(bool isFromShip = true)
	{
		List<GameItem> allPowerSourceItem = GetAllPowerSourceItem(GetAllPowerSourceFourniture(isFromShip));
		int num = 0;
		foreach (GameItem item in allPowerSourceItem)
		{
			num += GetAvailableEnergyFromItem(item);
		}
		return num;
	}

	public static int CheckAvailablePower(GameItem gameItem)
	{
		if (MachineHelper.GetBodyStorage(gameItem, 0) as PixelElement is GameSlotInventory { childItem: not null })
		{
			return GetAvailableEnergyFromItem(gameItem, useBandwidth: false);
		}
		return 0;
	}

	public static int CheckAvailablePowerStored(bool isFromShip = true)
	{
		List<GameItem> allPowerSourceItem = GetAllPowerSourceItem(GetAllPowerSourceFourniture(isFromShip));
		int num = 0;
		foreach (GameItem item in allPowerSourceItem)
		{
			int @int = item.GetTagReadonly("power_source_item_energy").GetInt();
			num += @int;
		}
		return num;
	}

	public static int CheckAvailableMaxPower(bool isFromShip = true)
	{
		List<GameItem> allPowerSourceItem = GetAllPowerSourceItem(GetAllPowerSourceFourniture(isFromShip));
		int num = 0;
		foreach (GameItem item in allPowerSourceItem)
		{
			int @int = item.GetTagReadonly("power_source_item_max_energy").GetInt();
			num += @int;
		}
		return num;
	}

	public static int CheckCurrentPowerUsage(bool isFromShip = true)
	{
		List<GameItem> allPoweredOnMachinery = MachineryHelper.GetAllPoweredOnMachinery(isFromShip);
		int num = 0;
		foreach (GameItem item in allPoweredOnMachinery)
		{
			num += item.GetTagReadonly("machinery_drawn_power").GetInt();
		}
		return num;
	}

	public static void InitPowerSourceItem(GameItem item, int energy, int bandiwith)
	{
		item.SetGameItemType("ENERGY_CELL");
		item.EnableTag("power_source_item");
		item.ModifyTag("power_source_item_energy", delegate(TagState state)
		{
			state.Enable().SetInt(energy);
		});
		item.ModifyTag("power_source_item_max_energy", delegate(TagState state)
		{
			state.Enable().SetInt(energy);
		});
		item.ModifyTag("power_source_item_bandwidth", delegate(TagState state)
		{
			state.Enable().SetInt(bandiwith);
		});
		item.canUseOutsideCombat = true;
		item.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.mayThisTargetItemFunc, new Func<GameItem, GameItem, bool>(MayBatterySwap));
		item.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.canThisTargetItemFunc, new Func<GameItem, GameItem, bool>(MayBatterySwap));
		item.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(item.onThisTargetItemFunc, new Action<GameItem, GameItem>(SwapBatteries));
		AudioHelper.InitBattery(item);
	}

	private static bool MayBatterySwap(GameItem source, GameItem target)
	{
		if (source == null || target == null || source == target)
		{
			return false;
		}
		if (!source.IsTag("power_source_item"))
		{
			return false;
		}
		if (!target.IsTag("power_source_item"))
		{
			return false;
		}
		if (target.MayStack(source))
		{
			return false;
		}
		if (source.parentInventory == null || target.parentInventory == null)
		{
			return false;
		}
		if (source.parentInventory == target.parentInventory)
		{
			return false;
		}
		return true;
	}

	private static void SwapBatteries(GameItem source, GameItem target)
	{
		if (MayBatterySwap(source, target))
		{
			GraphUtils.SwapItemsInPlace(source, target);
		}
	}

	public static bool IsPowerSourceFull(GameItem gameItem)
	{
		if (gameItem.IsTag("power_source_item"))
		{
			int @int = gameItem.GetTagReadonly("power_source_item_energy").GetInt();
			int int2 = gameItem.GetTagReadonly("power_source_item_max_energy").GetInt();
			if (@int >= int2)
			{
				return true;
			}
		}
		return false;
	}

	public static void SetPowerSourceAt(GameItem item, int energy)
	{
		item.ModifyTag("power_source_item_energy", delegate(TagState state)
		{
			state.Enable().SetInt(energy);
		});
		UpdateEnergyCreditSprite(item);
	}

	public static void UpdateEnergyCreditSprite(GameItem item)
	{
		int @int = item.GetTagReadonly("power_source_item_energy").GetInt();
		int int2 = item.GetTagReadonly("power_source_item_max_energy").GetInt();
		if (int2 <= 0)
		{
			return;
		}
		int num = @int * 100 / int2;
		if (item.identifier.Equals("energy_credit"))
		{
			if (num >= 90)
			{
				item.SetSpriteAndShape("Items/items_tool", "battery100");
			}
			else if (num >= 70)
			{
				item.SetSpriteAndShape("Items/items_tool", "battery80");
			}
			else if (num >= 50)
			{
				item.SetSpriteAndShape("Items/items_tool", "battery60");
			}
			else if (num >= 30)
			{
				item.SetSpriteAndShape("Items/items_tool", "battery40");
			}
			else if (num >= 10)
			{
				item.SetSpriteAndShape("Items/items_tool", "battery20");
			}
			else if (num >= 1)
			{
				item.SetSpriteAndShape("Items/items_tool", "battery10");
			}
			else
			{
				item.SetSpriteAndShape("Items/items_tool", "battery0");
			}
		}
		if (item.identifier.Equals("energy_credit_ext"))
		{
			if (num >= 90)
			{
				item.SetSpriteAndShape("Items/items_tool", "battery_ext100");
			}
			else if (num >= 70)
			{
				item.SetSpriteAndShape("Items/items_tool", "battery_ext80");
			}
			else if (num >= 50)
			{
				item.SetSpriteAndShape("Items/items_tool", "battery_ext60");
			}
			else if (num >= 30)
			{
				item.SetSpriteAndShape("Items/items_tool", "battery_ext40");
			}
			else if (num >= 10)
			{
				item.SetSpriteAndShape("Items/items_tool", "battery_ext20");
			}
			else if (num >= 1)
			{
				item.SetSpriteAndShape("Items/items_tool", "battery_ext10");
			}
			else
			{
				item.SetSpriteAndShape("Items/items_tool", "battery_ext0");
			}
		}
		if (item.identifier.Equals("energy_credit_breeder"))
		{
			if (num >= 90)
			{
				item.SetSpriteAndShape("Items/items_tool2", "sbattery100");
			}
			else if (num >= 70)
			{
				item.SetSpriteAndShape("Items/items_tool2", "sbattery80");
			}
			else if (num >= 50)
			{
				item.SetSpriteAndShape("Items/items_tool2", "sbattery60");
			}
			else if (num >= 30)
			{
				item.SetSpriteAndShape("Items/items_tool2", "sbattery40");
			}
			else if (num >= 10)
			{
				item.SetSpriteAndShape("Items/items_tool2", "sbattery20");
			}
			else if (num >= 1)
			{
				item.SetSpriteAndShape("Items/items_tool2", "sbattery10");
			}
			else
			{
				item.SetSpriteAndShape("Items/items_tool2", "sbattery0");
			}
		}
	}

	public static void InitPowerSourceFourniture(GameItem item)
	{
		item.EnableTag("power_source_furniture");
	}

	public static List<GameItem> GetAllPowerSourceFourniture(bool isFromShip = true)
	{
		List<GameItem> list = ((!isFromShip) ? GameMaster.current.raidManager.GetAllGameItemExcludeOnShip() : GraphUtils.FindAllChildrenType<GameItem>(GameMaster.current.shipManager.ship));
		List<GameItem> list2 = new List<GameItem>();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num].IsTag("power_source_furniture"))
			{
				list2.Add(list[num]);
			}
		}
		return list2;
	}

	public static List<GameItem> GetAllPowerSourceItem(List<GameItem> powerStorage)
	{
		List<GameItem> list = new List<GameItem>();
		for (int num = powerStorage.Count - 1; num >= 0; num--)
		{
			list.AddRange(GraphUtils.FindAllChildrenType<GameItem>(powerStorage[num]));
		}
		return list;
	}

	public static void CreatePowerSourceFournitureTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!item.IsTag("power_source_furniture"))
		{
			return;
		}
		List<GameItem> list = new List<GameItem>();
		list.AddRange(GraphUtils.FindAllChildrenType<GameItem>(item));
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (GameItem item2 in list)
		{
			num += item2.GetTagReadonly("power_source_item_energy").GetInt();
			num2 += item2.GetTagReadonly("power_source_item_max_energy").GetInt();
			if (item2.GetTagReadonly("power_source_item_energy").GetInt() != 0)
			{
				num3 += GetAvailableEnergyFromItem(item2);
			}
		}
		if (num > 0)
		{
			builder.AddLine(" ");
			builder.AddLine("Container Energy Data ", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			builder.AddLine("Total Power: " + num3 + "kW", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue);
			builder.Add("Total Energy: ", RenderHandler.ColorPalette.LightCyan);
			builder.AddProgressBar((float)num / (float)num2, 10, RenderHandler.ColorPalette.LightCyan, RenderHandler.ColorPalette.VeryDarkCyan, null, " ");
			builder.AddLine("(" + num + "/" + (float)num2 + "kJ)", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
	}

	public static void CreatePowerSourceItemTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("power_source_item"))
		{
			int @int = item.GetTagReadonly("power_source_item_energy").GetInt();
			int int2 = item.GetTagReadonly("power_source_item_max_energy").GetInt();
			item.GetTagReadonly("power_source_item_bandwidth").GetInt();
			int int3 = item.GetTagReadonly("power_source_item_recharge").GetInt();
			if (int2 > 0)
			{
				builder.AddLine(" ");
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_power_energy_data"), ignoreEmpty: true, RenderHandler.ColorPalette.Cyan);
				builder.Add(LocHelper.GetLocalizedMechanic("mech_power_energy_label"), RenderHandler.ColorPalette.LightCyan);
				builder.AddProgressBar((float)@int / (float)int2, 10, RenderHandler.ColorPalette.LightCyan, RenderHandler.ColorPalette.VeryDarkCyan, null, " ");
				builder.AddLine("(" + @int + "/" + (float)int2 + "kJ)", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
			if (int3 != 0)
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_power_self_recharge", int3 + "kJ"), ignoreEmpty: true, RenderHandler.ColorPalette.Purple);
			}
			if (IsDisposablePowerSource(item))
			{
				builder.AddLine("Disposable: cannot be recharged", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
			}
		}
	}

	public static void CreatePowerAnalyserTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("power_analyser"))
		{
			bool flag = false;
			int num = CheckAvailableMaxPower(flag);
			int num2 = CheckAvailablePowerStored(flag);
			builder.AddLine(" ");
			if (flag)
			{
				builder.AddLine("Shipwide Energy Data ", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
			else
			{
				builder.AddLine("Stationwide Energy Data ", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
			builder.AddLine("Totale Power: " + CheckAvailablePower(flag) + "kW", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue);
			builder.AddLine("Totale Use: " + CheckCurrentPowerUsage(flag) + "kW", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
			builder.Add("Totale Energy: ", RenderHandler.ColorPalette.LightCyan);
			builder.AddProgressBar((float)num2 / (float)num, 10, RenderHandler.ColorPalette.LightCyan, RenderHandler.ColorPalette.VeryDarkCyan, null, " ");
			builder.AddLine("(" + num2 + "/" + (float)num + "kJ)", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
	}

	public static bool IsAvailableEnergyFromItem(GameItem powerSourceItem)
	{
		return powerSourceItem.IsTag("power_source_item");
	}

	public static int GetAvailableEnergyFromItem(GameItem powerSourceItem, bool useBandwidth = true)
	{
		if (powerSourceItem.IsTag("power_source_item"))
		{
			int @int = powerSourceItem.GetTagReadonly("power_source_item_bandwidth").GetInt();
			int int2 = powerSourceItem.GetTagReadonly("power_source_item_energy").GetInt();
			if (@int < int2 && useBandwidth)
			{
				return @int;
			}
			return int2;
		}
		return 0;
	}

	public static void ChargeBattery(GameItem gameItem)
	{
		if (!gameItem.IsTag("power_source_item_recharge"))
		{
			return;
		}
		int energy = gameItem.GetTagReadonly("power_source_item_energy").GetInt();
		int max = gameItem.GetTagReadonly("power_source_item_max_energy").GetInt();
		int recharge = gameItem.GetTagReadonly("power_source_item_recharge").GetInt();
		if (energy + recharge <= max)
		{
			gameItem.ModifyTag("power_source_item_energy", delegate(TagState state)
			{
				state.SetInt(energy + recharge);
			});
		}
		else
		{
			gameItem.ModifyTag("power_source_item_energy", delegate(TagState state)
			{
				state.SetInt(max);
			});
		}
	}

	public static void ChargeBattery(GameItem gameItem, int amount, bool ignoreBlackout = false)
	{
		if (IsDisposablePowerSource(gameItem) || CurrentConverterHelper.IsCurrentConverter(gameItem) || HypertechBatteryHelper.IsHypertechBattery(gameItem) || (!ignoreBlackout && StoreStation.Instance.storeEventManager.IsBlackoutEventActive() && StoreStation.Instance.storeEventManager.IsPlayerAffectedByBlackout()))
		{
			return;
		}
		int energy = gameItem.GetTagReadonly("power_source_item_energy").GetInt();
		int max = gameItem.GetTagReadonly("power_source_item_max_energy").GetInt();
		if (energy + amount <= max)
		{
			gameItem.ModifyTag("power_source_item_energy", delegate(TagState state)
			{
				state.SetInt(energy + amount);
			});
		}
		else
		{
			gameItem.ModifyTag("power_source_item_energy", delegate(TagState state)
			{
				state.SetInt(max);
			});
		}
		gameItem.GetTagReadonly("power_source_item_energy").GetInt();
		UpdateEnergyCreditSprite(gameItem);
	}

	public static void InitRecharge(GameItem gameItem, int rechargeRate)
	{
		gameItem.ModifyTag("power_source_item_recharge", delegate(TagState state)
		{
			state.Enable().SetInt(rechargeRate);
		});
		gameItem.EnableTag("BREEDER_POWER_SOURCE_ITEM_TAG");
	}

	public static void ChargeAllRechargable(List<GameItem> powerSources)
	{
		foreach (GameItem powerSource in powerSources)
		{
			ChargeBattery(powerSource);
		}
	}

	public static int GetPowerSourceItemValue(GameItem gameItem)
	{
		if (gameItem.IsTag("power_source_item"))
		{
			int num = gameItem.GetTagReadonly("power_source_item_energy").GetInt();
			if (gameItem.IsTag("POWER_SOURCE_ITEM_VALUE_PERCENT_INT"))
			{
				num = num * gameItem.GetTagReadonly("POWER_SOURCE_ITEM_VALUE_PERCENT_INT").GetInt() / 100;
			}
			return (int)(num + gameItem.unitBaseValue);
		}
		return 0;
	}

	public static void SetPowerSourceValuePercent(GameItem gameItem, int valuePercent)
	{
		gameItem.ModifyTag("POWER_SOURCE_ITEM_VALUE_PERCENT_INT", delegate(TagState state)
		{
			state.Enable().SetInt(valuePercent);
		});
	}

	public static void InitDisposable(GameItem gameItem)
	{
		gameItem.EnableTag("POWER_SOURCE_ITEM_DISPOSABLE_TAG");
	}

	public static bool IsDisposablePowerSource(GameItem gameItem)
	{
		return gameItem?.IsTag("POWER_SOURCE_ITEM_DISPOSABLE_TAG") ?? false;
	}

	public static void RandomizedEnergyCredit(GameItem gameItem)
	{
		if (gameItem.IsTag("power_source_item"))
		{
			int @int = gameItem.GetTagReadonly("power_source_item_max_energy").GetInt();
			int randomizedEnergyValue = RNG.GetRandomInt(0, @int);
			gameItem.ModifyTag("power_source_item_energy", delegate(TagState state)
			{
				state.Enable().SetInt(randomizedEnergyValue);
			});
		}
	}

	public static bool DrawPowerSource(GameItem gameItem, int amount)
	{
		if (CurrentConverterHelper.IsCurrentConverter(gameItem))
		{
			return CurrentConverterHelper.Supply(gameItem, amount);
		}
		if (HypertechBatteryHelper.IsHypertechBattery(gameItem))
		{
			return HypertechBatteryHelper.CanSupply(gameItem, amount);
		}
		if (gameItem.IsTag("power_source_item"))
		{
			int energy = gameItem.GetTagReadonly("power_source_item_energy").GetInt();
			gameItem.GetTagReadonly("power_source_item_max_energy").GetInt();
			if (energy - amount >= 0)
			{
				gameItem.ModifyTag("power_source_item_energy", delegate(TagState state)
				{
					state.SetInt(energy - amount);
				});
				UpdateEnergyCreditSprite(gameItem);
				return true;
			}
			return false;
		}
		return false;
	}

	public static bool CanDrawPowerSource(GameItem gameItem, int amount)
	{
		if (gameItem == null)
		{
			return false;
		}
		if (CurrentConverterHelper.IsCurrentConverter(gameItem))
		{
			return CurrentConverterHelper.CanSupply(gameItem, amount);
		}
		if (HypertechBatteryHelper.IsHypertechBattery(gameItem))
		{
			return HypertechBatteryHelper.CanSupply(gameItem, amount);
		}
		if (gameItem.IsTag("power_source_item"))
		{
			int @int = gameItem.GetTagReadonly("power_source_item_energy").GetInt();
			gameItem.GetTagReadonly("power_source_item_max_energy").GetInt();
			if (@int - amount >= 0)
			{
				return true;
			}
			return false;
		}
		return false;
	}
}
