using System.Collections.Generic;
using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using UnityEngine;

namespace Duckov.ItemBuilders;

public class ItemBuilder
{
	public int typeID;

	public Sprite icon;

	public int maxStackCount = 1;

	public int stackCount = 1;

	public Dictionary<string, SlotDesc> slots = new Dictionary<string, SlotDesc>();

	public Dictionary<string, StatDesc> stats = new Dictionary<string, StatDesc>();

	public CustomDataCollection constants = new CustomDataCollection();

	public CustomDataCollection variables = new CustomDataCollection();

	public List<ModifierDescription> modifiers = new List<ModifierDescription>();

	public static ItemBuilder New()
	{
		return new ItemBuilder();
	}

	public Item Instantiate()
	{
		Item item = new GameObject($"CustomItem_{typeID}").AddComponent<Item>();
		item.SetTypeID(typeID);
		item.Icon = icon;
		item.MaxStackCount = maxStackCount;
		if (maxStackCount > 1)
		{
			item.StackCount = stackCount;
		}
		item.CreateSlotsComponent();
		item.CreateStatsComponent();
		item.CreateInventoryComponent();
		item.CreateModifiersComponent();
		foreach (SlotDesc value in slots.Values)
		{
			Slot slot = new Slot(value.key);
			slot.requireTags.AddRange(value.requireTags);
			slot.excludeTags.AddRange(value.excludeTags);
			item.Slots.Add(slot);
		}
		foreach (StatDesc value2 in stats.Values)
		{
			Stat item2 = new Stat(value2.key, value2.value, value2.display);
			item.Stats.Add(item2);
		}
		foreach (CustomData constant in constants)
		{
			item.Constants.Add(new CustomData(constant));
		}
		foreach (CustomData variable in variables)
		{
			item.Variables.Add(new CustomData(variable));
		}
		foreach (ModifierDescription modifier in modifiers)
		{
			item.Modifiers.Add(modifier);
		}
		item.Modifiers.ReapplyModifiers();
		return item;
	}

	public ItemBuilder Icon(Sprite sprite)
	{
		icon = sprite;
		return this;
	}

	public ItemBuilder TypeID(int id)
	{
		typeID = id;
		return this;
	}

	public ItemBuilder EnableStacking(int maxStackCount, int stackCount)
	{
		this.maxStackCount = maxStackCount;
		this.stackCount = stackCount;
		return this;
	}

	public ItemBuilder DisableStacking()
	{
		maxStackCount = 1;
		stackCount = 1;
		return this;
	}

	public ItemBuilder Slot(string key, List<Tag> requireTags, List<Tag> excludeTags)
	{
		List<Tag> list = new List<Tag>();
		List<Tag> list2 = new List<Tag>();
		if (requireTags != null)
		{
			list.AddRange(requireTags);
		}
		if (excludeTags != null)
		{
			list2.AddRange(excludeTags);
		}
		slots[key] = new SlotDesc
		{
			key = key,
			requireTags = list,
			excludeTags = list2
		};
		return this;
	}

	public ItemBuilder Slot(string key, Tag requireTag, Tag excludeTag = null)
	{
		if (string.IsNullOrWhiteSpace(key))
		{
			Debug.LogError("[Item Builder] Key of slot cannot be empty");
			return this;
		}
		List<Tag> list = new List<Tag>();
		List<Tag> list2 = new List<Tag>();
		if (requireTag != null)
		{
			list.Add(requireTag);
		}
		if (excludeTag != null)
		{
			list2.Add(excludeTag);
		}
		slots[key] = new SlotDesc
		{
			key = key,
			requireTags = list,
			excludeTags = list2
		};
		return this;
	}

	public ItemBuilder Stat(string key, float value, bool display = false)
	{
		if (string.IsNullOrWhiteSpace(key))
		{
			Debug.LogError("[Item Builder] Key of stat cannot be empty");
			return this;
		}
		stats[key] = new StatDesc
		{
			key = key,
			value = value,
			display = display
		};
		return this;
	}

	public ItemBuilder SetVariable(string key, float value, bool display = false)
	{
		variables.Set(key, value);
		variables.SetDisplay(key, display);
		return this;
	}

	public ItemBuilder SetVariable(string key, int value, bool display = false)
	{
		variables.Set(key, value);
		variables.SetDisplay(key, display);
		return this;
	}

	public ItemBuilder SetVariable(string key, bool value, bool display = false)
	{
		variables.Set(key, value);
		variables.SetDisplay(key, display);
		return this;
	}

	public ItemBuilder SetVariable(string key, string value, bool display = false)
	{
		variables.Set(key, value);
		variables.SetDisplay(key, display);
		return this;
	}

	public ItemBuilder SetConstant(string key, float value, bool display = false)
	{
		constants.Set(key, value);
		variables.SetDisplay(key, display);
		return this;
	}

	public ItemBuilder SetConstant(string key, int value, bool display = false)
	{
		constants.Set(key, value);
		variables.SetDisplay(key, display);
		return this;
	}

	public ItemBuilder SetConstant(string key, bool value, bool display = false)
	{
		constants.Set(key, value);
		variables.SetDisplay(key, display);
		return this;
	}

	public ItemBuilder SetConstant(string key, string value, bool display = false)
	{
		constants.Set(key, value);
		variables.SetDisplay(key, display);
		return this;
	}

	public ItemBuilder Modifier(ModifierDescription description)
	{
		modifiers.Add(description);
		return this;
	}
}
