using System.Collections.Generic;

public interface GraphNodeWindow : GraphNodeStorage
{
	List<GameItem> parentItems { get; }

	GraphNodeStorage child { get; }

	int xPadding { get; }

	int yPadding { get; }
}
