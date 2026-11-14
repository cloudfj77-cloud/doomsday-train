using System.Collections.Generic;
using UnityEngine;

public class CombatMaster
{
	public CombatManager combatManager;

	private CombatEntity currentCombatTarget;

	public CombatMaster(List<CombatEntity> playerTeam, List<CombatEntity> enemyTeam)
	{
		combatManager = CombatManager.Instance;
		SetPlayerTeam(playerTeam);
		SetEnemyTeam(enemyTeam);
		Reset();
	}

	public CombatEntity WhichCurrentPlayer()
	{
		return combatManager.currentPlayingEntity;
	}

	public bool IsCombatDone()
	{
		if (combatManager.IsCombatDone() == 0 || combatManager.IsCombatDone() == 1)
		{
			return true;
		}
		return false;
	}

	public bool SetNextAction(CombatEntity caster, CombatAbility ability, CombatEntity target)
	{
		if (combatManager.currentPlayingEntity != caster)
		{
			return false;
		}
		CombatAbility currentSelectedAbility = caster.currentSelectedAbility;
		caster.currentSelectedAbility = ability;
		if (!combatManager.IsTargetValid(target))
		{
			caster.currentSelectedAbility = currentSelectedAbility;
			return false;
		}
		currentCombatTarget = target;
		return true;
	}

	public bool Next()
	{
		if (combatManager.currentPlayingEntity == null)
		{
			return false;
		}
		bool result = false;
		if (combatManager.currentPlayingEntity.isPlayerControlled && currentCombatTarget != null && !combatManager.currentPlayingEntity.IsEntityLostOfAgency())
		{
			combatManager.UseAbility(currentCombatTarget);
			result = true;
		}
		currentCombatTarget = null;
		while (!combatManager.currentPlayingEntity.isPlayerControlled || combatManager.currentPlayingEntity.IsEntityLostOfAgency())
		{
			if (combatManager.currentPlayingEntity.IsEntityLostOfAgency())
			{
				CombatEntity currentPlayingEntity = combatManager.currentPlayingEntity;
				bool flag = false;
				if (!flag)
				{
					flag = combatManager.HandleStunnedPlayingEntity(currentPlayingEntity);
				}
				if (!flag)
				{
					flag = combatManager.HandleLostofAgencyEntity(currentPlayingEntity);
				}
				result = true;
			}
			else
			{
				if (combatManager.partiesList[0].list.Count <= 0)
				{
					break;
				}
				combatManager.currentPlayingEntity.combatAI.PlayNextAction(combatManager, combatManager.currentPlayingEntity);
				result = true;
			}
			if (combatManager.IsCombatDone() > -1)
			{
				break;
			}
		}
		ActionQueue.current.Next();
		return result;
	}

	private void Reset()
	{
		CombatLog.log("Combat started, turn 0");
		combatManager.Reset();
	}

	private void SetPlayerTeam(List<CombatEntity> list)
	{
		CombatParty combatParty = new CombatParty();
		combatParty.isPlayerTeam = true;
		foreach (CombatEntity item in list)
		{
			combatParty.AddMember(item);
		}
		if (combatManager.partiesList.Count == 0)
		{
			combatManager.AddParty(combatParty);
		}
		else
		{
			combatManager.partiesList[0] = combatParty;
		}
	}

	private void SetEnemyTeam(List<CombatEntity> list)
	{
		CombatParty combatParty = new CombatParty();
		combatParty.isPlayerTeam = false;
		foreach (CombatEntity item in list)
		{
			combatParty.AddMember(item);
		}
		if (combatManager.partiesList.Count == 1)
		{
			combatManager.AddParty(combatParty);
			return;
		}
		if (combatManager.partiesList.Count == 0)
		{
			throw new UnityException("you need to add player team before adding enemy team to the combatManager!");
		}
		combatManager.partiesList[1] = combatParty;
	}
}
