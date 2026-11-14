using Duckov.Buffs;
using UnityEngine;

public class GunSettingExpend_AddBuffToSelfOnHurtEnemy : MonoBehaviour
{
	private ItemSetting_Gun gunSetting;

	public bool checkCrit;

	public bool onlyOnEnemyDead;

	public Buff buffPrefab;

	public int layers = 1;

	private void Awake()
	{
		gunSetting = GetComponent<ItemSetting_Gun>();
		if ((bool)gunSetting)
		{
			gunSetting.onHurtEnemyEvent.AddListener(OnHurtEnemy);
		}
	}

	private void OnDestroy()
	{
		if ((bool)gunSetting)
		{
			gunSetting.onHurtEnemyEvent.RemoveListener(OnHurtEnemy);
		}
	}

	protected virtual void OnHurtEnemy(DamageReceiver dmgReceiver, DamageInfo dmgInfo)
	{
		if ((bool)dmgReceiver && (!checkCrit || !(dmgInfo.critRate < 0.99f)) && (!onlyOnEnemyDead || dmgReceiver.IsDead) && (bool)dmgInfo.fromCharacter)
		{
			for (int i = 0; i < layers; i++)
			{
				Debug.Log($"add buff to:{dmgInfo.fromCharacter}  is main Character：{dmgInfo.fromCharacter.IsMainCharacter}");
				dmgInfo.fromCharacter.AddBuff(buffPrefab, dmgInfo.fromCharacter);
			}
		}
	}
}
