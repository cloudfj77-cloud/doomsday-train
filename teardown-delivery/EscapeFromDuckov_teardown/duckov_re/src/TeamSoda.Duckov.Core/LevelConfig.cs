using System.Collections.Generic;
using Duckov.Buffs;
using Duckov.Rules;
using Duckov.Utilities;
using Duckov.Weathers;
using UnityEngine;

public class LevelConfig : MonoBehaviour
{
	private static LevelConfig instance;

	[SerializeField]
	private bool isBaseLevel;

	[SerializeField]
	private bool isRaidMap = true;

	[SerializeField]
	private bool spawnTomb = true;

	[SerializeField]
	private bool saveCharacter = true;

	[SerializeField]
	private bool savePet = true;

	[SerializeField]
	private int minExitCount;

	[SerializeField]
	private int maxExitCount;

	[SerializeField]
	private bool accountAvailable = true;

	[SerializeField]
	private bool lockRule;

	[SerializeField]
	private RulesetFile rule;

	[SerializeField]
	private Seasons season;

	[SerializeField]
	private PerkTreeIDList enabledPerkTrees;

	[SerializeField]
	private bool needInspectDeadLootBox = true;

	public TimeOfDayConfig timeOfDayConfig;

	[SerializeField]
	[Min(1f)]
	private float lootBoxHighQualityChanceMultiplier = 1f;

	[SerializeField]
	[Range(0.1f, 10f)]
	private float lootboxItemCountMultiplier = 1f;

	public List<Buff> startBuffPrefabs;

	public static LevelConfig Instance
	{
		get
		{
			if (!instance)
			{
				SetInstance();
			}
			return instance;
		}
	}

	public bool AccountAvailable => accountAvailable;

	public bool LockRule => lockRule;

	public RulesetFile Rule => rule;

	public bool NeedInspectDeadLootBox => needInspectDeadLootBox;

	public static bool SaveCharacter
	{
		get
		{
			if ((bool)Instance)
			{
				return Instance.saveCharacter;
			}
			return false;
		}
	}

	public static bool SavePet
	{
		get
		{
			if ((bool)Instance)
			{
				return Instance.savePet;
			}
			return false;
		}
	}

	public static Seasons Season
	{
		get
		{
			if (!Instance)
			{
				return Seasons.spring;
			}
			return Instance.season;
		}
	}

	public float LootBoxQualityLowPercent => 1f - 1f / lootBoxHighQualityChanceMultiplier;

	public float LootboxItemCountMultiplier => lootboxItemCountMultiplier;

	public static bool IsBaseLevel
	{
		get
		{
			if (!Instance)
			{
				return false;
			}
			return Instance.isBaseLevel;
		}
	}

	public static bool IsRaidMap
	{
		get
		{
			if (!Instance)
			{
				return false;
			}
			return Instance.isRaidMap;
		}
	}

	public static int MinExitCount
	{
		get
		{
			if (!Instance)
			{
				return 0;
			}
			return Instance.minExitCount;
		}
	}

	public static bool SpawnTomb
	{
		get
		{
			if (!Instance)
			{
				return true;
			}
			return Instance.spawnTomb;
		}
	}

	public static int MaxExitCount
	{
		get
		{
			if (!Instance)
			{
				return 0;
			}
			return Instance.maxExitCount;
		}
	}

	private void Awake()
	{
		Object.Instantiate(GameplayDataSettings.Prefabs.LevelManagerPrefab).transform.SetParent(base.transform);
	}

	private static void SetInstance()
	{
		if (!instance)
		{
			instance = Object.FindFirstObjectByType<LevelConfig>();
			_ = (bool)instance;
		}
	}

	internal static bool IsPerkTreeEnabled(string perkTreeID)
	{
		if (instance == null)
		{
			return false;
		}
		PerkTreeIDList defaultEnabledPerkTrees = instance.enabledPerkTrees;
		if (defaultEnabledPerkTrees == null)
		{
			defaultEnabledPerkTrees = GameplayDataSettings.DefaultEnabledPerkTrees;
		}
		return defaultEnabledPerkTrees.perkTrees.Contains(perkTreeID);
	}
}
