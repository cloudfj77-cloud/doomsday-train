using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MachinerySlider : PixelElement, MachineryWidget
{
	public Action<float> onChanged;

	public Action<float> onReleased;

	private readonly TreeNodeRender track;

	private readonly TreeNodeRender filled;

	private readonly TreeNodeRender empty;

	private readonly TreeNodeRender thumb;

	private int forceWidth = -1;

	private int requestedWidth;

	private bool dragging;

	private bool hovering;

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

	public float value { get; private set; }

	public bool isDisabled { get; private set; }

	public string description { get; set; }

	public float normalized
	{
		get
		{
			if (!(max > min))
			{
				return 0f;
			}
			return (value - min) / (max - min);
		}
	}

	public MachinerySlider(float value = 0f, int width = -1)
	{
		forceWidth = width;
		this.value = value;
		handler = TreeNodeRender.Instantiate(this, "MachinerySlider");
		track = TreeNodeRender.Instantiate().SetParent(handler);
		filled = TreeNodeRender.Instantiate().SetParent(handler);
		empty = TreeNodeRender.Instantiate().SetParent(handler);
		thumb = TreeNodeRender.Instantiate().SetParent(handler);
		MachineryStyle.SetSliced(track, MachineryStyle.PlateNotched, MachineryStyle.SLIDER_TRACK);
		MachineryStyle.SetFlat(filled, MachineryStyle.SLIDER_FILL);
		MachineryStyle.SetFlat(empty, MachineryStyle.SLIDER_EMPTY);
		MachineryStyle.SetSimple(thumb, MachineryStyle.SliderThumbNormal, Color.white);
		filled.image.raycastTarget = false;
		empty.image.raycastTarget = false;
		track.InitEventHandler();
		track.eventHandler.mouseDownFunc.Add(delegate(PointerEventData r)
		{
			if (isDisabled)
			{
				return true;
			}
			SetFromPointer(r.position, notify: true);
			return true;
		});
		track.eventHandler.mouseBeginDragFunc.Add(delegate
		{
			if (!isDisabled)
			{
				dragging = true;
			}
			return true;
		});
		track.eventHandler.mouseDragFunc.Add(delegate(PointerEventData r)
		{
			if (dragging)
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
		thumb.InitEventHandler();
		thumb.eventHandler.mouseBeginDragFunc.Add(delegate
		{
			if (!isDisabled)
			{
				dragging = true;
			}
			return true;
		});
		thumb.eventHandler.mouseDragFunc.Add(delegate(PointerEventData r)
		{
			if (dragging)
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
			hovering = true;
			RefreshThumb();
			return true;
		});
		thumb.eventHandler.mouseExitFunc.Add(delegate
		{
			hovering = false;
			RefreshThumb();
			return true;
		});
		thumb.eventHandler.mouseDownFunc.Add(delegate
		{
			handler.FindParentWindow()?.ToFront();
			return true;
		});
		Validate();
	}

	public MachinerySlider SetRange(float min, float max, float step = 0f)
	{
		this.min = min;
		this.max = Mathf.Max(min, max);
		this.step = Mathf.Max(0f, step);
		SetValue(value);
		return this;
	}

	public MachinerySlider SetSteps(int count)
	{
		count = Mathf.Max(2, count);
		return SetRange(0f, count - 1, 1f);
	}

	public MachinerySlider SetValue(float newValue, bool notify = false)
	{
		float a = Snap(newValue);
		bool num = !Mathf.Approximately(a, value);
		value = a;
		Validate();
		if (num && notify)
		{
			onChanged?.Invoke(value);
		}
		return this;
	}

	public MachinerySlider SetInt(int newValue, bool notify = false)
	{
		return SetValue(newValue, notify);
	}

	public int GetInt()
	{
		return Mathf.RoundToInt(value);
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

	public MachinerySlider SetDisabled(bool disabled)
	{
		isDisabled = disabled;
		dragging = false;
		RefreshThumb();
		return this;
	}

	public MachinerySlider SetWidth(int width)
	{
		forceWidth = width;
		Validate();
		return this;
	}

	public MachinerySlider SetFillWidth(bool fill)
	{
		fillWidth = fill;
		return this;
	}

	private void SetFromPointer(Vector2 screenPosition, bool notify)
	{
		Vector2 vector = rectTransform.InverseTransformPoint(screenPosition);
		int num = Mathf.Max(1, widthPixels - 16);
		float num2 = Mathf.Clamp01((vector.x - 8f) / (float)num);
		SetValue(min + num2 * (max - min), notify);
	}

	private void EndDrag()
	{
		if (dragging)
		{
			dragging = false;
			onReleased?.Invoke(value);
			RefreshThumb();
		}
	}

	private void RefreshThumb()
	{
		thumb.image.sprite = ((hovering || dragging) ? MachineryStyle.SliderThumbHover : MachineryStyle.SliderThumbNormal);
		thumb.image.color = (isDisabled ? MachineryStyle.BUTTON_DISABLED_TINT : Color.white);
		filled.image.color = (isDisabled ? (MachineryStyle.BUTTON_DISABLED_TINT * MachineryStyle.SLIDER_FILL) : MachineryStyle.SLIDER_FILL);
	}

	public void Validate()
	{
		widthPixels = ((forceWidth > 0) ? forceWidth : Mathf.Max(96, fillWidth ? requestedWidth : 0));
		heightPixels = 8;
		int num = 2;
		int num2 = 4;
		MachineryStyle.Place(track, 0, num, widthPixels, num2);
		int num3 = Mathf.RoundToInt((float)(widthPixels - 16) * normalized);
		int num4 = num3 + 8;
		MachineryStyle.Place(filled, 1, num + 1, Mathf.Max(0, num4 - 1), num2 - 2);
		int width = Mathf.Max(0, widthPixels - 2 - num4);
		MachineryStyle.Place(empty, num4, num + 1, width, num2 - 2);
		MachineryStyle.Place(thumb, num3, 0, 16, 8);
		RefreshThumb();
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
