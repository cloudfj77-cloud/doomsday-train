using System.Collections.Generic;

public interface Schematic
{
	public enum StationSize
	{
		small = 1,
		medium,
		large
	}

	Station Generate();

	static void AddBasicRooms(List<RoomFactory.RoomPrefab> rooms, StationSize stationSize)
	{
		if (stationSize >= StationSize.small)
		{
			rooms.Add(RoomFactory.RoomPrefab.crew_quarter);
			rooms.Add(RoomFactory.RoomPrefab.power_bay);
		}
		if (stationSize >= StationSize.medium)
		{
			rooms.Add(RoomFactory.RoomPrefab.crew_quarter);
			rooms.Add(RoomFactory.RoomPrefab.infirmary);
			rooms.Add(RoomFactory.RoomPrefab.checkpoint);
			rooms.Add(RoomFactory.RoomPrefab.cafeteria);
			rooms.Add(RoomFactory.RoomPrefab.kitchen);
		}
		if (stationSize >= StationSize.large)
		{
			rooms.Add(RoomFactory.RoomPrefab.crew_quarter);
			rooms.Add(RoomFactory.RoomPrefab.bridge);
			rooms.Add(RoomFactory.RoomPrefab.engineering_bay);
			rooms.Add(RoomFactory.RoomPrefab.security);
		}
		_ = 1;
		_ = 2;
		_ = 3;
	}

	static void AddRoomHydroponicStation(List<RoomFactory.RoomPrefab> rooms, StationSize stationSize)
	{
		if (stationSize >= StationSize.small)
		{
			rooms.Add(RoomFactory.RoomPrefab.hydroponics);
		}
		if (stationSize >= StationSize.medium)
		{
			rooms.Add(RoomFactory.RoomPrefab.hydroponics);
		}
		if (stationSize >= StationSize.large)
		{
			rooms.Add(RoomFactory.RoomPrefab.hydroponics);
		}
		if (stationSize == StationSize.small && RNG.Roll(50))
		{
			rooms.Add(RoomFactory.RoomPrefab.hydroponics);
		}
		_ = 2;
		if (stationSize == StationSize.large && RNG.Roll(50))
		{
			rooms.Add(RoomFactory.RoomPrefab.hydroponics);
		}
	}

	static void AddRoomMilitaryStation(List<RoomFactory.RoomPrefab> rooms, StationSize stationSize)
	{
		if (stationSize >= StationSize.small)
		{
			rooms.Add(RoomFactory.RoomPrefab.barack);
		}
		if (stationSize >= StationSize.medium)
		{
			rooms.Add(RoomFactory.RoomPrefab.barack);
		}
		if (stationSize >= StationSize.large)
		{
			rooms.Add(RoomFactory.RoomPrefab.barack);
		}
		if (stationSize == StationSize.small)
		{
			rooms.Add(RoomFactory.RoomPrefab.infirmary);
			if (RNG.Roll(25))
			{
				rooms.Add(RoomFactory.RoomPrefab.armory);
			}
		}
		if (stationSize == StationSize.medium && RNG.Roll(50))
		{
			rooms.Add(RoomFactory.RoomPrefab.armory);
		}
		if (stationSize == StationSize.large)
		{
			rooms.Add(RoomFactory.RoomPrefab.armory);
		}
		while (rooms.Contains(RoomFactory.RoomPrefab.crew_quarter))
		{
			rooms.Remove(RoomFactory.RoomPrefab.crew_quarter);
		}
	}

	static void AddRoomResearchStation(List<RoomFactory.RoomPrefab> rooms, StationSize stationSize)
	{
		if (stationSize >= StationSize.small)
		{
			rooms.Add(RoomFactory.RoomPrefab.research_lab);
			rooms.Add(RoomFactory.RoomPrefab.research_lobby);
		}
		_ = 2;
		_ = 3;
		_ = 1;
		if (stationSize == StationSize.medium)
		{
			rooms.Add(RoomFactory.RoomPrefab.research_lab);
		}
		if (stationSize == StationSize.large)
		{
			if (RNG.Roll(50))
			{
				rooms.Add(RoomFactory.RoomPrefab.research_lab_medical);
			}
			else
			{
				rooms.Add(RoomFactory.RoomPrefab.research_lab_defense);
			}
		}
	}

	static void AddRoomWaystation(List<RoomFactory.RoomPrefab> rooms, StationSize stationSize)
	{
		if (stationSize >= StationSize.small)
		{
			rooms.Add(RoomFactory.RoomPrefab.hotel_reception);
			rooms.Add(RoomFactory.RoomPrefab.hotel_room);
		}
		if (stationSize >= StationSize.medium)
		{
			rooms.Add(RoomFactory.RoomPrefab.hotel_room);
			rooms.Remove(RoomFactory.RoomPrefab.crew_quarter);
		}
		_ = 3;
		_ = 1;
		_ = 2;
		_ = 3;
	}

	static void AddRoomHospitalStation(List<RoomFactory.RoomPrefab> rooms, StationSize stationSize)
	{
		if (stationSize >= StationSize.small)
		{
			rooms.Add(RoomFactory.RoomPrefab.medbay);
			rooms.Add(RoomFactory.RoomPrefab.medbay_reception);
		}
		if (stationSize >= StationSize.medium)
		{
			rooms.Add(RoomFactory.RoomPrefab.medical_storage);
			rooms.Add(RoomFactory.RoomPrefab.operation_theater);
		}
		if (stationSize >= StationSize.large)
		{
			rooms.Add(RoomFactory.RoomPrefab.medbay);
		}
		_ = 1;
		_ = 2;
		_ = 3;
	}

	static void AddRoomTradeStation(List<RoomFactory.RoomPrefab> rooms, StationSize stationSize, SchematicStandard schematic)
	{
		if (stationSize >= StationSize.small)
		{
			rooms.Add(RoomFactory.RoomPrefab.marketplace);
			rooms.Add(RoomFactory.RoomPrefab.marketplace_clinic);
			rooms.Add(RoomFactory.RoomPrefab.marketplace_military_surplus);
		}
		_ = 2;
		_ = 3;
		_ = 1;
		_ = 2;
		_ = 3;
		AddRoomFromFeature(rooms, stationSize, schematic);
	}

	static void AddRoomFromFeature(List<RoomFactory.RoomPrefab> rooms, StationSize stationSize, SchematicStandard schematic)
	{
		foreach (StationFeature feature in schematic.features)
		{
			if (feature.identifier.Equals("ENERGY_FARM"))
			{
				rooms.Add(RoomFactory.RoomPrefab.trader_energy_farm);
			}
			if (feature.identifier.Equals("NUTRIFRUIT_FARM"))
			{
				rooms.Add(RoomFactory.RoomPrefab.trader_nutrifruit_farm);
			}
			if (feature.identifier.Equals("FOUNDRY"))
			{
				rooms.Add(RoomFactory.RoomPrefab.trader_foundry);
			}
		}
	}

	static void AddRoomFramework(List<RoomFactory.RoomPrefab> rooms, StationSize stationSize)
	{
		_ = 1;
		_ = 2;
		_ = 3;
		_ = 1;
		_ = 2;
		_ = 3;
	}
}
