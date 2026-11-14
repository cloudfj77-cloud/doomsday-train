using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MachineryRangeSlider : PixelElement, MachineryWidget
{
	private enum Thumb
	{
		None,
		Low,
		High
	}

	public Action<float, float> onChanged;

	public Action<float, float> onReleased;

	private readonly TreeNodeRender track;

	private readonly TreeNodeRender filled;

	private readonly TreeNodeRender emptyLeft;

	private readonly TreeNodeRender emptyRight;

	private readonly TreeNodeRender lowThumb;

	private readonly TreeNodeRender highThumb;

	private int forceWidth = -1;

	private int requestedWidth;

	private Thumb dragging;

	private Thumb hovering;

	public bool canResize => false;

	public int widthPixels { get; private set; }

	public int heightPixels { get; private set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner => handler;

	public bool fillWidth { get; set; } = true;


	public float min { get; private set; }

	public float max { get; private set; } = 1f;


	public float step { get; private set; }

	public float low { get; private set; }

	public float high { get; private set; }

	public bool isDisabled { get; private set; }

	public string description { get; set; }

	public MachineryRangeSlider(float low = 0f, float high = 1f, int width = -1)
	{
		forceWidth = width;
		this.low = low;
		this.high = high;
		handler = TreeNodeRender.Instantiate(this, "MachineryRangeSlider");
		track = TreeNodeRender.Instantiate().SetParent(handler);
		emptyLeft = TreeNodeRender.Instantiate().SetParent(handler);
		filled = TreeNodeRender.Instantiate().SetParent(handler);
		emptyRight = TreeNodeRender.Instantiate().SetParent(handler);
		lowThumb = TreeNodeRender.Instantiate().SetParent(handler);
		highThumb = TreeNodeRender.Instantiate().SetParent(handler);
		MachineryStyle.SetSliced(track, MachineryStyle.PlateNotched, MachineryStyle.SLIDER_TRACK);
		MachineryStyle.SetFlat(filled, MachineryStyle.SLIDER_FILL);
		MachineryStyle.SetFlat(emptyLeft, MachineryStyle.SLIDER_EMPTY);
		MachineryStyle.SetFlat(emptyRight, MachineryStyle.SLIDER_EMPTY);
		MachineryStyle.SetSimple(lowThumb, MachineryStyle.SliderThumbNormal, Color.white);
		MachineryStyle.SetSimple(highThumb, MachineryStyle.SliderThumbNormal, Color.white);
		filled.image.raycastTarget = false;
		emptyLeft.image.raycastTarget = false;
		emptyRight.image.raycastTarget = false;
		track.InitEventHandler();
		track.eventHandler.mouseDownFunc.Add(delegate(PointerEventData r)
		{
			if (isDisabled)
			{
				return true;
			}
			dragging = NearestThumb(r.position);
			SetFromPointer(r.position, notify: true);
			return true;
		});
		track.eventHandler.mouseBeginDragFunc.Add(delegate(PointerEventData r)
		{
			if (!isDisabled && dragging == Thumb.None)
			{
				dragging = NearestThumb(r.position);
			}
			return true;
		});
		track.eventHandler.mouseDragFunc.Add(delegate(PointerEventData r)
		{
			if (dragging != 0)
			{
				SetFromPointer(r.position, notify: true);
			}
			return true;
		});
		track.eventHandler.mouseEndDragFunc.Add(delegate
		{
			EndDrag();
			return true;
		});
		track.eventHandler.mouseUpFunc.Add(delegate
		{
			EndDrag();
			return true;
		});
		InitThumb(lowThumb, Thumb.Low);
		InitThumb(highThumb, Thumb.High);
		Validate();
	}

	private void InitThumb(TreeNodeRender thumb, Thumb which)
	{
		thumb.InitEventHandler();
		thumb.eventHandler.mouseBeginDragFunc.Add(delegate
		{
			if (!isDisabled)
			{
				dragging = which;
			}
			return true;
		});
		thumb.eventHandler.mouseDragFunc.Add(delegate(PointerEventData r)
		{
			if (dragging != 0)
			{
				SetFromPointer(r.position, notify: true);
			}
			return true;
		});
		thumb.eventHandler.mouseEndDragFunc.Add(delegate
		{
			EndDrag();
			return true;
		});
		thumb.eventHandler.mouseUpFunc.Add(delegate
		{
			EndDrag();
			return true;
		});
		thumb.eventHandler.mouseEnterFunc.Add(delegate
		{
			hovering = which;
			RefreshThumbs();
			return true;
		});
		thumb.eventHandler.mouseExitFunc.Add(delegate
		{
			if (hovering == which)
			{
				hovering = Thumb.None;
			}
			RefreshThumbs();
			return true;
		});
		thumb.eventHandler.mouseDownFunc.Add(delegate
		{
			handler.FindParentWindow()?.ToFront();
			return true;
		});
	}

	public MachineryRangeSlider SetRange(float min, float max, float step = 0f)
	{
		this.min = min;
		this.max = Mathf.Max(min, max);
		this.step = Mathf.Max(0f, step);
		SetValues(low, high);
		return this;
	}

	public MachineryRangeSlider SetValues(float newLow, float newHigh, bool notify = false)
	{
		float num = Snap(newHigh);
		float a = Mathf.Min(Snap(newLow), num);
		bool num2 = !Mathf.Approximately(a, low) || !Mathf.Approximately(num, high);
		low = a;
		high = num;
		Validate();
		if (num2 && notify)
		{
			onChanged?.Invoke(low, high);
		}
		return this;
	}

	public MachineryRangeSlider SetInts(int newLow, int newHigh, bool notify = false)
	{
		return SetValues(newLow, newHigh, notify);
	}

	public int GetLowInt()
	{
		return Mathf.RoundToInt(low);
	}

	public int GetHighInt()
	{
		return Mathf.RoundToInt(high);
	}

	private float Normalized(float value)
	{
		if (!(max > min))
		{
			return 0f;
		}
		return (value - min) / (max - min);
	}

	private float Snap(float raw)
	{
		raw = Mathf.Clamp(raw, min, max);
		if (step > 0f)
		{
			raw = min + Mathf.Round((raw - min) / step) * step;
		}
		return Mathf.Clamp(raw, min, max);
	}

	public MachineryRangeSlider SetDisabled(bool disabled)
	{
		isDisabled = disabled;
		dragging = Thumb.None;
		RefreshThumbs();
		return this;
	}

	public MachineryRangeSlider SetWidth(int width)
	{
		forceWidth = width;
		Validate();
		return this;
	}

	public MachineryRangeSlider SetFillWidth(bool fill)
	{
		fillWidth = fill;
		return this;
	}

	private float PointerValue(Vector2 screenPosition)
	{
		Vector2 vector = rectTransform.InverseTransformPoint(screenPosition);
		int num = Mathf.Max(1, widthPixels - 16);
		float num2 = Mathf.Clamp01((vector.x - 8f) / (float)num);
		return min + num2 * (max - min);
	}

	private Thumb NearestThumb(Vector2 screenPosition)
	{
		float num = PointerValue(screenPosition);
		float num2 = Mathf.Abs(num - low);
		float num3 = Mathf.Abs(num - high);
		if (Mathf.Approximately(num2, num3))
		{
			if (!(num < low))
			{
				return Thumb.High;
			}
			return Thumb.Low;
		}
		if (!(num2 < num3))
		{
			return Thumb.High;
		}
		return Thumb.Low;
	}

	private void SetFromPointer(Vector2 screenPosition, bool notify)
	{
		if (dragging == Thumb.None)
		{
			return;
		}
		float num = Snap(PointerValue(screenPosition));
		float num2 = low;
		float num3 = high;
		if (dragging == Thumb.Low)
		{
			num2 = num;
			if (num3 < num2)
			{
				num3 = num2;
			}
		}
		else
		{
			num3 = num;
			if (num2 > num3)
			{
				num2 = num3;
			}
		}
		bool num4 = !Mathf.Approximately(num2, low) || !Mathf.Approximately(num3, high);
		low = num2;
		high = num3;
		Validate();
		if (num4 && notify)
		{
			onChanged?.Invoke(low, high);
		}
	}

	private void EndDrag()
	{
		if (dragging != 0)
		{
			dragging = Thumb.None;
			onReleased?.Invoke(low, high);
			RefreshThumbs();
		}
	}

	private void RefreshThumbs()
	{
		lowThumb.image.sprite = ((hovering == Thumb.Low || dragging == Thumb.Low) ? MachineryStyle.SliderThumbHover : MachineryStyle.SliderThumbNormal);
		highThumb.image.sprite = ((hovering == Thumb.High || dragging == Thumb.High) ? MachineryStyle.SliderThumbHover : MachineryStyle.SliderThumbNormal);
		Color color = (isDisabled ? MachineryStyle.BUTTON_DISABLED_TINT : Color.white);
		lowThumb.image.color = color;
		highThumb.image.color = color;
		filled.image.color = (isDisabled ? (MachineryStyle.BUTTON_DISABLED_TINT * MachineryStyle.SLIDER_FILL) : MachineryStyle.SLIDER_FILL);
		if (dragging == Thumb.Low || (dragging == Thumb.None && hovering == Thumb.Low))
		{
			lowThumb.rectTransform.SetAsLastSibling();
		}
		else
		{
			highThumb.rectTransform.SetAsLastSibling();
		}
	}

	public void Validate()
	{
		widthPixels = ((forceWidth > 0) ? forceWidth : Mathf.Max(96, fillWidth ? requestedWidth : 0));
		heightPixels = 8;
		int num = 2;
		int num2 = 4;
		MachineryStyle.Place(track, 0, num, widthPixels, num2);
		int num3 = widthPixels - 16;
		int num4 = Mathf.RoundToInt((float)num3 * Normalized(low));
		int num5 = Mathf.RoundToInt((float)num3 * Normalized(high));
		int num6 = num4 + 8;
		int num7 = num5 + 8;
		MachineryStyle.Place(emptyLeft, 1, num + 1, Mathf.Max(0, num6 - 1), num2 - 2);
		MachineryStyle.Place(filled, num6, num + 1, Mathf.Max(0, num7 - num6), num2 - 2);
		int width = Mathf.Max(0, widthPixels - 2 - num7);
		MachineryStyle.Place(emptyRight, num7, num + 1, width, num2 - 2);
		MachineryStyle.Place(lowThumb, num4, 0, 16, 8);
		MachineryStyle.Place(highThumb, num5, 0, 16, 8);
		RefreshThumbs();
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
