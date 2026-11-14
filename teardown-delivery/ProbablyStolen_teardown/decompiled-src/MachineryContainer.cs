using System.Collections.Generic;

public interface MachineryContainer : PixelElement
{
	MachineryStorageOwner owner { get; set; }

	void CollectStorage(List<GraphNodeStorage> list);
}
