using System.Collections.Generic;
using Duckov.Quests;
using Duckov.Weathers;

public class RequireSeasons : Condition
{
	public List<Seasons> seasons;

	public override bool Evaluate()
	{
		if (!LevelManager.LevelInited)
		{
			return false;
		}
		Seasons season = WeatherManager.Season;
		return seasons.Contains(season);
	}
}
