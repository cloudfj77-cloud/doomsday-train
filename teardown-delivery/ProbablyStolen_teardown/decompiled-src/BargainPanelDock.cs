using UnityEngine;

public class BargainPanelDock : MonoBehaviour
{
	public Transform tablet;

	private void LateUpdate()
	{
		if (!(tablet == null) && !(tablet.parent != base.transform.parent))
		{
			int siblingIndex = tablet.GetSiblingIndex();
			int siblingIndex2 = base.transform.GetSiblingIndex();
			if (siblingIndex2 > siblingIndex)
			{
				base.transform.SetSiblingIndex(siblingIndex);
			}
			else if (siblingIndex2 < siblingIndex - 1)
			{
				base.transform.SetSiblingIndex(siblingIndex - 1);
			}
		}
	}
}
