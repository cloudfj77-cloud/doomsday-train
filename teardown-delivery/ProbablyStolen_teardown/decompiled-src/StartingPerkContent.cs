using System.Collections.Generic;
using UnityEngine;

public class StartingPerkContent
{
	private static object gamblingClientTracker;

	private static int lotteryBoughtCount;

	private static int scratcherBoughtCount;

	public static void HandleNewGamePerkBeforeItems()
	{
		if (StartingPerk.IsHoarder())
		{
			DebrisHelper.PlaceHoarderPiles();
		}
	}

	public static void HandleNewGamePerk()
	{
		if (StartingPerk.IsSaving())
		{
			PlayerStore.Instance.playerCash += 150;
		}
		if (StartingPerk.IsBadLease())
		{
			PlayerStore.Instance.rentValue += 40;
		}
		if (StartingPerk.IsConvict())
		{
			StoreReputation.ModReputationRaw("FACTION_SECURITY", -30);
		}
		if (StartingPerk.IsModelCitizen())
		{
			StoreReputation.ModReputationRaw("FACTION_SECURITY", 30);
		}
		if (StartingPerk.IsSycophant())
		{
			StoreReputation.ModReputationRaw("FACTION_UPPER_LEVEL", 15);
		}
		if (StartingPerk.IsRevolutionSympathizer())
		{
			StoreReputation.ModReputationRaw("FACTION_REVOLUTION", 25);
		}
		if (StartingPerk.IsKnownSnitch())
		{
			StoreReputation.ModReputationRaw("FACTION_REVOLUTION", -10);
			StoreReputation.ModReputationRaw("FACTION_BLACK_MARKET", -10);
		}
		if (StartingPerk.IsWellConnected())
		{
			foreach (StoreReputation storeReputation in PlayerStore.Instance.storeReputations)
			{
				storeReputation.ModReputationRaw(15.0);
			}
		}
		if (StartingPerk.IsPublicEnemy())
		{
			foreach (StoreReputation storeReputation2 in PlayerStore.Instance.storeReputations)
			{
				storeReputation2.ModReputationRaw(-15.0);
			}
		}
		if (StartingPerk.IsTapehead())
		{
			AddTapeheadCassettes();
		}
		if (StartingPerk.IsHandy())
		{
			AddHandyToolbox();
		}
		if (StartingPerk.IsMakeshiftReactor())
		{
			GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("makeshift_reactor"));
		}
		if (StartingPerk.IsAugMenace())
		{
			UnlockAugMenace();
		}
		if (StartingPerk.IsLucky())
		{
			PlayerStore.Instance.healthData.luck += 15;
		}
		if (StartingPerk.IsUnlucky())
		{
			PlayerStore.Instance.healthData.luck += -15;
		}
		if (StartingPerk.IsGamblingAddiction())
		{
			LotteryHelper.GiveStartingGamblersCoin();
		}
		if (StartingPerk.IsExpeditionOrganizer())
		{
			PlayerStore.Instance.expeditionData.UnlockExpedition();
		}
		if (StartingPerk.IsVending())
		{
			VendingMachineHelper.UnlockVendingMachine(50);
			PlayerStore.Instance.isVendingMachineUnlocked = true;
		}
		if (StartingPerk.IsWorkingPipes())
		{
			NetworkUpgrade.GetUpgradeById("GUTTERFLOW_TAP").Unlock();
			NetworkUpgrade.GetUpgradeById("RUSTWATER_TAP").Unlock();
		}
		if (StartingPerk.IsPrimeRealEstate())
		{
			PlayerStore.Instance.rentValue += 75;
		}
		if (StartingPerk.IsDegenerativeOsteoarthritis())
		{
			for (int i = 0; i < 2; i++)
			{
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("topical_bandage_item"));
			}
		}
	}

	private static void AddHandyToolbox()
	{
		GameItem gameItem = ItemFeatureHelper.SetShabby(DirectoryMaster.Item("toolbox"));
		gameItem.ModifyTag("TOOLBOX_UNLOCKED_SLOTS_INT", delegate(TagState s)
		{
			s.Enable().SetInt(4);
		});
		GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, gameItem);
		bool flag = gameItem.parentInventory != null;
		string[] array = new string[4] { "screwdriver", "wire_cutter", "crowbar", "welder" };
		for (int i = 0; i < array.Length; i++)
		{
			GameItem item = ItemFeatureHelper.SetShabby(DirectoryMaster.Item(array[i]));
			if (!flag || GraphUtils.TryAcceptAll(gameItem, item) > 0)
			{
				GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, item);
			}
		}
	}

	private static void UnlockAugMenace()
	{
		Shotgun.Unlock();
		GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, IntelHelper.CreateMentorNoteShotgun());
		GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("aug_id_guide"));
		PlayerStore.Instance.nextAugSpawnDay = StoreStation.Instance.dayCounter + RNG.GetRandomInt(2, 4);
	}

	public static void AddTapeheadCassettes()
	{
		List<GameItem> list = EmporiumEntry.Instance.cassettePlayer.FindAllChildItems();
		HashSet<string> hashSet = new HashSet<string>();
		foreach (GameItem item in list)
		{
			hashSet.Add(item.identifier);
		}
		string[] array = new string[7] { "cassette1", "cassette2", "cassette3", "cassette4", "cassette5", "cassette6", "cassette7" };
		foreach (string text in array)
		{
			if (!hashSet.Contains(text))
			{
				GameItem gameItem = DirectoryMaster.Item(text);
				if (gameItem != null)
				{
					ItemFeatureHelper.SetShabby(gameItem);
					GraphUtils.TryAcceptAllOrDestroy(EmporiumEntry.Instance.cassettePlayer, gameItem, "cassette player full");
				}
			}
		}
	}

	public static GameItem CreateUnownedDegradedCassette()
	{
		List<GameItem> list = EmporiumEntry.Instance.cassettePlayer.FindAllChildItems();
		HashSet<string> hashSet = new HashSet<string>();
		foreach (GameItem item2 in list)
		{
			hashSet.Add(item2.identifier);
		}
		string[] array = new string[14]
		{
			"cassette1d", "cassette2d", "cassette3d", "cassette4d", "cassette5d", "cassette6d", "cassette7d", "cassette8d", "cassette9d", "cassette10d",
			"cassette11d", "cassette12d", "cassette13d", "cassette14d"
		};
		List<string> list2 = new List<string>();
		string[] array2 = array;
		foreach (string item in array2)
		{
			if (!hashSet.Contains(item))
			{
				list2.Add(item);
			}
		}
		if (list2.Count == 0)
		{
			return DirectoryMaster.Item(array[Random.Range(0, array.Length)]);
		}
		GameItem gameItem = DirectoryMaster.Item(list2[Random.Range(0, list2.Count)]);
		if (gameItem == null)
		{
			return null;
		}
		return gameItem;
	}

	public static int GetSecretAdmirerChance()
	{
		int num = PlayerStore.Instance.storeClientManager.secretAdmirerDaysWithoutGift + 1;
		if (num >= 12)
		{
			return 100;
		}
		if (num <= 3)
		{
			return 3;
		}
		return 3 + 6 * (num - 3);
	}

	private static bool IsOwningUsableImmunivax()
	{
		foreach (GameItem allOwnedItem in EmporiumEntry.Instance.GetAllOwnedItems())
		{
			ItemFeature itemFeature = allOwnedItem.FindItemFeatureByID("CATEGORY_GENUINE_INJECTOR");
			if (itemFeature != null)
			{
				string identifier = itemFeature.realCondition.identifier;
				if (identifier == "genuineInjector" || identifier == "expiredInjector")
				{
					return true;
				}
			}
		}
		return false;
	}

	public static void HandleSecretAdmirerOnOpenShutter()
	{
		if (!StartingPerk.IsSecretAdmirer())
		{
			return;
		}
		StoreClientManager storeClientManager = PlayerStore.Instance.storeClientManager;
		if (StoreStation.Instance.dayCounter == 2)
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("galaxy_blend", isOwend: true);
			storeClientManager.secretAdmirerDaysWithoutGift = 0;
			return;
		}
		if (!RNG.Roll(GetSecretAdmirerChance()))
		{
			storeClientManager.secretAdmirerDaysWithoutGift++;
			return;
		}
		storeClientManager.secretAdmirerDaysWithoutGift = 0;
		if (RNG.Roll(15))
		{
			GameItem gameItem = CreateUnownedDegradedCassette();
			if (gameItem != null)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(gameItem, isOwend: true);
			}
		}
		if (StartingPerk.IsSentinel() && !storeClientManager.secretAdmirerInjectorGiven && !IsOwningUsableImmunivax())
		{
			storeClientManager.secretAdmirerInjectorGiven = true;
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRealGenuineInjector(), isOwend: true);
		}
		else if (HealthData.HasAnyAddiction() && !storeClientManager.secretAdmirerBlackInjectorGiven)
		{
			storeClientManager.secretAdmirerBlackInjectorGiven = true;
			PlayerStore.Instance.AddDirectSellingItemToTable("black_injector", isOwend: true);
		}
		else if (PlayerStore.Instance.healthData.woundState > 0)
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("topical_bandage_item", isOwend: true);
			PlayerStore.Instance.AddDirectSellingItemToTable("topical_bandage_item", isOwend: true);
		}
		else if (RNG.Roll(10))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("cat_bar", isOwend: true);
		}
		else if (RNG.Roll(10))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("li_eat_snackbar", isOwend: true);
		}
		else if (RNG.Roll(10))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("scratcher", isOwend: true);
		}
		else if (RNG.Roll(5))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("pipe_weapon", isOwend: true);
		}
		else if (RNG.Roll(5))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("shampoo", isOwend: true);
		}
		else if (RNG.Roll(5))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("pack_condom", isOwend: true);
		}
		else if (RNG.Roll(5))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("red_beer", isOwend: true);
		}
		else if (RNG.Roll(10))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("soda_red", isOwend: true);
		}
		else if (RNG.Roll(10))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("toilet_paper", isOwend: true);
		}
		else if (RNG.Roll(5))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("common_ore", isOwend: true);
		}
		else if (RNG.Roll(10))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("toothpaste", isOwend: true);
		}
		else if (RNG.Roll(10))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("small_raw_meat", isOwend: true);
		}
		else if (RNG.Roll(10))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("caffeine_pill", isOwend: true);
			PlayerStore.Instance.AddDirectSellingItemToTable("caffeine_pill", isOwend: true);
		}
		else if (RNG.Roll(4))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("blood_bag", isOwend: true);
		}
		else if (RNG.Roll(2))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("tazer", isOwend: true);
		}
		else if (RNG.Roll(1))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("blue_blood_bag", isOwend: true);
		}
		else if (RNG.Roll(1))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("black_injector", isOwend: true);
		}
		else if (RNG.Roll(3))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("salve", isOwend: true);
		}
		else if (RNG.Roll(1))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("system_capped_neural_core", isOwend: true);
		}
		else
		{
			PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit", isOwend: true);
			PlayerStore.Instance.AddDirectSellingItemToTable("nutrifruit", isOwend: true);
		}
		if (RNG.Roll(50))
		{
			PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateSecretAdmirerNote(), isOwend: true);
		}
	}

	public static void InitWelfareProgram()
	{
		if (StartingPerk.IsWelfareProgram() && !PlayerStore.Instance.isStampIntroduced)
		{
			PlayerStore.Instance.isStampIntroduced = true;
			GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("stamp_guide"));
			GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, IntelHelper.CreateMentorNoteStamp());
			GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("magnifier"));
		}
	}

	public static void HandleWelfareProgram()
	{
		if (StartingPerk.IsWelfareProgram() && (StoreStation.GetDayCounter() % 7 == 1 || StoreStation.GetDayCounter() == 1))
		{
			int num = (PlayerStore.Instance.GetTotalEstimatedValue() - PlayerStore.Instance.playerCash) / 3 + PlayerStore.Instance.playerCash;
			Debug.Log("Welfare Program triggered. Player modified computed wealth: " + num);
			if (num <= 600)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRealStamp(160), isOwend: true);
			}
			else if (num <= 1200)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRealStamp(160), isOwend: true);
			}
			else if (num <= 1800)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRealStamp(120), isOwend: true);
			}
			else if (num <= 2400)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRealStamp(80), isOwend: true);
			}
			else if (num <= 3000)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(PreBuiltItemHelper.CreateRealStamp(40), isOwend: true);
			}
		}
	}

	public static void ConsumeUpgradeMaterial(GameItem material)
	{
		if (!StartingPerk.IsTinkerer() || !RNG.Roll(20))
		{
			material.Destroy();
		}
	}

	public static int GetAgoraphobiaReadyDay()
	{
		int lastAfterhourOutingDay = PlayerStore.Instance.lastAfterhourOutingDay;
		if (lastAfterhourOutingDay >= 0)
		{
			return lastAfterhourOutingDay + 2 + 1;
		}
		return 0;
	}

	public static bool IsAgoraphobiaResting()
	{
		if (StartingPerk.IsAgoraphobia())
		{
			return StoreStation.Instance.dayCounter < GetAgoraphobiaReadyDay();
		}
		return false;
	}

	public static int GetAgoraphobiaRestNightsLeft()
	{
		return Mathf.Clamp(GetAgoraphobiaReadyDay() - StoreStation.Instance.dayCounter, 0, 2);
	}

	public static void HandleAgoraphobiaReady()
	{
		if (StartingPerk.IsAgoraphobia() && PlayerStore.Instance.lastAfterhourOutingDay >= 0 && StoreStation.Instance.dayCounter + 1 == GetAgoraphobiaReadyDay())
		{
			NightLog.AgoraphobiaReady();
		}
	}

	public static int HandleReportRewardConvict(int amount)
	{
		if (StartingPerk.IsConvict())
		{
			return RNG.MultiplyAndRound(amount, 0.75);
		}
		return amount;
	}

	public static int HandleReportRewardModelCitizen(int amount)
	{
		if (StartingPerk.IsModelCitizen())
		{
			return RNG.MultiplyAndRound(amount, 1.5);
		}
		return amount;
	}

	public static void HandleGamblingAddictionItemAdded(GameItem gameItem)
	{
		if (!StartingPerk.IsGamblingAddiction() || GeneralHelper.IsItemOwned(gameItem))
		{
			return;
		}
		bool flag = gameItem.identifier.Equals("lottery_ticket");
		bool flag2 = gameItem.identifier.Equals("scratcher");
		bool flag3 = gameItem.identifier.Equals("scratcher_stack");
		if ((!flag && !flag2 && !flag3) || (PlayerStore.Instance.currentClientInstance != null && PlayerStore.Instance.currentClientInstance.GetClientBlueprint().barterOffer != null))
		{
			return;
		}
		if (PlayerStore.Instance.currentClientInstance != gamblingClientTracker)
		{
			gamblingClientTracker = PlayerStore.Instance.currentClientInstance;
			lotteryBoughtCount = 0;
			scratcherBoughtCount = 0;
		}
		if (flag)
		{
			if (RollGamblingPurchase(lotteryBoughtCount, 25))
			{
				lotteryBoughtCount++;
				ForceBuyGamblingItem(gameItem, 25);
			}
			return;
		}
		int price = (flag3 ? 100 : 20);
		if (RollGamblingPurchase(scratcherBoughtCount, price))
		{
			scratcherBoughtCount++;
			ForceBuyGamblingItem(gameItem, price);
		}
	}

	private static bool RollGamblingPurchase(int alreadyBought, int price)
	{
		switch (alreadyBought)
		{
		case 0:
			return PlayerStore.Instance.playerCash >= price;
		case 1:
			if (PlayerStore.Instance.playerCash >= price * 2)
			{
				return RNG.Roll(50);
			}
			return false;
		case 2:
			if (PlayerStore.Instance.playerCash >= price * 5)
			{
				return RNG.Roll(40);
			}
			return false;
		case 3:
			if (PlayerStore.Instance.playerCash >= price * 10)
			{
				return RNG.Roll(30);
			}
			return false;
		case 4:
			if (PlayerStore.Instance.playerCash >= price * 25)
			{
				return RNG.Roll(20);
			}
			return false;
		default:
			return false;
		}
	}

	private static void ForceBuyGamblingItem(GameItem gamblingItem, int price)
	{
		PlayerStore.Instance.ModCash(-price);
		GeneralHelper.SetItemOwned(gamblingItem);
		GraphUtils.TryAcceptAll(EmporiumEntry.Instance.backInvinvElement, gamblingItem);
	}
}
