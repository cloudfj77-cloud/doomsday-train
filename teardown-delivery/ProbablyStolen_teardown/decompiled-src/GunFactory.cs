public class GunFactory
{
	public static GameItem Shotgun(bool isGrip = false)
	{
		GameItem gameItem = DirectoryMaster.Item("shotgun_receiver");
		if (isGrip)
		{
			GameItem item = DirectoryMaster.Item("shotgun_grip");
			gameItem.TryFindOneValidInventorySlot(item)?.TryAcceptOnce();
		}
		else
		{
			GameItem item2 = DirectoryMaster.Item("shotgun_stock");
			gameItem.TryFindOneValidInventorySlot(item2)?.TryAcceptOnce();
		}
		return gameItem;
	}
}
