using System.Collections.Generic;

public class StationMachinerieHelper
{
	public const int ML_PER_ENERGY = 100;

	public const int NUTIRION_PASTE_COST = 10;

	public static void WaterFountainUpdateFillButton(ButtonElement button, GameSlotInventory bottleInv, GameSlotInventory batteryInv)
	{
		List<GameItem> list = GraphUtils.FindAllChildrenType(bottleInv, (GameItem item) => item.IsTag("LIQUID_CONTAINER_TAG"));
		List<GameItem> list2 = GraphUtils.FindAllChildrenType(batteryInv, (GameItem item) => item.IsTag("power_source_item"));
		if (list.Count != 0 && list2.Count != 0)
		{
			int @int = list2[0].GetTagReadonly("power_source_item_energy").GetInt();
			int num = list[0].GetTagReadonly("LIQUID_CONTAINER_MAX").GetInt() - list[0].GetTagReadonly("LIQUID_CONTAINER_CURRENT").GetInt();
			if (100 * @int > num)
			{
				button.SetColor(ButtonElement.Color.Green);
			}
			else
			{
				button.SetColor(ButtonElement.Color.Orange);
			}
		}
	}

	public static void WaterFountainUpdateDrinkButton(ButtonElement button, GameSlotInventory batteryInv)
	{
		List<GameCharacterItem> allPlayerCharacters = GameMaster.current.raidManager.GetAllPlayerCharacters();
		List<GameItem> list = GraphUtils.FindAllChildrenType(batteryInv, (GameItem item) => item.IsTag("power_source_item"));
		if (allPlayerCharacters.Count == 0 || list.Count == 0)
		{
			return;
		}
		int num = 0;
		foreach (GameCharacterItem item in allPlayerCharacters)
		{
			num += item.maxThirst - item.currentThirst;
		}
		int @int = list[0].GetTagReadonly("power_source_item_energy").GetInt();
		if (num < @int * 100)
		{
			button.SetColor(ButtonElement.Color.Green);
		}
		else
		{
			button.SetColor(ButtonElement.Color.Orange);
		}
	}
}
