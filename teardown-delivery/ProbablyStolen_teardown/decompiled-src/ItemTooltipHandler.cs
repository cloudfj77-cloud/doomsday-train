using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemTooltipHandler : EmptyActionHandler
{
	public static ItemTooltipHandler current;

	[SerializeField]
	public KeyCode key;

	[SerializeField]
	public Vector2 tooltipTranslation = new Vector3(32f, 0f);

	public const int BASE_TOOLTIP_CHAR_WIDTH = 60;

	public const float MACHINE_TOOLTIP_WIDTH_MULT = 1.2f;

	private bool advanced;

	private TooltipListener currentTooltipListener;

	public override List<KeyCode> keyListeners => new KeyCode[1] { key }.ToList();

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		current = this;
	}

	protected override void OnEventPress(ISet<KeyCode> keysPressed)
	{
		base.OnEventPress(keysPressed);
		advanced = true;
	}

	protected override void OnEventRelease(ISet<KeyCode> keysReleased)
	{
		base.OnEventRelease(keysReleased);
		advanced = false;
	}

	protected override bool OnEventUpdate()
	{
		if (PlayerStore.Instance.cinematicMode)
		{
			return false;
		}
		TooltipListener tooltipListener = RenderHandler.RaycastElement<TooltipListener>(Input.mousePosition);
		if (tooltipListener != null && !tooltipListener.HasTooltip())
		{
			tooltipListener = null;
		}
		if (tooltipListener != null && !UIDragHandler.isDraggingAnyUI)
		{
			int maxCharWidth = 60;
			if (tooltipListener is GameItem gameItem && gameItem.IsTag("FEED_DISPENSER_TAG"))
			{
				maxCharWidth = 90;
			}
			else if (tooltipListener is GameItem gameItem2 && gameItem2.IsGameItemType("MACHINE"))
			{
				maxCharWidth = 72;
			}
			else if (tooltipListener is GameItem gameItem3 && gameItem3.IsTag("MACHINE_MANUAL_TAG"))
			{
				maxCharWidth = 120;
			}
			if (advanced)
			{
				OverlayHandler.current.ShowTooltip(Input.mousePosition, tooltipTranslation, tooltipListener.GetTooltipAdvanced(), maxCharWidth);
			}
			else
			{
				OverlayHandler.current.ShowTooltip(Input.mousePosition, tooltipTranslation, tooltipListener.GetTooltip(), maxCharWidth);
			}
		}
		else
		{
			OverlayHandler.current.HideTooltip();
		}
		currentTooltipListener = tooltipListener;
		return base.OnEventUpdate();
	}

	protected override void OnEventReset()
	{
		if (currentTooltipListener != null)
		{
			OverlayHandler.current.HideTooltip();
		}
		currentTooltipListener = null;
	}
}
