public abstract class OverworldPOI
{
	public string displayName;

	public string identifier;

	public string customSpritePath;

	public int spawnOdd;

	public int defaultSpawnOdd;

	public int distance;

	public OverworldPOI()
	{
		distance = RNG.GetRandomInt(20, 100);
		spawnOdd = 10;
		defaultSpawnOdd = 10;
	}
}
