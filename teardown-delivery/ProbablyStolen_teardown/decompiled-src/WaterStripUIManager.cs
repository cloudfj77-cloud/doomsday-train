using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaterStripUIManager : MonoBehaviour
{
	public Button confrontButton;

	public Button matchButton;

	public GameObject panel;

	public TextMeshProUGUI testedTMP;

	public TextMeshProUGUI labelTMP;

	public TextMeshProUGUI purityTMP;

	public Image qualityStrip;

	public GameItem currentWaterContainer;

	public static WaterStripUIManager Instance { get; private set; }

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

	public void Open(GameItem testStrip)
	{
		panel.SetActive(value: true);
		PaperPanelFocusStamp.BumpPanel(panel);
		int @int = testStrip.GetTagReadonly("TEST_WATER_QUALITY").GetInt();
		int int2 = testStrip.GetTagReadonly("ASSOCIATED_ITEM_ID").GetInt();
		testedTMP.text = LocHelper.GetLocalizedMechanic("mech_tooltip_tested") + "\n" + WaterFeatureHelper.GetWaterDisplayName(@int);
		purityTMP.text = WaterFeatureHelper.FormatPurityPercentage(@int);
		GameItem gameItem = (currentWaterContainer = EmporiumEntry.Instance.FindItemByUniqueID(int2));
		ItemFeature waterFeature = WaterFeatureHelper.GetWaterFeature(gameItem);
		labelTMP.text = LocHelper.GetLocalizedMechanic("mech_tooltip_labeled") + "\n" + waterFeature.GetCurrentItemCondition().display;
		EmporiumAudioPlayer.Instance.audioButton2.Play();
		if (WaterFeatureHelper.GetWaterDisplayName(@int).Equals(WaterFeatureHelper.PURE_WATER_DISPLAY))
		{
			qualityStrip.color = MetaHelper.HexToColor("3d823f");
			testStrip.SetSpriteAndShape("Items/items_amenities", "strip0");
		}
		else if (WaterFeatureHelper.GetWaterDisplayName(@int).Equals(WaterFeatureHelper.HIGH_QUALITY_WATER_DISPLAY))
		{
			qualityStrip.color = MetaHelper.HexToColor("7ab053");
			testStrip.SetSpriteAndShape("Items/items_amenities", "strip1");
		}
		else if (WaterFeatureHelper.GetWaterDisplayName(@int).Equals(WaterFeatureHelper.BASE_WATER_DISPLAY))
		{
			qualityStrip.color = MetaHelper.HexToColor("ccb987");
			testStrip.SetSpriteAndShape("Items/items_amenities", "strip2");
		}
		else if (WaterFeatureHelper.GetWaterDisplayName(@int).Equals(WaterFeatureHelper.GHOSTWATER_DISPLAY))
		{
			qualityStrip.color = MetaHelper.HexToColor("ba8953");
			testStrip.SetSpriteAndShape("Items/items_amenities", "strip3");
		}
		else if (WaterFeatureHelper.GetWaterDisplayName(@int).Equals(WaterFeatureHelper.RUST_WATER_DISPLAY))
		{
			qualityStrip.color = MetaHelper.HexToColor("9b4a3d");
			testStrip.SetSpriteAndShape("Items/items_amenities", "strip4");
		}
		else if (WaterFeatureHelper.GetWaterDisplayName(@int).Equals(WaterFeatureHelper.GUTTERFLOW_WATER_DISPLAY))
		{
			qualityStrip.color = MetaHelper.HexToColor("74313b");
			testStrip.SetSpriteAndShape("Items/items_amenities", "strip5");
		}
		if (CanConfront(gameItem))
		{
			confrontButton.gameObject.SetActive(value: true);
		}
		else
		{
			confrontButton.gameObject.SetActive(value: false);
		}
		if (CanMatch(gameItem))
		{
			matchButton.gameObject.SetActive(value: true);
		}
		else
		{
			matchButton.gameObject.SetActive(value: false);
		}
	}

	public void TestWater(GameItem container)
	{
	}

	public void Close()
	{
		panel.SetActive(value: false);
		EmporiumAudioPlayer.Instance.audioButton4.Play();
	}

	public void OnConfront()
	{
		if (currentWaterContainer != null && CanConfront(currentWaterContainer))
		{
			ItemFeature waterFeature = WaterFeatureHelper.GetWaterFeature(currentWaterContainer);
			if (DialogUIManager.Instance.StartDialogue(waterFeature.GetExposeDialog()))
			{
				InspectableHelper.OnConfrontSuccess(currentWaterContainer);
				confrontButton.gameObject.SetActive(value: false);
			}
		}
	}

	public bool CanConfront(GameItem associatedWaterContainer)
	{
		ItemFeature waterFeature = WaterFeatureHelper.GetWaterFeature(associatedWaterContainer);
		if (!waterFeature.CanExposeFeature())
		{
			return false;
		}
		if (InspectionUIManager.IsConfrontLocked())
		{
			return false;
		}
		if (GeneralHelper.IsItemOwned(associatedWaterContainer))
		{
			return false;
		}
		if (waterFeature.IsExposed())
		{
			return false;
		}
		if (waterFeature.IsFeatureMatch())
		{
			return false;
		}
		return true;
	}

	public bool CanMatch(GameItem associatedWaterContainer)
	{
		if (WaterHelper.IsEmpty(associatedWaterContainer))
		{
			return false;
		}
		ItemFeature waterFeature = WaterFeatureHelper.GetWaterFeature(associatedWaterContainer);
		if (!GeneralHelper.IsItemOwned(associatedWaterContainer))
		{
			return false;
		}
		if (waterFeature.IsExposed())
		{
			return false;
		}
		if (waterFeature.IsFeatureMatch())
		{
			return false;
		}
		return true;
	}

	public void OnMatch()
	{
		if (currentWaterContainer != null && CanMatch(currentWaterContainer))
		{
			WaterFeatureHelper.GetWaterFeature(currentWaterContainer).ExposeFeature(lowkey: true);
			WaterFeatureHelper.OnWaterLabelUpdated(currentWaterContainer);
			matchButton.gameObject.SetActive(value: false);
			labelTMP.text = testedTMP.text;
			labelTMP.text = Regex.Replace(labelTMP.text, "\\[[^\\]]*\\]", LocHelper.GetLocalizedMechanic("mech_tooltip_labeled"));
			EmporiumAudioPlayer.Instance.audioLabeler.Play();
		}
	}
}
