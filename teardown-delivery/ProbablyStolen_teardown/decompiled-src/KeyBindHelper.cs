using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class KeyBindHelper
{
	public class Binding
	{
		public string id;

		public KeyCode defaultKey;

		public Func<string> label;
	}

	public enum CaptureResult
	{
		None,
		Key,
		Cancelled
	}

	public const string ROTATE_CW = "rotate_cw";

	public const string ROTATE_CCW = "rotate_ccw";

	public const string FLIP_ITEM = "flip_item";

	public const string RESET_ORIENTATION = "reset_orientation";

	public const string TRASH_ITEM = "trash_item";

	public const string QUICK_MOVE_INVENTORY = "quick_move_inventory";

	public const string QUICK_MOVE_SHOWCASE = "quick_move_showcase";

	public const string QUICK_MOVE_STORAGE = "quick_move_storage";

	public const string OPEN_OPTIONS = "open_options";

	public const string CONTROL_HINTS = "control_hints";

	public const string ADVANCE_DIALOGUE = "advance_dialogue";

	public const string PIN_MODE = "pin_mode";

	public const string SORT_MENU = "sort_menu";

	public const string CALL_NEXT_CUSTOMER = "call_next_customer";

	public const string MENU_BACK = "menu_back";

	public const string SECONDARY_CLICK = "secondary_click";

	public const string FLIP_ITEM_ALT = "flip_item_alt";

	private const string TOOLBOX_SLOT_PREFIX = "toolbox_slot_";

	private const string SAVE_KEY_PREFIX = "keybind_";

	public static readonly List<Binding> Bindings;

	private static readonly Dictionary<string, KeyCode> bound;

	private static bool loaded;

	private static readonly HashSet<KeyCode> reservedKeys;

	private static KeyCode[] keyboardKeys;

	private static string capturingOptionId;

	private static int captureEndFrame;

	private static readonly Regex pascalCaseSplit;

	public static bool IsCapturing => capturingOptionId != null;

	public static string CapturingOptionId => capturingOptionId;

	public static bool SuppressHotkeys
	{
		get
		{
			if (capturingOptionId == null)
			{
				return captureEndFrame == Time.frameCount;
			}
			return true;
		}
	}

	static KeyBindHelper()
	{
		Bindings = new List<Binding>();
		bound = new Dictionary<string, KeyCode>();
		reservedKeys = new HashSet<KeyCode>
		{
			KeyCode.Escape,
			KeyCode.Mouse0
		};
		captureEndFrame = -1;
		pascalCaseSplit = new Regex("(?<=[a-z])(?=[A-Z0-9])", RegexOptions.Compiled);
		Define("rotate_cw", KeyCode.E, () => LocHelper.GetLocalizedMainMenu("option_key_rotate_cw"));
		Define("rotate_ccw", KeyCode.Q, () => LocHelper.GetLocalizedMainMenu("option_key_rotate_ccw"));
		Define("flip_item", KeyCode.F, () => LocHelper.GetLocalizedMainMenu("option_key_flip_item"));
		Define("flip_item_alt", KeyCode.Mouse2, () => LocHelper.GetLocalizedOrDefault("MainMenu", "option_key_flip_item_alt", "Flip item (alternate key)"));
		Define("reset_orientation", KeyCode.R, () => LocHelper.GetLocalizedMainMenu("option_key_reset_orientation"));
		Define("secondary_click", KeyCode.Mouse1, () => LocHelper.GetLocalizedOrDefault("MainMenu", "option_key_secondary_click", "Item menu, cancel (right click)"));
		Define("trash_item", KeyCode.Delete, () => LocHelper.GetLocalizedMainMenu("option_key_trash_item"));
		Define("sort_menu", KeyCode.S, () => LocHelper.GetLocalizedMainMenu("option_key_sort_menu"));
		Define("quick_move_inventory", KeyCode.LeftControl, () => LocHelper.GetLocalizedMainMenu("option_key_quick_move_inventory"));
		Define("quick_move_showcase", KeyCode.LeftShift, () => LocHelper.GetLocalizedMainMenu("option_key_quick_move_showcase"));
		Define("quick_move_storage", KeyCode.LeftAlt, () => LocHelper.GetLocalizedMainMenu("option_key_quick_move_storage"));
		Define("pin_mode", KeyCode.P, () => LocHelper.GetLocalizedMainMenu("option_key_pin_mode"));
		Define("open_options", KeyCode.K, () => LocHelper.GetLocalizedMainMenu("option_key_open_options"));
		Define("menu_back", KeyCode.None, () => LocHelper.GetLocalizedOrDefault("MainMenu", "option_key_menu_back", "Escape (alternate key)"));
		Define("control_hints", KeyCode.O, () => LocHelper.GetLocalizedMainMenu("option_key_control_hints"));
		Define("advance_dialogue", KeyCode.Space, () => LocHelper.GetLocalizedMainMenu("option_key_advance_dialogue"));
		Define("call_next_customer", KeyCode.B, () => LocHelper.GetLocalizedOrDefault("MainMenu", "option_key_call_next_customer", "Call next customer (Bell)"));
		for (int i = 0; i < 9; i++)
		{
			int slotNumber = i + 1;
			Define(ToolboxSlot(i), (KeyCode)(48 + slotNumber), () => LocHelper.GetLocalizedMainMenu("option_key_toolbox_slot", slotNumber));
		}
	}

	public static string ToolboxSlot(int slotIndex)
	{
		return "toolbox_slot_" + (slotIndex + 1);
	}

	private static void Define(string id, KeyCode defaultKey, Func<string> label)
	{
		Bindings.Add(new Binding
		{
			id = id,
			defaultKey = defaultKey,
			label = label
		});
	}

	public static Binding Find(string id)
	{
		return Bindings.Find((Binding b) => b.id == id);
	}

	public static KeyCode Get(string id)
	{
		EnsureLoaded();
		if (!bound.TryGetValue(id, out var value))
		{
			return GetDefault(id);
		}
		return value;
	}

	public static KeyCode GetDefault(string id)
	{
		return Find(id)?.defaultKey ?? KeyCode.None;
	}

	public static void Set(string id, KeyCode key)
	{
		EnsureLoaded();
		bound[id] = key;
	}

	public static void ResetToDefaults()
	{
		EnsureLoaded();
		foreach (Binding binding in Bindings)
		{
			bound[binding.id] = binding.defaultKey;
		}
	}

	public static bool GetKeyDown(string id)
	{
		if (SuppressHotkeys)
		{
			return false;
		}
		KeyCode keyCode = Get(id);
		if (keyCode != 0)
		{
			return Input.GetKeyDown(keyCode);
		}
		return false;
	}

	public static bool GetKey(string id)
	{
		if (SuppressHotkeys)
		{
			return false;
		}
		KeyCode keyCode = Get(id);
		if (keyCode != 0)
		{
			return Input.GetKey(keyCode);
		}
		return false;
	}

	public static bool GetBackDown()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			return true;
		}
		if (StoreUIManager.IsTextFieldFocused())
		{
			return false;
		}
		return GetKeyDown("menu_back");
	}

	private static void EnsureLoaded()
	{
		if (loaded)
		{
			return;
		}
		loaded = true;
		foreach (Binding binding in Bindings)
		{
			string key = "keybind_" + binding.id;
			bound[binding.id] = (ES3.KeyExists(key) ? ((KeyCode)ES3.Load<int>(key)) : binding.defaultKey);
		}
	}

	public static void Save()
	{
		EnsureLoaded();
		foreach (Binding binding in Bindings)
		{
			ES3.Save("keybind_" + binding.id, (int)Get(binding.id));
		}
	}

	public static void StartCapture(string optionId)
	{
		capturingOptionId = optionId;
	}

	public static void StopCapture()
	{
		if (capturingOptionId != null)
		{
			capturingOptionId = null;
			captureEndFrame = Time.frameCount;
		}
	}

	public static CaptureResult PollCapture(out KeyCode key)
	{
		key = KeyCode.None;
		if (capturingOptionId == null || !Input.anyKeyDown)
		{
			return CaptureResult.None;
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			return CaptureResult.Cancelled;
		}
		KeyCode[] array = CaptureKeys();
		foreach (KeyCode keyCode in array)
		{
			if (Input.GetKeyDown(keyCode))
			{
				key = keyCode;
				return CaptureResult.Key;
			}
		}
		return CaptureResult.None;
	}

	public static bool IsReserved(KeyCode key)
	{
		return reservedKeys.Contains(key);
	}

	private static KeyCode[] CaptureKeys()
	{
		if (keyboardKeys != null)
		{
			return keyboardKeys;
		}
		keyboardKeys = (from KeyCode k in Enum.GetValues(typeof(KeyCode))
			where k != 0 && (k < KeyCode.Mouse0 || (k > KeyCode.Mouse0 && k <= KeyCode.Mouse6))
			select k).Distinct().ToArray();
		return keyboardKeys;
	}

	public static string KeyName(KeyCode key)
	{
		switch (key)
		{
		case KeyCode.None:
			return LocHelper.GetLocalizedMainMenu("option_key_none");
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
			return ((int)(key - 48)).ToString();
		default:
			if (key >= KeyCode.Keypad0 && key <= KeyCode.Keypad9)
			{
				return "Numpad " + (int)(key - 256);
			}
			if (key >= KeyCode.Mouse3 && key <= KeyCode.Mouse6)
			{
				return "Mouse " + (int)(key - 323 + 1);
			}
			return key switch
			{
				KeyCode.Mouse0 => "Left Click", 
				KeyCode.Mouse1 => "Right Click", 
				KeyCode.Mouse2 => "Wheel Click", 
				KeyCode.Return => "Enter", 
				KeyCode.LeftControl => "Left Ctrl", 
				KeyCode.RightControl => "Right Ctrl", 
				KeyCode.LeftMeta => "Left Cmd", 
				KeyCode.RightMeta => "Right Cmd", 
				KeyCode.KeypadEnter => "Numpad Enter", 
				KeyCode.KeypadPlus => "Numpad +", 
				KeyCode.KeypadMinus => "Numpad -", 
				KeyCode.KeypadMultiply => "Numpad *", 
				KeyCode.KeypadDivide => "Numpad /", 
				KeyCode.KeypadPeriod => "Numpad .", 
				_ => pascalCaseSplit.Replace(key.ToString(), " "), 
			};
		}
	}
}
