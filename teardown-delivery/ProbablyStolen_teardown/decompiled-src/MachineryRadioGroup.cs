using System;
using System.Collections.Generic;

public class MachineryRadioGroup
{
	public readonly List<MachineryRadioButton> buttons = new List<MachineryRadioButton>();

	public Action<int> onChanged;

	public int selected => buttons.FindIndex((MachineryRadioButton b) => b.isOn);

	public MachineryRadioGroup Add(MachineryRadioButton button)
	{
		if (button == null || buttons.Contains(button))
		{
			return this;
		}
		buttons.Add(button);
		button.group = this;
		if (button.isOn)
		{
			NotifyOn(button, notify: false);
		}
		return this;
	}

	public MachineryRadioGroup Select(int index, bool notify = false)
	{
		if (index < 0 || index >= buttons.Count)
		{
			return this;
		}
		buttons[index].SetOn(on: true, notify);
		return this;
	}

	internal void NotifyOn(MachineryRadioButton source, bool notify)
	{
		foreach (MachineryRadioButton button in buttons)
		{
			if (button != source && button.isOn)
			{
				button.SetOn(on: false, notify);
			}
		}
		if (notify)
		{
			onChanged?.Invoke(buttons.IndexOf(source));
		}
	}
}
