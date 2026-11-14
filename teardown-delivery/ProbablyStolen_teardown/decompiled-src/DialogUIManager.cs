using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogUIManager : MonoBehaviour
{
	public Dialogue currentDialog;

	public TextBubble clientBubble;

	public TextBubble playerBubble;

	public TextBubble secBubble;

	public GameObject clientNameBubble;

	public GameObject secNameBubble;

	public Image clientNameBubbleImage;

	public TextMeshProUGUI clientNameTMP;

	public List<DialogChoiceBubble> dialogChoiceBubbles;

	public const string PLAYER = "PLAYER";

	public const string SEC = "SEC";

	public int isConversationActiveFlag;

	public bool isSkipCurrentDialog;

	public bool isWaitingForManualAdvance;

	private bool isAutoAdvancePending;

	private int advanceSerial;

	private bool isEndPending;

	private Dialogue displayingUncompletedDialog;

	public List<Coroutine> activeCoroutines = new List<Coroutine>();

	private readonly List<Dialogue> scheduledEndActionLines = new List<Dialogue>();

	private int conversationEpoch;

	private float defaultModeY;

	private float defaultModeYName;

	private float shootingModeY = 54f;

	private float shootingModeYName = 70f;

	private readonly DialogBubbleScaler bubbleScaler = new DialogBubbleScaler();

	public bool isSecMonologuePlaying;

	private Dialogue pausedDialog;

	private bool pausedLineWasDisplayed;

	public static DialogUIManager Instance { get; private set; }

	public bool HasPausedConversation => pausedDialog != null;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private void ScheduleFlow(float delay, Action action)
	{
		int epoch = conversationEpoch;
		activeCoroutines.Add(StartCoroutine(MetaHelper.WaitAndDo(delay, delegate
		{
			if (epoch == conversationEpoch)
			{
				action();
			}
		})));
	}

	private void ScheduleAutoAdvance(float delay)
	{
		isAutoAdvancePending = true;
		int serial = advanceSerial;
		ScheduleFlow(delay, delegate
		{
			if (serial == advanceSerial)
			{
				ContinueConversation();
			}
		});
	}

	private void ClearPendingAdvance()
	{
		advanceSerial++;
		isAutoAdvancePending = false;
		isWaitingForManualAdvance = false;
		isEndPending = false;
		isSkipCurrentDialog = false;
	}

	private bool IsAnyBubbleTyping()
	{
		if (!IsBubbleTyping(clientBubble) && !IsBubbleTyping(playerBubble))
		{
			return IsBubbleTyping(secBubble);
		}
		return true;
	}

	private static bool IsBubbleTyping(TextBubble bubble)
	{
		if (bubble.gameObject.activeInHierarchy)
		{
			return bubble.typewriter.isShowingText;
		}
		return false;
	}

	private void SkipTypewriters()
	{
		if (clientBubble.gameObject.activeSelf)
		{
			clientBubble.typewriter.SkipTypewriter();
		}
		if (secBubble.gameObject.activeSelf)
		{
			secBubble.typewriter.SkipTypewriter();
		}
		if (playerBubble.gameObject.activeSelf)
		{
			playerBubble.typewriter.SkipTypewriter();
		}
	}

	public void OnAdvancePressed()
	{
		if (IsAnyBubbleTyping())
		{
			if (currentDialog != null && !IsChoice())
			{
				isSkipCurrentDialog = true;
			}
			SkipTypewriters();
		}
		else if (currentDialog != null && !IsChoice())
		{
			if (isEndPending)
			{
				FinishConversationNow();
			}
			else if (isWaitingForManualAdvance || isAutoAdvancePending)
			{
				ContinueConversation();
			}
		}
	}

	private void FinishConversationNow()
	{
		Dialogue dialogue = currentDialog;
		EndConversation();
		if (dialogue != null && dialogue.endAction != null)
		{
			dialogue.ExecuteEndAction();
			CheckOwned();
		}
	}

	public void ApplyBubbleScale()
	{
		bubbleScaler.Apply();
	}

	private void Start()
	{
		defaultModeY = clientBubble.transform.localPosition.y;
		defaultModeYName = clientNameBubble.transform.localPosition.y;
		bubbleScaler.Register(clientBubble);
		bubbleScaler.Register(clientNameBubble, clientBubble);
		bubbleScaler.Register(secBubble);
		bubbleScaler.Register(secNameBubble, secBubble);
		bubbleScaler.Register(playerBubble);
		for (int i = 0; i < dialogChoiceBubbles.Count; i++)
		{
			bubbleScaler.Register(dialogChoiceBubbles[i], (i == 0) ? null : dialogChoiceBubbles[0]);
		}
		ApplyBubbleScale();
		DialogBubbleClickAdvance.Attach(clientBubble.gameObject, OnBubbleClicked);
		DialogBubbleClickAdvance.Attach(playerBubble.gameObject, OnBubbleClicked);
		DialogBubbleClickAdvance.Attach(secBubble.gameObject, OnBubbleClicked);
		DialogBubbleClickAdvance.Attach(clientNameBubble, OnBubbleClicked);
		DialogBubbleClickAdvance.Attach(secNameBubble, OnBubbleClicked);
	}

	private void OnBubbleClicked()
	{
		if (TutorialUIManager.Instance != null)
		{
			TutorialUIManager.Instance.DismissAdvanceDialogHint();
		}
		OnAdvancePressed();
	}

	private void Update()
	{
	}

	public void UpdateDialogHistory(string text)
	{
		PlayerStore.Instance.dialogHistory.Enqueue(RemoveTag(text));
		if (PlayerStore.Instance.dialogHistory.Count > 100)
		{
			PlayerStore.Instance.dialogHistory.Dequeue();
		}
		if (RecorderUIManager.Instance != null)
		{
			RecorderUIManager.Instance.RefreshUI();
		}
	}

	public static string RemoveTag(string text)
	{
		string pattern = "<(?!/?(?:b|u|i)\\b)[^>]*>";
		return Regex.Replace(text, pattern, "");
	}

	public bool StartDialogue(Dialogue dialogue, bool forcedStart = false)
	{
		if (dialogue == null)
		{
			return false;
		}
		if (currentDialog != null)
		{
			bool flag = currentDialog.priority >= dialogue.priority;
			if (flag && isWaitingForManualAdvance && !HasPendingChainConsequence(currentDialog))
			{
				flag = false;
			}
			if (flag && !forcedStart)
			{
				return false;
			}
		}
		if (currentDialog != null && currentDialog == displayingUncompletedDialog)
		{
			displayingUncompletedDialog = null;
			currentDialog.ExecuteEndAction();
		}
		conversationEpoch++;
		currentDialog = dialogue;
		StallWatchdog.Crumb("dialogue " + StallWatchdog.Short(dialogue.plainText));
		ClearPendingAdvance();
		if (TutorialUIManager.Instance != null)
		{
			TutorialUIManager.Instance.ShowAdvanceDialogHintIfArmed();
		}
		if (currentDialog != null && currentDialog.startAction != null)
		{
			Debug.Log("Start Action Executed!");
			currentDialog.startAction();
		}
		if (currentDialog.title.Equals("PLAYER"))
		{
			DisplayerPlayerText(currentDialog);
		}
		else if (currentDialog.title.Equals("SEC"))
		{
			DisplaySecText(currentDialog);
		}
		else
		{
			DisplayClientText(currentDialog);
		}
		return true;
	}

	private static bool HasPendingChainConsequence(Dialogue dialogue)
	{
		for (Dialogue nextDialogue = dialogue.nextDialogue; nextDialogue != null; nextDialogue = nextDialogue.nextDialogue)
		{
			if (nextDialogue.startAction != null)
			{
				return true;
			}
			if (nextDialogue.endAction != null && !nextDialogue.isEndActionExecuted)
			{
				return true;
			}
			if (nextDialogue.choices.Count > 0)
			{
				return true;
			}
		}
		return false;
	}

	public void CheckOwned()
	{
		EmporiumEntry.Instance.RemoveNonOwnedItem();
	}

	public void OnCurrentTextDisplayed()
	{
		displayingUncompletedDialog = null;
		if (currentDialog == null)
		{
			return;
		}
		if (currentDialog.endAction != null)
		{
			_ = currentDialog.endAction;
			Dialogue currentDialogTemp = currentDialog;
			scheduledEndActionLines.Add(currentDialogTemp);
			StartCoroutine(MetaHelper.WaitAndDo(currentDialogTemp.endActionDelay, delegate
			{
				scheduledEndActionLines.Remove(currentDialogTemp);
				Debug.Log("End Action Executed!");
				if (currentDialog == currentDialogTemp && currentDialogTemp.nextDialogue == null && currentDialogTemp.choices.Count == 0)
				{
					EndConversation();
				}
				currentDialogTemp.ExecuteEndAction();
				CheckOwned();
			}));
		}
		if (IsChoice())
		{
			isSkipCurrentDialog = false;
			ScheduleFlow(ScaleByDialogSpeed(0.85f) + GetTypewriterDelay(), DisplayChoice);
		}
		else if (!IsCurrentDialogEnd())
		{
			if (isSkipCurrentDialog)
			{
				isSkipCurrentDialog = false;
				currentDialog.ExecuteEndAction();
				if (EmporiumOption.Instance.autoNextDialog)
				{
					ScheduleAutoAdvance(0.01f);
				}
				else
				{
					isWaitingForManualAdvance = true;
				}
			}
			else if (EmporiumOption.Instance.autoNextDialog)
			{
				ScheduleAutoAdvance(ScaleByDialogSpeed(0.99f) + GetTypewriterDelay());
			}
			else
			{
				isWaitingForManualAdvance = true;
			}
		}
		else
		{
			isSkipCurrentDialog = false;
			isEndPending = true;
			ScheduleFlow(ScaleByDialogSpeed(1f) + GetTypewriterDelay(), EndConversation);
		}
	}

	public static float ScaleByDialogSpeed(float basePause)
	{
		float num = Mathf.Clamp01(EmporiumOption.Instance.dialogSpeed);
		return basePause * (1.5f - num);
	}

	public static float GetTypewriterDelay()
	{
		if (!EmporiumOption.Instance.enableTypewriterEffect)
		{
			return ScaleByDialogSpeed(2f);
		}
		return 0f;
	}

	public bool IsLineStillDisplaying(Dialogue line)
	{
		if (line != null && currentDialog == line)
		{
			return displayingUncompletedDialog == line;
		}
		return false;
	}

	public bool IsCurrentDialogEnd()
	{
		if (currentDialog == null || currentDialog.nextDialogue == null)
		{
			return true;
		}
		return false;
	}

	public void ContinueConversation()
	{
		ClearPendingAdvance();
		if (!isSecMonologuePlaying && (currentDialog == null || PlayerStore.Instance.currentClientInstance == null || StoreUIManager.Instance.isShot))
		{
			RemoveCurrentDialog();
			return;
		}
		if (currentDialog != null && currentDialog == displayingUncompletedDialog)
		{
			displayingUncompletedDialog = null;
			currentDialog.ExecuteEndAction();
		}
		currentDialog = currentDialog.nextDialogue;
		if (currentDialog != null && currentDialog.startAction != null)
		{
			currentDialog.startAction();
		}
		if (currentDialog == null)
		{
			EndConversation();
		}
		else if (currentDialog.title.Equals("PLAYER"))
		{
			DisplayerPlayerText(currentDialog);
		}
		else if (currentDialog.title.Equals("SEC"))
		{
			DisplaySecText(currentDialog);
		}
		else
		{
			DisplayClientText(currentDialog);
		}
	}

	public void ResumeIfWaitingForAdvance()
	{
		if (isWaitingForManualAdvance && !IsChoice() && !IsCurrentDialogEnd() && !clientBubble.typewriter.isShowingText && !playerBubble.typewriter.isShowingText && !secBubble.typewriter.isShowingText)
		{
			if (isSkipCurrentDialog)
			{
				isSkipCurrentDialog = false;
				currentDialog.ExecuteEndAction();
			}
			ContinueConversation();
		}
	}

	public void EndConversation()
	{
		conversationEpoch++;
		ClearPendingAdvance();
		Debug.Log("<color=blue>Conversation Ended!</color>");
		HideBubbles();
		HideChoiceBubble();
		currentDialog = null;
	}

	public bool IsChoice()
	{
		if (currentDialog == null)
		{
			return false;
		}
		if (currentDialog.choices.Count != 0)
		{
			return true;
		}
		return false;
	}

	public void DisplayChoice()
	{
		for (int i = 0; i < currentDialog.choices.Count; i++)
		{
			dialogChoiceBubbles[i].gameObject.SetActive(value: true);
			LocGenderHelper.ResolveDialogueLine(currentDialog.choices[i].Item2);
			dialogChoiceBubbles[i].TMP.text = currentDialog.choices[i].Item2.plainText;
			dialogChoiceBubbles[i].SetInteractable(currentDialog.IsChoiceEnabledAt(i));
		}
	}

	public void HideChoiceBubble()
	{
		foreach (DialogChoiceBubble dialogChoiceBubble in dialogChoiceBubbles)
		{
			dialogChoiceBubble.gameObject.SetActive(value: false);
		}
	}

	public void HideAllDialogUI()
	{
		HideChoiceBubble();
		clientBubble.gameObject.SetActive(value: false);
		playerBubble.gameObject.SetActive(value: false);
		clientNameBubble.SetActive(value: false);
	}

	public void SelectChoice(int index)
	{
		Debug.Log("Choice Selected");
		if (currentDialog == null || index >= currentDialog.choices.Count)
		{
			HideChoiceBubble();
		}
		else if (currentDialog.IsChoiceEnabledAt(index))
		{
			if (currentDialog.choices[index].Item3 != null)
			{
				Debug.Log("Choice Action Executed!");
				currentDialog.choices[index].Item3();
			}
			currentDialog = currentDialog.choices[index].Item2;
			HideChoiceBubble();
			ScheduleFlow(0.5f, ContinueConversation);
		}
	}

	public void DisplayClientText(Dialogue dialogue)
	{
		displayingUncompletedDialog = dialogue;
		LocGenderHelper.ResolveDialogueLine(dialogue);
		HideBubbles();
		clientBubble.typewriter.StopShowingText();
		if (PlayerStore.Instance.currentClientInstance != null)
		{
			clientNameBubble.SetActive(value: true);
			clientNameTMP.text = PlayerStore.Instance.currentClientInstance.GetClientBlueprint().displayName;
			UpdateDialogHistory("<color=#eb4034>" + PlayerStore.Instance.currentClientInstance.GetClientBlueprint().displayName + ":</color> " + dialogue.plainText);
		}
		else
		{
			clientNameBubble.SetActive(value: false);
			UpdateDialogHistory("<color=#eb4034>Customer :</color> " + dialogue.plainText);
		}
		clientBubble.typewriter.hideAppearancesOnSkip = false;
		clientBubble.gameObject.SetActive(value: true);
		clientBubble.typewriter.ShowText(dialogue.plainText);
		if (!EmporiumOption.Instance.enableTypewriterEffect)
		{
			clientBubble.typewriter.hideAppearancesOnSkip = true;
			clientBubble.typewriter.SkipTypewriter();
		}
		SetClientBubbleColor();
	}

	public void DisplaySecText(Dialogue dialogue)
	{
		displayingUncompletedDialog = dialogue;
		LocGenderHelper.ResolveDialogueLine(dialogue);
		clientBubble.typewriter.StopShowingText();
		HideBubbles();
		if (PlayerStore.Instance.currentClientInstance != null)
		{
			clientNameTMP.text = PlayerStore.Instance.currentClientInstance.GetClientBlueprint().displayName;
			UpdateDialogHistory("<color=#eb4034>" + LocHelper.GetLocalizedClientName("name_security_officer") + ":</color> " + dialogue.plainText);
		}
		else
		{
			clientNameBubble.SetActive(value: false);
			UpdateDialogHistory("<color=#eb4034>Customer :</color> " + dialogue.plainText);
		}
		secBubble.typewriter.hideAppearancesOnSkip = false;
		secNameBubble.gameObject.SetActive(value: true);
		secBubble.gameObject.SetActive(value: true);
		secBubble.typewriter.ShowText(dialogue.plainText);
		SetClientBubbleColor();
		if (!EmporiumOption.Instance.enableTypewriterEffect)
		{
			secBubble.typewriter.hideAppearancesOnSkip = true;
			secBubble.typewriter.SkipTypewriter();
		}
	}

	public void SetClientBubbleColor()
	{
		if (PlayerStore.Instance.currentClientInstance != null)
		{
			string bubbleColor = PlayerStore.Instance.currentClientInstance.GetClientBlueprint().bubbleColor;
			StoreClient clientBlueprint = PlayerStore.Instance.currentClientInstance.GetClientBlueprint();
			if (bubbleColor == null || bubbleColor.Equals("") || bubbleColor.Equals("DEFAULT"))
			{
				if (clientBlueprint.clientFaction.Equals("FACTION_SECURITY"))
				{
					SetColor("sec");
				}
				else if (clientBlueprint.clientFaction.Equals("FACTION_BLACK_MARKET"))
				{
					SetColor("dark");
				}
				else if (clientBlueprint.clientFaction.Equals("FACTION_REVOLUTION"))
				{
					SetColor("greenrev");
				}
				else if (clientBlueprint.clientFaction.Equals("FACTION_UPPER_LEVEL"))
				{
					SetColor("upper");
				}
				else if (clientBlueprint.clientFaction.Equals("FACTION_TOURIST"))
				{
					SetColor("upper");
				}
				else
				{
					SetColor("default");
				}
			}
			else if (StoreUIManager.Instance.dialogBubbleList.IsContainKey(bubbleColor))
			{
				clientBubble.backgroundImage.sprite = StoreUIManager.Instance.dialogBubbleList.GetSprite(bubbleColor + "p");
				clientNameBubbleImage.sprite = StoreUIManager.Instance.dialogBubbleList.GetSprite(bubbleColor);
			}
			else
			{
				clientNameBubbleImage.sprite = StoreUIManager.Instance.dialogBubbleList.GetSprite("default");
				clientBubble.backgroundImage.sprite = StoreUIManager.Instance.dialogBubbleList.GetSprite("defaultp");
			}
		}
		else
		{
			SetColor("default");
		}
	}

	private void SetColor(string color)
	{
		clientBubble.backgroundImage.sprite = StoreUIManager.Instance.dialogBubbleList.GetSprite(color + "p");
		clientNameBubbleImage.sprite = StoreUIManager.Instance.dialogBubbleList.GetSprite(color);
	}

	public void DisplayerPlayerText(Dialogue dialogue)
	{
		displayingUncompletedDialog = dialogue;
		LocGenderHelper.ResolveDialogueLine(dialogue);
		playerBubble.typewriter.StopShowingText();
		HideBubbles();
		playerBubble.gameObject.SetActive(value: true);
		playerBubble.typewriter.hideAppearancesOnSkip = false;
		playerBubble.typewriter.ShowText(dialogue.plainText);
		if (!EmporiumOption.Instance.enableTypewriterEffect)
		{
			playerBubble.typewriter.hideAppearancesOnSkip = true;
			playerBubble.typewriter.SkipTypewriter();
		}
		UpdateDialogHistory("<color=#6ecf5f>Player:</color> " + dialogue.plainText);
	}

	public void HideBubbles()
	{
		StopAllTypewriters();
		clientNameBubble.SetActive(value: false);
		secNameBubble.SetActive(value: false);
		clientBubble.gameObject.SetActive(value: false);
		playerBubble.gameObject.SetActive(value: false);
		secBubble.gameObject.SetActive(value: false);
	}

	private void StopAllTypewriters()
	{
		clientBubble.typewriter.StopShowingText();
		playerBubble.typewriter.StopShowingText();
		secBubble.typewriter.StopShowingText();
	}

	public void PlayClientSingleText(string text)
	{
		clientBubble.gameObject.SetActive(value: true);
		clientBubble.typewriter.ShowText(LocGenderHelper.ResolveForCurrentClient(text));
		clientNameBubble.SetActive(value: true);
		clientNameTMP.text = PlayerStore.Instance.currentClientInstance.GetClientBlueprint().displayName;
	}

	public void PlayPlayerSingleText()
	{
	}

	public void CurrentClientLeave(float time = 1f)
	{
		if (time.Equals(0f))
		{
			PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
			return;
		}
		StartCoroutine(MetaHelper.WaitAndDo(time, delegate
		{
			PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
		}));
	}

	public void FlushScheduledEndActions()
	{
		if (scheduledEndActionLines.Count == 0)
		{
			return;
		}
		List<Dialogue> list = new List<Dialogue>(scheduledEndActionLines);
		scheduledEndActionLines.Clear();
		foreach (Dialogue item in list)
		{
			if (currentDialog == item && item.nextDialogue == null && item.choices.Count == 0)
			{
				EndConversation();
			}
			item.ExecuteEndAction();
		}
		CheckOwned();
	}

	public void PauseConversation()
	{
		pausedDialog = currentDialog;
		pausedLineWasDisplayed = pausedDialog != null && displayingUncompletedDialog != pausedDialog;
		InterruptDialog();
		HideChoiceBubble();
	}

	public void ClearPausedConversation()
	{
		pausedDialog = null;
	}

	public bool ResumeConversation()
	{
		Dialogue dialogue = pausedDialog;
		bool flag = pausedLineWasDisplayed;
		pausedDialog = null;
		if (dialogue == null || PlayerStore.Instance.currentClientInstance == null)
		{
			return false;
		}
		InterruptDialog();
		conversationEpoch++;
		currentDialog = dialogue;
		ClearPendingAdvance();
		if (flag)
		{
			if (dialogue.choices.Count > 0)
			{
				DisplayLine(dialogue);
				return true;
			}
			ContinueConversation();
			return true;
		}
		DisplayLine(dialogue);
		return true;
	}

	private void DisplayLine(Dialogue line)
	{
		if (line.title.Equals("PLAYER"))
		{
			DisplayerPlayerText(line);
		}
		else if (line.title.Equals("SEC"))
		{
			DisplaySecText(line);
		}
		else
		{
			DisplayClientText(line);
		}
	}

	public void InterruptDialog()
	{
		conversationEpoch++;
		if (currentDialog != null && currentDialog == displayingUncompletedDialog)
		{
			displayingUncompletedDialog = null;
			currentDialog.ExecuteEndAction();
			CheckOwned();
		}
		ClearPendingAdvance();
		displayingUncompletedDialog = null;
		clientBubble.typewriter.StopShowingText();
		playerBubble.typewriter.StopShowingText();
		secBubble.typewriter.StopShowingText();
		RemoveCurrentDialog();
		clientBubble.gameObject.SetActive(value: false);
		clientNameBubble.gameObject.SetActive(value: false);
		secBubble.gameObject.SetActive(value: false);
		secNameBubble.gameObject.SetActive(value: false);
		playerBubble.gameObject.SetActive(value: false);
		StopAllTrackedDialogueCoroutines();
	}

	public void RemoveCurrentDialog()
	{
		Debug.Log("Current Dialog Removed");
		currentDialog = null;
	}

	public void RemoveCustomerDialogue()
	{
		if (PlayerStore.Instance.currentClientInstance != null && PlayerStore.Instance.currentClientInstance.GetClientBlueprint().mainDialogue != null)
		{
			PlayerStore.Instance.currentClientInstance.GetClientBlueprint().mainDialogue = null;
		}
	}

	public void SetCurrentDialog(Dialogue dialogue)
	{
		currentDialog = dialogue;
	}

	public Dialogue GetCurrentDialog()
	{
		return currentDialog;
	}

	public void StopAllTrackedDialogueCoroutines()
	{
		if (activeCoroutines.Count > 0)
		{
			Debug.Log("Dialog Correctly Ended thanks to Stop coroutine Function");
		}
		foreach (Coroutine activeCoroutine in activeCoroutines)
		{
			if (activeCoroutine != null)
			{
				StopCoroutine(activeCoroutine);
			}
		}
		activeCoroutines.Clear();
	}

	public static void PlayQuickDialogue(int priority, string speaker, string text)
	{
		Dialogue dialogue = new Dialogue();
		dialogue.SetText(speaker, text);
		dialogue.priority = priority;
		Instance.StartDialogue(dialogue);
	}

	public void SetNormalMode()
	{
		clientBubble.transform.localPosition = new Vector3(clientBubble.transform.localPosition.x, defaultModeY, clientBubble.transform.localPosition.z);
		clientNameBubble.transform.localPosition = new Vector3(clientNameBubble.transform.localPosition.x, defaultModeYName, clientNameBubble.transform.localPosition.z);
		ApplyBubbleScale();
	}

	public void SetShootingMode()
	{
		clientBubble.transform.localPosition = new Vector3(clientBubble.transform.localPosition.x, shootingModeY, clientBubble.transform.localPosition.z);
		clientNameBubble.transform.localPosition = new Vector3(clientNameBubble.transform.localPosition.x, shootingModeYName, clientNameBubble.transform.localPosition.z);
		ApplyBubbleScale();
	}
}
