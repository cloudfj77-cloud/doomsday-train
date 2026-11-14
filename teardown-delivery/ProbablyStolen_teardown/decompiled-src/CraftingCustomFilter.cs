using System;

public class CraftingCustomFilter : CraftingFilter
{
	private Func<GameItem, bool> funcFilter;

	public CraftingCustomFilter(string displayName, Func<GameItem, bool> funcFilter)
		: this(displayName, -1, funcFilter)
	{
	}

	public CraftingCustomFilter(string displayName, int amount, Func<GameItem, bool> funcFilter)
	{
		base.displayName = displayName;
		base.amount = amount;
		this.funcFilter = funcFilter;
	}

	public override bool Filter(GameItem item)
	{
		if (item == null)
		{
			return false;
		}
		if (funcFilter == null)
		{
			return false;
		}
		return funcFilter(item);
	}

	public override CraftingFilter Clone()
	{
		return new CraftingCustomFilter(base.displayName, base.amount, funcFilter);
	}
}
