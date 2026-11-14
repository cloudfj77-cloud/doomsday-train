using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnusedDirectory : ItemDirectory
{
	public CraftingFilter MEDS_FILTER = new CraftingTagsFilter("Any Meds", 1, "pill", "injector");

	public CraftingFilter WRENCH_FILTER = new CraftingIdentifierFilter("Wrench", 1, "wrench_space", "wrench_orange", "wrench_old");

	public const string FOURNITURE_TAG = "FOURNITURE_TAG";

	protected override void InitDirectory()
	{
		Add("lootbox_small", () => LootboxSmall());
		Add("lootbox_medium", () => LootboxMedium());
		Add("lootbox_large", () => LootboxLarge());
		Add("technician_backpack", () => TechnicianBackpack());
		Add("firstaid_box", () => FirstAidBox());
		Add("pill_bottle", () => PillBottle());
		Add("bicarsole_pill_bottle", () => BicarsolePillBottle());
		Add("ocutol_pill_bottle", () => OcutolPillBottle());
		Add("fixalin_pill_bottle", () => FixalinPillBottle());
		Add("phagimycin_pill_bottle", () => PhagimycinPillBottle());
		Add("unlicensed_phagimycin_pill_bottle", () => UnlicensedPhagimycinPillBottle());
		Add("oxycodone_pill_bottle", () => OxycodonePillBottle());
		Add("pouch_injector", () => InjectorPouch());
		Add("pouch_injector_large", () => InjectorPouchLarge());
		Add("pouch_injector_large_double", () => InjectorPouchLargeDouble());
		Add("pouch_medical", () => FirstAidPouch());
		Add("pouch_general", () => PouchGeneral());
		Add("pouch_general_large", () => PouchGeneralLarge());
		Add("pouch_hydration", () => PouchHydration());
		Add("organ_transport_container", () => OrganTransportContainer());
		Add("loan_contract", () => LoanContract());
		Add("lockpicks", () => Lockpicks());
		Add("exotic_fuel", () => ExoticFuel());
		Add("box_nail", () => BoxNail());
		Add("trash_pile", () => TrashPile());
		Add("wrench_orange", () => Wrench());
		Add("steel_sheet", () => SheetSteel());
		Add("spectral_analyser", () => CreateSpectralAnalyser());
		Add("backpack_small_modular", () => BackpackSmallModular());
		Add("backpack_medium_modular", () => BackpackMediumModular());
		Add("backpack_large_modular", () => BackpackLargeModular());
		Add("molotov_cocktail", () => MolotovCocktail());
		Add("safe", () => Safe());
		Add("large_storage_bay", () => LargeStorageBay());
		Add("aloe_vera", () => AloeVera());
		Add("mirage_generator", () => MirageGenerator());
		Add("antique_grenade", () => AntiqueGrenade());
		Add("sec_card", () => SecCard());
		Add("inspection_book", () => VisualInspectionBook());
		Add("organ_heart", () => CreateHeart());
		Add("organ_lung", () => CreateLung());
		Add("organ_kidney", () => CreateKidney());
		Add("organ_liver", () => CreateLiver());
		Add("revolver_short_tac", () => CreateRevolverShortTactical());
		Add("revolver_long", () => CreateRevolverLong());
		Add("revolver_short", () => CreateRevolverShort());
		Add("ammo_box", () => AmmoBox());
		Add("codebook", () => CodeBook());
		Add("hemostatic_agent", () => HemostaticAgent());
		Add("aloe_vera_gel", () => AloeVeraGel());
		Add("test_water", () => TestWater());
		Add("test_water2", () => TestWater2());
		Add("test_water3", () => TestWater3());
		Add("heavy_handmade_pistol_s", () => CreateHeavyHandmadePistolSuppressed());
		Add("armor_titan10", () => ArmorTitan10());
		Add("armor_titan10l", () => ArmorTitan10L());
		Add("armor_titan10h", () => ArmorTitan10H());
		Add("armor_titan10s", () => ArmorTitan10S());
		Add("armor_titan9", () => ArmorTitan9());
	}

	private GameItem EquippableBack()
	{
		GameItem gameItem = Empty();
		gameItem.EnableTag("backpack");
		gameItem.EnableTag("equippable");
		return gameItem;
	}

	private GameItem TechnicianBackpack()
	{
		(PixelWindow, GameInventory, GameInventory) tuple = DirectoryUtils.CreateDoubleInventoryWindowSlotInput(5, 5, isDraggable: true, isCentered: true, 0);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameInventory item3 = tuple.Item3;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_backpack", "satchel");
		gameItem.EnableTag("backpack");
		gameItem.EnableTag("equippable");
		gameItem.isCombatBackpack = true;
		gameItem.unitValue = 150L;
		AudioHelper.InitBackpack(gameItem);
		gameItem.name = "Technician Backpack";
		((GameSlotInventory)item2).SetBackgroundFadeSprite("Items/itematlas", "energy_credit");
		ContainerHelper.AllowOnlyTaggedItem(item2, "power_source_item");
		TechnicianBackpackHelper.InitComplexeEquipment(gameItem, item2, item3);
		ContainerHelper.InitBackpackItem(item3, gameItem);
		return gameItem;
	}

	private GameItem LootboxSmall()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(6, 6);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_backpack2", "simple_backpack");
		gameItem.name = "Loots";
		gameItem.soundContainerOpen = null;
		ContainerHelper.InitBackpackItem(item2, gameItem);
		return gameItem;
	}

	private GameItem LootboxMedium()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(12, 12);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_backpack2", "simple_backpack");
		gameItem.name = "Loots";
		gameItem.soundContainerOpen = null;
		ContainerHelper.InitBackpackItem(item2, gameItem);
		return gameItem;
	}

	private GameItem LootboxLarge()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(24, 24);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_backpack2", "simple_backpack");
		gameItem.name = "Loots";
		gameItem.soundContainerOpen = null;
		ContainerHelper.InitBackpackItem(item2, gameItem);
		return gameItem;
	}

	private GameItem LifeweedSeedPacket()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(3, 3);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_hydroponic_plant", "nutrifruit_seed_packet");
		gameItem.name = "Lifeweed Seed Packet";
		gameItem.EnableManufacturer("pastel");
		gameItem.unitValue = 1L;
		gameItem.SetGameItemType("SEED");
		ContainerHelper.InitContainerItem(item2, gameItem);
		return gameItem;
	}

	private GameItem OrganTransportContainer()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(6, 6);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_container", "organ_transport_box");
		gameItem.name = "Organ Transport Container";
		gameItem.EnableManufacturer("carmicheal");
		gameItem.unitValue = 500L;
		ContainerHelper.InitContainerItem(item2, gameItem);
		ContainerHelper.AllowOnlyTaggedItem(item2, "ORGAN_ITEM_TAG");
		gameItem.shortDescription = "A specialised container that's design to store organ with the exact conditions to keep them alive";
		gameItem.EnableTag("ORGAN_TRANSPORT_CONTAINER_TAG");
		item2.onSlotAddItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(item2.onSlotAddItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			if (addedItem.IsTag("ORGAN_ITEM_TAG"))
			{
				addedItem.EnableTag("IS_ORGAN_STORE_IN_TRANSPORT_CONTAINER");
			}
		});
		item2.onSlotRemoveItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(item2.onSlotRemoveItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			if (addedItem.IsTag("ORGAN_ITEM_TAG"))
			{
				addedItem.DisableTag("IS_ORGAN_STORE_IN_TRANSPORT_CONTAINER");
			}
		});
		return gameItem;
	}

	private GameItem CardboardBox()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(8, 8).Item1;
		return Empty().SetContentWindow(item).SetSpriteAndShape("Items/itematlas", "cardboard_box");
	}

	private GameItem FirstAidBox()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow("0011100000000000000000000000", 7);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_medical", "first_aid_box");
		gameItem.name = "First Aid Kit";
		ContainerHelper.InitContainerItem(item2, gameItem);
		AudioHelper.InitCase(gameItem);
		gameItem.unitValue = 15L;
		return gameItem;
	}

	private GameItem PillBottle()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(7, 1).Item1;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_medical", "pill_bottle");
		gameItem.name = "Pill Bottle";
		gameItem.EnableTag("ITEM_MEDICAL_TAG");
		gameItem.unitValue = 1L;
		AudioHelper.InitPillBottleAudio(gameItem);
		return gameItem;
	}

	private GameItem BicarsolePillBottle()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(7, 1).Item1;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_medical", "bicarsole_pill_bottle");
		gameItem.name = "Bicarsole Pill Bottle";
		gameItem.EnableManufacturer("carmicheal");
		gameItem.EnableTag("ITEM_MEDICAL_TAG");
		gameItem.unitValue = 1L;
		AudioHelper.InitPillBottleAudio(gameItem);
		return gameItem;
	}

	private GameItem OcutolPillBottle()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(7, 1).Item1;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_medical", "ocutol_pill_bottle");
		gameItem.name = "Ocutol Pill Bottle";
		gameItem.EnableManufacturer("carmicheal");
		gameItem.EnableTag("ITEM_MEDICAL_TAG");
		gameItem.unitValue = 1L;
		AudioHelper.InitPillBottleAudio(gameItem);
		return gameItem;
	}

	private GameItem FixalinPillBottle()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(7, 1).Item1;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_medical", "fixalin_pill_bottle");
		gameItem.name = "Fixalin Pill Bottle";
		gameItem.EnableManufacturer("carmicheal");
		gameItem.EnableTag("ITEM_MEDICAL_TAG");
		gameItem.unitValue = 1L;
		AudioHelper.InitPillBottleAudio(gameItem);
		return gameItem;
	}

	private GameItem PhagimycinPillBottle()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(7, 1).Item1;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_medical", "pillbottle1");
		gameItem.name = "Phagimycin Pill Bottle";
		gameItem.flavorText = "A pill bottle that contain a powerful antibiotic made with bacteriophages and fungi to counter antibiotic resistance.";
		gameItem.EnableManufacturer("carmicheal");
		gameItem.EnableTag("ITEM_MEDICAL_TAG");
		gameItem.unitValue = 1L;
		AudioHelper.InitPillBottleAudio(gameItem);
		return gameItem;
	}

	private GameItem UnlicensedPhagimycinPillBottle()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(7, 1).Item1;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_medical", "pillbottle2");
		gameItem.name = "Unlicensed Phagimycin Pill Bottle";
		gameItem.flavorText = "A pill bottle that contain a powerful unlicensed antibiotic made with bacteriophages and fungi to counter antibiotic resistance.";
		gameItem.EnableManufacturer("spacer");
		gameItem.EnableTag("ITEM_MEDICAL_TAG");
		gameItem.unitValue = 1L;
		AudioHelper.InitPillBottleAudio(gameItem);
		return gameItem;
	}

	private GameItem OxycodonePillBottle()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(7, 1).Item1;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_medical", "oxycodone_pill_bottle");
		gameItem.name = "Oxycodone Pill Bottle";
		gameItem.EnableManufacturer("carmicheal");
		gameItem.EnableTag("ITEM_MEDICAL_TAG");
		gameItem.unitValue = 1L;
		AudioHelper.InitPillBottleAudio(gameItem);
		return gameItem;
	}

	private GameItem BearPawBox()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(4, 4);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_food", "bear_paw_box");
		gameItem.name = "Bear Paw Cookie Box";
		gameItem.EnableManufacturer("davre");
		gameItem.unitValue = 1L;
		ContainerHelper.InitContainerItem(item2, gameItem);
		return gameItem;
	}

	private GameItem LunchBox()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(5, 4);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_container", "lunch_box");
		gameItem.name = "Lunch Box";
		gameItem.EnableManufacturer("pastel");
		gameItem.unitValue = 15L;
		ContainerHelper.InitContainerItem(item2, gameItem);
		return gameItem;
	}

	private GameItem InjectorPouch()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(3, 4);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_container", "injector_pouch");
		gameItem.name = "Injector Pouch";
		gameItem.EnableManufacturer("mnsl");
		gameItem.unitValue = 25L;
		ContainerHelper.AllowOnlyTaggedItem(item2, "injector");
		ContainerHelper.InitPouchItem(gameItem);
		ContainerHelper.InitContainerItemTagOnly(item2, gameItem);
		AudioHelper.InitPouch(gameItem);
		return gameItem;
	}

	private GameItem InjectorPouchProto()
	{
		PixelWindow contentWindow = DirectoryUtils.CreateInjectorPouchInv(3, 4);
		GameItem gameItem = Empty().SetContentWindow(contentWindow).SetSpriteAndShape("Items/items_container", "injector_pouch");
		gameItem.name = "Injector Pouch";
		gameItem.EnableManufacturer("mnsl");
		gameItem.unitValue = 25L;
		ContainerHelper.InitPouchItem(gameItem);
		AudioHelper.InitPouch(gameItem);
		return gameItem;
	}

	private GameItem InjectorPouchLarge()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(5, 4);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_container", "pouch_injector_large");
		gameItem.name = "Large Injector Pouch";
		gameItem.EnableManufacturer("mnsl");
		gameItem.unitValue = 25L;
		ContainerHelper.AllowOnlyTaggedItem(item2, "injector");
		ContainerHelper.InitPouchItem(gameItem);
		ContainerHelper.InitContainerItemTagOnly(item2, gameItem);
		AudioHelper.InitPouch(gameItem);
		return gameItem;
	}

	private GameItem InjectorPouchLargeDouble()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(5, 8);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_container", "pouch_injector_large_double");
		gameItem.name = "Large Double Stacked Injector Pouch";
		gameItem.EnableManufacturer("mnsl");
		gameItem.unitValue = 25L;
		ContainerHelper.AllowOnlyTaggedItem(item2, "injector");
		ContainerHelper.InitPouchItem(gameItem);
		ContainerHelper.InitContainerItemTagOnly(item2, gameItem);
		AudioHelper.InitPouch(gameItem);
		return gameItem;
	}

	private GameItem FirstAidPouch()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(4, 4);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_container", "pouch_medical");
		gameItem.unitValue = 40L;
		ContainerHelper.InitPouchItem(gameItem);
		ContainerHelper.InitContainerItem(item2, gameItem);
		ContainerHelper.AllowOnlyTaggedItem(item2, "ITEM_MEDICAL_TAG");
		gameItem.name = "Medical Pouch";
		gameItem.EnableManufacturer("mnsl");
		AudioHelper.InitPouch(gameItem);
		return gameItem;
	}

	private GameItem PouchGeneral()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(3, 3);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_container", "pouch_general");
		gameItem.unitValue = 30L;
		ContainerHelper.InitPouchItem(gameItem);
		ContainerHelper.InitContainerItem(item2, gameItem);
		gameItem.name = "Pouch";
		gameItem.EnableManufacturer("mnsl");
		AudioHelper.InitPouch(gameItem);
		return gameItem;
	}

	private GameItem PouchGeneralLarge()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(3, 4);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_container", "pouch_general_large");
		gameItem.unitValue = 50L;
		ContainerHelper.InitPouchItem(gameItem);
		ContainerHelper.InitContainerItem(item2, gameItem);
		gameItem.name = "Large Pouch";
		gameItem.EnableManufacturer("mnsl");
		AudioHelper.InitPouch(gameItem);
		return gameItem;
	}

	private GameItem PouchHydration()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_container", "pouch_hydration");
		gameItem.unitValue = 85L;
		ContainerHelper.InitPouchItem(gameItem);
		gameItem.name = "Hydration Pouch";
		gameItem.EnableManufacturer("mnsl");
		AudioHelper.InitPouch(gameItem);
		LiquidContainerHelper.InitLiquidContainerItem(gameItem, 4000, isStartFull: false);
		return gameItem;
	}

	public GameItem LoanContract(string title = "Title", string body = "Body", string signature = "")
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "mentor_contract");
		gameItem.name = "Loan Contract";
		gameItem.shortDescription = "The contract made with the Loan Shark";
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.unitValue = 0L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.onActivateSlotItemFunc = delegate
		{
			LoanPanelUIManager.Instance.OpenUI();
		};
		return gameItem;
	}

	private GameItem Lockpicks()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "lockpicks");
		gameItem.name = "Set of Lockpicks";
		gameItem.unitValue = 20L;
		gameItem.shortDescription = "A set of quality lockpicks, tension wrenches and rakes.";
		SecuredContainerHelper.InitKey(gameItem, "LOCK_TYPE_MECHANICAL", 1, isBreaching: true);
		return gameItem;
	}

	private GameItem ExoticFuel()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "exotic_matter_cell");
		gameItem.name = "Exotic Fuel Cell";
		gameItem.EnableTag("EXOTIC_FUEL_TAG");
		gameItem.shortDescription = "The standard fuel cell used in modern propulsion system allowing FTL travels.";
		gameItem.mayStackItemFunc = (GameItem item, GameItem otherItem) => item.identifier == otherItem.identifier;
		gameItem.maxSlotAmountItemFunc = (GameItem _, GameInventory inventory, SlotMarker _) => 99;
		gameItem.unitCount = 1;
		gameItem.unitValue = 5L;
		return gameItem;
	}

	private GameItem BoxNail()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_material", "box_nail");
		gameItem.name = "Box of nails";
		gameItem.shortDescription = "A box full of nails";
		gameItem.unitValue = 18L;
		gameItem.SetType("MATERIAL_TAG");
		gameItem.SetGameItemType("MATERIAL");
		return gameItem;
	}

	private GameItem TrashPile()
	{
		(PixelWindow, GameSlotInventory, ButtonElement) tuple = DirectoryUtils.CreateTrashPileWindow();
		PixelWindow item2 = tuple.Item1;
		GameSlotInventory gameSlotInventory = tuple.Item2;
		ButtonElement item3 = tuple.Item3;
		GameItem item = Empty().SetSpriteAndShape("Items/itematlas", "trash_pile").SetContentWindow(item2);
		item.name = "Trash Pile";
		item.shortDescription = "A massive pile of junk from your days as a scavenger. Can't be moved. I can search it once per day.";
		item.unitValue = 0L;
		item.EnableTag("CONTAINER_TAG");
		item.EnableTag("IMPORTANT_TAG");
		item.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		item.onCycleEndSlotItemFunc = delegate
		{
			item.DisableTag("PILE_SEARCHED_TODAY");
		};
		gameSlotInventory.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
		UseCountHelper.InitUseCountItem(item, 20, transferable: false);
		item3.SetCallback(delegate
		{
			if (!item.IsTag("PILE_SEARCHED_TODAY") && gameSlotInventory.childItem == null)
			{
				UseCountHelper.Use(item);
				item.EnableTag("PILE_SEARCHED_TODAY");
				if (StoreStation.Instance.dayCounter == 1)
				{
					gameSlotInventory.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => true;
					GraphUtils.TryAcceptAll(gameSlotInventory, DirectoryMaster.Item("scrap_metal"));
					gameSlotInventory.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
				}
				if (StoreStation.Instance.dayCounter == 2)
				{
					gameSlotInventory.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => true;
					GraphUtils.TryAcceptAll(gameSlotInventory, DirectoryMaster.Item("pink_injector"));
					gameSlotInventory.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
				}
				else
				{
					gameSlotInventory.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => true;
					if (RNG.Roll(18))
					{
						GraphUtils.TryAcceptAll(gameSlotInventory, DirectoryMaster.Item("scrap_metal"));
					}
					else if (RNG.Roll(8))
					{
						GraphUtils.TryAcceptAll(gameSlotInventory, DirectoryMaster.Item("flux_agent"));
					}
					else if (RNG.Roll(8))
					{
						GraphUtils.TryAcceptAll(gameSlotInventory, DirectoryMaster.Item("cat_bar"));
					}
					else if (RNG.Roll(8))
					{
						GraphUtils.TryAcceptAll(gameSlotInventory, DirectoryMaster.Item("common_electronic"));
					}
					else if (RNG.Roll(8))
					{
						GraphUtils.TryAcceptAll(gameSlotInventory, DirectoryMaster.Item("red_beer"));
					}
					else if (RNG.Roll(8))
					{
						GraphUtils.TryAcceptAll(gameSlotInventory, DirectoryMaster.Item("birth_control_pill"));
					}
					else if (RNG.Roll(8))
					{
						GraphUtils.TryAcceptAll(gameSlotInventory, DirectoryMaster.Item("processed_meat"));
					}
					else if (RNG.Roll(8))
					{
						GraphUtils.TryAcceptAll(gameSlotInventory, DirectoryMaster.Item("toilet_paper"));
					}
					else if (RNG.Roll(8))
					{
						GraphUtils.TryAcceptAll(gameSlotInventory, DirectoryMaster.Item("kitchen_knife"));
					}
					else if (RNG.Roll(1))
					{
						GraphUtils.TryAcceptAll(gameSlotInventory, DirectoryMaster.Item("scav_token"));
					}
					else if (RNG.Roll(1))
					{
						GraphUtils.TryAcceptAll(gameSlotInventory, DirectoryMaster.Item("module_extractor"));
					}
					else if (RNG.Roll(2))
					{
						GraphUtils.TryAcceptAll(gameSlotInventory, PreBuiltItemHelper.EnergyCredit(RNG.GetRandomInt(0, 100)));
					}
					else if (RNG.Roll(8))
					{
						GraphUtils.TryAcceptAll(gameSlotInventory, DirectoryMaster.Item("nuts_metal"));
					}
					else if (RNG.Roll(4))
					{
						GraphUtils.TryAcceptAll(gameSlotInventory, DirectoryMaster.Item("small_pistol_ammo_nl"));
					}
					else if (RNG.Roll(4))
					{
						GraphUtils.TryAcceptAll(gameSlotInventory, DirectoryMaster.Item("caffeine_pill"));
					}
					else if (RNG.Roll(4))
					{
						GraphUtils.TryAcceptAll(gameSlotInventory, DirectoryMaster.Item("box_cutter"));
					}
					else if (RNG.Roll(6))
					{
						GraphUtils.TryAcceptAll(gameSlotInventory, DirectoryMaster.Item("pipe_weapon"));
					}
					else
					{
						GraphUtils.TryAcceptAll(gameSlotInventory, DirectoryMaster.Item("scrap_metal"));
					}
					gameSlotInventory.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
				}
			}
		});
		return item;
	}

	private GameItem Wrench()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_construction", "wrench_orange");
		gameItem.name = "Wrench";
		gameItem.SetGameItemType("TOOL");
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("MELEE_WEAPON");
		gameItem.unitValue = 30L;
		return gameItem;
	}

	private GameItem SheetSteel(int count = 1)
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_construction", "steel_sheet_rough");
		gameItem.mayStackItemFunc = (GameItem item, GameItem otherItem) => item.identifier == otherItem.identifier;
		gameItem.maxSlotAmountItemFunc = (GameItem _, GameInventory inventory, SlotMarker _) => 10;
		gameItem.unitCount = count;
		gameItem.name = "Steel Sheet";
		gameItem.unitValue = 8L;
		gameItem.SetType("CONSTRUCTION_MATERIAL_TAG");
		return gameItem;
	}

	private GameItem CreateSpectralAnalyser()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "spectral_analyser");
		gameItem.name = "Spectral Analyser";
		gameItem.shortDescription = "Can be use to test the purity of metal ingots";
		gameItem.SetGameItemType("TOOL");
		gameItem.unitValue = 50L;
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("INGOT_PURITY_TAG")));
		gameItem.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("INGOT_PURITY_TAG")));
		gameItem.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			IngotPurityHelper.HandleIngotCheckAnalyser(t);
		});
		return gameItem;
	}

	private GameItem BackpackSmallModular()
	{
		(PixelWindow, GameInventory, GameInventory) tuple = DirectoryUtils.CreateModularBackpackInv(6, 6, isDraggable: true, isCentered: true, 0);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameInventory item3 = tuple.Item3;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_backpack2", "simple_backpack_modular");
		((GameSlotInventory)item2).SetBackgroundFadeSprite("Items/items_container", "injector_pouch");
		AudioHelper.InitBackpack(gameItem);
		gameItem.unitValue = 60L;
		gameItem.name = "Small Modular Backpack";
		ContainerHelper.InitModularBackpack(gameItem);
		((GameSlotInventory)item2).SetTooltipName("[Pouch Slot]");
		ContainerHelper.InitBackpackItem(item3, gameItem);
		ContainerHelper.AllowOnlyTaggedItem(item2, "POUCH_TAG");
		return gameItem;
	}

	private GameItem BackpackMediumModular()
	{
		(PixelWindow, GameInventory, GameInventory, GameInventory) tuple = DirectoryUtils.CreateMediumModularBackpackInv(6, 8, isDraggable: true, isCentered: true, 0);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameInventory item3 = tuple.Item3;
		GameInventory item4 = tuple.Item4;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_backpack2", "simple_backpack_modular_medium");
		((GameSlotInventory)item2).SetBackgroundFadeSprite("Items/items_container", "injector_pouch");
		((GameSlotInventory)item4).SetBackgroundFadeSprite("Items/items_container", "injector_pouch");
		((GameSlotInventory)item2).SetTooltipName("[Pouch Slot]");
		((GameSlotInventory)item4).SetTooltipName("[Pouch Slot]");
		AudioHelper.InitBackpack(gameItem);
		gameItem.unitValue = 130L;
		gameItem.name = "Modular Backpack";
		ContainerHelper.InitModularBackpack(gameItem);
		ContainerHelper.AllowOnlyTaggedItem(item2, "POUCH_TAG");
		ContainerHelper.AllowOnlyTaggedItem(item4, "POUCH_TAG");
		ContainerHelper.InitBackpackItem(item3, gameItem);
		return gameItem;
	}

	private GameItem BackpackLargeModular()
	{
		(PixelWindow, GameInventory, GameInventory, GameInventory) tuple = DirectoryUtils.CreateMediumModularBackpackInv(7, 10, isDraggable: true, isCentered: true, 0);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameInventory item3 = tuple.Item3;
		GameInventory item4 = tuple.Item4;
		GameItem gameItem = EquippableBack().SetContentWindow(item).SetSpriteAndShape("Items/items_backpack2", "simple_backpack_modular_large");
		((GameSlotInventory)item2).SetBackgroundFadeSprite("Items/items_container", "injector_pouch");
		((GameSlotInventory)item4).SetBackgroundFadeSprite("Items/items_container", "injector_pouch");
		((GameSlotInventory)item2).SetTooltipName("[Pouch Slot]");
		((GameSlotInventory)item4).SetTooltipName("[Pouch Slot]");
		AudioHelper.InitBackpack(gameItem);
		gameItem.unitValue = 280L;
		gameItem.name = "Large Modular Backpack";
		ContainerHelper.InitModularBackpack(gameItem);
		ContainerHelper.AllowOnlyTaggedItem(item2, "POUCH_TAG");
		ContainerHelper.AllowOnlyTaggedItem(item4, "POUCH_TAG");
		ContainerHelper.InitBackpackItem(item3, gameItem);
		return gameItem;
	}

	private GameItem MolotovCocktail()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "molotov_cocktail");
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (GameMaster.current.raidManager.CastAbility(s, t))
			{
				s.Destroy();
			}
		};
		gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("molotov_cocktail_burn"));
		gameItem.unitValue = 35L;
		gameItem.name = "Molotov Cocktail";
		AudioHelper.InitGrenade(gameItem);
		return gameItem;
	}

	private GameItem Safe()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(7, 7);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_furniture", "safe");
		gameItem.unitValue = 225L;
		ContainerHelper.InitContainerItem(item2, gameItem);
		gameItem.name = "Safe";
		gameItem.shortDescription = "Items stored inside will not be stolen but can still be found during inspections. Can't store other containers or machines.";
		gameItem.EnableTag("SAFE_TAG");
		return gameItem;
	}

	private GameItem LargeStorageBay()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(20, 20);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "large_storage_bay");
		gameItem.unitValue = 300L;
		ContainerHelper.InitContainerItemTagOnly(item2, gameItem);
		gameItem.name = "Large Storage Bay";
		gameItem.shortDescription = "A large storage bay usually found in specialised cargo ship.";
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		gameItem.EnableTag("SYSTEM_TAG_UTILITY");
		return gameItem;
	}

	private GameItem AloeVera()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "aloe_vera");
		HydroponicHelper.InitSeedItem(gameItem, 100, 6, "aloe_vera");
		gameItem.name = "Aloe Vera Branch";
		gameItem.shortDescription = "A heavily modified plant that's good at producing oxygen and can be craft into medical gel. Aloe Vera branch can be directly planted to grown more.";
		gameItem.unitValue = 10L;
		return gameItem;
	}

	private GameItem Lifeweed()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "nutrifruit");
		gameItem.name = "Lifeweed";
		gameItem.EnableManufacturer("pastel");
		gameItem.unitValue = 10L;
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => !t.name.Contains("Enemy");
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => !t.name.Contains("Enemy");
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (GameMaster.current.raidManager.CastAbility(s, t))
			{
				s.Destroy();
			}
		};
		gameItem.bonusAccuracy = 1000;
		gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("lifeweed_heal"));
		gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("lifeweed_accuracy"));
		gameItem.EnableProficiency("MEDICAL_PROFICIENCY");
		return gameItem;
	}

	private GameItem LifeweedSeed()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "nutrifruit_seed");
		gameItem.name = "Lifeweed Seed";
		gameItem.EnableManufacturer("pastel");
		HydroponicHelper.InitSeedItem(gameItem, 100, 3, "lifeweed");
		return gameItem;
	}

	private GameItem MirageGenerator()
	{
		(PixelWindow, GameInventory, GameInventory) tuple = DirectoryUtils.CreateEquipmentWindow(8, 12);
		PixelWindow item3 = tuple.Item1;
		GameInventory item4 = tuple.Item2;
		GameInventory item5 = tuple.Item3;
		GameItem item2 = Empty().SetSpriteAndShape("Items/itematlas", "mirage_generator").SetContentWindow(item3);
		item2.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item2.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		item2.mayItemTargetItemFunc = (GameItem s, GameItem t) => GraphUtils.FindParentType<GameCharacterItem>(s) == t;
		item2.canItemTargetItemFunc = (GameItem s, GameItem t) => GraphUtils.FindParentType<GameCharacterItem>(s) == t;
		item2.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (EquipmentHelper.IsEquipmentCharged(s))
			{
				GameMaster.current.raidManager.CastAbility(s, t);
				EquipmentHelper.ResetEquipmentDrawProgress(item2);
			}
			else
			{
				CombatLog.log("Mirage Projector is still charging...");
			}
		};
		item2.ability = DirectoryMaster.CombatAbility("mirage_generator");
		EquipmentHelper.InitEquipment(item2, item5, item4, 2, 10, switchOn: true);
		item2.SetType("TECHNICAL_EQUIPMENT_TAG");
		item2.unitValue = 100L;
		return item2;
	}

	private GameItem AntiqueGrenade()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "frag");
		gameItem.name = "Antique Fragmentation Grenade";
		gameItem.shortDescription = "A very old grenade, but still deadly.";
		AudioHelper.InitGrenade(gameItem);
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("MUNITION");
		gameItem.unitValue = 40L;
		return gameItem;
	}

	private GameItem FragGrenade()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "frag");
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (GameMaster.current.raidManager.CastAbility(s, t))
			{
				s.Destroy();
			}
		};
		gameItem.ability = DirectoryMaster.CombatAbility("frag_grenade");
		AudioHelper.InitGrenade(gameItem);
		gameItem.unitValue = 45L;
		gameItem.EnableProficiency("TACTICAL_PROFICIENCY");
		gameItem.SetType("TACTICAL_EQUIPMENT_TAG");
		return gameItem;
	}

	private GameItem IncendiaryGrenade()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "small_incendiary");
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (GameMaster.current.raidManager.CastAbility(s, t))
			{
				s.Destroy();
			}
		};
		gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("molotov_cocktail_burn"));
		gameItem.unitValue = 35L;
		gameItem.name = "Incendiary Grenade";
		AudioHelper.InitGrenade(gameItem);
		gameItem.EnableProficiency("TACTICAL_PROFICIENCY");
		gameItem.SetType("TACTICAL_EQUIPMENT_TAG");
		return gameItem;
	}

	public GameItem SecCard()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "sec_card");
		gameItem.name = "Security Business Card";
		gameItem.flavorText = "Stay Safe.";
		gameItem.unitValue = 0L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.onActivateSlotItemFunc = delegate
		{
			PaperUIManager.Instance.OpenSecCard();
		};
		return gameItem;
	}

	private GameItem VisualInspectionBook()
	{
		(PixelWindow, GameGridInventory) tuple = DirectoryUtils.CreateBookWindow();
		PixelWindow item = tuple.Item1;
		GameGridInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "visual_book").SetContentWindow(item);
		gameItem.name = "Inspection Guide Handbook";
		gameItem.shortDescription = "A handy book that can hold all the visual inspection and appraisal guides.";
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("CONTAINER_TAG");
		gameItem.unitValue = 0L;
		ContainerHelper.AllowOnlyTaggedItemsOr(item2, new string[2] { "GUIDE_PAPER_TAG", "MAGNIFYING_GLASS" });
		return gameItem;
	}

	private GameItem CreateHeart()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_organ", "organ_heart");
		gameItem.name = "Heart";
		gameItem.unitValue = 300L;
		OrganHelper.InitHeart(gameItem);
		gameItem.SetGameItemType("ORGAN");
		return gameItem;
	}

	private GameItem CreateLung()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_organ", "organ_lung_left");
		gameItem.name = "Lung";
		gameItem.unitValue = 1L;
		OrganHelper.InitLung(gameItem);
		return gameItem;
	}

	private GameItem CreateKidney()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_organ", "organ_kidney_left");
		gameItem.name = "Kidney";
		gameItem.unitValue = 1L;
		OrganHelper.InitKidney(gameItem);
		return gameItem;
	}

	private GameItem CreateLiver()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_organ", "organ_liver");
		gameItem.name = "Liver";
		gameItem.unitValue = 1L;
		OrganHelper.InitLiver(gameItem);
		return gameItem;
	}

	private GameItem CreateRevolverLong()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sec", "revolver_long");
		gameItem.name = "Lancewell 7\" Revolver";
		gameItem.shortDescription = "A 7\" Lancewell revolver chambered in .45 AFP. This long barrel variant sport better ballistic performance.";
		gameItem.flavorText = "A simple and highly reliable weapon. Designed to endure extreme conditions, while requiring minimum maintenance.";
		gameItem.EnableManufacturer("frontier");
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("FIREARM");
		gameItem.SetValue(200L);
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		return gameItem;
	}

	private GameItem CreateRevolverShort()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sec", "revolver_short");
		gameItem.name = "Lancewell Snub Nose Revolver";
		gameItem.shortDescription = "A compact revolver chambered in .45 AFP. This one is designed for conceal carry.";
		gameItem.flavorText = "A simple and highly reliable weapon. Designed to endure extreme conditions, while requiring minimum maintenance.";
		gameItem.EnableManufacturer("frontier");
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("FIREARM");
		gameItem.SetValue(180L);
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		return gameItem;
	}

	private GameItem CreateRevolverShortTactical()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sec", "revolver_short_tac");
		gameItem.name = "Lancewell Snub Nose Tactical";
		gameItem.shortDescription = "A compact revolver chambered in .45 AFP with built-in mods. This one is designed for conceal carry.";
		gameItem.flavorText = "A simple and highly reliable weapon. Designed to endure extreme conditions, while requiring minimum maintenance.";
		gameItem.EnableManufacturer("frontier");
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("FIREARM");
		gameItem.SetGameItemType("MIDLINE");
		AudioHelper.InitPistol(gameItem);
		GunHelper.InitGun(gameItem, 25, 55, 85, 40, 65, 35, 10, 75, moddable: false);
		gameItem.SetValue(230L);
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		return gameItem;
	}

	private GameItem AmmoBox()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateInventoryWindow(3, 2);
		PixelWindow item = tuple.Item1;
		GameInventory itemInventory = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_sec", "ammo_box");
		gameItem.unitValue = 1L;
		gameItem.name = "Ammo Box (Unpackaged)";
		gameItem.flavorText = "A paper box used to store ammunition. This one is unpackaged";
		gameItem.shortDescription = "Fill this box with ammunition and package it.";
		gameItem.canToggleSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => itemInventory.childItems.Count != 0;
		gameItem.mayToggleSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => itemInventory.childItems.Count != 0;
		itemInventory.mayInventoryAddItemFunc = (GameItem addedItem, GameInventory parentInventory) => (addedItem.IsGameItemType("AMMUNITION") && GeneralHelper.IsItemOwned(addedItem)) ? true : false;
		gameItem.EnableTag("AMMO_BOX_TAG");
		gameItem.onToggleSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			if (curItem.IsTag("AMMO_BOX_PACKAGED_TAG"))
			{
				AmmoBoxHelper.Unpackage(curItem);
			}
			else
			{
				AmmoBoxHelper.Package(curItem);
				if (curInv != null && curInv == EmporiumEntry.Instance.backInvinvElement)
				{
					PlayerStore.Instance.AddedItemToWeightedArea(curItem);
				}
			}
		};
		return gameItem;
	}

	public GameItem CodeBook()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "codebook");
		gameItem.name = "Codebook";
		gameItem.flavorText = "";
		gameItem.unitValue = 0L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.onActivateSlotItemFunc = delegate
		{
			PaperUIManager.Instance.OpenCodeBook();
		};
		return gameItem;
	}

	private GameItem AloeVeraGel()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "hemostatic_agent");
		gameItem.name = "Bottle of Aloe Vera Gel";
		gameItem.shortDescription = "This gel contains medical propriety and help with wounds when directly applied.";
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t is GameCharacterItem && !t.IsTag("enemy")));
		gameItem.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t is GameCharacterItem && !t.IsTag("enemy")));
		gameItem.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (t is GameCharacterItem && !t.IsTag("enemy") && UseCountHelper.CanUse(s) && GameMaster.current.raidManager.CastAbility(s, t))
			{
				UseCountHelper.Use(s);
			}
		});
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.identifier.Equals("bandage_item")));
		gameItem.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.identifier.Equals("bandage_item")));
		gameItem.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (t.identifier.Equals("bandage_item") && UseCountHelper.CanUse(s))
			{
				GraphUtils.TryAcceptAll(t.parentInventory, DirectoryMaster.Item("topical_bandage_item"));
				t.Destroy();
				UseCountHelper.Use(s);
			}
		});
		gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("aloe_vera_heal"));
		gameItem.EnableProficiency("MEDICAL_PROFICIENCY");
		gameItem.unitValue = 7L;
		gameItem.EnableTag("ITEM_MEDICAL_TAG");
		UseCountHelper.InitUseCountItem(gameItem, 5);
		return gameItem;
	}

	private GameItem HemostaticAgent()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "hemostatic_agent");
		gameItem.name = "Bottle of Hemostatic Powder";
		gameItem.shortDescription = "This contains hemostatic agent that helps with stopping bleeding by assisting the blood to clot. Combine this with a bandage. Do not directly apply on wounds";
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.identifier.Equals("bandage_item")));
		gameItem.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.identifier.Equals("bandage_item")));
		gameItem.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (t.identifier.Equals("bandage_item") && UseCountHelper.CanUse(s))
			{
				GraphUtils.TryAcceptAll(t.parentInventory, DirectoryMaster.Item("hemostatic_bandage_item"));
				t.Destroy();
				UseCountHelper.Use(s);
			}
		});
		gameItem.EnableProficiency("MEDICAL_PROFICIENCY");
		gameItem.unitValue = 10L;
		gameItem.EnableTag("ITEM_MEDICAL_TAG");
		UseCountHelper.InitUseCountItem(gameItem, 5);
		return gameItem;
	}

	public GameItem TestWater()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/water_container", "water_test1");
		gameItem.name = "Test Water";
		gameItem.shortDescription = "";
		gameItem.getItemSpriteModifierForItemFunc = (GameItem machine) => new List<ItemSpriteModifier>
		{
			new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/water_container", "water_test2"), new Vector2(0f, 0f), 0, isFlipped: false, 0, haveMask: false, MetaHelper.HexToColor("9b4a3d"))
		};
		gameItem.Validate();
		return gameItem;
	}

	public GameItem TestWater2()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/water_container", "water_test1");
		gameItem.name = "Test Water 2 (Blue Base)";
		gameItem.shortDescription = "";
		gameItem.getItemSpriteModifierForItemFunc = (GameItem machine) => new List<ItemSpriteModifier>
		{
			new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/water_container", "blue_test_base"), new Vector2(0f, 0f), 0, isFlipped: false, 0, haveMask: false, MetaHelper.HexToColor("9b4a3d"))
		};
		gameItem.Validate();
		return gameItem;
	}

	public GameItem TestWater3()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/water_container", "water_test1");
		gameItem.name = "Test Water 3 (Red Base)";
		gameItem.shortDescription = "";
		gameItem.getItemSpriteModifierForItemFunc = (GameItem machine) => new List<ItemSpriteModifier>
		{
			new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/water_container", "red_test_base"), new Vector2(0f, 0f), 0, isFlipped: false, 0, haveMask: false, MetaHelper.HexToColor("9b4a3d"))
		};
		gameItem.Validate();
		return gameItem;
	}

	private GameItem CreateHeavyHandmadePistolSuppressed()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sec", "heavy_handmade_pistol_s");
		gameItem.name = "Ironcraft 10S";
		gameItem.shortDescription = "A heavy open-sourced pistol chambered in 10mm. This one is contains build-in mods including an integral suppressor.";
		gameItem.flavorText = "This variant is known to be used by Republic intelligence services in operations that require plausible deniability.";
		gameItem.EnableManufacturer("spacer");
		GunHelper.InitGun(gameItem, 38, 45, 40, 40, 40, 45, 10, 70, moddable: false);
		gameItem.SetGameItemType("WEAPON");
		gameItem.SetGameItemType("FIREARM");
		gameItem.SetGameItemType("LOWTECH");
		gameItem.ModifyTag("GUN_SUPPRESSED_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(1);
		});
		gameItem.SetValue(225L);
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		return gameItem;
	}

	private GameItem ArmorTitan10()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_armor", "titan_armor");
		gameItem.name = "Titanite Serie X Armor Plate";
		gameItem.shortDescription = "The latest version of the Republic's standard body armor. While it provides less protection, more importantly it's more durable and much cheaper to produce.";
		gameItem.EnableManufacturer("republic");
		ArmorHelper.InitArmorItem(gameItem, 100, 15, 0, 0, 0, -5, -5);
		AudioHelper.InitArmour(gameItem);
		gameItem.unitValue = 50L;
		return gameItem;
	}

	private GameItem ArmorTitan10L()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_armor", "titan_armor10l");
		gameItem.name = "Titanite Serie X (L) Armor Plate";
		gameItem.shortDescription = "Lightweight variant of the Serie X armor. Issued to security forces and troops stationned in low-risks area. Prefered by Republic's Long Range Reconnaissance Patrol units. It is designed to be lightweight and flexible.";
		gameItem.EnableManufacturer("republic");
		gameItem.SetGameItemType("ARMOR");
		AudioHelper.InitArmour(gameItem);
		gameItem.unitValue = 150L;
		return gameItem;
	}

	private GameItem ArmorTitan10H()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_armor", "titan_armor10h");
		gameItem.name = "Titanite Serie X (H) Armor Plate";
		gameItem.shortDescription = "Heavy variant of the Serie X armor. Designed to be robust and durable, able to withstand the rigors of prolonged combat while providing superior protection. It can be heavy and cumbersome to wear for extended periods.";
		gameItem.EnableManufacturer("republic");
		ArmorHelper.InitArmorItem(gameItem, 150, 20, 5, 3, 0, -6, -8);
		AudioHelper.InitArmour(gameItem);
		gameItem.unitValue = 90L;
		return gameItem;
	}

	private GameItem ArmorTitan10S()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_armor", "titan_armor10s");
		gameItem.name = "Titanite Serie X (S) Armor Plate";
		gameItem.shortDescription = "Rare variant of the Serie X armor. Issued sparingly to Republic's special forces. This variant offer better protection and durability without sacrificing mobility but made it too expensive to ever mass produce. This armor is devoid of any Republic marking on it.";
		gameItem.EnableManufacturer("republic");
		ArmorHelper.InitArmorItem(gameItem, 125, 18, 5, 1, 0, -2, -3);
		AudioHelper.InitArmour(gameItem);
		gameItem.unitValue = 120L;
		return gameItem;
	}

	private GameItem ArmorTitan9()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_armor", "titan_armor9");
		gameItem.name = "Titanite Serie IX Armor Plate";
		gameItem.shortDescription = "An older serie Titanite armour. Provide more protection than the new serie X but is less durable. Prefered by the Republic's veteran troops over it's newer counterpart";
		gameItem.EnableManufacturer("republic");
		ArmorHelper.InitArmorItem(gameItem, 65, 30, 0, 3, 0, -6, -6);
		AudioHelper.InitArmour(gameItem);
		gameItem.unitValue = 100L;
		return gameItem;
	}

	private GameItem BoxTeaBag()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "box_tea_bag");
		gameItem.name = "Box of 100 Iption Black Tea Bag";
		gameItem.AddAmenities("amenities_nutrition", 1, 100);
		return gameItem;
	}

	private GameItem PacketCigaretteBlack()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "cig_black");
		gameItem.name = "Pack of Black Cigarette";
		gameItem.AddAmenities("amenities_substance", 3, 20);
		gameItem.EnableManufacturer("pastel");
		gameItem.EnableTag("CIGARETTE_TAG");
		return gameItem;
	}

	private GameItem Soap()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "soap");
		gameItem.name = "Bar of Soap";
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		gameItem.AddAmenities("amenities_hygene", 1, 25);
		gameItem.EnableManufacturer("pastel");
		return gameItem;
	}

	private GameItem Bedsheet()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_armor", "kotton_fabric");
		gameItem.name = "Bed Sheet";
		gameItem.SetValue(4L);
		return gameItem;
	}

	private GameItem TacitalEntryDevice()
	{
		(PixelWindow, GameInventory, GameInventory) tuple = DirectoryUtils.CreateEquipmentWindow(8, 12);
		PixelWindow item3 = tuple.Item1;
		GameInventory itemInvModule = tuple.Item2;
		GameInventory item4 = tuple.Item3;
		GameItem item2 = Empty().SetSpriteAndShape("Items/itematlas", "mirage_generator").SetContentWindow(item3);
		item2.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item2.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		item2.mayItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("DOOR_TAG");
		item2.canItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("DOOR_TAG");
		item2.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (EquipmentHelper.IsEquipmentCharged(s))
			{
				List<Room> allRooms = GameMaster.current.raidManager.station.GetAllRooms();
				int @int = t.GetTagReadonly("ROOM_ID_INT").GetInt();
				{
					foreach (Room item5 in allRooms)
					{
						if (item5.roomId == @int)
						{
							CombatLog.log("Scanning Room...");
							if (item5.npcParty != null)
							{
								CombatLog.log("T.E.D. has detected " + item5.npcParty.childItems.Count + " entities in the target room.");
							}
							else
							{
								CombatLog.log("T.E.D. has not detected any entity in the target room.");
							}
						}
						EquipmentHelper.ResetEquipmentDrawProgress(item2);
					}
					return;
				}
			}
			CombatLog.log("T.E.D. is still charging...");
		};
		EquipmentHelper.InitEquipment(item2, item4, itemInvModule, 5, 5);
		GameInventory gameInventory = itemInvModule;
		gameInventory.onSlotAddItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameInventory.onSlotAddItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			if (addedItem.IsTag("EQUIPMENT_MODULE_TAG"))
			{
				ModuleHelper.ComputeModuleEffect(itemInvModule, addedItem, item2);
				ModuleHelper.ApplyEquipmentModifiedPower(item2);
			}
		});
		GameInventory gameInventory2 = itemInvModule;
		gameInventory2.onSlotRemoveItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameInventory2.onSlotRemoveItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem addedItem, GameInventory parentInventory, SlotMarker slot)
		{
			if (addedItem.IsTag("EQUIPMENT_MODULE_TAG"))
			{
				ModuleHelper.ComputeModuleEffect(itemInvModule, addedItem, item2);
				ModuleHelper.ApplyEquipmentModifiedPower(item2);
			}
		});
		item2.name = "Tactical Entry Device";
		item2.shortDescription = "T.E.D. for short. A device able to detect the presence of a entity behind a door. Used by police and military in urban operation.";
		item2.unitValue = 250L;
		return item2;
	}

	private GameItem NutrifruitCrate()
	{
		GameItem item = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "nutrifruit_crate");
		item.name = "Crate of Nutrifruit";
		item.unitValue = 32L;
		item.identifier = "nutrifruit_crate";
		GeneralHelper.InitWeight(item, 2000);
		item.SetGameItemType("FOOD");
		FoodItemHelper.InitFreshFood(item);
		item.shortDescription = "A small crate full of Nutrifruits, Activate it to unbox it.";
		item.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		AudioHelper.InitPackagedAudio(item);
		item.canActivateSlotItemFunc = (GameItem currentItem, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(currentItem);
		item.onActivateSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			List<int> list = RNG.DivideWithDeviation(GeneralHelper.GetWeight(item), 4, 0.2f);
			item.modifiedShape.Clone();
			curInv.Expel(item);
			for (int i = 0; i < 4; i++)
			{
				GameItem gameItem = DirectoryMaster.Item("nutrifruit");
				FoodItemHelper.CloneFreshness(item, gameItem);
				GeneralHelper.SetItemWeight(gameItem, list[i]);
				GraphUtils.TryAcceptAll(curInv, gameItem);
				gameItem.Validate();
			}
		};
		return item;
	}

	private GameItem NutrifruitPackaged()
	{
		GameItem item = Empty().SetSpriteAndShape("Items/items_packaged_food", "nutrifruit_packaged");
		item.name = "Packaged Nutrifruit";
		item.EnableManufacturer("pastel");
		item.shortDescription = "A vaccum sealed nutrifruit, grown and packaged on a pastel Megafarm. It has an expiration date printed on the packaging, but it is known that as long the seal is intact, the nutrifruit will not perish. The packaging probably cost more than the fruit it contains.";
		item.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		AudioHelper.InitPackagedAudio(item);
		GameItem gameItem = item;
		gameItem.onActivateSlotItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameItem.onActivateSlotItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			GridShape tempShape = item.modifiedShape.Clone();
			curInv.Expel(item);
			GameItem gameItem2 = DirectoryMaster.Item("nutrifruit");
			curInv.TryInventorySlot(gameItem2, tempShape).TryAcceptOnce();
			gameItem2.Validate();
		});
		item.unitValue = 10L;
		return item;
	}

	private GameItem HighweedSeed()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "nutrifruit_seed");
		gameItem.name = "Highweed Seed";
		gameItem.EnableManufacturer("pastel");
		HydroponicHelper.InitSeedItem(gameItem, 250, 6, "highweed_leaf");
		gameItem.unitValue = 5L;
		return gameItem;
	}

	private GameItem HighweedLeaf()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_hydroponic_plant", "kotton_plant");
		gameItem.name = "Highweed leaf";
		gameItem.EnableManufacturer("pastel");
		gameItem.unitValue = 10L;
		return gameItem;
	}

	private GameItem CreateRangeWeaponAbilityItem(string sheet, string sprite, string[] effectList, int bonusAccuracy, string name, bool canUseOutsideCombat = true, bool triggerOverwatch = false, bool canButcher = false, long value = 1L)
	{
		GameItem item2 = Empty().SetSpriteAndShape(sheet, sprite);
		item2.name = name;
		item2.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item2.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		item2.triggerOverwatch = triggerOverwatch;
		_ = effectList.LongLength;
		switch (DirectoryMaster.CombatAbilityEffect(effectList[0]).abilityTargetType)
		{
		case CombatAbilityEffect.TargetType.singleEnemy:
		{
			GameItem gameItem9 = item2;
			gameItem9.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem9.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("enemy")));
			GameItem gameItem10 = item2;
			gameItem10.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem10.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("enemy")));
			break;
		}
		case CombatAbilityEffect.TargetType.allEnemy:
		{
			GameItem gameItem3 = item2;
			gameItem3.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem3.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("enemy")));
			GameItem gameItem4 = item2;
			gameItem4.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem4.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("enemy")));
			break;
		}
		case CombatAbilityEffect.TargetType.singleFriendly:
		{
			GameItem gameItem5 = item2;
			gameItem5.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem5.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem));
			GameItem gameItem6 = item2;
			gameItem6.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem6.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem));
			break;
		}
		case CombatAbilityEffect.TargetType.singleFriendlyExceptSelf:
		{
			GameItem gameItem13 = item2;
			gameItem13.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem13.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem && GraphUtils.FindParentType<GameCharacterItem>(s) != t));
			GameItem gameItem14 = item2;
			gameItem14.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem14.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem && GraphUtils.FindParentType<GameCharacterItem>(s) != t));
			break;
		}
		case CombatAbilityEffect.TargetType.allFriendly:
		{
			GameItem gameItem7 = item2;
			gameItem7.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem7.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem));
			GameItem gameItem8 = item2;
			gameItem8.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem8.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem));
			break;
		}
		case CombatAbilityEffect.TargetType.self:
		{
			GameItem gameItem11 = item2;
			gameItem11.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem11.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GraphUtils.FindParentType<GameCharacterItem>(s) == t));
			GameItem gameItem12 = item2;
			gameItem12.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem12.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GraphUtils.FindParentType<GameCharacterItem>(s) == t));
			break;
		}
		case CombatAbilityEffect.TargetType.all:
		{
			GameItem gameItem = item2;
			gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t is GameCharacterItem));
			GameItem gameItem2 = item2;
			gameItem2.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem2.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t is GameCharacterItem));
			break;
		}
		}
		item2.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (t is GameCharacterItem { isDead: false })
			{
				GameMaster.current.raidManager.CastAbility(s, t);
				EquipmentHelper.ResetEquipmentDrawProgress(item2);
				EquipmentHelper.DisableCombatEffect(item2);
			}
		};
		for (int i = 0; i < effectList.Length; i++)
		{
			item2.effects.Add(DirectoryMaster.CombatAbilityEffect(effectList[i]));
		}
		item2.bonusAccuracy = bonusAccuracy;
		item2.unitValue = value;
		return item2;
	}

	private GameItem CreatSTANAG(string sprite, int size)
	{
		PixelWindow pixelWindow = new PixelWindow(canMove: true);
		GameSlotInventory element = new GameSlotInventory(2, 2);
		pixelWindow.Attach(element);
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateSlotInventoryWindow(2, 2);
		PixelWindow item2 = tuple.Item1;
		GameInventory item3 = tuple.Item2;
		item3.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => (item.IsTag("cartridge") && item.IsTag("nato556x45")) ? true : false;
		item3.maxSlotAmountItemFunc = (GameItem item, GameInventory inv, SlotMarker slot) => (item.IsTag("cartridge") && item.IsTag("nato556x45")) ? size : 0;
		GameItem gameItem = Empty().SetContentWindow(item2).SetSpriteAndShape("Items/items_weapons_ranged_antique", sprite);
		gameItem.EnableTag("stanag");
		gameItem.EnableTag("magazine");
		gameItem.tooltipRichTextFunc = delegate
		{
			RichTextBuilder richTextBuilder = new RichTextBuilder(30);
			richTextBuilder.AddLine(size + "-Round STANAG Magazine", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true);
			return richTextBuilder;
		};
		gameItem.parentItemShapeModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem _)
		{
			GameItem gameItem3 = GraphUtils.FindParentType<GameItem>(inventory);
			return (gameItem3 == null || !gameItem3.IsTag("m16")) ? null : new GridShapeBuilder().SetDataFill(2, item.modifiedShape.height, 1).SetTransform(4, 1, flipped: false, 0);
		};
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem _)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			GameItem gameItem2 = GraphUtils.FindParentType<GameItem>(inventory);
			if (gameItem2 == null || !gameItem2.IsTag("m16"))
			{
				return list;
			}
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_weapons_ranged_antique", sprite + "_mod"), new Vector2(64f, -16f)));
			return list;
		};
		return gameItem;
	}

	private GameItem CreateM16Receiver()
	{
		PixelWindow pixelWindow = new PixelWindow(canMove: true);
		GameSlotInventory element = new GameSlotInventory(2, 2);
		pixelWindow.Attach(element);
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateSlotInventoryWindow(2, 2);
		PixelWindow item2 = tuple.Item1;
		GameInventory item3 = tuple.Item2;
		item3.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => (item.IsTag("magazine") && item.IsTag("stanag")) ? true : false;
		item3.maxSlotAddItemFunc = (GameItem item, GameInventory inv, SlotMarker slot) => (item.IsTag("magazine") && item.IsTag("stanag")) ? 1 : 0;
		GameItem gameItem = Empty().SetContentWindow(item2).SetSpriteAndShape("Items/items_weapons_ranged_antique", "m16_receiver");
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GraphUtils.FindChildrenType<GameItem>(item).Find((GameItem gi) => gi.IsTag("magazine")) != null;
		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.EnableTag("m16");
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			GameMaster.current.raidManager.CastAbility(s, t);
		};
		gameItem.ability = DirectoryMaster.CombatAbility("weapon_attack");
		gameItem.name = "M16 Receiver";
		return gameItem;
	}

	private GameItem CreateMP9Receiver()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_ranged_antique", "mp9_receiver");
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			GameMaster.current.raidManager.CastAbility(s, t);
		};
		gameItem.ability = DirectoryMaster.CombatAbility("mp9_attack_ability");
		gameItem.name = "MP9 Receiver";
		gameItem.unitValue = 120L;
		AudioHelper.InitSMG(gameItem);
		return gameItem;
	}

	private GameItem CreateUMPReceiver()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_ranged_antique", "ump_receiver");
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			GameMaster.current.raidManager.CastAbility(s, t);
		};
		gameItem.ability = DirectoryMaster.CombatAbility("ump_attack_ability");
		gameItem.name = "UMP Receiver";
		gameItem.unitValue = 175L;
		AudioHelper.InitUMP(gameItem);
		return gameItem;
	}

	private GameItem CreateMagnumReceiver()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_ranged_antique", "magnum_revolver");
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			GameMaster.current.raidManager.CastAbility(s, t);
		};
		gameItem.ability = DirectoryMaster.CombatAbility("ump_attack_ability");
		gameItem.name = ".44 Magnum Revolver";
		gameItem.unitValue = 100L;
		return gameItem;
	}

	private GameItem CreateGlockReceiver2()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_ranged_antique", "glock_receiver2");
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			GameMaster.current.raidManager.CastAbility(s, t);
		};
		gameItem.ability = DirectoryMaster.CombatAbility("glock_attack_ability2");
		gameItem.name = "Antique Improved Handgun";
		gameItem.unitValue = 100L;
		AudioHelper.InitPistol(gameItem);
		return gameItem;
	}

	private GameItem CreateShotgunReceiver()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateSlotInventoryWindow(2, 2);
		PixelWindow item2 = tuple.Item1;
		GameInventory item3 = tuple.Item2;
		item3.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => (item.spritePath.Contains("shotgun_stock") || item.spritePath.Contains("shotgun_grip")) ? true : false;
		item3.maxSlotAddItemFunc = (GameItem item, GameInventory inv, SlotMarker slot) => (item.spritePath.Contains("shotgun_stock") || item.spritePath.Contains("shotgun_grip")) ? 1 : 0;
		GameItem gameItem = Empty().SetContentWindow(item2).SetSpriteAndShape("Items/itematlas", "shotgun");
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GraphUtils.FindChildType<GameInventory>(item).childItems.Any((GameItem i) => i.spritePath.Contains("shotgun_"));
		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			GameMaster.current.raidManager.CastAbility(s, t);
		};
		gameItem.isItemStateValidFunc = (GameItem _, GameInventory _, SlotMarker slot, GridShape _, TagSystem state) => !slot.IsEquipmentSlot() || state.IsTag("mod_grip");
		gameItem.ability = DirectoryMaster.CombatAbility("weapon_attack");
		gameItem.name = "Shotgun Receiver";
		gameItem.tooltipRichTextFunc = delegate(GameItem _, GameInventory _, SlotMarker _, GridShape _, TagSystem state)
		{
			RichTextBuilder richTextBuilder = new RichTextBuilder(30);
			richTextBuilder.AddLine("Shotgun" + (state.IsTag("folded") ? " (Folded)" : ""), ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true);
			return richTextBuilder;
		};
		AudioHelper.InitShotgun(gameItem);
		gameItem.unitValue = 150L;
		return gameItem;
	}

	private GameItem CreateShotgunStock()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "shotgun_stock");
		gameItem.parentItemStateModifierForItemFunc = delegate(GameItem _, GameInventory _, SlotMarker _, GridShape _, TagSystem state, GameItem parentItem)
		{
			List<ItemStateModifier> list2 = new List<ItemStateModifier>();
			if (parentItem.spritePath == "shotgun")
			{
				list2.Add(ItemStateModifier.Create("mod_grip"));
			}
			list2.Add(ItemStateModifier.Create("folded", ItemStateModifier.Operation.SetEnabled).SetEnabled(state.IsTag("folded")));
			return list2;
		};
		gameItem.parentItemShapeModifierForItemFunc = (GameItem item, GameInventory _, SlotMarker _, GridShape _, TagSystem state, GameItem _) => state.IsTag("folded") ? new GridShapeBuilder().SetDataFill(3, 2, 1).SetTransform(2, 0, flipped: false, 0) : new GridShapeBuilder().SetDataFill(3, 2, 1).SetTransform(-3, 0, flipped: false, 0);
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory _, SlotMarker _, GridShape _, TagSystem state, GameItem _)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			if (state.IsTag("folded"))
			{
				list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/itematlas", "shotgun_stock_mod"), new Vector2(9f, -1f), 0, isFlipped: true));
			}
			else
			{
				list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/itematlas", "shotgun_stock_mod"), new Vector2(-49f, -1f)));
			}
			return list;
		};
		gameItem.mayToggleSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canToggleSlotItemFunc = (GameItem _, GameInventory inventory, SlotMarker _) => GraphUtils.FindParentType<GameItem>(inventory)?.spritePath == "shotgun";
		gameItem.tooltipRichTextFunc = delegate(GameItem _, GameInventory _, SlotMarker _, GridShape _, TagSystem state)
		{
			RichTextBuilder richTextBuilder = new RichTextBuilder(30);
			richTextBuilder.AddLine("Shotgun Stock" + (state.IsTag("folded") ? " (Folded)" : ""), ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true);
			return richTextBuilder;
		};
		gameItem.onToggleSlotItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _)
		{
			item.ModifyTag("folded", delegate(TagState state)
			{
				state.ToggleEnabled();
			});
			if (item.CheckValidShapeState())
			{
				item.ValidateShapeState();
			}
			else
			{
				item.ModifyTag("folded", delegate(TagState state)
				{
					state.ToggleEnabled();
				});
			}
		};
		gameItem.onSlotRemoveItemFunc = delegate(GameItem item, GameInventory _, SlotMarker _)
		{
			item.DisableTag("folded");
		};
		return gameItem;
	}

	private GameItem CreateShotgunGrip()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "shotgun_grip");
		gameItem.parentItemStateModifierForItemFunc = delegate(GameItem _, GameInventory _, SlotMarker _, GridShape _, TagSystem _, GameItem parentItem)
		{
			List<ItemStateModifier> list2 = new List<ItemStateModifier>();
			if (parentItem.spritePath == "shotgun")
			{
				list2.Add(ItemStateModifier.Create("mod_grip"));
			}
			return list2;
		};
		gameItem.parentItemShapeModifierForItemFunc = (GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem _) => (GraphUtils.FindParentType<GameItem>(inventory)?.spritePath != "shotgun") ? null : new GridShapeBuilder().SetDataFill(1, 2, 1).SetTransform(-1, 0, flipped: false, 0);
		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem _)
		{
			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
			if (GraphUtils.FindParentType<GameItem>(inventory)?.spritePath != "shotgun")
			{
				return list;
			}
			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/itematlas", "shotgun_grip_mod"), new Vector2(-20f, -3f)));
			return list;
		};
		return gameItem;
	}

	private GameItem BicarsolePill()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "items_medical_11");
		gameItem.name = "Bircarsole Pill";
		MedicalHelper.InitPillItem(gameItem);
		gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("bicarsole_heal"));
		gameItem.EnableManufacturer("carmicheal");
		gameItem.unitValue = 10L;
		return gameItem;
	}

	private GameItem OcutolPill()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "items_medical_10");
		gameItem.name = "Ocutol Pill";
		MedicalHelper.InitPillItem(gameItem);
		gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("ocutol_acc"));
		gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("ocutol_critical"));
		gameItem.EnableManufacturer("carmicheal");
		gameItem.unitValue = 8L;
		return gameItem;
	}

	private GameItem FixalinPill()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "items_medical_12");
		gameItem.name = "Fixalin Pill";
		MedicalHelper.InitPillItem(gameItem);
		gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("fixalin_dodge"));
		gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("fixalin_speed"));
		gameItem.EnableManufacturer("carmicheal");
		gameItem.unitValue = 9L;
		return gameItem;
	}

	private GameItem OxycodonePill()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "oxycodone_pill");
		gameItem.name = "Oxy Pill";
		MedicalHelper.InitPillItem(gameItem);
		gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("fixalin_speed"));
		gameItem.EnableManufacturer("carmicheal");
		gameItem.unitValue = 30L;
		gameItem.shortDescription = "A highly addictive and commonly abused opioid drug used for treatment of moderate and severe pain. The medication kickstarted the Opioid crisis as Big Pharma aggressively pushed and incentivize doctors to over-prescribe this drug, downplaying it's addictive potential. This one is a 10mg pill.";
		return gameItem;
	}

	private GameItem HeroinPowder()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "oxycodone_pill");
		gameItem.name = "Hero Powder";
		MedicalHelper.InitPillItem(gameItem);
		gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("fixalin_speed"));
		gameItem.EnableManufacturer("carmicheal");
		gameItem.unitValue = 25L;
		gameItem.shortDescription = "A extremely addictive and dangerous synthetic opioid drug used for treatment of severe pain. ";
		return gameItem;
	}

	private GameItem CreateStandardCombatAbilityItem(string spriteName, string[] effectList, int bonusAccuracy, string name, bool canUseOutsideCombat = true, bool triggerOverwatch = false)
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/abilityatlas", spriteName);
		gameItem.name = name;
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.triggerOverwatch = triggerOverwatch;
		_ = effectList.LongLength;
		gameItem.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => inv.IsTag("ability") ? true : false;
		gameItem.maxSlotAddItemFunc = delegate(GameItem item, GameInventory inv, SlotMarker _)
		{
			if (item.IsTag("character"))
			{
				string text = GraphUtils.FindParentType<GameCharacterItem>(inv)?.name;
				if (item.GetTagReadonly("character").valueString != text)
				{
					return 0;
				}
			}
			return 1;
		};
		gameItem.onSlotAddItemFunc = delegate(GameItem item, GameInventory inv, SlotMarker _)
		{
			string characterName = GraphUtils.FindParentType<GameCharacterItem>(inv)?.name;
			if (characterName != null)
			{
				item.ModifyTag("character", delegate(TagState state)
				{
					state.Enable().SetString(characterName);
				});
			}
		};
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			GameMaster.current.raidManager.CastAbility(s, t);
		};
		gameItem.EnableTag("ability");
		for (int i = 0; i < effectList.Length; i++)
		{
			gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect(effectList[i]));
		}
		gameItem.bonusAccuracy = bonusAccuracy;
		return gameItem;
	}

	private GameItem CreateMeleeWeaponAbilityItem(string sprite, string[] effectList, int bonusAccuracy, string name, bool canUseOutsideCombat = true, bool triggerOverwatch = false, bool canButcher = false, long value = 1L)
	{
		GameItem item2 = Empty().SetSpriteAndShape("Items/items_weapons_melee", sprite);
		item2.name = name;
		item2.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item2.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		item2.triggerOverwatch = triggerOverwatch;
		_ = effectList.LongLength;
		switch (DirectoryMaster.CombatAbilityEffect(effectList[0]).abilityTargetType)
		{
		case CombatAbilityEffect.TargetType.singleEnemy:
		{
			GameItem gameItem9 = item2;
			gameItem9.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem9.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("enemy")));
			GameItem gameItem10 = item2;
			gameItem10.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem10.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("enemy")));
			break;
		}
		case CombatAbilityEffect.TargetType.allEnemy:
		{
			GameItem gameItem3 = item2;
			gameItem3.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem3.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("enemy")));
			GameItem gameItem4 = item2;
			gameItem4.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem4.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("enemy")));
			break;
		}
		case CombatAbilityEffect.TargetType.singleFriendly:
		{
			GameItem gameItem5 = item2;
			gameItem5.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem5.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem));
			GameItem gameItem6 = item2;
			gameItem6.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem6.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem));
			break;
		}
		case CombatAbilityEffect.TargetType.singleFriendlyExceptSelf:
		{
			GameItem gameItem13 = item2;
			gameItem13.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem13.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem && GraphUtils.FindParentType<GameCharacterItem>(s) != t));
			GameItem gameItem14 = item2;
			gameItem14.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem14.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem && GraphUtils.FindParentType<GameCharacterItem>(s) != t));
			break;
		}
		case CombatAbilityEffect.TargetType.allFriendly:
		{
			GameItem gameItem7 = item2;
			gameItem7.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem7.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem));
			GameItem gameItem8 = item2;
			gameItem8.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem8.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => !t.IsTag("enemy") && t is GameCharacterItem));
			break;
		}
		case CombatAbilityEffect.TargetType.self:
		{
			GameItem gameItem11 = item2;
			gameItem11.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem11.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GraphUtils.FindParentType<GameCharacterItem>(s) == t));
			GameItem gameItem12 = item2;
			gameItem12.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem12.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => GraphUtils.FindParentType<GameCharacterItem>(s) == t));
			break;
		}
		case CombatAbilityEffect.TargetType.all:
		{
			GameItem gameItem = item2;
			gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t is GameCharacterItem));
			GameItem gameItem2 = item2;
			gameItem2.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem2.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t is GameCharacterItem));
			break;
		}
		}
		GameItem gameItem15 = item2;
		gameItem15.onItemTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem15.onItemTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (t is GameCharacterItem { isDead: false })
			{
				GameMaster.current.raidManager.CastAbility(s, t);
				EquipmentHelper.ResetEquipmentDrawProgress(item2);
				EquipmentHelper.DisableCombatEffect(item2);
			}
			if (t.identifier.Equals("bedsheet") && canButcher)
			{
				GraphUtils.TryAcceptAll(t.parentInventory, DirectoryMaster.Item("bandage_item"));
				GraphUtils.TryAcceptAll(t.parentInventory, DirectoryMaster.Item("bandage_item"));
				t.Destroy();
			}
		});
		for (int i = 0; i < effectList.Length; i++)
		{
			item2.effects.Add(DirectoryMaster.CombatAbilityEffect(effectList[i]));
		}
		item2.bonusAccuracy = bonusAccuracy;
		if (canButcher)
		{
			item2.EnableTag("butchering");
			GameItem gameItem16 = item2;
			gameItem16.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem16.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("enemy") || t is GameCharacterItem { isDead: not false } || t.identifier.Equals("bedsheet")));
			GameItem gameItem17 = item2;
			gameItem17.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem17.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("enemy") || t is GameCharacterItem { isDead: not false } || t.identifier.Equals("bedsheet")));
			AudioHelper.InitSharpWeapon(item2);
		}
		else
		{
			AudioHelper.InitBluntWeapon(item2);
		}
		item2.unitValue = value;
		return item2;
	}

	private GameItem Coin(int count = 1)
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "coin");
		gameItem.mayStackItemFunc = (GameItem item, GameItem otherItem) => item.spritePath == otherItem.spritePath;
		gameItem.maxSlotAmountItemFunc = (GameItem _, GameInventory inventory, SlotMarker _) => 5;
		gameItem.unitCount = count;
		return gameItem;
	}

	private GameItem TestCoin()
	{
		GameItem gameItem2 = Empty().SetSpriteAndShape("Items/itematlas", "coin");
		gameItem2.name = "Debug Coin";
		gameItem2.onCycleEndEarlySlotItemFunc = delegate(GameItem gameItem, GameInventory _, SlotMarker _)
		{
			foreach (GameItem item in gameItem.FindAdjacentType<GameItem>())
			{
				Debug.Log(item.name);
			}
		};
		return gameItem2;
	}

	private GameItem EnergyCreditCounterfeit()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "energy_credit_counterfeit");
		gameItem.name = "Energy Cell";
		gameItem.EnableManufacturer("eninence");
		PowerHelper.InitPowerSourceItem(gameItem, 30, 4);
		gameItem.unitBaseValue = 4L;
		return gameItem;
	}

	private GameItem Wall()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/connections", "wall");
		gameItem.name = "Wall";
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		SalvageHelper.InitSalvagableItem(gameItem, destroy_after_salvage: false, "wall_stripped");
		return gameItem;
	}

	private GameItem WallStripped()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/connections", "stripped_wall");
		gameItem.name = "Stripped Wall";
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		return gameItem;
	}

	private GameItem WallMold()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/connections", "wall_mold");
		gameItem.name = "Modly Wall";
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		return gameItem;
	}

	private GameItem WallWindow()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/connections", "wall_mold");
		gameItem.name = "Window Wall";
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetThisFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayItemTargetThisFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag(MeleeWeaponItemDirectory.MELEE_WEAPON_TAG)));
		gameItem.canItemTargetThisFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.canItemTargetThisFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag(MeleeWeaponItemDirectory.MELEE_WEAPON_TAG)));
		return gameItem;
	}

	private GameItem WallWindowSmashed()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/connections", "wall_mold");
		gameItem.name = "Smashed Window Wall";
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		return gameItem;
	}

	private GameItem IntelItem()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "intel_blue");
		gameItem.name = "Blank Inten Item";
		gameItem.shortDescription = "";
		gameItem.unitValue = 200L;
		return gameItem;
	}

	private GameItem NavDisk()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "intel_blue");
		gameItem.name = "Nav Disk";
		gameItem.shortDescription = "";
		gameItem.unitValue = 0L;
		NavDiskHelper.InitNavDisk(gameItem);
		return gameItem;
	}

	private GameItem IntelSpawner()
	{
		(PixelWindow, GameGridInventory, ButtonElement) tuple = DirectoryUtils.CreateIntelSpawnerWindow();
		PixelWindow item = tuple.Item1;
		GameGridInventory gridInventory = tuple.Item2;
		ButtonElement item2 = tuple.Item3;
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "intel_tan").SetContentWindow(item);
		gameItem.name = "Intel Spawn";
		gameItem.shortDescription = "";
		item2.SetCallback(delegate
		{
			GraphUtils.TryAcceptAll(gridInventory, IntelUnusedHelper.SpawnRandomNewIntel());
		});
		gameItem.unitValue = 200L;
		return gameItem;
	}

	private GameItem EventDebugger()
	{
		(PixelWindow, GameGridInventory, ButtonElement) tuple = DirectoryUtils.CreateEventDebuggerWindow();
		PixelWindow item = tuple.Item1;
		ButtonElement item2 = tuple.Item3;
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "intel_tan").SetContentWindow(item);
		gameItem.name = "Event Debugger";
		gameItem.shortDescription = "";
		item2.SetCallback(delegate
		{
		});
		gameItem.unitValue = 200L;
		return gameItem;
	}

	private GameItem CombatScavengeItem()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "c4");
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => GraphUtils.FindParentType<GameCharacterItem>(s) == t;
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => GraphUtils.FindParentType<GameCharacterItem>(s) == t;
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (GameMaster.current.raidManager.CastAbility(s, t))
			{
				s.Destroy();
			}
		};
		gameItem.ability = DirectoryMaster.CombatAbility("combat_scav");
		gameItem.name = "Scavenger Item";
		gameItem.customText = "Scav.";
		gameItem.shortDescription = "An item used to scavenge during combat.";
		gameItem.longDescription = "During the great war of 2056, a platoon leader that was a scavenger in his past live thought: \"How amazing would it be if we could scavenge during fights?\", and so, the scavenger item was born after long nights of research in his scavenger base.";
		return gameItem;
	}

	private GameItem PassengerCabin()
	{
		PixelWindow contentWindow = DirectoryUtils.CreatePassengerCabinWindow();
		GameItem gameItem = Empty().SetContentWindow(contentWindow).SetSpriteAndShape("Items/items_ship", "storage_bay");
		gameItem.unitValue = 200L;
		gameItem.name = "Passenger Cabin";
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		gameItem.EnableTag("SYSTEM_TAG_UTILITY");
		return gameItem;
	}

	private GameItem NavigationSystem()
	{
		(PixelWindow, GameInventory, GameInventory) tuple = DirectoryUtils.CreateNavSystemWindow(8, 12);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameInventory item3 = tuple.Item3;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "nav_system");
		gameItem.unitValue = 200L;
		gameItem.name = "Navigation System";
		string[] tags = new string[2] { "MODULE_TYPE_UNIVERSAL", "MODULE_TYPE_NAVIGATION" };
		ContainerHelper.AllowOnlyTaggedItemsOr(item2, tags);
		ContainerHelper.AllowOnlyTaggedItem(item3, "NAV_DISK_TAG");
		ShipHelper.InitNavigationSystem(gameItem, item2, 2);
		return gameItem;
	}

	private GameItem PropulsionSystem()
	{
		(PixelWindow, GameInventory, GameInventory) tuple = DirectoryUtils.CreatePropulsionSystemWindow(3, 3);
		PixelWindow item2 = tuple.Item1;
		GameInventory item3 = tuple.Item2;
		GameInventory item4 = tuple.Item3;
		GameItem gameItem = Empty().SetContentWindow(item2).SetSpriteAndShape("Items/items_ship", "engine");
		gameItem.unitValue = 200L;
		gameItem.name = "Propulsion System";
		string[] tags = new string[1] { "MODULE_TYPE_UNIVERSAL" };
		ContainerHelper.AllowOnlyTaggedItemsOr(item3, tags);
		((GameSlotInventory)item4).SetBackgroundFadeSprite("Items/itematlas", "exotic_matter_cell");
		item4.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => item.identifier.Equals("exotic_fuel");
		ShipHelper.InitPropulsionSystem(gameItem, item3, 10, 10);
		return gameItem;
	}

	private GameItem PowerSystem()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_armor", "armor_lining");
		gameItem.name = "Power System";
		return gameItem;
	}

	private GameItem HydroponicSystem()
	{
		(PixelWindow, GameInventory, GameInventory, GameInventory) tuple = DirectoryUtils.CreateHydroponicSystemWindow(8, 12);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameInventory item3 = tuple.Item3;
		GameInventory item4 = tuple.Item4;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "hydroponic_system");
		gameItem.name = "Hydroponic System";
		gameItem.shortDescription = "A compact and portable hydroponic farm.";
		((GameSlotInventory)item3).SetBackgroundFadeSprite("Items/items_hydroponic_plant", "nutrifruit_seed");
		string[] tags = new string[2] { "MODULE_TYPE_UNIVERSAL", "MODULE_TYPE_HYDROPONIC" };
		ContainerHelper.AllowOnlyTaggedItemsOr(item2, tags);
		ContainerHelper.AllowOnlyTaggedItem(item3, "SEED_ITEM_TAG");
		ShipHelper.InitHydroponicSystem(gameItem, item2, item3, item4, 4, 20);
		gameItem.unitValue = 1000L;
		return gameItem;
	}

	private GameItem ArmorConditionerSystem()
	{
		(PixelWindow, GameInventory, GameInventory) tuple = DirectoryUtils.CreateArmorConditionerSystemWindow();
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameInventory item3 = tuple.Item3;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "armor_conditioner_system");
		gameItem.name = "Armor Conditioner System";
		((GameSlotInventory)item3).SetBackgroundFadeSprite("Items/items_armor", "armor_lining");
		string[] tags = new string[1] { "MODULE_TYPE_UNIVERSAL" };
		ContainerHelper.AllowOnlyTaggedItemsOr(item2, tags);
		ContainerHelper.AllowOnlyTaggedItem(item3, "armor_item");
		ShipHelper.InitArmorConditionerSystem(gameItem, item2, item3, 4, 5);
		gameItem.unitValue = 1000L;
		return gameItem;
	}

	private GameItem BioPrinterSystem()
	{
		(PixelWindow, GameInventory, GameInventory, GameInventory) tuple = DirectoryUtils.CreateBioPrinterSystemWindow();
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameInventory item3 = tuple.Item3;
		GameInventory item4 = tuple.Item4;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "bioprinter_system");
		gameItem.name = "Bio Printer System";
		((GameSlotInventory)item3).SetBackgroundFadeSprite("Items/items_food", "small_raw_meat");
		string[] tags = new string[1] { "MODULE_TYPE_UNIVERSAL" };
		string[] tags2 = new string[2] { "ORGANIC_ORGAN_TAG", "BIO_PRINTABLE_TAG" };
		ContainerHelper.AllowOnlyTaggedItemsOr(item2, tags);
		ContainerHelper.AllowOnlyTaggedItemsOr(item3, tags2);
		ShipHelper.InitBioPrinterSystem(gameItem, item2, item3, item4, 4, 4);
		gameItem.unitValue = 1000L;
		return gameItem;
	}

	private GameItem WaterRecyclerSystem()
	{
		(PixelWindow, GameInventory, GameInventory, GameInventory) tuple = DirectoryUtils.CreateWaterRecyclerSystemWindow();
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameInventory item3 = tuple.Item3;
		GameInventory item4 = tuple.Item4;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "water_recycler_full");
		gameItem.name = "Water Recycler System";
		((GameSlotInventory)item3).SetBackgroundFadeSprite("Items/items_food", "water_jug");
		((GameSlotInventory)item4).SetBackgroundFadeSprite("Items/items_food", "canteen");
		string[] tags = new string[1] { "MODULE_TYPE_UNIVERSAL" };
		ContainerHelper.AllowOnlyTaggedItemsOr(item2, tags);
		ContainerHelper.AllowOnlyTaggedItem(item3, "LIQUID_CONTAINER_TAG");
		ContainerHelper.AllowOnlyTaggedItem(item4, "LIQUID_CONTAINER_TAG");
		ShipHelper.InitWaterRecyclerSystem(gameItem, item2, item3, item4, 2);
		gameItem.unitValue = 1000L;
		return gameItem;
	}

	private GameItem AutonomousMedicalSystem()
	{
		(PixelWindow, GameInventory, GameInventory) tuple = DirectoryUtils.CreateAutonomousMedicalSystemWindow();
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameInventory item3 = tuple.Item3;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "medical_system");
		gameItem.name = "Autonomous Medical System";
		gameItem.shortDescription = "A standard AMS machine. Capable of tending to external wounds and mild medical ailement.";
		string[] tags = new string[1] { "MODULE_TYPE_UNIVERSAL" };
		ContainerHelper.AllowOnlyTaggedItemsOr(item2, tags);
		ShipHelper.InitAutonomousMedicalSystem(gameItem, item2, item3, 2);
		gameItem.unitValue = 1000L;
		return gameItem;
	}

	private GameItem MakeShiftHullSystem()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateModuleInv(8, 12);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "makeshift_hull");
		gameItem.unitValue = 200L;
		string[] tags = new string[1] { "MODULE_TYPE_HULL" };
		ContainerHelper.AllowOnlyTaggedItemsOr(item2, tags);
		gameItem.name = "Makeshift Hull";
		gameItem.EnableManufacturer("spacer");
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		gameItem.EnableTag("SYSTEM_TAG_HULL");
		HullHelper.InitMakeshiftHull(gameItem, item2);
		return gameItem;
	}

	private GameItem CivilianHullSystem()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateModuleInv(8, 12);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "civilian_hull");
		gameItem.unitValue = 200L;
		string[] tags = new string[1] { "MODULE_TYPE_HULL" };
		ContainerHelper.AllowOnlyTaggedItemsOr(item2, tags);
		gameItem.name = "Civilian-Grade Hull";
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		gameItem.EnableTag("SYSTEM_TAG_HULL");
		HullHelper.InitCivilianHull(gameItem, item2);
		return gameItem;
	}

	private GameItem HeavyDutyHullSystem()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateModuleInv(8, 12);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "heavy_duty_hull");
		gameItem.unitValue = 200L;
		string[] tags = new string[1] { "MODULE_TYPE_HULL" };
		ContainerHelper.AllowOnlyTaggedItemsOr(item2, tags);
		gameItem.name = "Heavy Duty Hull";
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		gameItem.EnableTag("SYSTEM_TAG_HULL");
		HullHelper.InitHeavyDutyHull(gameItem, item2);
		return gameItem;
	}

	private GameItem ArmoredHullSystem()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateModuleInv(8, 12);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "armored_hull");
		gameItem.unitValue = 200L;
		string[] tags = new string[1] { "MODULE_TYPE_HULL" };
		ContainerHelper.AllowOnlyTaggedItemsOr(item2, tags);
		gameItem.name = "Armored Hull";
		gameItem.EnableManufacturer("republic_navy");
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		gameItem.EnableTag("SYSTEM_TAG_HULL");
		HullHelper.InitArmoredHull(gameItem, item2);
		return gameItem;
	}

	private GameItem HullRepairSystem()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateModuleInv(8, 12);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "nav_system");
		gameItem.unitValue = 200L;
		gameItem.name = "Autonomous Repair System";
		gameItem.shortDescription = "A robotic bay storing semi-autonomous drones capable of conducting limited external repair on most spacecraft, including on itself.";
		string[] tags = new string[1] { "MODULE_TYPE_UNIVERSAL" };
		ContainerHelper.AllowOnlyTaggedItemsOr(item2, tags);
		ShipHelper.InitHullRepairSystem(gameItem, item2, 2);
		gameItem.unitValue = 1000L;
		return gameItem;
	}

	private GameItem MiningDrillSystem()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateModuleInv(8, 12);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "mining_system");
		gameItem.unitValue = 200L;
		gameItem.name = "Mining Drill System";
		string[] tags = new string[1] { "MODULE_TYPE_UNIVERSAL" };
		ContainerHelper.AllowOnlyTaggedItemsOr(item2, tags);
		ShipHelper.InitMiningDrillSystem(gameItem, item2, 1);
		gameItem.unitValue = 1000L;
		return gameItem;
	}

	private GameItem SalvageGantrySystem()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateModuleInv(8, 12);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "salvage_system");
		gameItem.unitValue = 200L;
		gameItem.name = "Salvage Gantry System";
		string[] tags = new string[1] { "MODULE_TYPE_UNIVERSAL" };
		ContainerHelper.AllowOnlyTaggedItemsOr(item2, tags);
		ShipHelper.InitSalvageGantrySystem(gameItem, item2, 1);
		gameItem.unitValue = 1000L;
		return gameItem;
	}

	private GameItem RailgunSystem()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateModuleInv(8, 12);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "nav_system");
		gameItem.unitValue = 200L;
		gameItem.name = "Railgun System";
		string[] tags = new string[1] { "MODULE_TYPE_UNIVERSAL" };
		ContainerHelper.AllowOnlyTaggedItemsOr(item2, tags);
		return gameItem;
	}

	private GameItem PointDefenseSystem()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateModuleInv(8, 12);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "nav_system");
		gameItem.unitValue = 200L;
		gameItem.name = "Point Defense System";
		string[] tags = new string[1] { "MODULE_TYPE_UNIVERSAL" };
		ContainerHelper.AllowOnlyTaggedItemsOr(item2, tags);
		return gameItem;
	}

	private GameItem XrayArtillerySystem()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateModuleInv(8, 12);
		PixelWindow item = tuple.Item1;
		GameInventory item2 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_ship", "nav_system");
		gameItem.unitValue = 200L;
		gameItem.name = "X-Ray Artillery System";
		gameItem.shortDescription = "The X-Ray Artillery fires high energy blast capable of traversing station and ship hull to target organic target";
		string[] tags = new string[1] { "MODULE_TYPE_UNIVERSAL" };
		ContainerHelper.AllowOnlyTaggedItemsOr(item2, tags);
		return gameItem;
	}

	private GameItem CreateCraftingTable()
	{
		GameItem gameItem = Empty().SetName("Crafting Table").SetSpriteAndShape("Items/items_machine", "meat_culture_machine");
		CraftingRecipeManager craftingRecipeManager = CraftingHelper.InitCraftingTable(gameItem);
		craftingRecipeManager.AddRecipe(new CraftingRecipe("Mint: Coins (x2)").AddMaterial(new CraftingIdentifierFilter("Metal Sheet", 1, "steel_sheet")).AddTool(new CraftingIdentifierFilter("Wrench", "wrench_orange")).SetCraftingConstantFunc("coin", 2));
		craftingRecipeManager.AddRecipe(new CraftingRecipe("Scrap: Wrench").AddMaterialIdentifier("wrench_orange").SetCraftingConstantFunc("steel_sheet"));
		craftingRecipeManager.AddRecipe(new CraftingRecipe("Battery to bandage").AddToolTag("Any Battery", 10, "power_source_item").SetCraftingConstantFunc("bandage_item"));
		craftingRecipeManager.AddRecipe(new CraftingRecipe("Scrap: Wrench").AddMaterial(WRENCH_FILTER.ModifyClone(2)).SetCraftingConstantFunc("steel_sheet"));
		craftingRecipeManager.AddRecipe("kotton_fabric", "armor_lining");
		craftingRecipeManager.AddRecipe("armor_lining", 1, "kotton_fabric", 2);
		return gameItem;
	}

	private GameItem CreateTestVendingMachine()
	{
		GameItem gameItem = Empty().SetName("Test Vending Machine").SetSpriteAndShape("Items/items_machine", "meat_culture_machine");
		VendingRecipeManager vendingRecipeManager = CraftingHelper.InitVendingTable(gameItem);
		vendingRecipeManager.AddVendingAnyEnergy(10, "bandage_item", 1, 4);
		vendingRecipeManager.AddVendingAnyEnergy(5, "coin");
		return gameItem;
	}

	private GameItem LargeCardboardBox()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(16, 16).Item1;
		return Empty().SetContentWindow(item).SetSpriteAndShape("Items/itematlas", "cardboard_box");
	}

	private GameItem CreateIndustrialFridge()
	{
		(PixelWindow, GameInventory, GameInventory) tuple = DirectoryUtils.CreateDoubleInventoryWindowSlotInput(8, 12);
		PixelWindow item2 = tuple.Item1;
		GameInventory item3 = tuple.Item2;
		GameItem gameItem = Empty().SetContentWindow(item2).SetSpriteAndShape("Items/items_machine", "industrial_fridge");
		gameItem.name = "Industrial Fridge";
		item3.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => item.identifier.Equals("energy_credit") ? true : false;
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		((GameSlotInventory)item3).SetBackgroundFadeSprite("Items/itematlas", "energy_credit");
		return gameItem;
	}

	private GameItem CreateSafe()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(8, 8).Item1;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_furniture", "safe");
		gameItem.name = "Safe";
		SecuredContainerHelper.InitSecuredContainer(gameItem, "LOCK_TYPE_MECHANICAL", 1);
		return gameItem;
	}

	private GameItem CreateGunSafe()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(8, 8).Item1;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_furniture", "safe");
		gameItem.name = "Gun Safe";
		SecuredContainerHelper.InitSecuredContainer(gameItem, "LOCK_TYPE_MECHANICAL", 1);
		return gameItem;
	}

	private GameItem CreateBed()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(8, 8).Item1;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_furniture", "safe");
		gameItem.name = "Bed";
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		gameItem.SetType("FOURNITURE_TAG");
		return gameItem;
	}

	private GameItem CreateMirrorCabinet()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(8, 8).Item1;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_furniture", "safe");
		gameItem.name = "Mirror Cabinet";
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		gameItem.SetType("FOURNITURE_TAG");
		return gameItem;
	}

	private GameItem CreateEndTable()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(8, 8).Item1;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_furniture", "safe");
		gameItem.name = "End Table";
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		gameItem.SetType("FOURNITURE_TAG");
		return gameItem;
	}

	private GameItem CreateDesk()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(8, 8).Item1;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_furniture", "safe");
		gameItem.name = "Desk";
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		gameItem.SetType("FOURNITURE_TAG");
		return gameItem;
	}

	private GameItem CreateSecuritySign()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(8, 8).Item1;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_furniture", "safe");
		gameItem.name = "Security Sign";
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		gameItem.SetType("FOURNITURE_TAG");
		return gameItem;
	}

	private GameItem AmenitiesDispenser()
	{
		PixelWindow item2 = DirectoryUtils.CreateInventoryWindow(16, 16).Item1;
		GameItem gameItem = Empty().SetContentWindow(item2).SetSpriteAndShape("Items/itematlas", "cardboard_box");
		gameItem.name = "Amenities Dispenser";
		gameItem.EnableTag("amenities_storage");
		gameItem.onCycleEndSlotItemFunc = delegate(GameItem item, GameInventory parentInventory, SlotMarker slot)
		{
			GraphUtils.TryAcceptAll(item.contentWindow, DirectoryMaster.Item("coin"));
		};
		return gameItem;
	}

	private GameItem WaterDispenser()
	{
		(PixelWindow, GameInventory, GameInventory) tuple = DirectoryUtils.CreateDoubleInventoryWindow(8, 8);
		PixelWindow item2 = tuple.Item1;
		GameInventory itemInventoryLeft = tuple.Item2;
		GameInventory itemInventoryRight = tuple.Item3;
		GameItem gameItem = Empty().SetContentWindow(item2).SetSpriteAndShape("Items/itematlas", "cardboard_box");
		gameItem.name = "Water Dispenser";
		gameItem.EnableTag("dispenser");
		gameItem.onCycleEndSlotItemFunc = delegate
		{
			List<GameItem> items = GraphUtils.FindAllChildrenType(itemInventoryLeft, (GameItem item) => item.identifier == "coin");
			if (GraphUtils.CountAll(items) > 2 && GraphUtils.TryAcceptAll(itemInventoryRight, DirectoryMaster.Item("bottled_water")) == 0)
			{
				GraphUtils.TryDestroyAll(items, 2);
			}
		};
		return gameItem;
	}

	private GameItem AutomaticMeatProcessor()
	{
		(PixelWindow, GameInventory, GameInventory) tuple = DirectoryUtils.CreateDoubleInventoryWindow(10, 10);
		PixelWindow item2 = tuple.Item1;
		GameInventory itemInventoryLeft = tuple.Item2;
		GameInventory itemInventoryRight = tuple.Item3;
		GameItem gameItem = Empty().SetContentWindow(item2).SetSpriteAndShape("Items/items_machine", "automatic_meat_processor");
		itemInventoryRight.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
		gameItem.name = "Automatic Meat Processor";
		gameItem.EnableTag("manufacture");
		gameItem.onCycleEndSlotItemFunc = delegate
		{
			itemInventoryRight.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => true;
			List<GameItem> list = GraphUtils.FindAllChildrenType(itemInventoryLeft, (GameItem item) => item.identifier == "small_raw_meat");
			GraphUtils.FindAllChildrenType(itemInventoryLeft, (GameItem item) => item.identifier == "raw_meat");
			GraphUtils.FindAllChildrenType(itemInventoryLeft, (GameItem item) => item.identifier == "fat_meat");
			GraphUtils.FindAllChildrenType(itemInventoryLeft, (GameItem item) => item.identifier == "meat_scrap");
			List<GameItem> items = GraphUtils.FindAllChildrenType(itemInventoryLeft, (GameItem item) => item.identifier == "steel_sheet");
			List<GameCharacterItem> source = GraphUtils.FindAllChildrenType(itemInventoryLeft, (GameCharacterItem item) => item.IsTag("humanoid"));
			if (source.Any() && GraphUtils.CountAll(items) >= 1)
			{
				if (GraphUtils.TryAcceptAll(itemInventoryRight, DirectoryMaster.Item("processed_meat").SetAmount(5)) == 0)
				{
					GraphUtils.TryDestroyAll(source.Cast<GameItem>().ToList(), 1);
					GraphUtils.TryDestroyAll(items, 1);
				}
			}
			else if (GraphUtils.CountAll(items) >= 1 && list.Any() && GraphUtils.TryAcceptAll(itemInventoryRight, DirectoryMaster.Item("processed_meat").SetAmount(5)) == 0)
			{
				GraphUtils.TryDestroyAll(list, 1);
				GraphUtils.TryDestroyAll(items, 1);
			}
			itemInventoryRight.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
		};
		return gameItem;
	}

	private GameItem AutomaticMeatProcessorExample()
	{
		(PixelWindow, GameInventory, GameInventory) tuple = DirectoryUtils.CreateDoubleInventoryWindow(10, 10);
		PixelWindow item2 = tuple.Item1;
		GameInventory itemInventoryLeft = tuple.Item2;
		GameInventory itemInventoryRight = tuple.Item3;
		GameItem gameItem = Empty().SetContentWindow(item2).SetSpriteAndShape("Items/items_machine", "automatic_meat_processor");
		itemInventoryRight.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
		gameItem.name = "Automatic Meat Processor";
		gameItem.EnableTag("manufacture");
		gameItem.EnableManufacturer("davre");
		List<CraftingRecipe> allRecipes = new List<CraftingRecipe>();
		CraftingRecipe item3 = new CraftingRecipe("Automatic Small Meat Processing").AddMaterial(new CraftingIdentifierFilter("Any Small Meat", 2, "small_raw_meat", "fat_meat")).AddMaterial(new CraftingIdentifierFilter("Any Metal", 1, "steel_sheet", "meat_scrap")).SetCraftingConstantFunc("processed_meat", 4);
		CraftingRecipe item4 = new CraftingRecipe("Automatic Small Meat Processing").AddMaterial(new CraftingCustomFilter("Any Small Meat", 2, (GameItem item) => item.IsTag("small_meat"))).AddMaterial(new CraftingCustomFilter("Any Metal", 1, (GameItem item) => item.IsTag("metal") && item.IsTag("material"))).SetCraftingConstantFunc("processed_meat", 4);
		CraftingRecipe item5 = new CraftingRecipe("Automatic Large Meat Processing").AddMaterial(new CraftingIdentifierFilter("Any Large Meat", 1, "raw_meat")).AddMaterial(new CraftingIdentifierFilter("Any Metal", 1, "steel_sheet", "meat_scrap")).SetCraftingConstantFunc("processed_meat", 5);
		allRecipes.Add(item3);
		allRecipes.Add(item4);
		allRecipes.Add(item5);
		gameItem.onCycleEndSlotItemFunc = delegate
		{
			itemInventoryRight.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => true;
			foreach (CraftingRecipe item6 in allRecipes)
			{
				if (item6.CanCraft(itemInventoryLeft))
				{
					item6.TryCraftInside(itemInventoryLeft, itemInventoryRight);
					break;
				}
			}
			itemInventoryRight.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
		};
		MachineryHelper.InitMachinery(gameItem, 5);
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		return gameItem;
	}

	private GameItem CreateRecycler()
	{
		(PixelWindow, GameInventory, GameInventory) tuple = DirectoryUtils.CreateDoubleInventoryWindow(10, 10);
		PixelWindow item2 = tuple.Item1;
		GameInventory itemInventoryLeft = tuple.Item2;
		GameInventory itemInventoryRight = tuple.Item3;
		GameItem gameItem = Empty().SetContentWindow(item2).SetSpriteAndShape("Items/items_machine", "recycler");
		itemInventoryRight.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
		gameItem.name = "Recycler";
		gameItem.EnableTag("manufacture");
		gameItem.EnableManufacturer("davre");
		List<CraftingRecipe> allRecipes = new List<CraftingRecipe>();
		CraftingRecipe item3 = new CraftingRecipe("Recycle").AddMaterial(new CraftingIdentifierFilter("Kitchen Cleaver", 1, "kitchen_cleaver")).SetCraftingConstantFunc("metal_ingot");
		allRecipes.Add(item3);
		gameItem.onCycleEndSlotItemFunc = delegate
		{
			itemInventoryRight.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => true;
			foreach (CraftingRecipe item4 in allRecipes)
			{
				if (item4.CanCraft(itemInventoryLeft))
				{
					item4.TryCraftInside(itemInventoryLeft, itemInventoryRight);
					break;
				}
			}
			itemInventoryRight.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
		};
		MachineryHelper.InitMachinery(gameItem, 5);
		return gameItem;
	}

	private GameItem AutomaticMeatCultureLab()
	{
		var (contentWindow, itemInventoryLeft, itemInventoryRight) = DirectoryUtils.CreateDoubleInventoryWindowSlotInput(8, 8);
		itemInventoryLeft.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => (item.identifier.Equals("small_raw_meat") || item.identifier.Equals("raw_meat")) ? true : false;
		((GameSlotInventory)itemInventoryLeft).SetBackgroundFadeSprite("Items/items_food", "raw_meat");
		itemInventoryRight.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
		GameItem box = Empty().SetContentWindow(contentWindow).SetSpriteAndShape("Items/items_machine", "meat_culture_machine");
		box.name = "Automatic Meat Culture Lab";
		box.shortDescription = "Fully automatic meat culture lab that produce fresh lab grown meat from a sample";
		box.EnableTag("manufacture");
		box.onCycleEndSlotItemFunc = delegate
		{
			if (!MachineryHelper.CanMachineOutput(box))
			{
				MachineryHelper.ConsumePoweredTag(box);
			}
			else
			{
				MachineryHelper.ConsumePoweredTag(box);
				itemInventoryRight.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => true;
				List<GameItem> source = GraphUtils.FindAllChildrenType(itemInventoryLeft, (GameItem item) => item.identifier == "small_raw_meat");
				List<GameItem> source2 = GraphUtils.FindAllChildrenType(itemInventoryLeft, (GameItem item) => item.identifier == "raw_meat");
				if (source.Any())
				{
					GraphUtils.TryAcceptAll(itemInventoryRight, DirectoryMaster.Item("small_raw_meat"));
				}
				else if (source2.Any())
				{
					GraphUtils.TryAcceptAll(itemInventoryRight, DirectoryMaster.Item("raw_meat"));
				}
				itemInventoryRight.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
			}
		};
		MachineryHelper.InitMachinery(box, 4);
		return box;
	}

	private GameItem CreateArmorConditioner()
	{
		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateSlotInventoryWindow(8, 8);
		PixelWindow item2 = tuple.Item1;
		GameInventory itemInventory = tuple.Item2;
		GameItem box = Empty().SetContentWindow(item2).SetSpriteAndShape("Items/items_machine", "armor_conditionner");
		box.name = "Automatic Armor Conditioner";
		box.shortDescription = "A fully automate but slow armor condionner";
		((GameSlotInventory)itemInventory).SetBackgroundFadeSprite("Items/items_armor", "armor_lining");
		box.onCycleEndSlotItemFunc = delegate
		{
			if (!MachineryHelper.CanMachineOutput(box))
			{
				MachineryHelper.ConsumePoweredTag(box);
			}
			else
			{
				MachineryHelper.ConsumePoweredTag(box);
				if (itemInventory.childItems.Any() && itemInventory.childItems[0].IsTag("armor_item"))
				{
					ArmorHelper.RepairArmorWithLoss(itemInventory.childItems[0]);
				}
			}
		};
		itemInventory.mayInventoryAddItemFunc = (GameItem item, GameInventory _) => item.IsTag("armor_item") ? true : false;
		MachineryHelper.InitMachinery(box, 4);
		return box;
	}

	public static GameItem Hangar()
	{
		return null;
	}

	public GameItem CreateSpeaker()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_machine", "speaker");
		gameItem.name = "Speaker";
		MachineryHelper.InitMachinery(gameItem, 2);
		return gameItem;
	}

	public GameItem CreateBatteryHub()
	{
		PixelWindow item = DirectoryUtils.CreateInventoryWindow(4, 2).Item1;
		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_machine", "battery_hub");
		gameItem.name = "Battery Hub";
		gameItem.EnableTag("SYSTEM_TAG_POWER");
		PowerHelper.InitPowerSourceFourniture(gameItem);
		return gameItem;
	}

	public GameItem CreatePowerAnalyser()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_machine", "power_analyser");
		gameItem.name = "Power Analyser";
		gameItem.EnableManufacturer("eminence");
		gameItem.EnableTag("power_analyser");
		return gameItem;
	}
}
