using System;
using System.Collections.Generic;

public class MiscItemDirectory : ItemDirectory
{
	public const string TUTORIAL_PAPER_TAG = "TUTORIAL_PAPER_TAG";

	public const string GUIDE_PAPER_TAG = "GUIDE_PAPER_TAG";

	public const string PRISTINE = " (Pristine)";

	public const string DAMAGED = " (Damaged)";

	public const string CORRUPTED = " (Corrupted)";

	protected override void InitDirectory()
	{
		Add("energy_credit", () => EnergyCredit());
		Add("energy_credit_breeder", () => EnergyCreditBreeder());
		Add("energy_credit_ext", () => EnergyCreditExtended());
		Add("energy_credit_dis", () => DisposableBattery());
		Add("current_converter", () => CurrentConverter());
		Add("energy_credit_hyp", () => EnergyCreditHypertech());
		Add("glass_shard", () => GlassShard());
		Add("intel_sec", () => IntelSec());
		Add("flashlight", () => Flashlight());
		Add("newspaper", () => Newspaper());
		Add("tutorial_book", () => TutorialBook());
		Add("tutorial_page", () => TutorialPage());
		Add("cigarette_guide", () => CigaretteGuide());
		Add("cigarette_color", () => CigaretteColor());
		Add("hand_guide", () => HandGuide());
		Add("fuel_guide", () => FuelGuide());
		Add("ribwich_guide", () => RibwichGuide());
		Add("stamp_guide", () => StampGuide());
		Add("injector_guide", () => InjectorGuide());
		Add("water_guide", () => WaterGuide());
		Add("lottery_info", () => LotteryInfo());
		Add("hydroponic_guide", () => HydroponicGuide());
		Add("chemistry_guide", () => ChemistryGuide());
		Add("deed", () => Deed());
		Add("ingot_book", () => IngotGuide());
		Add("wine_book", () => WineGuide());
		Add("aug_id_guide", () => AugIdGuide());
		Add("lou_old_orders", () => LouToyBoxHelper.CreateOldOrders());
		Add("republic_transmitter", () => LouOperativeHelper.CreateTransmitter());
		Add("logo_checker", () => LogoChecker());
		Add("mentor_contract", () => MentorContract());
		Add("business_permit", () => BusinessPermit());
		Add("procurement_slip", () => ProcurementSlip());
		Add("slip", () => Slip());
		Add("handnote", () => Handnote());
		Add("scav_token", () => ScavToken());
		Add("black_lamp", () => BlackLamp());
		Add("dossier", () => Dossier());
		Add("trashcan", () => Trashcan());
		Add("drain", () => Drain());
		Add("faucet", () => Faucet());
		Add("cmd_keycard", () => CommandKeycard());
		Add("sec_keycard", () => SecKeycard());
		Add("sci_keycard", () => SciKeycard());
		Add("med_keycard", () => MedicalKeycard());
		Add("eng_keycard", () => EngineerKeycard());
		Add("sup_keycard", () => SupplyKeycard());
		Add("ser_keycard", () => ServiceKeycard());
		Add("blank_keycard", () => BlankKeycard());
		Add("joe_card", () => JoeWildCard());
		Add("card_brewer", () => CardBrewer());
		Add("card_mining", () => CardMining());
		Add("card_pharma", () => CardPharma());
		Add("card_chem", () => CardChem());
		Add("card_rev", () => CardRev());
		Add("permit_gun_1", () => GunPermit1());
		Add("permit_gun_2", () => GunPermit2());
		Add("permit_gun_3", () => GunPermit3());
		Add("canister_mbs", () => CanisterMBS());
		Add("sec_slip", () => SecSlip());
		Add("wanted_paper", () => WantedPaper());
		Add("rev_codeword", () => RevCodeword());
		Add("food_stamp", () => FoodStamp());
		Add("debris_pile1", () => DebrisHelper.CreateRestPile());
		Add("debris_pile2", () => DebrisHelper.CreateToolPile());
		Add("debris_pile3", () => DebrisHelper.CreateServicePile());
		Add("hand", () => Hand());
		Add("cert1", () => AppraisalCertificate(0));
		Add("cert2", () => AppraisalCertificate(1));
		Add("cert3", () => AppraisalCertificate(2));
		Add("cert4", () => AppraisalCertificate(3));
		Add("postit", () => Note());
		Add("exchange_directive", () => ExchangeDirective());
		Add("cassette1", () => MusicCassette("track1", LocHelper.GetLocalizedItem("item_cassette_track_perseverance") + LocHelper.GetLocalizedItem("item_cassette_pristine"), "cassette5"));
		Add("cassette2", () => MusicCassette("track2", LocHelper.GetLocalizedItem("item_cassette_track_datasphere") + LocHelper.GetLocalizedItem("item_cassette_pristine"), "cassette8"));
		Add("cassette3", () => MusicCassette("track3", LocHelper.GetLocalizedItem("item_cassette_track_high_stakes") + LocHelper.GetLocalizedItem("item_cassette_pristine"), "cassette6"));
		Add("cassette4", () => MusicCassette("track4", LocHelper.GetLocalizedItem("item_cassette_track_stardust") + LocHelper.GetLocalizedItem("item_cassette_pristine"), "cassette9"));
		Add("cassette5", () => MusicCassette("track5", LocHelper.GetLocalizedItem("item_cassette_track_afterglow") + LocHelper.GetLocalizedItem("item_cassette_pristine"), "cassette10"));
		Add("cassette6", () => MusicCassette("track6", LocHelper.GetLocalizedItem("item_cassette_track_spaceflight") + LocHelper.GetLocalizedItem("item_cassette_pristine"), "cassette3"));
		Add("cassette7", () => MusicCassette("track7", LocHelper.GetLocalizedItem("item_cassette_track_dark_matter") + LocHelper.GetLocalizedItem("item_cassette_pristine"), "cassette4"));
		Add("cassette1l", () => MusicCassette("track1l", LocHelper.GetLocalizedItem("item_cassette_track_perseverance") + LocHelper.GetLocalizedItem("item_cassette_damaged"), "cassette5", "alison", 1));
		Add("cassette2l", () => MusicCassette("track2l", LocHelper.GetLocalizedItem("item_cassette_track_datasphere") + LocHelper.GetLocalizedItem("item_cassette_damaged"), "cassette8", "alison", 1));
		Add("cassette3l", () => MusicCassette("track3l", LocHelper.GetLocalizedItem("item_cassette_track_high_stakes") + LocHelper.GetLocalizedItem("item_cassette_damaged"), "cassette6", "alison", 1));
		Add("cassette4l", () => MusicCassette("track4l", LocHelper.GetLocalizedItem("item_cassette_track_stardust") + LocHelper.GetLocalizedItem("item_cassette_damaged"), "cassette9", "alison", 1));
		Add("cassette5l", () => MusicCassette("track5l", LocHelper.GetLocalizedItem("item_cassette_track_afterglow") + LocHelper.GetLocalizedItem("item_cassette_damaged"), "cassette10", "alison", 1));
		Add("cassette6l", () => MusicCassette("track6l", LocHelper.GetLocalizedItem("item_cassette_track_spaceflight") + LocHelper.GetLocalizedItem("item_cassette_damaged"), "cassette3", "alison", 1));
		Add("cassette7l", () => MusicCassette("track7l", LocHelper.GetLocalizedItem("item_cassette_track_dark_matter") + LocHelper.GetLocalizedItem("item_cassette_damaged"), "cassette4", "alison", 1));
		Add("cassette1d", () => MusicCassette("track1d", LocHelper.GetLocalizedItem("item_cassette_track_perseverance") + LocHelper.GetLocalizedItem("item_cassette_corrupted"), "cassette5", "alison", 2));
		Add("cassette2d", () => MusicCassette("track2d", LocHelper.GetLocalizedItem("item_cassette_track_datasphere") + LocHelper.GetLocalizedItem("item_cassette_corrupted"), "cassette8", "alison", 2));
		Add("cassette3d", () => MusicCassette("track3d", LocHelper.GetLocalizedItem("item_cassette_track_high_stakes") + LocHelper.GetLocalizedItem("item_cassette_corrupted"), "cassette6", "alison", 2));
		Add("cassette4d", () => MusicCassette("track4d", LocHelper.GetLocalizedItem("item_cassette_track_stardust") + LocHelper.GetLocalizedItem("item_cassette_corrupted"), "cassette9", "alison", 2));
		Add("cassette5d", () => MusicCassette("track5d", LocHelper.GetLocalizedItem("item_cassette_track_afterglow") + LocHelper.GetLocalizedItem("item_cassette_corrupted"), "cassette10", "alison", 2));
		Add("cassette6d", () => MusicCassette("track6d", LocHelper.GetLocalizedItem("item_cassette_track_spaceflight") + LocHelper.GetLocalizedItem("item_cassette_corrupted"), "cassette3", "alison", 2));
		Add("cassette7d", () => MusicCassette("track7d", LocHelper.GetLocalizedItem("item_cassette_track_dark_matter") + LocHelper.GetLocalizedItem("item_cassette_corrupted"), "cassette4", "alison", 2));
		Add("cassette8", () => MusicCassette("track8", LocHelper.GetLocalizedOrDefault("Item", "item_cassette_track_a_bao_a_qu", "A Bao A Qu") + LocHelper.GetLocalizedItem("item_cassette_pristine"), "cassette8b", "harvey_bee"));
		Add("cassette9", () => MusicCassette("track9", LocHelper.GetLocalizedOrDefault("Item", "item_cassette_track_the_wailing", "The Wailing") + LocHelper.GetLocalizedItem("item_cassette_pristine"), "cassette9b", "harvey_bee"));
		Add("cassette10", () => MusicCassette("track10", LocHelper.GetLocalizedOrDefault("Item", "item_cassette_track_long_star_river", "Long Star River") + LocHelper.GetLocalizedItem("item_cassette_pristine"), "cassette10b", "harvey_bee"));
		Add("cassette11", () => MusicCassette("track11", LocHelper.GetLocalizedOrDefault("Item", "item_cassette_track_deluge", "Deluge") + LocHelper.GetLocalizedItem("item_cassette_pristine"), "cassette11b", "harvey_bee"));
		Add("cassette12", () => MusicCassette("track12", LocHelper.GetLocalizedOrDefault("Item", "item_cassette_track_spellsinger", "Spellsinger Radio Edition") + LocHelper.GetLocalizedItem("item_cassette_pristine"), "cassette12b", "harvey_bee"));
		Add("cassette13", () => MusicCassette("track13", LocHelper.GetLocalizedOrDefault("Item", "item_cassette_track_golden_lotus", "Golden Lotus") + LocHelper.GetLocalizedItem("item_cassette_pristine"), "cassette13b", "harvey_bee"));
		Add("cassette14", () => MusicCassette("track14", LocHelper.GetLocalizedOrDefault("Item", "item_cassette_track_junkyard_jockey", "Junkyard Jockey") + LocHelper.GetLocalizedItem("item_cassette_pristine"), "cassette14b", "harvey_bee"));
		Add("cassette8l", () => MusicCassette("track8l", LocHelper.GetLocalizedOrDefault("Item", "item_cassette_track_a_bao_a_qu", "A Bao A Qu") + LocHelper.GetLocalizedItem("item_cassette_damaged"), "cassette8b", "harvey_bee", 1));
		Add("cassette9l", () => MusicCassette("track9l", LocHelper.GetLocalizedOrDefault("Item", "item_cassette_track_the_wailing", "The Wailing") + LocHelper.GetLocalizedItem("item_cassette_damaged"), "cassette9b", "harvey_bee", 1));
		Add("cassette10l", () => MusicCassette("track10l", LocHelper.GetLocalizedOrDefault("Item", "item_cassette_track_long_star_river", "Long Star River") + LocHelper.GetLocalizedItem("item_cassette_damaged"), "cassette10b", "harvey_bee", 1));
		Add("cassette11l", () => MusicCassette("track11l", LocHelper.GetLocalizedOrDefault("Item", "item_cassette_track_deluge", "Deluge") + LocHelper.GetLocalizedItem("item_cassette_damaged"), "cassette11b", "harvey_bee", 1));
		Add("cassette12l", () => MusicCassette("track12l", LocHelper.GetLocalizedOrDefault("Item", "item_cassette_track_spellsinger", "Spellsinger Radio Edition") + LocHelper.GetLocalizedItem("item_cassette_damaged"), "cassette12b", "harvey_bee", 1));
		Add("cassette13l", () => MusicCassette("track13l", LocHelper.GetLocalizedOrDefault("Item", "item_cassette_track_golden_lotus", "Golden Lotus") + LocHelper.GetLocalizedItem("item_cassette_damaged"), "cassette13b", "harvey_bee", 1));
		Add("cassette14l", () => MusicCassette("track14l", LocHelper.GetLocalizedOrDefault("Item", "item_cassette_track_junkyard_jockey", "Junkyard Jockey") + LocHelper.GetLocalizedItem("item_cassette_damaged"), "cassette14b", "harvey_bee", 1));
		Add("cassette8d", () => MusicCassette("track8d", LocHelper.GetLocalizedOrDefault("Item", "item_cassette_track_a_bao_a_qu", "A Bao A Qu") + LocHelper.GetLocalizedItem("item_cassette_corrupted"), "cassette8b", "harvey_bee", 2));
		Add("cassette9d", () => MusicCassette("track9d", LocHelper.GetLocalizedOrDefault("Item", "item_cassette_track_the_wailing", "The Wailing") + LocHelper.GetLocalizedItem("item_cassette_corrupted"), "cassette9b", "harvey_bee", 2));
		Add("cassette10d", () => MusicCassette("track10d", LocHelper.GetLocalizedOrDefault("Item", "item_cassette_track_long_star_river", "Long Star River") + LocHelper.GetLocalizedItem("item_cassette_corrupted"), "cassette10b", "harvey_bee", 2));
		Add("cassette11d", () => MusicCassette("track11d", LocHelper.GetLocalizedOrDefault("Item", "item_cassette_track_deluge", "Deluge") + LocHelper.GetLocalizedItem("item_cassette_corrupted"), "cassette11b", "harvey_bee", 2));
		Add("cassette12d", () => MusicCassette("track12d", LocHelper.GetLocalizedOrDefault("Item", "item_cassette_track_spellsinger", "Spellsinger Radio Edition") + LocHelper.GetLocalizedItem("item_cassette_corrupted"), "cassette12b", "harvey_bee", 2));
		Add("cassette13d", () => MusicCassette("track13d", LocHelper.GetLocalizedOrDefault("Item", "item_cassette_track_golden_lotus", "Golden Lotus") + LocHelper.GetLocalizedItem("item_cassette_corrupted"), "cassette13b", "harvey_bee", 2));
		Add("cassette14d", () => MusicCassette("track14d", LocHelper.GetLocalizedOrDefault("Item", "item_cassette_track_junkyard_jockey", "Junkyard Jockey") + LocHelper.GetLocalizedItem("item_cassette_corrupted"), "cassette14b", "harvey_bee", 2));
		Add("expedition_log", () => ExpeditionLog());
		Add("ribwich", () => Ribwich());
		Add("scratcher", () => ScratcherTicket());
		Add("gamblers_coin", () => GamblersCoin());
		Add("scratcher_stack", () => ScratcherTicketStack());
		Add("plasma_fuel", () => PlasmaFuel());
		Add("plastic_bag", () => PlasticBag());
		Add("aug_chip", () => AugChip());
		Add("aug_scanner", () => AugScanner());
	}

	private GameItem EnergyCredit()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "battery100");
		gameItem.name = LocHelper.GetLocalizedItem("item_energy_credit_name");
		gameItem.EnableManufacturer("eminence");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_energy_credit_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_energy_credit_desc");
		PowerHelper.InitPowerSourceItem(gameItem, 100, 10);
		gameItem.unitBaseValue = 10L;
		return gameItem;
	}

	private GameItem EnergyCreditExtended()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "battery_ext100");
		gameItem.name = LocHelper.GetLocalizedItem("item_energy_credit_ext_name");
		gameItem.EnableManufacturer("eminence");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_energy_credit_ext_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_energy_credit_ext_desc");
		PowerHelper.InitPowerSourceItem(gameItem, 250, 10);
		gameItem.unitBaseValue = 30L;
		return gameItem;
	}

	private GameItem EnergyCreditBreeder()
	{
		GameItem item = Empty().SetSpriteAndShape("Items/items_tool2", "sbattery100");
		item.name = LocHelper.GetLocalizedItem("item_energy_credit_breeder_name");
		item.shortDescription = LocHelper.GetLocalizedItem("item_energy_credit_breeder_desc");
		item.EnableManufacturer("eminence");
		PowerHelper.InitPowerSourceItem(item, 100, 4);
		PowerHelper.InitRecharge(item, 4);
		item.onCycleEndEarlySlotItemFunc = delegate(GameItem gameItem, GameInventory _, SlotMarker _)
		{
			int @int = item.GetTagReadonly("power_source_item_energy").GetInt();
			int int2 = item.GetTagReadonly("power_source_item_max_energy").GetInt();
			int newEnergy = @int + 4;
			if (int2 < newEnergy)
			{
				newEnergy = int2;
			}
			gameItem.ModifyTag("power_source_item_energy", delegate(TagState state)
			{
				state.SetInt(newEnergy);
			});
			PowerHelper.UpdateEnergyCreditSprite(gameItem);
		};
		item.unitBaseValue = 400L;
		return item;
	}

	private GameItem DisposableBattery()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "battery_dis100");
		gameItem.name = LocHelper.GetLocalizedOrDefault("Item", "item_energy_credit_dis_name", "Disposable Battery");
		gameItem.shortDescription = LocHelper.GetLocalizedOrDefault("Item", "item_energy_credit_dis_desc", "A high capacity single-use energy cell. Cannot be recharged.");
		gameItem.flavorText = LocHelper.GetLocalizedOrDefault("Item", "item_energy_credit_dis_flavor", "Favored in the Upper Levels for its convenience.");
		gameItem.EnableManufacturer("eminence");
		PowerHelper.InitPowerSourceItem(gameItem, 1000, 10);
		PowerHelper.InitDisposable(gameItem);
		PowerHelper.SetPowerSourceValuePercent(gameItem, 33);
		ContrabandHelper.InitContrabandItem(gameItem, 1);
		gameItem.unitBaseValue = 1L;
		return gameItem;
	}

	private GameItem CurrentConverter()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool2", "current_converter");
		gameItem.name = LocHelper.GetLocalizedItem("item_current_converter_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_current_converter_desc");
		gameItem.EnableManufacturer("eminence");
		gameItem.unitValue = 300L;
		gameItem.EnableTag("IMPORTANT_TAG");
		CurrentConverterHelper.InitCurrentConverter(gameItem);
		return gameItem;
	}

	private GameItem EnergyCreditHypertech()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool2", "battery_hyp");
		gameItem.name = LocHelper.GetLocalizedItem("item_hypertech_power_cell_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_hypertech_power_cell_desc");
		gameItem.EnableManufacturer("eminence");
		gameItem.unitValue = 2500L;
		HypertechBatteryHelper.InitHypertechBattery(gameItem);
		return gameItem;
	}

	private GameItem GlassShard()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_weapons_melee", "glass_shard");
		gameItem.name = LocHelper.GetLocalizedItem("item_glass_shard_name");
		gameItem.unitValue = 2L;
		gameItem.SetGameItemType("JUNK");
		gameItem.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem.onLoaded, (Action<GameItem>)delegate(GameItem loaded)
		{
			loaded.SetGameItemType("JUNK");
		});
		gameItem.shortDescription = LocHelper.GetLocalizedOrDefault("Item", "item_glass_shard_desc", "Use a bandage on it to wrap a Glass Shiv. The bandage is not used up.");
		gameItem.mayItemTargetThisFunc = (GameItem source, GameItem shard) => IsShivWrap(source);
		gameItem.canItemTargetThisFunc = (GameItem source, GameItem shard) => IsShivWrap(source);
		gameItem.onItemTargetThisFunc = delegate(GameItem source, GameItem shard)
		{
			if (IsShivWrap(source))
			{
				GameInventory parentInventory = shard.parentInventory;
				GraphUtils.TryDestroyAll(new List<GameItem> { shard }, 1);
				GraphUtils.EmporiumTryAdd(parentInventory, DirectoryMaster.Item("glass_shard_shiv"));
				AudioManager.Instance.Play3("textile_drop");
			}
		};
		return gameItem;
	}

	private static bool IsShivWrap(GameItem item)
	{
		if (item == null)
		{
			return false;
		}
		if (item.IsID("bandage_item") || item.IsID("hemostatic_bandage_item") || item.IsID("topical_bandage_item"))
		{
			return true;
		}
		if (item.IsID("tilapia_skin_graft"))
		{
			return true;
		}
		return false;
	}

	private GameItem IntelSec()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "intel_sec");
		gameItem.name = LocHelper.GetLocalizedItem("item_intel_sec_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_intel_sec_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_intel_sec_desc");
		gameItem.unitValue = 0L;
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.onActivateSlotItemFunc = delegate
		{
			RecordUIManager.Instance.OpenUI();
		};
		return gameItem;
	}

	private GameItem Flashlight()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "flashlight3");
		gameItem.SetGameItemType("TOOL");
		gameItem.name = LocHelper.GetLocalizedItem("item_flashlight_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_flashlight_desc");
		gameItem.unitValue = 40L;
		return gameItem;
	}

	private GameItem Newspaper()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "newspaper");
		gameItem.name = LocHelper.GetLocalizedItem("item_newspaper_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_newspaper_desc");
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.unitValue = 0L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.onActivateSlotItemFunc = delegate
		{
			NewsUIManager.Instance.ToggleUI();
		};
		gameItem.onCycleEndSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			curItem.Destroy();
		};
		return gameItem;
	}

	private GameItem TutorialBook()
	{
		(PixelWindow, GameGridInventory) tuple = DirectoryUtils.CreateBookWindow();
		PixelWindow item2 = tuple.Item1;
		GameGridInventory item3 = tuple.Item2;
		GameItem item = Empty().SetSpriteAndShape("Items/itematlas", "tutorial_book").SetContentWindow(item2);
		item.name = LocHelper.GetLocalizedItem("item_tutorial_book_name");
		item.shortDescription = LocHelper.GetLocalizedItem("item_tutorial_book_desc");
		item.EnableTag("DOCUMENT");
		item.SetGameItemType("DOCUMENT");
		item.EnableTag("IMPORTANT_TAG");
		item.EnableTag("CONTAINER_TAG");
		item.unitValue = 0L;
		item3.onSlotAddItemFunc = delegate
		{
			TutorialUIManager.Instance.OnDrag();
			item.SetSpriteAndShape("Items/itematlas", "tutorial_book_full");
		};
		item3.onSlotRemoveItemEndFunc = delegate(GameItem _, GameInventory inv, SlotMarker _)
		{
			if (inv.childItems.Count == 0)
			{
				item.SetSpriteAndShape("Items/itematlas", "tutorial_book");
			}
		};
		ContainerHelper.AllowOnlyTaggedItem(item3, "TUTORIAL_PAPER_TAG");
		return item;
	}

	public GameItem TutorialPage(string title = "Title", string body = "Body", string signature = "")
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "tutorial_page");
		gameItem.name = "Mentor's Note";
		gameItem.shortDescription = "";
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.name = title;
		gameItem.EnableTag("TUTORIAL_PAPER_TAG");
		gameItem.unitValue = 0L;
		BookHelper.InitPaperGuide(gameItem);
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.onActivateSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			string @string = curItem.GetTagReadonly("TUTORIAL_PAGE_TITLE").GetString();
			string string2 = curItem.GetTagReadonly("TUTORIAL_PAGE_BODY").GetString();
			string string3 = curItem.GetTagReadonly("TUTORIAL_PAGE_SIGNATURE").GetString();
			TutorialUIManager.Instance.OnDoubleClick();
			TutorialUIManager.Instance.OnPageRead();
			PaperUIManager.Instance.OpenUI(@string, string2, string3);
		};
		return gameItem;
	}

	public GameItem LotteryInfo()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "guide0");
		gameItem.name = LocHelper.GetLocalizedItem("item_lottery_info_name");
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("GUIDE_PAPER_TAG");
		BookHelper.InitPaperGuide(gameItem);
		gameItem.unitValue = 1L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.onActivateSlotItemFunc = delegate
		{
			PaperUIManager.Instance.OpenLotteryInfo();
		};
		AudioHelper.InitPaperSheet(gameItem);
		return gameItem;
	}

	public GameItem CigaretteGuide()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "guide1");
		gameItem.name = LocHelper.GetLocalizedItem("item_cigarette_guide_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_cigarette_guide_desc");
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("GUIDE_PAPER_TAG");
		BookHelper.InitPaperGuide(gameItem);
		gameItem.unitValue = 50L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.onActivateSlotItemFunc = delegate
		{
			PaperUIManager.Instance.OpenCigaretteGuide();
		};
		return gameItem;
	}

	public GameItem HandGuide()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "manual5");
		gameItem.name = LocHelper.GetLocalizedUI("book_hand_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_hand_guide_desc");
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("GUIDE_PAPER_TAG");
		BookHelper.InitPaperGuide(gameItem);
		gameItem.unitValue = 50L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.onActivateSlotItemFunc = delegate
		{
			MultiPageUIManager.Instance.OpenUI(6);
		};
		return gameItem;
	}

	public GameItem FuelGuide()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "manual1");
		gameItem.name = LocHelper.GetLocalizedItem("item_fuel_guide_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_fuel_guide_desc");
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("GUIDE_PAPER_TAG");
		BookHelper.InitPaperGuide(gameItem);
		gameItem.unitValue = 50L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.onActivateSlotItemFunc = delegate
		{
			MultiPageUIManager.Instance.OpenUI(0);
		};
		return gameItem;
	}

	public GameItem RibwichGuide()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "manual4");
		gameItem.name = LocHelper.GetLocalizedItem("item_ribwich_guide_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_ribwich_guide_desc");
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("GUIDE_PAPER_TAG");
		BookHelper.InitPaperGuide(gameItem);
		gameItem.unitValue = 50L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.onActivateSlotItemFunc = delegate
		{
			MultiPageUIManager.Instance.OpenUI(4);
		};
		return gameItem;
	}

	public GameItem WaterGuide()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "book1");
		gameItem.name = LocHelper.GetLocalizedItem("item_water_guide_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_water_guide_desc");
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("GUIDE_PAPER_TAG");
		BookHelper.InitGuide(gameItem);
		gameItem.unitValue = 25L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.onActivateSlotItemFunc = delegate
		{
			MultiPageUIManager.Instance.OpenUI(1);
		};
		return gameItem;
	}

	public GameItem HydroponicGuide()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "book3");
		gameItem.name = LocHelper.GetLocalizedItem("item_hydroponic_guide_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_hydroponic_guide_desc");
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("GUIDE_PAPER_TAG");
		BookHelper.InitGuide(gameItem);
		gameItem.unitValue = 25L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.onActivateSlotItemFunc = delegate
		{
			MultiPageUIManager.Instance.OpenUI(2);
		};
		return gameItem;
	}

	public GameItem ChemistryGuide()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "book2");
		gameItem.name = LocHelper.GetLocalizedItem("item_chemistry_guide_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_chemistry_guide_desc");
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("GUIDE_PAPER_TAG");
		BookHelper.InitGuide(gameItem);
		gameItem.unitValue = 50L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.onActivateSlotItemFunc = delegate
		{
			MultiPageUIManager.Instance.OpenUI(3);
		};
		return gameItem;
	}

	public GameItem AugIdGuide()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "aug_id_guide");
		gameItem.name = LocHelper.GetLocalizedUI("ui_aug_guide_name");
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("GUIDE_PAPER_TAG");
		gameItem.unitValue = 50L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.onActivateSlotItemFunc = delegate
		{
			PaperUIManager.Instance.OpenAugIdGuide();
		};
		return gameItem;
	}

	public GameItem InjectorGuide()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "guide5");
		gameItem.name = LocHelper.GetLocalizedItem("item_injector_guide_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_injector_guide_desc");
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("GUIDE_PAPER_TAG");
		BookHelper.InitPaperGuide(gameItem);
		gameItem.unitValue = 50L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.onActivateSlotItemFunc = delegate
		{
			PaperUIManager.Instance.OpenInjectorGuide();
		};
		return gameItem;
	}

	public GameItem StampGuide()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "guide3");
		gameItem.name = LocHelper.GetLocalizedItem("item_stamp_guide_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_stamp_guide_desc");
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("GUIDE_PAPER_TAG");
		BookHelper.InitPaperGuide(gameItem);
		gameItem.unitValue = 50L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.onActivateSlotItemFunc = delegate
		{
			PaperUIManager.Instance.OpenStampGuide();
		};
		return gameItem;
	}

	public GameItem LogoChecker()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "republic_logo_checker");
		gameItem.name = LocHelper.GetLocalizedItem("item_logo_checker_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_logo_checker_desc");
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("GUIDE_PAPER_TAG");
		gameItem.unitValue = 50L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.onActivateSlotItemFunc = delegate
		{
			RepublicLogoUIManager.Instance.OpenPanel();
		};
		return gameItem;
	}

	public GameItem CigaretteColor()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "cigarette_color_guide");
		gameItem.name = LocHelper.GetLocalizedItem("item_cigarette_color_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_cigarette_color_desc");
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.unitValue = 50L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.onActivateSlotItemFunc = delegate
		{
			PaperUIManager.Instance.OpenCigaretteColor();
		};
		return gameItem;
	}

	public GameItem MentorContract(string title = "Title", string body = "Body", string signature = "")
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "deed");
		gameItem.name = LocHelper.GetLocalizedItem("item_mentor_contract_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_mentor_contract_desc");
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		BookHelper.InitPaperGuide(gameItem);
		gameItem.unitValue = 0L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.onActivateSlotItemFunc = delegate
		{
			PaperUIManager.Instance.OpenContract();
		};
		return gameItem;
	}

	public GameItem Deed(string title = "Title", string body = "Body", string signature = "")
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "deed");
		gameItem.name = LocHelper.GetLocalizedItem("item_deed_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_deed_desc");
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		BookHelper.InitPaperGuide(gameItem);
		gameItem.unitValue = 0L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.onActivateSlotItemFunc = delegate
		{
			PaperUIManager.Instance.OpenDeed();
		};
		return gameItem;
	}

	public GameItem BusinessPermit()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "deed");
		gameItem.name = LocHelper.GetLocalizedItem("item_business_permit_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_business_permit_desc");
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		BookHelper.InitPaperGuide(gameItem);
		gameItem.unitValue = 0L;
		return gameItem;
	}

	public GameItem ProcurementSlip()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "slip");
		gameItem.name = LocHelper.GetLocalizedItem("item_procurement_slip_name");
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.unitValue = 0L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.onActivateSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			PaperUIManager.Instance.OpenSlipUI(curItem);
		};
		gameItem.onCycleEndSlotItemFunc = delegate(GameItem curItem, GameInventory _, SlotMarker _)
		{
			int dueDate = curItem.GetTagReadonly("PROCUREMENT_DAY").GetInt() - 1;
			if (dueDate < 0)
			{
				curItem.Destroy();
			}
			else
			{
				curItem.ModifyTag("PROCUREMENT_DAY", delegate(TagState state)
				{
					state.Enable().SetInt(dueDate);
				});
			}
		};
		return gameItem;
	}

	public GameItem Slip()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "slip");
		gameItem.name = LocHelper.GetLocalizedItem("item_slip_name");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.unitValue = 0L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.onActivateSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			PaperUIManager.Instance.OpenGeneralSlipUI(curItem);
		};
		return gameItem;
	}

	public GameItem Handnote()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "handnote");
		gameItem.name = LocHelper.GetLocalizedItem("item_handnote_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_handnote_desc");
		gameItem.ModifyTag("HANDNOTE_TITLE", delegate(TagState state)
		{
			state.Enable().SetString("");
		});
		gameItem.ModifyTag("HANDNOTE_BODY", delegate(TagState state)
		{
			state.Enable().SetString("");
		});
		gameItem.ModifyTag("HANDNOTE_SIGN", delegate(TagState state)
		{
			state.Enable().SetString("");
		});
		gameItem.ModifyTag("HANDNOTE_FONT", delegate(TagState state)
		{
			state.Enable().SetInt(1);
		});
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.unitValue = 0L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.onActivateSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			HandnoteUIManager.Instance.OpenPanel(curItem);
		};
		return gameItem;
	}

	public GameItem JoeWildCard()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "card_wild");
		gameItem.name = LocHelper.GetLocalizedItem("item_joe_card_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_joe_card_flavor");
		gameItem.unitValue = 0L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		AudioHelper.InitCartonCard(gameItem);
		gameItem.onActivateSlotItemFunc = delegate
		{
			PaperUIManager.Instance.ToggleJoeCard();
		};
		return gameItem;
	}

	public GameItem CardBrewer()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "card_brewer");
		gameItem.name = LocHelper.GetLocalizedItem("item_card_brewer_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_card_brewer_flavor");
		gameItem.unitValue = 0L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		AudioHelper.InitCartonCard(gameItem);
		gameItem.onActivateSlotItemFunc = delegate
		{
			PaperUIManager.Instance.OpenBrewerCard();
		};
		return gameItem;
	}

	public GameItem CardMining()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "card_mining");
		gameItem.name = LocHelper.GetLocalizedItem("item_card_mining_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_card_mining_flavor");
		gameItem.unitValue = 0L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.SetGameItemType("DOCUMENT");
		AudioHelper.InitCartonCard(gameItem);
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.onActivateSlotItemFunc = delegate
		{
			PaperUIManager.Instance.OpenMiningCard();
		};
		return gameItem;
	}

	public GameItem CardPharma()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "card_pharma");
		gameItem.name = LocHelper.GetLocalizedItem("item_card_pharma_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_card_pharma_flavor");
		gameItem.unitValue = 0L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.SetGameItemType("DOCUMENT");
		AudioHelper.InitCartonCard(gameItem);
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.onActivateSlotItemFunc = delegate
		{
			PaperUIManager.Instance.OpenPharmaCard();
		};
		return gameItem;
	}

	public GameItem CardChem()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "card_chem");
		gameItem.name = LocHelper.GetLocalizedItem("item_card_chem_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_card_chem_flavor");
		gameItem.unitValue = 0L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.SetGameItemType("DOCUMENT");
		AudioHelper.InitCartonCard(gameItem);
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.onActivateSlotItemFunc = delegate
		{
			PaperUIManager.Instance.OpenChemCard();
		};
		return gameItem;
	}

	public GameItem CardRev()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "card_rev");
		gameItem.name = LocHelper.GetLocalizedItem("item_card_rev_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_card_rev_flavor");
		gameItem.unitValue = 0L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.SetGameItemType("DOCUMENT");
		AudioHelper.InitCartonCard(gameItem);
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.onActivateSlotItemFunc = delegate
		{
			PaperUIManager.Instance.OpenCard("card_rev");
		};
		return gameItem;
	}

	public GameItem GunPermit1()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "permit_gun_1");
		gameItem.name = LocHelper.GetLocalizedItem("item_permit_gun_1_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_permit_gun_1_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_permit_gun_1_flavor");
		gameItem.unitValue = 0L;
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		PermitHelper.InitPermitItem(gameItem, 1, new List<string> { "FIREARM", "AMMUNITION" });
		return gameItem;
	}

	public GameItem GunPermit2()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "permit_gun_2");
		gameItem.name = LocHelper.GetLocalizedItem("item_permit_gun_2_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_permit_gun_2_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_permit_gun_2_flavor");
		gameItem.unitValue = 0L;
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		PermitHelper.InitPermitItem(gameItem, 2, new List<string> { "FIREARM", "AMMUNITION" });
		return gameItem;
	}

	public GameItem GunPermit3()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "permit_gun_3");
		gameItem.name = LocHelper.GetLocalizedItem("item_permit_gun_3_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_permit_gun_3_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_permit_gun_3_flavor");
		gameItem.unitValue = 0L;
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		PermitHelper.InitPermitItem(gameItem, 3, new List<string> { "FIREARM", "AMMUNITION" });
		return gameItem;
	}

	public GameItem SecSlip()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "sec_slip");
		gameItem.name = LocHelper.GetLocalizedItem("item_sec_slip_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_sec_slip_desc");
		gameItem.unitValue = 0L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.onActivateSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			PaperUIManager.Instance.OpenCard("sec_slip", curItem);
		};
		AudioHelper.InitPaperSheet(gameItem);
		return gameItem;
	}

	public GameItem WantedPaper()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "paper_wanted");
		gameItem.name = LocHelper.GetLocalizedItem("item_wanted_paper_name");
		gameItem.unitValue = 0L;
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.onActivateSlotItemFunc = delegate
		{
			PaperUIManager.Instance.OpenWantedPaper();
		};
		AudioHelper.InitPaperSheet(gameItem);
		return gameItem;
	}

	public GameItem RevCodeword()
	{
		return IntelHelper.CreateRevCodeword();
	}

	public GameItem Dossier()
	{
		(PixelWindow, GameGridInventory) tuple = DirectoryUtils.CreateGridWindow(10, 10);
		PixelWindow item2 = tuple.Item1;
		GameGridInventory gridInventory = tuple.Item2;
		GameItem item = Empty().SetSpriteAndShape("Items/dossier", "dossier_0").SetContentWindow(item2);
		item.name = LocHelper.GetLocalizedItem("item_dossier_name");
		item.shortDescription = LocHelper.GetLocalizedItem("item_dossier_desc");
		ContainerHelper.InitContainerItemTagOnly(item);
		item.EnableTag("IMPORTANT_TAG");
		item.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		gridInventory.onSlotAddItemFunc = delegate
		{
			TutorialUIManager.Instance.OnDossierInserted();
		};
		gridInventory.onSlotAddItemEndFunc = delegate
		{
			SpriteHelper.UpdateDossierSprite(gridInventory, item);
		};
		gridInventory.onSlotRemoveItemEndFunc = delegate
		{
			SpriteHelper.UpdateDossierSprite(gridInventory, item);
		};
		ContainerHelper.AllowOnlyTaggedItemsOr(gridInventory, new string[9] { "DOCUMENT", "LOTTERY_PAPER_TAG", "MAGNIFYING_GLASS", "LABELER", "LABELER_BLUE", "SHOWCASE_SPECIAL_ITEM_TAG", "MARKING_PEN_TAG", "SCAV_TOKEN_TAG", "GAMBLERS_COIN_TAG" }, ownedOnly: true);
		item.onUnloadChildItemFunc = delegate(GameItem _, GameItem child)
		{
			GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, child);
		};
		item.unitValue = 0L;
		AudioHelper.InitDossier(item);
		return item;
	}

	public GameItem Trashcan()
	{
		(PixelWindow, GameGridInventory) tuple = DirectoryUtils.CreateGridWindow(10, 10);
		PixelWindow item2 = tuple.Item1;
		GameGridInventory gridInventory = tuple.Item2;
		GameItem item = Empty().SetSpriteAndShape("Items/items_furniture", "trashcan0").SetContentWindow(item2);
		item.name = LocHelper.GetLocalizedItem("item_trashcan_name");
		item.shortDescription = LocHelper.GetLocalizedItem("item_trashcan_desc");
		ContainerHelper.InitContainerItemTagOnly(item);
		AudioHelper.InitTrashcan(item);
		item.EnableTag("IMPORTANT_TAG");
		item.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		EmporiumEntry.Instance.trashcanInvElement = gridInventory;
		gridInventory.onSlotAddItemEndFunc = delegate
		{
			SpriteHelper.UpdateTrashcanSprite(gridInventory, item);
		};
		gridInventory.onSlotRemoveItemEndFunc = delegate
		{
			SpriteHelper.UpdateTrashcanSprite(gridInventory, item);
		};
		ContainerHelper.AllowOnlyTaggedItem(gridInventory, "IS_OWNED_TAG");
		item.onUnloadChildItemFunc = delegate(GameItem _, GameItem child)
		{
			GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvinvElement, child);
		};
		item.unitValue = 0L;
		return item;
	}

	public GameItem Drain()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_furniture", "drain");
		gameItem.name = LocHelper.GetLocalizedItem("item_drain_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_drain_desc");
		gameItem.unitValue = 0L;
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		return gameItem;
	}

	public GameItem Faucet()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_furniture", "faucet");
		gameItem.name = LocHelper.GetLocalizedItem("item_faucet_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_faucet_desc");
		gameItem.unitValue = 0L;
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
		gameItem.EnableTag("IS_TOOLTIP_DISABLED_TAG");
		return gameItem;
	}

	public GameItem IngotGuide()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "book4");
		gameItem.name = LocHelper.GetLocalizedItem("item_ingot_book_name");
		BookHelper.InitBookItem(gameItem, "INGOT_BOOK");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		BookHelper.InitGuide(gameItem);
		gameItem.unitValue = 25L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.onActivateSlotItemFunc = delegate
		{
			MultiPageUIManager.Instance.OpenUI(5);
		};
		return gameItem;
	}

	public GameItem WineGuide()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "book5");
		gameItem.name = LocHelper.GetLocalizedItem("item_wine_book_name");
		gameItem.EnableTag("DOCUMENT");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("GUIDE_PAPER_TAG");
		BookHelper.InitGuide(gameItem);
		gameItem.unitValue = 25L;
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => HealthData.CanRead();
		gameItem.onActivateSlotItemFunc = delegate
		{
			MultiPageUIManager.Instance.OpenUI(7);
		};
		return gameItem;
	}

	public GameItem SecKeycard()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "sec_keycard");
		gameItem.name = LocHelper.GetLocalizedItem("item_sec_keycard_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_sec_keycard_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_sec_keycard_desc");
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		gameItem.SetGameItemType("ACCESS_CARD");
		AudioHelper.InitPlasticCard(gameItem);
		gameItem.unitValue = 100L;
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s) && GeneralHelper.IsItemOwned(t) && LockHelper.IsLockedContainer(t);
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s) && GeneralHelper.IsItemOwned(t) && LockHelper.IsLockedContainer(t);
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			LockHelper.TryOpenLock(s, t, "LOCK_ID_SEC");
		};
		return gameItem;
	}

	public GameItem SciKeycard()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "sci_card");
		gameItem.name = LocHelper.GetLocalizedItem("item_sci_keycard_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_sci_keycard_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_sci_keycard_desc");
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		gameItem.SetGameItemType("ACCESS_CARD");
		gameItem.unitValue = 100L;
		AudioHelper.InitPlasticCard(gameItem);
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s) && GeneralHelper.IsItemOwned(t) && LockHelper.IsLockedContainer(t);
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s) && GeneralHelper.IsItemOwned(t) && LockHelper.IsLockedContainer(t);
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			LockHelper.TryOpenLock(s, t, "LOCK_ID_SCI");
		};
		return gameItem;
	}

	public GameItem CommandKeycard()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "command_keycard");
		gameItem.name = LocHelper.GetLocalizedItem("item_cmd_keycard_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_cmd_keycard_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_cmd_keycard_desc");
		ContrabandHelper.InitContrabandItem(gameItem, 3);
		gameItem.SetGameItemType("ACCESS_CARD");
		AudioHelper.InitPlasticCard(gameItem);
		gameItem.unitValue = 125L;
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s) && GeneralHelper.IsItemOwned(t) && LockHelper.IsLockedContainer(t);
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s) && GeneralHelper.IsItemOwned(t) && LockHelper.IsLockedContainer(t);
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			LockHelper.TryOpenLock(s, t, "LOCK_ID_COMMAND");
		};
		return gameItem;
	}

	public GameItem MedicalKeycard()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "med_card");
		gameItem.name = LocHelper.GetLocalizedItem("item_med_keycard_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_med_keycard_flavor");
		ContrabandHelper.InitContrabandItem(gameItem, 1);
		gameItem.SetGameItemType("ACCESS_CARD");
		AudioHelper.InitPlasticCard(gameItem);
		gameItem.unitValue = 75L;
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s) && GeneralHelper.IsItemOwned(t) && LockHelper.IsLockedContainer(t);
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s) && GeneralHelper.IsItemOwned(t) && LockHelper.IsLockedContainer(t);
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			LockHelper.TryOpenLock(s, t, "LOCK_ID_MED");
		};
		return gameItem;
	}

	public GameItem EngineerKeycard()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "eng_card");
		gameItem.name = LocHelper.GetLocalizedItem("item_eng_keycard_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_eng_keycard_flavor");
		ContrabandHelper.InitContrabandItem(gameItem, 1);
		gameItem.SetGameItemType("ACCESS_CARD");
		AudioHelper.InitPlasticCard(gameItem);
		gameItem.unitValue = 75L;
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s) && GeneralHelper.IsItemOwned(t) && LockHelper.IsLockedContainer(t);
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s) && GeneralHelper.IsItemOwned(t) && LockHelper.IsLockedContainer(t);
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			LockHelper.TryOpenLock(s, t, "LOCK_ID_ENG");
		};
		return gameItem;
	}

	public GameItem ServiceKeycard()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "ser_card");
		gameItem.name = LocHelper.GetLocalizedItem("item_ser_keycard_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_ser_keycard_flavor");
		ContrabandHelper.InitContrabandItem(gameItem, 1);
		gameItem.SetGameItemType("ACCESS_CARD");
		AudioHelper.InitPlasticCard(gameItem);
		gameItem.unitValue = 40L;
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s) && GeneralHelper.IsItemOwned(t) && LockHelper.IsLockedContainer(t);
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s) && GeneralHelper.IsItemOwned(t) && LockHelper.IsLockedContainer(t);
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			LockHelper.TryOpenLock(s, t, "LOCK_ID_SER");
		};
		return gameItem;
	}

	public GameItem SupplyKeycard()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "sup_card");
		gameItem.name = LocHelper.GetLocalizedItem("item_sup_keycard_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_sup_keycard_flavor");
		ContrabandHelper.InitContrabandItem(gameItem, 1);
		gameItem.SetGameItemType("ACCESS_CARD");
		AudioHelper.InitPlasticCard(gameItem);
		gameItem.unitValue = 40L;
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s) && GeneralHelper.IsItemOwned(t) && LockHelper.IsLockedContainer(t);
		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => GeneralHelper.IsItemOwned(s) && GeneralHelper.IsItemOwned(t) && LockHelper.IsLockedContainer(t);
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			LockHelper.TryOpenLock(s, t, "LOCK_ID_SUP");
		};
		return gameItem;
	}

	public GameItem BlankKeycard()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "blank_card");
		gameItem.name = LocHelper.GetLocalizedItem("item_blank_keycard_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_blank_keycard_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_blank_keycard_desc");
		gameItem.SetGameItemType("ACCESS_CARD");
		AudioHelper.InitPlasticCard(gameItem);
		gameItem.unitValue = 15L;
		return gameItem;
	}

	public GameItem CanisterMBS()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sec", "canister_nerve");
		gameItem.name = LocHelper.GetLocalizedItem("item_canister_mbs_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_canister_mbs_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_canister_mbs_desc");
		ContrabandHelper.InitContrabandItem(gameItem, 3);
		gameItem.unitValue = 400L;
		gameItem.SetGameItemType("WEAPON");
		return gameItem;
	}

	public GameItem FoodStamp()
	{
		GameItem item = Empty().SetSpriteAndShape("Items/items_misc", "food_stamp");
		item.name = LocHelper.GetLocalizedItem("item_food_stamp_name");
		item.shortDescription = LocHelper.GetLocalizedItem("item_food_stamp_desc");
		item.flavorText = LocHelper.GetLocalizedItem("item_food_stamp_flavor");
		item.unitValue = 30L;
		InsStampHelper.Init(item);
		InsStampHelper.SetGenuine(item, 40);
		item.canActivateSlotItemFunc = (GameItem curItem, GameInventory _, SlotMarker _) => curItem.IsTag("IS_OWNED_TAG");
		item.mayActivateSlotItemFunc = (GameItem curItem, GameInventory _, SlotMarker _) => curItem.IsTag("IS_OWNED_TAG");
		item.onActivateSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			Debug.Log(curItem.FindItemFeatureByCategory("CATEGORY_GENUINE_STAMP") == null);
			if (curItem.FindItemFeatureByCategory("CATEGORY_GENUINE_STAMP").realCondition.identifier.Equals("genuineStamp"))
			{
				int valueInt = curItem.GetTagReadonly("STAMP_AMOUNT").valueInt;
				PlayerStore.Instance.commissaryData.redeemedCredit += valueInt;
				EmporiumAudioPlayer.Instance.PlayBeatVictory();
				curInv.Expel(item);
			}
			else if (!curItem.FindItemFeatureByCategory("CATEGORY_GENUINE_STAMP").IsExposed())
			{
				curItem.FindItemFeatureByCategory("CATEGORY_GENUINE_STAMP").ExposeFeature(lowkey: true);
				EmporiumAudioPlayer.Instance.audioExposeScam.Play();
				curItem.canActivateSlotItemFunc = (GameItem curItem, GameInventory _, SlotMarker _) => false;
				curItem.mayActivateSlotItemFunc = (GameItem curItem, GameInventory _, SlotMarker _) => false;
			}
		};
		return item;
	}

	public GameItem ScavToken()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "scav_medal");
		gameItem.name = LocHelper.GetLocalizedItem("item_scav_token_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_scav_token_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_scav_token_desc");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("SCAV_TOKEN_TAG");
		gameItem.unitValue = 100L;
		return gameItem;
	}

	public GameItem BlackLamp()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_sign", "black_lamp");
		gameItem.name = LocHelper.GetLocalizedItem("item_black_lamp_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_black_lamp_desc");
		gameItem.EnableTag("IMPORTANT_TAG");
		BlackLampHelper.InitBlackLamp(gameItem);
		gameItem.unitValue = 120L;
		return gameItem;
	}

	public GameItem AppraisalCertificate(int rank)
	{
		GameItem gameItem = Empty();
		AppraisalRankHelper.InitCertificate(gameItem, rank);
		return gameItem;
	}

	public static GameItem Note()
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetSpriteAndShape("Items/items_tool", "postit");
		gameItem.name = LocHelper.GetLocalizedItem("item_postit_name");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.unitValue = 0L;
		return gameItem;
	}

	public GameItem ExchangeDirective()
	{
		GameItem item = Empty().SetSpriteAndShape("Items/items_misc", "exchange_directive");
		item.name = LocHelper.GetLocalizedItem("item_exchange_directive_name");
		item.SetGameItemType("DOCUMENT");
		item.EnableTag("IMPORTANT_TAG");
		item.unitValue = 0L;
		item.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.onActivateSlotItemFunc = delegate
		{
			ExchangeUIManager.Instance.OpenPanel(item);
		};
		AudioHelper.InitPaperSheet(item);
		return item;
	}

	public static GameItem MusicCassette(string trackID, string name, string cassette_sprite = "cassette1", string composer = "alison", int state = 0)
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetSpriteAndShape("Items/items_amenities", cassette_sprite);
		gameItem.name = LocHelper.GetLocalizedItem("item_cassette_prefix") + name;
		CassetteHelper.InitCassetteItem(gameItem, trackID);
		gameItem.EnableManufacturer(composer);
		gameItem.unitValue = 35L;
		gameItem.SetGameItemType("ART");
		AudioHelper.InitCassette(gameItem);
		switch (state)
		{
		case 0:
			gameItem.unitValue = 150L;
			gameItem.SetGameItemType("LUXURY_ITEM");
			break;
		case 1:
			gameItem.unitValue = 70L;
			break;
		}
		return gameItem;
	}

	public GameItem ExpeditionLog()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "handnote");
		gameItem.name = LocHelper.GetLocalizedItem("item_expedition_log_name");
		gameItem.unitValue = 0L;
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.onActivateSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			LogUIManager.Instance.Toggle(curItem);
		};
		return gameItem;
	}

	private GameItem Ribwich()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_food", "ribwich");
		gameItem.name = LocHelper.GetLocalizedItem("item_ribwich_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_ribwich_desc");
		gameItem.unitValue = 150L;
		AudioHelper.InitEdibleAudio(gameItem);
		AppraisalHelper.InitAppraisalItem(gameItem, 150);
		return gameItem;
	}

	private GameItem GamblersCoin()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "the_coin");
		gameItem.name = LocHelper.GetLocalizedItem("item_gamblers_coin_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_gamblers_coin_flavor");
		gameItem.unitValue = 50L;
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.EnableTag("GAMBLERS_COIN_TAG");
		gameItem.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem.onLoaded, (Action<GameItem>)delegate(GameItem loaded)
		{
			loaded.EnableTag("IMPORTANT_TAG");
			loaded.EnableTag("GAMBLERS_COIN_TAG");
		});
		AudioHelper.InitTinyMetal(gameItem);
		return gameItem;
	}

	private GameItem ScratcherTicket()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "scratch");
		gameItem.name = LocHelper.GetLocalizedItem("item_scratcher_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_scratcher_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_scratcher_flavor");
		gameItem.unitValue = 20L;
		ScratcherHelper.InitScratcherItem(gameItem);
		gameItem.SetGameItemType("DOCUMENT");
		gameItem.canActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item);
		gameItem.mayActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item);
		gameItem.onActivateSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			if (!ScratcherHelper.TryRedeemPrize(curItem))
			{
				ScratcherUIManager.Instance.OpenUI(curItem);
			}
		};
		AudioHelper.InitPaperSheet(gameItem);
		return gameItem;
	}

	private GameItem ScratcherTicketStack()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_misc", "scratch_stack");
		gameItem.name = LocHelper.GetLocalizedItem("item_scratcher_stack_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_scratcher_stack_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_scratcher_stack_flavor");
		gameItem.unitValue = 100L;
		gameItem.SetGameItemType("DOCUMENT");
		LotteryHelper.StampSpawnLuck(gameItem);
		gameItem.canActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item);
		gameItem.mayActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item);
		gameItem.onActivateSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			PaperUIManager.Instance.audioSourcePickup.Play();
			GameInventory parentInventory = curItem.parentInventory;
			bool flag = LotteryHelper.HasSpawnLuck(curItem);
			int spawnLuck = LotteryHelper.GetSpawnLuck(curItem);
			curItem.Destroy();
			for (int i = 0; i < 5; i++)
			{
				GameItem gameItem2 = ItemSpawner.Spawn("scratcher");
				if (flag)
				{
					LotteryHelper.SetSpawnLuck(gameItem2, spawnLuck);
					ScratcherHelper.SetRandomSymbols(gameItem2);
				}
				GraphUtils.EmporiumTryAdd(parentInventory, gameItem2);
			}
		};
		return gameItem;
	}

	private GameItem PlasmaFuel()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_ship", "plasma_fuel_cell");
		gameItem.name = LocHelper.GetLocalizedItem("item_plasma_fuel_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_plasma_fuel_desc");
		gameItem.unitValue = 300L;
		AppraisalHelper.InitAppraisalItem(gameItem, 300);
		return gameItem;
	}

	private GameItem Hand()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_inspectable", "hand_steel");
		gameItem.name = LocHelper.GetLocalizedItem("item_hand");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_hand_flavor");
		gameItem.unitValue = 100L;
		AppHandHelper.InitAppHandItem(gameItem);
		return gameItem;
	}

	private GameItem PlasticBag()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_container", "bag");
		gameItem.name = "bag";
		gameItem.unitValue = 0L;
		return gameItem;
	}

	private GameItem AugChip()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_module", "aug_chip");
		gameItem.name = "Augmentation Chip";
		gameItem.unitValue = 0L;
		return gameItem;
	}

	private GameItem AugScanner()
	{
		(PixelWindow, GameSlotInventory) tuple = global::AugScanner.CreateWindow();
		PixelWindow item = tuple.Item1;
		GameSlotInventory batterySlot = tuple.Item2;
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "aug_scanner").SetContentWindow(item);
		gameItem.name = LocHelper.GetLocalizedItem("item_aug_scanner_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_aug_scanner_flavor");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_aug_scanner_desc");
		gameItem.unitValue = 200L;
		gameItem.EnableTag("AUG_SCANNER_TAG");
		gameItem.EnableTag("CONTAINER_TAG");
		MachineHelper.SetupBatterySlot(batterySlot, gameItem);
		gameItem.canActivateSlotItemFunc = delegate(GameItem curItem, GameInventory _, SlotMarker _)
		{
			if (!GeneralHelper.IsItemOwned(curItem))
			{
				return false;
			}
			if (PlayerStore.Instance.currentClientInstance == null)
			{
				return false;
			}
			if (PlayerStore.Instance.currentClientInstance.GetClientBlueprint().isAugScanned)
			{
				return false;
			}
			return batterySlot.childItem != null && PowerHelper.CanDrawPowerSource(batterySlot.childItem, 25);
		};
		gameItem.mayActivateSlotItemFunc = delegate(GameItem curItem, GameInventory _, SlotMarker _)
		{
			if (!GeneralHelper.IsItemOwned(curItem))
			{
				return false;
			}
			if (PlayerStore.Instance.currentClientInstance == null)
			{
				return false;
			}
			if (PlayerStore.Instance.currentClientInstance.GetClientBlueprint().isAugScanned)
			{
				return false;
			}
			return batterySlot.childItem != null && PowerHelper.CanDrawPowerSource(batterySlot.childItem, 25);
		};
		gameItem.onActivateSlotItemFunc = delegate
		{
			PowerHelper.DrawPowerSource(batterySlot.childItem, 25);
			StoreClient clientBlueprint = PlayerStore.Instance.currentClientInstance.GetClientBlueprint();
			clientBlueprint.isAugScanned = true;
			if (clientBlueprint.isAug)
			{
				EmporiumAudioPlayer.Instance.alarm2.Play();
			}
			else
			{
				AudioManager.Instance.Play("PING_SONAR");
			}
		};
		return gameItem;
	}
}
