using System;
using System.Collections.Generic;

public class EnemyFactory
{
	public enum EnemyPartyPrefab
	{
		None,
		standard,
		sadist,
		boss,
		easy,
		junker,
		sadistDuo,
		pirateRookieTriple,
		pirateRookieFull,
		pirateVeteranLeaderTriple,
		pirateChiefLeaderTriple,
		pirateChielfVeteranFull
	}

	public Dictionary<EnemyPartyPrefab, Func<List<GameCharacterItem>>> EnemyPartyDictionary;

	private static EnemyFactory instance = null;

	private static readonly object padlock = new object();

	public static EnemyFactory Instance
	{
		get
		{
			lock (padlock)
			{
				if (instance == null)
				{
					instance = new EnemyFactory();
					instance.EnemyPartyDictionary = new Dictionary<EnemyPartyPrefab, Func<List<GameCharacterItem>>>();
					instance.EnemyPartyDictionary[EnemyPartyPrefab.standard] = CreatePremadeRaiderParty;
					instance.EnemyPartyDictionary[EnemyPartyPrefab.sadist] = CreatePremadeRaiderParty2;
					instance.EnemyPartyDictionary[EnemyPartyPrefab.boss] = CreatePremadeRaiderBoss;
					instance.EnemyPartyDictionary[EnemyPartyPrefab.easy] = CreatePremadeEasy;
					instance.EnemyPartyDictionary[EnemyPartyPrefab.junker] = CreatePremadeJunkers;
					instance.EnemyPartyDictionary[EnemyPartyPrefab.sadistDuo] = CreatePremadeSadistDuoParty;
					instance.EnemyPartyDictionary[EnemyPartyPrefab.pirateRookieTriple] = CreatePirateRookieTriple;
					instance.EnemyPartyDictionary[EnemyPartyPrefab.pirateRookieFull] = CreatePirateRookieFull;
					instance.EnemyPartyDictionary[EnemyPartyPrefab.pirateVeteranLeaderTriple] = CreatePirateVeteranLeaderTriple;
					instance.EnemyPartyDictionary[EnemyPartyPrefab.pirateChiefLeaderTriple] = CreatePirateChiefLeaderTriple;
					instance.EnemyPartyDictionary[EnemyPartyPrefab.pirateChielfVeteranFull] = CreatePirateChiefVeteranFull;
				}
				return instance;
			}
		}
	}

	public static List<GameCharacterItem> CreatePremadeRaiderParty()
	{
		List<GameCharacterItem> list = new List<GameCharacterItem>();
		GameCharacterItem gameCharacterItem = DirectoryMaster.CharacterItem("raider_technician");
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("pipe_weapon")).TryAcceptOnce();
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("raider_marine_overwatch");
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("kitchen_knife")).TryAcceptOnce();
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("raider_marine_sadistic");
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("kitchen_cleaver")).TryAcceptOnce();
		list.Add(gameCharacterItem);
		return list;
	}

	public static List<GameCharacterItem> CreatePremadeRaiderParty2()
	{
		List<GameCharacterItem> list = new List<GameCharacterItem>();
		GameCharacterItem gameCharacterItem = DirectoryMaster.CharacterItem("raider_technician_murderous");
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("pipe_weapon")).TryAcceptOnce();
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("raider_scav_murderous");
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("kitchen_knife")).TryAcceptOnce();
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("raider_medic_murderous");
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("kitchen_cleaver")).TryAcceptOnce();
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("raider_scav_murderous");
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("kitchen_knife")).TryAcceptOnce();
		list.Add(gameCharacterItem);
		return list;
	}

	public static List<GameCharacterItem> CreatePremadeRaiderBoss()
	{
		List<GameCharacterItem> list = new List<GameCharacterItem>();
		GameCharacterItem gameCharacterItem = DirectoryMaster.CharacterItem("raider_boss");
		gameCharacterItem.TryFindOneValidInventorySlot(GunFactory.Shotgun()).TryAcceptOnce();
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("armor_lining")).TryAcceptOnce();
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("white_injector")).TryAcceptOnce();
		list.Add(gameCharacterItem);
		return list;
	}

	public static List<GameCharacterItem> CreatePremadeEasy()
	{
		List<GameCharacterItem> list = new List<GameCharacterItem>();
		GameCharacterItem gameCharacterItem = DirectoryMaster.CharacterItem("raider_scav");
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("pipe_weapon")).TryAcceptOnce();
		gameCharacterItem.AddLoot(DropDirectory.GetRaiderDrop().GetDrops());
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("raider_scav");
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("baseball_bat")).TryAcceptOnce();
		gameCharacterItem.AddLoot(DropDirectory.GetRaiderDrop().GetDrops());
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("raider_scav");
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("baseball_bat")).TryAcceptOnce();
		gameCharacterItem.AddLoot(DropDirectory.GetRaiderDrop().GetDrops());
		list.Add(gameCharacterItem);
		return list;
	}

	public static List<GameCharacterItem> CreatePremadeJunkers()
	{
		List<GameCharacterItem> list = new List<GameCharacterItem>();
		GameCharacterItem gameCharacterItem = DirectoryMaster.CharacterItem("raider_junker");
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("pipe_weapon")).TryAcceptOnce();
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("bicarsole_pill")).TryAcceptOnce();
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("raider_junker");
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("baseball_bat")).TryAcceptOnce();
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("bicarsole_pill"));
		list.Add(gameCharacterItem);
		return list;
	}

	public static List<GameCharacterItem> CreatePremadeSadistDuoParty()
	{
		List<GameCharacterItem> list = new List<GameCharacterItem>();
		GameCharacterItem gameCharacterItem = DirectoryMaster.CharacterItem("raider_marine_sadistic");
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("kitchen_knife")).TryAcceptOnce();
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("raider_marine_sadistic");
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("kitchen_knife")).TryAcceptOnce();
		list.Add(gameCharacterItem);
		return list;
	}

	public static List<GameCharacterItem> CreatePremadeGearedTrioParty()
	{
		List<GameCharacterItem> list = new List<GameCharacterItem>();
		GameCharacterItem gameCharacterItem = DirectoryMaster.CharacterItem("raider_marine_sadistic");
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("kitchen_knife")).TryAcceptOnce();
		gameCharacterItem.AddLoot(DropDirectory.GetRaiderDrop(3).GetDrops());
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("raider_marine");
		gameCharacterItem.TryFindOneValidInventorySlot(GunFactory.Shotgun()).TryAcceptOnce();
		gameCharacterItem.AddLoot(DropDirectory.GetRaiderDrop().GetDrops());
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("armor_lining")).TryAcceptOnce();
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("armor_lining")).TryAcceptOnce();
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("raider_junker");
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("baseball_bat")).TryAcceptOnce();
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("bicarsole_pill"));
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("armor_lining")).TryAcceptOnce();
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("armor_lining")).TryAcceptOnce();
		list.Add(gameCharacterItem);
		return list;
	}

	public static List<GameCharacterItem> CreatePremadeGearedGuards()
	{
		List<GameCharacterItem> list = new List<GameCharacterItem>();
		GameCharacterItem gameCharacterItem = DirectoryMaster.CharacterItem("guard_marine");
		gameCharacterItem.TryFindOneValidInventorySlot(GunFactory.Shotgun()).TryAcceptOnce();
		gameCharacterItem.AddLoot(DropDirectory.GetRaiderDrop().GetDrops());
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("armor_lining")).TryAcceptOnce();
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("armor_lining")).TryAcceptOnce();
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("guard_marine");
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("baseball_bat")).TryAcceptOnce();
		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("bicarsole_pill"));
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("armor_lining")).TryAcceptOnce();
		gameCharacterItem.TryFindOneValidInventorySlot(DirectoryMaster.Item("armor_lining")).TryAcceptOnce();
		list.Add(gameCharacterItem);
		return list;
	}

	public static List<GameCharacterItem> CreatePremadeMilitaryTeam()
	{
		List<GameCharacterItem> list = new List<GameCharacterItem>();
		GameCharacterItem gameCharacterItem = DirectoryMaster.CharacterItem("deserter_marine");
		Loadout.CreateDeserterMarineLoadout(gameCharacterItem);
		list.Add(gameCharacterItem);
		if (RNG.Roll(50))
		{
			gameCharacterItem = DirectoryMaster.CharacterItem("deserter_marine");
			Loadout.CreateDeserterMarineLoadout(gameCharacterItem);
			list.Add(gameCharacterItem);
		}
		if (RNG.Roll(30))
		{
			gameCharacterItem = DirectoryMaster.CharacterItem("deserter_combat_medic");
			Loadout.CreateDeserterCombatMedicLoadout(gameCharacterItem);
			list.Add(gameCharacterItem);
		}
		if (RNG.Roll(10))
		{
			gameCharacterItem = DirectoryMaster.CharacterItem("deserter_marine");
			Loadout.CreateDeserterMarineLoadout(gameCharacterItem);
			list.Add(gameCharacterItem);
		}
		return list;
	}

	public static List<GameCharacterItem> CreatePremadeXenoTeam()
	{
		List<GameCharacterItem> list = new List<GameCharacterItem>();
		GameCharacterItem gameCharacterItem = DirectoryMaster.CharacterItem("xeno_drone");
		Loadout.CreateXenoDroneLoadout(gameCharacterItem);
		list.Add(gameCharacterItem);
		if (RNG.Roll(75))
		{
			gameCharacterItem = DirectoryMaster.CharacterItem("xeno_drone");
			Loadout.CreateXenoDroneLoadout(gameCharacterItem);
			list.Add(gameCharacterItem);
		}
		if (RNG.Roll(100))
		{
			gameCharacterItem = DirectoryMaster.CharacterItem("xeno_herald");
			Loadout.CreateXenoHeraldLoadout(gameCharacterItem);
			list.Add(gameCharacterItem);
		}
		if (RNG.Roll(75))
		{
			gameCharacterItem = DirectoryMaster.CharacterItem("xeno_hunter");
			Loadout.CreateXenoHunterLoadout(gameCharacterItem);
			list.Add(gameCharacterItem);
		}
		return list;
	}

	public static List<GameCharacterItem> CreatePirateRookieTriple()
	{
		List<GameCharacterItem> list = new List<GameCharacterItem>();
		GameCharacterItem gameCharacterItem = DirectoryMaster.CharacterItem("pirate_rookie");
		Loadout.CreatePirateRookieLoadout(gameCharacterItem);
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("pirate_rookie");
		Loadout.CreatePirateRookieLoadout(gameCharacterItem);
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("pirate_rookie");
		Loadout.CreatePirateRookieLoadout(gameCharacterItem);
		list.Add(gameCharacterItem);
		return list;
	}

	public static List<GameCharacterItem> CreatePirateRookieFull()
	{
		List<GameCharacterItem> list = new List<GameCharacterItem>();
		GameCharacterItem gameCharacterItem = DirectoryMaster.CharacterItem("pirate_rookie");
		Loadout.CreatePirateRookieLoadout(gameCharacterItem);
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("pirate_rookie");
		Loadout.CreatePirateRookieLoadout(gameCharacterItem);
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("pirate_rookie");
		Loadout.CreatePirateRookieLoadout(gameCharacterItem);
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("pirate_rookie");
		Loadout.CreatePirateRookieLoadout(gameCharacterItem);
		list.Add(gameCharacterItem);
		return list;
	}

	public static List<GameCharacterItem> CreatePirateVeteranLeaderTriple()
	{
		List<GameCharacterItem> list = new List<GameCharacterItem>();
		GameCharacterItem gameCharacterItem = DirectoryMaster.CharacterItem("pirate_veteran");
		Loadout.CreatePirateRookieLoadout(gameCharacterItem);
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("pirate_rookie");
		Loadout.CreatePirateRookieLoadout(gameCharacterItem);
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("pirate_rookie");
		Loadout.CreatePirateRookieLoadout(gameCharacterItem);
		list.Add(gameCharacterItem);
		return list;
	}

	public static List<GameCharacterItem> CreatePirateChiefLeaderTriple()
	{
		List<GameCharacterItem> list = new List<GameCharacterItem>();
		GameCharacterItem gameCharacterItem = DirectoryMaster.CharacterItem("pirate_chief");
		Loadout.CreatePirateRookieLoadout(gameCharacterItem);
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("pirate_rookie");
		Loadout.CreatePirateRookieLoadout(gameCharacterItem);
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("pirate_rookie");
		Loadout.CreatePirateRookieLoadout(gameCharacterItem);
		list.Add(gameCharacterItem);
		return list;
	}

	public static List<GameCharacterItem> CreatePirateChiefVeteranFull()
	{
		List<GameCharacterItem> list = new List<GameCharacterItem>();
		GameCharacterItem gameCharacterItem = DirectoryMaster.CharacterItem("pirate_chief");
		Loadout.CreatePirateRookieLoadout(gameCharacterItem);
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("pirate_veteran");
		Loadout.CreatePirateRookieLoadout(gameCharacterItem);
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("pirate_veteran");
		Loadout.CreatePirateRookieLoadout(gameCharacterItem);
		list.Add(gameCharacterItem);
		gameCharacterItem = DirectoryMaster.CharacterItem("pirate_veteran");
		Loadout.CreatePirateRookieLoadout(gameCharacterItem);
		list.Add(gameCharacterItem);
		return list;
	}

	public static void PopulatePirate(List<EnemyPartyPrefab> list)
	{
		list.Add(EnemyPartyPrefab.pirateRookieTriple);
		list.Add(EnemyPartyPrefab.pirateRookieTriple);
		list.Add(EnemyPartyPrefab.pirateRookieTriple);
		list.Add(EnemyPartyPrefab.pirateRookieTriple);
		list.Add(EnemyPartyPrefab.pirateRookieFull);
		list.Add(EnemyPartyPrefab.pirateRookieTriple);
		list.Add(EnemyPartyPrefab.pirateRookieFull);
		list.Add(EnemyPartyPrefab.pirateVeteranLeaderTriple);
		list.Add(EnemyPartyPrefab.pirateChiefLeaderTriple);
	}

	public static void PopulateXeno(List<EnemyPartyPrefab> list)
	{
		list.Add(EnemyPartyPrefab.pirateRookieTriple);
	}
}
