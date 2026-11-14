using Febucci.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUIManager : MonoBehaviour
{
	public GameObject uiPanel;

	public TextMeshProUGUI reason;

	public TextMeshProUGUI description;

	public GameObject darkScreen;

	public TypewriterByCharacter typewriterByCharacter;

	public RectTransform rectTransform;

	public GameObject continuePlayButton;

	private string endingId = "";

	private bool canKeepPlaying;

	public static GameOverUIManager Instance { get; private set; }

	public bool IsShowing
	{
		get
		{
			if (!(uiPanel != null) || !uiPanel.activeSelf)
			{
				return RunSummaryUI.IsOpen;
			}
			return true;
		}
	}

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	public void SetEnding(string ending)
	{
		endingId = ending ?? "";
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void DisplayUI(string givenReason, string givenDescription, bool showContinuePlay = false)
	{
		uiPanel.SetActive(value: true);
		darkScreen.SetActive(value: true);
		Image component = darkScreen.GetComponent<Image>();
		if (component != null)
		{
			component.color = Color.black;
		}
		reason.text = givenReason;
		description.text = "  " + givenDescription;
		typewriterByCharacter.ShowText(description.text);
		rectTransform.SetAsLastSibling();
		MachineCassettePlayer.PauseForGameOver();
		StoreSoundPlayer.Instance.StartGameOverMusic();
		canKeepPlaying = showContinuePlay;
		continuePlayButton.SetActive(showContinuePlay && !RunSummaryUI.CanOpen());
	}

	public void OnMenuButtonClick()
	{
		if (RunSummaryUI.CanOpen())
		{
			uiPanel.SetActive(value: false);
			if (RunSummaryUI.Open(endingId, canKeepPlaying, GoToMainMenu, ContinuePlayClick))
			{
				return;
			}
			uiPanel.SetActive(value: true);
		}
		GoToMainMenu();
	}

	public void GoToMainMenu()
	{
		Debug.Log("Main menu Button Clicked");
		StoreSoundPlayer.Instance.StopGameOverMusic();
		uiPanel.SetActive(value: false);
		darkScreen.SetActive(value: false);
		StoreUIManager.Instance.escapeUIManager.OnMainMenu();
	}

	public void ContinuePlayClick()
	{
		StoreSoundPlayer.Instance.StopGameOverMusic();
		uiPanel.SetActive(value: false);
		darkScreen.SetActive(value: false);
	}
}
