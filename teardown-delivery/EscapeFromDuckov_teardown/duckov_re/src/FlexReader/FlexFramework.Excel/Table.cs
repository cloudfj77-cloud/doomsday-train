using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FlexFramework.Excel;

public abstract class Table : ReadOnlyCollection<Row>, ICloneable<Table>
{
	public IList<Row> Rows => base.Items;

	public Cell this[string address]
	{
		get
		{
			if (!Address.IsValid(address))
			{
				throw new FormatException();
			}
			return this[new Address(address)];
		}
	}

	public Cell this[Address address] => this.SelectMany((Row row) => row).First((Cell cell) => cell.Address == address);

	public IEnumerable<Cell> this[Range range] => from cell in this.SelectMany((Row row) => row)
		where range.Contains(cell.Address)
		select cell;

	public Table(IList<Row> list)
		: base(list)
	{
	}

	public abstract Table DeepClone();

	public abstract Table ShallowClone();
}
