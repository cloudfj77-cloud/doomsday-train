public class ConsumableHelper
{
	public const string NARCOTIC_VALUE = "NARCOTIC_VALUE";

	public const string ALCOHOL_VALUE = "ALCOHOL_VALUE";

	public const string NICOTINE_VALUE = "NICOTINE_VALUE";

	public static void InitNarcotic(GameItem gameItem, int level)
	{
		gameItem.canActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item) && HealthData.CanUseNarcotic();
		gameItem.mayActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item) && HealthData.CanUseNarcotic();
		gameItem.ModifyTag("NARCOTIC_VALUE", delegate(TagState state)
		{
			state.Enable().SetInt(level);
		});
		gameItem.onActivateSlotItemFunc = delegate(GameItem item, GameInventory _, SlotMarker _)
		{
			PlayerStore.Instance.healthData.ConsumeNarcotic(item);
			item.Destroy();
		};
	}

	public static void InitNarcoticRelief(GameItem gameItem, int cover)
	{
		gameItem.canActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item) && HealthData.CanUseNarcotic();
		gameItem.mayActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item) && HealthData.CanUseNarcotic();
		gameItem.onActivateSlotItemFunc = delegate(GameItem item, GameInventory _, SlotMarker _)
		{
			PlayerStore.Instance.healthData.ConsumeNarcoticRelief(item, cover);
			item.Destroy();
		};
	}

	public static void InitAlcohol(GameItem gameItem, int level)
	{
		gameItem.canActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item) && HealthData.CanUseAlcohol();
		gameItem.mayActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item) && HealthData.CanUseAlcohol();
		gameItem.ModifyTag("ALCOHOL_VALUE", delegate(TagState state)
		{
			state.Enable().SetInt(level);
		});
		gameItem.onActivateSlotItemFunc = delegate(GameItem item, GameInventory _, SlotMarker _)
		{
			PlayerStore.Instance.healthData.ConsumeAlcohol(item.GetTagReadonly("ALCOHOL_VALUE").GetInt());
			item.Destroy();
		};
	}

	public static void InitNicotine(GameItem gameItem, int level)
	{
		gameItem.canActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item) && HealthData.CanUseNicotine();
		gameItem.mayActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item) && HealthData.CanUseNicotine();
		gameItem.ModifyTag("NICOTINE_VALUE", delegate(TagState state)
		{
			state.Enable().SetInt(level);
		});
		gameItem.onActivateSlotItemFunc = delegate(GameItem item, GameInventory _, SlotMarker _)
		{
			PlayerStore.Instance.healthData.ConsumeNicotine(item.GetTagReadonly("NICOTINE_VALUE").GetInt());
			item.Destroy();
		};
	}

	public static void InitSynaprest(GameItem gameItem)
	{
		gameItem.canActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item) && HealthData.CanUseSynaprest();
		gameItem.mayActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item) && HealthData.CanUseSynaprest();
		gameItem.onActivateSlotItemFunc = delegate(GameItem item, GameInventory _, SlotMarker _)
		{
			PlayerStore.Instance.healthData.RemoveOneRandomAddiction();
			item.Destroy();
		};
	}

	public static void InitImmunivax(GameItem gameItem)
	{
		gameItem.canActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item) && HealthData.CanUseImmunivax();
		gameItem.mayActivateSlotItemFunc = (GameItem item, GameInventory _, SlotMarker _) => GeneralHelper.IsItemOwned(item) && HealthData.CanUseImmunivax();
		gameItem.onActivateSlotItemFunc = delegate(GameItem item, GameInventory _, SlotMarker _)
		{
			PlayerStore.Instance.healthData.ConsumeImmunivax(item);
			item.Destroy();
		};
	}
}
