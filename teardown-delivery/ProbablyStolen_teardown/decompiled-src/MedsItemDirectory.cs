public class MedsItemDirectory : ItemDirectory
{
	private enum InjectorColor
	{
		White,
		Black,
		Red,
		Green,
		Turquoise,
		Purple,
		Blue,
		Blood_Red,
		Large_Purple,
		Pure_White,
		Pink,
		PinkAlt
	}

	public const int NOMORE_PROTOTYPE_MIN_PURITY = 45;

	public const int NOMORE_PROTOTYPE_MAX_PURITY = 60;

	protected override void InitDirectory()
	{
		Add("black_injector", () => SynaprestInjector());
		Add("pink_injector", () => Injector(InjectorColor.Pink, LocHelper.GetLocalizedItem("item_pink_injector_name"), "pink_injector"));
		Add("pink_alt_injector", () => Injector(InjectorColor.PinkAlt, LocHelper.GetLocalizedItem("item_pink_alt_injector_name"), "pink_injector_alt"));
		Add("large_purple_injector", () => ImmunivaxInjector());
		Add("pure_white_injector", () => Injector(InjectorColor.Pure_White, LocHelper.GetLocalizedItem("item_pure_white_injector_name"), "experimental_injector_white"));
		Add("birth_control_pill", () => BirthControlPill());
		Add("fanta_pill", () => FentanylPill());
		Add("phagimycin_pill", () => PhagimycinPill());
		Add("unlicensed_phagimycin_pill", () => UnlicensedPhagimycinPill());
		Add("zyanide_pill", () => ZyanidePill());
		Add("smelling_salt", () => SmellingSalt());
		Add("bandage_item", () => Bandage());
		Add("hemostatic_bandage_item", () => HemostaticBandage());
		Add("topical_bandage_item", () => TopicalBandage());
		Add("blood_bag", () => BloodBag());
		Add("blue_blood_bag", () => BlueBloodBag());
		Add("dream_dust", () => DreamDust());
		Add("nightmare_dust", () => NightmareDust());
		Add("caffeine_pill", () => CaffeinePill());
		Add("salve", () => HealingSalve());
		Add("rubbing_alcohol", () => RubbingAlcohol());
		Add("zerochew", () => ZeroChew());
		Add("chem_scanner", () => ChemScanner());
		Add("surgery_tool", () => SurgeryToolset());
	}

	private GameItem BirthControlPill()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "items_medical_15");
		gameItem.name = LocHelper.GetLocalizedItem("item_birth_control_pill_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_birth_control_pill_flavor");
		MedicalHelper.InitPillItem(gameItem);
		gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("null"));
		gameItem.unitValue = 15L;
		gameItem.SetGameItemType("MEDICAL");
		ChemicalHelper.InitChemItem(gameItem, isBlendable: true, isSoluble: true);
		ChemFinisherRecipe.SetCompositionStreetStandard(gameItem, "birth_control_pill");
		ChemicalProductHelper.InitChemicalProduct(gameItem, "contraceptive");
		ChemicalFeedbackHelper.InitChemicalIntentItem(gameItem, ChemicalFeedbackHelper.ChemicalIntent.BIRTH_CONTROL);
		return gameItem;
	}

	private GameItem PhagimycinPill()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "pill1");
		gameItem.name = LocHelper.GetLocalizedItem("item_phagimycin_pill_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_phagimycin_pill_flavor");
		MedicalHelper.InitPillItem(gameItem);
		gameItem.EnableManufacturer("carmicheal");
		gameItem.unitValue = 25L;
		return gameItem;
	}

	private GameItem UnlicensedPhagimycinPill()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "pill2");
		gameItem.name = LocHelper.GetLocalizedItem("item_unlicensed_phagimycin_pill_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_unlicensed_phagimycin_pill_flavor");
		MedicalHelper.InitPillItem(gameItem);
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		gameItem.EnableManufacturer("spacer");
		gameItem.unitValue = 5L;
		gameItem.SetGameItemType("MEDICAL");
		return gameItem;
	}

	private GameItem ZyanidePill()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "items_medical_12");
		gameItem.name = LocHelper.GetLocalizedItem("item_zyanide_pill_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_zyanide_pill_desc");
		MedicalHelper.InitPillItem(gameItem, enableMedical: false);
		gameItem.unitValue = 33L;
		gameItem.SetGameItemType("POISON");
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		ChemicalHelper.InitChemItem(gameItem, isBlendable: true, isSoluble: true);
		ChemFinisherRecipe.SetCompositionStreetStandard(gameItem, "zyanide_pill");
		ChemicalProductHelper.InitChemicalProduct(gameItem, "zyanide");
		ChemicalHelper.SetChemContentML(gameItem, "zyanide", 1.0);
		ChemicalFeedbackHelper.InitChemicalIntentItem(gameItem, ChemicalFeedbackHelper.ChemicalIntent.POISON);
		return gameItem;
	}

	private GameItem FentanylPill()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "oxycodone_pill");
		gameItem.name = LocHelper.GetLocalizedItem("item_fanta_pill_name");
		MedicalHelper.InitPillItem(gameItem);
		gameItem.EnableManufacturer("spacer");
		gameItem.unitValue = 12L;
		ContrabandHelper.InitContrabandItem(gameItem, 3);
		gameItem.SetGameItemType("MEDICAL");
		gameItem.SetGameItemType("NARCOTIC");
		gameItem.SetGameItemType("SUBSTANCE");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_fanta_pill_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_fanta_pill_flavor");
		ConsumableHelper.InitNarcotic(gameItem, 3);
		ChemicalHelper.InitChemItem(gameItem, isBlendable: true, isSoluble: true);
		ChemFinisherRecipe.SetCompositionStreetStandard(gameItem, "fanta_pill");
		ChemicalProductHelper.InitChemicalProduct(gameItem, "fanta");
		ChemicalFeedbackHelper.InitChemicalIntentItem(gameItem, ChemicalFeedbackHelper.ChemicalIntent.NARCOTIC);
		return gameItem;
	}

	private GameItem ZeroChew()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "zerochew");
		gameItem.name = LocHelper.GetLocalizedItem("item_zerochew_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_zerochew_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_zerochew_flavor");
		MedicalHelper.InitPillItem(gameItem);
		gameItem.EnableManufacturer("carmicheal");
		AudioHelper.InitTinyItem(gameItem);
		gameItem.unitValue = 8L;
		gameItem.SetGameItemType("FOOD");
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		gameItem.RemoveGameItemType("MEDICAL");
		return gameItem;
	}

	private GameItem SmellingSalt()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "smelling_salt");
		gameItem.name = LocHelper.GetLocalizedItem("item_smelling_salt_name");
		AudioHelper.InitTinyItem(gameItem);
		gameItem.EnableManufacturer("carmicheal");
		AudioHelper.InitPillAudio(gameItem);
		gameItem.unitValue = 15L;
		gameItem.EnableTag("ITEM_MEDICAL_TAG");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_smelling_salt_desc");
		gameItem.SetGameItemType("MEDICAL");
		return gameItem;
	}

	private GameItem BloodBag()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "blood_bag");
		gameItem.name = LocHelper.GetLocalizedItem("item_blood_bag_name");
		gameItem.SetGameItemType("MEDICAL");
		AudioHelper.InitEdibleAudio(gameItem);
		gameItem.unitValue = 33L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_blood_bag_desc");
		gameItem.EnableProficiency("MEDICAL_PROFICIENCY");
		gameItem.EnableTag("ITEM_MEDICAL_TAG");
		return gameItem;
	}

	private GameItem BlueBloodBag()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "blue_blood_bag");
		gameItem.name = LocHelper.GetLocalizedItem("item_blue_blood_bag_name");
		gameItem.SetGameItemType("MEDICAL");
		gameItem.SetGameItemType("LUXURY_ITEM");
		AudioHelper.InitEdibleAudio(gameItem);
		gameItem.EnableManufacturer("ashvins");
		gameItem.unitValue = 250L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_blue_blood_bag_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_blue_blood_bag_flavor");
		gameItem.EnableProficiency("MEDICAL_PROFICIENCY");
		gameItem.EnableTag("ITEM_MEDICAL_TAG");
		return gameItem;
	}

	private GameItem DreamDust()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "dream_dust");
		gameItem.name = LocHelper.GetLocalizedItem("item_dream_dust_name");
		gameItem.SetGameItemType("MEDICAL");
		gameItem.SetGameItemType("NARCOTIC");
		gameItem.SetGameItemType("SUBSTANCE");
		ContrabandHelper.InitContrabandItem(gameItem, 2);
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_dream_dust_flavor");
		gameItem.unitValue = 18L;
		ConsumableHelper.InitNarcotic(gameItem, 1);
		ChemicalHelper.InitChemItem(gameItem, isBlendable: true, isSoluble: true);
		ChemFinisherRecipe.SetCompositionStreetStandard(gameItem, "dream_dust");
		ChemicalProductHelper.InitChemicalProduct(gameItem, "dream_cap_extract");
		ChemicalFeedbackHelper.InitChemicalIntentItem(gameItem, ChemicalFeedbackHelper.ChemicalIntent.NARCOTIC);
		AudioHelper.InitGlassVial(gameItem);
		return gameItem;
	}

	private GameItem NightmareDust()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "nightmare_dust");
		gameItem.name = LocHelper.GetLocalizedItem("item_nightmare_dust_name");
		gameItem.SetGameItemType("POISON");
		ContrabandHelper.InitContrabandItem(gameItem, 3);
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_nightmare_dust_flavor");
		gameItem.unitValue = 36L;
		AudioHelper.InitGlassVial(gameItem);
		ChemicalHelper.InitChemItem(gameItem, isBlendable: true, isSoluble: true);
		ChemFinisherRecipe.SetCompositionStreetStandard(gameItem, "nightmare_dust");
		ChemicalProductHelper.InitChemicalProduct(gameItem, "nightmare_extract");
		ChemicalFeedbackHelper.InitChemicalIntentItem(gameItem, ChemicalFeedbackHelper.ChemicalIntent.POISON);
		return gameItem;
	}

	private GameItem CaffeinePill()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "cafeine_pill");
		gameItem.name = LocHelper.GetLocalizedItem("item_caffeine_pill_name");
		gameItem.SetGameItemType("MEDICAL");
		gameItem.SetGameItemType("HOUSEHOLD_GOOD");
		gameItem.EnableManufacturer("spacer");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_caffeine_pill_flavor");
		gameItem.unitValue = 10L;
		ChemicalHelper.InitChemItem(gameItem, isBlendable: true, isSoluble: true);
		ChemFinisherRecipe.SetCompositionStreetStandard(gameItem, "caffeine_pill");
		ChemicalProductHelper.InitChemicalProduct(gameItem, "caffeine");
		ChemicalFeedbackHelper.InitChemicalIntentItem(gameItem, ChemicalFeedbackHelper.ChemicalIntent.MEDICAL);
		return gameItem;
	}

	private GameItem HealingSalve()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "salve");
		gameItem.name = LocHelper.GetLocalizedItem("item_salve_name");
		gameItem.SetGameItemType("MEDICAL");
		gameItem.EnableManufacturer("spacer");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_salve_flavor");
		gameItem.unitValue = 80L;
		return gameItem;
	}

	private GameItem RubbingAlcohol()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "black_bottle");
		gameItem.name = LocHelper.GetLocalizedItem("item_rubbing_alcohol_name");
		gameItem.SetGameItemType("MEDICAL");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_rubbing_alcohol_flavor");
		gameItem.unitValue = 35L;
		return gameItem;
	}

	private GameItem Bandage()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "bandage_small");
		gameItem.name = LocHelper.GetLocalizedItem("item_bandage_name");
		gameItem.canActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item) && !PlayerStore.Instance.healthData.isWoundStable;
		gameItem.mayActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item) && !PlayerStore.Instance.healthData.isWoundStable;
		gameItem.onActivateSlotItemFunc = delegate(GameItem item, GameInventory _, SlotMarker _)
		{
			PlayerStore.Instance.healthData.isWoundStable = true;
			item.Destroy();
			PlayerStore.Instance.healthData.OnHealingItemUsed();
		};
		gameItem.EnableProficiency("MEDICAL_PROFICIENCY");
		gameItem.EnableTag("ITEM_MEDICAL_TAG");
		AudioHelper.InitTextile(gameItem);
		gameItem.SetGameItemType("MEDICAL");
		gameItem.unitValue = 8L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_bandage_desc");
		return gameItem;
	}

	private GameItem HemostaticBandage()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "hemostatic_bandage_small");
		gameItem.name = LocHelper.GetLocalizedItem("item_hemostatic_bandage_name");
		gameItem.canActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item) && !PlayerStore.Instance.healthData.isWoundStable;
		gameItem.mayActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item) && !PlayerStore.Instance.healthData.isWoundStable;
		gameItem.onActivateSlotItemFunc = delegate(GameItem item, GameInventory _, SlotMarker _)
		{
			PlayerStore.Instance.healthData.isWoundStable = true;
			PlayerStore.Instance.healthData.woundState--;
			item.Destroy();
			PlayerStore.Instance.healthData.OnHealingItemUsed();
		};
		gameItem.EnableProficiency("MEDICAL_PROFICIENCY");
		gameItem.EnableTag("ITEM_MEDICAL_TAG");
		AudioHelper.InitTextile(gameItem);
		gameItem.SetGameItemType("MEDICAL");
		gameItem.unitValue = 16L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_hemostatic_bandage_desc");
		return gameItem;
	}

	private GameItem TopicalBandage()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "topical_bandage_small");
		gameItem.name = LocHelper.GetLocalizedItem("item_topical_bandage_name");
		gameItem.canActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item) && !PlayerStore.Instance.healthData.isWoundStable;
		gameItem.mayActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item) && !PlayerStore.Instance.healthData.isWoundStable;
		gameItem.onActivateSlotItemFunc = delegate(GameItem item, GameInventory _, SlotMarker _)
		{
			PlayerStore.Instance.healthData.isWoundStable = true;
			PlayerStore.Instance.healthData.woundState--;
			item.Destroy();
			PlayerStore.Instance.healthData.OnHealingItemUsed();
		};
		gameItem.EnableTag("ITEM_MEDICAL_TAG");
		AudioHelper.InitTextile(gameItem);
		gameItem.SetGameItemType("MEDICAL");
		gameItem.unitValue = 16L;
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_topical_bandage_desc");
		return gameItem;
	}

	private GameItem ImmunivaxInjector()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "injector_purple_large");
		gameItem.name = LocHelper.GetLocalizedItem("item_large_purple_injector_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_large_purple_injector_desc");
		gameItem.unitValue = 160L;
		InsInjectorHelper.Init(gameItem);
		InsInjectorHelper.SetGenuine(gameItem);
		gameItem.EnableManufacturer("carmicheal");
		AudioHelper.InitInjectorAudio(gameItem);
		gameItem.SetGameItemType("MEDICAL");
		ConsumableHelper.InitImmunivax(gameItem);
		return gameItem;
	}

	private GameItem SynaprestInjector()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_medical", "injector_black");
		gameItem.name = LocHelper.GetLocalizedItem("item_black_injector_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_black_injector_desc");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_black_injector_flavor");
		gameItem.unitValue = 350L;
		AudioHelper.InitInjectorAudio(gameItem);
		gameItem.SetGameItemType("MEDICAL");
		gameItem.SetGameItemType("LUXURY_ITEM");
		gameItem.EnableManufacturer("pastel");
		ConsumableHelper.InitSynaprest(gameItem);
		return gameItem;
	}

	public static void SetNomoreInjectorPurity(GameItem item, int minPurity, int maxPurity)
	{
		ChemicalHelper.RemoveAllLiquid(item);
		ChemFinisherRecipe.SetCompositionByPurity(item, minPurity, maxPurity, 4000, "nomore");
		ChemicalProductHelper.GradeProduct(item);
	}

	private GameItem Injector(InjectorColor color = InjectorColor.White, string name = "Injector", string ability = null)
	{
		GameItem gameItem = Empty();
		string spritePath = "injector_white";
		gameItem.EnableManufacturer("carmicheal");
		switch (color)
		{
		case InjectorColor.Pure_White:
			spritePath = "injector_pure_white_s";
			gameItem.shortDescription = LocHelper.GetLocalizedItem("item_pure_white_injector_desc");
			gameItem.SetGameItemType("NARCOTIC");
			gameItem.unitValue = 30L;
			break;
		case InjectorColor.White:
			spritePath = "injector_white";
			gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("white_stim_heal"));
			gameItem.unitValue = 75L;
			break;
		case InjectorColor.Black:
			spritePath = "injector_black";
			break;
		case InjectorColor.Red:
			spritePath = "injector_red";
			gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("red_stim_heal"));
			gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("red_stim_bleed"));
			gameItem.EnableManufacturer("vision");
			gameItem.unitValue = 35L;
			break;
		case InjectorColor.Green:
			spritePath = "injector_green";
			gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("green_stim_heal"));
			gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("antitoxin"));
			gameItem.unitValue = 50L;
			break;
		case InjectorColor.Turquoise:
			spritePath = "injector_turquoise";
			gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("combat_stim_acc_buff"));
			gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("combat_stim_cc_buff"));
			gameItem.unitValue = 40L;
			break;
		case InjectorColor.Purple:
			spritePath = "injector_purple";
			gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("white_stim_heal"));
			gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("green_stim_heal"));
			gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("red_stim_heal"));
			gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("red_stim_bleed"));
			gameItem.unitValue = 80L;
			break;
		case InjectorColor.Large_Purple:
			spritePath = "injector_purple_large";
			gameItem.shortDescription = LocHelper.GetLocalizedItem("item_large_purple_injector_desc");
			gameItem.unitValue = 160L;
			InsInjectorHelper.Init(gameItem);
			InsInjectorHelper.SetGenuine(gameItem);
			break;
		case InjectorColor.Blue:
			spritePath = "injector_blue";
			gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("blue_stim_heal"));
			gameItem.EnableManufacturer("vision");
			gameItem.unitValue = 25L;
			break;
		case InjectorColor.Blood_Red:
			spritePath = "injector_blood_red";
			gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("blood_red_max_health"));
			gameItem.effects.Add(DirectoryMaster.CombatAbilityEffect("blood_red_heal"));
			gameItem.unitValue = 65L;
			break;
		case InjectorColor.Pink:
			spritePath = "injector_pink";
			gameItem.shortDescription = LocHelper.GetLocalizedItem("item_pink_injector_desc");
			gameItem.unitValue = 50L;
			gameItem.SetGameItemType("NARCOTIC");
			gameItem.SetGameItemType("SUBSTANCE");
			ConsumableHelper.InitNarcotic(gameItem, 1);
			break;
		case InjectorColor.PinkAlt:
			spritePath = "injector_pink_alt";
			gameItem.shortDescription = LocHelper.GetLocalizedItem("item_pink_alt_injector_desc");
			gameItem.unitValue = 50L;
			gameItem.SetGameItemType("MEDICAL");
			ConsumableHelper.InitNarcoticRelief(gameItem, 1);
			ChemicalHelper.InitChemItem(gameItem, isBlendable: true, isSoluble: true);
			SetNomoreInjectorPurity(gameItem, 45, 60);
			ChemicalProductHelper.InitChemicalProduct(gameItem, "nomore");
			ChemicalFeedbackHelper.InitChemicalIntentItem(gameItem, ChemicalFeedbackHelper.ChemicalIntent.MEDICAL);
			break;
		}
		if (ability == null)
		{
			ability = "nothing";
		}
		gameItem.SetSpriteAndShape("Items/items_medical", spritePath);
		gameItem.name = name;
		gameItem.bonusAccuracy = 1000;
		gameItem.EnableTag("injector");
		gameItem.EnableProficiency("MEDICAL_PROFICIENCY");
		gameItem.EnableTag("ITEM_MEDICAL_TAG");
		gameItem.SetGameItemType("MEDICAL");
		AudioHelper.InitInjectorAudio(gameItem);
		return gameItem;
	}

	private GameItem ChemScanner()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_amenities", "chem_scanner");
		gameItem.name = LocHelper.GetLocalizedItem("item_chem_scanner_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_chem_scanner_desc");
		gameItem.EnableManufacturer("carmicheal");
		gameItem.SetValue(400L);
		AudioHelper.InitScanner(gameItem);
		gameItem.EnableTag("CAN_USE_ON_NON_OWNED_TAG");
		gameItem.maySelectSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => true;
		gameItem.mayThisTargetItemFunc = (GameItem s, GameItem t) => (t.IsTag("LIQUID_CONTAINER_TAG") || t.IsTag("CHEM_ITEM_TAG") || t.IsTag("CHEMICAL_PRODUCT_TAG")) ? true : false;
		gameItem.canThisTargetItemFunc = (GameItem s, GameItem t) => (t.IsTag("LIQUID_CONTAINER_TAG") || t.IsTag("CHEM_ITEM_TAG") || t.IsTag("CHEMICAL_PRODUCT_TAG")) ? true : false;
		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
		{
			if (t.IsTag("LIQUID_CONTAINER_TAG") || t.IsTag("CHEM_ITEM_TAG") || t.IsTag("CHEMICAL_PRODUCT_TAG"))
			{
				ChemScannerUIManager.Instance.Open(s, t);
				AudioManager.Instance.Play3("scanner_activate");
			}
		};
		return gameItem;
	}

	private GameItem SurgeryToolset()
	{
		GameItem gameItem = Empty().SetSpriteAndShape("Items/items_tool", "surgery_toolkit");
		gameItem.name = LocHelper.GetLocalizedItem("item_surgery_tool_name");
		gameItem.flavorText = LocHelper.GetLocalizedItem("item_surgery_tool_flavor");
		gameItem.SetGameItemType("MEDICAL");
		gameItem.SetGameItemType("MEDICAL_TOOL");
		gameItem.unitValue = 200L;
		AquacultureHelper.InitEggSurgeryTool(gameItem);
		HusbandryHelper.InitAnimalSurgeryTool(gameItem);
		return gameItem;
	}
}
