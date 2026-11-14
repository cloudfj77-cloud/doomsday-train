public class ExpeditionAccident
{
	public static void ActivateAccident(Expedition expedition)
	{
		if (expedition.expeditionLocation.IsDumpingGrounds())
		{
			ActivateDumpingGroundsAccident(expedition);
		}
	}

	public static void ActivateDumpingGroundsAccident(Expedition expedition)
	{
		if (RNG.Roll(50))
		{
			expedition.Log("and cut myself on a sharp metal!");
			int randomInt = RNG.GetRandomInt(5, 8);
			expedition.hireling.Damage(randomInt);
		}
		else
		{
			expedition.Log("and almost cut myself on a sharp metal, but managed to avoid injury.");
		}
	}
}
