using System;

public class PauseMenu : UIPanel
{
	public static PauseMenu Instance => GameManager.PauseMenu;

	public bool Shown
	{
		get
		{
			if (fadeGroup == null)
			{
				return false;
			}
			return fadeGroup.IsShown;
		}
	}

	public static event Action onPauseMenuOn;

	public static event Action onPauseMenuOff;

	public static void Show()
	{
		Instance.Open();
		PauseMenu.onPauseMenuOn?.Invoke();
	}

	public static void Hide()
	{
		Instance.Close();
		PauseMenu.onPauseMenuOff?.Invoke();
	}

	public static void Toggle()
	{
		if (Instance.fadeGroup.IsShown)
		{
			Hide();
		}
		else
		{
			Show();
		}
	}
}
