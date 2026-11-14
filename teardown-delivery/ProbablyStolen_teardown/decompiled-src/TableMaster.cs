using System.Collections.Generic;
using UnityEngine;

public class TableMaster : MonoBehaviour
{
	public const string junkTable = "junkTable";

	public const string accessCardTable = "accessCardTable";

	public const string allModuleTable = "allModuleTable";

	public const string makeshiftWeaponTable = "makeshiftWeaponTable";

	public const string materialTable = "materialTable";

	public const string householdTable = "householdTable";

	public const string packedFoodTable = "packedFoodTable";

	public const string t1moduleTable = "t1moduleTable";

	public const string t2moduleTable = "t2moduleTable";

	public const string toolTable = "toolTable";

	public const string medicalTable = "medicalTable";

	[SerializeField]
	private List<TableEntry> tableEntries = new List<TableEntry>();

	public static TableMaster Instance { get; private set; }

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
		LootRegistry.LoadVanillaTables(tableEntries);
	}

	public string GetTableId(LootTable asset)
	{
		if (asset == null)
		{
			return null;
		}
		foreach (TableEntry tableEntry in tableEntries)
		{
			if (tableEntry.lootTable == asset && !string.IsNullOrEmpty(tableEntry.tableId))
			{
				return tableEntry.tableId;
			}
		}
		return null;
	}

	public static string Roll(string tableId)
	{
		return LootRegistry.Roll(tableId);
	}
}
