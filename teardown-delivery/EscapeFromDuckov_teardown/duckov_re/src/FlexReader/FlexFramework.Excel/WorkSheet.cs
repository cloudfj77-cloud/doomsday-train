using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FlexFramework.Excel;

public sealed class WorkSheet : Table
{
	public string ID { get; private set; }

	public string Name { get; private set; }

	public ReadOnlyCollection<Range> Spans { get; private set; }

	public WorkSheet(string id, string name, IList<Row> rows)
		: base(rows)
	{
		ID = id;
		Name = name;
	}

	public WorkSheet(string id, string name, IList<Row> rows, IList<Range> spans)
		: base(rows)
	{
		ID = id;
		Name = name;
		Spans = new ReadOnlyCollection<Range>(spans);
	}

	public void Merge()
	{
		using (IEnumerator<Row> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				foreach (Cell item in enumerator.Current.Where((Cell c) => c.IsSpan))
				{
					foreach (Range span in Spans)
					{
						if (span.Contains(item.Address))
						{
							item.Value = base[span.From].Value;
							item.IsSpan = false;
							break;
						}
					}
				}
			}
		}
		Spans = new List<Range>().AsReadOnly();
	}

	public override Table DeepClone()
	{
		return new WorkSheet(ID, Name, base.Items.Select((Row row) => row.DeepClone()).ToList(), Spans.ToList());
	}

	public override Table ShallowClone()
	{
		return (Table)MemberwiseClone();
	}
}
