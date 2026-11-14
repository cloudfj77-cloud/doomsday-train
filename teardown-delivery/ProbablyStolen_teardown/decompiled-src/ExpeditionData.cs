using System.Collections.Generic;

public class ExpeditionData
{
	public List<Hireling> hirelings;

	public List<ExpeditionLocation> expeditionLocations;

	public List<Expedition> ongoingExpeditions;

	public bool isExpeditionUnlocked;

	public ExpeditionData()
	{
		hirelings = new List<Hireling>();
		expeditionLocations = new List<ExpeditionLocation>();
		ongoingExpeditions = new List<Expedition>();
		expeditionLocations.Add(ExpeditionLocationList.DumpingGrounds());
	}

	public void OnDayEnd()
	{
		if (!isExpeditionUnlocked)
		{
			return;
		}
		foreach (Expedition ongoingExpedition in ongoingExpeditions)
		{
			ongoingExpedition.ExecuteExpedition();
		}
	}

	public void MintHireling(Hireling hireling)
	{
		hirelings.Add(hireling);
		ExpeditionHelper.MintHirelingEquipment(hireling.id);
	}

	public void UnlockExpedition()
	{
		if (!isExpeditionUnlocked)
		{
			isExpeditionUnlocked = true;
			MintHireling(HirelingListUnique.Juan());
			GraphUtils.EmporiumTryAdd(ItemSpawner.Spawn("sign_expedition"));
		}
	}

	public void CreateExpedition(Hireling hireling, ExpeditionLocation expeditionLocation)
	{
		Expedition item = new Expedition(hireling, expeditionLocation);
		ongoingExpeditions.Add(item);
		hireling.Hire(expeditionLocation.id);
		ExpeditionHelper.RemoveHirelingEquipmentFromStore(hireling.id);
	}

	public void DeleteExpedition(Expedition expedition)
	{
		ongoingExpeditions.Remove(expedition);
	}
}
