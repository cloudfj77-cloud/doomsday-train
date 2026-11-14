using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[Serializable]
public class PaperTemplateData
{
	public string id = "";

	public List<PaperElementData> elements = new List<PaperElementData>();

	[JsonExtensionData]
	public IDictionary<string, JToken> Extra;
}
