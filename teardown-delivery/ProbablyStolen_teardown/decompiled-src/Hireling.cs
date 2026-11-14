using System.Collections.Generic;

public class Hireling
{
	public enum HirelingState
	{
		Normal,
		Combat
	}

	public enum HirelingStatus
	{
		Available,
		OnExpedition,
		Injured,
		Complete
	}

	public enum HirelingTrait
	{
		Fighter,
		Coward,
		Survivor,
		Murderous,
		Butcher,
		Addict,
		Antisocial,
		Lucky,
		Junker,
		TreasureHunter,
		Friendly
	}

	public string id;

	public string displayName;

	public string spriteId = "default";

	public int health;

	public int maxHealth;

	public int courage;

	public List<HirelingTrait> traits;

	public string assignedLocationId;

	public int baseCost = 50;

	public HirelingState state;

	public HirelingStatus status;

	public bool IsPerk(HirelingTrait trait)
	{
		return traits.Contains(trait);
	}

	public void Hire(string locationId)
	{
		status = HirelingStatus.OnExpedition;
		assignedLocationId = locationId;
	}

	public void OnExpeditionReported()
	{
		Expedition currentExpedition = GetCurrentExpedition();
		if (currentExpedition != null)
		{
			currentExpedition.DisplayDebugLog();
			GameItem item = currentExpedition.GeneratorLogItem();
			GraphUtils.EmporiumTryAdd(EmporiumEntry.Instance.frontInvinvElement, item);
			PlayerStore.Instance.expeditionData.DeleteExpedition(currentExpedition);
			UpdateInjuredStatus();
		}
	}

	public bool IsInjured()
	{
		if (health <= 50)
		{
			return true;
		}
		return false;
	}

	public void UpdateInjuredStatus()
	{
		if (IsInjured())
		{
			status = HirelingStatus.Injured;
		}
		else
		{
			status = HirelingStatus.Available;
		}
	}

	public Expedition GetCurrentExpedition()
	{
		for (int i = 0; i < PlayerStore.Instance.expeditionData.ongoingExpeditions.Count; i++)
		{
			if (PlayerStore.Instance.expeditionData.ongoingExpeditions[i].hireling.id == id)
			{
				return PlayerStore.Instance.expeditionData.ongoingExpeditions[i];
			}
		}
		return null;
	}

	public void Damage(int damage)
	{
		health -= damage;
		if (health <= 0)
		{
			health = 0;
			status = HirelingStatus.Injured;
		}
	}

	public void Heal(int heal)
	{
		health += heal;
		if (health > maxHealth)
		{
			health = maxHealth;
		}
	}

	public void Rest()
	{
		Heal(10);
	}
}
