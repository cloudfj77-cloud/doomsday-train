using System.Collections.Generic;

public class AppraisalHelper
{
	public const string APPRAISAL_ITEM = "APPRAISAL_ITEM";

	public const string APPRAISAL_CURRENT_VALUE = "APPRAISAL_DEFAULT_VALUE";

	public const string APPRAISAL_CLIENT_VALUE = "APPRAISAL_CLIENT_VALUE";

	public static void InitAppraisalItem(GameItem gameItem, int defaultValue)
	{
		gameItem.EnableTag("APPRAISAL_ITEM");
		SetDefaultAppraisalValue(gameItem, defaultValue);
	}

	public static void SetDefaultAppraisalValue(GameItem gameItem, int defaultValue)
	{
		gameItem.ModifyTag("APPRAISAL_DEFAULT_VALUE", delegate(TagState state)
		{
			state.Enable().SetInt(defaultValue);
		});
		gameItem.ModifyTag("APPRAISAL_CLIENT_VALUE", delegate(TagState state)
		{
			state.Enable().SetInt(defaultValue);
		});
	}

	public static void GetStartNegociationDialog(GameItem gameItem)
	{
		new Dialogue().SetText("CLIENT", "This item's base value is worth at least " + gameItem.unitValue + " credits");
	}

	public static void GetStartNegotiationDialog(GameItem gameItem)
	{
		List<string> list = new List<string>
		{
			"This item's base value is worth at least " + gameItem.unitValue + " credits.",
			"I believe this item should fetch around " + gameItem.unitValue + " credits.",
			"The value of this item is approximately " + gameItem.unitValue + " credits.",
			"I think this item is around " + gameItem.unitValue + " credits."
		};
		int randomIntExculsive = RNG.GetRandomIntExculsive(0, list.Count);
		new Dialogue().SetText("CLIENT", list[randomIntExculsive]);
	}

	public static int GetCorrectBaseValue(GameItem gameItem)
	{
		if (!gameItem.IsTag("APPRAISAL_ITEM"))
		{
			return 0;
		}
		AppraisalItemType appraisalItemType = AppraisalItemRegistry.Find(gameItem);
		int num;
		if (appraisalItemType == null)
		{
			num = 0;
		}
		else
		{
			num = appraisalItemType.getCorrectValue(gameItem);
			if (num != 0)
			{
				goto IL_0035;
			}
		}
		Debug.Log("Correct Value not Found, is the item's type registered in AppraisalItemRegistry (ModHelper.AddAppraisalItem)?");
		goto IL_0035;
		IL_0035:
		return num;
	}

	public static void CreateAppraisalTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("APPRAISAL_ITEM") && PlayerStore.Instance.devMode)
		{
			builder.AddLine("TRUE VALUE  : " + GetCorrectBaseValue(item), ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
		}
		if (item.IsTag("APPRAISAL_ITEM"))
		{
			builder.AddLine(LocHelper.GetLocalizedUI("mech_appraisal_item_tip"), ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
		}
	}
}
