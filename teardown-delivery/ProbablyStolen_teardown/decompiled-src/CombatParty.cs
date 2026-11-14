using System.Collections.Generic;
using UnityEngine;

public class CombatParty
{
	public List<CombatEntity> list { get; set; }

	public bool isPlayerTeam { get; set; }

	public CombatParty()
	{
		list = new List<CombatEntity>();
	}

	public bool IsPartyWiped()
	{
		if (list.Count == 0)
		{
			return true;
		}
		return false;
	}

	public bool IsPartyWipedLogic()
	{
		if (list.Count == 0)
		{
			return true;
		}
		foreach (GameCharacterItem item in list)
		{
			if (!item.isDeadLogic)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsPartyTurnFinished()
	{
		int num = 0;
		foreach (CombatEntity item in list)
		{
			num += item.action;
		}
		if (num == 0)
		{
			return true;
		}
		return false;
	}

	public CombatEntity GetNextPlayingEntity()
	{
		CombatEntity result = null;
		int num = int.MinValue;
		foreach (CombatEntity item in list)
		{
			if (item.action > 0 && item.reaction > num && !((GameCharacterItem)item).isDeadLogic)
			{
				result = item;
				num = item.reaction;
			}
		}
		return result;
	}

	public void AddMember(CombatEntity newMember)
	{
		if (isPlayerTeam)
		{
			newMember.isPlayerControlled = true;
		}
		else
		{
			newMember.isPlayerControlled = false;
		}
		list.Add(newMember);
	}

	public CombatEntity RemoveMember(CombatEntity elem)
	{
		if (list.Contains(elem))
		{
			list.Remove(elem);
			return elem;
		}
		return null;
	}

	public void ResetEntitiesAction()
	{
		foreach (CombatEntity item in list)
		{
			item.resetCurrentAction();
		}
	}

	public void AfterBattleCleanUp()
	{
		foreach (GameCharacterItem item in list)
		{
			item.CleanUpProficiency();
		}
	}

	public CombatEntity getPriorityTarget()
	{
		int num = 0;
		foreach (CombatEntity item in list)
		{
			num += item.priority;
		}
		int num2 = Random.Range(0, num);
		int num3 = 0;
		foreach (CombatEntity item2 in list)
		{
			if (num2 <= num3 + item2.priority)
			{
				return item2;
			}
			num3 += item2.priority;
		}
		return null;
	}

	public CombatEntity getLowestHealthEntity()
	{
		CombatEntity combatEntity = null;
		foreach (CombatEntity item in list)
		{
			if (combatEntity == null)
			{
				combatEntity = item;
			}
			else if (combatEntity.health > item.health)
			{
				combatEntity = item;
			}
		}
		return combatEntity;
	}
}
