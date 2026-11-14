using System;
using UnityEngine;
using UnityEngine.UI;

public class MachineryButton : SelectableElement, MachineryWidget, PixelElement
{
	private readonly TreeNodeRender plate;

	private readonly TreeNodeRender iconNode;

	private readonly TreeNodeRender textNode;

	private int forceWidth = -1;

	private int forceHeight = -1;

	private Action callback;

	private bool wasHover;

	public string onClickAudio = "interface_click";

	private Sprite faceNormal;

	private Sprite faceHover;

	private Sprite facePressed;

	private Sprite faceDisabled;

	public string text { get; private set; }

	public Sprite icon { get; private set; }

	public bool hasText => !string.IsNullOrEmpty(text);

	public bool fillWidth { get; set; }

	public string description { get; set; }

	private bool hasFaceSet => faceNormal != null;

	public MachineryButton(string text = null, Action callback = null, int width = -1, int height = -1)
		: base("MachineryButton")
	{
		this.text = text;
		this.callback = callback;
		forceWidth = width;
		forceHeight = height;
		fillWidth = text != null;
		plate = TreeNodeRender.Instantiate().SetParent(base.handler);
		iconNode = TreeNodeRender.Instantiate().SetParent(base.handler);
		textNode = TreeNodeRender.InstantiateText().SetParent(base.handler);
		plate.InitEventHandler();
		plate.name = "Node (MachineryButton)";
		MachineryStyle.SetSliced(plate, MachineryStyle.ButtonNormal);
		iconNode.image.enabled = false;
		iconNode.image.raycastTarget = false;
		MachineryStyle.InitText(textNode, 7, MachineryStyle.BUTTON_TEXT);
		textNode.text.text = text ?? "";
		textNode.text.enabled = hasText;
		Validate();
	}

	public MachineryButton(Sprite icon, Action callback = null, int size = 16)
		: this(null, callback, size, size)
	{
		SetIcon(icon);
		fillWidth = false;
	}

	public MachineryButton(Sprite normal, Sprite hover, Sprite pressed, Sprite disabled, Action callback = null)
		: this(null, callback, -1, -1)
	{
		fillWidth = false;
		SetFaceSprites(normal, hover, pressed, disabled);
	}

	public static MachineryButton FromAtlas(string atlasPath, string name, Action callback = null)
	{
		return new MachineryButton(RenderHandler.LoadFromAtlas(atlasPath, name), RenderHandler.TryLoadFromAtlas(atlasPath, name + "_hl"), RenderHandler.TryLoadFromAtlas(atlasPath, name + "_c"), RenderHandler.TryLoadFromAtlas(atlasPath, name + "_d"), callback);
	}

	public MachineryButton SetFaceSprites(Sprite normal, Sprite hover = null, Sprite pressed = null, Sprite disabled = null)
	{
		faceNormal = normal;
		faceHover = ((hover != null) ? hover : normal);
		facePressed = ((pressed != null) ? pressed : normal);
		faceDisabled = ((disabled != null) ? disabled : normal);
		plate.image.type = Image.Type.Simple;
		OnReset();
		return this;
	}

	public MachineryButton SetText(string text)
	{
		this.text = text;
		textNode.text.text = text ?? "";
		textNode.text.enabled = hasText && !base.isHidden;
		Validate();
		MachineryContainerUtils.NotifySizeChanged(this);
		return this;
	}

	public MachineryButton SetIcon(Sprite sprite)
	{
		icon = sprite;
		MachineryStyle.SetSimple(iconNode, sprite, Color.white);
		iconNode.image.enabled = sprite != null && !base.isHidden;
		Validate();
		return this;
	}

	public MachineryButton SetIcon(string atlasPath, string spriteName)
	{
		return SetIcon(RenderHandler.LoadFromAtlas(atlasPath, spriteName));
	}

	public MachineryButton SetCallback(Action callback)
	{
		this.callback = callback;
		return this;
	}

	public MachineryButton SetDescription(string description)
	{
		this.description = description;
		return this;
	}

	public MachineryButton SetFillWidth(bool fill)
	{
		fillWidth = fill;
		return this;
	}

	public MachineryButton SetSize(int width, int height = -1)
	{
		forceWidth = width;
		forceHeight = height;
		Validate();
		return this;
	}

	public override void Validate()
	{
		if (base.isHidden)
		{
			return;
		}
		if (hasFaceSet)
		{
			base.widthPixels = ((forceWidth > 0) ? forceWidth : ((int)faceNormal.rect.width));
			base.heightPixels = ((forceHeight > 0) ? forceHeight : ((int)faceNormal.rect.height));
			MachineryStyle.Place(plate, 0, 0, base.widthPixels, base.heightPixels);
			return;
		}
		int num = (hasText ? MachineryStyle.TextWidth(textNode) : 0);
		int num2 = ((icon != null) ? ((int)icon.rect.width) : 0);
		int num3 = ((icon != null) ? ((int)icon.rect.height) : 0);
		int num4 = num + num2 + ((hasText && icon != null) ? 3 : 0);
		int value = Mathf.Max(16, num4 + 12);
		base.widthPixels = ((forceWidth > 0) ? forceWidth : MachineryStyle.Even(value));
		base.heightPixels = ((forceHeight > 0) ? forceHeight : Mathf.Max(16, num3 + 4));
		MachineryStyle.Place(plate, 0, 0, base.widthPixels, base.heightPixels);
		int num5 = ((base.currentPreStatus == PreStatus.Press && !base.isDisabled) ? 1 : 0);
		int num6 = (base.widthPixels - num4) / 2;
		if (icon != null)
		{
			MachineryStyle.Place(iconNode, num6, (base.heightPixels - num3) / 2 + num5, num2, num3);
			num6 += num2 + (hasText ? 3 : 0);
		}
		if (hasText)
		{
			MachineryStyle.Place(textNode, num6, num5, num, base.heightPixels);
		}
	}

	public override void ResizePixels(int width, int height)
	{
		forceWidth = width;
		Validate();
	}

	private void ApplyState(Sprite sprite, Color tint, Color textColor)
	{
		if (hasFaceSet)
		{
			bool flag = sprite == MachineryStyle.ButtonPressed;
			bool flag2 = sprite == MachineryStyle.ButtonHover;
			sprite = (base.isDisabled ? faceDisabled : (flag ? facePressed : (flag2 ? faceHover : faceNormal)));
			tint = ((base.isDisabled && faceDisabled == faceNormal) ? MachineryStyle.BUTTON_DISABLED_TINT : Color.white);
		}
		plate.image.sprite = sprite;
		plate.image.color = tint;
		textNode.text.color = textColor;
		iconNode.image.color = (base.isDisabled ? MachineryStyle.BUTTON_DISABLED_TINT : Color.white);
		Validate();
	}

	protected override void OnDefocus()
	{
		ApplyState(MachineryStyle.ButtonNormal, Color.white, MachineryStyle.BUTTON_TEXT);
		wasHover = false;
	}

	protected override void OnHover()
	{
		ApplyState(MachineryStyle.ButtonHover, Color.white, MachineryStyle.BUTTON_TEXT);
		wasHover = true;
	}

	protected override void OnPress()
	{
		ApplyState(MachineryStyle.ButtonPressed, Color.white, MachineryStyle.BUTTON_TEXT);
		if (wasHover && !string.IsNullOrEmpty(onClickAudio))
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
		if (base.isDisabled)
		{
			ApplyState(MachineryStyle.ButtonNormal, MachineryStyle.BUTTON_DISABLED_TINT, MachineryStyle.BUTTON_TEXT_DISABLED);
		}
		else
		{
			OnDefocus();
		}
	}

	protected override void OnDisable()
	{
		ApplyState(MachineryStyle.ButtonNormal, MachineryStyle.BUTTON_DISABLED_TINT, MachineryStyle.BUTTON_TEXT_DISABLED);
	}

	protected override void OnEnable()
	{
		OnDefocus();
	}

	protected override void OnHide()
	{
		plate.image.enabled = false;
		iconNode.image.enabled = false;
		textNode.text.enabled = false;
		base.widthPixels = 0;
		base.heightPixels = 0;
		MachineryContainerUtils.NotifySizeChanged(this);
	}

	protected override void OnShow()
	{
		plate.image.enabled = true;
		iconNode.image.enabled = icon != null;
		textNode.text.enabled = hasText;
		Validate();
		MachineryContainerUtils.NotifySizeChanged(this);
	}

	public override bool HasTooltip()
	{
		return !string.IsNullOrEmpty(description);
	}

	public override RichText GetTooltip()
	{
		return new RichTextBuilder().Add(description).Build();
	}
}
