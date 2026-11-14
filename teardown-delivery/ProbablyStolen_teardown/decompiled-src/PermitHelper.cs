using System.Collections.Generic;

public class PermitHelper
{
	public const string PERMIT_ITEM_TAG = "PERMIT_ITEM_TAG";

	public const string PERMIT_ITEM_TYPE = "PERMIT_ITEM_TYPE";

	public const string PERMIT_ITEM_LEVEL = "PERMIT_ITEM_LEVEL";

	public const string PERMIT_ITEM_COUNT = "PERMIT_ITEM_COUNT";

	public static void OnPermitGranted(int contrabandLevel, string itemType)
	{
		foreach (GameItem allOwnedItem in EmporiumEntry.Instance.GetAllOwnedItems())
		{
			if (contrabandLevel == ContrabandHelper.GetContrabandLevel(allOwnedItem) && IsCoveredType(allOwnedItem, itemType))
			{
				SetRestrictedClass(allOwnedItem);
				ContrabandHelper.RemoveContrabandStatus(allOwnedItem);
			}
		}
		PlayerStore.Instance.ActivePermits.Add((itemType, contrabandLevel));
		if (itemType == "FIREARM" && contrabandLevel == 1)
		{
			Shotgun.OnGunPermitIGranted();
		}
	}

	public static bool IsCoveredType(GameItem gameItem, string itemType)
	{
		if (gameItem.IsGameItemType(itemType))
		{
			return true;
		}
		if (itemType == "FIREARM")
		{
			return gameItem.IsTag("GUN_FRAME_TAG");
		}
		return false;
	}

	public static void ReapplyPermits()
	{
		if (EmporiumEntry.Instance == null || PlayerStore.Instance?.ActivePermits == null)
		{
			return;
		}
		foreach (GameItem allOwnedItem in EmporiumEntry.Instance.GetAllOwnedItems())
		{
			OnNewItemCreated(allOwnedItem);
		}
	}

	public static void GrantedFromPermit(GameItem permit)
	{
		if (!permit.IsTag("PERMIT_ITEM_TAG"))
		{
			return;
		}
		int @int = permit.GetTagReadonly("PERMIT_ITEM_LEVEL").GetInt();
		int int2 = permit.GetTagReadonly("PERMIT_ITEM_COUNT").GetInt();
		for (int i = 1; i <= int2; i++)
		{
			string tagName = $"PERMIT_ITEM_TAG{i}";
			string @string = permit.GetTagReadonly(tagName).GetString();
			foreach (GameItem allOwnedItem in EmporiumEntry.Instance.GetAllOwnedItems())
			{
				if (ContrabandHelper.GetContrabandLevel(allOwnedItem) == @int && allOwnedItem.IsGameItemType(@string))
				{
					SetRestrictedClass(allOwnedItem);
					ContrabandHelper.RemoveContrabandStatus(allOwnedItem);
				}
			}
			PlayerStore.Instance.ActivePermits.Add((@string, @int));
			if (@string == "FIREARM" && @int == 1)
			{
				Shotgun.OnGunPermitIGranted();
			}
		}
	}

	public static void OnPermitRevoked(int contrabandLevel, string itemType)
	{
		foreach (GameItem allOwnedItem in EmporiumEntry.Instance.GetAllOwnedItems())
		{
			if (contrabandLevel == ContrabandHelper.GetRestrictedLevel(allOwnedItem) && allOwnedItem.IsGameItemType(itemType))
			{
				RemoveRestrictedClass(allOwnedItem);
			}
		}
		PlayerStore.Instance.ActivePermits.RemoveAll(((string, int) permit) => permit.Item1 == itemType && permit.Item2 == contrabandLevel);
		FindAndDeletePermitItem(contrabandLevel, itemType);
		if (itemType == "FIREARM" && contrabandLevel == 1)
		{
			Shotgun.OnGunPermitIRevoked();
		}
	}

	public static void RemoveRestrictedClass(GameItem gameItem)
	{
		if (gameItem.IsGameItemType("RESTRICTED_1"))
		{
			ContrabandHelper.InitContrabandItem(gameItem, 1);
			gameItem.RemoveGameItemType("RESTRICTED_1");
		}
		else if (gameItem.IsGameItemType("RESTRICTED_2"))
		{
			ContrabandHelper.InitContrabandItem(gameItem, 2);
			gameItem.RemoveGameItemType("RESTRICTED_2");
		}
		else if (gameItem.IsGameItemType("RESTRICTED_3"))
		{
			ContrabandHelper.InitContrabandItem(gameItem, 3);
			gameItem.RemoveGameItemType("RESTRICTED_3");
		}
	}

	public static bool HavePermit(string itemType, int contrabandLevel)
	{
		return PlayerStore.Instance.ActivePermits.Exists(((string, int) permit) => permit.Item1 == itemType && permit.Item2 == contrabandLevel);
	}

	public static void SetRestrictedClass(GameItem gameItem)
	{
		switch (ContrabandHelper.GetContrabandLevel(gameItem))
		{
		case 1:
			gameItem.SetGameItemType("RESTRICTED_1");
			break;
		case 2:
			gameItem.SetGameItemType("RESTRICTED_2");
			break;
		case 3:
			gameItem.SetGameItemType("RESTRICTED_3");
			break;
		}
	}

	public static void ApplyRestrictedFeature(GameItem gameItem)
	{
		if (gameItem.IsGameItemType("RESTRICTED_1"))
		{
			gameItem.AddItemFeature(ItemFeatureList.RestrictedLowMarkUp());
		}
		else if (gameItem.IsGameItemType("RESTRICTED_2"))
		{
			gameItem.AddItemFeature(ItemFeatureList.RestrictedMidMarkUp());
		}
		else if (gameItem.IsGameItemType("RESTRICTED_3"))
		{
			gameItem.AddItemFeature(ItemFeatureList.RestrictedHighMarkUp());
		}
	}

	public static void OnNewItemCreated(GameItem gameItem)
	{
		if (!ContrabandHelper.IsContraband(gameItem))
		{
			return;
		}
		foreach (var activePermit in PlayerStore.Instance.ActivePermits)
		{
			if (IsCoveredType(gameItem, activePermit.Item1) && ContrabandHelper.GetContrabandLevel(gameItem) == activePermit.Item2)
			{
				SetRestrictedClass(gameItem);
				ContrabandHelper.RemoveContrabandStatus(gameItem);
			}
		}
	}

	public static void InitPermitItem(GameItem gameItem, int level, List<string> itemTypes)
	{
		gameItem.EnableTag("PERMIT_ITEM_TAG");
		gameItem.ModifyTag("PERMIT_ITEM_COUNT", delegate(TagState state)
		{
			state.Enable().SetInt(itemTypes.Count);
		});
		gameItem.ModifyTag("PERMIT_ITEM_LEVEL", delegate(TagState state)
		{
			state.Enable().SetInt(level);
		});
		int i;
		for (i = 0; i < itemTypes.Count; i++)
		{
			string tagName = $"PERMIT_ITEM_TAG{i + 1}";
			gameItem.ModifyTag(tagName, delegate(TagState state)
			{
				state.Enable().SetString(itemTypes[i]);
			});
		}
	}

	public static void FindAndDeletePermitItem(int level, string itemType)
	{
		foreach (GameItem allOwnedItem in EmporiumEntry.Instance.GetAllOwnedItems())
		{
			if (!allOwnedItem.IsTag("PERMIT_ITEM_TAG") || allOwnedItem.GetTagReadonly("PERMIT_ITEM_LEVEL").GetInt() != level)
			{
				continue;
			}
			int @int = allOwnedItem.GetTagReadonly("PERMIT_ITEM_COUNT").GetInt();
			for (int i = 1; i <= @int; i++)
			{
				string tagName = $"PERMIT_ITEM_TAG{i}";
				if (allOwnedItem.GetTagReadonly(tagName).GetString() == itemType)
				{
					allOwnedItem.Destroy();
					return;
				}
			}
		}
	}

	public static bool IsGunPermitI()
	{
		return PlayerStore.Instance.ActivePermits.Exists(((string, int) permit) => permit.Item1 == "FIREARM" && permit.Item2 == 1);
	}

	public static bool IsGunPermitII()
	{
		return PlayerStore.Instance.ActivePermits.Exists(((string, int) permit) => permit.Item1 == "FIREARM" && permit.Item2 == 2);
	}

	public static bool IsGunPermitIII()
	{
		return PlayerStore.Instance.ActivePermits.Exists(((string, int) permit) => permit.Item1 == "FIREARM" && permit.Item2 == 3);
	}

	public static bool IsNoGunPermit()
	{
		if (!IsGunPermitI() && !IsGunPermitII())
		{
			return !IsGunPermitIII();
		}
		return false;
	}
}
