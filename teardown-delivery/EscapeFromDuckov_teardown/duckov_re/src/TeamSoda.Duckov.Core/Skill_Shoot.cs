using Cysharp.Threading.Tasks;
using ItemStatsSystem;
using UnityEngine;

public class Skill_Shoot : SkillBase
{
	[ItemTypeID]
	public int gunItem;

	public Transform gunParent;

	private ItemAgent_Gun gun;

	public float shootTime = 0.25f;

	private float shootTimer;

	private void Start()
	{
		CreateGun().Forget();
	}

	private async UniTaskVoid CreateGun()
	{
		Item item = await ItemAssetsCollection.InstantiateAsync(gunItem);
		if ((bool)item.GetComponent<ItemSetting_Gun>() && item.CreateHandheldAgent() is ItemAgent_Gun itemAgent_Gun)
		{
			if (gunParent == null)
			{
				gunParent = base.transform;
			}
			itemAgent_Gun.transform.SetParent(gunParent, worldPositionStays: false);
			itemAgent_Gun.transform.localPosition = Vector3.zero;
			itemAgent_Gun.transform.localRotation = Quaternion.identity;
			gun = itemAgent_Gun;
		}
	}

	private async UniTaskVoid Shoot(Vector3 targetPoint)
	{
		float timer = 0f;
		if ((bool)gun)
		{
			gun.OverrideShootPoint(targetPoint);
			gun.fallbackTeam = fromCharacter.Team;
			gun.SetTrigger(trigger: true, _triggerThisFrame: true, _releaseThisFrame: false);
		}
		while (timer < shootTime)
		{
			timer += Time.deltaTime;
			if ((bool)gun)
			{
				gun.SetTrigger(trigger: true, _triggerThisFrame: false, _releaseThisFrame: false);
			}
			await UniTask.WaitForEndOfFrame();
		}
		if ((bool)gun)
		{
			gun.SetTrigger(trigger: false, _triggerThisFrame: false, _releaseThisFrame: false);
		}
	}

	public override void OnRelease()
	{
		if (!(fromCharacter == null))
		{
			Vector3 releasePoint = skillReleaseContext.releasePoint;
			Vector3 vector = releasePoint - fromCharacter.transform.position;
			vector.y = 0f;
			_ = vector.magnitude;
			vector.Normalize();
			if ((bool)gun)
			{
				Shoot(releasePoint + Vector3.up * 0.5f);
			}
		}
	}
}
