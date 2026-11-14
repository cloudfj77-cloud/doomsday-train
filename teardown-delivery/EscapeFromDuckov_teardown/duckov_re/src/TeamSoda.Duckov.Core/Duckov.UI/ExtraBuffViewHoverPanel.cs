using Duckov.Buffs;
using Duckov.UI.Animations;
using TMPro;
using UnityEngine;

namespace Duckov.UI;

public class ExtraBuffViewHoverPanel : MonoBehaviour
{
	[SerializeField]
	private FadeGroup fadeGroup;

	[SerializeField]
	private TextMeshProUGUI displayName;

	[SerializeField]
	private TextMeshProUGUI layer;

	[SerializeField]
	private TextMeshProUGUI description;

	private ExtraBuffViewEntry target;

	internal void NotifyEnter(ExtraBuffViewEntry extraBuffViewEntry)
	{
		target = extraBuffViewEntry;
		if (!(target == null) && !(target.target == null))
		{
			Refresh();
			SkipShow();
		}
	}

	private void FixedUpdate()
	{
		if (target == null || target.target == null)
		{
			SkipHide();
		}
		else
		{
			Refresh();
		}
	}

	private void Refresh()
	{
		if (!(target == null) && !(target.target == null))
		{
			Buff buff = target.target;
			displayName.text = buff.DisplayName;
			layer.text = ((buff.CurrentLayers > 1) ? $"{buff.CurrentLayers}" : "");
			description.text = buff.Description;
		}
	}

	internal void NotifyExit(ExtraBuffViewEntry extraBuffViewEntry)
	{
		if (target == null)
		{
			SkipHide();
		}
		if (!(target != extraBuffViewEntry))
		{
			SkipHide();
		}
	}

	public void SkipShow()
	{
		fadeGroup.SkipShow();
	}

	public void SkipHide()
	{
		target = null;
		fadeGroup.SkipHide();
	}
}
