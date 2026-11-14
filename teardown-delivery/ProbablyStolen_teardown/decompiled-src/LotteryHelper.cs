using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LotteryHelper
{
	public const string GAMBLERS_COIN_ID = "gamblers_coin";

	public const string GAMBLERS_COIN_TAG = "GAMBLERS_COIN_TAG";

	public const int GAMBLERS_COIN_LUCK = 5;

	public const float GAMBLERS_COIN_SCRATCH_RADIUS_MULTIPLIER = 1.33f;

	public static float scratchRadiusMultiplier = 1f;

	public const int GAMBLERS_COIN_SCRATCH_COUNT = 10;

	public const string SCRATCHER_WINNINGS_COUNTED_TAG = "SCRATCHER_WINNINGS_COUNTED_TAG";

	public const string LOTTERY_TICKET_TAG = "LOTTERY_TICKET_TAG";

	public const string LOTTERY_PAPER_TAG = "LOTTERY_PAPER_TAG";

	public const string LOTTERY_NUMBER_TAG = "LOTTERY_NUMBER_TAG";

	public const string LOTTERY_EDITION_TAG = "LOTTERY_EDITION_TAG";

	public const string LOTTERY_WINNING_NUMBER_TAG = "LOTTERY_WINNING_NUMBER_TAG";

	public const string LOTTERY_MATCH_COUNT_TAG = "LOTTERY_MATCH_COUNT_TAG";

	public const string LOTTERY_IS_WINNING_TAG = "LOTTERY_IS_WINNING_TAG";

	public const string LOTTERY_SPAWN_LUCK_INT = "LOTTERY_SPAWN_LUCK_INT";

	public const int STARTING_EDITION = 1405;

	public const int LOTTERY_TICKET_PRICE = 25;

	public const int MATCH_6_PRIZE = 1000000;

	public const int MATCH_5_PRIZE = 100000;

	public const int MATCH_4_PRIZE = 2500;

	public const int MATCH_3_PRIZE = 300;

	public const int MATCH_2_PRIZE = 35;

	public const int MAX_ATTEMPTS = 100;

	public const int LOTTERY_CONTENT_UNLOCK_DAY = 14;

	public static bool IsCarryingGamblersCoin()
	{
		if (EmporiumEntry.Instance == null || EmporiumEntry.Instance.invWindow == null)
		{
			return false;
		}
		if (GraphUtils.FindAllChildrenType<GameItem>(EmporiumEntry.Instance.invWindow).Exists(IsOwnedCoin))
		{
			return true;
		}
		if (EmporiumEntry.Instance.dossier != null)
		{
			return GraphUtils.FindAllChildrenType<GameItem>(EmporiumEntry.Instance.dossier).Exists(IsOwnedCoin);
		}
		return false;
	}

	private static bool IsOwnedCoin(GameItem item)
	{
		if (item.identifier == "gamblers_coin")
		{
			return GeneralHelper.IsItemOwned(item);
		}
		return false;
	}

	public static int GetCarriedLuckBonus()
	{
		if (!IsCarryingGamblersCoin())
		{
			return 0;
		}
		return 5;
	}

	public static void UpdateScratchRadius()
	{
		scratchRadiusMultiplier = (IsCarryingGamblersCoin() ? 1.33f : 1f);
	}

	public static void OnScratcherStarted()
	{
		PlayerStore.Instance.scratchersStartedTotal++;
	}

	public static void OnScratcherFinished(GameItem ticket, int reward)
	{
		if (!ticket.IsTag("SCRATCHER_WINNINGS_COUNTED_TAG"))
		{
			ticket.EnableTag("SCRATCHER_WINNINGS_COUNTED_TAG");
			PlayerStore.Instance.scratchersFinishedTotal++;
			PlayerStore.Instance.scratcherWinningsTotal += reward;
		}
	}

	public static void CheckGamblersCoinReward()
	{
		PlayerStore instance = PlayerStore.Instance;
		if (!instance.isGamblersCoinRewardGiven && instance.scratchersStartedTotal >= 10)
		{
			instance.isGamblersCoinRewardGiven = true;
			instance.AddDirectSellingItemToTable(ItemSpawner.Spawn("gamblers_coin"), isOwend: true);
		}
	}

	public static void GiveStartingGamblersCoin()
	{
		GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, DirectoryMaster.Item("gamblers_coin"));
		PlayerStore.Instance.isGamblersCoinRewardGiven = true;
	}

	public static void CreateGamblersCoinTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!(item.identifier != "gamblers_coin"))
		{
			PlayerStore instance = PlayerStore.Instance;
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_gamblers_coin_tickets", instance.scratchersStartedTotal), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			if (instance.scratchersFinishedTotal > 0)
			{
				float num = (float)instance.scratcherWinningsTotal / (float)instance.scratchersFinishedTotal;
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_gamblers_coin_average", num.ToString("0.#")), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
		}
	}

	public static void StampSpawnLuck(GameItem gameItem)
	{
		int luck = HealthData.GetLuck();
		gameItem.ModifyTag("LOTTERY_SPAWN_LUCK_INT", delegate(TagState state)
		{
			state.Enable().SetInt(luck);
		});
	}

	public static bool HasSpawnLuck(GameItem gameItem)
	{
		return gameItem.IsTag("LOTTERY_SPAWN_LUCK_INT");
	}

	public static int GetSpawnLuck(GameItem gameItem)
	{
		if (!HasSpawnLuck(gameItem))
		{
			return 0;
		}
		return gameItem.GetTagReadonly("LOTTERY_SPAWN_LUCK_INT").GetInt();
	}

	public static void SetSpawnLuck(GameItem gameItem, int luck)
	{
		gameItem.ModifyTag("LOTTERY_SPAWN_LUCK_INT", delegate(TagState state)
		{
			state.Enable().SetInt(luck);
		});
	}

	public static void InitLotteryTicket(GameItem gameItem)
	{
		gameItem.EnableTag("LOTTERY_TICKET_TAG");
		StampSpawnLuck(gameItem);
		int edition = GetCurrentEdition();
		gameItem.ModifyTag("LOTTERY_EDITION_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(edition);
		});
		gameItem.SetGameItemType("LOTTERY");
		string newNumber = null;
		int num = 0;
		while (num < 100)
		{
			num++;
			string text = GenerateLotteryTicket();
			if (!IsLotteryNumberTaken(edition, text))
			{
				newNumber = text;
				break;
			}
		}
		if (newNumber == null)
		{
			Debug.LogWarning("Failed to generate a unique lottery number after 100 attempts.");
		}
		gameItem.ModifyTag("LOTTERY_NUMBER_TAG", delegate(TagState state)
		{
			state.Enable().SetString(newNumber);
		});
		TrackLotteryNumber(edition, newNumber);
	}

	public static void InitLotteryTicketDebug(GameItem gameItem, string winningNumber, int match)
	{
		gameItem.EnableTag("LOTTERY_TICKET_TAG");
		int edition = GetCurrentEdition();
		gameItem.ModifyTag("LOTTERY_EDITION_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(edition);
		});
		match = Mathf.Clamp(match, 0, 6);
		int[] array = winningNumber.Split(' ').Select(int.Parse).ToArray();
		List<int> list = Enumerable.Range(1, 49).ToList();
		int[] array2 = array;
		foreach (int item in array2)
		{
			list.Remove(item);
		}
		List<int> list2 = new List<int>();
		if (match > 0)
		{
			list2.AddRange(array.Take(match));
		}
		System.Random random = RNG.random;
		while (list2.Count < 6)
		{
			int item2 = list[random.Next(list.Count)];
			if (!list2.Contains(item2))
			{
				list2.Add(item2);
			}
		}
		list2.Sort();
		string ticketString = string.Join(" ", list2.Select((int n) => n.ToString("D2")));
		gameItem.ModifyTag("LOTTERY_NUMBER_TAG", delegate(TagState state)
		{
			state.Enable().SetString(ticketString);
		});
		gameItem.ModifyTag("LOTTERY_EDITION_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(edition);
		});
		TrackLotteryNumber(edition, ticketString);
		gameItem.ModifyTag("LOTTERY_WINNING_NUMBER_TAG", delegate(TagState state)
		{
			state.Enable().SetString(winningNumber);
		});
	}

	public static void TrackLotteryNumber(int edition, string lotteryNumber)
	{
		if (!PlayerStore.Instance.takenLotteryNumbers.ContainsKey(edition))
		{
			PlayerStore.Instance.takenLotteryNumbers[edition] = new List<string>();
		}
		PlayerStore.Instance.takenLotteryNumbers[edition].Add(lotteryNumber);
	}

	public static bool IsLotteryNumberTaken(int edition, string lotteryNumber)
	{
		if (!PlayerStore.Instance.takenLotteryNumbers.ContainsKey(edition))
		{
			return false;
		}
		return PlayerStore.Instance.takenLotteryNumbers[edition].Contains(lotteryNumber);
	}

	public static string GenerateLotteryTicket()
	{
		int[] array = Enumerable.Range(1, 49).ToArray();
		new System.Random();
		for (int num = array.Length - 1; num > 0; num--)
		{
			int randomIntExculsive = RNG.GetRandomIntExculsive(0, num + 1);
			int num2 = array[num];
			array[num] = array[randomIntExculsive];
			array[randomIntExculsive] = num2;
		}
		int[] source = (from n in array.Take(6)
			orderby n
			select n).ToArray();
		return string.Join(" ", source.Select((int n) => n.ToString("D2")));
	}

	public static void CreateLotteryTicketTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!item.IsTag("LOTTERY_TICKET_TAG"))
		{
			return;
		}
		int @int = item.GetTagReadonly("LOTTERY_EDITION_TAG").GetInt();
		builder.AddLine(LocHelper.GetLocalizedMechanic("mech_lottery_edition_tooltip", @int), ignoreEmpty: true, RenderHandler.ColorPalette.LightYellow);
		string @string = item.GetTagReadonly("LOTTERY_NUMBER_TAG").GetString();
		string string2 = item.GetTagReadonly("LOTTERY_WINNING_NUMBER_TAG").GetString();
		if (string.IsNullOrEmpty(string2))
		{
			builder.AddLine(" " + @string, ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
			return;
		}
		string[] array = @string.Split(' ');
		HashSet<string> hashSet = new HashSet<string>(string2.Split(' '));
		builder.Add(" ");
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			RenderHandler.ColorPalette color = (hashSet.Contains(text) ? RenderHandler.ColorPalette.LightGreen : RenderHandler.ColorPalette.LightRed);
			builder.Add(text, color);
			if (i < array.Length - 1)
			{
				builder.Add(" ");
			}
		}
		builder.AddLine();
	}

	public static void OnDrawWiningNumber()
	{
		int currentEdition = GetCurrentEdition();
		List<GameItem> list = new List<GameItem>();
		foreach (GameItem allOwnedItem in EmporiumEntry.Instance.GetAllOwnedItems())
		{
			if (allOwnedItem.IsTag("LOTTERY_TICKET_TAG") && allOwnedItem.GetTagReadonly("LOTTERY_EDITION_TAG").GetInt() == currentEdition)
			{
				list.Add(allOwnedItem);
			}
		}
		string winningNumber = DrawWinningNumberWithLuck(list);
		PlayerStore.Instance.currentWinningLotteryNumber = winningNumber;
		Debug.Log("Winning Lottery Number: " + winningNumber + " (Edition " + currentEdition + ")");
		foreach (GameItem item in list)
		{
			item.ModifyTag("LOTTERY_WINNING_NUMBER_TAG", delegate(TagState state)
			{
				state.Enable().SetString(winningNumber);
			});
			CheckTicket(item, winningNumber);
		}
		PlayerStore.Instance.currentLotteryEdition++;
		PlayerStore.Instance.takenLotteryNumbers.Clear();
	}

	public static string DrawWinningNumberWithLuck(List<GameItem> tickets)
	{
		string text = GenerateLotteryTicket();
		if (tickets.Count == 0)
		{
			return text;
		}
		int effectiveDrawLuck = GetEffectiveDrawLuck(tickets);
		if (effectiveDrawLuck == 0)
		{
			return text;
		}
		int extraDrawCount = GetExtraDrawCount(effectiveDrawLuck);
		if (extraDrawCount == 0)
		{
			return text;
		}
		int num = GetTotalPrize(tickets, text);
		int num2 = num;
		for (int i = 0; i < extraDrawCount; i++)
		{
			string text2 = GenerateLotteryTicket();
			int totalPrize = GetTotalPrize(tickets, text2);
			bool flag = totalPrize > num;
			bool flag2 = totalPrize < num;
			if ((effectiveDrawLuck > 0 && flag) || (effectiveDrawLuck < 0 && flag2))
			{
				text = text2;
				num = totalPrize;
			}
		}
		if (num > num2)
		{
			PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_lottery_luck_good"), "5c6f46");
		}
		else if (num < num2)
		{
			PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_lottery_luck_bad"), "9c3738");
		}
		Debug.Log("Lottery luck " + effectiveDrawLuck + ": " + extraDrawCount + " extra draw(s), prize " + num2 + " -> " + num);
		return text;
	}

	private static int GetEffectiveDrawLuck(List<GameItem> tickets)
	{
		int num = HealthData.GetLuck();
		foreach (GameItem ticket in tickets)
		{
			if (HasSpawnLuck(ticket))
			{
				num = Mathf.Max(num, GetSpawnLuck(ticket));
			}
		}
		return num;
	}

	private static int GetExtraDrawCount(int luck)
	{
		int num = Mathf.Abs(luck);
		int num2 = num / 100;
		if (RNG.Roll(num % 100))
		{
			num2++;
		}
		return num2;
	}

	private static int GetTotalPrize(List<GameItem> tickets, string winningNumber)
	{
		int num = 0;
		foreach (GameItem ticket in tickets)
		{
			num += GetPrizeForMatches(CountMatches(ticket, winningNumber));
		}
		return num;
	}

	public static int CountMatches(GameItem ticket, string winningNumber)
	{
		int[] source = ticket.GetTagReadonly("LOTTERY_NUMBER_TAG").GetString().Split(' ')
			.Select(int.Parse)
			.ToArray();
		int[] winningNumbers = winningNumber.Split(' ').Select(int.Parse).ToArray();
		return source.Count((int n) => winningNumbers.Contains(n));
	}

	public static int GetPrizeForMatches(int matches)
	{
		return matches switch
		{
			6 => 1000000, 
			5 => 100000, 
			4 => 2500, 
			3 => 300, 
			2 => 35, 
			_ => 0, 
		};
	}

	public static void OnDrawWiningNumberDebug()
	{
		string winningNumber = GenerateLotteryTicket();
		PlayerStore.Instance.currentWinningLotteryNumber = winningNumber;
		for (int i = 0; i <= 6; i++)
		{
			GameItem gameItem = DirectoryMaster.Item("lottery_ticket");
			InitLotteryTicketDebug(gameItem, winningNumber, i);
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.backInvinvElement, gameItem);
		}
		List<GameItem> allOwnedItems = EmporiumEntry.Instance.GetAllOwnedItems();
		int currentEdition = GetCurrentEdition();
		foreach (GameItem item in allOwnedItems)
		{
			if (item.IsTag("LOTTERY_TICKET_TAG") && item.GetTagReadonly("LOTTERY_EDITION_TAG").GetInt() == currentEdition)
			{
				item.ModifyTag("LOTTERY_WINNING_NUMBER_TAG", delegate(TagState state)
				{
					state.Enable().SetString(winningNumber);
				});
				item.GetTagReadonly("LOTTERY_NUMBER_TAG").GetString();
				CheckTicket(item, winningNumber);
			}
		}
		PlayerStore.Instance.currentLotteryEdition++;
		PlayerStore.Instance.takenLotteryNumbers.Clear();
	}

	public static void CheckTicket(GameItem item, string winningNumber)
	{
		if (item.IsTag("LOTTERY_TICKET_TAG"))
		{
			int matches = CountMatches(item, winningNumber);
			item.ModifyTag("LOTTERY_MATCH_COUNT_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(matches);
			});
			int prizeForMatches = GetPrizeForMatches(matches);
			if (prizeForMatches > 0)
			{
				item.SetValue(prizeForMatches);
				item.EnableTag("LOTTERY_IS_WINNING_TAG");
				return;
			}
			item.SetSpriteAndShape("Items/items_amenities", "lottery_old");
			item.name = LocHelper.GetLocalizedItem("item_lottery_ticket_old_name");
			item.SetValue(1L);
			item.RemoveGameItemType("LOTTERY");
		}
	}

	public static bool CanRedeemPrize(GameItem gameItem)
	{
		if (gameItem.IsTag("LOTTERY_IS_WINNING_TAG"))
		{
			return gameItem.unitValue > 0;
		}
		return false;
	}

	public static bool TryRedeemPrize(GameItem gameItem)
	{
		if (!CanRedeemPrize(gameItem))
		{
			return false;
		}
		int num = (int)gameItem.unitValue;
		gameItem.Destroy();
		PlayerStore.Instance.ModCash(num);
		EmporiumAudioPlayer.Instance.audioRegister.Play();
		ReputationUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("mech_lottery_redeemed", num));
		return true;
	}

	public static void CreateLotteryRedeemTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("LOTTERY_TICKET_TAG") && CanRedeemPrize(item))
		{
			builder.AddLine(" " + LocHelper.GetLocalizedMechanic("mech_tooltip_lottery_redeem"), ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue);
		}
	}

	public static int GetCurrentEdition()
	{
		return PlayerStore.Instance.currentLotteryEdition;
	}

	public static bool IsLotteryUnlocked()
	{
		if (!StartingPerk.IsGamblingAddiction())
		{
			return StoreStation.Instance.dayCounter > 14;
		}
		return true;
	}
}
