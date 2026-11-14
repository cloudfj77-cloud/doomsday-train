public class CommonTooltipHelper
{
	public static string UNIQUE_TYPE_STRING = "UNIQUE_TYPE_STRING";

	public const string TECHNICAL_EQUIPMENT_TAG = "TECHNICAL_EQUIPMENT_TAG";

	public const string IMPROVISED_EQUIPMENT_TAG = "IMPROVISED_EQUIPMENT_TAG";

	public const string TACTICAL_EQUIPMENT_TAG = "TACTICAL_EQUIPMENT_TAG";

	public static void AddUniqueTypeTooltip(GameItem gameItem, string tag)
	{
		gameItem.ModifyTag(UNIQUE_TYPE_STRING, delegate(TagState state)
		{
			state.Enable().SetString(tag);
		});
	}

	public static void CreateItemTypeTooltip(RichTextBuilder builder, GameItem item)
	{
		foreach (string item2 in item.GetGameItemType())
		{
			string text = " [" + TypeHelper.GetTypeDisplayName(item2) + "]";
			if (item2.Equals("CONTRABAND"))
			{
				builder.Add(text, RenderHandler.ColorPalette.LightRed);
			}
			else
			{
				builder.Add(text, RenderHandler.ColorPalette.LightCyan);
			}
		}
		builder.AddLine("");
	}
}
