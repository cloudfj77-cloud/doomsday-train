using System;
using System.Collections.Generic;

public class TechnicianBackpackHelper
{
	public const string COMPLEXE_EQUIPMENT_TAG = "COMPLEXE_EQUIPMENT_TAG";

	public const string CHARGE_RATE_INT = "CHARGE_RATE_INT";

	public const string CURRENT_CHARGE_INT = "CURRENT_CHARGE_INT";

	public const string MAX_CHARGE_INT = "MAX_CHARGE_INT";

	public const string POWER_SWITCH_BOOL = "POWER_SWITCH_BOOL";

	public const string COMPLEXE_MODULE_TAG = "COMPLEXE_MODULE_TAG";

	public const string COMPLEXE_MODULE_ADJACENCY_TAG = "COMPLEXE_MODULE_ADJACENCY_TAG";

	public const string CHARGE_RATE_MODIFIER_INT = "CHARGE_RATE_MODIFIER_INT";

	public const string MAX_CHARGE_MODIFIER_INT = "MAX_CHARGE_MODIFIER_INT";

	public const string COMPLEXE_MODULE_POWER_ECO_AMOUNT_INT = "COMPLEXE_MODULE_POWER_ECO_AMOUNT_INT";

	public const string COMPLEXE_MODULE_EFFECTIVENESS_AMOUNT_INT = "COMPLEXE_MODULE_EFFECTIVENESS_AMOUNT_INT";

	public const string COMPLEXE_SYSTEM_TAG = "COMPLEXE_SYSTEM_TAG";

	public const string POWER_USE_INT = "POWER_USE_INT";

	public const string POWER_USE_BASE_INT = "POWER_USE_BASE_INT";

	public static void InitComplexeEquipment(GameItem item, GameInventory slotInventory, GameInventory itenInventory)
	{
		item.ModifyTag("COMPLEXE_EQUIPMENT_TAG", delegate(TagState state)
		{
			state.Enable().SetBool(val: true);
		});
		item.ModifyTag("CHARGE_RATE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		item.ModifyTag("CURRENT_CHARGE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		item.ModifyTag("MAX_CHARGE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		item.mayToggleSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.canToggleSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		GameItem gameItem = item;
		gameItem.onTurnEndSlotItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameItem.onTurnEndSlotItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem item, GameInventory parentInventory, SlotMarker slot)
		{
			if (item.GetTagReadonly("POWER_SWITCH_BOOL").GetBool() && !IsEquipmentCharged(item))
			{
				List<GameItem> list = GraphUtils.FindAllChildrenType(slotInventory, (GameItem item) => item.IsTag("power_source_item"));
				if (list.Count > 0)
				{
					int @int = list[0].GetTagReadonly("power_source_item_energy").GetInt();
					int int2 = list[0].GetTagReadonly("power_source_item_bandwidth").GetInt();
					int num = 0;
					num = ((@int >= int2) ? int2 : @int);
					int current_draw = item.GetTagReadonly("CURRENT_CHARGE_INT").GetInt();
					int int3 = item.GetTagReadonly("MAX_CHARGE_INT").GetInt();
					int int4 = item.GetTagReadonly("CHARGE_RATE_INT").GetInt();
					int num2 = int3 - current_draw;
					int newAvailableEnergy = 0;
					int amountCharged = 0;
					if (num >= int4)
					{
						newAvailableEnergy = @int - int4;
						amountCharged = int4;
					}
					else
					{
						newAvailableEnergy = 0;
						amountCharged = num;
					}
					if (amountCharged >= num2)
					{
						newAvailableEnergy = @int - num2;
						amountCharged = num2;
					}
					item.ModifyTag("CURRENT_CHARGE_INT", delegate(TagState state)
					{
						state.SetInt(current_draw + amountCharged);
					});
					list[0].ModifyTag("power_source_item_energy", delegate(TagState state)
					{
						state.SetInt(newAvailableEnergy);
					});
				}
			}
		});
		itenInventory.onSlotAddItemEndFunc = delegate
		{
			CheckInventoryStatus(item, itenInventory);
		};
		itenInventory.onSlotRemoveItemEndFunc = delegate
		{
			CheckInventoryStatus(item, itenInventory);
		};
		item.onToggleSlotItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _)
		{
			bool currentState = !item.GetTagReadonly("POWER_SWITCH_BOOL").GetBool();
			item.ModifyTag("POWER_SWITCH_BOOL", delegate(TagState state)
			{
				state.Enable().SetBool(currentState);
			});
		};
	}

	public static void CheckBasicModule(List<GameItem> modules, GameItem item)
	{
		int totalChargeRate = 0;
		int totalCapacity = 0;
		int currentCharge = item.GetTagReadonly("CURRENT_CHARGE_INT").GetInt();
		foreach (GameItem module in modules)
		{
			int @int = module.GetTagReadonly("CHARGE_RATE_MODIFIER_INT").GetInt();
			int int2 = module.GetTagReadonly("MAX_CHARGE_MODIFIER_INT").GetInt();
			totalChargeRate += @int;
			totalCapacity += int2;
		}
		if (currentCharge >= totalCapacity)
		{
			currentCharge = totalCapacity;
		}
		item.ModifyTag("CURRENT_CHARGE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(currentCharge);
		});
		item.ModifyTag("CHARGE_RATE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(totalChargeRate);
		});
		item.ModifyTag("MAX_CHARGE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(totalCapacity);
		});
	}

	public static void CheckAdjacentModule(List<GameItem> systems, GameItem item)
	{
		foreach (GameItem system in systems)
		{
			List<GameItem> list = system.FindAdjacentType<GameItem>();
			int num = 0;
			int num2 = 0;
			foreach (GameItem item2 in list)
			{
				if (item2.IsTag("COMPLEXE_MODULE_ADJACENCY_TAG"))
				{
					int @int = item2.GetTagReadonly("COMPLEXE_MODULE_POWER_ECO_AMOUNT_INT").GetInt();
					num += @int;
					int int2 = item2.GetTagReadonly("COMPLEXE_MODULE_EFFECTIVENESS_AMOUNT_INT").GetInt();
					num2 += int2;
				}
			}
			ApplyPowerEcoAmount(system, num);
			ApplyEffectivenessAmount(system, num2);
		}
	}

	public static void ApplyPowerEcoAmount(GameItem system, int powerReduction)
	{
		int @int = system.GetTagReadonly("POWER_USE_BASE_INT").GetInt();
		int newPowerUsage = 0;
		if (@int + powerReduction < 0)
		{
			newPowerUsage = 0;
		}
		else
		{
			newPowerUsage = @int + powerReduction;
		}
		system.ModifyTag("POWER_USE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(newPowerUsage);
		});
	}

	public static void ApplyEffectivenessAmount(GameItem system, int totalEffectivenessAmount)
	{
		foreach (CombatAbilityEffect effect in system.effects)
		{
			if (effect.valueType == CombatAbilityEffect.ValueType.standard)
			{
				effect.value = effect.valueBase + totalEffectivenessAmount;
			}
			else if (effect.valueType == CombatAbilityEffect.ValueType.uniform)
			{
				effect.valueLow = effect.valueLowBase + totalEffectivenessAmount;
				effect.valueHigh = effect.valueHighBase + totalEffectivenessAmount;
			}
		}
	}

	public static void CheckInventoryStatus(GameItem item, GameInventory inventory)
	{
		List<GameItem> modules = GraphUtils.FindAllChildrenType(inventory, (GameItem item) => item.IsTag("COMPLEXE_MODULE_TAG"));
		List<GameItem> systems = GraphUtils.FindAllChildrenType(inventory, (GameItem item) => item.IsTag("COMPLEXE_SYSTEM_TAG"));
		CheckBasicModule(modules, item);
		CheckAdjacentModule(systems, item);
	}

	public static bool IsEquipmentCharged(GameItem item)
	{
		int @int = item.GetTagReadonly("CURRENT_CHARGE_INT").GetInt();
		int int2 = item.GetTagReadonly("MAX_CHARGE_INT").GetInt();
		if (@int >= int2)
		{
			return true;
		}
		return false;
	}

	public static void InitComplexeModule(GameItem item, int chargeRateMod = 0, int maxChargeMod = 0, bool adjacent = false, int powerEcoAmount = 0, int effectivenessAmount = 0)
	{
		item.ModifyTag("COMPLEXE_MODULE_TAG", delegate(TagState state)
		{
			state.Enable().SetBool(val: true);
		});
		item.ModifyTag("CHARGE_RATE_MODIFIER_INT", delegate(TagState state)
		{
			state.Enable().SetInt(chargeRateMod);
		});
		item.ModifyTag("MAX_CHARGE_MODIFIER_INT", delegate(TagState state)
		{
			state.Enable().SetInt(maxChargeMod);
		});
		if (adjacent)
		{
			item.ModifyTag("COMPLEXE_MODULE_ADJACENCY_TAG", delegate(TagState state)
			{
				state.Enable().SetBool(val: true);
			});
			item.ModifyTag("COMPLEXE_MODULE_POWER_ECO_AMOUNT_INT", delegate(TagState state)
			{
				state.Enable().SetInt(powerEcoAmount);
			});
			item.ModifyTag("COMPLEXE_MODULE_EFFECTIVENESS_AMOUNT_INT", delegate(TagState state)
			{
				state.Enable().SetInt(effectivenessAmount);
			});
		}
	}

	public static void InitComplexeSystem(GameItem item, int energyPerUse = 1)
	{
		item.ModifyTag("COMPLEXE_SYSTEM_TAG", delegate(TagState state)
		{
			state.Enable().SetBool(val: true);
		});
		item.ModifyTag("POWER_USE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(energyPerUse);
		});
		item.ModifyTag("POWER_USE_BASE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(energyPerUse);
		});
		item.onItemTargetItemFunc = delegate
		{
		};
		GameItem gameItem = item;
		gameItem.onItemTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem.onItemTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			GameItem gameItem2 = GraphUtils.FindParentType<GameItem>(item);
			if (gameItem2 != null && gameItem2.IsTag("COMPLEXE_EQUIPMENT_TAG") && HasPowerToUseComplexeSystem(item, gameItem2) && GameMaster.current.raidManager.CastAbility(s, t))
			{
				UsePowerForComplexeSystem(item, gameItem2);
			}
		});
	}

	public static bool HasPowerToUseComplexeSystem(GameItem system, GameItem equipment)
	{
		int @int = system.GetTagReadonly("POWER_USE_INT").GetInt();
		int int2 = equipment.GetTagReadonly("CURRENT_CHARGE_INT").GetInt();
		if (@int <= int2)
		{
			return true;
		}
		return false;
	}

	public static void UsePowerForComplexeSystem(GameItem system, GameItem equipment)
	{
		int required = system.GetTagReadonly("POWER_USE_INT").GetInt();
		int available = equipment.GetTagReadonly("CURRENT_CHARGE_INT").GetInt();
		equipment.ModifyTag("CURRENT_CHARGE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(available - required);
		});
	}

	public static void CreateComplexeEquipmentTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("COMPLEXE_EQUIPMENT_TAG"))
		{
			bool @bool = item.GetTagReadonly("POWER_SWITCH_BOOL").GetBool();
			int @int = item.GetTagReadonly("CURRENT_CHARGE_INT").GetInt();
			int int2 = item.GetTagReadonly("MAX_CHARGE_INT").GetInt();
			int int3 = item.GetTagReadonly("CHARGE_RATE_INT").GetInt();
			if (@bool)
			{
				builder.AddLine("Switched On", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
			else
			{
				builder.AddLine("Switched Off", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
			}
			builder.AddLine("Charge Speed: " + int3 + "/Turn", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			builder.AddLine(" ");
			builder.AddLine("Internal Accumulateur:", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			builder.AddProgressBar((float)@int / (float)int2, 10, RenderHandler.ColorPalette.LightCyan, RenderHandler.ColorPalette.VeryDarkCyan, null, " ");
			builder.AddLine("(" + @int + "/" + (float)int2 + ")", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			if (!@bool)
			{
				builder.AddLine("Charging Paused", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
			}
		}
	}

	public static void CreateComplexeModuleTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!item.IsTag("COMPLEXE_MODULE_TAG"))
		{
			return;
		}
		int @int = item.GetTagReadonly("CHARGE_RATE_MODIFIER_INT").GetInt();
		int int2 = item.GetTagReadonly("MAX_CHARGE_MODIFIER_INT").GetInt();
		if (@int != 0)
		{
			builder.AddLine("Charge Rate: " + MetaHelper.FormatTooltipInt(@int) + "/Turn", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		if (int2 != 0)
		{
			builder.AddLine("Capacitor Size: " + MetaHelper.FormatTooltipInt(int2), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		if (item.IsTag("COMPLEXE_MODULE_ADJACENCY_TAG"))
		{
			int int3 = item.GetTagReadonly("COMPLEXE_MODULE_POWER_ECO_AMOUNT_INT").GetInt();
			int int4 = item.GetTagReadonly("COMPLEXE_MODULE_EFFECTIVENESS_AMOUNT_INT").GetInt();
			builder.AddLine("Adjacency Bonus:", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			if (int3 != 0)
			{
				builder.AddLine("Power Use Modifier: " + MetaHelper.FormatTooltipInt(int3) + "/Turn", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
			if (int4 != 0)
			{
				builder.AddLine("Effectiveness Modifier: " + MetaHelper.FormatTooltipInt(int4) + "/Turn", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
		}
	}

	public static void CreateComplexeSystemTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!item.IsTag("COMPLEXE_SYSTEM_TAG"))
		{
			return;
		}
		int @int = item.GetTagReadonly("POWER_USE_INT").GetInt();
		item.GetTagReadonly("MAX_CHARGE_MODIFIER_INT").GetInt();
		if (@int != 0)
		{
			builder.AddLine("Power Use: " + @int, ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		GameItem gameItem = GraphUtils.FindParentType<GameItem>(item);
		if (gameItem != null && gameItem.IsTag("COMPLEXE_EQUIPMENT_TAG"))
		{
			if (HasPowerToUseComplexeSystem(item, gameItem))
			{
				builder.AddLine("Ready.", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
			else
			{
				builder.AddLine("Not Enough Power.", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
			}
		}
	}
}
