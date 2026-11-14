public class StoreRequestList
{
	private static readonly LocalizedLines revCellNames = LocalizedLines.Mechanic("request_rev_cell_shadow", "request_rev_cell_gold", "request_rev_cell_eagle", "request_rev_cell_hq", "request_rev_cell_alpha", "request_rev_cell_theta");

	private static readonly LocalizedLines revFakeNames = LocalizedLines.Mechanic("request_rev_name_alexandra", "request_rev_name_jordan", "request_rev_name_taylor", "request_rev_name_louis", "request_rev_name_harun", "request_rev_name_kim");

	public static readonly string[] UL_REQUEST = new string[6] { "black_injector", "blue_blood_bag", "stun_gun", "box_tampon", "bottled_water_premium", "skincare_cream" };

	public static readonly string[] UL_REQUEST_CHARITY = new string[3] { "processed_meat", "toothpaste", "toilet_paper" };

	public static readonly string[] LOWER_STAY_AWAKE_ITEMS = new string[3] { "caffeine_pill", "energy_drink", "zerochew" };

	public static StoreRequest Blank()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = "";
		storeRequest.id = "";
		storeRequest.factionId = "FACTION_LOWER_LEVEL";
		storeRequest.description = "";
		storeRequest.duration = 1;
		storeRequest.reputationReward = 1;
		storeRequest.requiredItem = "eng_box";
		storeRequest.SetCreditReward(200, 1.5f);
		return storeRequest;
	}

	public static StoreRequest MeteorLL()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_meteor_ll_title");
		storeRequest.id = "meteor_ll";
		storeRequest.factionId = "FACTION_LOWER_LEVEL";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_meteor_ll_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_meteor_ll_requester");
		storeRequest.duration = 3;
		storeRequest.reputationReward = 10;
		storeRequest.requiredItem = "eng_box";
		storeRequest.SetCreditReward(200, 1.75f);
		return storeRequest;
	}

	public static StoreRequest MeteorUL()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_meteor_ul_title");
		storeRequest.id = "meteor_ul";
		storeRequest.factionId = "FACTION_UPPER_LEVEL";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_meteor_ul_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_meteor_ul_requester");
		storeRequest.duration = 3;
		storeRequest.reputationReward = 5;
		storeRequest.requiredItem = "eng_box";
		storeRequest.SetCreditReward(200, 2.5f);
		return storeRequest;
	}

	public static StoreRequest MedShortageLL()
	{
		return new StoreRequest
		{
			title = LocHelper.GetLocalizedMechanic("request_medshortage_ll_title"),
			id = "medshortage_ll",
			factionId = "FACTION_LOWER_LEVEL",
			description = LocHelper.GetLocalizedMechanic("request_medshortage_ll_desc"),
			requesterName = LocHelper.GetLocalizedMechanic("request_medshortage_ll_requester"),
			requiredItemCount = 3,
			duration = 3,
			reputationReward = 8,
			requiredItem = "common_medical",
			creditReward = 60
		};
	}

	public static StoreRequest MedShortageUL()
	{
		return new StoreRequest
		{
			title = LocHelper.GetLocalizedMechanic("request_medshortage_ul_title"),
			id = "medshortage_ul",
			factionId = "FACTION_UPPER_LEVEL",
			description = LocHelper.GetLocalizedMechanic("request_medshortage_ul_desc"),
			requesterName = LocHelper.GetLocalizedMechanic("request_medshortage_ul_requester"),
			requiredItemCount = 3,
			duration = 3,
			reputationReward = 3,
			requiredItem = "common_medical",
			creditReward = 100
		};
	}

	public static StoreRequest MedShortageBM()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_medshortage_bm_title");
		storeRequest.id = "medshortage_bm";
		storeRequest.factionId = "FACTION_BLACK_MARKET";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_medshortage_bm_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_medshortage_bm_requester");
		storeRequest.requiredItemCount = 3;
		storeRequest.duration = 3;
		storeRequest.reputationReward = 3;
		storeRequest.requiredItem = "common_medical";
		storeRequest.SetCreditReward(40, 3f);
		return storeRequest;
	}

	public static StoreRequest LLConstruction()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_ll_construction_title");
		storeRequest.id = "ll_construction";
		storeRequest.factionId = "FACTION_LOWER_LEVEL";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_ll_construction_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_ll_construction_requester");
		storeRequest.requiredItemCount = 5;
		storeRequest.duration = 3;
		storeRequest.reputationReward = 6;
		storeRequest.requiredItem = "scrap_metal";
		storeRequest.SetCreditReward(10, 1.5f);
		return storeRequest;
	}

	public static StoreRequest BMConstruction()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_bm_construction_title");
		storeRequest.id = "bm_construction";
		storeRequest.factionId = "FACTION_BLACK_MARKET";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_bm_construction_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_bm_construction_requester");
		storeRequest.requiredItemCount = 5;
		storeRequest.duration = 3;
		storeRequest.reputationReward = 3;
		storeRequest.requiredItem = "scrap_metal";
		storeRequest.SetCreditReward(10, 2f);
		return storeRequest;
	}

	public static StoreRequest LLSoupKitchen()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_ll_soup_kitchen_title");
		storeRequest.id = "ll_soup_kitchen";
		storeRequest.factionId = "FACTION_LOWER_LEVEL";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_ll_soup_kitchen_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_ll_soup_kitchen_requester");
		storeRequest.requiredItemCount = 5;
		storeRequest.duration = 4;
		storeRequest.reputationReward = 8;
		storeRequest.requiredItem = "processed_meat";
		storeRequest.SetCreditReward(25, 1.5f);
		return storeRequest;
	}

	public static StoreRequest SecEvidence()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_sec_evidence_title");
		storeRequest.id = "sec_evidence";
		storeRequest.factionId = "FACTION_SECURITY";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_sec_evidence_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_sec_evidence_requester");
		storeRequest.duration = 3;
		storeRequest.reputationReward = 10;
		storeRequest.requiredItem = "evidence_box";
		storeRequest.SetCreditReward(200, 2f);
		return storeRequest;
	}

	public static StoreRequest BMEvidence()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_bm_evidence_title");
		storeRequest.id = "bm_evidence";
		storeRequest.factionId = "FACTION_BLACK_MARKET";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_bm_evidence_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_bm_evidence_requester");
		storeRequest.duration = 3;
		storeRequest.reputationReward = 5;
		storeRequest.requiredItem = "evidence_box";
		storeRequest.SetCreditReward(200, 3.5f);
		return storeRequest;
	}

	public static string GetRandomCellName()
	{
		return RNG.GetRandomElement(revFakeNames);
	}

	public static StoreRequest RevOxymore()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_rev_oxymore_title");
		storeRequest.id = "rev_oxymore";
		storeRequest.factionId = "FACTION_REVOLUTION";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_rev_oxymore_desc");
		storeRequest.requesterName = GetRandomCellName();
		storeRequest.requestedItem = LocHelper.GetLocalizedMechanic("request_rev_oxymore_item");
		storeRequest.useCustomItemName = true;
		storeRequest.requiredItemCount = 3;
		storeRequest.duration = 5;
		storeRequest.reputationReward = 5;
		storeRequest.requiredItem = "pink_injector";
		storeRequest.SetCreditReward(50, 1.75f);
		return storeRequest;
	}

	public static StoreRequest RevScrapMetal()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_rev_scrap_title");
		storeRequest.id = "rev_scrap";
		storeRequest.factionId = "FACTION_REVOLUTION";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_rev_scrap_desc");
		storeRequest.requesterName = GetRandomCellName();
		storeRequest.requiredItemCount = 5;
		storeRequest.duration = 5;
		storeRequest.reputationReward = 5;
		storeRequest.requiredItem = "scrap_metal";
		storeRequest.SetCreditReward(10, 2f);
		return storeRequest;
	}

	public static StoreRequest RevSecCard()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_rev_seccard_title");
		storeRequest.id = "rev_seccard";
		storeRequest.factionId = "FACTION_REVOLUTION";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_rev_seccard_desc");
		storeRequest.requesterName = GetRandomCellName();
		storeRequest.requestedItem = LocHelper.GetLocalizedMechanic("request_rev_seccard_item");
		storeRequest.useCustomItemName = true;
		storeRequest.requiredItemCount = 1;
		storeRequest.duration = 5;
		storeRequest.reputationReward = 6;
		storeRequest.requiredItem = "sec_keycard";
		storeRequest.SetCreditReward(100, 3.5f);
		return storeRequest;
	}

	public static StoreRequest RevPeat()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_rev_peat_title");
		storeRequest.id = "rev_peat";
		storeRequest.factionId = "FACTION_REVOLUTION";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_rev_peat_desc");
		storeRequest.requesterName = GetRandomCellName();
		storeRequest.requiredItemCount = 4;
		storeRequest.duration = 5;
		storeRequest.reputationReward = 4;
		storeRequest.requiredItem = "processed_meat";
		storeRequest.SetCreditReward(25, 2f);
		return storeRequest;
	}

	public static StoreRequest ULRareItemRequest()
	{
		GameItem gameItem = DirectoryMaster.Item(RNG.GetRandomElement(UL_REQUEST));
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = gameItem.name;
		storeRequest.id = "ul_rare_item";
		storeRequest.factionId = "FACTION_UPPER_LEVEL";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_ul_rare_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_ul_rare_requester");
		storeRequest.requiredItemCount = 1;
		storeRequest.duration = 7;
		storeRequest.reputationReward = 2;
		storeRequest.requiredItem = gameItem.identifier;
		storeRequest.SetCreditReward((int)gameItem.unitValue, RNG.GetRandomFloat(1.5f, 2f));
		gameItem.Destroy();
		return storeRequest;
	}

	public static StoreRequest ULRareItemCharityRequest()
	{
		GameItem gameItem = DirectoryMaster.Item(RNG.GetRandomElement(UL_REQUEST_CHARITY));
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = gameItem.name;
		storeRequest.id = "ul_charity";
		storeRequest.factionId = "FACTION_UPPER_LEVEL";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_ul_charity_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_ul_charity_requester");
		storeRequest.requiredItemCount = 3;
		storeRequest.duration = 6;
		storeRequest.reputationReward = 5;
		storeRequest.requiredItem = gameItem.identifier;
		storeRequest.SetCreditReward((int)gameItem.unitValue, RNG.GetRandomFloat(1.2f, 1.3f));
		gameItem.Destroy();
		return storeRequest;
	}

	public static StoreRequest LowerWorkerStayAwakeRequest()
	{
		GameItem gameItem = DirectoryMaster.Item(RNG.GetRandomElement(LOWER_STAY_AWAKE_ITEMS));
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_lower_worker_title");
		storeRequest.id = "lower_worker_stayawake";
		storeRequest.factionId = "FACTION_LOWER_LEVEL";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_lower_worker_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_lower_worker_requester");
		storeRequest.requiredItemCount = 3;
		storeRequest.duration = 9;
		storeRequest.reputationReward = 1;
		storeRequest.requiredItem = gameItem.identifier;
		storeRequest.SetCreditReward((int)gameItem.unitValue, 1.5f);
		storeRequest.isUnique = false;
		gameItem.Destroy();
		return storeRequest;
	}

	public static StoreRequest GeorgioToiletPaper()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_georgio_tp_title");
		storeRequest.id = "unique_georgio_tp";
		storeRequest.factionId = "FACTION_LOWER_LEVEL";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_georgio_tp_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_georgio_tp_requester");
		storeRequest.requiredItemCount = 4;
		storeRequest.duration = 7;
		storeRequest.reputationReward = 1;
		storeRequest.requiredItem = "toilet_paper";
		storeRequest.SetCreditReward(25, 2f);
		storeRequest.isUnique = true;
		return storeRequest;
	}

	public static StoreRequest ButcherCleaver()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_butcher_cleaver_title");
		storeRequest.id = "unique_butcher_cleaver";
		storeRequest.factionId = "FACTION_LOWER_LEVEL";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_butcher_cleaver_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_butcher_cleaver_requester");
		storeRequest.requiredItemCount = 1;
		storeRequest.duration = 7;
		storeRequest.reputationReward = 0;
		storeRequest.requiredItem = "kitchen_cleaver";
		storeRequest.SetCreditReward(65, 2.5f);
		storeRequest.isUnique = true;
		return storeRequest;
	}

	public static StoreRequest SawyerBeer()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_sawyer_beer_title");
		storeRequest.id = "unique_sawyer_beer";
		storeRequest.factionId = "FACTION_BLACK_MARKET";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_sawyer_beer_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_sawyer_beer_requester");
		storeRequest.requiredItemCount = 6;
		storeRequest.duration = 5;
		storeRequest.reputationReward = 2;
		storeRequest.requiredItem = "red_beer";
		storeRequest.SetCreditReward(18, 1.5f);
		storeRequest.isUnique = true;
		return storeRequest;
	}

	public static StoreRequest LunSnackBar()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_lun_snackbar_title");
		storeRequest.id = "unique_lun_snackbar";
		storeRequest.factionId = "FACTION_LOWER_LEVEL";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_lun_snackbar_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_lun_snackbar_requester");
		storeRequest.requiredItemCount = 4;
		storeRequest.duration = 14;
		storeRequest.reputationReward = 1;
		storeRequest.requiredItem = "li_eat_snackbar";
		storeRequest.SetCreditReward(17, 2f);
		storeRequest.isUnique = true;
		return storeRequest;
	}

	public static StoreRequest CrowbarSawyer()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_sawyer_crowbar_title");
		storeRequest.id = "unique_sawyer_crowbar";
		storeRequest.factionId = "FACTION_BLACK_MARKET";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_sawyer_crowbar_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_sawyer_beer_requester");
		storeRequest.requiredItemCount = 1;
		storeRequest.duration = 14;
		storeRequest.reputationReward = 1;
		storeRequest.requiredItem = "crowbar";
		storeRequest.SetCreditReward(75, 2f);
		storeRequest.isUnique = true;
		return storeRequest;
	}

	public static StoreRequest EnergyDrinkJackson()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_jackson_energy_title");
		storeRequest.id = "unique_jackson_energy";
		storeRequest.factionId = "FACTION_LOWER_LEVEL";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_jackson_energy_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_jackson_energy_requester");
		storeRequest.requiredItemCount = 4;
		storeRequest.duration = 14;
		storeRequest.reputationReward = 1;
		storeRequest.requiredItem = "energy_drink";
		storeRequest.SetCreditReward(18, 2f);
		storeRequest.isUnique = true;
		return storeRequest;
	}

	public static StoreRequest LandlordSpecial()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_landlord_special1_title");
		storeRequest.id = "unique_landlord_special";
		storeRequest.factionId = "FACTION_UPPER_LEVEL";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_landlord_special1_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_landlord_requester");
		storeRequest.requiredItemCount = 5;
		storeRequest.duration = 7;
		storeRequest.reputationReward = 0;
		storeRequest.requiredItem = "scrap_metal";
		storeRequest.SetCreditReward(10, 1.4f);
		storeRequest.isUnique = true;
		return storeRequest;
	}

	public static StoreRequest LandlordSpecial2()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_landlord_special2_title");
		storeRequest.id = "unique_landlord_special2";
		storeRequest.factionId = "FACTION_UPPER_LEVEL";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_landlord_special2_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_landlord_requester");
		storeRequest.requiredItemCount = 3;
		storeRequest.duration = 12;
		storeRequest.reputationReward = 0;
		storeRequest.requiredItem = "common_electronic";
		storeRequest.SetCreditReward(33, 1.5f);
		storeRequest.isUnique = true;
		return storeRequest;
	}

	public static StoreRequest LandlordSpecial3()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_landlord_special3_title");
		storeRequest.id = "unique_landlord_special3";
		storeRequest.factionId = "FACTION_UPPER_LEVEL";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_landlord_special3_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_landlord_requester");
		storeRequest.requiredItemCount = 2;
		storeRequest.duration = 12;
		storeRequest.reputationReward = 0;
		storeRequest.requiredItem = "kotton_fabric";
		storeRequest.SetCreditReward(65, 1.5f);
		storeRequest.isUnique = true;
		return storeRequest;
	}

	public static StoreRequest LunRetirementParty()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_lun_retirement_title");
		storeRequest.id = "unique_lun_retirement";
		storeRequest.factionId = "FACTION_SECURITY";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_lun_retirement_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_lun_snackbar_requester");
		storeRequest.requiredItemCount = 1;
		storeRequest.duration = 14;
		storeRequest.reputationReward = 2;
		storeRequest.requiredItem = "raw_meat";
		storeRequest.SetCreditReward(159, 1.75f);
		storeRequest.isUnique = true;
		return storeRequest;
	}

	public static StoreRequest UpperCitizenPetMeat()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_upper_petmeat_title");
		storeRequest.id = "unique_upper_petmeat";
		storeRequest.factionId = "FACTION_UPPER_LEVEL";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_upper_petmeat_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_upper_petmeat_requester");
		storeRequest.requiredItemCount = 2;
		storeRequest.duration = 14;
		storeRequest.reputationReward = 2;
		storeRequest.requiredItem = "raw_meat";
		storeRequest.SetCreditReward(159, 2f);
		storeRequest.isUnique = true;
		return storeRequest;
	}

	public static StoreRequest MotherGalaxyBlend()
	{
		StoreRequest storeRequest = new StoreRequest();
		storeRequest.title = LocHelper.GetLocalizedMechanic("request_mother_galaxy_title");
		storeRequest.id = "unique_mother_galaxy_blend";
		storeRequest.factionId = "FACTION_LOWER_LEVEL";
		storeRequest.description = LocHelper.GetLocalizedMechanic("request_mother_galaxy_desc");
		storeRequest.requesterName = LocHelper.GetLocalizedMechanic("request_mother_galaxy_requester");
		storeRequest.requiredItemCount = 3;
		storeRequest.duration = 14;
		storeRequest.reputationReward = 2;
		storeRequest.requiredItem = "galaxy_blend";
		storeRequest.SetCreditReward(25, 1.5f);
		storeRequest.isUnique = true;
		return storeRequest;
	}
}
