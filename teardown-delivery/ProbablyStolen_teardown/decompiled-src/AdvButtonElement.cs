using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class AdvButtonElement : SelectableElement
{
	private readonly TreeNodeRender textNode;

	private readonly TreeNodeRender buttonNode;

	private int forceWidth;

	private int forceHeight;

	private Action callback;

	private Color32 fallbackNormalColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	private Color32 fallbackHighlightColor = new Color32(200, 200, 200, byte.MaxValue);

	private Color32 fallbackClickedColor = new Color32(160, 160, 160, byte.MaxValue);

	private Color32 fallbackDisabledColor = new Color32(100, 100, 100, byte.MaxValue);

	private Color32 buttonTextColorNormal = new Color32(178, 107, 78, byte.MaxValue);

	private Color32 buttonTextColorHighlight = new Color32(214, 155, 102, byte.MaxValue);

	private Color32 buttonTextColorClicked = new Color32(178, 107, 78, byte.MaxValue);

	private Color32 buttonTextColorDisabled = new Color32(125, 125, 125, byte.MaxValue);

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

	public Sprite spriteNormal { get; private set; }

	public Sprite spriteHighlight { get; private set; }

	public Sprite spriteClicked { get; private set; }

	public Sprite spriteDisabled { get; private set; }

	public string description { get; set; }

	public AdvButtonElement SetSpriteNormal(Sprite sprite)
	{
		spriteNormal = sprite;
		ApplyCurrentState();
		return this;
	}

	public AdvButtonElement SetSpriteHighlight(Sprite sprite)
	{
		spriteHighlight = sprite;
		return this;
	}

	public AdvButtonElement SetSpriteClicked(Sprite sprite)
	{
		spriteClicked = sprite;
		return this;
	}

	public AdvButtonElement SetSpriteDisabled(Sprite sprite)
	{
		spriteDisabled = sprite;
		return this;
	}

	public AdvButtonElement SetFallbackHighlightColor(Color32 color)
	{
		fallbackHighlightColor = color;
		return this;
	}

	public AdvButtonElement SetFallbackClickedColor(Color32 color)
	{
		fallbackClickedColor = color;
		return this;
	}

	public AdvButtonElement SetFallbackDisabledColor(Color32 color)
	{
		fallbackDisabledColor = color;
		return this;
	}

	public AdvButtonElement(Sprite normalSprite = null, string text = null, int width = -1, int height = -1, Action callback = null, string onClickAudio = "interface_click")
	{
		this.text = text;
		forceWidth = width;
		forceHeight = height;
		this.callback = callback;
		this.onClickAudio = onClickAudio;
		spriteNormal = ((normalSprite != null) ? normalSprite : RenderHandler.LoadFromAtlas("Window/elements_inner", "default"));
		spriteHighlight = ((normalSprite != null) ? null : RenderHandler.LoadFromAtlas("Window/elements_inner", "default_hl"));
		spriteClicked = ((normalSprite != null) ? null : RenderHandler.LoadFromAtlas("Window/elements_inner", "default_c"));
		spriteDisabled = ((normalSprite != null) ? null : RenderHandler.LoadFromAtlas("Window/elements_inner", "default_d"));
		buttonNode = TreeNodeRender.Instantiate().SetParent(base.handler);
		textNode = TreeNodeRender.InstantiateText().SetParent(base.handler);
		buttonNode.InitEventHandler();
		buttonNode.name = "Node (AdvButton)";
		textNode.text.text = text ?? "";
		textNode.text.color = buttonTextColorNormal;
		textNode.text.fontStyle = FontStyles.Bold;
		textNode.text.SetNativeSize();
		textNode.text.enabled = hasText;
		buttonNode.image.sprite = spriteNormal;
		buttonNode.image.color = fallbackNormalColor;
		buttonNode.image.SetNativeSize();
		Enable();
		buttonNode.image.enabled = true;
		ResizePixels(width, height);
		Validate();
	}

	public AdvButtonElement Set(string text = null, int width = -1, int height = -1)
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

	public AdvButtonElement SetCallback(Action callback)
	{
		this.callback = callback;
		return this;
	}

	public override void Validate()
	{
		if (!base.isHidden)
		{
			int num = (text?.Length ?? 0) * UISettings.current.effectiveFontWidth;
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
				base.widthPixels = (text?.Length ?? 0) * UISettings.current.effectiveFontWidth + 7 + 4;
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

	private void ApplyStateSprite(Sprite stateSprite, Color32 fallbackTint)
	{
		if (stateSprite != null)
		{
			buttonNode.image.sprite = stateSprite;
			buttonNode.image.color = fallbackNormalColor;
		}
		else
		{
			buttonNode.image.sprite = spriteNormal;
			buttonNode.image.color = fallbackTint;
		}
	}

	private void ApplyCurrentState()
	{
		if (base.isDisabled)
		{
			ApplyStateSprite(spriteDisabled, fallbackDisabledColor);
			return;
		}
		if (base.currentPreStatus == PreStatus.Press)
		{
			ApplyStateSprite(spriteClicked, fallbackClickedColor);
			return;
		}
		if (base.currentPreStatus == PreStatus.Hover)
		{
			ApplyStateSprite(spriteHighlight, fallbackHighlightColor);
			return;
		}
		buttonNode.image.sprite = spriteNormal;
		buttonNode.image.color = fallbackNormalColor;
	}

	protected override void OnDefocus()
	{
		textNode.text.color = buttonTextColorNormal;
		buttonNode.rectTransform.pivot = new Vector2(0f, 1f);
		buttonNode.rectTransform.localScale = new Vector2(1f, 1f);
		ApplyStateSprite(spriteNormal, fallbackNormalColor);
		wasHover = false;
	}

	protected override void OnHover()
	{
		textNode.text.color = buttonTextColorHighlight;
		buttonNode.rectTransform.pivot = new Vector2(0f, 1f);
		buttonNode.rectTransform.localScale = new Vector2(1f, 1f);
		ApplyStateSprite(spriteHighlight, fallbackHighlightColor);
		wasHover = true;
	}

	protected override void OnPress()
	{
		textNode.text.color = buttonTextColorClicked;
		buttonNode.rectTransform.pivot = new Vector2(0f, 1f);
		buttonNode.rectTransform.localScale = new Vector2(1f, 1f);
		ApplyStateSprite(spriteClicked, fallbackClickedColor);
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
		ApplyStateSprite(spriteDisabled, fallbackDisabledColor);
		textNode.text.color = buttonTextColorDisabled;
	}

	protected override void OnEnable()
	{
		buttonNode.image.sprite = spriteNormal;
		buttonNode.image.color = fallbackNormalColor;
		textNode.text.color = buttonTextColorNormal;
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

	public AdvButtonElement SetSprite(string atlasPath, string spriteName)
	{
		spriteNormal = RenderHandler.LoadFromAtlas(atlasPath, spriteName);
		spriteHighlight = RenderHandler.LoadFromAtlas(atlasPath, spriteName + "_hl");
		spriteClicked = RenderHandler.LoadFromAtlas(atlasPath, spriteName + "_c");
		spriteDisabled = RenderHandler.LoadFromAtlas(atlasPath, spriteName + "_d");
		ApplyCurrentState();
		return this;
	}

	public AdvButtonElement SetDescription(string description)
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
