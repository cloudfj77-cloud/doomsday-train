using System;

public class AppraisalItemType
{
	public string id;

	public int unlockRank;

	public Func<GameItem> createRandom;

	public Func<GameItem, bool> isType;

	public Func<GameItem, int> getCorrectValue;

	public string itemId;

	public string guideItemId;

	public Func<string> displayName;

	public string source = "vanilla";

	public string GetDisplayName()
	{
		if (displayName != null)
		{
			try
			{
				return displayName();
			}
			catch (Exception ex)
			{
				Debug.LogError("[AppraisalItemRegistry] Display name of '" + id + "' threw: " + ex);
			}
		}
		return GeneralHelper.GetDisplayNameFromID(itemId);
	}

	public bool Matches(GameItem item)
	{
		if (item == null)
		{
			return false;
		}
		if (isType != null)
		{
			return isType(item);
		}
		return item.identifier == itemId;
	}
}
