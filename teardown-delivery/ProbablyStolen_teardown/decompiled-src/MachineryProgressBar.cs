using UnityEngine;

public class MachineryProgressBar : PixelElement, MachineryWidget
{
	private readonly TreeNodeRender frame;

	private readonly TreeNodeRender fill;

	private int forceWidth = -1;

	private int requestedWidth;

	public bool canResize => false;

	public int widthPixels { get; private set; }

	public int heightPixels { get; private set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner => handler;

	public bool fillWidth { get; set; } = true;


	public float value { get; private set; }

	public MachineryProgressBar(float value = 0f, int width = -1)
	{
		this.value = Mathf.Clamp01(value);
		forceWidth = width;
		handler = TreeNodeRender.Instantiate(this, "MachineryProgressBar");
		frame = TreeNodeRender.Instantiate().SetParent(handler);
		fill = TreeNodeRender.Instantiate().SetParent(handler);
		MachineryStyle.SetSliced(frame, MachineryStyle.ProgressFrame);
		frame.image.raycastTarget = false;
		MachineryStyle.SetFlat(fill, MachineryStyle.PROGRESS_FILL);
		fill.image.raycastTarget = false;
		Validate();
	}

	public MachineryProgressBar SetValue(float value)
	{
		this.value = Mathf.Clamp01(value);
		Validate();
		return this;
	}

	public MachineryProgressBar SetColor(Color color)
	{
		fill.image.color = color;
		return this;
	}

	public MachineryProgressBar SetWidth(int width)
	{
		forceWidth = width;
		Validate();
		return this;
	}

	public MachineryProgressBar SetFillWidth(bool fill)
	{
		fillWidth = fill;
		return this;
	}

	public void Validate()
	{
		widthPixels = ((forceWidth > 0) ? forceWidth : Mathf.Max(70, fillWidth ? requestedWidth : 0));
		heightPixels = 8;
		MachineryStyle.Place(frame, 0, 0, widthPixels, heightPixels);
		int num = 2;
		int num2 = Mathf.RoundToInt((float)(widthPixels - 2 * num) * value);
		fill.image.enabled = num2 > 0;
		MachineryStyle.Place(fill, num, num, num2, heightPixels - 2 * num);
	}

	public void ResizePixels(int width, int height)
	{
		requestedWidth = width;
		Validate();
	}

	public bool Attach(PixelElement element)
	{
		return false;
	}

	public bool Detach(PixelElement element)
	{
		return false;
	}

	public int WidthPixelsClamp(int width)
	{
		return widthPixels;
	}

	public int HeightPixelsClamp(int height)
	{
		return heightPixels;
	}

	public int XClamp(int x)
	{
		return 0;
	}

	public int YClamp(int y)
	{
		return 0;
	}
}
