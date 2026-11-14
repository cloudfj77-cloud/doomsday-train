using System.Collections.Generic;
using System.Linq;

namespace FlexFramework.Excel;

public sealed class ListConverter<T> : CustomConverter<List<T>>
{
	public override List<T> Convert(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return null;
		}
		return (from p in Split(input, '#')
			select ValueConverter.Convert<T>(p)).ToList();
	}
}
