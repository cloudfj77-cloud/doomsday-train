using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FlexFramework.Excel;

public class Row : ReadOnlyCollection<Cell>, ICloneable<Row>
{
	public IList<Cell> Cells => base.Items;

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

	public Cell this[Address address] => this.First((Cell cell) => cell.Address == address);

	public IEnumerable<Cell> this[Range range] => this.Where((Cell cell) => range.Contains(cell.Address));

	public Row(IList<Cell> list)
		: base(list)
	{
	}

	public Row()
		: base((IList<Cell>)new List<Cell>())
	{
	}

	public Row DeepClone()
	{
		return new Row(base.Items.Select((Cell cell) => cell.DeepClone()).ToList());
	}

	public Row ShallowClone()
	{
		return (Row)MemberwiseClone();
	}
}
