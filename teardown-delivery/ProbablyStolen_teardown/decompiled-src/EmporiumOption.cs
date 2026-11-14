using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmporiumOption : MonoBehaviour
{
	public float masterVolume = 1f;

	public float musicVolume = 0.5f;

	public float backgroundVolume = 0.5f;

	public int fullscreenMode;

	public int resolution;

	public int borderlessDivisor = 1;

	public float dialogSpeed = 1f;

	public float dialogVolume = 1f;

	public bool enableTypewriterEffect = true;

	public bool autoNextDialog = true;

	public bool autoCallNext;

	public bool optionSave;

	public bool hasWishlistRecommended;

	public bool consentAnalytics;

	public bool fpsCapWhileMinimized;

	public bool restoreDisplayAfterFocus = true;

	public bool enableFpsCap = true;

	public bool instantDismissCustomer;

	public bool instantCallSecurity;

	public bool instantCallNextCustomer;

	public bool swapDragRotateFlip;

	public bool disableSortMenu;

	public bool sortAutoRotate;

	public bool singleClickInteract;

	public bool singleClickActivate;

	public bool autoOpenNegotiationUI = true;

	public bool moveBoughtItemsToInventory = true;

	public bool newspaperToTrash;

	public bool disablePopCultureClients;

	public bool singleClickChoice = true;

	public bool showModSourceInTooltip = true;

	public bool showModSourceInRecorder = true;

	public const int SAVE_HISTORY_DAYS_DEFAULT = 3;

	public const int SAVE_HISTORY_DAYS_MAX = 10;

	public int saveHistoryDays = 3;

	public const float DIALOG_BUBBLE_SCALE_MIN = 0.5f;

	public const float DIALOG_BUBBLE_SCALE_MAX = 1.5f;

	public const float DIALOG_BUBBLE_SCALE_STEP = 0.1f;

	public float dialogBubbleScale = 1f;

	private const string FPS_CAP_IS_VSYNC_KEY = "fpsCapIsVsync";

	private const int BACKGROUND_FPS_CAP = 10;

	private bool appHasFocus = true;

	private bool recoveringDisplay;

	private float lastDisplayRecovery = -100f;

	private const float DISPLAY_RECOVERY_COOLDOWN = 3f;

	private const int DISPLAY_RECOVERY_SETTLE_FRAMES = 5;

	public static EmporiumOption Instance { get; private set; }

	public static int SaveHistoryDays
	{
		get
		{
			if (!(Instance == null))
			{
				return Mathf.Clamp(Instance.saveHistoryDays, 1, 10);
			}
			return 3;
		}
	}

	public static float DialogBubbleScale
	{
		get
		{
			if (!(Instance == null))
			{
				return Mathf.Clamp(Instance.dialogBubbleScale, 0.5f, 1.5f);
			}
			return 1f;
		}
	}

	public static bool SortMenuDisabled
	{
		get
		{
			if (Instance != null)
			{
				return Instance.disableSortMenu;
			}
			return false;
		}
	}

	public static bool SingleClickInteract
	{
		get
		{
			if (Instance != null)
			{
				return Instance.singleClickInteract;
			}
			return false;
		}
	}

	public static bool SingleClickActivate
	{
		get
		{
			if (Instance != null)
			{
				return Instance.singleClickActivate;
			}
			return false;
		}
	}

	public static bool AutoOpenNegotiationUI
	{
		get
		{
			if (!(Instance == null))
			{
				return Instance.autoOpenNegotiationUI;
			}
			return true;
		}
	}

	public static bool MoveBoughtItemsToInventory
	{
		get
		{
			if (!(Instance == null))
			{
				return Instance.moveBoughtItemsToInventory;
			}
			return true;
		}
	}

	public static bool NewspaperToTrash
	{
		get
		{
			if (Instance != null)
			{
				return Instance.newspaperToTrash;
			}
			return false;
		}
	}

	public static bool DisablePopCultureClients
	{
		get
		{
			if (Instance != null)
			{
				return Instance.disablePopCultureClients;
			}
			return false;
		}
	}

	public static bool SingleClickChoice
	{
		get
		{
			if (!(Instance == null))
			{
				return Instance.singleClickChoice;
			}
			return true;
		}
	}

	public static bool ShowModSourceInTooltip
	{
		get
		{
			if (!(Instance == null))
			{
				return Instance.showModSourceInTooltip;
			}
			return true;
		}
	}

	public static bool ShowModSourceInRecorder
	{
		get
		{
			if (!(Instance == null))
			{
				return Instance.showModSourceInRecorder;
			}
			return true;
		}
	}

	public static bool SortAutoRotate
	{
		get
		{
			if (Instance != null)
			{
				return Instance.sortAutoRotate;
			}
			return false;
		}
	}

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void ApplyDialogBubbleScale()
	{
		if (DialogUIManager.Instance != null)
		{
			DialogUIManager.Instance.ApplyBubbleScale();
		}
		if (DialoguePhoneUIManager.Instance != null)
		{
			DialoguePhoneUIManager.Instance.ApplyBubbleScale();
		}
	}

	private void Start()
	{
		if (!(Instance != this))
		{
			SetDefault();
			ApplySetting();
		}
	}

	private void Update()
	{
	}

	public void SetDefault()
	{
		bool flag = !ES3.KeyExists("optionSave");
		if (ES3.KeyExists("optionSave"))
		{
			Debug.Log("Option loaded");
			optionSave = ES3.Load<bool>("optionSave");
		}
		else
		{
			Debug.Log("Option NOT loaded");
			optionSave = false;
		}
		if (ES3.KeyExists("masterVolume"))
		{
			masterVolume = ES3.Load<float>("masterVolume");
		}
		else
		{
			masterVolume = 0.5f;
		}
		if (ES3.KeyExists("musicVolume"))
		{
			musicVolume = ES3.Load<float>("musicVolume");
		}
		else
		{
			musicVolume = 0.5f;
		}
		if (ES3.KeyExists("backgroundVolume"))
		{
			backgroundVolume = ES3.Load<float>("backgroundVolume");
		}
		else
		{
			backgroundVolume = 0.5f;
		}
		if (ES3.KeyExists("resolution"))
		{
			resolution = ES3.Load<int>("resolution");
		}
		else
		{
			resolution = GameOptionUIManager.MapResolution();
		}
		if (ES3.KeyExists("fullscreenMode"))
		{
			fullscreenMode = ES3.Load<int>("fullscreenMode");
		}
		else
		{
			fullscreenMode = 1;
		}
		if (ES3.KeyExists("borderlessDivisor"))
		{
			borderlessDivisor = ES3.Load<int>("borderlessDivisor");
		}
		else
		{
			borderlessDivisor = 1;
		}
		ClampResolutionToAvailable();
		if (ES3.KeyExists("dialogSpeed"))
		{
			dialogSpeed = ES3.Load<float>("dialogSpeed");
		}
		else
		{
			dialogSpeed = 0.5f;
		}
		if (ES3.KeyExists("dialogVolume"))
		{
			dialogVolume = ES3.Load<float>("dialogVolume");
		}
		else
		{
			dialogVolume = 0.5f;
		}
		if (ES3.KeyExists("enableTypewriterEffect"))
		{
			enableTypewriterEffect = ES3.Load<bool>("enableTypewriterEffect");
		}
		else
		{
			enableTypewriterEffect = true;
		}
		if (ES3.KeyExists("autoNextDialog"))
		{
			autoNextDialog = ES3.Load<bool>("autoNextDialog");
		}
		else
		{
			autoNextDialog = true;
		}
		if (ES3.KeyExists("dialogBubbleScale"))
		{
			dialogBubbleScale = ES3.Load<float>("dialogBubbleScale");
		}
		else
		{
			dialogBubbleScale = 1f;
		}
		if (ES3.KeyExists("autoCallNext"))
		{
			autoCallNext = ES3.Load<bool>("autoCallNext");
		}
		else
		{
			autoCallNext = false;
		}
		if (ES3.KeyExists("hasWishlistRecommended"))
		{
			hasWishlistRecommended = ES3.Load<bool>("hasWishlistRecommended");
		}
		else
		{
			hasWishlistRecommended = false;
		}
		if (ES3.KeyExists("consentAnalytics"))
		{
			consentAnalytics = ES3.Load<bool>("consentAnalytics");
		}
		else
		{
			consentAnalytics = false;
		}
		if (ES3.KeyExists("fpsCapWhileMinimized"))
		{
			fpsCapWhileMinimized = ES3.Load<bool>("fpsCapWhileMinimized");
		}
		else
		{
			fpsCapWhileMinimized = false;
		}
		if (ES3.KeyExists("restoreDisplayAfterFocus"))
		{
			restoreDisplayAfterFocus = ES3.Load<bool>("restoreDisplayAfterFocus");
		}
		else
		{
			restoreDisplayAfterFocus = true;
		}
		if (ES3.KeyExists("enableFpsCap"))
		{
			enableFpsCap = ES3.Load<bool>("enableFpsCap");
		}
		else
		{
			enableFpsCap = true;
		}
		if (!ES3.KeyExists("fpsCapIsVsync"))
		{
			enableFpsCap = true;
		}
		if (ES3.KeyExists("instantDismissCustomer"))
		{
			instantDismissCustomer = ES3.Load<bool>("instantDismissCustomer");
		}
		else
		{
			instantDismissCustomer = flag;
		}
		if (ES3.KeyExists("instantCallSecurity"))
		{
			instantCallSecurity = ES3.Load<bool>("instantCallSecurity");
		}
		else
		{
			instantCallSecurity = false;
		}
		if (ES3.KeyExists("instantCallNextCustomer"))
		{
			instantCallNextCustomer = ES3.Load<bool>("instantCallNextCustomer");
		}
		else
		{
			instantCallNextCustomer = flag;
		}
		if (ES3.KeyExists("swapDragRotateFlip"))
		{
			swapDragRotateFlip = ES3.Load<bool>("swapDragRotateFlip");
		}
		else
		{
			swapDragRotateFlip = false;
		}
		if (ES3.KeyExists("disableSortMenu"))
		{
			disableSortMenu = ES3.Load<bool>("disableSortMenu");
		}
		else
		{
			disableSortMenu = false;
		}
		if (ES3.KeyExists("sortAutoRotate"))
		{
			sortAutoRotate = ES3.Load<bool>("sortAutoRotate");
		}
		else
		{
			sortAutoRotate = false;
		}
		if (ES3.KeyExists("singleClickInteract"))
		{
			singleClickInteract = ES3.Load<bool>("singleClickInteract");
		}
		else
		{
			singleClickInteract = false;
		}
		if (ES3.KeyExists("singleClickActivate"))
		{
			singleClickActivate = ES3.Load<bool>("singleClickActivate");
		}
		else
		{
			singleClickActivate = false;
		}
		if (ES3.KeyExists("autoOpenNegotiationUI"))
		{
			autoOpenNegotiationUI = ES3.Load<bool>("autoOpenNegotiationUI");
		}
		else
		{
			autoOpenNegotiationUI = true;
		}
		if (ES3.KeyExists("moveBoughtItemsToInventory"))
		{
			moveBoughtItemsToInventory = ES3.Load<bool>("moveBoughtItemsToInventory");
		}
		else
		{
			moveBoughtItemsToInventory = true;
		}
		if (ES3.KeyExists("newspaperToTrash"))
		{
			newspaperToTrash = ES3.Load<bool>("newspaperToTrash");
		}
		else
		{
			newspaperToTrash = false;
		}
		if (ES3.KeyExists("disablePopCultureClients"))
		{
			disablePopCultureClients = ES3.Load<bool>("disablePopCultureClients");
		}
		else
		{
			disablePopCultureClients = false;
		}
		if (ES3.KeyExists("singleClickChoice"))
		{
			singleClickChoice = ES3.Load<bool>("singleClickChoice");
		}
		else
		{
			singleClickChoice = true;
		}
		if (ES3.KeyExists("saveHistoryDays"))
		{
			saveHistoryDays = ES3.Load<int>("saveHistoryDays");
		}
		else
		{
			saveHistoryDays = 3;
		}
		if (ES3.KeyExists("showModSourceInTooltip"))
		{
			showModSourceInTooltip = ES3.Load<bool>("showModSourceInTooltip");
		}
		else
		{
			showModSourceInTooltip = true;
		}
		if (ES3.KeyExists("showModSourceInRecorder"))
		{
			showModSourceInRecorder = ES3.Load<bool>("showModSourceInRecorder");
		}
		else
		{
			showModSourceInRecorder = true;
		}
	}

	public bool ClampResolutionToAvailable()
	{
		if (fullscreenMode != 2 && !IsMonitor16by9())
		{
			if (GetBorderlessDivisors().Contains(borderlessDivisor))
			{
				return false;
			}
			borderlessDivisor = 1;
			return true;
		}
		GetResolutionDimensions(resolution, out var width, out var height, out var _);
		if (IsResolutionAvailableForMode(width, height, fullscreenMode))
		{
			return false;
		}
		resolution = GetClosestWindowResolution();
		return true;
	}

	public void SyncDisplayStateFromScreen()
	{
		switch (Screen.fullScreenMode)
		{
		case FullScreenMode.ExclusiveFullScreen:
			fullscreenMode = 0;
			break;
		case FullScreenMode.Windowed:
			fullscreenMode = 2;
			break;
		default:
			fullscreenMode = 1;
			break;
		}
		int resolutionValueFromDimensions = GetResolutionValueFromDimensions(Screen.width, Screen.height);
		if (resolutionValueFromDimensions >= 0)
		{
			resolution = resolutionValueFromDimensions;
		}
		if (fullscreenMode == 1 && !IsMonitor16by9())
		{
			int borderlessDivisorFromDimensions = GetBorderlessDivisorFromDimensions(Screen.width, Screen.height);
			if (borderlessDivisorFromDimensions > 0)
			{
				borderlessDivisor = borderlessDivisorFromDimensions;
			}
		}
	}

	public static int GetResolutionValueFromDimensions(int width, int height)
	{
		if (width == 1920 && height == 1080)
		{
			return 0;
		}
		if (width == 960 && height == 540)
		{
			return 1;
		}
		if (width == 3840 && height == 2160)
		{
			return 2;
		}
		if (width == 1280 && height == 720)
		{
			return 3;
		}
		if (width == 1366 && height == 768)
		{
			return 4;
		}
		if (width == 1600 && height == 900)
		{
			return 5;
		}
		if (width == 2560 && height == 1440)
		{
			return 6;
		}
		return -1;
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		appHasFocus = hasFocus;
		StallWatchdog.Crumb("focus " + hasFocus + " at " + Screen.width + "x" + Screen.height);
		if (!Application.isEditor)
		{
			ApplyFpsCap();
		}
		if (hasFocus && !Application.isEditor)
		{
			StartCoroutine(RecoverDisplayAfterFocus());
		}
	}

	private IEnumerator RecoverDisplayAfterFocus()
	{
		if (recoveringDisplay)
		{
			yield break;
		}
		recoveringDisplay = true;
		int w = Screen.width;
		int h = Screen.height;
		int stable = 0;
		while (stable < 5 && appHasFocus)
		{
			yield return null;
			if (Screen.width == w && Screen.height == h)
			{
				stable++;
				continue;
			}
			w = Screen.width;
			h = Screen.height;
			stable = 0;
		}
		recoveringDisplay = false;
		if (!appHasFocus || fullscreenMode == 2)
		{
			yield break;
		}
		ResolveTargetDisplay(out var targetW, out var targetH, out var pixelPerfect);
		if (Screen.width == targetW && Screen.height == targetH)
		{
			if (EmporiumEntry.Instance != null && EmporiumEntry.Instance.pixelPerfectCamera != null && EmporiumEntry.Instance.pixelPerfectCamera.enabled != pixelPerfect)
			{
				ApplyPixelPerfectCamera();
			}
		}
		else if (Screen.fullScreenMode != FullScreenMode.Windowed || !restoreDisplayAfterFocus)
		{
			Debug.Log("[Display] " + Screen.width + "x" + Screen.height + " " + Screen.fullScreenMode.ToString() + " after focus, configured " + targetW + "x" + targetH + ", left alone" + (restoreDisplayAfterFocus ? "" : " (restore option off)"));
		}
		else if (!(Time.unscaledTime - lastDisplayRecovery < 3f))
		{
			lastDisplayRecovery = Time.unscaledTime;
			FullScreenMode fullScreenMode = ConfiguredFullScreenMode();
			Debug.Log("[Display] Restoring " + targetW + "x" + targetH + " " + fullScreenMode.ToString() + " after focus (was " + Screen.width + "x" + Screen.height + " " + Screen.fullScreenMode.ToString() + ")");
			StallWatchdog.Crumb("display restore " + targetW + "x" + targetH + " " + fullScreenMode);
			Screen.SetResolution(targetW, targetH, fullScreenMode);
			ApplyPixelPerfectCamera(pixelPerfect);
			StartCoroutine(WaitForResolutionThenAdjust(targetW, targetH));
		}
	}

	private FullScreenMode ConfiguredFullScreenMode()
	{
		if (fullscreenMode == 0)
		{
			return FullScreenMode.ExclusiveFullScreen;
		}
		if (fullscreenMode == 1)
		{
			return FullScreenMode.FullScreenWindow;
		}
		return FullScreenMode.Windowed;
	}

	public void ApplyFpsCap()
	{
		if (!appHasFocus && !Application.isEditor && fpsCapWhileMinimized)
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 10;
		}
		else if (enableFpsCap)
		{
			QualitySettings.vSyncCount = 1;
			Application.targetFrameRate = -1;
		}
		else
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = -1;
		}
	}

	public void OnArriveGameScene()
	{
		if (StoreUIManager.Instance != null)
		{
			StoreUIManager.Instance.dialogSpeed = dialogSpeed * 2f + 0.2f;
		}
		else
		{
			Debug.Log("Dialog Speed Not Set");
		}
	}

	public void ApplySetting(bool applyResolution = true)
	{
		if (applyResolution)
		{
			FullScreenMode fullScreenMode = ConfiguredFullScreenMode();
			ResolveTargetDisplay(out var targetW, out var targetH, out var pixelPerfect);
			Debug.Log(targetW + "x" + targetH);
			StallWatchdog.Crumb("display apply " + targetW + "x" + targetH + " " + fullScreenMode);
			Screen.SetResolution(targetW, targetH, fullScreenMode);
			ApplyPixelPerfectCamera(pixelPerfect);
			StartCoroutine(WaitForResolutionThenAdjust(targetW, targetH));
		}
		AudioListener.volume = masterVolume;
		ApplyFpsCap();
		if (StoreSoundPlayer.Instance != null)
		{
			Debug.Log("Setting Applied in store scene");
			StoreSoundPlayer.Instance.typewriterAudioSource.volume = dialogVolume;
		}
		ApplyMusicVolume();
		ApplyBackgroundVolume();
		ApplyDialogBubbleScale();
		if (StoreUIManager.Instance != null)
		{
			StoreUIManager.Instance.dialogSpeed = dialogSpeed * 2f + 0.2f;
		}
		ES3.Save("masterVolume", masterVolume);
		ES3.Save("musicVolume", musicVolume);
		ES3.Save("backgroundVolume", backgroundVolume);
		ES3.Save("fullscreenMode", fullscreenMode);
		ES3.Save("resolution", resolution);
		ES3.Save("borderlessDivisor", borderlessDivisor);
		ES3.Save("dialogSpeed", dialogSpeed);
		ES3.Save("dialogVolume", dialogVolume);
		ES3.Save("enableTypewriterEffect", enableTypewriterEffect);
		ES3.Save("autoNextDialog", autoNextDialog);
		ES3.Save("dialogBubbleScale", dialogBubbleScale);
		ES3.Save("autoCallNext", autoCallNext);
		ES3.Save("fpsCapWhileMinimized", fpsCapWhileMinimized);
		ES3.Save("restoreDisplayAfterFocus", restoreDisplayAfterFocus);
		ES3.Save("enableFpsCap", enableFpsCap);
		ES3.Save("fpsCapIsVsync", value: true);
		ES3.Save("instantDismissCustomer", instantDismissCustomer);
		ES3.Save("instantCallSecurity", instantCallSecurity);
		ES3.Save("instantCallNextCustomer", instantCallNextCustomer);
		ES3.Save("swapDragRotateFlip", swapDragRotateFlip);
		ES3.Save("disableSortMenu", disableSortMenu);
		ES3.Save("sortAutoRotate", sortAutoRotate);
		ES3.Save("singleClickInteract", singleClickInteract);
		ES3.Save("singleClickActivate", singleClickActivate);
		ES3.Save("autoOpenNegotiationUI", autoOpenNegotiationUI);
		ES3.Save("moveBoughtItemsToInventory", moveBoughtItemsToInventory);
		ES3.Save("newspaperToTrash", newspaperToTrash);
		ES3.Save("disablePopCultureClients", disablePopCultureClients);
		ES3.Save("singleClickChoice", singleClickChoice);
		ES3.Save("saveHistoryDays", saveHistoryDays);
		ES3.Save("showModSourceInTooltip", showModSourceInTooltip);
		ES3.Save("showModSourceInRecorder", showModSourceInRecorder);
		ES3.Save("consentAnalytics", consentAnalytics);
		KeyBindHelper.Save();
		if (consentAnalytics)
		{
			EmporiumAnalytics.InitAnalytics();
		}
		else
		{
			EmporiumAnalytics.StopAnalytics();
		}
		ES3.Save("optionSave", value: true);
		Debug.Log("Option Saved");
	}

	private void ResolveTargetDisplay(out int targetW, out int targetH, out bool pixelPerfect)
	{
		GetResolutionDimensions(resolution, out targetW, out targetH, out pixelPerfect);
		if (fullscreenMode != 2 && !IsMonitor16by9())
		{
			int num = ((fullscreenMode != 1 || !GetBorderlessDivisors().Contains(borderlessDivisor)) ? 1 : borderlessDivisor);
			targetW = Display.main.systemWidth / num;
			targetH = Display.main.systemHeight / num;
			pixelPerfect = false;
		}
	}

	public static List<int> GetBorderlessDivisors()
	{
		int systemWidth = Display.main.systemWidth;
		int systemHeight = Display.main.systemHeight;
		List<int> list = new List<int> { 1 };
		for (int i = 2; i <= 8; i++)
		{
			if (systemWidth % i == 0 && systemHeight % i == 0)
			{
				int num = systemWidth / i;
				if (Mathf.Min(systemHeight / i, num * 9 / 16) < 1080)
				{
					break;
				}
				list.Add(i);
			}
		}
		return list;
	}

	public static int GetBorderlessDivisorFromDimensions(int width, int height)
	{
		if (width <= 0 || height <= 0)
		{
			return -1;
		}
		int systemWidth = Display.main.systemWidth;
		int systemHeight = Display.main.systemHeight;
		if (systemWidth % width != 0 || systemHeight % height != 0 || systemWidth / width != systemHeight / height)
		{
			return -1;
		}
		return systemWidth / width;
	}

	private static void ApplyPixelPerfectCamera(bool pixelPerfect)
	{
		if (EmporiumEntry.Instance != null)
		{
			EmporiumEntry.Instance.pixelPerfectCamera.enabled = pixelPerfect;
		}
	}

	public void ApplyPixelPerfectCamera()
	{
		ResolveTargetDisplay(out var _, out var _, out var pixelPerfect);
		ApplyPixelPerfectCamera(pixelPerfect);
		if (EmporiumEntry.Instance != null && EmporiumEntry.Instance.outliner != null)
		{
			EmporiumEntry.Instance.outliner.enabled = false;
			EmporiumEntry.Instance.outliner.enabled = true;
		}
	}

	public void OnAutoAdvanceSettingApplied(bool wasEnabled)
	{
		if (autoNextDialog && !wasEnabled)
		{
			ResumeDialogIfAutoAdvanceEnabled();
		}
		else if (wasEnabled && !autoNextDialog && TutorialUIManager.Instance != null)
		{
			TutorialUIManager.Instance.ArmAdvanceDialogHint();
		}
	}

	public void ResumeDialogIfAutoAdvanceEnabled()
	{
		if (autoNextDialog)
		{
			if (DialogUIManager.Instance != null)
			{
				DialogUIManager.Instance.ResumeIfWaitingForAdvance();
			}
			if (DialoguePhoneUIManager.Instance != null)
			{
				DialoguePhoneUIManager.Instance.ResumeIfWaitingForAdvance();
			}
		}
	}

	public void SaveMusicVolume()
	{
		ES3.Save("musicVolume", musicVolume);
	}

	public void ApplyMusicVolume()
	{
		if (StoreSoundPlayer.Instance != null)
		{
			StoreSoundPlayer.Instance.musicAudioSource.volume = musicVolume;
		}
		if (MainMenuUIController.Instance != null && MainMenuUIController.Instance.themeSource != null)
		{
			MainMenuUIController.Instance.themeSource.volume = musicVolume;
		}
	}

	public void ApplyBackgroundVolume()
	{
		if (StoreSoundPlayer.Instance != null && StoreSoundPlayer.Instance.ambianceAudioSource != null)
		{
			StoreSoundPlayer.Instance.ambianceAudioSource.volume = backgroundVolume;
		}
		if (AudioManager.Exists && AudioManager.Instance.audioSourceAmbient != null)
		{
			AudioManager.Instance.audioSourceAmbient.volume = backgroundVolume;
		}
	}

	public static bool IsResolutionAvailableForMode(int width, int height, int fullscreenMode)
	{
		switch (fullscreenMode)
		{
		case 1:
			return true;
		case 2:
			if (width <= Display.main.systemWidth)
			{
				return height <= Display.main.systemHeight;
			}
			return false;
		default:
		{
			Resolution[] resolutions = Screen.resolutions;
			for (int i = 0; i < resolutions.Length; i++)
			{
				Resolution resolution = resolutions[i];
				if (resolution.width == width && resolution.height == height)
				{
					return true;
				}
			}
			if (width <= Display.main.systemWidth)
			{
				return height <= Display.main.systemHeight;
			}
			return false;
		}
		}
	}

	public static bool IsMonitor16by9(float tolerance = 0.05f)
	{
		float num = (float)Display.main.systemWidth / (float)Display.main.systemHeight;
		bool result = Mathf.Abs(num - 1.7777778f) <= tolerance;
		Debug.Log("Monitor Aspect Ratio: " + num + ", Is 16:9: " + result);
		return result;
	}

	private static void GetResolutionDimensions(int resValue, out int width, out int height, out bool pixelPerfect)
	{
		switch (resValue)
		{
		case 1:
			width = 960;
			height = 540;
			pixelPerfect = true;
			break;
		case 2:
			width = 3840;
			height = 2160;
			pixelPerfect = true;
			break;
		case 3:
			width = 1280;
			height = 720;
			pixelPerfect = false;
			break;
		case 4:
			width = 1366;
			height = 768;
			pixelPerfect = false;
			break;
		case 5:
			width = 1600;
			height = 900;
			pixelPerfect = false;
			break;
		case 6:
			width = 2560;
			height = 1440;
			pixelPerfect = false;
			break;
		default:
			width = 1920;
			height = 1080;
			pixelPerfect = true;
			break;
		}
	}

	private IEnumerator WaitForResolutionThenAdjust(int targetW, int targetH)
	{
		for (int j = 0; j < 30; j++)
		{
			if (Screen.width == targetW && Screen.height == targetH)
			{
				break;
			}
			yield return null;
		}
		if (Screen.width != targetW || Screen.height != targetH)
		{
			Screen.SetResolution(targetW, targetH, Screen.fullScreenMode);
			for (int j = 0; j < 30; j++)
			{
				if (Screen.width == targetW && Screen.height == targetH)
				{
					break;
				}
				yield return null;
			}
		}
		LetterboxManager.Refresh();
		ApplyFpsCap();
		if (!(EmporiumEntry.Instance == null))
		{
			EmporiumEntry.Instance.SetAspectRatio();
			EmporiumEntry.Instance.Validate();
			EmporiumEntry.Instance.outliner.enabled = false;
			EmporiumEntry.Instance.outliner.enabled = true;
		}
	}

	public static int GetClosestWindowResolution()
	{
		int systemWidth = Display.main.systemWidth;
		int systemHeight = Display.main.systemHeight;
		if (systemWidth >= 3840 && systemHeight >= 2160)
		{
			return 2;
		}
		if (systemWidth >= 2560 && systemHeight >= 1440)
		{
			return 6;
		}
		if (systemWidth >= 1920 && systemHeight >= 1080)
		{
			return 0;
		}
		if (systemWidth >= 1600 && systemHeight >= 900)
		{
			return 5;
		}
		if (systemWidth >= 1366 && systemHeight >= 768)
		{
			return 4;
		}
		if (systemWidth >= 1280 && systemHeight >= 720)
		{
			return 3;
		}
		return 1;
	}
}
