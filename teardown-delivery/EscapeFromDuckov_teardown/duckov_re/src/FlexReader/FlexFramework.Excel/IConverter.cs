using System;

namespace FlexFramework.Excel;

public interface IConverter
{
	Type Type { get; }

	object Convert(string input);
}
