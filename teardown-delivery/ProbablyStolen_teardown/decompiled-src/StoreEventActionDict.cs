using System;
using System.Collections.Generic;

public class StoreEventActionDict
{
	public const string ADD_CLIENT_ACTION = "addClientAction";

	public static Dictionary<string, Action> actions = new Dictionary<string, Action>
	{
		{
			"medicalSuppliesShortageOnActive",
			delegate
			{
				StoreRequest.ActivateRequest(StoreRequestList.MedShortageLL());
				StoreRequest.ActivateRequest(StoreRequestList.MedShortageUL());
				StoreRequest.ActivateRequest(StoreRequestList.MedShortageBM());
			}
		},
		{
			"construction_projectOnActive",
			delegate
			{
				StoreRequest.ActivateRequest(StoreRequestList.LLConstruction());
				StoreRequest.ActivateRequest(StoreRequestList.BMConstruction());
			}
		},
		{
			"meteorShowerOnActive",
			delegate
			{
				StoreRequest.ActivateRequest(StoreRequestList.MeteorLL());
				StoreRequest.ActivateRequest(StoreRequestList.MeteorUL());
			}
		},
		{
			"evidenceLockerBreakInOnActive",
			delegate
			{
				StoreRequest.ActivateRequest(StoreRequestList.SecEvidence());
				StoreRequest.ActivateRequest(StoreRequestList.BMEvidence());
			}
		},
		{
			"SecurityBudgetCutOnActive",
			delegate
			{
				PlayerStore.Instance.storeClientManager.ResetInspection();
			}
		},
		{
			"badNutrifruitHarvestOnActive",
			delegate
			{
				StoreRequest.ActivateRequest(StoreRequestList.LLSoupKitchen());
			}
		},
		{
			"evidenceLockerBreakInaddClientAction",
			delegate
			{
				StoreClient storeClient23 = StoreClientUniqueList.UniqueEvidenceSeller();
				storeClient23.eventSourceId = "evidenceLockerBreakIn";
				PlayerStore.Instance.storeClientManager.AddClient(storeClient23);
			}
		},
		{
			"badNutrifruitHarvestaddClientAction",
			delegate
			{
				StoreClient storeClient22 = StoreClientListEvent.CreateFoodBuyerClient();
				storeClient22.eventSourceId = "badNutrifruitHarvest";
				PlayerStore.Instance.storeClientManager.AddClient(storeClient22);
			}
		},
		{
			"seedShortageaddClientAction",
			delegate
			{
				StoreClient storeClient21 = StoreClientListEvent.CreateFarmerSeedClient();
				storeClient21.eventSourceId = "seedShortage";
				PlayerStore.Instance.storeClientManager.AddClient(storeClient21);
			}
		},
		{
			"medicalSuppliesShortageaddClientAction",
			delegate
			{
				StoreClient storeClient20 = StoreClientListEvent.CreatePharmacistClient();
				storeClient20.eventSourceId = "medicalSuppliesShortage";
				PlayerStore.Instance.storeClientManager.AddClient(storeClient20);
			}
		},
		{
			"water_treatment_breakdownaddClientAction",
			delegate
			{
				if (RNG.Roll(50))
				{
					StoreClient storeClient18 = StoreClientListEvent.CreatePanicBuyerClient();
					storeClient18.eventSourceId = "water_treatment_breakdown";
					PlayerStore.Instance.storeClientManager.AddClient(storeClient18);
				}
				else
				{
					StoreClient storeClient19 = StoreClientListEvent.CreateCorporateBuyerClient();
					storeClient19.eventSourceId = "water_treatment_breakdown";
					PlayerStore.Instance.storeClientManager.AddClient(storeClient19);
				}
			}
		},
		{
			"scheduled_treatment_maintenanceaddClientAction",
			delegate
			{
				StoreClient storeClient17 = StoreClientListEvent.CreateGenericWaterBuyer();
				storeClient17.eventSourceId = "scheduled_treatment_maintenance";
				PlayerStore.Instance.storeClientManager.AddClient(storeClient17);
			}
		},
		{
			"miner_return_ice_asteroidaddClientAction",
			delegate
			{
				StoreClient storeClient16 = StoreClientListEvent.CreateIceMiner();
				storeClient16.eventSourceId = "miner_return_ice_asteroid";
				PlayerStore.Instance.storeClientManager.AddClient(storeClient16);
			}
		},
		{
			"lower_level_mob_justiceaddClientAction",
			delegate
			{
				StoreClient storeClient15 = StoreClientListEvent.CreateLynchingCitizen();
				storeClient15.eventSourceId = "lower_level_mob_justice";
				PlayerStore.Instance.storeClientManager.AddClient(storeClient15);
			}
		},
		{
			"storeroom_excavatedaddClientAction",
			delegate
			{
				StoreClient storeClient14 = StoreClientListEvent.CreateScavengerHouseholdClient();
				storeClient14.eventSourceId = "storeroom_excavated";
				PlayerStore.Instance.storeClientManager.AddClient(storeClient14);
			}
		},
		{
			"miningExpeditionReturnaddClientAction",
			delegate
			{
				StoreClient storeClient13 = StoreClientList.CreateMiner();
				storeClient13.eventSourceId = "miningExpeditionReturn";
				PlayerStore.Instance.storeClientManager.AddClient(storeClient13);
			}
		},
		{
			"construction_projectaddClientAction",
			delegate
			{
				StoreClient storeClient12 = StoreClientListEvent.CreateForeman();
				storeClient12.eventSourceId = "construction_project";
				PlayerStore.Instance.storeClientManager.AddClient(storeClient12);
			}
		},
		{
			"meteorShoweraddClientAction",
			delegate
			{
				StoreClient storeClient11 = StoreClientListEvent.CreateForeman();
				storeClient11.eventSourceId = "meteorShower";
				PlayerStore.Instance.storeClientManager.AddClient(storeClient11);
			}
		},
		{
			"crackdownaddClientAction",
			delegate
			{
				StoreClient storeClient10 = StoreClientList.CreateInspectionClient();
				storeClient10.eventSourceId = "crackdown";
				PlayerStore.Instance.storeClientManager.AddClient(storeClient10);
			}
		},
		{
			"powerShortageaddClientAction",
			delegate
			{
				StoreClient storeClient9 = StoreClientListEvent.CreatePowerEventLLBuyer();
				storeClient9.eventSourceId = "powerShortage";
				PlayerStore.Instance.storeClientManager.AddClient(storeClient9);
			}
		},
		{
			"powerPlantMaintenanceaddClientAction",
			delegate
			{
				StoreClient storeClient8 = StoreClientListEvent.CreatePowerMaintenanceEventLLBuyer();
				storeClient8.eventSourceId = "powerPlantMaintenance";
				PlayerStore.Instance.storeClientManager.AddClient(storeClient8);
			}
		},
		{
			"stationWideBlackoutaddClientAction",
			delegate
			{
				StoreClient storeClient6 = StoreClientListEvent.CreatePowerEventULBuyer();
				storeClient6.eventSourceId = "stationWideBlackout";
				PlayerStore.Instance.storeClientManager.AddClient(storeClient6);
				if (RNG.Roll(25))
				{
					StoreClient storeClient7 = StoreClientListEvent.CreatePanickedShopkeeperBuyer();
					storeClient7.eventSourceId = "stationWideBlackout";
					PlayerStore.Instance.storeClientManager.AddClient(storeClient7);
				}
			}
		},
		{
			"cruiseShipDockedaddClientAction",
			delegate
			{
				if (RNG.Roll(33))
				{
					StoreClient storeClient3 = StoreClientList.CreateCluelessTouristBuyer();
					storeClient3.eventSourceId = "cruiseShipDocked";
					PlayerStore.Instance.storeClientManager.AddClient(storeClient3);
				}
				else if (RNG.Roll(33))
				{
					StoreClient storeClient4 = StoreClientList.CreateTouristWeaponSouvenirBuyer();
					storeClient4.eventSourceId = "cruiseShipDocked";
					PlayerStore.Instance.storeClientManager.AddClient(storeClient4);
				}
				else
				{
					StoreClient storeClient5 = StoreClientList.CreateTouristJunkSouvenirBuyer();
					storeClient5.eventSourceId = "cruiseShipDocked";
					PlayerStore.Instance.storeClientManager.AddClient(storeClient5);
				}
			}
		},
		{
			"oxymoreWaveaddClientAction",
			delegate
			{
				if (RNG.Roll(33))
				{
					StoreClient storeClient = StoreClientListEvent.CreateClinicVolunteerClient();
					storeClient.eventSourceId = "oxymoreWave";
					PlayerStore.Instance.storeClientManager.AddClient(storeClient);
				}
				else if (RNG.Roll(33))
				{
					StoreClient storeClient2 = StoreClientListTierSubstance.CreateDesperateAddict();
					storeClient2.eventSourceId = "oxymoreWave";
					PlayerStore.Instance.storeClientManager.AddClient(storeClient2);
				}
			}
		}
	};
}
