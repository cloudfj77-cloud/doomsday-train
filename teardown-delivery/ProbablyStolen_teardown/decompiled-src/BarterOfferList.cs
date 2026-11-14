using System;

public class BarterOfferList
{
	public static BarterOffer CreateAddictBarter()
	{
		BarterOffer barterOffer = new BarterOffer();
		barterOffer.barterTextBody = LocHelper.GetLocalizedDialogue("dialog_barter_for", LocHelper.GetLocalizedItem("item_pink_injector_name"));
		barterOffer.acceptedResponse = LocHelper.GetLocalizedDialogue("dialog_barter_addict_blood_yes");
		barterOffer.AddOfferSet(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("wire");
			PlayerStore.Instance.AddDirectSellingItemToTable("wire");
			PlayerStore.Instance.AddDirectSellingItemToTable("wire");
			PlayerStore.Instance.AddDirectSellingItemToTable("wire");
		});
		barterOffer.AddOfferSet(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("blood_bag");
			PlayerStore.Instance.AddDirectSellingItemToTable("blood_bag");
			PlayerStore.Instance.AddDirectSellingItemToTable("blood_bag");
		});
		barterOffer.AddOfferSet(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat");
			PlayerStore.Instance.AddDirectSellingItemToTable("processed_meat");
			PlayerStore.Instance.AddDirectSellingItemToTable("soda_red");
		});
		if (RNG.Roll(5))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("c4");
			});
		}
		if (RNG.Roll(15))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("box_tampon");
			});
		}
		if (RNG.Roll(15))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("combat_knife");
			});
		}
		BarterCondition barterCondition = new BarterCondition();
		barterCondition.requiredItemId = "pink_injector";
		barterOffer.conditions.Add(barterCondition);
		return barterOffer;
	}

	public static BarterOffer CreateAddictGeneralBarter()
	{
		BarterOffer barterOffer = new BarterOffer();
		barterOffer.barterTextBody = LocHelper.GetLocalizedDialogue("dialog_barter_for", TypeHelper.GetTypeDisplayName("NARCOTIC"));
		barterOffer.AddOfferSet(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("wire");
			PlayerStore.Instance.AddDirectSellingItemToTable("wire");
			PlayerStore.Instance.AddDirectSellingItemToTable("wire");
		});
		barterOffer.AddOfferSet(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("blood_bag");
			PlayerStore.Instance.AddDirectSellingItemToTable("blood_bag");
		});
		barterOffer.AddOfferSet(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("glass_shard_shiv");
			PlayerStore.Instance.AddDirectSellingItemToTable("pipe_weapon");
			PlayerStore.Instance.AddDirectSellingItemToTable("box_cutter");
		});
		if (RNG.Roll(8))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill");
				PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill");
				PlayerStore.Instance.AddDirectSellingItemToTable("birth_control_pill");
			});
		}
		if (RNG.Roll(8))
		{
			string food1 = TableMaster.Roll("packedFoodTable");
			string food2 = TableMaster.Roll("packedFoodTable");
			string food3 = TableMaster.Roll("packedFoodTable");
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn(food1));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn(food2));
				PlayerStore.Instance.AddDirectSellingItemToTable(ItemSpawner.Spawn(food3));
			});
		}
		if (RNG.Roll(7))
		{
			int energyCreditAmount = 50 + RNG.GetRandomInt(0, 50);
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.EnergyCredit(energyCreditAmount));
			});
		}
		if (RNG.Roll(5))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("wire_cutter");
				PlayerStore.Instance.AddDirectSellingItemToTable("screwdriver");
			});
		}
		if (RNG.Roll(5))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("box_tampon");
			});
		}
		if (RNG.Roll(5))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("c4");
			});
		}
		if (RNG.Roll(8))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("kotton_fabric");
				PlayerStore.Instance.AddDirectSellingItemToTable("kotton_fabric");
			});
		}
		if (RNG.Roll(6))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("system_module_efficiancy");
			});
		}
		if (RNG.Roll(6))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("system_module_performance");
			});
		}
		if (RNG.Roll(6))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("system_module_quality");
			});
		}
		if (RNG.Roll(3))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("system_module_overclock");
			});
		}
		if (RNG.Roll(3))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("system_module_eco");
			});
		}
		if (RNG.Roll(3))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("system_module_fineness");
			});
		}
		if (RNG.Roll(10))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRealExpiredInjector());
			});
		}
		if (RNG.Roll(2))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRealGenuineInjector());
			});
		}
		if (RNG.Roll(1))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("system_capped_neural_core");
			});
		}
		if (RNG.Roll(1))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("advanced_flux_agent");
			});
		}
		if (RNG.Roll(1))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("energy_credit_breeder");
			});
		}
		if (RNG.Roll(8))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("common_medical");
				PlayerStore.Instance.AddDirectSellingItemToTable("common_medical");
			});
		}
		if (RNG.Roll(8))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("common_chemical");
				PlayerStore.Instance.AddDirectSellingItemToTable("common_chemical");
			});
		}
		if (RNG.Roll(8))
		{
			barterOffer.AddOfferSet(delegate
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("combat_knife");
			});
		}
		BarterCondition barterCondition = new BarterCondition();
		barterCondition.requiredItemTag = "NARCOTIC";
		barterCondition.blacklistedConditionId.Add("starch_grade");
		barterCondition.blacklistedConditionId.Add("junk_grade");
		barterOffer.conditions.Add(barterCondition);
		barterOffer.acceptedResponse = LocHelper.GetLocalizedDialogue("dialog_barter_addict_blood_yes");
		return barterOffer;
	}

	public static BarterOffer CreateBarterFoodTutorial()
	{
		BarterOffer barterOffer = new BarterOffer();
		barterOffer.barterTextBody = LocHelper.GetLocalizedDialogue("dialog_barter_for", TypeHelper.GetTypeDisplayName("FOOD"));
		barterOffer.onInstance = delegate
		{
			TutorialUIManager.Instance.OnBarterTutorialStart();
			if (NewGameData.Instance == null)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable("blood_bag");
				PlayerStore.Instance.AddDirectSellingItemToTable("blood_bag");
			}
			else if (NewGameData.Instance.startType == NewGameData.StartType.APPRENTICE)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.EnergyCredit(33));
			}
			else if (NewGameData.Instance.startType == NewGameData.StartType.WATER_MERCHANT)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.FullWaterPurificationTabletBox());
			}
			else if (NewGameData.Instance.startType == NewGameData.StartType.SCAV)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.EnergyCredit(33));
			}
			else
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.EnergyCredit(33));
			}
		};
		BarterCondition barterCondition = new BarterCondition();
		barterCondition.requiredItemTag = "FOOD";
		barterOffer.conditions.Add(barterCondition);
		return barterOffer;
	}

	public static BarterOffer CreateBarterFoodMultiOffer()
	{
		BarterOffer barterOffer = new BarterOffer();
		barterOffer.barterTextBody = LocHelper.GetLocalizedDialogue("dialog_barter_for", TypeHelper.GetTypeDisplayName("FOOD"));
		barterOffer.AddOfferSet(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("blood_bag");
			PlayerStore.Instance.AddDirectSellingItemToTable("blood_bag");
		});
		barterOffer.AddOfferSet(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
			PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
			PlayerStore.Instance.AddDirectSellingItemToTable("scrap_metal");
		});
		barterOffer.AddOfferSet(delegate
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("scratcher");
			PlayerStore.Instance.AddDirectSellingItemToTable("scratcher");
		});
		BarterCondition barterCondition = new BarterCondition();
		barterCondition.requiredItemTag = "FOOD";
		barterOffer.conditions.Add(barterCondition);
		return barterOffer;
	}

	public static BarterOffer CreateFreeRewardChoice(string body, params Action[] bundles)
	{
		BarterOffer barterOffer = new BarterOffer();
		barterOffer.isFreeReward = true;
		barterOffer.barterTextBody = body;
		foreach (Action set in bundles)
		{
			barterOffer.AddOfferSet(set);
		}
		return barterOffer;
	}
}
