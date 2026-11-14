using Duckov.Economy;
using Saves;
using TMPro;
using UnityEngine;

namespace Duckov.UI;

public class CashDisplay : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private string format = "n0";

	private void Awake()
	{
		ItemUtilities.OnPlayerItemOperation += OnPlayerItemOperation;
		SavesSystem.OnSetFile += OnSaveFileChanged;
		Refresh();
	}

	private void OnDestroy()
	{
		ItemUtilities.OnPlayerItemOperation -= OnPlayerItemOperation;
		SavesSystem.OnSetFile -= OnSaveFileChanged;
	}

	private void OnPlayerItemOperation()
	{
		Refresh();
	}

	private void OnEnable()
	{
		Refresh();
	}

	private void Refresh()
	{
		text.text = EconomyManager.Cash.ToString(format);
	}

	private void OnMoneyChanged(long arg1, long arg2)
	{
		Refresh();
	}

	private void OnSaveFileChanged()
	{
		Refresh();
	}
}
