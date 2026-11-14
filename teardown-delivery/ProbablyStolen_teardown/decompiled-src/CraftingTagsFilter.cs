using System.Collections.Generic;
using System.Linq;

public class CraftingTagsFilter : CraftingFilter
{
	public HashSet<string> tagFilter { get; private set; }

	public CraftingTagsFilter(string displayName, string tag, params string[] additionalTags)
		: this(displayName, -1, tag, additionalTags)
	{
	}

	public CraftingTagsFilter(string displayName, int amount, string tag, params string[] additionalTags)
	{
		base.displayName = displayName;
		base.amount = amount;
		tagFilter = new HashSet<string>();
		tagFilter.Add(tag);
		foreach (string item in additionalTags)
		{
			tagFilter.Add(item);
		}
	}

	public override bool Filter(GameItem item)
	{
		if (item == null)
		{
			return false;
		}
		if (item?.modifiedState?.dict == null)
		{
			return false;
		}
		return tagFilter.Intersect(item.modifiedState.dict.Keys).Any();
	}

	public override CraftingFilter Clone()
	{
		return new CraftingTagsFilter(base.displayName, base.amount, "")
		{
			tagFilter = new HashSet<string>(tagFilter)
		};
	}
}
