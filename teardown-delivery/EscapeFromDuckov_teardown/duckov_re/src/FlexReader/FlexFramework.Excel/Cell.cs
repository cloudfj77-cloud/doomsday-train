using System;

namespace FlexFramework.Excel;

public class Cell : ICloneable<Cell>
{
	public Address Address { get; set; }

	public object Value { get; set; }

	public bool IsSpan { get; set; }

	public virtual string Text => Convert.ToString(Value) ?? string.Empty;

	public string String
	{
		get
		{
			if (!IsString)
			{
				throw new InvalidCastException();
			}
			return (string)Value;
		}
	}

	public int Integer
	{
		get
		{
			if (!IsInteger)
			{
				throw new InvalidCastException();
			}
			return (int)Value;
		}
	}

	public bool Boolean
	{
		get
		{
			if (!IsBoolean)
			{
				throw new InvalidCastException();
			}
			return (bool)Value;
		}
	}

	public float Single
	{
		get
		{
			if (!IsSingle)
			{
				if (!IsInteger)
				{
					throw new InvalidCastException();
				}
				return (int)Value;
			}
			return (float)Value;
		}
	}

	public double Double
	{
		get
		{
			if (!IsDouble)
			{
				throw new InvalidCastException();
			}
			return (double)Value;
		}
	}

	public bool IsInteger => Value is int;

	public bool IsSingle => Value is float;

	public bool IsDouble => Value is double;

	public bool IsBoolean => Value is bool;

	public bool IsString => Value is string;

	public Cell()
	{
	}

	public Cell(Address address)
		: this()
	{
		Address = address;
	}

	public Cell(string value, Address address)
		: this(address)
	{
		Value = value;
	}

	public Cell(bool value, Address address)
		: this(address)
	{
		Value = value;
	}

	public Cell(int value, Address address)
		: this(address)
	{
		Value = value;
	}

	public Cell(long value, Address address)
		: this(address)
	{
		Value = value;
	}

	public Cell(float value, Address address)
		: this(address)
	{
		Value = value;
	}

	public Cell(double value, Address address)
		: this(address)
	{
		Value = value;
	}

	public override string ToString()
	{
		return Text;
	}

	public Cell DeepClone()
	{
		return (Cell)MemberwiseClone();
	}

	public Cell ShallowClone()
	{
		return (Cell)MemberwiseClone();
	}

	public static implicit operator string(Cell cell)
	{
		return cell.String;
	}

	public static implicit operator int(Cell cell)
	{
		return cell.Integer;
	}

	public static implicit operator bool(Cell cell)
	{
		return cell.Boolean;
	}

	public static implicit operator float(Cell cell)
	{
		return cell.Single;
	}

	public static implicit operator double(Cell cell)
	{
		return cell.Double;
	}
}
