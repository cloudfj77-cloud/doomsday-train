using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomUIElement
{
	public string id;

	public GameObject root;

	public TextMeshProUGUI text;

	public Image image;

	public Button button;

	public Toggle toggle;

	public Slider slider;

	public TMP_InputField input;

	public TMP_Dropdown dropdown;

	public CustomUIProgressBar progressBar;

	private const float DISABLED_ALPHA = 0.45f;

	public bool IsAlive => root != null;

	public RectTransform Rect
	{
		get
		{
			if (!(root != null))
			{
				return null;
			}
			return root.transform as RectTransform;
		}
	}

	public static CustomUIElement Bind(GameObject go, string id)
	{
		CustomUIElement customUIElement = new CustomUIElement
		{
			id = id,
			root = go
		};
		if (go == null)
		{
			return customUIElement;
		}
		customUIElement.text = go.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
		customUIElement.image = go.GetComponentInChildren<Image>(includeInactive: true);
		customUIElement.button = go.GetComponentInChildren<Button>(includeInactive: true);
		customUIElement.toggle = go.GetComponentInChildren<Toggle>(includeInactive: true);
		customUIElement.slider = go.GetComponentInChildren<Slider>(includeInactive: true);
		customUIElement.input = go.GetComponentInChildren<TMP_InputField>(includeInactive: true);
		if (customUIElement.input != null && customUIElement.input.GetComponent<TMPInputFieldLocalizer>() == null)
		{
			customUIElement.input.gameObject.AddComponent<TMPInputFieldLocalizer>().fontRole = FontLoader.FontRole.Noto_Sans;
		}
		customUIElement.dropdown = go.GetComponentInChildren<TMP_Dropdown>(includeInactive: true);
		customUIElement.progressBar = go.GetComponentInChildren<CustomUIProgressBar>(includeInactive: true);
		return customUIElement;
	}

	private bool Warn(bool missing, string what)
	{
		if (missing)
		{
			Debug.LogWarning("[CustomUI] Element '" + id + "' has no " + what + ", call ignored");
		}
		return missing;
	}

	public CustomUIElement SetVisible(bool visible)
	{
		if (root != null)
		{
			root.SetActive(visible);
		}
		return this;
	}

	public CustomUIElement SetInteractable(bool interactable)
	{
		if (button != null)
		{
			button.interactable = interactable;
		}
		if (toggle != null)
		{
			toggle.interactable = interactable;
		}
		if (slider != null)
		{
			slider.interactable = interactable;
		}
		if (input != null)
		{
			input.interactable = interactable;
		}
		if (dropdown != null)
		{
			dropdown.interactable = interactable;
		}
		if (root != null && button != null && button.transition == Selectable.Transition.SpriteSwap && button.spriteState.disabledSprite == null)
		{
			CanvasGroup canvasGroup = root.GetComponent<CanvasGroup>();
			if (canvasGroup == null)
			{
				canvasGroup = root.AddComponent<CanvasGroup>();
			}
			canvasGroup.alpha = (interactable ? 1f : 0.45f);
			canvasGroup.blocksRaycasts = interactable;
		}
		return this;
	}

	public CustomUIElement SetText(string value)
	{
		if (Warn(text == null, "text component"))
		{
			return this;
		}
		text.text = value;
		return this;
	}

	public string GetText()
	{
		if (!(text != null))
		{
			return "";
		}
		return text.text;
	}

	public CustomUIElement SetTextColor(Color color)
	{
		if (Warn(text == null, "text component"))
		{
			return this;
		}
		text.color = color;
		return this;
	}

	public CustomUIElement SetSprite(Sprite sprite)
	{
		if (Warn(image == null, "image component"))
		{
			return this;
		}
		image.sprite = sprite;
		return this;
	}

	public CustomUIElement SetSprite(string spriteKey)
	{
		if (CustomUIManager.Instance == null)
		{
			return this;
		}
		Sprite sprite = CustomUIManager.Instance.GetSprite(spriteKey);
		if (!(sprite != null))
		{
			return this;
		}
		return SetSprite(sprite);
	}

	public CustomUIElement SetImageColor(Color color)
	{
		if (Warn(image == null, "image component"))
		{
			return this;
		}
		image.color = color;
		return this;
	}

	public CustomUIElement OnClick(Action callback)
	{
		if (Warn(button == null, "button"))
		{
			return this;
		}
		if (callback != null)
		{
			button.onClick.AddListener(delegate
			{
				callback();
			});
		}
		return this;
	}

	public bool GetBool()
	{
		if (toggle != null)
		{
			return toggle.isOn;
		}
		return false;
	}

	public CustomUIElement SetBool(bool value, bool notify = false)
	{
		if (Warn(toggle == null, "toggle"))
		{
			return this;
		}
		if (notify)
		{
			toggle.isOn = value;
		}
		else
		{
			toggle.SetIsOnWithoutNotify(value);
		}
		return this;
	}

	public CustomUIElement OnToggle(Action<bool> callback)
	{
		if (Warn(toggle == null, "toggle"))
		{
			return this;
		}
		if (callback != null)
		{
			toggle.onValueChanged.AddListener(delegate(bool v)
			{
				callback(v);
			});
		}
		return this;
	}

	public float GetFloat()
	{
		if (!(slider != null))
		{
			return 0f;
		}
		return slider.value;
	}

	public CustomUIElement SetFloat(float value, bool notify = false)
	{
		if (Warn(slider == null, "slider"))
		{
			return this;
		}
		if (notify)
		{
			slider.value = value;
		}
		else
		{
			slider.SetValueWithoutNotify(value);
		}
		return this;
	}

	public CustomUIElement OnSlider(Action<float> callback)
	{
		if (Warn(slider == null, "slider"))
		{
			return this;
		}
		if (callback != null)
		{
			slider.onValueChanged.AddListener(delegate(float v)
			{
				callback(v);
			});
		}
		return this;
	}

	public string GetString()
	{
		if (!(input != null))
		{
			return "";
		}
		return input.text;
	}

	public CustomUIElement SetString(string value, bool notify = false)
	{
		if (Warn(input == null, "input field"))
		{
			return this;
		}
		if (notify)
		{
			input.text = value;
		}
		else
		{
			input.SetTextWithoutNotify(value);
		}
		return this;
	}

	public CustomUIElement OnInput(Action<string> callback)
	{
		if (Warn(input == null, "input field"))
		{
			return this;
		}
		if (callback != null)
		{
			input.onEndEdit.AddListener(delegate(string v)
			{
				callback(v);
			});
		}
		return this;
	}

	public CustomUIElement OnInputChanged(Action<string> callback)
	{
		if (Warn(input == null, "input field"))
		{
			return this;
		}
		if (callback != null)
		{
			input.onValueChanged.AddListener(delegate(string v)
			{
				callback(v);
			});
		}
		return this;
	}

	public int GetIndex()
	{
		if (!(dropdown != null))
		{
			return -1;
		}
		return dropdown.value;
	}

	public CustomUIElement SetIndex(int index, bool notify = false)
	{
		if (Warn(dropdown == null, "dropdown"))
		{
			return this;
		}
		if (notify)
		{
			dropdown.value = index;
		}
		else
		{
			dropdown.SetValueWithoutNotify(index);
		}
		return this;
	}

	public CustomUIElement SetOptions(IList<string> options)
	{
		if (Warn(dropdown == null, "dropdown"))
		{
			return this;
		}
		dropdown.ClearOptions();
		dropdown.AddOptions(new List<string>(options));
		return this;
	}

	public CustomUIElement OnDropdown(Action<int> callback)
	{
		if (Warn(dropdown == null, "dropdown"))
		{
			return this;
		}
		if (callback != null)
		{
			dropdown.onValueChanged.AddListener(delegate(int v)
			{
				callback(v);
			});
		}
		return this;
	}

	public CustomUIElement SetProgress(float normalized, string labelText = null)
	{
		if (Warn(progressBar == null, "progress bar"))
		{
			return this;
		}
		progressBar.SetProgress(normalized);
		if (labelText != null)
		{
			progressBar.SetLabel(labelText);
		}
		return this;
	}

	public CustomUIElement SetTooltip(string tooltipText)
	{
		if (root == null)
		{
			return this;
		}
		CustomUITooltipTrigger customUITooltipTrigger = root.GetComponent<CustomUITooltipTrigger>();
		if (customUITooltipTrigger == null)
		{
			customUITooltipTrigger = root.AddComponent<CustomUITooltipTrigger>();
		}
		customUITooltipTrigger.tooltipText = tooltipText;
		if (text != null)
		{
			text.raycastTarget = true;
		}
		if (image != null)
		{
			image.raycastTarget = true;
		}
		return this;
	}

	public CustomUIElement SetPreferredSize(float width, float height)
	{
		if (root == null)
		{
			return this;
		}
		LayoutElement layoutElement = root.GetComponent<LayoutElement>();
		if (layoutElement == null)
		{
			layoutElement = root.AddComponent<LayoutElement>();
		}
		if (width > 0f)
		{
			layoutElement.preferredWidth = width;
		}
		if (height > 0f)
		{
			layoutElement.preferredHeight = height;
		}
		return this;
	}

	public CustomUIElement SetFixedWidth(float width)
	{
		if (root == null)
		{
			return this;
		}
		LayoutElement layoutElement = root.GetComponent<LayoutElement>();
		if (layoutElement == null)
		{
			layoutElement = root.AddComponent<LayoutElement>();
		}
		layoutElement.minWidth = width;
		layoutElement.preferredWidth = width;
		layoutElement.flexibleWidth = 0f;
		return this;
	}

	public CustomUIElement SetFlexible(float width, float height)
	{
		if (root == null)
		{
			return this;
		}
		LayoutElement layoutElement = root.GetComponent<LayoutElement>();
		if (layoutElement == null)
		{
			layoutElement = root.AddComponent<LayoutElement>();
		}
		if (width >= 0f)
		{
			layoutElement.flexibleWidth = width;
		}
		if (height >= 0f)
		{
			layoutElement.flexibleHeight = height;
		}
		return this;
	}
}
