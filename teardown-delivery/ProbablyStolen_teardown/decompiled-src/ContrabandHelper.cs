using System;
using System.Collections.Generic;

public class ContrabandHelper
{
	public const string CONTRABAND_ITEM_TAG = "CONTRABAND_ITEM_TAG";

	public const string ITEM_HIDDEN_TAG = "ITEM_HIDDEN_TAG";

	public const string CONTRABAND_LEVEL = "CONTRABAND_LEVEL";

	public const string CONTRABAND_LEVEL_NONE = "CONTRABAND_LEVEL_NONE";

	public const string CONTRABAND_LEVEL_LOW = "CONTRABAND_LEVEL_LOW";

	public const string CONTRABAND_LEVEL_MID = "CONTRABAND_LEVEL_MID";

	public const string CONTRABAND_LEVEL_HIGH = "CONTRABAND_LEVEL_HIGH";

	public const string CONTRABAND_LEVEL_CRITICAL = "CONTRABAND_LEVEL_CRITICAL";

	public const int INSPECTION_SLIP_LISTED_ITEMS = 3;

	public static void InitContrabandItem(GameItem item)
	{
		item.SetGameItemType("CONTRABAND");
		item.EnableTag("CONTRABAND_ITEM_TAG");
		item.ModifyTag("CONTRABAND_LEVEL", delegate(TagState state)
		{
			state.Enable().SetString("CONTRABAND_LEVEL_LOW");
		});
	}

	public static void RemoveContrabandStatus(GameItem item)
	{
		item.ModifyTag("CONTRABAND_LEVEL", delegate(TagState state)
		{
			state.Enable().SetString("CONTRABAND_LEVEL_NONE");
		});
		item.DisableTag("CONTRABAND_ITEM_TAG");
		item.DisableTag("CONTRABAND_LEVEL");
		item.RemoveGameItemType("CONTRABAND");
	}

	public static void InitContrabandItem(GameItem item, int level)
	{
		item.EnableTag("CONTRABAND_ITEM_TAG");
		item.SetGameItemType("CONTRABAND");
		switch (level)
		{
		case 1:
			item.ModifyTag("CONTRABAND_LEVEL", delegate(TagState state)
			{
				state.Enable().SetString("CONTRABAND_LEVEL_LOW");
			});
			item.ModifyTag("CONTRABAND_LEVEL", delegate(TagState state)
			{
				state.Enable().SetInt(1);
			});
			break;
		case 2:
			item.ModifyTag("CONTRABAND_LEVEL", delegate(TagState state)
			{
				state.Enable().SetString("CONTRABAND_LEVEL_MID");
			});
			item.ModifyTag("CONTRABAND_LEVEL", delegate(TagState state)
			{
				state.Enable().SetInt(2);
			});
			break;
		case 3:
			item.ModifyTag("CONTRABAND_LEVEL", delegate(TagState state)
			{
				state.Enable().SetString("CONTRABAND_LEVEL_HIGH");
			});
			item.ModifyTag("CONTRABAND_LEVEL", delegate(TagState state)
			{
				state.Enable().SetInt(3);
			});
			break;
		case 4:
			item.ModifyTag("CONTRABAND_LEVEL", delegate(TagState state)
			{
				state.Enable().SetString("CONTRABAND_LEVEL_CRITICAL");
			});
			item.ModifyTag("CONTRABAND_LEVEL", delegate(TagState state)
			{
				state.Enable().SetInt(4);
			});
			break;
		}
	}

	public static void CreateContrabandItemTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("CONTRABAND_ITEM_TAG"))
		{
			if (item.GetTagReadonly("CONTRABAND_LEVEL").GetString().Equals("CONTRABAND_LEVEL_LOW"))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_contraband_low"), ignoreEmpty: true, RenderHandler.ColorPalette.LightRed, bold: true, italic: false, underline: false, strikethrough: false, RenderHandler.ColorPalette.Black);
			}
			else if (item.GetTagReadonly("CONTRABAND_LEVEL").GetString().Equals("CONTRABAND_LEVEL_MID"))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_contraband_mid"), ignoreEmpty: true, RenderHandler.ColorPalette.LightRed, bold: true, italic: false, underline: false, strikethrough: false, RenderHandler.ColorPalette.Black);
			}
			else if (item.GetTagReadonly("CONTRABAND_LEVEL").GetString().Equals("CONTRABAND_LEVEL_HIGH"))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_contraband_high"), ignoreEmpty: true, RenderHandler.ColorPalette.LightRed, bold: true, italic: false, underline: false, strikethrough: false, RenderHandler.ColorPalette.Black);
			}
			else if (item.GetTagReadonly("CONTRABAND_LEVEL").GetString().Equals("CONTRABAND_LEVEL_CRITICAL"))
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_contraband_critical"), ignoreEmpty: true, RenderHandler.ColorPalette.LightRed, bold: true, italic: false, underline: false, strikethrough: false, RenderHandler.ColorPalette.Black);
			}
		}
	}

	public static void StartInspection(int personality, bool fromShowcase = false)
	{
		bool flag = false;
		if (PlayerStore.Instance.currentClientInstance != null && PlayerStore.Instance.currentClientInstance.GetClientBlueprint().identifier.Equals("wanted9"))
		{
			Debug.Log("Fake Inspector detected");
			flag = true;
		}
		Debug.Log("Starting Inspection");
		List<GameItem> list = new List<GameItem>();
		List<GameItem> list2 = new List<GameItem>();
		if (PlayerStore.Instance.currentClientInstance.GetClientBlueprint().isLazyInspector)
		{
			personality = 2;
		}
		foreach (GameItem childItem in EmporiumEntry.Instance.invElement.childItems)
		{
			RecursiveInspect(childItem, list, list2);
		}
		foreach (GameItem childItem2 in EmporiumEntry.Instance.frontInvinvElement.childItems)
		{
			RecursiveInspect(childItem2, list, list2);
		}
		foreach (GameItem childItem3 in EmporiumEntry.Instance.backInvinvElement.childItems)
		{
			RecursiveInspect(childItem3, list, list2);
		}
		foreach (GameItem childItem4 in EmporiumEntry.Instance.showcaseElement.childItems)
		{
			RecursiveInspect(childItem4, list, list2);
		}
		if (StorePoster.Instance.isPulled)
		{
			foreach (GameItem childItem5 in EmporiumEntry.Instance.hiddenElement.childItems)
			{
				RecursiveInspect(childItem5, list, list2);
			}
		}
		else
		{
			CollectConcealed(EmporiumEntry.Instance.hiddenElement, list2);
		}
		if (personality != 2)
		{
			foreach (GameItem childItem6 in EmporiumEntry.Instance.trashInvElement.childItems)
			{
				RecursiveInspect(childItem6, list, list2);
			}
		}
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (!IsContraband(list[num]) && RNG.Roll(100 - StolenHelper.GetFoundChance(list[num])))
			{
				Debug.Log("Removing Stolen Item: " + list[num].name);
				list.RemoveAt(num);
			}
		}
		bool flag2 = false;
		if (list.Count > 0 && HealthData.RollLuck())
		{
			flag2 = true;
			Debug.Log("Luck saved the inspection, " + list.Count + " item(s) overlooked");
			list.Clear();
			StoreUIManager.Instance.Notify(LocHelper.GetLocalizedMechanic("mech_inspection_luck_saved"));
		}
		int num2 = 0;
		foreach (GameItem item in list)
		{
			num2++;
			num2 += GetContrabandLevel(item);
			if (item.IsTag("STOLEN_TAG") && item.GetBaseValue() > 17)
			{
				num2 = ((item.GetBaseValue() >= 50) ? ((item.GetBaseValue() >= 85) ? ((item.GetBaseValue() >= 140) ? (num2 + 4) : (num2 + 3)) : (num2 + 2)) : (num2 + 1));
			}
		}
		if (num2 > 25)
		{
			num2 = 25;
		}
		string text = GetHighestContrabandLevel(list);
		Debug.Log("Contraband Level: " + text);
		int fineAmount = GetFineAmount(list);
		if (flag)
		{
			num2 = 0;
			if (text.Equals("CONTRABAND_LEVEL_CRITICAL") || text.Equals("CONTRABAND_LEVEL_HIGH"))
			{
				text = "CONTRABAND_LEVEL_MID";
			}
		}
		if (!text.Equals("CONTRABAND_LEVEL_CRITICAL") && !text.Equals("CONTRABAND_LEVEL_HIGH") && PlayerStore.Instance.currentClientInstance != null)
		{
			PlayerStore.Instance.currentClientInstance.GetClientBlueprint().dismissable = true;
		}
		if (text.Equals("CONTRABAND_LEVEL_CRITICAL"))
		{
			if (NetworkUpgrade.IsSecGraceUnlocked())
			{
				StoreReputation.ModSecReputation(-num2);
				DialogUIManager.Instance.StartDialogue(StoreClientDialogList.SecurityDialogInspectionGrace());
			}
			else
			{
				DialogUIManager.Instance.StartDialogue(StoreClientDialogList.SecurityDialogInspectionArrest());
			}
		}
		else if (text.Equals("CONTRABAND_LEVEL_HIGH"))
		{
			StoreReputation.ModSecReputation(-num2);
			DialogUIManager.Instance.StartDialogue(StoreClientDialogList.SecurityDialogInspectionFine(fineAmount));
		}
		else if (text.Equals("CONTRABAND_LEVEL_MID"))
		{
			StoreReputation.ModSecReputation(-num2);
			DialogUIManager.Instance.StartDialogue(StoreClientDialogList.SecurityDialogInspectionSeizure());
		}
		else if (text.Equals("STOLEN_TAG"))
		{
			StoreReputation.ModSecReputation(-num2);
			DialogUIManager.Instance.StartDialogue(StoreClientDialogList.SecurityDialogInspectionSeizureStolen());
		}
		else if (text.Equals("CONTRABAND_LEVEL_LOW"))
		{
			StoreReputation.ModSecReputation(-num2);
			DialogUIManager.Instance.StartDialogue(StoreClientDialogList.SecurityDialogInspectionWarning());
		}
		else
		{
			if (!fromShowcase && list2.Count == 0 && !flag2 && !flag)
			{
				StoreReputation.ModSecReputation(3);
			}
			else if (list2.Count > 0)
			{
				Debug.Log("Inspection cleared but " + list2.Count + " item(s) were concealed, no reputation gain");
			}
			DialogUIManager.Instance.StartDialogue(StoreClientDialogList.SecurityDialogInspectionCleared());
		}
		bool flag3 = text.Equals("CONTRABAND_LEVEL_HIGH") && PlayerStore.Instance.playerCash < fineAmount && NetworkUpgrade.IsSecGraceUnlocked();
		bool usedGrace = (text.Equals("CONTRABAND_LEVEL_CRITICAL") && NetworkUpgrade.IsSecGraceUnlocked()) || flag3;
		int num3 = 0;
		foreach (GameItem item2 in list)
		{
			num3 += (int)item2.GetNegociatedValue();
		}
		EmporiumAnalytics.TrackOnInspected(text, list.Count, num3, usedGrace);
		int num4;
		int seizedValue;
		if (!text.Equals("CONTRABAND_LEVEL_NONE"))
		{
			num4 = (text.Equals("CONTRABAND_LEVEL_LOW") ? 1 : 0);
			if (num4 == 0)
			{
				seizedValue = num3;
				goto IL_05f1;
			}
		}
		else
		{
			num4 = 1;
		}
		seizedValue = 0;
		goto IL_05f1;
		IL_05f1:
		RunStats.OnInspection((byte)num4 != 0, seizedValue);
		if (!text.Equals("CONTRABAND_LEVEL_NONE"))
		{
			PlayerStore.Instance.lastInspectionLossDay = StoreStation.Instance.dayCounter;
			PlayerStore.Instance.lastInspectionLossValue = num3 + ((text.Equals("CONTRABAND_LEVEL_HIGH") && !flag3) ? fineAmount : 0);
		}
		bool flag4 = (text.Equals("CONTRABAND_LEVEL_CRITICAL") || (text.Equals("CONTRABAND_LEVEL_HIGH") && PlayerStore.Instance.playerCash < fineAmount)) && !NetworkUpgrade.IsSecGraceUnlocked();
		int finePaid = ((text.Equals("CONTRABAND_LEVEL_HIGH") && PlayerStore.Instance.playerCash >= fineAmount) ? fineAmount : 0);
		string text2 = ((flag || flag4) ? null : BuildInspectionSlipBody(GetSeizedItems(list), finePaid));
		DisposeContraband(list);
		if (text2 != null)
		{
			LeaveInspectionSlip(text2);
		}
	}

	public static List<GameItem> GetSeizedItems(List<GameItem> contraband)
	{
		List<GameItem> list = new List<GameItem>();
		foreach (GameItem item in contraband)
		{
			if (!item.GetTagReadonly("CONTRABAND_LEVEL").GetString().Equals("CONTRABAND_LEVEL_LOW"))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static string BuildInspectionSlipBody(List<GameItem> seized, int finePaid)
	{
		if (seized.Count == 0)
		{
			return null;
		}
		List<GameItem> list = new List<GameItem>(seized);
		list.Sort((GameItem a, GameItem b) => b.GetNegociatedValue().CompareTo(a.GetNegociatedValue()));
		List<string> list2 = new List<string>();
		for (int i = 0; i < list.Count && i < 3; i++)
		{
			list2.Add(LocHelper.GetLocalizedMechanicOrDefault("mech_inspection_slip_seized_line", "{0} ({1} Credits)", list[i].name, (int)list[i].GetNegociatedValue()));
		}
		string text = string.Join(LocHelper.GetLocalizedMechanicOrDefault("mech_inspection_slip_separator", ", "), list2);
		if (list.Count > 3)
		{
			text = LocHelper.GetLocalizedMechanicOrDefault("mech_inspection_slip_and_more", "{0} and {1} more", text, list.Count - 3);
		}
		string text2 = ((seized.Count == 1) ? LocHelper.GetLocalizedMechanicOrDefault("mech_inspection_slip_confiscated_one", "1 item confiscated: {0}.", text) : LocHelper.GetLocalizedMechanicOrDefault("mech_inspection_slip_confiscated_many", "{0} items confiscated: {1}.", seized.Count, text));
		if (finePaid > 0)
		{
			text2 += " ";
			text2 += LocHelper.GetLocalizedMechanic("mech_report_fine", finePaid).Trim();
		}
		text2 += "\n\n";
		return text2 + LocHelper.GetLocalizedMechanic("mech_report_rep_decreased");
	}

	public static void LeaveInspectionSlip(string body)
	{
		GameItem gameItem = DirectoryMaster.Item("sec_slip");
		if (gameItem != null)
		{
			gameItem.ModifyTag("title", delegate(TagState state)
			{
				state.Enable().SetString(LocHelper.GetLocalizedMechanicOrDefault("mech_inspection_slip_title", "Inspection Report"));
			});
			gameItem.ModifyTag("body", delegate(TagState state)
			{
				state.Enable().SetString(body);
			});
			PlayerStore.Instance.AddDirectSellingItemToTable(gameItem, isOwend: true);
		}
	}

	public static void RecursiveInspect(GameItem gameItem, List<GameItem> contraband, List<GameItem> concealed = null)
	{
		if (IsBlockSearchContainer(gameItem))
		{
			CollectConcealed(gameItem, concealed);
			return;
		}
		if (InspectItem(gameItem))
		{
			contraband.Add(gameItem);
		}
		else if (IsConcealedFromInspection(gameItem))
		{
			AddConcealed(gameItem, concealed);
		}
		foreach (GameItem item in GraphUtils.FindChildrenType<GameItem>(gameItem))
		{
			RecursiveInspect(item, contraband, concealed);
		}
	}

	public static bool IsFlaggableItem(GameItem gameItem)
	{
		if (!gameItem.IsTag("CONTRABAND_ITEM_TAG") && !gameItem.IsTag("STOLEN_TAG"))
		{
			return false;
		}
		if (gameItem.GetTagReadonly("CONTRABAND_LEVEL").GetString().Equals("CONTRABAND_LEVEL_NONE") && !gameItem.IsTag("STOLEN_TAG"))
		{
			return false;
		}
		return true;
	}

	public static bool IsConcealedFromInspection(GameItem gameItem)
	{
		if (!IsFlaggableItem(gameItem))
		{
			return false;
		}
		if (!gameItem.IsTag("ITEM_BEHIND_LOCK_TAG") && !gameItem.IsTag("ITEM_HIDDEN_TAG"))
		{
			return gameItem.IsTag("LOCKED_TAG");
		}
		return true;
	}

	public static void CollectConcealed(GraphNodeStorage node, List<GameItem> concealed)
	{
		if (concealed == null || node == null)
		{
			return;
		}
		foreach (GameItem item in GraphUtils.FindAllChildrenType<GameItem>(node))
		{
			if (IsFlaggableItem(item))
			{
				AddConcealed(item, concealed);
			}
		}
	}

	private static void AddConcealed(GameItem gameItem, List<GameItem> concealed)
	{
		if (concealed != null && !concealed.Contains(gameItem))
		{
			Debug.Log("Concealed from inspection: " + gameItem.name);
			concealed.Add(gameItem);
		}
	}

	public static int GetHighestContrabandLevelInt(List<GameItem> contraband)
	{
		int num = 0;
		foreach (GameItem item in contraband)
		{
			int @int = item.GetTagReadonly("CONTRABAND_LEVEL").GetInt();
			if (@int > num)
			{
				num = @int;
			}
		}
		return num;
	}

	public static string GetHighestContrabandLevel(List<GameItem> contraband)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		foreach (GameItem item in contraband)
		{
			string @string = item.GetTagReadonly("CONTRABAND_LEVEL").GetString();
			if (@string.Equals("CONTRABAND_LEVEL_CRITICAL"))
			{
				flag = true;
			}
			if (@string.Equals("CONTRABAND_LEVEL_HIGH"))
			{
				flag2 = true;
			}
			if (@string.Equals("CONTRABAND_LEVEL_MID"))
			{
				flag3 = true;
			}
			if (@string.Equals("CONTRABAND_LEVEL_LOW"))
			{
				flag4 = true;
			}
			if (StolenHelper.IsStolenItem(item))
			{
				flag5 = true;
			}
		}
		if (flag)
		{
			return "CONTRABAND_LEVEL_CRITICAL";
		}
		if (flag2)
		{
			return "CONTRABAND_LEVEL_HIGH";
		}
		if (flag3)
		{
			return "CONTRABAND_LEVEL_MID";
		}
		if (flag5)
		{
			return "STOLEN_TAG";
		}
		if (flag4)
		{
			return "CONTRABAND_LEVEL_LOW";
		}
		return "CONTRABAND_LEVEL_NONE";
	}

	public static int GetFineAmount(List<GameItem> allItems)
	{
		int num = 0;
		foreach (GameItem allItem in allItems)
		{
			num += (int)allItem.unitValue;
		}
		return num;
	}

	public static bool InspectItem(GameItem gameItem)
	{
		if (gameItem.IsTag("ITEM_BEHIND_LOCK_TAG"))
		{
			return false;
		}
		if (gameItem.IsTag("ITEM_HIDDEN_TAG"))
		{
			return false;
		}
		if (gameItem.IsTag("LOCKED_TAG"))
		{
			return false;
		}
		if (!gameItem.IsTag("CONTRABAND_ITEM_TAG") && !gameItem.IsTag("STOLEN_TAG"))
		{
			return false;
		}
		if (gameItem.GetTagReadonly("CONTRABAND_LEVEL").GetString().Equals("CONTRABAND_LEVEL_NONE") && !gameItem.IsTag("STOLEN_TAG"))
		{
			return false;
		}
		Debug.Log("Found During Inspection Item " + gameItem.name);
		return true;
	}

	public static void DisposeContraband(List<GameItem> allItems)
	{
		EmporiumEntry.Instance.DeselectDragItem();
		int count = allItems.Count;
		while (count-- > 0)
		{
			if (!allItems[count].GetTagReadonly("CONTRABAND_LEVEL").GetString().Equals("CONTRABAND_LEVEL_LOW"))
			{
				allItems[count].Destroy();
			}
		}
	}

	public static int GetBribeAmount()
	{
		return Math.Clamp(100 + StoreStation.Instance.dayCounter * 2 + StoreReputation.GetSecReputation() * -2, 40, 400);
	}

	public static void OnShowContrabandItemToOfficier(GameItem item)
	{
		if ((PlayerStore.Instance.currentClientInstance == null || PlayerStore.Instance.currentClientInstance.GetClientBlueprint().isSecurity) && item.IsTag("CONTRABAND_ITEM_TAG") && item.GetTagReadonly("CONTRABAND_LEVEL").GetString() == "CONTRABAND_LEVEL_LOW")
		{
			DialogUIManager.Instance.StartDialogue(StoreClientDialogList.SecurityOnShowLowLevel());
		}
	}

	public static bool IsOnBadTermWithSecurity()
	{
		return false;
	}

	public static bool IsSeriousContraband(GameItem gameItem)
	{
		string @string = gameItem.GetTagReadonly("CONTRABAND_LEVEL").GetString();
		if (@string.Equals("CONTRABAND_LEVEL_LOW"))
		{
			return false;
		}
		if (@string.Equals("CONTRABAND_LEVEL_MID"))
		{
			return true;
		}
		if (@string.Equals("CONTRABAND_LEVEL_HIGH"))
		{
			return true;
		}
		if (@string.Equals("CONTRABAND_LEVEL_CRITICAL"))
		{
			return true;
		}
		return false;
	}

	public static int GetReportReward(List<GameItem> items)
	{
		int num = 0;
		foreach (GameItem item in items)
		{
			if (item.IsTag("CONTRABAND_ITEM_TAG"))
			{
				switch (item.GetTagReadonly("CONTRABAND_LEVEL").GetInt())
				{
				case 2:
					num += 25;
					break;
				case 3:
					num += 40;
					break;
				case 4:
					num += 60;
					break;
				}
			}
			if (StolenHelper.IsStolenItem(item))
			{
				num += RNG.MultiplyAndRound((int)item.GetNegociatedValue(), 0.03999999910593033);
			}
		}
		return num;
	}

	public static List<GameItem> FindAllContrabandItem(GameItem ship, bool includeHiddenItem = false)
	{
		List<GameItem> list = GraphUtils.FindAllChildrenType<GameItem>(ship);
		List<GameItem> list2 = new List<GameItem>();
		foreach (GameItem item in list)
		{
			if (includeHiddenItem)
			{
				if (item.IsTag("CONTRABAND_ITEM_TAG"))
				{
					list2.Add(item);
				}
			}
			else if (item.IsTag("CONTRABAND_ITEM_TAG") && !item.IsTag("ITEM_HIDDEN_TAG"))
			{
				list2.Add(item);
			}
		}
		return list2;
	}

	public static int GetAllContrabandItemValue(GameItem ship, bool includeHiddenItem = false, List<GameItem> contraband = null)
	{
		List<GameItem> list = ((contraband != null) ? contraband : FindAllContrabandItem(ship, includeHiddenItem));
		long num = 0L;
		foreach (GameItem item in list)
		{
			num += item.GetValue();
		}
		return (int)num;
	}

	public static bool CheckIfContrabandWasFound(GameItem ship, int contraBandValue)
	{
		int num = 0;
		if (contraBandValue <= 100)
		{
			num = 10;
		}
		else if (contraBandValue <= 400)
		{
			num = 25;
		}
		else if (contraBandValue <= 750)
		{
			num = 35;
		}
		else if (contraBandValue <= 1000)
		{
			num = 50;
		}
		else
		{
			if (contraBandValue > 2000)
			{
				return true;
			}
			num = 90;
		}
		return RNG.Roll(num);
	}

	public static int GetContrabandValue(List<GameItem> allItems)
	{
		int num = 0;
		foreach (GameItem allItem in allItems)
		{
			if (allItem.IsTag("CONTRABAND_ITEM_TAG"))
			{
				num = (int)(num + allItem.unitValue);
			}
		}
		return num;
	}

	public static int GetContrabandLevel(GameItem gameItem)
	{
		if (!gameItem.IsTag("CONTRABAND_ITEM_TAG"))
		{
			return 0;
		}
		if (gameItem.GetTagReadonly("CONTRABAND_LEVEL").GetString().Equals("CONTRABAND_LEVEL_NONE"))
		{
			return 0;
		}
		string @string = gameItem.GetTagReadonly("CONTRABAND_LEVEL").GetString();
		if (@string.Equals("CONTRABAND_LEVEL_LOW"))
		{
			return 1;
		}
		if (@string.Equals("CONTRABAND_LEVEL_MID"))
		{
			return 2;
		}
		if (@string.Equals("CONTRABAND_LEVEL_HIGH"))
		{
			return 3;
		}
		if (@string.Equals("CONTRABAND_LEVEL_CRITICAL"))
		{
			return 4;
		}
		return 0;
	}

	public static int GetRestrictedLevel(GameItem gameItem)
	{
		if (gameItem.IsGameItemType("RESTRICTED_1"))
		{
			return 1;
		}
		if (gameItem.IsGameItemType("RESTRICTED_2"))
		{
			return 2;
		}
		if (gameItem.IsGameItemType("RESTRICTED_3"))
		{
			return 3;
		}
		return 0;
	}

	public static bool IsCounterContainContraband()
	{
		foreach (GameItem childItem in EmporiumEntry.Instance.frontInvinvElement.childItems)
		{
			if (childItem.IsTag("CONTRABAND_ITEM_TAG"))
			{
				return true;
			}
		}
		foreach (GameItem childItem2 in EmporiumEntry.Instance.backInvinvElement.childItems)
		{
			if (childItem2.IsTag("CONTRABAND_ITEM_TAG"))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsCounterContainStolen()
	{
		foreach (GameItem childItem in EmporiumEntry.Instance.frontInvinvElement.childItems)
		{
			if (childItem.IsTag("STOLEN_TAG"))
			{
				return true;
			}
		}
		foreach (GameItem childItem2 in EmporiumEntry.Instance.backInvinvElement.childItems)
		{
			if (childItem2.IsTag("STOLEN_TAG"))
			{
				return true;
			}
		}
		return false;
	}

	public static GameItem SecurityCheckCounter()
	{
		foreach (GameItem childItem in EmporiumEntry.Instance.frontInvinvElement.childItems)
		{
			if (childItem.IsTag("CONTRABAND_ITEM_TAG"))
			{
				return childItem;
			}
		}
		foreach (GameItem childItem2 in EmporiumEntry.Instance.backInvinvElement.childItems)
		{
			if (childItem2.IsTag("CONTRABAND_ITEM_TAG"))
			{
				return childItem2;
			}
		}
		foreach (GameItem childItem3 in EmporiumEntry.Instance.frontInvinvElement.childItems)
		{
			if (childItem3.IsTag("STOLEN_TAG") && StolenHelper.IsFound(childItem3))
			{
				StolenHelper.SetHeat(childItem3, 100);
				return childItem3;
			}
		}
		foreach (GameItem childItem4 in EmporiumEntry.Instance.backInvinvElement.childItems)
		{
			if (childItem4.IsTag("STOLEN_TAG") && StolenHelper.IsFound(childItem4))
			{
				StolenHelper.SetHeat(childItem4, 100);
				return childItem4;
			}
		}
		return null;
	}

	public static GameItem SecurityCheckDisplayCase()
	{
		foreach (GameItem childItem in EmporiumEntry.Instance.showcaseElement.childItems)
		{
			if (childItem.IsTag("CONTRABAND_ITEM_TAG"))
			{
				return childItem;
			}
		}
		foreach (GameItem childItem2 in EmporiumEntry.Instance.showcaseElement.childItems)
		{
			if (childItem2.IsTag("STOLEN_TAG") && StolenHelper.IsFound(childItem2))
			{
				StolenHelper.SetHeat(childItem2, 100);
				return childItem2;
			}
		}
		return null;
	}

	public static bool IsBlockSearchContainer(GameItem gameItem)
	{
		if (gameItem.IsTag("ITEM_HIDDEN_TAG"))
		{
			return true;
		}
		return IsSmugglerBay(gameItem);
	}

	public static bool IsSmugglerBay(GameItem gameItem)
	{
		if (!(gameItem.identifier == "smuggler_bay") && !(gameItem.identifier == "smuggler_bay_mod"))
		{
			return gameItem.identifier == "smuggler_bay_mini";
		}
		return true;
	}

	public static void ApplyContrabandFeature(GameItem gameItem)
	{
		if (gameItem.IsTag("CONTRABAND_ITEM_TAG"))
		{
			string @string = gameItem.GetTagReadonly("CONTRABAND_LEVEL").GetString();
			if (@string.Equals("CONTRABAND_LEVEL_LOW"))
			{
				gameItem.AddItemFeature(ItemFeatureList.ContrabandLowMarkUp());
			}
			else if (@string.Equals("CONTRABAND_LEVEL_MID"))
			{
				gameItem.AddItemFeature(ItemFeatureList.ContrabandMidMarkUp());
			}
			else if (@string.Equals("CONTRABAND_LEVEL_HIGH"))
			{
				gameItem.AddItemFeature(ItemFeatureList.ContrabandHighMarkUp());
			}
			else if (@string.Equals("CONTRABAND_LEVEL_CRITICAL"))
			{
				gameItem.AddItemFeature(ItemFeatureList.ContrabandCriticalMarkUp());
			}
		}
	}

	public static bool IsContraband(GameItem gameItem)
	{
		if (gameItem.IsTag("CONTRABAND_ITEM_TAG"))
		{
			return true;
		}
		return false;
	}

	public static int GetContrabandLevelInInt(GameItem gameItem)
	{
		if (!gameItem.IsTag("CONTRABAND_ITEM_TAG"))
		{
			return 0;
		}
		if (gameItem.GetTagReadonly("CONTRABAND_LEVEL").GetString().Equals("CONTRABAND_LEVEL_NONE"))
		{
			return 0;
		}
		string @string = gameItem.GetTagReadonly("CONTRABAND_LEVEL").GetString();
		if (@string.Equals("CONTRABAND_LEVEL_LOW"))
		{
			return 1;
		}
		if (@string.Equals("CONTRABAND_LEVEL_MID"))
		{
			return 2;
		}
		if (@string.Equals("CONTRABAND_LEVEL_HIGH"))
		{
			return 3;
		}
		if (@string.Equals("CONTRABAND_LEVEL_CRITICAL"))
		{
			return 4;
		}
		return 0;
	}
}
