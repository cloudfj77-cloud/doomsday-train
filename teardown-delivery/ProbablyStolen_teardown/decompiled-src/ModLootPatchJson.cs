using System;

[Serializable]
public class ModLootPatchJson
{
	public string table;

	public string group;

	public bool clear;

	public ModLootEntryJson[] add;

	public string[] remove;

	public ModLootEntryJson[] setWeight;
}
