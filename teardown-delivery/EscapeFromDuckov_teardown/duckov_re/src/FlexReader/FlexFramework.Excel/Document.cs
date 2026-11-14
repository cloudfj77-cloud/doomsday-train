using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FlexFramework.Excel;

public sealed class Document : Table
{
	public static char Delimiter = ',';

	public static char Enclose = '"';

	public Document(IList<Row> list)
		: base(list)
	{
	}

	public Document()
		: this(new List<Row>())
	{
	}

	public static Document LoadAt(string path)
	{
		return Load(File.ReadAllText(path));
	}

	public static Document Load(string content)
	{
		using StringReader reader = new StringReader(content);
		return Load(reader);
	}

	public static Document Load(Stream stream)
	{
		return Load(new StreamReader(stream));
	}

	public static Document Load(byte[] buffer)
	{
		using MemoryStream stream = new MemoryStream(buffer);
		return Load(stream);
	}

	public static Document Load(TextReader reader)
	{
		Document document = new Document();
		Row row = new Row();
		int num = 1;
		int num2 = 1;
		int num3 = 0;
		StringBuilder stringBuilder = new StringBuilder();
		while (reader.Peek() != -1)
		{
			char c = (char)reader.Read();
			num3++;
			if (c == '\n' || c == '\r')
			{
				if (c == '\r' && reader.Peek() == 10)
				{
					reader.Read();
					num3++;
				}
				row.Cells.Add(new Cell(stringBuilder.ToString(), new Address(num, num2)));
				document.Items.Add(row);
				num = 1;
				num3 = 0;
				stringBuilder = new StringBuilder();
				num2++;
				row = new Row();
			}
			else if (c == Delimiter)
			{
				row.Cells.Add(new Cell(stringBuilder.ToString(), new Address(num, num2)));
				num++;
				stringBuilder = new StringBuilder();
			}
			else if (c == Enclose)
			{
				while (reader.Peek() != -1)
				{
					c = (char)reader.Read();
					num3++;
					if (c != Enclose)
					{
						stringBuilder.Append(c);
						continue;
					}
					if (reader.Peek() != Enclose)
					{
						break;
					}
					reader.Read();
					num3++;
					stringBuilder.Append(c);
				}
			}
			else
			{
				stringBuilder.Append(c);
			}
		}
		row.Cells.Add(new Cell(stringBuilder.ToString(), new Address(num, num2)));
		document.Items.Add(row);
		return document;
	}

	public static void Reset()
	{
		Delimiter = ',';
		Enclose = '"';
	}

	public override Table DeepClone()
	{
		return new Document(base.Items.Select((Row row) => row.DeepClone()).ToList());
	}

	public override Table ShallowClone()
	{
		return (Table)MemberwiseClone();
	}
}
