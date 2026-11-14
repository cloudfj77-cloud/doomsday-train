using System.Collections.Generic;

public class AmmoBoxHelper
{
	public const string AMMO_BOX_PACKAGED_TAG = "AMMO_BOX_PACKAGED_TAG";

	public const string AMMO_BOX_TAG = "AMMO_BOX_TAG";

	public const string AMMO_BOX_CONTENT_TAG = "AMMO_BOX_CONTENT_TAG";

	public const string AMMO_BOX_CONTENT_CALIBER_TAG = "AMMO_BOX_CONTENT_CALIBER_TAG";

	public const string AMMO_BOX_CONTENT_UNIT_VALUE_TAG = "AMMO_BOX_CONTENT_UNIT_VALUE_TAG";

	public const string AMMO_BOX_EXPOSE_REASON_TAG = "AMMO_BOX_EXPOSE_REASON_TAG";

	public const int BASE_PRICE_22_NL = 14;

	public const int BASE_PRICE_22 = 20;

	public const int BASE_PRICE_22_AP = 26;

	public const int BASE_PRICE_10MM_NL = 24;

	public const int BASE_PRICE_10MM = 30;

	public const int BASE_PRICE_10MM_AP = 36;

	public const int AMMO_BOX_SIZE = 6;

	public const string AMMO_TYPE_TAG = "AMMO_TYPE_TAG";

	public const string NORMAL_AMMO = "NORMAL_AMMO";

	public const string ARMOR_PIERCING_AMMO = "ARMOR_PIERCING_AMMO";

	public const string NON_LETHAL_AMMO = "NON_LETHAL_AMMO";

	private static string[] line1Options = new string[6] { "Are you trying to scam me?", "Hey, what's this? This ammo box isn't right.", "Is this some kind of joke? What's with this ammo?", "Something's off with this ammo.", "You think I wouldn't notice this mess?", "I'm not stupid. What did you pack in here?" };

	public static void Package(GameItem item)
	{
		item.EnableTag("AMMO_BOX_PACKAGED_TAG");
		item.SetSpriteAndShape("Items/items_sec", "ammo_box_packed");
		GameItem itemFound = FindMostNumerousItem(GraphUtils.FindAllChildrenType<GameItem>(item));
		switch (itemFound.GetTagReadonly("AMMO_TYPE_TAG").GetString())
		{
		case "ARMOR_PIERCING_AMMO":
			item.SetSpriteAndShape("Items/items_sec", "ap_ammo_box_packed");
			break;
		case "NON_LETHAL_AMMO":
			item.SetSpriteAndShape("Items/items_sec", "nl_ammo_box_packed");
			break;
		}
		item.EnableTag("LOCKED_CONTAINER_TAG");
		item.ModifyTag("LOCK_ID", delegate(TagState state)
		{
			state.Enable().SetString("AMMO_BOX_LOCK");
		});
		item.ModifyTag("LOCK_STATUS", delegate(TagState state)
		{
			state.Enable().SetBool(val: true);
		});
		item.name = "Ammo Box (x" + 6 + " " + itemFound.GetDisplayName() + ")";
		item.unitValue = itemFound.unitValue * 6;
		item.shortDescription = "The label says its packaged full with " + itemFound.GetDisplayName() + ".";
		item.flavorText = "";
		item.ModifyTag("AMMO_BOX_CONTENT_TAG", delegate(TagState state)
		{
			state.Enable().SetString(itemFound.identifier);
		});
		Debug.Log("Caliber: " + itemFound.GetTagReadonly("GUN_CALIBER_TAG").GetInt());
		item.ModifyTag("AMMO_BOX_CONTENT_CALIBER_TAG", delegate(TagState state)
		{
			state.Enable().SetInt(itemFound.GetTagReadonly("GUN_CALIBER_TAG").GetInt());
		});
		item.ModifyTag("AMMO_BOX_CONTENT_UNIT_VALUE_TAG", delegate(TagState state)
		{
			state.Enable().SetInt((int)itemFound.unitValue);
		});
		item.TransferGameItemTypeFrom(itemFound);
		LockHelper.LockUpContainer(item);
	}

	public static void Unpackage(GameItem item)
	{
		item.DisableTag("AMMO_BOX_PACKAGED_TAG");
		item.name = "Ammo Box (Unpackaged)";
		item.flavorText = "A paper box used to store ammunition. This one is unpackaged";
		item.shortDescription = "Fill this box with ammunition and package it.";
		item.SetSpriteAndShape("Items/items_sec", "ammo_box");
		item.unitValue = 1L;
		item.ModifyTag("LOCK_STATUS", delegate(TagState state)
		{
			state.Enable().SetBool(val: false);
		});
		item.RemoveAllGameItemType();
		item.ModifyTag("AMMO_BOX_CONTENT_TAG", delegate(TagState state)
		{
			state.Disable().SetString("");
		});
		item.ModifyTag("AMMO_BOX_CONTENT_CALIBER_TAG", delegate(TagState state)
		{
			state.Disable().SetInt(0);
		});
		item.ModifyTag("AMMO_BOX_CONTENT_UNIT_VALUE_TAG", delegate(TagState state)
		{
			state.Disable().SetInt(0);
		});
		LockHelper.DisableChildBehindLock(item);
		item.contentWindow.isLocked = false;
		((GameGridInventory)item.contentWindow.child).mayInventoryAddItemFunc = (GameItem addedItem, GameInventory parentInventory) => (addedItem.IsGameItemType("AMMUNITION") && GeneralHelper.IsItemOwned(addedItem)) ? true : false;
	}

	public static GameItem FindMostNumerousItem(IEnumerable<GameItem> items)
	{
		Dictionary<string, (int, GameItem)> dictionary = new Dictionary<string, (int, GameItem)>();
		GameItem result = null;
		int num = 0;
		foreach (GameItem item2 in items)
		{
			if (!dictionary.TryGetValue(item2.identifier, out var value))
			{
				dictionary[item2.identifier] = (1, item2);
			}
			else
			{
				dictionary[item2.identifier] = (value.Item1 + 1, value.Item2);
			}
			int item = dictionary[item2.identifier].Item1;
			if (item > num)
			{
				num = item;
				result = dictionary[item2.identifier].Item2;
			}
		}
		return result;
	}

	public static void HandleAmmoBoxCheck(GameItem item, StoreClient storeClient)
	{
		if (item.IsTag("AMMO_BOX_PACKAGED_TAG") && !IsBoxAccepted(item, storeClient))
		{
			Unpackage(item);
			int @int = item.GetTagReadonly("AMMO_BOX_EXPOSE_REASON_TAG").GetInt();
			DialogUIManager.Instance.StartDialogue(storeClient.AttachConfrontResume(CreateConfrontDialog(@int)), forcedStart: true);
			if (!storeClient.isAug)
			{
				StoreReputation.ModCurrentClientReputation(-5);
			}
		}
	}

	public static bool IsBoxAccepted(GameItem item, StoreClient storeClient)
	{
		if (!item.IsTag("AMMO_BOX_PACKAGED_TAG") || !item.IsTag("AMMO_BOX_TAG"))
		{
			return true;
		}
		if (storeClient.IsTag("SCAV"))
		{
			return !ClientCanExposeAmmoBox(item, 5, 4, 3);
		}
		if (storeClient.IsTag("LOWER_LEVEL_CITIZEN"))
		{
			return !ClientCanExposeAmmoBox(item, 5, 4, 3);
		}
		if (storeClient.IsTag("UPPER_LEVEL_CITIZEN") || storeClient.IsTag("TOURIST"))
		{
			return !ClientCanExposeAmmoBox(item, 4, 3, 2);
		}
		if (storeClient.IsTag("SECURITY") || storeClient.IsTag("REV"))
		{
			return !ClientCanExposeAmmoBox(item, 6, 6, 5);
		}
		if (storeClient.IsTag("BLACK_MARKET"))
		{
			return !ClientCanExposeAmmoBox(item, 6, 5, 4);
		}
		return !ClientCanExposeAmmoBox(item, 5, 4, 3);
	}

	public static bool IsAmmoBox(GameItem item)
	{
		if (item.IsTag("AMMO_BOX_PACKAGED_TAG") && item.IsTag("AMMO_BOX_TAG"))
		{
			return true;
		}
		return false;
	}

	public static bool ClientCanExposeAmmoBox(GameItem item, int minItemCount, int minCorrectCaliberCount, int minExactCount)
	{
		if (!item.IsTag("AMMO_BOX_PACKAGED_TAG") || !item.IsTag("AMMO_BOX_TAG"))
		{
			Debug.Log("AmmoBox: not correct item");
			return false;
		}
		string @string = item.GetTagReadonly("AMMO_BOX_CONTENT_TAG").GetString();
		int num = 0;
		foreach (GameItem item2 in GraphUtils.FindAllChildrenType<GameItem>(item))
		{
			_ = item2;
			num++;
		}
		if (num < minItemCount)
		{
			Debug.Log("AmmoBox: not enough item");
			item.ModifyTag("AMMO_BOX_EXPOSE_REASON_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(1);
			});
			return true;
		}
		int @int = item.GetTagReadonly("AMMO_BOX_CONTENT_CALIBER_TAG").GetInt();
		int num2 = 0;
		foreach (GameItem item3 in GraphUtils.FindAllChildrenType<GameItem>(item))
		{
			if (item3.GetTagReadonly("GUN_CALIBER_TAG").GetInt() == @int)
			{
				num2++;
			}
		}
		if (num2 < minCorrectCaliberCount)
		{
			Debug.Log("AmmoBox: not enough correct caliber");
			item.ModifyTag("AMMO_BOX_EXPOSE_REASON_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(2);
			});
			return true;
		}
		num2 = 0;
		foreach (GameItem item4 in GraphUtils.FindAllChildrenType<GameItem>(item))
		{
			if (item4.identifier.Equals(@string))
			{
				num2++;
			}
		}
		if (num2 < minExactCount)
		{
			Debug.Log("AmmoBox: not enough exact item " + num2 + " < " + minExactCount);
			item.ModifyTag("AMMO_BOX_EXPOSE_REASON_TAG", delegate(TagState state)
			{
				state.Enable().SetInt(2);
			});
			return true;
		}
		return false;
	}

	public static void CheckSoldAmmoBox(GameItem item, StoreClient storeClient)
	{
		string text = "FACTION_LOWER_LEVEL";
		if (storeClient != null)
		{
			text = storeClient.clientFaction;
		}
		if (!item.IsTag("AMMO_BOX_PACKAGED_TAG") || !item.IsTag("AMMO_BOX_TAG") || text.Equals("FACTION_TOURIST") || IsPackageAccurate(item))
		{
			return;
		}
		string @string = item.GetTagReadonly("AMMO_BOX_CONTENT_TAG").GetString();
		int @int = item.GetTagReadonly("AMMO_BOX_CONTENT_UNIT_VALUE_TAG").GetInt();
		int num = 0;
		foreach (GameItem item2 in GraphUtils.FindAllChildrenType<GameItem>(item))
		{
			if (!item2.identifier.Equals(@string))
			{
				num++;
			}
		}
		int value = num * @int;
		PlayerStore.Instance.secData.SellBadAmmo(value, ContrabandHelper.IsContraband(item));
	}

	public static bool IsPackageAccurate(GameItem item)
	{
		string @string = item.GetTagReadonly("AMMO_BOX_CONTENT_TAG").GetString();
		foreach (GameItem item2 in GraphUtils.FindAllChildrenType<GameItem>(item))
		{
			if (!item2.identifier.Equals(@string))
			{
				return false;
			}
		}
		return true;
	}

	public static Dialogue CreateConfrontDialog(int reason)
	{
		Dialogue dialogue = new Dialogue();
		dialogue.priority = 6;
		string title = "CLIENT";
		dialogue.priority = 4;
		string randomElement = RNG.GetRandomElement(line1Options);
		string text = "";
		dialogue.SetText(title, randomElement).NextDialogue().SetText(title, reason switch
		{
			1 => "This is not a full ammo box!", 
			2 => "You packed this with the wrong caliber!", 
			_ => "You packed this with the wrong type of ammo!", 
		})
			.SetEndAction(delegate
			{
			});
		return dialogue;
	}
}
