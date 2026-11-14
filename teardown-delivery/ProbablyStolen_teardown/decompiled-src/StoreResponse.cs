using System;
using System.Collections.Generic;

public class StoreResponse
{
	public string identifier;

	public string title;

	public bool closable;

	public List<(GameItem, Action)> choices;

	public static void ShowAfterhourResponse()
	{
		EmporiumEntry.Instance.ShowResponseWindow(ResponseList.CreateAfterhoursResponse());
	}

	public static void ShowSupplyCrateResponse(GameItem supplyCrate)
	{
		PreBuiltItemHelper.MarkCrateScanned(supplyCrate);
		StoreResponse storeResponse = new StoreResponse();
		storeResponse.identifier = "supplyCrateResponse";
		storeResponse.title = "Supply Crate Contents";
		storeResponse.choices = new List<(GameItem, Action)>();
		storeResponse.closable = true;
		foreach (GameItem item in GraphUtils.FindChildrenType<GameItem>(supplyCrate))
		{
			storeResponse.choices.Add(EmporiumEntry.PrepChoice(DirectoryMaster.Item(item.identifier), "Scanned Item", isRealItem: false, "", null, null, delegate
			{
			}, isBlackout: true, canActivate: false));
		}
		EmporiumEntry.Instance.ShowResponseWindow(storeResponse);
	}
}
