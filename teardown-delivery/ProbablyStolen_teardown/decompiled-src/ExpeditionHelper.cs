public class ExpeditionHelper
{
	public const string HIRELING_EQUIPMENT_OWNER = "HIRELING_EQUIPMENT_OWNER";

	public static void MintHirelingEquipment(string hirelingId)
	{
		GameItem gameItem = ItemSpawner.Spawn("expedition_box");
		gameItem.ModifyTag("HIRELING_EQUIPMENT_OWNER", delegate(TagState state)
		{
			state.SetString(hirelingId);
		});
		GraphUtils.TryAccept(EmporiumEntry.Instance.hirelingInv, gameItem);
	}

	public static GameItem GetHirelingEquipment(string hirelingId)
	{
		foreach (GameItem childItem in EmporiumEntry.Instance.hirelingInv.childItems)
		{
			if (childItem.GetTagReadonly("HIRELING_EQUIPMENT_OWNER").GetString().Equals(hirelingId))
			{
				return childItem;
			}
		}
		return null;
	}

	public static void SetHirelingEquipmentOnCounter(string hirelingId)
	{
		GameItem hirelingEquipment = GetHirelingEquipment(hirelingId);
		if (hirelingEquipment != null)
		{
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.frontInvWindow, hirelingEquipment);
		}
	}

	public static void RemoveHirelingEquipmentFromStore(string hirelingId)
	{
		GameItem gameItem = EmporiumEntry.Instance.GetAllItems().Find((GameItem i) => i.identifier.Equals("expedition_box"));
		if (gameItem != null)
		{
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.hirelingInv, gameItem);
			gameItem.contentWindow.Hide();
		}
	}

	public static GameItem GetExpeditionBackpack(string hirelingId)
	{
		GameItem hirelingEquipment = GetHirelingEquipment(hirelingId);
		if (hirelingEquipment != null)
		{
			GameItem childItem = (MachineHelper.GetBodyStorage(hirelingEquipment, 4) as GameSlotInventory).childItem;
			Debug.Log(childItem.name);
			return childItem;
		}
		return null;
	}
}
