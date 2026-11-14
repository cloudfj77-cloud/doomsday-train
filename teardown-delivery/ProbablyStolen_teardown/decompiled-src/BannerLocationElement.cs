using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class BannerLocationElement : SelectableElement
{
	public enum Color
	{
		None,
		Red,
		Green
	}

	private readonly TreeNodeRender textNode;

	private readonly TreeNodeRender distTextNode;

	private readonly TreeNodeRender buttonNode;

	private int forceWidth;

	private int forceHeight;

	private Action callback;

	private Sprite buttonUpSprite = RenderHandler.LoadFromAtlas("Window/elements_inner", "button_up");

	private Sprite customSprite;

	private Color32 buttonTextColor = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.LightGray);

	private Color32 buttonTextColorSelect = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.White);

	private UnityEngine.Color customDefault = new UnityEngine.Color(0.8f, 0.8f, 0.8f);

	private UnityEngine.Color customSelect = new UnityEngine.Color(1f, 1f, 1f);

	private bool wasHover;

	private int si;

	public string text { get; private set; }

	public string distText { get; private set; }

	public bool hasText
	{
		get
		{
			if (text != null)
			{
				return text.Count() > 0;
			}
			return false;
		}
	}

	public bool isScanning { get; set; }

	private Color color { get; set; }

	public BannerLocationElement SetColor(Color color)
	{
		this.color = color;
		ValidateColor(base.isDisabled);
		return this;
	}

	public BannerLocationElement(string text = null, string distText = null, int width = -1, int height = -1, Action callback = null, Color color = Color.None)
	{
		this.text = text;
		this.distText = distText;
		forceWidth = width;
		forceHeight = height;
		this.callback = callback;
		buttonNode = TreeNodeRender.Instantiate().SetParent(base.handler);
		textNode = TreeNodeRender.InstantiateText().SetParent(base.handler);
		distTextNode = TreeNodeRender.InstantiateText().SetParent(base.handler);
		buttonNode.InitEventHandler();
		textNode.text.text = text ?? "";
		textNode.text.color = buttonTextColor;
		textNode.text.fontStyle = FontStyles.Bold;
		textNode.text.SetNativeSize();
		textNode.text.enabled = hasText;
		textNode.text.overflowMode = TextOverflowModes.Overflow;
		textNode.text.enableWordWrapping = false;
		distTextNode.text.text = distText ?? "";
		distTextNode.text.color = buttonTextColor;
		distTextNode.text.fontStyle = FontStyles.Bold;
		distTextNode.text.SetNativeSize();
		distTextNode.text.enabled = hasText;
		buttonNode.image.sprite = buttonUpSprite;
		buttonNode.image.SetNativeSize();
		SetColor(color).Enable();
		buttonNode.image.enabled = true;
		ResizePixels(width, height);
		Validate();
	}

	public BannerLocationElement Set(string text = null, string distText = null, string customSpritePath = null, int width = -1, int height = -1)
	{
		this.text = text;
		this.distText = distText;
		if (width != -1)
		{
			forceWidth = width;
		}
		if (height != -1)
		{
			forceHeight = height;
		}
		textNode.text.text = text ?? "";
		textNode.text.fontStyle = FontStyles.Bold;
		textNode.text.SetNativeSize();
		textNode.text.enabled = hasText && !base.isHidden;
		distTextNode.text.text = distText ?? "";
		distTextNode.text.fontStyle = FontStyles.Bold;
		distTextNode.text.SetNativeSize();
		distTextNode.text.enabled = hasText && !base.isHidden;
		if (customSpritePath != null)
		{
			customSprite = RenderHandler.LoadFromAtlas("POICards", customSpritePath);
		}
		else
		{
			customSprite = null;
		}
		buttonNode.image.color = customSelect;
		ValidateText();
		Validate();
		return this;
	}

	public BannerLocationElement SetCallback(Action callback)
	{
		this.callback = callback;
		return this;
	}

	public override void Validate()
	{
		if (!base.isHidden)
		{
			int a = (textNode.text.text?.Length ?? 0) * UISettings.current.fontWidth;
			int fontHeight = UISettings.current.fontHeight;
			int num = (distTextNode.text.text?.Length ?? 0) * UISettings.current.fontWidth;
			if (forceWidth > 0)
			{
				base.widthPixels = forceWidth;
			}
			else
			{
				base.widthPixels = (textNode.text.text?.Length ?? 0) * UISettings.current.fontWidth + 7 + 4;
			}
			if (forceHeight > 0)
			{
				base.heightPixels = forceHeight;
			}
			else
			{
				base.heightPixels = UISettings.current.fontHeight + 3;
			}
			Mathf.Min(a, base.widthPixels);
			fontHeight = Mathf.Min(fontHeight, base.heightPixels);
			textNode.rectTransform.sizeDelta = new Vector2(base.widthPixels - UISettings.current.fontWidth, fontHeight);
			buttonNode.rectTransform.sizeDelta = new Vector2(base.widthPixels, base.heightPixels);
			buttonNode.rectTransform.anchoredPosition = Vector2.zero;
			textNode.rectTransform.anchoredPosition = new Vector2(4f, -3f);
			distTextNode.rectTransform.sizeDelta = new Vector2(num, fontHeight);
			distTextNode.rectTransform.anchoredPosition = new Vector2(4f, -base.heightPixels + fontHeight - 3);
		}
	}

	public override void ResizePixels(int width, int height)
	{
		forceHeight = height;
		forceWidth = width;
		Validate();
	}

	protected override void OnDefocus()
	{
		textNode.text.color = buttonTextColor;
		distTextNode.text.color = buttonTextColor;
		buttonNode.rectTransform.pivot = new Vector2(0f, 1f);
		buttonNode.rectTransform.localScale = new Vector2(1f, 1f);
		if ((object)customSprite != null)
		{
			buttonNode.image.color = customDefault;
		}
		else
		{
			buttonNode.image.color = customSelect;
		}
		wasHover = false;
	}

	protected override void OnHover()
	{
		textNode.text.color = buttonTextColorSelect;
		distTextNode.text.color = buttonTextColorSelect;
		buttonNode.rectTransform.pivot = new Vector2(0f, 1f);
		buttonNode.rectTransform.localScale = new Vector2(1f, 1f);
		buttonNode.image.color = customSelect;
		wasHover = true;
	}

	protected override void OnPress()
	{
		textNode.text.color = buttonTextColorSelect;
		distTextNode.text.color = buttonTextColorSelect;
		buttonNode.rectTransform.pivot = new Vector2(1f, 0f);
		buttonNode.rectTransform.localScale = new Vector2(-1f, -1f);
		if (wasHover)
		{
			AudioHelper.UIAudioVintageClicked();
		}
		if ((object)customSprite != null)
		{
			buttonNode.rectTransform.pivot = new Vector2(0f, 1f);
			buttonNode.rectTransform.localScale = new Vector2(1f, 1f);
		}
		wasHover = false;
	}

	protected override void OnSelect()
	{
		callback?.Invoke();
		Deselect();
	}

	protected override void OnDeselect()
	{
	}

	protected override void OnReset()
	{
		Defocus();
	}

	protected override void OnDisable()
	{
		ValidateColor(isDisabled: true);
		isScanning = false;
	}

	protected override void OnEnable()
	{
		ValidateColor(isDisabled: false);
		isScanning = false;
	}

	private void ValidateText()
	{
		if (base.isDisabled)
		{
			textNode.text.text = "";
			distTextNode.text.text = "?";
		}
		else
		{
			textNode.text.text = text;
			distTextNode.text.text = distText;
		}
	}

	private void ValidateColor(bool isDisabled)
	{
		if (isDisabled)
		{
			switch (color)
			{
			case Color.Red:
				buttonUpSprite = RenderHandler.LoadFromAtlas("Window/elements_inner", "button_disabled_red");
				buttonTextColor = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.LightRed);
				break;
			case Color.Green:
				buttonUpSprite = RenderHandler.LoadFromAtlas("Window/elements_inner", "button_disabled_green");
				buttonTextColor = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.LightGreen);
				break;
			default:
				buttonUpSprite = RenderHandler.LoadFromAtlas("Window/elements_inner", "button_disabled");
				buttonTextColor = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.LightGray);
				break;
			}
		}
		else
		{
			switch (color)
			{
			case Color.Red:
				buttonUpSprite = RenderHandler.LoadFromAtlas("Window/elements_inner", "button_up_red");
				buttonTextColor = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.White);
				buttonTextColorSelect = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.DarkRed);
				break;
			case Color.Green:
				buttonUpSprite = RenderHandler.LoadFromAtlas("Window/elements_inner", "button_up_green");
				buttonTextColor = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.White);
				buttonTextColorSelect = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.DarkGreen);
				break;
			default:
				buttonUpSprite = RenderHandler.LoadFromAtlas("Window/elements_inner", "button_up");
				buttonTextColor = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.White);
				buttonTextColorSelect = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.Gray);
				break;
			}
		}
		if ((object)customSprite != null && !isDisabled)
		{
			buttonTextColor = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.LightGray);
			buttonTextColorSelect = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.White);
		}
		buttonNode.image.sprite = buttonUpSprite;
		textNode.text.color = buttonTextColor;
		distTextNode.text.color = buttonTextColor;
		if ((object)customSprite != null && !isDisabled)
		{
			buttonNode.image.sprite = customSprite;
			buttonNode.image.color = customDefault;
		}
		ValidateText();
		Validate();
	}

	protected override void OnHide()
	{
		buttonNode.image.enabled = false;
		textNode.text.enabled = false;
		distTextNode.text.enabled = false;
	}

	protected override void OnShow()
	{
		buttonNode.image.enabled = true;
		textNode.text.enabled = true;
		distTextNode.text.enabled = true;
	}

	public void OnScanning()
	{
		if (isScanning)
		{
			distTextNode.text.text = "?";
			switch (si)
			{
			case 0:
				textNode.text.text = ".";
				break;
			case 1:
				textNode.text.text = "..";
				break;
			default:
				textNode.text.text = "...";
				break;
			}
			Validate();
		}
		si++;
		si %= 3;
	}
}
