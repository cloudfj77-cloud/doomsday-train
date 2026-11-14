using System;

public class ProcurementData
{
	public int initialReward;

	public int finalReward;

	public Action onInstance;

	public Func<BarterResult> getBarterResult;

	public string descriptionOffer;

	public string descriptionCollect;

	public string clientName;

	public int day;

	public const string PROCUREMENT_HEADER = "PROCUREMENT_HEADER";

	public const string PROCUREMENT_CLIENT_NAME = "PROCUREMENT_CLIENT_NAME";

	public const string PROCUREMENT_ORDER_BODY = "PROCUREMENT_ORDER_BODY";

	public const string PROCUREMENT_FINAL_REWARD = "PROCUREMENT_FINAL_REWARD";

	public const string PROCUREMENT_DAY = "PROCUREMENT_DAY";

	public void InstanciateOffer()
	{
		if (onInstance != null)
		{
			onInstance();
		}
		else
		{
			Debug.Log("Warning, ProcurementData.onInstance was Null");
		}
	}

	public string GetDescription(StoreClient storeClient)
	{
		if (storeClient.clientIntent == StoreClient.ClientIntent.PROCUREMENT_OFFER)
		{
			return descriptionOffer;
		}
		if (storeClient.clientIntent == StoreClient.ClientIntent.PROCUREMENT_COLLECT)
		{
			return descriptionCollect;
		}
		return "ERROR ProcurementData";
	}

	public GameItem CreateSlip()
	{
		GameItem gameItem = DirectoryMaster.Item("procurement_slip");
		gameItem.ModifyTag("PROCUREMENT_HEADER", delegate(TagState state)
		{
			state.Enable().SetString("Procurement Order");
		});
		gameItem.ModifyTag("PROCUREMENT_CLIENT_NAME", delegate(TagState state)
		{
			state.Enable().SetString(clientName);
		});
		gameItem.ModifyTag("PROCUREMENT_ORDER_BODY", delegate(TagState state)
		{
			state.Enable().SetString(descriptionOffer);
		});
		gameItem.ModifyTag("PROCUREMENT_FINAL_REWARD", delegate(TagState state)
		{
			state.Enable().SetInt(finalReward);
		});
		gameItem.ModifyTag("PROCUREMENT_DAY", delegate(TagState state)
		{
			state.Enable().SetInt(day);
		});
		return gameItem;
	}
}
