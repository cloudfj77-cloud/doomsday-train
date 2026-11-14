using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServiceElement : MonoBehaviour
{
	public TextMeshProUGUI titleTMP;

	public Toggle toggle;

	public TextMeshProUGUI costTMP;

	public string serviceID;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnToggled()
	{
		TutorialUIManager.Instance.DisableNightlyServiceTutorial();
		EmporiumAudioPlayer.Instance.audioButton1.Play();
		StoreService storeService = null;
		foreach (StoreService item in PlayerStore.Instance.storeService)
		{
			if (serviceID.Equals(item.id))
			{
				storeService = item;
			}
		}
		if (storeService != null)
		{
			storeService.isUsing = toggle.isOn;
		}
		EndOfDayUIManager.Instance.RefreshUI();
	}

	public void Init(string id)
	{
		serviceID = id;
		StoreService storeService = null;
		foreach (StoreService item in PlayerStore.Instance.storeService)
		{
			if (id.Equals(item.id))
			{
				storeService = item;
			}
		}
		if (storeService != null)
		{
			if (!storeService.unlocked)
			{
				base.gameObject.SetActive(value: false);
				return;
			}
			base.gameObject.SetActive(value: true);
			titleTMP.text = storeService.displayName;
			if (!string.IsNullOrEmpty(storeService.tooltip))
			{
				TextMeshProUGUI textMeshProUGUI = titleTMP;
				textMeshProUGUI.text = textMeshProUGUI.text + " <link=\"" + storeService.tooltip + "\"><sprite name=\"info\" tint=1></link>";
			}
			costTMP.text = LocHelper.GetLocalizedMechanic("mech_service_cost", storeService.cost);
			toggle.interactable = true;
			if (storeService.isMandatory)
			{
				toggle.interactable = false;
				storeService.isUsing = true;
			}
			toggle.isOn = storeService.isUsing;
			if (storeService.IsOnCooldown())
			{
				storeService.isUsing = false;
				toggle.interactable = false;
				toggle.isOn = false;
				titleTMP.text = LocHelper.GetLocalizedMechanic("mech_service_cooldown", storeService.displayName, storeService.currentCooldown);
				EndOfDayUIManager.Instance.RefreshUI();
			}
		}
		else
		{
			Debug.Log("A ServiceElement contain a serviceID that's invalid");
		}
	}
}
