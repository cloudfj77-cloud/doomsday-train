using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[Serializable]
public class PaperBookData
{
	public string id = "";

	public string title = "";

	public string background = "";

	public float width;

	public float height;

	public float designWidth;

	public float designHeight;

	public float[] border;

	public float posX;

	public float posY;

	[DefaultValue(true)]
	public bool sounds = true;

	[DefaultValue(true)]
	public bool draggable = true;

	[DefaultValue(true)]
	public bool zoomable = true;

	public List<PaperElementData> chrome = new List<PaperElementData>();

	public List<PaperTemplateData> templates = new List<PaperTemplateData>();

	public List<PaperPageData> pages = new List<PaperPageData>();

	public PaperItemData item;

	public List<string> locales = new List<string>();

	[JsonExtensionData]
	public IDictionary<string, JToken> Extra;

	public PaperBookData Clone()
	{
		return PaperJson.Clone(this);
	}

	public PaperPageData GetPage(int index)
	{
		if (index < 0 || index >= pages.Count)
		{
			return null;
		}
		return pages[index];
	}

	public PaperTemplateData GetTemplate(string templateId)
	{
		if (string.IsNullOrEmpty(templateId))
		{
			return null;
		}
		foreach (PaperTemplateData template in templates)
		{
			if (template.id == templateId)
			{
				return template;
			}
		}
		return PaperStarters.GetBuiltInTemplate(templateId);
	}

	public IEnumerable<PaperElementData> AllElements()
	{
		foreach (PaperElementData item in chrome)
		{
			yield return item;
		}
		foreach (PaperTemplateData template in templates)
		{
			foreach (PaperElementData element in template.elements)
			{
				yield return element;
			}
		}
		foreach (PaperPageData page in pages)
		{
			foreach (PaperElementData element2 in page.elements)
			{
				yield return element2;
			}
		}
	}
}
