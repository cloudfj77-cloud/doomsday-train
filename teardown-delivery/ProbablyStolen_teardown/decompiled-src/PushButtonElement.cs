using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class PushButtonElement : PixelElement
{
	public readonly bool canMove;

	private readonly TreeNodeRender textNode;

	private readonly TreeNodeRender buttonFrameNode;

	private readonly TreeNodeRender buttonNode;

	private readonly TreeNodeRender buttonNode2;

	private int forceWidth;

	private int forceHeight;

	private bool isToggle;

	private bool isDown;

	private bool isLocked;

	public Action onDown;

	public Action onUp;

	private IndicatorElement.IndicatorColor currentColor;

	private IndicatorElement.IndicatorColor currentTextColor;

	private bool isLight;

	private Color32 buttonTextColor = RenderHandler.GetColorFromPalette(RenderHandler.ColorPalette.LightGray);

	private bool isButtonPressed;

	public bool canResize => false;

	public int widthPixels { get; private set; }

	public int heightPixels { get; private set; }

	public TreeNodeRender handler { get; private set; }

	public RectTransform rectTransform => handler.rectTransform;

	public TreeNodeRender inner => handler;

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

	public PushButtonElement()
	{
		handler = TreeNodeRender.Instantiate(this, "PushButton");
		forceWidth = 32;
		forceHeight = 32;
		buttonNode = TreeNodeRender.Instantiate().SetParent(handler);
		buttonNode2 = TreeNodeRender.Instantiate().SetParent(handler);
		textNode = TreeNodeRender.InstantiateText().SetParent(handler);
		buttonFrameNode = TreeNodeRender.Instantiate().SetParent(handler);
		buttonNode2.image.raycastTarget = false;
		buttonNode.InitEventHandler();
		textNode.text.text = text ?? "";
		textNode.text.color = buttonTextColor;
		textNode.text.fontStyle = FontStyles.Bold;
		textNode.text.SetNativeSize();
		textNode.text.enabled = hasText;
		buttonFrameNode.image.sprite = RenderHandler.LoadFromAtlas("Window/elements_inner", "push_button_frame");
		buttonFrameNode.image.enabled = true;
		SetColor(IndicatorElement.IndicatorColor.None);
		buttonNode.image.SetNativeSize();
		buttonNode2.image.SetNativeSize();
		buttonNode.image.enabled = true;
		buttonNode2.image.enabled = true;
		buttonNode.eventHandler.mouseDownFunc.Add(delegate
		{
			Press();
			return true;
		});
		buttonNode.eventHandler.mouseEnterFunc.Add(delegate
		{
			Release();
			return true;
		});
		buttonNode.eventHandler.mouseExitFunc.Add(delegate
		{
			Release();
			return true;
		});
		buttonNode.eventHandler.mouseUpFunc.Add(delegate
		{
			Release();
			return true;
		});
		Validate();
	}

	public IndicatorElement.IndicatorColor GetColor()
	{
		return currentColor;
	}

	public IndicatorElement.IndicatorColor GetTextColor()
	{
		return currentTextColor;
	}

	public PushButtonElement SetColor(IndicatorElement.IndicatorColor color)
	{
		if (color != currentColor)
		{
			currentColor = color;
			ValidateSprite();
		}
		return this;
	}

	public PushButtonElement SetTextColor(IndicatorElement.IndicatorColor color)
	{
		if (color != currentTextColor)
		{
			currentTextColor = color;
			ValidateSprite();
		}
		return this;
	}

	public bool IsLight()
	{
		return isLight;
	}

	public bool IsDark()
	{
		return !isLight;
	}

	public PushButtonElement Light()
	{
		if (!isLight)
		{
			isLight = true;
			ValidateSprite();
		}
		return this;
	}

	public PushButtonElement Dark()
	{
		if (isLight)
		{
			isLight = false;
			ValidateSprite();
		}
		return this;
	}

	public PushButtonElement ToggleLight()
	{
		if (isLight)
		{
			Dark();
		}
		else
		{
			Light();
		}
		return this;
	}

	public bool IsDown()
	{
		return isDown;
	}

	public PushButtonElement Activate()
	{
		Press(checkLock: false);
		Release(checkLock: false);
		return this;
	}

	public PushButtonElement Reset()
	{
		if (isDown && isToggle)
		{
			Press(checkLock: false);
			Release(checkLock: false);
		}
		else
		{
			Release(checkLock: false);
		}
		return this;
	}

	private void Down()
	{
		if (!isDown)
		{
			isDown = true;
			buttonNode.rectTransform.pivot = new Vector2(1f, 0f);
			buttonNode.rectTransform.localScale = new Vector2(-1f, -1f);
			buttonNode2.rectTransform.pivot = buttonNode.rectTransform.pivot;
			buttonNode2.rectTransform.localScale = buttonNode.rectTransform.localScale;
			onDown?.Invoke();
			AudioHelper.UIAudioClicked();
		}
	}

	private void Up()
	{
		if (isDown)
		{
			isDown = false;
			buttonNode.rectTransform.pivot = new Vector2(0f, 1f);
			buttonNode.rectTransform.localScale = new Vector2(1f, 1f);
			buttonNode2.rectTransform.pivot = buttonNode.rectTransform.pivot;
			buttonNode2.rectTransform.localScale = buttonNode.rectTransform.localScale;
			onUp?.Invoke();
			AudioHelper.UIAudioClicked();
		}
	}

	private void ValidateSprite()
	{
		buttonNode.image.sprite = RenderHandler.LoadFromAtlas("Window/elements_inner", IndicatorElement.ColorString(currentColor, isLight));
		buttonNode2.image.sprite = buttonNode.image.sprite;
		textNode.text.color = RenderHandler.GetColorFromPalette(IndicatorElement.ColorText(currentTextColor, isLight));
	}

	public bool IsToggle()
	{
		return isToggle;
	}

	public PushButtonElement SetToggle(bool isToggle)
	{
		this.isToggle = isToggle;
		if (!isToggle && isDown)
		{
			Up();
		}
		return this;
	}

	public bool IsLocked()
	{
		return isLocked;
	}

	public PushButtonElement Lock()
	{
		isLocked = true;
		return this;
	}

	public PushButtonElement Unlock()
	{
		isLocked = false;
		return this;
	}

	public PushButtonElement SetText(string text)
	{
		this.text = text;
		Validate();
		return this;
	}

	public PushButtonElement SetText(string text, int width = -1, int height = -1)
	{
		this.text = text;
		forceWidth = width;
		forceHeight = height;
		Validate();
		return this;
	}

	public PushButtonElement SetSize(int width = -1, int height = -1)
	{
		forceWidth = width;
		forceHeight = height;
		Validate();
		return this;
	}

	private void Press(bool checkLock = true)
	{
		if (isButtonPressed)
		{
			return;
		}
		isButtonPressed = true;
		if (!(isLocked && checkLock))
		{
			if (!isToggle)
			{
				Down();
			}
			else if (isDown)
			{
				Up();
			}
			else
			{
				Down();
			}
		}
	}

	private void Release(bool checkLock = true)
	{
		if (isButtonPressed)
		{
			isButtonPressed = false;
			if (!(isLocked && checkLock) && !isToggle)
			{
				Up();
			}
		}
	}

	public void Validate()
	{
		ValidateSprite();
		int num = (text?.Length ?? 0) * UISettings.current.fontWidth;
		int fontHeight = UISettings.current.fontHeight;
		int num2 = 0;
		int num3 = 0;
		if (forceWidth > 0)
		{
			widthPixels = forceWidth;
			num2 = (widthPixels - num) / 2 - 4;
		}
		else
		{
			widthPixels = (text?.Length ?? 0) * UISettings.current.fontWidth + 7 + 4;
		}
		if (forceHeight > 0)
		{
			heightPixels = forceHeight;
			num3 = (heightPixels - fontHeight) / 2;
		}
		else
		{
			heightPixels = UISettings.current.fontHeight + 3;
		}
		num = Mathf.Min(16, widthPixels);
		fontHeight = Mathf.Min(fontHeight, heightPixels);
		textNode.rectTransform.sizeDelta = new Vector2(num, fontHeight);
		buttonNode.rectTransform.sizeDelta = new Vector2(widthPixels - 8, heightPixels - 8);
		buttonNode2.rectTransform.sizeDelta = new Vector2(widthPixels - 10, heightPixels - 10);
		buttonFrameNode.rectTransform.sizeDelta = new Vector2(widthPixels, heightPixels);
		buttonNode.rectTransform.anchoredPosition = new Vector2(4f, -4f);
		buttonNode2.rectTransform.anchoredPosition = new Vector2(5f, -5f);
		textNode.rectTransform.anchoredPosition = new Vector2(num2 + 1, -num3 - 1);
		textNode.text.verticalAlignment = VerticalAlignmentOptions.Middle;
		textNode.text.horizontalAlignment = HorizontalAlignmentOptions.Center;
	}

	public void ResizePixels(int width, int height)
	{
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
}
