using UnityEngine;

public class CommissaryLocation : AfterhourLocation
{
	public override string Id => "commissary";

	public override GameObject StageObject
	{
		get
		{
			if (!(MapUIManager.Instance != null))
			{
				return null;
			}
			return MapUIManager.Instance.commissaryScene;
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
			return MapUIManager.Instance.UICommissaryPanel;
		}
	}

	public override string GetTitle()
	{
		return LocHelper.GetLocalizedUI("commissary_title");
	}

	public override bool IsDiscovered()
	{
		if (PlayerStore.Instance != null && PlayerStore.Instance.isStampIntroduced)
		{
			return !StartingPerk.IsBannedCommissary();
		}
		return false;
	}

	public override void BuildPanel(CustomUIBuilder builder)
	{
		builder.AddButton(LocHelper.GetLocalizedUI("afterhour_trade_action"), delegate
		{
			CommissaryUIManager.Instance.OpenUI();
		});
	}

	public override void Leave()
	{
		CommissaryUIManager.Instance.CloseUI();
	}
}
