using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDefaultActivateHandler : EmptyActionHandler
{
	public static ItemDefaultActivateHandler current;

	[SerializeField]
	public KeyCode key;

	[SerializeField]
	public Vector2 tooltipTranslation = new Vector3(32f, 0f);

	private GameItem currentItem;

	private HighlightManager highlightManager = new HighlightManager();

	private RichTextBuilder builder = new RichTextBuilder(60);

	public override List<KeyCode> keyListeners => new KeyCode[1] { key }.ToList();

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		current = this;
	}

	protected override void OnEventPress(ISet<KeyCode> keysPressed)
	{
		if (ItemPinHelper.BlocksClicks)
		{
			return;
		}
		_ = currentItem;
		GameItem gameItem = RenderHandler.RaycastElement<GameItem>(Input.mousePosition);
		GameItem gameItem2 = gameItem;
		if (gameItem != null && currentItem == null && gameItem2.MayActivateSlotItem() && gameItem.activateDefault)
		{
			InputActionManager.current.ResetAfter(this);
			if (EmporiumOption.SingleClickChoice && gameItem2.CanActivateSlotItem())
			{
				gameItem2.ActivateSlotItem();
				OnEventReset();
			}
			else
			{
				SlotMarker incompleteSlotInInventory = gameItem2.GetIncompleteSlotInInventory();
				highlightManager.Try(incompleteSlotInInventory, gameItem2.CanActivateSlotItem() ? RenderHandler.BackgroundColor.Green : RenderHandler.BackgroundColor.Red);
				currentItem = gameItem;
			}
		}
		else if (gameItem != null && currentItem != null && currentItem == gameItem)
		{
			if (gameItem2.MayActivateSlotItem() && gameItem2.CanActivateSlotItem())
			{
				gameItem2.ActivateSlotItem();
			}
			OnEventReset();
		}
		else
		{
			OnEventReset();
		}
	}

	protected override void OnEventMouseMove(Vector3 mousePosition, Vector3 mouseDelta)
	{
		if (currentItem != null)
		{
			GameItem gameItem = RenderHandler.RaycastElement<GameItem>(Input.mousePosition);
			if (currentItem != gameItem)
			{
				OnEventReset();
			}
		}
	}

	protected override bool OnEventUpdate()
	{
		if (currentItem != null)
		{
			GameItem gameItem = currentItem;
			builder.Clear();
			if (gameItem.CanActivateSlotItem())
			{
				builder.Add(LocHelper.GetLocalizedMechanic("handler_activate_click_again")).Add(LocHelper.GetLocalizedMechanic("handler_activate_confirm"), RenderHandler.ColorPalette.Green, "", bold: true).Add(LocHelper.GetLocalizedMechanic("handler_activate_activation_of"))
					.Add(gameItem.GetDisplayName(preferCustomName: true), RenderHandler.ColorPalette.None, "", bold: true, italic: false, underline: true);
			}
			else
			{
				builder.Add(LocHelper.GetLocalizedMechanic("handler_activate_cannot", gameItem.GetDisplayName(preferCustomName: true)), RenderHandler.ColorPalette.Red);
			}
			if (!builder.IsEmpty())
			{
				OverlayHandler.current.ShowTooltip(Input.mousePosition, tooltipTranslation, builder.Build());
			}
			else
			{
				OverlayHandler.current.HideTooltip();
			}
		}
		return currentItem != null;
	}

	protected override void OnEventReset()
	{
		highlightManager.Clear();
		if (currentItem != null)
		{
			OverlayHandler.current.HideTooltip();
		}
		currentItem = null;
	}
}
