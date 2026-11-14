using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ItemStatsSystem;

public class UsageUtilities : ItemComponent
{
	[SerializeField]
	private float useTime;

	public List<UsageBehavior> behaviors = new List<UsageBehavior>();

	public bool hasSound;

	public string actionSound;

	public string useSound;

	public bool useDurability;

	public bool dontUseDurabilityInBase;

	public int durabilityUsage = 1;

	public UnityEvent OnItemUsedEvent;

	public float UseTime => useTime;

	public static event Action<Item> OnItemUsedStaticEvent;

	public bool IsUsable(Item item, object user)
	{
		if (!item)
		{
			return false;
		}
		if (useDurability && item.Durability < (float)durabilityUsage)
		{
			return false;
		}
		if (behaviors.Count <= 0)
		{
			return true;
		}
		foreach (UsageBehavior behavior in behaviors)
		{
			if (!(behavior == null) && behavior.CanBeUsed(item, user))
			{
				return true;
			}
		}
		return false;
	}

	public void Use(Item item, object user, bool inBaseLevel = false)
	{
		foreach (UsageBehavior behavior in behaviors)
		{
			if (!(behavior == null) && behavior.CanBeUsed(item, user))
			{
				behavior.Use(item, user);
			}
		}
		if (useDurability && (!inBaseLevel || !dontUseDurabilityInBase) && item.Durability > 0f)
		{
			item.Durability -= durabilityUsage;
		}
		OnItemUsedEvent?.Invoke();
		UsageUtilities.OnItemUsedStaticEvent?.Invoke(item);
	}
}
