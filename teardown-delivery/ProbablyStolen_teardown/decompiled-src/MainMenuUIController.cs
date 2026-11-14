using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
	public const string DISCORD_LINK = "https://discord.gg/probablystolen";

	public const string STORE_LINK = "steam://store/4348910";

	public const string DEMO_LINK = "steam://store/4349200";

	public const string SURVEY_LINK = "https://docs.google.com/forms/d/e/1FAIpQLScVUotJhNfnyAlV31wH3j2zln4mc-c5zhjnHEACKdOstk1dOw/viewform";

	public const string PRIVACY_POLICY_LINK = "https://www.tinybuild.com/privacy-policy";

	public const string CREATOR_LINK = null;

	public const bool IS_PLAYTEST_APP = false;

	public AudioSource audioSource;

	public AudioSource themeSource;

	public GameObject newGamePanel;

	public TextMeshProUGUI title;

	public TextMeshProUGUI body;

	public TextMeshProUGUI startingEquipment;

	public TextMeshProUGUI note;

	public GameObject comingsoon;

	public TextMeshProUGUI unavailableInDemo;

	public Button startGameButton;

	public Image blackscreen;

	public Toggle skipToggle;

	public Toggle hardToggle;

	public Toggle endlessToggle;

	public TextMeshProUGUI versionTMP;

	public GameObject creditPanel;

	public GameObject welcomePanel;

	public GameObject modPanel;

	public GameObject skipIntroLock;

	public ConfirmPanelUIManager confirmPanelUIManager;

	public SaveUIManager saveUIManager;

	public TMP_Dropdown localeDropdown;

	public GameObject creditButton;

	public GameObject discordButton;

	public GameObject creatorButton;

	public TextMeshProUGUI showcaseTMP;

	public Sprite logoSprite;

	public Sprite logolessSprite;

	public SpriteRenderer logoImage;

	public Sprite tab;

	public Sprite tabSelected;

	public Image tabWelcome;

	public Image tabGeneralist;

	public Image tabJunker;

	public Image tabWaterMerchant;

	public Image tabFence;

	public Image tabFarmer;

	public Image tabGunsmith;

	public Image tabChemist;

	public Image tabOrganTrader;

	public Image tabRockBottom;

	public Image tabStreetFoodVendor;

	public Image tabRancher;

	public Image darknessOverlay;

	public Image darknessOverlay2;

	public Toggle welcomePanelDataConsentToggle;

	public GameObject moddingButton;

	public GameObject playtestNotice;

	public GameObject playtestEndedPanel;

	private static readonly (string code, string label)[] LOCALE_DROPDOWN = new(string, string)[8]
	{
		("en", "English"),
		("zh", "Simplified Chinese"),
		("ru", "Russian"),
		("de", "German"),
		("ja", "Japanese"),
		("fr", "French"),
		("es", "Spanish"),
		("pl", "Polish")
	};

	public static MainMenuUIController Instance { get; private set; }

	private void Awake()
	{
		Debug.Log("MainMenuUIController Awake.");
		EmporiumAnalytics.InitAnalytics();
		NewGameData.HardReset();
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	private IEnumerator Start()
	{
		skipToggle.isOn = false;
		hardToggle.isOn = false;
		endlessToggle.isOn = false;
		versionTMP.text = PlayerStore.VERSION;
		if (EmporiumOption.Instance != null)
		{
			EmporiumOption.Instance.ApplyMusicVolume();
		}
		yield return LocalizationSettings.InitializationOperation;
		PreLoadLocalizationTable();
		HandleLocaleDropdown();
		HandleWelcomePanel();
		if (NewGameData.Instance.isShowcase)
		{
			creditButton.SetActive(value: false);
			discordButton.SetActive(value: false);
			showcaseTMP.gameObject.SetActive(value: true);
			logoImage.sprite = logolessSprite;
		}
		else
		{
			logoImage.sprite = logolessSprite;
		}
		moddingButton.SetActive(value: true);
		if (creatorButton != null && string.IsNullOrEmpty(null))
		{
			creatorButton.SetActive(value: false);
		}
		if (playtestNotice != null)
		{
			playtestNotice.SetActive(value: false);
		}
	}

	private void Update()
	{
	}

	public void OnAnyButtonClick()
	{
		audioSource.Play();
	}

	public bool IsPreviousSaveExist()
	{
		return false;
	}

	public void OnNewGameClick()
	{
		OnAnyButtonClick();
		NewGameSpecializationUI.Open();
	}

	public void OnScenarioContinue(StartingScenario scenario)
	{
		if (scenario == null)
		{
			return;
		}
		OnAnyButtonClick();
		NewGameData.Instance.startType = scenario.startType;
		NewGameData.Instance.startScenarioId = scenario.id;
		if (skipIntroLock != null)
		{
			skipIntroLock.SetActive(scenario.skipIntro);
		}
		if (scenario.startType != NewGameData.StartType.APPRENTICE && ES3.Load("totalRunCount", "saves_index.es3", 0) == 0)
		{
			NewGameSpecializationUI.Hide();
			darknessOverlay2.gameObject.SetActive(value: true);
			confirmPanelUIManager.OpenPanel(LocHelper.GetLocalizedMainMenu("first_run_confirm"), delegate
			{
				darknessOverlay2.gameObject.SetActive(value: false);
				OpenPerkStepFromScenario();
			}, delegate
			{
				darknessOverlay2.gameObject.SetActive(value: false);
				NewGameSpecializationUI.Open();
			});
		}
		else
		{
			OpenPerkStepFromScenario();
		}
	}

	private void OpenPerkStepFromScenario()
	{
		NewGameSpecializationUI.Close();
		OpenPerkInterface();
	}

	public void OnPlaytestEndedClick()
	{
		OnAnyButtonClick();
		if (!(playtestEndedPanel == null))
		{
			playtestEndedPanel.SetActive(value: true);
			darknessOverlay.gameObject.SetActive(value: true);
		}
	}

	public void ClosePlaytestEndedPanel()
	{
		OnAnyButtonClick();
		if (playtestEndedPanel != null)
		{
			playtestEndedPanel.SetActive(value: false);
		}
		darknessOverlay.gameObject.SetActive(value: false);
	}

	public void OnDemoLink()
	{
		Application.OpenURL("steam://store/4349200");
	}

	public void OnContinueClick()
	{
		if (NewGameData.Instance.startType != NewGameData.StartType.APPRENTICE && ES3.Load("totalRunCount", "saves_index.es3", 0) == 0)
		{
			darknessOverlay2.gameObject.SetActive(value: true);
			confirmPanelUIManager.OpenPanel(LocHelper.GetLocalizedMainMenu("first_run_confirm"), delegate
			{
				darknessOverlay2.gameObject.SetActive(value: false);
				newGamePanel.SetActive(value: false);
				OpenPerkInterface();
			}, delegate
			{
				skipToggle.isOn = false;
				darknessOverlay2.gameObject.SetActive(value: false);
			});
		}
		else
		{
			newGamePanel.SetActive(value: false);
			OpenPerkInterface();
		}
	}

	public void OpenPerkInterface()
	{
		OnAnyButtonClick();
		NewGamePerksUI.Open();
	}

	public void OnPerksContinue()
	{
		OnAnyButtonClick();
		NewGameDifficultyUI.Open();
	}

	public void OnPerkFinishedClick()
	{
		PerkUIController.Instance.HideUI();
		CloseNewGameMenu();
		StartNewGameClick();
	}

	public void StartNewGameClick()
	{
		NewGameData.Instance.skipIntro = NewGameData.Instance.skipIntro || skipIntroLock.activeSelf || skipToggle.isOn;
		OnAnyButtonClick();
		StartBlackscreenSequence(playFullSequence: false, delegate
		{
			EmporiumEntry.sceneLoadStartTime = Time.realtimeSinceStartup;
			SceneManager.LoadScene("InventoryScene", LoadSceneMode.Single);
		});
	}

	public void CloseNewGameMenu()
	{
		OnAnyButtonClick();
		newGamePanel.SetActive(value: false);
		darknessOverlay.gameObject.SetActive(value: false);
	}

	public void OnLoadGameClick()
	{
		LoadGameUI.Open();
		OnAnyButtonClick();
	}

	public void OnOptionClick()
	{
		OnAnyButtonClick();
	}

	public void OnExitClick()
	{
		OnAnyButtonClick();
		Application.Quit();
	}

	public void OpenModPanel()
	{
		OnAnyButtonClick();
		ModUIManager.Instance.OpenPanel();
	}

	public void CloseModPanel()
	{
		OnAnyButtonClick();
		ModUIManager.Instance.ClosePanel();
	}

	public void SetAvailableInDemo()
	{
	}

	public void SetUnavailableInDemo()
	{
		UpdateDemoAvailability();
	}

	public void OnWelcomeClick()
	{
		OnAnyButtonClick();
		title.text = LocHelper.GetLocalizedMainMenu("welcome_title");
		body.text = LocHelper.GetLocalizedNewGame("welcome_main") ?? "";
		startingEquipment.text = LocHelper.GetLocalizedNewGame("welcome_hint");
		note.text = "";
		comingsoon.SetActive(value: false);
		startGameButton.gameObject.SetActive(value: false);
		skipToggle.gameObject.SetActive(value: false);
		hardToggle.gameObject.SetActive(value: false);
		endlessToggle.gameObject.SetActive(value: false);
		skipIntroLock.SetActive(value: false);
		ResetAllTab();
		tabWelcome.sprite = tabSelected;
		SetAvailableInDemo();
	}

	public void OnAnyStartClick()
	{
		skipToggle.gameObject.SetActive(value: true);
		hardToggle.gameObject.SetActive(value: true);
		endlessToggle.gameObject.SetActive(value: true);
		if (NewGameData.Instance.isShowcase && NewGameData.Instance.startType != NewGameData.StartType.APPRENTICE)
		{
			comingsoon.SetActive(value: true);
			startGameButton.interactable = false;
		}
		UpdateDemoAvailability();
	}

	public void UpdateDemoAvailability()
	{
	}

	public void OnWaterMerchantClick()
	{
		OnAnyButtonClick();
		title.text = LocHelper.GetLocalizedNewGame("water_merchant");
		body.text = "    " + LocHelper.GetLocalizedNewGame("water_merchant_body");
		startingEquipment.text = " - " + LocHelper.GetLocalizedNewGame("water_merchant_gear") + "\n - " + LocHelper.GetLocalizedNewGame("water_merchant_gear1") + "\n - " + LocHelper.GetLocalizedNewGame("water_merchant_gear2");
		note.text = "";
		comingsoon.SetActive(value: false);
		startGameButton.interactable = true;
		startGameButton.gameObject.SetActive(value: true);
		skipIntroLock.SetActive(value: false);
		NewGameData.Instance.startType = NewGameData.StartType.WATER_MERCHANT;
		ResetAllTab();
		tabWaterMerchant.sprite = tabSelected;
		OnAnyStartClick();
	}

	public void OnGeneralistClick()
	{
		OnAnyButtonClick();
		title.text = LocHelper.GetLocalizedNewGame("generalist");
		body.text = "    " + LocHelper.GetLocalizedNewGame("generalist_body");
		startingEquipment.text = " - " + LocHelper.GetLocalizedNewGame("generalist_gear") + "\n - " + LocHelper.GetLocalizedNewGame("generalist_gear2") + "\n - " + LocHelper.GetLocalizedNewGame("generalist_gear3");
		comingsoon.SetActive(value: false);
		startGameButton.interactable = true;
		startGameButton.gameObject.SetActive(value: true);
		note.text = LocHelper.GetLocalizedNewGame("new_player_hint");
		skipIntroLock.SetActive(value: false);
		NewGameData.Instance.startType = NewGameData.StartType.APPRENTICE;
		ResetAllTab();
		tabGeneralist.sprite = tabSelected;
		OnAnyStartClick();
	}

	public void OnScavengerClick()
	{
		OnAnyButtonClick();
		title.text = LocHelper.GetLocalizedNewGame("junker");
		body.text = "    " + LocHelper.GetLocalizedNewGame("junker_body");
		startingEquipment.text = " - " + LocHelper.GetLocalizedNewGame("junker_gear") + "\n - " + LocHelper.GetLocalizedNewGame("junker_gear2") + "\n - " + LocHelper.GetLocalizedNewGame("junker_gear3");
		comingsoon.SetActive(value: false);
		startGameButton.interactable = true;
		startGameButton.gameObject.SetActive(value: true);
		skipIntroLock.SetActive(value: false);
		note.text = "";
		NewGameData.Instance.startType = NewGameData.StartType.SCAV;
		ResetAllTab();
		tabJunker.sprite = tabSelected;
		OnAnyStartClick();
	}

	public void OnFenceClick()
	{
		OnAnyButtonClick();
		title.text = LocHelper.GetLocalizedNewGame("fence");
		body.text = "   " + LocHelper.GetLocalizedNewGame("fence_body");
		startingEquipment.text = " - " + LocHelper.GetLocalizedNewGame("fence_gear") + "\n - " + LocHelper.GetLocalizedNewGame("fence_gear2") + "\n - " + LocHelper.GetLocalizedNewGame("fence_gear3") + "\n - " + LocHelper.GetLocalizedNewGame("fence_gear4") + "\n - " + LocHelper.GetLocalizedNewGame("fence_gear5") + "\n - " + LocHelper.GetLocalizedNewGame("fence_gear6");
		note.text = "";
		comingsoon.SetActive(value: false);
		startGameButton.interactable = true;
		startGameButton.gameObject.SetActive(value: true);
		skipIntroLock.SetActive(value: true);
		NewGameData.Instance.startType = NewGameData.StartType.FENCE;
		ResetAllTab();
		tabFence.sprite = tabSelected;
		OnAnyStartClick();
	}

	public void OnMoonshinerClick()
	{
		OnAnyButtonClick();
		title.text = LocHelper.GetLocalizedNewGame("moonshiner");
		body.text = "    " + LocHelper.GetLocalizedNewGame("moonshiner_body");
		startingEquipment.text = " - " + LocHelper.GetLocalizedNewGame("moonshiner_gear") + "\n - " + LocHelper.GetLocalizedNewGame("moonshiner_gear2");
		note.text = "";
		comingsoon.SetActive(value: true);
		startGameButton.interactable = false;
		startGameButton.gameObject.SetActive(value: true);
		skipIntroLock.SetActive(value: false);
		NewGameData.Instance.startType = NewGameData.StartType.MOONSHINER;
		ResetAllTab();
		tabFence.sprite = tabSelected;
		OnAnyStartClick();
	}

	public void OnGunsmithClick()
	{
		OnAnyButtonClick();
		title.text = LocHelper.GetLocalizedNewGame("gunsmith");
		body.text = "    " + LocHelper.GetLocalizedNewGame("gunsmith_body");
		startingEquipment.text = " - " + LocHelper.GetLocalizedNewGame("gunsmith_gear") + "\n - " + LocHelper.GetLocalizedNewGame("gunsmith_gear2") + "\n - " + LocHelper.GetLocalizedNewGame("gunsmith_gear3");
		note.text = "";
		comingsoon.SetActive(value: false);
		startGameButton.interactable = true;
		startGameButton.gameObject.SetActive(value: true);
		skipIntroLock.SetActive(value: true);
		note.text = LocHelper.GetLocalizedNewGame("advanced_start");
		NewGameData.Instance.startType = NewGameData.StartType.GUNSMITH;
		ResetAllTab();
		tabGunsmith.sprite = tabSelected;
		OnAnyStartClick();
	}

	public void OnRockBottomClick()
	{
		OnAnyButtonClick();
		title.text = LocHelper.GetLocalizedNewGame("rock_bottom");
		body.text = LocHelper.GetLocalizedNewGame("rock_bottom_body");
		startingEquipment.text = LocHelper.GetLocalizedNewGame("rock_bottom_gear");
		note.text = "";
		comingsoon.SetActive(value: false);
		startGameButton.interactable = true;
		startGameButton.gameObject.SetActive(value: true);
		skipIntroLock.SetActive(value: true);
		note.text = LocHelper.GetLocalizedNewGame("extreme_challenge_start_note");
		NewGameData.Instance.startType = NewGameData.StartType.ROCK_BOTTOM;
		ResetAllTab();
		tabRockBottom.sprite = tabSelected;
		OnAnyStartClick();
	}

	public void OnDrugDealer()
	{
		OnAnyButtonClick();
		title.text = LocHelper.GetLocalizedNewGame("chemist");
		body.text = "    " + LocHelper.GetLocalizedNewGame("chemist_body");
		startingEquipment.text = " - " + LocHelper.GetLocalizedNewGame("chemist_gear") + "\n - " + LocHelper.GetLocalizedNewGame("chemist_gear2");
		note.text = "";
		comingsoon.SetActive(value: false);
		startGameButton.interactable = true;
		startGameButton.gameObject.SetActive(value: true);
		skipIntroLock.SetActive(value: true);
		note.text = LocHelper.GetLocalizedNewGame("advanced_start");
		NewGameData.Instance.startType = NewGameData.StartType.DRUG_DEALER;
		ResetAllTab();
		tabChemist.sprite = tabSelected;
		OnAnyStartClick();
	}

	public void OnOrganTrader()
	{
		OnAnyButtonClick();
		title.text = LocHelper.GetLocalizedNewGame("organ_dealer");
		body.text = "    " + LocHelper.GetLocalizedNewGame("organ_trader_body");
		startingEquipment.text = " - " + LocHelper.GetLocalizedNewGame("organ_trader_gear") + "\n - " + LocHelper.GetLocalizedNewGame("organ_trader_gear2") + "\n - " + LocHelper.GetLocalizedNewGame("organ_trader_gear3");
		note.text = "";
		comingsoon.SetActive(value: true);
		startGameButton.interactable = false;
		startGameButton.gameObject.SetActive(value: true);
		skipIntroLock.SetActive(value: false);
		NewGameData.Instance.startType = NewGameData.StartType.ORGAN_TRADER;
		ResetAllTab();
		tabOrganTrader.sprite = tabSelected;
		OnAnyStartClick();
	}

	public void OnPharmacist()
	{
		OnAnyButtonClick();
		title.text = LocHelper.GetLocalizedNewGame("pharmacist");
		body.text = "    " + LocHelper.GetLocalizedNewGame("pharmacist_body");
		startingEquipment.text = " - " + LocHelper.GetLocalizedNewGame("pharmacist_gear") + "\n - " + LocHelper.GetLocalizedNewGame("pharmacist_gear2") + "\n - " + LocHelper.GetLocalizedNewGame("pharmacist_gear3");
		note.text = "";
		comingsoon.SetActive(value: true);
		startGameButton.interactable = false;
		startGameButton.gameObject.SetActive(value: true);
		skipIntroLock.SetActive(value: false);
		NewGameData.Instance.startType = NewGameData.StartType.PHARMACIST;
		OnAnyStartClick();
	}

	public void OnStreetFood()
	{
		OnAnyButtonClick();
		title.text = LocHelper.GetLocalizedNewGame("street_food_vendor");
		body.text = "    " + LocHelper.GetLocalizedNewGame("street_food_vendor_body");
		startingEquipment.text = " - " + LocHelper.GetLocalizedNewGame("street_food_vendor_gear") + "\n - " + LocHelper.GetLocalizedNewGame("street_food_vendor_gear2") + "\n - " + LocHelper.GetLocalizedNewGame("street_food_vendor_gear3");
		note.text = "";
		comingsoon.SetActive(value: true);
		startGameButton.interactable = false;
		startGameButton.gameObject.SetActive(value: true);
		skipIntroLock.SetActive(value: false);
		NewGameData.Instance.startType = NewGameData.StartType.STREET_FOOD_VENDOR;
		ResetAllTab();
		tabStreetFoodVendor.sprite = tabSelected;
		OnAnyStartClick();
	}

	public void OnFarmer()
	{
		OnAnyButtonClick();
		title.text = LocHelper.GetLocalizedNewGame("hydroponic_farmer");
		body.text = "    " + LocHelper.GetLocalizedNewGame("hydroponic_farmer_body");
		startingEquipment.text = " - " + LocHelper.GetLocalizedNewGame("hydroponic_farmer_gear") + "\n - " + LocHelper.GetLocalizedNewGame("hydroponic_farmer_gear2") + "\n - " + LocHelper.GetLocalizedNewGame("hydroponic_farmer_gear3") + "\n - " + LocHelper.GetLocalizedNewGame("hydroponic_farmer_gear4");
		note.text = LocHelper.GetLocalizedNewGame("advanced_start");
		comingsoon.SetActive(value: false);
		startGameButton.interactable = true;
		startGameButton.gameObject.SetActive(value: true);
		skipIntroLock.SetActive(value: false);
		NewGameData.Instance.startType = NewGameData.StartType.FARMER;
		ResetAllTab();
		tabFarmer.sprite = tabSelected;
		OnAnyStartClick();
	}

	public void OnRancher()
	{
		OnAnyButtonClick();
		title.text = LocHelper.GetLocalizedNewGame("rancher");
		body.text = "    " + LocHelper.GetLocalizedNewGame("rancher_body");
		startingEquipment.text = " - " + LocHelper.GetLocalizedNewGame("rancher_gear1") + "\n - " + LocHelper.GetLocalizedNewGame("rancher_gear2") + "\n - " + LocHelper.GetLocalizedNewGame("rancher_gear3");
		note.text = "";
		comingsoon.SetActive(value: false);
		startGameButton.interactable = true;
		startGameButton.gameObject.SetActive(value: true);
		skipIntroLock.SetActive(value: false);
		NewGameData.Instance.startType = NewGameData.StartType.RANCHER;
		ResetAllTab();
		tabRancher.sprite = tabSelected;
		OnAnyStartClick();
	}

	public void StartBlackscreenSequence(bool playFullSequence = true, Action actionDuringBlack = null, Action actionAfterBlack = null, float waitTime = 0.25f, float sequenceTime = 0.2f)
	{
		StartCoroutine(BlackScreenSequence(playFullSequence, actionDuringBlack, actionAfterBlack, waitTime, sequenceTime));
	}

	private IEnumerator BlackScreenSequence(bool playFullSequence = true, Action actionDuringBlack = null, Action actionAfterBlack = null, float waitTime = 0.25f, float sequenceTime = 0.2f)
	{
		blackscreen.gameObject.SetActive(value: true);
		Color color = new Color(0.16078432f, 2f / 15f, 0.20392157f, 0f);
		blackscreen.color = color;
		float t2 = 0f;
		while (t2 < sequenceTime)
		{
			t2 += Time.deltaTime;
			color.a = Mathf.Clamp01(t2 / sequenceTime);
			blackscreen.color = color;
			yield return null;
		}
		actionDuringBlack?.Invoke();
		if (playFullSequence)
		{
			yield return new WaitForSeconds(waitTime);
			t2 = 0f;
			while (t2 < sequenceTime)
			{
				t2 += Time.deltaTime;
				color.a = Mathf.Clamp01(1f - t2 / sequenceTime);
				blackscreen.color = color;
				yield return null;
			}
			blackscreen.gameObject.SetActive(value: false);
			actionAfterBlack?.Invoke();
		}
	}

	public void OnSkipIntroToggled()
	{
		NewGameData.Instance.skipIntro = skipToggle.isOn;
		Debug.Log("Skip Intro Toggled : " + NewGameData.Instance.skipIntro);
		if (skipToggle.isOn && ES3.Load("totalRunCount", "saves_index.es3", 0) == 0)
		{
			darknessOverlay2.gameObject.SetActive(value: true);
			confirmPanelUIManager.OpenPanel(LocHelper.GetLocalizedMainMenu("skip_intro_first_run_confirm"), delegate
			{
				darknessOverlay2.gameObject.SetActive(value: false);
			}, delegate
			{
				skipToggle.isOn = false;
				darknessOverlay2.gameObject.SetActive(value: false);
			});
		}
	}

	public void OnHardToggled()
	{
		NewGameData.Instance.hardMode = !NewGameData.Instance.hardMode;
		NewGameData.Instance.difficultyId = (NewGameData.Instance.hardMode ? "hard" : "normal");
		Debug.Log("Hardmode Toggled : " + NewGameData.Instance.hardMode);
	}

	public void OnEndlessToggled()
	{
		NewGameData.Instance.endlessMode = !NewGameData.Instance.endlessMode;
		Debug.Log("Endless Mode Toggled : " + NewGameData.Instance.endlessMode);
	}

	public void DiscordLink()
	{
		Application.OpenURL("https://discord.gg/probablystolen");
	}

	public void OnWishlistLink()
	{
		Application.OpenURL("steam://store/4348910");
	}

	public void OnSurveyLink()
	{
		Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLScVUotJhNfnyAlV31wH3j2zln4mc-c5zhjnHEACKdOstk1dOw/viewform");
	}

	public void OnCreatorLink()
	{
		if (!string.IsNullOrEmpty(null))
		{
			Application.OpenURL(null);
		}
	}

	public void OpenCreditPanel()
	{
		OnAnyButtonClick();
		CreditsUI.Open();
	}

	public void CloseCreditPanel()
	{
		CreditsUI.Close();
		OnAnyButtonClick();
	}

	public void OpenWelcomePanel()
	{
		welcomePanelDataConsentToggle.isOn = EmporiumOption.Instance.consentAnalytics;
		welcomePanel.SetActive(value: true);
		OnAnyButtonClick();
	}

	public void CloseWelcomePanel()
	{
		EmporiumOption.Instance.consentAnalytics = welcomePanelDataConsentToggle.isOn;
		EmporiumOption.Instance.ApplySetting(applyResolution: false);
		welcomePanel.SetActive(value: false);
		OnAnyButtonClick();
	}

	public void HandleWelcomePanel()
	{
		if (ES3.KeyExists("welcomeVersion"))
		{
			if (!ES3.Load<string>("welcomeVersion").Equals(PlayerStore.VERSION) && !NewGameData.Instance.isShowcase)
			{
				OpenWelcomePanel();
				ES3.Save("welcomeVersion", PlayerStore.VERSION);
			}
		}
		else
		{
			if (!NewGameData.Instance.isShowcase)
			{
				OpenWelcomePanel();
			}
			ES3.Save("welcomeVersion", PlayerStore.VERSION);
		}
	}

	public void ResetAllTab()
	{
		tabWelcome.sprite = tab;
		tabGeneralist.sprite = tab;
		tabJunker.sprite = tab;
		tabWaterMerchant.sprite = tab;
		tabFence.sprite = tab;
		tabFarmer.sprite = tab;
		tabGunsmith.sprite = tab;
		tabChemist.sprite = tab;
		tabOrganTrader.sprite = tab;
		tabRockBottom.sprite = tab;
		tabStreetFoodVendor.sprite = tab;
		tabRancher.sprite = tab;
	}

	public void OnLocaleChanged(int localeIndex)
	{
		if (localeIndex < 0 || localeIndex >= LOCALE_DROPDOWN.Length)
		{
			Debug.LogWarning("Unsupported language index: " + localeIndex);
			return;
		}
		string item = LOCALE_DROPDOWN[localeIndex].code;
		ChangeLanguage(item);
		ES3.Save("locale", item);
		Debug.Log("Locale changed to: " + item);
	}

	private void ChangeLanguage(string localeCode)
	{
		StartCoroutine(LocHelper.SetLocale(localeCode));
	}

	public void PreLoadLocalizationTable()
	{
		LocalizationSettings.StringDatabase.GetTable("MainMenu");
		LocalizationSettings.StringDatabase.GetTable("NewGame");
	}

	public void HandleLocaleDropdown()
	{
		string text = LocHelper.ResolveStartupLocale();
		List<string> list = new List<string>();
		int num = 0;
		for (int i = 0; i < LOCALE_DROPDOWN.Length; i++)
		{
			list.Add(LOCALE_DROPDOWN[i].label);
			if (LOCALE_DROPDOWN[i].code == text)
			{
				num = i;
			}
		}
		text = LOCALE_DROPDOWN[num].code;
		localeDropdown.ClearOptions();
		localeDropdown.AddOptions(list);
		localeDropdown.SetValueWithoutNotify(num);
		ChangeLanguage(text);
	}

	public void OnPrivacyPolicyClick()
	{
		Application.OpenURL("https://www.tinybuild.com/privacy-policy");
	}

	public void OnJoinDemoClick()
	{
		OnAnyButtonClick();
		Application.OpenURL("steam://store/4349200");
	}
}
