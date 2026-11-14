using UnityEngine;

public interface PixelElement
{
	bool canResize { get; }

	int widthPixels { get; }

	int heightPixels { get; }

	TreeNodeRender handler { get; }

	RectTransform rectTransform { get; }

	TreeNodeRender inner { get; }

	int XClamp(int x);

	int YClamp(int y);

	int SetXClamp(int x)
	{
		x = XClamp(x);
		int num = (int)handler.rectTransform.anchoredPosition.y;
		handler.rectTransform.anchoredPosition = new Vector2(x, num);
		Validate();
		return x;
	}

	int SetYClamp(int y)
	{
		y = YClamp(y);
		int num = (int)handler.rectTransform.anchoredPosition.x;
		handler.rectTransform.anchoredPosition = new Vector2(num, y);
		Validate();
		return y;
	}

	int WidthPixelsClamp(int width);

	int HeightPixelsClamp(int height);

	int SetWidthPixelsClamp(int width)
	{
		ResizePixels(width, heightPixels);
		return widthPixels;
	}

	int SetHeightPixelsClamp(int height)
	{
		ResizePixels(widthPixels, height);
		return heightPixels;
	}

	void ResizePixels(int width, int height);

	void Validate();

	bool Attach(PixelElement element);

	bool Detach(PixelElement element);

	bool SetRenderingParent(PixelElement parent)
	{
		if (parent != null && (object)parent.inner != null)
		{
			handler.SetParent(parent.inner);
			return true;
		}
		return false;
	}

	bool IsRenderingChildOf(PixelElement parent)
	{
		if (parent != null && (object)parent.inner != null)
		{
			return handler.parent == parent.inner;
		}
		return false;
	}

	bool RemoveRenderingParent(PixelElement parent)
	{
		if (!IsRenderingChildOf(parent))
		{
			return false;
		}
		handler.ResetParent();
		return true;
	}

	void ToFront()
	{
		handler.rectTransform.SetAsLastSibling();
	}

	void ToBack()
	{
		handler.rectTransform.SetAsFirstSibling();
	}
}
