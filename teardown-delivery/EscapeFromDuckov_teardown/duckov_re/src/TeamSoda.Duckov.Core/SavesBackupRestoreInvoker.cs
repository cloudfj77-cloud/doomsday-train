using System.Collections.Generic;
using Duckov.UI.Animations;
using Duckov.UI.SavesRestore;
using Saves;
using UnityEngine;
using UnityEngine.UI;

public class SavesBackupRestoreInvoker : MonoBehaviour
{
	[SerializeField]
	private Button mainButton;

	[SerializeField]
	private FadeGroup menuFadeGroup;

	[SerializeField]
	private List<Button> buttons;

	[SerializeField]
	private SavesBackupRestorePanel restorePanel;

	private void Awake()
	{
		mainButton.onClick.AddListener(OnMainButtonClicked);
		for (int i = 0; i < buttons.Count; i++)
		{
			Button button = buttons[i];
			int slot = i + 1;
			button.onClick.AddListener(delegate
			{
				OnButtonClicked(slot);
			});
		}
	}

	private void OnMainButtonClicked()
	{
		menuFadeGroup.Toggle();
	}

	private void OnButtonClicked(int index)
	{
		menuFadeGroup.Hide();
		SavesSystem.SetFile(index);
		restorePanel.Open(index);
	}
}
