using Duckov.Buffs;
using ItemStatsSystem;
using UnityEngine;

public class ExplosionAction : EffectAction
{
	public Buff selfBuff;

	public DamageInfo damageInfo;

	public float damageRange;

	public bool canHurtSelf = true;

	public ExplosionFxTypes fxType;

	public GameObject fx;

	[Range(0f, 1f)]
	public float explosionShakeStrength = 1f;

	private bool needCustomFx => fxType == ExplosionFxTypes.custom;

	private CharacterMainControl MainControl => base.Master?.Item?.GetCharacterMainControl();

	protected override void OnTriggered(bool positive)
	{
		CharacterMainControl mainControl = MainControl;
		if ((bool)mainControl)
		{
			Vector3 position = mainControl.transform.position;
			damageInfo.isExplosion = true;
			if (!selfBuff)
			{
				selfBuff = GetComponentInParent<Buff>();
			}
			if ((bool)selfBuff)
			{
				damageInfo.fromCharacter = selfBuff.fromWho;
			}
			LevelManager.Instance.ExplosionManager.CreateExplosion(position, damageRange, damageInfo, fxType, explosionShakeStrength, canHurtSelf);
			if (needCustomFx && fx != null)
			{
				Object.Instantiate(fx, position, Quaternion.identity);
			}
		}
	}
}
