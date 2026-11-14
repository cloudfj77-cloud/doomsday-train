using System;
using System.Collections.Generic;

public class ItemConditionActionList
{
	[ES3NonSerializable]
	public static Dictionary<string, Action<GameItem>> ItemConditionActionDict = new Dictionary<string, Action<GameItem>>
	{
		{ "counterfeitCigarette", CounterfeitCigaretteAction },
		{ "genuineCigarette", GenuineCigaretteAction },
		{ "counterfeitStamp", CounterfeitStampAction },
		{ "genuineInjector", GenuineInjectorAction },
		{ "unusableInjector", UnusableInjectorAction },
		{ "expiredInjector", ExpiredInjectorAction },
		{ "counterfeitInjector", CounterfeitInjectorAction }
	};

	public static void CounterfeitCigaretteAction(GameItem item)
	{
		item.name = LocHelper.GetLocalizedItem("item_packet_red_cigarette_name") + " " + LocHelper.GetLocalizedMechanic("mech_suffix_counterfeit");
		item.unitValue = 50L;
		item.RemoveGameItemType("LUXURY_ITEM");
	}

	public static void GenuineCigaretteAction(GameItem item)
	{
		item.name = LocHelper.GetLocalizedItem("item_packet_red_cigarette_name");
		item.unitValue = 100L;
		item.SetGameItemType("LUXURY_ITEM");
	}

	public static void CounterfeitStampAction(GameItem item)
	{
		item.name = LocHelper.GetLocalizedItem("item_food_stamp_name") + " " + LocHelper.GetLocalizedMechanic("mech_suffix_counterfeit");
		item.unitValue = 8L;
		item.canActivateSlotItemFunc = (GameItem curItem, GameInventory _, SlotMarker _) => false;
		item.mayActivateSlotItemFunc = (GameItem curItem, GameInventory _, SlotMarker _) => false;
	}

	public static void GenuineInjectorAction(GameItem item)
	{
		item.name = LocHelper.GetLocalizedItem("item_large_purple_injector_name");
		item.unitValue = 160L;
	}

	public static void UnusableInjectorAction(GameItem item)
	{
		item.name = LocHelper.GetLocalizedItem("item_large_purple_injector_name") + " " + LocHelper.GetLocalizedMechanic("mech_suffix_unusable");
		item.unitValue = 8L;
	}

	public static void ExpiredInjectorAction(GameItem item)
	{
		item.name = LocHelper.GetLocalizedItem("item_large_purple_injector_name") + " " + LocHelper.GetLocalizedMechanic("mech_suffix_expired");
		item.unitValue = 80L;
	}

	public static void CounterfeitInjectorAction(GameItem item)
	{
		item.name = LocHelper.GetLocalizedItem("item_large_purple_injector_name") + " " + LocHelper.GetLocalizedMechanic("mech_suffix_counterfeit");
		item.unitValue = 40L;
	}
}
