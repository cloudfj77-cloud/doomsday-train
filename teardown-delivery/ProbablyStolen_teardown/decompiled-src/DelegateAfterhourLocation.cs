using System;
using UnityEngine;

public class DelegateAfterhourLocation : AfterhourLocation
{
	public string id;

	public string source = "vanilla";

	public Func<string> title;

	public string imageKey;

	public GameObject stageObject;

	public GameObject panelObject;

	public string backgroundKey;

	public int backgroundSortingOrder = 20;

	public string ambienceId = "";

	public Func<string> panelTitle;

	public Func<bool> isDiscovered;

	public Func<bool> isAvailable;

	public bool usesAfterhourBag = true;

	public bool showsStoreCounters;

	public bool allowsHazardousWaste;

	public bool countsDumpedWaste;

	public Func<int, int, Vector2?> frontCounterPosition;

	public Func<int, int, Vector2?> backCounterPosition;

	public Action onArrive;

	public Action onLeave;

	public Action onTick;

	public Action<CustomUIBuilder> buildPanel;

	public Func<string> leaveWarning;

	public int maxConcurrentEvents;

	public override string Id => id;

	public override string Source => source;

	public override string ImageKey
	{
		get
		{
			if (!string.IsNullOrEmpty(imageKey))
			{
				return imageKey;
			}
			return id;
		}
	}

	public override GameObject StageObject => stageObject;

	public override GameObject PanelObject => panelObject;

	public override string BackgroundKey
	{
		get
		{
			if (!string.IsNullOrEmpty(backgroundKey))
			{
				return backgroundKey;
			}
			return id;
		}
	}

	public override int BackgroundSortingOrder => backgroundSortingOrder;

	public override string AmbienceId => ambienceId ?? "";

	public override bool UsesAfterhourBag => usesAfterhourBag;

	public override bool ShowsStoreCounters => showsStoreCounters;

	public override bool AllowsHazardousWaste => allowsHazardousWaste;

	public override bool CountsDumpedWaste => countsDumpedWaste;

	public override int MaxConcurrentEvents => maxConcurrentEvents;

	public override string GetTitle()
	{
		return AfterhourLocation.SafeText(title, this);
	}

	public override string GetPanelTitle()
	{
		if (panelTitle == null)
		{
			return GetTitle();
		}
		return AfterhourLocation.SafeText(panelTitle, this);
	}

	public override bool IsDiscovered()
	{
		if (isDiscovered != null)
		{
			return AfterhourLocation.Safe(isDiscovered, this);
		}
		return true;
	}

	public override bool IsAvailable()
	{
		if (isAvailable == null || AfterhourLocation.Safe(isAvailable, this))
		{
			return !IsClosedByEvent();
		}
		return false;
	}

	public override Vector2? GetFrontCounterPosition(int w, int h)
	{
		return frontCounterPosition?.Invoke(w, h);
	}

	public override Vector2? GetBackCounterPosition(int w, int h)
	{
		return backCounterPosition?.Invoke(w, h);
	}

	public override void Arrive()
	{
		Run(onArrive, "onArrive");
	}

	public override void Leave()
	{
		Run(onLeave, "onLeave");
	}

	public override void Tick()
	{
		Run(onTick, "onTick");
	}

	public override string GetLeaveWarning()
	{
		if (leaveWarning == null)
		{
			return null;
		}
		return AfterhourLocation.SafeText(leaveWarning, this);
	}

	public override void BuildPanel(CustomUIBuilder builder)
	{
		if (buildPanel == null)
		{
			return;
		}
		try
		{
			buildPanel(builder);
		}
		catch (Exception arg)
		{
			Debug.LogError($"[AfterhourLocation] '{id}' buildPanel threw: {arg}");
		}
	}

	private void Run(Action action, string name)
	{
		if (action == null)
		{
			return;
		}
		try
		{
			action();
		}
		catch (Exception arg)
		{
			Debug.LogError($"[AfterhourLocation] '{id}' {name} threw: {arg}");
		}
	}
}
