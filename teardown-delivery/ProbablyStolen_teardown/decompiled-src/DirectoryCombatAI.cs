using System.Collections.Generic;
using UnityEngine;

public class DirectoryCombatAI
{
	private static DirectoryCombatAI instance = null;

	private static readonly object padlock = new object();

	public static DirectoryCombatAI Instance
	{
		get
		{
			lock (padlock)
			{
				if (instance == null)
				{
					instance = new DirectoryCombatAI();
				}
				return instance;
			}
		}
	}

	public CombatAI CreateMurderousAI()
	{
		return new CombatAI
		{
			CustomHandleTurn = CallBackMurderousAI
		};
	}

	public CombatAI CreateRaiderBossAI()
	{
		return new CombatAI
		{
			CustomHandleTurn = CallBackRaiderBossAI
		};
	}

	public CombatAI CreateOverwatchAI()
	{
		return new CombatAI
		{
			CustomHandleTurn = CallBackOverwatchAI
		};
	}

	public CombatAI CreateSadisticAI()
	{
		return new CombatAI
		{
			CustomHandleTurn = CallBackSadisticAI
		};
	}

	public CombatAI CreateJunkerAI()
	{
		return new CombatAI
		{
			CustomHandleTurn = CallBackJunkerAI
		};
	}

	public CombatAI CreateDeserterMarineAI()
	{
		return new CombatAI
		{
			CustomHandleTurn = CallBackDeserterMarineAI
		};
	}

	public CombatAI CreateDeserterCombatMedicAI()
	{
		return new CombatAI
		{
			CustomHandleTurn = CallBackDeserterCombatMedicAI
		};
	}

	public CombatAI CreateXenoHerealdAI()
	{
		return new CombatAI
		{
			CustomHandleTurn = CallBackXenoHeraldAI
		};
	}

	public CombatAI CreateXenoHunterAI()
	{
		return new CombatAI
		{
			CustomHandleTurn = CallBackXenoHunterAI
		};
	}

	public CombatAI CreateXenoBrewerAI()
	{
		return new CombatAI
		{
			CustomHandleTurn = CallBackXenoBrewerAI
		};
	}

	public CombatAI CreateXenoDroneAI()
	{
		return new CombatAI
		{
			CustomHandleTurn = CallBackXenoDroneAI
		};
	}

	public CombatAI CreatePirateRookieAI()
	{
		return new CombatAI
		{
			CustomHandleTurn = CallBackPirateRookieAI
		};
	}

	public CombatAI CreatePirateVeteranAI()
	{
		return new CombatAI
		{
			CustomHandleTurn = CallBackPirateVeteranAI
		};
	}

	public CombatAI CreatePirateChiefAI()
	{
		return new CombatAI
		{
			CustomHandleTurn = CallBackPirateChiefAI
		};
	}

	private void CallBackXenoHeraldAI(CombatManager combatManager, CombatEntity entity)
	{
		GameCharacterItem gameCharacterItem = (GameCharacterItem)entity;
		if (gameCharacterItem.health < 20 && combatManager.partiesList[entity.getTeam()].list.Count > 1)
		{
			List<CombatEntity> list = combatManager.partiesList[entity.getTeam()].list;
			CombatEntity combatEntity = null;
			foreach (CombatEntity item in list)
			{
				if (combatEntity == null && item != entity)
				{
					combatEntity = item;
				}
				if (((combatEntity != null) & (item.health < combatEntity.health)) && item != entity)
				{
					combatEntity = item;
				}
			}
			if (combatEntity == null || combatEntity == entity)
			{
				CombatEntity priorityTarget = CombatAIHelper.getPriorityTarget(combatManager, entity, getTargetFromOtherTeam: true);
				CombatAIHelper.AITryFindAndUse(gameCharacterItem, combatManager, "xeno_scratch_item_ability", priorityTarget, isConsumeItem: false);
			}
			else
			{
				CombatAIHelper.AITryFindAndUse(gameCharacterItem, combatManager, "xeno_consume_item_ability", combatEntity, isConsumeItem: false);
			}
			return;
		}
		switch (CombatAIHelper.CheckStage(gameCharacterItem))
		{
		case 1:
			CombatAIHelper.AITryFindAndUse(gameCharacterItem, combatManager, "xeno_warcry_item_ability", gameCharacterItem, isConsumeItem: false);
			return;
		case 2:
		{
			CombatEntity priorityTarget2 = CombatAIHelper.getPriorityTarget(combatManager, entity, getTargetFromOtherTeam: true);
			CombatAIHelper.AITryFindAndUse(gameCharacterItem, combatManager, "xeno_mark_item_ability", priorityTarget2, isConsumeItem: false);
			return;
		}
		case 3:
			if (combatManager.partiesList[entity.getTeam()].list.Count > 1)
			{
				List<CombatEntity> teamListExceptSelf = CombatAIHelper.GetTeamListExceptSelf(gameCharacterItem, combatManager);
				int randomInt = RNG.GetRandomInt(0, teamListExceptSelf.Count - 1);
				CombatAIHelper.AITryFindAndUse(gameCharacterItem, combatManager, "xeno_empower_item_ability", teamListExceptSelf[randomInt], isConsumeItem: false);
			}
			else
			{
				CombatEntity priorityTarget3 = CombatAIHelper.getPriorityTarget(combatManager, entity, getTargetFromOtherTeam: true);
				CombatAIHelper.AITryFindAndUse(gameCharacterItem, combatManager, "xeno_scratch_item_ability", priorityTarget3, isConsumeItem: false);
			}
			return;
		}
		if (combatManager.partiesList[entity.getTeam()].list.Count > 1)
		{
			List<CombatEntity> teamListExceptSelf2 = CombatAIHelper.GetTeamListExceptSelf(gameCharacterItem, combatManager);
			int randomInt2 = RNG.GetRandomInt(0, teamListExceptSelf2.Count - 1);
			CombatAIHelper.AITryFindAndUse(gameCharacterItem, combatManager, "xeno_empower_item_ability", teamListExceptSelf2[randomInt2], isConsumeItem: false);
		}
		else
		{
			CombatEntity priorityTarget4 = CombatAIHelper.getPriorityTarget(combatManager, entity, getTargetFromOtherTeam: true);
			CombatAIHelper.AITryFindAndUse(gameCharacterItem, combatManager, "xeno_scratch_item_ability", priorityTarget4, isConsumeItem: false);
		}
		CombatAIHelper.ResetStage(gameCharacterItem);
	}

	private void CallBackXenoHunterAI(CombatManager combatManager, CombatEntity entity)
	{
		GameCharacterItem gameCharacterItem = (GameCharacterItem)entity;
		CombatAIHelper.CheckStage(gameCharacterItem);
		if (gameCharacterItem.HaveEffect("xeno_empower_effect"))
		{
			CombatEntity priorityTarget = CombatAIHelper.getPriorityTarget(combatManager, entity, getTargetFromOtherTeam: true);
			CombatAIHelper.AITryFindAndUse(gameCharacterItem, combatManager, "xeno_knockdown_item_ability", priorityTarget, isConsumeItem: false);
		}
		else if (!gameCharacterItem.HaveEffect("xeno_prey_acc_buff"))
		{
			CombatAIHelper.AITryFindAndUse(gameCharacterItem, combatManager, "xeno_prey_item_ability", gameCharacterItem, isConsumeItem: false);
		}
		else
		{
			CombatEntity priorityTarget2 = CombatAIHelper.getPriorityTarget(combatManager, entity, getTargetFromOtherTeam: true);
			CombatAIHelper.AITryFindAndUse(gameCharacterItem, combatManager, "xeno_laceration_item_ability", priorityTarget2, isConsumeItem: false);
		}
	}

	private void CallBackXenoBrewerAI(CombatManager combatManager, CombatEntity entity)
	{
		GameCharacterItem gameCharacterItem = (GameCharacterItem)entity;
		if (gameCharacterItem.HaveEffect("xeno_empower_effect"))
		{
			CombatEntity target = CombatAIHelper.FindMostWounded(combatManager, entity);
			CombatAIHelper.AITryFindAndUse(gameCharacterItem, combatManager, "xeno_healing_spray_item_ability", target, isConsumeItem: false);
		}
		else if (gameCharacterItem.health < 30)
		{
			CombatAIHelper.AITryFindAndUse(gameCharacterItem, combatManager, "brace_item_ability", gameCharacterItem, isConsumeItem: false);
		}
		else if (RNG.Roll(25))
		{
			CombatEntity priorityTarget = CombatAIHelper.getPriorityTarget(combatManager, entity, getTargetFromOtherTeam: true);
			CombatAIHelper.AITryFindAndUse(gameCharacterItem, combatManager, "xeno_irritant_item_ability", priorityTarget, isConsumeItem: false);
		}
		else
		{
			CombatEntity priorityTarget2 = CombatAIHelper.getPriorityTarget(combatManager, entity, getTargetFromOtherTeam: true);
			CombatAIHelper.AITryFindAndUse(gameCharacterItem, combatManager, "xeno_toxin_item_ability", priorityTarget2, isConsumeItem: false);
		}
	}

	private void CallBackXenoDroneAI(CombatManager combatManager, CombatEntity entity)
	{
		GameCharacterItem gameCharacterItem = (GameCharacterItem)entity;
		if (gameCharacterItem.HaveEffect("xeno_empower_effect"))
		{
			CombatEntity priorityTarget = CombatAIHelper.getPriorityTarget(combatManager, entity, getTargetFromOtherTeam: true);
			CombatAIHelper.AITryFindAndUse(gameCharacterItem, combatManager, "xeno_laceration_item_ability", priorityTarget, isConsumeItem: false);
		}
		else
		{
			CombatEntity priorityTarget2 = CombatAIHelper.getPriorityTarget(combatManager, entity, getTargetFromOtherTeam: true);
			CombatAIHelper.AITryFindAndUse(gameCharacterItem, combatManager, "xeno_scratch_item_ability", priorityTarget2, isConsumeItem: false);
		}
	}

	private void CallBackMurderousAI(CombatManager combatManager, CombatEntity entity)
	{
		combatManager.ChangePlayingEntityAbility(entity.GetEntityWeaponAbility());
		combatManager.UseAbility(combatManager.partiesList[0].getLowestHealthEntity());
	}

	private void CallBackRaiderBossAI(CombatManager combatManager, CombatEntity entity)
	{
		combatManager.ChangePlayingEntityAbility(entity.GetEntityWeaponAbility());
		combatManager.UseAbility(combatManager.partiesList[0].getLowestHealthEntity());
	}

	private void CallBackOverwatchAI(CombatManager combatManager, CombatEntity entity)
	{
		CombatAbility combatAbility = DirectoryMaster.CombatAbility("overwatch");
		combatManager.ChangePlayingEntityAbility(combatAbility);
		combatManager.UseAbility(entity);
	}

	private void CallBackJunkerAI(CombatManager combatManager, CombatEntity entity)
	{
		int num = Random.Range(1, 3);
		GameCharacterItem gameCharacterItem = (GameCharacterItem)entity;
		if (((GameCharacterItem)entity).IsTag("ai_stage"))
		{
			int tagInt = ((GameCharacterItem)entity).GetTagInt("ai_stage");
			((GameCharacterItem)entity).SetTagInt("ai_stage", tagInt + 1);
		}
		else
		{
			((GameCharacterItem)entity).EnableTag("ai_stage");
			((GameCharacterItem)entity).SetTagInt("ai_stage", 1);
		}
		if (((GameCharacterItem)entity).GetTagInt("ai_stage") == 1)
		{
			GameItem gameItem = GraphUtils.FindChildType(gameCharacterItem, (GameItem item) => item.identifier == "bicarsole_pill");
			if (gameItem != null)
			{
				combatManager.ChangePlayingEntityAbility(gameItem);
				combatManager.UseAbility(gameCharacterItem);
				gameItem.parentInventory?.Expel(gameItem);
			}
			else
			{
				combatManager.ChangePlayingEntityAbility(entity.GetEntityWeaponAbility());
				combatManager.UseAbility(combatManager.partiesList[0].getPriorityTarget());
			}
			return;
		}
		switch (num)
		{
		case 1:
		{
			CombatAbility combatAbility = DirectoryMaster.CombatAbility("retaliate");
			combatManager.ChangePlayingEntityAbility(combatAbility);
			combatManager.UseAbility(entity);
			break;
		}
		case 2:
			combatManager.ChangePlayingEntityAbility(entity.GetEntityWeaponAbility());
			combatManager.UseAbility(combatManager.partiesList[0].getPriorityTarget());
			break;
		}
	}

	private void CallBackPirateRookieAI(CombatManager combatManager, CombatEntity entity)
	{
		GameCharacterItem gameCharacterItem = (GameCharacterItem)entity;
		GameItem gameItem = null;
		gameItem = GraphUtils.FindChildType(gameCharacterItem, (GameItem item) => item.identifier == "green_injector" || item.identifier == "red_injector" || item.identifier == "blue_injector");
		if (gameItem != null && entity.health <= 45)
		{
			combatManager.ChangePlayingEntityAbility(gameItem);
			combatManager.UseAbility(gameCharacterItem);
			gameItem.parentInventory?.Expel(gameItem);
			return;
		}
		gameItem = GraphUtils.FindChildType(gameCharacterItem, (GameItem item) => item.identifier == "turquoise_injector");
		if (gameItem != null)
		{
			combatManager.ChangePlayingEntityAbility(gameItem);
			combatManager.UseAbility(gameCharacterItem);
			gameItem.parentInventory?.Expel(gameItem);
			return;
		}
		gameItem = GraphUtils.FindChildType(gameCharacterItem, (GameItem item) => item.identifier == "overwatch");
		if (RNG.Roll(75) && gameItem != null)
		{
			combatManager.ChangePlayingEntityAbility(gameItem);
			combatManager.UseAbility(entity);
			return;
		}
		gameItem = GraphUtils.FindChildType(gameCharacterItem, (GameItem item) => item.identifier == "smoke_grenade" || item.identifier == "flashbang_grenade" || item.identifier == "frag_grenade");
		if (RNG.Roll(40) && gameItem != null)
		{
			combatManager.ChangePlayingEntityAbility(gameItem);
			combatManager.UseAbility(combatManager.partiesList[0].getPriorityTarget());
			gameItem.parentInventory?.Expel(gameItem);
		}
		else
		{
			CombatAbility entityWeaponAbility = entity.GetEntityWeaponAbility();
			combatManager.ChangePlayingEntityAbility(entityWeaponAbility);
			combatManager.UseAbility(combatManager.partiesList[0].getPriorityTarget());
		}
	}

	private void CallBackPirateVeteranAI(CombatManager combatManager, CombatEntity entity)
	{
		_ = (GameCharacterItem)entity;
	}

	private void CallBackPirateChiefAI(CombatManager combatManager, CombatEntity entity)
	{
		_ = (GameCharacterItem)entity;
	}

	private void CallBackDeserterMarineAI(CombatManager combatManager, CombatEntity entity)
	{
		GameCharacterItem currentNode = (GameCharacterItem)entity;
		CombatAbility entityWeaponAbility = entity.GetEntityWeaponAbility();
		combatManager.ChangePlayingEntityAbility(entityWeaponAbility);
		string[] listEffect = new string[2] { "overwatch_effect", "retaliate_effect" };
		List<GameCharacterItem> list = CombatAIHelper.FindTargetWithEffectActive(combatManager, entity, listEffect, getTargetFromOtherTeam: true);
		GameItem gameItem = GraphUtils.FindChildType(currentNode, (GameItem item) => item.identifier == "smoke_grenade" || item.identifier == "flashbang_grenade" || item.identifier == "frag_grenade");
		if (list.Count != 0 && gameItem != null)
		{
			combatManager.ChangePlayingEntityAbility(gameItem);
			combatManager.UseAbility(CombatAIHelper.getPriorityTarget(list));
			gameItem.parentInventory?.Expel(gameItem);
			return;
		}
		gameItem = GraphUtils.FindChildType(currentNode, (GameItem item) => item.identifier == "smoke_grenade" || item.identifier == "flashbang_grenade" || item.identifier == "frag_grenade");
		if (RNG.Roll(50) && gameItem != null && entity.health < 45)
		{
			combatManager.ChangePlayingEntityAbility(gameItem);
			combatManager.UseAbility(combatManager.partiesList[0].getPriorityTarget());
			gameItem.parentInventory?.Expel(gameItem);
			return;
		}
		gameItem = GraphUtils.FindChildType(currentNode, (GameItem item) => item.identifier == "overwatch");
		if (RNG.Roll(20) && gameItem != null)
		{
			combatManager.ChangePlayingEntityAbility(gameItem);
			combatManager.UseAbility(entity);
			return;
		}
		gameItem = GraphUtils.FindChildType(currentNode, (GameItem item) => item.identifier == "mark_item_ability");
		if (RNG.Roll(20) && gameItem != null)
		{
			CombatEntity priorityTarget = combatManager.partiesList[0].getPriorityTarget();
			if (priorityTarget.HaveEffect("mark_dodge_debuff"))
			{
				combatManager.UseAbility(combatManager.partiesList[0].getPriorityTarget());
				return;
			}
			combatManager.ChangePlayingEntityAbility(gameItem);
			combatManager.UseAbility(priorityTarget);
		}
		else
		{
			combatManager.UseAbility(combatManager.partiesList[0].getPriorityTarget());
		}
	}

	private void CallBackDeserterCombatMedicAI(CombatManager combatManager, CombatEntity entity)
	{
		GameCharacterItem currentNode = (GameCharacterItem)entity;
		CombatAbility entityWeaponAbility = entity.GetEntityWeaponAbility();
		combatManager.ChangePlayingEntityAbility(entityWeaponAbility);
		GameCharacterItem gameCharacterItem = CombatAIHelper.FindMostWounded(combatManager, entity);
		GameItem gameItem = GraphUtils.FindChildType(currentNode, (GameItem item) => item.identifier == "green_injector" || item.identifier == "red_injector");
		if (gameItem != null && gameCharacterItem != null)
		{
			combatManager.ChangePlayingEntityAbility(gameItem);
			combatManager.UseAbility(gameCharacterItem);
			gameItem.parentInventory?.Expel(gameItem);
			return;
		}
		gameItem = GraphUtils.FindChildType(currentNode, (GameItem item) => item.identifier == "turquoise_injector");
		if (RNG.Roll(33) && gameItem != null)
		{
			combatManager.ChangePlayingEntityAbility(gameItem);
			combatManager.UseAbility(CombatAIHelper.getPriorityTarget(combatManager, entity, getTargetFromOtherTeam: false));
			gameItem.parentInventory?.Expel(gameItem);
			return;
		}
		gameItem = GraphUtils.FindChildType(currentNode, (GameItem item) => item.identifier == "bandage_item");
		string[] listEffect = new string[2] { "bleed_light", "bleed_heavy" };
		List<GameCharacterItem> list = CombatAIHelper.FindTargetWithEffectActive(combatManager, entity, listEffect, getTargetFromOtherTeam: false);
		if (RNG.Roll(75) && gameItem != null && list.Count != 0)
		{
			combatManager.ChangePlayingEntityAbility(gameItem);
			combatManager.UseAbility(CombatAIHelper.getPriorityTarget(list));
			gameItem.parentInventory?.Expel(gameItem);
		}
		else
		{
			entityWeaponAbility = entity.GetEntityWeaponAbility();
			combatManager.ChangePlayingEntityAbility(entityWeaponAbility);
			combatManager.UseAbility(combatManager.partiesList[0].getPriorityTarget());
		}
	}

	private void CallBackSadisticAI(CombatManager combatManager, CombatEntity entity)
	{
		CombatAbility entityWeaponAbility = entity.GetEntityWeaponAbility();
		combatManager.ChangePlayingEntityAbility(entityWeaponAbility);
		int adverseTeam = entity.getAdverseTeam();
		bool flag = false;
		foreach (CombatEntity item in combatManager.partiesList[adverseTeam].list)
		{
			if (!item.IsBleeding())
			{
				flag = true;
				combatManager.UseAbility(item);
				break;
			}
		}
		if (!flag)
		{
			combatManager.UseAbility(combatManager.partiesList[adverseTeam].getLowestHealthEntity());
		}
	}
}
