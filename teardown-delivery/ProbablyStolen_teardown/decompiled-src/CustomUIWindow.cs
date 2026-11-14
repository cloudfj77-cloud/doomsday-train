using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class CustomUIWindow
{
	public string id;

	public string layerId;

	public GameObject root;

	public CustomUIWindowFrame frame;

	public bool closeOnEscape;

	public bool useMetaPrefabs;

	public bool useMachineryStyle;

	public Action onClose;

	private readonly Dictionary<string, CustomUIElement> elements = new Dictionary<string, CustomUIElement>();

	public bool IsAlive => root != null;

	public bool IsVisible
	{
		get
		{
			if (root != null)
			{
				return root.activeSelf;
			}
			return false;
		}
	}

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

	public RectTransform ContentRoot
	{
		get
		{
			if (!(frame != null) || !(frame.contentRoot != null))
			{
				return Rect;
			}
			return frame.contentRoot;
		}
	}

	public void RegisterElement(CustomUIElement element)
	{
		if (element != null && !string.IsNullOrEmpty(element.id))
		{
			if (elements.ContainsKey(element.id))
			{
				Debug.LogWarning("[CustomUI] Window '" + id + "' already has an element '" + element.id + "', overwriting");
			}
			elements[element.id] = element;
		}
	}

	public CustomUIElement Get(string elementId)
	{
		if (elements.TryGetValue(elementId, out var value))
		{
			return value;
		}
		Debug.LogWarning("[CustomUI] Window '" + id + "' has no element '" + elementId + "'");
		return null;
	}

	public void Show()
	{
		if (!(root == null))
		{
			root.SetActive(value: true);
			BringToFront();
		}
	}

	public void Hide()
	{
		if (root != null)
		{
			root.SetActive(value: false);
		}
	}

	public void Toggle()
	{
		if (IsVisible)
		{
			Hide();
		}
		else
		{
			Show();
		}
	}

	public void Close()
	{
		if (CustomUIManager.Instance != null)
		{
			CustomUIManager.Instance.NotifyWindowClosed(this);
		}
		try
		{
			onClose?.Invoke();
		}
		catch (Exception arg)
		{
			Debug.LogError($"[CustomUI] onClose of window '{id}' threw: {arg}");
		}
		if (root != null)
		{
			UnityEngine.Object.Destroy(root);
			root = null;
		}
	}

	public void SetTitle(string title)
	{
		if (frame != null && frame.titleText != null)
		{
			LocalizeStringEvent component = frame.titleText.GetComponent<LocalizeStringEvent>();
			if (component != null)
			{
				component.enabled = false;
			}
			frame.titleText.text = title;
		}
	}

	public void SetSize(float width, float height)
	{
		if (Rect != null)
		{
			Rect.sizeDelta = new Vector2(width, height);
		}
	}

	public void SetPosition(Vector2 anchoredPosition)
	{
		if (Rect != null)
		{
			Rect.anchoredPosition = anchoredPosition;
		}
	}

	public void Center()
	{
		if (Rect != null)
		{
			Rect.anchoredPosition = Vector2.zero;
		}
	}

	public void BringToFront()
	{
		if (root != null)
		{
			root.transform.SetAsLastSibling();
		}
	}
}
