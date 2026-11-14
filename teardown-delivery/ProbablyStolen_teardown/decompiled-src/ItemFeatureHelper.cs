public class ItemFeatureHelper
{
	public static GameItem SetShabby(GameItem gameItem)
	{
		gameItem.AddItemFeature(ItemFeatureList.ShabbyFeature());
		return gameItem;
	}

	public static bool IsShabby(GameItem gameItem)
	{
		return gameItem.FindItemFeatureByCategory("CATEGORY_SHABBY") != null;
	}

	public static GameItem SetSecondHand(GameItem gameItem)
	{
		if (!IsSecondHand(gameItem))
		{
			gameItem.AddItemFeature(ItemFeatureList.SecondHandFeature());
		}
		return gameItem;
	}

	public static bool IsSecondHand(GameItem gameItem)
	{
		return gameItem.FindItemFeatureByCategory("CATEGORY_SECOND_HAND") != null;
	}

	public static void HandleFactoryNewCheckFlashlight(GameItem targetGameItem)
	{
		targetGameItem.FindItemFeatureByCategory("CATEGORY_OPEN_BOX")?.DiscoverFeature();
	}

	public static void HandleRustCheckFlashlight(GameItem targetGameItem)
	{
		targetGameItem.FindItemFeatureByCategory("CATEGORY_RUSTED")?.DiscoverFeature();
	}

	public static void HandleCannedCheckFlashlight(GameItem targetGameItem)
	{
		targetGameItem.FindItemFeatureByCategory("canned")?.DiscoverFeature();
	}
}
