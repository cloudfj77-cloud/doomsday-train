using System;
using System.Collections.Generic;

public class MachineryRadioList : MachineryStack
{
	public readonly MachineryRadioGroup group = new MachineryRadioGroup();

	public int selected => group.selected;

	public MachineryRadioList(IList<string> captions, int selected = 0, Action<int> onChanged = null)
		: base(vertical: true, 6)
	{
		align = Align.Start;
		for (int i = 0; i < captions.Count; i++)
		{
			MachineryRadioButton machineryRadioButton = new MachineryRadioButton(captions[i], i == selected);
			group.Add(machineryRadioButton);
			Add(machineryRadioButton);
		}
		group.onChanged = onChanged;
	}

	public MachineryRadioButton Get(int index)
	{
		if (index < 0 || index >= group.buttons.Count)
		{
			return null;
		}
		return group.buttons[index];
	}

	public MachineryRadioList Select(int index, bool notify = false)
	{
		group.Select(index, notify);
		return this;
	}
}
