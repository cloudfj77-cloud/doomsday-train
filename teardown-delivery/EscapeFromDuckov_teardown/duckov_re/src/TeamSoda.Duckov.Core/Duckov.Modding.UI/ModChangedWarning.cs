using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.Modding.UI;

public class ModChangedWarning : MonoBehaviour
{
	[SerializeField]
	private FadeGroup fadeGroup;

	[SerializeField]
	private Button continueButton;

	[SerializeField]
	private Button cancelButton;

	[SerializeField]
	private TextMeshProUGUI contentText;

	private bool continueBtnClicked;

	private bool cancelBtnClicked;

	private bool checking;

	private void Awake()
	{
		continueButton.onClick.AddListener(OnContinueButtonClicked);
		cancelButton.onClick.AddListener(OnCancelButtonClicked);
	}

	private void OnContinueButtonClicked()
	{
		continueBtnClicked = true;
	}

	private void OnCancelButtonClicked()
	{
		cancelBtnClicked = true;
	}

	public async UniTask<bool> Check()
	{
		if (checking)
		{
			return false;
		}
		checking = true;
		List<string> list = ModManager.LoadLastActiveModList();
		List<string> currentActiveModList = ModManager.GetCurrentActiveModList();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			string item = list[num];
			if (currentActiveModList.Remove(item))
			{
				list.RemoveAt(num);
			}
		}
		int count = currentActiveModList.Count;
		int count2 = list.Count;
		if (count <= 0 && count2 <= 0)
		{
			checking = false;
			return true;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string item2 in list)
		{
			stringBuilder.AppendLine("<color=#F85566>- " + item2 + "</color>");
		}
		foreach (string item3 in currentActiveModList)
		{
			stringBuilder.AppendLine("<color=#FFE999>+ " + item3 + "</color>");
		}
		contentText.text = stringBuilder.ToString();
		continueBtnClicked = false;
		cancelBtnClicked = false;
		fadeGroup.Show();
		while (!continueBtnClicked && !cancelBtnClicked)
		{
			await UniTask.Yield();
		}
		fadeGroup.Hide();
		checking = false;
		if (cancelBtnClicked)
		{
			return false;
		}
		_ = continueBtnClicked;
		return true;
	}
}
