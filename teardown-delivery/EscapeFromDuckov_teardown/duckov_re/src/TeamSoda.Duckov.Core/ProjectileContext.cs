using Duckov.Buffs;
using ItemStatsSystem;
using UnityEngine;

public struct ProjectileContext
{
	public Vector3 direction;

	public CharacterMainControl traceTarget;

	public float traceAbility;

	public bool firstFrameCheck;

	public Vector3 firstFrameCheckStartPoint;

	public float halfDamageDistance;

	public float distance;

	public float speed;

	public Teams team;

	public int penetrate;

	public float damage;

	public float damageFactorToZombie;

	public float dmgOverDistance;

	public float critDamageFactor;

	public float critRate;

	public float armorPiercing;

	public float armorBreak;

	public float element_Physics;

	public float element_Fire;

	public float element_Poison;

	public float element_Electricity;

	public float element_Space;

	public float element_Ghost;

	public float element_Ice;

	public CharacterMainControl fromCharacter;

	public CharacterMainControl realFromCharacter;

	public ControlMindTypes controlMindType;

	public float controlMindTime;

	public float gravity;

	public float explosionRange;

	public float explosionDamage;

	public float buffChance;

	public Buff buff;

	public float bleedChance;

	public bool ignoreHalfObsticle;

	public ItemSetting_Gun fromGunItemSetting;

	[ItemTypeID]
	public int fromWeaponItemID;
}
