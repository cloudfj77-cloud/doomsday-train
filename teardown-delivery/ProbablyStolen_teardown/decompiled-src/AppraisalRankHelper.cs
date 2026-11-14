using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AppraisalRankHelper
{
	public class Rank
	{
		public int baseFee;

		public int[] feePercent;

		public float[] progress;

		public float walkoutProgress;

		public float progressToPromote;

		public float costShare;

		public int attractionChance;

		public string certificateId;
	}

	public const int RANK_JUNIOR = 0;

	public const int RANK_APPRAISER = 1;

	public const int RANK_SENIOR = 2;

	public const int RANK_MASTER = 3;

	public const string PROMOTION_CLIENT_ID = "appraisalBoardPromotion";

	public const int PROMOTION_RETRY_DAYS = 7;

	public const int GRADE_PERFECT = 1;

	public const int GRADE_EXCELLENT = 2;

	public const int GRADE_GOOD = 3;

	public const int GRADE_MEDIOCRE = 4;

	public const int GRADE_AWFUL = 5;

	private static readonly Rank[] Ranks = new Rank[4]
	{
		new Rank
		{
			baseFee = 20,
			feePercent = new int[5] { 15, 12, 10, 8, 5 },
			progress = new float[5] { 10f, 7f, 5f, 4f, 3f },
			walkoutProgress = 0f,
			progressToPromote = 100f,
			costShare = 1f,
			attractionChance = 33,
			certificateId = "cert1"
		},
		new Rank
		{
			baseFee = 40,
			feePercent = new int[5] { 18, 15, 12, 10, 0 },
			progress = new float[5] { 10f, 7f, 5f, 4f, -3f },
			walkoutProgress = -5f,
			progressToPromote = 150f,
			costShare = 1.25f,
			attractionChance = 42,
			certificateId = "cert2"
		},
		new Rank
		{
			baseFee = 60,
			feePercent = new int[5] { 21, 18, 15, 0, 0 },
			progress = new float[5] { 10f, 7f, 5f, -3f, -4f },
			walkoutProgress = -5f,
			progressToPromote = 250f,
			costShare = 1.5f,
			attractionChance = 48,
			certificateId = "cert3"
		},
		new Rank
		{
			baseFee = 100,
			feePercent = new int[5] { 25, 21, 0, 0, 0 },
			progress = new float[5],
			walkoutProgress = 0f,
			progressToPromote = 0f,
			costShare = 1.75f,
			attractionChance = 55,
			certificateId = "cert4"
		}
	};

	public const string APPRAISAL_CUSTOMER_ID = "scavAppraisalRandom";

	public static Rank Current => GetRankData(GetRank());

	public static int GetRank()
	{
		return Mathf.Clamp(PlayerStore.Instance.appraiserLevel, 0, 3);
	}

	public static Rank GetRankData(int rank)
	{
		return Ranks[Mathf.Clamp(rank, 0, 3)];
	}

	public static string GetRankName(int rank)
	{
		return rank switch
		{
			0 => LocHelper.GetLocalizedMechanic("mech_appraisal_rank_junior"), 
			1 => LocHelper.GetLocalizedMechanic("mech_appraisal_rank_appraiser"), 
			2 => LocHelper.GetLocalizedMechanic("mech_appraisal_rank_senior"), 
			_ => LocHelper.GetLocalizedMechanic("mech_appraisal_rank_master"), 
		};
	}

	public static bool IsGradeAccepted(int grade)
	{
		return GetFeePercent(GetRank(), grade) > 0;
	}

	private static int GetFeePercent(int rank, int grade)
	{
		return GetRankData(rank).feePercent[Mathf.Clamp(grade, 1, 5) - 1];
	}

	public static int GetBaseFee()
	{
		return Current.baseFee;
	}

	public static int GetFeePercent(int grade)
	{
		return GetFeePercent(GetRank(), grade);
	}

	public static int GetBaseAttractionChance()
	{
		return Current.attractionChance;
	}

	public static int GetAttractionChance()
	{
		return Mathf.Min(100, Current.attractionChance * (2 + GetDaysMissed()) / 2);
	}

	public static int GetDaysMissed()
	{
		return (PlayerStore.Instance?.storeClientManager?.appraisalDaysMissed).GetValueOrDefault();
	}

	public static void OnCustomerRolled(List<(string, int, string)> prediction, HashSet<string> spawnedIds)
	{
		if (PlayerStore.Instance?.storeClientManager != null && prediction.Exists(((string, int, string) entry) => entry.Item1 == "scavAppraisalRandom"))
		{
			PlayerStore.Instance.storeClientManager.appraisalDaysMissed = ((!spawnedIds.Contains("scavAppraisalRandom")) ? (GetDaysMissed() + 1) : 0);
		}
	}

	public static int GetPromotionCost(int toRank)
	{
		return Mathf.RoundToInt((float)PlayerStore.Instance.GetAppraisalJoinCost() * GetRankData(toRank).costShare);
	}

	public static bool CanProgress()
	{
		if (GetRank() < 3)
		{
			return !PlayerStore.Instance.appraisalPromotionDeclined;
		}
		return false;
	}

	public static float GetThreshold()
	{
		return Current.progressToPromote;
	}

	public static bool IsPromotionReady()
	{
		if (CanProgress())
		{
			return PlayerStore.Instance.appraisalProgress >= (double)GetThreshold();
		}
		return false;
	}

	public static int GetProgressPercent()
	{
		float threshold = GetThreshold();
		if (threshold <= 0f)
		{
			return 100;
		}
		return Mathf.Clamp(Mathf.FloorToInt((float)PlayerStore.Instance.appraisalProgress * 100f / threshold), 0, 100);
	}

	private static void AddProgress(float raw)
	{
		if (raw != 0f && CanProgress() && !IsPromotionReady())
		{
			float num = ((raw > 0f) ? (raw * Difficulty.Rules.appraisalProgressGainMultiplier) : (raw * Difficulty.Rules.appraisalProgressLossMultiplier));
			double num2 = GetThreshold();
			PlayerStore.Instance.appraisalProgress = Math.Max(0.0, Math.Min(num2, PlayerStore.Instance.appraisalProgress + (double)num));
			if (PlayerStore.Instance.appraisalProgress >= num2)
			{
				PlayerStore.Instance.appraisalPromotionDay = StoreStation.Instance.dayCounter + 1;
			}
		}
	}

	public static void OnEstimateAccepted(int grade)
	{
		AddProgress(Current.progress[Mathf.Clamp(grade, 1, 5) - 1]);
	}

	public static void OnEstimateRefused(int grade)
	{
		AddProgress(Mathf.Min(0f, Current.progress[Mathf.Clamp(grade, 1, 5) - 1]));
	}

	public static void OnCustomerWalkout()
	{
		AddProgress(Current.walkoutProgress);
	}

	public static void HandlePromotionClient(StoreClientManager manager)
	{
		if (PlayerStore.Instance.isAppraisalIntroduced && IsPromotionReady())
		{
			int appraisalPromotionDay = PlayerStore.Instance.appraisalPromotionDay;
			if (appraisalPromotionDay >= 0 && StoreStation.Instance.dayCounter >= appraisalPromotionDay)
			{
				manager.AddClient(StoreClientListAppraisal.CreatePromotionOffer());
			}
		}
	}

	public static void Promote()
	{
		int num = Mathf.Min(3, GetRank() + 1);
		PlayerStore.Instance.appraiserLevel = num;
		PlayerStore.Instance.appraisalProgress = 0.0;
		PlayerStore.Instance.appraisalPromotionDay = -1;
		ReplaceCertificate(num);
		foreach (AppraisalItemType item in AppraisalItemRegistry.GetUnlockedAt(num))
		{
			if (!string.IsNullOrEmpty(item.guideItemId) && EmporiumEntry.Instance.GetItemCountById(item.guideItemId) <= 0)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(item.guideItemId, isOwend: true);
			}
		}
	}

	public static bool PromotionUnlocksItems(int toRank)
	{
		return AppraisalItemRegistry.GetUnlockedAt(toRank).Count > 0;
	}

	public static void SetRank(int rank)
	{
		rank = Mathf.Clamp(rank, 0, 3);
		PlayerStore.Instance.isAppraisalIntroduced = true;
		PlayerStore.Instance.appraiserLevel = rank;
		PlayerStore.Instance.appraisalProgress = 0.0;
		PlayerStore.Instance.appraisalPromotionDay = -1;
		PlayerStore.Instance.appraisalPromotionDeclined = false;
		ReplaceCertificate(rank);
		foreach (AppraisalItemType item in AppraisalItemRegistry.GetUnlocked(rank))
		{
			if (!string.IsNullOrEmpty(item.guideItemId) && EmporiumEntry.Instance.GetItemCountById(item.guideItemId) <= 0)
			{
				PlayerStore.Instance.AddDirectSellingItemToTable(item.guideItemId, isOwend: true);
			}
		}
	}

	public static void DeclinePromotionForNow()
	{
		PlayerStore.Instance.appraisalPromotionDay = StoreStation.Instance.dayCounter + 7;
	}

	public static void DeclinePromotionForGood()
	{
		PlayerStore.Instance.appraisalPromotionDeclined = true;
		PlayerStore.Instance.appraisalPromotionDay = -1;
	}

	public static void InitCertificate(GameItem item, int rank)
	{
		rank = Mathf.Clamp(rank, 0, 3);
		item.SetSpriteAndShape("Items/items_misc", GetRankData(rank).certificateId);
		switch (rank)
		{
		case 0:
			item.name = LocHelper.GetLocalizedItem("item_cert1_name");
			item.flavorText = LocHelper.GetLocalizedItem("item_cert1_flavor");
			break;
		case 1:
			item.name = LocHelper.GetLocalizedItem("item_cert2_name");
			break;
		case 2:
			item.name = LocHelper.GetLocalizedItem("item_cert3_name");
			break;
		default:
			item.name = LocHelper.GetLocalizedItem("item_cert4_name");
			break;
		}
		if (rank > 0)
		{
			item.flavorText = LocHelper.GetLocalizedItem("item_appraisal_cert_flavor");
		}
		item.shortDescription = LocHelper.GetLocalizedItem("item_cert1_desc");
		item.unitValue = 0L;
		item.EnableTag("IMPORTANT_TAG");
		item.SetGameItemType("DOCUMENT");
		AudioHelper.InitPaperSheet(item);
		item.canActivateSlotItemFunc = (GameItem self, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(self);
		item.mayActivateSlotItemFunc = (GameItem self, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(self);
		item.onActivateSlotItemFunc = delegate
		{
			AppraisalItemsWindow.Open();
		};
	}

	public static int GetCertificateRank(GameItem item)
	{
		if (item == null)
		{
			return -1;
		}
		for (int i = 0; i <= 3; i++)
		{
			if (item.identifier == Ranks[i].certificateId)
			{
				return i;
			}
		}
		return -1;
	}

	public static bool IsCertificate(GameItem item)
	{
		return GetCertificateRank(item) >= 0;
	}

	public static GameItem CreateCertificate(int rank)
	{
		GameItem gameItem = DirectoryMaster.Item(GetRankData(rank).certificateId);
		GeneralHelper.SetItemOwned(gameItem);
		return gameItem;
	}

	private static List<GameItem> FindOwnedCertificates()
	{
		List<GameItem> allItems = EmporiumEntry.Instance.GetAllItems();
		if (EmporiumEntry.Instance.hirelingInv != null)
		{
			allItems.AddRange(GraphUtils.FindAllChildrenType<GameItem>(EmporiumEntry.Instance.hirelingInv));
		}
		return allItems.Where((GameItem item) => IsCertificate(item) && GeneralHelper.IsItemOwned(item)).ToList();
	}

	public static GameItem GetDisplayedCertificate()
	{
		return EmporiumEntry.Instance.showcaseElement.childItems.FirstOrDefault(IsCertificate);
	}

	private static void ReplaceCertificate(int rank)
	{
		List<GameItem> list = FindOwnedCertificates();
		GraphNodeStorage graphNodeStorage = ((list.Count > 0) ? list[0].parentInventory : null);
		GraphNodeStorage graphNodeStorage2 = ((EmporiumEntry.Instance.trashcan != null && EmporiumEntry.Instance.trashcan.contentWindow != null) ? EmporiumEntry.Instance.trashcan.contentWindow.child : null);
		if (graphNodeStorage == null || graphNodeStorage == graphNodeStorage2)
		{
			graphNodeStorage = EmporiumEntry.Instance.invElement;
		}
		GameItem item = CreateCertificate(rank);
		foreach (GameItem item2 in list)
		{
			item2.Destroy();
		}
		GraphUtils.EmporiumTryAdd(graphNodeStorage, item);
	}

	public static void ReissueLostCertificate()
	{
		if (PlayerStore.Instance.isAppraisalIntroduced && FindOwnedCertificates().Count <= 0)
		{
			GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.invElement, CreateCertificate(GetRank()));
		}
	}

	private static string GradeName(int grade)
	{
		return grade switch
		{
			1 => LocHelper.GetLocalizedMechanic("appraisal_quality_perfect"), 
			2 => LocHelper.GetLocalizedMechanic("appraisal_quality_excellent"), 
			3 => LocHelper.GetLocalizedMechanic("appraisal_quality_good"), 
			4 => LocHelper.GetLocalizedMechanic("appraisal_quality_mediocre"), 
			_ => LocHelper.GetLocalizedMechanic("appraisal_quality_awful"), 
		};
	}

	public static void CreateCertificateTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!IsCertificate(item))
		{
			return;
		}
		int certificateRank = GetCertificateRank(item);
		Rank rankData = GetRankData(certificateRank);
		RenderHandler.ColorPalette colorPalette = RenderHandler.ColorPalette.LightCyan;
		builder.AddLine(" ");
		builder.AddLine(LocHelper.GetLocalizedMechanic("mech_appraisal_cert_base_fee", rankData.baseFee), ignoreEmpty: true, colorPalette);
		for (int i = 1; i <= 5; i++)
		{
			int feePercent = GetFeePercent(certificateRank, i);
			string text = ((feePercent > 0) ? LocHelper.GetLocalizedMechanic("mech_appraisal_cert_grade_fee", GradeName(i), feePercent) : LocHelper.GetLocalizedMechanic("mech_appraisal_cert_grade_refused", GradeName(i)));
			builder.AddLine(LocHelper.BulletLine(text), ignoreEmpty: true, (feePercent > 0) ? colorPalette : RenderHandler.ColorPalette.LightRed);
		}
		bool flag = certificateRank == GetRank();
		int num = (flag ? GetAttractionChance() : rankData.attractionChance);
		builder.AddLine(LocHelper.GetLocalizedMechanic("mech_appraisal_cert_chance", num), ignoreEmpty: true, colorPalette);
		if (flag && GetDaysMissed() > 0 && num < 100)
		{
			builder.AddLine(LocHelper.GetLocalizedMechanicOrDefault("mech_appraisal_cert_chance_ramp", "Rises every day without one, from {0}%", rankData.attractionChance), ignoreEmpty: true, colorPalette);
		}
		builder.AddLine(LocHelper.GetLocalizedMechanic("mech_appraisal_cert_qualified"), ignoreEmpty: true, colorPalette);
		foreach (AppraisalItemType item2 in AppraisalItemRegistry.GetUnlocked(certificateRank))
		{
			builder.AddLine(LocHelper.BulletLine(item2.GetDisplayName()), ignoreEmpty: true, colorPalette);
		}
		if (certificateRank == GetRank())
		{
			if (IsPromotionReady())
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_appraisal_cert_progress_ready"), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
			else if (CanProgress())
			{
				builder.AddLine(LocHelper.GetLocalizedMechanic("mech_appraisal_cert_progress", GetProgressPercent()), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_appraisal_cert_activate"), ignoreEmpty: true, colorPalette);
		}
	}
}
