using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class HopperHelper
{
	private enum Pick
	{
		None,
		Items
	}

	public const string INTAKE_ID = "intake_hopper";

	public const string OUTLET_ID = "outlet_hopper";

	public const string INTAKE_TAG = "HOPPER_INTAKE_TAG";

	public const string OUTLET_TAG = "HOPPER_OUTLET_TAG";

	public const string LINK_INT = "HOPPER_LINK_INT";

	public const string ORDER_INT = "HOPPER_ORDER_INT";

	public const string FILTER_MODE_INT = "HOPPER_FILTER_MODE_INT";

	public const string FILTER_STRING = "HOPPER_FILTER_STRING";

	public const string KEEP_INT = "HOPPER_KEEP_INT";

	public const string ON_ADD_BOOL = "HOPPER_ON_ADD_BOOL";

	public const string ON_MOVE_BOOL = "HOPPER_ON_MOVE_BOOL";

	public const int FILTER_ALL = 0;

	public const int FILTER_ALLOW = 1;

	public const int FILTER_BLOCK = 2;

	private const int FILTER_MODE_COUNT = 3;

	public const int ORDER_MIN = 1;

	public const int ORDER_MAX = 15;

	public const int ORDER_DEFAULT = 6;

	public const int KEEP_MAX = 20;

	public const int VALUE = 300;

	public const string ATLAS = "Items/items_tool";

	public const string SPRITE_INTAKE = "hopper_in";

	public const string SPRITE_OUTLET = "hopper_out";

	public const string COLOR_INT = "HOPPER_COLOR_INT";

	public const string ATLAS_COLORS = "Items/items_tool3";

	public const int COLOR_COUNT = 8;

	private static readonly Color[] SWATCHES = new Color[9]
	{
		Swatch("767674"),
		Swatch("a55f44"),
		Swatch("96599b"),
		Swatch("748389"),
		Swatch("4c3f47"),
		Swatch("428d87"),
		Swatch("4f9993"),
		Swatch("777d4e"),
		Swatch("c37352")
	};

	private const float SWATCH_SIZE = 28f;

	private const float SWATCH_BORDER = 2f;

	private const float SWATCH_GAP = 8f;

	private static readonly Color SWATCH_SELECTED = MachineryStyle.VALUE_TEXT;

	private static readonly Color SWATCH_IDLE = new Color(1f, 1f, 1f, 0.12f);

	private const string WINDOW_ID = "intake_hopper";

	private const float WINDOW_WIDTH = 700f;

	private const float WINDOW_HEIGHT = 760f;

	private const float LIST_PAD = 4f;

	private const float LIST_HEIGHT = 196f;

	private const int CHIP_COLUMNS = 3;

	private const float CHIP_WIDTH = 200f;

	private const float CHIP_HEIGHT = 92f;

	private const float CHIP_ICON = 40f;

	private const string UNLINK_BUTTON_ID = "unlink";

	private const string ORDER_LABEL_ID = "order";

	private const string KEEP_LABEL_ID = "keep";

	private const string ADD_BUTTON_ID = "add_items";

	private const string PASTE_BUTTON_ID = "paste";

	public const string NO_HOPPER_INVENTORY_TAG = "NO_HOPPER_INVENTORY_TAG";

	private static readonly HashSet<GameItem> pending = new HashSet<GameItem>();

	private static readonly HashSet<(GameItem, GameInventory)> watched = new HashSet<(GameItem, GameInventory)>();

	private static bool batchQueued;

	private static bool batchRunning;

	private const int MAX_PENDING_PER_BATCH = 64;

	private static bool transferring;

	private static bool hasClipboard;

	private static int clipOrder;

	private static int clipKeep;

	private static int clipMode;

	private static bool clipOnAdd;

	private static bool clipOnMove;

	private static List<string> clipList = new List<string>();

	private static Pick picking = Pick.None;

	private static GameItem pickingFor;

	private static int consumedEscapeFrame = -1;

	private static int restoreCloseOnEscapeFrame = -1;

	private static GameItem openItem;

	private static RectTransform listHost;

	private static bool silentRebuild;

	private const string OUTLET_WINDOW_ID = "outlet_hopper";

	private const float OUTLET_WINDOW_HEIGHT = 240f;

	private static GameItem openOutlet;

	private static readonly Dictionary<string, (string name, Sprite sprite)> describeCache = new Dictionary<string, (string, Sprite)>();

	public static bool HasClipboard => hasClipboard;

	public static bool IsBusy => picking != Pick.None;

	private static Color Swatch(string hex)
	{
		if (!ColorUtility.TryParseHtmlString("#" + hex, out var color))
		{
			return Color.white;
		}
		return color;
	}

	private static string ColorSwatchId(int index)
	{
		return "color_" + index;
	}

	private static string FilterToggleId(int mode)
	{
		return "filter_" + mode;
	}

	public static bool IsIntake(GameItem item)
	{
		return item?.IsTag("HOPPER_INTAKE_TAG") ?? false;
	}

	public static bool IsOutlet(GameItem item)
	{
		return item?.IsTag("HOPPER_OUTLET_TAG") ?? false;
	}

	public static bool IsHopper(GameItem item)
	{
		if (!IsIntake(item))
		{
			return IsOutlet(item);
		}
		return true;
	}

	public static bool MayBypassGridRules(GameItem item, GameInventory inv)
	{
		if (IsHopper(item) && GeneralHelper.IsItemOwned(item))
		{
			if (inv != null)
			{
				return !inv.IsTag("NO_HOPPER_INVENTORY_TAG");
			}
			return true;
		}
		return false;
	}

	public static GameItem CreateIntake()
	{
		GameItem gameItem = CreateCommon("hopper_in", LocHelper.GetLocalizedItem("item_intake_hopper_name"), LocHelper.GetLocalizedItem("item_intake_hopper_desc"));
		gameItem.EnableTag("HOPPER_INTAKE_TAG");
		gameItem.ModifyTag("HOPPER_LINK_INT", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("HOPPER_ORDER_INT", delegate(TagState state)
		{
			state.Enable().SetInt(6);
		});
		gameItem.ModifyTag("HOPPER_FILTER_MODE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("HOPPER_FILTER_STRING", delegate(TagState state)
		{
			state.Enable().SetString("");
		});
		gameItem.ModifyTag("HOPPER_KEEP_INT", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		gameItem.ModifyTag("HOPPER_ON_ADD_BOOL", delegate(TagState state)
		{
			state.Enable().SetBool(val: false);
		});
		gameItem.ModifyTag("HOPPER_ON_MOVE_BOOL", delegate(TagState state)
		{
			state.Enable().SetBool(val: false);
		});
		gameItem.mayActivateSlotItemFunc = (GameItem self, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(self);
		gameItem.canActivateSlotItemFunc = (GameItem self, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(self);
		gameItem.onActivateSlotItemFunc = delegate(GameItem self, GameInventory _, SlotMarker _)
		{
			OpenWindow(self);
		};
		gameItem.onSlotAddItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameItem.onSlotAddItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem self, GameInventory inv, SlotMarker _)
		{
			Watch(self, inv);
			if (!SaveManager.IsDecoding && IsOnMove(self))
			{
				Schedule(self);
			}
		});
		return gameItem;
	}

	public static bool IsOnAdd(GameItem intake)
	{
		return intake.GetTagReadonly("HOPPER_ON_ADD_BOOL").GetBool();
	}

	public static bool IsOnMove(GameItem intake)
	{
		return intake.GetTagReadonly("HOPPER_ON_MOVE_BOOL").GetBool();
	}

	private static void Watch(GameItem intake, GameInventory inv)
	{
		if (intake == null || inv == null || watched.Contains((intake, inv)))
		{
			return;
		}
		watched.Add((intake, inv));
		inv.onSlotAddItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(inv.onSlotAddItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem added, GameInventory where, SlotMarker _)
		{
			if (added != null && added != intake && !IsHopper(added) && intake.parentInventory == where && !SaveManager.IsDecoding && IsOnAdd(intake))
			{
				Schedule(intake);
			}
		});
	}

	private static void Schedule(GameItem intake)
	{
		if (!batchRunning && !transferring && intake != null && !IsAway())
		{
			pending.Add(intake);
			if (!batchQueued && !(OverlayHandler.current == null))
			{
				batchQueued = true;
				OverlayHandler.current.StartCoroutine(RunPending());
			}
		}
	}

	private static IEnumerator RunPending()
	{
		yield return null;
		batchQueued = false;
		batchRunning = true;
		try
		{
			int num = 0;
			foreach (GameItem item in pending.Take(64).ToList())
			{
				if ((!(item is GameItemElement gameItemElement) || !gameItemElement.IsDestroyed()) && GeneralHelper.IsItemOwned(item))
				{
					num += RunAndRecord(item);
				}
			}
			if (num > 0)
			{
				PlayTransfer();
			}
		}
		finally
		{
			batchRunning = false;
			pending.Clear();
		}
	}

	public static GameItem CreateOutlet()
	{
		GameItem gameItem = CreateCommon("hopper_out", LocHelper.GetLocalizedItem("item_outlet_hopper_name"), LocHelper.GetLocalizedItem("item_outlet_hopper_desc"));
		gameItem.EnableTag("HOPPER_OUTLET_TAG");
		gameItem.mayActivateSlotItemFunc = (GameItem self, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(self);
		gameItem.canActivateSlotItemFunc = (GameItem self, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(self) && FindIntakesOf(self).Count > 0;
		gameItem.onActivateSlotItemFunc = delegate(GameItem self, GameInventory _, SlotMarker _)
		{
			RunOutlet(self);
		};
		gameItem.EnableTag("CONFIGURE_TAG");
		gameItem.onConfigureSlotItemFunc = delegate(GameItem self, GameInventory _, SlotMarker _)
		{
			OpenOutletWindow(self);
		};
		gameItem.onSlotAddItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameItem.onSlotAddItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem self, GameInventory _, SlotMarker _)
		{
			self.EnableTag("CONFIGURE_TAG");
		});
		return gameItem;
	}

	private static void RunOutlet(GameItem outlet)
	{
		int num = 0;
		foreach (GameItem item in FindIntakesOf(outlet))
		{
			num += RunAndRecord(item);
		}
		if (num > 0)
		{
			PlayTransfer();
		}
	}

	public static int GetColor(GameItem hopper)
	{
		if (hopper == null || !hopper.IsTag("HOPPER_COLOR_INT"))
		{
			return 0;
		}
		return Mathf.Clamp(hopper.GetTagReadonly("HOPPER_COLOR_INT").GetInt(), 0, 8);
	}

	public static void SetColor(GameItem hopper, int index)
	{
		if (IsHopper(hopper))
		{
			index = Mathf.Clamp(index, 0, 8);
			hopper.ModifyTag("HOPPER_COLOR_INT", delegate(TagState state)
			{
				state.Enable().SetInt(index);
			});
			string text = (IsIntake(hopper) ? "hopper_in" : "hopper_out");
			if (index == 0)
			{
				hopper.SetSpriteQuick("Items/items_tool", text);
			}
			else
			{
				hopper.SetSpriteQuick("Items/items_tool3", text + "_" + index);
			}
		}
	}

	private static void AddColorRow(CustomUIBuilder builder, GameItem hopper, Func<CustomUIWindow> window)
	{
		builder.AddLabel(LocHelper.GetLocalizedMechanicOrDefault("mech_hopper_label_color", "Color"));
		builder.BeginRow(8f, TextAnchor.MiddleLeft);
		int color = GetColor(hopper);
		for (int i = 0; i <= 8; i++)
		{
			int index = i;
			builder.AddImage((Sprite)null, 28f, 28f, ColorSwatchId(index));
			CustomUIElement last = builder.Last;
			builder.AddImage((Sprite)null, 24f, 24f, (string)null);
			CustomUIElement last2 = builder.Last;
			if (last?.root == null || last.image == null || last2?.Rect == null)
			{
				continue;
			}
			last2.SetImageColor(SWATCHES[index]);
			MachineStorageCoordinator.NestInPlate(last2, last);
			last2.Rect.anchorMin = Vector2.zero;
			last2.Rect.anchorMax = Vector2.one;
			last2.Rect.offsetMin = new Vector2(2f, 2f);
			last2.Rect.offsetMax = new Vector2(-2f, -2f);
			last.SetImageColor((index == color) ? SWATCH_SELECTED : SWATCH_IDLE);
			last.image.raycastTarget = true;
			Button button = last.root.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.onClick.AddListener(delegate
			{
				PlayClick();
				SetColor(hopper, index);
				CustomUIWindow customUIWindow = window();
				for (int j = 0; j <= 8; j++)
				{
					customUIWindow?.Get(ColorSwatchId(j))?.SetImageColor((j == index) ? SWATCH_SELECTED : SWATCH_IDLE);
				}
			});
		}
		builder.End();
	}

	private static GameItem CreateCommon(string sprite, string name, string description)
	{
		GameItem gameItem = ItemDirectory.CreateEmptyItem().SetSpriteAndShape("Items/items_tool", sprite);
		gameItem.name = name;
		gameItem.shortDescription = description;
		gameItem.unitValue = 300L;
		gameItem.EnableTag("IMPORTANT_TAG");
		AudioHelper.InitSmallMachine(gameItem);
		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		gameItem.canSelectSlotItemFunc = (GameItem self, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(self);
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => CanLinkPair(s, t)));
		gameItem.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => CanLinkPair(s, t)));
		gameItem.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (CanLinkPair(s, t))
			{
				GameItem gameItem2 = (IsIntake(s) ? s : t);
				GameItem outlet = (IsOutlet(s) ? s : t);
				Link(gameItem2, outlet);
				AudioManager.Instance?.Play3("scanner_activate");
				if (openItem == gameItem2)
				{
					RefreshLink();
				}
			}
		});
		gameItem.onSlotAddItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameItem.onSlotAddItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate(GameItem self, GameInventory _, SlotMarker _)
		{
			EnsureLabel(self);
		});
		return gameItem;
	}

	private static void EnsureLabel(GameItem hopper)
	{
		if (hopper != null && !GeneralHelper.HasCustomName(hopper) && GeneralHelper.IsItemOwned(hopper) && PlayerStore.Instance != null)
		{
			PlayerStore.Instance.hopperLabelCounter++;
			GeneralHelper.SetCustomName(hopper, "#" + PlayerStore.Instance.hopperLabelCounter);
		}
	}

	private static string Label(GameItem hopper)
	{
		return hopper.GetDisplayName(preferCustomName: true);
	}

	private static string WindowTitle(GameItem hopper)
	{
		if (!GeneralHelper.HasCustomName(hopper))
		{
			return hopper.name;
		}
		return Label(hopper) + " (" + hopper.name + ")";
	}

	public static bool CanLink(GameItem intake, GameItem outlet)
	{
		if (IsIntake(intake) && IsOutlet(outlet))
		{
			return GeneralHelper.IsItemOwned(intake, outlet);
		}
		return false;
	}

	private static bool CanLinkPair(GameItem a, GameItem b)
	{
		if (a == null || b == null || a == b)
		{
			return false;
		}
		if ((!IsIntake(a) || !IsOutlet(b)) && (!IsOutlet(a) || !IsIntake(b)))
		{
			return false;
		}
		return GeneralHelper.IsItemOwned(a, b);
	}

	public static void Link(GameItem intake, GameItem outlet)
	{
		if (CanLink(intake, outlet))
		{
			int id = outlet.GetUniqueID();
			intake.ModifyTag("HOPPER_LINK_INT", delegate(TagState state)
			{
				state.Enable().SetInt(id);
			});
		}
	}

	public static void Unlink(GameItem intake)
	{
		intake.ModifyTag("HOPPER_LINK_INT", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
	}

	public static GameItem FindOutlet(GameItem intake)
	{
		int @int = intake.GetTagReadonly("HOPPER_LINK_INT").GetInt();
		if (@int == 0)
		{
			return null;
		}
		foreach (GameItem item in PlayerStore.Instance.FindAllItem())
		{
			if (item.uniqueId == @int && IsOutlet(item) && GeneralHelper.IsItemOwned(item))
			{
				return item;
			}
		}
		return null;
	}

	public static List<GameItem> FindIntakesOf(GameItem outlet)
	{
		List<GameItem> list = new List<GameItem>();
		if (outlet == null || outlet.uniqueId == 0)
		{
			return list;
		}
		foreach (GameItem item in PlayerStore.Instance.FindAllItem())
		{
			if (IsIntake(item) && GeneralHelper.IsItemOwned(item) && item.GetTagReadonly("HOPPER_LINK_INT").GetInt() == outlet.uniqueId)
			{
				list.Add(item);
			}
		}
		return list;
	}

	private static string PlaceName(GameItem hopper)
	{
		GameItem gameItem = hopper?.parentInventory?.GetParentItem();
		if (gameItem == null)
		{
			return LocHelper.GetLocalizedMechanic("mech_hopper_place_store");
		}
		return gameItem.GetDisplayName(preferCustomName: true);
	}

	public static int GetOrder(GameItem intake)
	{
		return Mathf.Clamp(intake.GetTagReadonly("HOPPER_ORDER_INT").GetInt(), 1, 15);
	}

	public static int GetKeep(GameItem intake)
	{
		return Mathf.Clamp(intake.GetTagReadonly("HOPPER_KEEP_INT").GetInt(), 0, 20);
	}

	public static int GetFilterMode(GameItem intake)
	{
		return Mathf.Clamp(intake.GetTagReadonly("HOPPER_FILTER_MODE_INT").GetInt(), 0, 2);
	}

	public static int WindowOf(int order)
	{
		if (order > 5)
		{
			if (order > 10)
			{
				return 3;
			}
			return 2;
		}
		return 1;
	}

	private static string WindowName(int order)
	{
		return WindowOf(order) switch
		{
			1 => LocHelper.GetLocalizedMechanic("mech_hopper_window_before_machines"), 
			2 => LocHelper.GetLocalizedMechanic("mech_hopper_window_after_machines"), 
			_ => LocHelper.GetLocalizedMechanic("mech_hopper_window_after_everything"), 
		};
	}

	private static string Unlinked()
	{
		return LocHelper.GetLocalizedMechanic("mech_hopper_unlinked");
	}

	private static string LinkedTo(string labels)
	{
		return LocHelper.GetLocalizedMechanic("mech_hopper_linked_to", labels);
	}

	private static string KeepLine(int keep)
	{
		return LocHelper.GetLocalizedMechanic("mech_hopper_keep", keep);
	}

	private static string OnAddCaption()
	{
		return LocHelper.GetLocalizedMechanic("mech_hopper_toggle_on_add");
	}

	private static string OnMoveCaption()
	{
		return LocHelper.GetLocalizedMechanic("mech_hopper_toggle_on_move");
	}

	private static string PickHint()
	{
		return LocHelper.GetLocalizedMechanic("mech_hopper_pick_hint");
	}

	private static string AddItemsCaption()
	{
		return LocHelper.GetLocalizedMechanic("mech_hopper_btn_add_items");
	}

	private static string FilterCaption(int mode)
	{
		return mode switch
		{
			1 => LocHelper.GetLocalizedMechanic("mech_hopper_filter_allow"), 
			2 => LocHelper.GetLocalizedMechanic("mech_hopper_filter_block"), 
			_ => LocHelper.GetLocalizedMechanic("mech_hopper_filter_all"), 
		};
	}

	public static List<string> GetFilterList(GameItem intake)
	{
		string @string = intake.GetTagReadonly("HOPPER_FILTER_STRING").GetString();
		if (!string.IsNullOrEmpty(@string))
		{
			return (from s in @string.Split(';')
				where s.Length > 0
				select s).ToList();
		}
		return new List<string>();
	}

	public static void SetFilterList(GameItem intake, List<string> identifiers)
	{
		string joined = string.Join(";", identifiers.Distinct());
		intake.ModifyTag("HOPPER_FILTER_STRING", delegate(TagState state)
		{
			state.Enable().SetString(joined);
		});
	}

	private static bool Passes(int mode, HashSet<string> list, GameItem item)
	{
		if (mode == 0)
		{
			return true;
		}
		bool flag = list.Contains(item.identifier);
		if (mode != 1)
		{
			return !flag;
		}
		return flag;
	}

	public static int RunAndRecord(GameItem intake)
	{
		Watch(intake, intake.parentInventory);
		return Run(intake).moved;
	}

	private static bool IsAway()
	{
		if (MapUIManager.Instance != null)
		{
			return MapUIManager.Instance.IsOutside;
		}
		return false;
	}

	private static (int moved, int stuck) Run(GameItem intake)
	{
		if (transferring || IsAway())
		{
			return (moved: 0, stuck: 0);
		}
		transferring = true;
		try
		{
			return RunUnguarded(intake);
		}
		finally
		{
			transferring = false;
		}
	}

	private static (int moved, int stuck) RunUnguarded(GameItem intake)
	{
		if (!IsIntake(intake) || !GeneralHelper.IsItemOwned(intake))
		{
			return (moved: 0, stuck: 0);
		}
		GameInventory parentInventory = intake.parentInventory;
		GameInventory gameInventory = FindOutlet(intake)?.parentInventory;
		if (parentInventory == null || gameInventory == null || parentInventory == gameInventory)
		{
			return (moved: 0, stuck: 0);
		}
		int keep = GetKeep(intake);
		int mode = GetFilterMode(intake);
		HashSet<string> list = new HashSet<string>(GetFilterList(intake));
		int num = 0;
		int num2 = 0;
		foreach (IGrouping<string, GameItem> item in from i in parentInventory.childItems.Where((GameItem i) => i != intake && !IsHopper(i) && GeneralHelper.IsItemOwned(i) && !string.IsNullOrEmpty(i.identifier) && i.MayRemove() && Passes(mode, list, i)).ToList()
			group i by i.identifier)
		{
			int num3 = ((IEnumerable<GameItem>)item).Sum((Func<GameItem, int>)Units) - keep;
			foreach (GameItem item2 in item)
			{
				if (num3 <= 0)
				{
					break;
				}
				int num4 = Mathf.Min(Units(item2), num3);
				if (!GraphUtils.CanAccept(gameInventory, item2))
				{
					num2 += num4;
					continue;
				}
				int b = GraphUtils.TryAcceptAll(gameInventory, item2, num4);
				int num5 = num4 - Mathf.Max(0, b);
				num += num5;
				num2 += num4 - num5;
				num3 -= num5;
			}
		}
		return (moved: num, stuck: num2);
	}

	private static int Units(GameItem item)
	{
		return Mathf.Max(1, item.unitCount);
	}

	public static bool TryRouteOverflow(GameInventory grid, GameItem item)
	{
		if (grid == null || item == null || transferring || IsAway())
		{
			return false;
		}
		transferring = true;
		try
		{
			foreach (GameItem item2 in grid.childItems.Where((GameItem i) => IsIntake(i) && GeneralHelper.IsItemOwned(i)).OrderBy(GetOrder).ThenBy((GameItem i) => i.uniqueId)
				.ToList())
			{
				if (!Passes(GetFilterMode(item2), new HashSet<string>(GetFilterList(item2)), item))
				{
					continue;
				}
				GameInventory gameInventory = FindOutlet(item2)?.parentInventory;
				if (gameInventory != null && gameInventory != grid && GraphUtils.CanAccept(gameInventory, item) && GraphUtils.TryAcceptAll(gameInventory, item) <= 0)
				{
					if (item.parentInventory == null)
					{
						GraphUtils.DestroyIfUnplaced(item);
					}
					return true;
				}
			}
			return false;
		}
		finally
		{
			transferring = false;
		}
	}

	public static void RunWindow(int window)
	{
		foreach (GameItem item in (from i in PlayerStore.Instance.FindAllItem()
			where IsIntake(i) && GeneralHelper.IsItemOwned(i) && WindowOf(GetOrder(i)) == window
			select i).OrderBy(GetOrder).ThenBy((GameItem i) => i.uniqueId).ToList())
		{
			RunAndRecord(item);
		}
	}

	private static void PlayClick()
	{
		AudioManager.Instance?.Play("interface_click");
	}

	private static void PlayTransfer()
	{
		EmporiumAudioPlayer.Instance?.PlayUnMatched();
	}

	public static void CopySettings(GameItem intake)
	{
		clipOrder = GetOrder(intake);
		clipKeep = GetKeep(intake);
		clipMode = GetFilterMode(intake);
		clipList = GetFilterList(intake);
		clipOnAdd = IsOnAdd(intake);
		clipOnMove = IsOnMove(intake);
		hasClipboard = true;
	}

	public static bool PasteSettings(GameItem intake)
	{
		if (!hasClipboard)
		{
			return false;
		}
		intake.ModifyTag("HOPPER_ORDER_INT", delegate(TagState state)
		{
			state.Enable().SetInt(clipOrder);
		});
		intake.ModifyTag("HOPPER_KEEP_INT", delegate(TagState state)
		{
			state.Enable().SetInt(clipKeep);
		});
		intake.ModifyTag("HOPPER_FILTER_MODE_INT", delegate(TagState state)
		{
			state.Enable().SetInt(clipMode);
		});
		SetFilterList(intake, clipList);
		intake.ModifyTag("HOPPER_ON_ADD_BOOL", delegate(TagState state)
		{
			state.Enable().SetBool(clipOnAdd);
		});
		intake.ModifyTag("HOPPER_ON_MOVE_BOOL", delegate(TagState state)
		{
			state.Enable().SetBool(clipOnMove);
		});
		return true;
	}

	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (IsHopper(item))
		{
			EnsureLabel(item);
		}
		if (IsIntake(item))
		{
			builder.AddLine(" ");
			GameItem gameItem = FindOutlet(item);
			if (gameItem == null)
			{
				builder.AddLine(Unlinked(), ignoreEmpty: true, RenderHandler.ColorPalette.LightGray);
			}
			else
			{
				builder.AddLine(LinkedTo(Label(gameItem)), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
			builder.AddLine(OrderText(item), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			builder.AddLine(FilterSummary(item), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			int keep = GetKeep(item);
			if (keep > 0)
			{
				builder.AddLine(KeepLine(keep), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
			if (IsOnAdd(item))
			{
				builder.AddLine(OnAddCaption(), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
			if (IsOnMove(item))
			{
				builder.AddLine(OnMoveCaption(), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
		}
		else if (IsOutlet(item))
		{
			builder.AddLine(" ");
			List<GameItem> list = FindIntakesOf(item);
			if (list.Count == 0)
			{
				builder.AddLine(Unlinked(), ignoreEmpty: true, RenderHandler.ColorPalette.LightGray);
			}
			else
			{
				builder.AddLine(LinkedTo(string.Join(", ", list.Select(Label))), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
		}
	}

	private static string FilterSummary(GameItem intake)
	{
		int filterMode = GetFilterMode(intake);
		int count = GetFilterList(intake).Count;
		return filterMode switch
		{
			0 => LocHelper.GetLocalizedMechanic("mech_hopper_moves_everything"), 
			1 => LocHelper.GetLocalizedMechanic("mech_hopper_moves_only_listed", count), 
			_ => LocHelper.GetLocalizedMechanic("mech_hopper_moves_except_listed", count), 
		};
	}

	public static bool ConsumedEscapeThisFrame()
	{
		return consumedEscapeFrame == Time.frameCount;
	}

	public static void HandleInput()
	{
		RestoreCloseOnEscapeIfDue();
		if (picking == Pick.None)
		{
			return;
		}
		if (pickingFor == null || (pickingFor is GameItemElement gameItemElement && gameItemElement.IsDestroyed()))
		{
			StopPick();
			return;
		}
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
		TooltipManager._instance?.SetAndShowTooltip(message);
		if (Input.GetMouseButtonDown(0) && gameItem != null)
		{
			if (!flag)
			{
				EmporiumAudioPlayer.Instance?.audioNegativeTwoBeep.Play();
				return;
			}
			List<string> filterList = GetFilterList(pickingFor);
			filterList.Add(gameItem.identifier);
			SetFilterList(pickingFor, filterList);
			EmporiumAudioPlayer.Instance?.PlayUnMatched();
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
		if (hit == null)
		{
			message = PickHint();
			return false;
		}
		if (IsHopper(hit) || !GeneralHelper.IsItemOwned(hit) || string.IsNullOrEmpty(hit.identifier))
		{
			message = LocHelper.GetLocalizedMechanic("mech_hopper_pick_cannot", hit.GetDisplayName(preferCustomName: true));
			return false;
		}
		if (GetFilterList(pickingFor).Contains(hit.identifier))
		{
			message = LocHelper.GetLocalizedMechanic("mech_hopper_pick_already", hit.GetDisplayName(preferCustomName: true));
			return false;
		}
		message = LocHelper.GetLocalizedMechanic("mech_hopper_pick_add", hit.GetDisplayName(preferCustomName: true));
		return true;
	}

	private static void StartPick(Pick what)
	{
		if (openItem != null)
		{
			picking = what;
			pickingFor = openItem;
			GuardEscape(active: true);
			RefreshLink();
			RebuildList();
		}
	}

	private static void StopPick()
	{
		if (picking != 0)
		{
			picking = Pick.None;
			pickingFor = null;
			TooltipManager._instance?.HideTooltip();
			if (!(StoreUIManager.Instance != null) || !StoreUIManager.Instance.hoveringStoreUI)
			{
				ItemTooltipHandler.current?.Enable();
			}
			GuardEscape(active: false);
			RefreshLink();
			RebuildList();
		}
	}

	private static void GuardEscape(bool active)
	{
		CustomUIWindow customUIWindow = ((CustomUIManager.Instance != null) ? CustomUIManager.Instance.GetWindow("intake_hopper") : null);
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
		if (picking == Pick.None)
		{
			CustomUIWindow customUIWindow = ((CustomUIManager.Instance != null) ? CustomUIManager.Instance.GetWindow("intake_hopper") : null);
			if (customUIWindow != null)
			{
				customUIWindow.closeOnEscape = true;
			}
		}
	}

	public static void OpenWindow(GameItem intake)
	{
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[Hopper] No CustomUIManager, cannot open the window");
			return;
		}
		CustomUIWindow window = CustomUIManager.Instance.GetWindow("intake_hopper");
		bool flag = window?.IsAlive ?? false;
		if (flag && openItem == intake)
		{
			window.Close();
			return;
		}
		Vector2? vector = (flag ? new Vector2?(window.Rect.anchoredPosition) : null);
		if (flag)
		{
			window.Close();
		}
		openItem = intake;
		listHost = null;
		picking = Pick.None;
		pickingFor = null;
		CustomUIBuilder customUIBuilder = CustomUIManager.Instance.CreateWindow("intake_hopper", WindowTitle(intake), "paper", null, CustomUIManager.Style.Machinery).SetSize(700f, 760f).SetDraggable(draggable: true)
			.SetCloseOnEscape()
			.OnClose(ResetWindowState);
		List<CustomUIElement> list = new List<CustomUIElement>();
		AddColorRow(customUIBuilder, intake, Window);
		customUIBuilder.AddSeparator();
		customUIBuilder.BeginRow(6f, TextAnchor.MiddleLeft);
		customUIBuilder.AddButton(LocHelper.GetLocalizedMechanic("mech_hopper_btn_transfer_now"), delegate
		{
			PlayClick();
			if (RunAndRecord(intake) > 0)
			{
				PlayTransfer();
			}
		});
		list.Add(customUIBuilder.Last);
		customUIBuilder.AddButton(LocHelper.GetLocalizedMechanic("mech_auto_butcher_btn_copy"), delegate
		{
			CopySettings(intake);
			EmporiumAudioPlayer.Instance?.PlayMatched();
			Window()?.Get("paste")?.SetInteractable(interactable: true);
		});
		list.Add(customUIBuilder.Last);
		customUIBuilder.AddButton(LocHelper.GetLocalizedMechanic("mech_auto_butcher_btn_paste"), delegate
		{
			if (PasteSettings(intake))
			{
				EmporiumAudioPlayer.Instance?.PlayUnMatched();
				CustomUIWindow customUIWindow = Window();
				Vector2 anchoredPosition = customUIWindow?.Rect.anchoredPosition ?? Vector2.zero;
				silentRebuild = true;
				customUIWindow?.Close();
				OpenWindow(intake);
				silentRebuild = false;
				CustomUIWindow customUIWindow2 = Window();
				if (customUIWindow2 != null)
				{
					customUIWindow2.Rect.anchoredPosition = anchoredPosition;
				}
			}
		}, "paste");
		customUIBuilder.Last?.SetInteractable(HasClipboard);
		list.Add(customUIBuilder.Last);
		customUIBuilder.AddFlexibleSpace();
		customUIBuilder.AddButton(LocHelper.GetLocalizedMechanic("mech_hopper_btn_unlink"), delegate
		{
			Unlink(intake);
			EmporiumAudioPlayer.Instance?.PlayUnMatched();
			RefreshLink();
		}, "unlink");
		list.Add(customUIBuilder.Last);
		customUIBuilder.End();
		customUIBuilder.AddSeparator();
		customUIBuilder.AddLabel(OrderText(intake), "order");
		customUIBuilder.AddSlider(1f, 15f, GetOrder(intake), delegate(float value)
		{
			int order = Mathf.Clamp(Mathf.RoundToInt(value), 1, 15);
			intake.ModifyTag("HOPPER_ORDER_INT", delegate(TagState state)
			{
				state.Enable().SetInt(order);
			});
			Window()?.Get("order")?.SetText(OrderText(intake));
		}, wholeNumbers: true);
		customUIBuilder.AddLabel(KeepText(intake), "keep");
		customUIBuilder.AddSlider(0f, 20f, GetKeep(intake), delegate(float value)
		{
			int keep = Mathf.Clamp(Mathf.RoundToInt(value), 0, 20);
			intake.ModifyTag("HOPPER_KEEP_INT", delegate(TagState state)
			{
				state.Enable().SetInt(keep);
			});
			Window()?.Get("keep")?.SetText(KeepText(intake));
		}, wholeNumbers: true);
		customUIBuilder.AddToggle(OnAddCaption(), IsOnAdd(intake), delegate(bool on)
		{
			PlayClick();
			intake.ModifyTag("HOPPER_ON_ADD_BOOL", delegate(TagState state)
			{
				state.Enable().SetBool(on);
			});
			if (on)
			{
				Watch(intake, intake.parentInventory);
			}
		});
		customUIBuilder.AddToggle(OnMoveCaption(), IsOnMove(intake), delegate(bool on)
		{
			PlayClick();
			intake.ModifyTag("HOPPER_ON_MOVE_BOOL", delegate(TagState state)
			{
				state.Enable().SetBool(on);
			});
		});
		customUIBuilder.AddSeparator();
		customUIBuilder.BeginRow(6f, TextAnchor.MiddleLeft);
		customUIBuilder.AddLabel(LocHelper.GetLocalizedMechanic("mech_hopper_label_filter"));
		customUIBuilder.AddFlexibleSpace();
		customUIBuilder.AddButton(AddItemsCaption(), delegate
		{
			PlayClick();
			if (picking == Pick.Items)
			{
				StopPick();
			}
			else
			{
				StartPick(Pick.Items);
			}
		}, "add_items").WithTooltip(PickHint());
		list.Add(customUIBuilder.Last);
		customUIBuilder.End();
		int filterMode = GetFilterMode(intake);
		for (int i = 0; i < 3; i++)
		{
			int mode = i;
			customUIBuilder.AddToggle(FilterCaption(mode), filterMode == mode, delegate(bool on)
			{
				if (!on)
				{
					Window()?.Get(FilterToggleId(mode))?.SetBool(value: true);
				}
				else
				{
					PlayClick();
					for (int j = 0; j < 3; j++)
					{
						if (j != mode)
						{
							Window()?.Get(FilterToggleId(j))?.SetBool(value: false);
						}
					}
					intake.ModifyTag("HOPPER_FILTER_MODE_INT", delegate(TagState state)
					{
						state.Enable().SetInt(mode);
					});
					if (mode == 0 && picking == Pick.Items)
					{
						StopPick();
					}
					RefreshLink();
					RebuildList();
				}
			}, FilterToggleId(mode));
		}
		customUIBuilder.BeginScrollFlexible(196f);
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
		SizeButtonsToCloseButton(Window(), list);
		if (!silentRebuild && AudioManager.Instance != null)
		{
			AudioManager.Instance.TryPlayOne(intake.soundContainerOpen);
		}
		RefreshLink();
		RebuildList();
	}

	private static void SizeButtonsToCloseButton(CustomUIWindow window, List<CustomUIElement> buttons)
	{
		RectTransform rectTransform = window?.frame?.closeButton?.transform as RectTransform;
		if (rectTransform == null)
		{
			return;
		}
		float height = rectTransform.rect.height;
		if (height <= 0f)
		{
			return;
		}
		foreach (CustomUIElement button in buttons)
		{
			button?.SetPreferredSize(-1f, height);
		}
	}

	private static CustomUIWindow Window()
	{
		CustomUIWindow customUIWindow = ((CustomUIManager.Instance != null) ? CustomUIManager.Instance.GetWindow("intake_hopper") : null);
		if (customUIWindow == null || !customUIWindow.IsAlive)
		{
			return null;
		}
		return customUIWindow;
	}

	private static CustomUIWindow OutletWindow()
	{
		CustomUIWindow customUIWindow = ((CustomUIManager.Instance != null) ? CustomUIManager.Instance.GetWindow("outlet_hopper") : null);
		if (customUIWindow == null || !customUIWindow.IsAlive)
		{
			return null;
		}
		return customUIWindow;
	}

	public static void OpenOutletWindow(GameItem outlet)
	{
		if (CustomUIManager.Instance == null)
		{
			Debug.LogError("[Hopper] No CustomUIManager, cannot open the outlet window");
			return;
		}
		CustomUIWindow customUIWindow = OutletWindow();
		if (customUIWindow != null && openOutlet == outlet)
		{
			customUIWindow.Close();
			return;
		}
		Vector2? vector = customUIWindow?.Rect.anchoredPosition;
		customUIWindow?.Close();
		openOutlet = outlet;
		CustomUIBuilder customUIBuilder = CustomUIManager.Instance.CreateWindow("outlet_hopper", WindowTitle(outlet), "paper", null, CustomUIManager.Style.Machinery).SetSize(700f, 240f).SetDraggable(draggable: true)
			.SetCloseOnEscape()
			.OnClose(delegate
			{
				if (openOutlet != null && AudioManager.Instance != null)
				{
					AudioManager.Instance.TryPlayOne(openOutlet.soundContainerClose);
				}
				openOutlet = null;
			});
		AddColorRow(customUIBuilder, outlet, OutletWindow);
		customUIBuilder.AddSeparator();
		customUIBuilder.BeginRow(6f, TextAnchor.MiddleLeft);
		customUIBuilder.AddButton(LocHelper.GetLocalizedMechanic("mech_hopper_btn_transfer_now"), delegate
		{
			PlayClick();
			RunOutlet(outlet);
		});
		CustomUIElement last = customUIBuilder.Last;
		last?.SetInteractable(FindIntakesOf(outlet).Count > 0);
		customUIBuilder.End();
		if (vector.HasValue)
		{
			customUIBuilder.SetPosition(vector.Value);
		}
		else
		{
			customUIBuilder.Center();
		}
		CustomUIWindow window = customUIBuilder.Show();
		SizeButtonsToCloseButton(window, new List<CustomUIElement> { last });
		FitWindowHeight(window);
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.TryPlayOne(outlet.soundContainerOpen);
		}
	}

	private static void ResetWindowState()
	{
		if (!silentRebuild && openItem != null && AudioManager.Instance != null)
		{
			AudioManager.Instance.TryPlayOne(openItem.soundContainerClose);
		}
		if (picking != 0)
		{
			picking = Pick.None;
			pickingFor = null;
			TooltipManager._instance?.HideTooltip();
			if (!(StoreUIManager.Instance != null) || !StoreUIManager.Instance.hoveringStoreUI)
			{
				ItemTooltipHandler.current?.Enable();
			}
		}
		openItem = null;
		listHost = null;
		restoreCloseOnEscapeFrame = -1;
	}

	private static string OrderText(GameItem intake)
	{
		int order = GetOrder(intake);
		return LocHelper.GetLocalizedMechanic("mech_hopper_run_order", order, WindowName(order));
	}

	private static string KeepText(GameItem intake)
	{
		int keep = GetKeep(intake);
		if (keep != 0)
		{
			return KeepLine(keep);
		}
		return LocHelper.GetLocalizedMechanic("mech_hopper_keep_none");
	}

	private static void RefreshLink()
	{
		CustomUIWindow customUIWindow = Window();
		if (customUIWindow != null && openItem != null)
		{
			GameItem gameItem = FindOutlet(openItem);
			string text = ((gameItem == null) ? RichTextBuilder.Color(Unlinked(), RenderHandler.ColorPalette.LightRed) : RichTextBuilder.Color(LocHelper.GetLocalizedMechanic("mech_hopper_connected_to", Label(gameItem), PlaceName(gameItem)), RenderHandler.ColorPalette.LightCyan));
			customUIWindow.SetTitle(WindowTitle(openItem) + " · " + text);
			customUIWindow.Get("unlink")?.SetInteractable(gameItem != null);
			customUIWindow.Get("add_items")?.SetText((picking == Pick.Items) ? LocHelper.GetLocalizedUI("ui_storage_coordinator_stop") : AddItemsCaption());
		}
	}

	private static void FitWindowHeight()
	{
		FitWindowHeight(Window());
	}

	private static void FitWindowHeight(CustomUIWindow window)
	{
		RectTransform rectTransform = window?.frame?.contentRoot;
		if (window != null && !(window.Rect == null) && !(rectTransform == null))
		{
			float height = window.Rect.rect.height;
			LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
			float num = height - rectTransform.rect.height;
			float num2 = Mathf.Ceil(LayoutUtility.GetPreferredHeight(rectTransform) + num);
			if (!Mathf.Approximately(num2, height))
			{
				window.SetSize(700f, num2);
				Vector2 anchoredPosition = window.Rect.anchoredPosition;
				anchoredPosition.y += (1f - window.Rect.pivot.y) * (height - num2);
				window.Rect.anchoredPosition = anchoredPosition;
			}
		}
	}

	private static void RebuildList()
	{
		CustomUIWindow customUIWindow = Window();
		if (customUIWindow != null && !(listHost == null) && openItem != null)
		{
			FillList(customUIWindow);
			CustomUIBuilder.RebuildLayout(customUIWindow.Rect);
		}
	}

	private static void FillList(CustomUIWindow window)
	{
		CustomUIBuilder.ClearContainer(listHost);
		CustomUIBuilder customUIBuilder = new CustomUIBuilder(CustomUIManager.Instance, window, listHost);
		int filterMode = GetFilterMode(openItem);
		bool flag = filterMode != 0;
		ScrollRect componentInParent = listHost.GetComponentInParent<ScrollRect>(includeInactive: true);
		if (componentInParent != null)
		{
			componentInParent.gameObject.SetActive(flag);
		}
		window.Get("add_items")?.SetVisible(flag);
		FitWindowHeight();
		if (!flag)
		{
			return;
		}
		List<string> filterList = GetFilterList(openItem);
		if (filterList.Count == 0)
		{
			customUIBuilder.AddLabel((filterMode == 1) ? LocHelper.GetLocalizedMechanic("mech_hopper_list_empty_allow") : LocHelper.GetLocalizedMechanic("mech_hopper_list_empty_block"));
			return;
		}
		customUIBuilder.BeginGrid(3, 200f, 92f);
		foreach (string item in filterList)
		{
			string id = item;
			var (text, sprite) = Describe(id);
			AddChip(customUIBuilder, sprite, text, LocHelper.GetLocalizedMechanic("mech_hopper_chip_remove", text), delegate
			{
				PlayClick();
				List<string> filterList2 = GetFilterList(openItem);
				filterList2.Remove(id);
				SetFilterList(openItem, filterList2);
				RebuildList();
			});
		}
		customUIBuilder.End();
	}

	private static void AddChip(CustomUIBuilder builder, Sprite sprite, string itemName, string tooltip, Action onClick)
	{
		builder.AddImage((Sprite)null, 200f, 92f, (string)null).WithTooltip(tooltip);
		CustomUIElement last = builder.Last;
		builder.AddImage(sprite, 40f, 40f);
		CustomUIElement last2 = builder.Last;
		if (sprite == null)
		{
			last2?.SetImageColor(new Color(1f, 1f, 1f, 0.15f));
		}
		builder.AddLabel(itemName);
		CustomUIElement last3 = builder.Last;
		if (last3?.text != null)
		{
			last3.text.horizontalAlignment = HorizontalAlignmentOptions.Center;
			last3.text.enableWordWrapping = true;
		}
		if (!(last?.root == null))
		{
			MachineStorageCoordinator.SetupChipPlate(last, onClick);
			if (last.image != null)
			{
				last.image.sprite = MachineryStyle.PlateNotched;
				last.image.type = Image.Type.Sliced;
				last.image.pixelsPerUnitMultiplier = 0.5f;
				last.image.preserveAspect = false;
			}
			MachineStorageCoordinator.NestInPlate(last2, last);
			MachineStorageCoordinator.NestInPlate(last3, last);
		}
	}

	private static (string name, Sprite sprite) Describe(string identifier)
	{
		if (describeCache.TryGetValue(identifier, out (string, Sprite) value))
		{
			return value;
		}
		string item = identifier;
		Sprite item2 = null;
		GameItem gameItem = DirectoryMaster.Item(identifier);
		if (gameItem != null)
		{
			item = gameItem.GetDisplayName();
			item2 = (gameItem as GameItemElement)?.ResolveSpriteByName(gameItem.spritePath);
			GraphUtils.DestroyIfUnplaced(gameItem);
		}
		describeCache[identifier] = (item, item2);
		return (name: item, sprite: item2);
	}
}
