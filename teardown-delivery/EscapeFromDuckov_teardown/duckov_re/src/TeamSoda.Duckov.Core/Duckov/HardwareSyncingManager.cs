using System;
using Duckov.Buildings;
using Duckov.Economy;
using ItemStatsSystem;
using UnityEngine;

namespace Duckov;

public class HardwareSyncingManager : MonoBehaviour
{
	public static event Action<string> OnSetEvent;

	public static void SetEvent(string eventName)
	{
		try
		{
			HardwareSyncingManager.OnSetEvent?.Invoke(eventName);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private void Awake()
	{
		InteractableLootbox.OnStartLoot += OnStartLoot;
		Health.OnHurt += OnHurt;
		Health.OnDead += OnDead;
		MainMenu.OnMainMenuAwake = (Action)Delegate.Combine(MainMenu.OnMainMenuAwake, new Action(OnMainMenuAwake));
		MainMenu.OnMainMenuDestroy = (Action)Delegate.Combine(MainMenu.OnMainMenuDestroy, new Action(OnMainMenuDestroy));
		LevelManager.OnEvacuated += OnEvacuated;
		PauseMenu.onPauseMenuOn += OnPauseMenuOn;
		PauseMenu.onPauseMenuOff += OnPauseMenuOff;
		StockShop.OnItemPurchased += OnItemPurchased;
		StockShop.OnItemSoldByPlayer += OnItemSoldByPlayer;
		CA_UseItem.OnItemUsedByPlayer += OnItemUsedByPlayer;
		BuildingManager.OnBuildingBuilt += OnBuildingBuilt;
		EXPManager.onLevelChanged = (Action<int, int>)Delegate.Combine(EXPManager.onLevelChanged, new Action<int, int>(OnLevelChanged));
		ItemUtilities.OnItemSentToPlayerInventory += OnItemSentToPlayerInventory;
	}

	private void OnDestroy()
	{
		InteractableLootbox.OnStartLoot -= OnStartLoot;
		Health.OnHurt -= OnHurt;
		Health.OnDead -= OnDead;
		MainMenu.OnMainMenuAwake = (Action)Delegate.Remove(MainMenu.OnMainMenuAwake, new Action(OnMainMenuAwake));
		MainMenu.OnMainMenuDestroy = (Action)Delegate.Remove(MainMenu.OnMainMenuDestroy, new Action(OnMainMenuDestroy));
		LevelManager.OnEvacuated -= OnEvacuated;
		PauseMenu.onPauseMenuOn -= OnPauseMenuOn;
		PauseMenu.onPauseMenuOff -= OnPauseMenuOff;
		CA_UseItem.OnItemUsedByPlayer -= OnItemUsedByPlayer;
		BuildingManager.OnBuildingBuilt -= OnBuildingBuilt;
		EXPManager.onLevelChanged = (Action<int, int>)Delegate.Remove(EXPManager.onLevelChanged, new Action<int, int>(OnLevelChanged));
		ItemUtilities.OnItemSentToPlayerInventory -= OnItemSentToPlayerInventory;
	}

	private void OnItemSentToPlayerInventory(Item item)
	{
		SetEvent("Get_Item");
	}

	private void OnLevelChanged(int arg1, int arg2)
	{
		SetEvent("LevelUp");
	}

	private void OnBuildingBuilt(int obj)
	{
		SetEvent("Build_Furniture");
	}

	private void OnItemUsedByPlayer(Item item)
	{
		SetEvent("Use_Item");
	}

	private void OnItemSoldByPlayer(StockShop shop, Item item, int arg3)
	{
		SetEvent("Sell");
	}

	private void OnItemPurchased(StockShop shop, Item item)
	{
		SetEvent("Buy");
	}

	private void OnPauseMenuOn()
	{
		SetEvent("PauseMenu_On");
	}

	private void OnPauseMenuOff()
	{
		SetEvent("PauseMenu_Off");
	}

	private void OnEvacuated(EvacuationInfo info)
	{
		SetEvent("Escaped");
	}

	private void OnMainMenuAwake()
	{
		SetEvent("MainMenu_On");
	}

	private void OnMainMenuDestroy()
	{
		SetEvent("MainMenu_Off");
	}

	private void OnDead(Health health, DamageInfo info)
	{
		if (!(health == null))
		{
			if (health.IsMainCharacterHealth)
			{
				SetEvent("Die");
			}
			else if (info.fromCharacter != null && info.fromCharacter.IsMainCharacter)
			{
				SetEvent("Kill_Enemy");
			}
		}
	}

	private void OnHurt(Health health, DamageInfo info)
	{
		if (!(health == null) && health.IsMainCharacterHealth)
		{
			SetEvent("Take_Damage");
		}
	}

	private void OnStartLoot(InteractableLootbox lootbox)
	{
		SetEvent("Interaction");
	}
}
