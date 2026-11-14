using System.Collections.Generic;

public class GunHelper
{
	public const string STUN_GUN_ID = "stun_gun";

	public const string SERVICE_GUN_TAG = "SERVICE_GUN_TAG";

	public const string SERVICE_GUN_PARTS_USED_INT = "SERVICE_GUN_PARTS_USED_INT";

	public const string SERVICE_GUN_REPAIR_VALUE_INT = "SERVICE_GUN_REPAIR_VALUE_INT";

	public const string GUN_FIREPOWER_TAG = "GUN_FIREPOWER_TAG";

	public const string GUN_STYLE_TAG = "GUN_STYLE_TAG";

	public const string GUN_RELIABILITY_TAG = "GUN_RELIABILITY_TAG";

	public const string GUN_RECOIL_TAG = "GUN_RECOIL_TAG";

	public const string GUN_ERGONOMICS_TAG = "GUN_ERGONOMICS_TAG";

	public const string GUN_ACCURACY_TAG = "GUN_ACCURACY_TAG";

	public const string GUN_CURRENT_FIREPOWER_TAG = "GUN_CURRENT_FIREPOWER_TAG";

	public const string GUN_CURRENT_STYLE_TAG = "GUN_CURRENT_STYLE_TAG";

	public const string GUN_CURRENT_RELIABILITY_TAG = "GUN_CURRENT_RELIABILITY_TAG";

	public const string GUN_CURRENT_RECOIL_TAG = "GUN_CURRENT_RECOIL_TAG";

	public const string GUN_CURRENT_ERGONOMICS_TAG = "GUN_CURRENT_ERGONOMICS_TAG";

	public const string GUN_CURRENT_ACCURACY_TAG = "GUN_CURRENT_ACCURACY_TAG";

	public const string GUN_CALIBER_TAG = "GUN_CALIBER_TAG";

	public const string GUN_SIZE_TAG = "GUN_SIZE_TAG";

	public const string GUN_SUPPRESSED_TAG = "GUN_SUPPRESSED_TAG";

	public const int SUPPRESSED_NONE = 0;

	public const int SUPPRESSED_MAKESHIFT = 1;

	public const int SUPPRESSED_PROPER = 2;

	public const int CALIBER_22SR = 22;

	public const int CALIBER_10MM = 10;

	public const int CALIBER_45AFP = 45;

	public const int CALIBER_12GAUGE = 12;

	public const string ADV_GUN_TAG = "ADV_GUN_TAG";

	public const string GUN_REPAIRBILITY_TAG = "GUN_REPAIRBILITY_TAG";

	public const string GUN_CURRENT_REPAIRBILITY_TAG = "GUN_CURRENT_REPAIRBILITY_TAG";

	public static bool HasGunInside(GameItem container)
	{
		if (container == null)
		{
			return false;
		}
		return GraphUtils.FindChildrenType(container, (GameItem item) => IsSelfDefenceGun(item)).Count > 0;
	}

	public static bool IsSelfDefenceGun(GameItem item)
	{
		if (item != null)
		{
			if (!item.IsGameItemType("FIREARM"))
			{
				return item.identifier == "stun_gun";
			}
			return true;
		}
		return false;
	}

	public static bool IsServiceGun(GameItem item)
	{
		return item?.IsTag("SERVICE_GUN_TAG") ?? false;
	}

	public static List<GameItem> GetPlayerParts(GameItem serviceGun)
	{
		List<GameItem> list = new List<GameItem>();
		if (!IsServiceGun(serviceGun))
		{
			return list;
		}
		foreach (GameItem item in serviceGun.FindAllChildItems())
		{
			if (GeneralHelper.IsItemOwned(item))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static void ReturnPlayerParts(GameItem serviceGun)
	{
		foreach (GameItem playerPart in GetPlayerParts(serviceGun))
		{
			GraphUtils.EmporiumTryAdd(playerPart);
		}
		if (IsServiceGun(serviceGun))
		{
			int @int = serviceGun.GetTagReadonly("SERVICE_GUN_PARTS_USED_INT").GetInt();
			for (int i = 0; i < @int; i++)
			{
				GraphUtils.EmporiumTryAdd(DirectoryMaster.Item("gun_part"));
			}
			serviceGun.ModifyTag("SERVICE_GUN_PARTS_USED_INT", delegate(TagState state)
			{
				state.Enable().SetInt(0);
			});
			serviceGun.ModifyTag("SERVICE_GUN_REPAIR_VALUE_INT", delegate(TagState state)
			{
				state.Enable().SetInt(0);
			});
		}
	}

	public static void RepairServiceGun(GameItem serviceGun, GameItem part)
	{
		int partValue = (int)part.GetCurrentValue(useRetailMarkup: false, withChild: false, includeEvents: true, forceMarkup: true, 0L);
		ItemFeatureList.EquipmentCondition condition = GunFrameHelper.GetCondition(serviceGun);
		TryRepairGun(serviceGun, part);
		int used = serviceGun.GetTagReadonly("SERVICE_GUN_PARTS_USED_INT").GetInt();
		serviceGun.ModifyTag("SERVICE_GUN_PARTS_USED_INT", delegate(TagState state)
		{
			state.Enable().SetInt(used + 1);
		});
		if (GunFrameHelper.GetCondition(serviceGun) != condition)
		{
			int repairValue = serviceGun.GetTagReadonly("SERVICE_GUN_REPAIR_VALUE_INT").GetInt();
			serviceGun.ModifyTag("SERVICE_GUN_REPAIR_VALUE_INT", delegate(TagState state)
			{
				state.Enable().SetInt(repairValue + partValue);
			});
		}
	}

	public static int GetServiceRepairValue(GameItem gun)
	{
		if (!IsServiceGun(gun))
		{
			return 0;
		}
		return gun.GetTagReadonly("SERVICE_GUN_REPAIR_VALUE_INT").GetInt();
	}

	public static string GetCaliberName(int caliber)
	{
		return caliber switch
		{
			22 => LocHelper.GetLocalizedMechanicOrDefault("mech_caliber_22sr", ".22 SR"), 
			10 => LocHelper.GetLocalizedMechanicOrDefault("mech_caliber_10mm", "10mm"), 
			45 => LocHelper.GetLocalizedMechanicOrDefault("mech_caliber_45afp", ".45 AFP"), 
			12 => LocHelper.GetLocalizedMechanicOrDefault("mech_caliber_12gauge", "12 gauge"), 
			_ => caliber.ToString(), 
		};
	}

	public static bool IsAmmunitionOfCaliber(GameItem item, int caliber)
	{
		if (item == null || !item.IsTag("AMMUNITION"))
		{
			return false;
		}
		if (item.IsTag("GUN_CALIBER_TAG") && item.GetTagReadonly("GUN_CALIBER_TAG").GetInt() == caliber)
		{
			return true;
		}
		if (item.IsTag("AMMO_BOX_CONTENT_CALIBER_TAG") && item.GetTagReadonly("AMMO_BOX_CONTENT_CALIBER_TAG").GetInt() == caliber)
		{
			return true;
		}
		return false;
	}

	public static string IDToString(string id)
	{
		switch (id)
		{
		case "GUN_FIREPOWER_TAG":
		case "GUN_CURRENT_FIREPOWER_TAG":
			return LocHelper.GetLocalizedMechanic("mech_gun_firepower");
		case "GUN_STYLE_TAG":
		case "GUN_CURRENT_STYLE_TAG":
			return LocHelper.GetLocalizedMechanic("mech_gun_style");
		case "GUN_RELIABILITY_TAG":
		case "GUN_CURRENT_RELIABILITY_TAG":
			return LocHelper.GetLocalizedMechanic("mech_gun_reliability");
		case "GUN_RECOIL_TAG":
		case "GUN_CURRENT_RECOIL_TAG":
			return LocHelper.GetLocalizedMechanic("mech_gun_recoil");
		case "GUN_ERGONOMICS_TAG":
		case "GUN_CURRENT_ERGONOMICS_TAG":
			return LocHelper.GetLocalizedMechanic("mech_gun_ergonomics");
		case "GUN_ACCURACY_TAG":
		case "GUN_CURRENT_ACCURACY_TAG":
			return LocHelper.GetLocalizedMechanic("mech_gun_accuracy");
		case "GUN_SIZE_TAG":
			return LocHelper.GetLocalizedMechanic("mech_gun_size");
		case "GUN_SUPPRESSED_TAG":
			return LocHelper.GetLocalizedMechanic("mech_gun_suppressed");
		case "GUN_CALIBER_TAG":
			return LocHelper.GetLocalizedMechanicOrDefault("mech_gun_caliber", "Caliber");
		default:
			return id;
		}
	}

	public static void InitGun(GameItem item, int firepower = 0, int style = 0, int reliability = 0, int recoil = 0, int ergonomics = 0, int accuracy = 0, int caliber = 22, int repairbility = 5, bool moddable = true)
	{
		item.ModifyTag("GUN_FIREPOWER_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(firepower);
		});
		item.ModifyTag("GUN_STYLE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(style);
		});
		item.ModifyTag("GUN_RELIABILITY_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(reliability);
		});
		item.ModifyTag("GUN_RECOIL_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(recoil);
		});
		item.ModifyTag("GUN_ERGONOMICS_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(ergonomics);
		});
		item.ModifyTag("GUN_ACCURACY_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(accuracy);
		});
		item.ModifyTag("GUN_CALIBER_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(caliber);
		});
		item.ModifyTag("GUN_REPAIRBILITY_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(repairbility);
		});
		item.ModifyTag("GUN_CURRENT_REPAIRBILITY_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		item.EnableTag("ADV_GUN_TAG");
		if (moddable)
		{
			item.SetGameItemType("MODDABLE");
		}
		UpdateCurrentGunStats(item);
	}

	public static void MultiplyBaseStat(GameItem item, int multiplier)
	{
		if (item == null)
		{
			Debug.LogError("GunHelper.MultiplyBaseStat: item is null");
			return;
		}
		if (!item.IsTag("ADV_GUN_TAG"))
		{
			Debug.LogError("GunHelper.MultiplyBaseStat: item does not have ADV_GUN_TAG");
			return;
		}
		multiplier += 100;
		item.ModifyTag("GUN_FIREPOWER_TAG", delegate(TagState state)
		{
			state.SetInt(item.GetTagReadonly("GUN_FIREPOWER_TAG").GetInt() * multiplier / 100);
		});
		item.ModifyTag("GUN_STYLE_TAG", delegate(TagState state)
		{
			state.SetInt(item.GetTagReadonly("GUN_STYLE_TAG").GetInt() * multiplier / 100);
		});
		item.ModifyTag("GUN_RELIABILITY_TAG", delegate(TagState state)
		{
			state.SetInt(item.GetTagReadonly("GUN_RELIABILITY_TAG").GetInt() * multiplier / 100);
		});
		item.ModifyTag("GUN_RECOIL_TAG", delegate(TagState state)
		{
			state.SetInt(item.GetTagReadonly("GUN_RECOIL_TAG").GetInt() * multiplier / 100);
		});
		item.ModifyTag("GUN_ERGONOMICS_TAG", delegate(TagState state)
		{
			state.SetInt(item.GetTagReadonly("GUN_ERGONOMICS_TAG").GetInt() * multiplier / 100);
		});
		item.ModifyTag("GUN_ACCURACY_TAG", delegate(TagState state)
		{
			state.SetInt(item.GetTagReadonly("GUN_ACCURACY_TAG").GetInt() * multiplier / 100);
		});
		UpdateCurrentGunStats(item);
	}

	public static void UpdateCurrentGunStats(GameItem item)
	{
		int num = 100;
		int num2 = 100;
		int num3 = 100;
		int num4 = 100;
		int num5 = 100;
		int num6 = 100;
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		int num10 = 0;
		int num11 = 0;
		int num12 = 0;
		int num13 = 0;
		string text = item.FindItemFeatureByCategory("CATEGORY_EQUIPMENT_CONDITION")?.realCondition.identifier;
		if (text != null)
		{
			if (text.Equals("equipment_condition_new"))
			{
				num13 = 30;
			}
			else if (text.Equals("equipment_condition_excellent"))
			{
				num13 = 15;
			}
			else if (text.Equals("equipment_condition_good"))
			{
				num13 = 0;
			}
			else if (text.Equals("equipment_condition_fair"))
			{
				num13 = -15;
			}
			else if (text.Equals("equipment_condition_poor"))
			{
				num13 = -30;
			}
		}
		num += num13;
		num2 += num13;
		num3 += num13;
		num4 += num13;
		num5 += num13;
		num6 += num13;
		item.ModifyTag("GUN_SUPPRESSED_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		foreach (GameItem item2 in GraphUtils.FindAllChildrenType<GameItem>(item))
		{
			if (!item2.IsTag("WEAPON_MOD_TAG"))
			{
				continue;
			}
			num += item2.GetTagReadonly("GUN_FIREPOWER_TAG").GetInt();
			num2 += item2.GetTagReadonly("GUN_STYLE_TAG").GetInt();
			num3 += item2.GetTagReadonly("GUN_RELIABILITY_TAG").GetInt();
			num4 += item2.GetTagReadonly("GUN_RECOIL_TAG").GetInt();
			num5 += item2.GetTagReadonly("GUN_ERGONOMICS_TAG").GetInt();
			num6 += item2.GetTagReadonly("GUN_ACCURACY_TAG").GetInt();
			num7 += item2.GetTagReadonly("MOD_BASE_FIREPOWER_TAG").GetInt();
			num8 += item2.GetTagReadonly("MOD_BASE_STYLE_TAG").GetInt();
			num9 += item2.GetTagReadonly("MOD_BASE_RELIABILITY_TAG").GetInt();
			num10 += item2.GetTagReadonly("MOD_BASE_RECOIL_TAG").GetInt();
			num11 += item2.GetTagReadonly("MOD_BASE_ERGONOMICS_TAG").GetInt();
			num12 += item2.GetTagReadonly("MOD_BASE_ACCURACY_TAG").GetInt();
			if (!item2.IsTag("MOD_SUPPRESSOR_TAG"))
			{
				continue;
			}
			int level = (item2.IsGameItemType("MAKESHIFT") ? 1 : 2);
			int @int = item.GetTagReadonly("GUN_SUPPRESSED_TAG").GetInt();
			if (level > @int)
			{
				item.ModifyTag("GUN_SUPPRESSED_TAG", delegate(TagState state)
				{
					state.Enable().SetInt(level);
				});
			}
		}
		int firepower = item.GetTagReadonly("GUN_FIREPOWER_TAG").GetInt() * num / 100 + num7;
		int style = item.GetTagReadonly("GUN_STYLE_TAG").GetInt() * num2 / 100 + num8;
		int reliability = item.GetTagReadonly("GUN_RELIABILITY_TAG").GetInt() * num3 / 100 + num9;
		int recoil = item.GetTagReadonly("GUN_RECOIL_TAG").GetInt() * num4 / 100 + num10;
		int ergonomics = item.GetTagReadonly("GUN_ERGONOMICS_TAG").GetInt() * num5 / 100 + num11;
		int accuracy = item.GetTagReadonly("GUN_ACCURACY_TAG").GetInt() * num6 / 100 + num12;
		item.ModifyTag("GUN_CURRENT_FIREPOWER_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(firepower);
		});
		item.ModifyTag("GUN_CURRENT_STYLE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(style);
		});
		item.ModifyTag("GUN_CURRENT_RELIABILITY_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(reliability);
		});
		item.ModifyTag("GUN_CURRENT_RECOIL_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(recoil);
		});
		item.ModifyTag("GUN_CURRENT_ERGONOMICS_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(ergonomics);
		});
		item.ModifyTag("GUN_CURRENT_ACCURACY_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(accuracy);
		});
		item.ModifyTag("GUN_SIZE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(item.GetModifiedItemSize());
		});
	}

	public static void CreateTooltipLine(RichTextBuilder builder, GameItem item, string tag, string currentTag, string label)
	{
		if (item.IsTag(tag))
		{
			int @int = item.GetTagReadonly(tag).GetInt();
			int int2 = item.GetTagReadonly(currentTag).GetInt();
			builder.Add(label + ": ", RenderHandler.ColorPalette.LightBlue);
			builder.Add(int2.ToString() ?? "", GetColor(int2, @int));
			if (int2 != @int)
			{
				builder.AddLine("(" + @int + ")", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue);
			}
			else
			{
				builder.AddLine();
			}
		}
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("ADV_GUN_TAG"))
		{
			CreateTooltipLine(builder, item, "GUN_FIREPOWER_TAG", "GUN_CURRENT_FIREPOWER_TAG", IDToString("GUN_FIREPOWER_TAG"));
			CreateTooltipLine(builder, item, "GUN_STYLE_TAG", "GUN_CURRENT_STYLE_TAG", IDToString("GUN_STYLE_TAG"));
			CreateTooltipLine(builder, item, "GUN_RELIABILITY_TAG", "GUN_CURRENT_RELIABILITY_TAG", IDToString("GUN_RELIABILITY_TAG"));
			CreateTooltipLine(builder, item, "GUN_RECOIL_TAG", "GUN_CURRENT_RECOIL_TAG", IDToString("GUN_RECOIL_TAG"));
			CreateTooltipLine(builder, item, "GUN_ERGONOMICS_TAG", "GUN_CURRENT_ERGONOMICS_TAG", IDToString("GUN_ERGONOMICS_TAG"));
			CreateTooltipLine(builder, item, "GUN_ACCURACY_TAG", "GUN_CURRENT_ACCURACY_TAG", IDToString("GUN_ACCURACY_TAG"));
		}
	}

	public static void SetEquipmentCondition(GameItem item, ItemFeatureList.EquipmentCondition condition)
	{
		if (item == null)
		{
			Debug.LogError("GunHelper.SetEquipmentCondition: item is null");
			return;
		}
		item.RemoveFeatureByCategory("CATEGORY_EQUIPMENT_CONDITION");
		ItemFeature itemFeature = ItemFeatureList.EquipementConditionFeature(condition);
		item.AddItemFeature(itemFeature);
		UpdateCurrentGunStats(item);
	}

	public static void ImproveEquipmentCondition(GameItem item)
	{
		if (item == null)
		{
			Debug.LogError("GunHelper.ImproveEquipmentCondition: item is null");
			return;
		}
		ItemFeature itemFeature = item.FindItemFeatureByCategory("CATEGORY_EQUIPMENT_CONDITION");
		if (itemFeature == null)
		{
			Debug.LogError("GunHelper.ImproveEquipmentCondition: item does not have equipment condition feature");
			return;
		}
		ItemCondition realCondition = itemFeature.realCondition;
		if (realCondition.identifier.Equals("equipment_condition_poor"))
		{
			SetEquipmentCondition(item, ItemFeatureList.EquipmentCondition.Fair);
		}
		else if (realCondition.identifier.Equals("equipment_condition_fair"))
		{
			SetEquipmentCondition(item, ItemFeatureList.EquipmentCondition.Good);
		}
		else if (realCondition.identifier.Equals("equipment_condition_good"))
		{
			SetEquipmentCondition(item, ItemFeatureList.EquipmentCondition.Excellent);
		}
	}

	public static RenderHandler.ColorPalette GetColor(int valueCurrent, int valueNormal)
	{
		RenderHandler.ColorPalette result = RenderHandler.ColorPalette.LightOrange;
		if (valueCurrent > valueNormal)
		{
			result = RenderHandler.ColorPalette.LightGreen;
		}
		else if (valueCurrent == valueNormal)
		{
			result = RenderHandler.ColorPalette.LightBlue;
		}
		return result;
	}

	public static void DisassembleWeapon(GameItem targetItem)
	{
		if (!targetItem.IsTag("ADV_GUN_TAG"))
		{
			return;
		}
		GameInventory parentInventory = targetItem.parentInventory;
		if (parentInventory == null)
		{
			return;
		}
		targetItem.TryUnloadAll();
		targetItem.Destroy();
		if (targetItem.IsID("printed_gun"))
		{
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("printer_plastic"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("printer_plastic"));
		}
		else if (targetItem.IsID("handmade_pistol"))
		{
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("printer_plastic"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("scrap_metal"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("scrap_metal"));
		}
		else if (targetItem.IsID("heavy_handmade_pistol") || targetItem.IsID("heavy_handmade_pistol_s"))
		{
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("printer_plastic"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("scrap_metal"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("scrap_metal"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("scrap_metal"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("scrap_metal"));
			if (targetItem.IsID("heavy_handmade_pistol_s"))
			{
				GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			}
		}
		else if (targetItem.IsID("glock_receiver") || targetItem.IsID("revolver_short_tac"))
		{
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			if (targetItem.IsID("revolver_short_tac"))
			{
				GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			}
		}
		else if (targetItem.IsID("revolver"))
		{
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
		}
		else if (targetItem.IsID("smg"))
		{
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
		}
		else if (targetItem.IsID("shotgun"))
		{
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("gun_part"));
		}
		EmporiumAudioPlayer.Instance.ratchet.Play();
	}

	public static void TryRepairGun(GameItem targetItem, GameItem sourceItem)
	{
		int newRepairbility = GetRefurbishChance(targetItem);
		if (RNG.Roll(newRepairbility))
		{
			newRepairbility = 0;
			AudioManager.Instance.Play2("modding1");
			ImproveEquipmentCondition(targetItem);
		}
		else
		{
			AudioManager.Instance.Play2("modding2");
		}
		sourceItem.Destroy();
		targetItem.ModifyTag("GUN_CURRENT_REPAIRBILITY_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(newRepairbility);
		});
		UpdateCurrentGunStats(targetItem);
	}

	public static bool CanRepair(GameItem targetItem)
	{
		if (targetItem == null || !targetItem.IsTag("ADV_GUN_TAG"))
		{
			return false;
		}
		ItemFeature itemFeature = targetItem.FindItemFeatureByCategory("CATEGORY_EQUIPMENT_CONDITION");
		if (itemFeature == null)
		{
			return false;
		}
		ItemCondition realCondition = itemFeature.realCondition;
		if (!realCondition.identifier.Equals("equipment_condition_new") && !realCondition.identifier.Equals("equipment_condition_excellent"))
		{
			return true;
		}
		return false;
	}

	public static int GetRefurbishChance(GameItem item)
	{
		if (item == null || !item.IsTag("ADV_GUN_TAG"))
		{
			return 0;
		}
		int @int = item.GetTagReadonly("GUN_CURRENT_REPAIRBILITY_TAG").GetInt();
		int int2 = item.GetTagReadonly("GUN_REPAIRBILITY_TAG").GetInt();
		return @int + int2;
	}
}
