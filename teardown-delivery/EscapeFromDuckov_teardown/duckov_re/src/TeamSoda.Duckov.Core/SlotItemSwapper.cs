using ItemStatsSystem;
using ItemStatsSystem.Items;
using UnityEngine;

public class SlotItemSwapper : InteractableBase
{
	[SerializeField]
	private SlotCollection targetSlotCollection;

	protected override void OnInteractStart(CharacterMainControl interactCharacter)
	{
		Swap(interactCharacter);
	}

	public void Swap(CharacterMainControl character)
	{
		if (!(character == null))
		{
			if (targetSlotCollection == null)
			{
				Debug.LogError("Target Slot Collection is not set up in SlotItemSwapper");
			}
			Swap(character.CharacterItem.Slots, targetSlotCollection);
		}
	}

	public static void Swap(SlotCollection slotsA, SlotCollection slotsB)
	{
		foreach (Slot item in slotsA)
		{
			if (item == null)
			{
				continue;
			}
			string key = item.Key;
			Slot slot = slotsB.GetSlot(key);
			if (slot == null)
			{
				continue;
			}
			Item content = item.Content;
			bool flag = content != null;
			Item content2 = slot.Content;
			bool flag2 = content2 != null;
			if ((!flag || slot.CanPlug(content)) && (!flag2 || item.CanPlug(content2)))
			{
				if (flag)
				{
					content.Detach();
				}
				if (flag2)
				{
					content2.Detach();
				}
				Item unpluggedItem;
				if (flag)
				{
					slot.Plug(content, out unpluggedItem);
				}
				if (flag2)
				{
					item.Plug(content2, out unpluggedItem);
				}
			}
		}
	}
}
