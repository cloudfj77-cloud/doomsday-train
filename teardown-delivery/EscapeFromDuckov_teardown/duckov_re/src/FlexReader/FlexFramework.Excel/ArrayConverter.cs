using System.Linq;

namespace FlexFramework.Excel;

public sealed class ArrayConverter<T> : CustomConverter<T[]>
{
	public override T[] Convert(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return null;
		}
		return (from p in Split(input, '#')
			select ValueConverter.Convert<T>(p)).ToArray();
	}
}
