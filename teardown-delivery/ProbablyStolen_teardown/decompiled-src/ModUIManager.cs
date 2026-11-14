using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ModUIManager : MonoBehaviour
{
	private class Row
	{
		public LoadedMod mod;

		public ModElement element;

		public GameObject warningIcon;

		public ModRowHoverTooltip warningTooltip;
	}

	private const float COLUMN_WIDTH = 388f;

	private const float COLUMN_GAP = 14f;

	private const float BLOCK_GAP = 32f;

	private const float ROW_HEIGHT = 34f;

	private const float ROW_ICON_SIZE = 20f;

	private const float SEARCH_ICON_SIZE = 48f;

	private const float LIST_PANEL_PADDING = 4f;

	private const float PREVIEW_PADDING = 8f;

	private const float FALLBACK_CONTENT_WIDTH = 1464f;

	private const float DEPENDENCY_HEADER_HEIGHT = 34f;

	private const float GEAR_BUTTON_SIZE = 42f;

	private const float BOTTOM_BUTTON_WIDTH = 260f;

	private const string COLOR_OK = "#4CBF6A";

	private const string COLOR_BAD = "#D9483F";

	private const string COLOR_LABEL = "#AFADA4";

	private const string COLOR_VALUE = "#D9D9D9";

	private static readonly Color DEPENDENCY_HEADER_COLOR = new Color32(15, 15, 15, 51);

	private static readonly Color DEPENDENCY_HEADER_TEXT_COLOR = new Color32(196, 196, 176, byte.MaxValue);

	private static readonly Color SCROLLBAR_COLOR = new Color32(138, 191, 187, byte.MaxValue);

	private const float SCROLLBAR_TRACK_WIDTH = 4f;

	private const float SCROLLBAR_HANDLE_WIDTH = 10f;

	public GameObject panel;

	[Tooltip("Editor-built area the whole mod UI is built into. Sized and placed in the editor; a layout group is added at runtime.")]
	public GameObject content;

	public GameObject modElementPrefab;

	[HideInInspector]
	public GameObject activeModLayoutObject;

	[HideInInspector]
	public GameObject inactiveModLayoutObject;

	private CustomUIWindow handle;

	private ScrollRect inactiveScroll;

	private ScrollRect activeScroll;

	private RectTransform dependenciesContainer;

	private readonly List<Row> rows = new List<Row>();

	private readonly Dictionary<string, Sprite> previewCache = new Dictionary<string, Sprite>();

	private readonly Dictionary<string, Sprite> localPreviewCache = new Dictionary<string, Sprite>();

	private readonly HashSet<string> previewDownloads = new HashSet<string>();

	private float previewImageWidth;

	private float previewMaxImageHeight;

	private string filter = "";

	private string selectedModID;

	public static ModUIManager Instance { get; private set; }

	public bool IsFiltering => !string.IsNullOrEmpty(filter);

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		ModWorkshop.OnItemInstalled += OnWorkshopItemInstalled;
		ModWorkshop.OnDetailsUpdated += OnWorkshopDetailsUpdated;
		ModWorkshop.OnDownloadsChanged += RefreshNotes;
		ModLoader.OnTrustRefreshed += OnTrustListChanged;
	}

	public void OpenPanel()
	{
		EnsureBuilt();
		RefreshNotes();
		filter = "";
		handle?.Get("search")?.SetString("");
		PopulateLayoutList();
		panel.SetActive(value: true);
		ModWorkshop.RequestDetails(from m in ModLoader.Mods.Where(IsWorkshopMod)
			select m.WorkshopId);
		ModWarningUI.ShowIfNeeded(DialogRoot());
	}

	public void ClosePanel()
	{
		ModWarningUI.Close();
		ModEnablePromptUI.Close();
		ModRefusalUI.Close();
		panel.SetActive(value: false);
	}

	private void Update()
	{
		if (panel == null || !panel.activeInHierarchy || !KeyBindHelper.GetBackDown() || StoreUIManager.IsTextFieldFocused() || ModWarningUI.IsOpen)
		{
			return;
		}
		if (ModEnablePromptUI.IsOpen || ModRefusalUI.IsOpen)
		{
			ModEnablePromptUI.Close();
			ModRefusalUI.Close();
			return;
		}
		ConfirmPanelUIManager confirmPanelUIManager = ((MainMenuUIController.Instance != null) ? MainMenuUIController.Instance.confirmPanelUIManager : null);
		if ((!(confirmPanelUIManager != null) || !(confirmPanelUIManager.panel != null) || !confirmPanelUIManager.panel.activeInHierarchy) && (!(CustomUIManager.Instance != null) || !CustomUIManager.Instance.HasEscapeTarget()))
		{
			if (MainMenuUIController.Instance != null)
			{
				MainMenuUIController.Instance.CloseModPanel();
			}
			else
			{
				ClosePanel();
			}
		}
	}

	private void EnsureBuilt()
	{
		if (handle == null || !handle.IsAlive)
		{
			CustomUIManager instance = CustomUIManager.Instance;
			if (content == null || instance == null)
			{
				Debug.LogError("[ModUI] ModUIManager needs a content container and a CustomUIManager in the scene");
				return;
			}
			RectTransform rectTransform = content.transform as RectTransform;
			CustomUIBuilder.ClearContainer(rectTransform);
			CustomUIBuilder customUIBuilder = instance.BuildInto(rectTransform, useMetaPrefabs: true, "mod_ui");
			float num = ((rectTransform.rect.width > 0f) ? rectTransform.rect.width : 1464f);
			float blockWidth = Mathf.Max(200f, num - 790f - 32f);
			customUIBuilder.BeginRow(32f);
			BuildListBlock(customUIBuilder, instance);
			BuildDetailBlock(customUIBuilder, instance, blockWidth);
			customUIBuilder.End();
			customUIBuilder.AddSeparator(null, 4);
			customUIBuilder.BeginRow(12f, TextAnchor.MiddleCenter);
			customUIBuilder.AddFlexibleSpace();
			customUIBuilder.AddButton(LocHelper.GetLocalizedUI("ui_mods_open_folder"), OpenModFolder, "open_folder");
			customUIBuilder.Last.SetFixedWidth(260f);
			customUIBuilder.AddButton(LocHelper.GetLocalizedUI("ui_mods_workshop"), ModWorkshop.OpenHub, "workshop");
			customUIBuilder.Last.SetFixedWidth(260f);
			customUIBuilder.AddButton(LocHelper.GetLocalizedUI("ui_mods_unsubscribe"), ConfirmUnsubscribe, "unsubscribe");
			customUIBuilder.Last.SetFixedWidth(260f).SetInteractable(interactable: false);
			customUIBuilder.AddButton(LocHelper.GetLocalizedUI("ui_mods_autosort"), ConfirmAutoSort, "autosort");
			customUIBuilder.Last.SetFixedWidth(260f);
			customUIBuilder.AddFlexibleSpace();
			customUIBuilder.End();
			customUIBuilder.BeginRow(0f, TextAnchor.MiddleCenter);
			customUIBuilder.AddFlexibleSpace();
			customUIBuilder.AddWarning(LocHelper.GetLocalizedUI("ui_mods_restart_note"), "restart_note");
			customUIBuilder.AddFlexibleSpace();
			customUIBuilder.End();
			customUIBuilder.BeginRow(0f, TextAnchor.MiddleCenter);
			customUIBuilder.AddFlexibleSpace();
			customUIBuilder.AddLabel("", "downloads_note");
			customUIBuilder.AddFlexibleSpace();
			customUIBuilder.End();
			customUIBuilder.BeginRow(0f, TextAnchor.MiddleCenter);
			customUIBuilder.AddFlexibleSpace();
			customUIBuilder.AddLabel("", "trust_note");
			customUIBuilder.AddFlexibleSpace();
			customUIBuilder.End();
			handle = customUIBuilder.Handle;
			RefreshNotes();
			ShowMod(null);
		}
	}

	private void BuildListBlock(CustomUIBuilder b, CustomUIManager manager)
	{
		b.BeginColumn(8f);
		SetFixedWidth(b.CurrentContainer, 790f, 1f);
		b.BeginRow(14f);
		b.BeginColumn();
		SetFixedWidth(b.CurrentContainer, 388f);
		b.AddLabel(LocHelper.GetLocalizedUI("ui_mods_inactive"), "header_inactive");
		b.End();
		b.BeginColumn();
		SetFixedWidth(b.CurrentContainer, 388f);
		b.AddLabel(LocHelper.GetLocalizedUI("ui_mods_active"), "header_active");
		b.End();
		b.End();
		b.BeginRow(14f);
		inactiveModLayoutObject = BuildListColumn(b, out inactiveScroll);
		activeModLayoutObject = BuildListColumn(b, out activeScroll);
		b.End();
		b.BeginRow(0f, TextAnchor.MiddleLeft);
		b.AddImage(manager.spriteMagnifier, 48f, 48f);
		b.AddInput(LocHelper.GetLocalizedUI("ui_mods_search_placeholder"), "", null, "search", ApplyFilter);
		b.Last.SetPreferredSize(-1f, 48f);
		b.End();
		b.End();
	}

	private static GameObject BuildListColumn(CustomUIBuilder b, out ScrollRect scroll)
	{
		b.BeginColumn();
		SetFixedWidth(b.CurrentContainer, 388f, 1f);
		b.BeginPanel(-1f, -1f, TextAnchor.UpperLeft, 4f, 0f);
		SetFlexible(b.CurrentContainer, 1f, 1f);
		b.BeginScrollFlexible();
		RectTransform currentContainer = b.CurrentContainer;
		scroll = currentContainer.GetComponentInParent<ScrollRect>();
		StyleScrollbar(scroll);
		b.End();
		b.End();
		b.End();
		return currentContainer.gameObject;
	}

	private void BuildDetailBlock(CustomUIBuilder b, CustomUIManager manager, float blockWidth)
	{
		b.BeginColumn(8f);
		SetFixedWidth(b.CurrentContainer, blockWidth, 1f);
		previewImageWidth = blockWidth - 16f;
		previewMaxImageHeight = Mathf.Round(blockWidth * 9f / 16f) - 16f;
		b.BeginPanel(-1f, previewMaxImageHeight + 16f, TextAnchor.MiddleCenter, 8f, 0f, "preview_panel");
		b.AddImage((Sprite)null, -1f, previewMaxImageHeight, "preview");
		b.Last.SetFlexible(1f, -1f);
		if (b.Last.image != null)
		{
			b.Last.image.preserveAspect = true;
		}
		b.End();
		b.AddHeader("", "detail_name");
		b.BeginRow(8f, TextAnchor.MiddleLeft);
		b.BeginColumn(2f);
		b.AddLabel("", "detail_author");
		b.AddLabel("", "detail_version");
		b.AddLabel("", "detail_game_versions");
		b.AddLabel("", "detail_override");
		b.AddLabel("", "detail_capabilities");
		b.AddLabel("", "detail_trust");
		b.AddButton("", ToggleDeveloperMode, "devmode");
		b.Last.SetFixedWidth(260f);
		b.End();
		b.AddFlexibleSpace();
		b.AddButton(LocHelper.GetLocalizedUIOrDefault("ui_mods_review", "Review and enable"), ReviewSelectedMod, "review");
		b.AddButton(LocHelper.GetLocalizedUI("ui_mods_activate"), ToggleSelectedMod, "activate");
		b.AddIconButton(manager.spriteSetting, 42f, OpenSelectedModSettings, manager.spriteSettingHL, "settings");
		b.AddIconButton(manager.spriteSteam, 42f, OpenSelectedModPage, null, "workshop_page");
		b.End();
		b.BeginScrollFlexible();
		StyleScrollbar(b.CurrentContainer.GetComponentInParent<ScrollRect>());
		b.BeginPanel(-1f, 34f, TextAnchor.MiddleCenter, 4f, 0f, "dependencies_header");
		if (b.Last.image != null)
		{
			b.Last.image.sprite = null;
			b.Last.image.type = Image.Type.Simple;
			b.Last.image.color = DEPENDENCY_HEADER_COLOR;
		}
		b.AddLabel(LocHelper.GetLocalizedUI("ui_mods_dependencies"));
		b.Last.SetTextColor(DEPENDENCY_HEADER_TEXT_COLOR);
		b.End();
		b.BeginColumn(2f);
		dependenciesContainer = b.CurrentContainer;
		b.End();
		b.AddSpace(8f);
		b.AddLabel("", "detail_description");
		b.End();
		b.End();
	}

	private static void StyleScrollbar(ScrollRect scroll)
	{
		if (scroll == null || scroll.verticalScrollbar == null)
		{
			return;
		}
		Scrollbar verticalScrollbar = scroll.verticalScrollbar;
		RectTransform rectTransform = verticalScrollbar.transform as RectTransform;
		rectTransform.sizeDelta = new Vector2(10f, rectTransform.sizeDelta.y);
		rectTransform.anchoredPosition = new Vector2(-5f, rectTransform.anchoredPosition.y);
		Image component = verticalScrollbar.GetComponent<Image>();
		if (component != null)
		{
			component.enabled = false;
		}
		GameObject obj = new GameObject("track", typeof(RectTransform));
		RectTransform obj2 = (RectTransform)obj.transform;
		obj2.SetParent(verticalScrollbar.transform, worldPositionStays: false);
		obj2.SetAsFirstSibling();
		obj2.anchorMin = new Vector2(0.5f, 0f);
		obj2.anchorMax = new Vector2(0.5f, 1f);
		obj2.sizeDelta = new Vector2(4f, 0f);
		obj2.anchoredPosition = Vector2.zero;
		Image image = obj.AddComponent<Image>();
		image.sprite = null;
		image.color = SCROLLBAR_COLOR;
		image.raycastTarget = false;
		if (verticalScrollbar.handleRect != null)
		{
			Image component2 = verticalScrollbar.handleRect.GetComponent<Image>();
			if (component2 != null)
			{
				component2.sprite = null;
				component2.type = Image.Type.Simple;
				component2.color = SCROLLBAR_COLOR;
			}
		}
	}

	private static void SetFixedWidth(RectTransform rect, float width, float flexibleHeight = -1f)
	{
		LayoutElement layoutElement = rect.GetComponent<LayoutElement>();
		if (layoutElement == null)
		{
			layoutElement = rect.gameObject.AddComponent<LayoutElement>();
		}
		layoutElement.minWidth = width;
		layoutElement.preferredWidth = width;
		layoutElement.flexibleWidth = 0f;
		if (flexibleHeight >= 0f)
		{
			layoutElement.flexibleHeight = flexibleHeight;
		}
	}

	private static void SetFlexible(RectTransform rect, float width, float height)
	{
		LayoutElement layoutElement = rect.GetComponent<LayoutElement>();
		if (layoutElement == null)
		{
			layoutElement = rect.gameObject.AddComponent<LayoutElement>();
		}
		layoutElement.flexibleWidth = width;
		layoutElement.flexibleHeight = height;
	}

	public void PopulateLayoutList()
	{
		if (activeModLayoutObject == null || inactiveModLayoutObject == null)
		{
			return;
		}
		for (int num = activeModLayoutObject.transform.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(activeModLayoutObject.transform.GetChild(num).gameObject);
		}
		for (int num2 = inactiveModLayoutObject.transform.childCount - 1; num2 >= 0; num2--)
		{
			UnityEngine.Object.Destroy(inactiveModLayoutObject.transform.GetChild(num2).gameObject);
		}
		rows.Clear();
		foreach (LoadedMod mod in ModLoader.Mods)
		{
			GameObject gameObject = (IsActive(mod) ? activeModLayoutObject : inactiveModLayoutObject);
			rows.Add(CreateRow(mod, gameObject.transform));
		}
		RefreshRowIssues();
		ApplyFilterToRows();
		SelectMod(null);
	}

	private Row CreateRow(LoadedMod mod, Transform parent)
	{
		CustomUIManager instance = CustomUIManager.Instance;
		GameObject gameObject = UnityEngine.Object.Instantiate(modElementPrefab, parent);
		LayoutElement layoutElement = gameObject.GetComponent<LayoutElement>();
		if (layoutElement == null)
		{
			layoutElement = gameObject.AddComponent<LayoutElement>();
		}
		layoutElement.minHeight = 34f;
		layoutElement.preferredHeight = 34f;
		layoutElement.flexibleWidth = 1f;
		HorizontalLayoutGroup horizontalLayoutGroup = gameObject.GetComponent<HorizontalLayoutGroup>();
		if (horizontalLayoutGroup == null)
		{
			horizontalLayoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
		}
		horizontalLayoutGroup.padding = new RectOffset(8, 8, 0, 0);
		horizontalLayoutGroup.spacing = 8f;
		horizontalLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
		horizontalLayoutGroup.childControlWidth = true;
		horizontalLayoutGroup.childControlHeight = true;
		horizontalLayoutGroup.childForceExpandWidth = false;
		horizontalLayoutGroup.childForceExpandHeight = false;
		ModElement component = gameObject.GetComponent<ModElement>();
		component.Init(mod.Manifest.ID, mod.Manifest.Name);
		CreateIcon("source_icon", gameObject.transform, IsWorkshopMod(mod) ? instance.spriteSteam : instance.spriteFile, raycast: false).transform.SetSiblingIndex(0);
		if (component.modNameTMP != null)
		{
			LayoutElement layoutElement2 = component.modNameTMP.GetComponent<LayoutElement>();
			if (layoutElement2 == null)
			{
				layoutElement2 = component.modNameTMP.gameObject.AddComponent<LayoutElement>();
			}
			layoutElement2.flexibleWidth = 1f;
			component.modNameTMP.enableWordWrapping = false;
			component.modNameTMP.overflowMode = TextOverflowModes.Ellipsis;
			component.modNameTMP.raycastTarget = false;
		}
		Image image = CreateIcon("warning_icon", gameObject.transform, instance.spriteWarning, raycast: true);
		ModRowHoverTooltip warningTooltip = image.gameObject.AddComponent<ModRowHoverTooltip>();
		return new Row
		{
			mod = mod,
			element = component,
			warningIcon = image.gameObject,
			warningTooltip = warningTooltip
		};
	}

	private static Image CreateIcon(string name, Transform parent, Sprite sprite, bool raycast)
	{
		GameObject obj = new GameObject(name, typeof(RectTransform));
		obj.transform.SetParent(parent, worldPositionStays: false);
		Image image = obj.AddComponent<Image>();
		image.sprite = sprite;
		image.preserveAspect = true;
		image.raycastTarget = raycast;
		LayoutElement layoutElement = obj.AddComponent<LayoutElement>();
		layoutElement.minWidth = 20f;
		layoutElement.minHeight = 20f;
		layoutElement.preferredWidth = 20f;
		layoutElement.preferredHeight = 20f;
		return image;
	}

	private static bool IsWorkshopMod(LoadedMod mod)
	{
		if (mod != null)
		{
			return mod.Source == ModSource.Workshop;
		}
		return false;
	}

	private static ulong SubscriptionId(LoadedMod mod)
	{
		if (mod == null)
		{
			return 0uL;
		}
		if (!IsWorkshopMod(mod))
		{
			return mod.ShadowedWorkshopId;
		}
		return mod.WorkshopId;
	}

	private static bool IsActive(LoadedMod mod)
	{
		if (mod != null)
		{
			return ModLoader.IsModEnabled(mod.Manifest.ID);
		}
		return false;
	}

	private static string LoadErrorLine(LoadedMod mod)
	{
		return LocHelper.GetLocalizedUI("ui_mods_issue_load_failed", ModLoader.GetModLabel(mod.Manifest), mod.LoadError);
	}

	private void RefreshRowIssues()
	{
		foreach (Row row in rows)
		{
			if (!(row.element == null))
			{
				string modIssue = GetModIssue(row.mod);
				row.warningIcon.SetActive(modIssue != null);
				row.warningTooltip.text = modIssue;
			}
		}
	}

	private Row FindRow(string modID)
	{
		return rows.Find((Row r) => r.mod.Manifest.ID == modID);
	}

	private void ApplyFilter(string text)
	{
		filter = text ?? "";
		ApplyFilterToRows();
		ScrollToTop(inactiveScroll);
		ScrollToTop(activeScroll);
	}

	private void ApplyFilterToRows()
	{
		foreach (Row row in rows)
		{
			if (!(row.element == null))
			{
				row.element.gameObject.SetActive(Matches(row.element));
			}
		}
	}

	private bool Matches(ModElement element)
	{
		if (!IsFiltering)
		{
			return true;
		}
		if (!Contains(element.ModName, filter))
		{
			return Contains(element.ModID, filter);
		}
		return true;
	}

	private static bool Contains(string haystack, string needle)
	{
		if (!string.IsNullOrEmpty(haystack))
		{
			return haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
		}
		return false;
	}

	private static void ScrollToTop(ScrollRect scroll)
	{
		if (scroll != null)
		{
			scroll.verticalNormalizedPosition = 1f;
		}
	}

	public void OnModElementClicked(string modID, ModElement element)
	{
		SelectMod(modID);
	}

	public void OnModElementDoubleClicked(string modID, ModElement element)
	{
		SelectMod(modID);
		ToggleSelectedMod();
	}

	private void SelectMod(string modID)
	{
		selectedModID = modID;
		ShowMod((modID != null) ? ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == modID) : null);
	}

	private void ShowMod(LoadedMod mod)
	{
		if (handle == null)
		{
			return;
		}
		CustomUIManager instance = CustomUIManager.Instance;
		ApplyPreview(mod);
		if (mod == null)
		{
			ScriptFontSwap.Apply(handle.Get("detail_name")?.text, "");
			handle.Get("detail_author")?.SetText("");
			handle.Get("detail_version")?.SetText("");
			handle.Get("detail_game_versions")?.SetText("");
			ScriptFontSwap.Apply(handle.Get("detail_description")?.text, "");
			handle.Get("activate")?.SetText(LocHelper.GetLocalizedUI("ui_mods_activate"))?.SetInteractable(interactable: false);
			handle.Get("settings")?.SetInteractable(interactable: false);
			handle.Get("detail_override")?.SetVisible(visible: false);
			handle.Get("detail_capabilities")?.SetVisible(visible: false);
			handle.Get("detail_trust")?.SetVisible(visible: false);
			handle.Get("devmode")?.SetVisible(visible: false);
			handle.Get("review")?.SetVisible(visible: false);
			handle.Get("workshop_page")?.SetVisible(visible: false);
			handle.Get("unsubscribe")?.SetInteractable(interactable: false);
			SetDependenciesVisible(visible: false);
			CustomUIBuilder.ClearContainer(dependenciesContainer);
			return;
		}
		ModManifest manifest = mod.Manifest;
		CustomUIElement customUIElement = handle.Get("detail_name");
		if (customUIElement != null)
		{
			ScriptFontSwap.Apply(customUIElement.text, manifest.Name);
			customUIElement.SetTextColor(Color.white);
		}
		handle.Get("detail_author")?.SetText(InfoLine("ui_mods_author", manifest.Author));
		handle.Get("detail_version")?.SetText(InfoLine("ui_mods_mod_version", manifest.ModVersion));
		handle.Get("detail_game_versions")?.SetText(InfoLine("ui_mods_game_versions", FormatGameVersions(manifest)));
		ScriptFontSwap.Apply(handle.Get("detail_description")?.text, manifest.Description);
		handle.Get("activate")?.SetText(LocHelper.GetLocalizedUI(IsActive(mod) ? "ui_mods_deactivate" : "ui_mods_activate"))?.SetInteractable(interactable: true);
		handle.Get("settings")?.SetInteractable(ModHasSettings(manifest.ID));
		bool flag = !IsWorkshopMod(mod) && mod.ShadowedWorkshopId != 0;
		handle.Get("detail_override")?.SetText(flag ? ("<color=#AFADA4>" + LocHelper.GetLocalizedUI("ui_mods_local_override") + "</color>") : "");
		handle.Get("detail_override")?.SetVisible(flag);
		ShowTrust(mod);
		bool flag2 = SubscriptionId(mod) != 0;
		handle.Get("workshop_page")?.SetVisible(flag2);
		handle.Get("unsubscribe")?.SetInteractable(flag2 && ModWorkshop.IsAvailable);
		CustomUIBuilder.ClearContainer(dependenciesContainer);
		SetDependenciesVisible(manifest.Prerequisites.Count > 0);
		CustomUIBuilder customUIBuilder = new CustomUIBuilder(instance, handle, dependenciesContainer);
		foreach (string prerequisiteID in manifest.Prerequisites)
		{
			LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == prerequisiteID);
			bool flag3 = IsActive(loadedMod) && loadedMod.LoadError == null;
			customUIBuilder.BeginRow(8f, TextAnchor.MiddleLeft);
			customUIBuilder.AddImage((loadedMod != null && IsWorkshopMod(loadedMod)) ? instance.spriteSteam : instance.spriteFile, 20f, 20f);
			customUIBuilder.AddLabel("");
			ScriptFontSwap.Apply(customUIBuilder.Last?.text, (loadedMod != null) ? loadedMod.Manifest.Name : prerequisiteID);
			customUIBuilder.AddFlexibleSpace();
			customUIBuilder.AddImage(flag3 ? instance.spriteGreenCheck : instance.spriteRedXMark, 20f, 20f);
			customUIBuilder.End();
		}
	}

	private static string InfoLine(string key, string value)
	{
		return "<color=#AFADA4>" + LocHelper.GetLocalizedUI(key, "<color=#D9D9D9>" + value + "</color>") + "</color>";
	}

	private static string InfoLineOrDefault(string key, string english, string value)
	{
		return "<color=#AFADA4>" + LocHelper.GetLocalizedUIOrDefault(key, english, "<color=#D9D9D9>" + value + "</color>") + "</color>";
	}

	private void ShowTrust(LoadedMod mod)
	{
		ModManifest manifest = mod.Manifest;
		string value = ((manifest.Capabilities.Count == 0) ? LocHelper.GetLocalizedUIOrDefault("ui_mods_capabilities_none", "none") : string.Join(", ", ModCapability.Order(manifest.Capabilities).Select(ModCapability.DisplayName)));
		handle.Get("detail_capabilities")?.SetText(InfoLineOrDefault("ui_mods_capabilities", "Capabilities: {0}", value));
		handle.Get("detail_capabilities")?.SetVisible(visible: true);
		string text = TrustLine(mod);
		handle.Get("detail_trust")?.SetText((text != null) ? ("<color=#AFADA4>" + text + "</color>") : "");
		handle.Get("detail_trust")?.SetVisible(text != null);
		bool visible = IsActive(mod) && (mod.TrustState == ModTrustState.Unknown || mod.TrustState == ModTrustState.Changed);
		handle.Get("review")?.SetVisible(visible);
		if ((mod.TrustState == ModTrustState.Blocked || mod.TrustState == ModTrustState.Stopped) && !IsActive(mod))
		{
			handle.Get("activate")?.SetText(LocHelper.GetLocalizedUIOrDefault("ui_mods_blocked", "Blocked"))?.SetInteractable(interactable: false);
		}
		ModTrustRecord modTrustRecord = ModTrust.Get(manifest.ID);
		bool flag = mod.Source == ModSource.Local && modTrustRecord != null && modTrustRecord.source == ModSource.Local.ToString();
		handle.Get("devmode")?.SetVisible(flag);
		if (flag)
		{
			handle.Get("devmode")?.SetText(modTrustRecord.developerMode ? LocHelper.GetLocalizedUIOrDefault("ui_mods_devmode_on", "Developer mode: on") : LocHelper.GetLocalizedUIOrDefault("ui_mods_devmode_off", "Developer mode: off"));
		}
	}

	private static string TrustLine(LoadedMod mod)
	{
		ModTrustRecord modTrustRecord = ModTrust.Get(mod.Manifest.ID);
		switch (mod.TrustState)
		{
		case ModTrustState.Unknown:
		case ModTrustState.Changed:
		case ModTrustState.Blocked:
		case ModTrustState.Paused:
		case ModTrustState.Stopped:
			return TrustIssue(mod);
		case ModTrustState.Updated:
			return LocHelper.GetLocalizedUIOrDefault("ui_mods_trust_updated", "Updated {0} with no new capabilities. Approved {1}.", FormatDate(modTrustRecord?.UpdatedAtUtc), FormatDate(modTrustRecord?.ApprovedAtUtc));
		case ModTrustState.DeveloperMode:
			return LocHelper.GetLocalizedUIOrDefault("ui_mods_trust_devmode", "Developer mode: every change of this folder is trusted.");
		default:
			if (mod.ListVerdict == ModListVerdict.Verified)
			{
				return LocHelper.GetLocalizedUIOrDefault("ui_mods_trust_verified", "Verified by the developers.");
			}
			if (modTrustRecord != null)
			{
				return LocHelper.GetLocalizedUIOrDefault("ui_mods_trust_approved", "Approved on this computer {0}.", FormatDate(modTrustRecord.ApprovedAtUtc));
			}
			return null;
		}
	}

	internal static string FormatDate(DateTime? utc)
	{
		if (!utc.HasValue)
		{
			return "-";
		}
		return utc.Value.ToLocalTime().ToString("d");
	}

	private static string TrustIssue(LoadedMod mod)
	{
		return mod.TrustState switch
		{
			ModTrustState.Blocked => LocHelper.GetLocalizedUIOrDefault("ui_mods_trust_blocked", "Blocked by the developers: {0}", mod.TrustNote ?? ""), 
			ModTrustState.Stopped => LocHelper.GetLocalizedUIOrDefault("ui_mods_trust_stopped", "Stopped and deactivated by the game: {0}. Delete or unsubscribe this mod, and change your Steam password if it ran on this computer.", mod.TrustNote ?? ""), 
			ModTrustState.Paused => LocHelper.GetLocalizedUIOrDefault("ui_mods_trust_paused", "Not loaded: mod loading is paused by the developers."), 
			ModTrustState.Unknown => LocHelper.GetLocalizedUIOrDefault("ui_mods_trust_unknown", "Not loaded: not yet approved on this computer. Use Review and enable to load it."), 
			ModTrustState.Changed => LocHelper.GetLocalizedUIOrDefault("ui_mods_trust_changed", "Not loaded: changed since you approved it ({0}). Use Review and enable to approve it again.", mod.TrustNote ?? ""), 
			_ => null, 
		};
	}

	private void BeginEnable(LoadedMod mod, Action onApproved)
	{
		ModEnableCheck check = ModLoader.CheckEnable(mod);
		ConfirmPanelUIManager confirmPanelUIManager = ((MainMenuUIController.Instance != null) ? MainMenuUIController.Instance.confirmPanelUIManager : null);
		string modLabel = ModLoader.GetModLabel(mod.Manifest);
		if (check.Refusal != null)
		{
			if (!ModRefusalUI.Show(DialogRoot(), modLabel, check.Refusal, ModRefusalUI.BuildLog(check)))
			{
				if (confirmPanelUIManager != null)
				{
					confirmPanelUIManager.OpenPanel(check.Refusal, null, null, hideNoButton: true);
				}
				else
				{
					Debug.LogWarning("[ModUI] " + check.Refusal);
				}
			}
			RefreshRowIssues();
			ShowMod(mod);
		}
		else if (check.NeedsPrompt)
		{
			ulong publishedFileId = (IsWorkshopMod(mod) ? mod.WorkshopId : 0);
			if (!ModEnablePromptUI.Show(DialogRoot(), modLabel, mod.Manifest.Capabilities, publishedFileId, Approve))
			{
				if (confirmPanelUIManager == null)
				{
					Debug.LogWarning("[ModUI] Cannot enable '" + mod.Manifest.ID + "': it declares capabilities and there is no dialog to ask the player.");
				}
				else
				{
					confirmPanelUIManager.OpenPanel(EnablePromptText(modLabel, mod.Manifest.Capabilities), Approve);
				}
			}
		}
		else
		{
			Approve();
		}
		void Approve()
		{
			ModLoader.ApproveAndEnable(mod, check);
			onApproved?.Invoke();
		}
	}

	private Transform DialogRoot()
	{
		Canvas canvas = ((panel != null) ? panel.GetComponentInParent<Canvas>() : null);
		if (!(canvas != null))
		{
			return null;
		}
		return canvas.rootCanvas.transform;
	}

	public static string EnablePromptText(string modLabel, IEnumerable<string> capabilities)
	{
		return LocHelper.GetLocalizedUIOrDefault("ui_mods_enable_prompt", "{0} asks for these capabilities:\n{1}\n\nA mod with these capabilities can do anything on this computer that you can. Only enable mods from authors you trust. Enable it?", modLabel, ModEnablePromptUI.FormatCapabilities(capabilities));
	}

	private void MoveRow(Row row, bool active)
	{
		if (row != null && !(row.element == null))
		{
			GameObject gameObject = (active ? activeModLayoutObject : inactiveModLayoutObject);
			row.element.transform.SetParent(gameObject.transform, worldPositionStays: false);
			RefreshNotes();
			RefreshRowIssues();
			ShowMod(row.mod);
		}
	}

	private void ReviewSelectedMod()
	{
		LoadedMod mod = ((selectedModID != null) ? ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == selectedModID) : null);
		if (mod != null)
		{
			BeginEnable(mod, delegate
			{
				RefreshNotes();
				RefreshRowIssues();
				ShowMod(mod);
			});
		}
	}

	private void ToggleDeveloperMode()
	{
		LoadedMod loadedMod = ((selectedModID != null) ? ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == selectedModID) : null);
		if (loadedMod != null)
		{
			ModTrustRecord modTrustRecord = ModTrust.Get(loadedMod.Manifest.ID);
			if (modTrustRecord != null)
			{
				ModLoader.SetDeveloperMode(loadedMod, !modTrustRecord.developerMode);
				ShowMod(loadedMod);
			}
		}
	}

	private void OnTrustListChanged()
	{
		RefreshNotes();
		RefreshRowIssues();
		if (selectedModID != null)
		{
			ShowMod(ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == selectedModID));
		}
	}

	private void SetDependenciesVisible(bool visible)
	{
		handle?.Get("dependencies_header")?.SetVisible(visible);
		if (dependenciesContainer != null)
		{
			dependenciesContainer.gameObject.SetActive(visible);
		}
	}

	private static string FormatGameVersions(ModManifest manifest)
	{
		if (manifest.GameVersions.Count == 0)
		{
			return "-";
		}
		return string.Join("  ", manifest.GameVersions.Select((string v) => "<color=" + (VersionMatchesGame(v) ? "#4CBF6A" : "#D9483F") + ">" + v + "</color>"));
	}

	private static bool VersionMatchesGame(string version)
	{
		string text = NormalizeVersion(version);
		string text2 = NormalizeVersion(Application.version);
		if (text.Length == 0 || text2.Length == 0)
		{
			return false;
		}
		if (!(text == text2) && !text.StartsWith(text2 + "."))
		{
			return text2.StartsWith(text + ".");
		}
		return true;
	}

	private static string NormalizeVersion(string version)
	{
		return new string((version ?? "").Trim().TakeWhile((char c) => char.IsDigit(c) || c == '.').ToArray()).TrimEnd('.');
	}

	private static bool ModHasSettings(string modID)
	{
		return ModConfig.HasSettings(modID);
	}

	private static string GetModIssue(LoadedMod mod)
	{
		List<string> list = new List<string>();
		if (mod.LoadError != null)
		{
			list.Add(LoadErrorLine(mod));
		}
		string text = TrustIssue(mod);
		if (text != null)
		{
			list.Add(text);
		}
		foreach (string unlistedDll in mod.UnlistedDlls)
		{
			list.Add(LocHelper.GetLocalizedUIOrDefault("ui_mods_issue_unlisted_dll", "{0} is in the mod folder but not listed in its manifest and was not loaded", unlistedDll));
		}
		foreach (string prerequisiteID in mod.Manifest.Prerequisites)
		{
			LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == prerequisiteID);
			if (loadedMod == null)
			{
				list.Add(LocHelper.GetLocalizedUI("ui_mods_issue_missing", prerequisiteID));
			}
			else if (!IsActive(loadedMod))
			{
				list.Add(LocHelper.GetLocalizedUI("ui_mods_issue_inactive", loadedMod.Manifest.Name));
			}
			else if (loadedMod.LoadError != null)
			{
				list.Add(LoadErrorLine(loadedMod));
			}
		}
		if (mod.Manifest.GameVersions.Count > 0 && !mod.Manifest.GameVersions.Any(VersionMatchesGame))
		{
			list.Add(LocHelper.GetLocalizedUI("ui_mods_issue_version", Application.version));
		}
		if (list.Count <= 0)
		{
			return null;
		}
		return string.Join("\n", list);
	}

	private void ApplyPreview(LoadedMod mod)
	{
		CustomUIElement customUIElement = handle?.Get("preview_panel");
		if (customUIElement != null)
		{
			Sprite sprite = ((mod != null) ? GetPreview(mod) : null);
			if (sprite == null)
			{
				customUIElement.SetVisible(visible: false);
				return;
			}
			float num = sprite.rect.width / Mathf.Max(1f, sprite.rect.height);
			float num2 = Mathf.Min(Mathf.Round(previewImageWidth / num), previewMaxImageHeight);
			SetFixedHeight(customUIElement.Rect, num2 + 16f);
			handle.Get("preview")?.SetSprite(sprite).SetPreferredSize(-1f, num2);
			customUIElement.SetVisible(visible: true);
		}
	}

	private Sprite GetPreview(LoadedMod mod)
	{
		string iD = mod.Manifest.ID;
		if (previewCache.TryGetValue(iD, out var value))
		{
			return value;
		}
		if (IsWorkshopMod(mod) && ModWorkshop.IsAvailable)
		{
			if (ModWorkshop.TryGetPreviewUrl(mod.WorkshopId, out var url))
			{
				if (base.isActiveAndEnabled && previewDownloads.Add(iD))
				{
					StartCoroutine(DownloadPreview(mod, url));
				}
				return null;
			}
			if (ModWorkshop.IsDetailsPending(mod.WorkshopId))
			{
				return null;
			}
			if (!ModWorkshop.HasDetails(mod.WorkshopId))
			{
				return GetLocalPreview(mod);
			}
		}
		Sprite localPreview = GetLocalPreview(mod);
		previewCache[iD] = localPreview;
		return localPreview;
	}

	private IEnumerator DownloadPreview(LoadedMod mod, string url)
	{
		string id = mod.Manifest.ID;
		Sprite sprite = null;
		using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url, nonReadable: true))
		{
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				Texture2D texture2D = DownloadHandlerTexture.GetContent(request);
				if (texture2D != null && (texture2D.width > 8 || texture2D.height > 8))
				{
					sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100f);
				}
			}
			else
			{
				Debug.LogWarning("[ModUI] Could not download the Workshop preview of '" + id + "': " + request.error);
			}
		}
		previewDownloads.Remove(id);
		previewCache[id] = ((sprite != null) ? sprite : GetLocalPreview(mod));
		if (selectedModID == id)
		{
			ApplyPreview(mod);
		}
	}

	private void OnWorkshopDetailsUpdated()
	{
		if (base.isActiveAndEnabled && selectedModID != null)
		{
			LoadedMod mod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == selectedModID);
			if (IsWorkshopMod(mod))
			{
				ApplyPreview(mod);
			}
		}
	}

	private void OnDisable()
	{
		previewDownloads.Clear();
	}

	private Sprite GetLocalPreview(LoadedMod mod)
	{
		string iD = mod.Manifest.ID;
		if (localPreviewCache.TryGetValue(iD, out var value))
		{
			return value;
		}
		Sprite sprite = null;
		if (mod.Sprites != null && mod.Sprites.TryGetValue("preview", out var value2))
		{
			sprite = value2;
		}
		else if (!string.IsNullOrEmpty(mod.FolderPath))
		{
			string path = Path.Combine(mod.FolderPath, "preview.png");
			if (File.Exists(path))
			{
				try
				{
					Texture2D texture2D = new Texture2D(2, 2, TextureFormat.RGBA32, mipChain: false);
					if (texture2D.LoadImage(File.ReadAllBytes(path)))
					{
						sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100f);
					}
				}
				catch (Exception ex)
				{
					Debug.LogWarning("[ModUI] Could not load preview for '" + iD + "': " + ex.Message);
				}
			}
		}
		localPreviewCache[iD] = sprite;
		return sprite;
	}

	private static void SetFixedHeight(RectTransform rect, float height)
	{
		if (!(rect == null))
		{
			LayoutElement layoutElement = rect.GetComponent<LayoutElement>();
			if (layoutElement == null)
			{
				layoutElement = rect.gameObject.AddComponent<LayoutElement>();
			}
			layoutElement.minHeight = height;
			layoutElement.preferredHeight = height;
		}
	}

	private void ToggleSelectedMod()
	{
		if (selectedModID == null)
		{
			return;
		}
		LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == selectedModID);
		Row row = FindRow(selectedModID);
		if (loadedMod == null || row == null || row.element == null)
		{
			return;
		}
		if (!IsActive(loadedMod))
		{
			BeginEnable(loadedMod, delegate
			{
				MoveRow(row, active: true);
			});
		}
		else
		{
			ModLoader.SetModEnabled(selectedModID, enabled: false);
			MoveRow(row, active: false);
		}
	}

	public void OnModOrderChanged()
	{
		List<string> list = new List<string>();
		foreach (Transform item in activeModLayoutObject.transform)
		{
			ModElement component = item.GetComponent<ModElement>();
			if (component != null && ModLoader.IsModEnabled(component.ModID))
			{
				list.Add(component.ModID);
			}
		}
		ModLoader.ReorderMods(list);
		RefreshNotes();
	}

	private void ConfirmAutoSort()
	{
		if (MainMenuUIController.Instance == null || MainMenuUIController.Instance.confirmPanelUIManager == null)
		{
			AutoSortActiveMods();
		}
		else
		{
			MainMenuUIController.Instance.confirmPanelUIManager.OpenPanel(LocHelper.GetLocalizedUI("ui_mods_autosort_confirm"), AutoSortActiveMods);
		}
	}

	private void AutoSortActiveMods()
	{
		List<LoadedMod> active = ModLoader.Mods.Where(IsActive).ToList();
		List<LoadedMod> sorted = new List<LoadedMod>();
		HashSet<string> done = new HashSet<string>();
		HashSet<string> visiting = new HashSet<string>();
		foreach (LoadedMod item in active)
		{
			Visit(item);
		}
		for (int i = 0; i < sorted.Count; i++)
		{
			Row row = FindRow(sorted[i].Manifest.ID);
			if (row != null && row.element != null)
			{
				row.element.transform.SetSiblingIndex(i);
			}
		}
		ModLoader.ReorderMods(sorted.Select((LoadedMod m) => m.Manifest.ID).ToList());
		RefreshNotes();
		void Visit(LoadedMod mod)
		{
			string iD = mod.Manifest.ID;
			if (!done.Contains(iD) && !visiting.Contains(iD))
			{
				visiting.Add(iD);
				foreach (string prerequisiteID in mod.Manifest.Prerequisites)
				{
					LoadedMod loadedMod = active.Find((LoadedMod m) => m.Manifest.ID == prerequisiteID);
					if (loadedMod != null)
					{
						Visit(loadedMod);
					}
				}
				visiting.Remove(iD);
				done.Add(iD);
				sorted.Add(mod);
			}
		}
	}

	private void OpenSelectedModSettings()
	{
		if (selectedModID != null && ModHasSettings(selectedModID))
		{
			LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == selectedModID);
			if (loadedMod != null)
			{
				ModConfigWindow.Open(loadedMod);
			}
		}
	}

	private void OpenSelectedModPage()
	{
		ulong num = SubscriptionId((selectedModID != null) ? ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == selectedModID) : null);
		if (num != 0L)
		{
			ModWorkshop.OpenItemPage(num);
		}
	}

	private void ConfirmUnsubscribe()
	{
		LoadedMod mod = ((selectedModID != null) ? ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == selectedModID) : null);
		if (SubscriptionId(mod) == 0L || !ModWorkshop.IsAvailable)
		{
			return;
		}
		string body = (IsWorkshopMod(mod) ? LocHelper.GetLocalizedUI("ui_mods_unsubscribe_confirm", ModLoader.GetModLabel(mod.Manifest)) : LocHelper.GetLocalizedUI("ui_mods_unsubscribe_shadowed_confirm", ModLoader.GetModLabel(mod.Manifest)));
		if (MainMenuUIController.Instance != null && MainMenuUIController.Instance.confirmPanelUIManager != null)
		{
			MainMenuUIController.Instance.confirmPanelUIManager.OpenPanel(body, delegate
			{
				UnsubscribeMod(mod);
			});
		}
		else
		{
			UnsubscribeMod(mod);
		}
	}

	private void UnsubscribeMod(LoadedMod mod)
	{
		string modID = mod.Manifest.ID;
		if (!IsWorkshopMod(mod))
		{
			ModWorkshop.Unsubscribe(mod.ShadowedWorkshopId, delegate(bool ok)
			{
				if (ok && !(this == null))
				{
					mod.ShadowedWorkshopId = 0uL;
					if (selectedModID == modID)
					{
						ShowMod(mod);
					}
				}
			});
			return;
		}
		ModWorkshop.Unsubscribe(mod.WorkshopId, delegate(bool ok)
		{
			if (ok && !(this == null))
			{
				ModLoader.ForgetMod(modID);
				Row row = FindRow(modID);
				if (row != null)
				{
					if (row.element != null)
					{
						UnityEngine.Object.Destroy(row.element.gameObject);
					}
					rows.Remove(row);
				}
				if (selectedModID == modID)
				{
					SelectMod(null);
				}
				RefreshRowIssues();
				RefreshNotes();
			}
		});
	}

	private void OnWorkshopItemInstalled(ulong publishedFileId)
	{
		RefreshNotes();
	}

	private void OnDestroy()
	{
		ModWorkshop.OnItemInstalled -= OnWorkshopItemInstalled;
		ModWorkshop.OnDetailsUpdated -= OnWorkshopDetailsUpdated;
		ModWorkshop.OnDownloadsChanged -= RefreshNotes;
		ModLoader.OnTrustRefreshed -= OnTrustListChanged;
		if (Instance == this)
		{
			Instance = null;
		}
	}

	private void RefreshNotes()
	{
		if (handle != null)
		{
			handle.Get("restart_note")?.SetVisible(ModLoader.RestartRequired);
			int pendingDownloads = ModWorkshop.PendingDownloads;
			CustomUIElement customUIElement = handle.Get("downloads_note");
			customUIElement?.SetText((pendingDownloads > 0) ? LocHelper.GetLocalizedUI("ui_mods_downloading", pendingDownloads) : "");
			customUIElement?.SetVisible(pendingDownloads > 0);
			string text = null;
			if (ModTrustList.KillSwitchActive)
			{
				text = LocHelper.GetLocalizedUIOrDefault("ui_mods_killswitch", "Mod loading is paused by the developers. {0}", ModTrustList.KillSwitchMessage);
			}
			else if (ModTrustList.IsUnsignedDevList)
			{
				text = "Unsigned test trust list loaded (devMode, this session only)";
			}
			else if (ModTrustList.IsStale)
			{
				text = LocHelper.GetLocalizedUIOrDefault("ui_mods_trust_list_stale", "The mod safety list could not be refreshed recently; newly blocked mods may not be known yet.");
			}
			CustomUIElement customUIElement2 = handle.Get("trust_note");
			customUIElement2?.SetText(text ?? "");
			customUIElement2?.SetVisible(text != null);
		}
	}

	public void OpenModFolder()
	{
		string fullPath = Path.GetFullPath(ModLoader.ModsPath);
		try
		{
			System.IO.Directory.CreateDirectory(fullPath);
			Process.Start(fullPath);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[ModUI] Could not open the Mods folder '" + fullPath + "': " + ex.Message);
		}
	}
}
