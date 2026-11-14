using Duckov.Buffs;
using UnityEngine;

public class GunSettingExpend_AddBuffToEnemyOnHurt : MonoBehaviour
{
	private ItemSetting_Gun gunSetting;

	public bool checkCrit;

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
		if ((!checkCrit || !(dmgInfo.critRate < 0.99f)) && (bool)dmgReceiver)
		{
			for (int i = 0; i < layers; i++)
			{
				dmgReceiver.AddBuff(buffPrefab, dmgInfo.fromCharacter);
			}
		}
	}
}
