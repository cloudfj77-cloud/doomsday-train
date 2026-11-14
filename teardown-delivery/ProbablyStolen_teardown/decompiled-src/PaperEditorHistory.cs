using System.Collections.Generic;

public class PaperEditorHistory
{
	public class Snapshot
	{
		public string json;

		public List<string> selection;

		public string label;
	}

	private const int LIMIT = 200;

	private readonly List<Snapshot> undo = new List<Snapshot>();

	private readonly List<Snapshot> redo = new List<Snapshot>();

	private Snapshot current;

	public bool CanUndo => undo.Count > 0;

	public bool CanRedo => redo.Count > 0;

	public string UndoLabel
	{
		get
		{
			if (!CanUndo)
			{
				return null;
			}
			return current?.label;
		}
	}

	public string RedoLabel
	{
		get
		{
			if (!CanRedo)
			{
				return null;
			}
			return redo[redo.Count - 1].label;
		}
	}

	public int UndoCount => undo.Count;

	public void Reset(PaperBookData data, List<string> selection, string label = "open")
	{
		undo.Clear();
		redo.Clear();
		current = Make(data, selection, label);
	}

	public void Push(PaperBookData data, List<string> selection, string label)
	{
		Snapshot snapshot = Make(data, selection, label);
		if (current != null)
		{
			if (current.json == snapshot.json)
			{
				current.selection = snapshot.selection;
				return;
			}
			undo.Add(current);
			if (undo.Count > 200)
			{
				undo.RemoveAt(0);
			}
		}
		current = snapshot;
		redo.Clear();
	}

	public Snapshot Undo()
	{
		if (!CanUndo)
		{
			return null;
		}
		redo.Add(current);
		current = undo[undo.Count - 1];
		undo.RemoveAt(undo.Count - 1);
		return current;
	}

	public Snapshot Redo()
	{
		if (!CanRedo)
		{
			return null;
		}
		undo.Add(current);
		current = redo[redo.Count - 1];
		redo.RemoveAt(redo.Count - 1);
		return current;
	}

	private static Snapshot Make(PaperBookData data, List<string> selection, string label)
	{
		return new Snapshot
		{
			json = PaperJson.Serialize(data),
			selection = ((selection != null) ? new List<string>(selection) : new List<string>()),
			label = label
		};
	}
}
