public class GunOrderRequirement
{
	public int value;

	public string id;

	public int reward;

	public bool isMustBeLargerThan = true;

	public bool isCaliber;

	public bool IsMet(GameItem item)
	{
		int @int = item.GetTagReadonly(id).GetInt();
		if (isCaliber)
		{
			if (item.IsTag("ADV_GUN_TAG"))
			{
				return @int == value;
			}
			return false;
		}
		if (@int > value && isMustBeLargerThan)
		{
			return true;
		}
		if (@int < value && !isMustBeLargerThan)
		{
			return true;
		}
		return false;
	}

	public string GetCompareSymbol()
	{
		if (isMustBeLargerThan)
		{
			return ">";
		}
		return "<";
	}

	public bool IsFlag()
	{
		return id == "GUN_SUPPRESSED_TAG";
	}

	public string GetDisplayName()
	{
		if (isCaliber)
		{
			return GunHelper.IDToString("GUN_CALIBER_TAG") + ": " + GunHelper.GetCaliberName(value);
		}
		if (IsFlag())
		{
			if (value < 1)
			{
				return LocHelper.GetLocalizedItem("item_silencer_makeshift_name");
			}
			return LocHelper.GetLocalizedItem("item_silencer_name");
		}
		return GunHelper.IDToString(id) + " " + GetCompareSymbol() + " " + value;
	}

	public int GetReward()
	{
		return GunOrderData.ApplyRewardModifiers(reward);
	}
}
