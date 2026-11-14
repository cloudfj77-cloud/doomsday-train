public class SpaceBattleHelper
{
	public static bool TakingHit(int damage)
	{
		var (flag, num, _, num2) = GameMaster.current.shipManager.DamageHullVerbose(damage);
		if (num2 > 0)
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Your ship was hit and took " + num2 + " damage!. It now have a integrity of " + num + ".", RenderHandler.ColorPalette.LightRed));
		}
		else
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Your ship was hit but thanks to its armor, it wasn't damaged at all. It still have a integrity of " + num + ".", RenderHandler.ColorPalette.LightGreen));
		}
		if (!flag)
		{
			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Your ship escapes! You may now select another destionation from the Navigation System.", RenderHandler.ColorPalette.LightGreen));
			return false;
		}
		OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Your ship has taken critical amount of damage. Your ship is destroyed!", RenderHandler.ColorPalette.LightRed));
		return true;
	}
}
