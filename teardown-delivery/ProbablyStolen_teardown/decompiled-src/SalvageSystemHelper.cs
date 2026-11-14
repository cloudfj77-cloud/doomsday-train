public class SalvageSystemHelper
{
	public const int DEBRIS_RISE_RATE = 20;

	public static (int, int) SalvageWreck(GameItem container, int debrisLevel, int salvagePower, int wreckState, int shipSize)
	{
		string[] ids;
		int[] oddArray;
		switch (wreckState)
		{
		case 0:
			ids = new string[4] { "scrap_metal", "steel_sheet", "wire", "nuts_metal" };
			oddArray = new int[4] { 10, 2, 4, 3 };
			break;
		case 1:
			ids = new string[8] { "scrap_metal", "steel_sheet", "wire", "nuts_metal", "exotic_fuel", "energy_credit", "common_electronic", "rare_electronic" };
			oddArray = new int[12]
			{
				6, 16, 4, 4, 1, 1, 1, 1, 1, 1,
				3, 1
			};
			break;
		default:
			ids = new string[8] { "scrap_metal", "steel_sheet", "wire", "nuts_metal", "exotic_fuel", "energy_credit", "common_electronic", "rare_electronic" };
			oddArray = new int[12]
			{
				1, 8, 3, 3, 1, 1, 1, 1, 1, 1,
				5, 2
			};
			break;
		}
		int num = (shipSize + 1) * salvagePower;
		int item = 0;
		if (RNG.Roll(debrisLevel))
		{
			item = debrisLevel / 4;
		}
		if (debrisLevel < 100)
		{
			debrisLevel += 20;
		}
		while (num > 100)
		{
			num -= 100;
			GameItem item2 = DirectoryMaster.Item(RNG.GetOddString(oddArray, ids));
			GraphUtils.TryAcceptAll(container, item2);
		}
		if (RNG.Roll(num))
		{
			GameItem item3 = DirectoryMaster.Item(RNG.GetOddString(oddArray, ids));
			GraphUtils.TryAcceptAll(container, item3);
		}
		return (item, debrisLevel);
	}
}
