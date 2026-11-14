public abstract class ItemInsertRemoveBehaviour
{
	public int priority { get; }

	public ItemInsertRemoveBehaviour(int priority = 0)
	{
		this.priority = priority;
	}

	public virtual bool? SourceFilter(GameItem source)
	{
		return null;
	}

	public virtual bool? TargetFilter(GameItem target)
	{
		return null;
	}

	public virtual bool? SourceTargetFilter(GameItem source, GameItem target)
	{
		return null;
	}

	public virtual bool? MayInsert(GameItem source, GameItem target)
	{
		return null;
	}

	public virtual bool? CanInsert(GameItem source, GameItem target)
	{
		return null;
	}

	public virtual bool? CanInsertPassthrough(GameItem source, GameItem target)
	{
		return null;
	}

	public virtual bool? OnInsert(GameItem source, GameItem target)
	{
		return null;
	}

	public virtual bool? MayRemove(GameItem source, GameItem target)
	{
		return null;
	}

	public virtual bool? CanRemove(GameItem source, GameItem target)
	{
		return null;
	}

	public virtual bool? CanRemovePassthrough(GameItem source, GameItem target)
	{
		return null;
	}

	public virtual bool? OnRemove(GameItem source, GameItem target)
	{
		return null;
	}

	public virtual string InsertTooltip(GameItem source, GameItem target)
	{
		return null;
	}

	public virtual RichTextBuilder InsertRichTooltip(GameItem source, GameItem target)
	{
		return null;
	}

	public virtual string RemoveTooltip(GameItem source, GameItem target)
	{
		return null;
	}

	public virtual RichTextBuilder RemoveRichTooltip(GameItem source, GameItem target)
	{
		return null;
	}
}
