using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChemScannerUIManager : MonoBehaviour
{
	public Button confrontButton;

	public Button matchButton;

	public GameObject panel;

	public TextMeshProUGUI productNameTMP;

	public TextMeshProUGUI testedTMP;

	public TextMeshProUGUI labelTMP;

	public TextMeshProUGUI contaminantTMP;

	public TextMeshProUGUI totalVolumeTMP;

	public GameItem currentScannedItem;

	public Coroutine coroutine;

	public ChemScannerEffect euphoriaEffect;

	public ChemScannerEffect toxicityEffect;

	public ChemScannerEffect synaprestEffect;

	public ChemScannerEffect birthControlEffect;

	public static ChemScannerUIManager Instance { get; private set; }

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
			currentScannedItem = water;
			RefreshUI();
			coroutine = StartCoroutine(CallFunctionWhileCondition());
			EmporiumAudioPlayer.Instance.audioButton2.Play();
		}
	}

	public void RefreshUI()
	{
		if (currentScannedItem == null)
		{
			testedTMP.text = "";
			labelTMP.text = "";
			contaminantTMP.text = "";
			totalVolumeTMP.text = "";
			matchButton.gameObject.SetActive(value: false);
			matchButton.gameObject.SetActive(value: false);
			euphoriaEffect.gameObject.SetActive(value: false);
			toxicityEffect.gameObject.SetActive(value: false);
			synaprestEffect.gameObject.SetActive(value: false);
			birthControlEffect.gameObject.SetActive(value: false);
			return;
		}
		productNameTMP.text = currentScannedItem.name;
		euphoriaEffect.gameObject.SetActive(value: false);
		toxicityEffect.gameObject.SetActive(value: false);
		synaprestEffect.gameObject.SetActive(value: false);
		birthControlEffect.gameObject.SetActive(value: false);
		if (currentScannedItem.IsTag("CHEM_ITEM_TAG"))
		{
			RefreshChemMode();
		}
		else
		{
			RefreshWaterMode();
		}
		int num = 0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			num += currentScannedItem.GetTagReadonly(liquid.currentPart).GetInt();
		}
		contaminantTMP.text = "";
		for (int i = 0; i < Liquid.Liquids.Count; i++)
		{
			int @int = currentScannedItem.GetTagReadonly(Liquid.Liquids[i].currentPart).GetInt();
			if (@int > 0)
			{
				string displayName = Liquid.Liquids[i].displayName;
				float num2 = (float)@int / (float)num * 100f;
				contaminantTMP.text += $"{displayName}: {WaterScannerUIManager.FormatAmount(@int)} ({num2:F2}%)\n";
			}
		}
		totalVolumeTMP.text = ((num > 0) ? LocHelper.GetLocalizedMechanic("mech_scanner_total_volume", WaterScannerUIManager.FormatAmount(num)) : "");
		if (ChemicalFeedbackHelper.IsLethal(currentScannedItem))
		{
			toxicityEffect.gameObject.SetActive(value: true);
			toxicityEffect.effectTMP.text = LocHelper.GetLocalizedMechanic("mech_scanner_toxicity_lethal") + " (x" + ChemicalFeedbackHelper.GetLethalDose(currentScannedItem) + ")";
		}
		else if (ChemicalFeedbackHelper.IsToxic(currentScannedItem))
		{
			toxicityEffect.gameObject.SetActive(value: true);
			toxicityEffect.effectTMP.text = LocHelper.GetLocalizedMechanic("mech_scanner_toxicity_toxic");
		}
		if (ChemicalFeedbackHelper.IsNarcotic(currentScannedItem))
		{
			switch (ChemicalFeedbackHelper.GetEuphoriaLevel(currentScannedItem))
			{
			case 1:
				euphoriaEffect.gameObject.SetActive(value: true);
				euphoriaEffect.effectTMP.text = LocHelper.GetLocalizedMechanic("mech_scanner_euphoria_weak");
				break;
			case 2:
				euphoriaEffect.gameObject.SetActive(value: true);
				euphoriaEffect.effectTMP.text = LocHelper.GetLocalizedMechanic("mech_scanner_euphoria_mild");
				break;
			case 3:
				euphoriaEffect.gameObject.SetActive(value: true);
				euphoriaEffect.effectTMP.text = LocHelper.GetLocalizedMechanic("mech_scanner_euphoria_strong");
				break;
			case 4:
				euphoriaEffect.gameObject.SetActive(value: true);
				euphoriaEffect.effectTMP.text = LocHelper.GetLocalizedMechanic("mech_scanner_euphoria_extreme");
				break;
			}
		}
		if (ChemicalFeedbackHelper.IsSynaprest(currentScannedItem))
		{
			synaprestEffect.gameObject.SetActive(value: true);
			synaprestEffect.effectTMP.text = LocHelper.GetLocalizedMechanic("mech_scanner_anti_addiction");
		}
		if (ChemicalFeedbackHelper.IsBirthControl(currentScannedItem))
		{
			birthControlEffect.gameObject.SetActive(value: true);
			birthControlEffect.effectTMP.text = LocHelper.GetLocalizedMechanic("mech_scanner_contraceptive");
		}
	}

	public void RefreshChemMode()
	{
		if (CanConfrontChem(currentScannedItem))
		{
			confrontButton.gameObject.SetActive(value: true);
		}
		else
		{
			confrontButton.gameObject.SetActive(value: false);
		}
		if (CanMatchChem(currentScannedItem))
		{
			matchButton.gameObject.SetActive(value: true);
		}
		else
		{
			matchButton.gameObject.SetActive(value: false);
		}
		if (currentScannedItem.IsTag("CHEMICAL_PRODUCT_TAG"))
		{
			double @double = currentScannedItem.GetTagReadonly("CHEMICAL_PRODUCT_PURITY_TAG").GetDouble();
			testedTMP.text = LocHelper.GetLocalizedMechanic("mech_tooltip_tested") + "\n" + WaterFeatureHelper.FormatPurityPercentageFromDouble(@double) + "\n" + currentScannedItem.FindItemFeatureByCategory("CATEGORY_CHEMICAL_PURITY").realCondition.display;
			labelTMP.text = LocHelper.GetLocalizedMechanic("mech_tooltip_labeled") + "\n" + currentScannedItem.FindItemFeatureByCategory("CATEGORY_CHEMICAL_PURITY").fakeCondition.display;
		}
		else
		{
			testedTMP.text = "";
			labelTMP.text = "";
		}
	}

	public void RefreshWaterMode()
	{
		if (WaterScannerUIManager.CanConfront(currentScannedItem))
		{
			confrontButton.gameObject.SetActive(value: true);
		}
		else
		{
			confrontButton.gameObject.SetActive(value: false);
		}
		if (WaterScannerUIManager.CanMatch(currentScannedItem))
		{
			matchButton.gameObject.SetActive(value: true);
		}
		else
		{
			matchButton.gameObject.SetActive(value: false);
		}
		testedTMP.text = WaterScannerUIManager.GetTestedString(currentScannedItem);
		labelTMP.text = WaterScannerUIManager.GetLabeledString(currentScannedItem);
		if (WaterHelper.IsEmpty(currentScannedItem))
		{
			testedTMP.text = "";
			labelTMP.text = "";
			contaminantTMP.text = "";
			matchButton.gameObject.SetActive(value: false);
		}
	}

	public void OnConfront()
	{
		if (currentScannedItem != null && !InspectionUIManager.IsConfrontLocked())
		{
			if (currentScannedItem.IsTag("LIQUID_CONTAINER_TAG") && WaterScannerUIManager.CanConfront(currentScannedItem))
			{
				WaterScannerUIManager.ConfrontWater(currentScannedItem, confrontButton);
			}
			else if (currentScannedItem.IsTag("CHEM_ITEM_TAG"))
			{
				ConFrontChem(currentScannedItem, confrontButton);
			}
		}
	}

	public static void ConFrontChem(GameItem associatedItem, Button confrontButton)
	{
		ItemFeature chemicalFeature = ChemicalProductHelper.GetChemicalFeature(associatedItem);
		if (DialogUIManager.Instance.StartDialogue(chemicalFeature.GetExposeDialog()))
		{
			InspectableHelper.OnConfrontSuccess(associatedItem);
			confrontButton.gameObject.SetActive(value: false);
		}
	}

	public static bool CanConfrontChem(GameItem associatedItem)
	{
		ItemFeature chemicalFeature = ChemicalProductHelper.GetChemicalFeature(associatedItem);
		if (chemicalFeature == null)
		{
			return false;
		}
		if (InspectionUIManager.IsConfrontLocked())
		{
			return false;
		}
		if (!chemicalFeature.CanExposeFeature())
		{
			return false;
		}
		if (GeneralHelper.IsItemOwned(associatedItem))
		{
			return false;
		}
		if (chemicalFeature.IsExposed())
		{
			return false;
		}
		if (chemicalFeature.IsFeatureMatch())
		{
			return false;
		}
		return true;
	}

	public bool CanMatchChem(GameItem chemItem)
	{
		ItemFeature chemicalFeature = ChemicalProductHelper.GetChemicalFeature(chemItem);
		if (chemicalFeature == null)
		{
			return false;
		}
		if (chemicalFeature.realCondition.identifier.Equals(chemicalFeature.fakeCondition.identifier))
		{
			return false;
		}
		return true;
	}

	public void OnMatch()
	{
		if (currentScannedItem != null)
		{
			if (currentScannedItem.IsTag("LIQUID_CONTAINER_TAG") && WaterScannerUIManager.CanMatch(currentScannedItem))
			{
				WaterScannerUIManager.MatchWaterLabel(currentScannedItem, matchButton, labelTMP, testedTMP);
			}
			else if (currentScannedItem.IsTag("CHEM_ITEM_TAG"))
			{
				MatchChemLabel(currentScannedItem, matchButton, labelTMP, testedTMP);
			}
		}
	}

	public void Close()
	{
		panel.SetActive(value: false);
		EmporiumAudioPlayer.Instance.audioButton4.Play();
		StopCoroutine(coroutine);
		currentScannedItem = null;
	}

	public static void MatchChemLabel(GameItem chemContainer, Button matchButton, TextMeshProUGUI labelTMP, TextMeshProUGUI testedTMP)
	{
		ChemicalProductHelper.GetChemicalFeature(chemContainer).ExposeFeature(lowkey: true);
		ChemicalProductHelper.GradeProduct(chemContainer);
		matchButton.gameObject.SetActive(value: false);
		labelTMP.text = testedTMP.text;
		labelTMP.text = Regex.Replace(labelTMP.text, "\\[[^\\]]*\\]", LocHelper.GetLocalizedMechanic("mech_tooltip_labeled"));
		EmporiumAudioPlayer.Instance.audioLabeler.Play();
	}
}
