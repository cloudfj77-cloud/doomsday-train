public class NewsPaper : TooltipInterface, IClickable
{
	public override string GetTooltip()
	{
		return "Read the local newspaper";
	}

	public override bool IsTooltipEnabled()
	{
		return true;
	}

	public void OnClick()
	{
		NewsUIManager.Instance.ToggleUI();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
