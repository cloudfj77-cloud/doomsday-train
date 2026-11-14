using System.Collections.Generic;
using TMPro;

public class NewsUIElement : NegociationElement
{
	public TextMeshProUGUI title;

	public TextMeshProUGUI textContent;

	public StoreEvent savedStoreEvent;

	private string negociationToolID = "";

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void PopulateUI(StoreEvent storeEvent)
	{
		title.text = storeEvent.newsName;
		textContent.text = "     " + storeEvent.newsDescription;
		if (storeEvent.duration == 1 && !storeEvent.isPermanent && storeEvent.eventType != StoreEvent.EventType.COSMETIC)
		{
			TextMeshProUGUI textMeshProUGUI = textContent;
			textMeshProUGUI.text = textMeshProUGUI.text + "\n<color=#BE514C>[" + LocHelper.GetLocalizedUI("ui_newspaper_end_today") + "]</color>";
		}
		else if (!storeEvent.isPermanent && storeEvent.eventType != StoreEvent.EventType.COSMETIC && Difficulty.Rules.eventEndCountdown)
		{
			TextMeshProUGUI textMeshProUGUI2 = textContent;
			textMeshProUGUI2.text = textMeshProUGUI2.text + "\n<color=#BE514C>[" + LocHelper.GetLocalizedUI("ui_newspaper_end_in_days", storeEvent.duration) + "]</color>";
		}
		savedStoreEvent = storeEvent;
		UpdateNegociationToolID();
	}

	public void ClearElement()
	{
		savedStoreEvent = null;
	}

	public void OnClick()
	{
	}

	public void UpdateNegociationToolID()
	{
		negociationToolID = "EXAMPLE";
	}

	public override string GetNegociationToolID()
	{
		return "NEWS";
	}

	public override bool IsNegociationTool()
	{
		return true;
	}

	public override List<NegociationData> GetNegociationData()
	{
		return savedStoreEvent.negociationDatas;
	}
}
