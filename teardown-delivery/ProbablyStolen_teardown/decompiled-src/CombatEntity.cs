using System.Collections.Generic;

public interface CombatEntity : DirectoryEntry
{
	public enum EffectType
	{
		health = 0,
		maxHealth = 1,
		reaction = 2,
		action = 3,
		maxAction = 4,
		accuracyPercentage = 5,
		damagePercentage = 6,
		deathchance = 9,
		criticalChanceDealing = 10,
		criticalChanceReceving = 11,
		criticalDamageModifierDealing = 12,
		criticalDamageModifierReceving = 13,
		priority = 14,
		none = 15,
		dodge = 16
	}

	const int EFFECT_TYPE_COUNT = 17;

	List<CombatAbilityEffect> abilityEffects { get; set; }

	List<CombatAbilityEffect> abilityEffectsToBeApplied { get; set; }

	int accuracyPercentage { get; set; }

	int action { get; set; }

	CombatAI combatAI { get; set; }

	int criticalChanceDealing { get; set; }

	int criticalChanceReceving { get; set; }

	int criticalDamageModifierDealing { get; set; }

	int criticalDamageModifierReceving { get; set; }

	CombatAbility currentSelectedAbility { get; set; }

	int damagePercentage { get; set; }

	int deathchance { get; set; }

	int health { get; set; }

	bool isDead { get; set; }

	bool isPlayerControlled { get; set; }

	int maxAction { get; set; }

	int maxHealth { get; set; }

	string name { get; set; }

	int priority { get; set; }

	int reaction { get; set; }

	int dodge { get; set; }

	void AppliedCombatEffect(CombatAbilityEffect effect, CombatEntity caster);

	void AppliedOverTimeEffect(CombatAbilityEffect effect);

	void AppliedStatToEffect(CombatAbilityEffect effect);

	void CancelOverwatch();

	void CancelRetaliate();

	CombatAbility CastSelectedAbility();

	bool CheckDeathDoor();

	bool DamageEntity(int damage, bool isArmorPercing = false);

	int getAdverseTeam();

	CombatAbility GetEntityWeaponAbility();

	CombatAbility GetSelectedAbility();

	int getTeam();

	bool HasEffect(CombatAbilityEffect effect);

	bool HasEffectType(CombatAbilityEffect.EffectType effectType);

	bool HaveEffect(string identifier);

	bool HaveEffect(CombatAbilityEffect.EffectTag tag);

	void HealEntity(int healing);

	bool IsAnyAbilitySelected();

	bool IsBleeding();

	bool IsEntityLostOfAgency();

	bool IsStunned();

	void KillCurrentEntity();

	void OnCombatStart();

	bool OnTurnStart();

	int RemoveEffectByID(string identifier);

	void resetCurrentAction();

	void ResolveEffect(CombatAbilityEffect effect, CombatEntity caster);

	void ReverseBuffStat(CombatAbilityEffect effect);

	void SelectAbility(CombatAbility combatAbility);

	void UpdateCombatEffect();

	void ForceUpdateEffect();

	void ForceUpdateThisEffect(CombatAbilityEffect effect);

	void EntityTurnDone();

	int RemoveEffectByTag(CombatAbilityEffect.EffectTag tag);

	static List<CombatEntity> HandleAbilityTarget(CombatEntity caster, CombatEntity target, List<CombatEntity> party0, List<CombatEntity> party1)
	{
		CombatAbility ability = caster.CastSelectedAbility();
		ActionQueue.current.Add(new EmptyAction().SetBeginFunc(delegate
		{
			SplashTextManager.Instance.DisplayFloatText(ability.name, SplashTextManager.TextMode.Buffed);
		}));
		CombatEntity currentPlayingEntity = caster;
		List<CombatEntity> list = new List<CombatEntity>();
		List<CombatEntity> list2 = new List<CombatEntity>();
		int num = 0;
		float num2 = 0.1f;
		foreach (CombatAbilityEffect effect in ability.effects)
		{
			if (effect.abilityTargetType == CombatAbilityEffect.TargetType.all)
			{
				list.AddRange(party0);
				list.AddRange(party1);
				if (num == 0)
				{
					ActionQueue.current.Add(new EmptyAction(num2).SetBeginFunc(delegate
					{
						CombatLog.log(currentPlayingEntity.name + " attempt to use " + ability.name + " on all.");
					}));
				}
			}
			else if (effect.abilityTargetType == CombatAbilityEffect.TargetType.singleEnemy)
			{
				list.Add(target);
				if (num == 0)
				{
					ActionQueue.current.Add(new EmptyAction(num2).SetBeginFunc(delegate
					{
						CombatLog.log(currentPlayingEntity.name + " attempt to use " + ability.name + " on " + target.name);
					}));
				}
			}
			else if (effect.abilityTargetType == CombatAbilityEffect.TargetType.singleFriendly)
			{
				list.Add(target);
				if (num == 0)
				{
					ActionQueue.current.Add(new EmptyAction(num2).SetBeginFunc(delegate
					{
						CombatLog.log(currentPlayingEntity.name + " attempt to use " + ability.name + " on " + target.name);
					}));
				}
			}
			else if (effect.abilityTargetType == CombatAbilityEffect.TargetType.singleFriendlyExceptSelf)
			{
				list.Add(target);
				if (num == 0)
				{
					ActionQueue.current.Add(new EmptyAction(num2).SetBeginFunc(delegate
					{
						CombatLog.log(currentPlayingEntity.name + " attempt to use " + ability.name + " on " + target.name);
					}));
				}
			}
			else if (effect.abilityTargetType == CombatAbilityEffect.TargetType.allEnemy)
			{
				if (currentPlayingEntity.isPlayerControlled)
				{
					list.AddRange(party1);
				}
				else
				{
					list.AddRange(party0);
				}
				if (num == 0)
				{
					ActionQueue.current.Add(new EmptyAction(num2).SetBeginFunc(delegate
					{
						CombatLog.log(currentPlayingEntity.name + " attempt to use " + ability.name + " on all enemies.");
					}));
				}
			}
			else if (effect.abilityTargetType == CombatAbilityEffect.TargetType.allFriendly)
			{
				if (currentPlayingEntity.isPlayerControlled)
				{
					list.AddRange(party0);
				}
				else
				{
					list.AddRange(party1);
				}
				if (num == 0)
				{
					ActionQueue.current.Add(new EmptyAction(num2).SetBeginFunc(delegate
					{
						CombatLog.log(currentPlayingEntity.name + " attempt to use " + ability.name + " on all friendlies.");
					}));
				}
			}
			else if (effect.abilityTargetType == CombatAbilityEffect.TargetType.self)
			{
				list.Add(currentPlayingEntity);
				if (num == 0)
				{
					ActionQueue.current.Add(new EmptyAction(num2).SetBeginFunc(delegate
					{
						CombatLog.log(currentPlayingEntity.name + " attempt to use " + ability.name + " on themself.");
					}));
				}
			}
			num = 1;
			foreach (CombatEntity item2 in list)
			{
				CombatAbilityEffect item = DirectoryMaster.CombatAbilityEffect(effect.identifier).CopyFrom(effect);
				item2.abilityEffectsToBeApplied.Add(item);
				if (!list2.Contains(item2))
				{
					list2.Add(item2);
				}
			}
			list.Clear();
		}
		return list2;
	}

	static bool UseAbility(CombatEntity caster, CombatAbility ability, CombatEntity target, List<CombatEntity> party0, List<CombatEntity> party1)
	{
		caster.currentSelectedAbility = ability;
		List<CombatEntity> list = HandleAbilityTarget(caster, target, party0, party1);
		bool result = list.Count > 0;
		CombatManager.Instance.AppliedEffectsOnTargets(list, caster.currentSelectedAbility, caster);
		ActionQueue.current.Next();
		return result;
	}
}
