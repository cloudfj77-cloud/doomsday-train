public abstract class ItemUseBehaviour : PriorityTrigger
{
	public bool CheckFilter(GameItem source)
	{
		if ((!Filter(source)) ?? false)
		{
			return false;
		}
		if (new bool?(CheckMay(source)) ?? false)
		{
			return true;
		}
		return false;
	}

	public bool CheckMay(GameItem source)
	{
		return MayUse(source) ?? CanUse(source).GetValueOrDefault();
	}

	public bool CheckCan(GameItem source)
	{
		bool? flag = MayUse(source);
		if ((!flag) ?? false)
		{
			return false;
		}
		return CanUse(source) ?? flag.GetValueOrDefault();
	}

	public RichTextBuilder GetTooltip(GameItem source)
	{
		if ((!ShowTooltip(source)) ?? false)
		{
			return null;
		}
		RichTextBuilder richTextBuilder = UseRichTooltip(source);
		if (richTextBuilder == null)
		{
			string text = UseTooltip(source);
			if (text != null)
			{
				return new RichTextBuilder().AddLine(text);
			}
		}
		return richTextBuilder;
	}

	public virtual bool? Filter(GameItem source)
	{
		return null;
	}

	public virtual bool? MayUse(GameItem source)
	{
		return null;
	}

	public virtual bool? CanUse(GameItem source)
	{
		return null;
	}

	public virtual bool? CanUsePassthrough(GameItem source)
	{
		return null;
	}

	public virtual bool? OnUse(GameItem source)
	{
		return null;
	}

	public virtual bool? ShowTooltip(GameItem source)
	{
		return null;
	}

	public virtual string UseTooltip(GameItem source)
	{
		return null;
	}

	public virtual RichTextBuilder UseRichTooltip(GameItem source)
	{
		return null;
	}
}
