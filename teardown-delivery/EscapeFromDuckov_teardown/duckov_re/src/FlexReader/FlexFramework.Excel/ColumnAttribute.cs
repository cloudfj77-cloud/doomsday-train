using System;

namespace FlexFramework.Excel;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
public sealed class ColumnAttribute : Attribute
{
	public int Column { get; private set; }

	public object Default { get; private set; }

	public bool Fallback { get; private set; }

	public ColumnAttribute(int column)
	{
		Column = column;
	}

	public ColumnAttribute(int column, object @default)
		: this(column)
	{
		Default = @default;
		Fallback = true;
	}

	public ColumnAttribute(string column)
		: this(Address.ParseColumn(column))
	{
	}

	public ColumnAttribute(string column, object @default)
		: this(Address.ParseColumn(column), @default)
	{
	}
}
