using System;

public class MiningHelper
{
	public const int RARE_MINERAL_SPAWN_CHANCE = 8;

	public const int ACCIDENT_RATE = 20;

	public static (int, int, int) MineAsteroid(int timeMined, int mineralLevel, int miningPower)
	{
		int num = 0;
		int num2 = 0;
		int num3 = miningPower;
		double num4 = (double)mineralLevel + 2.0 - Math.Log10((double)timeMined + 1.0) * 3.0 + 1.0;
		for (int i = 0; i < (int)num4; i++)
		{
			miningPower = num3;
			while (miningPower > 100)
			{
				miningPower -= 100;
				(num, num2) = SpawnOre(num, num2);
			}
			if (RNG.Roll(miningPower))
			{
				(num, num2) = SpawnOre(num, num2);
			}
		}
		return (CheckAccident(timeMined), num, num2);
	}

	public static (int, int) SpawnOre(int normalOre, int rareOre)
	{
		if (RNG.Roll(8))
		{
			return (normalOre, rareOre + 1);
		}
		return (normalOre + 1, rareOre);
	}

	public static int CheckAccident(int timeMined)
	{
		if (timeMined * 20 >= 100)
		{
			return 20;
		}
		if (RNG.Roll(timeMined * 20))
		{
			return timeMined * 2;
		}
		return 0;
	}

	public static void AddMinedOre(GameItem container, int normalOre, int rareOre)
	{
		for (int i = 0; i < normalOre; i++)
		{
			GameItem item = DirectoryMaster.Item("common_ore");
			GraphUtils.TryAcceptAll(container, item);
		}
		for (int j = 0; j < rareOre; j++)
		{
			GameItem item = DirectoryMaster.Item("rare_ore");
			GraphUtils.TryAcceptAll(container, item);
		}
	}

	public static string GetDangerLevel(int timeMined)
	{
		if (timeMined < 2)
		{
			return "low risk of accident";
		}
		if (timeMined == 2)
		{
			return "significant risk of accident";
		}
		if (timeMined <= 4)
		{
			return "high risk of accident";
		}
		return "garanteed accident";
	}
}
