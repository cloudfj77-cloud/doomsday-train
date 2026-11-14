public class OverworldSpace : OverworldPOI
{
	public bool isEventTriggered;

	public OverworldSpace()
	{
		isEventTriggered = false;
		distance = RNG.GetRandomInt(10, 90);
	}
}
