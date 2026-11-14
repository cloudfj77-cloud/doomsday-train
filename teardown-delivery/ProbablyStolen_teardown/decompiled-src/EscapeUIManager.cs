using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeUIManager : MonoBehaviour
{
	public GameObject discordButton;

	public GameObject menuPanel;

	public AudioSource audioSource;

	private bool isPendingTransition;

	public GameObject playtestExitUI;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnAnyClick()
	{
		audioSource.Play();
	}

	public void OpenEscapeMenu()
	{
		if (NewGameData.Instance.isShowcase)
		{
			discordButton.SetActive(value: false);
		}
		else
		{
			discordButton.SetActive(value: true);
		}
		menuPanel.SetActive(value: true);
		StoreUI[] componentsInChildren = menuPanel.GetComponentsInChildren<StoreUI>(includeInactive: true);
		foreach (StoreUI storeUI in componentsInChildren)
		{
			if (storeUI.transform.parent == menuPanel.transform)
			{
				storeUI.gameObject.SetActive(value: true);
			}
		}
		OnAnyClick();
	}

	public void CloseEscapeMenu()
	{
		menuPanel.SetActive(value: false);
		OnAnyClick();
	}

	public void ToggleEscapeMenu()
	{
		if (menuPanel.activeSelf)
		{
			CloseEscapeMenu();
		}
		else
		{
			OpenEscapeMenu();
		}
	}

	public void OnResume()
	{
		CloseEscapeMenu();
		OnAnyClick();
	}

	public void OnOverlay()
	{
		StoreUIManager.Instance.ToggleControlHintOverlay();
		CloseEscapeMenu();
	}

	public void OnOption()
	{
		CloseEscapeMenu();
		StoreUIManager.Instance.gameOptionUIManager.OpenUI();
		OnAnyClick();
	}

	public void OnMainMenu()
	{
		if (!isPendingTransition)
		{
			isPendingTransition = true;
			OnAnyClick();
			StartCoroutine(MainMenuWithAnalytics());
		}
	}

	private IEnumerator MainMenuWithAnalytics()
	{
		EmporiumAnalytics.TrackLeaveToMainMenu();
		if (EmporiumOption.Instance.consentAnalytics)
		{
			yield return new WaitForSecondsRealtime(1f);
		}
		NewGameData.Instance.isLoadFromMainMenu = true;
		PlayerStore.Instance.Reset();
		EmporiumEntry.ResetData();
		PlayerStore.ResetData();
		StoreStation.ResetData();
		SceneManager.LoadScene("EmporiumMenu", LoadSceneMode.Single);
	}

	public void OnExit()
	{
		if (!isPendingTransition)
		{
			_ = EmporiumOption.Instance.hasWishlistRecommended;
			isPendingTransition = true;
			OnAnyClick();
			StartCoroutine(QuitWithAnalytics());
		}
	}

	private IEnumerator QuitWithAnalytics()
	{
		EmporiumAnalytics.TrackGameExit();
		if (EmporiumOption.Instance.consentAnalytics)
		{
			yield return new WaitForSecondsRealtime(1f);
		}
		Application.Quit();
	}

	public void OnDiscordClicked()
	{
		OnAnyClick();
		Application.OpenURL("https://discord.gg/probablystolen");
	}

	public void OnWishListClicked()
	{
		OnAnyClick();
		Application.OpenURL("steam://store/4348910");
	}

	public void OnFeedbackClicked()
	{
		OnAnyClick();
		Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLScVUotJhNfnyAlV31wH3j2zln4mc-c5zhjnHEACKdOstk1dOw/viewform");
	}

	public void OnPlaytestExitClicked()
	{
		if (!isPendingTransition)
		{
			isPendingTransition = true;
			OnAnyClick();
			StartCoroutine(QuitWithAnalytics());
		}
	}
}
