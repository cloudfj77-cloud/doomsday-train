using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AppraisalItemRegistry
{
	public const string HAND = "hand";

	public const string RIBWICH = "ribwich";

	public const string PLASMA_FUEL = "plasma_fuel";

	private static List<AppraisalItemType> types;

	private static List<AppraisalItemType> Types => types ?? (types = CreateVanilla());

	private static List<AppraisalItemType> CreateVanilla()
	{
		return new List<AppraisalItemType>
		{
			new AppraisalItemType
			{
				id = "hand",
				itemId = "hand",
				unlockRank = 0,
				createRandom = PreBuiltItemHelper.CreateHandRandom,
				isType = (GameItem item) => item.IsTag("HAND_TAG"),
				getCorrectValue = AppHandHelper.GetCorrectValue,
				guideItemId = "hand_guide"
			},
			new AppraisalItemType
			{
				id = "ribwich",
				itemId = "ribwich",
				unlockRank = 1,
				createRandom = PreBuiltItemHelper.CreateRibwichRandom,
				isType = (GameItem item) => item.IsTag("RIBWICH_TAG"),
				getCorrectValue = RibwichHelper.GetCorrectValue,
				guideItemId = "ribwich_guide"
			},
			new AppraisalItemType
			{
				id = "plasma_fuel",
				itemId = "plasma_fuel",
				unlockRank = 2,
				createRandom = () => PreBuiltItemHelper.CreatePlasmaRandom(300),
				isType = (GameItem item) => item.IsTag("PLASMA_CELL_TAG"),
				getCorrectValue = PlasmaHelper.GetCorrectValue,
				guideItemId = "fuel_guide"
			}
		};
	}

	public static IReadOnlyList<AppraisalItemType> GetAll()
	{
		return Types;
	}

	public static AppraisalItemType Get(string id)
	{
		return Types.FirstOrDefault((AppraisalItemType t) => t.id == id);
	}

	public static AppraisalItemType Find(GameItem item)
	{
		return Types.FirstOrDefault((AppraisalItemType t) => t.Matches(item));
	}

	public static void Register(AppraisalItemType type)
	{
		if (type == null || string.IsNullOrEmpty(type.id) || type.createRandom == null || type.getCorrectValue == null)
		{
			Debug.LogError("[AppraisalItemRegistry] An appraisal item needs an id, a createRandom and a getCorrectValue");
			return;
		}
		type.unlockRank = Mathf.Clamp(type.unlockRank, 0, 3);
		Types.RemoveAll((AppraisalItemType t) => t.id == type.id);
		Types.Add(type);
	}

	public static int RemoveAll(string source)
	{
		return Types.RemoveAll((AppraisalItemType t) => t.source == source);
	}

	public static AppraisalItemType GetLowestRankType()
	{
		return Types.OrderBy((AppraisalItemType t) => t.unlockRank).FirstOrDefault();
	}

	public static List<AppraisalItemType> GetUnlocked(int rank)
	{
		return Types.Where((AppraisalItemType t) => t.unlockRank <= rank).ToList();
	}

	public static List<AppraisalItemType> GetUnlockedAt(int rank)
	{
		return Types.Where((AppraisalItemType t) => t.unlockRank == rank).ToList();
	}

	public static int GetNewestUnlockRank(int rank)
	{
		List<AppraisalItemType> unlocked = GetUnlocked(rank);
		if (unlocked.Count != 0)
		{
			return unlocked.Max((AppraisalItemType t) => t.unlockRank);
		}
		return 0;
	}

	public static bool CanBlock(AppraisalItemType type, int rank)
	{
		if (type.unlockRank > rank)
		{
			return false;
		}
		if (rank >= 3)
		{
			return true;
		}
		return type.unlockRank < GetNewestUnlockRank(rank);
	}

	public static bool IsBlocked(AppraisalItemType type, int rank)
	{
		List<string> appraisalBlacklist = PlayerStore.Instance.appraisalBlacklist;
		if (appraisalBlacklist != null && appraisalBlacklist.Contains(type.id))
		{
			return CanBlock(type, rank);
		}
		return false;
	}

	public static bool IsLastAllowed(AppraisalItemType type, int rank)
	{
		if (IsBlocked(type, rank))
		{
			return false;
		}
		return GetUnlocked(rank).Count((AppraisalItemType t) => !IsBlocked(t, rank)) <= 1;
	}

	public static List<AppraisalItemType> GetPool(int rank)
	{
		List<AppraisalItemType> unlocked = GetUnlocked(rank);
		List<AppraisalItemType> list = unlocked.Where((AppraisalItemType t) => !IsBlocked(t, rank)).ToList();
		if (list.Count <= 0)
		{
			return unlocked;
		}
		return list;
	}
}
