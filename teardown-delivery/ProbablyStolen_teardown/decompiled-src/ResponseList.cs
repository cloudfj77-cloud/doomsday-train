using System;
using System.Collections.Generic;

public class ResponseList
{
	public static StoreResponse CreateTestResponse()
	{
		StoreResponse storeResponse = new StoreResponse();
		storeResponse.identifier = "test_response";
		storeResponse.title = "Test Response";
		storeResponse.choices = new List<(GameItem, Action)>();
		storeResponse.closable = true;
		storeResponse.choices.Add(EmporiumEntry.PrepChoice(DirectoryMaster.Item("trade"), "(Trade) See what they have to offer.", isRealItem: false, "", null, null, delegate
		{
			Debug.Log("Trade choice selected");
		}));
		return storeResponse;
	}

	public static StoreResponse CreateAfterhoursResponse()
	{
		StoreResponse storeResponse = new StoreResponse();
		storeResponse.identifier = "afterhours_response";
		storeResponse.title = LocHelper.GetLocalizedMechanic("response_afterhours_title");
		storeResponse.choices = new List<(GameItem, Action)>();
		storeResponse.closable = true;
		List<GameItem> allOwnedItemsByTag = EmporiumEntry.Instance.GetAllOwnedItemsByTag("BACKPACK_TAG");
		bool isHunted = StoreClientData.IsButcherHunting();
		string needGun = LocHelper.GetLocalizedMechanic("response_afterhours_need_gun", LocHelper.GetLocalizedClientName("name_the_butcher"));
		storeResponse.choices.Add(EmporiumEntry.PrepChoice(DirectoryMaster.Item("plastic_bag"), LocHelper.GetLocalizedMechanic("response_afterhours_no_backpack"), isRealItem: false, isHunted ? needGun : "", null, null, delegate
		{
			if (isHunted)
			{
				StoreUIManager.Instance.Notify(needGun);
			}
			else
			{
				EmporiumEntry.Instance.HideResponseWindow();
				MapUIManager.Instance.OpenUI();
			}
		}));
		foreach (GameItem backpack in allOwnedItemsByTag)
		{
			GameItem gameItem = DirectoryMaster.Item("");
			gameItem.SetSpriteAndShape(backpack.spriteAtlasPath, backpack.spritePath);
			gameItem.SetValue(backpack.GetCurrentValue(useRetailMarkup: false, withChild: true, includeEvents: true, forceMarkup: false, 0L));
			bool flag = isHunted && !GunHelper.HasGunInside(backpack);
			storeResponse.choices.Add(EmporiumEntry.PrepChoice(gameItem, LocHelper.GetLocalizedMechanic("response_afterhours_select_backpack", backpack.name), isRealItem: true, flag ? needGun : "", null, null, delegate
			{
				if (StoreClientData.IsButcherHunting() && !GunHelper.HasGunInside(backpack))
				{
					StoreUIManager.Instance.Notify(needGun);
				}
				else
				{
					Debug.Log("Backpack selected: " + backpack.name);
					EmporiumEntry.Instance.selectedBackpack = backpack;
					EmporiumEntry.Instance.HideResponseWindow();
					MapUIManager.Instance.OpenUI();
				}
			}));
		}
		return storeResponse;
	}
}
