using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[Serializable]
public class PaperPageData
{
	public string name = "";

	public string template;

	public Dictionary<string, string> slots = new Dictionary<string, string>();

	public List<PaperElementData> elements = new List<PaperElementData>();

	[JsonExtensionData]
	public IDictionary<string, JToken> Extra;
}
