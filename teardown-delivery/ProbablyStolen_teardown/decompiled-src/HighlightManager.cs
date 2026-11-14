using System.Collections.Generic;

public class HighlightManager
{
	private List<TreeNodeRender> highlightedThisNodes = new List<TreeNodeRender>();

	public void Try(SlotMarker slot, RenderHandler.BackgroundColor color)
	{
		Clear();
		if (slot?.inventory != null && slot?.itemGridShape != null)
		{
			List<TreeNodeRender> list = slot.inventory.HighlightSlot(slot, color, doReplace: false);
			if (list != null)
			{
				highlightedThisNodes.AddRange(list);
			}
		}
	}

	public void Clear()
	{
		foreach (TreeNodeRender highlightedThisNode in highlightedThisNodes)
		{
			if (highlightedThisNode != null && highlightedThisNode.image != null)
			{
				highlightedThisNode.image.enabled = false;
			}
		}
		highlightedThisNodes.Clear();
	}
}
