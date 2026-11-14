using System;
using System.Collections.Generic;

public static class ShareholderHelper
{
	private class Dividend
	{
		public string upgradeId;

		public string supplierUpgradeId;

		public int weekday;

		public Func<List<GameItem>> goods;
	}

	public const int WHOLESALE_BONUS = -15;

	public const int BREWER_DIVIDEND_WEEKDAY = 4;

	public const int MINER_DIVIDEND_WEEKDAY = 5;

	public const int CHEMIST_DIVIDEND_WEEKDAY = 6;

	public const int PHARMA_DIVIDEND_WEEKDAY = 0;

	public const int DIVIDEND_BEER_CASES = 1;

	public const int DIVIDEND_ORE = 6;

	public const int DIVIDEND_DREAM_DUST = 8;

	public const int DIVIDEND_OXYMORE = 5;

	private static readonly List<Dividend> dividends = new List<Dividend>
	{
		new Dividend
		{
			upgradeId = "BREWER_SHAREHOLDER",
			supplierUpgradeId = "BREWER",
			weekday = 4,
			goods = () => Repeat(1, () => PreBuiltItemHelper.FullBeer())
		},
		new Dividend
		{
			upgradeId = "MINER_SHAREHOLDER",
			supplierUpgradeId = "MINER",
			weekday = 5,
			goods = () => Repeat(6, () => DirectoryMaster.Item("common_ore"))
		},
		new Dividend
		{
			upgradeId = "CHEMIST_SHAREHOLDER",
			supplierUpgradeId = "CHEMIST",
			weekday = 6,
			goods = () => Repeat(8, () => DirectoryMaster.Item("dream_dust"))
		},
		new Dividend
		{
			upgradeId = "PHARMA_SHAREHOLDER",
			supplierUpgradeId = "PHARMA",
			weekday = 0,
			goods = () => Repeat(5, () => DirectoryMaster.Item("pink_injector"))
		}
	};

	public static int WholesaleDiscount(string shareholderUpgradeId, int baseDiscount)
	{
		if (!NetworkUpgrade.IsUnlocked(shareholderUpgradeId))
		{
			return baseDiscount;
		}
		return baseDiscount + -15;
	}

	public static void HandleDividends()
	{
		if (StoreStation.Instance == null || PlayerStore.Instance == null)
		{
			return;
		}
		int num = StoreStation.Instance.dayCounter % 7;
		foreach (Dividend dividend in dividends)
		{
			if (dividend.weekday != num || !NetworkUpgrade.IsUnlocked(dividend.upgradeId))
			{
				continue;
			}
			List<GameItem> list;
			try
			{
				list = dividend.goods();
			}
			catch (Exception arg)
			{
				Debug.LogError($"[ShareholderHelper] dividend of '{dividend.upgradeId}' failed: {arg}");
				continue;
			}
			foreach (GameItem item in list)
			{
				if (item != null)
				{
					PlayerStore.Instance.AddDirectSellingItemToTable(item, isOwend: true);
				}
			}
			NetworkUpgrade value;
			string text = (PlayerStore.Instance.networkUpgrade.TryGetValue(dividend.supplierUpgradeId, out value) ? value.GetLocalizedTitle() : dividend.supplierUpgradeId);
			PlayerStore.Instance.AddNightLog(LocHelper.GetLocalizedMechanic("mech_nightlog_dividend", text));
		}
	}

	private static List<GameItem> Repeat(int count, Func<GameItem> factory)
	{
		List<GameItem> list = new List<GameItem>(count);
		for (int i = 0; i < count; i++)
		{
			list.Add(factory());
		}
		return list;
	}
}
