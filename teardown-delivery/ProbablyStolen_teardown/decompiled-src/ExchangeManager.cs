using System.Collections.Generic;
using System.Linq;

public class ExchangeManager
{
	[ES3NonSerializable]
	public List<ExchangeBarter> activeBarter;

	[ES3NonSerializable]
	public List<ExchangeBarter> promotionalBarter;

	[ES3Serializable]
	public List<string> activePromotionalBarterIds;

	[ES3Serializable]
	public bool isExchangeUnlocked;

	[ES3NonSerializable]
	public List<GameItem> itemsBuffer;

	public ExchangeManager()
	{
		itemsBuffer = new List<GameItem>();
		activePromotionalBarterIds = new List<string>();
		Init();
	}

	public void Init()
	{
		activeBarter = new List<ExchangeBarter>();
		activeBarter.Add(ExchangeList.FilterBarter());
		activeBarter.Add(ExchangeList.ModuleBarter());
		activeBarter.Add(ExchangeList.DreamDustBarter());
		activeBarter.Add(ExchangeList.CommandKeycardBarter());
		activeBarter.Add(ExchangeList.PoisonBarter());
		activeBarter.Add(ExchangeList.BulletBarter());
		activeBarter.Add(ExchangeList.SmugglerBarter());
		promotionalBarter = new List<ExchangeBarter>();
		promotionalBarter.Add(ExchangeList.ImprovedDreamDustBarter());
		promotionalBarter.Add(ExchangeList.SecCrateBarter());
		promotionalBarter.Add(ExchangeList.MedCrateBarter());
		promotionalBarter.Add(ExchangeList.EngCrateBarter());
		promotionalBarter.Add(ExchangeList.SeedBarter());
		promotionalBarter.Add(ExchangeList.BMRepBarter());
		promotionalBarter.Add(ExchangeList.ImmunivaxBarter());
	}

	public void HandleExchange()
	{
		Debug.Log("HandlingExchange");
		int num = 0;
		List<ExchangeBarter> list = new List<ExchangeBarter>();
		list.AddRange(activeBarter);
		foreach (ExchangeBarter item in promotionalBarter)
		{
			if (activePromotionalBarterIds.Contains(item.identifier))
			{
				list.Add(item);
			}
		}
		list.Sort((ExchangeBarter x, ExchangeBarter y) => y.priority.CompareTo(x.priority));
		foreach (ExchangeBarter item2 in list)
		{
			if (item2.CanAccept())
			{
				num++;
				item2.Accept();
				StoreReputation.GetBMFaction().ModProgressReputation(75);
			}
		}
		Debug.Log(num + " barter(s) accepted");
	}

	public void Reset()
	{
		foreach (ExchangeBarter item in activeBarter)
		{
			item.Reset();
		}
		foreach (ExchangeBarter item2 in promotionalBarter)
		{
			item2.Reset();
		}
	}

	public void TransferBuffer()
	{
		foreach (GameItem item in itemsBuffer)
		{
			GraphUtils.TryAcceptAllOrDestroy(EmporiumEntry.Instance.trashcan, item, "exchange delivery, trashcan full");
		}
		itemsBuffer.Clear();
	}

	public void ResetPromotional()
	{
		activePromotionalBarterIds.Clear();
		if (promotionalBarter.Count == 0)
		{
			return;
		}
		foreach (int item in (from _ in Enumerable.Range(0, promotionalBarter.Count)
			orderby RNG.random.Next()
			select _).Take(3))
		{
			ExchangeBarter exchangeBarter = promotionalBarter[item];
			activePromotionalBarterIds.Add(exchangeBarter.identifier);
		}
		if (PlayerStore.Instance.exchangeManager.isExchangeUnlocked)
		{
			NightLog.AddNewPromotionalOffer();
		}
	}
}
