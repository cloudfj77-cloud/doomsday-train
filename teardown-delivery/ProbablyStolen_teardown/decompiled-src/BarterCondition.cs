using System.Collections.Generic;

public class BarterCondition
{
	public string requiredItemId;

	public string requiredItemTag;

	public string requiredDisplay;

	public string offeredDisplay;

	public List<string> blacklistedConditionId;

	public BarterCondition()
	{
		requiredItemId = "";
		requiredItemTag = "";
		blacklistedConditionId = new List<string>();
	}

	public bool IsItemValid(GameItem gameItem)
	{
		if (!requiredItemId.Equals("") && !gameItem.identifier.Equals(requiredItemId))
		{
			return false;
		}
		if (!requiredItemTag.Equals("") && !gameItem.IsTag(requiredItemTag))
		{
			return false;
		}
		foreach (ItemFeature itemFeature in gameItem.itemFeatures)
		{
			if (itemFeature.useCondition && blacklistedConditionId.Contains(itemFeature.GetCurrentItemCondition().identifier))
			{
				return false;
			}
		}
		return true;
	}
}
