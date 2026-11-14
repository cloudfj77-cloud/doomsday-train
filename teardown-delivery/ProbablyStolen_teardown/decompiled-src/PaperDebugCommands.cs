using System.Collections.Generic;
using System.Linq;
using System.Text;
using QFSW.QC;

public static class PaperDebugCommands
{
	[Command("paper-list", "Lists every registered paper book with its mod, file and item", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string List()
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<PaperBookRegistry.Entry> list = PaperBookRegistry.Loaded.OrderBy((PaperBookRegistry.Entry e) => e.fullId).ToList();
		if (list.Count == 0)
		{
			return "No paper books registered (mods add them in Books/*.json, or make one with F9 > Modding > Paper Editor).";
		}
		foreach (PaperBookRegistry.Entry item in list)
		{
			stringBuilder.Append(item.fullId).Append("  pages=").Append(item.data.pages.Count);
			if (item.data.item != null)
			{
				stringBuilder.Append("  item=").Append(item.data.item.id);
			}
			if (item.IsFileBacked)
			{
				stringBuilder.Append("  file=").Append(item.path);
			}
			if (item.open != null && item.open.IsVisible)
			{
				stringBuilder.Append("  [open]");
			}
			stringBuilder.AppendLine();
		}
		return stringBuilder.ToString();
	}

	[Command("paper-open", "Opens a paper book by id (modId:bookId, or the bare id when unique)", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string Open(string id)
	{
		PaperUI paperUI = PaperBookRegistry.Open(id);
		if (paperUI == null)
		{
			return "No book '" + id + "', or no paper canvas in this scene.";
		}
		return $"Opened {paperUI.Id} ({paperUI.PageCount} page(s))";
	}

	[Command("paper-close", "Closes a paper book, or every open one without an id", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string Close(string id = null)
	{
		if (string.IsNullOrEmpty(id))
		{
			PaperBookRegistry.CloseAll();
		}
		else
		{
			PaperBookRegistry.Close(id);
		}
		return "ok";
	}

	[Command("paper-reload", "Re-reads every mod's Books folder; changed files are rebuilt in place", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string Reload()
	{
		int num = PaperBookRegistry.ReloadAll();
		return $"{num} book(s) changed.";
	}

	[Command("paper-edit", "Opens the paper editor on a book (or just the editor window without an id)", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string Edit(string id = null)
	{
		PaperEditor paperEditor = PaperEditor.Ensure();
		if (paperEditor == null)
		{
			return "No CustomUIManager in the scene.";
		}
		PaperEditorWindow.Open(paperEditor);
		if (string.IsNullOrEmpty(id))
		{
			return "Editor window opened.";
		}
		PaperBookRegistry.Entry entry = PaperBookRegistry.Resolve(id);
		if (entry == null || entry.data == null)
		{
			return "No book '" + id + "'.";
		}
		paperEditor.Edit(entry);
		return paperEditor.Status;
	}

	[Command("paper-item", "Spawns the document item of a book into the inventory", Platform.AllPlatforms, MonoTargetType.Single)]
	public static string Item(string id)
	{
		GameItem gameItem = PaperBookRegistry.SpawnItem(id);
		if (gameItem == null)
		{
			return "Book '" + id + "' has no item or does not exist.";
		}
		return "Spawned " + gameItem.name;
	}
}
