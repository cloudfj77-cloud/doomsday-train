using System;

namespace FlexFramework.Excel;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class TableAttribute : Attribute
{
	public int[] Ignore { get; private set; }

	public bool SafeMode { get; set; }

	public TableAttribute(params int[] ignore)
	{
		Ignore = ignore;
	}
}
