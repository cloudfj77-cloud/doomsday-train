using UnityEngine;

public class GunSettingExpend_ExplosionOnHurt : MonoBehaviour
{
	public bool checkCrit;

	public bool onlyOnEnemyDead;

	public DamageInfo damageInfo;

	public float damageRange;

	public bool canHurtSelf = true;

	public ExplosionFxTypes fxType;

	public GameObject fx;

	[Range(0f, 1f)]
	public float explosionShakeStrength = 1f;

	private ItemSetting_Gun gunSetting;

	private bool needCustomFx => fxType == ExplosionFxTypes.custom;

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

	protected void OnHurtEnemy(DamageReceiver dmgReceiver, DamageInfo dmgInfo)
	{
		if (!dmgReceiver || (checkCrit && dmgInfo.critRate < 0.99f) || (onlyOnEnemyDead && !dmgReceiver.IsDead))
		{
			return;
		}
		Health health = dmgReceiver.health;
		if (!health)
		{
			return;
		}
		CharacterMainControl characterMainControl = health.TryGetCharacter();
		if ((bool)characterMainControl)
		{
			Vector3 position = characterMainControl.transform.position;
			if ((bool)gunSetting && (bool)gunSetting.Item)
			{
				damageInfo.fromWeaponItemID = gunSetting.Item.TypeID;
			}
			damageInfo.fromCharacter = dmgInfo.fromCharacter;
			damageInfo.isExplosion = true;
			LevelManager.Instance.ExplosionManager.CreateExplosion(position, damageRange, damageInfo, fxType, explosionShakeStrength, canHurtSelf);
			if (needCustomFx && fx != null)
			{
				Object.Instantiate(fx, position, Quaternion.identity);
			}
		}
	}
}
