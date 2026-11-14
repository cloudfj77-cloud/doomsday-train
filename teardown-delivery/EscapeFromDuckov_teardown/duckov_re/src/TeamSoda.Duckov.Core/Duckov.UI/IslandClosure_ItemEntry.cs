using Cysharp.Threading.Tasks;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.UI;

public class IslandClosure_ItemEntry : MonoBehaviour
{
	[SerializeField]
	private Image icon;

	[SerializeField]
	private PunchReceiver punch;

	internal async UniTask Show(Item item)
	{
		icon.sprite = item.Icon;
		base.gameObject.SetActive(value: true);
		punch.Punch();
	}
}
