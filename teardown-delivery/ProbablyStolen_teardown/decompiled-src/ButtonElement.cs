using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class ButtonElement : SelectableElement
{
	public enum Color
	{
		None,
		Red,
		Green,
		Blue,
		Orange,
		Purple
	}

	private readonly TreeNodeRender textNode;

	private readonly TreeNodeRender buttonNode;

	private int forceWidth;

	private int forceHeight;

	private Action callback;

	private Sprite buttonUpSprite = RenderHandler.LoadFromAtlas("Window/elements_inner", "button_up");

	private Color32 buttonTextColor = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.White);

	private Color32 buttonTextColorSelect = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.Gray);

	public string onClickAudio;

	private bool wasHover;

	public string text { get; private set; }

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

	public string description { get; set; }

	public Color color { get; private set; }

	public ButtonElement SetColor(Color color)
	{
		if (color == this.color)
		{
			return this;
		}
		this.color = color;
		ValidateColor(base.isDisabled);
		return this;
	}

	public ButtonElement(string text = null, int width = -1, int height = -1, Action callback = null, Color color = Color.None, string onClickAudio = "interface_click")
	{
		this.text = text;
		forceWidth = width;
		forceHeight = height;
		this.callback = callback;
		this.onClickAudio = onClickAudio;
		buttonNode = TreeNodeRender.Instantiate().SetParent(base.handler);
		textNode = TreeNodeRender.InstantiateText().SetParent(base.handler);
		buttonNode.InitEventHandler();
		buttonNode.name = "Node (Button)";
		textNode.text.text = text ?? "";
		textNode.text.color = buttonTextColor;
		textNode.text.fontStyle = FontStyles.Bold;
		textNode.text.SetNativeSize();
		textNode.text.enabled = hasText;
		buttonNode.image.sprite = buttonUpSprite;
		buttonNode.image.SetNativeSize();
		SetColor(color).Enable();
		buttonNode.image.enabled = true;
		ResizePixels(width, height);
		Validate();
	}

	public ButtonElement Set(string text = null, int width = -1, int height = -1)
	{
		if (text == this.text)
		{
			return this;
		}
		this.text = text;
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
		Validate();
		return this;
	}

	public ButtonElement SetCallback(Action callback)
	{
		this.callback = callback;
		return this;
	}

	public override void Validate()
	{
		if (!base.isHidden)
		{
			int num = (text?.Length ?? 0) * UISettings.current.fontWidth;
			int fontHeight = UISettings.current.fontHeight;
			int num2 = 0;
			int num3 = 0;
			if (forceWidth > 0)
			{
				base.widthPixels = forceWidth;
				num2 = (base.widthPixels - num) / 2 - 4;
			}
			else
			{
				base.widthPixels = (text?.Length ?? 0) * UISettings.current.fontWidth + 7 + 4;
			}
			if (forceHeight > 0)
			{
				base.heightPixels = forceHeight;
				num3 = (base.heightPixels - fontHeight) / 2;
			}
			else
			{
				base.heightPixels = UISettings.current.fontHeight + 3;
			}
			num = Mathf.Min(num, base.widthPixels);
			fontHeight = Mathf.Min(fontHeight, base.heightPixels);
			textNode.rectTransform.sizeDelta = new Vector2(base.widthPixels - 2, base.heightPixels - 2);
			buttonNode.rectTransform.sizeDelta = new Vector2(base.widthPixels, base.heightPixels);
			buttonNode.rectTransform.anchoredPosition = Vector2.zero;
			textNode.rectTransform.anchoredPosition = new Vector2(num2 + 1, -num3 - 1);
			textNode.text.verticalAlignment = VerticalAlignmentOptions.Middle;
			textNode.text.horizontalAlignment = HorizontalAlignmentOptions.Center;
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
		buttonNode.rectTransform.pivot = new Vector2(0f, 1f);
		buttonNode.rectTransform.localScale = new Vector2(1f, 1f);
		wasHover = false;
	}

	protected override void OnHover()
	{
		textNode.text.color = buttonTextColorSelect;
		buttonNode.rectTransform.pivot = new Vector2(0f, 1f);
		buttonNode.rectTransform.localScale = new Vector2(1f, 1f);
		wasHover = true;
	}

	protected override void OnPress()
	{
		textNode.text.color = buttonTextColorSelect;
		buttonNode.rectTransform.pivot = new Vector2(1f, 0f);
		buttonNode.rectTransform.localScale = new Vector2(-1f, -1f);
		if (wasHover)
		{
			AudioManager.Instance.Play(onClickAudio);
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
	}

	protected override void OnEnable()
	{
		ValidateColor(isDisabled: false);
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
			case Color.Blue:
				buttonUpSprite = RenderHandler.LoadFromAtlas("Window/elements_inner", "button_disabled_blue");
				buttonTextColor = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.LightBlue);
				break;
			case Color.Orange:
				buttonUpSprite = RenderHandler.LoadFromAtlas("Window/elements_inner", "button_disabled_orange");
				buttonTextColor = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.LightOrange);
				break;
			case Color.Purple:
				buttonUpSprite = RenderHandler.LoadFromAtlas("Window/elements_inner", "button_disabled_purple");
				buttonTextColor = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.LightPurple);
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
			case Color.Blue:
				buttonUpSprite = RenderHandler.LoadFromAtlas("Window/elements_inner", "button_up_blue");
				buttonTextColor = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.White);
				buttonTextColorSelect = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.DarkBlue);
				break;
			case Color.Orange:
				buttonUpSprite = RenderHandler.LoadFromAtlas("Window/elements_inner", "button_up_orange");
				buttonTextColor = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.White);
				buttonTextColorSelect = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.DarkOrange);
				break;
			case Color.Purple:
				buttonUpSprite = RenderHandler.LoadFromAtlas("Window/elements_inner", "button_up_purple");
				buttonTextColor = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.White);
				buttonTextColorSelect = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.DarkPurple);
				break;
			default:
				buttonUpSprite = RenderHandler.LoadFromAtlas("Window/elements_inner", "button_up");
				buttonTextColor = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.White);
				buttonTextColorSelect = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.Gray);
				break;
			}
		}
		buttonNode.image.sprite = buttonUpSprite;
		textNode.text.color = buttonTextColor;
	}

	protected override void OnHide()
	{
		buttonNode.image.enabled = false;
		textNode.text.enabled = false;
	}

	protected override void OnShow()
	{
		buttonNode.image.enabled = true;
		textNode.text.enabled = true;
	}

	public ButtonElement SetDescription(string description)
	{
		this.description = description;
		return this;
	}

	public override bool HasTooltip()
	{
		string obj = description;
		if (obj == null)
		{
			return false;
		}
		return obj.Length > 0;
	}

	public override RichText GetTooltip()
	{
		return new RichTextBuilder().Add(description).Build();
	}
}
