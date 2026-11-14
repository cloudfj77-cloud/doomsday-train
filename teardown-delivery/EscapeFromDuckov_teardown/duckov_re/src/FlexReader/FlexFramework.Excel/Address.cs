using System;
using System.Text.RegularExpressions;

namespace FlexFramework.Excel;

public struct Address : IEquatable<Address>
{
	public int Column { get; private set; }

	public int Row { get; private set; }

	public string ColumnName => ParseColumn(Column);

	bool IEquatable<Address>.Equals(Address other)
	{
		if (Column == other.Column)
		{
			return Row == other.Row;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj is Address)
		{
			return Equals((Address)obj);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (Column * 397) ^ Row;
	}

	public Address(string address)
	{
		Match match = Regex.Match(address, "^[A-Z]+");
		Match match2 = Regex.Match(address, "\\d+$");
		if (!match.Success || !match2.Success)
		{
			throw new FormatException("Invalid address: " + address);
		}
		Column = ParseColumn(match.Value);
		Row = int.Parse(match2.Value);
	}

	public Address(int column, int row)
	{
		Column = column;
		Row = row;
	}

	public override string ToString()
	{
		return $"{ColumnName}{Row}";
	}

	public static bool operator ==(Address address, Address other)
	{
		if (address.Row == other.Row)
		{
			return address.Column == other.Column;
		}
		return false;
	}

	public static bool operator !=(Address address, Address other)
	{
		if (address.Row == other.Row)
		{
			return address.Column != other.Column;
		}
		return true;
	}

	public static bool operator >=(Address address, Address other)
	{
		if (address.Row >= other.Row)
		{
			return address.Column >= other.Column;
		}
		return false;
	}

	public static bool operator <=(Address address, Address other)
	{
		if (address.Row <= other.Row)
		{
			return address.Column <= other.Column;
		}
		return false;
	}

	public static bool operator >(Address address, Address other)
	{
		if (address.Row > other.Row)
		{
			return address.Column > other.Column;
		}
		return false;
	}

	public static bool operator <(Address address, Address other)
	{
		if (address.Row < other.Row)
		{
			return address.Column < other.Column;
		}
		return false;
	}

	public static Range operator +(Address from, Address to)
	{
		return new Range(from, to);
	}

	public static Address operator >>(Address address, int column)
	{
		return new Address(address.Column + column, address.Row);
	}

	public static Address operator <<(Address address, int column)
	{
		return new Address(address.Column - column, address.Row);
	}

	public static Address operator +(Address address, int row)
	{
		return new Address(address.Column, address.Row + row);
	}

	public static Address operator -(Address address, int row)
	{
		return new Address(address.Column, address.Row - row);
	}

	public static string ParseColumn(int column)
	{
		if (column <= 0)
		{
			throw new ArgumentException("Column value must be greater than 0");
		}
		if (column <= 26)
		{
			return Convert.ToChar(column + 64).ToString();
		}
		int num = column / 26;
		int num2 = column % 26;
		if (num2 != 0)
		{
			return ParseColumn(num) + ParseColumn(num2);
		}
		num2 = 26;
		num--;
		return ParseColumn(num) + ParseColumn(num2);
	}

	public static int ParseColumn(string column)
	{
		if (!Regex.IsMatch(column, "^[A-Z]+$"))
		{
			throw new FormatException("Invalid address: " + column);
		}
		int[] array = new int[column.Length];
		for (int i = 0; i < column.Length; i++)
		{
			array[i] = Convert.ToInt32(column[i]) - 64;
		}
		int num = 1;
		int num2 = 0;
		for (int num3 = array.Length - 1; num3 >= 0; num3--)
		{
			num2 += array[num3] * num;
			num *= 26;
		}
		return num2;
	}

	public static bool IsValid(string address)
	{
		return Regex.IsMatch(address, "^[A-Z]+\\d+$");
	}
}
