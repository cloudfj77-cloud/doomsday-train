using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ClienteleUIManager : MonoBehaviour
{
	public GameObject panel;

	public TextMeshProUGUI merchandiseCustomerTMP;

	public TextMeshProUGUI normalCustomerTitleTMP;

	public TextMeshProUGUI normalCustomerTMP;

	public static ClienteleUIManager Instance { get; private set; }

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
	}

	public void ToggleUI()
	{
		if (panel.activeSelf)
		{
			CloseUI();
		}
		else
		{
			OpenUI();
		}
	}

	public void OpenUI()
	{
		panel.SetActive(value: true);
		EmporiumAudioPlayer.Instance.audioButton1.Play();
		RefreshUI();
		TutorialUIManager.Instance.OnClienteleUIOpened();
	}

	public void CloseUI()
	{
		panel.SetActive(value: false);
		EmporiumAudioPlayer.Instance.audioButton1.Play();
	}

	public void RefreshUI()
	{
		if (!panel.activeSelf)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		List<string> displayCaseType = ShowcaseHelper.GetDisplayCaseType();
		int discerningShopperChance = ShowcaseHelper.GetDiscerningShopperChance();
		int discerningShopperCooldownOpenings = ShowcaseHelper.GetDiscerningShopperCooldownOpenings();
		if (displayCaseType.Count == 0 && discerningShopperChance == 0 && discerningShopperCooldownOpenings == 0)
		{
			merchandiseCustomerTMP.text = LocHelper.GetLocalizedUI("ui_clientele_no_merch_detected");
		}
		else
		{
			foreach (string item4 in displayCaseType)
			{
				stringBuilder.Append("- ").Append(LocHelper.GetLocalizedUI("ui_clientele_buyer") + " (" + TypeHelper.GetTypeDisplayName(item4) + ")").Append('\n');
			}
			string text = StoreClientListDict.GetClientDisplayName("ulDiscerningShopper") + " (" + TypeHelper.GetTypeDisplayName("LUXURY_ITEM") + ")";
			if (discerningShopperCooldownOpenings > 0)
			{
				stringBuilder.Append("- ").Append(LocHelper.GetLocalizedMechanic("mech_service_cooldown", text, discerningShopperCooldownOpenings)).Append('\n');
			}
			else if (discerningShopperChance > 0)
			{
				stringBuilder.Append("- ").Append(text).Append(" (")
					.Append(discerningShopperChance)
					.Append("%)")
					.Append('\n');
			}
			merchandiseCustomerTMP.text = stringBuilder.ToString();
		}
		normalCustomerTitleTMP.text = LocHelper.GetLocalizedUI("ui_clientele_random", PlayerStore.Instance.storeClientManager.GetNormalCustomerCount()) + " <link=\"ui_clientele_normal_tooltip\"><sprite name=\"info\" tint=1></link>";
		StringBuilder stringBuilder2 = new StringBuilder();
		List<(string, int, string)> displayCasePrediction = ShowcaseHelper.GetDisplayCasePrediction();
		if (displayCasePrediction.Count == 0)
		{
			int normalCustomerCount = PlayerStore.Instance.storeClientManager.GetNormalCustomerCount();
			for (int i = 0; i < normalCustomerCount; i++)
			{
				stringBuilder2.Append("- ").Append(LocHelper.GetLocalizedUI("ui_clientele_random_customer")).Append('\n');
			}
		}
		else
		{
			List<GameItem> childItems = EmporiumEntry.Instance.showcaseElement.childItems;
			foreach (var item5 in displayCasePrediction)
			{
				string item2 = item5.Item1;
				int item3 = item5.Item2;
				string itemId = item5.Item3;
				GameItem gameItem = childItems.Find((GameItem item) => item.identifier.Equals(itemId));
				if (gameItem == null)
				{
					stringBuilder2.Append("- ").Append(StoreClientListDict.GetClientDisplayName(item2)).Append(" (")
						.Append(item3)
						.Append("%")
						.Append(ShowcaseHelper.GetChanceSuffix(item2))
						.Append(")")
						.Append('\n');
					continue;
				}
				if (!gameItem.IsTag("SHOWCASE_SPECIAL_ITEM_TAG"))
				{
					stringBuilder2.Append("- ").Append(GeneralHelper.GetDisplayNameFromID(itemId) + ": ").Append(StoreClientListDict.GetClientDisplayName(item2))
						.Append(" (")
						.Append(item3)
						.Append("%")
						.Append(ShowcaseHelper.GetChanceSuffix(item2))
						.Append(")")
						.Append('\n');
					continue;
				}
				string @string = gameItem.GetTagReadonly("SHOWCASE_SPECIAL_ITEM_TAG").GetString();
				stringBuilder2.Append("- ").Append(GeneralHelper.GetDisplayNameFromID(itemId) + ": ").Append(LocHelper.GetLocalizedUI("ui_clientele_seller") + " (" + TypeHelper.GetTypeDisplayName(@string) + ")")
					.Append(" (")
					.Append(item3)
					.Append("%")
					.Append(ShowcaseHelper.GetChanceSuffix(item2))
					.Append(")")
					.Append('\n');
			}
		}
		normalCustomerTMP.text = stringBuilder2.ToString();
	}
}
