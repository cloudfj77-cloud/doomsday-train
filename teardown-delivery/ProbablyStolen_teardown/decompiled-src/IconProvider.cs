using System;
using System.Collections.Generic;
using UnityEngine;

public class IconProvider : MonoBehaviour
{
	[Serializable]
	public struct ScenarioIconEntry
	{
		public string scenarioId;

		public Sprite sprite;
	}

	[Tooltip("Icons by StartingScenario id, for scenarios without a field below (and overriding one if both are set).")]
	public List<ScenarioIconEntry> scenarioIcons = new List<ScenarioIconEntry>();

	public Sprite apprentice;

	public Sprite junker;

	public Sprite waterMerchant;

	public Sprite fence;

	public Sprite farmer;

	public Sprite gunsmith;

	public Sprite chemist;

	public Sprite organTrader;

	public Sprite rockBottom;

	public Sprite streetFoodVendor;

	public Sprite rancher;

	public Sprite aquafarmer;

	public static IconProvider Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	public Sprite GetIcon(string scenarioId)
	{
		if (string.IsNullOrEmpty(scenarioId))
		{
			return null;
		}
		foreach (ScenarioIconEntry scenarioIcon in scenarioIcons)
		{
			if (scenarioIcon.scenarioId == scenarioId && scenarioIcon.sprite != null)
			{
				return scenarioIcon.sprite;
			}
		}
		StartingScenario startingScenario = StartingScenario.Get(scenarioId);
		if (startingScenario == null)
		{
			return null;
		}
		return GetLegacyIcon(startingScenario.startType);
	}

	public Sprite GetIcon(NewGameData.StartType startType, string scenarioId)
	{
		Sprite icon = GetIcon(StartingScenario.ResolveId(startType, scenarioId));
		if (!(icon != null))
		{
			return GetLegacyIcon(startType);
		}
		return icon;
	}

	public Sprite GetIcon(NewGameData.StartType startType)
	{
		return GetIcon(startType, null);
	}

	private Sprite GetLegacyIcon(NewGameData.StartType startType)
	{
		return startType switch
		{
			NewGameData.StartType.APPRENTICE => apprentice, 
			NewGameData.StartType.SCAV => junker, 
			NewGameData.StartType.WATER_MERCHANT => waterMerchant, 
			NewGameData.StartType.FENCE => fence, 
			NewGameData.StartType.FARMER => farmer, 
			NewGameData.StartType.GUNSMITH => gunsmith, 
			NewGameData.StartType.DRUG_DEALER => chemist, 
			NewGameData.StartType.ORGAN_TRADER => organTrader, 
			NewGameData.StartType.ROCK_BOTTOM => rockBottom, 
			NewGameData.StartType.STREET_FOOD_VENDOR => streetFoodVendor, 
			NewGameData.StartType.RANCHER => rancher, 
			NewGameData.StartType.AQUAFARMER => aquafarmer, 
			_ => null, 
		};
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
