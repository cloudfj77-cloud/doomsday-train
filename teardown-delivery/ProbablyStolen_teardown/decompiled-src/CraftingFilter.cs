public abstract class CraftingFilter
{
	public string displayName { get; protected set; }

	public int amount { get; protected set; }

	public abstract bool Filter(GameItem item);

	public abstract CraftingFilter Clone();

	public CraftingFilter ModifyClone(int newAmount)
	{
		CraftingFilter craftingFilter = Clone();
		craftingFilter.amount = newAmount;
		return craftingFilter;
	}
}
