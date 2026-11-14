using System.Collections.Generic;

public interface GraphNodeStorage
{
	List<GraphNodeStorage> parents { get; }

	List<GraphNodeStorage> children { get; }

	void Validate();

	bool MayHaveValidInventorySlot(GameItem item);

	SlotMarker TryFindOneValidInventorySlot(GameItem item);

	bool AddParent(GraphNodeStorage node);

	bool RemoveParent(GraphNodeStorage node);
}
