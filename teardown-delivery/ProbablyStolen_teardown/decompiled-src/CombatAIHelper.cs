using System.Collections.Generic;
using UnityEngine;

public class CombatAIHelper
{
	public static string GetOneFromList(string[] list)
	{
		return list[Random.Range(0, list.Length)];
	}

	public static List<GameCharacterItem> FindTargetWithEffectActive(CombatManager combatManager, CombatEntity entity, string[] listEffect, bool getTargetFromOtherTeam)
	{
		int index = entity.getTeam();
		if (getTargetFromOtherTeam)
		{
			index = entity.getAdverseTeam();
		}
		List<CombatEntity> list = combatManager.partiesList[index].list;
		List<GameCharacterItem> list2 = new List<GameCharacterItem>();
		foreach (GameCharacterItem item in list)
		{
			if (item.HaveEffects(listEffect))
			{
				list2.Add(item);
			}
		}
		return list2;
	}

	public static GameCharacterItem FindMostWounded(CombatManager combatManager, CombatEntity entity)
	{
		int team = entity.getTeam();
		List<CombatEntity> list = combatManager.partiesList[team].list;
		GameCharacterItem gameCharacterItem = (GameCharacterItem)list[0];
		foreach (CombatEntity item in list)
		{
			if (item.health < gameCharacterItem.health)
			{
				gameCharacterItem = (GameCharacterItem)item;
			}
		}
		if (gameCharacterItem.health < 65)
		{
			return gameCharacterItem;
		}
		return null;
	}

	public static CombatEntity getPriorityTarget(List<GameCharacterItem> list)
	{
		int num = 0;
		foreach (GameCharacterItem item in list)
		{
			num += ((CombatEntity)item).priority;
		}
		int num2 = Random.Range(0, num);
		int num3 = 0;
		foreach (GameCharacterItem item2 in list)
		{
			if (num2 <= num3 + ((CombatEntity)item2).priority)
			{
				return item2;
			}
			num3 += ((CombatEntity)item2).priority;
		}
		return null;
	}

	public static CombatEntity getPriorityTarget(CombatManager combatManager, CombatEntity entity, bool getTargetFromOtherTeam)
	{
		if (getTargetFromOtherTeam)
		{
			return combatManager.partiesList[entity.getAdverseTeam()].getPriorityTarget();
		}
		return combatManager.partiesList[entity.getTeam()].getPriorityTarget();
	}

	public static int CheckStage(GameCharacterItem gci)
	{
		int num;
		if (gci.IsTag("ai_stage"))
		{
			num = gci.GetTagInt("ai_stage") + 1;
			gci.SetTagInt("ai_stage", num);
		}
		else
		{
			gci.EnableTag("ai_stage");
			gci.SetTagInt("ai_stage", 1);
			num = 1;
		}
		return num;
	}

	public static void ResetStage(GameCharacterItem gci)
	{
		int value = 0;
		if (gci.IsTag("ai_stage"))
		{
			gci.SetTagInt("ai_stage", value);
			return;
		}
		gci.EnableTag("ai_stage");
		gci.SetTagInt("ai_stage", value);
	}

	public static bool AITryFindAndUse(GameCharacterItem gci, CombatManager combatManager, string identifier, CombatEntity target, bool isConsumeItem)
	{
		GameItem gameItem = GraphUtils.FindChildType(gci, (GameItem item) => item.identifier == identifier);
		if (gameItem == null)
		{
			return false;
		}
		combatManager.ChangePlayingEntityAbility(gameItem);
		combatManager.UseAbility(target);
		if (isConsumeItem)
		{
			gameItem.parentInventory?.Expel(gameItem);
		}
		return true;
	}

	public static List<CombatEntity> GetTeamListExceptSelf(GameCharacterItem gci, CombatManager combatManager)
	{
		List<CombatEntity> list = combatManager.partiesList[gci.getTeam()].list;
		List<CombatEntity> list2 = new List<CombatEntity>();
		foreach (CombatEntity item in list)
		{
			if ((GameCharacterItem)item != gci)
			{
				list2.Add(item);
			}
		}
		return list2;
	}

	public static bool UseItemOnTarget(CombatManager combatManager, GameItem gameItem, GameCharacterItem target, bool expel)
	{
		combatManager.ChangePlayingEntityAbility(gameItem);
		combatManager.UseAbility(target);
		if (expel)
		{
			gameItem.parentInventory?.Expel(gameItem);
		}
		return true;
	}
}
