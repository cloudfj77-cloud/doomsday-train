using UnityEngine;

public class BaseWallPaperSlot : MonoBehaviour
{
	public BaseRoomTypes roomType;

	private void OnTransformChildrenChanged()
	{
		if (base.transform.childCount <= 0)
		{
			return;
		}
		Transform child = base.transform.GetChild(0);
		if (child != null)
		{
			BaseWallPaper component = child.GetComponent<BaseWallPaper>();
			if (component != null)
			{
				BaseWallMaterialSetter.SetWallPaperStatic(component, roomType);
			}
		}
	}
}
