using System;
using System.Collections.Generic;

public class MachineProgressHelper
{
	public const string PROGRESS_TYPE_MACHINE_TAG = "PROGRESS_TYPE_MACHINE_TAG";

	public const string STATE_WORKING = "STATE_WORKING";

	public const string STATE_READY = "STATE_READY";

	public const string MACHINE_STATE_TAG = "MACHINE_STATE_TAG";

	public const string MACHINE_PROGRESS_CURRENT_TAG = "MACHINE_PROGRESS_CURRENT_TAG";

	public const string MACHINE_PROGRESS_TARGET_TAG = "MACHINE_PROGRESS_TARGET_TAG";

	public const string BASE_PROCESSING_SPEED_TAG = "BASE_PROCESSING_SPEED_TAG";

	public const string CURRENT_PROCESSING_SPEED_TAG = "CURRENT_PROCESSING_SPEED_TAG";

	public const string PROGRESS_TYPE_DEFAULT_TOOLTIP = "PROGRESS_TYPE_DEFAULT_TOOLTIP";

	public const string PROGRESS_ITEM_TAG = "PROGRESS_ITEM_TAG";

	public const string PROGRESS_ITEM_TARGET_TAG = "PROGRESS_ITEM_TARGET_TAG";

	public const string PROGRESS_ITEM_TARGET_ITEM_ID_TAG = "PROGRESS_ITEM_TARGET_ITEM_ID_TAG";

	public const string PROGRESS_ITEM_TARGET_ITEM_COUNT_TAG = "PROGRESS_ITEM_TARGET_ITEM_COUNT_TAG";

	public const string PROGRESS_ITEM_SHOW_DEFAULT_TOOLTIP = "PROGRESS_ITEM_SHOW_DEFAULT_TOOLTIP";

	public const string PROGRESS_ITEM_REQUIRED_MACHINE_TAG = "PROGRESS_ITEM_REQUIRED_MACHINE_TAG";

	public const string USE_ADDITIONAL_ITEMS_TAG = "PROGRESS_ITEM_USE_ADDITIONAL_ITEMS";

	public const string REQUIRED_ITEM_ID_PREFIX = "REQUIRED_ITEM_ID_";

	public const string REQUIRED_ITEM_COUNT_PREFIX = "REQUIRED_ITEM_COUNT_";

	public const string REQUIRED_ITEM_TYPE_COUNT_TAG = "REQUIRED_ITEM_TYPE_COUNT_TAG";

	public const string PROGRESS_ITEM_REQUIRED_MACHINE_TAG_COUNT = "PROGRESS_ITEM_REQUIRED_MACHINE_TAG_COUNT";

	public const string PROGRESS_ITEM_REQUIRED_MACHINE_TAG_PREFIX = "PROGRESS_ITEM_REQUIRED_MACHINE_TAG_";

	private static readonly Dictionary<string, Func<string>> machineTagDisplayNames = new Dictionary<string, Func<string>>();

	public static string GetMachineState(GameItem machine)
	{
		return machine.GetTagReadonly("MACHINE_STATE_TAG").GetString();
	}

	public static void InitProgressTypeMachine(GameItem machine, int baseProcessingSpeed, bool useDefaultTooltip = true)
	{
		machine.EnableTag("PROGRESS_TYPE_MACHINE_TAG");
		machine.ModifyTag("MACHINE_STATE_TAG", delegate(TagState state)
		{
			state.Enable().SetString("STATE_READY");
		});
		machine.ModifyTag("MACHINE_PROGRESS_CURRENT_TAG", delegate(TagState progress)
		{
			progress.Enable().SetInt(0);
		});
		if (useDefaultTooltip)
		{
			machine.EnableTag("PROGRESS_TYPE_DEFAULT_TOOLTIP");
		}
		else
		{
			machine.DisableTag("PROGRESS_TYPE_DEFAULT_TOOLTIP");
		}
		machine.ModifyTag("BASE_PROCESSING_SPEED_TAG", delegate(TagState progress)
		{
			progress.Enable().SetInt(baseProcessingSpeed);
		});
		machine.ModifyTag("CURRENT_PROCESSING_SPEED_TAG", delegate(TagState progress)
		{
			progress.Enable().SetInt(baseProcessingSpeed);
		});
	}

	public static void InitProgressSourceItem(GameItem sourceItem, int targetAmount, bool useDefaultTooltip = true)
	{
		sourceItem.EnableTag("PROGRESS_ITEM_TAG");
		sourceItem.ModifyTag("PROGRESS_ITEM_TARGET_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(targetAmount);
		});
		if (useDefaultTooltip)
		{
			sourceItem.EnableTag("PROGRESS_ITEM_SHOW_DEFAULT_TOOLTIP");
		}
		else
		{
			sourceItem.DisableTag("PROGRESS_ITEM_SHOW_DEFAULT_TOOLTIP");
		}
	}

	public static void InitProgressSourceItem(GameItem sourceItem, int targetAmount, string targetItemID, bool useDefaultTooltip = true, int targetItemCount = 1, string requiredMachineTag = null)
	{
		sourceItem.EnableTag("PROGRESS_ITEM_TAG");
		sourceItem.ModifyTag("PROGRESS_ITEM_TARGET_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(targetAmount);
		});
		sourceItem.ModifyTag("PROGRESS_ITEM_TARGET_ITEM_ID_TAG", delegate(TagState state)
		{
			state.Enable().SetString(targetItemID);
		});
		sourceItem.ModifyTag("PROGRESS_ITEM_TARGET_ITEM_COUNT_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(targetItemCount);
		});
		if (requiredMachineTag != null)
		{
			sourceItem.ModifyTag("PROGRESS_ITEM_REQUIRED_MACHINE_TAG", delegate(TagState tag)
			{
				tag.Enable().SetString(requiredMachineTag);
			});
		}
		if (useDefaultTooltip)
		{
			sourceItem.EnableTag("PROGRESS_ITEM_SHOW_DEFAULT_TOOLTIP");
		}
		else
		{
			sourceItem.DisableTag("PROGRESS_ITEM_SHOW_DEFAULT_TOOLTIP");
		}
	}

	public static void AddRequiredMachineTag(GameItem sourceItem, string machineTag)
	{
		if (sourceItem != null && !string.IsNullOrEmpty(machineTag))
		{
			int count = (sourceItem.IsTag("PROGRESS_ITEM_REQUIRED_MACHINE_TAG_COUNT") ? sourceItem.GetTagReadonly("PROGRESS_ITEM_REQUIRED_MACHINE_TAG_COUNT").GetInt() : 0);
			sourceItem.ModifyTag("PROGRESS_ITEM_REQUIRED_MACHINE_TAG_" + count, delegate(TagState tag)
			{
				tag.Enable().SetString(machineTag);
			});
			sourceItem.ModifyTag("PROGRESS_ITEM_REQUIRED_MACHINE_TAG_COUNT", delegate(TagState tag)
			{
				tag.Enable().SetInt(count + 1);
			});
		}
	}

	public static List<GameItem> GetFinishedItem(GameItem blueprintItem)
	{
		List<GameItem> list = new List<GameItem>();
		if (!blueprintItem.IsTag("PROGRESS_ITEM_TAG"))
		{
			Debug.Log("The item is not a progress item");
			return list;
		}
		string @string = blueprintItem.GetTagReadonly("PROGRESS_ITEM_TARGET_ITEM_ID_TAG").GetString();
		int @int = blueprintItem.GetTagReadonly("PROGRESS_ITEM_TARGET_ITEM_COUNT_TAG").GetInt();
		for (int i = 0; i < @int; i++)
		{
			GameItem item = DirectoryMaster.Item(@string);
			list.Add(item);
		}
		return list;
	}

	public static void StartProgressTypeMachine(GameItem machine, GameItem sourceItem)
	{
		machine.ModifyTag("MACHINE_STATE_TAG", delegate(TagState state)
		{
			state.Enable().SetString("STATE_WORKING");
		});
		if (!sourceItem.IsTag("PROGRESS_ITEM_TAG"))
		{
			Debug.Log("The source item is not a progress item");
			return;
		}
		machine.ModifyTag("MACHINE_PROGRESS_CURRENT_TAG", delegate(TagState progress)
		{
			progress.Enable().SetInt(0);
		});
		int targetAmount = sourceItem.GetTagReadonly("PROGRESS_ITEM_TARGET_TAG").GetInt();
		machine.ModifyTag("MACHINE_PROGRESS_TARGET_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(targetAmount);
		});
	}

	public static void StartProgressTypeMachine(GameItem machine, int targetAmount)
	{
		machine.ModifyTag("MACHINE_STATE_TAG", delegate(TagState state)
		{
			state.Enable().SetString("STATE_WORKING");
		});
		machine.ModifyTag("MACHINE_PROGRESS_CURRENT_TAG", delegate(TagState progress)
		{
			progress.Enable().SetInt(0);
		});
		machine.ModifyTag("MACHINE_PROGRESS_TARGET_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(targetAmount);
		});
	}

	public static void ContinueProgressTypeMachine(GameItem machine)
	{
		int currentProgressSpeed = machine.GetTagReadonly("CURRENT_PROCESSING_SPEED_TAG").GetInt();
		machine.ModifyTag("MACHINE_PROGRESS_CURRENT_TAG", delegate(TagState progress)
		{
			progress.Enable().SetInt(progress.GetInt() + currentProgressSpeed);
		});
	}

	public static void FinishProgressTypeMachine(GameItem machine)
	{
		machine.ModifyTag("MACHINE_STATE_TAG", delegate(TagState state)
		{
			state.Enable().SetString("STATE_READY");
		});
		machine.ModifyTag("MACHINE_PROGRESS_CURRENT_TAG", delegate(TagState progress)
		{
			progress.Enable().SetInt(0);
		});
	}

	public static bool IsProgressTypeMachineFinished(GameItem machine)
	{
		return machine.GetTagReadonly("MACHINE_PROGRESS_CURRENT_TAG").GetInt() >= machine.GetTagReadonly("MACHINE_PROGRESS_TARGET_TAG").GetInt();
	}

	public static (int, int) GetProgress(GameItem machine)
	{
		int @int = machine.GetTagReadonly("MACHINE_PROGRESS_CURRENT_TAG").GetInt();
		int int2 = machine.GetTagReadonly("MACHINE_PROGRESS_TARGET_TAG").GetInt();
		return (@int, int2);
	}

	public static void CreateSourceItemDefaultTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("PROGRESS_ITEM_TAG") && item.IsTag("PROGRESS_ITEM_SHOW_DEFAULT_TOOLTIP"))
		{
			int @int = item.GetTagReadonly("PROGRESS_ITEM_TARGET_TAG").GetInt();
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_machine_required_work") + " " + @int, ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			AddAdditionalItemRequirementsTooltip(builder, item);
		}
		foreach (string requiredMachineTag in GetRequiredMachineTags(item))
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_machine_required") + " " + GetMachineTagDisplayName(requiredMachineTag), ignoreEmpty: true, RenderHandler.ColorPalette.LightYellow);
		}
	}

	public static List<string> GetRequiredMachineTags(GameItem sourceItem)
	{
		List<string> list = new List<string>();
		if (sourceItem == null)
		{
			return list;
		}
		if (sourceItem.IsTag("PROGRESS_ITEM_REQUIRED_MACHINE_TAG"))
		{
			list.Add(sourceItem.GetTagReadonly("PROGRESS_ITEM_REQUIRED_MACHINE_TAG").GetString());
		}
		if (sourceItem.IsTag("PROGRESS_ITEM_REQUIRED_MACHINE_TAG_COUNT"))
		{
			int @int = sourceItem.GetTagReadonly("PROGRESS_ITEM_REQUIRED_MACHINE_TAG_COUNT").GetInt();
			for (int i = 0; i < @int; i++)
			{
				string tagName = "PROGRESS_ITEM_REQUIRED_MACHINE_TAG_" + i;
				if (sourceItem.IsTag(tagName))
				{
					list.Add(sourceItem.GetTagReadonly(tagName).GetString());
				}
			}
		}
		return list;
	}

	public static void SetMachineTagDisplayName(string machineTag, Func<string> displayName)
	{
		if (!string.IsNullOrEmpty(machineTag) && displayName != null)
		{
			machineTagDisplayNames[machineTag] = displayName;
		}
	}

	public static string GetMachineTagDisplayName(string machineTag)
	{
		if (!machineTagDisplayNames.TryGetValue(machineTag, out var value))
		{
			return machineTag;
		}
		return value();
	}

	public static void AddAdditionalItemRequirementsTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!item.IsTag("PROGRESS_ITEM_USE_ADDITIONAL_ITEMS"))
		{
			return;
		}
		int num = (item.IsTag("REQUIRED_ITEM_TYPE_COUNT_TAG") ? item.GetTagReadonly("REQUIRED_ITEM_TYPE_COUNT_TAG").GetInt() : int.MaxValue);
		builder.AddLine(LocHelper.GetLocalizedMechanic("mech_required_items"), ignoreEmpty: true, RenderHandler.ColorPalette.LightYellow);
		for (int i = 0; i < num; i++)
		{
			string tagName = "REQUIRED_ITEM_ID_" + i;
			string tagName2 = "REQUIRED_ITEM_COUNT_" + i;
			if (item.IsTag(tagName) && item.IsTag(tagName2))
			{
				string displayNameFromID = GeneralHelper.GetDisplayNameFromID(item.GetTagReadonly(tagName).GetString());
				int @int = item.GetTagReadonly(tagName2).GetInt();
				builder.AddLine($"-{displayNameFromID} x{@int}", ignoreEmpty: true, RenderHandler.ColorPalette.Yellow);
				continue;
			}
			break;
		}
	}

	public static void CreateMachineTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("PROGRESS_TYPE_MACHINE_TAG") && item.IsTag("PROGRESS_TYPE_DEFAULT_TOOLTIP"))
		{
			if (GetMachineState(item).Equals("STATE_WORKING"))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_status_working"), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
				int @int = item.GetTagReadonly("MACHINE_PROGRESS_CURRENT_TAG").GetInt();
				int int2 = item.GetTagReadonly("MACHINE_PROGRESS_TARGET_TAG").GetInt();
				builder.AddProgressBar((float)@int / (float)int2, 10, RenderHandler.ColorPalette.LightGreen, RenderHandler.ColorPalette.VeryDarkGreen, null, " ");
				builder.AddLine("(" + @int + "/" + (float)int2 + ")", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
			else
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_desequencer_status_ready"), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			}
		}
	}

	public static void SetAdditionalRequiredItems(GameItem sourceItem, List<(string itemID, int count)> requirements)
	{
		if (requirements == null || requirements.Count == 0)
		{
			sourceItem.DisableTag("PROGRESS_ITEM_USE_ADDITIONAL_ITEMS");
			sourceItem.DisableTag("REQUIRED_ITEM_TYPE_COUNT_TAG");
			return;
		}
		sourceItem.EnableTag("PROGRESS_ITEM_USE_ADDITIONAL_ITEMS");
		sourceItem.ModifyTag("REQUIRED_ITEM_TYPE_COUNT_TAG", delegate(TagState tag)
		{
			tag.Enable().SetInt(requirements.Count);
		});
		for (int i = 0; i < requirements.Count; i++)
		{
			(string itemID, int count) req = requirements[i];
			sourceItem.ModifyTag("REQUIRED_ITEM_ID_" + i, delegate(TagState tag)
			{
				tag.Enable().SetString(req.itemID);
			});
			sourceItem.ModifyTag("REQUIRED_ITEM_COUNT_" + i, delegate(TagState tag)
			{
				tag.Enable().SetInt(req.count);
			});
		}
	}

	public static void SetSingleAdditionalRequiredItem(GameItem sourceItem, string itemID, int count)
	{
		if (string.IsNullOrEmpty(itemID) || count <= 0)
		{
			sourceItem.DisableTag("PROGRESS_ITEM_USE_ADDITIONAL_ITEMS");
			sourceItem.DisableTag("REQUIRED_ITEM_TYPE_COUNT_TAG");
			return;
		}
		sourceItem.EnableTag("PROGRESS_ITEM_USE_ADDITIONAL_ITEMS");
		sourceItem.ModifyTag("REQUIRED_ITEM_TYPE_COUNT_TAG", delegate(TagState tag)
		{
			tag.Enable().SetInt(1);
		});
		sourceItem.ModifyTag("REQUIRED_ITEM_ID_0", delegate(TagState tag)
		{
			tag.Enable().SetString(itemID);
		});
		sourceItem.ModifyTag("REQUIRED_ITEM_COUNT_0", delegate(TagState tag)
		{
			tag.Enable().SetInt(count);
		});
	}

	public static void SetDualAdditionalRequiredItem(GameItem sourceItem, (string itemID, int count) first, (string itemID, int count) second)
	{
		bool num = !string.IsNullOrEmpty(first.itemID) && first.count > 0;
		bool flag = !string.IsNullOrEmpty(second.itemID) && second.count > 0;
		if (!num || !flag)
		{
			sourceItem.DisableTag("PROGRESS_ITEM_USE_ADDITIONAL_ITEMS");
			sourceItem.DisableTag("REQUIRED_ITEM_TYPE_COUNT_TAG");
			return;
		}
		sourceItem.EnableTag("PROGRESS_ITEM_USE_ADDITIONAL_ITEMS");
		sourceItem.ModifyTag("REQUIRED_ITEM_TYPE_COUNT_TAG", delegate(TagState tag)
		{
			tag.Enable().SetInt(2);
		});
		sourceItem.ModifyTag("REQUIRED_ITEM_ID_0", delegate(TagState tag)
		{
			tag.Enable().SetString(first.itemID);
		});
		sourceItem.ModifyTag("REQUIRED_ITEM_COUNT_0", delegate(TagState tag)
		{
			tag.Enable().SetInt(first.count);
		});
		sourceItem.ModifyTag("REQUIRED_ITEM_ID_1", delegate(TagState tag)
		{
			tag.Enable().SetString(second.itemID);
		});
		sourceItem.ModifyTag("REQUIRED_ITEM_COUNT_1", delegate(TagState tag)
		{
			tag.Enable().SetInt(second.count);
		});
	}

	public static bool CanStart(GameItem sourceItem, GameItem machine)
	{
		if (sourceItem == null || machine == null)
		{
			return false;
		}
		foreach (string requiredMachineTag in GetRequiredMachineTags(sourceItem))
		{
			if (!machine.IsTag(requiredMachineTag))
			{
				return false;
			}
		}
		return true;
	}

	public static bool HasRequiredAdditionalItems(GameItem sourceItem, List<GameItem> providedItems)
	{
		if (!sourceItem.IsTag("PROGRESS_ITEM_USE_ADDITIONAL_ITEMS"))
		{
			return true;
		}
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		int num = (sourceItem.IsTag("REQUIRED_ITEM_TYPE_COUNT_TAG") ? sourceItem.GetTagReadonly("REQUIRED_ITEM_TYPE_COUNT_TAG").GetInt() : int.MaxValue);
		for (int i = 0; i < num; i++)
		{
			string tagName = "REQUIRED_ITEM_ID_" + i;
			string tagName2 = "REQUIRED_ITEM_COUNT_" + i;
			if (!sourceItem.IsTag(tagName) || !sourceItem.IsTag(tagName2))
			{
				break;
			}
			string @string = sourceItem.GetTagReadonly(tagName).GetString();
			int @int = sourceItem.GetTagReadonly(tagName2).GetInt();
			if (!dictionary.ContainsKey(@string))
			{
				dictionary[@string] = 0;
			}
			dictionary[@string] += @int;
		}
		Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
		foreach (GameItem providedItem in providedItems)
		{
			string identifier = providedItem.identifier;
			if (!dictionary2.ContainsKey(identifier))
			{
				dictionary2[identifier] = 0;
			}
			dictionary2[identifier]++;
		}
		foreach (KeyValuePair<string, int> item in dictionary)
		{
			if (!dictionary2.ContainsKey(item.Key) || dictionary2[item.Key] < item.Value)
			{
				return false;
			}
		}
		return true;
	}

	public static List<GameItem> GetItemsToConsume(GameItem sourceItem, List<GameItem> inventory)
	{
		List<GameItem> list = new List<GameItem>();
		if (!sourceItem.IsTag("PROGRESS_ITEM_USE_ADDITIONAL_ITEMS"))
		{
			return list;
		}
		int num = (sourceItem.IsTag("REQUIRED_ITEM_TYPE_COUNT_TAG") ? sourceItem.GetTagReadonly("REQUIRED_ITEM_TYPE_COUNT_TAG").GetInt() : 0);
		for (int i = 0; i < num; i++)
		{
			string tagName = "REQUIRED_ITEM_ID_" + i;
			string tagName2 = "REQUIRED_ITEM_COUNT_" + i;
			if (!sourceItem.IsTag(tagName) || !sourceItem.IsTag(tagName2))
			{
				continue;
			}
			string @string = sourceItem.GetTagReadonly(tagName).GetString();
			int num2 = sourceItem.GetTagReadonly(tagName2).GetInt();
			int num3 = inventory.Count - 1;
			while (num3 >= 0 && num2 > 0)
			{
				GameItem gameItem = inventory[num3];
				if (gameItem.identifier == @string && !list.Contains(gameItem))
				{
					list.Add(gameItem);
					num2--;
				}
				num3--;
			}
			if (num2 > 0)
			{
				Debug.LogWarning("Not enough of required item '" + @string + "' to consume fully.");
			}
		}
		return list;
	}

	public static void ConsumeRequiredAdditionalItems(GameItem sourceItem, List<GameItem> inventory)
	{
		foreach (GameItem item in GetItemsToConsume(sourceItem, inventory))
		{
			item.Destroy();
		}
	}
}
