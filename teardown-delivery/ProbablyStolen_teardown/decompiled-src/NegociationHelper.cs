using System.Collections.Generic;

public class NegociationHelper
{
	public const string CHECK_INGOT_FLASHLIGHT = "CHECK_INGOT_FLASHLIGHT";

	public const string CHECK_INGOT_DETECTOR = "CHECK_INGOT_DETECTOR";

	public const string CHECK_OPEN_BOX = "CHECK_OPEN_BOX";

	public const string CHECK_RUST = "CHECK_RUST";

	public const string CHECK_DENTED = "CHECK_DENTED";

	public const string CHECK_EXPIRED = "CHECK_EXPIRED";

	public const string CHECK_FRESHNESS = "CHECK_FRESHNESS";

	public const string CHECK_WATER_FLASHLIGHT = "CHECK_WATER_FLASHLIGHT";

	public const string CHECK_WATER_STRIP = "CHECK_WATER_STRIP";

	public static bool IsClientNegociating(GameItem gameItem, StoreClient storeClient)
	{
		int num = 0;
		foreach (ItemFeature itemFeature in gameItem.itemFeatures)
		{
			num += CheckIngotFeatureFlashlight(gameItem, itemFeature, storeClient);
			num += CheckIngotFeatureDetector(gameItem, itemFeature, storeClient);
			num += CheckWaterFeatureFlashlight(gameItem, itemFeature, storeClient);
			num += CheckWaterFeatureStrip(gameItem, itemFeature, storeClient);
			num += CheckOpenBoxFeature(gameItem, itemFeature, storeClient);
			num += CheckRustedFeature(gameItem, itemFeature, storeClient);
			num += CheckDentedFeature(gameItem, itemFeature, storeClient);
			num += CheckExpiredFeature(gameItem, itemFeature, storeClient);
			num += CheckFreshnessFeature(gameItem, itemFeature, storeClient);
		}
		if (num > 0)
		{
			return true;
		}
		return false;
	}

	public static void ClientNegociate(GameItem gameItem, StoreClient storeClient)
	{
		List<Dialogue> list = new List<Dialogue>();
		foreach (ItemFeature itemFeature in gameItem.itemFeatures)
		{
			AddDialogToList(list, NegociateIngotFeatureFlashlight(gameItem, itemFeature, storeClient));
			AddDialogToList(list, NegociateIngotFeatureDetector(gameItem, itemFeature, storeClient));
			AddDialogToList(list, NegociateWaterFeatureFlashlight(gameItem, itemFeature, storeClient));
			AddDialogToList(list, NegociateWaterFeatureStrip(gameItem, itemFeature, storeClient));
			AddDialogToList(list, NegociateOpenBoxFeature(gameItem, itemFeature, storeClient));
			AddDialogToList(list, NegociateRustedFeature(gameItem, itemFeature, storeClient));
			AddDialogToList(list, NegociateDentedFeature(gameItem, itemFeature, storeClient));
			AddDialogToList(list, NegociateExpiredFeature(gameItem, itemFeature, storeClient));
			AddDialogToList(list, NegociateFreshnessFeature(gameItem, itemFeature, storeClient));
		}
		for (int i = 0; i < list.Count - 1; i++)
		{
			list[i].nextDialogue = list[i + 1];
		}
		if (list.Count > 0)
		{
			DialogUIManager.Instance.StartDialogue(list[0]);
		}
	}

	public static int CheckFreshnessFeature(GameItem gameItem, ItemFeature itemFeature, StoreClient storeClient)
	{
		if (!CheckBasic(gameItem, itemFeature))
		{
			return 0;
		}
		if (storeClient.ContainCheck("CHECK_FRESHNESS") && itemFeature.IsCategory("CATEGORY_FRESHNESS") && itemFeature.WillClientExposeFeature())
		{
			return 1;
		}
		return 0;
	}

	public static Dialogue NegociateFreshnessFeature(GameItem gameItem, ItemFeature itemFeature, StoreClient storeClient)
	{
		if (CheckFreshnessFeature(gameItem, itemFeature, storeClient) == 1)
		{
			Dialogue dialogue = new Dialogue();
			dialogue.SetText("CLIENT", "This is not as fresh as you said.").SetEndAction(delegate
			{
				itemFeature.ExposeFeature(lowkey: true);
				OnFeatureExposed(storeClient);
			});
			return dialogue;
		}
		return null;
	}

	public static int CheckRustedFeature(GameItem gameItem, ItemFeature itemFeature, StoreClient storeClient)
	{
		if (!CheckBasic(gameItem, itemFeature))
		{
			return 0;
		}
		if (storeClient.ContainCheck("CHECK_RUST") && itemFeature.IsCategory("CATEGORY_RUSTED") && itemFeature.realCondition.identifier.Equals("rusted"))
		{
			return 1;
		}
		return 0;
	}

	public static Dialogue NegociateRustedFeature(GameItem gameItem, ItemFeature itemFeature, StoreClient storeClient)
	{
		if (CheckRustedFeature(gameItem, itemFeature, storeClient) == 1)
		{
			Dialogue dialogue = new Dialogue();
			dialogue.SetText("CLIENT", "This thing is rusted.").SetEndAction(delegate
			{
				itemFeature.ExposeFeature(lowkey: true);
				OnFeatureExposed(storeClient);
			});
			return dialogue;
		}
		return null;
	}

	public static int CheckDentedFeature(GameItem gameItem, ItemFeature itemFeature, StoreClient storeClient)
	{
		if (!CheckBasic(gameItem, itemFeature))
		{
			return 0;
		}
		if (storeClient.ContainCheck("CHECK_DENTED") && itemFeature.IsCategory("CATEGORY_DENTED") && itemFeature.realCondition.identifier.Equals("dented"))
		{
			return 1;
		}
		return 0;
	}

	public static Dialogue NegociateDentedFeature(GameItem gameItem, ItemFeature itemFeature, StoreClient storeClient)
	{
		if (CheckDentedFeature(gameItem, itemFeature, storeClient) == 1)
		{
			Dialogue dialogue = new Dialogue();
			dialogue.SetText("CLIENT", "This thing is rusted.").SetEndAction(delegate
			{
				itemFeature.ExposeFeature(lowkey: true);
				OnFeatureExposed(storeClient);
			});
			return dialogue;
		}
		return null;
	}

	public static int CheckExpiredFeature(GameItem gameItem, ItemFeature itemFeature, StoreClient storeClient)
	{
		if (!CheckBasic(gameItem, itemFeature))
		{
			return 0;
		}
		if (storeClient.ContainCheck("CHECK_EXPIRED") && itemFeature.IsCategory("CATEGORY_EXPIRED") && itemFeature.realCondition.identifier.Equals("expired"))
		{
			return 1;
		}
		return 0;
	}

	public static Dialogue NegociateExpiredFeature(GameItem gameItem, ItemFeature itemFeature, StoreClient storeClient)
	{
		if (CheckExpiredFeature(gameItem, itemFeature, storeClient) == 1)
		{
			Dialogue dialogue = new Dialogue();
			dialogue.SetText("CLIENT", "This thing is rusted.").SetEndAction(delegate
			{
				itemFeature.ExposeFeature(lowkey: true);
				OnFeatureExposed(storeClient);
			});
			return dialogue;
		}
		return null;
	}

	public static int CheckOpenBoxFeature(GameItem gameItem, ItemFeature itemFeature, StoreClient storeClient)
	{
		if (!CheckBasic(gameItem, itemFeature))
		{
			return 0;
		}
		if (storeClient.ContainCheck("CHECK_OPEN_BOX") && itemFeature.IsCategory("CATEGORY_OPEN_BOX") && itemFeature.realCondition.identifier.Equals("open_box"))
		{
			return 1;
		}
		return 0;
	}

	public static Dialogue NegociateOpenBoxFeature(GameItem gameItem, ItemFeature itemFeature, StoreClient storeClient)
	{
		if (CheckOpenBoxFeature(gameItem, itemFeature, storeClient) == 1)
		{
			Dialogue dialogue = new Dialogue();
			dialogue.SetText("CLIENT", "This is not factory-sealed like you said.").SetEndAction(delegate
			{
				itemFeature.ExposeFeature(lowkey: true);
				OnFeatureExposed(storeClient);
			});
			return dialogue;
		}
		return null;
	}

	public static int CheckIngotFeatureFlashlight(GameItem gameItem, ItemFeature itemFeature, StoreClient storeClient)
	{
		if (!CheckBasic(gameItem, itemFeature))
		{
			return 0;
		}
		if (storeClient.ContainCheck("CHECK_INGOT_FLASHLIGHT") && itemFeature.IsCategory("CATEGORY_INGOT_PURITY") && IngotPurityHelper.IsDiscovableByFlashlight(gameItem))
		{
			return 1;
		}
		return 0;
	}

	public static Dialogue NegociateIngotFeatureFlashlight(GameItem gameItem, ItemFeature itemFeature, StoreClient storeClient)
	{
		if (CheckIngotFeatureFlashlight(gameItem, itemFeature, storeClient) == 1)
		{
			Dialogue dialogue = new Dialogue();
			dialogue.SetText("CLIENT", "This ingot's purity is clearly lower").SetEndAction(delegate
			{
				itemFeature.ExposeFeature(lowkey: true);
				OnFeatureExposed(storeClient);
			});
			return dialogue;
		}
		return null;
	}

	public static int CheckIngotFeatureDetector(GameItem gameItem, ItemFeature itemFeature, StoreClient storeClient)
	{
		if (!CheckBasic(gameItem, itemFeature))
		{
			return 0;
		}
		if (storeClient.ContainCheck("CHECK_INGOT_DETECTOR") && itemFeature.IsCategory("CATEGORY_INGOT_PURITY") && IngotPurityHelper.IsDiscovableByDector(gameItem) && itemFeature.WillClientExposeFeature())
		{
			return 1;
		}
		return 0;
	}

	public static Dialogue NegociateIngotFeatureDetector(GameItem gameItem, ItemFeature itemFeature, StoreClient storeClient)
	{
		if (CheckIngotFeatureDetector(gameItem, itemFeature, storeClient) == 1)
		{
			Dialogue dialogue = new Dialogue();
			dialogue.SetText("CLIENT", "This ingot's purity is clearly lower").SetEndAction(delegate
			{
				itemFeature.ExposeFeature(lowkey: true);
				OnFeatureExposed(storeClient);
			});
			return dialogue;
		}
		return null;
	}

	public static int CheckWaterFeatureStrip(GameItem gameItem, ItemFeature itemFeature, StoreClient storeClient)
	{
		if (!CheckBasic(gameItem, itemFeature))
		{
			return 0;
		}
		if (storeClient.ContainCheck("CHECK_WATER_FLASHLIGHT") && itemFeature.IsCategory("CATEGORY_WATER_PURITY") && itemFeature.WillClientExposeFeature())
		{
			return 1;
		}
		return 0;
	}

	public static Dialogue NegociateWaterFeatureStrip(GameItem gameItem, ItemFeature itemFeature, StoreClient storeClient)
	{
		if (CheckWaterFeatureStrip(gameItem, itemFeature, storeClient) == 1)
		{
			Dialogue dialogue = new Dialogue();
			dialogue.SetText("CLIENT", "This water's purity is lower").SetEndAction(delegate
			{
				itemFeature.ExposeFeature(lowkey: true);
				OnFeatureExposed(storeClient);
			});
			return dialogue;
		}
		return null;
	}

	public static int CheckWaterFeatureFlashlight(GameItem gameItem, ItemFeature itemFeature, StoreClient storeClient)
	{
		if (!CheckBasic(gameItem, itemFeature))
		{
			return 0;
		}
		if (storeClient.ContainCheck("CHECK_WATER_FLASHLIGHT") && itemFeature.IsCategory("CATEGORY_WATER_PURITY") && WaterFeatureHelper.CanBeCheckByFlashlight(gameItem) && itemFeature.WillClientExposeFeature())
		{
			return 1;
		}
		return 0;
	}

	public static Dialogue NegociateWaterFeatureFlashlight(GameItem gameItem, ItemFeature itemFeature, StoreClient storeClient)
	{
		if (CheckWaterFeatureFlashlight(gameItem, itemFeature, storeClient) == 1)
		{
			Dialogue dialogue = new Dialogue();
			dialogue.SetText("CLIENT", "This water is clearly not as pure as you are saying.").SetEndAction(delegate
			{
				itemFeature.ExposeFeature(lowkey: true);
				OnFeatureExposed(storeClient);
			});
			return dialogue;
		}
		return null;
	}

	public static void AddDialogToList(List<Dialogue> dialogues, Dialogue dialogue)
	{
		if (dialogue != null)
		{
			dialogues.Add(dialogue);
		}
	}

	public static void OnFeatureExposed(StoreClient storeClient)
	{
		EmporiumAudioPlayer.Instance.audioExposeScam.Play();
		if (!storeClient.isAug)
		{
			StoreReputation.ModReputation(storeClient.clientFaction, -1);
		}
	}

	public static bool CheckBasic(GameItem gameItem, ItemFeature itemFeature)
	{
		if (itemFeature.isDisabled)
		{
			return false;
		}
		if (!itemFeature.IsExposed() && !itemFeature.IsFeatureMatch())
		{
			return true;
		}
		return false;
	}
}
