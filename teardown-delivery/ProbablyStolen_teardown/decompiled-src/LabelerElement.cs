using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LabelerElement : MonoBehaviour
{
	public TextMeshProUGUI featureName;

	public TextMeshProUGUI currentFeatureDisplay;

	public TMP_Dropdown dropdown;

	private ItemFeature thisFeature;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void InitElement(ItemFeature itemFeature)
	{
		thisFeature = itemFeature;
		featureName.text = ItemFeatureList.CategoryNames[itemFeature.category];
		currentFeatureDisplay.text = LocHelper.GetLocalizedMechanic("mech_label_condition_value", itemFeature.GetCurrentItemCondition().display, itemFeature.GetCurrentItemCondition().GetModValueFormated());
		dropdown.AddOptions(CreateOptionList(itemFeature.category));
		SetCurrentlySelectedToDropdownSelection(itemFeature);
	}

	public List<TMP_Dropdown.OptionData> CreateOptionList(string featureCategory)
	{
		List<ItemCondition> list = ItemConditionList.conditionLookup[featureCategory];
		List<TMP_Dropdown.OptionData> list2 = new List<TMP_Dropdown.OptionData>();
		foreach (ItemCondition item2 in list)
		{
			TMP_Dropdown.OptionData item = new TMP_Dropdown.OptionData(LocHelper.GetLocalizedMechanic("mech_label_condition_value", item2.display, item2.GetModValueFormated()));
			list2.Add(item);
		}
		return list2;
	}

	public void SetCurrentlySelectedToDropdownSelection(ItemFeature itemFeature)
	{
		List<ItemCondition> list = ItemConditionList.conditionLookup[itemFeature.category];
		int num = 0;
		foreach (ItemCondition item in list)
		{
			if (itemFeature.GetCurrentItemCondition().identifier.Equals(item.identifier))
			{
				dropdown.value = num;
			}
			num++;
		}
	}

	public string GetCategory()
	{
		return thisFeature.category;
	}

	public string GetSelectedConditionIdentifier()
	{
		return ItemConditionList.conditionLookup[thisFeature.category][dropdown.value].identifier;
	}

	public void OnDropdownValueChanged()
	{
		List<ItemCondition> list = ItemConditionList.conditionLookup[thisFeature.category];
		thisFeature.fakeCondition = list[dropdown.value];
		thisFeature.SetExposed(value: false);
		WaterFeatureHelper.OnWaterLabelUpdated(thisFeature, dropdown.value);
		EmporiumAudioPlayer.Instance.audioLabeler.Play();
		AchievementTracker.Instance.CheckLowerLevelsSpecial();
	}
}
