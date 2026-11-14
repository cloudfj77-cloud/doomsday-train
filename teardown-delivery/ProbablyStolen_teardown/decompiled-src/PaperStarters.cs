using System.Collections.Generic;
using UnityEngine;

public static class PaperStarters
{
	public class Starter
	{
		public string id;

		public string title;

		public PaperBookData data;
	}

	public const string RESOURCE_FOLDER = "PaperUI/Starters";

	public const string BLANK = "blank";

	private static List<Starter> starters;

	private static readonly Dictionary<string, PaperTemplateData> builtInTemplates = new Dictionary<string, PaperTemplateData>();

	public static IReadOnlyList<Starter> All
	{
		get
		{
			Load();
			return starters;
		}
	}

	public static Starter Get(string id)
	{
		Load();
		foreach (Starter starter in starters)
		{
			if (starter.id == id)
			{
				return starter;
			}
		}
		return null;
	}

	public static PaperTemplateData GetBuiltInTemplate(string id)
	{
		Load();
		if (string.IsNullOrEmpty(id) || !builtInTemplates.TryGetValue(id, out var value))
		{
			return null;
		}
		return value;
	}

	public static IEnumerable<string> BuiltInTemplateIds()
	{
		Load();
		return builtInTemplates.Keys;
	}

	private static void Load()
	{
		if (starters != null)
		{
			return;
		}
		starters = new List<Starter>();
		TextAsset[] array = Resources.LoadAll<TextAsset>("PaperUI/Starters");
		foreach (TextAsset textAsset in array)
		{
			PaperBookData paperBookData = PaperJson.ParseBook(textAsset.text, textAsset.name + ".json");
			if (paperBookData == null)
			{
				continue;
			}
			if (string.IsNullOrEmpty(paperBookData.id))
			{
				paperBookData.id = textAsset.name;
			}
			starters.Add(new Starter
			{
				id = textAsset.name,
				title = (string.IsNullOrEmpty(paperBookData.title) ? textAsset.name : paperBookData.title),
				data = paperBookData
			});
			foreach (PaperTemplateData template in paperBookData.templates)
			{
				if (!string.IsNullOrEmpty(template.id) && !builtInTemplates.ContainsKey(template.id))
				{
					builtInTemplates[template.id] = template;
				}
			}
		}
		if (Get("blank") == null)
		{
			PaperBookData paperBookData2 = new PaperBookData
			{
				id = "blank",
				title = "Blank notebook",
				background = "notebook_manual"
			};
			paperBookData2.chrome = DefaultChrome(new Vector2(314f, 402f), 2f);
			paperBookData2.pages.Add(new PaperPageData
			{
				name = "Page 1"
			});
			starters.Add(new Starter
			{
				id = "blank",
				title = paperBookData2.title,
				data = paperBookData2
			});
		}
		starters.Sort((Starter a, Starter b) => (!(a.id == "blank")) ? ((b.id == "blank") ? 1 : string.CompareOrdinal(a.title, b.title)) : (-1));
	}

	public static PaperBookData NewBook(string starterId, string bookId, string title)
	{
		PaperBookData paperBookData = (Get(starterId) ?? Get("blank")).data.Clone();
		paperBookData.id = bookId;
		paperBookData.title = (string.IsNullOrEmpty(title) ? bookId : title);
		paperBookData.item = null;
		if (paperBookData.pages.Count == 0)
		{
			paperBookData.pages.Add(new PaperPageData
			{
				name = "Page 1",
				template = ((paperBookData.templates.Count > 0) ? paperBookData.templates[0].id : null)
			});
		}
		return paperBookData;
	}

	public static List<PaperElementData> DefaultChrome(Vector2 design, float scale, bool close = true, bool zoom = true, bool flip = true, bool pageIndicator = true)
	{
		if (scale <= 0f)
		{
			scale = 1f;
		}
		float num = 1f / scale;
		List<PaperElementData> list = new List<PaperElementData>();
		if (close)
		{
			list.Add(new PaperElementData
			{
				type = "button",
				action = "close",
				id = "close",
				x = Round(design.x - 34f * num),
				y = Round(5f * num),
				w = Round(28f * num),
				h = Round(30f * num)
			});
		}
		if (zoom)
		{
			list.Add(new PaperElementData
			{
				type = "button",
				action = "zoom",
				id = "zoom",
				x = Round(design.x - 81f * num),
				y = Round(7f * num),
				w = Round(28f * num),
				h = Round(30f * num)
			});
		}
		if (flip)
		{
			list.Add(new PaperElementData
			{
				type = "button",
				action = "flip_left",
				id = "flip_left",
				x = Round(design.x * 0.5f - 66f * num),
				y = Round(design.y - 44f * num),
				w = Round(32f * num),
				h = Round(32f * num)
			});
			list.Add(new PaperElementData
			{
				type = "button",
				action = "flip_right",
				id = "flip_right",
				x = Round(design.x * 0.5f + 38.5f * num),
				y = Round(design.y - 43f * num),
				w = Round(28f * num),
				h = Round(30f * num)
			});
		}
		if (pageIndicator)
		{
			list.Add(new PaperElementData
			{
				type = "text",
				action = "page_indicator",
				id = "page_indicator",
				style = "page_number",
				align = "center",
				valign = "middle",
				wrap = false,
				x = Round(design.x * 0.5f - 33f * num),
				y = Round(design.y - 55.5f * num),
				w = Round(66f * num),
				h = Round(50f * num)
			});
		}
		return list;
	}

	private static float Round(float value)
	{
		return Mathf.Round(value * 2f) * 0.5f;
	}
}
