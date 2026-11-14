using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace FlexFramework.Excel;

public abstract class MapperBase<T> where T : MapperBase<T>
{
	protected readonly Mapping[] mappings;

	protected readonly Type type;

	public bool SafeMode { get; set; }

	protected MapperBase(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		this.type = type;
		MemberInfo[] members = GetMembers();
		mappings = members.Select((MemberInfo m) => new Mapping(m)).ToArray();
	}

	protected virtual MemberInfo[] GetMembers()
	{
		return FormatterServices.GetSerializableMembers(type);
	}

	protected virtual object CreateInstance()
	{
		return FormatterServices.GetUninitializedObject(type);
	}

	protected virtual void Assign(object obj, MemberInfo[] members, object[] data)
	{
		FormatterServices.PopulateObjectMembers(obj, members, data);
	}

	public virtual void Extract()
	{
		Mapping[] array = mappings;
		foreach (Mapping mapping in array)
		{
			if (!(Attribute.GetCustomAttribute(mapping.Member, typeof(ColumnAttribute)) is ColumnAttribute columnAttribute))
			{
				mapping.Column = 0;
				continue;
			}
			if (columnAttribute.Column < 1)
			{
				throw new ArgumentException("One-based column index must be greater than 0");
			}
			mapping.Column = columnAttribute.Column;
			mapping.Default = columnAttribute.Default;
			mapping.Fallback = columnAttribute.Fallback;
		}
	}

	protected IEnumerable<object> Cast(Row row)
	{
		foreach (Mapping item in mappings.Where((Mapping m) => m.Column > 0))
		{
			int num = item.Column - 1;
			if (num >= row.Count)
			{
				throw new InvalidOperationException($"Column '{item.Member.Name}' index '{num}' out of range '{row.Count}' on type '{type.FullName}'");
			}
			object obj = null;
			try
			{
				obj = row[num].Convert(item.Type);
			}
			catch (FormatException)
			{
				if (item.Fallback)
				{
					obj = item.Default;
				}
				else
				{
					if (!SafeMode)
					{
						throw new FormatException($"Input cell at {row[num].Address} was not in the correct format for type {item.Type}");
					}
					item.Failed = true;
				}
			}
			if (!item.Failed)
			{
				yield return obj;
			}
		}
	}

	public T Map(string member, int column)
	{
		Mapping mapping = Array.Find(mappings, (Mapping m) => m.Member.Name == member);
		if (mapping == null)
		{
			throw new InvalidOperationException($"Member '{member}' not found in type '{type.FullName}'");
		}
		if (mapping.Column > 0)
		{
			throw new InvalidOperationException($"Member '{member}' for type '{type.FullName}' already mapped to Column '{mapping.Column}'");
		}
		if (column < 1)
		{
			throw new ArgumentException("One-based column index must be greater than 0");
		}
		mapping.Column = column;
		return (T)this;
	}

	public T Map(string member, int column, object @default)
	{
		Mapping mapping = Array.Find(mappings, (Mapping m) => m.Member.Name == member);
		if (mapping == null)
		{
			throw new InvalidOperationException($"Member '{member}' not found in type '{type.FullName}'");
		}
		if (mapping.Column > 0)
		{
			throw new InvalidOperationException($"Member '{member}' for type '{type.FullName}' already mapped to Column '{mapping.Column}'");
		}
		if (column < 1)
		{
			throw new ArgumentException("One-based column index must be greater than 0");
		}
		mapping.Default = @default;
		mapping.Column = column;
		mapping.Fallback = true;
		return (T)this;
	}

	public T Map(string member, string column)
	{
		return Map(member, Address.ParseColumn(column));
	}

	public T Map(string member, string column, object @default)
	{
		return Map(member, Address.ParseColumn(column), @default);
	}

	public T Map(string expression)
	{
		if (!Regex.IsMatch(expression, "^(\\w+\\s*:(\\d+|[A-Z]+)\\s*)(,\\s*\\w+\\s*:(\\d+|[A-Z]+)\\s*)*$"))
		{
			throw new FormatException("Invalid mapping expression");
		}
		string[] array = expression.Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new string[1] { ":" }, StringSplitOptions.RemoveEmptyEntries);
			string text = array2[1].Trim();
			if (Regex.IsMatch(text, "^\\d+$"))
			{
				Map(array2[0].Trim(), int.Parse(text));
			}
			else
			{
				Map(array2[0].Trim(), text);
			}
		}
		return (T)this;
	}

	public T Map(IEnumerable<Cell> row)
	{
		foreach (Cell item in row)
		{
			Map(item.Text, item.Address.Column);
		}
		return (T)this;
	}

	public T Remove(string member)
	{
		(Array.Find(mappings, (Mapping m) => m.Member.Name == member) ?? throw new InvalidOperationException($"Member '{member}' not found in type '{type.FullName}'")).Column = 0;
		return (T)this;
	}

	public T Clear()
	{
		Array.ForEach(mappings, delegate(Mapping m)
		{
			m.Column = 0;
		});
		return (T)this;
	}

	public T Copy(T mapping)
	{
		Array.ForEach(mappings, delegate(Mapping m)
		{
			m.Column = 0;
		});
		Mapping[] array = mapping.mappings;
		foreach (Mapping source in array)
		{
			Mapping mapping2 = Array.Find(mappings, (Mapping m) => m.Member.Name == source.Member.Name && m.Member.DeclaringType == source.Member.DeclaringType);
			if (mapping2 != null)
			{
				mapping2.Column = source.Column;
				mapping2.Default = source.Default;
				mapping2.Fallback = source.Fallback;
			}
		}
		return (T)this;
	}

	protected object Instantiate(Row row)
	{
		object obj = CreateInstance();
		object[] data = Cast(row).ToArray();
		MemberInfo[] members = (from m in mappings
			where m.Column > 0 && !m.Failed
			select m.Member).ToArray();
		Array.ForEach(mappings, delegate(Mapping m)
		{
			m.Failed = false;
		});
		Assign(obj, members, data);
		return obj;
	}
}
