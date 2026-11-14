public class LootEntry
{
	public string id;

	public float weight;

	public string sourceModId;

	public string modifiedByModId;

	public LootEntry(string id, float weight, string sourceModId = null)
	{
		this.id = id;
		this.weight = weight;
		this.sourceModId = sourceModId;
	}

	public LootEntry Clone()
	{
		return new LootEntry(id, weight, sourceModId)
		{
			modifiedByModId = modifiedByModId
		};
	}
}
