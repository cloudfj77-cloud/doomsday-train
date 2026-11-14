using System;
using System.Collections.Generic;
using System.Linq;

namespace FlexFramework.Excel;

public static class Extensions
{
	public static object Convert(this Cell cell, Type type)
	{
		return ValueConverter.Convert(type, cell.Text);
	}

	public static T Convert<T>(this Cell cell)
	{
		return (T)cell.Convert(typeof(T));
	}

	public static T Convert<T>(this Row row, IGenerator<T> generator) where T : new()
	{
		return generator.Instantiate(row);
	}

	public static T Convert<T>(this Row row, string expression) where T : new()
	{
		return row.Convert(new Mapper<T>().Map(expression));
	}

	public static T Convert<T>(this Row row, IEnumerable<Cell> cols) where T : new()
	{
		return row.Convert(new Mapper<T>().Map(cols));
	}

	public static object Convert(this Row row, Type type, string expression)
	{
		return row.Convert(new Mapper(type).Map(expression));
	}

	public static object Convert(this Row row, Type type, IEnumerable<Cell> cols)
	{
		return row.Convert(new Mapper(type).Map(cols));
	}

	public static object Convert(this Row row, IGenerator generator)
	{
		return generator.Instantiate(row);
	}

	public static IEnumerable<T> Convert<T>(this Table table) where T : new()
	{
		TableMapper<T> tableMapper = new TableMapper<T>();
		tableMapper.Extract();
		return table.Convert(tableMapper);
	}

	public static IEnumerable<object> Convert(this Table table, Type type)
	{
		TableMapper tableMapper = new TableMapper(type);
		tableMapper.Extract();
		return table.Convert(tableMapper);
	}

	public static IEnumerable<T> Convert<T>(this Table table, ITableGenerator<T> generator) where T : new()
	{
		return generator.Instantiate(table);
	}

	public static IEnumerable<T> Convert<T>(this Table table, int row) where T : new()
	{
		if (row < 1)
		{
			throw new ArgumentException("One-based row index must be greater than 0");
		}
		return ((ITableGenerator<T>)new TableMapper<T>().Map(table[row - 1]).Exclude(row)).Instantiate(table);
	}

	public static IEnumerable<T> Convert<T>(this Table table, string expression, params int[] exclude) where T : new()
	{
		TableMapper<T> tableMapper = new TableMapper<T>().Map(expression);
		if (exclude != null && exclude.Length != 0)
		{
			tableMapper.Exclude(exclude);
		}
		return table.Convert(tableMapper);
	}

	public static IEnumerable<object> Convert(this Table table, Type type, string expression, params int[] exclude)
	{
		TableMapper tableMapper = new TableMapper(type).Map(expression);
		if (exclude != null && exclude.Length != 0)
		{
			tableMapper.Exclude(exclude);
		}
		return table.Convert(tableMapper);
	}

	public static IEnumerable<object> Convert(this Table table, ITableGenerator generator)
	{
		return generator.Instantiate(table);
	}

	public static IEnumerable<object> Convert(this Table table, Type type, int row)
	{
		if (row < 1)
		{
			throw new ArgumentException("One-based row index must be greater than 0");
		}
		return ((ITableGenerator)new TableMapper(type).Map(table[row - 1]).Exclude(row)).Instantiate(table);
	}

	public static string Dump(this Row row, char delimiter, char enclose)
	{
		return row.Cells.Select((Cell c) => c.Dump(delimiter, enclose)).Aggregate(string.Empty, (string a, string b) => (!string.IsNullOrEmpty(a)) ? (a + delimiter + b) : b);
	}

	public static string Dump(this Row row)
	{
		return row.Dump(Document.Delimiter, Document.Enclose);
	}

	public static string Dump(this Cell cell, char delimiter, char enclose)
	{
		string text = cell.Text;
		if (string.IsNullOrEmpty(text))
		{
			return string.Empty;
		}
		if (text.Contains(enclose) || text.Contains(delimiter))
		{
			return enclose + text.Replace(enclose.ToString(), enclose.ToString() + enclose) + enclose;
		}
		return text;
	}

	public static string Dump(this Cell cell)
	{
		return cell.Dump(Document.Delimiter, Document.Enclose);
	}

	public static string Dump(this Table table, char delimiter, char enclose)
	{
		return table.Select((Row r) => r.Dump(delimiter, enclose)).Aggregate(string.Empty, (string a, string b) => (!string.IsNullOrEmpty(a)) ? (a + Environment.NewLine + b) : b);
	}

	public static string Dump(this Table table)
	{
		return table.Dump(Document.Delimiter, Document.Enclose);
	}

	public static Cell Select(this Row row, string address)
	{
		if (!Address.IsValid(address))
		{
			throw new FormatException();
		}
		Address ad = new Address(address);
		return row.FirstOrDefault((Cell cell) => cell.Address == ad);
	}

	public static IEnumerable<Cell> SelectRange(this Row row, string range)
	{
		if (!Range.IsValid(range))
		{
			throw new FormatException();
		}
		return row[new Range(range)];
	}

	public static Cell Select(this Table table, string address)
	{
		if (!Address.IsValid(address))
		{
			throw new FormatException();
		}
		Address ad = new Address(address);
		return table.SelectMany((Row row) => row).FirstOrDefault((Cell cell) => cell.Address == ad);
	}

	public static IEnumerable<Cell> SelectRange(this Table table, string range)
	{
		if (!Range.IsValid(range))
		{
			throw new FormatException();
		}
		return table[new Range(range)];
	}

	public static Cell Select(this WorkBook book, string path)
	{
		string[] args = path.Split(new char[1] { '!' }, StringSplitOptions.RemoveEmptyEntries);
		if (args.Length != 2)
		{
			throw new FormatException();
		}
		return book.FirstOrDefault((WorkSheet s) => s.Name == args[0])?.Select(args[1]);
	}

	public static IEnumerable<Cell> SelectRange(this WorkBook book, string path)
	{
		string[] args = path.Split(new char[1] { '!' }, StringSplitOptions.RemoveEmptyEntries);
		if (args.Length != 2)
		{
			throw new FormatException();
		}
		return book.FirstOrDefault((WorkSheet s) => s.Name == args[0])?.SelectRange(args[1]);
	}

	public static void Recalculate(this Table table)
	{
		for (int i = 0; i < table.Count; i++)
		{
			for (int j = 0; j < table[i].Count; j++)
			{
				table[i][j].Address = new Address(j + 1, i + 1);
			}
		}
	}

	public static Table Expand(this Table table)
	{
		Table table2 = table.DeepClone();
		int num = table2.Max((Row row) => row.Count);
		for (int num2 = 0; num2 < table2.Count; num2++)
		{
			for (int num3 = num - table2[num2].Count; num3 > 0; num3--)
			{
				table2[num2].Cells.Add(new Cell());
			}
		}
		table2.Recalculate();
		return table2;
	}

	public static Table Collapse(this Table table)
	{
		Table table2 = table.DeepClone();
		for (int i = 0; i < table2.Count; i++)
		{
			for (int j = 0; j < table2[i].Count && string.IsNullOrEmpty(table2[i][j].Text); j++)
			{
				table2[i].Cells.RemoveAt(j--);
			}
			int num = table2[i].Count - 1;
			while (num >= 0 && string.IsNullOrEmpty(table2[i][num].Text))
			{
				table2[i].Cells.RemoveAt(num);
				num--;
			}
		}
		for (int num2 = table2.Count - 1; num2 >= 0; num2--)
		{
			if (table2[num2].Count == 0)
			{
				table2.Rows.RemoveAt(num2);
			}
		}
		table2.Recalculate();
		return table2;
	}

	public static Table Rotate(this Table table)
	{
		Table table2 = table.Expand();
		if (table2.Count == 0)
		{
			return table2;
		}
		int count = table2[0].Count;
		if (count == 0)
		{
			return table2;
		}
		int count2 = table2.Count;
		Table table3 = table.DeepClone();
		table3.Rows.Clear();
		for (int i = 0; i < count; i++)
		{
			Row row = new Row();
			for (int num = count2 - 1; num >= 0; num--)
			{
				row.Cells.Add(table2[num][i]);
			}
			table3.Rows.Add(row);
		}
		table3.Recalculate();
		return table3;
	}

	public static bool IsEmpty(this Row row)
	{
		if (row.Count != 0)
		{
			return row.All((Cell col) => string.IsNullOrEmpty(col.Text));
		}
		return true;
	}
}
