using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOptionUIManager : MonoBehaviour, GameOptionRegistry.IPendingValues
{
	private class OverrideState
	{
		public int keptIndex;

		public IList<string> options;

		public Action<int> apply;
	}

	public const string WINDOW_ID = "game_options";

	private const float DROPDOWN_WIDTH = 306f;

	private const float SLIDER_WIDTH = 306f;

	private const float SLIDER_VALUE_WIDTH = 56f;

	private const float SECTION_GAP = 12f;

	private const int SEPARATOR_PADDING = 2;

	private const float APPLY_BUTTON_WIDTH = 180f;

	private const float KEYBIND_BUTTON_WIDTH = 160f;

	private const string TOOLTIP_LINK_PREFIX = "gameopt:";

	[Tooltip("Legacy editor-built panel, kept disabled for reference. The window is built from GameOptionRegistry instead.")]
	public GameObject optionPanel;

	private CustomUIWindow window;

	private readonly Dictionary<string, KeyCode> pendingKeys = new Dictionary<string, KeyCode>();

	private readonly Dictionary<string, OverrideState> overriddenDropdowns = new Dictionary<string, OverrideState>();

	private static int consumedEscapeFrame = -1;

	private int lastCapturePollFrame = -1;

	private int restoreCloseOnEscapeFrame = -1;

	private bool closeOnEscapeBeforeCapture;

	public static GameOptionUIManager Instance { get; private set; }

	public static string KeyBindStatus { get; private set; } = "";


	public bool IsOpen
	{
		get
		{
			if (window != null && window.IsAlive)
			{
				return window.IsVisible;
			}
			return false;
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	private void Update()
	{
		PollKeyCapture();
		if (KeyBindHelper.GetKeyDown("open_options") && StoreUIManager.Instance != null && !StoreUIManager.Instance.IsTyping())
		{
			OpenUI();
		}
	}

	public void OpenUI()
	{
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[GameOption] No CustomUIManager in the scene, cannot open the options window");
			return;
		}
		if (EmporiumOption.Instance == null)
		{
			Debug.LogError("[GameOption] No EmporiumOption instance, cannot open the options window");
			return;
		}
		EmporiumOption.Instance.SyncDisplayStateFromScreen();
		GameOptionRegistry.EnsureVanilla();
		GameOptionRegistry.RefreshMonitorInfo();
		KeyBindHelper.StopCapture();
		pendingKeys.Clear();
		overriddenDropdowns.Clear();
		KeyBindStatus = "";
		bool flag = StoreUIManager.Instance != null;
		string layerId = (flag ? "meta" : "menu");
		CustomUIBuilder customUIBuilder = CustomUIManager.Instance.CreateWindow("game_options", null, layerId, CustomUIManager.Instance.optionUIPrefab).SetCloseOnEscape(!flag).OnClose(OnWindowClosed);
		customUIBuilder.BeginScrollFlexible();
		foreach (GameOptionRegistry.Section section in GameOptionRegistry.Sections)
		{
			BuildSection(customUIBuilder, section);
		}
		customUIBuilder.End();
		customUIBuilder.AddSeparator(null, 2);
		customUIBuilder.BeginRow(4f, TextAnchor.MiddleCenter).AddFlexibleSpace().AddButton(LocHelper.GetLocalizedMainMenu("option_apply"), OnApplySetting, "apply")
			.AddFlexibleSpace()
			.End();
		customUIBuilder.Last.SetFixedWidth(180f);
		window = customUIBuilder.Show();
		RefreshRules();
	}

	public void CloseUI()
	{
		if (window != null)
		{
			window.Close();
		}
		else if (optionPanel != null && optionPanel.activeSelf)
		{
			optionPanel.SetActive(value: false);
			OnWindowClosed();
		}
	}

	private void OnWindowClosed()
	{
		window = null;
		KeyBindHelper.StopCapture();
		pendingKeys.Clear();
		overriddenDropdowns.Clear();
		KeyBindStatus = "";
		restoreCloseOnEscapeFrame = -1;
		if (StoreUIManager.Instance != null)
		{
			StoreUIManager.Instance.SetTypewriterSpeed();
		}
	}

	private void BuildSection(CustomUIBuilder builder, GameOptionRegistry.Section section)
	{
		if (section.entries.Count == 0)
		{
			return;
		}
		builder.BeginRow(4f, TextAnchor.MiddleLeft);
		builder.AddHeader(Safe(section.label, section.id), "section_" + section.id);
		if (section.note != null)
		{
			builder.AddFlexibleSpace();
			builder.AddWarning(Safe(section.note, ""), NoteId(section));
		}
		builder.End();
		builder.AddSeparator(null, 2);
		foreach (GameOptionRegistry.Entry entry in section.entries)
		{
			BuildEntry(builder, entry);
		}
		builder.AddSpace(12f);
	}

	private void BuildEntry(CustomUIBuilder builder, GameOptionRegistry.Entry entry)
	{
		string text = Safe(entry.label, entry.id);
		if (entry.tooltip != null)
		{
			string text2 = "gameopt:" + entry.id;
			TMPIconHover.CustomLinks[text2] = entry.tooltip;
			text = text + " <link=\"" + text2 + "\"><sprite name=\"info\" tint=1></link>";
		}
		builder.BeginRow(4f, TextAnchor.MiddleLeft);
		builder.AddLabel(text, "label_" + entry.id);
		builder.AddFlexibleSpace();
		switch (entry.kind)
		{
		case GameOptionRegistry.Kind.Toggle:
			builder.AddToggle("", Safe(entry.getBool, fallback: false), delegate
			{
				RefreshRules();
			}, entry.id);
			break;
		case GameOptionRegistry.Kind.Slider:
		{
			float initial = Safe(entry.getFloat, entry.min);
			Func<float, string> valueText = entry.valueText ?? (entry.wholeNumbers ? ((Func<float, string>)((float v) => Mathf.RoundToInt(v).ToString())) : null);
			CustomUIElement valueLabel = null;
			if (valueText != null)
			{
				builder.AddLabel(Safe(() => valueText(initial), ""), "value_" + entry.id);
				valueLabel = builder.Last;
				valueLabel.SetFixedWidth(56f);
				if (valueLabel.text != null)
				{
					valueLabel.text.alignment = TextAlignmentOptions.Right;
					valueLabel.text.enableWordWrapping = false;
					valueLabel.text.overflowMode = TextOverflowModes.Overflow;
				}
			}
			builder.AddSlider(entry.min, entry.max, initial, delegate(float v)
			{
				if (valueLabel != null)
				{
					valueLabel.SetText(Safe(() => valueText(v), ""));
				}
				RefreshRules();
			}, entry.wholeNumbers, entry.id);
			builder.Last.SetFixedWidth(306f);
			break;
		}
		case GameOptionRegistry.Kind.Dropdown:
		{
			IList<string> options = Safe(entry.options, null) ?? new List<string>();
			builder.AddDropdown(options, Safe(entry.getIndex, 0), delegate
			{
				RefreshRules();
			}, entry.id);
			builder.Last.SetFixedWidth(306f);
			break;
		}
		case GameOptionRegistry.Kind.KeyBind:
		{
			KeyCode keyCode = Safe(entry.getKey, KeyCode.None);
			pendingKeys[entry.id] = keyCode;
			string optionId = entry.id;
			builder.AddButton(KeyBindHelper.KeyName(keyCode), delegate
			{
				OnKeyBindClicked(optionId);
			}, entry.id);
			builder.Last.SetFixedWidth(160f);
			break;
		}
		}
		builder.End();
	}

	public static bool ConsumedEscapeThisFrame()
	{
		return consumedEscapeFrame == Time.frameCount;
	}

	private void OnKeyBindClicked(string optionId)
	{
		if (IsOpen)
		{
			if (KeyBindHelper.CapturingOptionId == optionId)
			{
				SetKeyBindStatus("");
				EndCapture();
			}
			else
			{
				StartCapture(optionId);
			}
		}
	}

	private void StartCapture(string optionId)
	{
		if (KeyBindHelper.IsCapturing)
		{
			EndCapture();
		}
		KeyBindHelper.StartCapture(optionId);
		if (restoreCloseOnEscapeFrame < 0)
		{
			closeOnEscapeBeforeCapture = window.closeOnEscape;
		}
		restoreCloseOnEscapeFrame = -1;
		window.closeOnEscape = false;
		SetKeyBindStatus(LocHelper.GetLocalizedMainMenu("option_keybind_status_waiting"));
		RefreshKeyBindButton(optionId);
	}

	private void EndCapture()
	{
		string capturingOptionId = KeyBindHelper.CapturingOptionId;
		if (capturingOptionId != null)
		{
			KeyBindHelper.StopCapture();
			restoreCloseOnEscapeFrame = Time.frameCount + 1;
			RefreshKeyBindButton(capturingOptionId);
		}
	}

	public void PollKeyCapture()
	{
		if (lastCapturePollFrame == Time.frameCount)
		{
			return;
		}
		lastCapturePollFrame = Time.frameCount;
		if (restoreCloseOnEscapeFrame >= 0 && Time.frameCount >= restoreCloseOnEscapeFrame)
		{
			restoreCloseOnEscapeFrame = -1;
			if (IsOpen && !KeyBindHelper.IsCapturing)
			{
				window.closeOnEscape = closeOnEscapeBeforeCapture;
			}
		}
		if (!KeyBindHelper.IsCapturing)
		{
			return;
		}
		if (!IsOpen)
		{
			KeyBindHelper.StopCapture();
			return;
		}
		string capturingOptionId = KeyBindHelper.CapturingOptionId;
		KeyCode key;
		switch (KeyBindHelper.PollCapture(out key))
		{
		case KeyBindHelper.CaptureResult.Cancelled:
			consumedEscapeFrame = Time.frameCount;
			SetKeyBindStatus("");
			EndCapture();
			break;
		case KeyBindHelper.CaptureResult.Key:
		{
			if (KeyBindHelper.IsReserved(key))
			{
				SetKeyBindStatus(LocHelper.GetLocalizedMainMenu("option_keybind_status_reserved", KeyBindHelper.KeyName(key)));
				break;
			}
			string text = FindPendingKeyOwner(key, capturingOptionId);
			if (text != null)
			{
				GameOptionRegistry.Entry entry = GameOptionRegistry.Find(text);
				string text2 = ((entry != null) ? Safe(entry.label, text) : text);
				SetKeyBindStatus(LocHelper.GetLocalizedMainMenu("option_keybind_status_conflict", KeyBindHelper.KeyName(key), text2));
			}
			else
			{
				pendingKeys[capturingOptionId] = key;
				SetKeyBindStatus("");
				EndCapture();
				RefreshRules();
			}
			break;
		}
		}
	}

	private string FindPendingKeyOwner(KeyCode key, string exceptOptionId)
	{
		foreach (KeyValuePair<string, KeyCode> pendingKey in pendingKeys)
		{
			if (pendingKey.Key != exceptOptionId && pendingKey.Value == key)
			{
				return pendingKey.Key;
			}
		}
		return null;
	}

	private void RefreshKeyBindButton(string optionId)
	{
		if (IsOpen)
		{
			KeyCode value;
			string text = ((KeyBindHelper.CapturingOptionId == optionId) ? LocHelper.GetLocalizedMainMenu("option_keybind_press_key") : KeyBindHelper.KeyName(pendingKeys.TryGetValue(optionId, out value) ? value : KeyCode.None));
			window.Get(optionId)?.SetText(text);
		}
	}

	private void SetKeyBindStatus(string text)
	{
		KeyBindStatus = text ?? "";
		RefreshRules();
	}

	private static string NoteId(GameOptionRegistry.Section section)
	{
		return "note_" + section.id;
	}

	private static T Safe<T>(Func<T> getter, T fallback)
	{
		if (getter == null)
		{
			return fallback;
		}
		try
		{
			return getter();
		}
		catch (Exception arg)
		{
			Debug.LogError($"[GameOption] Option callback threw: {arg}");
			return fallback;
		}
	}

	private void RefreshRules()
	{
		if (!IsOpen)
		{
			return;
		}
		foreach (GameOptionRegistry.Section section in GameOptionRegistry.Sections)
		{
			if (section.note != null && section.noteVisible != null)
			{
				CustomUIElement customUIElement = window.Get(NoteId(section));
				if (customUIElement != null)
				{
					customUIElement.SetText(Safe(section.note, ""));
					customUIElement.SetVisible(Safe(() => section.noteVisible(this), fallback: true));
				}
			}
			foreach (GameOptionRegistry.Entry entry in section.entries)
			{
				if (entry.kind == GameOptionRegistry.Kind.Dropdown && entry.overrideOptions != null)
				{
					ApplyDropdownOverride(entry);
				}
				if (entry.interactable != null)
				{
					window.Get(entry.id)?.SetInteractable(Safe(() => entry.interactable(this), fallback: true));
				}
			}
		}
	}

	private void ApplyDropdownOverride(GameOptionRegistry.Entry entry)
	{
		CustomUIElement customUIElement = window.Get(entry.id);
		if (customUIElement == null)
		{
			return;
		}
		GameOptionRegistry.DropdownOverride dropdownOverride = Safe(() => entry.overrideOptions(this), null);
		OverrideState value;
		bool flag = overriddenDropdowns.TryGetValue(entry.id, out value);
		if (dropdownOverride != null)
		{
			if (!flag)
			{
				value = new OverrideState
				{
					keptIndex = customUIElement.GetIndex()
				};
				overriddenDropdowns[entry.id] = value;
			}
			value.apply = dropdownOverride.apply;
			IList<string> list = dropdownOverride.options ?? new List<string>();
			if (!SameOptions(value.options, list))
			{
				value.options = list;
				customUIElement.SetOptions(list).SetIndex(Mathf.Clamp(dropdownOverride.index, 0, Mathf.Max(0, list.Count - 1)));
			}
		}
		else if (flag)
		{
			overriddenDropdowns.Remove(entry.id);
			customUIElement.SetOptions(Safe(entry.options, null) ?? new List<string>()).SetIndex(value.keptIndex);
		}
	}

	private static bool SameOptions(IList<string> a, IList<string> b)
	{
		if (a == null || b == null || a.Count != b.Count)
		{
			return false;
		}
		for (int i = 0; i < a.Count; i++)
		{
			if (a[i] != b[i])
			{
				return false;
			}
		}
		return true;
	}

	public bool GetBool(string optionId)
	{
		if (IsOpen)
		{
			return window.Get(optionId)?.GetBool() ?? false;
		}
		return false;
	}

	public float GetFloat(string optionId)
	{
		if (!IsOpen)
		{
			return 0f;
		}
		return window.Get(optionId)?.GetFloat() ?? 0f;
	}

	public int GetIndex(string optionId)
	{
		if (!IsOpen)
		{
			return -1;
		}
		return window.Get(optionId)?.GetIndex() ?? (-1);
	}

	public KeyCode GetKey(string optionId)
	{
		if (!IsOpen || !pendingKeys.TryGetValue(optionId, out var value))
		{
			return KeyCode.None;
		}
		return value;
	}

	public void OnApplySetting()
	{
		if (!IsOpen || EmporiumOption.Instance == null)
		{
			return;
		}
		EmporiumOption instance = EmporiumOption.Instance;
		if (KeyBindHelper.IsCapturing)
		{
			SetKeyBindStatus("");
			EndCapture();
		}
		bool autoNextDialog = instance.autoNextDialog;
		int resolution = instance.resolution;
		int fullscreenMode = instance.fullscreenMode;
		int borderlessDivisor = instance.borderlessDivisor;
		foreach (GameOptionRegistry.Section section in GameOptionRegistry.Sections)
		{
			foreach (GameOptionRegistry.Entry entry in section.entries)
			{
				CustomUIElement customUIElement = window.Get(entry.id);
				if (customUIElement == null || !customUIElement.IsAlive)
				{
					continue;
				}
				try
				{
					switch (entry.kind)
					{
					case GameOptionRegistry.Kind.Toggle:
						entry.setBool?.Invoke(customUIElement.GetBool());
						break;
					case GameOptionRegistry.Kind.Slider:
						entry.setFloat?.Invoke(customUIElement.GetFloat());
						break;
					case GameOptionRegistry.Kind.Dropdown:
					{
						if (overriddenDropdowns.TryGetValue(entry.id, out var value2))
						{
							value2.apply?.Invoke(customUIElement.GetIndex());
						}
						else
						{
							entry.setIndex?.Invoke(customUIElement.GetIndex());
						}
						break;
					}
					case GameOptionRegistry.Kind.KeyBind:
					{
						if (pendingKeys.TryGetValue(entry.id, out var value))
						{
							entry.setKey?.Invoke(value);
						}
						break;
					}
					}
				}
				catch (Exception arg)
				{
					Debug.LogError($"[GameOption] Setter of option '{entry.id}' threw: {arg}");
				}
			}
		}
		if (instance.ClampResolutionToAvailable() && !overriddenDropdowns.ContainsKey("resolution"))
		{
			window.Get("resolution")?.SetIndex(instance.resolution);
		}
		bool applyResolution = instance.resolution != resolution || instance.fullscreenMode != fullscreenMode || instance.borderlessDivisor != borderlessDivisor;
		instance.ApplySetting(applyResolution);
		instance.OnAutoAdvanceSettingApplied(autoNextDialog);
		RefreshRules();
	}

	public static int MapResolution()
	{
		int systemWidth = Display.main.systemWidth;
		int systemHeight = Display.main.systemHeight;
		Debug.Log("Current Monitor Size: " + systemWidth + "x" + systemHeight);
		if (!EmporiumOption.IsMonitor16by9())
		{
			Debug.Log("Non 16:9 monitor detected. Using closest supported resolution.");
			return EmporiumOption.GetClosestWindowResolution();
		}
		int result = ((systemWidth == 3840 && systemHeight == 2160) ? 2 : ((systemWidth == 2560 && systemHeight == 1440) ? 6 : ((systemWidth != 1920 || systemHeight != 1080) ? ((systemWidth == 1600 && systemHeight == 900) ? 5 : ((systemWidth == 1366 && systemHeight == 768) ? 4 : ((systemWidth == 1280 && systemHeight == 720) ? 3 : ((systemWidth == 960 && systemHeight == 540) ? 1 : EmporiumOption.GetClosestWindowResolution())))) : 0)));
		Debug.Log("Mapped " + systemWidth + "x" + systemHeight + " to ResMode " + result);
		return result;
	}

	public static bool IsResolutionSupported(int width, int height)
	{
		return EmporiumOption.GetResolutionValueFromDimensions(width, height) >= 0;
	}
}
