using System.Collections.Generic;
using UnityEngine;

public class ScavHelper
{
	public const float MINOR_WOUND_CHANCE = 5f;

	public const float MAJOR_WOUND_CHANCE = 1f;

	public const int SCAV_TIME = 5;

	public const int INTRO_MAJOR_WOUND_IMMUNITY_SEARCHES = 20;

	public const int INTRO_MINOR_WOUND_IMMUNITY_SEARCHES = 10;

	public const float rareChance = 0.5f;

	public const float midChance = 1f;

	public const float junkChance = 2f;

	public const float scrapChance = 5f;

	public const string MODIFIER_INJURY_CHANCE = "injury_chance";

	public const string MODIFIER_INJURY_IMMUNITY = "injury_immunity";

	public const string MODIFIER_JUNK_CHANCE = "junk_chance";

	public const string MODIFIER_LOOT_DOUBLE_CHANCE = "loot_double_chance";

	public const string MODIFIER_SCAV_ATTEMPTS = "scav_attempts";

	public const string SCAV_TOKEN_TAG = "SCAV_TOKEN_TAG";

	public const string SCAV_SCANNER_DOUBLE_CHANCE = "SCAV_SCANNER_DOUBLE_CHANCE";

	public const int SCAV_SCANNER_MAX_DOUBLE_CHANCE = 100;

	public const string BACKPACK_FIND_SATCHEL = "satchel";

	public const string BACKPACK_FIND_KIDS = "kids_backpack";

	public const int BACKPACK_FIND_CHANCE = 3;

	private static float EventModifier(string key)
	{
		return AfterhourLocation.Get("dumping_ground")?.GetModifier(key) ?? 0f;
	}

	public static GameItem SpawnDumpingGroundLoot()
	{
		float num = EventModifier("junk_chance");
		if (num == 0f)
		{
			return ItemSpawner.SpawnFromTableGroup("dumpingGroundTG");
		}
		float scale = Mathf.Max(0f, 1f + num / 100f);
		return ItemSpawner.Spawn(LootRegistry.RollGroup("dumpingGroundTG", (string tableId) => (!(tableId == "junkTable")) ? 1f : scale));
	}

	public static void LoadScavTokenTag(GameItem item)
	{
		if (item.identifier == "scav_token")
		{
			item.EnableTag("SCAV_TOKEN_TAG");
		}
	}

	public static void ScavengeDumpingGrounds()
	{
		if (!CanScavenge())
		{
			return;
		}
		bool flag = PlayerStore.Instance.dumpingGroundMinorWoundImmunitySearchesLeft > 0;
		bool flag2 = PlayerStore.Instance.dumpingGroundMajorWoundImmunitySearchesLeft > 0;
		if (!flag && RollMinorWound())
		{
			if (HealthData.RollLuck())
			{
				NotifyLuckSavedFromInjury();
			}
			else
			{
				HealthData.ReceiveMinorWound();
				RunStats.OnScavengeInjury(major: false);
				AudioManager.Instance.Play2("debris_impact2");
			}
		}
		else if (!flag2 && RollMajorWound())
		{
			if (HealthData.RollLuck())
			{
				NotifyLuckSavedFromInjury();
			}
			else
			{
				HealthData.ReceiveMajorWound();
				RunStats.OnScavengeInjury(major: true);
				AudioManager.Instance.Play2("debris_impact");
			}
		}
		else if (HealthData.RollBadLuck())
		{
			NotifyBadLuckEmptySearch();
			AudioManager.Instance.Play2("debris");
		}
		else
		{
			StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("mech_scav_success"));
			GameItem backpackFind;
			List<GameItem> randomScavengedItem = GetRandomScavengedItem(out backpackFind);
			RunStats.OnScavengedItems(randomScavengedItem);
			for (int i = 0; i < randomScavengedItem.Count; i++)
			{
				GraphUtils.TryAcceptAll(EmporiumEntry.Instance.afterhourInventory, randomScavengedItem[i]);
			}
			MarkBackpackFound(backpackFind);
			AudioManager.Instance.Play2("debris");
		}
		if (flag)
		{
			PlayerStore.Instance.dumpingGroundMinorWoundImmunitySearchesLeft--;
		}
		if (flag2)
		{
			PlayerStore.Instance.dumpingGroundMajorWoundImmunitySearchesLeft--;
		}
		PlayerStore.Instance.scavengingAttempts++;
		RunStats.OnScavengeAttempt();
	}

	private static void NotifyLuckSavedFromInjury()
	{
		StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("mech_scav_luck_saved"));
	}

	private static void NotifyBadLuckEmptySearch()
	{
		StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("mech_scav_bad_luck_empty"), "sec");
	}

	public static bool RollMinorWound()
	{
		return RNG.Roll(GetMinorWoundChance());
	}

	public static bool RollMajorWound()
	{
		return RNG.Roll(GetMajorWoundChance());
	}

	public static string GetEligibleBackpackFind()
	{
		PlayerStore instance = PlayerStore.Instance;
		bool flag = instance.IsPlayerOwnThisItem("satchel");
		if (!instance.dumpingGroundSatchelFound && !flag)
		{
			return "satchel";
		}
		if ((instance.dumpingGroundSatchelFound || flag) && !instance.dumpingGroundKidsBackpackFound && !instance.IsPlayerOwnThisItem("kids_backpack"))
		{
			return "kids_backpack";
		}
		return null;
	}

	private static void MarkBackpackFound(GameItem backpack)
	{
		if (backpack != null && backpack.parentInventory != null)
		{
			if (backpack.identifier == "satchel")
			{
				PlayerStore.Instance.dumpingGroundSatchelFound = true;
			}
			else if (backpack.identifier == "kids_backpack")
			{
				PlayerStore.Instance.dumpingGroundKidsBackpackFound = true;
			}
		}
	}

	public static List<GameItem> GetRandomScavengedItem()
	{
		GameItem backpackFind;
		return GetRandomScavengedItem(out backpackFind);
	}

	public static List<GameItem> GetRandomScavengedItem(out GameItem backpackFind)
	{
		List<GameItem> list = new List<GameItem>();
		list.Add(SpawnDumpingGroundLoot());
		backpackFind = null;
		string eligibleBackpackFind = GetEligibleBackpackFind();
		if (eligibleBackpackFind != null && RNG.Roll(3))
		{
			backpackFind = DirectoryMaster.Item(eligibleBackpackFind);
			list.Add(backpackFind);
		}
		if (RNG.Roll(1))
		{
			GameItem gameItem = StartingPerkContent.CreateUnownedDegradedCassette();
			if (gameItem != null)
			{
				list.Add(gameItem);
			}
		}
		if (RNG.Roll(1))
		{
			list.Add(ItemSpawner.Spawn("mouse_trap"));
		}
		if (EmporiumEntry.Instance.IsAfterhourHaveOwnedItems("screwdriver") && RNG.Roll(10))
		{
			list.Add(ItemSpawner.Spawn("nuts_metal"));
		}
		if (EmporiumEntry.Instance.IsAfterhourHaveOwnedItems("wire_cutter") && RNG.Roll(10))
		{
			list.Add(ItemSpawner.Spawn("wire"));
		}
		if (EmporiumEntry.Instance.IsAfterhourHaveOwnedItems("welder") && RNG.Roll(10))
		{
			list.Add(ItemSpawner.Spawn("scrap_metal"));
		}
		float num = EventModifier("loot_double_chance");
		if (EmporiumEntry.Instance.IsAfterhourHaveOwnedItems("metal_scanner"))
		{
			num += (float)EmporiumEntry.Instance.GetAfterhourItemById("metal_scanner").GetTagReadonly("SCAV_SCANNER_DOUBLE_CHANCE").GetInt();
		}
		if (num > 0f && RNG.Roll(num))
		{
			AudioManager.Instance.Play3("click_vintage");
			list.Add(SpawnDumpingGroundLoot());
		}
		return list;
	}

	public static void ResetScavenging()
	{
		PlayerStore.Instance.scavengingAttempts = 0;
	}

	public static bool CanScavenge()
	{
		if (PlayerStore.Instance.devMode)
		{
			return true;
		}
		if (PlayerStore.Instance.healthData.isWoundedFresh)
		{
			return false;
		}
		if (GetScavTimeLeft() <= 0)
		{
			return false;
		}
		return true;
	}

	public static int GetMaxScavAttempts()
	{
		if (StartingPerk.IsMaimed())
		{
			return 0;
		}
		int num = 5;
		if (StartingPerk.IsProficientScavenger())
		{
			num += 2;
		}
		if (StartingPerk.IsCarefulScavenger())
		{
			num -= 2;
		}
		if (StartingPerk.IsDegenerativeOsteoarthritis())
		{
			int num2 = (StoreStation.Instance.dayCounter - 1) / 7;
			num -= num2;
			if (num < 3)
			{
				return 0;
			}
		}
		num += Mathf.RoundToInt(EventModifier("scav_attempts"));
		return Mathf.Max(0, num);
	}

	public static int GetScavTimeLeft()
	{
		return GetMaxScavAttempts() - PlayerStore.Instance.scavengingAttempts;
	}

	public static float GetMinorWoundChance()
	{
		if (StartingPerk.IsCarefulScavenger())
		{
			return 0f;
		}
		if (EventModifier("injury_immunity") > 0f)
		{
			return 0f;
		}
		float num = 5f + (float)PlayerStore.Instance.scavengingAttempts * 1f;
		if (EmporiumEntry.Instance.IsAfterhourHaveOwnedItems("flashlight"))
		{
			num /= 2f;
		}
		return num * (1f + EventModifier("injury_chance") / 100f);
	}

	public static float GetMajorWoundChance()
	{
		if (StartingPerk.IsCarefulScavenger())
		{
			return 0f;
		}
		if (EventModifier("injury_immunity") > 0f)
		{
			return 0f;
		}
		float num = 1f + (float)PlayerStore.Instance.scavengingAttempts * 0.25f;
		if (EmporiumEntry.Instance.IsAfterhourHaveOwnedItems("flashlight"))
		{
			num /= 2f;
		}
		return num * (1f + EventModifier("injury_chance") / 100f);
	}

	public static void InitScavScanner(GameItem gameItem)
	{
		gameItem.ModifyTag("SCAV_SCANNER_DOUBLE_CHANCE", delegate(TagState state)
		{
			state.Enable().SetInt(15);
		});
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.identifier.Equals("metal_scanner"))
		{
			if (CanUpgradeScavScanner(item))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_scav_scanner_upgrade"), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_scav_scanner_chance", item.GetTagReadonly("SCAV_SCANNER_DOUBLE_CHANCE").GetInt()), ignoreEmpty: true, RenderHandler.ColorPalette.LightTurquoise);
		}
	}

	public static void UpgradeScavScanner(GameItem scavScanner, GameItem upgradeItem)
	{
		if (scavScanner.identifier.Equals("metal_scanner") && CanUpgradeScavScanner(scavScanner))
		{
			int doubleLootChance = scavScanner.GetTagReadonly("SCAV_SCANNER_DOUBLE_CHANCE").GetInt();
			doubleLootChance += 5;
			if (doubleLootChance > 100)
			{
				doubleLootChance = 100;
			}
			scavScanner.ModifyTag("SCAV_SCANNER_DOUBLE_CHANCE", delegate(TagState state)
			{
				state.Enable().SetInt(doubleLootChance);
			});
			scavScanner.unitValue += 15L;
			StartingPerkContent.ConsumeUpgradeMaterial(upgradeItem);
			EmporiumAudioPlayer.Instance.ratchet.Play();
		}
	}

	public static bool CanUpgradeScavScanner(GameItem item)
	{
		if (item.GetTagReadonly("SCAV_SCANNER_DOUBLE_CHANCE").GetInt() < 100)
		{
			return true;
		}
		return false;
	}
}
