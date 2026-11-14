public class Shotgun : TooltipInterface, IHoldClickable, IClickable
{
	public float HoldDuration => 0.4f;

	public bool ShouldRequireHold()
	{
		if (CanPickup())
		{
			return !StoreUIManager.Instance.isShootingMode;
		}
		return false;
	}

	public override string GetTooltip()
	{
		if (CanPickup())
		{
			return LocHelper.GetLocalizedMechanic("shotgun_equip");
		}
		return LocHelper.GetLocalizedMechanic("shotgun_cant_equip");
	}

	public override bool IsTooltipEnabled()
	{
		return true;
	}

	public bool CanPickup()
	{
		if (PlayerStore.Instance.currentClientInstance == null)
		{
			return false;
		}
		if (!PlayerStore.Instance.currentClientInstance.GetClientBlueprint().arrestable && !PlayerStore.Instance.currentClientInstance.GetClientBlueprint().forceShootable)
		{
			return false;
		}
		if (!PlayerStore.Instance.currentClientInstance.GetClientBlueprint().dismissable && !PlayerStore.Instance.currentClientInstance.GetClientBlueprint().forceShootable)
		{
			return false;
		}
		if (PlayerStore.Instance.currentClientInstance.GetClientBlueprint().isSecurity)
		{
			return false;
		}
		if (PlayerStore.Instance.currentClientInstance.GetClientBlueprint().nonShootable)
		{
			return false;
		}
		if (PlayerStore.Instance.isClientBeingArrested)
		{
			return false;
		}
		if (PlayerStore.Instance.isHandlingShootout)
		{
			return false;
		}
		if (BargainUIManager.Instance != null && BargainUIManager.Instance.ThreatenOffered)
		{
			return false;
		}
		return true;
	}

	public void OnClick()
	{
		if (StoreUIManager.Instance.isShootingMode)
		{
			StoreUIManager.Instance.ExitShootingMode();
		}
		else if (CanPickup())
		{
			StoreUIManager.Instance.EnterShootingMode();
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public static void LoadShotgun()
	{
		if (PlayerStore.Instance.isShootingUnlocked)
		{
			StoreUIManager.Instance.shotgun.SetActive(value: true);
		}
		else
		{
			StoreUIManager.Instance.shotgun.SetActive(value: false);
		}
	}

	public static void Unlock()
	{
		if (!PlayerStore.Instance.isShootingUnlocked)
		{
			AudioManager.Instance.Play2("shotgun_pump");
			PlayerStore.Instance.augPresence = 15;
			PlayerStore.Instance.isAugIntroduced = true;
			PlayerStore.Instance.isShootingUnlocked = true;
			LoadShotgun();
		}
	}

	public static void LockShotgun()
	{
		AudioManager.Instance.Play2("shotgun_pump");
		PlayerStore.Instance.isShootingUnlocked = false;
		LoadShotgun();
	}

	public static void OnGunPermitIRevoked()
	{
		if (PlayerStore.Instance.isShootingUnlocked)
		{
			LockShotgun();
		}
	}

	public static void OnGunPermitIGranted()
	{
		ReturnIfLost();
	}

	public static void ReturnIfLost()
	{
		if (!PlayerStore.Instance.isShootingUnlocked && PlayerStore.Instance.isAugIntroduced)
		{
			AudioManager.Instance.Play2("shotgun_pump");
			PlayerStore.Instance.isShootingUnlocked = true;
			LoadShotgun();
		}
	}

	public static void ReturnIfLostWithPermit()
	{
		if (PermitHelper.IsGunPermitI())
		{
			ReturnIfLost();
		}
	}

	public static bool WouldPermitRevocationTakeGun()
	{
		if (PlayerStore.Instance.isShootingUnlocked && PermitHelper.IsGunPermitI())
		{
			return StoreReputation.GetSecReputation() < 0;
		}
		return false;
	}
}
