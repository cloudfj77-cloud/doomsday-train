using System.Collections.Generic;
using System.Linq;

public class Station
{
	public Room startRoom;

	public string stationName;

	public List<StationFeature> feature;

	public Station(Room startRoom)
	{
		feature = new List<StationFeature>();
		this.startRoom = startRoom;
		stationName = "default name";
	}

	public List<Room> GetAllRooms()
	{
		ISet<Room> set = new HashSet<Room>();
		Stack<Room> stack = new Stack<Room>();
		stack.Push(startRoom);
		set.Add(startRoom);
		while (stack.Any())
		{
			foreach (Room item in stack.Pop().connections.Values.ToList())
			{
				if (!set.Contains(item))
				{
					stack.Push(item);
					set.Add(item);
				}
			}
		}
		return set.ToList();
	}

	public void HarmonizedTraderPrice()
	{
		List<GameCharacterItem> list = FindAllTrader();
		foreach (StationFeature item in feature)
		{
			foreach (GameCharacterItem item2 in list)
			{
				var (list2, list3) = MetaHelper.RetrieveListInTagSystemStringInt("DISCOUNT_TAG_LIST", item2);
				MetaHelper.WipeListInTagSystemStringInt("DISCOUNT_TAG_LIST", item2);
				list2.AddRange(item.discountedItemTagList);
				list3.AddRange(item.discountedItemDiscountList);
				MetaHelper.StoreListInTagSystemStringInt(list2, list3, "DISCOUNT_TAG_LIST", item2);
			}
		}
	}

	public List<GameCharacterItem> FindAllTrader()
	{
		List<GameCharacterItem> list = new List<GameCharacterItem>();
		foreach (Room allRoom in GetAllRooms())
		{
			if (allRoom.npcParty != null && allRoom.npcParty.GetCharacterItems() != null)
			{
				list.AddRange(allRoom.npcParty.GetCharacterItems());
			}
		}
		return list.Where((GameCharacterItem element) => element.IsMerchant()).ToList();
	}

	public RichTextBuilder CreateStationDescription()
	{
		RichTextBuilder richTextBuilder = new RichTextBuilder();
		List<StationFeature> allBuildingFeature = GetAllBuildingFeature();
		List<StationFeature> allNonBuildingFeature = GetAllNonBuildingFeature();
		if (allBuildingFeature.Count > 0 || allNonBuildingFeature.Count > 0)
		{
			richTextBuilder.AddLine("From the comms chatter, you were able to pickup the following information:", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
		}
		if (allBuildingFeature.Count > 0)
		{
			richTextBuilder.Add("   This Station have a ");
			foreach (StationFeature item in allBuildingFeature)
			{
				if (item == allBuildingFeature.Last())
				{
					richTextBuilder.AddLine(item.displayName + ". ", ignoreEmpty: true, RenderHandler.ColorPalette.LightTurquoise);
				}
				else
				{
					richTextBuilder.Add(item.displayName + ", ", RenderHandler.ColorPalette.LightTurquoise);
				}
			}
		}
		if (allNonBuildingFeature.Count > 0)
		{
			richTextBuilder.AddLine("   This Station is experiencing a ");
			foreach (StationFeature item2 in allNonBuildingFeature)
			{
				if (item2 == allNonBuildingFeature.Last())
				{
					richTextBuilder.AddLine(item2.displayName + ". ", ignoreEmpty: true, RenderHandler.ColorPalette.LightTurquoise);
				}
				else
				{
					richTextBuilder.Add(item2.displayName + ", ", RenderHandler.ColorPalette.LightTurquoise);
				}
			}
		}
		richTextBuilder.AddLine();
		return richTextBuilder;
	}

	public List<StationFeature> GetAllBuildingFeature()
	{
		return feature.Where((StationFeature element) => element.spawnBuilding).ToList();
	}

	public List<StationFeature> GetAllNonBuildingFeature()
	{
		return feature.Where((StationFeature element) => !element.spawnBuilding).ToList();
	}

	public string GetArrivalEvent()
	{
		if (feature.Select((StationFeature e) => e.identifier).ToList().Contains("BLOCKADE"))
		{
			return "BLOCKADE";
		}
		if (feature.Select((StationFeature e) => e.identifier).ToList().Contains("FREE_PORT"))
		{
			return "FREE_PORT";
		}
		return "INSPECTION";
	}

	public bool HaveFeature(string featureId)
	{
		if (feature.Select((StationFeature e) => e.identifier).ToList().Contains(featureId))
		{
			return true;
		}
		return false;
	}

	public bool IsStationHaveCrisis()
	{
		foreach (StationFeature item in feature)
		{
			if (item.IsTag("IS_CRISIS_TAG"))
			{
				return true;
			}
		}
		return false;
	}
}
