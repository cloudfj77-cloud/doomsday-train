using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaterScannerUIManager : MonoBehaviour
{
	public Button confrontButton;

	public Button matchButton;

	public GameObject panel;

	public TextMeshProUGUI testedTMP;

	public TextMeshProUGUI labelTMP;

	public TextMeshProUGUI contaminantTMP;

	public GameItem currentWaterContainer;

	public Coroutine coroutine;

	public static WaterScannerUIManager Instance { get; private set; }

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

	private IEnumerator CallFunctionWhileCondition()
	{
		while (ConditionMet())
		{
			RefreshUI();
			yield return new WaitForSeconds(1f);
		}
	}

	private bool ConditionMet()
	{
		return panel.activeSelf;
	}

	public void Open(GameItem scanner, GameItem water)
	{
		if (water != null)
		{
			panel.SetActive(value: true);
			PaperPanelFocusStamp.BumpPanel(panel);
			currentWaterContainer = water;
			RefreshUI();
			coroutine = StartCoroutine(CallFunctionWhileCondition());
			EmporiumAudioPlayer.Instance.audioButton2.Play();
		}
	}

	public void RefreshUI()
	{
		if (currentWaterContainer == null)
		{
			testedTMP.text = "";
			labelTMP.text = "";
			contaminantTMP.text = "";
			matchButton.gameObject.SetActive(value: false);
			matchButton.gameObject.SetActive(value: false);
			return;
		}
		testedTMP.text = GetTestedString(currentWaterContainer);
		labelTMP.text = GetLabeledString(currentWaterContainer);
		WaterHelper.SetLastScanResult(currentWaterContainer);
		if (WaterHelper.IsEmpty(currentWaterContainer))
		{
			testedTMP.text = "";
			labelTMP.text = "";
			contaminantTMP.text = "";
			matchButton.gameObject.SetActive(value: false);
			return;
		}
		if (CanConfront(currentWaterContainer))
		{
			confrontButton.gameObject.SetActive(value: true);
		}
		else
		{
			confrontButton.gameObject.SetActive(value: false);
		}
		if (CanMatch(currentWaterContainer))
		{
			matchButton.gameObject.SetActive(value: true);
		}
		else
		{
			matchButton.gameObject.SetActive(value: false);
		}
		int num = 0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			num += currentWaterContainer.GetTagReadonly(liquid.currentPart).GetInt();
		}
		contaminantTMP.text = "";
		for (int i = 0; i < Liquid.Liquids.Count; i++)
		{
			int @int = currentWaterContainer.GetTagReadonly(Liquid.Liquids[i].currentPart).GetInt();
			if (@int > 0)
			{
				string displayName = Liquid.Liquids[i].displayName;
				float num2 = (float)@int / (float)num * 100f;
				contaminantTMP.text += $"{displayName}: {FormatAmount(@int)} ({num2:F2}%)\n";
			}
		}
	}

	public static string GetTestedString(GameItem currentWaterContainer)
	{
		int waterPurity = WaterHelper.GetWaterPurity(currentWaterContainer);
		return LocHelper.GetLocalizedMechanic("mech_tooltip_tested") + "\n" + WaterFeatureHelper.FormatPurityPercentage(waterPurity) + "\n" + WaterFeatureHelper.GetWaterDisplayName(waterPurity);
	}

	public static string GetLabeledString(GameItem currentWaterContainer)
	{
		ItemFeature waterFeature = WaterFeatureHelper.GetWaterFeature(currentWaterContainer);
		return LocHelper.GetLocalizedMechanic("mech_tooltip_labeled") + "\n" + waterFeature.GetCurrentItemCondition().display;
	}

	public static string FormatAmount(int currentAmount)
	{
		if (currentAmount >= 1000000)
		{
			return ((double)(currentAmount / 10000) / 100.0).ToString("0.00") + " L";
		}
		if (currentAmount >= 1000)
		{
			return ((double)(currentAmount / 10) / 100.0).ToString("0.00") + " mL";
		}
		return currentAmount + " uL";
	}

	public void Close()
	{
		panel.SetActive(value: false);
		EmporiumAudioPlayer.Instance.audioButton4.Play();
		WaterHelper.SetLastScanResult(currentWaterContainer);
		StopCoroutine(coroutine);
		currentWaterContainer = null;
	}

	public void OnConfront()
	{
		if (currentWaterContainer != null && CanConfront(currentWaterContainer))
		{
			ConfrontWater(currentWaterContainer, confrontButton);
		}
	}

	public static void ConfrontWater(GameItem waterContainer, Button confrontButton)
	{
		ItemFeature waterFeature = WaterFeatureHelper.GetWaterFeature(waterContainer);
		if (DialogUIManager.Instance.StartDialogue(waterFeature.GetExposeDialog()))
		{
			InspectableHelper.OnConfrontSuccess(waterContainer);
			confrontButton.gameObject.SetActive(value: false);
		}
	}

	public static bool CanConfront(GameItem associatedWaterContainer)
	{
		if (WaterHelper.IsEmpty(associatedWaterContainer))
		{
			return false;
		}
		if (InspectionUIManager.IsConfrontLocked())
		{
			return false;
		}
		ItemFeature waterFeature = WaterFeatureHelper.GetWaterFeature(associatedWaterContainer);
		if (!waterFeature.CanExposeFeature())
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

	public static bool CanMatch(GameItem associatedWaterContainer)
	{
		ItemFeature waterFeature = WaterFeatureHelper.GetWaterFeature(associatedWaterContainer);
		if (WaterHelper.IsEmpty(associatedWaterContainer))
		{
			return false;
		}
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
			MatchWaterLabel(currentWaterContainer, matchButton, labelTMP, testedTMP);
		}
	}

	public static void MatchWaterLabel(GameItem waterContainer, Button matchButton, TextMeshProUGUI labelTMP, TextMeshProUGUI testedTMP)
	{
		WaterFeatureHelper.GetWaterFeature(waterContainer).ExposeFeature(lowkey: true);
		WaterFeatureHelper.OnWaterLabelUpdated(waterContainer);
		matchButton.gameObject.SetActive(value: false);
		labelTMP.text = testedTMP.text;
		labelTMP.text = Regex.Replace(labelTMP.text, "\\[[^\\]]*\\]", LocHelper.GetLocalizedMechanic("mech_tooltip_labeled"));
		EmporiumAudioPlayer.Instance.audioLabeler.Play();
	}
}
