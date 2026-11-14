using System;
using System.Collections.Generic;

public class EventData
{
	public List<(GameItem, Func<bool>, Func<bool>, Action)> choices = new List<(GameItem, Func<bool>, Func<bool>, Action)>();

	public int turns;

	public int startTurn;

	public int endTurn;

	public bool blockStation;

	public bool blockTravel;

	public bool blockTurns;

	private bool isEventStarted;

	private bool isEventEnded;

	public Func<bool> canEndFunc;

	public Action<EventData> startAction;

	public Action<EventData> turnAction;

	public Action<EventData> endAction;

	private int previousValidateTurn = -1;

	public string title { get; set; }

	public RichTextBuilder text { get; set; }

	public string imagePath { get; set; }

	public EventData()
	{
		turns = 0;
		startTurn = 0;
		endTurn = int.MaxValue;
	}

	public EventData SetText(string title, string text)
	{
		this.title = title;
		this.text = new RichTextBuilder().Add(text);
		return this;
	}

	public EventData SetText(string title, RichTextBuilder text)
	{
		this.title = title;
		this.text = new RichTextBuilder().Add(text);
		return this;
	}

	public void AddText(string text)
	{
		this.text = this.text.Add(text);
	}

	public void AddText(RichTextBuilder text)
	{
		this.text = this.text.Add(text);
	}

	public EventData SetImage(string imagePath)
	{
		this.imagePath = "EventCards/" + imagePath;
		return this;
	}

	public static void OnChoiceEnable(GameItem choiceItem)
	{
		if (choiceItem != null)
		{
			choiceItem.activateDefault = true;
			choiceItem.forceDisableActivate = false;
			if (choiceItem is GameItemElement gameItemElement)
			{
				gameItemElement.ToggleBackground(enabled: true);
			}
		}
	}

	public static void OnChoiceDisable(GameItem choiceItem)
	{
		if (choiceItem != null)
		{
			choiceItem.activateDefault = false;
			choiceItem.forceDisableActivate = true;
			if (choiceItem is GameItemElement gameItemElement)
			{
				gameItemElement.ToggleBackground(enabled: true, 0.5f, 0.5f);
			}
		}
	}

	public EventData AddCustomChoice(GameItem choiceItem, Func<bool> choiceEnabled = null, Func<bool> choiceShow = null, Action choiceAction = null)
	{
		if (choiceItem != null)
		{
			choices.Add((choiceItem, choiceEnabled, choiceShow, choiceAction));
		}
		return this;
	}

	public EventData AddChoice(GameItem choiceItem, string name, string description = "", Func<bool> choiceEnabled = null, Func<bool> choiceShow = null, Action choiceAction = null)
	{
		if (choiceItem != null)
		{
			choiceItem.name = name;
			choiceItem.shortDescription = description;
			OverlayHelper.TransformItemChoice(choiceItem);
			choices.Add((choiceItem, choiceEnabled, choiceShow, choiceAction));
		}
		return this;
	}

	public EventData SetStartTurn(int turn)
	{
		startTurn = turn;
		ValidateState();
		return this;
	}

	public EventData SetEndTurn(int turn)
	{
		endTurn = turn;
		ValidateState();
		return this;
	}

	public EventData BlockAll()
	{
		blockStation = true;
		blockTravel = true;
		blockTurns = true;
		return this;
	}

	public EventData BlockStation()
	{
		blockStation = true;
		return this;
	}

	public EventData BlockTravel()
	{
		blockTravel = true;
		return this;
	}

	public EventData BlockTurns()
	{
		blockTurns = true;
		return this;
	}

	public EventData IncrementTurn()
	{
		turns++;
		ValidateState();
		return this;
	}

	public bool IsRunning()
	{
		if (isEventStarted && !isEventEnded)
		{
			return GameMaster.current.shipManager.currentEvent == this;
		}
		return false;
	}

	public EventData ValidateState()
	{
		if (GameMaster.current.shipManager.currentEvent != this)
		{
			return this;
		}
		if (turns >= startTurn && !isEventStarted && !isEventEnded)
		{
			startAction?.Invoke(this);
			isEventStarted = true;
		}
		if (turns != previousValidateTurn && isEventStarted && !isEventEnded)
		{
			turnAction?.Invoke(this);
		}
		previousValidateTurn = turns;
		if (turns >= endTurn && isEventStarted && !isEventEnded)
		{
			endAction?.Invoke(this);
			isEventEnded = true;
		}
		if (isEventStarted && isEventEnded)
		{
			GameMaster.current.shipManager.currentEvent = null;
		}
		return this;
	}

	public EventData ForceStart()
	{
		if (GameMaster.current.shipManager.currentEvent != this)
		{
			return this;
		}
		if (!isEventStarted && !isEventEnded)
		{
			startAction?.Invoke(this);
			isEventStarted = true;
		}
		ValidateState();
		return this;
	}

	public EventData ForceEnd()
	{
		if (GameMaster.current.shipManager.currentEvent != this)
		{
			return this;
		}
		if (isEventStarted && !isEventEnded)
		{
			endAction?.Invoke(this);
			isEventEnded = true;
		}
		ValidateState();
		return this;
	}
}
