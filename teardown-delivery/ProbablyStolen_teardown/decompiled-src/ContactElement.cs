using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContactElement : MonoBehaviour
{
	public TextMeshProUGUI titleTMP;

	public TextMeshProUGUI statusTMP;

	public long number;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnQuickDial()
	{
		PhoneUIManager.Instance.OpenUI();
		PhoneUIManager.Instance.AutoCall(number);
	}

	public void OnInit(long targetNumber)
	{
		number = targetNumber;
		Button componentInChildren = GetComponentInChildren<Button>(includeInactive: true);
		if (componentInChildren != null)
		{
			Navigation navigation = componentInChildren.navigation;
			navigation.mode = Navigation.Mode.None;
			componentInChildren.navigation = navigation;
		}
		if (PlayerStore.Instance.PhoneClientDict.TryGetValue(targetNumber, out var value))
		{
			titleTMP.text = value.GetLocalizedDisplayName();
			StorePhoneClient storePhoneClient = PlayerStore.Instance.PhoneClientDict[targetNumber];
			if (storePhoneClient.phoneState == StorePhoneClient.PhoneState.Regular)
			{
				statusTMP.text = LocHelper.GetLocalizedMechanic("mech_contact_ready");
			}
			else if (storePhoneClient.phoneState == StorePhoneClient.PhoneState.Cooldown)
			{
				string key = ((storePhoneClient.currentCooldown == 1) ? "mech_contact_cooldown_1" : "mech_contact_cooldown_n");
				statusTMP.text = LocHelper.GetLocalizedMechanic(key, storePhoneClient.currentCooldown);
			}
			else
			{
				statusTMP.text = LocHelper.GetLocalizedMechanic("mech_contact_unknown");
			}
		}
		else
		{
			titleTMP.text = LocHelper.GetLocalizedMechanic("mech_contact_unknown");
			statusTMP.text = LocHelper.GetLocalizedMechanic("mech_contact_unknown");
		}
	}
}
