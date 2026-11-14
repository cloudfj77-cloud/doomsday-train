public class InspectableHelper
{
	public const string INSPECTABLE_TAG = "INSPECTABLE_TAG";

	public const string ALREADY_CONFRONTED_TAG = "ALREADY_CONFRONTED_TAG";

	public static void InitInspectableItem(GameItem gameItem)
	{
		gameItem.EnableTag("INSPECTABLE_TAG");
	}

	public static void CreateInspectableTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("INSPECTABLE_TAG") && PlayerStore.Instance.isVisualInspectionIntroduced)
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_inspectable_tooltip"), ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange, bold: false, italic: true);
		}
	}

	public static void OnConfrontSuccess(GameItem gameItem)
	{
		gameItem.EnableTag("ALREADY_CONFRONTED_TAG");
		gameItem.AddItemFeature(ItemFeatureList.ConfrontSucessDiscount());
		InspectionUIManager.Instance.OnConfrontSuccess();
	}

	public static bool IsConfrontCorrect(string reason, GameItem item)
	{
		if (item.IsTag(reason))
		{
			return true;
		}
		return false;
	}
}
