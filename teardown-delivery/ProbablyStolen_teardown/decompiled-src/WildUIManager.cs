using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WildUIManager : MonoBehaviour
{
	public GameObject panel;

	public List<NetworkElement> networkElements = new List<NetworkElement>();

	public TextMeshProUGUI title;

	public TextMeshProUGUI creditCount;

	public TextMeshProUGUI subtitle;

	public TextMeshProUGUI description;

	public TextMeshProUGUI cooldown;

	public TextMeshProUGUI cost;

	public Button unlockButton;

	public TextMeshProUGUI alreadyMessageTMP;

	public TextMeshProUGUI wildFavor;

	public string currentSelectedId;

	public static WildUIManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		creditCount.text = LocHelper.GetLocalizedUI("network_ui_available_credit", PlayerStore.Instance.playerCash);
		wildFavor.text = LocHelper.GetLocalizedUI("network_ui_wild_favor", PlayerStore.Instance.wildFavor) + " <sprite name=\"info\" tint=1>";
	}

	public void OpenUI(bool debug = false)
	{
		title.text = "";
		subtitle.text = "";
		description.text = "";
		cooldown.text = "";
		cost.text = "";
		alreadyMessageTMP.text = "";
		unlockButton.gameObject.SetActive(value: false);
		currentSelectedId = "";
		panel.SetActive(value: true);
		PlayerStore.Instance.daySinceLastBrowseUpgrade = 0;
		if (!debug)
		{
			EmporiumAnalytics.TrackWildUIOpened();
		}
	}

	public void CloseUI()
	{
		if (TutorialUIManager.Instance.isIntroBrowsingUpgrade)
		{
			TutorialUIManager.Instance.isIntroBrowsingUpgrade = false;
			DialogUIManager.Instance.StartDialogue(StoreClientDialogList.WildNetworkContinue());
		}
		else if (PlayerStore.Instance.currentClientInstance != null && PlayerStore.Instance.currentClientInstance.GetClientBlueprint().identifier.Equals("wild_upgrade_reminder"))
		{
			PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
		}
		EmporiumAudioPlayer.Instance.audioButton1.Play();
	}

	public void OnCloseButtonClicked()
	{
		panel.SetActive(value: false);
	}

	public void OnElementClicked(string id)
	{
		EmporiumAudioPlayer.Instance.audioLampClick.Play();
		unlockButton.gameObject.SetActive(value: true);
		NetworkUpgrade networkUpgrade = PlayerStore.Instance.networkUpgrade[id];
		if (networkUpgrade != null)
		{
			title.text = networkUpgrade.GetLocalizedTitle();
			subtitle.text = networkUpgrade.GetLocalizedSubtitle();
			description.text = networkUpgrade.GetLocalizedDescription();
			cooldown.text = ((networkUpgrade.cooldownDuration != 0 && networkUpgrade.cooldownCurrent != 0) ? LocHelper.GetLocalizedUI("network_ui_cooldown", networkUpgrade.cooldownCurrent) : "");
			cost.text = LocHelper.GetLocalizedUI("network_ui_cost", networkUpgrade.GetCost());
			currentSelectedId = id;
			if (!networkUpgrade.IsReady())
			{
				unlockButton.interactable = false;
			}
			else if (networkUpgrade.IsUnlocked())
			{
				unlockButton.interactable = false;
			}
			else if (networkUpgrade.GetCost() > PlayerStore.Instance.playerCash + PlayerStore.Instance.wildFavor)
			{
				unlockButton.interactable = false;
			}
			else
			{
				unlockButton.interactable = true;
			}
			if (networkUpgrade.isRepeatable && networkUpgrade.IsUnlocked())
			{
				alreadyMessageTMP.gameObject.SetActive(value: true);
				alreadyMessageTMP.text = networkUpgrade.GetLocalizedAlreadyBought();
				unlockButton.gameObject.SetActive(value: false);
			}
			else if (!networkUpgrade.IsUnlocked() && !networkUpgrade.IsHavePrerequisite())
			{
				alreadyMessageTMP.gameObject.SetActive(value: true);
				alreadyMessageTMP.text = LocHelper.GetLocalizedUI("network_ui_prerequisite", networkUpgrade.GetMissingPrerequisite());
				unlockButton.gameObject.SetActive(value: false);
			}
			else
			{
				alreadyMessageTMP.gameObject.SetActive(value: false);
				unlockButton.gameObject.SetActive(value: true);
			}
		}
	}

	public void OnUnlockClicked()
	{
		NetworkUpgrade networkUpgrade = PlayerStore.Instance.networkUpgrade[currentSelectedId];
		int num = networkUpgrade.GetCost();
		int cashSpent = 0;
		if (PlayerStore.Instance.wildFavor >= num)
		{
			PlayerStore.Instance.wildFavor -= num;
		}
		else
		{
			int num2 = num - PlayerStore.Instance.wildFavor;
			PlayerStore.Instance.wildFavor = 0;
			PlayerStore.Instance.playerCash -= num2;
			cashSpent = num2;
		}
		RunStats.OnWildUpgradeBought(cashSpent);
		networkUpgrade.Unlock();
		EmporiumAnalytics.TrackWildUpgradePurchased(currentSelectedId);
		EmporiumAudioPlayer.Instance.PlayRegister();
		OnElementClicked(currentSelectedId);
	}
}
