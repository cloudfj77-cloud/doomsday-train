public class PreBuildChemHelper
{
	public static string[] impurityTypes = new string[4] { "chemical_contaminant", "physical_contaminant", "microbe", "heavy_metal" };

	public static void SetImpurity(GameItem gameItem, int minImpurity, int maxImpurity)
	{
		if (!gameItem.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return;
		}
		int randomInt = RNG.GetRandomInt(minImpurity, maxImpurity);
		int num = (int)((long)WaterHelper.GetCurrentCapacityUL(gameItem) * (long)randomInt / 10000);
		WaterHelper.Remove(gameItem, num);
		int num2 = num;
		for (int i = 0; i < impurityTypes.Length; i++)
		{
			if (i == impurityTypes.Length - 1)
			{
				WaterHelper.AddLiquid(gameItem, impurityTypes[i], num2);
				continue;
			}
			int randomInt2 = RNG.GetRandomInt(0, num2);
			WaterHelper.AddLiquid(gameItem, impurityTypes[i], randomInt2);
			num2 -= randomInt2;
		}
	}

	public static GameItem CreateFantaPrecusorBottle()
	{
		GameItem gameItem = DirectoryMaster.Item("med_bottle_violet");
		WaterHelper.AddLiquid(gameItem, "methylol", 125000);
		WaterHelper.AddLiquid(gameItem, "stabilizing_agent", 125000);
		SetImpurity(gameItem, 200, 800);
		AudioHelper.InitPlasticContainer(gameItem);
		return gameItem;
	}

	public static GameItem CreateAcidBottle()
	{
		GameItem gameItem = DirectoryMaster.Item("med_bottle_red");
		WaterHelper.AddLiquid(gameItem, "acid", 250000);
		AudioHelper.InitPlasticContainer(gameItem);
		SetImpurity(gameItem, 200, 800);
		return gameItem;
	}

	public static GameItem CreateBaseBottle()
	{
		GameItem gameItem = DirectoryMaster.Item("med_bottle_blue");
		WaterHelper.AddLiquid(gameItem, "base", 250000);
		AudioHelper.InitPlasticContainer(gameItem);
		SetImpurity(gameItem, 200, 800);
		return gameItem;
	}

	public static GameItem CreateReducingAgentBottle()
	{
		GameItem gameItem = DirectoryMaster.Item("med_bottle_orange");
		WaterHelper.AddLiquid(gameItem, "reducing_agent", 250000);
		SetImpurity(gameItem, 200, 800);
		AudioHelper.InitPlasticContainer(gameItem);
		return gameItem;
	}

	public static GameItem CreateLipobondAgentBottle()
	{
		GameItem gameItem = DirectoryMaster.Item("med_bottle_small_pink");
		WaterHelper.AddLiquid(gameItem, "lipobond_agent", 150000);
		AudioHelper.InitPlasticContainer(gameItem);
		SetImpurity(gameItem, 200, 800);
		return gameItem;
	}

	public static GameItem CreateMiniAlcoholBottle()
	{
		GameItem gameItem = DirectoryMaster.Item("mini_bottle");
		gameItem.name = LocHelper.GetLocalizedOrDefault("Item", "item_mini_alcohol_bottle_name", "Mini Alcohol Bottle");
		WaterHelper.AddLiquid(gameItem, "alcohol", 175000);
		WaterHelper.AddLiquid(gameItem, "water", 75000);
		return gameItem;
	}
}
