using System;
using UnityEngine;

public abstract class SelectableElement : PixelElement, TooltipListener
{
	protected enum PreStatus
	{
		None,
		Hover,
		Press
	}

	public bool canResize => false;

	public int widthPixels { get; protected set; }

	public int heightPixels { get; protected set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner => handler;

	protected PreStatus currentPreStatus { get; private set; }

	public bool isSelected { get; private set; }

	public bool requiresHold { get; private set; }

	public float holdDuration { get; private set; } = 1.25f;


	public bool isDisabled { get; private set; }

	public bool isHidden { get; private set; }

	public SelectableElement(string treeNodeName = null)
	{
		handler = TreeNodeRender.Instantiate(this, "SelectableElement" + ((treeNodeName != null) ? (" (" + treeNodeName + ")") : ""));
	}

	public abstract void Validate();

	public virtual void ResizePixels(int width, int height)
	{
		widthPixels = width;
		heightPixels = height;
		Validate();
	}

	protected abstract void OnDefocus();

	protected abstract void OnHover();

	protected abstract void OnPress();

	protected abstract void OnSelect();

	protected abstract void OnDeselect();

	protected abstract void OnReset();

	protected abstract void OnDisable();

	protected abstract void OnEnable();

	protected abstract void OnHide();

	protected abstract void OnShow();

	public SelectableElement SetRequiresHold(bool value, float duration = 1.25f)
	{
		requiresHold = value;
		if (duration > 0f)
		{
			holdDuration = duration;
		}
		return this;
	}

	public bool Hover()
	{
		if (currentPreStatus != PreStatus.Hover)
		{
			OnHover();
			currentPreStatus = PreStatus.Hover;
			return true;
		}
		return false;
	}

	public bool Press()
	{
		if (currentPreStatus != PreStatus.Press)
		{
			OnPress();
			currentPreStatus = PreStatus.Press;
			return true;
		}
		return false;
	}

	public bool Defocus()
	{
		if (currentPreStatus != 0)
		{
			OnDefocus();
			currentPreStatus = PreStatus.None;
			return true;
		}
		return false;
	}

	public bool Select()
	{
		if (!isSelected)
		{
			isSelected = true;
			OnSelect();
			return true;
		}
		return false;
	}

	public bool Deselect()
	{
		if (isSelected)
		{
			isSelected = false;
			OnDeselect();
			return true;
		}
		return false;
	}

	public void Reset()
	{
		Deselect();
		Defocus();
		OnReset();
	}

	public void SetDisabled(bool isDisabled)
	{
		if (isDisabled)
		{
			Disable();
		}
		else
		{
			Enable();
		}
	}

	public bool Disable()
	{
		if (!isDisabled)
		{
			isDisabled = true;
			Reset();
			OnDisable();
			return true;
		}
		return false;
	}

	public bool Enable()
	{
		if (isDisabled)
		{
			isDisabled = false;
			Reset();
			OnEnable();
			return true;
		}
		return false;
	}

	public void SetHidden(bool isHidden)
	{
		if (isHidden)
		{
			Hide();
		}
		else
		{
			Show();
		}
	}

	public bool Hide()
	{
		if (!isHidden)
		{
			isHidden = true;
			Reset();
			OnHide();
			return true;
		}
		return false;
	}

	public bool Show()
	{
		if (isHidden)
		{
			isHidden = false;
			Reset();
			OnShow();
			return true;
		}
		return false;
	}

	public bool Attach(PixelElement element)
	{
		throw new NotImplementedException();
	}

	public bool Detach(PixelElement element)
	{
		throw new NotImplementedException();
	}

	public int XClamp(int x)
	{
		throw new NotImplementedException();
	}

	public int YClamp(int y)
	{
		throw new NotImplementedException();
	}

	public int WidthPixelsClamp(int width)
	{
		throw new NotImplementedException();
	}

	public int HeightPixelsClamp(int height)
	{
		throw new NotImplementedException();
	}

	public virtual bool HasTooltip()
	{
		return false;
	}

	public virtual RichText GetTooltip()
	{
		return null;
	}

	public virtual RichText GetTooltipAdvanced()
	{
		return GetTooltip();
	}
}
