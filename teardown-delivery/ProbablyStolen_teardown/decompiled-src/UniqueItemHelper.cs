public class UniqueItemHelper
{
	public const string UNIQUE_ITEM_ID = "UNIQUE_ITEM_ID";

	public const string georgio2_cigarette1 = "georgio2_cigarette1";

	public const string georgio2_cigarette2 = "georgio2_cigarette2";

	public static void SetUniqueItemId(GameItem item, string uniqueId)
	{
		item.ModifyTag("UNIQUE_ITEM_ID", delegate(TagState state)
		{
			state.Enable().SetString(uniqueId);
		});
	}

	public static bool IsUniqueId(GameItem item, string uniqueId)
	{
		if (item.GetTagReadonly("UNIQUE_ITEM_ID").GetString().Equals(uniqueId))
		{
			return true;
		}
		return false;
	}

	public static bool IsHaveUniqueId(GameItem item)
	{
		if (!item.IsTag("UNIQUE_ITEM_ID"))
		{
			return false;
		}
		if (item.GetTagReadonly("UNIQUE_ITEM_ID").GetString().Equals(""))
		{
			return false;
		}
		return true;
	}
}
