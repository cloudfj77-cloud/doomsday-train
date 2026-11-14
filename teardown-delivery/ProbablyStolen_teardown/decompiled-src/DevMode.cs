public class DevMode
{
	private static DevMode instance = null;

	private static readonly object padlock = new object();

	public bool isOn { get; set; }

	public static DevMode Instance
	{
		get
		{
			lock (padlock)
			{
				if (instance == null)
				{
					instance = new DevMode();
				}
				return instance;
			}
		}
	}

	private DevMode()
	{
		isOn = true;
	}

	public void DevModeBuffEntity(CombatEntity entity)
	{
		entity.accuracyPercentage = 200;
		entity.damagePercentage = 200;
	}

	public void DevModeAddItem()
	{
	}
}
