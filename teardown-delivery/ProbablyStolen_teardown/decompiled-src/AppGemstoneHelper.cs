public class AppGemstoneHelper
{
	public const string GEMSTONE_TAG = "GEMSTONE_TAG";

	public static void InitItem(GameItem gameItem)
	{
		gameItem.EnableTag("INSPECTABLE_TAG");
		AppraisalHelper.InitAppraisalItem(gameItem, 1000);
		gameItem.EnableTag("GEMSTONE_TAG");
		InitDefault(gameItem);
	}

	public static void InitDefault(GameItem gameItem)
	{
	}
}
