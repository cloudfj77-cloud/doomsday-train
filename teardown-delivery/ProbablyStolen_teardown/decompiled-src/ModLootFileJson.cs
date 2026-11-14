using System;

[Serializable]
public class ModLootFileJson
{
	public ModLootListJson[] tables;

	public ModLootListJson[] groups;

	public ModLootPatchJson[] patches;
}
