using System;
using System.Collections.Generic;
using System.Linq;

public class ArmorHelper
{
	public const string ARMOR_TAG = "armor_item";

	public const string DEFAULT_ARMOR_VALUE = "default_armor_value";

	public const string CURRENT_MAX_ARMOR_VALUE = "current_max_armor_value";

	public const string CURRENT_ARMOR_VALUE = "current_armor_value";

	public const string ARMOR_EFFICIENCY = "armor_efficiency";

	public const string ARMOR_MITIGATION = "armor_mitigation";

	public const string ARMOR_PROTECTION = "armor_protection";

	public const string ARMOR_RESILIENCE = "armor_resilience";

	public const string ARMOR_ACCURACY_MODIFIER = "armor_accuracy_modifier";

	public const string ARMOR_DODGE_MODIFIER = "armor_dodge_modifier";

	public static int DistributeDamage(List<GameItem> allArmorItem, int damage, GameCharacterItem entity)
	{
		int num = damage * -1;
		int absorbedDamdage = 0;
		for (bool flag = true; flag && num > 0; flag = AnyArmorValueRemain(allArmorItem))
		{
			foreach (GameItem item in allArmorItem)
			{
				int armorValue = item.GetTagReadonly("current_armor_value").GetInt();
				if (armorValue > 0 && num > 0)
				{
					if (!RNG.Roll(item.GetTagReadonly("armor_mitigation").GetInt()))
					{
						item.ModifyTag("current_armor_value", delegate(TagState state)
						{
							state.SetInt(armorValue - 1);
						});
					}
					num--;
					absorbedDamdage++;
				}
				if (num <= 0)
				{
					return 0;
				}
			}
		}
		if (absorbedDamdage != 0)
		{
			ActionQueue.current.Add(new EmptyAction(0.20000000298023224).SetBeginFunc(delegate
			{
				CombatLog.log(entity.name + "'s armor was able to defend against " + absorbedDamdage + " damage!");
				SplashTextManager.Instance.DisplayFloatText(entity, (-absorbedDamdage).ToString(), SplashTextManager.TextMode.ArmorDamage);
			}));
		}
		return num;
	}

	public static int DistributeDamageWithEfficiency(List<GameItem> allArmorItem, int damage, GameCharacterItem entity)
	{
		int num = damage * -1;
		int absorbedDamdage = 0;
		allArmorItem.OrderBy((GameItem item) => item.GetTagReadonly("armor_efficiency").GetInt()).ToList();
		foreach (GameItem item in allArmorItem)
		{
			int @int = item.GetTagReadonly("current_armor_value").GetInt();
			int int2 = item.GetTagReadonly("armor_resilience").GetInt();
			if (0 >= @int && int2 > 0)
			{
				int num2 = 0;
				num2 = ((num <= int2) ? num : int2);
				num -= num2;
				absorbedDamdage += num2;
			}
		}
		foreach (GameItem item2 in allArmorItem)
		{
			int int3 = item2.GetTagReadonly("armor_protection").GetInt();
			int armorValue2 = item2.GetTagReadonly("current_armor_value").GetInt();
			if (int3 > 0 || num < 1)
			{
				int maxBlockingDamage = 0;
				if (num > int3)
				{
					maxBlockingDamage = int3;
				}
				else
				{
					maxBlockingDamage = num;
				}
				if (armorValue2 <= maxBlockingDamage)
				{
					int3 = armorValue2;
				}
				num -= maxBlockingDamage;
				absorbedDamdage += maxBlockingDamage;
				item2.ModifyTag("current_armor_value", delegate(TagState state)
				{
					state.SetInt(armorValue2 - maxBlockingDamage);
				});
				if (num < 1)
				{
					break;
				}
			}
		}
		for (int i = 0; i <= num; i++)
		{
			foreach (GameItem item3 in allArmorItem)
			{
				int armorValue = item3.GetTagReadonly("current_armor_value").GetInt();
				int int4 = item3.GetTagReadonly("armor_efficiency").GetInt();
				if (armorValue <= 0 || num <= 0 || !RNG.Roll(int4))
				{
					continue;
				}
				if (!RNG.Roll(item3.GetTagReadonly("armor_mitigation").GetInt()))
				{
					item3.ModifyTag("current_armor_value", delegate(TagState state)
					{
						state.SetInt(armorValue - 1);
					});
				}
				num--;
				absorbedDamdage++;
				break;
			}
		}
		if (absorbedDamdage != 0)
		{
			ActionQueue.current.Add(new EmptyAction(0.20000000298023224).SetBeginFunc(delegate
			{
				CombatLog.log(entity.name + "'s armor was able to defend against " + absorbedDamdage + " damage!");
				SplashTextManager.Instance.DisplayFloatText(entity, (-absorbedDamdage).ToString(), SplashTextManager.TextMode.ArmorDamage);
			}));
		}
		return num;
	}

	public static bool AnyArmorValueRemain(List<GameItem> allArmorItem)
	{
		bool result = false;
		foreach (GameItem item in allArmorItem)
		{
			if (item.GetTagReadonly("current_armor_value").GetInt() > 0)
			{
				result = true;
			}
		}
		return result;
	}

	public static void CreateArmorItemTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!item.IsTag("armor_item"))
		{
			return;
		}
		int @int = item.GetTagReadonly("current_armor_value").GetInt();
		int int2 = item.GetTagReadonly("current_max_armor_value").GetInt();
		int int3 = item.GetTagReadonly("armor_efficiency").GetInt();
		int int4 = item.GetTagReadonly("armor_mitigation").GetInt();
		int int5 = item.GetTagReadonly("armor_protection").GetInt();
		int int6 = item.GetTagReadonly("armor_resilience").GetInt();
		int int7 = item.GetTagReadonly("armor_accuracy_modifier").GetInt();
		int int8 = item.GetTagReadonly("armor_dodge_modifier").GetInt();
		if (int2 > 0)
		{
			builder.AddLine("Efficiency: " + int3 + "%", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			if (int4 != 0)
			{
				builder.AddLine("Mitigation: " + int4 + "%", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
			if (int5 != 0)
			{
				builder.AddLine("Protection: " + int5, ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
			if (int6 != 0)
			{
				builder.AddLine("Resilience: " + int6, ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
			if (int7 != 0)
			{
				builder.AddLine("Accuracy  : " + int7, ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
			}
			if (int8 != 0)
			{
				builder.AddLine("Dodge     : " + int8, ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
			}
			builder.Add(" Armor: ", RenderHandler.ColorPalette.LightCyan);
			builder.AddProgressBar((float)@int / (float)int2, 10, RenderHandler.ColorPalette.LightCyan, RenderHandler.ColorPalette.VeryDarkCyan, null, " ");
			builder.AddLine("(" + @int + "/" + (float)int2 + ")", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
	}

	public static void RepairArmorWithoutLoss(GameItem item)
	{
		int maxArmor = item.GetTagReadonly("current_max_armor_value").GetInt();
		item.ModifyTag("current_armor_value", delegate(TagState state)
		{
			state.SetInt(maxArmor);
		});
	}

	public static void RepairArmorWithLoss(GameItem item)
	{
		int maxArmor = item.GetTagReadonly("current_max_armor_value").GetInt();
		if (maxArmor > 0)
		{
			item.ModifyTag("current_max_armor_value", delegate(TagState state)
			{
				state.SetInt(maxArmor - 1);
			});
			item.ModifyTag("current_armor_value", delegate(TagState state)
			{
				state.SetInt(maxArmor - 1);
			});
		}
	}

	public static void RepairArmorWithAmount(GameItem item, int repairAmount)
	{
		int @int = item.GetTagReadonly("current_max_armor_value").GetInt();
		int int2 = item.GetTagReadonly("current_armor_value").GetInt();
		int newArmor = int2 + repairAmount;
		if (newArmor > @int)
		{
			newArmor = @int;
		}
		if (@int > 0)
		{
			item.ModifyTag("current_armor_value", delegate(TagState state)
			{
				state.SetInt(newArmor);
			});
		}
	}

	public static void InitArmorItem(GameItem item, int defaultArmorValue, int efficiency, int mitigation, int protection, int resilience = 0, int accuracyModifier = 0, int dodgeModifier = 0)
	{
		item.EnableTag("armor_item");
		item.ModifyTag("default_armor_value", delegate(TagState state)
		{
			state.Enable().SetInt(defaultArmorValue);
		});
		item.ModifyTag("current_max_armor_value", delegate(TagState state)
		{
			state.Enable().SetInt(defaultArmorValue);
		});
		item.ModifyTag("current_armor_value", delegate(TagState state)
		{
			state.Enable().SetInt(defaultArmorValue);
		});
		item.ModifyTag("armor_efficiency", delegate(TagState state)
		{
			state.Enable().SetInt(efficiency);
		});
		item.ModifyTag("armor_mitigation", delegate(TagState state)
		{
			state.Enable().SetInt(mitigation);
		});
		item.ModifyTag("armor_protection", delegate(TagState state)
		{
			state.Enable().SetInt(protection);
		});
		item.ModifyTag("armor_resilience", delegate(TagState state)
		{
			state.Enable().SetInt(resilience);
		});
		item.ModifyTag("armor_accuracy_modifier", delegate(TagState state)
		{
			state.Enable().SetInt(accuracyModifier);
		});
		item.ModifyTag("armor_dodge_modifier", delegate(TagState state)
		{
			state.Enable().SetInt(dodgeModifier);
		});
		item.SetType("armor_item");
		item.SetGameItemType("ARMOR");
		item.onSlotAddItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(item.onSlotAddItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem i, GameInventory _, SlotMarker _)
		{
			if (GraphUtils.FindParentType<GameItem>(i) is GameCharacterItem gameCharacterItem2)
			{
				gameCharacterItem2.accuracyPercentage += accuracyModifier;
				gameCharacterItem2.dodge += dodgeModifier;
			}
		});
		item.onSlotRemoveItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(item.onSlotRemoveItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem i, GameInventory _, SlotMarker _)
		{
			if (GraphUtils.FindParentType<GameItem>(i) is GameCharacterItem gameCharacterItem)
			{
				gameCharacterItem.accuracyPercentage -= accuracyModifier;
				gameCharacterItem.dodge -= dodgeModifier;
			}
		});
	}

	public static void ModifyArmor(GameItem item, int removeArmorValue, int removeMaxArmorValue)
	{
		int currentArmor = item.GetTagReadonly("current_armor_value").GetInt();
		int maxArmor = item.GetTagReadonly("current_max_armor_value").GetInt();
		item.ModifyTag("current_max_armor_value", delegate(TagState state)
		{
			state.Enable().SetInt(maxArmor - removeMaxArmorValue);
		});
		item.ModifyTag("current_armor_value", delegate(TagState state)
		{
			state.Enable().SetInt(currentArmor - removeArmorValue);
		});
	}

	public static int GetArmorItemValue(GameItem item)
	{
		if (item.IsTag("armor_item"))
		{
			int @int = item.GetTagReadonly("current_armor_value").GetInt();
			int int2 = item.GetTagReadonly("current_max_armor_value").GetInt();
			int int3 = item.GetTagReadonly("armor_efficiency").GetInt();
			return (@int + int2) * int3 / 100;
		}
		return 0;
	}
}
