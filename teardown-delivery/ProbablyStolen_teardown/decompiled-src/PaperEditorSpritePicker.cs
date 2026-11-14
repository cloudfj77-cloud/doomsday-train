using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class PaperEditorSpritePicker
{
	public const string WINDOW_ID = "paper_sprite_picker";

	private const string ALL = "All";

	private const int COLUMNS = 3;

	private const float CELL = 150f;

	private const float LABEL_HEIGHT = 18f;

	private static string filter = "";

	private static string group = "All";

	private static Action<string> onPick;

	private static string modId;

	private static string title;

	private static Func<string, bool> groupFilter;

	private static bool includeItemAtlases;

	private const int MAX_SHOWN = 400;

	public static void Open(string forModId, string windowTitle, Action<string> pick, Func<string, bool> groups = null, bool includeItems = false)
	{
		if (!(CustomUIManager.Instance == null))
		{
			modId = forModId;
			title = windowTitle;
			onPick = pick;
			groupFilter = groups;
			includeItemAtlases = includeItems;
			if (groupFilter != null && group != "All" && !groupFilter(group))
			{
				group = "All";
			}
			Build();
		}
	}

	public static void Close()
	{
		CustomUIManager.Instance?.CloseWindow("paper_sprite_picker");
	}

	private static void Build()
	{
		CustomUIWindow window = CustomUIManager.Instance.GetWindow("paper_sprite_picker");
		Vector2? vector = ((window != null && window.IsAlive) ? new Vector2?(window.Rect.anchoredPosition) : null);
		CustomUIBuilder customUIBuilder = CustomUIManager.Instance.CreateWindow("paper_sprite_picker", title ?? "Pick a sprite", "rear").SetSize(520f, 640f).SetDraggable(draggable: true)
			.SetCloseOnEscape();
		List<(string, Sprite, string)> list = PaperAssets.ListAvailable(modId, includeItemAtlases);
		if (groupFilter != null)
		{
			list = list.Where<(string, Sprite, string)>(((string key, Sprite sprite, string group) a) => groupFilter(a.group)).ToList();
		}
		List<string> groups = new List<string> { "All" };
		groups.AddRange(list.Select<(string, Sprite, string), string>(((string key, Sprite sprite, string group) a) => a.group).Distinct());
		int initialIndex = Mathf.Max(0, groups.IndexOf(group));
		List<(string, Sprite, string)> list2 = (from a in list
			where (@group == "All" || a.@group == @group) && (string.IsNullOrEmpty(filter) || a.key.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
			orderby a.@group
			select a).ThenBy<(string, Sprite, string), string>(((string key, Sprite sprite, string group) a) => a.key, StringComparer.OrdinalIgnoreCase).ToList();
		customUIBuilder.BeginRow();
		customUIBuilder.AddInput("filter by name...", filter, delegate(string v)
		{
			filter = v;
			Build();
		});
		customUIBuilder.AddDropdown(groups, initialIndex, delegate(int i)
		{
			group = groups[i];
			Build();
		});
		customUIBuilder.End();
		int count = list2.Count;
		if (list2.Count > 400)
		{
			list2 = list2.Take(400).ToList();
		}
		customUIBuilder.AddLabel($"{count} of {list.Count} sprites. Click one to use it." + ((count > 400) ? $" Showing the first {400}: filter by name or pick a group." : ""));
		if (list.Count == 0)
		{
			customUIBuilder.AddLabel("No catalog loaded: run Build > Collect Paper UI Assets in the Unity editor.");
		}
		customUIBuilder.BeginScroll(540f);
		customUIBuilder.BeginGrid(3, 150f, 168f, 6f);
		foreach (var item3 in list2)
		{
			string item = item3.Item1;
			Sprite item2 = item3.Item2;
			string picked = item;
			customUIBuilder.BeginColumn(2f, TextAnchor.UpperCenter);
			customUIBuilder.AddImage(item2, 150f, 146f);
			FitSprite(customUIBuilder.Last, item2);
			customUIBuilder.AddButton(Shorten(item), delegate
			{
				Pick(picked);
			});
			customUIBuilder.End();
		}
		customUIBuilder.End();
		customUIBuilder.End();
		if (vector.HasValue)
		{
			customUIBuilder.SetPosition(vector.Value);
		}
		else
		{
			customUIBuilder.SetPosition(new Vector2(-420f, 0f));
		}
		customUIBuilder.Show();
	}

	private static void FitSprite(CustomUIElement element, Sprite sprite)
	{
		if (!(element?.image == null))
		{
			element.image.preserveAspect = true;
			element.image.type = Image.Type.Simple;
			element.image.color = ((sprite != null) ? Color.white : Color.clear);
			element.SetTooltip((sprite != null) ? $"{sprite.name}  {sprite.rect.width}x{sprite.rect.height}" : "");
		}
	}

	private static string Shorten(string key)
	{
		if (key.Length <= 22)
		{
			return key;
		}
		return key.Substring(0, 20) + "..";
	}

	private static void Pick(string key)
	{
		Action<string> action = onPick;
		Close();
		action?.Invoke(key);
	}
}
