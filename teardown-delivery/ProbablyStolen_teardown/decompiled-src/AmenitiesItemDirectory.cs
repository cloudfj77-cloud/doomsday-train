using System;

public class AmenitiesItemDirectory : ItemDirectory
{
	private const string WATER_TESTING_STRIP_TAG = "WATER_TESTING_STRIP_TAG";

	private const string WATER_PURIFICATION_TABLET = "WATER_PURIFICATION_TABLET";

	private const string NUTRIENT_TABLET = "NUTRIENT_TABLET";

	public const string WATER_FILTER = "WATER_FILTER";

	private const int PURIFIER_UL_PER_DURABILITY = 100000;

	public const string SELF_REFILLING_PERFUME_ID = "self_refilling_neuroactive_perfume";

	public const int SELF_REFILLING_PERFUME_DAYS = 3;

	protected override void InitDirectory()
	{
		Add("box_tampon", () => BoxTampon());
		Add("packet_red_cigarette", () => PacketCigaretteRed());
		Add("pack_condom", () => PackCondom());
		Add("shampoo", () => Shampoo());
		Add("toothpaste", () => Toothpaste());
		Add("paper_towel", () => PaperTowel());
		Add("toilet_paper", () => ToiletPaper());
		Add("water_test_container", () => WaterTestKit());
		Add("water_test_strip", () => WaterTestStrip());
		Add("aquascan", () => AquaScan());
		Add("uv_filter", () => UVFilter());
		Add("metal_scanner", () => MetalScanner());
		Add("gene_scanner", () => GeneScanner());
		Add("water_tablet_container", () => WaterPurificationTabletContainer());
		Add("water_tablet", () => WaterPurificationTablet());
		Add("water_filter", () => WaterFilter());
		Add("water_filter_adv", () => WaterFilterAdvanced());
		Add("portable_water_purifier", () => PortableWaterPurifier());
		Add("water_pitcher", () => WaterPitcher());
		Add("water_tank", () => WaterTankHelper.Create());
		Add("hydroreed_filter", () => HydroreedFilter());
		Add("kotton_fabric", () => KottonFabric());
		Add("nutrient_tablet", () => HydroponicNutrientTablet());
		Add("nutrient_tablet_container", () => NutrientTabletContainer());
		Add("lottery_ticket", () => LotteryTicket());
		Add("lottery_ticket_old", () => LotteryTicketOld());
		Add("skincare_cream", () => SkincareCream());
		Add("neuroactive_perfume", () => NeuroactivePerfume());
		Add("self_refilling_neuroactive_perfume", () => SelfRefillingNeuroactivePerfume());
		Add("pheromone_perfume", () => PheromonePerfume());
	}

	private GameItem SkincareCream()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "skincare_cream");
		gameItem.name = LocHelper.GetLocalizedItem("item_skincare_cream_name");
		gameItem.SetValue(500L);
		gameItem.SetGameItemType("LUXURY_ITEM");
		gameItem.EnableManufacturer("solana");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_skincare_cream_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_skincare_cream_desc");
		AudioHelper.InitGlass(gameItem);
		return gameItem;
	}

	private GameItem BoxTampon()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "box_tampon");
		gameItem.name = LocHelper.GetLocalizedItem("item_box_tampon_name");
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		gameItem.SetGameItemType("LUXURY_ITEM");
		gameItem.AddAmenities("amenities_hygene", 2, 20);
		gameItem.EnableManufacturer("pastel");
		gameItem.unitValue = 100L;
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_box_tampon_flavor");
		AudioHelper.InitCartonBox(gameItem);
		return gameItem;
	}

	private GameItem PackCondom()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "pack_condom");
		gameItem.name = LocHelper.GetLocalizedItem("item_pack_condom_name");
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		gameItem.AddAmenities("amenities_wellbeing", 2, 10);
		gameItem.EnableManufacturer("pastel");
		gameItem.unitValue = 35L;
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_pack_condom_flavor");
		return gameItem;
	}

	private GameItem PacketCigaretteRed()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "cig_red");
		gameItem.name = LocHelper.GetLocalizedItem("item_packet_red_cigarette_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_packet_red_cigarette_flavor");
		gameItem.unitValue = 80L;
		InsCigaretteHelper.SetGenuine(gameItem);
		gameItem.AddAmenities("amenities_substance", 3, 20);
		gameItem.EnableManufacturer("pastel");
		gameItem.EnableTag("CIGARETTE_TAG");
		gameItem.SetGameItemType("SUBSTANCE");
		InspectableHelper.InitInspectableItem(gameItem);
		ConsumableHelper.InitNicotine(gameItem, 3);
		return gameItem;
	}

	private GameItem Shampoo()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "shampoo");
		gameItem.name = LocHelper.GetLocalizedItem("item_shampoo_name");
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		gameItem.SetValue(30L);
		gameItem.EnableManufacturer("pastel");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_shampoo_flavor");
		return gameItem;
	}

	private GameItem Toothpaste()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "toothpaste");
		gameItem.name = LocHelper.GetLocalizedItem("item_toothpaste_name");
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		gameItem.SetValue(15L);
		gameItem.EnableManufacturer("pastel");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_toothpaste_flavor");
		return gameItem;
	}

	private GameItem PaperTowel()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "paper_towel");
		gameItem.name = LocHelper.GetLocalizedItem("item_paper_towel_name");
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		gameItem.SetValue(35L);
		return gameItem;
	}

	private GameItem ToiletPaper()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "toilet_paper");
		gameItem.name = LocHelper.GetLocalizedItem("item_toilet_paper_name");
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		gameItem.SetValue(25L);
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_toilet_paper_flavor");
		AudioHelper.InitTextile(gameItem);
		return gameItem;
	}

	private GameItem WaterTestKit()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(10, 2);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_amenities", "water_test_container");
		gameItem.name = LocHelper.GetLocalizedItem("item_water_test_container_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_water_test_container_desc");
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		gameItem.SetGameItemType("WATER_PURIFICATION_SUPPLY");
		gameItem.EnableManufacturer("davre");
		gameItem.SetValue(1L);
		gameItem.EnableTag("PRE_CONTAINER_TAG");
		ContainerHelper.AllowOnlyTaggedItemOwn(item2, gameItem, "WATER_TESTING_STRIP_TAG");
		AudioHelper.InitPlasticVial(gameItem);
		AudioHelper.InitPlasticContainer(gameItem);
		gameItem.EnableTag("CONTAINER_TOOLTIP_TAG");
		return gameItem;
	}

	private GameItem WaterTestStrip()
	{
		GameItem item = Empty().SetSpriteAndShape("Items/items_amenities", "strip_new");
		item.name = LocHelper.GetLocalizedItem("item_water_test_strip_name");
		item.shortDescription = LocHelper.GetLocalizedItem("item_water_test_strip_desc");
		item.SetGameItemType("HOUSEHOLD_GOOD");
		item.SetGameItemType("WATER_PURIFICATION_SUPPLY");
		item.EnableManufacturer("davre");
		item.SetValue(3L);
		item.EnableTag("WATER_TESTING_STRIP_TAG");
		item.EnableTag("CAN_USE_ON_NON_OWNED_TAG");
		item.maySelectSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => !s.IsTag("IS_SPENT");
		item.canSelectSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => !s.IsTag("IS_SPENT");
		item.mayItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (s.IsTag("IS_SPENT"))
			{
				return false;
			}
			if (WaterHelper.IsEmpty(t))
			{
				return false;
			}
			return t.IsTag("LIQUID_CONTAINER_TAG") ? true : false;
		};
		item.canItemTargetItemFunc = (GameItem s, GameItem t) => true;
		item.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (t.IsTag("LIQUID_CONTAINER_TAG") != s.IsTag("IS_SPENT"))
			{
				s.SetSpriteAndShape("Items/items_amenities", "strip_used");
				WaterTestHelper.HandleWaterCheckTestingStrip(t, s);
				WaterStripUIManager.Instance.Open(s);
				item.SetValue(0L);
				item.name = LocHelper.GetLocalizedItem("item_used_water_testing_strip_name");
				item.shortDescription = LocHelper.GetLocalizedItem("item_used_water_testing_strip_desc");
			}
		};
		item.canActivateSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => s.IsTag("IS_SPENT");
		item.mayActivateSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => s.IsTag("IS_SPENT");
		item.onActivateSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			WaterStripUIManager.Instance.Open(curItem);
		};
		return item;
	}

	private GameItem UVFilter()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "uv");
		gameItem.name = LocHelper.GetLocalizedItem("item_uv_filter_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_uv_filter_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_uv_filter_desc");
		gameItem.SetGameItemType("WATER_PURIFICATION_SUPPLY");
		gameItem.EnableManufacturer("davre");
		gameItem.EnableTag("WATER_FILTER");
		gameItem.SetValue(150L);
		gameItem.maySelectSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (WaterHelper.IsEmpty(t))
			{
				return false;
			}
			return t.IsTag("LIQUID_CONTAINER_TAG") ? true : false;
		};
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => true;
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			WaterHelper.OnUVUsed(t);
		};
		return gameItem;
	}

	private GameItem AquaScan()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "aquascan");
		gameItem.name = LocHelper.GetLocalizedItem("item_aquascan_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_aquascan_desc");
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		gameItem.SetGameItemType("WATER_PURIFICATION_SUPPLY");
		AudioHelper.InitScanner(gameItem);
		gameItem.EnableManufacturer("davre");
		gameItem.SetValue(200L);
		gameItem.EnableTag("CAN_USE_ON_NON_OWNED_TAG");
		gameItem.maySelectSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s);
		gameItem.canSelectSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s);
		gameItem.mayItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (!GeneralHelper.IsItemOwned(s))
			{
				return false;
			}
			return t.IsTag("LIQUID_CONTAINER_TAG") ? true : false;
		};
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => true;
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (t.IsTag("LIQUID_CONTAINER_TAG"))
			{
				AudioManager.Instance.Play3("scanner_activate");
				WaterHelper.SetLastScanResult(t);
				WaterScannerUIManager.Instance.Open(s, t);
			}
		};
		return gameItem;
	}

	private GameItem MetalScanner()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "metal_scanner");
		gameItem.name = LocHelper.GetLocalizedItem("item_metal_scanner_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_metal_scanner_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_metal_scanner_flavor");
		gameItem.SetGameItemType("TOOL");
		gameItem.SetValue(250L);
		gameItem.EnableTag("IMPORTANT_TAG");
		ScavHelper.InitScavScanner(gameItem);
		AudioHelper.InitSmallMachine(gameItem);
		return gameItem;
	}

	private GameItem GeneScanner()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "genetic_scanner");
		gameItem.name = LocHelper.GetLocalizedItem("item_genetic_scanner_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_genetic_scanner_desc");
		gameItem.SetGameItemType("TOOL");
		gameItem.SetValue(500L);
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.mayToggleSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s);
		gameItem.canToggleSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s);
		gameItem.onToggleSlotItemFunc = delegate(GameItem s, GameInventory _, SlotMarker _)
		{
			AudioManager.Instance.Play3("scanner_activate");
			if (s.IsTag("IS_ITEM_TOGGLE_ON_TAG"))
			{
				s.DisableTag("IS_ITEM_TOGGLE_ON_TAG");
				PlayerStore.Instance.isGeneScannerScanning = false;
			}
			else
			{
				s.EnableTag("IS_ITEM_TOGGLE_ON_TAG");
				PlayerStore.Instance.isGeneScannerScanning = true;
			}
			UpdateGeneScannerSprite(s);
		};
		gameItem.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem.onLoaded, new Action<GameItem>(UpdateGeneScannerSprite));
		gameItem.onOneSecondItemFunc = delegate(GameItem s, GameInventory _, SlotMarker _)
		{
			if (GeneralHelper.IsItemOwned(s) && s.IsTag("IS_ITEM_TOGGLE_ON_TAG"))
			{
				PlayerStore.Instance.isGeneScannerScanning = true;
			}
		};
		AudioHelper.InitSmallMachine(gameItem);
		return gameItem;
	}

	private static void UpdateGeneScannerSprite(GameItem scanner)
	{
		bool flag = scanner.IsTag("IS_ITEM_TOGGLE_ON_TAG");
		scanner.SetSpriteAndShape("Items/items_amenities", flag ? "genetic_scanner_on" : "genetic_scanner");
		scanner.Validate();
	}

	private GameItem WaterPurificationTabletContainer()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(1, 9);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_amenities", "water_tablet_container");
		gameItem.name = LocHelper.GetLocalizedItem("item_water_tablet_container_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_water_tablet_container_desc");
		gameItem.EnableManufacturer("davre");
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		gameItem.SetGameItemType("WATER_PURIFICATION_SUPPLY");
		gameItem.EnableTag("PRE_CONTAINER_TAG");
		gameItem.SetValue(1L);
		ContainerHelper.AllowOnlyTaggedItemOwn(item2, gameItem, "WATER_PURIFICATION_TABLET");
		AudioHelper.InitPlasticVial(gameItem);
		AudioHelper.InitPlasticContainer(gameItem);
		gameItem.EnableTag("CONTAINER_TOOLTIP_TAG");
		return gameItem;
	}

	private GameItem WaterPurificationTablet()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "water_tablet");
		gameItem.name = LocHelper.GetLocalizedItem("item_water_tablet_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_water_tablet_desc");
		gameItem.EnableManufacturer("davre");
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		gameItem.SetGameItemType("WATER_PURIFICATION_SUPPLY");
		AudioHelper.InitTinyItem(gameItem);
		gameItem.SetValue(6L);
		gameItem.EnableTag("WATER_PURIFICATION_TABLET");
		ChemicalHelper.InitChemItem(gameItem, isBlendable: true, isSoluble: true);
		ChemicalHelper.SetChemContentML(gameItem, "chlorine", 15.0);
		return gameItem;
	}

	private GameItem HydroponicNutrientTablet()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "nutrient_tablet");
		gameItem.name = LocHelper.GetLocalizedItem("item_hydroponic_nutrient_tablet_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_hydroponic_nutrient_tablet_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_hydroponic_nutrient_tablet_flavor");
		gameItem.SetGameItemType("FARMING_SUPPLIES");
		gameItem.SetValue(5L);
		AudioHelper.InitTinyItem(gameItem);
		gameItem.EnableTag("NUTRIENT_TABLET");
		ChemicalHelper.InitChemItem(gameItem, isBlendable: true, isSoluble: true);
		ChemicalHelper.SetChemContentML(gameItem, "mineral", 15.0);
		return gameItem;
	}

	private GameItem NutrientTabletContainer()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(1, 9);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_hydroponic_plant", "nutrient_tablet_container");
		gameItem.name = LocHelper.GetLocalizedItem("item_nutrient_tablet_container_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_nutrient_tablet_container_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_nutrient_tablet_container_flavor");
		gameItem.SetGameItemType("FARMING_SUPPLIES");
		gameItem.EnableTag("PRE_CONTAINER_TAG");
		gameItem.SetValue(1L);
		ContainerHelper.AllowOnlyTaggedItemOwn(item2, gameItem, "NUTRIENT_TABLET");
		AudioHelper.InitPlasticVial(gameItem);
		AudioHelper.InitPlasticContainer(gameItem);
		gameItem.EnableTag("CONTAINER_TOOLTIP_TAG");
		return gameItem;
	}

	private GameItem KottonFabric()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_armor", "kotton_fabric");
		gameItem.name = LocHelper.GetLocalizedItem("item_kotton_fabric_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_kotton_fabric_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_kotton_fabric_flavor");
		gameItem.EnableManufacturer("pastel");
		gameItem.SetGameItemType("MATERIAL");
		gameItem.SetGameItemType("FILTER");
		gameItem.unitValue = 65L;
		AudioHelper.InitTextile(gameItem);
		gameItem.ModifyTag("FILTER_SIZE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(150);
		});
		DurabilityHelper.InitItemWithDurability(gameItem, 1000);
		gameItem.EnableTag("WATER_FILTER");
		return gameItem;
	}

	private GameItem WaterFilter()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "water_filter");
		gameItem.name = LocHelper.GetLocalizedItem("item_water_filter_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_water_filter_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_water_filter_flavor");
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		gameItem.SetGameItemType("FILTER");
		gameItem.SetGameItemType("WATER_PURIFICATION_SUPPLY");
		gameItem.ModifyTag("FILTER_SIZE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(22);
		});
		gameItem.SetValue(25L);
		gameItem.EnableTag("WATER_FILTER");
		DurabilityHelper.InitItemWithDurabilityPricingMode2(gameItem, 400, 1);
		AudioHelper.InitPlasticContainer(gameItem);
		return gameItem;
	}

	private GameItem WaterFilterAdvanced()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "water_filter_adv");
		gameItem.name = LocHelper.GetLocalizedItem("item_water_filter_adv_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_water_filter_adv_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_water_filter_adv_flavor");
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		gameItem.SetGameItemType("FILTER");
		gameItem.SetGameItemType("WATER_PURIFICATION_SUPPLY");
		gameItem.SetGameItemType("LUXURY_ITEM");
		gameItem.ModifyTag("FILTER_SIZE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(22);
		});
		gameItem.SetValue(350L);
		gameItem.EnableTag("WATER_FILTER");
		DurabilityHelper.InitItemWithDurability(gameItem, 400);
		AudioHelper.InitPlasticContainer(gameItem);
		gameItem.onCycleEndSlotItemFunc = delegate(GameItem curItem, GameInventory _, SlotMarker _)
		{
			DurabilityHelper.ChangeDurability(curItem, 50);
		};
		return gameItem;
	}

	private GameItem PortableWaterPurifier()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "portable_water_purifier");
		gameItem.name = LocHelper.GetLocalizedItem("item_portable_water_purifier_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_portable_water_purifier_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_portable_water_purifier_flavor");
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		gameItem.SetGameItemType("WATER_PURIFICATION_SUPPLY");
		gameItem.SetGameItemType("LUXURY_ITEM");
		gameItem.SetValue(325L);
		DurabilityHelper.InitItemWithDurabilityPricingMode2(gameItem, 300, 25);
		AudioHelper.InitPlasticContainer(gameItem);
		gameItem.maySelectSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s);
		gameItem.canSelectSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s);
		gameItem.mayThisTargetItemFunc = (GameItem s, GameItem t) => CanPurifierTarget(s, t);
		gameItem.canThisTargetItemFunc = (GameItem s, GameItem t) => CanPurifierTarget(s, t);
		gameItem.onThisTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			OnPortableWaterPurifierUsed(s, t);
		};
		return gameItem;
	}

	private static bool CanPurifierTarget(GameItem purifier, GameItem target)
	{
		if (GeneralHelper.IsItemOwned(purifier) && GeneralHelper.IsItemOwned(target) && target.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return !WaterHelper.IsEmpty(target);
		}
		return false;
	}

	private static void OnPortableWaterPurifierUsed(GameItem purifier, GameItem container)
	{
		if (WaterHelper.GetWaterPurity(container) >= 9800)
		{
			ShowPurifierMessage("mech_portable_purifier_already_clean");
			return;
		}
		int num = (int)(((long)WaterHelper.GetTotalVolume(container) + 100000L - 1) / 100000);
		if (num > DurabilityHelper.GetRealDurability(purifier))
		{
			ShowPurifierMessage("mech_portable_purifier_not_enough_charge");
			return;
		}
		WaterHelper.PurifyToBaseWater(container);
		DurabilityHelper.ChangeDurability(purifier, -num);
	}

	private static void ShowPurifierMessage(string mechanicKey)
	{
		GenericConfirmationUIManager.Instance.OpenPanelSingleButton(LocHelper.GetLocalizedMechanic(mechanicKey), LocHelper.GetLocalizedMainMenu("confirm"), delegate
		{
			GenericConfirmationUIManager.Instance.ClosePanel();
		});
	}

	private GameItem HydroreedFilter()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "hydroreed_filter");
		gameItem.name = LocHelper.GetLocalizedItem("item_hydroreed_filter_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_hydroreed_filter_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_hydroreed_filter_flavor");
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		gameItem.SetGameItemType("FILTER");
		gameItem.SetGameItemType("WATER_PURIFICATION_SUPPLY");
		gameItem.ModifyTag("FILTER_SIZE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(22);
		});
		gameItem.SetValue(20L);
		gameItem.EnableTag("WATER_FILTER");
		DurabilityHelper.InitItemWithDurability(gameItem, 200);
		PlantGeneticsHelper.ApplyFactoryGenetics(gameItem, "hydroreed_seed");
		return gameItem;
	}

	private GameItem WaterPitcher()
	{
		(PixelWindow, GameSlotInventory) tuple = DirectoryUtils.CreateSlotInventoryWindow();
		PixelWindow item2 = tuple.Item1;
		GameSlotInventory item3 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item2).SetSpriteAndShape("Items/water_container4", "pitcher");
		gameItem.name = LocHelper.GetLocalizedItem("item_water_pitcher_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_water_pitcher_desc");
		gameItem.identifier = "water_pitcher";
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		item3.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => GeneralHelper.IsItemOwned(item) && item.IsTag("WATER_FILTER") && item.identifier != "uv_filter";
		item3.SetBackgroundFadeSprite("Items/items_amenities", "water_filter");
		WaterHelper.InitLiquidContainerItem(gameItem, 3000, 25, isFilterContainer: true, isGraduated: true, isDropper: false, isTransparentContainer: true, 4);
		AudioHelper.InitWaterFilter(gameItem);
		gameItem.EnableTag("CONTAINER_TOOLTIP_TAG");
		return gameItem;
	}

	private GameItem LotteryTicket()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "lottery");
		gameItem.name = LocHelper.GetLocalizedItem("item_lottery_ticket_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_lottery_ticket_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_lottery_ticket_flavor");
		LotteryHelper.InitLotteryTicket(gameItem);
		gameItem.SetValue(25L);
		gameItem.EnableTag("REPO_ELIGIBLE_OVERRIDE_TAG");
		InitLotteryPaper(gameItem);
		gameItem.canActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item) && LotteryHelper.CanRedeemPrize(item);
		gameItem.mayActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item) && LotteryHelper.CanRedeemPrize(item);
		gameItem.onActivateSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			LotteryHelper.TryRedeemPrize(curItem);
		};
		AudioHelper.InitPaperSheet(gameItem);
		return gameItem;
	}

	private GameItem LotteryTicketOld()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "lottery_old");
		gameItem.name = LocHelper.GetLocalizedItem("item_lottery_ticket_old_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_lottery_ticket_old_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_lottery_ticket_old_flavor");
		gameItem.SetValue(1L);
		gameItem.EnableTag("REPO_ELIGIBLE_OVERRIDE_TAG");
		InitLotteryPaper(gameItem);
		AudioHelper.InitPaperSheet(gameItem);
		return gameItem;
	}

	private static void InitLotteryPaper(GameItem item)
	{
		item.EnableTag("LOTTERY_PAPER_TAG");
		item.onLoaded = (Action<GameItem>)Delegate.Combine(item.onLoaded, (Action<GameItem>)delegate(GameItem loaded)
		{
			loaded.EnableTag("LOTTERY_PAPER_TAG");
			loaded.RemoveGameItemType("DOCUMENT");
		});
	}

	private GameItem NeuroactivePerfume()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "perfume1");
		gameItem.name = LocHelper.GetLocalizedItem("item_neuroactive_perfume_name");
		gameItem.SetValue(150L);
		gameItem.SetGameItemType("LUXURY_ITEM");
		gameItem.EnableManufacturer("solana");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_neuroactive_perfume_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_neuroactive_perfume_desc");
		AudioHelper.InitGlass(gameItem);
		InitNeuroactivePerfumeSpray(gameItem);
		UseCountHelper.InitUseCountItem(gameItem, 5, transferable: true, destroyOnEmpty: true, useAutoValue: true, 0, 30);
		return gameItem;
	}

	private static void InitNeuroactivePerfumeSpray(GameItem item)
	{
		item.canActivateSlotItemFunc = delegate(GameItem s, GameInventory _, SlotMarker _)
		{
			if (!GeneralHelper.IsItemOwned(s))
			{
				return false;
			}
			if (PlayerStore.Instance.currentClientInstance == null)
			{
				return false;
			}
			if (PlayerStore.Instance.currentClientInstance.GetClientBlueprint().perfumeInfluence != 0)
			{
				return false;
			}
			return UseCountHelper.CanUse(s) ? true : false;
		};
		item.mayActivateSlotItemFunc = delegate(GameItem s, GameInventory _, SlotMarker _)
		{
			if (!GeneralHelper.IsItemOwned(s))
			{
				return false;
			}
			if (PlayerStore.Instance.currentClientInstance == null)
			{
				return false;
			}
			return PlayerStore.Instance.currentClientInstance.GetClientBlueprint().perfumeInfluence == 0;
		};
		item.onActivateSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			UseCountHelper.Use(curItem);
			PlayerStore.Instance.currentClientInstance.GetClientBlueprint().perfumeInfluence = 1;
			PlayerStore.Instance.RefreshCounterItem();
			AudioManager.Instance.Play2("perfume_spray");
		};
	}

	private GameItem SelfRefillingNeuroactivePerfume()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "eleanor_perfume");
		gameItem.name = LocHelper.GetLocalizedItem("item_self_refilling_perfume_name");
		gameItem.SetValue(1000L);
		gameItem.SetGameItemType("LUXURY_ITEM");
		gameItem.EnableManufacturer("solana");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_self_refilling_perfume_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_self_refilling_perfume_desc");
		AudioHelper.InitGlass(gameItem);
		InitNeuroactivePerfumeSpray(gameItem);
		gameItem.EnableTag("IMPORTANT_TAG");
		UseCountHelper.InitUseCountItem(gameItem, 5, transferable: true, destroyOnEmpty: false);
		UseCountHelper.InitAutoRefill(gameItem, 3);
		return gameItem;
	}

	private GameItem PheromonePerfume()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "perfume2");
		gameItem.name = LocHelper.GetLocalizedItem("item_pheromone_perfume_name");
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		gameItem.SetValue(200L);
		gameItem.SetGameItemType("LUXURY_ITEM");
		gameItem.EnableManufacturer("solana");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_pheromone_perfume_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_pheromone_perfume_desc");
		AudioHelper.InitGlass(gameItem);
		gameItem.canActivateSlotItemFunc = delegate(GameItem s, GameInventory _, SlotMarker _)
		{
			if (!GeneralHelper.IsItemOwned(s))
			{
				return false;
			}
			if (PlayerStore.Instance.currentClientInstance == null)
			{
				return false;
			}
			return PlayerStore.Instance.currentClientInstance.GetClientBlueprint().perfumeInfluence != 2;
		};
		gameItem.mayActivateSlotItemFunc = delegate(GameItem s, GameInventory _, SlotMarker _)
		{
			if (!GeneralHelper.IsItemOwned(s))
			{
				return false;
			}
			if (PlayerStore.Instance.currentClientInstance == null)
			{
				return false;
			}
			return PlayerStore.Instance.currentClientInstance.GetClientBlueprint().perfumeInfluence != 2;
		};
		gameItem.onActivateSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			UseCountHelper.Use(curItem);
			PlayerStore.Instance.currentClientInstance.GetClientBlueprint().perfumeInfluence = 2;
			PlayerStore.Instance.RefreshCounterItem();
			AudioManager.Instance.Play2("perfume_spray");
		};
		UseCountHelper.InitUseCountItem(gameItem, 5, transferable: true, destroyOnEmpty: true, useAutoValue: true, 0, 40);
		return gameItem;
	}
}
