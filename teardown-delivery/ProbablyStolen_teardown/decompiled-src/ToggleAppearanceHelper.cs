using System;

public class ToggleAppearanceHelper
{
	public const string TOGGLE_APPEARANCE_TAG = "TOGGLE_APPEARANCE_TAG";

	public static void MakeSpriteCyclable(GameItem item, string atlasPath, params string[] spriteVariants)
	{
		item.EnableTag("TOGGLE_APPEARANCE_TAG");
		item.mayToggleSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.canToggleSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.onToggleSlotItemFunc = delegate(GameItem toggled, GameInventory _, SlotMarker _)
		{
			int num = (Array.IndexOf(spriteVariants, toggled.spritePath) + 1) % spriteVariants.Length;
			toggled.SetSpriteQuick(atlasPath, spriteVariants[num]);
		};
	}

	public static void CreateToggleAppearanceTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("TOGGLE_APPEARANCE_TAG"))
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_toggle_appearance_hint"), ignoreEmpty: true, RenderHandler.ColorPalette.LightGray, bold: false, italic: true);
		}
	}
}
