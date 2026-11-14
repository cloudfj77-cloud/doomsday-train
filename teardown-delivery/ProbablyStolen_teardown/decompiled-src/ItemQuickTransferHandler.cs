using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemQuickTransferHandler : EmptyActionHandler
{
	public static ItemQuickTransferHandler current;

	[SerializeField]
	public KeyCode key;

	private GameItem currentItem;

	private bool isQuickTransfer;

	public override List<KeyCode> keyListeners => new KeyCode[2] { key, quickTransferKey }.ToList();

	public KeyCode quickTransferKey => KeyBindHelper.Get("quick_move_inventory");

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		current = this;
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (hasFocus)
		{
			InputActionManager.current.ResetAfter(this);
		}
	}

	protected override void OnEventPress(ISet<KeyCode> keysPressed)
	{
		if (keysPressed.Contains(key))
		{
			GameItem gameItem = RenderHandler.RaycastElement<GameItem>(Input.mousePosition);
			currentItem = gameItem;
		}
		if (keysPressed.Contains(quickTransferKey))
		{
			isQuickTransfer = true;
			InputActionManager.current.ResetAfter(this);
		}
	}

	protected override void OnEventRelease(ISet<KeyCode> keysPressed)
	{
		if (keysPressed.Contains(key))
		{
			if (isQuickTransfer)
			{
				GameItemElement newItem = RenderHandler.RaycastElement<GameItemElement>(Input.mousePosition);
				if (newItem == null)
				{
					return;
				}
				PixelWindow parentWindow = newItem.handler.FindParentWindow();
				AudioManager.Instance.TryPlayOne(newItem.soundDragStart, newItem.soundDragEnd);
				if (newItem != null && newItem == currentItem && newItem.MayRemove())
				{
					if (newItem.IsTag(GameCharacterItem.TAG_NOT_PURCHASED) && parentWindow != null)
					{
						foreach (GameInventory item in GraphUtils.FindChildrenType<GameInventory>(parentWindow))
						{
							if (item != newItem.parentInventory && item.TryFindOneValidInventorySlot(newItem) != null)
							{
								GraphUtils.TryAcceptOnce(item, newItem);
								break;
							}
						}
					}
					else if (GraphUtils.CanInsertToActiveContainer(parentWindow, newItem, EmporiumEntry.Instance.invWindow))
					{
						GraphUtils.InsertToActiveContainer(WindowsHandler.current.visibleWindows.FindLast((PixelWindow w) => w != parentWindow && (w.TryFindOneValidInventorySlot(newItem)?.IsValid() ?? false)), newItem);
					}
					else if (EmporiumEntry.Instance.invWindow == parentWindow)
					{
						GraphUtils.TryAcceptOnce(EmporiumEntry.Instance.backInvWindow, newItem);
					}
					else if (EmporiumEntry.Instance.backInvWindow == parentWindow)
					{
						if (GeneralHelper.IsItemOwned(newItem))
						{
							if (!MapUIManager.Instance.isOutside)
							{
								GraphUtils.TryAcceptOnce(EmporiumEntry.Instance.invWindow, newItem);
							}
							else
							{
								GraphUtils.TryAcceptOnce(EmporiumEntry.Instance.afterhourWindow, newItem);
							}
						}
						else
						{
							GraphUtils.TryAcceptOnce(EmporiumEntry.Instance.frontInvWindow, newItem);
						}
					}
					else if (EmporiumEntry.Instance.frontInvWindow == parentWindow)
					{
						if (GeneralHelper.IsItemOwned(newItem))
						{
							if (!MapUIManager.Instance.isOutside)
							{
								GraphUtils.TryAcceptOnce(EmporiumEntry.Instance.invWindow, newItem);
							}
							else
							{
								GraphUtils.TryAcceptOnce(EmporiumEntry.Instance.afterhourWindow, newItem);
							}
						}
						else
						{
							GraphUtils.TryAcceptOnce(EmporiumEntry.Instance.backInvWindow, newItem);
						}
					}
					else if (EmporiumEntry.Instance.showcaseWindow == parentWindow)
					{
						GraphUtils.TryAcceptOnce(EmporiumEntry.Instance.invWindow, newItem);
					}
					else if (EmporiumEntry.Instance.afterhourWindow == parentWindow)
					{
						if (EmporiumEntry.Instance.backInvWindow.IsVisible())
						{
							GraphUtils.TryAcceptOnce(EmporiumEntry.Instance.backInvWindow, newItem);
						}
					}
					else if (!MapUIManager.Instance.isOutside)
					{
						GraphUtils.TryAcceptOnce(EmporiumEntry.Instance.invWindow, newItem);
					}
					else
					{
						GraphUtils.TryAcceptOnce(EmporiumEntry.Instance.afterhourWindow, newItem);
					}
				}
				InputActionManager.current.ResetAfter(this);
			}
			currentItem = null;
		}
		if (keysPressed.Contains(quickTransferKey))
		{
			isQuickTransfer = false;
		}
	}

	protected override bool OnEventUpdate()
	{
		return isQuickTransfer;
	}

	protected override void OnEventReset()
	{
		currentItem = null;
		isQuickTransfer = false;
	}
}
