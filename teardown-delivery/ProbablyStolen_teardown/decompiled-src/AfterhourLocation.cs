using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AfterhourLocation
{
	public enum StatusTone
	{
		Attention,
		Unavailable
	}

	public const string VANILLA = "vanilla";

	public const string DUMPING_GROUND = "dumping_ground";

	public const string COMMISSARY = "commissary";

	public const string DR_JACKSON = "dr_jackson";

	private static readonly List<AfterhourLocation> all = new List<AfterhourLocation>();

	private static bool vanillaRegistered;

	public abstract string Id { get; }

	public virtual string Source => "vanilla";

	public virtual string ImageKey => Id;

	public virtual GameObject StageObject => null;

	public virtual GameObject PanelObject => null;

	public virtual string BackgroundKey => Id;

	public virtual int BackgroundSortingOrder => 20;

	public virtual string AmbienceId => Id;

	public virtual bool UsesAfterhourBag => true;

	public virtual bool ShowsStoreCounters => false;

	public virtual bool AllowsHazardousWaste => false;

	public virtual bool CountsDumpedWaste => false;

	public List<AfterhourEventDefinition> ActiveEvents => AfterhourEvents.GetActive(Id);

	public virtual int MaxConcurrentEvents => 0;

	public AfterhourLocationState State
	{
		get
		{
			if (!PlayerStore.HasInstance)
			{
				return null;
			}
			return PlayerStore.Instance.GetAfterhourState(Id);
		}
	}

	public abstract string GetTitle();

	public virtual bool IsDiscovered()
	{
		return true;
	}

	public virtual bool IsAvailable()
	{
		return !IsClosedByEvent();
	}

	public bool IsClosedByEvent()
	{
		foreach (AfterhourEventDefinition activeEvent in ActiveEvents)
		{
			if (activeEvent.closesLocation)
			{
				return true;
			}
		}
		return false;
	}

	public virtual string GetPanelTitle()
	{
		return GetTitle();
	}

	public virtual Vector2? GetFrontCounterPosition(int canvasWidth, int canvasHeight)
	{
		return null;
	}

	public virtual Vector2? GetBackCounterPosition(int canvasWidth, int canvasHeight)
	{
		return null;
	}

	public virtual void Arrive()
	{
	}

	public virtual void Leave()
	{
	}

	public virtual void Tick()
	{
	}

	public virtual void BuildPanel(CustomUIBuilder builder)
	{
	}

	public virtual string GetLeaveWarning()
	{
		return null;
	}

	public float GetModifier(string key)
	{
		float num = 0f;
		foreach (AfterhourEventDefinition activeEvent in ActiveEvents)
		{
			num += activeEvent.GetModifier(key);
		}
		return num;
	}

	public static List<AfterhourLocation> GetAll()
	{
		EnsureVanilla();
		return all;
	}

	public static List<AfterhourLocation> GetDiscovered()
	{
		List<AfterhourLocation> list = new List<AfterhourLocation>();
		foreach (AfterhourLocation item in GetAll())
		{
			if (Safe(item.IsDiscovered, item))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static AfterhourLocation Get(string id)
	{
		foreach (AfterhourLocation item in GetAll())
		{
			if (item.Id == id)
			{
				return item;
			}
		}
		return null;
	}

	public static void Register(AfterhourLocation location)
	{
		if (location == null || string.IsNullOrEmpty(location.Id))
		{
			Debug.LogError("[AfterhourLocation] Register needs a location with an id");
			return;
		}
		EnsureVanilla();
		for (int i = 0; i < all.Count; i++)
		{
			if (all[i].Id == location.Id)
			{
				all[i] = location;
				return;
			}
		}
		all.Add(location);
	}

	public static int RemoveSource(string source)
	{
		EnsureVanilla();
		return all.RemoveAll((AfterhourLocation location) => location.Source == source);
	}

	private static void EnsureVanilla()
	{
		if (!vanillaRegistered)
		{
			vanillaRegistered = true;
			all.Insert(0, new DumpingGroundLocation());
			all.Insert(1, new CommissaryLocation());
			all.Insert(2, new DrJacksonLocation());
		}
	}

	internal static bool Safe(Func<bool> condition, AfterhourLocation location)
	{
		try
		{
			return condition();
		}
		catch (Exception arg)
		{
			Debug.LogError($"[AfterhourLocation] '{location.Id}' condition threw: {arg}");
			return false;
		}
	}

	internal static string SafeText(Func<string> provider, AfterhourLocation location)
	{
		if (provider == null)
		{
			return "";
		}
		try
		{
			return provider() ?? "";
		}
		catch (Exception arg)
		{
			Debug.LogError($"[AfterhourLocation] '{location.Id}' text provider threw: {arg}");
			return "";
		}
	}
}
