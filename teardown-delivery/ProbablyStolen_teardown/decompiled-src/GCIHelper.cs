using System.Collections.Generic;

public class GCIHelper
{
	public const int NUM_TURN_PASSED_ON_CYCLE = 40;

	public static void OnNextInBaseTurn(GameCharacterItem GCI)
	{
		for (int i = 0; i < 40; i++)
		{
			if (!GCI.isDead)
			{
				GCI.UpdateCombatEffect();
				Survival.UpdateSurvival(GCI, isInBase: true);
				GCI.EntityTurnDone();
			}
		}
		ActionQueue.current.Next();
	}

	public static void OnNextInBaseTurnAllGCI()
	{
		foreach (GameCharacterItem item in FindAllInBaseGCI())
		{
			OnNextInBaseTurn(item);
			item.Validate();
		}
	}

	public static List<GameCharacterItem> FindAllInBaseGCI()
	{
		List<GameItem> list = GraphUtils.FindAllChildrenType<GameItem>(GameMaster.current.shipManager.ship);
		List<GameCharacterItem> list2 = new List<GameCharacterItem>();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num].IsTag("humanoid"))
			{
				list2.Add((GameCharacterItem)list[num]);
			}
		}
		return list2;
	}

	public static void BasicFullHeal(GameCharacterItem gci)
	{
		int num = gci.maxHealth - gci.health;
		if (num > 0)
		{
			gci.HealEntity(num);
		}
	}

	public static void Bandage(GameCharacterItem gci)
	{
		gci.RemoveEffectByID("bleed_light");
		gci.RemoveEffectByID("bleed_heavy");
	}
}
