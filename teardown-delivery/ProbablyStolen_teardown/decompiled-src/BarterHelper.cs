using System.Collections.Generic;

public class BarterHelper
{
	public const double GLOBAL_BUY_PRICE_PENALTY = 1.1;

	public const double GLOBAL_SELL_PRICE_PENALTY = 0.9;

	public const string DISCOUNT_TAG_LIST = "DISCOUNT_TAG_LIST";

	public const string ONLY_ACCEPT_AS_PAYMENT_TAG_ITEM_LIST = "ONLY_ACCEPT_AS_PAYMENT_TAG_ITEM_LIST";

	public const string ONLY_ACCEPT_AS_PAYMENT_TAG = "ONLY_ACCEPT_AS_PAYMENT_TAG";

	public const string NEVER_ACCEPT_AS_PAYMENT_TAG_ITEM_LIST = "NEVER_ACCEPT_AS_PAYMENT_TAG_ITEM_LIST";

	public const string NEVER_ACCEPT_AS_PAYMENT_TAG = "NEVER_ACCEPT_AS_PAYMENT_TAG";

	public const string TRADER_PRICE_BUY_MULTIPLIER_TAG = "TRADER_PRICE_BUY_MULTIPLIER_TAG";

	public const string TRADER_PRICE_BUY_MULTIPLIER_DOUBLE = "TRADER_PRICE_BUY_MULTIPLIER_DOUBLE";

	public const string TRADER_PRICE_SELL_MULTIPLIER_TAG = "TRADER_PRICE_SELL_MULTIPLIER_TAG";

	public const string TRADER_PRICE_SELL_MULTIPLIER_DOUBLE = "TRADER_PRICE_SELL_MULTIPLIER_DOUBLE";

	public static double GetDiscountMultipler(GameItem item, List<string> tagList, List<int> discountList)
	{
		if (tagList.Count != discountList.Count)
		{
			Debug.Log("Error: Barter taglist and discout list count is not matching!");
			return 1.0;
		}
		double num = 1.0;
		for (int i = 0; i < tagList.Count; i++)
		{
			if (item.IsTag(tagList[i]))
			{
				double num2 = discountList[i];
				num += num2 / 100.0;
			}
		}
		if (num < 0.0)
		{
			num = 0.01;
		}
		return num;
	}

	public static double GetVendorDiscount(GameCharacterItem gci, GameItem item)
	{
		(List<string>, List<int>) tuple = MetaHelper.RetrieveListInTagSystemStringInt("DISCOUNT_TAG_LIST", gci);
		List<string> item2 = tuple.Item1;
		List<int> item3 = tuple.Item2;
		double num = 1.0;
		if (gci.IsTag("TRADER_PRICE_BUY_MULTIPLIER_TAG") && item.IsTag(GameCharacterItem.TAG_NOT_PURCHASED))
		{
			num = gci.GetTagReadonly("TRADER_PRICE_BUY_MULTIPLIER_DOUBLE").GetDouble();
		}
		if (gci.IsTag("TRADER_PRICE_SELL_MULTIPLIER_DOUBLE") && !item.IsTag(GameCharacterItem.TAG_NOT_PURCHASED))
		{
			num = gci.GetTagReadonly("TRADER_PRICE_SELL_MULTIPLIER_DOUBLE").GetDouble();
		}
		return GetDiscountMultipler(item, item2, item3) * num;
	}

	public static double GetVendorDiscountItemOnly(GameCharacterItem gci, GameItem item)
	{
		var (tagList, discountList) = MetaHelper.RetrieveListInTagSystemStringInt("DISCOUNT_TAG_LIST", gci);
		return GetDiscountMultipler(item, tagList, discountList);
	}

	public static double GetVendorOverrallBuyMultiplier(GameCharacterItem gci, GameItem item)
	{
		double result = 1.0;
		if (gci.IsTag("TRADER_PRICE_BUY_MULTIPLIER_TAG"))
		{
			result = gci.GetTagReadonly("TRADER_PRICE_BUY_MULTIPLIER_DOUBLE").GetDouble();
		}
		return result;
	}

	public static double GetVendorOverrallSellMultiplier(GameCharacterItem gci, GameItem item)
	{
		double result = 1.0;
		if (gci.IsTag("TRADER_PRICE_SELL_MULTIPLIER_DOUBLE"))
		{
			result = gci.GetTagReadonly("TRADER_PRICE_SELL_MULTIPLIER_TAG").GetDouble();
		}
		return result;
	}

	public static void SetVendorDiscount(GameCharacterItem gci, List<string> tagList, List<int> discountList)
	{
		MetaHelper.StoreListInTagSystemStringInt(tagList, discountList, "DISCOUNT_TAG_LIST", gci);
	}

	public static void CreateDiscountedBarteringItemTooltip(RichTextBuilder builder, GameItem item)
	{
		GameCharacterItem currentBarterCharacter = OverlayHandler.current.currentBarterCharacter;
		if (currentBarterCharacter == null)
		{
			return;
		}
		double vendorDiscountItemOnly = GetVendorDiscountItemOnly(currentBarterCharacter, item);
		double vendorOverrallBuyMultiplier = GetVendorOverrallBuyMultiplier(currentBarterCharacter, item);
		double vendorOverrallSellMultiplier = GetVendorOverrallSellMultiplier(currentBarterCharacter, item);
		if (vendorDiscountItemOnly > 1.0)
		{
			if (item.IsTag(GameCharacterItem.TAG_NOT_PURCHASED))
			{
				builder.AddLine("Value Modifier (Shortage): x" + vendorDiscountItemOnly, ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
			}
			else
			{
				builder.AddLine("Value Modifier (Shortage): x" + vendorDiscountItemOnly, ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
		}
		else if (vendorDiscountItemOnly < 1.0)
		{
			if (item.IsTag(GameCharacterItem.TAG_NOT_PURCHASED))
			{
				builder.AddLine("Value Modifier (Discount): x" + vendorDiscountItemOnly, ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
			else
			{
				builder.AddLine("Value Modifier (Surplus): x" + vendorDiscountItemOnly, ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
			}
		}
		if (!vendorOverrallBuyMultiplier.Equals(1.0) && item.IsTag(GameCharacterItem.TAG_NOT_PURCHASED))
		{
			if (vendorOverrallBuyMultiplier > 1.0)
			{
				builder.AddLine("Trader Buy Modifier: x" + vendorOverrallBuyMultiplier, ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
			}
			else
			{
				builder.AddLine("Trader Buy Modifier: x" + vendorOverrallBuyMultiplier, ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
		}
		if (!vendorOverrallSellMultiplier.Equals(1.0) && !item.IsTag(GameCharacterItem.TAG_NOT_PURCHASED))
		{
			if (vendorOverrallSellMultiplier > 1.0)
			{
				builder.AddLine("Trader Sell Modifier: x" + vendorOverrallSellMultiplier, ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
			else
			{
				builder.AddLine("Trader Sell Modifier: x" + vendorOverrallSellMultiplier, ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
			}
		}
	}

	public static void CreateTaggedItemAsPaymentTooltip(RichTextBuilder builder, GameItem item)
	{
		GameCharacterItem currentBarterCharacter = OverlayHandler.current.currentBarterCharacter;
		if (currentBarterCharacter != null && !item.IsTag(GameCharacterItem.TAG_NOT_PURCHASED) && DoesTraderAcceptThisItemAsPayment(item, currentBarterCharacter) != 1.0)
		{
			builder.AddLine("The Trader's not interested to trade this", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
		}
	}

	public static void SetOnlyAcceptTaggedItemAsPayment(List<string> itemTags, GameCharacterItem trader)
	{
		trader.EnableTag("ONLY_ACCEPT_AS_PAYMENT_TAG");
		MetaHelper.StoreListInTagSystem(itemTags, "ONLY_ACCEPT_AS_PAYMENT_TAG_ITEM_LIST", trader);
	}

	public static double DoesTraderAcceptThisItemAsPayment(GameItem item, GameCharacterItem trader)
	{
		if (trader.IsTag("ONLY_ACCEPT_AS_PAYMENT_TAG"))
		{
			foreach (string item2 in MetaHelper.RetrieveListInTagSystem("ONLY_ACCEPT_AS_PAYMENT_TAG_ITEM_LIST", trader))
			{
				if (item.IsTag(item2))
				{
					return 1.0;
				}
			}
			return 0.0;
		}
		if (trader.IsTag("NEVER_ACCEPT_AS_PAYMENT_TAG"))
		{
			foreach (string item3 in MetaHelper.RetrieveListInTagSystem("NEVER_ACCEPT_AS_PAYMENT_TAG_ITEM_LIST", trader))
			{
				if (item.IsTag(item3))
				{
					return 0.0;
				}
			}
			return 1.0;
		}
		return 1.0;
	}

	public static void SetTraderOverrallBuyPrice(GameCharacterItem trader, double multiplier)
	{
		trader.EnableTag("TRADER_PRICE_BUY_MULTIPLIER_TAG");
		trader.ModifyTag("TRADER_PRICE_BUY_MULTIPLIER_DOUBLE", delegate(TagState state)
		{
			state.SetDouble(multiplier);
		});
	}

	public static void SetTraderOverrallSellPrice(GameCharacterItem trader, double multiplier)
	{
		trader.EnableTag("TRADER_PRICE_SELL_MULTIPLIER_TAG");
		trader.ModifyTag("TRADER_PRICE_SELL_MULTIPLIER_DOUBLE", delegate(TagState state)
		{
			state.SetDouble(multiplier);
		});
	}
}
