using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public static class PaperJson
{
	public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
	{
		Formatting = Formatting.Indented,
		DefaultValueHandling = DefaultValueHandling.Ignore,
		NullValueHandling = NullValueHandling.Ignore,
		MissingMemberHandling = MissingMemberHandling.Ignore
	};

	public static string Serialize(object value)
	{
		return JsonConvert.SerializeObject(value, Settings);
	}

	public static T Deserialize<T>(string json)
	{
		return JsonConvert.DeserializeObject<T>(json, Settings);
	}

	public static T Clone<T>(T value)
	{
		return Deserialize<T>(Serialize(value));
	}

	public static PaperBookData ParseBook(string json, string sourceName)
	{
		try
		{
			PaperBookData paperBookData = Deserialize<PaperBookData>(json);
			if (paperBookData == null)
			{
				Debug.LogError("[PaperUI] " + sourceName + ": empty book file");
				return null;
			}
			Normalize(paperBookData);
			return paperBookData;
		}
		catch (Exception ex)
		{
			Debug.LogError("[PaperUI] " + sourceName + ": cannot parse book: " + ex.Message);
			return null;
		}
	}

	public static void Normalize(PaperBookData data)
	{
		if (data.chrome == null)
		{
			data.chrome = new List<PaperElementData>();
		}
		if (data.templates == null)
		{
			data.templates = new List<PaperTemplateData>();
		}
		if (data.pages == null)
		{
			data.pages = new List<PaperPageData>();
		}
		if (data.locales == null)
		{
			data.locales = new List<string>();
		}
		foreach (PaperTemplateData template in data.templates)
		{
			if (template.elements == null)
			{
				template.elements = new List<PaperElementData>();
			}
		}
		foreach (PaperPageData page in data.pages)
		{
			if (page.elements == null)
			{
				page.elements = new List<PaperElementData>();
			}
			if (page.slots == null)
			{
				page.slots = new Dictionary<string, string>();
			}
		}
	}
}
