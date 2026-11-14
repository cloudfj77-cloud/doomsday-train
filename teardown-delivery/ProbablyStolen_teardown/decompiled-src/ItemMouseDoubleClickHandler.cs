using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemMouseDoubleClickHandler : EmptyActionHandler
{
	public static ItemMouseDoubleClickHandler current;

	[SerializeField]
	public float doubleclickTimeThresholdSeconds;

	[SerializeField]
	public KeyCode key;

	public Vector2 openWindowTranslation = new Vector2(6f, -6f);

	private float timeDeltaSeconds = 100000f;

	private GameItem lastClickedItem;

	private KeyCode lastKey;

	private Vector2 pressPosition;

	private int pressFrame = -1;

	private int consumedPressFrame = -1;

	private bool isDoubleClick;

	public override List<KeyCode> keyListeners => new KeyCode[1] { key }.ToList();

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		current = this;
	}

	public void ConsumePress()
	{
		consumedPressFrame = Time.frameCount;
	}

	protected override void OnEventPress(ISet<KeyCode> keysPressed)
	{
		if (ItemPinHelper.BlocksClicks)
		{
			OnEventReset();
			return;
		}
		KeyCode keyCode = lastKey;
		if (!keysPressed.Contains(lastKey))
		{
			keyCode = keysPressed.First();
		}
		Vector2 screenPosition = Input.mousePosition;
		GameItem gameItem = RenderHandler.RaycastElement<GameItem>(screenPosition);
		if (gameItem == null || GeneralHelper.IsDisplayOnly(gameItem))
		{
			OnEventReset();
			return;
		}
		if (lastClickedItem != null && pressFrame == consumedPressFrame)
		{
			OnEventReset();
		}
		if (Time.frameCount == consumedPressFrame)
		{
			OnEventReset();
			return;
		}
		if (lastClickedItem == gameItem && timeDeltaSeconds < doubleclickTimeThresholdSeconds && keyCode == lastKey)
		{
			isDoubleClick = true;
			return;
		}
		isDoubleClick = false;
		timeDeltaSeconds = 0f;
		lastClickedItem = gameItem;
		lastKey = keyCode;
		pressPosition = screenPosition;
		pressFrame = Time.frameCount;
	}

	protected override void OnEventRelease(ISet<KeyCode> keysReleased)
	{
		if (!keysReleased.Contains(key) || lastClickedItem == null || isDoubleClick || pressFrame == consumedPressFrame)
		{
			return;
		}
		bool singleClickInteract = EmporiumOption.SingleClickInteract;
		bool flag = EmporiumOption.SingleClickActivate && !lastClickedItem.activateDefault;
		if ((singleClickInteract || flag) && !ItemPinHelper.BlocksClicks && (!(ItemSelectHandler.current != null) || !ItemSelectHandler.current.IsEquipped) && !KeyBindHelper.GetKey("quick_move_inventory") && !KeyBindHelper.GetKey("quick_move_showcase") && !KeyBindHelper.GetKey("quick_move_storage"))
		{
			Vector2 vector = Input.mousePosition;
			float num = ((ItemMouseDragHandler.current != null) ? ItemMouseDragHandler.current.dragThreshold : 0f);
			if (!((vector - pressPosition).magnitude >= num) && RenderHandler.RaycastElement<GameItem>(vector) == lastClickedItem)
			{
				ClickAction(lastClickedItem, vector, singleClickInteract, flag);
			}
		}
	}

	protected override bool OnEventUpdate()
	{
		if (isDoubleClick && lastClickedItem != null)
		{
			DoubleClickAction(lastClickedItem, Input.mousePosition);
			OnEventReset();
			return true;
		}
		timeDeltaSeconds += Time.deltaTime;
		return false;
	}

	protected override void OnEventReset()
	{
		timeDeltaSeconds = 100000f;
		lastClickedItem = null;
		lastKey = KeyCode.None;
		isDoubleClick = false;
		pressFrame = -1;
	}

	private void DoubleClickAction(GameItem newItem, Vector2 mousePosition)
	{
		if (newItem != null)
		{
			bool interact = !EmporiumOption.SingleClickInteract;
			bool activate = !EmporiumOption.SingleClickActivate || newItem.activateDefault;
			ClickAction(newItem, mousePosition, interact, activate);
		}
	}

	private void ClickAction(GameItem newItem, Vector2 mousePosition, bool interact, bool activate)
	{
		if (newItem == null)
		{
			return;
		}
		if (interact)
		{
			if (newItem.contentWindow != null)
			{
				if (newItem.contentWindow.IsVisible())
				{
					newItem.CloseContentWindow();
					AudioManager.Instance.TryPlayOne(newItem.soundContainerClose, newItem.soundContainerOpen);
				}
				else
				{
					OpenContentAction(newItem, mousePosition);
				}
			}
			else
			{
				OpenExamineAction(newItem, mousePosition);
			}
		}
		if (activate && newItem.MayActivateSlotItem() && newItem.CanActivateSlotItem())
		{
			newItem.ActivateSlotItem();
		}
		else if (interact)
		{
			if (newItem.MaySelectSlotItem() && newItem.CanSelectSlotItem() && !newItem.MayActivateSlotItem() && newItem.contentWindow == null)
			{
				ItemSelectHandler.current.TrySelectItem(newItem);
			}
			else if (newItem.contentWindow == null && newItem.MayToggleSlotItem() && newItem.CanToggleSlotItem())
			{
				newItem.ToggleSlotItem();
			}
		}
	}

	public void OpenContentAction(GameItem newItem, Vector2 mousePosition)
	{
		if (newItem == null || newItem.contentWindow == null)
		{
			return;
		}
		if (!newItem.contentWindow.IsVisible())
		{
			newItem.contentWindow?.SetTitle(newItem.GetDisplayName(preferCustomName: true));
			bool num = newItem.contentWindow.IsVisible();
			newItem.contentWindow?.Show(mousePosition, openWindowTranslation);
			newItem.RestoreContentWindowPosition();
			bool flag = newItem.contentWindow.IsVisible();
			if (num != flag)
			{
				AudioManager.Instance.TryPlayOne(newItem.soundContainerOpen);
			}
			if (flag && newItem.IsTag("TRACK_OPENED_TAG") && !newItem.IsTag("OPENED_TAG"))
			{
				newItem.EnableTag("OPENED_TAG");
			}
		}
		else
		{
			newItem.contentWindow?.ToFront();
		}
	}

	public void OpenExamineAction(GameItem newItem, Vector2 mousePosition)
	{
		if (newItem == null || newItem.examineWindow == null)
		{
			return;
		}
		if (!newItem.examineWindow.IsVisible())
		{
			newItem.examineWindow?.SetTitle(newItem.GetDisplayName(preferCustomName: true));
			bool num = newItem.examineWindow.IsVisible();
			newItem.examineWindow?.Show(mousePosition, openWindowTranslation);
			bool flag = newItem.examineWindow.IsVisible();
			if (num != flag)
			{
				AudioManager.Instance.TryPlayOne(newItem.soundExamineOpen);
			}
		}
		else
		{
			newItem.examineWindow?.ToFront();
		}
	}
}
