public class TooltipUseHelper
{
	public static void HandleUseTooltip(GameItem sourceItem, GameItem targetItem, RichTextBuilder customBuilder)
	{
		string text = "<u>" + targetItem.GetDisplayName(preferCustomName: true) + "</u>";
		if (sourceItem.IsID("screwdriver") && targetItem.IsTag("ADV_GUN_TAG"))
		{
			customBuilder.Add(LocHelper.GetLocalizedMechanic("mech_drag_disassemble", text), RenderHandler.ColorPalette.LightRed);
		}
		else if (sourceItem.IsID("gun_part") && targetItem.IsTag("ADV_GUN_TAG"))
		{
			int refurbishChance = GunHelper.GetRefurbishChance(targetItem);
			customBuilder.Add(LocHelper.GetLocalizedMechanic("mech_drag_refurbish", text, refurbishChance), RenderHandler.ColorPalette.LightOrange);
		}
	}

	public static bool HasUseTooltip(GameItem sourceItem, GameItem targetItem)
	{
		if (sourceItem.IsID("screwdriver") && targetItem.IsTag("ADV_GUN_TAG"))
		{
			return true;
		}
		if (sourceItem.IsID("gun_part") && targetItem.IsTag("ADV_GUN_TAG"))
		{
			return true;
		}
		return false;
	}
}
