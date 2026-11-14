using UnityEngine;

public class GunSettingExpend_StickBoomOnHurt : MonoBehaviour
{
	public bool checkCrit;

	public bool onlyOnEnemyDead;

	public DamageInfo damageInfo;

	public float delay = 1f;

	public float range = 5f;

	public Grenade grenadePfb;

	public bool createExplosion = true;

	public bool canHurtSelf = true;

	[Range(0f, 1f)]
	public float explosionShakeStrength = 1f;

	private ItemSetting_Gun gunSetting;

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

	public void OnHurtEnemy(DamageReceiver dmgReceiver, DamageInfo dmgInfo)
	{
		if ((bool)dmgReceiver && (!checkCrit || !(dmgInfo.critRate < 0.99f)) && (!onlyOnEnemyDead || dmgReceiver.IsDead))
		{
			CharacterMainControl fromCharacter = dmgInfo.fromCharacter;
			if (!(fromCharacter == null))
			{
				Vector3 damagePoint = dmgInfo.damagePoint;
				Grenade grenade = Object.Instantiate(grenadePfb, damagePoint, fromCharacter.CurrentUsingAimSocket.rotation);
				damageInfo.fromCharacter = fromCharacter;
				grenade.damageInfo = damageInfo;
				grenade.createExplosion = createExplosion;
				grenade.explosionShakeStrength = explosionShakeStrength;
				grenade.damageRange = range;
				grenade.delayFromCollide = false;
				grenade.delayTime = delay;
				grenade.isLandmine = false;
				grenade.StickTo(dmgReceiver.transform);
				grenade.Launch(damagePoint, Vector3.zero, fromCharacter, canHurtSelf);
				grenade.SetWeaponIdInfo(dmgInfo.fromWeaponItemID);
			}
		}
	}
}
