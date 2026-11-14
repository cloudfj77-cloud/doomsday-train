public abstract class ItemTargetBehaviour : PriorityTrigger
{
	public bool CheckFilter(GameItem source, GameItem target)
	{
		if ((!Filter(source, target)) ?? false)
		{
			return false;
		}
		if (new bool?(CheckMay(source, target)) ?? false)
		{
			return true;
		}
		return false;
	}

	public bool CheckMay(GameItem source, GameItem target)
	{
		return MayTarget(source, target) ?? CanTarget(source, target).GetValueOrDefault();
	}

	public bool CheckCan(GameItem source, GameItem target)
	{
		bool? flag = MayTarget(source, target);
		if ((!flag) ?? false)
		{
			return false;
		}
		return CanTarget(source, target) ?? flag.GetValueOrDefault();
	}

	public RichTextBuilder GetTooltip(GameItem source, GameItem target)
	{
		if ((!ShowTooltip(source, target)) ?? false)
		{
			return null;
		}
		RichTextBuilder richTextBuilder = TargetRichTooltip(source, target);
		if (richTextBuilder == null)
		{
			string text = TargetTooltip(source, target);
			if (text != null)
			{
				return new RichTextBuilder().AddLine(text);
			}
		}
		return richTextBuilder;
	}

	public virtual bool? Filter(GameItem source, GameItem target)
	{
		return null;
	}

	public virtual bool? MayTarget(GameItem source, GameItem target)
	{
		return null;
	}

	public virtual bool? CanTarget(GameItem source, GameItem target)
	{
		return null;
	}

	public virtual bool? CanTargetPassthrough(GameItem source, GameItem target)
	{
		return null;
	}

	public virtual bool? OnTarget(GameItem source, GameItem target)
	{
		return null;
	}

	public virtual bool? ShowTooltip(GameItem source, GameItem target)
	{
		return null;
	}

	public virtual string TargetTooltip(GameItem source, GameItem target)
	{
		return null;
	}

	public virtual RichTextBuilder TargetRichTooltip(GameItem source, GameItem target)
	{
		return null;
	}
}
