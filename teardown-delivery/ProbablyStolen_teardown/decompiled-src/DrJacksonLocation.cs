using System.Collections.Generic;
using UnityEngine;

public class DrJacksonLocation : AfterhourLocation
{
	private const float SHEET_X = -2.6f;

	private const float SHEET_Y = 1.35f;

	private const float SHEET_WIDTH = 0.9f;

	private const float SHEET_HEIGHT = 1f;

	private GameObject sheetTablet;

	public override string Id => "dr_jackson";

	public override GameObject StageObject
	{
		get
		{
			if (!(MapUIManager.Instance != null))
			{
				return null;
			}
			return MapUIManager.Instance.upgradeStoreScene;
		}
	}

	public override GameObject PanelObject
	{
		get
		{
			if (!(MapUIManager.Instance != null))
			{
				return null;
			}
			return MapUIManager.Instance.UIUpgradeMerchantPanel;
		}
	}

	public override bool ShowsStoreCounters => true;

	public override string GetTitle()
	{
		return LocHelper.GetLocalizedClientName("name_dr._jackson");
	}

	public override bool IsDiscovered()
	{
		if (PlayerStore.Instance != null && (PlayerStore.Instance.storeClientData.isJacksonIntroduced || PlayerStore.Instance.skipIntro))
		{
			return !StoreReputation.IsPerkUnlocked("LLAVOIDED");
		}
		return false;
	}

	public override Vector2? GetFrontCounterPosition(int w, int h)
	{
		return new Vector2((float)(w / 100) * 78.5f, -(h / 100 * 10));
	}

	public override Vector2? GetBackCounterPosition(int w, int h)
	{
		return new Vector2((float)(w / 2) - (float)(w / 100) * 18f, 0f - (float)(h / 100) * 55f);
	}

	public override string GetLeaveWarning()
	{
		return LocHelper.GetLocalizedMechanic("afterhour_jackson_warning");
	}

	private void EnsureSheetTablet()
	{
		GameObject stageObject = StageObject;
		if (!(sheetTablet != null) && !(stageObject == null))
		{
			sheetTablet = new GameObject("DrJacksonTablet");
			sheetTablet.transform.SetParent(stageObject.transform, worldPositionStays: false);
			sheetTablet.transform.localPosition = new Vector3(-2.6f, 1.35f, 0f);
			sheetTablet.AddComponent<BoxCollider2D>().size = new Vector2(0.9f, 1f);
			Interactable interactable = sheetTablet.AddComponent<Interactable>();
			interactable.isControlable = false;
			interactable.doNotOutline = true;
			sheetTablet.AddComponent<StoreTablet>();
			sheetTablet.AddComponent<TooltipContainer>();
		}
	}

	public override void Arrive()
	{
		MapUIManager instance = MapUIManager.Instance;
		PlayerStore.Instance.playerLocation = PlayerStore.PlayerLocation.INVENTOR;
		EnsureSheetTablet();
		EmporiumEntry.Instance.frontInvWindow.Show();
		EmporiumEntry.Instance.backInvWindow.Show();
		instance.TransferBackInvToCounter();
		EmporiumEntry.Instance.ExpandFrontCounterGrid();
		if (AfterhourEvents.IsActive("special_stock"))
		{
			StoreClientList.PlaceInventorSpecialStock();
		}
		else
		{
			StoreClientList.PlaceInventorInventory();
		}
		instance.ActivateClientOutsideStore();
		StoreUIManager.Instance.SetClientPositionToDrJackson();
		StoreUIManager.Instance.storeCreditDisplay.creditDisplayText.gameObject.SetActive(value: true);
		instance.SetCreditDisplayPositionToDrJackson();
		PlayerStore.Instance.daySinceLastBrowseUpgrade = 0;
	}

	public override void Leave()
	{
		MapUIManager instance = MapUIManager.Instance;
		NegociationUIManager.Instance.CloseUI();
		List<GameItem> list = new List<GameItem>();
		GameGridInventory[] array = new GameGridInventory[2]
		{
			EmporiumEntry.Instance.frontInvinvElement,
			EmporiumEntry.Instance.backInvinvElement
		};
		foreach (GameInventory gameInventory in array)
		{
			foreach (GameItem childItem in gameInventory.childItems)
			{
				if (GeneralHelper.IsItemOwned(childItem))
				{
					childItem.CloseContentWindow(includeChildren: true);
					gameInventory.Expel(childItem);
					list.Add(childItem);
				}
			}
		}
		EmporiumEntry.Instance.ResetFrontCounterGrid();
		EmporiumEntry.Instance.ClearFrontInv();
		EmporiumEntry.Instance.ClearBackInv();
		instance.TransferCounterToBackInv();
		foreach (GameItem item in list)
		{
			Debug.Log("Adding owned item left on Dr. Jackson's counter to back inv: " + item.name);
			GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvWindow, item);
		}
		foreach (GameItem childItem2 in EmporiumEntry.Instance.afterhourInventory.childItems)
		{
			Debug.Log("Adding item left on ground to back inv: " + childItem2.name);
			GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.backInvWindow, childItem2);
		}
		instance.ResetCreditDisplayPosition();
		instance.DeactivateClientOutsideStore();
	}
}
