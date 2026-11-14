using UnityEngine;

public static class ItemPinHelper
{
	public const string PINNED_TAG = "PINNED_TAG";

	public const string OVERLAY_ATLAS = "Window/background";

	public const string OVERLAY_SPRITE = "pinned_top";

	public const RenderHandler.ColorPalette PIN_COLOR = RenderHandler.ColorPalette.LightOrange;

	private static Sprite overlaySprite;

	private static bool overlaySpriteResolved;

	private static bool pinMode;

	private static int consumedEscapeFrame = -1;

	private static int exitedFrame = -1;

	public static bool IsPinMode => pinMode;

	public static bool BlocksClicks
	{
		get
		{
			if (!pinMode)
			{
				return exitedFrame == Time.frameCount;
			}
			return true;
		}
	}

	public static bool IsPinned(GameItem item)
	{
		return item?.IsTag("PINNED_TAG") ?? false;
	}

	public static bool MayPin(GameItem item)
	{
		if (item == null)
		{
			return false;
		}
		if (item is GameItemElement gameItemElement && gameItemElement.IsDestroyed())
		{
			return false;
		}
		if (item.parentInventory == null)
		{
			return false;
		}
		return GeneralHelper.IsItemOwned(item);
	}

	public static void SetPinned(GameItem item, bool pinned)
	{
		item?.SetEnableDisableTag("PINNED_TAG", pinned);
	}

	public static bool TogglePinned(GameItem item)
	{
		if (item == null)
		{
			return false;
		}
		bool flag = !IsPinned(item);
		SetPinned(item, flag);
		return flag;
	}

	public static void CreatePinnedTooltip(RichTextBuilder builder, GameItem item)
	{
		if (IsPinned(item))
		{
			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_pinned_tooltip"), ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
		}
	}

	public static Sprite GetOverlaySprite()
	{
		if (!overlaySpriteResolved)
		{
			overlaySprite = RenderHandler.TryLoadFromAtlas("Window/background", "pinned_top");
			overlaySpriteResolved = true;
		}
		return overlaySprite;
	}

	public static bool ConsumedEscapeThisFrame()
	{
		return consumedEscapeFrame == Time.frameCount;
	}

	public static void EnterPinMode()
	{
		if (!pinMode)
		{
			pinMode = true;
		}
	}

	public static void ExitPinMode()
	{
		if (pinMode)
		{
			pinMode = false;
			exitedFrame = Time.frameCount;
			TooltipManager._instance?.HideTooltip();
			if (!(StoreUIManager.Instance != null) || !StoreUIManager.Instance.hoveringStoreUI)
			{
				ItemTooltipHandler.current?.Enable();
			}
		}
	}

	public static void HandleInput()
	{
		if (!pinMode)
		{
			if (!MachineStorageCoordinator.IsBusy && !HopperHelper.IsBusy && KeyBindHelper.GetKeyDown("pin_mode"))
			{
				EnterPinMode();
			}
			return;
		}
		if (KeyBindHelper.GetBackDown())
		{
			consumedEscapeFrame = Time.frameCount;
			ExitPinMode();
			return;
		}
		if (KeyBindHelper.GetKeyDown("pin_mode") || KeyBindHelper.GetKeyDown("secondary_click"))
		{
			ExitPinMode();
			return;
		}
		ItemTooltipHandler.current?.Disable();
		GameItem gameItem = HoveredStoreItem();
		string message;
		bool flag = PinVerdict(gameItem, out message);
		TooltipManager._instance?.SetAndShowTooltip(message);
		if (Input.GetMouseButtonDown(0) && gameItem != null && flag)
		{
			TogglePinned(gameItem);
		}
	}

	private static GameItem HoveredStoreItem()
	{
		if (StoreUIManager.Instance != null && StoreUIManager.Instance.hoveringStoreUI)
		{
			return null;
		}
		return RenderHandler.RaycastElement<GameItem>(Input.mousePosition);
	}

	private static bool PinVerdict(GameItem hit, out string message)
	{
		if (hit == null)
		{
			message = LocHelper.GetLocalizedMechanic("mech_pin_mode_hint");
			return false;
		}
		string text = RichTextBuilder.Color(hit.GetDisplayName(preferCustomName: true), RenderHandler.ColorPalette.LightOrange);
		if (!MayPin(hit))
		{
			message = LocHelper.GetLocalizedMechanic("mech_pin_cant_pin", text);
			return false;
		}
		message = (IsPinned(hit) ? LocHelper.GetLocalizedMechanic("mech_pin_click_unpin", text) : LocHelper.GetLocalizedMechanic("mech_pin_click_pin", text));
		return true;
	}
}
