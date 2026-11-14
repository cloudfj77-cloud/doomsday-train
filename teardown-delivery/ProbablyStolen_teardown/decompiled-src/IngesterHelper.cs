using UnityEngine;

public class IngesterHelper
{
	public const string INGESTER_TAG = "INGESTER";

	public const string INGESTER_ACTIVE_SUFFIX = "_INGESTER_ACTIVE";

	public static void InitIngester(GameItem item)
	{
		item.EnableTag("INGESTER");
	}

	public static void Inject(GameItem animal, string liquidID, int amountUL)
	{
		Liquid liquidFromId = Liquid.GetLiquidFromId(liquidID);
		if (liquidFromId != null)
		{
			int previous = animal.GetTagReadonly(liquidFromId.currentPart).GetInt();
			animal.ModifyTag(liquidFromId.currentPart, delegate(TagState state)
			{
				state.Enable().SetInt(previous + amountUL);
			});
			if (previous <= 0)
			{
				animal.EnableTag(liquidFromId.currentPart + "_INGESTER_ACTIVE");
				liquidFromId.OnEnterSystem?.Invoke(animal);
			}
		}
	}

	public static void TickIngester(GameItem item)
	{
		if (!item.IsTag("INGESTER"))
		{
			return;
		}
		foreach (Liquid ingestableLiquid in Liquid.IngestableLiquids)
		{
			int @int = item.GetTagReadonly(ingestableLiquid.currentPart).GetInt();
			if (@int > 0)
			{
				int newConcentration = Mathf.Max(0, @int - ingestableLiquid.metabolismRatePerDay);
				item.ModifyTag(ingestableLiquid.currentPart, delegate(TagState state)
				{
					state.SetInt(newConcentration);
				});
				if (newConcentration <= 0)
				{
					item.DisableTag(ingestableLiquid.currentPart + "_INGESTER_ACTIVE");
					ingestableLiquid.OnLeftSystem?.Invoke(item);
				}
			}
		}
	}

	public static void CreateIngesterTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!item.IsTag("INGESTER"))
		{
			return;
		}
		foreach (Liquid ingestableLiquid in Liquid.IngestableLiquids)
		{
			int @int = item.GetTagReadonly(ingestableLiquid.currentPart).GetInt();
			if (@int > 0)
			{
				builder.AddLine(LocHelper.GetLocalizedMechanicOrDefault("mech_ingester_concentration", "{0}: {1} ul (-{2} ul/day)", ingestableLiquid.displayName, @int, ingestableLiquid.metabolismRatePerDay), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
		}
	}
}
