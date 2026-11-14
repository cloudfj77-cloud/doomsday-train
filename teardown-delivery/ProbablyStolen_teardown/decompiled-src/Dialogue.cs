using System;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue
{
	public const int IMPORTANT_PRIORITY = 6;

	public const int DEFAULT_PRIORITY = 5;

	public const int REACTION_PRIORITY = 4;

	public string title;

	public RichTextBuilder text;

	public string plainText;

	public Sprite portrait;

	public int seenTimes;

	public Func<Dialogue, bool> isDisabledFunc;

	public Func<Dialogue, bool> isHiddenFunc;

	public bool forceDisabled;

	public bool forceHidden;

	public bool isDummyChoiceDialogue;

	public Action startAction;

	public Action endAction;

	public float endActionDelay = 1.1f;

	public bool isEndActionExecuted;

	public int priority = 5;

	public bool isMainDialog;

	public Dialogue nextDialogue;

	public List<(GameItem, Dialogue, Action)> choices = new List<(GameItem, Dialogue, Action)>();

	public Dialogue AddCustomChoice(GameItem choiceItem, Dialogue choiceNextDialogue = null, Action choiceAction = null)
	{
		if (choiceItem != null)
		{
			choices.Add((choiceItem, choiceNextDialogue, choiceAction));
		}
		return this;
	}

	public Dialogue AddChoice(GameItem choiceItem, string name, string description = "", Dialogue choiceNextDialogue = null, Action choiceAction = null, Func<bool> choiceEnabled = null)
	{
		if (choiceItem != null)
		{
			choiceItem.name = name;
			choiceItem.shortDescription = description;
			OverlayHelper.TransformItemChoice(choiceItem);
			if (choiceEnabled != null)
			{
				if (choiceNextDialogue == null)
				{
					Debug.LogWarning("AddChoice: choiceEnabled ignored for '" + name + "' because it has no choiceNextDialogue to carry the condition.");
				}
				else
				{
					choiceNextDialogue.isDisabledFunc = (Dialogue _) => !choiceEnabled();
				}
			}
			choices.Add((choiceItem, choiceNextDialogue, choiceAction));
		}
		return this;
	}

	public bool IsChoiceEnabledAt(int index)
	{
		if (index < 0 || index >= choices.Count)
		{
			return false;
		}
		Dialogue item = choices[index].Item2;
		if (item != null)
		{
			return !item.IsDisabled();
		}
		return true;
	}

	public Dialogue AddChoiceTargetable(GameItem choiceItem, string name, string description = "", Dialogue choiceNextDialogue = null, Action choiceAction = null)
	{
		if (choiceItem != null)
		{
			choiceItem.name = name;
			choiceItem.shortDescription = description;
			choiceItem.mayStackItemFunc = (GameItem _, GameItem _) => false;
			choiceItem.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => true;
			choiceItem.maxSlotAddItemFunc = (GameItem _, GameInventory _, SlotMarker _) => 1;
			choiceItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => true;
			choices.Add((choiceItem, choiceNextDialogue, choiceAction));
		}
		return this;
	}

	public bool HasChoices()
	{
		foreach (var choice in choices)
		{
			GameItem item = choice.Item1;
			if (IsChoiceEnabled(item))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsChoiceEnabled(GameItem choiceItem)
	{
		if (!HasChoice(choiceItem))
		{
			return false;
		}
		Dialogue dialogue = FindChoiceDialogue(choiceItem);
		if ((dialogue == null || (!dialogue.IsDisabled() && !dialogue.IsHidden())) && choiceItem.MayActivateSlotItem() && choiceItem.CanActivateSlotItem())
		{
			return true;
		}
		return false;
	}

	public bool HasChoice(GameItem choiceItem)
	{
		foreach (var choice in choices)
		{
			if (choice.Item1 == choiceItem)
			{
				return true;
			}
		}
		return false;
	}

	public Action FindChoiceAction(GameItem choiceItem)
	{
		foreach (var choice in choices)
		{
			if (choice.Item1 == choiceItem)
			{
				return choice.Item3;
			}
		}
		return null;
	}

	public Dialogue FindChoiceDialogue(GameItem choiceItem)
	{
		foreach (var choice in choices)
		{
			if (choice.Item1 == choiceItem)
			{
				return choice.Item2;
			}
		}
		return null;
	}

	public Dialogue SetText(string title, string text)
	{
		this.title = title;
		this.text = new RichTextBuilder().Add(text);
		plainText = text;
		return this;
	}

	public Dialogue SetText(string title, RichTextBuilder text)
	{
		this.title = title;
		this.text = new RichTextBuilder().Add(text);
		return this;
	}

	public Dialogue SetDummy()
	{
		isDummyChoiceDialogue = true;
		return this;
	}

	public Dialogue SetPortrait(string atlasPath, string spriteName)
	{
		portrait = RenderHandler.LoadFromAtlas(atlasPath, spriteName);
		return this;
	}

	public Dialogue SetPortrait(Sprite portrait)
	{
		this.portrait = portrait;
		return this;
	}

	public Dialogue ForceDisable()
	{
		forceDisabled = true;
		return this;
	}

	public Dialogue RemoveForceDisable()
	{
		forceDisabled = false;
		return this;
	}

	public Dialogue ForceHide()
	{
		forceHidden = true;
		return this;
	}

	public Dialogue RemoveForceHide()
	{
		forceHidden = false;
		return this;
	}

	public Dialogue SetStartAction(Action startAction)
	{
		this.startAction = startAction;
		return this;
	}

	public Dialogue SetEndAction(Action endAction)
	{
		this.endAction = endAction;
		return this;
	}

	public Dialogue NextDialogue()
	{
		if (nextDialogue == null)
		{
			nextDialogue = new Dialogue();
			nextDialogue.priority = priority;
		}
		return nextDialogue;
	}

	public Dialogue SetNextDialogue(Dialogue dialogue)
	{
		nextDialogue = dialogue;
		return this;
	}

	public void SetUnique()
	{
		isHiddenFunc = (Dialogue d) => d.seenTimes < 1;
	}

	public bool IsDisabled()
	{
		if (!IsHidden())
		{
			Func<Dialogue, bool> func = isDisabledFunc;
			if (func == null || !func(this))
			{
				return forceDisabled;
			}
		}
		return true;
	}

	public bool IsHidden()
	{
		Func<Dialogue, bool> func = isHiddenFunc;
		if (func == null || !func(this))
		{
			return forceHidden;
		}
		return true;
	}

	public void SetPriority(int priority)
	{
		this.priority = priority;
	}

	public void ExecuteEndAction()
	{
		if (!isEndActionExecuted && endAction != null)
		{
			isEndActionExecuted = true;
			endAction();
		}
	}
}
