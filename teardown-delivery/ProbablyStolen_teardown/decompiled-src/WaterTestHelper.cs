public class WaterTestHelper
{
	public const string IS_SPENT = "IS_SPENT";

	public const string LABELED_WATER_QUALITY = "LABELED_WATER_QUALITY";

	public const string TEST_WATER_QUALITY = "TEST_WATER_QUALITY";

	public const string ASSOCIATED_ITEM_ID = "ASSOCIATED_ITEM_ID";

	public static void HandleWaterCheckTestingStrip(GameItem targetGameItem, GameItem testingStrip)
	{
		testingStrip.EnableTag("IS_SPENT");
		testingStrip.ModifyTag("TEST_WATER_QUALITY", delegate(TagState state)
		{
			state.Enable().SetInt(WaterHelper.GetWaterPurity(targetGameItem));
		});
		testingStrip.ModifyTag("ASSOCIATED_ITEM_ID", delegate(TagState state)
		{
			state.Enable().SetInt(targetGameItem.GetUniqueID());
		});
	}
}
