using System;
using System.Collections.Generic;

public class LiquidContainerHelper
{
	public const int LIQUID_CONTAINER_CAPACITY = 500;

	public const string LIQUID_CONTAINER_TAG = "LIQUID_CONTAINER_TAG";

	public const string LIQUID_CONTAINER_CURRENT = "LIQUID_CONTAINER_CURRENT";

	public const string LIQUID_CONTAINER_MAX = "LIQUID_CONTAINER_MAX";

	public const string LIQUID_CONTAINER_SIPPING = "LIQUID_CONTAINER_SIPPING";

	public static void InitLiquidContainerItem(GameItem item, int capacity = 500, bool isStartFull = true)
	{
		item.SetGameItemType("WATER");
		item.EnableTag("LIQUID_CONTAINER_TAG");
		item.ModifyTag("LIQUID_CONTAINER_MAX", delegate(TagState state)
		{
			state.Enable().SetInt(capacity);
		});
		if (isStartFull)
		{
			item.ModifyTag("LIQUID_CONTAINER_CURRENT", delegate(TagState state)
			{
				state.Enable().SetInt(capacity);
			});
		}
		else
		{
			item.ModifyTag("LIQUID_CONTAINER_CURRENT", delegate(TagState state)
			{
				state.Enable().SetInt(0);
			});
		}
		item.ModifyTag("LIQUID_CONTAINER_SIPPING", delegate(TagState state)
		{
			state.Enable().SetBool(val: false);
		});
		item.mayToggleSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.canToggleSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.mayActivateSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => CanSplitWater(s);
		item.canActivateSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => CanSplitWater(s);
		item.onToggleSlotItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _)
		{
			bool currentState = !item.GetTagReadonly("LIQUID_CONTAINER_SIPPING").GetBool();
			item.ModifyTag("LIQUID_CONTAINER_SIPPING", delegate(TagState state)
			{
				state.Enable().SetBool(currentState);
			});
		};
		item.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("LIQUID_CONTAINER_TAG")));
		item.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => t.IsTag("LIQUID_CONTAINER_TAG")));
		item.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(item.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (t.IsTag("LIQUID_CONTAINER_TAG"))
			{
				TransferLiquid(t, s);
			}
		});
		item.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		AudioHelper.InitDrinkableAudio(item);
		item.onActivateSlotItemFunc = delegate(GameItem s, GameInventory inv, SlotMarker _)
		{
			SplitWater(s);
		};
	}

	public static void CreateLiquidContainerToolTip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("LIQUID_CONTAINER_TAG"))
		{
			int @int = item.GetTagReadonly("LIQUID_CONTAINER_CURRENT").GetInt();
			int int2 = item.GetTagReadonly("LIQUID_CONTAINER_MAX").GetInt();
			builder.AddLine("Water: ");
			builder.AddProgressBar((float)@int / (float)int2, 10, RenderHandler.ColorPalette.LightCyan, RenderHandler.ColorPalette.VeryDarkCyan, null, " ");
			builder.AddLine("(" + @int + "/" + (float)int2 + ")", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			bool @bool = item.GetTagReadonly("LIQUID_CONTAINER_SIPPING").GetBool();
			RenderHandler.ColorPalette color = RenderHandler.ColorPalette.LightCyan;
			string text = "Auto Drink When Carried: On";
			if (!@bool)
			{
				text = "Auto Drink When Carried: Off";
				color = RenderHandler.ColorPalette.Red;
			}
			builder.AddLine(" ");
			builder.AddLine(text, ignoreEmpty: true, color);
			builder.AddLine(" ");
		}
	}

	public static void AddLiquid(GameItem item, int volume)
	{
		if (item.IsTag("LIQUID_CONTAINER_TAG"))
		{
			int @int = item.GetTagReadonly("LIQUID_CONTAINER_CURRENT").GetInt();
			int int2 = item.GetTagReadonly("LIQUID_CONTAINER_MAX").GetInt();
			int newVolume = @int + volume;
			if (newVolume > int2)
			{
				newVolume = int2;
			}
			item.ModifyTag("LIQUID_CONTAINER_CURRENT", delegate(TagState state)
			{
				state.SetInt(newVolume);
			});
		}
	}

	public static void TransferLiquid(GameItem targetItem, GameItem sourceItem)
	{
		if (!targetItem.IsTag("LIQUID_CONTAINER_TAG") || !sourceItem.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return;
		}
		int source_current_storage = sourceItem.GetTagReadonly("LIQUID_CONTAINER_CURRENT").GetInt();
		sourceItem.GetTagReadonly("LIQUID_CONTAINER_MAX").GetInt();
		int source_can_give_amount = source_current_storage;
		int target_current_storage = targetItem.GetTagReadonly("LIQUID_CONTAINER_CURRENT").GetInt();
		int @int = targetItem.GetTagReadonly("LIQUID_CONTAINER_MAX").GetInt();
		int target_can_receive_amount = @int - target_current_storage;
		if (source_can_give_amount < target_can_receive_amount)
		{
			targetItem.ModifyTag("LIQUID_CONTAINER_CURRENT", delegate(TagState state)
			{
				state.SetInt(source_can_give_amount + target_current_storage);
			});
			sourceItem.ModifyTag("LIQUID_CONTAINER_CURRENT", delegate(TagState state)
			{
				state.SetInt(0);
			});
		}
		else
		{
			targetItem.ModifyTag("LIQUID_CONTAINER_CURRENT", delegate(TagState state)
			{
				state.SetInt(target_can_receive_amount + target_current_storage);
			});
			sourceItem.ModifyTag("LIQUID_CONTAINER_CURRENT", delegate(TagState state)
			{
				state.SetInt(source_current_storage - target_can_receive_amount);
			});
		}
	}

	public static bool CanRemoveVolume(GameItem sourceItem, int volume)
	{
		if (!sourceItem.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return false;
		}
		if (sourceItem.GetTagReadonly("LIQUID_CONTAINER_CURRENT").GetInt() >= volume)
		{
			return true;
		}
		return false;
	}

	public static void RemoveVolume(GameItem sourceItem, int volume)
	{
		int @int = sourceItem.GetTagReadonly("LIQUID_CONTAINER_CURRENT").GetInt();
		int new_volume = @int - volume;
		if (new_volume < 0)
		{
			new_volume = 0;
		}
		sourceItem.ModifyTag("LIQUID_CONTAINER_CURRENT", delegate(TagState state)
		{
			state.SetInt(new_volume);
		});
	}

	public static void DrinkFromContainer(GameItem item, GameCharacterItem GCI)
	{
		int missingHydration = GCI.maxThirst - GCI.currentThirst;
		int current_storage = item.GetTagReadonly("LIQUID_CONTAINER_CURRENT").GetInt();
		if (current_storage < missingHydration)
		{
			missingHydration = current_storage;
		}
		item.ModifyTag("LIQUID_CONTAINER_CURRENT", delegate(TagState state)
		{
			state.SetInt(current_storage - missingHydration);
		});
		GCI.currentThirst += missingHydration;
	}

	private static void AutoSipFromContainer(GameItem item, GameCharacterItem GCI)
	{
		int missingHydration = GCI.maxThirst - GCI.currentThirst;
		int current_storage = item.GetTagReadonly("LIQUID_CONTAINER_CURRENT").GetInt();
		if (current_storage < missingHydration)
		{
			missingHydration = current_storage;
		}
		item.ModifyTag("LIQUID_CONTAINER_CURRENT", delegate(TagState state)
		{
			state.SetInt(current_storage - missingHydration);
		});
		GCI.currentThirst += missingHydration;
	}

	public static void CheckLiquidContainers(GameCharacterItem GCI)
	{
		foreach (GameItem item in GraphUtils.FindChildrenType(GCI, (GameItem item) => item.IsTag("LIQUID_CONTAINER_TAG") && item.GetTagReadonly("LIQUID_CONTAINER_SIPPING").GetBool()))
		{
			if (item != null)
			{
				AutoSipFromContainer(item, GCI);
			}
		}
	}

	public static int GetLiquidContainerValue(GameItem item)
	{
		if (item.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return item.GetTagReadonly("LIQUID_CONTAINER_CURRENT").GetInt() / 100;
		}
		return 0;
	}

	public static void SplitWater(GameItem item)
	{
		int current_storage = item.GetTagReadonly("LIQUID_CONTAINER_CURRENT").GetInt();
		List<GameCharacterItem> currentPartyGCI = MetaHelper.GetCurrentPartyGCI();
		int num = current_storage / currentPartyGCI.Count;
		bool flag = true;
		foreach (GameCharacterItem item2 in currentPartyGCI)
		{
			int num2 = item2.maxThirst - item2.currentThirst;
			if (num > num2)
			{
				flag = false;
			}
		}
		if (flag)
		{
			foreach (GameCharacterItem item3 in currentPartyGCI)
			{
				item3.currentThirst += num;
			}
			item.ModifyTag("LIQUID_CONTAINER_CURRENT", delegate(TagState state)
			{
				state.SetInt(0);
			});
			return;
		}
		int amountDrunk = 0;
		int num3 = 0;
		int num4 = 0;
		foreach (GameCharacterItem item4 in currentPartyGCI)
		{
			num4 = TryDrinkAmount(item4, num);
			amountDrunk = amountDrunk + num - num4;
			num3 += num4;
		}
		while (num3 > 0 && CanAnyoneDrink(currentPartyGCI))
		{
			foreach (GameCharacterItem item5 in currentPartyGCI)
			{
				if (num3 > 0)
				{
					num4 = TryDrinkAmount(item5, 1);
					amountDrunk = amountDrunk + 1 - num4;
					num3 = num3 - 1 + num4;
				}
			}
		}
		item.ModifyTag("LIQUID_CONTAINER_CURRENT", delegate(TagState state)
		{
			state.SetInt(current_storage - amountDrunk);
		});
	}

	public static bool CanSplitWater(GameItem item)
	{
		return true;
	}

	public static int TryDrinkAmount(GameCharacterItem gci, int amount)
	{
		int num = gci.maxThirst - gci.currentThirst;
		if (amount > num)
		{
			int result = amount - num;
			gci.currentThirst = gci.maxThirst;
			return result;
		}
		gci.currentThirst += amount;
		return 0;
	}

	public static bool CanAnyoneDrink(List<GameCharacterItem> party)
	{
		foreach (GameCharacterItem item in party)
		{
			if (item.currentThirst < item.maxThirst)
			{
				return true;
			}
		}
		return false;
	}
}
