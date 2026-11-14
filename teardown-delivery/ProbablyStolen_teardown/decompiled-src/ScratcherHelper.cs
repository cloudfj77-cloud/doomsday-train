using System.Collections.Generic;
using UnityEngine;

public class ScratcherHelper
{
	public const int SCRATCHER_PRICE = 20;

	public const string SCRATCHER_TAG = "SCRATCHER_TAG";

	public const int SYMBOL_COUNT = 5;

	public const string SCRATCHER_SYMBOL = "SCRATCHER_SYMBOL";

	public const string SCRATCHER_STATE = "SCRATCHER_STATE";

	public const string SCRATCHER_NEW_TAG = "SCRATCHER_NEW_TAG";

	public const int NUTRIFRUIT_SYMBOL = 1;

	public const string SCRATCHER_LUCK_INT = "SCRATCHER_LUCK_INT";

	public const int MATCH_2_REWARD = 10;

	public const int MATCH_3_REWARD = 75;

	public const int MATCH_2_SPECIAL_REWARD = 30;

	public const int MATCH_3_SPECIAL_REWARD = 350;

	public static void InitScratcherItem(GameItem gameItem)
	{
		gameItem.EnableTag("SCRATCHER_TAG");
		LotteryHelper.StampSpawnLuck(gameItem);
		SetRandomSymbols(gameItem);
		gameItem.EnableTag("SCRATCHER_NEW_TAG");
		gameItem.SetGameItemType("LOTTERY");
	}

	public static void SetRandomSymbols(GameItem gameItem)
	{
		float luckMult = 1f + (float)LotteryHelper.GetSpawnLuck(gameItem) / 100f;
		int luckDirection;
		int num = RollSymbol(1, luckMult, out luckDirection);
		int luckDirection2;
		int num2 = RollSymbol(num, luckMult, out luckDirection2);
		int luckDirection3;
		int symbol = RollSymbol((num == 1 || num2 == 1) ? 1 : num2, luckMult, out luckDirection3);
		SetSymbol(gameItem, 1, num, luckDirection);
		SetSymbol(gameItem, 2, num2, luckDirection2);
		SetSymbol(gameItem, 3, symbol, luckDirection3);
	}

	private static void SetSymbol(GameItem gameItem, int index, int symbol, int luckDirection)
	{
		gameItem.ModifyTag("SCRATCHER_SYMBOL" + index, delegate(TagState state)
		{
			state.Enable().SetInt(symbol);
		});
		gameItem.ModifyTag("SCRATCHER_STATE" + index, delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		if (luckDirection == 0)
		{
			gameItem.DisableTag("SCRATCHER_LUCK_INT" + index);
			return;
		}
		gameItem.ModifyTag("SCRATCHER_LUCK_INT" + index, delegate(TagState state)
		{
			state.Enable().SetInt(luckDirection);
		});
	}

	private static int RollSymbol(int favored, float luckMult, out int luckDirection)
	{
		float value = Random.value;
		float value2 = Random.value;
		float num = 0.2f;
		float num2 = Mathf.Clamp01(num * luckMult);
		bool flag = value < num;
		bool flag2 = value < num2;
		luckDirection = ((flag != flag2) ? (flag2 ? 1 : (-1)) : 0);
		if (!flag2)
		{
			return OtherSymbol(favored, value2);
		}
		return favored;
	}

	private static int OtherSymbol(int favored, float v)
	{
		int num = Mathf.Min((int)(v * 4f), 3) + 1;
		if (num >= favored)
		{
			num++;
		}
		return num;
	}

	public static bool IsAnySymbolScratched(GameItem gameItem)
	{
		for (int i = 1; i <= 3; i++)
		{
			if (gameItem.GetTagReadonly("SCRATCHER_STATE" + i).GetInt() == 1)
			{
				return true;
			}
		}
		return false;
	}

	public static void TryRerollForBetterLuck(GameItem gameItem)
	{
		if (gameItem.IsTag("SCRATCHER_TAG") && !IsAnySymbolScratched(gameItem))
		{
			int luck = HealthData.GetLuck();
			if (luck > LotteryHelper.GetSpawnLuck(gameItem))
			{
				LotteryHelper.SetSpawnLuck(gameItem, luck);
				SetRandomSymbols(gameItem);
			}
		}
	}

	public static void MarkSymbolScratched(GameItem gameItem, int symbolIndex)
	{
		gameItem.ModifyTag("SCRATCHER_STATE" + symbolIndex, delegate(TagState state)
		{
			state.SetInt(1);
		});
		int reward = UpdateValue(gameItem);
		NotifyLuckOnSymbol(gameItem, symbolIndex);
		if (IsFullyScratched(gameItem))
		{
			LotteryHelper.OnScratcherFinished(gameItem, reward);
		}
	}

	private static void NotifyLuckOnSymbol(GameItem gameItem, int symbolIndex)
	{
		if (gameItem.IsTag("SCRATCHER_LUCK_INT" + symbolIndex))
		{
			int @int = gameItem.GetTagReadonly("SCRATCHER_LUCK_INT" + symbolIndex).GetInt();
			if (@int > 0)
			{
				StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("mech_scratcher_luck_good"));
			}
			else if (@int < 0)
			{
				StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("mech_scratcher_luck_bad"), "sec");
			}
		}
	}

	public static int UpdateValue(GameItem gameItem)
	{
		List<int> list = new List<int>();
		for (int i = 1; i <= 3; i++)
		{
			if (gameItem.GetTagReadonly("SCRATCHER_STATE" + i).GetInt() == 1)
			{
				list.Add(gameItem.GetTagReadonly("SCRATCHER_SYMBOL" + i).GetInt());
			}
		}
		int num = 0;
		if (list.Count >= 2)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			foreach (int item in list)
			{
				if (!dictionary.ContainsKey(item))
				{
					dictionary[item] = 0;
				}
				dictionary[item]++;
			}
			foreach (KeyValuePair<int, int> item2 in dictionary)
			{
				int key = item2.Key;
				int value = item2.Value;
				if (value >= 3)
				{
					num = ((key == 1) ? 350 : 75);
					break;
				}
				if (value == 2)
				{
					num = ((key == 1) ? 30 : 10);
				}
			}
		}
		if (num > 0)
		{
			gameItem.EnableTag("LOTTERY_IS_WINNING_TAG");
		}
		gameItem.SetValue(num);
		AchievementTracker.Instance.CheckLucky(num >= 350);
		return num;
	}

	public static bool IsFullyScratched(GameItem gameItem)
	{
		for (int i = 1; i <= 3; i++)
		{
			if (gameItem.GetTagReadonly("SCRATCHER_STATE" + i).GetInt() != 1)
			{
				return false;
			}
		}
		return true;
	}

	public static bool CanRedeemPrize(GameItem gameItem)
	{
		if (IsFullyScratched(gameItem))
		{
			return LotteryHelper.CanRedeemPrize(gameItem);
		}
		return false;
	}

	public static bool TryRedeemPrize(GameItem gameItem)
	{
		if (!IsFullyScratched(gameItem))
		{
			return false;
		}
		return LotteryHelper.TryRedeemPrize(gameItem);
	}

	public static void CreateScratcherTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("SCRATCHER_TAG"))
		{
			if (CanRedeemPrize(item))
			{
				builder.AddLine(" " + LocHelper.GetLocalizedMechanic("mech_tooltip_lottery_redeem"), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen, bold: true);
			}
			else
			{
				builder.AddLine(" " + LocHelper.GetLocalizedMechanic("mech_container_tooltip"), ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue);
			}
		}
	}
}
