public class StoreNotebook : TooltipInterface, IClickable
{
	public override string GetTooltip()
	{
		return "Open the notebook";
	}

	public override bool IsTooltipEnabled()
	{
		return true;
	}

	public void OnClick()
	{
		NotebookUIManager.Instance.Toggle();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
