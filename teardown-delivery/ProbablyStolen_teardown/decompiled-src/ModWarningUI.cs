using UnityEngine;
using UnityEngine.UI;

public static class ModWarningUI
{
	public const int TEXT_VERSION = 2;

	public const string ES3_KEY = "modUiWarningAcceptedVersion";

	private const float PANEL_WIDTH = 760f;

	private const float BUTTON_WIDTH = 260f;

	private static readonly Color BACKDROP_COLOR = new Color(0f, 0f, 0f, 0.7f);

	private static GameObject backdrop;

	private static bool dontShowAgain;

	private static bool previewing;

	public static bool IsAccepted
	{
		get
		{
			if (ES3.KeyExists("modUiWarningAcceptedVersion"))
			{
				return ES3.Load<int>("modUiWarningAcceptedVersion") >= 2;
			}
			return false;
		}
	}

	public static bool IsOpen => backdrop != null;

	public static void ShowIfNeeded(Transform canvasRoot)
	{
		if (!IsOpen && !IsAccepted)
		{
			Show(canvasRoot, preview: false);
		}
	}

	public static bool ShowPreview(Transform canvasRoot)
	{
		Close();
		return Show(canvasRoot, preview: true);
	}

	private static bool Show(Transform canvasRoot, bool preview)
	{
		backdrop = CreateDialog(canvasRoot, "mod_warning", 760f, out var builder);
		if (backdrop == null)
		{
			return false;
		}
		dontShowAgain = false;
		previewing = preview;
		builder.BeginPanel(760f, -1f, TextAnchor.UpperLeft, 24f, 12f, "frame");
		builder.AddHeader(LocHelper.GetLocalizedUIOrDefault("ui_mods_warning_title", "WARNING"), "title");
		builder.AddWarning(LocHelper.GetLocalizedUI("ui_mods_dll_warning"), "body");
		builder.AddSpace(8f);
		builder.AddToggle(LocHelper.GetLocalizedUIOrDefault("ui_mods_warning_dont_show", "Don't show this again"), initial: false, delegate(bool v)
		{
			dontShowAgain = v;
		}, "dont_show");
		builder.BeginRow(0f, TextAnchor.MiddleCenter);
		builder.AddFlexibleSpace();
		builder.AddButton(LocHelper.GetLocalizedUIOrDefault("ui_mods_warning_confirm", "I understand"), Confirm, "confirm");
		builder.Last.SetFixedWidth(260f);
		builder.AddFlexibleSpace();
		builder.End();
		builder.End();
		return true;
	}

	internal static GameObject CreateDialog(Transform canvasRoot, string id, float width, out CustomUIBuilder builder)
	{
		builder = null;
		CustomUIManager instance = CustomUIManager.Instance;
		if (instance == null || canvasRoot == null)
		{
			Debug.LogWarning("[ModWarningUI] Cannot show the " + id + " dialog: no CustomUIManager or canvas");
			return null;
		}
		GameObject gameObject = new GameObject(id + "_backdrop", typeof(RectTransform), typeof(Image));
		RectTransform rectTransform = (RectTransform)gameObject.transform;
		rectTransform.SetParent(canvasRoot, worldPositionStays: false);
		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.one;
		rectTransform.offsetMin = Vector2.zero;
		rectTransform.offsetMax = Vector2.zero;
		rectTransform.SetAsLastSibling();
		Image component = gameObject.GetComponent<Image>();
		component.color = BACKDROP_COLOR;
		component.raycastTarget = true;
		GameObject gameObject2 = new GameObject("content", typeof(RectTransform));
		RectTransform rectTransform2 = (RectTransform)gameObject2.transform;
		rectTransform2.SetParent(rectTransform, worldPositionStays: false);
		rectTransform2.anchorMin = new Vector2(0.5f, 0.5f);
		rectTransform2.anchorMax = new Vector2(0.5f, 0.5f);
		rectTransform2.pivot = new Vector2(0.5f, 0.5f);
		rectTransform2.anchoredPosition = Vector2.zero;
		rectTransform2.sizeDelta = new Vector2(width, 0f);
		gameObject2.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		builder = instance.BuildInto(rectTransform2, useMetaPrefabs: true, id);
		return gameObject;
	}

	private static void Confirm()
	{
		if (dontShowAgain && !previewing)
		{
			ES3.Save("modUiWarningAcceptedVersion", 2);
		}
		Close();
	}

	public static void Close()
	{
		if (backdrop != null)
		{
			Object.Destroy(backdrop);
		}
		backdrop = null;
	}
}
