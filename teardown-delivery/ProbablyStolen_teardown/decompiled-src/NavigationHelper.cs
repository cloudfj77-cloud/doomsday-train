using System.Collections.Generic;
using System.Linq;

public class NavigationHelper
{
	public const string SPAWN_CARGO_POD_WORLD_TAG = "SPAWN_CARGO_POD_WORLD_TAG";

	public static OverworldPOI currentEncounter;

	public static List<OverworldPOI> currentScannedEncounters;

	public const int CHANCE_TO_GET_SPACE_ENCOUNTER = 75;

	public const int BASE_NAV_SCAN_ALT = 2;

	public const int MAX_NAV_SCAN_ALT = 5;

	public static OverworldSpace tutorialSpace = new OverworldSpace
	{
		displayName = "Empty Space",
		identifier = "tutorial_space",
		customSpritePath = "asteroid"
	};

	public static OverworldSpace[] spaceEncounters = new OverworldSpace[22]
	{
		new OverworldSpace
		{
			displayName = "Mineral Rich Asteroid",
			identifier = "mining_asteroid",
			customSpritePath = "asteroid",
			spawnOdd = 10
		},
		new OverworldSpace
		{
			displayName = "Distress Signal",
			identifier = "distree_signal",
			customSpritePath = "signal",
			spawnOdd = 10
		},
		new OverworldSpace
		{
			displayName = "Ship Signature",
			identifier = "bandit_ship",
			customSpritePath = "small_ship",
			spawnOdd = 10
		},
		new OverworldSpace
		{
			displayName = "Spa Ads Broadcast",
			identifier = "spa_ship",
			customSpritePath = "signal",
			spawnOdd = 10
		},
		new OverworldSpace
		{
			displayName = "Ship Signature",
			identifier = "butcher_xeno_ship",
			customSpritePath = "small_ship",
			spawnOdd = 10
		},
		new OverworldSpace
		{
			displayName = "Cargo Pods",
			identifier = "cargo_pod",
			customSpritePath = "cargo_pod",
			spawnOdd = 10
		},
		new OverworldSpace
		{
			displayName = "Debris Field",
			identifier = "debris_field",
			customSpritePath = "debris_field",
			spawnOdd = 10
		},
		new OverworldSpace
		{
			displayName = "Large Ship Signature",
			identifier = "stolen_fuel_tanker",
			customSpritePath = "large_ship",
			spawnOdd = 10
		},
		new OverworldSpace
		{
			displayName = "Small Ship signature",
			identifier = "ghost_ship",
			customSpritePath = "small_ship",
			spawnOdd = 10
		},
		new OverworldSpace
		{
			displayName = "Hospital Ship Transponder",
			identifier = "hospital_ship",
			customSpritePath = "hospital",
			spawnOdd = 10
		},
		new OverworldSpace
		{
			displayName = "pastel Broadcast Signal",
			identifier = "nutrifruit_drone",
			customSpritePath = "signal",
			spawnOdd = 10
		},
		new OverworldSpace
		{
			displayName = "Small Ship Signature",
			identifier = "refugee_ship",
			customSpritePath = "small_ship",
			spawnOdd = 10
		},
		new OverworldSpace
		{
			displayName = "Shipwreck",
			identifier = "shipwreck",
			customSpritePath = "small_ship",
			spawnOdd = 10
		},
		new OverworldSpace
		{
			displayName = "Fleet Signature",
			identifier = "cargo_caravan_convoy",
			customSpritePath = "fleet",
			spawnOdd = 10
		},
		new OverworldSpace
		{
			displayName = "Fleet Signature",
			identifier = "space_battle",
			customSpritePath = "fleet",
			spawnOdd = 10
		},
		new OverworldSpace
		{
			displayName = "Merchant Ship Broadcast",
			identifier = "merchant_ship",
			customSpritePath = "merchant",
			spawnOdd = 10
		},
		new OverworldSpace
		{
			displayName = "Merchant Ship Broadcast",
			identifier = "scalper_ship",
			customSpritePath = "merchant",
			spawnOdd = 10
		},
		new OverworldSpace
		{
			displayName = "Small Ship Signature",
			identifier = "navy_supply_ship",
			customSpritePath = "small_ship",
			spawnOdd = 10
		},
		new OverworldSpace
		{
			displayName = "Small Ship Signature",
			identifier = "mining_ship",
			customSpritePath = "small_ship",
			spawnOdd = 10
		},
		new OverworldSpace
		{
			displayName = "Small Ship Signature",
			identifier = "salvage_ship",
			customSpritePath = "small_ship",
			spawnOdd = 10
		},
		new OverworldSpace
		{
			displayName = "Repair Ship Broadcast",
			identifier = "repair_ship",
			customSpritePath = "repair_ship",
			spawnOdd = 10
		},
		new OverworldSpace
		{
			displayName = "Drug Lab Ship Broadcast",
			identifier = "drug_lab_ship",
			customSpritePath = "drug_lab_ship",
			spawnOdd = 10
		}
	};

	public static bool CanScan()
	{
		return MachineryHelper.IsDrawSystemCharged(GetActiveNavSystem());
	}

	public static List<OverworldPOI> ScanForEncounters()
	{
		int scanCount = GetScanCount(GetActiveNavSystem());
		List<OverworldPOI> list = new List<OverworldPOI>();
		OverworldPOI item = new OverworldSpace
		{
			displayName = "Test Event",
			distance = 1,
			identifier = "mining_asteroid",
			customSpritePath = "medical_ship"
		};
		SpawnRewardEncounter(list);
		if (GameMaster.current.isDebugMode)
		{
			list.Add(item);
			list.Add(new OverworldStation().PrepareEncounter(forceTrade: true));
		}
		for (int i = 0; i < scanCount; i++)
		{
			if (RNG.Roll(75))
			{
				list.Add(GenerateRandomSpaceEncounter());
			}
			else
			{
				list.Add(new OverworldStation().PrepareEncounter());
			}
		}
		currentScannedEncounters = list;
		return list;
	}

	public static void ResetNavProgress()
	{
		MachineryHelper.ResetProgress(GetActiveNavSystem());
	}

	public static int GetScanCount(GameItem navSystem)
	{
		return navSystem.GetTagReadonly("NAVIGATION_ALTERNATIF_INT").GetInt();
	}

	public static int GetTurnUntilCharged(GameItem navSystem)
	{
		int @int = navSystem.GetTagReadonly("machinery_drawn_power").GetInt();
		int int2 = navSystem.GetTagReadonly("DRAW_CURRENT_INT").GetInt();
		int int3 = navSystem.GetTagReadonly("DRAW_TARGET_INT").GetInt();
		int num = int3 - int2;
		if (int3 == 0 || num <= 0 || @int == 0)
		{
			return 1;
		}
		return (@int + num - 1) / @int;
	}

	public static GameItem GetActiveNavSystem()
	{
		foreach (GameItem item in GraphUtils.FindAllChildrenType<GameItem>(GameMaster.current.shipManager.ship))
		{
			if (item.IsTag("SYSTEM_TAG_NAVIGATION") && item.IsTag("IN_SLOT_BOOL"))
			{
				return item;
			}
		}
		return null;
	}

	public static OverworldSpace GenerateRandomSpaceEncounter()
	{
		ApplySpaceEncounterOddModifier();
		OverworldSpace result = spaceEncounters[RNG.GetOdd(spaceEncounters.Select((OverworldSpace x) => x.spawnOdd).ToArray())];
		ResetSpaceEncounterOddModifier();
		return result;
	}

	public static void ApplySpaceEncounterOddModifier()
	{
		foreach (GameItem item in GraphUtils.FindAllChildrenType<GameItem>(GetActiveNavSystem()))
		{
			if (!item.IsTag("MODULE_TYPE_NAVIGATION") || !item.IsTag("DETECTION_ENCOUNTER_NAME"))
			{
				continue;
			}
			string @string = item.GetTagReadonly("DETECTION_ENCOUNTER_NAME").GetString();
			int @int = item.GetTagReadonly("DETECTION_ENCOUNTER_VALUE_INT").GetInt();
			OverworldSpace[] array = spaceEncounters;
			foreach (OverworldSpace overworldSpace in array)
			{
				if (@string.Equals(overworldSpace.identifier))
				{
					overworldSpace.spawnOdd += @int;
				}
			}
		}
	}

	public static void ResetSpaceEncounterOddModifier()
	{
		OverworldSpace[] array = spaceEncounters;
		foreach (OverworldSpace obj in array)
		{
			obj.spawnOdd = obj.defaultSpawnOdd;
		}
	}

	public static void SpawnRewardEncounter(List<OverworldPOI> encounters)
	{
		if (GameMaster.current != null && GameMaster.current.worldTags != null && GameMaster.current.worldTags.IsTag("SPAWN_CARGO_POD_WORLD_TAG"))
		{
			GameMaster.current.worldTags.DisableTag("SPAWN_CARGO_POD_WORLD_TAG");
			OverworldSpace item = new OverworldSpace
			{
				displayName = "Cargo Pod",
				distance = 1,
				identifier = "cargo_pod"
			};
			encounters.Add(item);
		}
	}
}
