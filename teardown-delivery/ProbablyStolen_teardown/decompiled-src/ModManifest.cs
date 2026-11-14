using System.Collections.Generic;

public class ModManifest
{
	public string ID = "";

	public string Name = "";

	public string Author = "";

	public string ModVersion = "";

	public string Description = "";

	public List<string> GameVersions = new List<string>();

	public List<string> Prerequisites = new List<string>();

	public List<string> Assemblies = new List<string>();

	public List<string> Capabilities = new List<string>();
}
