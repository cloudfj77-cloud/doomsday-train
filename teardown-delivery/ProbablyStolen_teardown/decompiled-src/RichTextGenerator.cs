public class RichTextGenerator
{
	private static RichTextBuilder _builder;

	private static RichTextBuilder builder
	{
		get
		{
			if (_builder == null)
			{
				_builder = new RichTextBuilder(60);
			}
			return _builder;
		}
	}

	private static string Underline(string text)
	{
		return "<u>" + text + "</u>";
	}

	public static RichText InsertItemInOtherItem(GameItem sourceItem, GameItem targetItem, bool canInsert, RichTextBuilder customBuilder = null)
	{
		if (customBuilder == null)
		{
			customBuilder = builder;
		}
		customBuilder.Clear();
		string displayName = sourceItem.GetDisplayName(preferCustomName: true);
		string displayName2 = targetItem.GetDisplayName(preferCustomName: true);
		if (canInsert)
		{
			customBuilder.Add(LocHelper.GetLocalizedMechanic("mech_drag_insert_into", displayName, Underline(displayName2)), RenderHandler.ColorPalette.LightGreen);
		}
		else
		{
			customBuilder.Add(LocHelper.GetLocalizedMechanic("mech_drag_cant_insert", displayName, displayName2), RenderHandler.ColorPalette.Red);
		}
		return customBuilder.Build();
	}

	public static RichText UseItemOnOtherItem(GameItem sourceItem, GameItem targetItem, SlotMarker targetSlot, bool canInsert, RichTextBuilder customBuilder = null)
	{
		if (customBuilder == null)
		{
			customBuilder = builder;
		}
		customBuilder.Clear();
		string displayName = sourceItem.GetDisplayName(preferCustomName: true);
		string displayName2 = targetItem.GetDisplayName(preferCustomName: true);
		if (canInsert)
		{
			if (TooltipUseHelper.HasUseTooltip(sourceItem, targetItem))
			{
				TooltipUseHelper.HandleUseTooltip(sourceItem, targetItem, customBuilder);
			}
			else
			{
				customBuilder.Add(LocHelper.GetLocalizedMechanic("mech_drag_use_on", displayName, Underline(displayName2)), RenderHandler.ColorPalette.LightGreen);
			}
		}
		else
		{
			customBuilder.Add(LocHelper.GetLocalizedMechanic("mech_drag_cant_use_on", displayName, displayName2), RenderHandler.ColorPalette.Red);
		}
		return customBuilder.Build();
	}
}
