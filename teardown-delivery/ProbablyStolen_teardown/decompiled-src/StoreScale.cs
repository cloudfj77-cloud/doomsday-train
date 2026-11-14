using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoreScale : NegociationElement, IClickable
{
	public TextMeshProUGUI weightTMP;

	public static StoreScale Instance { get; private set; }

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

	public void UpdateScaleDisplay()
	{
		if (EmporiumEntry.Instance.backInvinvElement.childItems.Count == 0)
		{
			weightTMP.text = "0.00 kg";
			return;
		}
		int num = 0;
		foreach (GameItem childItem in EmporiumEntry.Instance.backInvinvElement.childItems)
		{
			num += GeneralHelper.GetWeight(childItem);
		}
		weightTMP.text = ConvertGramToKilo(num);
	}

	public static string ConvertGramToKilo(int value)
	{
		double num = (double)value / 1000.0;
		return $"{num:F2} kg";
	}

	public override string GetNegociationToolID()
	{
		return "NEGO_TOOL_SCALE";
	}

	public override bool IsNegociationTool()
	{
		return true;
	}

	public override List<NegociationData> GetNegociationData()
	{
		return null;
	}

	public void OnClick()
	{
		Debug.Log("Scale Clicked");
		if (EmporiumEntry.Instance.backInvinvElement.childItems.Count == 1 && PlayerStore.Instance.currentNegociatedItem.FindItemFeatureByCategory("weight") != null)
		{
			PlayerStore.Instance.currentNegociatedItem.FindItemFeatureByCategory("weight").DiscoverFeature();
		}
	}
}
