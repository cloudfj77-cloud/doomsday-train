using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public static class RunSummaryUI
{
	private struct Segment
	{
		public string name;

		public int value;

		public string display;

		public Color color;
	}

	private const float PANEL_W = 1280f;

	private const float PANEL_H = 920f;

	private const float PAD_X = 40f;

	private const float PAD_Y = 32f;

	private const float CONTENT_W = 1200f;

	private const float GAP = 14f;

	private const float STAT_W = 289.5f;

	private const float STAT_H = 84f;

	private const float COLUMN_GAP = 36f;

	private const float THIRD_W = 376f;

	private const float HALF_W = 588f;

	private const float CHART_H = 110f;

	private const int MAX_BARS = 150;

	private const float SCENARIO_ICON_SIZE = 76f;

	private static readonly Color CREAM = NewGameStyle.Hex("d1c8a5");

	private static readonly Color GREY = NewGameStyle.Hex("a8aea5");

	private static readonly Color TEAL = NewGameStyle.Hex("8abfbb");

	private static readonly Color TEAL_DARK = NewGameStyle.Hex("45918b");

	private static readonly Color GOLD = NewGameStyle.Hex("e0d09b");

	private static readonly Color WHITE = NewGameStyle.Hex("fefefe");

	private static readonly Color SUBTITLE = NewGameStyle.Hex("c4c4b0");

	private static readonly Color RUST = NewGameStyle.Hex("9b4a3d");

	private static readonly Color AMBER = NewGameStyle.PERK_NEUTRAL;

	private static readonly Color GREEN = NewGameStyle.PERK_POSITIVE;

	private static readonly Color RED = NewGameStyle.PERK_NEGATIVE;

	private static GameObject root;

	private static readonly List<GameObject> pages = new List<GameObject>();

	private static readonly List<CustomUIElement> tabs = new List<CustomUIElement>();

	private static readonly List<Sprite> tabNormalSprites = new List<Sprite>();

	private static readonly Dictionary<Sprite, Sprite> discCache = new Dictionary<Sprite, Sprite>();

	public static bool IsOpen => root != null;

	public static bool CanOpen()
	{
		if (CustomUIManager.Instance != null && CustomUIManager.Instance.metaPanelPrefab != null)
		{
			return PlayerStore.HasInstance;
		}
		return false;
	}

	public static bool Open(string endingId, bool canKeepPlaying, Action onMainMenu, Action onKeepPlaying)
	{
		Close();
		if (!CanOpen())
		{
			return false;
		}
		try
		{
			Build(endingId, canKeepPlaying, onMainMenu, onKeepPlaying);
			SelectPage(0);
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError("[RunSummary] Building the run summary failed: " + ex);
			Close();
			return false;
		}
	}

	public static void Close()
	{
		if (root != null)
		{
			UnityEngine.Object.Destroy(root);
		}
		root = null;
		pages.Clear();
		tabs.Clear();
		tabNormalSprites.Clear();
	}

	private static void Build(string endingId, bool canKeepPlaying, Action onMainMenu, Action onKeepPlaying)
	{
		CustomUIManager instance = CustomUIManager.Instance;
		PlayerStore store = PlayerStore.Instance;
		RunStats stats = RunStats.Current;
		root = new GameObject("RunSummary", typeof(RectTransform));
		RectTransform rectTransform = (RectTransform)root.transform;
		rectTransform.SetParent(instance.GetLayerRoot("overlay"), worldPositionStays: false);
		Stretch(rectTransform);
		root.AddComponent<Image>().color = Color.black;
		GameObject gameObject = UnityEngine.Object.Instantiate(instance.metaPanelPrefab, rectTransform);
		gameObject.name = "panel";
		RectTransform obj = (RectTransform)gameObject.transform;
		obj.anchorMin = new Vector2(0.5f, 0.5f);
		obj.anchorMax = new Vector2(0.5f, 0.5f);
		obj.pivot = new Vector2(0.5f, 0.5f);
		obj.sizeDelta = new Vector2(1280f, 920f);
		obj.anchoredPosition = Vector2.zero;
		RectTransform rectTransform2 = NewRect(obj, "content");
		rectTransform2.anchorMin = Vector2.zero;
		rectTransform2.anchorMax = Vector2.one;
		rectTransform2.offsetMin = new Vector2(40f, 32f);
		rectTransform2.offsetMax = new Vector2(-40f, -32f);
		VerticalLayoutGroup verticalLayoutGroup = rectTransform2.gameObject.AddComponent<VerticalLayoutGroup>();
		verticalLayoutGroup.spacing = 16f;
		verticalLayoutGroup.childControlWidth = true;
		verticalLayoutGroup.childControlHeight = true;
		verticalLayoutGroup.childForceExpandWidth = true;
		verticalLayoutGroup.childForceExpandHeight = false;
		CustomUIBuilder customUIBuilder = instance.BuildInto(rectTransform2, useMetaPrefabs: true, "run_summary");
		BuildHeader(customUIBuilder, store, endingId);
		customUIBuilder.AddSeparator();
		string[] array = new string[6]
		{
			T("ui_run_summary_tab_overview", "Overview"),
			LocHelper.GetLocalizedMechanic("mech_credits"),
			T("ui_run_summary_tab_expenses", "Expenses"),
			T("ui_run_summary_tab_station", "Station"),
			T("ui_run_summary_tab_nights", "Nights Out"),
			LocHelper.GetLocalizedUI("inte_security")
		};
		customUIBuilder.BeginRow(12f, TextAnchor.MiddleLeft);
		for (int i = 0; i < array.Length; i++)
		{
			int index = i;
			customUIBuilder.AddButton(array[i], delegate
			{
				SelectPage(index);
			});
			CustomUIElement last = customUIBuilder.Last;
			tabs.Add(last);
			tabNormalSprites.Add((last.button != null && last.button.image != null) ? last.button.image.sprite : null);
		}
		customUIBuilder.End();
		customUIBuilder.BeginColumn(0f);
		RectTransform currentContainer = customUIBuilder.CurrentContainer;
		customUIBuilder.End();
		ExpandChildrenWidth(currentContainer);
		LayoutElement layoutElement = currentContainer.gameObject.AddComponent<LayoutElement>();
		layoutElement.flexibleHeight = 1f;
		layoutElement.flexibleWidth = 1f;
		AddPage(instance, customUIBuilder, currentContainer, delegate(CustomUIBuilder pb)
		{
			BuildOverview(pb, store, stats);
		});
		AddPage(instance, customUIBuilder, currentContainer, delegate(CustomUIBuilder pb)
		{
			BuildMoney(pb, stats);
		});
		AddPage(instance, customUIBuilder, currentContainer, delegate(CustomUIBuilder pb)
		{
			BuildExpenses(pb, store, stats);
		});
		AddPage(instance, customUIBuilder, currentContainer, delegate(CustomUIBuilder pb)
		{
			BuildStation(pb, store, stats);
		});
		AddPage(instance, customUIBuilder, currentContainer, delegate(CustomUIBuilder pb)
		{
			BuildNightsOut(pb, store, stats);
		});
		AddPage(instance, customUIBuilder, currentContainer, delegate(CustomUIBuilder pb)
		{
			BuildCrime(pb, store, stats);
		});
		customUIBuilder.BeginRow(16f, TextAnchor.MiddleLeft);
		Label(customUIBuilder, PlayTimeText(store), 18f, GREY, bold: false, null, wrap: false);
		customUIBuilder.AddFlexibleSpace();
		if (canKeepPlaying)
		{
			customUIBuilder.AddButton(LocHelper.GetLocalizedMainMenu("new_continue"), delegate
			{
				Close();
				onKeepPlaying?.Invoke();
			});
		}
		customUIBuilder.AddButton(LocHelper.GetLocalizedUI("esc_main_menu"), delegate
		{
			Close();
			onMainMenu?.Invoke();
		});
		customUIBuilder.End();
	}

	private static void AddPage(CustomUIManager manager, CustomUIBuilder b, RectTransform host, Action<CustomUIBuilder> fill)
	{
		GameObject gameObject = new GameObject("page", typeof(RectTransform));
		gameObject.transform.SetParent(host, worldPositionStays: false);
		VerticalLayoutGroup verticalLayoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
		verticalLayoutGroup.spacing = 18f;
		verticalLayoutGroup.childControlWidth = true;
		verticalLayoutGroup.childControlHeight = true;
		verticalLayoutGroup.childForceExpandWidth = true;
		verticalLayoutGroup.childForceExpandHeight = false;
		pages.Add(gameObject);
		CustomUIBuilder obj = new CustomUIBuilder(manager, b.Handle, (RectTransform)gameObject.transform);
		try
		{
			fill(obj);
		}
		catch (Exception ex)
		{
			Debug.LogError("[RunSummary] A page of the run summary failed: " + ex);
		}
	}

	private static void SelectPage(int index)
	{
		for (int i = 0; i < pages.Count; i++)
		{
			if (pages[i] != null)
			{
				pages[i].SetActive(i == index);
			}
		}
		for (int j = 0; j < tabs.Count; j++)
		{
			StyleTab(tabs[j], tabNormalSprites[j], j == index);
		}
	}

	private static void StyleTab(CustomUIElement tab, Sprite normalSprite, bool active)
	{
		if (tab != null && !(tab.button == null))
		{
			Image image = tab.button.image;
			Sprite pressedSprite = tab.button.spriteState.pressedSprite;
			if (image != null)
			{
				image.sprite = ((active && pressedSprite != null) ? pressedSprite : normalSprite);
			}
			ButtonTextColor componentInChildren = tab.root.GetComponentInChildren<ButtonTextColor>(includeInactive: true);
			if (componentInChildren != null)
			{
				componentInChildren.enabled = !active;
			}
			tab.button.interactable = !active;
			if (tab.text != null)
			{
				tab.text.color = (active ? TEAL_DARK : TEAL);
			}
		}
	}

	private static void BuildHeader(CustomUIBuilder b, PlayerStore store, string endingId)
	{
		StartingScenario startingScenario = StartingScenario.Get(StartingScenario.ResolveId(store.startType, store.startScenarioId));
		string text = ((startingScenario != null) ? startingScenario.GetTitle() : NewGameData.GetStartDisplayName(store.startType));
		b.BeginRow(24f, TextAnchor.MiddleLeft);
		AddScenarioIcon(b.CurrentContainer, ResolveScenarioIcon(startingScenario));
		b.BeginColumn(8f, TextAnchor.MiddleLeft);
		Flexible(b.CurrentContainer);
		Label(b, text, 52f, WHITE, bold: false, FontLoader.FontRole.Lubrifont, wrap: false);
		Label(b, EndingTitle(endingId), 24f, CREAM, bold: true, null, wrap: false);
		b.End();
		Difficulty current = Difficulty.Current;
		if (current != null)
		{
			b.BeginPanel(300f, 96f, TextAnchor.MiddleCenter, 8f, 8f);
			CustomUIElement customUIElement = Label(b, current.GetTitle(), 32f, WHITE, bold: false, FontLoader.FontRole.Lubrifont, wrap: false);
			if (customUIElement.text != null)
			{
				customUIElement.text.fontStyle |= FontStyles.UpperCase;
				customUIElement.text.alignment = TextAlignmentOptions.Center;
			}
			CustomUIElement customUIElement2 = Label(b, current.GetSubtitle(), 16f, SUBTITLE, bold: false, null, wrap: false);
			if (customUIElement2.text != null)
			{
				customUIElement2.text.alignment = TextAlignmentOptions.Center;
			}
			b.End();
		}
		b.End();
	}

	private static string EndingTitle(string endingId)
	{
		return endingId switch
		{
			"rent" => LocHelper.GetLocalizedMechanic("mech_end_rent_reason"), 
			"arrest" => LocHelper.GetLocalizedMechanic("mech_end_arrest_reason"), 
			"execution" => LocHelper.GetLocalizedMechanic("mech_end_execution_reason"), 
			"buyout" => LocHelper.GetLocalizedMechanic("mech_end_buyout_reason"), 
			"buyout_mortgage" => LocHelper.GetLocalizedMechanic("mech_end_buyout_reason"), 
			"repo" => LocHelper.GetLocalizedMechanic("mech_end_repo_reason"), 
			"mob" => LocHelper.GetLocalizedMechanic("mech_end_mob_reason"), 
			"sec" => LocHelper.GetLocalizedMechanic("mech_end_sec_reason"), 
			"hitman" => LocHelper.GetLocalizedMechanic("mech_end_hitman_reason"), 
			"sentinel" => LocHelper.GetLocalizedMechanic("mech_end_sentinel_reason"), 
			"bombed" => LocHelper.GetLocalizedMechanic("mech_end_bombed_reason"), 
			"lou_bombed" => LocHelper.GetLocalizedMechanic("mech_end_bombed_reason"), 
			"lou_bombed_debt" => LocHelper.GetLocalizedMechanic("mech_end_bombed_reason"), 
			"cartel_retired" => LocHelper.GetLocalizedMechanic("mech_end_cartel_retired_reason"), 
			"vendor_permit" => LocHelper.GetLocalizedMechanic("mech_end_vendor_permit_reason"), 
			"aug" => LocHelper.GetLocalizedMechanic("mech_end_aug_reason"), 
			"reactor" => LocHelper.GetLocalizedMechanic("mech_end_reactor_reason"), 
			_ => T("ui_run_summary_ending_unknown", "Game Over"), 
		};
	}

	private static string PlayTimeText(PlayerStore store)
	{
		int num = Mathf.Max(0, Mathf.FloorToInt(store.GetCurrentRunPlayTime() / 60f));
		int num2 = num / 60;
		int num3 = num % 60;
		return T("ui_run_summary_play_time", "{0} h {1} min played", num2, num3);
	}

	private static void BuildOverview(CustomUIBuilder b, PlayerStore store, RunStats s)
	{
		if (store.startingPerks != null && store.startingPerks.Count > 0)
		{
			b.BeginColumn(10f);
			ExpandChildrenWidth(b.CurrentContainer);
			SectionTitle(b, LocHelper.GetLocalizedMainMenu("main_starting_perk"));
			b.BeginRow(28f);
			foreach (StartingPerk startingPerk in store.startingPerks)
			{
				if (startingPerk != null)
				{
					b.BeginColumn(8f, TextAnchor.UpperCenter);
					FixedWidth(b.CurrentContainer, 120f);
					AddPerkSlot(b.CurrentContainer, startingPerk);
					CustomUIElement customUIElement = Label(b, startingPerk.GetLocalizedDisplayName(), 16f, GREY);
					if (customUIElement.text != null)
					{
						customUIElement.text.alignment = TextAlignmentOptions.Top;
						customUIElement.text.enableAutoSizing = true;
						customUIElement.text.fontSizeMax = 16f;
						customUIElement.text.fontSizeMin = 11f;
					}
					customUIElement.SetFixedWidth(120f);
					customUIElement.SetPreferredSize(120f, 40f);
					b.End();
				}
			}
			b.End();
			b.End();
		}
		b.BeginRow(14f);
		StatPanel(b, T("ui_run_summary_days_survived", "Days Survived"), Number(StoreStation.Instance.dayCounter), CREAM);
		StatPanel(b, LocHelper.GetLocalizedUI("calendar_estimated_wealth"), Money(FinalNetWorth(store, s)), CREAM);
		if (store.IsPropertyPaid && s.storePrice > 0)
		{
			StatPanel(b, LocHelper.GetLocalizedMechanic("mech_end_buyout_reason"), Money(s.storePrice), GOLD);
		}
		else
		{
			StatPanel(b, T("ui_run_summary_customers_served", "Customers Served"), Number(s.customersServed), CREAM);
		}
		StatPanel(b, T("ui_run_summary_total_sales", "Total Sales"), Money(s.totalSales), CREAM);
		b.End();
		List<(string, string)> list = new List<(string, string)>();
		if (s.bestDay >= 0 && s.bestDaySales > 0)
		{
			list.Add((T("ui_run_summary_hl_best_day", "Best Day"), T("ui_run_summary_hl_best_day_text", "Day {0}: {1} of goods sold", s.bestDay, Money(s.bestDaySales))));
		}
		if (s.bestFlipDay >= 0)
		{
			list.Add((T("ui_run_summary_hl_best_flip", "Best Flip"), T("ui_run_summary_hl_best_flip_text", "{0}: bought for {1}, sold for {2}", s.bestFlipItem, Money(s.bestFlipBought), Money(s.bestFlipSold))));
		}
		if (s.biggestSaleDay >= 0)
		{
			list.Add((T("ui_run_summary_hl_biggest_sale", "Biggest Sale"), T("ui_run_summary_hl_biggest_sale_text", "{0} to {1}: {2}", s.biggestSaleItem, s.biggestSaleClient, Money(s.biggestSaleValue))));
		}
		if (s.bestFindDay >= 0)
		{
			list.Add((T("ui_run_summary_hl_best_find", "Best Find"), T("ui_run_summary_hl_best_find_text", "{0}, worth {1} (Day {2})", s.bestFindItem, Money(s.bestFindValue), s.bestFindDay)));
		}
		if (s.worstBurglaryDay >= 0)
		{
			list.Add((T("ui_run_summary_hl_worst_night", "Worst Night"), T("ui_run_summary_hl_worst_night_text", "Burglary on Day {0}: {1} of stock taken", s.worstBurglaryDay, Money(s.worstBurglaryValue))));
		}
		if (s.closestCallDay >= 0)
		{
			list.Add((T("ui_run_summary_hl_closest_call", "Closest Call"), T("ui_run_summary_hl_closest_call_text", "Paid on Day {0} with {1} to spare", s.closestCallDay, Money(s.closestCallCredits))));
		}
		if (list.Count == 0)
		{
			return;
		}
		b.BeginColumn(12f);
		ExpandChildrenWidth(b.CurrentContainer);
		SectionTitle(b, T("ui_run_summary_highlights", "Highlights"));
		for (int i = 0; i < list.Count; i += 3)
		{
			b.BeginRow(36f);
			for (int j = i; j < Mathf.Min(i + 3, list.Count); j++)
			{
				b.BeginColumn(2f);
				FixedWidth(b.CurrentContainer, 376f);
				ExpandChildrenWidth(b.CurrentContainer);
				Label(b, list[j].Item1, 16f, TEAL);
				Label(b, list[j].Item2, 20f, GREY);
				b.End();
			}
			b.End();
		}
		b.End();
	}

	private static void BuildMoney(CustomUIBuilder b, RunStats s)
	{
		b.BeginRow(14f);
		StatPanel(b, T("ui_run_summary_total_sales", "Total Sales"), Money(s.totalSales), CREAM);
		StatPanel(b, T("ui_run_summary_stock_bought", "Stock Bought"), Money(s.stockSpent), CREAM);
		StatPanel(b, LocHelper.GetLocalizedItem("item_vending_machine_name"), Money(s.vendingIncome), CREAM);
		StatPanel(b, LocHelper.GetLocalizedItem("item_vending_fountain_name"), Money(s.fountainIncome), CREAM);
		b.End();
		Chart(b, LocHelper.GetLocalizedUI("calendar_estimated_wealth"), s.netWorthByDay, s.seriesStartDay, TEAL);
		Chart(b, LocHelper.GetLocalizedMechanic("mech_credits"), s.creditsByDay, s.seriesStartDay, CREAM);
	}

	private static void BuildExpenses(CustomUIBuilder b, PlayerStore store, RunStats s)
	{
		b.BeginRow(14f);
		StatPanel(b, T("ui_run_summary_rent_paid", "Rent Paid to the Landlord"), Money(s.rentPaid), CREAM);
		StatPanel(b, LocHelper.GetLocalizedMechanic("service_power"), Money(s.powerSpent), CREAM);
		StatPanel(b, LocHelper.GetLocalizedMechanic("service_janitorial"), Money(s.janitorialSpent), CREAM);
		StatPanel(b, LocHelper.GetLocalizedMechanic("service_insurance"), Money(s.insuranceSpent), CREAM);
		b.End();
		b.BeginRow(24f);
		List<string> list = StoreLines(store, s);
		if (list.Count > 0)
		{
			BeginBox(b, 588f);
			SectionTitle(b, LocHelper.GetLocalizedMechanic("mech_end_buyout_reason"));
			foreach (string item in list)
			{
				Label(b, item, 18f, GREY);
			}
			b.End();
		}
		BeginBox(b, 588f);
		SectionTitle(b, LocHelper.GetLocalizedItem("item_business_permit_name"));
		if (!store.hasBusinessPermit)
		{
			Label(b, LocHelper.GetLocalizedMechanic("sec_data_none"), 18f, GREY);
		}
		else if (s.permitBoughtDay >= 0)
		{
			Label(b, T("ui_run_summary_permit_bought", "Bought on Day {0} for {1}", s.permitBoughtDay, Money(s.permitPrice)), 18f, GREY);
		}
		KeyValueRow(b, T("ui_run_summary_bribes", "Bribes"), (s.permitBribes > 0) ? Money(s.permitBribes) : LocHelper.GetLocalizedMechanic("sec_data_none"), 18f);
		Label(b, LocHelper.GetLocalizedClientName("name_trade_board_official"), 16f, GREY);
		b.End();
		b.End();
		List<Segment> list2 = new List<Segment>();
		AddSegment(list2, T("ui_run_summary_stock_bought", "Stock Bought"), s.stockSpent, TEAL, money: true);
		AddSegment(list2, LocHelper.GetLocalizedMechanic("mech_calendar_keyword_rent"), s.rentPaid, CREAM, money: true);
		if (s.storeBoughtDay >= 0 && !s.storeBoughtWithMortgage)
		{
			AddSegment(list2, LocHelper.GetLocalizedMechanic("mech_end_buyout_reason"), s.storePrice, GOLD, money: true);
		}
		AddSegment(list2, LocHelper.GetLocalizedMechanic("event_mortgage_payment"), s.mortgagePaid, GOLD, money: true);
		AddSegment(list2, LocationTitle("dr_jackson"), s.drJacksonSpent, TEAL_DARK, money: true);
		AddSegment(list2, LocHelper.GetLocalizedMechanic("service_power"), s.powerSpent, AMBER, money: true);
		AddSegment(list2, LocHelper.GetLocalizedMechanic("service_janitorial"), s.janitorialSpent, WHITE, money: true);
		AddSegment(list2, LocHelper.GetLocalizedMechanic("service_insurance"), s.insuranceSpent, RUST, money: true);
		AddSegment(list2, LocHelper.GetLocalizedUI("inte_wild_network"), s.wildNetworkSpent, GREY, money: true);
		AddSegment(list2, T("ui_run_summary_seg_permit", "Permit and Bribes"), s.permitPrice + s.permitBribes, RED, money: true);
		int num = list2.Sum((Segment seg) => seg.value);
		if (num > 0)
		{
			b.BeginColumn(10f);
			ExpandChildrenWidth(b.CurrentContainer);
			b.BeginRow(8f, TextAnchor.LowerLeft);
			SectionTitle(b, T("ui_run_summary_where_money_went", "Where the Money Went"));
			b.AddFlexibleSpace();
			Label(b, LocHelper.GetLocalizedUI("ui_nego_total"), 18f, GREY, bold: false, null, wrap: false);
			Label(b, Money(num), 18f, CREAM, bold: false, null, wrap: false);
			b.End();
			SegmentBar(b, list2);
			b.End();
		}
		b.BeginRow(36f);
		int num2 = s.rentWeeksPaid + s.rentWeeksMissed;
		string value = ((s.rentWeeksMissed == 0) ? T("ui_run_summary_rent_weeks_value", "{0} of {1}, none missed", s.rentWeeksPaid, num2) : T("ui_run_summary_rent_weeks_value_missed", "{0} of {1}, {2} missed", s.rentWeeksPaid, num2, s.rentWeeksMissed));
		Fact(b, T("ui_run_summary_rent_weeks", "Rent Weeks Paid"), value);
		Fact(b, LocHelper.GetLocalizedUI("inte_wild_network"), Number(s.wildUpgradesBought));
		b.End();
	}

	private static List<string> StoreLines(PlayerStore store, RunStats s)
	{
		List<string> list = new List<string>();
		if (s.storeBoughtDay >= 0 && !s.storeBoughtWithMortgage)
		{
			list.Add(T("ui_run_summary_store_cash", "Bought with cash on Day {0} for {1}", s.storeBoughtDay, Money(s.storePrice)));
		}
		else if (s.mortgageTakenDay >= 0)
		{
			if (store.IsPropertyPaid)
			{
				list.Add(T("ui_run_summary_store_mortgage_paid", "Mortgage taken on Day {0} for a price of {1}, paid off on Day {2}", s.mortgageTakenDay, Money(s.storePrice), s.storeBoughtDay));
			}
			else
			{
				list.Add(T("ui_run_summary_store_mortgage_running", "Mortgage taken on Day {0} for a price of {1}: {2} of {3} weekly payments made", s.mortgageTakenDay, Money(s.storePrice), s.mortgageWeeksPaid, s.mortgageWeeks));
			}
			list.Add(T("ui_run_summary_store_mortgage_total", "Paid {0}: a {1} down payment and {2} weekly payments of {3}", Money(s.mortgagePaid), Money(s.mortgageDownPayment), s.mortgageWeeksPaid, Money(s.mortgageWeeklyPayment)));
		}
		else if (!store.IsPropertyPaid)
		{
			list.Add(T("ui_run_summary_store_renting", "Still renting. The store's price on the last day: {0}", Money(store.GetStoreBuyPrice())));
		}
		return list;
	}

	private static void BuildStation(CustomUIBuilder b, PlayerStore store, RunStats s)
	{
		b.BeginRow(14f);
		StatPanel(b, T("ui_run_summary_customers_served", "Customers Served"), Number(s.customersServed), CREAM);
		b.End();
		string localizedMechanic = LocHelper.GetLocalizedMechanic("sec_data_none");
		BeginBox(b, -1f);
		SectionTitle(b, LocHelper.GetLocalizedClientName("name_georgio"));
		float width = 273f;
		b.BeginRow(24f);
		Cell(b, width, T("ui_run_summary_visits", "Visits"), Number(s.georgioVisits), CREAM);
		Cell(b, width, T("ui_run_summary_georgio_bought", "Bought from Him"), Money(s.georgioBoughtFrom), CREAM);
		bool flag = s.georgioLent > 0;
		Cell(b, width, T("ui_run_summary_georgio_lent", "Lent to Him"), flag ? Money(s.georgioLent) : localizedMechanic, flag ? CREAM : GREY);
		if (flag)
		{
			bool flag2 = s.georgioPaidBack > 0;
			Cell(b, width, T("ui_run_summary_georgio_paid", "Paid Back"), flag2 ? ("+" + Money(s.georgioPaidBack)) : localizedMechanic, flag2 ? GREEN : GREY);
		}
		b.End();
		b.End();
		b.BeginColumn(10f);
		ExpandChildrenWidth(b.CurrentContainer);
		SectionTitle(b, T("ui_run_summary_reputation", "Reputation"));
		List<StoreReputation> list = store.storeReputations ?? new List<StoreReputation>();
		float width2 = 390.66666f;
		for (int i = 0; i < list.Count; i += 3)
		{
			b.BeginRow(14f);
			for (int j = i; j < Mathf.Min(i + 3, list.Count); j++)
			{
				StoreReputation storeReputation = list[j];
				int reputation = storeReputation.GetReputation();
				Color color = ((reputation > 0) ? GREEN : ((reputation < 0) ? RED : GREY));
				string text = ((reputation > 0) ? ("+" + reputation.ToString(CultureInfo.InvariantCulture)) : reputation.ToString(CultureInfo.InvariantCulture));
				b.BeginPanel(width2, -1f, TextAnchor.UpperLeft, 12f);
				ExpandChildrenWidth(b.CurrentContainer);
				SetPadding(b.CurrentContainer, 18, 12);
				Label(b, storeReputation.factionDisplay, 16f, GREY, bold: false, null, wrap: false);
				b.BeginRow(8f, TextAnchor.LowerLeft);
				Label(b, StoreReputation.GetReputationStatus(reputation), 20f, CREAM, bold: true, null, wrap: false);
				b.AddFlexibleSpace();
				Label(b, text, 24f, color, bold: true, null, wrap: false);
				b.End();
				b.End();
			}
			b.End();
		}
		b.End();
	}

	private static void BuildNightsOut(CustomUIBuilder b, PlayerStore store, RunStats s)
	{
		Color[] array = new Color[5] { TEAL, CREAM, AMBER, TEAL_DARK, GOLD };
		List<Segment> list = new List<Segment>();
		int num = 0;
		int num2 = 0;
		foreach (AfterhourLocation item in AfterhourLocation.GetAll())
		{
			int num3 = Visits(store, item.Id);
			if (num3 > 0)
			{
				num += num3;
				AddSegment(list, item.GetTitle(), num3, array[num2 % array.Length], money: false);
				num2++;
			}
		}
		int num4 = Mathf.Max(Mathf.Max(0, StoreStation.Instance.dayCounter - 1), num);
		if (num4 - num > 0)
		{
			List<Segment> list2 = new List<Segment>();
			AddSegment(list2, LocHelper.GetLocalizedNetworkUpgrade("network_subtitle_Store"), num4 - num, GREY, money: false);
			list2.AddRange(list);
			list = list2;
		}
		if (list.Count > 0)
		{
			b.BeginColumn(10f);
			ExpandChildrenWidth(b.CurrentContainer);
			b.BeginRow(8f, TextAnchor.LowerLeft);
			SectionTitle(b, T("ui_run_summary_nights_went", "Where Your Nights Went"));
			b.AddFlexibleSpace();
			Label(b, T("ui_run_summary_nights_count", "{0} nights", num4), 18f, GREY, bold: false, null, wrap: false);
			b.End();
			SegmentBar(b, list);
			b.End();
		}
		float width = 273f;
		BeginBox(b, -1f);
		SectionTitle(b, LocationTitle("dumping_ground"));
		b.BeginRow(24f);
		Cell(b, width, T("ui_run_summary_visits", "Visits"), Number(Visits(store, "dumping_ground")), CREAM);
		Cell(b, width, T("ui_run_summary_scavenge_attempts", "Scavenge Attempts"), Number(s.scavengeAttempts), CREAM);
		Cell(b, width, T("ui_run_summary_scavenged_value", "Value of Scavenged Items"), Money(s.scavengedValue), CREAM);
		Cell(b, width, T("ui_run_summary_times_injured", "Times Injured"), Number(s.injuriesMinor + s.injuriesMajor), CREAM, T("ui_run_summary_injuries_split", "{0} minor, {1} major", s.injuriesMinor, s.injuriesMajor));
		b.End();
		if (s.bestFindDay >= 0)
		{
			b.AddSeparator();
			Label(b, T("ui_run_summary_best_find", "Best find: {0}, worth {1} (Day {2})", s.bestFindItem, Money(s.bestFindValue), s.bestFindDay), 18f, GREY);
		}
		b.End();
		BeginBox(b, -1f);
		SectionTitle(b, LocationTitle("dr_jackson"));
		b.BeginRow(24f);
		Cell(b, width, T("ui_run_summary_visits", "Visits"), Number(Visits(store, "dr_jackson")), CREAM);
		Cell(b, width, LocHelper.GetLocalizedUI("ui_nego_total"), Money(s.drJacksonSpent), CREAM);
		Cell(b, width, T("ui_run_summary_jackson_machines", "Machines and Modules Bought"), Number(s.drJacksonMachinesBought), CREAM);
		b.End();
		b.End();
	}

	private static void BuildCrime(CustomUIBuilder b, PlayerStore store, RunStats s)
	{
		b.BeginRow(24f);
		BeginBox(b, 588f);
		SectionTitle(b, LocHelper.GetLocalizedUI("inte_sec_record"));
		SecData secData = store.secData;
		int num = 0;
		if (secData != null && secData.crimeTypes != null && secData.crimeAmountTotal != null)
		{
			for (int i = 0; i < Mathf.Min(secData.crimeTypes.Count, secData.crimeAmountTotal.Count); i++)
			{
				int num2 = secData.crimeAmountTotal[i];
				if (num2 > 0)
				{
					num += num2;
					KeyValueRow(b, secData.GetCrimeString(secData.crimeTypes[i]), Money(num2), 20f);
				}
			}
		}
		if (num == 0)
		{
			Label(b, LocHelper.GetLocalizedMechanic("sec_data_none"), 20f, TEAL);
		}
		else
		{
			b.AddSeparator();
			KeyValueRow(b, LocHelper.GetLocalizedUI("ui_nego_total"), Money(num), 24f, bold: true);
		}
		b.End();
		int num3 = ((store.wantedStatus != null) ? store.wantedStatus.Values.Count((int v) => v == 1) : 0);
		BeginBox(b, 588f);
		SectionTitle(b, LocHelper.GetLocalizedUI("inte_security"));
		KeyValueRow(b, T("ui_run_summary_inspections_passed", "Inspections Passed"), T("ui_run_summary_x_of_y", "{0} of {1}", s.inspectionsPassed, s.inspections), 20f);
		KeyValueRow(b, T("ui_run_summary_goods_seized", "Goods Seized by Inspectors"), Money(s.goodsSeized), 20f);
		KeyValueRow(b, T("ui_run_summary_inspector_bribes", "Bribes Paid to Inspectors"), Money(s.inspectorBribes), 20f);
		KeyValueRow(b, LocHelper.GetLocalizedMechanic("mech_afterhour_event_burglary_title"), Money(s.burglaryLosses), 20f);
		KeyValueRow(b, T("ui_run_summary_wanted_caught", "Wanted Criminals Caught"), T("ui_run_summary_x_of_y", "{0} of {1}", num3, StoreClientListWanted.POSTER_IDS.Count), 20f);
		KeyValueRow(b, T("ui_run_summary_wrongful_reports", "Wrongful Reports"), Number(s.wrongfulReports), 20f);
		KeyValueRow(b, T("ui_run_summary_augs_shot", "Augs Shot"), Number(s.augsShot), 20f);
		b.End();
		b.End();
	}

	private static CustomUIElement Label(CustomUIBuilder b, string text, float size, Color color, bool bold = false, FontLoader.FontRole? role = null, bool wrap = true)
	{
		b.AddLabel(text ?? "");
		CustomUIElement last = b.Last;
		TextMeshProUGUI text2 = last.text;
		if (text2 != null)
		{
			if (role.HasValue)
			{
				SetFontRole(text2, role.Value);
			}
			text2.enableAutoSizing = false;
			text2.fontSize = size;
			text2.color = color;
			text2.fontStyle = (bold ? FontStyles.Bold : FontStyles.Normal);
			text2.enableWordWrapping = wrap;
			text2.raycastTarget = false;
		}
		return last;
	}

	private static void SectionTitle(CustomUIBuilder b, string text)
	{
		Label(b, text, 22f, CREAM, bold: true, null, wrap: false);
	}

	private static void StatPanel(CustomUIBuilder b, string label, string value, Color valueColor)
	{
		b.BeginPanel(289.5f, 84f, TextAnchor.MiddleLeft, 10f, 2f);
		ExpandChildrenWidth(b.CurrentContainer);
		SetPadding(b.CurrentContainer, 18, 8);
		Label(b, label, 16f, GREY, bold: false, null, wrap: false);
		Label(b, value, 28f, valueColor, bold: true, null, wrap: false);
		b.End();
	}

	private static void BeginBox(CustomUIBuilder b, float width)
	{
		b.BeginPanel(width, -1f, TextAnchor.UpperLeft, 18f, 10f);
		ExpandChildrenWidth(b.CurrentContainer);
		SetPadding(b.CurrentContainer, 24, 18);
	}

	private static void Cell(CustomUIBuilder b, float width, string label, string value, Color valueColor, string note = null)
	{
		b.BeginColumn(2f);
		FixedWidth(b.CurrentContainer, width);
		ExpandChildrenWidth(b.CurrentContainer);
		Label(b, label, 16f, GREY);
		Label(b, value, 26f, valueColor, bold: true, null, wrap: false);
		if (!string.IsNullOrEmpty(note))
		{
			Label(b, note, 16f, GREY);
		}
		b.End();
	}

	private static void Fact(CustomUIBuilder b, string label, string value)
	{
		b.BeginColumn(2f);
		FixedWidth(b.CurrentContainer, 376f);
		ExpandChildrenWidth(b.CurrentContainer);
		Label(b, label, 16f, TEAL);
		Label(b, value, 20f, GREY);
		b.End();
	}

	private static void KeyValueRow(CustomUIBuilder b, string key, string value, float size, bool bold = false)
	{
		b.BeginRow(16f, TextAnchor.MiddleLeft);
		Label(b, key, size, bold ? CREAM : GREY, bold, null, wrap: false);
		b.AddFlexibleSpace();
		Label(b, value, size, CREAM, bold, null, wrap: false);
		b.End();
	}

	private static void Chart(CustomUIBuilder b, string title, List<int> series, int startDay, Color barColor)
	{
		if (series == null || series.Count == 0 || startDay < 0)
		{
			return;
		}
		b.BeginColumn(6f);
		ExpandChildrenWidth(b.CurrentContainer);
		SectionTitle(b, title);
		List<int> list = series;
		if (series.Count > 150)
		{
			int num = Mathf.CeilToInt((float)series.Count / 150f);
			list = new List<int>();
			for (int i = 0; i < series.Count; i += num)
			{
				list.Add(series.Skip(i).Take(num).Max());
			}
		}
		int count = list.Count;
		float num2 = ((count > 90) ? 1f : ((count > 45) ? 2f : 3f));
		float width = Mathf.Max(1f, Mathf.Floor((1200f - (float)(count - 1) * num2) / (float)count));
		float num3 = Mathf.Max(1, list.Max());
		int num4 = list.IndexOf(list.Max());
		b.BeginRow(num2, TextAnchor.LowerLeft);
		RectTransform currentContainer = b.CurrentContainer;
		FixedHeight(currentContainer, 110f);
		for (int j = 0; j < count; j++)
		{
			float height = Mathf.Max(2f, Mathf.Round((float)Mathf.Max(0, list[j]) / num3 * 110f));
			NewBlock(currentContainer, width, height, (j == num4) ? GOLD : barColor);
		}
		b.End();
		b.AddSeparator();
		int num5 = startDay + series.Count - 1;
		b.BeginRow(8f, TextAnchor.MiddleLeft);
		Label(b, LocHelper.GetLocalizedMechanic("mech_day_label", startDay), 16f, GREY, bold: false, null, wrap: false);
		b.AddFlexibleSpace();
		Label(b, LocHelper.GetLocalizedMechanic("mech_day_label", num5), 16f, GREY, bold: false, null, wrap: false);
		b.End();
		b.End();
	}

	private static void AddSegment(List<Segment> segments, string name, int value, Color color, bool money)
	{
		if (value > 0)
		{
			segments.Add(new Segment
			{
				name = name,
				value = value,
				color = color,
				display = (money ? Money(value) : Number(value))
			});
		}
	}

	private static void SegmentBar(CustomUIBuilder b, List<Segment> segments)
	{
		int num = segments.Sum((Segment seg) => seg.value);
		if (num <= 0)
		{
			return;
		}
		b.BeginRow(2f, TextAnchor.MiddleLeft);
		RectTransform currentContainer = b.CurrentContainer;
		FixedHeight(currentContainer, 28f);
		float num2 = 1200f - (float)(segments.Count - 1) * 2f;
		float num3 = 0f;
		for (int i = 0; i < segments.Count; i++)
		{
			float b2 = ((i == segments.Count - 1) ? (num2 - num3) : Mathf.Max(2f, Mathf.Round(num2 * (float)segments[i].value / (float)num)));
			b2 = Mathf.Max(1f, b2);
			num3 += b2;
			NewBlock(currentContainer, b2, 28f, segments[i].color);
		}
		b.End();
		for (int j = 0; j < segments.Count; j += 3)
		{
			b.BeginRow(36f, TextAnchor.MiddleLeft);
			for (int k = j; k < Mathf.Min(j + 3, segments.Count); k++)
			{
				b.BeginRow(10f, TextAnchor.MiddleLeft);
				FixedWidth(b.CurrentContainer, 376f);
				NewBlock(b.CurrentContainer, 14f, 14f, segments[k].color);
				Label(b, segments[k].name, 18f, GREY, bold: false, null, wrap: false);
				b.AddFlexibleSpace();
				Label(b, segments[k].display, 18f, CREAM, bold: false, null, wrap: false);
				b.End();
			}
			b.End();
		}
	}

	private static void AddPerkSlot(RectTransform parent, StartingPerk perk)
	{
		RectTransform rectTransform = NewRect(parent, "perk_slot");
		LayoutElement layoutElement = rectTransform.gameObject.AddComponent<LayoutElement>();
		float minWidth = (layoutElement.preferredWidth = 64f);
		layoutElement.minWidth = minWidth;
		minWidth = (layoutElement.preferredHeight = 72f);
		layoutElement.minHeight = minWidth;
		layoutElement.flexibleWidth = 0f;
		NewGamePerksFrame newGamePerksFrame = PerksFrame();
		if (newGamePerksFrame != null && newGamePerksFrame.perkSlotNormal != null)
		{
			Image image = rectTransform.gameObject.AddComponent<Image>();
			image.sprite = newGamePerksFrame.perkSlotNormal;
			image.raycastTarget = false;
		}
		Sprite sprite = ResolvePerkIcon(perk);
		if (sprite != null)
		{
			NewGameUIElements.FitPixelArt(NewImage(rectTransform, "icon", sprite, NewGameStyle.PerkColor(perk.type)), 0f, 0f, 64f, 64f);
		}
	}

	private static Sprite ResolvePerkIcon(StartingPerk perk)
	{
		return StartingPerkIconLoader.GetIcon(perk);
	}

	private static void AddScenarioIcon(RectTransform parent, Sprite icon)
	{
		if (!(icon == null))
		{
			float num = Mathf.Max(icon.rect.width, icon.rect.height);
			int num2 = Mathf.Max(1, Mathf.FloorToInt(76f / num));
			float num3 = icon.rect.width * (float)num2;
			float num4 = icon.rect.height * (float)num2;
			RectTransform rectTransform = NewRect(parent, "scenario_icon");
			LayoutElement layoutElement = rectTransform.gameObject.AddComponent<LayoutElement>();
			float minWidth = (layoutElement.preferredWidth = num3);
			layoutElement.minWidth = minWidth;
			minWidth = (layoutElement.preferredHeight = num4);
			layoutElement.minHeight = minWidth;
			layoutElement.flexibleWidth = 0f;
			Sprite discSprite = GetDiscSprite(icon);
			Stretch(NewImage(rectTransform, "disc", discSprite, (discSprite != null) ? Color.white : TEAL).rectTransform);
			Stretch(NewImage(rectTransform, "icon", icon, Color.white).rectTransform);
		}
	}

	private static Sprite ResolveScenarioIcon(StartingScenario scenario)
	{
		if (scenario == null)
		{
			return null;
		}
		Sprite sprite = ((IconProvider.Instance != null) ? IconProvider.Instance.GetIcon(scenario.id) : null);
		NewGameSpecializationFrame newGameSpecializationFrame = ((CustomUIManager.Instance.newGameSpecializationPrefab != null) ? CustomUIManager.Instance.newGameSpecializationPrefab.GetComponent<NewGameSpecializationFrame>() : null);
		if (sprite == null && !string.IsNullOrEmpty(scenario.iconKey))
		{
			if (newGameSpecializationFrame != null)
			{
				sprite = newGameSpecializationFrame.FindIcon(scenario.iconKey);
			}
			if (sprite == null)
			{
				sprite = CustomUIManager.Instance.GetSprite(scenario.iconKey, quiet: true);
			}
		}
		if (sprite == null && newGameSpecializationFrame != null)
		{
			sprite = newGameSpecializationFrame.FindIcon("icons_" + scenario.id);
		}
		return sprite;
	}

	private static Sprite GetDiscSprite(Sprite icon)
	{
		if (discCache.TryGetValue(icon, out var value) && value != null)
		{
			return value;
		}
		Sprite sprite = null;
		try
		{
			sprite = BuildDiscSprite(icon, TEAL);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[RunSummary] Could not build the scenario icon's disc, using a square: " + ex.Message);
		}
		discCache[icon] = sprite;
		return sprite;
	}

	private static Sprite BuildDiscSprite(Sprite icon, Color fill)
	{
		Rect textureRect = icon.textureRect;
		int num = Mathf.RoundToInt(textureRect.x);
		int num2 = Mathf.RoundToInt(textureRect.y);
		int w = Mathf.RoundToInt(textureRect.width);
		int num3 = Mathf.RoundToInt(textureRect.height);
		if (w <= 0 || num3 <= 0)
		{
			return null;
		}
		Texture2D texture = icon.texture;
		RenderTexture temporary = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.ARGB32);
		RenderTexture active = RenderTexture.active;
		Color32[] pixels;
		try
		{
			Graphics.Blit(texture, temporary);
			RenderTexture.active = temporary;
			Texture2D texture2D = new Texture2D(w, num3, TextureFormat.RGBA32, mipChain: false);
			texture2D.ReadPixels(new Rect(num, num2, w, num3), 0, 0);
			texture2D.Apply();
			pixels = texture2D.GetPixels32();
			UnityEngine.Object.Destroy(texture2D);
		}
		finally
		{
			RenderTexture.active = active;
			RenderTexture.ReleaseTemporary(temporary);
		}
		bool[] outside = new bool[w * num3];
		Queue<int> queue = new Queue<int>();
		for (int i = 0; i < w; i++)
		{
			Seed(i, 0);
			Seed(i, num3 - 1);
		}
		for (int j = 0; j < num3; j++)
		{
			Seed(0, j);
			Seed(w - 1, j);
		}
		while (queue.Count > 0)
		{
			int num4 = queue.Dequeue();
			int num5 = num4 % w;
			int num6 = num4 / w;
			if (num5 > 0)
			{
				Seed(num5 - 1, num6);
			}
			if (num5 < w - 1)
			{
				Seed(num5 + 1, num6);
			}
			if (num6 > 0)
			{
				Seed(num5, num6 - 1);
			}
			if (num6 < num3 - 1)
			{
				Seed(num5, num6 + 1);
			}
		}
		Color32 color = fill;
		Color32 color2 = new Color32(0, 0, 0, 0);
		Color32[] array = new Color32[w * num3];
		bool flag = false;
		for (int k = 0; k < array.Length; k++)
		{
			bool flag2 = pixels[k].a < 128 && !outside[k];
			array[k] = (flag2 ? color : color2);
			flag = flag || flag2;
		}
		if (!flag)
		{
			return null;
		}
		Texture2D texture2D2 = new Texture2D(w, num3, TextureFormat.RGBA32, mipChain: false);
		texture2D2.filterMode = FilterMode.Point;
		texture2D2.wrapMode = TextureWrapMode.Clamp;
		texture2D2.SetPixels32(array);
		texture2D2.Apply();
		return Sprite.Create(texture2D2, new Rect(0f, 0f, w, num3), new Vector2(0.5f, 0.5f), 100f);
		void Seed(int x, int y)
		{
			int num7 = y * w + x;
			if (!outside[num7] && pixels[num7].a < 128)
			{
				outside[num7] = true;
				queue.Enqueue(num7);
			}
		}
	}

	private static string T(string key, string english, params object[] args)
	{
		return LocHelper.GetLocalizedUIOrDefault(key, english, args);
	}

	private static string Money(long value)
	{
		return value.ToString("N0", CultureInfo.InvariantCulture) + GeneralHelper.CreditIcon(spaced: true);
	}

	private static string Number(int value)
	{
		return value.ToString("N0", CultureInfo.InvariantCulture);
	}

	private static int FinalNetWorth(PlayerStore store, RunStats s)
	{
		if (s.netWorthByDay != null && s.netWorthByDay.Count > 0)
		{
			return s.netWorthByDay[s.netWorthByDay.Count - 1];
		}
		return store.GetTotalEstimatedValue();
	}

	private static int Visits(PlayerStore store, string locationId)
	{
		if (store.afterhourState != null && store.afterhourState.TryGetValue(locationId, out var value) && value != null)
		{
			return value.visits;
		}
		return 0;
	}

	private static string LocationTitle(string locationId)
	{
		AfterhourLocation afterhourLocation = AfterhourLocation.Get(locationId);
		if (afterhourLocation == null)
		{
			return locationId;
		}
		return afterhourLocation.GetTitle();
	}

	private static NewGamePerksFrame PerksFrame()
	{
		GameObject gameObject = ((CustomUIManager.Instance != null) ? CustomUIManager.Instance.newGamePerksPrefab : null);
		if (!(gameObject != null))
		{
			return null;
		}
		return gameObject.GetComponent<NewGamePerksFrame>();
	}

	private static void SetFontRole(TextMeshProUGUI text, FontLoader.FontRole role)
	{
		TMPLocalizer component = text.GetComponent<TMPLocalizer>();
		if (component != null)
		{
			component.fontRole = role;
		}
		TMP_FontAsset tMP_FontAsset = null;
		if (FontLoader.Instance != null && LocalizationSettings.SelectedLocale != null)
		{
			tMP_FontAsset = FontLoader.Instance.GetFontByRole(role, LocalizationSettings.SelectedLocale);
		}
		if (tMP_FontAsset == null && role == FontLoader.FontRole.Lubrifont)
		{
			NewGamePerksFrame newGamePerksFrame = PerksFrame();
			if (newGamePerksFrame != null)
			{
				tMP_FontAsset = newGamePerksFrame.fontLubrifont;
			}
		}
		if (tMP_FontAsset != null)
		{
			text.font = tMP_FontAsset;
		}
	}

	private static RectTransform NewRect(Transform parent, string name)
	{
		GameObject gameObject = new GameObject(name, typeof(RectTransform));
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		return (RectTransform)gameObject.transform;
	}

	private static Image NewImage(Transform parent, string name, Sprite sprite, Color color)
	{
		Image image = NewRect(parent, name).gameObject.AddComponent<Image>();
		image.sprite = sprite;
		image.color = color;
		image.type = Image.Type.Simple;
		image.preserveAspect = false;
		image.raycastTarget = false;
		return image;
	}

	private static Image NewBlock(RectTransform parent, float width, float height, Color color)
	{
		Image image = NewImage(parent, "block", null, color);
		LayoutElement layoutElement = image.gameObject.AddComponent<LayoutElement>();
		float minWidth = (layoutElement.preferredWidth = width);
		layoutElement.minWidth = minWidth;
		minWidth = (layoutElement.preferredHeight = height);
		layoutElement.minHeight = minWidth;
		layoutElement.flexibleWidth = 0f;
		layoutElement.flexibleHeight = 0f;
		return image;
	}

	private static void Stretch(RectTransform rect)
	{
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;
	}

	private static void Flexible(RectTransform rect)
	{
		LayoutElement layoutElement = rect.GetComponent<LayoutElement>();
		if (layoutElement == null)
		{
			layoutElement = rect.gameObject.AddComponent<LayoutElement>();
		}
		layoutElement.flexibleWidth = 1f;
	}

	private static void FixedWidth(RectTransform rect, float width)
	{
		LayoutElement layoutElement = rect.GetComponent<LayoutElement>();
		if (layoutElement == null)
		{
			layoutElement = rect.gameObject.AddComponent<LayoutElement>();
		}
		layoutElement.minWidth = width;
		layoutElement.preferredWidth = width;
		layoutElement.flexibleWidth = 0f;
	}

	private static void FixedHeight(RectTransform rect, float height)
	{
		LayoutElement layoutElement = rect.GetComponent<LayoutElement>();
		if (layoutElement == null)
		{
			layoutElement = rect.gameObject.AddComponent<LayoutElement>();
		}
		layoutElement.minHeight = height;
		layoutElement.preferredHeight = height;
		layoutElement.flexibleHeight = 0f;
	}

	private static void ExpandChildrenWidth(RectTransform rect)
	{
		HorizontalOrVerticalLayoutGroup component = rect.GetComponent<HorizontalOrVerticalLayoutGroup>();
		if (component != null)
		{
			component.childForceExpandWidth = true;
		}
	}

	private static void SetPadding(RectTransform rect, int horizontal, int vertical)
	{
		LayoutGroup component = rect.GetComponent<LayoutGroup>();
		if (component != null)
		{
			component.padding = new RectOffset(horizontal, horizontal, vertical, vertical);
		}
	}
}
