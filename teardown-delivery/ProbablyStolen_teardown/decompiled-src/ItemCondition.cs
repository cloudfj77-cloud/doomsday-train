public class ItemCondition
{
	[ES3Serializable]
	public string display;

	[ES3Serializable]
	public string identifier;

	[ES3Serializable]
	public int modValue;

	[ES3Serializable]
	public int modValueDisplay;

	[ES3Serializable]
	public bool isTransformative;

	[ES3Serializable]
	public string category;

	[ES3Serializable]
	public bool hiddenAsPublic;

	[ES3Serializable]
	public int customValue;

	[ES3Serializable]
	public string onActivateActionId;

	[ES3Serializable]
	public int compareValue;

	public string GetModValueFormated()
	{
		if (isTransformative)
		{
			if (modValueDisplay > 0)
			{
				return "+" + modValueDisplay;
			}
			return modValueDisplay.ToString() ?? "";
		}
		if (modValueDisplay != 0)
		{
			if (modValueDisplay > 0)
			{
				return "+" + modValueDisplay;
			}
			return modValueDisplay.ToString() ?? "";
		}
		if (modValue > 0)
		{
			return "+" + modValue;
		}
		return modValue.ToString() ?? "";
	}

	public bool HasOnActivateAction()
	{
		if (onActivateActionId == null || onActivateActionId.Equals(""))
		{
			return false;
		}
		return true;
	}

	public void ExecuteOnActivateAction(GameItem item)
	{
		if (HasOnActivateAction())
		{
			ItemConditionActionList.ItemConditionActionDict[onActivateActionId](item);
		}
	}
}
