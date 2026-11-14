using System;
using System.Text.RegularExpressions;

namespace FlexFramework.Excel;

public struct Range : IEquatable<Range>
{
	public Address From { get; private set; }

	public Address To { get; private set; }

	public bool Equals(Range other)
	{
		if (From.Equals(other.From))
		{
			return To.Equals(other.To);
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj is Range)
		{
			return Equals((Range)obj);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (From.GetHashCode() * 397) ^ To.GetHashCode();
	}

	public Range(Address from, Address to)
	{
		if (from >= to)
		{
			throw new ArgumentException("begin address is larger than or equal to end address");
		}
		From = from;
		To = to;
	}

	public Range(string range)
	{
		string[] array = range.Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length != 2)
		{
			throw new FormatException();
		}
		Address address = new Address(array[0]);
		Address address2 = new Address(array[1]);
		if (address >= address2)
		{
			throw new ArgumentException("begin address is larger than or equal to end address");
		}
		From = address;
		To = address2;
	}

	public bool Contains(Address address)
	{
		if (address >= From)
		{
			return address <= To;
		}
		return false;
	}

	public bool Contains(Range range)
	{
		if (range.From >= From)
		{
			return range.To <= To;
		}
		return false;
	}

	public override string ToString()
	{
		return $"{From}:{To}";
	}

	public static bool operator ==(Range range, Range other)
	{
		if (range.From == other.From)
		{
			return range.To == other.To;
		}
		return false;
	}

	public static bool operator !=(Range range, Range other)
	{
		if (!(range.From != other.From))
		{
			return range.To != other.To;
		}
		return true;
	}

	public static bool IsValid(string range)
	{
		return Regex.IsMatch(range, "^[A-Z]+\\d+:[A-Z]+\\d+$");
	}
}
