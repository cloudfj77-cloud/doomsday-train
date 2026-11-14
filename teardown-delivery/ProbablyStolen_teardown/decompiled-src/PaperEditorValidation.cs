using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public static class PaperEditorValidation
{
	public class Issue
	{
		public bool error;

		public string message;

		public PaperElementData element;

		public int page = -1;

		public string templateId;

		public string Where
		{
			get
			{
				if (templateId == null)
				{
					if (page < 0)
					{
						if (element == null)
						{
							return "book";
						}
						return "chrome";
					}
					return $"page {page + 1}";
				}
				return "template " + templateId;
			}
		}
	}

	public static List<Issue> Run(PaperEditor editor)
	{
		List<Issue> list = new List<Issue>();
		if (!editor.IsEditing)
		{
			return list;
		}
		PaperBookData data = editor.Data;
		string modId = editor.Entry.modId;
		Vector2 designSize = editor.Ui.DesignSize;
		if (string.IsNullOrEmpty(data.background) || !PaperAssets.TryResolve(data.background, modId, out var _))
		{
			list.Add(new Issue
			{
				error = true,
				message = "Background '" + data.background + "' not found"
			});
		}
		if (data.pages.Count == 0)
		{
			list.Add(new Issue
			{
				error = true,
				message = "The book has no page"
			});
		}
		List<Locale> list2 = LocalizationSettings.AvailableLocales?.Locales;
		if (list2 != null)
		{
			foreach (string code2 in data.locales)
			{
				if (list2.All((Locale l) => l.Identifier.Code != code2))
				{
					list.Add(new Issue
					{
						error = false,
						message = "Locale '" + code2 + "' in locales is not one of the game's (" + string.Join(", ", list2.Select((Locale l) => l.Identifier.Code)) + ")"
					});
				}
			}
			foreach (PaperElementData item in data.AllElements())
			{
				if (item.translations == null)
				{
					continue;
				}
				foreach (string code in item.translations.Keys)
				{
					if (list2.All((Locale l) => l.Identifier.Code != code))
					{
						list.Add(new Issue
						{
							error = false,
							message = "'" + item.DisplayName + "': translation locale '" + code + "' is not one of the game's",
							element = item
						});
					}
				}
			}
		}
		if (data.item != null && string.IsNullOrEmpty(data.item.id))
		{
			list.Add(new Issue
			{
				error = true,
				message = "The item block needs an id"
			});
		}
		if (data.item != null && !string.IsNullOrEmpty(data.item.id) && PaperEditorFiles.SanitizeId(data.item.id) != data.item.id)
		{
			list.Add(new Issue
			{
				error = false,
				message = "Item id '" + data.item.id + "' should use letters, digits and underscores"
			});
		}
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (PaperElementData item2 in data.chrome.Concat(data.pages.SelectMany((PaperPageData p) => p.elements)))
		{
			if (!string.IsNullOrEmpty(item2.id))
			{
				dictionary[item2.id] = ((!dictionary.TryGetValue(item2.id, out var value)) ? 1 : (value + 1));
			}
		}
		foreach (KeyValuePair<string, int> item3 in dictionary.Where((KeyValuePair<string, int> p) => p.Value > 1))
		{
			list.Add(new Issue
			{
				error = true,
				message = $"Id '{item3.Key}' is used {item3.Value} times"
			});
		}
		CheckElements(list, data.chrome, editor, modId, designSize, -1, null, null);
		for (int i = 0; i < data.pages.Count; i++)
		{
			PaperPageData paperPageData = data.pages[i];
			PaperTemplateData template = data.GetTemplate(paperPageData.template);
			if (!string.IsNullOrEmpty(paperPageData.template) && template == null)
			{
				list.Add(new Issue
				{
					error = true,
					message = "Template '" + paperPageData.template + "' does not exist",
					page = i
				});
			}
			if (template != null)
			{
				foreach (string item4 in (from e in template.elements
					where !string.IsNullOrEmpty(e.slot)
					select e.slot).Distinct())
				{
					if (!paperPageData.slots.ContainsKey(item4))
					{
						list.Add(new Issue
						{
							error = false,
							message = "Slot '" + item4 + "' is not filled",
							page = i
						});
					}
				}
				foreach (string slot in paperPageData.slots.Keys)
				{
					if (template.elements.All((PaperElementData e) => e.slot != slot))
					{
						list.Add(new Issue
						{
							error = false,
							message = "Slot value '" + slot + "' matches no slot of template '" + template.id + "'",
							page = i
						});
					}
				}
			}
			CheckElements(list, paperPageData.elements, editor, modId, designSize, i, null, template);
		}
		foreach (PaperTemplateData template3 in data.templates)
		{
			if (string.IsNullOrEmpty(template3.id))
			{
				list.Add(new Issue
				{
					error = true,
					message = "A template has no id"
				});
			}
			CheckElements(list, template3.elements, editor, modId, designSize, -1, template3.id, null);
		}
		for (int j = 0; j < data.pages.Count; j++)
		{
			PaperPageData paperPageData2 = data.pages[j];
			PaperTemplateData template2 = data.GetTemplate(paperPageData2.template);
			List<PaperElementData> elements = paperPageData2.elements;
			IEnumerable<PaperElementData> second;
			if (template2 == null)
			{
				second = Enumerable.Empty<PaperElementData>();
			}
			else
			{
				IEnumerable<PaperElementData> elements2 = template2.elements;
				second = elements2;
			}
			List<PaperElementData> list3 = (from e in elements.Concat(second)
				where e.visible && string.IsNullOrEmpty(e.flow) && !e.IsFlow && !e.IsLine
				select e).ToList();
			for (int k = 0; k < list3.Count; k++)
			{
				for (int m = k + 1; m < list3.Count; m++)
				{
					Rect bounds = list3[k].Bounds;
					Rect bounds2 = list3[m].Bounds;
					if (bounds.Overlaps(bounds2) && list3[k].IsText == list3[m].IsText)
					{
						list.Add(new Issue
						{
							error = false,
							message = "'" + list3[k].DisplayName + "' overlaps '" + list3[m].DisplayName + "'",
							element = list3[k],
							page = j
						});
					}
				}
			}
		}
		return list;
	}

	private static void CheckElements(List<Issue> issues, List<PaperElementData> elements, PaperEditor editor, string modId, Vector2 design, int page, string templateId, PaperTemplateData pageTemplate)
	{
		HashSet<string> hashSet = new HashSet<string>(from e in elements
			where e.IsFlow && !string.IsNullOrEmpty(e.id)
			select e.id);
		if (pageTemplate != null)
		{
			foreach (PaperElementData item in pageTemplate.elements.Where((PaperElementData e) => e.IsFlow && !string.IsNullOrEmpty(e.id)))
			{
				hashSet.Add(item.id);
			}
		}
		foreach (PaperElementData element2 in elements)
		{
			PaperElementData element = element2;
			if (Array.IndexOf(PaperElementType.All, element.type) < 0)
			{
				issues.Add(Make(error: true, "'" + element.DisplayName + "': unknown type '" + element.type + "'"));
			}
			if (element.IsText || (element.IsButton && !element.IsIconButton))
			{
				if (!PaperTextStyle.Exists(element.style))
				{
					issues.Add(Make(error: true, "'" + element.DisplayName + "': unknown style '" + element.style + "'"));
				}
				if (!string.IsNullOrEmpty(element.loc) && !string.IsNullOrEmpty(modId))
				{
					LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == modId);
					if (loadedMod == null || !loadedMod.Localization.Values.Any((Dictionary<string, string> table) => table.ContainsKey(element.loc)))
					{
						issues.Add(Make(error: false, "'" + element.DisplayName + "': loc key '" + element.loc + "' is in none of the mod's Localization files"));
					}
				}
				if (element.IsText && string.IsNullOrEmpty(element.text) && string.IsNullOrEmpty(element.loc) && string.IsNullOrEmpty(element.slot) && element.action != "page_indicator")
				{
					issues.Add(Make(error: false, "'" + element.DisplayName + "': empty text" + (string.IsNullOrEmpty(element.id) ? "" : " (fine if set from code)")));
				}
			}
			Sprite sprite;
			if (element.IsImage || element.IsIconButton)
			{
				string text = element.sprite;
				if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(element.slot))
				{
					text = null;
				}
				if (!string.IsNullOrEmpty(text) && !PaperAssets.TryResolve(text, modId, out sprite))
				{
					issues.Add(Make(error: true, "'" + element.DisplayName + "': sprite '" + text + "' not found"));
				}
				if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(element.slot) && !element.IsChromeAction)
				{
					issues.Add(Make(error: false, "'" + element.DisplayName + "': no sprite"));
				}
			}
			if (element.IsImage && element.fit == "native" && element.scale <= 0f)
			{
				issues.Add(Make(error: true, "'" + element.DisplayName + "': scale must be above zero"));
			}
			if (element.IsLine)
			{
				if (Mathf.Abs(element.x - element.x2) < 0.01f && Mathf.Abs(element.y - element.y2) < 0.01f)
				{
					issues.Add(Make(error: true, "'" + element.DisplayName + "': zero length"));
				}
				if (Array.IndexOf(PaperLineArrow.All, element.arrow ?? "none") < 0)
				{
					issues.Add(Make(error: false, "'" + element.DisplayName + "': unknown arrow '" + element.arrow + "'"));
				}
			}
			if (!string.IsNullOrEmpty(element.color) && !ColorUtility.TryParseHtmlString(element.color.StartsWith("#") ? element.color : ("#" + element.color), out var color))
			{
				issues.Add(Make(error: false, "'" + element.DisplayName + "': colour '" + element.color + "' is not #rrggbb"));
			}
			if (!string.IsNullOrEmpty(element.fill) && !ColorUtility.TryParseHtmlString(element.fill.StartsWith("#") ? element.fill : ("#" + element.fill), out color))
			{
				issues.Add(Make(error: false, "'" + element.DisplayName + "': fill '" + element.fill + "' is not #rrggbb"));
			}
			if (element.IsButton && string.IsNullOrEmpty(element.action))
			{
				issues.Add(Make(error: false, "'" + element.DisplayName + "': button without an action name"));
			}
			if (element.IsButton && !element.IsChromeAction && !string.IsNullOrEmpty(element.action) && !element.IsIconButton && string.IsNullOrEmpty(element.text) && string.IsNullOrEmpty(element.loc) && string.IsNullOrEmpty(element.slot))
			{
				issues.Add(Make(error: false, "'" + element.DisplayName + "': text button without a label"));
			}
			if (!string.IsNullOrEmpty(element.flow) && !hashSet.Contains(element.flow))
			{
				issues.Add(Make(error: true, "'" + element.DisplayName + "': flow '" + element.flow + "' does not exist here"));
			}
			if (!string.IsNullOrEmpty(element.hoverSprite) && !PaperAssets.TryResolve(element.hoverSprite, modId, out sprite))
			{
				issues.Add(Make(error: true, "'" + element.DisplayName + "': hover sprite '" + element.hoverSprite + "' not found"));
			}
			if (Array.IndexOf(PaperImageFit.All, element.fit) < 0)
			{
				issues.Add(Make(error: false, "'" + element.DisplayName + "': unknown fit '" + element.fit + "'"));
			}
			if (string.IsNullOrEmpty(element.flow))
			{
				Rect bounds = element.Bounds;
				if (bounds.xMax < 0f || bounds.yMax < 0f || bounds.xMin > design.x || bounds.yMin > design.y)
				{
					issues.Add(Make(error: false, "'" + element.DisplayName + "': outside the page"));
				}
				else if (bounds.xMin < 0f || bounds.yMin < 0f || bounds.xMax > design.x || bounds.yMax > design.y)
				{
					issues.Add(Make(error: false, "'" + element.DisplayName + "': crosses the page edge"));
				}
				if (!element.IsLine && (element.w <= 0f || element.h <= 0f))
				{
					issues.Add(Make(error: true, "'" + element.DisplayName + "': zero size"));
				}
			}
			if (element.IsText && editor.Ui.IsOverflowing(element))
			{
				issues.Add(Make(error: false, "'" + element.DisplayName + "': text overflows its box (visible page)"));
			}
			Issue Make(bool error, string message)
			{
				return new Issue
				{
					error = error,
					message = message,
					element = element,
					page = page,
					templateId = templateId
				};
			}
		}
	}
}
