using System.Collections.Generic;

public class CraftingIdentifierFilter : CraftingFilter
{
	public HashSet<string> identifierFilter { get; private set; }

	public CraftingIdentifierFilter(string displayName, string identifier, params string[] additionalIdentifiers)
		: this(displayName, -1, identifier, additionalIdentifiers)
	{
	}

	public CraftingIdentifierFilter(string displayName, int amount, string identifier, params string[] additionalIdentifiers)
	{
		base.displayName = displayName;
		base.amount = amount;
		identifierFilter = new HashSet<string>();
		identifierFilter.Add(identifier);
		foreach (string item in additionalIdentifiers)
		{
			identifierFilter.Add(item);
		}
	}

	public override bool Filter(GameItem item)
	{
		if (item == null)
		{
			return false;
		}
		string identifier = item.identifier;
		if (identifier == null)
		{
			return false;
		}
		return identifierFilter.Contains(identifier);
	}

	public override CraftingFilter Clone()
	{
		return new CraftingIdentifierFilter(base.displayName, base.amount, "")
		{
			identifierFilter = new HashSet<string>(identifierFilter)
		};
	}
}
