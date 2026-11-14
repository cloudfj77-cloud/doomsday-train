using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotElement : MonoBehaviour
{
	public TextMeshProUGUI titleTMP;

	public TextMeshProUGUI infoTMP;

	public TextMeshProUGUI dateTMP;

	public TextMeshProUGUI versionTMP;

	public Image icon;

	public int slotId;

	public string startLocale;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnInit()
	{
	}

	public void OnLoadClick()
	{
		if (!ES3.FileExists(SaveFiles.Slot(slotId)))
		{
			SaveFiles.DeleteSlot(slotId);
			MainMenuUIController.Instance.saveUIManager.OpenUI();
			MainMenuUIController.Instance.confirmPanelUIManager.OpenPanel("Save file not found. The entry has been removed.", null, null, hideNoButton: true);
			MainMenuUIController.Instance.OnAnyButtonClick();
			return;
		}
		string currentLocaleCode = LocHelper.GetCurrentLocaleCode();
		if (!string.IsNullOrEmpty(startLocale) && startLocale != currentLocaleCode)
		{
			string localizedMainMenu = LocHelper.GetLocalizedMainMenu("load_locale_mismatch_warning", startLocale, currentLocaleCode);
			MainMenuUIController.Instance.confirmPanelUIManager.OpenPanel(localizedMainMenu, LoadSlot);
			MainMenuUIController.Instance.OnAnyButtonClick();
		}
		else
		{
			LoadSlot();
		}
	}

	private void LoadSlot()
	{
		NewGameData.Instance.saveSlot = slotId;
		MainMenuUIController.Instance.saveUIManager.CloseUI();
		MainMenuUIController.Instance.StartNewGameClick();
	}

	public void OnDeleteClick()
	{
		Action yesAction = delegate
		{
			SaveFiles.DeleteSlot(slotId);
			MainMenuUIController.Instance.saveUIManager.OpenUI();
			MainMenuUIController.Instance.OnAnyButtonClick();
		};
		MainMenuUIController.Instance.confirmPanelUIManager.OpenPanel(LocHelper.GetLocalizedUI("ui_delete_save"), yesAction);
		MainMenuUIController.Instance.OnAnyButtonClick();
	}
}
