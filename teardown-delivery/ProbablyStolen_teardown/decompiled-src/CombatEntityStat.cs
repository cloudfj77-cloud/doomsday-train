public class CombatEntityStat
{
	public int statValue { get; set; }

	public int statId { get; set; }

	public int baseStatValue { get; set; }

	public CombatEntityStat(int statId)
	{
		this.statId = statId;
	}
}
