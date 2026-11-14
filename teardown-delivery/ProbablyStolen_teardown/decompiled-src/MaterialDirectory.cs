using System;

public class MaterialDirectory : ItemDirectory
{
	public const string ORE_TAG = "ORE_TAG";

	public const string SCRAP_METAL = "SCRAP_METAL";

	public const string MATERIAL_TAG = "MATERIAL_TAG";

	protected override void InitDirectory()
	{
		Add("metal_ingot", () => MetalIngot());
		Add("common_chemical", () => CommonChemicalSupplies());
		Add("restricted_chemical", () => RestrictedChemicalSupplies());
		Add("common_medical", () => CommonMedicalSupplies());
		Add("restricted_medical", () => RestrictedMedicalSupplies());
		Add("scrap_metal", () => ScrapMetal());
		Add("junk", () => Junk());
		Add("common_ore", () => MetalOre());
		Add("rare_ore", () => RareMetalOre());
		Add("common_electronic", () => Electronic());
		Add("rare_electronic", () => RareElectronic());
		Add("hypertech_electronic", () => HypertechElectronic());
		Add("wire", () => Wire());
		Add("printer_plastic", () => PrinterPlastic());
		Add("flux_agent", () => FluxAgent());
		Add("advanced_flux_agent", () => AdvancedFluxAgent());
		Add("gun_part", () => GunPart());
		Add("nuclear_waste", () => NuclearWaste());
	}

	private GameItem MetalIngot()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_material", "metal_ingot");
		gameItem.name = LocHelper.GetLocalizedItem("item_metal_ingot_name");
		gameItem.SetType("MATERIAL_TAG");
		gameItem.SetGameItemType("MATERIAL");
		gameItem.SetValue(50L);
		AudioHelper.InitSolidMetal(gameItem);
		IngotPurityHelper.InitIngotItem(gameItem);
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayThisTargetItemFunc = (GameItem s, GameItem t) => CanIngotTarget(s, t);
		gameItem.canThisTargetItemFunc = (GameItem s, GameItem t) => CanIngotTarget(s, t);
		gameItem.onThisTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			OnIngotTarget(s, t);
		};
		return gameItem;
	}

	public static bool CanIngotTarget(GameItem source, GameItem target)
	{
		if (target.identifier.Equals("vending_machine_core") && VendingMachineHelper.CanUpgradedCapacity(GeneralHelper.GetParentItem(target)))
		{
			return true;
		}
		if (target.IsTag("CAGE_TAG") && AnimalCage.CanUpgradeSize(target))
		{
			return true;
		}
		if (WaterTankHelper.CanUpgrade(target))
		{
			return true;
		}
		if (target.IsTag("RAS_TAG") && MachineRAS.CanUpgradeHeight(target))
		{
			return true;
		}
		return false;
	}

	public static void OnIngotTarget(GameItem source, GameItem target)
	{
		if (target.identifier.Equals("vending_machine_core"))
		{
			VendingMachineHelper.UpgradeCapacity(target, source);
		}
		else if (target.IsTag("CAGE_TAG"))
		{
			AnimalCage.UpgradeSize(target, source);
		}
		else if (WaterTankHelper.IsTank(target))
		{
			WaterTankHelper.Upgrade(target, source);
		}
		else if (target.IsTag("RAS_TAG"))
		{
			MachineRAS.UpgradeHeight(target, source);
		}
	}

	private GameItem CommonChemicalSupplies()
	{
		GameItem item = Empty().SetSpriteAndShape("Items/items_material", "chemical1");
		item.name = LocHelper.GetLocalizedItem("item_common_chemical_name");
		item.SetGameItemType("CHEMICAL_SUPPLIES");
		item.SetValue(50L);
		item.shortDescription = LocHelper.GetLocalizedItem("item_common_chemical_desc");
		item.flavorText = LocHelper.GetLocalizedItem("item_common_chemical_flavor");
		item.canActivateSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s);
		item.mayActivateSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s);
		item.onActivateSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			UnpackOrWarn(delegate
			{
				item.Destroy();
				GraphUtils.EmporiumTryAdd(curInv, PreBuildChemHelper.CreateMiniAlcoholBottle());
				GraphUtils.EmporiumTryAdd(curInv, PreBuildChemHelper.CreateMiniAlcoholBottle());
				GraphUtils.EmporiumTryAdd(curInv, PreBuildChemHelper.CreateLipobondAgentBottle());
				GraphUtils.EmporiumTryAdd(curInv, PreBuildChemHelper.CreateAcidBottle());
				GraphUtils.EmporiumTryAdd(curInv, PreBuildChemHelper.CreateBaseBottle());
			});
		};
		return item;
	}

	private static void UnpackOrWarn(Action unpack)
	{
		if (NetworkUpgrade.IsUnlocked("RETIRED_CHEMIST"))
		{
			unpack();
			return;
		}
		GenericConfirmationUIManager.Instance.OpenPanel(LocHelper.GetLocalizedMechanic("mech_chemical_supplies_unpack_warning"), LocHelper.GetLocalizedMainMenu("yes"), LocHelper.GetLocalizedMainMenu("no"), delegate
		{
			GenericConfirmationUIManager.Instance.ClosePanel();
			unpack();
		}, delegate
		{
			GenericConfirmationUIManager.Instance.ClosePanel();
		});
	}

	private GameItem RestrictedChemicalSupplies()
	{
		GameItem item = Empty().SetSpriteAndShape("Items/items_material", "chemical2");
		item.name = LocHelper.GetLocalizedItem("item_restricted_chemical_name");
		item.SetGameItemType("CHEMICAL_SUPPLIES");
		ContrabandHelper.InitContrabandItem(item, 2);
		item.SetValue(100L);
		item.shortDescription = LocHelper.GetLocalizedItem("item_restricted_chemical_desc");
		item.flavorText = LocHelper.GetLocalizedItem("item_restricted_chemical_flavor");
		item.canActivateSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s);
		item.mayActivateSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s);
		item.onActivateSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			UnpackOrWarn(delegate
			{
				item.Destroy();
				GraphUtils.EmporiumTryAdd(curInv, PreBuildChemHelper.CreateFantaPrecusorBottle());
				GraphUtils.EmporiumTryAdd(curInv, PreBuildChemHelper.CreateFantaPrecusorBottle());
				GraphUtils.EmporiumTryAdd(curInv, PreBuildChemHelper.CreateReducingAgentBottle());
			});
		};
		return item;
	}

	private GameItem CommonMedicalSupplies()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_material", "med1");
		gameItem.name = LocHelper.GetLocalizedItem("item_common_medical_name");
		gameItem.SetValue(40L);
		gameItem.SetGameItemType("MEDICAL");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_common_medical_flavor");
		return gameItem;
	}

	private GameItem RestrictedMedicalSupplies()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_material", "med2");
		gameItem.name = LocHelper.GetLocalizedItem("item_restricted_medical_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_restricted_medical_flavor");
		return gameItem;
	}

	private GameItem ScrapMetal()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_material", "scrap_metal");
		gameItem.name = LocHelper.GetLocalizedItem("item_scrap_metal_name");
		gameItem.SetGameItemType("MATERIAL");
		gameItem.SetGameItemType("BASIC_MATERIAL");
		gameItem.SetGameItemType("JUNK");
		gameItem.EnableTag("SCRAP_METAL");
		AudioHelper.InitScrapMetal(gameItem);
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_scrap_metal_desc");
		gameItem.unitValue = 10L;
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => CanScrapMetalTargetItem(s, t)));
		gameItem.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => CanScrapMetalTargetItem(s, t)));
		gameItem.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			OnScrapMetalTargetItem(s, t);
		});
		return gameItem;
	}

	public static bool CanScrapMetalTargetItem(GameItem source, GameItem target)
	{
		if (target.IsID("small_bottled_water"))
		{
			return true;
		}
		if (target.IsTag("CAGE_TAG") && AnimalCage.CanRepair(target))
		{
			return true;
		}
		if (ReactorHelper.CanRepair(target))
		{
			return true;
		}
		if (target.IsTag("FRIDGE_TAG") && MachineFridge.CanUpgradeStorage(target))
		{
			return true;
		}
		if (ToolboxHelper.CanUpgradeToolbox(target))
		{
			return true;
		}
		if (WaterTankHelper.CanUpgrade(target))
		{
			return true;
		}
		if (target.IsTag("RAS_TAG") && MachineRAS.CanUpgradeWidth(target))
		{
			return true;
		}
		return false;
	}

	public static void OnScrapMetalTargetItem(GameItem source, GameItem target)
	{
		if (target.IsID("small_bottled_water"))
		{
			GameInventory parentInventory = target.parentInventory;
			source.Destroy();
			target.Destroy();
			GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("silencer_makeshift"));
		}
		else if (target.IsTag("CAGE_TAG"))
		{
			AnimalCage.Repair(target, source);
		}
		else if (ReactorHelper.IsReactor(target))
		{
			ReactorHelper.RepairWithScrap(target, source);
		}
		else if (target.IsTag("FRIDGE_TAG"))
		{
			MachineFridge.UpgradeStorage(target, source);
		}
		else if (WaterTankHelper.IsTank(target))
		{
			WaterTankHelper.Upgrade(target, source);
		}
		else if (target.IsTag("RAS_TAG"))
		{
			MachineRAS.UpgradeWidth(target, source);
		}
	}

	private GameItem Junk()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_material", "scrap_metal_2");
		gameItem.name = LocHelper.GetLocalizedItem("item_junk_name");
		gameItem.SetGameItemType("MATERIAL");
		gameItem.SetGameItemType("BASIC_MATERIAL");
		gameItem.SetGameItemType("JUNK");
		gameItem.EnableTag("SCRAP_METAL");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_junk_flavor");
		gameItem.unitValue = 3L;
		AudioHelper.InitScrapMetal(gameItem);
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => ReactorHelper.CanRepair(t)));
		gameItem.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => ReactorHelper.CanRepair(t)));
		gameItem.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			ReactorHelper.RepairWithJunk(t, s);
		});
		return gameItem;
	}

	private GameItem MetalOre()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_material", "ore1");
		gameItem.name = LocHelper.GetLocalizedItem("item_common_ore_name");
		gameItem.unitValue = 15L;
		gameItem.EnableTag("ORE_TAG");
		AudioHelper.InitSolidMetal(gameItem);
		gameItem.SetGameItemType("MATERIAL");
		gameItem.SetGameItemType("BASIC_MATERIAL");
		return gameItem;
	}

	private GameItem RareMetalOre()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_material", "ore2");
		gameItem.name = LocHelper.GetLocalizedItem("item_rare_ore_name");
		gameItem.unitValue = 30L;
		gameItem.EnableTag("ORE_TAG");
		gameItem.SetGameItemType("MATERIAL");
		return gameItem;
	}

	private GameItem Electronic()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_material", "electronic1");
		gameItem.name = LocHelper.GetLocalizedItem("item_common_electronic_name");
		gameItem.unitValue = 33L;
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_common_electronic_flavor");
		gameItem.SetType("MATERIAL_TAG");
		gameItem.SetGameItemType("MATERIAL");
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayThisTargetItemFunc = (GameItem s, GameItem t) => ModuleHelper.CanElectronicPartTarget(s, t);
		gameItem.canThisTargetItemFunc = (GameItem s, GameItem t) => ModuleHelper.CanElectronicPartTarget(s, t);
		gameItem.onThisTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			ModuleHelper.RestoreModule(t, s);
			ModuleHelper.RepairExtractor(t, s);
			VendingMachineHelper.UpgradeAttractiveness(t, s);
			ScavHelper.UpgradeScavScanner(t, s);
			HusbandryHelper.UpgradeVacuumRobot(t, s);
			MachineFridge.UpgradePowerEfficiency(t, s);
			MachineBottlePrinter.UpgradePrinter(t, s);
			CurrentConverterHelper.UpgradeEfficiency(t, s);
			MachineRAS.UpgradePowerEfficiency(t, s);
			ReactorHelper.Upgrade(t, s);
		};
		return gameItem;
	}

	private GameItem RareElectronic()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_material", "electronic2");
		gameItem.name = LocHelper.GetLocalizedItem("item_rare_electronic_name");
		gameItem.unitValue = 350L;
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_rare_electronic_flavor");
		gameItem.SetType("MATERIAL_TAG");
		gameItem.SetGameItemType("MATERIAL");
		return gameItem;
	}

	private GameItem HypertechElectronic()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_material", "electronic3");
		gameItem.name = LocHelper.GetLocalizedItem("item_hypertech_electronic_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_hypertech_electronic_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_hypertech_electronic_desc");
		gameItem.unitValue = 2500L;
		gameItem.SetType("MATERIAL_TAG");
		gameItem.SetGameItemType("MATERIAL");
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayThisTargetItemFunc = (GameItem s, GameItem t) => HypertechBatteryHelper.CanUpgradeOutput(t) || MindReprogrammerHelper.CanAddCharge(t);
		gameItem.canThisTargetItemFunc = (GameItem s, GameItem t) => HypertechBatteryHelper.CanUpgradeOutput(t) || MindReprogrammerHelper.CanAddCharge(t);
		gameItem.onThisTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (HypertechBatteryHelper.CanUpgradeOutput(t))
			{
				HypertechBatteryHelper.UpgradeOutput(t, s);
			}
			else if (MindReprogrammerHelper.CanAddCharge(t))
			{
				MindReprogrammerHelper.AddCharge(t, s);
			}
		};
		return gameItem;
	}

	private GameItem Wire()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_construction", "wire");
		gameItem.name = LocHelper.GetLocalizedItem("item_wire_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_wire_desc");
		gameItem.unitValue = 16L;
		gameItem.SetGameItemType("MATERIAL");
		gameItem.SetGameItemType("BASIC_MATERIAL");
		return gameItem;
	}

	private GameItem PrinterPlastic()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_material", "printer_plastic");
		gameItem.name = LocHelper.GetLocalizedItem("item_printer_plastic_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_printer_plastic_desc");
		gameItem.unitValue = 12L;
		gameItem.SetGameItemType("MATERIAL");
		gameItem.SetGameItemType("BASIC_MATERIAL");
		gameItem.EnableTag("PRINTER_MATERIAL_TAG");
		return gameItem;
	}

	private static bool CanRepairWithPart(GameItem part, GameItem gun)
	{
		if (!gun.IsTag("ADV_GUN_TAG") || !GunHelper.CanRepair(gun))
		{
			return false;
		}
		if (!GeneralHelper.IsItemOwned(part))
		{
			return false;
		}
		if (!GeneralHelper.IsItemOwned(gun))
		{
			return GunHelper.IsServiceGun(gun);
		}
		return true;
	}

	private GameItem GunPart()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_material", "gun_part");
		gameItem.name = LocHelper.GetLocalizedItem("item_gun_part_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_gun_part_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_gun_part_flavor");
		gameItem.unitValue = 25L;
		gameItem.SetGameItemType("MATERIAL");
		AudioHelper.InitTinyMetal(gameItem);
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.EnableTag("CAN_USE_ON_NON_OWNED_TAG");
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => CanRepairWithPart(s, t) || GunFrameHelper.CanFinish(t, s)));
		gameItem.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => CanRepairWithPart(s, t) || GunFrameHelper.CanFinish(t, s)));
		gameItem.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (GunFrameHelper.CanFinish(t, s))
			{
				GunFrameHelper.Finish(t, s);
			}
			else if (GunHelper.IsServiceGun(t))
			{
				GunHelper.RepairServiceGun(t, s);
			}
			else
			{
				GunHelper.TryRepairGun(t, s);
			}
		});
		return gameItem;
	}

	private GameItem NuclearWaste()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_material", "nuclear_waste");
		gameItem.name = LocHelper.GetLocalizedItem("item_nuclear_waste_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_nuclear_waste_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_nuclear_waste_flavor");
		gameItem.unitValue = 0L;
		gameItem.EnableTag("HAZARDOUS_WASTE_TAG");
		gameItem.EnableTag("VENDING_BLACKLISTED_TAG");
		AudioHelper.InitMetalMachine(gameItem);
		return gameItem;
	}

	private GameItem FluxAgent()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_material", "flux_agent");
		gameItem.name = LocHelper.GetLocalizedItem("item_flux_agent_name");
		gameItem.unitValue = 4L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_flux_agent_desc");
		gameItem.SetType("MATERIAL_TAG");
		gameItem.SetGameItemType("MATERIAL");
		AudioHelper.InitSolidMetal(gameItem);
		return gameItem;
	}

	private GameItem AdvancedFluxAgent()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_material", "flux_agent_advanced");
		gameItem.name = LocHelper.GetLocalizedItem("item_advanced_flux_agent_name");
		gameItem.unitValue = 300L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_advanced_flux_agent_desc");
		gameItem.SetType("MATERIAL_TAG");
		gameItem.SetGameItemType("MATERIAL");
		AudioHelper.InitSolidMetal(gameItem);
		return gameItem;
	}
}
