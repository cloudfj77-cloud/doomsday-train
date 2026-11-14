using System;

public class HusbandryDirectory : ItemDirectory
{
	protected override void InitDirectory()
	{
		Add("animal_cage", () => AnimalCage.CreateAnimalCage());
		Add("livestock_feed_dispenser", () => MachineFeedDispenser.CreateFeedDispenser());
		Add("auto_butcher", () => MachineAutoButcher.CreateAutoButcher());
		Add("marking_pen", () => AnimalMarkHelper.CreateMarkingPen());
		Add("feces_small", () => FecesSmall());
		Add("feces_medium", () => FecesMedium());
		Add("feces_large", () => FecesLarge());
		Add("vacuum_robot", () => VacuumRobot());
		Add("rat", () => Rat());
		Add("serum_red", () => SerumRed());
		Add("serum_green", () => SerumGreen());
		Add("mouse_trap", () => RatTrap.CreateMouseTrap());
		Add("ras", () => MachineRAS.CreateRAS());
		Add("ras_water_tank", () => MachineRAS.CreateWaterTank());
		Add(FishSpecies.Tilapia.fishIdentifier, () => AquacultureItems.Tilapia());
		Add(FishSpecies.Tilapia.roeIdentifier, () => AquacultureItems.TilapiaRoe());
		Add(FishSpecies.Tilapia.fryIdentifier, () => AquacultureItems.TilapiaFry());
		Add(FishSpecies.GutterTilapia.fishIdentifier, () => AquacultureItems.GutterTilapia());
		Add(FishSpecies.GutterTilapia.roeIdentifier, () => AquacultureItems.GutterTilapiaRoe());
		Add(FishSpecies.GutterTilapia.fryIdentifier, () => AquacultureItems.GutterTilapiaFry());
		Add(FishSpecies.GraftTilapia.fishIdentifier, () => AquacultureItems.GraftTilapia());
		Add(FishSpecies.GraftTilapia.roeIdentifier, () => AquacultureItems.GraftTilapiaRoe());
		Add(FishSpecies.GraftTilapia.fryIdentifier, () => AquacultureItems.GraftTilapiaFry());
		Add("tilapia_skin_graft", () => AquacultureItems.TilapiaSkinGraft());
		Add("tiny_fish_fillet", () => AquacultureItems.TinyFishFillet());
		Add("fish_scrap", () => AquacultureItems.FishScrap());
		Add("small_fish_fillet", () => AquacultureItems.SmallFishFillet());
		Add("fish_fillet", () => AquacultureItems.FishFillet());
		Add("large_fish_fillet", () => AquacultureItems.LargeFishFillet());
		Add("gutterflow_pellet", () => GutterflowPelletHelper.CreatePellet());
		Add("industrial_fish_feed", () => IndustrialFishFeedHelper.CreateFeed());
		Add(FishSpecies.Eel.fishIdentifier, () => AquacultureItems.Eel());
		Add(FishSpecies.ElectricEel.fishIdentifier, () => AquacultureItems.ElectricEel());
		Add("small_eel_fillet", () => AquacultureItems.SmallEelFillet());
		Add("eel_fillet", () => AquacultureItems.EelFillet());
		Add(FishSpecies.Sturgeon.fishIdentifier, () => AquacultureItems.Sturgeon());
		Add("caviar", () => AquacultureItems.Caviar());
		Add("sturgeon_roe", () => AquacultureItems.SturgeonRoe());
	}

	private GameItem VacuumRobot()
	{
		GameSlotInventory batterySlot = new GameSlotInventory();
		GameGridInventory storageGrid = new GameGridInventory(8, 4);
		MachineryPanel powerPanel = MachineHelper.PowerPanel(batterySlot);
		PixelWindow contentWindow = MachineHelper.CreateMachineryWindow(true, powerPanel, MachineHelper.Panel(LocHelper.GetLocalizedMechanic("type_storage"), storageGrid, "storage"));
		batterySlot.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => false;
		powerPanel.SetHidden(hidden: true);
		GameSlotInventory gameSlotInventory = batterySlot;
		gameSlotInventory.onSlotRemoveItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(gameSlotInventory.onSlotRemoveItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
		{
			powerPanel.SetHidden(batterySlot.childItem == null);
		});
		storageGrid.mayInventoryAddItemFunc = (GameItem fi, GameInventory _) => GeneralHelper.IsItemOwned(fi) && fi.IsTag("ANIMAL_FECES_TAG");
		GameItem item = Empty().SetName(LocHelper.GetLocalizedItem("husbandry_vacuum_robot_name")).SetSpriteAndShape("Items/items_husbandry", "vacuum_robot").SetContentWindow(contentWindow);
		item.unitValue = 250L;
		item.shortDescription = LocHelper.GetLocalizedItem("husbandry_vacuum_robot_desc");
		item.EnableTag("IMPORTANT_TAG");
		item.EnableTag("ALLOWED_IN_CAGE_TAG");
		GameItem gameItem = item;
		gameItem.onLoaded = (Action<GameItem>)Delegate.Combine(gameItem.onLoaded, (Action<GameItem>)delegate(GameItem loadedItem)
		{
			loadedItem.EnableTag("ALLOWED_IN_CAGE_TAG");
			loadedItem.DisableTag("machinery_drawn_power");
			loadedItem.DisableTag("MACHINERY_DRAWN_POWER_BASE");
			loadedItem.DisableTag("MACHINE_RECHARGABLE_TAG");
			powerPanel.SetHidden(batterySlot.childItem == null);
		});
		ContainerHelper.AddContainerTooltipTag(item);
		item.onCycleEndLateSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			Cleanup(curInv);
		};
		item.mayToggleSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s) && s.IsTag("VACUUM_INCINERATOR_UPGRADED_TAG");
		item.canToggleSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s) && s.IsTag("VACUUM_INCINERATOR_UPGRADED_TAG");
		item.onToggleSlotItemFunc = delegate(GameItem s, GameInventory _, SlotMarker _)
		{
			if (HusbandryHelper.IsIncineratorOn(s))
			{
				s.DisableTag("VACUUM_INCINERATOR_ON_TAG");
			}
			else
			{
				s.EnableTag("VACUUM_INCINERATOR_ON_TAG");
			}
			AudioManager.Instance.Play3("click_vintage");
		};
		item.mayActivateSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(s);
		item.canActivateSlotItemFunc = (GameItem s, GameInventory curInv, SlotMarker _) => GeneralHelper.IsItemOwned(s) && HasWork(curInv);
		item.onActivateSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			if (HasWork(curInv))
			{
				Cleanup(curInv);
			}
		};
		return item;
		void Cleanup(GameInventory curInv)
		{
			CollectFeces(curInv);
			if (HusbandryHelper.IsIncineratorOn(item))
			{
				IncinerateStorage();
			}
		}
		void CollectFeces(GameInventory curInv)
		{
			if (curInv == null)
			{
				return;
			}
			foreach (GameItem item2 in curInv.childItems.FindAll((GameItem f) => f.IsTag("ANIMAL_FECES_TAG")))
			{
				if (!GraphUtils.CanAccept(storageGrid, item2))
				{
					break;
				}
				GraphUtils.TryAcceptAll(storageGrid, item2);
			}
		}
		static bool HasFeces(GameInventory inv)
		{
			return inv?.childItems.Exists((GameItem f) => f.IsTag("ANIMAL_FECES_TAG")) ?? false;
		}
		bool HasWork(GameInventory curInv)
		{
			if (!HasFeces(curInv))
			{
				if (HusbandryHelper.IsIncineratorOn(item))
				{
					return HasFeces(storageGrid);
				}
				return false;
			}
			return true;
		}
		void IncinerateStorage()
		{
			foreach (GameItem item3 in storageGrid.childItems.FindAll((GameItem f) => f.IsTag("ANIMAL_FECES_TAG")))
			{
				item3.Destroy();
			}
		}
	}

	private GameItem FecesSmall()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("husbandry_feces_small_name")).SetSpriteAndShape("Items/items_husbandry", "feces_small");
		gameItem.unitValue = 0L;
		gameItem.EnableTag("ANIMAL_FECES_TAG");
		ChemicalHelper.InitChemItem(gameItem, isBlendable: true, isSoluble: true);
		ChemicalHelper.SetChemContentML(gameItem, "organic_waste", 10.0);
		ChemicalHelper.SetChemContentML(gameItem, "debris", 1.0);
		ChemicalHelper.SetChemContentML(gameItem, "microbe", 4.0);
		return gameItem;
	}

	private GameItem FecesMedium()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("husbandry_feces_medium_name")).SetSpriteAndShape("Items/items_husbandry", "feces");
		gameItem.unitValue = 0L;
		gameItem.EnableTag("ANIMAL_FECES_TAG");
		ChemicalHelper.InitChemItem(gameItem, isBlendable: true, isSoluble: true);
		ChemicalHelper.SetChemContentML(gameItem, "organic_waste", 20.0);
		ChemicalHelper.SetChemContentML(gameItem, "debris", 2.0);
		ChemicalHelper.SetChemContentML(gameItem, "microbe", 8.0);
		return gameItem;
	}

	private GameItem FecesLarge()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("husbandry_feces_large_name")).SetSpriteAndShape("Items/items_husbandry", "feces_large");
		gameItem.unitValue = 0L;
		gameItem.EnableTag("ANIMAL_FECES_TAG");
		ChemicalHelper.InitChemItem(gameItem, isBlendable: true, isSoluble: true);
		ChemicalHelper.SetChemContentML(gameItem, "organic_waste", 40.0);
		ChemicalHelper.SetChemContentML(gameItem, "debris", 4.0);
		ChemicalHelper.SetChemContentML(gameItem, "microbe", 16.0);
		return gameItem;
	}

	private GameItem Rat()
	{
		GameItem gameItem = Empty().SetName(LocHelper.GetLocalizedItem("husbandry_rat_name")).SetSpriteAndShape("Items/items_husbandry", "rat_s_m");
		gameItem.unitValue = 0L;
		HusbandryHelper.InitRatItem(gameItem);
		AudioHelper.InitRat(gameItem);
		return gameItem;
	}

	private GameItem SerumRed()
	{
		GameItem item2 = Empty().SetName(LocHelper.GetLocalizedMechanic("liquid_livestock_steroid")).SetSpriteAndShape("Items/items_medical", "red_serum");
		item2.unitValue = 100L;
		item2.shortDescription = LocHelper.GetLocalizedItem("husbandry_serum_red_desc");
		UseCountHelper.InitUseCountItem(item2, 10, transferable: true, destroyOnEmpty: false, useAutoValue: true, 0, 10);
		item2.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item2.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		GameItem gameItem = item2;
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => CanUseSerumOnAnimal(s, t)));
		GameItem gameItem2 = item2;
		gameItem2.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem2.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => CanUseSerumOnAnimal(s, t)));
		GameItem gameItem3 = item2;
		gameItem3.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem3.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (t.IsTag("ANIMAL_ITEM_TAG"))
			{
				UseCountHelper.Use(item2);
				IngesterHelper.Inject(t, "livestock_steroid", 20);
			}
		});
		AudioHelper.InitPlasticContainer(item2);
		return item2;
	}

	private GameItem SerumGreen()
	{
		GameItem item2 = Empty().SetName(LocHelper.GetLocalizedMechanic("liquid_livestock_immunity")).SetSpriteAndShape("Items/items_medical", "green_serum");
		item2.unitValue = 100L;
		item2.shortDescription = LocHelper.GetLocalizedItem("husbandry_serum_green_desc");
		UseCountHelper.InitUseCountItem(item2, 10, transferable: true, destroyOnEmpty: false, useAutoValue: true, 0, 10);
		item2.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
		item2.canSelectSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => true;
		GameItem gameItem = item2;
		gameItem.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => CanUseSerumOnAnimal(s, t)));
		GameItem gameItem2 = item2;
		gameItem2.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(gameItem2.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => CanUseSerumOnAnimal(s, t)));
		GameItem gameItem3 = item2;
		gameItem3.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(gameItem3.onThisTargetItemFunc, (Action<GameItem, GameItem>)delegate(GameItem s, GameItem t)
		{
			if (t.IsTag("ANIMAL_ITEM_TAG"))
			{
				UseCountHelper.Use(item2);
				IngesterHelper.Inject(t, "livestock_immunity", 20);
			}
		});
		AudioHelper.InitPlasticContainer(item2);
		return item2;
	}

	private bool CanUseSerumOnAnimal(GameItem serum, GameItem animal)
	{
		if (GeneralHelper.IsItemOwned(serum) && GeneralHelper.IsItemOwned(animal) && animal.IsTag("ANIMAL_ITEM_TAG") && !HusbandryHelper.IsAnimalDead(animal))
		{
			return UseCountHelper.CanUse(serum);
		}
		return false;
	}
}
