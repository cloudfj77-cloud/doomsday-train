using Duckov.Buffs;
using Duckov.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.UI;

public class ExtraBuffViewEntry : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[SerializeField]
	private TextMeshProUGUI displayName;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private TextMeshProUGUI layersCount;

	public ExtraBuffView Master { get; private set; }

	public Buff target { get; private set; }

	internal void Setup(ExtraBuffView master, Buff buff)
	{
		Master = master;
		target = buff;
		if (buff == null)
		{
			SetupEmpty();
			Debug.LogError("[Extra Buff View] No buff feeded in the view entry.");
		}
		else
		{
			icon.sprite = buff.Icon;
			displayName.text = buff.DisplayName;
			layersCount.text = ((buff.CurrentLayers > 1) ? $"{buff.CurrentLayers}" : "");
		}
	}

	private void SetupEmpty()
	{
		icon.sprite = GameplayDataSettings.UIStyle.FallbackItemIcon;
		displayName.text = "?";
		layersCount.text = "";
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Master.NotifyPointerEnter(this);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Master.NotifyPointerExit(this);
	}
}
