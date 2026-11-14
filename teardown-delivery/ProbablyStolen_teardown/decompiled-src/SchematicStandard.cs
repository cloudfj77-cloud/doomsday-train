using System;
using System.Collections.Generic;
using UnityEngine;

public class SchematicStandard : Schematic
{
	public string name;

	public List<RoomFactory.RoomPrefab> possibleRooms;

	public List<RoomFactory.RoomPrefab> mustRooms;

	public List<Room> currentRooms;

	public List<EnemyFactory.EnemyPartyPrefab> possibleEnemies;

	public bool isGenerateLoot;

	public bool isTrade;

	public List<StationFeature> features;

	public SchematicStandard()
	{
		name = "default";
		mustRooms = new List<RoomFactory.RoomPrefab>();
		possibleRooms = new List<RoomFactory.RoomPrefab>();
		possibleEnemies = new List<EnemyFactory.EnemyPartyPrefab>();
		currentRooms = new List<Room>();
		isGenerateLoot = true;
	}

	public Station Generate()
	{
		Room room = new Room(name + "'s Dock", Room.Type.Ship);
		Room room2 = room;
		if (mustRooms.Contains(RoomFactory.RoomPrefab.checkpoint))
		{
			Room room3 = RoomFactory.CreateSecurityCheckpoint(Room.Type.NPC);
			room.FullyConnectToRoom(room3);
			room3.AddWall();
			room2 = room3;
			currentRooms.Add(room3);
			AddEnemy(room2);
			mustRooms.Remove(RoomFactory.RoomPrefab.checkpoint);
		}
		Room room4 = RoomFactory.CreateHallway();
		room2.FullyConnectToRoom(room4);
		room2 = room4;
		currentRooms.Add(room4);
		room4.AddWall();
		int num = 200;
		int num2 = 0;
		while (num > 0 && mustRooms.Count != 0)
		{
			num--;
			RoomFactory.RoomPrefab roomPrefab = PickPrefabFromPossible(RoomFactory.RoomPrefab.hallway);
			if (roomPrefab == RoomFactory.RoomPrefab.None)
			{
				break;
			}
			mustRooms.Remove(roomPrefab);
			CreateRoomFromPrefab(room2, roomPrefab);
			num2++;
			if (num2 >= 2)
			{
				room2.AddWall();
				Room room5 = RoomFactory.CreateHallway();
				room2.FullyConnectToRoom(room5);
				currentRooms.Add(room5);
				room2 = room5;
				room5.AddWall();
				num2 = 0;
			}
		}
		if (isGenerateLoot)
		{
			SpawnLootInCurrentRooms();
		}
		Station station = new Station(room);
		station.stationName = name;
		ApplyFeatureToStation(station);
		station.HarmonizedTraderPrice();
		return station;
	}

	public void SpawnLootInCurrentRooms()
	{
		foreach (Room currentRoom in currentRooms)
		{
			currentRoom.SpawnAttachedLoot();
		}
	}

	public Room.Type HandleEnemy()
	{
		bool flag = false;
		Room.Type result = Room.Type.None;
		if (RNG.GetRandomInt(0, 9) >= 6)
		{
			flag = true;
		}
		if (flag)
		{
			result = Room.Type.NPC;
		}
		return result;
	}

	public Room.Type HandleTraders(RoomFactory.RoomPrefab newRoomPrefab)
	{
		Room.Type result = Room.Type.None;
		if (newRoomPrefab == RoomFactory.RoomPrefab.marketplace_clinic)
		{
			result = Room.Type.NPC;
		}
		if (newRoomPrefab == RoomFactory.RoomPrefab.marketplace_trader)
		{
			result = Room.Type.NPC;
		}
		if (newRoomPrefab == RoomFactory.RoomPrefab.marketplace)
		{
			result = Room.Type.NPC;
		}
		if (newRoomPrefab == RoomFactory.RoomPrefab.marketplace_military_surplus)
		{
			result = Room.Type.NPC;
		}
		if (newRoomPrefab == RoomFactory.RoomPrefab.trader_foundry)
		{
			result = Room.Type.NPC;
		}
		if (newRoomPrefab == RoomFactory.RoomPrefab.trader_energy_farm)
		{
			result = Room.Type.NPC;
		}
		if (newRoomPrefab == RoomFactory.RoomPrefab.trader_nutrifruit_farm)
		{
			result = Room.Type.NPC;
		}
		return result;
	}

	public void AddEnemy(Room generatedRoom)
	{
		int index = UnityEngine.Random.Range(0, possibleEnemies.Count);
		EnemyFactory.EnemyPartyPrefab key = possibleEnemies[index];
		List<GameCharacterItem> list = EnemyFactory.Instance.EnemyPartyDictionary[key]();
		generatedRoom.AddRaidersToRoom(list);
	}

	public void AddTrade(Room generatedRoom, RoomFactory.RoomPrefab currentRoomPrefab)
	{
		if (currentRoomPrefab == RoomFactory.RoomPrefab.marketplace_clinic)
		{
			GameCharacterItem npc = TraderFactory.CreateDoctor();
			generatedRoom.AddNPC(npc);
		}
		if (currentRoomPrefab == RoomFactory.RoomPrefab.marketplace)
		{
			GameCharacterItem npc2 = TraderFactory.CreateTrader();
			generatedRoom.AddNPC(npc2);
			GameCharacterItem npc3 = TraderFactory.CreateFenceTrader();
			generatedRoom.AddNPC(npc3);
		}
		if (currentRoomPrefab == RoomFactory.RoomPrefab.marketplace_military_surplus)
		{
			GameCharacterItem npc4 = TraderFactory.CreateArmyVendor();
			generatedRoom.AddNPC(npc4);
		}
		if (currentRoomPrefab == RoomFactory.RoomPrefab.trader_foundry)
		{
			GameCharacterItem npc5 = TraderFactory.CreateFoundryTrader();
			generatedRoom.AddNPC(npc5);
		}
		if (currentRoomPrefab == RoomFactory.RoomPrefab.trader_energy_farm)
		{
			GameCharacterItem npc6 = TraderFactory.CreateEnergyFarmTrader();
			generatedRoom.AddNPC(npc6);
		}
		if (currentRoomPrefab == RoomFactory.RoomPrefab.trader_nutrifruit_farm)
		{
			GameCharacterItem npc7 = TraderFactory.CreateNutrifruitFarmTrader();
			generatedRoom.AddNPC(npc7);
		}
	}

	public bool GenerateNeighbourRoom(Room lastRoom, RoomFactory.RoomPrefab currentRoomPrefab)
	{
		RoomFactory.RoomPrefab roomPrefab = PickPrefabFromPossible(currentRoomPrefab);
		if (roomPrefab == RoomFactory.RoomPrefab.None)
		{
			return false;
		}
		mustRooms.Remove(roomPrefab);
		CreateRoomFromPrefab(lastRoom, roomPrefab);
		return true;
	}

	public Room CreateRoomFromPrefab(Room lastRoom, RoomFactory.RoomPrefab newRoomPrefab)
	{
		Func<Room.Type, Room> func = RoomFactory.Instance.RoomDictionary[newRoomPrefab];
		Room.Type type = Room.Type.None;
		type = ((!isTrade) ? HandleEnemy() : HandleTraders(newRoomPrefab));
		Room room = func(type);
		if (isTrade)
		{
			AddTrade(room, newRoomPrefab);
		}
		else if (type == Room.Type.NPC)
		{
			AddEnemy(room);
		}
		lastRoom.FullyConnectToRoom(room);
		currentRooms.Add(room);
		room.AddWall();
		bool num = GenerateNeighbourRoom(room, newRoomPrefab);
		bool flag = GenerateNeighbourRoom(room, newRoomPrefab);
		if (num && flag)
		{
			room.AddWall();
		}
		return room;
	}

	public static List<RoomFactory.RoomPrefab> GetPrefabNeighbour(RoomFactory.RoomPrefab prefab)
	{
		List<RoomFactory.RoomPrefab> list = new List<RoomFactory.RoomPrefab>();
		switch (prefab)
		{
		case RoomFactory.RoomPrefab.hallway:
			list.Add(RoomFactory.RoomPrefab.hydroponics);
			list.Add(RoomFactory.RoomPrefab.crew_quarter);
			list.Add(RoomFactory.RoomPrefab.barack);
			list.Add(RoomFactory.RoomPrefab.cafeteria);
			list.Add(RoomFactory.RoomPrefab.infirmary);
			list.Add(RoomFactory.RoomPrefab.medbay_reception);
			list.Add(RoomFactory.RoomPrefab.security);
			list.Add(RoomFactory.RoomPrefab.checkpoint);
			list.Add(RoomFactory.RoomPrefab.power_bay);
			list.Add(RoomFactory.RoomPrefab.engineering_bay);
			list.Add(RoomFactory.RoomPrefab.hotel_reception);
			list.Add(RoomFactory.RoomPrefab.marketplace);
			list.Add(RoomFactory.RoomPrefab.trader_foundry);
			list.Add(RoomFactory.RoomPrefab.trader_energy_farm);
			list.Add(RoomFactory.RoomPrefab.trader_nutrifruit_farm);
			list.Add(RoomFactory.RoomPrefab.research_lobby);
			break;
		case RoomFactory.RoomPrefab.hydroponics:
			list.Add(RoomFactory.RoomPrefab.hydroponics);
			list.Add(RoomFactory.RoomPrefab.hydroponics_storage);
			break;
		case RoomFactory.RoomPrefab.crew_quarter:
			list.Add(RoomFactory.RoomPrefab.crew_quarter);
			break;
		case RoomFactory.RoomPrefab.checkpoint:
			list.Add(RoomFactory.RoomPrefab.bridge);
			break;
		case RoomFactory.RoomPrefab.cafeteria:
			list.Add(RoomFactory.RoomPrefab.kitchen);
			break;
		case RoomFactory.RoomPrefab.security:
			list.Add(RoomFactory.RoomPrefab.armory);
			break;
		case RoomFactory.RoomPrefab.infirmary:
			list.Add(RoomFactory.RoomPrefab.infirmary);
			break;
		case RoomFactory.RoomPrefab.medbay_reception:
			list.Add(RoomFactory.RoomPrefab.medbay);
			break;
		case RoomFactory.RoomPrefab.medbay:
			list.Add(RoomFactory.RoomPrefab.operation_theater);
			list.Add(RoomFactory.RoomPrefab.medical_storage);
			break;
		case RoomFactory.RoomPrefab.hotel_reception:
			list.Add(RoomFactory.RoomPrefab.hotel_room);
			break;
		case RoomFactory.RoomPrefab.barack:
			list.Add(RoomFactory.RoomPrefab.barack);
			break;
		case RoomFactory.RoomPrefab.marketplace:
			list.Add(RoomFactory.RoomPrefab.marketplace_trader);
			list.Add(RoomFactory.RoomPrefab.marketplace_clinic);
			list.Add(RoomFactory.RoomPrefab.marketplace_military_surplus);
			break;
		case RoomFactory.RoomPrefab.research_lobby:
			list.Add(RoomFactory.RoomPrefab.research_lab);
			list.Add(RoomFactory.RoomPrefab.research_lab_defense);
			list.Add(RoomFactory.RoomPrefab.research_lab_medical);
			break;
		}
		return list;
	}

	public RoomFactory.RoomPrefab PickPrefabFromPossible(RoomFactory.RoomPrefab prefab)
	{
		List<RoomFactory.RoomPrefab> prefabNeighbour = GetPrefabNeighbour(prefab);
		if (prefabNeighbour.Count == 0)
		{
			return RoomFactory.RoomPrefab.None;
		}
		List<RoomFactory.RoomPrefab> list = new List<RoomFactory.RoomPrefab>();
		foreach (RoomFactory.RoomPrefab mustRoom in mustRooms)
		{
			if (prefabNeighbour.Contains(mustRoom))
			{
				list.Add(mustRoom);
			}
		}
		if (list.Count == 0)
		{
			return RoomFactory.RoomPrefab.None;
		}
		int index = UnityEngine.Random.Range(0, list.Count);
		return list[index];
	}

	public static SchematicStandard SchematicStandardPrepare(Schematic.StationSize size, string name, OverworldStation.StationType type, bool isHostile)
	{
		SchematicStandard schematicStandard = new SchematicStandard();
		schematicStandard.name = name;
		if (isHostile)
		{
			Schematic.AddBasicRooms(schematicStandard.mustRooms, size);
			switch (type)
			{
			case OverworldStation.StationType.Waystation:
				Schematic.AddRoomWaystation(schematicStandard.mustRooms, size);
				break;
			case OverworldStation.StationType.Hospital:
				Schematic.AddRoomHospitalStation(schematicStandard.mustRooms, size);
				break;
			case OverworldStation.StationType.Hydroponics:
				Schematic.AddRoomHydroponicStation(schematicStandard.mustRooms, size);
				break;
			case OverworldStation.StationType.Military:
				Schematic.AddRoomMilitaryStation(schematicStandard.mustRooms, size);
				break;
			case OverworldStation.StationType.Research:
				Schematic.AddRoomResearchStation(schematicStandard.mustRooms, size);
				break;
			}
			schematicStandard.isTrade = false;
		}
		else
		{
			schematicStandard.GenerateTraderStationFeature();
			Schematic.AddRoomTradeStation(schematicStandard.mustRooms, size, schematicStandard);
			schematicStandard.isTrade = true;
		}
		EnemyFactory.PopulatePirate(schematicStandard.possibleEnemies);
		return schematicStandard;
	}

	public void GenerateTraderStationFeature()
	{
		features = new List<StationFeature>();
		int num = 2;
		StationFeature stationFeature = StationFeature.PickSpecialFeature();
		if (stationFeature != null)
		{
			features.Add(stationFeature);
		}
		else
		{
			features.Add(StationFeature.PickRandomFeature());
		}
		for (int i = 0; i < num - 1; i++)
		{
			features.Add(StationFeature.PickCompatibleFeature(features));
		}
	}

	public void ApplyFeatureToStation(Station station)
	{
		if (features == null || features.Count <= 0)
		{
			return;
		}
		foreach (StationFeature feature in features)
		{
			station.feature.Add(feature);
		}
	}
}
