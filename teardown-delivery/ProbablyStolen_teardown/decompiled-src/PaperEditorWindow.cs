using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public static class PaperEditorWindow
{
	public const string WINDOW_ID = "paper_editor_window";

	private const float WIDTH = 470f;

	private const float HEIGHT = 1000f;

	private const float SCROLL_HEIGHT = 860f;

	private static readonly string[] TABS = new string[7] { "Book", "Pages", "Elements", "Inspector", "Palette", "Tools", "Check" };

	private static PaperEditor editor;

	private static PaperEditor subscribedEditor;

	private static bool pendingRebuild;

	private static bool rebuilding;

	private static CustomUIWindow window;

	private static int newModIndex;

	private static string newId = "";

	private static string newTitle = "";

	private static int newStarterIndex;

	private static string newBackground = "";

	private static string newTemplateId = "";

	private static List<PaperEditorValidation.Issue> lastIssues;

	public static bool IsOpen
	{
		get
		{
			if (window != null && window.IsAlive)
			{
				return window.IsVisible;
			}
			return false;
		}
	}

	private static Func<string, bool> BackgroundGroups => (string g) => g == PaperAssetCatalog.Kind.Background.ToString() || g == "Mod paper";

	public static void Open(PaperEditor target)
	{
		editor = target;
		if (subscribedEditor != editor)
		{
			if (subscribedEditor != null)
			{
				subscribedEditor.Changed -= RequestRebuild;
			}
			editor.Changed += RequestRebuild;
			subscribedEditor = editor;
		}
		Build();
	}

	public static void Hide()
	{
		if (window != null && window.IsAlive)
		{
			window.Hide();
		}
	}

	public static void Close()
	{
		CustomUIManager.Instance?.CloseWindow("paper_editor_window");
		window = null;
	}

	public static void SetVisible(bool visible)
	{
		if (window != null && window.IsAlive)
		{
			if (visible)
			{
				window.Show();
			}
			else
			{
				window.Hide();
			}
		}
	}

	public static bool Contains(GameObject go)
	{
		if (window != null && window.IsAlive && go != null)
		{
			return go.transform.IsChildOf(window.root.transform);
		}
		return false;
	}

	public static bool PointerOverWindow()
	{
		if (window == null || !window.IsAlive || !window.IsVisible)
		{
			return false;
		}
		Canvas componentInParent = window.root.GetComponentInParent<Canvas>();
		Camera cam = ((componentInParent != null && componentInParent.renderMode != 0) ? componentInParent.worldCamera : null);
		if (RectTransformUtility.RectangleContainsScreenPoint(window.Rect, Input.mousePosition, cam))
		{
			return true;
		}
		CustomUIWindow customUIWindow = CustomUIManager.Instance?.GetWindow("paper_sprite_picker");
		if (customUIWindow != null && customUIWindow.IsAlive && customUIWindow.IsVisible)
		{
			return RectTransformUtility.RectangleContainsScreenPoint(customUIWindow.Rect, Input.mousePosition, cam);
		}
		return false;
	}

	public static void UpdateStatus(string text)
	{
		if (window != null && window.IsAlive)
		{
			window.Get("status")?.SetText(string.IsNullOrEmpty(text) ? " " : text);
		}
	}

	public static void ClearIssues()
	{
		lastIssues = null;
	}

	private static void RequestRebuild()
	{
		pendingRebuild = true;
	}

	public static void Tick()
	{
		if (pendingRebuild && !StoreUIManager.IsTextFieldFocused())
		{
			pendingRebuild = false;
			if (window != null && window.IsAlive)
			{
				Build();
			}
		}
	}

	private static void Build()
	{
		if (CustomUIManager.Instance == null || editor == null)
		{
			return;
		}
		pendingRebuild = false;
		CustomUIWindow customUIWindow = CustomUIManager.Instance.GetWindow("paper_editor_window");
		Vector2? vector = ((customUIWindow != null && customUIWindow.IsAlive) ? new Vector2?(customUIWindow.Rect.anchoredPosition) : null);
		bool flag = customUIWindow == null || !customUIWindow.IsAlive || customUIWindow.IsVisible;
		rebuilding = true;
		CustomUIBuilder customUIBuilder = CustomUIManager.Instance.CreateWindow("paper_editor_window", "Paper Editor", "rear").SetSize(470f, 1000f).SetDraggable(draggable: true)
			.OnClose(delegate
			{
				if (!rebuilding)
				{
					window = null;
					if (editor != null && editor.IsEditing)
					{
						editor.StopEditing();
					}
				}
			});
		rebuilding = false;
		customUIBuilder.AddLabel(string.IsNullOrEmpty(editor.Status) ? " " : editor.Status, "status");
		if (editor.IsEditing)
		{
			string arg = ((editor.EditingTemplate != null) ? ("template '" + editor.EditingTemplate + "'") : $"page {editor.Ui.CurrentPage + 1}/{Mathf.Max(1, editor.Data.pages.Count)}");
			customUIBuilder.AddLabel($"{editor.Entry.fullId}   {arg}   {editor.Selection.Count} selected" + (editor.Dirty ? "   (unsaved)" : ""));
		}
		customUIBuilder.BeginRow(2f);
		for (int i = 0; i < TABS.Length; i++)
		{
			int index = i;
			customUIBuilder.AddButton(TABS[i], delegate
			{
				editor.ActiveTab = index;
				Build();
			});
			if (i == editor.ActiveTab)
			{
				customUIBuilder.Last.SetInteractable(interactable: false);
			}
		}
		customUIBuilder.End();
		customUIBuilder.BeginScroll(860f);
		switch (editor.ActiveTab)
		{
		case 0:
			BuildBookTab(customUIBuilder);
			break;
		case 1:
			BuildPagesTab(customUIBuilder);
			break;
		case 2:
			BuildElementsTab(customUIBuilder);
			break;
		case 3:
			BuildInspectorTab(customUIBuilder);
			break;
		case 4:
			BuildPaletteTab(customUIBuilder);
			break;
		case 5:
			BuildToolsTab(customUIBuilder);
			break;
		default:
			BuildCheckTab(customUIBuilder);
			break;
		}
		customUIBuilder.End();
		if (vector.HasValue)
		{
			customUIBuilder.SetPosition(vector.Value);
		}
		else
		{
			customUIBuilder.SetPosition(new Vector2(715f, 0f));
		}
		window = (flag ? customUIBuilder.Show() : customUIBuilder.GetWindow());
	}

	private static string F(float value)
	{
		return value.ToString("0.##", CultureInfo.InvariantCulture);
	}

	private static bool TryParse(string text, out float value)
	{
		if (!float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
		{
			return float.TryParse(text, out value);
		}
		return true;
	}

	private static void Header(CustomUIBuilder b, string text)
	{
		b.AddSpace(6f);
		b.AddHeader(text);
	}

	private static void FloatField(CustomUIBuilder b, string label, float value, Action<float> set, float width = 70f)
	{
		b.BeginRow(4f, TextAnchor.MiddleLeft);
		b.AddLabel(label);
		b.Last.SetFixedWidth(90f);
		b.AddInput("", F(value), delegate(string text)
		{
			if (TryParse(text, out var value2) && Mathf.Abs(value2 - value) > 0.0001f)
			{
				set(value2);
			}
		});
		b.Last.SetFixedWidth(width);
		b.End();
	}

	private static void FloatPair(CustomUIBuilder b, string labelA, Func<float> getA, string labelB, Func<float> getB, Action<float, float> set)
	{
		b.BeginRow(4f, TextAnchor.MiddleLeft);
		b.AddLabel(labelA);
		b.Last.SetFixedWidth(30f);
		b.AddInput("", F(getA()), delegate(string text)
		{
			if (TryParse(text, out var value2) && Mathf.Abs(value2 - getA()) > 0.0001f)
			{
				set(value2, getB());
			}
		});
		b.Last.SetFixedWidth(70f);
		b.AddLabel(labelB);
		b.Last.SetFixedWidth(30f);
		b.AddInput("", F(getB()), delegate(string text)
		{
			if (TryParse(text, out var value) && Mathf.Abs(value - getB()) > 0.0001f)
			{
				set(getA(), value);
			}
		});
		b.Last.SetFixedWidth(70f);
		b.End();
	}

	private static void TextField(CustomUIBuilder b, string label, string value, Action<string> set, Action<string> typing = null, string placeholder = "")
	{
		b.BeginRow(4f, TextAnchor.MiddleLeft);
		b.AddLabel(label);
		b.Last.SetFixedWidth(90f);
		b.AddInput(placeholder, value ?? "", delegate(string text)
		{
			if (text != (value ?? ""))
			{
				set(text);
			}
		}, null, typing);
		b.Last.SetFlexible(1f, -1f);
		b.End();
	}

	private static void ToggleField(CustomUIBuilder b, string label, bool value, Action<bool> set)
	{
		b.AddToggle(label, value, delegate(bool v)
		{
			if (v != value)
			{
				set(v);
			}
		});
	}

	private static void DropdownField(CustomUIBuilder b, string label, List<string> options, int index, Action<int> set)
	{
		b.BeginRow(4f, TextAnchor.MiddleLeft);
		b.AddLabel(label);
		b.Last.SetFixedWidth(90f);
		b.AddDropdown(options, Mathf.Clamp(index, 0, Mathf.Max(0, options.Count - 1)), delegate(int i)
		{
			if (i != index)
			{
				set(i);
			}
		});
		b.Last.SetFlexible(1f, -1f);
		b.End();
	}

	private static void SpriteField(CustomUIBuilder b, string label, string value, Action<string> set, Func<string, bool> groups = null)
	{
		b.BeginRow(4f, TextAnchor.MiddleLeft);
		b.AddLabel(label);
		b.Last.SetFixedWidth(90f);
		b.AddInput("sprite key", value ?? "", delegate(string text)
		{
			if (text != (value ?? ""))
			{
				set(text);
			}
		});
		b.Last.SetFlexible(1f, -1f);
		b.AddButton("Pick", delegate
		{
			PaperEditorSpritePicker.Open(editor.Entry?.modId, "Pick " + label.ToLowerInvariant(), set, groups);
		});
		b.End();
	}

	private static void BuildBookTab(CustomUIBuilder b)
	{
		if (editor.IsEditing)
		{
			PaperBookData data = editor.Data;
			Header(b, "Current book");
			b.AddLabel(editor.Entry.IsFileBacked ? editor.Entry.path : "Not file-backed (registered from code); changes are not written anywhere.");
			b.BeginRow();
			b.AddButton("Save now", editor.Save);
			b.AddButton("Reload file", editor.ReloadFromDisk);
			b.AddButton("Open folder", delegate
			{
				PaperEditorFiles.OpenFolder(editor);
			});
			b.AddButton("Open file", delegate
			{
				PaperEditorFiles.OpenFile(editor);
			});
			b.End();
			b.BeginRow();
			b.AddButton("Copy JSON", delegate
			{
				PaperEditorFiles.CopyJson(editor);
			});
			b.AddButton("Copy C#", delegate
			{
				PaperEditorFiles.CopyCSharp(editor);
			});
			b.AddButton("Screenshot page", delegate
			{
				PaperEditorFiles.Screenshot(editor);
			});
			b.AddButton("Stop editing", editor.StopEditing);
			b.End();
			Header(b, "Settings");
			TextField(b, "Title", data.title, editor.SetTitle);
			SpriteField(b, "Background", data.background, editor.SetBackground, BackgroundGroups);
			b.AddLabel("Design space " + F(editor.Ui.DesignSize.x) + " x " + F(editor.Ui.DesignSize.y) + " (the coordinates), shown at " + F(editor.Ui.DisplaySize.x) + " x " + F(editor.Ui.DisplaySize.y) + ", scale " + F(editor.Ui.DisplayScale));
			FloatPair(b, "W", () => data.width, "H", () => data.height, editor.SetDisplaySize);
			b.AddLabel("Display size on the 1920x1080 canvas (0 = catalog or native size)").WithTooltip("The vanilla paper art is drawn at half size and shown at 2x.");
			FloatPair(b, "dW", () => data.designWidth, "dH", () => data.designHeight, editor.SetDesignSize);
			b.AddLabel("Design size override (0 = the background's pixel size)");
			Func<float[]> liveBorder = () => (data.border == null || data.border.Length != 4) ? new float[4] : data.border;
			b.BeginRow(4f, TextAnchor.MiddleLeft);
			b.AddLabel("Border L B R T");
			b.Last.SetFixedWidth(90f);
			for (int j = 0; j < 4; j++)
			{
				int index = j;
				b.AddInput("", F(liveBorder()[j]), delegate(string text)
				{
					if (TryParse(text, out var value))
					{
						float[] array = (float[])liveBorder().Clone();
						array[index] = value;
						editor.SetBorder(array[0], array[1], array[2], array[3]);
					}
				});
				b.Last.SetFixedWidth(48f);
			}
			b.End();
			FloatPair(b, "posX", () => data.posX, "posY", () => data.posY, editor.SetPanelPosition);
			b.AddLabel("Where the panel opens (center offset; alt-drag the book to move it)");
			b.BeginRow();
			b.AddToggle("Sounds", data.sounds, delegate(bool v)
			{
				editor.SetFlags(v, data.draggable, data.zoomable);
			});
			b.AddToggle("Draggable", data.draggable, delegate(bool v)
			{
				editor.SetFlags(data.sounds, v, data.zoomable);
			});
			b.AddToggle("Zoomable", data.zoomable, delegate(bool v)
			{
				editor.SetFlags(data.sounds, data.draggable, v);
			});
			b.End();
			Header(b, "Chrome");
			bool close = data.chrome.Any((PaperElementData e) => e.action == "close");
			bool zoom = data.chrome.Any((PaperElementData e) => e.action == "zoom");
			bool flip = data.chrome.Any((PaperElementData e) => e.action == "flip_left" || e.action == "flip_right");
			bool indicator = data.chrome.Any((PaperElementData e) => e.action == "page_indicator");
			b.BeginRow();
			b.AddToggle("Close", close, delegate(bool v)
			{
				editor.ResetChrome(v, zoom, flip, indicator);
			});
			b.AddToggle("Zoom", zoom, delegate(bool v)
			{
				editor.ResetChrome(close, v, flip, indicator);
			});
			b.AddToggle("Flip", flip, delegate(bool v)
			{
				editor.ResetChrome(close, zoom, v, indicator);
			});
			b.AddToggle("Pages", indicator, delegate(bool v)
			{
				editor.ResetChrome(close, zoom, flip, v);
			});
			b.End();
			b.AddButton("Reset chrome to the vanilla layout", delegate
			{
				editor.ResetChrome(close, zoom, flip, indicator);
			});
			b.AddLabel("Chrome elements are drawn on every page; select them on the canvas to move them.");
			Header(b, "Item");
			if (data.item == null)
			{
				b.AddButton("Add a document item that opens this book", delegate
				{
					editor.SetItem(new PaperItemData
					{
						id = PaperEditorFiles.SanitizeId(data.id) + "_guide",
						name = (string.IsNullOrEmpty(data.title) ? data.id : data.title),
						description = ""
					});
				});
			}
			else
			{
				PaperItemData item = data.item;
				TextField(b, "Item id", item.id, delegate(string v)
				{
					item.id = PaperEditorFiles.SanitizeId(v);
					editor.SetItem(item);
				});
				TextField(b, "Name", item.name, delegate(string v)
				{
					item.name = v;
					editor.SetItem(item);
				});
				TextField(b, "Name loc", item.nameLoc, delegate(string v)
				{
					item.nameLoc = (string.IsNullOrEmpty(v) ? null : v);
					editor.SetItem(item);
				}, null, "mod loc key (optional)");
				TextField(b, "Description", item.description, delegate(string v)
				{
					item.description = v;
					editor.SetItem(item);
				});
				TextField(b, "Desc loc", item.descriptionLoc, delegate(string v)
				{
					item.descriptionLoc = (string.IsNullOrEmpty(v) ? null : v);
					editor.SetItem(item);
				}, null, "mod loc key (optional)");
				TextField(b, "Sprite", item.sprite, delegate(string v)
				{
					item.sprite = v;
					editor.SetItem(item);
				}, null, "items_misc/manual1 or a Textures/Items name");
				FloatField(b, "Value", item.value, delegate(float v)
				{
					item.value = (long)v;
					editor.SetItem(item);
				});
				ToggleField(b, "Important (cannot be stolen)", item.important, delegate(bool v)
				{
					item.important = v;
					editor.SetItem(item);
				});
				b.BeginRow();
				b.AddButton("Spawn item", delegate
				{
					editor.Save();
					GameItem gameItem = PaperBookRegistry.SpawnItem(editor.Entry.fullId);
					editor.SetStatus((gameItem != null) ? ("Spawned " + gameItem.name) : "Could not spawn the item (is a save loaded?)");
				});
				b.AddButton("Remove item", delegate
				{
					editor.SetItem(null);
				});
				b.End();
			}
		}
		else
		{
			b.AddLabel("No book is being edited. Pick one below or create a new one.");
		}
		Header(b, "Books");
		List<PaperBookRegistry.Entry> list = (from e in PaperBookRegistry.Loaded
			orderby e.modId, e.bookId
			select e).ToList();
		if (list.Count == 0)
		{
			b.AddLabel("No books registered yet.");
		}
		string text2 = null;
		foreach (PaperBookRegistry.Entry entry in list)
		{
			if (entry.modId != text2)
			{
				text2 = entry.modId;
				LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == entry.modId);
				string text3 = ((loadedMod != null) ? (ModLoader.GetModLabel(loadedMod.Manifest) + ((loadedMod.Source == ModSource.Workshop) ? "  (Workshop, read-only)" : "")) : (entry.modId ?? "code"));
				b.AddLabel(text3);
			}
			PaperBookRegistry.Entry captured = entry;
			b.BeginRow();
			b.AddButton(entry.Title + "  (" + entry.bookId + ")", delegate
			{
				editor.Edit(captured);
			});
			b.Last.SetFlexible(1f, -1f);
			if (editor.IsEditing && editor.Entry == entry)
			{
				b.Last.SetInteractable(interactable: false);
			}
			b.AddButton("Open", delegate
			{
				PaperBookRegistry.Open(captured);
			});
			b.End();
		}
		Header(b, "New book");
		List<LoadedMod> mods = PaperEditorFiles.LocalMods();
		if (mods.Count == 0)
		{
			b.AddLabel("No local mod to write into. Create a mod folder under Mods/ with a manifest.xml and a DLL (see the modding guide), start the game, then come back.");
			return;
		}
		newModIndex = Mathf.Clamp(newModIndex, 0, mods.Count - 1);
		DropdownField(b, "Mod", mods.Select((LoadedMod m) => ModLoader.GetModLabel(m.Manifest)).ToList(), newModIndex, delegate(int i)
		{
			newModIndex = i;
		});
		TextField(b, "Book id", newId, delegate(string v)
		{
			newId = PaperEditorFiles.SanitizeId(v);
		}, null, "fuel_guide");
		TextField(b, "Title", newTitle, delegate(string v)
		{
			newTitle = v;
		}, null, "shown in the editor");
		List<PaperStarters.Starter> starters = PaperStarters.All.ToList();
		newStarterIndex = Mathf.Clamp(newStarterIndex, 0, starters.Count - 1);
		DropdownField(b, "Start from", starters.Select((PaperStarters.Starter s) => s.title).ToList(), newStarterIndex, delegate(int i)
		{
			newStarterIndex = i;
		});
		SpriteField(b, "Background", newBackground, delegate(string v)
		{
			newBackground = v;
			RequestRebuild();
		}, BackgroundGroups);
		b.AddLabel("Leave the background empty to keep the starter's. Your own art goes in Textures/Paper of the mod.");
		b.AddButton("Create and edit", delegate
		{
			if (editor.NewBook(mods[newModIndex].Manifest.ID, newId, newTitle, starters[newStarterIndex].id, newBackground) != null)
			{
				newId = "";
				newTitle = "";
				newBackground = "";
			}
		});
	}

	private static void BuildPagesTab(CustomUIBuilder b)
	{
		if (!editor.IsEditing)
		{
			b.AddLabel("Pick a book in the Book tab.");
			return;
		}
		PaperBookData data = editor.Data;
		if (editor.EditingTemplate != null)
		{
			Header(b, "Editing template '" + editor.EditingTemplate + "'");
			b.AddLabel("Elements with a slot name take their text or sprite from each page's slot values.");
			b.AddButton("Back to pages", delegate
			{
				editor.StopEditingTemplate();
			});
		}
		Header(b, "Pages");
		for (int j = 0; j < data.pages.Count; j++)
		{
			int index = j;
			PaperPageData paperPageData = data.pages[j];
			string text = string.Format("{0}. {1}", j + 1, string.IsNullOrEmpty(paperPageData.name) ? "(unnamed)" : paperPageData.name) + (string.IsNullOrEmpty(paperPageData.template) ? "" : ("  [" + paperPageData.template + "]"));
			b.AddButton(text, delegate
			{
				editor.GoToPage(index);
			});
			if (editor.EditingTemplate == null && index == editor.Ui.CurrentPage)
			{
				b.Last.SetInteractable(interactable: false);
			}
		}
		b.BeginRow();
		b.AddButton("Add", delegate
		{
			editor.AddPage();
		});
		b.AddButton("Duplicate", editor.DuplicatePage);
		b.AddButton("Delete", editor.DeletePage);
		b.AddButton("Up", delegate
		{
			editor.MovePage(-1);
		});
		b.AddButton("Down", delegate
		{
			editor.MovePage(1);
		});
		b.End();
		PaperPageData current = editor.CurrentPageData;
		if (current != null && editor.EditingTemplate == null)
		{
			Header(b, $"Page {editor.Ui.CurrentPage + 1}");
			TextField(b, "Name", current.name, editor.RenamePage);
			List<string> templates = new List<string> { "(none)" };
			templates.AddRange(editor.TemplateIds(includeBuiltIn: true));
			int index2 = ((!string.IsNullOrEmpty(current.template)) ? Mathf.Max(0, templates.IndexOf(current.template)) : 0);
			DropdownField(b, "Template", templates, index2, delegate(int i)
			{
				editor.SetPageTemplate((i == 0) ? null : templates[i]);
			});
			if (!string.IsNullOrEmpty(current.template))
			{
				b.BeginRow();
				b.AddButton("Edit template", delegate
				{
					editor.EditTemplate(current.template);
				});
				b.AddButton("Detach (copy elements into the page)", editor.DetachTemplate);
				b.End();
				List<string> list = editor.CurrentSlots();
				if (list.Count > 0)
				{
					b.AddLabel("Slots");
				}
				foreach (string slot in list)
				{
					string slotName = slot;
					current.slots.TryGetValue(slot, out var value);
					PaperTemplateData template = data.GetTemplate(current.template);
					if (template != null && template.elements.Any((PaperElementData e) => e.slot == slot && e.IsImage))
					{
						SpriteField(b, slot, value, delegate(string v)
						{
							editor.SetSlot(slotName, v);
						});
					}
					else
					{
						TextField(b, slot, value, delegate(string v)
						{
							editor.SetSlot(slotName, v);
						}, null, "text for this slot (\\n for a new line)");
					}
				}
			}
		}
		Header(b, "Templates");
		foreach (PaperTemplateData template2 in data.templates)
		{
			string id = template2.id;
			b.BeginRow();
			b.AddLabel($"{id}  ({template2.elements.Count} elements)");
			b.Last.SetFlexible(1f, -1f);
			b.AddButton("Edit", delegate
			{
				editor.EditTemplate(id);
			});
			b.AddButton("Delete", delegate
			{
				editor.DeleteTemplate(id);
			});
			b.End();
		}
		foreach (string item in from t in PaperStarters.BuiltInTemplateIds()
			where data.templates.All((PaperTemplateData x) => x.id != t)
			select t)
		{
			string builtIn = item;
			b.BeginRow();
			b.AddLabel(item + "  (built-in)");
			b.Last.SetFlexible(1f, -1f);
			b.AddButton("Copy and edit", delegate
			{
				editor.EditTemplate(builtIn);
			});
			b.End();
		}
		TextField(b, "New template", newTemplateId, delegate(string v)
		{
			newTemplateId = PaperEditorFiles.SanitizeId(v);
		}, null, "id");
		b.AddButton("Create from the selected page elements", delegate
		{
			editor.CreateTemplateFromSelection(newTemplateId);
			newTemplateId = "";
		});
		b.AddLabel("Mark elements of a template with a slot name (Inspector) so pages can fill them.");
	}

	private static void BuildElementsTab(CustomUIBuilder b)
	{
		if (!editor.IsEditing)
		{
			b.AddLabel("Pick a book in the Book tab.");
			return;
		}
		Header(b, (editor.EditingTemplate != null) ? "Template elements (bottom first)" : "Page elements (bottom first)");
		PaperPageData currentPageData = editor.CurrentPageData;
		PaperTemplateData paperTemplateData = ((currentPageData != null && editor.EditingTemplate == null) ? editor.Data.GetTemplate(currentPageData.template) : null);
		if (paperTemplateData != null)
		{
			b.AddLabel("From template '" + paperTemplateData.id + "' (edit the template to change them):");
			foreach (PaperElementData element in paperTemplateData.elements)
			{
				ElementRow(b, element, readOnly: true);
			}
		}
		List<PaperElementData> list = editor.EditableElements();
		if (list.Count == 0)
		{
			b.AddLabel("No own elements yet; add some from the Palette.");
		}
		foreach (PaperElementData item in list)
		{
			ElementRow(b, item, readOnly: false);
		}
		Header(b, "Chrome (every page)");
		foreach (PaperElementData item2 in editor.Data.chrome)
		{
			ElementRow(b, item2, readOnly: false);
		}
	}

	private static void ElementRow(CustomUIBuilder b, PaperElementData element, bool readOnly)
	{
		b.BeginRow(2f, TextAnchor.MiddleLeft);
		string text = (element.IsButton ? "btn" : (element.IsImage ? "img" : (element.IsFlow ? "flow" : (element.IsLine ? "line" : "txt")))) + "  " + element.DisplayName + (string.IsNullOrEmpty(element.flow) ? "" : ("  in " + element.flow));
		b.AddButton(text, delegate
		{
			editor.Select(element, Input.GetKey(KeyCode.LeftShift));
		});
		b.Last.SetFlexible(1f, -1f);
		if (editor.IsSelected(element))
		{
			b.Last.SetInteractable(interactable: false);
		}
		if (!readOnly)
		{
			b.AddToggle("", element.locked, delegate(bool v)
			{
				editor.Select(element);
				editor.SetLocked(v);
			}).WithTooltip("Locked");
			b.AddToggle("", element.visible, delegate(bool v)
			{
				editor.Select(element);
				editor.SetVisible(v);
			}).WithTooltip("Visible");
			b.AddButton("v", delegate
			{
				editor.Select(element);
				editor.ZOrder(-1);
			}).WithTooltip("Send backward");
			b.AddButton("^", delegate
			{
				editor.Select(element);
				editor.ZOrder(1);
			}).WithTooltip("Bring forward");
		}
		b.End();
	}

	private static void BuildInspectorTab(CustomUIBuilder b)
	{
		if (!editor.IsEditing)
		{
			b.AddLabel("Pick a book in the Book tab.");
			return;
		}
		if (editor.Selection.Count == 0)
		{
			b.AddLabel("Nothing selected. Click an element on the book, or pick one in the Elements tab.");
			return;
		}
		if (editor.Selection.Count > 1)
		{
			Rect rect = editor.SelectionBounds();
			b.AddLabel($"{editor.Selection.Count} elements selected: {F(rect.width)} x {F(rect.height)} at {F(rect.x)}, {F(rect.y)}");
			b.AddLabel("Alignment, distribution and size matching are in the Tools tab.");
			{
				foreach (PaperElementData item in editor.Selection)
				{
					b.AddLabel("  " + item.DisplayName);
				}
				return;
			}
		}
		PaperElementData e = editor.Selection[0];
		bool flag = editor.IsEditable(e) || editor.IsChrome(e);
		if (editor.IsTemplateElementOnPage(e))
		{
			b.AddLabel("Template element of '" + editor.CurrentPageData?.template + "'. Fields are read-only here.");
			b.AddButton("Edit the template", delegate
			{
				editor.EditTemplate(editor.CurrentPageData.template);
			});
			flag = false;
		}
		else if (editor.IsChrome(e))
		{
			b.AddLabel("Chrome element: shown on every page.");
		}
		if (e.locked)
		{
			b.AddLabel("Locked: unlock to move it on the canvas (fields still apply).");
		}
		Header(b, e.type + "  " + e.DisplayName);
		if (!flag && (!e.locked || !editor.IsOwned(e)))
		{
			return;
		}
		Action<string> commit = delegate(string label)
		{
			editor.Commit(label);
		};
		TextField(b, "Id", e.id, delegate(string v)
		{
			editor.RenameElement(e, v);
		}, null, "for Get(id) and the C# API");
		List<string> types = PaperElementType.All.ToList();
		DropdownField(b, "Type", types, Mathf.Max(0, types.IndexOf(e.type)), delegate(int i)
		{
			e.type = types[i];
			commit("type");
		});
		if (e.IsLine)
		{
			FloatPair(b, "X", () => e.x, "Y", () => e.y, delegate(float x, float y)
			{
				e.x = x;
				e.y = y;
				commit("position");
			});
			FloatPair(b, "X2", () => e.x2, "Y2", () => e.y2, delegate(float x, float y)
			{
				e.x2 = x;
				e.y2 = y;
				commit("end point");
			});
			b.AddLabel("Start and end points. Dragging moves both; type here to change the length or angle.");
		}
		else if (string.IsNullOrEmpty(e.flow))
		{
			FloatPair(b, "X", () => e.x, "Y", () => e.y, delegate(float x, float y)
			{
				e.x = x;
				e.y = y;
				commit("position");
			});
			FloatPair(b, "W", () => e.w, "H", () => e.h, delegate(float w, float h)
			{
				e.w = Mathf.Max(1f, w);
				e.h = Mathf.Max(1f, h);
				commit("size");
			});
		}
		else
		{
			b.AddLabel("Stacked inside flow '" + e.flow + "' (x and y follow the stack).");
			FloatPair(b, "W", () => e.w, "H", () => e.h, delegate(float w, float h)
			{
				e.w = Mathf.Max(0f, w);
				e.h = Mathf.Max(0f, h);
				commit("size");
			});
			b.AddLabel("Width 0 spans the flow; height is automatic for texts.");
		}
		b.BeginRow();
		b.AddToggle("Visible", e.visible, delegate(bool v)
		{
			e.visible = v;
			commit("visible");
		});
		b.AddToggle("Locked", e.locked, delegate(bool v)
		{
			e.locked = v;
			editor.Commit("locked", rebuild: false);
		});
		b.AddToggle("Catches pointer", e.raycast, delegate(bool v)
		{
			e.raycast = v;
			commit("raycast");
		});
		b.End();
		if (!e.IsFlow && !e.IsLine && !editor.IsChrome(e))
		{
			List<string> flows = new List<string> { "(not in a flow)" };
			PaperPageData currentPageData = editor.CurrentPageData;
			IEnumerable<PaperElementData> enumerable = editor.EditableElements();
			if (editor.EditingTemplate == null && currentPageData != null)
			{
				PaperTemplateData template = editor.Data.GetTemplate(currentPageData.template);
				if (template != null)
				{
					enumerable = enumerable.Concat(template.elements);
				}
			}
			flows.AddRange(from f in enumerable
				where f.IsFlow && !string.IsNullOrEmpty(f.id)
				select f.id);
			int index = ((!string.IsNullOrEmpty(e.flow)) ? Mathf.Max(0, flows.IndexOf(e.flow)) : 0);
			DropdownField(b, "Flow", flows, index, delegate(int i)
			{
				editor.Select(e);
				editor.SetFlow((i == 0) ? null : flows[i]);
			});
		}
		if (editor.EditingTemplate != null || !editor.IsChrome(e))
		{
			TextField(b, "Slot", e.slot, delegate(string v)
			{
				e.slot = (string.IsNullOrEmpty(v) ? null : v);
				commit("slot");
			}, null, "template slot name (optional)");
		}
		if (e.IsText || (e.IsButton && !e.IsIconButton))
		{
			Header(b, e.IsButton ? "Label" : "Text");
			if (e.action == "page_indicator")
			{
				b.AddLabel("Page indicator: the text is set by the book (\"1/3\").");
			}
			string originalText = e.text ?? "";
			TextField(b, "Text", e.text, delegate(string v)
			{
				e.text = v.Replace("\\n", "\n");
				if (e.text != originalText)
				{
					commit("text");
				}
			}, delegate(string v)
			{
				editor.PreviewText(e, v.Replace("\\n", "\n"));
			}, "literal text (\\n for a new line)");
			TextField(b, "Loc key", e.loc, delegate(string v)
			{
				e.loc = (string.IsNullOrEmpty(v) ? null : v);
				commit("loc");
			}, null, "key in the mod's Localization csv (wins over Text)");
			if (e.translations != null && e.translations.Count > 0)
			{
				b.AddLabel("Translations (locale code: text, shown when that language is selected; add codes in the JSON)");
				foreach (string item2 in e.translations.Keys.ToList())
				{
					string captured = item2;
					TextField(b, captured, e.translations[captured], delegate(string v)
					{
						e.translations[captured] = v.Replace("\\n", "\n");
						commit("translation " + captured);
					});
				}
			}
			if (e.IsText)
			{
				TextField(b, "Preview", e.preview, delegate(string v)
				{
					e.preview = (string.IsNullOrEmpty(v) ? null : v);
					commit("preview");
				}, null, "editor-only sample for texts set from code");
			}
			List<string> styles = PaperTextStyle.Ids();
			DropdownField(b, "Style", styles, Mathf.Max(0, styles.IndexOf(e.style)), delegate(int i)
			{
				e.style = styles[i];
				commit("style");
			});
			b.AddLabel(PaperTextStyle.Get(e.style).description + ", default size " + F(PaperTextStyle.Get(e.style).size));
			FloatField(b, "Font size", e.fontSize, delegate(float v)
			{
				e.fontSize = Mathf.Max(0f, v);
				commit("font size");
			});
			b.AddLabel("0 keeps the style's size (design units; shown at the display scale).");
			List<string> aligns2 = new List<string> { "left", "center", "right" };
			List<string> valigns = new List<string> { "top", "middle", "bottom" };
			DropdownField(b, "Align", aligns2, Mathf.Max(0, aligns2.IndexOf(e.align)), delegate(int i)
			{
				e.align = aligns2[i];
				commit("align");
			});
			DropdownField(b, "Vertical", valigns, Mathf.Max(0, valigns.IndexOf(e.valign)), delegate(int i)
			{
				e.valign = valigns[i];
				commit("valign");
			});
			TextField(b, "Colour", e.color, delegate(string v)
			{
				e.color = (string.IsNullOrEmpty(v) ? null : v);
				commit("colour");
			}, null, "#rrggbb (empty = style colour)");
			b.BeginRow();
			b.AddToggle("Wrap", e.wrap, delegate(bool v)
			{
				e.wrap = v;
				commit("wrap");
			});
			if (e.IsText)
			{
				b.AddToggle("Auto height", e.autoHeight, delegate(bool v)
				{
					e.autoHeight = v;
					commit("auto height");
				});
			}
			b.End();
			if (e.IsText && editor.Ui.IsOverflowing(e))
			{
				b.AddWarning("The text overflows its box.");
			}
		}
		if (e.IsImage || e.IsButton)
		{
			Header(b, e.IsButton ? "Button" : "Image");
			SpriteField(b, "Sprite", e.sprite, delegate(string v)
			{
				e.sprite = (string.IsNullOrEmpty(v) ? null : v);
				commit("sprite");
			});
			if (e.IsButton)
			{
				b.AddLabel(e.IsChromeAction ? "A chrome action: an empty sprite uses the vanilla button art." : "Empty sprite = text button (blue plate with the label).");
				SpriteField(b, "Hover sprite", e.hoverSprite, delegate(string v)
				{
					e.hoverSprite = (string.IsNullOrEmpty(v) ? null : v);
					commit("hover sprite");
				});
				TextField(b, "Action", e.action, delegate(string v)
				{
					e.action = (string.IsNullOrEmpty(v) ? null : v);
					commit("action");
				}, null, "name bound with ui.OnAction(...)");
				b.AddLabel("Reserved actions: close, zoom, flip_left, flip_right (buttons), page_indicator (text).");
			}
			else
			{
				List<string> fits = PaperImageFit.All.ToList();
				DropdownField(b, "Fit", fits, Mathf.Max(0, fits.IndexOf(e.fit)), delegate(int i)
				{
					e.fit = fits[i];
					commit("fit");
				});
				FloatField(b, "Scale", e.scale, delegate(float v)
				{
					e.scale = Mathf.Max(0.01f, v);
					commit("scale");
				});
				b.AddLabel("Native fit only: the pixel size times the scale (whole numbers keep pixel art crisp).");
				TextField(b, "Colour", e.color, delegate(string v)
				{
					e.color = (string.IsNullOrEmpty(v) ? null : v);
					commit("colour");
				}, null, "#rrggbb tint, or the silhouette fill");
				ToggleField(b, "Silhouette (flat shadow in the colour)", e.silhouette, delegate(bool v)
				{
					e.silhouette = v;
					commit("silhouette");
				});
				TextField(b, "Fill", e.fill, delegate(string v)
				{
					e.fill = (string.IsNullOrEmpty(v) ? null : v);
					commit("fill");
				}, null, "#rrggbb inside the outline (scenario icons)");
			}
		}
		if (e.IsLine)
		{
			Header(b, "Line");
			FloatField(b, "Thickness", e.thickness, delegate(float v)
			{
				e.thickness = Mathf.Max(0.5f, v);
				commit("thickness");
			});
			List<string> arrows = PaperLineArrow.All.ToList();
			DropdownField(b, "Arrow", arrows, Mathf.Max(0, arrows.IndexOf(e.arrow ?? "none")), delegate(int i)
			{
				e.arrow = arrows[i];
				commit("arrow");
			});
			FloatField(b, "Head size", e.arrowSize, delegate(float v)
			{
				e.arrowSize = Mathf.Max(0f, v);
				commit("head size");
			});
			TextField(b, "Colour", e.color, delegate(string v)
			{
				e.color = (string.IsNullOrEmpty(v) ? null : v);
				commit("colour");
			}, null, "#rrggbb (empty = ink)");
		}
		if (e.IsFlow)
		{
			Header(b, "Flow region");
			FloatField(b, "Spacing", e.spacing, delegate(float v)
			{
				e.spacing = Mathf.Max(0f, v);
				commit("spacing");
			});
			List<string> aligns = new List<string> { "left", "center", "right" };
			DropdownField(b, "Align", aligns, Mathf.Max(0, aligns.IndexOf(e.align)), delegate(int i)
			{
				e.align = aligns[i];
				commit("align");
			});
			b.AddLabel("Children: set their Flow field to this element's id (Inspector) or add them from the Palette while this flow is selected.");
		}
		Header(b, "Actions");
		b.BeginRow();
		b.AddButton("Duplicate", editor.Duplicate);
		b.AddButton("Delete", editor.DeleteSelection);
		b.AddButton("To front", delegate
		{
			editor.ZOrder(2);
		});
		b.AddButton("To back", delegate
		{
			editor.ZOrder(-2);
		});
		b.End();
	}

	private static void BuildPaletteTab(CustomUIBuilder b)
	{
		if (!editor.IsEditing)
		{
			b.AddLabel("Pick a book in the Book tab.");
			return;
		}
		PaperElementData selectedFlow = ((editor.Selection.Count == 1 && editor.Selection[0].IsFlow) ? editor.Selection[0] : null);
		if (selectedFlow != null)
		{
			b.AddLabel("New texts and images go into flow '" + selectedFlow.id + "'.");
		}
		Header(b, "Text");
		foreach (PaperTextStyle item3 in PaperTextStyle.All)
		{
			PaperTextStyle captured2 = item3;
			b.AddButton(item3.id ?? "", delegate
			{
				AddText(captured2, selectedFlow);
			}).WithTooltip(item3.description);
		}
		Header(b, "Image");
		b.AddButton("Image from a sprite...", delegate
		{
			PaperEditorSpritePicker.Open(editor.Entry.modId, "Pick an image", delegate(string key)
			{
				Sprite sprite4 = PaperAssets.Resolve(key, editor.Entry.modId, quiet: true);
				Vector2 vector4 = ((sprite4 != null) ? sprite4.rect.size : new Vector2(60f, 40f));
				PaperElementData paperElementData2 = new PaperElementData
				{
					type = "image",
					sprite = key,
					w = vector4.x,
					h = vector4.y,
					fit = "stretch"
				};
				if (selectedFlow != null)
				{
					paperElementData2.flow = selectedFlow.id;
				}
				editor.AddElement(paperElementData2, toChrome: false, selectedFlow == null);
			});
		});
		b.AddButton("Empty image box (fill the sprite later)", delegate
		{
			editor.AddElement(new PaperElementData
			{
				type = "image",
				w = 80f,
				h = 50f
			});
		});
		b.AddButton("Item sprite from the game atlases...", delegate
		{
			PaperEditorSpritePicker.Open(editor.Entry.modId, "Pick an item sprite", delegate(string key)
			{
				Sprite sprite3 = PaperAssets.Resolve(key, editor.Entry.modId, quiet: true);
				Vector2 vector3 = ((sprite3 != null) ? sprite3.rect.size : new Vector2(32f, 32f));
				PaperElementData paperElementData = new PaperElementData
				{
					type = "image",
					sprite = key,
					w = vector3.x,
					h = vector3.y,
					fit = "native"
				};
				if (selectedFlow != null)
				{
					paperElementData.flow = selectedFlow.id;
				}
				editor.AddElement(paperElementData, toChrome: false, selectedFlow == null);
			}, (string g) => g.StartsWith("Items "), includeItems: true);
		});
		b.AddLabel("Item sprites are keyed atlas/name (items_aquaculture/tilapia_large1); set Scale and Silhouette in the Inspector.");
		Header(b, "Line");
		b.BeginRow();
		b.AddButton("Line", delegate
		{
			editor.AddElement(new PaperElementData
			{
				type = "line",
				x = 0f,
				y = 0f,
				x2 = 120f,
				y2 = 0f,
				thickness = 2f
			});
		});
		b.AddButton("Arrow", delegate
		{
			editor.AddElement(new PaperElementData
			{
				type = "line",
				x = 0f,
				y = 0f,
				x2 = 120f,
				y2 = 0f,
				thickness = 2f,
				arrow = "end"
			});
		});
		b.End();
		Header(b, "Button");
		b.AddButton("Text button", delegate
		{
			editor.AddElement(new PaperElementData
			{
				type = "button",
				text = "Button",
				style = "button",
				action = "my_action",
				w = 65f,
				h = 17f
			});
		});
		b.AddButton("Icon button from a sprite...", delegate
		{
			PaperEditorSpritePicker.Open(editor.Entry.modId, "Pick a button sprite", delegate(string key)
			{
				Sprite sprite2 = PaperAssets.Resolve(key, editor.Entry.modId, quiet: true);
				Vector2 vector2 = ((sprite2 != null) ? sprite2.rect.size : new Vector2(14f, 15f));
				editor.AddElement(new PaperElementData
				{
					type = "button",
					sprite = key,
					action = "my_action",
					w = vector2.x,
					h = vector2.y
				});
			}, (string g) => g == PaperAssetCatalog.Kind.Button.ToString() || g == PaperAssetCatalog.Kind.Illustration.ToString() || g.StartsWith("Mod"));
		});
		Header(b, "Layout");
		b.AddButton("Flow region (stacks its children)", delegate
		{
			editor.AddElement(new PaperElementData
			{
				type = "flow",
				id = editor.UniqueId("flow"),
				w = 200f,
				h = 150f,
				spacing = 4f
			});
		});
		Header(b, "Chrome");
		b.AddLabel("Added to every page.");
		b.BeginRow();
		(string, string)[] array = new(string, string)[4]
		{
			("close", "Close"),
			("zoom", "Zoom"),
			("flip_left", "Flip left"),
			("flip_right", "Flip right")
		};
		for (int i = 0; i < array.Length; i++)
		{
			(string, string) tuple = array[i];
			string item = tuple.Item1;
			string item2 = tuple.Item2;
			string captured = item;
			b.AddButton(item2, delegate
			{
				editor.AddElement(new PaperElementData
				{
					type = "button",
					action = captured,
					id = captured,
					w = 14f,
					h = 15f
				}, toChrome: true);
			});
		}
		b.End();
		b.AddButton("Page indicator", delegate
		{
			editor.AddElement(new PaperElementData
			{
				type = "text",
				action = "page_indicator",
				id = "page_indicator",
				style = "page_number",
				align = "center",
				valign = "middle",
				wrap = false,
				w = 33f,
				h = 25f
			}, toChrome: true);
		});
		b.AddButton("Decoration image on every page...", delegate
		{
			PaperEditorSpritePicker.Open(editor.Entry.modId, "Pick a decoration", delegate(string key)
			{
				Sprite sprite = PaperAssets.Resolve(key, editor.Entry.modId, quiet: true);
				Vector2 vector = ((sprite != null) ? sprite.rect.size : new Vector2(60f, 40f));
				editor.AddElement(new PaperElementData
				{
					type = "image",
					sprite = key,
					w = vector.x,
					h = vector.y
				}, toChrome: true);
			});
		});
		Header(b, "Inspect vanilla UI");
		b.AddToggle("Inspect mode (hover other panels, click to pin)", editor.inspectMode, delegate(bool v)
		{
			editor.inspectMode = v;
			if (!v)
			{
				PaperEditorInspect.Clear();
			}
			editor.SetStatus(v ? "Inspect: hover a vanilla panel, click to pin it" : "");
		});
		PaperEditorInspect.Info info = PaperEditorInspect.Pinned ?? PaperEditorInspect.Hovered;
		if (!editor.inspectMode)
		{
			return;
		}
		b.AddLabel((info != null) ? info.Summary : "Open a vanilla guide, card or sheet and hover it.");
		if (info != null)
		{
			b.AddButton("Add like this", delegate
			{
				PaperEditorInspect.AddLikeThis(editor);
			});
		}
		b.AddLabel("Open guides from your inventory (manuals, cards) or the F8 spawner while inspect mode is on.");
	}

	private static void AddText(PaperTextStyle style, PaperElementData flow)
	{
		PaperElementData paperElementData = new PaperElementData
		{
			type = "text",
			text = ((style.id == "body") ? "Body text. Edit it in the Inspector tab." : style.id),
			style = style.id,
			w = ((style.id == "body") ? 200f : 150f),
			h = Mathf.Ceil(style.size * 1.6f),
			align = ((style.id == "title" || style.id == "header" || style.id == "page_number") ? "center" : "left"),
			autoHeight = (style.id == "body")
		};
		if (flow != null)
		{
			paperElementData.flow = flow.id;
			paperElementData.w = 0f;
			paperElementData.h = 0f;
			paperElementData.autoHeight = true;
		}
		editor.AddElement(paperElementData, toChrome: false, flow == null);
	}

	private static void BuildToolsTab(CustomUIBuilder b)
	{
		Header(b, "History");
		b.BeginRow();
		b.AddButton("Undo" + (editor.History.CanUndo ? (" (" + editor.History.UndoLabel + ")") : ""), editor.Undo);
		if (!editor.History.CanUndo || !editor.IsEditing)
		{
			b.Last.SetInteractable(interactable: false);
		}
		b.AddButton("Redo" + (editor.History.CanRedo ? (" (" + editor.History.RedoLabel + ")") : ""), editor.Redo);
		if (!editor.History.CanRedo || !editor.IsEditing)
		{
			b.Last.SetInteractable(interactable: false);
		}
		b.End();
		Header(b, "Snapping and view");
		b.BeginRow();
		b.AddToggle("Grid", editor.snapToGrid, delegate(bool v)
		{
			editor.snapToGrid = v;
		});
		b.AddToggle("Elements", editor.snapToElements, delegate(bool v)
		{
			editor.snapToElements = v;
		});
		b.AddToggle("Rulers", editor.showRulers, delegate(bool v)
		{
			editor.showRulers = v;
			editor.Overlay?.Refresh();
		});
		b.AddToggle("Ids", editor.showIds, delegate(bool v)
		{
			editor.showIds = v;
			editor.Overlay?.Refresh();
		});
		b.End();
		FloatField(b, "Grid size", editor.gridSize, delegate(float v)
		{
			editor.gridSize = Mathf.Max(0.5f, v);
		});
		b.AddLabel("Hold Ctrl while dragging to skip snapping.");
		b.BeginRow(4f, TextAnchor.MiddleLeft);
		b.AddLabel("Zoom");
		b.Last.SetFixedWidth(90f);
		b.AddSlider(0.5f, 3f, editor.editorZoom, delegate(float v)
		{
			editor.SetEditorZoom(Mathf.Round(v * 4f) / 4f);
		});
		b.Last.SetFlexible(1f, -1f);
		b.AddButton("1x", delegate
		{
			editor.SetEditorZoom(1f);
		});
		b.End();
		Header(b, "Arrange the selection");
		b.BeginRow();
		b.AddButton("Left", delegate
		{
			editor.Align(PaperEditor.AlignMode.Left);
		});
		b.AddButton("Center", delegate
		{
			editor.Align(PaperEditor.AlignMode.CenterX);
		});
		b.AddButton("Right", delegate
		{
			editor.Align(PaperEditor.AlignMode.Right);
		});
		b.End();
		b.BeginRow();
		b.AddButton("Top", delegate
		{
			editor.Align(PaperEditor.AlignMode.Top);
		});
		b.AddButton("Middle", delegate
		{
			editor.Align(PaperEditor.AlignMode.CenterY);
		});
		b.AddButton("Bottom", delegate
		{
			editor.Align(PaperEditor.AlignMode.Bottom);
		});
		b.End();
		b.AddLabel("One element aligns to the page, several to each other.");
		b.BeginRow();
		b.AddButton("Distribute horizontally", delegate
		{
			editor.Distribute(horizontal: true);
		});
		b.AddButton("Distribute vertically", delegate
		{
			editor.Distribute(horizontal: false);
		});
		b.End();
		b.BeginRow();
		b.AddButton("Match width", delegate
		{
			editor.MatchSize(width: true, height: false);
		});
		b.AddButton("Match height", delegate
		{
			editor.MatchSize(width: false, height: true);
		});
		b.AddButton("Match both", delegate
		{
			editor.MatchSize(width: true, height: true);
		});
		b.End();
		b.BeginRow();
		b.AddButton("To back", delegate
		{
			editor.ZOrder(-2);
		});
		b.AddButton("Backward", delegate
		{
			editor.ZOrder(-1);
		});
		b.AddButton("Forward", delegate
		{
			editor.ZOrder(1);
		});
		b.AddButton("To front", delegate
		{
			editor.ZOrder(2);
		});
		b.End();
		b.BeginRow();
		b.AddButton("Lock", delegate
		{
			editor.SetLocked(locked: true);
		});
		b.AddButton("Unlock", delegate
		{
			editor.SetLocked(locked: false);
		});
		b.AddButton("Hide", delegate
		{
			editor.SetVisible(visible: false);
		});
		b.AddButton("Show", delegate
		{
			editor.SetVisible(visible: true);
		});
		b.AddButton("Unlock all", delegate
		{
			if (editor.IsEditing)
			{
				editor.SelectMany(from e in editor.Data.AllElements()
					where e.locked
					select e);
				editor.SetLocked(locked: false);
			}
		});
		b.End();
		Header(b, "Locale preview");
		List<Locale> locales = editor.Locales();
		if (locales.Count > 0)
		{
			Locale selectedLocale = LocalizationSettings.SelectedLocale;
			int index = Mathf.Max(0, locales.IndexOf(selectedLocale));
			DropdownField(b, "Language", locales.Select((Locale l) => l.LocaleName).ToList(), index, delegate(int i)
			{
				editor.SetPreviewLocale(locales[i]);
			});
			b.AddLabel("Loc keys and fonts switch with the language; the original language comes back when editing stops.");
		}
		Header(b, "Shortcuts");
		b.AddLabel("Click: select   Shift+click: add   Drag on empty paper: marquee   Alt+drag: move the panel\nArrows: nudge 1 (Shift: 10)   Delete: remove   [ ]: backward / forward (Shift: back / front)\nCtrl+Z / Ctrl+Y: undo / redo   Ctrl+C / X / V: copy / cut / paste   Ctrl+D: duplicate   Ctrl+A: select all\nCtrl+S: save now   Ctrl+L: lock   Ctrl+H: hide   Ctrl+G: grid snapping   PageUp / PageDown: pages   Esc: deselect");
	}

	private static void BuildCheckTab(CustomUIBuilder b)
	{
		if (!editor.IsEditing)
		{
			b.AddLabel("Pick a book in the Book tab.");
			return;
		}
		b.AddButton("Run checks", delegate
		{
			lastIssues = PaperEditorValidation.Run(editor);
			Build();
		});
		if (lastIssues == null)
		{
			b.AddLabel("Checks ids, sprites, styles, loc keys, templates, slots, flows, page bounds, overlaps and text overflow.");
			return;
		}
		int num = lastIssues.Count((PaperEditorValidation.Issue i) => i.error);
		b.AddLabel($"{lastIssues.Count} finding(s), {num} error(s).");
		if (lastIssues.Count == 0)
		{
			b.AddLabel("All good.");
		}
		foreach (PaperEditorValidation.Issue lastIssue in lastIssues)
		{
			PaperEditorValidation.Issue captured = lastIssue;
			b.BeginRow(4f, TextAnchor.MiddleLeft);
			if (lastIssue.error)
			{
				b.AddWarning("[" + lastIssue.Where + "] " + lastIssue.message);
			}
			else
			{
				b.AddLabel("[" + lastIssue.Where + "] " + lastIssue.message);
			}
			b.Last.SetFlexible(1f, -1f);
			if (lastIssue.element != null)
			{
				b.AddButton("Select", delegate
				{
					if (captured.templateId != null)
					{
						editor.EditTemplate(captured.templateId);
					}
					else if (captured.page >= 0)
					{
						editor.GoToPage(captured.page);
					}
					editor.Select(captured.element);
				});
			}
			b.End();
		}
	}
}
