using System.Collections.Generic;
using UnityEngine;

public class TableGroupMaster : MonoBehaviour
{
	public const string dumpingGroundTG = "dumpingGroundTG";

	public const string dumpingGroundFreshTG = "dumpingGroundFreshTG";

	[SerializeField]
	private List<TableGroupEntry> tableGroupEntries = new List<TableGroupEntry>();

	public static TableGroupMaster Instance { get; private set; }

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
		LootRegistry.LoadVanillaGroups(tableGroupEntries);
	}

	public static string Roll(string groupId)
	{
		return LootRegistry.RollGroup(groupId);
	}
}
