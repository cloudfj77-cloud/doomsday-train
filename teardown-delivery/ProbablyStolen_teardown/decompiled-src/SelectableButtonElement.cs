using System;

public class SelectableButtonElement : SelectableElement
{
	public TreeNodeRender buttonSprite { get; }

	public SelectableButtonElement()
	{
		buttonSprite = TreeNodeRender.InstantiateText().SetParent(base.inner);
	}

	public override void Validate()
	{
		throw new NotImplementedException();
	}

	public override void ResizePixels(int width, int height)
	{
		throw new NotImplementedException();
	}

	protected override void OnHover()
	{
		throw new NotImplementedException();
	}

	protected override void OnPress()
	{
		throw new NotImplementedException();
	}

	protected override void OnDefocus()
	{
		throw new NotImplementedException();
	}

	protected override void OnSelect()
	{
		throw new NotImplementedException();
	}

	protected override void OnDeselect()
	{
		throw new NotImplementedException();
	}

	protected override void OnReset()
	{
		throw new NotImplementedException();
	}

	protected override void OnDisable()
	{
		throw new NotImplementedException();
	}

	protected override void OnEnable()
	{
		throw new NotImplementedException();
	}

	protected override void OnHide()
	{
		throw new NotImplementedException();
	}

	protected override void OnShow()
	{
		throw new NotImplementedException();
	}
}
