using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlantAnalyserUIManager : MonoBehaviour
{
	public GameObject UIPanel;

	public TextMeshProUGUI titleTMP;

	public TextMeshProUGUI bodyTMP;

	public TextMeshProUGUI nutrimentRangeTMP;

	public TextMeshProUGUI nutrientUseTMP;

	public TextMeshProUGUI contaminantSensitivityTMP;

	public TextMeshProUGUI waterUsageTMP;

	public TextMeshProUGUI body2TMP;

	public Image growthScoreImage;

	public Image growthScoreTilledImage;

	public GameItem currentItem;

	public Coroutine coroutine;

	public static PlantAnalyserUIManager Instance { get; private set; }

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
			Refresh();
			yield return new WaitForSeconds(1f);
		}
	}

	private bool ConditionMet()
	{
		return UIPanel.activeSelf;
	}

	public void OpenUI(GameItem seedItem)
	{
		Reset();
		currentItem = seedItem;
		Refresh();
		coroutine = StartCoroutine(CallFunctionWhileCondition());
		UIPanel.gameObject.SetActive(value: true);
		PaperPanelFocusStamp.BumpPanel(UIPanel);
		EmporiumAudioPlayer.Instance.audioButton1.Play();
	}

	public void HideUI()
	{
		UIPanel.gameObject.SetActive(value: false);
		EmporiumAudioPlayer.Instance.audioButton1.Play();
		if (coroutine != null)
		{
			StopCoroutine(coroutine);
		}
	}

	public void Refresh()
	{
		titleTMP.text = currentItem.name;
		double num = (float)(HydroponicHelper.GetNutrimentLevel(currentItem) - HydroponicHelper.GetNutrimentRange(currentItem)) / 100f;
		double num2 = (float)(HydroponicHelper.GetNutrimentLevel(currentItem) + HydroponicHelper.GetNutrimentRange(currentItem)) / 100f;
		if (num < 0.0)
		{
			num = 0.0;
		}
		double num3 = (float)HydroponicHelper.GetMineralUseRate(currentItem) / 1000f;
		nutrimentRangeTMP.text = LocHelper.GetLocalizedMechanic("mech_seed_nutriment_range", num, num2);
		nutrientUseTMP.text = LocHelper.GetLocalizedMechanic("mech_seed_nutrient_use", num3);
		waterUsageTMP.text = LocHelper.GetLocalizedMechanic("mech_seed_water_usage", HydroponicHelper.GetWaterUsePerCycle(currentItem));
		contaminantSensitivityTMP.text = LocHelper.GetLocalizedMechanic("mech_seed_contaminant", GetContaminantSentitivity(currentItem));
		if (currentItem.IsTag("GROWING_TAG"))
		{
			body2TMP.text = LocHelper.GetLocalizedMechanic("mech_seed_growth_score", (float)currentItem.GetTagReadonly("PLANT_HEALTH_INT").GetInt() / 100f);
			growthScoreImage.gameObject.SetActive(value: true);
			growthScoreTilledImage.gameObject.SetActive(value: true);
			growthScoreTilledImage.fillAmount = (float)currentItem.GetTagReadonly("PLANT_HEALTH_INT").GetInt() / 10000f;
		}
		else
		{
			growthScoreImage.gameObject.SetActive(value: false);
			growthScoreTilledImage.gameObject.SetActive(value: false);
		}
	}

	public string GetContaminantSentitivity(GameItem seedItem)
	{
		double num = HydroponicHelper.GetContaminantEffect(seedItem) * HydroponicHelper.GetContaminantSensitivity(seedItem);
		if (num == 0.0)
		{
			return LocHelper.GetLocalizedMechanic("mech_seed_sensitivity_immune");
		}
		if (num.Equals(1.0))
		{
			return LocHelper.GetLocalizedMechanic("mech_seed_sensitivity_average");
		}
		if (num >= 1.5)
		{
			return LocHelper.GetLocalizedMechanic("mech_seed_sensitivity_very_sensitive");
		}
		if (num > 1.25)
		{
			return LocHelper.GetLocalizedMechanic("mech_seed_sensitivity_sensitive");
		}
		if (num > 1.0)
		{
			return LocHelper.GetLocalizedMechanic("mech_seed_sensitivity_slightly_sensitive");
		}
		if (num <= 0.5)
		{
			return LocHelper.GetLocalizedMechanic("mech_seed_sensitivity_very_resistant");
		}
		if (num < 0.75)
		{
			return LocHelper.GetLocalizedMechanic("mech_seed_sensitivity_resistant");
		}
		if (num < 1.0)
		{
			return LocHelper.GetLocalizedMechanic("mech_seed_sensitivity_slightly_resistant");
		}
		return LocHelper.GetLocalizedMechanic("mech_seed_sensitivity_unknown");
	}

	public void Reset()
	{
		titleTMP.text = "";
		nutrimentRangeTMP.text = "";
		nutrientUseTMP.text = "";
		contaminantSensitivityTMP.text = "";
		waterUsageTMP.text = "";
		body2TMP.text = "";
	}
}
