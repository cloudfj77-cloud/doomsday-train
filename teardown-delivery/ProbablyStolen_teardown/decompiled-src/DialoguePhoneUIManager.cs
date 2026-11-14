using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePhoneUIManager : MonoBehaviour
{
	public Dialogue currentDialog;

	public TextBubble clientBubble;

	public TextBubble playerBubble;

	public GameObject clientNameBubble;

	public Image clientNameBubbleImage;

	public TextMeshProUGUI clientNameTMP;

	public List<DialogChoiceBubble> dialogChoiceBubbles;

	public const string PLAYER = "PLAYER";

	public int isConversationActiveFlag;

	private List<Coroutine> activeCoroutines = new List<Coroutine>();

	private int conversationEpoch;

	public bool isSkipCurrentDialog;

	public bool isWaitingForManualAdvance;

	private bool isAutoAdvancePending;

	private int advanceSerial;

	private bool isEndPending;

	private Dialogue displayingUncompletedDialog;

	private readonly DialogBubbleScaler bubbleScaler = new DialogBubbleScaler();

	public static DialoguePhoneUIManager Instance { get; private set; }

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
		if (!IsBubbleTyping(clientBubble))
		{
			return IsBubbleTyping(playerBubble);
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

	private void StopAllTypewriters()
	{
		clientBubble.typewriter.StopShowingText();
		playerBubble.typewriter.StopShowingText();
	}

	private void SkipTypewriters()
	{
		if (clientBubble.gameObject.activeSelf)
		{
			clientBubble.typewriter.SkipTypewriter();
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
		dialogue?.ExecuteEndAction();
	}

	private void Start()
	{
		bubbleScaler.Register(clientBubble);
		bubbleScaler.Register(clientNameBubble, clientBubble);
		bubbleScaler.Register(playerBubble);
		for (int i = 0; i < dialogChoiceBubbles.Count; i++)
		{
			bubbleScaler.Register(dialogChoiceBubbles[i], (i == 0) ? null : dialogChoiceBubbles[0]);
		}
		ApplyBubbleScale();
		DialogBubbleClickAdvance.Attach(clientBubble.gameObject, OnBubbleClicked);
		DialogBubbleClickAdvance.Attach(playerBubble.gameObject, OnBubbleClicked);
		DialogBubbleClickAdvance.Attach(clientNameBubble, OnBubbleClicked);
	}

	public void ApplyBubbleScale()
	{
		bubbleScaler.Apply();
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

	public Dialogue GetCurrentDialog()
	{
		return currentDialog;
	}

	public void UpdateDialogHistory(string text)
	{
		PlayerStore.Instance.dialogHistory.Enqueue(DialogUIManager.RemoveTag(text));
		if (PlayerStore.Instance.dialogHistory.Count > 100)
		{
			PlayerStore.Instance.dialogHistory.Dequeue();
		}
		if (RecorderUIManager.Instance != null)
		{
			RecorderUIManager.Instance.RefreshUI();
		}
	}

	public void StartDialogue(Dialogue dialogue)
	{
		conversationEpoch++;
		currentDialog = dialogue;
		ClearPendingAdvance();
		if (TutorialUIManager.Instance != null)
		{
			TutorialUIManager.Instance.ShowAdvanceDialogHintIfArmed();
		}
		if (currentDialog.title.Equals("PLAYER"))
		{
			DisplayerPlayerText(currentDialog);
		}
		else
		{
			DisplayClientText(currentDialog);
		}
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
			StartCoroutine(MetaHelper.WaitAndDo(1.1f, delegate
			{
				Debug.Log("Phone End Action Executed!");
				currentDialogTemp.ExecuteEndAction();
			}));
		}
		if (IsChoice())
		{
			isSkipCurrentDialog = false;
			ScheduleFlow(DialogUIManager.ScaleByDialogSpeed(0.85f) + DialogUIManager.GetTypewriterDelay(), DisplayChoice);
		}
		else if (!IsCurrentDialogEnd())
		{
			if (isSkipCurrentDialog)
			{
				isSkipCurrentDialog = false;
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
				ScheduleAutoAdvance(DialogUIManager.ScaleByDialogSpeed(0.99f) + DialogUIManager.GetTypewriterDelay());
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
			ScheduleFlow(DialogUIManager.ScaleByDialogSpeed(1f) + DialogUIManager.GetTypewriterDelay(), EndConversation);
		}
	}

	public bool IsCurrentDialogEnd()
	{
		if (currentDialog == null)
		{
			return true;
		}
		if (currentDialog.nextDialogue == null)
		{
			return true;
		}
		return false;
	}

	public void ContinueConversation()
	{
		ClearPendingAdvance();
		if (currentDialog == null)
		{
			currentDialog = null;
			return;
		}
		if (currentDialog == displayingUncompletedDialog)
		{
			displayingUncompletedDialog = null;
			currentDialog.ExecuteEndAction();
		}
		currentDialog = currentDialog.nextDialogue;
		if (currentDialog != null && currentDialog.startAction != null)
		{
			Debug.Log("Start Action Executed!");
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
		else
		{
			DisplayClientText(currentDialog);
		}
	}

	public void ResumeIfWaitingForAdvance()
	{
		if (isWaitingForManualAdvance && !IsCurrentDialogEnd() && !IsChoice() && !clientBubble.typewriter.isShowingText && !playerBubble.typewriter.isShowingText)
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
		if (currentDialog == null)
		{
			Debug.Log("Conversation Ended!");
			clientBubble.gameObject.SetActive(value: false);
			playerBubble.gameObject.SetActive(value: false);
			clientNameBubble.SetActive(value: false);
		}
	}

	public bool IsChoice()
	{
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
			LocGenderHelper.ResolveDialogueLine(currentDialog.choices[i].Item2, LocGenderHelper.LocGender.Neutral);
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
		LocGenderHelper.ResolveDialogueLine(dialogue, LocGenderHelper.LocGender.Neutral);
		StopAllTypewriters();
		clientBubble.gameObject.SetActive(value: false);
		playerBubble.gameObject.SetActive(value: false);
		clientBubble.typewriter.StopShowingText();
		if (PhoneUIManager.Instance.currentPhoneClient != null)
		{
			clientNameBubble.SetActive(value: true);
			clientNameTMP.text = "<i>" + PhoneUIManager.Instance.currentPhoneClient.GetLocalizedDisplayName() + "</i>";
			UpdateDialogHistory("<color=#eb4034>" + PhoneUIManager.Instance.currentPhoneClient.GetLocalizedDisplayName() + ":</color> " + dialogue.plainText);
		}
		else
		{
			clientNameTMP.gameObject.SetActive(value: false);
			UpdateDialogHistory("<color=#eb4034>Customer :</color> " + dialogue.plainText);
		}
		clientBubble.typewriter.hideAppearancesOnSkip = false;
		clientBubble.gameObject.SetActive(value: true);
		clientBubble.typewriter.ShowText(dialogue.plainText);
		SetClientBubbleColor();
		if (!EmporiumOption.Instance.enableTypewriterEffect)
		{
			clientBubble.typewriter.hideAppearancesOnSkip = true;
			clientBubble.typewriter.SkipTypewriter();
		}
	}

	public void SetClientBubbleColor()
	{
		if (PhoneUIManager.Instance.currentPhoneClient != null)
		{
			string text = "";
			if (StoreUIManager.Instance.dialogBubbleList.IsContainKey(text))
			{
				clientBubble.backgroundImage.sprite = StoreUIManager.Instance.dialogBubbleList.GetSprite(text + "p");
				clientNameBubbleImage.sprite = StoreUIManager.Instance.dialogBubbleList.GetSprite(text);
			}
			else
			{
				clientNameBubbleImage.sprite = StoreUIManager.Instance.dialogBubbleList.GetSprite("default");
				clientBubble.backgroundImage.sprite = StoreUIManager.Instance.dialogBubbleList.GetSprite("defaultp");
			}
		}
		else
		{
			clientNameBubbleImage.sprite = StoreUIManager.Instance.dialogBubbleList.GetSprite("default");
			clientBubble.backgroundImage.sprite = StoreUIManager.Instance.dialogBubbleList.GetSprite("defaultp");
		}
	}

	public void DisplayerPlayerText(Dialogue dialogue)
	{
		displayingUncompletedDialog = dialogue;
		LocGenderHelper.ResolveDialogueLine(dialogue, LocGenderHelper.LocGender.Neutral);
		StopAllTypewriters();
		clientBubble.gameObject.SetActive(value: false);
		playerBubble.gameObject.SetActive(value: false);
		clientNameBubble.SetActive(value: false);
		playerBubble.typewriter.hideAppearancesOnSkip = false;
		playerBubble.gameObject.SetActive(value: true);
		playerBubble.typewriter.ShowText(dialogue.plainText);
		UpdateDialogHistory("<color=#6ecf5f>Player:</color> " + dialogue.plainText);
		if (!EmporiumOption.Instance.enableTypewriterEffect)
		{
			playerBubble.typewriter.hideAppearancesOnSkip = true;
			playerBubble.typewriter.SkipTypewriter();
		}
	}

	public void PlayClientSingleText(string text)
	{
		clientBubble.gameObject.SetActive(value: true);
		clientBubble.typewriter.ShowText(text);
		clientNameBubble.SetActive(value: true);
		clientNameTMP.text = PlayerStore.Instance.currentClientInstance.GetClientBlueprint().displayName;
	}

	public void PlayPlayerSingleText()
	{
	}

	public void CurrentClientLeave(float time = 1f)
	{
		StartCoroutine(MetaHelper.WaitAndDo(time, delegate
		{
			PlayerStore.Instance.DismissCurrentClient(callNextWhenPossible: false);
		}));
	}

	public void InterruptDialog()
	{
		conversationEpoch++;
		ClearPendingAdvance();
		displayingUncompletedDialog = null;
		clientBubble.typewriter.StopShowingText();
		playerBubble.typewriter.StopShowingText();
		currentDialog = null;
		clientBubble.gameObject.SetActive(value: false);
		clientNameBubble.gameObject.SetActive(value: false);
		playerBubble.gameObject.SetActive(value: false);
		foreach (DialogChoiceBubble dialogChoiceBubble in dialogChoiceBubbles)
		{
			dialogChoiceBubble.gameObject.SetActive(value: false);
		}
		StopAllTrackedDialogueCoroutines();
	}

	public void StopAllTrackedDialogueCoroutines()
	{
		foreach (Coroutine activeCoroutine in activeCoroutines)
		{
			if (activeCoroutine != null)
			{
				StopCoroutine(activeCoroutine);
			}
		}
		activeCoroutines.Clear();
	}
}
