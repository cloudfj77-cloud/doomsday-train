using UnityEngine;

public class StorePC : TooltipInterface, IClickable
{
	public int state;

	public GameObject letter;

	public GameObject dot;

	public override string GetTooltip()
	{
		return LocHelper.GetLocalizedUI("inte_pc");
	}

	public override bool IsTooltipEnabled()
	{
		return true;
	}

	public void OnClick()
	{
		PCUIManager.Instance.ToggleUI();
		if (state == 2)
		{
			SetState(1);
		}
	}

	public void RefreshState()
	{
		if (PlayerStore.Instance.storeRequests.Count == 0)
		{
			SetState(0);
		}
		else
		{
			letter.SetActive(value: true);
		}
	}

	public void SetState(int newState)
	{
		state = newState;
		if (state == 0)
		{
			letter.SetActive(value: false);
			dot.SetActive(value: false);
		}
		else if (state == 1)
		{
			letter.SetActive(value: true);
			dot.SetActive(value: false);
		}
		else if (state == 2)
		{
			letter.SetActive(value: true);
			dot.SetActive(value: true);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
