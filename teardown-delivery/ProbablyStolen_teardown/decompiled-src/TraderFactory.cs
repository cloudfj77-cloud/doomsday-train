using System.Collections.Generic;

public class TraderFactory
{
	public static GameCharacterItem CreateTrader()
	{
		Dialogue mainDialogue = new Dialogue();
		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
		trader.name = "Trader";
		trader.BarterEnable(TradeSheet.GetGenericTraderShop());
		trader.SetDialogue(mainDialogue);
		mainDialogue.SetText("Trader", "Hello, looking to buy or sell?").SetEndAction(delegate
		{
			mainDialogue.SetText("Trader", "Need anything else?");
		}).AddChoice(DirectoryMaster.Item("trade"), "Trade", "", null, delegate
		{
			trader.BarterShow();
		});
		return trader;
	}

	public static GameCharacterItem CreateDoctor()
	{
		Dialogue mainDialogue = new Dialogue();
		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
		trader.name = "Doctor";
		trader.BarterEnable(TradeSheet.GetGenericMedicalShop());
		trader.SetDialogue(mainDialogue);
		mainDialogue.SetText("Trader", "Hello, looking to buy or sell?").SetEndAction(delegate
		{
			mainDialogue.SetText("Trader", "Need anything else?");
		}).AddChoice(DirectoryMaster.Item("trade"), "Trade", "", null, delegate
		{
			trader.BarterShow();
		});
		return trader;
	}

	public static GameCharacterItem CreateArmyVendor()
	{
		Dialogue mainDialogue = new Dialogue();
		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
		trader.name = "Military Surplus Vendor";
		trader.BarterEnable(TradeSheet.GetGenericMilitaryShop());
		trader.SetDialogue(mainDialogue);
		mainDialogue.SetText("Trader", "Hello, looking to buy or sell?").SetEndAction(delegate
		{
			mainDialogue.SetText("Trader", "Need anything else?");
		}).AddChoice(DirectoryMaster.Item("trade"), "Trade", "", null, delegate
		{
			trader.BarterShow();
		});
		return trader;
	}

	public static GameCharacterItem CreateFoundryTrader()
	{
		Dialogue mainDialogue = new Dialogue();
		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
		trader.name = "Foundry Sales Rep.";
		trader.BarterEnable(TradeSheet.GetFoundryShop());
		trader.SetDialogue(mainDialogue);
		mainDialogue.SetText("Foundry Sales Rep.", "Hello, looking to buy or sell?").SetEndAction(delegate
		{
			mainDialogue.SetText("Foundry Sales Rep.", "Need anything else?");
		}).AddChoice(DirectoryMaster.Item("trade"), "Trade", "", null, delegate
		{
			trader.BarterShow();
		});
		BarterHelper.SetOnlyAcceptTaggedItemAsPayment(new List<string> { "power_source_item", "ORE_TAG", "SCRAP_METAL" }, trader);
		return trader;
	}

	public static GameCharacterItem CreateEnergyFarmTrader()
	{
		Dialogue mainDialogue = new Dialogue();
		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
		trader.name = "Energy Farm Sales Rep.";
		trader.BarterEnable(TradeSheet.GetEnergyFarmShop());
		trader.SetDialogue(mainDialogue);
		mainDialogue.SetText(trader.name, "Hello, looking to buy or sell?").SetEndAction(delegate
		{
			mainDialogue.SetText(trader.name, "Need anything else?");
		}).AddChoice(DirectoryMaster.Item("trade"), "Trade", "", null, delegate
		{
			trader.BarterShow();
		});
		new List<string>();
		new List<int>();
		BarterHelper.SetOnlyAcceptTaggedItemAsPayment(new List<string> { "power_source_item" }, trader);
		return trader;
	}

	public static GameCharacterItem CreateNutrifruitFarmTrader()
	{
		Dialogue mainDialogue = new Dialogue();
		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
		trader.name = "Nutrifruit Farm Sales Rep.";
		trader.BarterEnable(TradeSheet.GetNutrifruitFarmShop());
		trader.SetDialogue(mainDialogue);
		mainDialogue.SetText(trader.name, "Hello, looking to buy or sell?").SetEndAction(delegate
		{
			mainDialogue.SetText(trader.name, "Need anything else?");
		}).AddChoice(DirectoryMaster.Item("trade"), "Trade", "", null, delegate
		{
			trader.BarterShow();
		});
		new List<string>();
		new List<int>();
		BarterHelper.SetOnlyAcceptTaggedItemAsPayment(new List<string> { "power_source_item" }, trader);
		return trader;
	}

	public static GameCharacterItem CreateFenceTrader()
	{
		Dialogue mainDialogue = new Dialogue();
		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
		trader.name = "Fence.";
		trader.BarterEnable(TradeSheet.GetFenceShop());
		trader.SetDialogue(mainDialogue);
		mainDialogue.SetText("Fence", "Hello, looking to buy or sell?").SetEndAction(delegate
		{
			mainDialogue.SetText("Fence.", "Need anything else?");
		}).AddChoice(DirectoryMaster.Item("trade"), "Trade", "", null, delegate
		{
			trader.BarterShow();
		});
		List<string> tagList = new List<string> { "CONTRABAND_ITEM_TAG" };
		List<int> discountList = new List<int> { 200 };
		BarterHelper.SetVendorDiscount(trader, tagList, discountList);
		BarterHelper.SetTraderOverrallBuyPrice(trader, 0.9);
		return trader;
	}
}
