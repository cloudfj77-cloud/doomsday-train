using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[Serializable]
public class PaperItemData
{
	public string id = "";

	public string name = "";

	public string nameLoc;

	public string description = "";

	public string descriptionLoc;

	[DefaultValue("items_misc/manual1")]
	public string sprite = "items_misc/manual1";

	[DefaultValue(50L)]
	public long value = 50L;

	[DefaultValue(true)]
	public bool important = true;

	[JsonExtensionData]
	public IDictionary<string, JToken> Extra;
}
