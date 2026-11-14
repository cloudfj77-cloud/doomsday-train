using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FlexFramework.Excel;

public sealed class SharedStringCollection : ReadOnlyCollection<string>
{
	public SharedStringCollection(IList<string> list)
		: base(list)
	{
	}
}
