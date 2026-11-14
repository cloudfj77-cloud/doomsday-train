using System.Collections.Generic;
using Duckov.Utilities;

namespace Duckov.ItemBuilders;

public struct SlotDesc
{
	public string key;

	public List<Tag> requireTags;

	public List<Tag> excludeTags;
}
