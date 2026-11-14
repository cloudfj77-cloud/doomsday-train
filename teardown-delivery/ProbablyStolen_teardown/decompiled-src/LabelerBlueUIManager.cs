using System;
using TMPro;
using UnityEngine;

public class LabelerBlueUIManager : MonoBehaviour
{
	public GameObject panel;

	public TMP_InputField inputField;

	private GameItem curItem;

	private Action<string> onTextDone;

	private Action onTextClear;

	private int itemCharacterLimit = -1;

	public static LabelerBlueUIManager Instance { get; private set; }

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

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OpenUI(string initial, Action<string> onDone, Action onClear, int characterLimit = 0)
	{
		panel.SetActive(value: true);
		curItem = null;
		onTextDone = onDone;
		onTextClear = onClear;
		if (itemCharacterLimit < 0)
		{
			itemCharacterLimit = inputField.characterLimit;
		}
		inputField.characterLimit = ((characterLimit > 0) ? characterLimit : itemCharacterLimit);
		inputField.text = initial ?? "";
		inputField.Select();
		inputField.ActivateInputField();
		StartEdit();
	}

	public void OpenUI(GameItem gameItem)
	{
		panel.SetActive(value: true);
		curItem = gameItem;
		onTextDone = null;
		onTextClear = null;
		if (itemCharacterLimit >= 0)
		{
			inputField.characterLimit = itemCharacterLimit;
		}
		if (GeneralHelper.HasCustomName(gameItem))
		{
			inputField.text = GeneralHelper.GetCustomName(gameItem);
		}
		else
		{
			inputField.text = "";
		}
		inputField.Select();
		inputField.ActivateInputField();
		StartEdit();
	}

	public void CloseUI()
	{
		EmporiumAudioPlayer.Instance.Play();
		panel.SetActive(value: false);
		curItem = null;
		onTextDone = null;
		onTextClear = null;
	}

	public void OnClearButtonClick()
	{
		inputField.text = "";
		if (curItem == null)
		{
			onTextClear?.Invoke();
			return;
		}
		GeneralHelper.RemoveCustomName(curItem);
		RefreshOpenWindowTitle();
	}

	public void OnEditEnd(string value)
	{
		StoreUIManager.Instance.StopTyping();
		if (curItem == null)
		{
			onTextDone?.Invoke(inputField.text);
			return;
		}
		GeneralHelper.SetCustomName(curItem, inputField.text);
		RefreshOpenWindowTitle();
	}

	private void RefreshOpenWindowTitle()
	{
		if (curItem != null && curItem.contentWindow != null && curItem.contentWindow.IsVisible())
		{
			curItem.contentWindow.SetTitle(curItem.GetDisplayName(preferCustomName: true));
		}
	}

	public void StartEdit()
	{
		StoreUIManager.Instance.StartTyping();
	}
}
