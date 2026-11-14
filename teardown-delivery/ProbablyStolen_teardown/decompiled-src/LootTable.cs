using RNGNeeds;
using UnityEngine;

[CreateAssetMenu(fileName = "table", menuName = "DSE/Loot Tables")]
public class LootTable : ScriptableObject
{
	public ProbabilityList<string> table;

	public string Roll()
	{
		return table.PickValue();
	}
}
