using RNGNeeds;
using UnityEngine;

[CreateAssetMenu(fileName = "table", menuName = "DSE/Loot Table Group")]
public class TableGroup : ScriptableObject
{
	public ProbabilityList<LootTable> tableGroup;

	public string Roll()
	{
		return tableGroup.PickValue().Roll();
	}
}
