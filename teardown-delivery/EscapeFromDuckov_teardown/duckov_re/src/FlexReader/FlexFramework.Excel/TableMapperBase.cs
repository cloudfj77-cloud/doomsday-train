using System;
using System.Collections.Generic;
using System.Linq;

namespace FlexFramework.Excel;

public abstract class TableMapperBase<T> : MapperBase<T> where T : TableMapperBase<T>
{
	protected readonly List<int> excludes;

	protected int index;

	protected TableMapperBase(Type type)
		: base(type)
	{
		excludes = new List<int>();
	}

	public override void Extract()
	{
		if (Attribute.GetCustomAttribute(type, typeof(TableAttribute)) is TableAttribute tableAttribute)
		{
			if (tableAttribute.Ignore != null && tableAttribute.Ignore.Length != 0)
			{
				Exclude(tableAttribute.Ignore);
			}
			base.SafeMode = tableAttribute.SafeMode;
		}
		base.Extract();
	}

	public T Exclude(params int[] rows)
	{
		if (rows == null || rows.Length == 0)
		{
			throw new ArgumentException("Rows must be specified");
		}
		for (int i = 0; i < rows.Length; i++)
		{
			if (rows[i] < 1)
			{
				throw new ArgumentException("One-based row index must be greater than 0");
			}
			rows[i]--;
		}
		excludes.AddRange(rows);
		return (T)this;
	}

	public T Include(params int[] rows)
	{
		if (rows == null || rows.Length == 0)
		{
			throw new ArgumentException("Rows must be specified");
		}
		for (int i = 0; i < rows.Length; i++)
		{
			if (rows[i] < 1)
			{
				throw new ArgumentException("One-based row index must be greater than 0");
			}
			rows[i]--;
		}
		excludes.RemoveAll((int value) => rows.Contains(value));
		return (T)this;
	}

	public T IncludeAll()
	{
		excludes.Clear();
		return (T)this;
	}
}
