using UnityEngine;

public class DumpingGroundLocation : AfterhourLocation
{
	private const string ATTEMPTS = "afterhour_attempts";

	private const string MINOR = "afterhour_minor";

	private const string MAJOR = "afterhour_major";

	private CustomUIWindow panel;

	public override string Id => "dumping_ground";

	public override GameObject StageObject
	{
		get
		{
			if (!(MapUIManager.Instance != null))
			{
				return null;
			}
			return MapUIManager.Instance.scavengingScene;
		}
	}

	public override GameObject PanelObject
	{
		get
		{
			if (!(MapUIManager.Instance != null))
			{
				return null;
			}
			return MapUIManager.Instance.UIScavengingPanel;
		}
	}

	public override bool AllowsHazardousWaste => true;

	public override bool CountsDumpedWaste => true;

	public override string GetTitle()
	{
		return LocHelper.GetLocalizedUI("ui_transit_dumping_ground");
	}

	public override bool IsDiscovered()
	{
		if (PlayerStore.Instance != null)
		{
			return !StartingPerk.IsMaimed();
		}
		return true;
	}

	public override void Arrive()
	{
		ScavHelper.ResetScavenging();
		MapUIManager.Instance.UpdateScavUI();
		SerpicoStoryline.OnArriveDumpingGround();
	}

	public override void Leave()
	{
		SerpicoStoryline.OnLeaveDumpingGround(EmporiumEntry.Instance.afterhourInventory);
	}

	public override void BuildPanel(CustomUIBuilder builder)
	{
		panel = builder.Handle;
		builder.AddLabel("", "afterhour_attempts");
		builder.AddLabel("", "afterhour_minor");
		builder.AddLabel("", "afterhour_major");
		builder.AddSpace(8f);
		builder.AddButton(LocHelper.GetLocalizedUI("afterhour_scavenge_action"), OnScavenge);
		RefreshLabels();
	}

	private void OnScavenge()
	{
		ScavHelper.ScavengeDumpingGrounds();
		RefreshLabels();
	}

	public override void Tick()
	{
		if (PanelObject != null)
		{
			MapUIManager.Instance.UpdateScavUI();
		}
		else
		{
			RefreshLabels();
		}
		SerpicoStoryline.TickDumpingGround(EmporiumEntry.Instance.afterhourInventory);
	}

	private void RefreshLabels()
	{
		if (panel != null && panel.IsAlive)
		{
			string text = (PlayerStore.Instance.healthData.isWoundedFresh ? LocHelper.GetLocalizedMechanic("afterhour_attempts_wounded") : LocHelper.GetLocalizedMechanic("afterhour_attempts_remaining", ScavHelper.GetScavTimeLeft()));
			string localizedMechanic = LocHelper.GetLocalizedMechanic("afterhour_minor_wound_chance", ScavHelper.GetMinorWoundChance().ToString("F2"));
			string localizedMechanic2 = LocHelper.GetLocalizedMechanic("afterhour_major_wound_chance", ScavHelper.GetMajorWoundChance().ToString("F2"));
			SetIfChanged("afterhour_attempts", text);
			SetIfChanged("afterhour_minor", localizedMechanic);
			SetIfChanged("afterhour_major", localizedMechanic2);
		}
	}

	private void SetIfChanged(string id, string text)
	{
		CustomUIElement customUIElement = panel.Get(id);
		if (customUIElement != null && !(customUIElement.text == null) && customUIElement.text.text != text)
		{
			customUIElement.SetText(text);
		}
	}
}
