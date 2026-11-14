using System.Collections.Generic;

namespace FlexFramework.Excel;

public interface ITableGenerator
{
	IEnumerable<object> Instantiate(Table table);
}
public interface ITableGenerator<T>
{
	IEnumerable<T> Instantiate(Table table);
}
