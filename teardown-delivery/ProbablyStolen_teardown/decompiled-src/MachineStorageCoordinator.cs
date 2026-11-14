using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MachineStorageCoordinator
{
	[Serializable]
	public class Preset
	{
		public string name = "";

		public int keyCode;

		public List<int> itemIds = new List<int>();

		public List<ItemTabs> itemTabs = new List<ItemTabs>();

		public string GetTabs(int itemId)
		{
			return itemTabs?.Find((ItemTabs t) => t.itemId == itemId)?.tabs;
		}

		public void SetTabs(int itemId, string tabs)
		{
			if (itemTabs == null)
			{
				itemTabs = new List<ItemTabs>();
			}
			itemTabs.RemoveAll((ItemTabs t) => t.itemId == itemId);
			if (!string.IsNullOrEmpty(tabs))
			{
				itemTabs.Add(new ItemTabs
				{
					itemId = itemId,
					tabs = tabs
				});
			}
		}
	}

	[Serializable]
	public class ItemTabs
	{
		public int itemId;

		public string tabs = "";
	}

	[Serializable]
	private class PresetList
	{
		public List<Preset> presets = new List<Preset>();
	}

	private enum StatusKind
	{
		Info,
		Success,
		Failure
	}

	public const string STORAGE_COORDINATOR_TAG = "STORAGE_COORDINATOR_TAG";

	public const string STORAGE_COORDINATOR_PRESETS_STRING = "STORAGE_COORDINATOR_PRESETS_STRING";

	private const string WINDOW_ID = "storage_coordinator";

	private const string STATUS_LABEL_ID = "status";

	private const float WINDOW_WIDTH = 760f;

	private const float WINDOW_HEIGHT = 440f;

	private const float LIST_HEIGHT = 300f;

	private const float NAME_FIELD_WIDTH = 180f;

	private const float ROW_BUTTON_WIDTH_FACTOR = 1.5f;

	private const string NEW_PRESET_BUTTON_ID = "new_preset";

	private const float INPUT_FALLBACK_HEIGHT = 32f;

	private const float STATUS_FALLBACK_HEIGHT = 24f;

	private const float PRESET_SPACING = 14f;

	public const int CHIP_COLUMNS = 4;

	public const float CHIP_WIDTH = 170f;

	public const float CHIP_HEIGHT = 124f;

	private const float ICON_SIZE = 48f;

	private const float CHIP_PADDING = 4f;

	private static readonly Color CHIP_COLOR_NORMAL = new Color(1f, 1f, 1f, 0.1f);

	private static readonly Color CHIP_COLOR_HIGHLIGHTED = new Color(1f, 1f, 1f, 0.28f);

	private static readonly Color CHIP_COLOR_PRESSED = new Color(1f, 1f, 1f, 0.45f);

	private static readonly HashSet<KeyCode> refusedKeys = new HashSet<KeyCode>
	{
		KeyCode.Escape,
		KeyCode.Space,
		KeyCode.Return,
		KeyCode.KeypadEnter,
		KeyCode.Tab,
		KeyCode.Delete,
		KeyCode.O,
		KeyCode.Alpha0,
		KeyCode.Keypad0,
		KeyCode.D,
		KeyCode.G,
		KeyCode.H,
		KeyCode.F9
	};

	private static KeyCode[] keyboardKeys;

	private static Preset pickingPreset;

	private static Preset capturingPreset;

	private static int consumedEscapeFrame = -1;

	private static int restoreCloseOnEscapeFrame = -1;

	private static GameItem openItem;

	private static List<Preset> presets;

	private static RectTransform listHost;

	public static bool IsBusy
	{
		get
		{
			if (pickingPreset == null)
			{
				return capturingPreset != null;
			}
			return true;
		}
	}

	public static GameItem StorageCoordinator()
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetSpriteAndShape("Items/items_tool", "storage_coordinator");
		gameItem.name = LocHelper.GetLocalizedItem("item_storage_coordinator_name");
		gameItem.shortDescription = LocHelper.GetLocalizedItem("item_storage_coordinator_desc");
		gameItem.unitValue = 400L;
		gameItem.EnableTag("STORAGE_COORDINATOR_TAG");
		gameItem.EnableTag("IMPORTANT_TAG");
		gameItem.SetGameItemType("MACHINE");
		gameItem.mayInventoryAddItemFunc = delegate(GameItem self, GameInventory inv)
		{
			for (GameItem gameItem2 = inv?.GetParentItem(); gameItem2 != null; gameItem2 = gameItem2.parentInventory?.GetParentItem())
			{
				if (gameItem2.IsTag("BACKPACK_TAG"))
				{
					return false;
				}
			}
			return true;
		};
		gameItem.mayActivateSlotItemFunc = (GameItem self, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(self);
		gameItem.canActivateSlotItemFunc = (GameItem self, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(self);
		gameItem.onActivateSlotItemFunc = delegate(GameItem self, GameInventory _, SlotMarker _)
		{
			OpenWindow(self);
		};
		AudioHelper.InitSmallMachine(gameItem);
		return gameItem;
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!item.IsTag("STORAGE_COORDINATOR_TAG"))
		{
			return;
		}
		List<Preset> list = LoadPresets(item);
		if (list.Count == 0)
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_storage_coordinator_no_presets"), ignoreEmpty: true, RenderHandler.ColorPalette.LightGray);
		}
		foreach (Preset item2 in list)
		{
			string text = ((item2.itemIds.Count == 1) ? LocHelper.GetLocalizedMechanic("mech_storage_coordinator_preset_line_one", item2.name, KeyName(item2.keyCode)) : LocHelper.GetLocalizedMechanic("mech_storage_coordinator_preset_line", item2.name, KeyName(item2.keyCode), item2.itemIds.Count));
			builder.AddLine(text, ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		builder.AddEmptyLine(14);
	}

	public static List<Preset> LoadPresets(GameItem item)
	{
		string @string = item.GetTagReadonly("STORAGE_COORDINATOR_PRESETS_STRING").GetString();
		if (string.IsNullOrEmpty(@string))
		{
			return new List<Preset>();
		}
		try
		{
			return JsonUtility.FromJson<PresetList>(@string)?.presets ?? new List<Preset>();
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[StorageCoordinator] Could not read presets, starting empty: " + ex.Message);
			return new List<Preset>();
		}
	}

	public static void SavePresets(GameItem item, List<Preset> presets)
	{
		string json = JsonUtility.ToJson(new PresetList
		{
			presets = presets
		});
		item.ModifyTag("STORAGE_COORDINATOR_PRESETS_STRING", delegate(TagState state)
		{
			state.Enable().SetString(json);
		});
	}

	private static string KeyName(int keyCode)
	{
		KeyCode keyCode2 = (KeyCode)keyCode;
		switch (keyCode2)
		{
		case KeyCode.None:
			return LocHelper.GetLocalizedMechanic("mech_storage_coordinator_no_key");
		case KeyCode.Alpha0:
		case KeyCode.Alpha1:
		case KeyCode.Alpha2:
		case KeyCode.Alpha3:
		case KeyCode.Alpha4:
		case KeyCode.Alpha5:
		case KeyCode.Alpha6:
		case KeyCode.Alpha7:
		case KeyCode.Alpha8:
		case KeyCode.Alpha9:
			return ((int)(keyCode2 - 48)).ToString();
		default:
			return keyCode2.ToString();
		}
	}

	public static GameItem FindCoordinatorInReach()
	{
		if (MapUIManager.Instance != null && MapUIManager.Instance.isOutside)
		{
			return FindCoordinatorIn(EmporiumEntry.Instance.afterhourWindow);
		}
		foreach (PixelWindow visibleWindow in WindowsHandler.current.visibleWindows)
		{
			GameItem gameItem = FindCoordinatorIn(visibleWindow);
			if (gameItem != null)
			{
				return gameItem;
			}
		}
		return null;
	}

	private static GameItem FindCoordinatorIn(GraphNodeStorage root)
	{
		List<GameItem> list = GraphUtils.FindAllChildrenType(root, (GameItem i) => i.IsTag("STORAGE_COORDINATOR_TAG") && GeneralHelper.IsItemOwned(i));
		if (list.Count <= 0)
		{
			return null;
		}
		return list[0];
	}

	private static bool CanOpenContent(GameItem item)
	{
		if (item != null && item.contentWindow != null && !item.contentWindow.isLocked)
		{
			return GeneralHelper.IsItemOwned(item);
		}
		return false;
	}

	private static Dictionary<int, GameItem> OwnedItemsById()
	{
		Dictionary<int, GameItem> dictionary = new Dictionary<int, GameItem>();
		foreach (GameItem item in PlayerStore.Instance.FindAllItem())
		{
			if (item.uniqueId != 0)
			{
				dictionary[item.uniqueId] = item;
			}
		}
		return dictionary;
	}

	public static void TogglePresets(List<Preset> presetGroup)
	{
		Dictionary<int, GameItem> dictionary = OwnedItemsById();
		List<GameItem> list = new List<GameItem>();
		Dictionary<GameItem, string> dictionary2 = new Dictionary<GameItem, string>();
		foreach (Preset item in presetGroup)
		{
			foreach (int itemId in item.itemIds)
			{
				if (dictionary.TryGetValue(itemId, out var value) && CanOpenContent(value) && !list.Contains(value))
				{
					list.Add(value);
					dictionary2[value] = item.GetTabs(itemId);
				}
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		if (list.Any((GameItem i) => !i.contentWindow.IsVisible()))
		{
			Vector2 vector = Input.mousePosition;
			int num = 0;
			{
				foreach (GameItem item2 in list)
				{
					if (!item2.contentWindow.IsVisible())
					{
						ItemMouseDoubleClickHandler.current.OpenContentAction(item2, vector + new Vector2(24f, -24f) * num);
						item2.ApplyContentTabsSelection(dictionary2[item2]);
						num++;
					}
				}
				return;
			}
		}
		foreach (GameItem item3 in list)
		{
			item3.CloseContentWindow();
		}
		AudioManager.Instance.TryPlayOne(list[0].soundContainerClose, list[0].soundContainerOpen);
	}

	public static bool ConsumedEscapeThisFrame()
	{
		return consumedEscapeFrame == Time.frameCount;
	}

	public static void HandleInput()
	{
		RestoreCloseOnEscapeIfDue();
		if (capturingPreset != null)
		{
			HandleKeyCapture();
		}
		else if (pickingPreset != null)
		{
			HandlePick();
		}
		else
		{
			HandleHotkeys();
		}
	}

	private static void HandleHotkeys()
	{
		if (!Input.anyKeyDown || (DialogUIManager.Instance != null && DialogUIManager.Instance.IsChoice()))
		{
			return;
		}
		GameItem gameItem = FindCoordinatorInReach();
		if (gameItem == null)
		{
			return;
		}
		foreach (IGrouping<int, Preset> item in from p in LoadPresets(gameItem)
			where p.keyCode != 0 && Input.GetKeyDown((KeyCode)p.keyCode)
			group p by p.keyCode)
		{
			TogglePresets(item.ToList());
		}
	}

	private static void HandlePick()
	{
		if (KeyBindHelper.GetBackDown())
		{
			consumedEscapeFrame = Time.frameCount;
			StopPick();
			return;
		}
		if (KeyBindHelper.GetKeyDown("secondary_click"))
		{
			StopPick();
			return;
		}
		ItemTooltipHandler.current?.Disable();
		GameItem gameItem = HoveredStoreItem();
		string message;
		bool flag = PickVerdict(gameItem, out message);
		ShowPickTooltip(message);
		if (Input.GetMouseButtonDown(0) && gameItem != null)
		{
			if (!flag)
			{
				SetStatus(message, StatusKind.Failure);
				PlayRefused();
				return;
			}
			pickingPreset.itemIds.Add(gameItem.GetUniqueID());
			pickingPreset.SetTabs(gameItem.GetUniqueID(), gameItem.GetContentTabsSelection());
			SaveOpenPresets();
			SetStatus(LocHelper.GetLocalizedUI("ui_storage_coordinator_pick_start", pickingPreset.name), StatusKind.Success);
			PlayAccepted();
			RebuildList();
		}
	}

	private static GameItem HoveredStoreItem()
	{
		if (StoreUIManager.Instance != null && StoreUIManager.Instance.hoveringStoreUI)
		{
			return null;
		}
		return RenderHandler.RaycastElement<GameItem>(Input.mousePosition);
	}

	private static bool PickVerdict(GameItem hit, out string message)
	{
		string name = pickingPreset.name;
		if (hit == null)
		{
			message = LocHelper.GetLocalizedUI("ui_storage_coordinator_hint_idle", name);
			return false;
		}
		string displayName = hit.GetDisplayName(preferCustomName: true);
		if (hit == openItem || hit.contentWindow == null || !GeneralHelper.IsItemOwned(hit) || hit.contentWindow.isLocked)
		{
			message = LocHelper.GetLocalizedUI("ui_storage_coordinator_hint_cant_add", displayName, name);
			return false;
		}
		if (pickingPreset.itemIds.Contains(hit.uniqueId))
		{
			message = LocHelper.GetLocalizedUI("ui_storage_coordinator_hint_already", displayName, name);
			return false;
		}
		message = LocHelper.GetLocalizedUI("ui_storage_coordinator_hint_click_add", displayName, name);
		return true;
	}

	private static void ShowPickTooltip(string message)
	{
		TooltipManager instance = TooltipManager._instance;
		if (!(instance == null))
		{
			instance.SetAndShowTooltip(message);
		}
	}

	private static void HidePickTooltip()
	{
		TooltipManager._instance?.HideTooltip();
	}

	private static void HandleKeyCapture()
	{
		if (!Input.anyKeyDown)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			consumedEscapeFrame = Time.frameCount;
			StopCapture();
			return;
		}
		if (KeyBindHelper.GetKeyDown("secondary_click"))
		{
			StopCapture();
			return;
		}
		KeyCode[] array = KeyboardKeys();
		foreach (KeyCode keyCode in array)
		{
			if (Input.GetKeyDown(keyCode))
			{
				if (refusedKeys.Contains(keyCode))
				{
					SetStatus(LocHelper.GetLocalizedUI("ui_storage_coordinator_key_refused", KeyName((int)keyCode)), StatusKind.Failure);
					PlayRefused();
				}
				else
				{
					capturingPreset.keyCode = (int)keyCode;
					SaveOpenPresets();
					PlayAccepted();
					StopCapture();
				}
				break;
			}
		}
	}

	private static KeyCode[] KeyboardKeys()
	{
		if (keyboardKeys != null)
		{
			return keyboardKeys;
		}
		keyboardKeys = (from KeyCode k in Enum.GetValues(typeof(KeyCode))
			where k != 0 && k < KeyCode.Mouse0
			select k).Distinct().ToArray();
		return keyboardKeys;
	}

	private static void StartPick(Preset preset)
	{
		StopCapture();
		pickingPreset = preset;
		GuardEscape(active: true);
		SetStatus(LocHelper.GetLocalizedUI("ui_storage_coordinator_pick_start", preset.name));
		RebuildList();
	}

	private static void StopPick()
	{
		if (pickingPreset != null)
		{
			LeavePickMode();
			GuardEscape(active: false);
			SetStatus("");
			RebuildList();
		}
	}

	private static void LeavePickMode()
	{
		pickingPreset = null;
		HidePickTooltip();
		if (!(StoreUIManager.Instance != null) || !StoreUIManager.Instance.hoveringStoreUI)
		{
			ItemTooltipHandler.current?.Enable();
		}
	}

	private static void StartCapture(Preset preset)
	{
		StopPick();
		capturingPreset = preset;
		GuardEscape(active: true);
		SetStatus(LocHelper.GetLocalizedUI("ui_storage_coordinator_capture_start", preset.name));
		RebuildList();
	}

	private static void StopCapture(bool clearStatus = true)
	{
		if (capturingPreset != null)
		{
			capturingPreset = null;
			GuardEscape(active: false);
			if (clearStatus)
			{
				SetStatus("");
			}
			RebuildList();
		}
	}

	private static void GuardEscape(bool active)
	{
		CustomUIWindow customUIWindow = ((CustomUIManager.Instance != null) ? CustomUIManager.Instance.GetWindow("storage_coordinator") : null);
		if (active)
		{
			if (customUIWindow != null)
			{
				customUIWindow.closeOnEscape = false;
			}
			restoreCloseOnEscapeFrame = -1;
		}
		else
		{
			restoreCloseOnEscapeFrame = Time.frameCount + 1;
		}
	}

	private static void RestoreCloseOnEscapeIfDue()
	{
		if (restoreCloseOnEscapeFrame < 0 || Time.frameCount < restoreCloseOnEscapeFrame)
		{
			return;
		}
		restoreCloseOnEscapeFrame = -1;
		if (pickingPreset == null && capturingPreset == null)
		{
			CustomUIWindow customUIWindow = ((CustomUIManager.Instance != null) ? CustomUIManager.Instance.GetWindow("storage_coordinator") : null);
			if (customUIWindow != null)
			{
				customUIWindow.closeOnEscape = true;
			}
		}
	}

	public static void OpenWindow(GameItem item)
	{
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[StorageCoordinator] No CustomUIManager, cannot open the window");
			return;
		}
		CustomUIWindow window = CustomUIManager.Instance.GetWindow("storage_coordinator");
		bool flag = window?.IsAlive ?? false;
		if (flag && openItem == item)
		{
			window.Close();
			return;
		}
		Vector2? vector = (flag ? new Vector2?(window.Rect.anchoredPosition) : null);
		if (flag)
		{
			window.Close();
		}
		openItem = item;
		presets = LoadPresets(item);
		listHost = null;
		pickingPreset = null;
		capturingPreset = null;
		CustomUIBuilder customUIBuilder = CustomUIManager.Instance.CreateWindow("storage_coordinator", LocHelper.GetLocalizedUI("ui_storage_coordinator_title"), "paper").SetSize(760f, 440f).SetDraggable(draggable: true)
			.SetCloseOnEscape()
			.OnClose(ResetWindowState);
		customUIBuilder.BeginRow(4f, TextAnchor.MiddleLeft);
		customUIBuilder.AddLabel(LocHelper.GetLocalizedItem("item_storage_coordinator_desc"));
		customUIBuilder.AddFlexibleSpace();
		customUIBuilder.AddButton(LocHelper.GetLocalizedUI("ui_storage_coordinator_new_preset"), AddPreset, "new_preset").WithTooltip(LocHelper.GetLocalizedUI("ui_storage_coordinator_new_preset_tooltip"));
		WidenButton(customUIBuilder.Last, 1.4f);
		customUIBuilder.Last?.SetPreferredSize(-1f, 32f);
		customUIBuilder.End();
		customUIBuilder.AddLabel("A", "status");
		CustomUIElement last = customUIBuilder.Last;
		float height = NaturalHeight(last, 24f);
		last?.SetText("");
		PinMinHeight(last, height);
		customUIBuilder.BeginScroll(300f);
		listHost = customUIBuilder.CurrentContainer;
		customUIBuilder.End();
		if (vector.HasValue)
		{
			customUIBuilder.SetPosition(vector.Value);
		}
		else
		{
			customUIBuilder.Center();
		}
		customUIBuilder.Show();
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.TryPlayOne(item.soundContainerOpen);
		}
		RebuildList();
	}

	private static void ResetWindowState()
	{
		if (openItem != null && AudioManager.Instance != null)
		{
			AudioManager.Instance.TryPlayOne(openItem.soundContainerClose);
		}
		if (pickingPreset != null)
		{
			LeavePickMode();
		}
		openItem = null;
		presets = null;
		listHost = null;
		pickingPreset = null;
		capturingPreset = null;
		restoreCloseOnEscapeFrame = -1;
	}

	private static void SaveOpenPresets()
	{
		if (openItem != null && presets != null)
		{
			SavePresets(openItem, presets);
		}
	}

	private static void PlayRefused()
	{
		EmporiumAudioPlayer.Instance?.audioNegativeTwoBeep.Play();
	}

	private static void PlayAccepted()
	{
		EmporiumAudioPlayer.Instance?.PlayUnMatched();
	}

	private static void SetStatus(string text, StatusKind kind = StatusKind.Info)
	{
		CustomUIWindow obj = ((CustomUIManager.Instance != null) ? CustomUIManager.Instance.GetWindow("storage_coordinator") : null);
		switch (kind)
		{
		case StatusKind.Success:
			text = RichTextBuilder.Color(text, RenderHandler.ColorPalette.LightGreen);
			break;
		case StatusKind.Failure:
			text = RichTextBuilder.Color(text, RenderHandler.ColorPalette.LightRed);
			break;
		}
		obj?.Get("status")?.SetText(text);
	}

	private static void AddPreset()
	{
		if (presets != null)
		{
			presets.Add(new Preset
			{
				name = LocHelper.GetLocalizedUI("ui_storage_coordinator_default_name", presets.Count + 1)
			});
			SaveOpenPresets();
			RebuildList();
		}
	}

	private static void RemovePreset(Preset preset)
	{
		if (presets != null)
		{
			if (pickingPreset == preset)
			{
				StopPick();
			}
			if (capturingPreset == preset)
			{
				StopCapture();
			}
			presets.Remove(preset);
			SaveOpenPresets();
			RebuildList();
		}
	}

	private static void RemoveItem(Preset preset, int itemId)
	{
		preset.itemIds.Remove(itemId);
		preset.SetTabs(itemId, null);
		SaveOpenPresets();
		RebuildList();
	}

	private static void RebuildList()
	{
		CustomUIWindow customUIWindow = ((CustomUIManager.Instance != null) ? CustomUIManager.Instance.GetWindow("storage_coordinator") : null);
		if (customUIWindow != null && customUIWindow.IsAlive && !(listHost == null) && presets != null)
		{
			FillList(customUIWindow);
			CustomUIBuilder.RebuildLayout(customUIWindow.Rect);
		}
	}

	private static void FillList(CustomUIWindow window)
	{
		CustomUIBuilder.ClearContainer(listHost);
		CustomUIBuilder customUIBuilder = new CustomUIBuilder(CustomUIManager.Instance, window, listHost);
		if (presets.Count == 0)
		{
			customUIBuilder.AddLabel(LocHelper.GetLocalizedUI("ui_storage_coordinator_empty"));
			return;
		}
		Dictionary<int, GameItem> dictionary = OwnedItemsById();
		bool flag = true;
		foreach (Preset preset in presets)
		{
			Preset current = preset;
			if (!flag)
			{
				customUIBuilder.AddSpace(14f);
			}
			flag = false;
			customUIBuilder.BeginColumn();
			customUIBuilder.BeginRow(6f, TextAnchor.MiddleLeft);
			customUIBuilder.AddInput(LocHelper.GetLocalizedUI("ui_storage_coordinator_name_field"), current.name, delegate(string value)
			{
				current.name = (string.IsNullOrWhiteSpace(value) ? current.name : value.Trim());
				SaveOpenPresets();
			});
			CustomUIElement last = customUIBuilder.Last;
			PinWidth(last, 180f);
			float height = NaturalHeight(last, 32f);
			window.Get("new_preset")?.SetPreferredSize(-1f, height);
			bool flag2 = pickingPreset == current;
			customUIBuilder.AddButton(flag2 ? LocHelper.GetLocalizedUI("ui_storage_coordinator_stop") : LocHelper.GetLocalizedUI("ui_storage_coordinator_add"), delegate
			{
				if (pickingPreset == current)
				{
					StopPick();
				}
				else
				{
					StartPick(current);
				}
			}).WithTooltip(LocHelper.GetLocalizedUI("ui_storage_coordinator_add_tooltip"));
			customUIBuilder.Last?.SetPreferredSize(-1f, height);
			WidenButton(customUIBuilder.Last, 1.5f);
			bool flag3 = capturingPreset == current;
			customUIBuilder.AddButton(flag3 ? LocHelper.GetLocalizedUI("ui_storage_coordinator_press_key") : KeyName(current.keyCode), delegate
			{
				if (capturingPreset == current)
				{
					StopCapture();
				}
				else
				{
					StartCapture(current);
				}
			}).WithTooltip(LocHelper.GetLocalizedUI("ui_storage_coordinator_key_tooltip"));
			customUIBuilder.Last?.SetPreferredSize(-1f, height);
			WidenButton(customUIBuilder.Last, 1.5f);
			customUIBuilder.AddFlexibleSpace();
			customUIBuilder.AddCloseButton(delegate
			{
				RemovePreset(current);
			}).WithTooltip(LocHelper.GetLocalizedUI("ui_storage_coordinator_delete_tooltip"));
			FitHeightKeepAspect(customUIBuilder.Last, height);
			customUIBuilder.End();
			if (current.itemIds.Count > 0)
			{
				customUIBuilder.BeginGrid(4, 170f, 124f);
				foreach (int item in current.itemIds.ToList())
				{
					int itemId = item;
					dictionary.TryGetValue(itemId, out var value2);
					Action onClick = delegate
					{
						RemoveItem(current, itemId);
					};
					Sprite sprite = null;
					string text;
					string localizedUI;
					if (value2 != null)
					{
						sprite = (value2 as GameItemElement)?.ResolveSpriteByName(value2.spritePath);
						text = value2.GetDisplayName(preferCustomName: true);
						localizedUI = LocHelper.GetLocalizedUI("ui_storage_coordinator_chip_tooltip", text);
					}
					else
					{
						text = LocHelper.GetLocalizedUI("ui_storage_coordinator_missing");
						localizedUI = LocHelper.GetLocalizedUI("ui_storage_coordinator_missing_tooltip");
					}
					AddChip(customUIBuilder, sprite, text, localizedUI, onClick);
				}
				customUIBuilder.End();
			}
			customUIBuilder.End();
		}
	}

	public static void AddChip(CustomUIBuilder builder, Sprite sprite, string itemName, string tooltip, Action onClick)
	{
		builder.AddImage((Sprite)null, 170f, 124f, (string)null).WithTooltip(tooltip);
		CustomUIElement last = builder.Last;
		builder.AddImage(sprite, 48f, 48f);
		CustomUIElement last2 = builder.Last;
		if (sprite == null)
		{
			last2?.SetImageColor(new Color(1f, 1f, 1f, 0.15f));
		}
		builder.AddLabel(itemName);
		CustomUIElement last3 = builder.Last;
		if (!(last?.root == null))
		{
			SetupChipPlate(last, onClick);
			NestInPlate(last2, last);
			NestInPlate(last3, last);
		}
	}

	public static void SetupChipPlate(CustomUIElement plate, Action onClick)
	{
		if (plate.image != null)
		{
			plate.image.sprite = null;
			plate.image.color = Color.white;
			plate.image.raycastTarget = true;
		}
		VerticalLayoutGroup verticalLayoutGroup = plate.root.GetComponent<VerticalLayoutGroup>();
		if (verticalLayoutGroup == null)
		{
			verticalLayoutGroup = plate.root.AddComponent<VerticalLayoutGroup>();
		}
		verticalLayoutGroup.padding = new RectOffset(4, 4, 4, 4);
		verticalLayoutGroup.spacing = 2f;
		verticalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
		verticalLayoutGroup.childControlWidth = true;
		verticalLayoutGroup.childControlHeight = true;
		verticalLayoutGroup.childForceExpandWidth = false;
		verticalLayoutGroup.childForceExpandHeight = false;
		Button button = plate.root.GetComponent<Button>();
		if (button == null)
		{
			button = plate.root.AddComponent<Button>();
		}
		button.targetGraphic = plate.image;
		button.transition = Selectable.Transition.ColorTint;
		ColorBlock colors = button.colors;
		colors.normalColor = CHIP_COLOR_NORMAL;
		colors.highlightedColor = CHIP_COLOR_HIGHLIGHTED;
		colors.selectedColor = CHIP_COLOR_NORMAL;
		colors.pressedColor = CHIP_COLOR_PRESSED;
		colors.disabledColor = CHIP_COLOR_NORMAL;
		colors.colorMultiplier = 1f;
		button.colors = colors;
		plate.button = button;
		button.onClick.AddListener(delegate
		{
			onClick();
		});
	}

	public static void NestInPlate(CustomUIElement element, CustomUIElement plate)
	{
		if (!(element?.Rect == null) && !(plate?.root == null))
		{
			element.Rect.SetParent(plate.root.transform, worldPositionStays: false);
			if (element.image != null)
			{
				element.image.raycastTarget = false;
			}
			if (element.text != null)
			{
				element.text.raycastTarget = false;
			}
		}
	}

	private static void PinMinHeight(CustomUIElement element, float height)
	{
		if (!(element?.root == null) && !(height <= 0f))
		{
			LayoutElement layoutElement = element.root.GetComponent<LayoutElement>();
			if (layoutElement == null)
			{
				layoutElement = element.root.AddComponent<LayoutElement>();
			}
			layoutElement.minHeight = height;
		}
	}

	private static void PinWidth(CustomUIElement element, float width)
	{
		if (!(element?.root == null))
		{
			LayoutElement layoutElement = element.root.GetComponent<LayoutElement>();
			if (layoutElement == null)
			{
				layoutElement = element.root.AddComponent<LayoutElement>();
			}
			layoutElement.minWidth = width;
			layoutElement.preferredWidth = width;
			layoutElement.flexibleWidth = 0f;
		}
	}

	private static void FitHeightKeepAspect(CustomUIElement element, float height)
	{
		if (!(element?.Rect == null) && !(height <= 0f))
		{
			Vector2 sizeDelta = element.Rect.sizeDelta;
			float num = ((sizeDelta.y > 0f && sizeDelta.x > 0f) ? (sizeDelta.x / sizeDelta.y) : 1f);
			if (element.image != null)
			{
				element.image.preserveAspect = true;
			}
			element.SetPreferredSize(height * num, height);
		}
	}

	private static float NaturalHeight(CustomUIElement element, float fallback)
	{
		if (element?.Rect == null)
		{
			return fallback;
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(element.Rect);
		float preferredHeight = LayoutUtility.GetPreferredHeight(element.Rect);
		if (!(preferredHeight > 0f))
		{
			return fallback;
		}
		return preferredHeight;
	}

	private static void WidenButton(CustomUIElement element, float factor)
	{
		if (!(element?.Rect == null))
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(element.Rect);
			float num = LayoutUtility.GetPreferredWidth(element.Rect);
			if (num <= 0f)
			{
				num = LayoutUtility.GetMinWidth(element.Rect);
			}
			if (!(num <= 0f))
			{
				element.SetPreferredSize(num * factor, -1f);
			}
		}
	}
}
