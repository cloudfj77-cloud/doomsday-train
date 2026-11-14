using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class LocalizationKeyAttribute : Attribute
{
	public string fallbackFile;

	public LocalizationKeyAttribute(string fallbackFile = "Default")
	{
		this.fallbackFile = fallbackFile;
	}
}
