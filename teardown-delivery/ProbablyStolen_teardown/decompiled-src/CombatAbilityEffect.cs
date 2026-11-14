using System;
using UnityEngine;

public class CombatAbilityEffect : TagSystem, DirectoryEntry
{
	public enum EffectType
	{
		none,
		attack,
		heal,
		buff,
		debuff,
		overtimeAttack,
		overtimeHeal,
		callback,
		setOverwatch,
		setRetaliate,
		stun,
		berserk,
		toxin,
		bleeding,
		burning
	}

	public enum TargetType
	{
		none,
		singleEnemy,
		allEnemy,
		self,
		singleFriendly,
		allFriendly,
		all,
		singleFriendlyExceptSelf
	}

	public enum EffectTag
	{
		none,
		survivalHunger,
		survivalThirst
	}

	public enum ValueType
	{
		standard,
		uniform
	}

	public bool isPermanent;

	public bool isInvisible;

	public EffectType abilityEffectType { get; set; }

	public TargetType abilityTargetType { get; set; }

	public int duration { get; set; }

	public int durationBase { get; set; }

	public CombatEntity.EffectType affectedStat { get; set; }

	public int value { get; set; }

	public int valueBase { get; set; }

	public int id
	{
		get
		{
			return name.GetHashCode();
		}
		set
		{
			identifier = "id_" + value;
		}
	}

	public string name { get; set; }

	public string identifier { get; set; }

	public string identifierName { get; set; }

	public Action<CombatManager, CombatEntity, CombatEntity> callback { get; set; }

	public Action<CombatManager, CombatEntity, CombatEntity> initCallback { get; set; }

	public Action<CombatManager, CombatEntity, CombatEntity> endCallback { get; set; }

	public int repeat { get; set; }

	public int repeatBase { get; set; }

	public int effectApplyChance { get; set; }

	public int effectApplyChanceBase { get; set; }

	public ValueType valueType { get; set; }

	public int valueLow { get; set; }

	public int valueLowBase { get; set; }

	public int valueHigh { get; set; }

	public int valueHighBase { get; set; }

	public CombatAbilityEffect()
	{
		callback = null;
		affectedStat = CombatEntity.EffectType.none;
		isPermanent = false;
		isInvisible = false;
		repeat = 1;
		effectApplyChance = 100;
		valueType = ValueType.standard;
	}

	public CombatAbilityEffect(string identifier, int value = 1, int duration = 1, EffectType abilityEffectType = EffectType.none, TargetType abilityTargetType = TargetType.singleEnemy, CombatEntity.EffectType affectedStat = CombatEntity.EffectType.health, string name = "DEFAULT EFFECT NAME", int repeat = 1, int effectApplyChance = 100, ValueType valueType = ValueType.standard, bool isPermanent = false, bool isInvisible = false, Action<CombatManager, CombatEntity, CombatEntity> callback = null, int valueHigh = 0, int valueLow = 0, Action<CombatManager, CombatEntity, CombatEntity> initCallback = null, Action<CombatManager, CombatEntity, CombatEntity> endCallback = null)
	{
		this.identifier = identifier;
		this.value = value;
		this.duration = duration;
		this.abilityEffectType = abilityEffectType;
		this.abilityTargetType = abilityTargetType;
		this.affectedStat = affectedStat;
		this.name = name;
		this.valueHigh = valueHigh;
		this.valueLow = valueLow;
		this.callback = callback;
		this.initCallback = initCallback;
		this.endCallback = endCallback;
		this.isPermanent = isPermanent;
		this.isInvisible = isInvisible;
		this.repeat = repeat;
		this.valueType = valueType;
		this.effectApplyChance = effectApplyChance;
		ApplyToBase();
	}

	public CombatAbilityEffect DeepCopy()
	{
		CombatAbilityEffect result = new CombatAbilityEffect
		{
			abilityEffectType = abilityEffectType,
			abilityTargetType = abilityTargetType,
			affectedStat = affectedStat,
			duration = duration,
			value = value,
			callback = callback,
			initCallback = initCallback,
			endCallback = endCallback,
			name = name,
			identifier = identifier,
			isInvisible = isInvisible,
			isPermanent = isPermanent
		};
		if (callback != null && duration == 0)
		{
			throw new UnityException("Warning: CombatAbilityEffect onUpdate Callback is not null but duration is 0. Callback will never be executed, do you mean to use initCallback Instead?");
		}
		return result;
	}

	public bool IsBuff()
	{
		if (abilityEffectType == EffectType.buff || abilityEffectType == EffectType.debuff)
		{
			return true;
		}
		return false;
	}

	public bool IsOvertime()
	{
		if (abilityEffectType == EffectType.overtimeAttack || abilityEffectType == EffectType.overtimeHeal || abilityEffectType == EffectType.toxin || abilityEffectType == EffectType.burning || abilityEffectType == EffectType.bleeding)
		{
			return true;
		}
		return false;
	}

	public bool IsInstant()
	{
		if (abilityEffectType == EffectType.attack || abilityEffectType == EffectType.heal)
		{
			return true;
		}
		return false;
	}

	public void OnExpire(CombatEntity target, CombatEntity caster)
	{
		if (endCallback != null)
		{
			endCallback(CombatManager.Instance, target, caster);
		}
	}

	public void OnApply(CombatEntity target, CombatEntity caster)
	{
		if (initCallback != null)
		{
			initCallback(CombatManager.Instance, target, caster);
		}
	}

	public void OnUpdate(CombatEntity target, CombatEntity caster)
	{
		if (callback != null)
		{
			callback(CombatManager.Instance, target, caster);
		}
	}

	public void DecrementDuration()
	{
		if (!isPermanent)
		{
			duration--;
		}
	}

	public bool IsArmorPercing()
	{
		if (abilityEffectType == EffectType.toxin || abilityEffectType == EffectType.bleeding)
		{
			return true;
		}
		return false;
	}

	public int ResolveValue()
	{
		if (valueType == ValueType.standard)
		{
			return value;
		}
		if (valueType == ValueType.uniform)
		{
			if (valueLow < valueHigh)
			{
				return RNG.GetRandomInt(valueLow, valueHigh);
			}
			return RNG.GetRandomInt(valueHigh, valueLow);
		}
		return value;
	}

	public bool RollApplyChance()
	{
		return RNG.Roll(effectApplyChance);
	}

	public CombatAbilityEffect CopyFrom(CombatAbilityEffect origin)
	{
		duration = origin.duration;
		value = origin.value;
		repeat = origin.repeat;
		effectApplyChance = origin.effectApplyChance;
		valueLow = origin.valueLow;
		valueHigh = origin.valueHigh;
		return this;
	}

	public void ApplyToBase()
	{
		durationBase = duration;
		valueBase = value;
		repeatBase = repeat;
		effectApplyChanceBase = effectApplyChance;
		valueLowBase = valueLow;
		valueHighBase = valueHigh;
	}

	public void HandlePerkOnEffectApply()
	{
		if (PerkDirectory.onEffectApplyPerks.Contains(identifier) && !identifier.Equals("perk_hemophilia") && !identifier.Equals("perk_thrombophilia"))
		{
			identifier.Equals("perk_venom_adapdation");
		}
	}
}
