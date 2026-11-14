using System;
using System.Collections.Generic;
using UnityEngine;

public class SchematicCorridor : Schematic
{
	public string name;

	public List<RoomFactory.RoomPrefab> possibleRooms;

	public List<RoomFactory.RoomPrefab> mustRooms;

	public List<EnemyFactory.EnemyPartyPrefab> possibleEnemies;

	private int maxRoom;

	private int minRoom;

	private int maxEnemies;

	private int minEnemies;

	public SchematicCorridor()
	{
		name = "default";
		possibleRooms = new List<RoomFactory.RoomPrefab>();
		possibleEnemies = new List<EnemyFactory.EnemyPartyPrefab>();
	}

	public Station Generate()
	{
		Room room = new Room(name + " Entrance", Room.Type.Ship);
		int num = UnityEngine.Random.Range(minRoom, maxRoom);
		Room room2 = RoomFactory.CreateSecurityCheckpoint(Room.Type.NPC);
		room.FullyConnectToRoom(room2);
		room2.AddRaidersToRoom(EnemyFactory.CreatePremadeRaiderParty());
		Room room3 = room2;
		List<Room> list = new List<Room>();
		for (int i = 0; i < num; i++)
		{
			int index = UnityEngine.Random.Range(0, possibleRooms.Count);
			RoomFactory.RoomPrefab key = possibleRooms[index];
			Func<Room.Type, Room> func = RoomFactory.Instance.RoomDictionary[key];
			bool flag = false;
			Room.Type arg = Room.Type.None;
			if (UnityEngine.Random.Range(0, 4) >= 3)
			{
				flag = true;
			}
			if (flag)
			{
				arg = Room.Type.NPC;
			}
			Room room4 = func(arg);
			room3.FullyConnectToRoom(room4);
			list.Add(room4);
			if (flag)
			{
				int index2 = UnityEngine.Random.Range(0, possibleEnemies.Count);
				EnemyFactory.EnemyPartyPrefab key2 = possibleEnemies[index2];
				List<GameCharacterItem> list2 = EnemyFactory.Instance.EnemyPartyDictionary[key2]();
				room4.AddRaidersToRoom(list2);
			}
			room3 = room4;
		}
		Room room5 = RoomFactory.CreateBridge(Room.Type.NPC);
		room5.FullyConnectToRoom(room3);
		room5.AddRaidersToRoom(EnemyFactory.CreatePremadeRaiderBoss());
		foreach (Room item in list)
		{
			item.SpawnAttachedLoot();
		}
		room2.SpawnAttachedLoot();
		room5.SpawnAttachedLoot();
		return new Station(room);
	}

	public static SchematicCorridor SchematicCorridorWaystation()
	{
		return new SchematicCorridor
		{
			name = "Waystation",
			possibleRooms = 
			{
				RoomFactory.RoomPrefab.cafeteria,
				RoomFactory.RoomPrefab.crew_quarter,
				RoomFactory.RoomPrefab.hydroponics,
				RoomFactory.RoomPrefab.infirmary,
				RoomFactory.RoomPrefab.kitchen
			},
			maxRoom = 6,
			minRoom = 3,
			possibleEnemies = 
			{
				EnemyFactory.EnemyPartyPrefab.standard,
				EnemyFactory.EnemyPartyPrefab.sadist,
				EnemyFactory.EnemyPartyPrefab.easy,
				EnemyFactory.EnemyPartyPrefab.junker
			},
			maxEnemies = 3,
			minEnemies = 3
		};
	}
}
