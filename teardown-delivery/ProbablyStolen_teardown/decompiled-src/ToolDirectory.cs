using System;
using System.Collections.Generic;
using UnityEngine;

public class ToolDirectory : ItemDirectory
{
	public const string SURGERY_TOOL_TAG = "SURGERY_TOOL_TAG";

	public const string MAGNIFYING_GLASS = "MAGNIFYING_GLASS";

	public const string LABELER = "LABELER";

	public const string LABELER_BLUE = "LABELER_BLUE";

	protected override void InitDirectory()
	{
		Add("screwdriver", () => CreateScrewdriver());
		Add("welder", () => CreateWelder());
		Add("wire_cutter", () => CreateWireCutter());
		Add("toolbox", () => CreateToolbox());
		Add("labeler", () => CreateLabeler());
		Add("labeler_blue", () => CreateLabelerBlue());
		Add("intake_hopper", () => HopperHelper.CreateIntake());
		Add("outlet_hopper", () => HopperHelper.CreateOutlet());
		Add("magnifier", () => CreateMagnifier());
		Add("turbo_booster", () => CreateTurboBooster());
		Add("turbo_booster_adv", () => CreateTurboBoosterAdv());
		Add("turbo_booster_hypertech", () => CreateTurboBoosterHypertech());
		Add("mind_reprogrammer", () => CreateMindReprogrammer());
		Add("sign_water", () => CreateWaterSign());
		Add("sign_food", () => CreateFoodSign());
		Add("sign_medical", () => CreateMedicalSign());
		Add("sign_household", () => CreateHouseholdSign());
		Add("sign_material", () => CreateMaterialSign());
		Add("sign_ruined_machine", () => CreateRuinedMachineSign());
		Add("sign_firearm", () => CreateFirearmSign());
		Add("sign_firearm_heavy", () => CreateHeavyFirearmSign());
		Add("sign_berry", () => CreateBerrySign());
		Add("xray_scanner", () => CreateXRayScanner());
		Add("sign_expedition", () => CreateExpeditionSign());
	}

	private GameItem CreateScrewdriver()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "screwdriver_s");
		gameItem.name = LocHelper.GetLocalizedItem("item_screwdriver_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_screwdriver_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_screwdriver_desc");
		gameItem.SetGameItemType("TOOL");
		gameItem.unitValue = 25L;
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => (t.IsTag("BROKEN_MACHINE_TAG") || t.IsID("system_module_ruined") || t.IsTag("ADV_GUN_TAG")) && GeneralHelper.IsItemOwned(t)));
		gameItem.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => (t.IsTag("BROKEN_MACHINE_TAG") || t.IsID("system_module_ruined") || t.IsTag("ADV_GUN_TAG")) && GeneralHelper.IsItemOwned(t)));
		gameItem.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (t.IsTag("ADV_GUN_TAG"))
			{
				GunHelper.DisassembleWeapon(t);
			}
			else if (t.IsID("system_module_ruined"))
			{
				GameInventory parentInventory = s.parentInventory;
				t.Destroy();
				GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("common_electronic"));
				GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("printer_plastic"));
				GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("junk"));
			}
			else if (t.IsTag("BROKEN_MACHINE_TAG"))
			{
				MachineBrokenHelper.OnScrewdriverUsed(t, s);
			}
		});
		return gameItem;
	}

	private GameItem CreateToolbox()
	{
		(PixelWindow, List<GameSlotInventory>, List<MachineryLabel>) tuple = ToolboxHelper.CreateToolboxWindow();
		PixelWindow item = tuple.Item1;
		List<GameSlotInventory> item2 = tuple.Item2;
		List<MachineryLabel> item3 = tuple.Item3;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_tool2", "toolbox_old");
		gameItem.name = LocHelper.GetLocalizedItem("item_toolbox_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_toolbox_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_toolbox_desc");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.SetGameItemType("STORAGE");
		gameItem.unitValue = 150L;
		ToolboxHelper.InitToolbox(gameItem, item, item2, item3);
		AudioHelper.InitSmallMachine(gameItem);
		return gameItem;
	}

	private GameItem CreateWelder()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "welder_s");
		gameItem.name = LocHelper.GetLocalizedItem("item_welder_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_welder_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_welder_desc");
		gameItem.SetGameItemType("TOOL");
		gameItem.unitValue = 80L;
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("BROKEN_MACHINE_TAG") && GeneralHelper.IsItemOwned(t)));
		gameItem.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("BROKEN_MACHINE_TAG") && GeneralHelper.IsItemOwned(t)));
		gameItem.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			MachineBrokenHelper.OnWelderUsed(t, s);
		});
		DebrisHelper.InitClearingTool(gameItem);
		return gameItem;
	}

	private GameItem CreateWireCutter()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "wirecutter");
		gameItem.name = LocHelper.GetLocalizedItem("item_wire_cutter_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_wire_cutter_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_wire_cutter_desc");
		gameItem.SetGameItemType("TOOL");
		gameItem.unitValue = 40L;
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => CanWireCutterTargetItem(s, t)));
		gameItem.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => CanWireCutterTargetItem(s, t)));
		gameItem.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			OnWireCutterTargetItem(s, t);
		});
		DebrisHelper.InitClearingTool(gameItem);
		return gameItem;
	}

	public static bool CanWireCutterTargetItem(GameItem source, GameItem target)
	{
		if (target.IsTag("BROKEN_MACHINE_TAG") && GeneralHelper.IsItemOwned(target))
		{
			return true;
		}
		if (target.IsTag("ANIMAL_ITEM_TAG") && GeneralHelper.IsItemOwned(target) && !target.IsTag("ANIMAL_IS_DEAD_TAG") && !target.IsTag("ANIMAL_NEUTERED_TAG"))
		{
			return true;
		}
		return false;
	}

	public static void OnWireCutterTargetItem(GameItem source, GameItem target)
	{
		if (target.IsTag("BROKEN_MACHINE_TAG"))
		{
			MachineBrokenHelper.OnWireCutterUsed(target, source);
		}
		else if (target.IsTag("ANIMAL_ITEM_TAG") && !target.IsTag("ANIMAL_IS_DEAD_TAG") && !target.IsTag("ANIMAL_NEUTERED_TAG"))
		{
			target.EnableTag("ANIMAL_NEUTERED_TAG");
			string text = HusbandryHelper.GetSpecies(target)?.killSoundEffect;
			if (text != null)
			{
				AudioManager.Instance.Play2(text);
			}
		}
	}

	private GameItem CreateXRayScanner()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "spectral_analyser");
		gameItem.name = LocHelper.GetLocalizedItem("item_xray_scanner_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_xray_scanner_desc");
		gameItem.SetGameItemType("TOOL");
		gameItem.unitValue = 400L;
		gameItem.EnableTag("CAN_USE_ON_NON_OWNED_TAG");
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.GetTagReadonly("LOCK_STATUS").GetBool() && t.IsGameItemType("SUPPLY_CRATE")));
		gameItem.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.GetTagReadonly("LOCK_STATUS").GetBool() && t.IsGameItemType("SUPPLY_CRATE")));
		gameItem.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			EmporiumEntry.Instance.HideResponseWindow();
			StoreResponse.ShowSupplyCrateResponse(t);
		});
		return gameItem;
	}

	private GameItem CreateLabeler()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "labeler");
		gameItem.name = LocHelper.GetLocalizedItem("item_labeler_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_labeler_desc");
		gameItem.unitValue = 50L;
		gameItem.EnableTag("MAGNIFYING_GLASS");
		LabelerHelper.InitLabelerItem(gameItem);
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(t) && LabelerHelper.GetEditableLabelCount(t) > 0));
		gameItem.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(t) && LabelerHelper.GetEditableLabelCount(t) > 0));
		gameItem.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			LabelerHelper.OnTargetItem(t);
		});
		return gameItem;
	}

	private GameItem CreateLabelerBlue()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "labeler_blue");
		gameItem.name = LocHelper.GetLocalizedItem("item_labeler_blue_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_labeler_blue_desc");
		gameItem.unitValue = 50L;
		gameItem.EnableTag("LABELER_BLUE");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s, t)));
		gameItem.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s, t)));
		gameItem.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (!t.identifier.Equals("dossier"))
			{
				LabelerBlueUIManager.Instance.OpenUI(t);
			}
		});
		return gameItem;
	}

	private GameItem CreateMagnifier()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "magnifier");
		gameItem.name = LocHelper.GetLocalizedItem("item_magnifier_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_magnifier_desc");
		gameItem.unitValue = 50L;
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("MAGNIFYING_GLASS");
		gameItem.EnableTag("CAN_USE_ON_NON_OWNED_TAG");
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("INSPECTABLE_TAG")));
		gameItem.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("INSPECTABLE_TAG")));
		gameItem.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			InspectionUIManager.Instance.OnInspectItem(t);
		});
		return gameItem;
	}

	private GameItem CreateRuinedMachineSign()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sign", "material_sign");
		gameItem.name = "Scrap Machine Sign";
		gameItem.shortDescription = "Put it in the display case to attract scrappers that sell Ruined Machines.";
		gameItem.unitValue = 100L;
		gameItem.EnableTag("IMPORTANT_TAG");
		ShowcaseHelper.InitShowcaseSpecialItem(gameItem, "MACHINE");
		return gameItem;
	}

	private GameItem CreateExpeditionSign()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sign", "hiring_sign");
		gameItem.name = LocHelper.GetLocalizedItem("item_sign_expedition_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_sign_expedition_desc");
		gameItem.unitValue = 50L;
		gameItem.EnableTag("IMPORTANT_TAG");
		return gameItem;
	}

	private GameItem CreateFirearmSign()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sign", "gun_sign");
		gameItem.name = LocHelper.GetLocalizedOrDefault("Item", "item_sign_firearm_name", "Firearm Neon Sign");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_sign_generic_desc", TypeHelper.GetTypeDisplayName("FIREARM"));
		gameItem.unitValue = 100L;
		gameItem.EnableTag("IMPORTANT_TAG");
		ShowcaseHelper.InitShowcaseSpecialItem(gameItem, "FIREARM");
		return gameItem;
	}

	private GameItem CreateHeavyFirearmSign()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sign", "gun_sign2");
		gameItem.name = LocHelper.GetLocalizedOrDefault("Item", "item_sign_firearm_heavy_name", "Heavy Firearm Neon Sign");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_sign_generic_desc", TypeHelper.GetTypeDisplayName("FIREARM"));
		gameItem.unitValue = 200L;
		gameItem.EnableTag("IMPORTANT_TAG");
		ShowcaseHelper.InitShowcaseSpecialItem(gameItem, "FIREARM");
		return gameItem;
	}

	private GameItem CreateWaterSign()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sign", "water_sign");
		gameItem.name = LocHelper.GetLocalizedItem("item_sign_water_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_sign_generic_desc", TypeHelper.GetTypeDisplayName("WATER"));
		gameItem.unitValue = 100L;
		gameItem.EnableTag("IMPORTANT_TAG");
		ShowcaseHelper.InitShowcaseSpecialItem(gameItem, "WATER");
		return gameItem;
	}

	private GameItem CreateFoodSign()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sign", "food_sign");
		gameItem.name = LocHelper.GetLocalizedItem("item_sign_food_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_sign_generic_desc", TypeHelper.GetTypeDisplayName("FOOD"));
		gameItem.unitValue = 100L;
		gameItem.EnableTag("IMPORTANT_TAG");
		ShowcaseHelper.InitShowcaseSpecialItem(gameItem, "FOOD");
		return gameItem;
	}

	private GameItem CreateMedicalSign()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sign", "medical_sign");
		gameItem.name = LocHelper.GetLocalizedItem("item_sign_medical_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_sign_generic_desc", TypeHelper.GetTypeDisplayName("MEDICAL"));
		gameItem.unitValue = 200L;
		gameItem.EnableTag("IMPORTANT_TAG");
		ShowcaseHelper.InitShowcaseSpecialItem(gameItem, "MEDICAL");
		return gameItem;
	}

	private GameItem CreateHouseholdSign()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sign", "household_sign");
		gameItem.name = LocHelper.GetLocalizedItem("item_sign_household_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_sign_generic_desc", TypeHelper.GetTypeDisplayName("HOUSEHOLD_GOOD"));
		gameItem.unitValue = 100L;
		gameItem.EnableTag("IMPORTANT_TAG");
		ShowcaseHelper.InitShowcaseSpecialItem(gameItem, "HOUSEHOLD_GOOD");
		return gameItem;
	}

	private GameItem CreateLuxurySign()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sign", "luxury_sign");
		gameItem.name = LocHelper.GetLocalizedItem("item_sign_luxury_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_sign_generic_desc", TypeHelper.GetTypeDisplayName("LUXURY_ITEM"));
		gameItem.unitValue = 300L;
		gameItem.EnableTag("IMPORTANT_TAG");
		ShowcaseHelper.InitShowcaseSpecialItem(gameItem, "LUXURY_ITEM");
		return gameItem;
	}

	private GameItem CreateMaterialSign()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sign", "material_sign");
		gameItem.name = LocHelper.GetLocalizedItem("item_sign_material_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_sign_generic_desc", TypeHelper.GetTypeDisplayName("MATERIAL"));
		gameItem.unitValue = 100L;
		gameItem.EnableTag("IMPORTANT_TAG");
		ShowcaseHelper.InitShowcaseSpecialItem(gameItem, "MATERIAL");
		return gameItem;
	}

	private GameItem CreateBerrySign()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sign", "berry_sign");
		gameItem.name = LocHelper.GetLocalizedItem("item_sign_berry_name");
		gameItem.unitValue = 100L;
		gameItem.EnableTag("IMPORTANT_TAG");
		return gameItem;
	}

	private GameItem CreateTurboBooster()
	{
		GameItem item2 = Empty().SetSpriteAndShape("Items/items_tool", "turbo_booster");
		item2.name = LocHelper.GetLocalizedItem("item_turbo_booster_name");
		item2.shortDescription = LocHelper.GetLocalizedItem("item_turbo_booster_desc");
		item2.SetGameItemType("TOOL");
		item2.unitValue = 35L;
		item2.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item2.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		GameItem gameItem = item2;
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(t) && GeneralHelper.IsItemOwned(s) && MachineTurboBoosterAdv.IsTurboBoostEligible(t)));
		GameItem gameItem2 = item2;
		gameItem2.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem2.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(t) && GeneralHelper.IsItemOwned(s) && MachineTurboBoosterAdv.CanBoostNow(t)));
		GameItem gameItem3 = item2;
		gameItem3.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem3.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			t.onCycleEndEarlySlotItemFunc?.Invoke(t, t.parentInventory, t.GetIncompleteSlotInInventory());
			t.onCycleEndSlotItemFunc?.Invoke(t, t.parentInventory, t.GetIncompleteSlotInInventory());
			AudioManager.Instance.Play2("turbo");
			item2.Destroy();
		});
		AudioHelper.InitPlasticMachinery(item2);
		return item2;
	}

	private GameItem CreateTurboBoosterAdv()
	{
		(PixelWindow, GameSlotInventory) tuple = MachineTurboBoosterAdv.CreateTurboBoosterWindow(15, 15, isDraggable: true, isCentered: true, 15);
		PixelWindow item3 = tuple.Item1;
		GameSlotInventory batterySlot = tuple.Item2;
		GameItem item2 = ItemDirectory.CreateEmptyItem().SetContentWindow(item3).SetSpriteAndShape("Items/items_tool", "turbo_booster_adv");
		item2.name = LocHelper.GetLocalizedItem("item_turbo_booster_adv_name");
		item2.shortDescription = LocHelper.GetLocalizedItem("item_turbo_booster_adv_desc");
		item2.SetGameItemType("TOOL");
		item2.unitValue = 300L;
		item2.EnableTag("TURBO_BOOST_ADV");
		item2.ModifyTag("machinery_drawn_power", delegate(TagState state)
		{
			state.Enable().SetInt(5);
		});
		item2.ModifyTag("MACHINERY_DRAWN_POWER_BASE", delegate(TagState state)
		{
			state.Enable().SetInt(5);
		});
		MachineHelper.SetupBatterySlot(batterySlot, item2);
		item2.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item2.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		GameItem gameItem = item2;
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(t) && GeneralHelper.IsItemOwned(s) && ((MachineTurboBoosterAdv.IsTurboReady(s) && MachineTurboBoosterAdv.IsTurboBoostEligible(t)) || MachineTurboBoosterAdv.CanUpgradeHypertech(t))));
		GameItem gameItem2 = item2;
		gameItem2.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem2.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(t) && GeneralHelper.IsItemOwned(s) && ((MachineTurboBoosterAdv.IsTurboReady(s) && MachineTurboBoosterAdv.CanBoostNow(t)) || MachineTurboBoosterAdv.CanUpgradeHypertech(t))));
		GameItem gameItem3 = item2;
		gameItem3.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem3.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (MachineTurboBoosterAdv.CanUpgradeHypertech(t))
			{
				MachineTurboBoosterAdv.UpgradeHypertech(t);
				AudioManager.Instance.Play2("turbo");
				item2.Destroy();
			}
			else
			{
				MachineTurboBoosterAdv.BoostMachine(t);
				item2.DisableTag("TURBO_READY_TAG");
				item2.ModifyTag("CURRENT_CHARGE_TAG", delegate(TagState state)
				{
					state.Enable().SetInt(0);
				});
				MachineTurboBoosterAdv.UpdateSprite(item2, batterySlot);
				AudioManager.Instance.Play2("turbo");
			}
		});
		item2.onCycleEndSlotItemFunc = delegate(GameItem item, GameInventory parentInventory, SlotMarker slot)
		{
			int currentCharge = item.GetTagReadonly("CURRENT_CHARGE_TAG").GetInt();
			if (currentCharge >= 15)
			{
				item.EnableTag("TURBO_READY_TAG");
			}
			else if (batterySlot.childItem != null && PowerHelper.DrawPowerSource(batterySlot.childItem, 5))
			{
				currentCharge += 5;
				item.ModifyTag("CURRENT_CHARGE_TAG", delegate(TagState state)
				{
					state.Enable().SetInt(currentCharge);
				});
				if (currentCharge >= 15)
				{
					item.EnableTag("TURBO_READY_TAG");
				}
			}
			MachineTurboBoosterAdv.UpdateSprite(item, batterySlot);
		};
		GameSlotInventory gameSlotInventory = batterySlot;
		gameSlotInventory.onSlotAddItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameSlotInventory.onSlotAddItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
		{
			MachineTurboBoosterAdv.UpdateSprite(item2, batterySlot);
		});
		GameSlotInventory gameSlotInventory2 = batterySlot;
		gameSlotInventory2.onSlotRemoveItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameSlotInventory2.onSlotRemoveItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
		{
			MachineTurboBoosterAdv.UpdateSprite(item2, batterySlot);
		});
		AudioHelper.InitPlasticMachinery(item2);
		ContainerHelper.AddContainerTooltipTag(item2);
		return item2;
	}

	private GameItem CreateTurboBoosterHypertech()
	{
		(PixelWindow, GameSlotInventory) tuple = MachineTurboBoosterAdv.CreateTurboBoosterWindow(15, 15, isDraggable: true, isCentered: true, 15);
		PixelWindow item3 = tuple.Item1;
		GameSlotInventory batterySlot = tuple.Item2;
		GameItem item2 = Empty().SetContentWindow(item3).SetSpriteAndShape("Items/items_tool2", "turbo_booster_hypertech");
		item2.name = LocHelper.GetLocalizedItem("item_turbo_booster_hypertech_name");
		item2.shortDescription = LocHelper.GetLocalizedItem("item_turbo_booster_hypertech_desc");
		item2.SetGameItemType("TOOL");
		item2.unitValue = 15000L;
		item2.EnableTag("TURBO_BOOST_HYPERTECH");
		item2.ModifyTag("CURRENT_CHARGE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		item2.ModifyTag("HYPERTECH_CHARGE_RATE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(5);
		});
		MachineTurboBoosterAdv.SetHypertechPowerTags(item2);
		MachineHelper.SetupBatterySlot(batterySlot, item2);
		item2.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item2.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		GameItem gameItem = item2;
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(t) && GeneralHelper.IsItemOwned(s) && MachineTurboBoosterAdv.IsTurboReady(s) && MachineTurboBoosterAdv.IsTurboBoostEligible(t)));
		GameItem gameItem2 = item2;
		gameItem2.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem2.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GeneralHelper.IsItemOwned(t) && GeneralHelper.IsItemOwned(s) && MachineTurboBoosterAdv.IsTurboReady(s) && MachineTurboBoosterAdv.CanBoostNow(t)));
		GameItem gameItem3 = item2;
		gameItem3.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem3.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			MachineTurboBoosterAdv.BoostMachine(t);
			int remaining = Mathf.Max(0, item2.GetTagReadonly("CURRENT_CHARGE_TAG").GetInt() - 15);
			item2.ModifyTag("CURRENT_CHARGE_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(remaining);
			});
			MachineTurboBoosterAdv.RefreshHypertechReady(item2);
			AudioManager.Instance.Play2("turbo");
		});
		item2.onCycleEndSlotItemFunc = delegate(GameItem item, GameInventory parentInventory, SlotMarker slot)
		{
			int hypertechChargeRate = MachineTurboBoosterAdv.GetHypertechChargeRate(item);
			if (batterySlot.childItem != null && PowerHelper.DrawPowerSource(batterySlot.childItem, hypertechChargeRate))
			{
				int currentCharge = item.GetTagReadonly("CURRENT_CHARGE_TAG").GetInt() + hypertechChargeRate;
				item.ModifyTag("CURRENT_CHARGE_TAG", delegate(TagState state)
				{
					state.Enable().SetInt(currentCharge);
				});
			}
			MachineTurboBoosterAdv.RefreshHypertechReady(item);
		};
		AudioHelper.InitPlasticMachinery(item2);
		ContainerHelper.AddContainerTooltipTag(item2);
		return item2;
	}

	private GameItem CreateMindReprogrammer()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "mind_reprogrammer");
		gameItem.name = LocHelper.GetLocalizedItem("item_mind_reprogrammer_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_mind_reprogrammer_desc");
		gameItem.SetGameItemType("TOOL");
		gameItem.unitValue = 5000L;
		MindReprogrammerHelper.InitMindReprogrammer(gameItem);
		AudioHelper.InitPlasticMachinery(gameItem);
		ContainerHelper.AddContainerTooltipTag(gameItem);
		return gameItem;
	}
}
