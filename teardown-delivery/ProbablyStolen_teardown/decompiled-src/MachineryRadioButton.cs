using System;
using TMPro;
using UnityEngine;

public class MachineryRadioButton : SelectableElement, MachineryWidget, PixelElement
{
	public Action<bool> onChanged;

	private readonly TreeNodeRender plate;

	private readonly TreeNodeRender textNode;

	private bool hovering;

	public string onClickAudio = "interface_click";

	private Color? textColor;

	public bool isOn { get; private set; }

	public string text { get; private set; }

	public bool fillWidth => false;

	public MachineryRadioGroup group { get; internal set; }

	public string description { get; set; }

	public MachineryRadioButton(string text = null, bool on = false, Action<bool> onChanged = null)
		: base("MachineryRadioButton")
	{
		this.text = text;
		isOn = on;
		this.onChanged = onChanged;
		plate = TreeNodeRender.Instantiate().SetParent(base.handler);
		textNode = TreeNodeRender.InstantiateText().SetParent(base.handler);
		plate.InitEventHandler();
		plate.name = "Node (MachineryRadioButton)";
		MachineryStyle.SetSimple(plate, MachineryStyle.RadioOff, Color.white);
		MachineryStyle.InitText(textNode, 6, MachineryStyle.BODY_TEXT, HorizontalAlignmentOptions.Left, uppercase: true);
		textNode.text.text = text ?? "";
		textNode.text.enabled = !string.IsNullOrEmpty(text);
		Refresh();
		Validate();
	}

	public MachineryRadioButton SetText(string text)
	{
		this.text = text;
		textNode.text.text = text ?? "";
		textNode.text.enabled = !string.IsNullOrEmpty(text) && !base.isHidden;
		Validate();
		return this;
	}

	public MachineryRadioButton SetDescription(string description)
	{
		this.description = description;
		return this;
	}

	public MachineryRadioButton SetTextColor(Color? color)
	{
		textColor = color;
		Refresh();
		return this;
	}

	public MachineryRadioButton SetOn(bool on, bool notify = false)
	{
		if (isOn == on)
		{
			return this;
		}
		isOn = on;
		Refresh();
		if (notify)
		{
			onChanged?.Invoke(on);
		}
		if (on)
		{
			group?.NotifyOn(this, notify);
		}
		return this;
	}

	private void Refresh()
	{
		Sprite sprite = ((base.currentPreStatus == PreStatus.Press && !base.isDisabled) ? MachineryStyle.RadioPressed : ((!isOn) ? (hovering ? MachineryStyle.RadioOffHover : MachineryStyle.RadioOff) : (hovering ? MachineryStyle.RadioOnHover : MachineryStyle.RadioOn)));
		plate.image.sprite = sprite;
		plate.image.color = (base.isDisabled ? MachineryStyle.BUTTON_DISABLED_TINT : Color.white);
		textNode.text.color = (base.isDisabled ? MachineryStyle.BUTTON_TEXT_DISABLED : (textColor ?? MachineryStyle.BODY_TEXT));
	}

	public override void Validate()
	{
		if (!base.isHidden)
		{
			int num = ((!string.IsNullOrEmpty(text)) ? MachineryStyle.TextWidth(textNode) : 0);
			base.widthPixels = 16 + ((num > 0) ? (6 + num) : 0);
			base.heightPixels = 16;
			MachineryStyle.Place(plate, 0, 0, 16, 16);
			MachineryStyle.Place(textNode, 22, 0, num, base.heightPixels);
		}
	}

	protected override void OnDefocus()
	{
		hovering = false;
		Refresh();
	}

	protected override void OnHover()
	{
		hovering = true;
		Refresh();
	}

	protected override void OnPress()
	{
		Refresh();
		if (hovering && !string.IsNullOrEmpty(onClickAudio))
		{
			AudioManager.Instance.Play(onClickAudio);
		}
	}

	protected override void OnSelect()
	{
		if (group != null && isOn)
		{
			Deselect();
			return;
		}
		SetOn(!isOn, notify: true);
		Deselect();
	}

	protected override void OnDeselect()
	{
	}

	protected override void OnReset()
	{
		Refresh();
	}

	protected override void OnDisable()
	{
		Refresh();
	}

	protected override void OnEnable()
	{
		Refresh();
	}

	protected override void OnHide()
	{
		plate.image.enabled = false;
		textNode.text.enabled = false;
		base.widthPixels = 0;
		base.heightPixels = 0;
	}

	protected override void OnShow()
	{
		plate.image.enabled = true;
		textNode.text.enabled = !string.IsNullOrEmpty(text);
		Validate();
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
