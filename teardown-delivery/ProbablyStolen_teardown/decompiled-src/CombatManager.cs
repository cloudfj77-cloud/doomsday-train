using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager
{
	public const float ANIMATION_DELAY_NONE = 0f;

	public const float ANIMATION_DELAY = 0.3f;

	public const float ANIMATION_DELAY_LONG = 0.5f;

	public const float ANIMATION_DELAY_VERY_LONG = 0.7f;

	public const float ANIMATION_DELAY_SHORT = 0.2f;

	public const float ANIMATION_DELAY_VERY_SHORT = 0.1f;

	public CombatManagerMono combatManagerMono;

	public System.Random random;

	public CombatEntity currentPlayingEntity;

	public CombatEventHandler combatEventHandler;

	private static CombatManager instance = null;

	private static readonly object padlock = new object();

	public List<CombatParty> partiesList { get; set; }

	public int currentTurn { get; set; }

	public static CombatManager Instance
	{
		get
		{
			lock (padlock)
			{
				if (instance == null)
				{
					instance = new CombatManager();
				}
				return instance;
			}
		}
	}

	public CombatManager()
	{
		currentTurn = 0;
		partiesList = new List<CombatParty>();
		random = new System.Random();
		combatEventHandler = new CombatEventHandler();
	}

	public static float GetActionQueueDelay()
	{
		if (GameMaster.current.turnManager.isProcessingTurn)
		{
			return 0f;
		}
		return 0.5f;
	}

	public bool IsTurnDone()
	{
		foreach (CombatParty parties in partiesList)
		{
			if (!parties.IsPartyTurnFinished())
			{
				return false;
			}
		}
		return true;
	}

	public void AddParty(CombatParty party)
	{
		partiesList.Add(party);
	}

	public CombatEntity GetNextPlayingEntity()
	{
		List<CombatEntity> list = new List<CombatEntity>();
		foreach (CombatParty parties in partiesList)
		{
			if (!parties.IsPartyTurnFinished())
			{
				CombatEntity nextPlayingEntity = parties.GetNextPlayingEntity();
				if (nextPlayingEntity != null)
				{
					list.Add(nextPlayingEntity);
				}
			}
		}
		if (list.Count == 1)
		{
			return list[0];
		}
		if (list.Count == 2)
		{
			if (list[0].reaction > list[1].reaction)
			{
				return list[0];
			}
			if (list[0].reaction < list[1].reaction)
			{
				return list[1];
			}
			return list[random.Next(0, 1)];
		}
		Debug.Log("can't find next playing entity");
		return null;
	}

	public void SetPlayingEntity(CombatEntity entity)
	{
		ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
		{
			CombatLog.log("―" + entity.name + "'s Turn to play―");
			SplashTextManager.Instance.DisplayFloatText(entity, entity.name + "'s Turn", SplashTextManager.TextMode.Heal);
		}));
		currentPlayingEntity = entity;
	}

	public int IsCombatDone()
	{
		if (partiesList[0].IsPartyWiped())
		{
			return 1;
		}
		if (partiesList[1].IsPartyWiped())
		{
			return 0;
		}
		return -1;
	}

	public int IsCombatDoneLogic()
	{
		if (partiesList[0].IsPartyWipedLogic())
		{
			return 1;
		}
		if (partiesList[1].IsPartyWipedLogic())
		{
			return 0;
		}
		return -1;
	}

	public void StartNewTurn()
	{
		currentTurn++;
		partiesList[0].ResetEntitiesAction();
		partiesList[1].ResetEntitiesAction();
		ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
		{
			CombatLog.log("*Turn " + currentTurn + " Started*");
			SplashTextManager.Instance.DisplayFloatText("*Turn " + currentTurn + " Started*", SplashTextManager.TextMode.Heal);
		}));
		combatEventHandler.HandleTurnStartEvents(this);
		CombatEntity nextPlayingEntity = GetNextPlayingEntity();
		SetPlayingEntity(nextPlayingEntity);
		if (currentPlayingEntity.OnTurnStart())
		{
			ManageTurn();
		}
	}

	public void UseAbility(CombatEntity target)
	{
		List<CombatEntity> targetEntityUniqueList = CombatEntity.HandleAbilityTarget(currentPlayingEntity, target, partiesList[0].list, partiesList[1].list);
		CheckOverwatch(currentPlayingEntity);
		AppliedEffectsOnTargets(targetEntityUniqueList, currentPlayingEntity.currentSelectedAbility, currentPlayingEntity);
		if (CheckActionFreePassive(currentPlayingEntity, currentPlayingEntity.currentSelectedAbility))
		{
			((GameCharacterItem)currentPlayingEntity).EnableTag("action_free_mode");
			ActionQueue.current.Add(new EmptyAction(0.20000000298023224).SetBeginFunc(delegate
			{
				CombatLog.log(target.name + "'s proficiency allow them to perform this action without ending their turn!");
				SplashTextManager.Instance.DisplayFloatText(target, "Proficiency", SplashTextManager.TextMode.Heal);
			}));
		}
		else
		{
			currentPlayingEntity.action--;
			currentPlayingEntity.EntityTurnDone();
			((GameCharacterItem)currentPlayingEntity).DisableTag("action_free_mode");
		}
		ManageTurn();
	}

	public bool CheckActionFreePassive(CombatEntity caster, CombatAbility ability)
	{
		if (((GameItem)ability).IsTag("proficiency"))
		{
			string @string = ((GameItem)ability).GetTagReadonly("proficiency").GetString();
			if (((GameCharacterItem)caster).IsTag(@string))
			{
				int @int = ((GameCharacterItem)caster).GetTagReadonly(@string + "_CURRENT").GetInt();
				if (@int > 0)
				{
					((GameCharacterItem)caster).SetTagInt(@string + "_CURRENT", @int - 1);
					return true;
				}
			}
		}
		return false;
	}

	public void UseAbilityOutsidePlayingTurn(CombatEntity caster, CombatEntity target, CombatAbility castersAbility)
	{
		caster.currentSelectedAbility = castersAbility;
		List<CombatEntity> targetEntityUniqueList = CombatEntity.HandleAbilityTarget(caster, target, partiesList[0].list, partiesList[1].list);
		AppliedEffectsOnTargets(targetEntityUniqueList, castersAbility, caster);
	}

	public void CheckOverwatch(CombatEntity originalCaster)
	{
		if (!originalCaster.currentSelectedAbility.triggerOverwatch)
		{
			return;
		}
		int adverseTeam = originalCaster.getAdverseTeam();
		List<CombatEntity> list = new List<CombatEntity>();
		list.AddRange(partiesList[adverseTeam].list);
		foreach (CombatEntity currentEntity in list)
		{
			if (currentEntity.HasEffectType(CombatAbilityEffect.EffectType.setOverwatch) && !currentEntity.isDead && !((GameCharacterItem)originalCaster).isDeadLogic)
			{
				ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
				{
					CombatLog.log(originalCaster.name + " triggers " + currentEntity.name + "'s overwatch!");
					SplashTextManager.Instance.DisplayFloatText(currentEntity, "Overwatch Triggered", SplashTextManager.TextMode.Buffed);
				}));
				CombatAbility entityWeaponAbility = currentEntity.GetEntityWeaponAbility();
				entityWeaponAbility.triggerOverwatch = false;
				UseAbilityOutsidePlayingTurn(currentEntity, originalCaster, entityWeaponAbility);
			}
		}
	}

	public static bool IsAbilityHit(CombatEntity caster, CombatAbility ability, CombatEntity target)
	{
		int rng = UnityEngine.Random.Range(0, 100);
		int num = caster.accuracyPercentage + ability.bonusAccuracy;
		int misschance = 100 - num;
		int totalMissChance = misschance + target.dodge;
		float num2 = 0f;
		if (totalMissChance <= 0 || ability.IsAlwaysAccurate())
		{
			misschance = 0;
			ActionQueue.current.Add(new EmptyAction(num2).SetBeginFunc(delegate
			{
				CombatLog.vlog(caster.name + "'s ability connected by rolling a " + rng + ", they needed at least " + misschance + " to connect.");
			}));
			return true;
		}
		if (target.dodge >= 0)
		{
			if (rng < misschance)
			{
				ActionQueue.current.Add(new EmptyAction(num2).SetBeginFunc(delegate
				{
					CombatLog.log(caster.name + " missed!");
					CombatLog.vlog("they rolled a " + rng + ", they needed at least " + totalMissChance + " to connect.");
				}));
				return false;
			}
			if (rng < misschance + target.dodge)
			{
				ActionQueue.current.Add(new EmptyAction(num2).SetBeginFunc(delegate
				{
					CombatLog.log(target.name + " dodged!");
					CombatLog.vlog("they rolled a " + rng + ", they needed at least " + totalMissChance + " to connect.");
				}));
				return false;
			}
			ActionQueue.current.Add(new EmptyAction(num2).SetBeginFunc(delegate
			{
				CombatLog.vlog(caster.name + "'s ability connected by rolling a " + rng + ", they needed at least " + totalMissChance + " to connect.");
			}));
			return true;
		}
		if (rng < misschance + target.dodge)
		{
			ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
			{
				CombatLog.log(caster.name + " missed!");
				CombatLog.vlog("they rolled a " + rng + ", they needed at least " + totalMissChance + " to connect.");
			}));
			return false;
		}
		if (rng < misschance)
		{
			ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
			{
				CombatLog.log(caster.name + " hit due to " + target.name + "'s low dodge!");
				CombatLog.vlog(caster.name + "'s ability connected by rolling a " + rng + ", they needed at least " + totalMissChance + " to connect.");
			}));
			return true;
		}
		ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
		{
			CombatLog.vlog(caster.name + "'s ability connected by rolling a " + rng + ", they needed at least " + totalMissChance + " to connect.");
		}));
		return true;
	}

	public void CheckRetalation(CombatEntity originalCaster, CombatEntity originalReceiver)
	{
		if (originalReceiver.HasEffectType(CombatAbilityEffect.EffectType.setRetaliate) && !originalCaster.isDead && !((GameCharacterItem)originalCaster).isDeadLogic && !((GameCharacterItem)originalReceiver).isDeadLogic && !IsSameTeam(originalCaster, originalReceiver))
		{
			ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
			{
				CombatLog.log(originalReceiver.name + " retaliate against " + originalCaster.name + "!");
				SplashTextManager.Instance.DisplayFloatText(originalReceiver, "Retaliate Triggered", SplashTextManager.TextMode.Buffed);
			}));
			CombatAbility entityWeaponAbility = originalReceiver.GetEntityWeaponAbility();
			entityWeaponAbility.triggerOverwatch = false;
			UseAbilityOutsidePlayingTurn(originalReceiver, originalCaster, entityWeaponAbility);
		}
	}

	public void ManageTurn()
	{
		int num = IsCombatDoneLogic();
		if (num == 0 || num == 1)
		{
			return;
		}
		if (IsTurnDone())
		{
			combatEventHandler.HandleTurnEndEvents(this);
			GameMaster.current.turnManager.NextTurn();
			StartNewTurn();
			return;
		}
		CombatEntity nextPlayingEntity = GetNextPlayingEntity();
		bool flag = false;
		if (((GameCharacterItem)nextPlayingEntity).IsTag("action_free_mode"))
		{
			((GameCharacterItem)nextPlayingEntity).DisableTag("action_free_mode");
		}
		else
		{
			SetPlayingEntity(nextPlayingEntity);
			flag = currentPlayingEntity.OnTurnStart();
		}
		if (flag)
		{
			ManageTurn();
		}
	}

	public void Reset()
	{
		partiesList[0].ResetEntitiesAction();
		CombatEntity nextPlayingEntity = GetNextPlayingEntity();
		SetPlayingEntity(nextPlayingEntity);
		currentTurn = 0;
	}

	public void PostCombatCleanup(int winningTeam)
	{
		CombatLog.log("Combat finished, team " + winningTeam + " Won.", RenderHandler.ColorPalette.Yellow);
		if (winningTeam == 0)
		{
			partiesList[0].ResetEntitiesAction();
			partiesList[0].AfterBattleCleanUp();
		}
	}

	public void EntityDied(CombatEntity diedEntity)
	{
		ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
		{
			partiesList[0].RemoveMember(diedEntity);
			partiesList[1].RemoveMember(diedEntity);
			int num = IsCombatDone();
			if (num == 0 || num == 1)
			{
				PostCombatCleanup(num);
			}
		}));
	}

	public List<CombatEntity> GetMergeParty()
	{
		List<CombatEntity> list = new List<CombatEntity>(partiesList[0].list.Count + partiesList[1].list.Count);
		list.AddRange(partiesList[0].list);
		list.AddRange(partiesList[1].list);
		return list;
	}

	public void ChangePlayingEntityAbility(CombatAbility combatAbility)
	{
		currentPlayingEntity.SelectAbility(combatAbility);
	}

	public bool IsTargetValid(CombatEntity entity)
	{
		CombatAbilityEffect.TargetType abilityTargetType = currentPlayingEntity.GetSelectedAbility().effects[0].abilityTargetType;
		if (abilityTargetType == CombatAbilityEffect.TargetType.singleEnemy && IsSameTeam(currentPlayingEntity, entity))
		{
			return false;
		}
		if (abilityTargetType == CombatAbilityEffect.TargetType.allEnemy && IsSameTeam(currentPlayingEntity, entity))
		{
			return false;
		}
		if (abilityTargetType == CombatAbilityEffect.TargetType.singleFriendly && !IsSameTeam(currentPlayingEntity, entity))
		{
			return false;
		}
		if (abilityTargetType == CombatAbilityEffect.TargetType.singleFriendlyExceptSelf && !IsSameTeam(currentPlayingEntity, entity))
		{
			return false;
		}
		if (abilityTargetType == CombatAbilityEffect.TargetType.allFriendly && !IsSameTeam(currentPlayingEntity, entity))
		{
			return false;
		}
		return true;
	}

	public static bool IsSameTeam(CombatEntity entity1, CombatEntity entity2)
	{
		if (entity1.isPlayerControlled && entity2.isPlayerControlled)
		{
			return true;
		}
		if (!entity1.isPlayerControlled && !entity2.isPlayerControlled)
		{
			return true;
		}
		return false;
	}

	public bool HandleStunnedPlayingEntity(CombatEntity workingEntity)
	{
		if (workingEntity.IsStunned())
		{
			workingEntity.action -= 1;
			ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
			{
				CombatLog.log(workingEntity.name + "'s stunned and lost their turn!");
				SplashTextManager.Instance.DisplayFloatText(workingEntity, "Stunned", SplashTextManager.TextMode.Stun);
			}));
			workingEntity.EntityTurnDone();
			ManageTurn();
			return true;
		}
		return false;
	}

	public bool HandleLostofAgencyEntity(CombatEntity workingEntity)
	{
		if (workingEntity.HasEffectType(CombatAbilityEffect.EffectType.berserk))
		{
			CombatLog.log(workingEntity.name + " is berserk and decided to attack randomly");
			ChangePlayingEntityAbility(workingEntity.GetEntityWeaponAbility());
			UseAbility(GetRandomTargetExceptSelf(workingEntity));
			return true;
		}
		return false;
	}

	public void AppliedCombatEffectToAll(CombatAbilityEffect effect, CombatEntity caster)
	{
		foreach (CombatEntity item in GetMergeParty())
		{
			item.AppliedCombatEffect(effect, caster);
		}
	}

	public CombatEntity GetRandomTargetExceptSelf(CombatEntity entity)
	{
		List<CombatEntity> mergeParty = GetMergeParty();
		mergeParty.Remove(entity);
		if (mergeParty.Count == 0)
		{
			return entity;
		}
		return mergeParty[random.Next(0, mergeParty.Count)];
	}

	public void AppliedEffectsOnTargets(List<CombatEntity> targetEntityUniqueList, CombatAbility ability, CombatEntity currentPlayingEntity)
	{
		foreach (CombatEntity targetedEntity in targetEntityUniqueList)
		{
			Debug.Log(currentPlayingEntity.name + " use " + ability.name + " on " + targetedEntity.name);
			if (IsAbilityHit(currentPlayingEntity, ability, targetedEntity))
			{
				foreach (CombatAbilityEffect item in targetedEntity.abilityEffectsToBeApplied)
				{
					for (int i = 0; i < item.repeat; i++)
					{
						CombatAbilityEffect combatAbilityEffect = DirectoryMaster.CombatAbilityEffect(item.identifier).CopyFrom(item);
						combatAbilityEffect.value = combatAbilityEffect.ResolveValue();
						if (!targetedEntity.isDead && !currentPlayingEntity.isDead && combatAbilityEffect.RollApplyChance())
						{
							targetedEntity.AppliedCombatEffect(combatAbilityEffect, currentPlayingEntity);
							if (!GameMaster.current.raidManager.IsInCombat())
							{
								targetedEntity.ForceUpdateThisEffect(combatAbilityEffect);
								((GameCharacterItem)targetedEntity).Validate();
							}
						}
					}
				}
				targetedEntity.abilityEffectsToBeApplied.Clear();
			}
			else
			{
				ActionQueue.current.Add(new EmptyAction(0.5).SetBeginFunc(delegate
				{
					SplashTextManager.Instance.DisplayFloatText(targetedEntity, "Missed!", SplashTextManager.TextMode.Miss);
				}));
			}
			targetedEntity.abilityEffectsToBeApplied.Clear();
			CheckRetalation(currentPlayingEntity, targetedEntity);
		}
	}
}
