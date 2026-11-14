using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemMouseDragHandler : EmptyActionHandler, Cancelable
{
	public static ItemMouseDragHandler current;

	[SerializeField]
	public float dragThreshold;

	[SerializeField]
	public KeyCode key;

	[SerializeField]
	public Vector2 tooltipTranslation = new Vector3(32f, 0f);

	[SerializeField]
	public KeyCode scrollMult10Key;

	[SerializeField]
	public KeyCode scrollMult100Key;

	[SerializeField]
	public KeyCode scrollIgnoreKey;

	[SerializeField]
	public float dragSnapAnimationSpeed;

	private GameItemElement currentItem;

	private Vector3 lastMouseClickedPosition;

	private KeyCode lastMouseClickedKey;

	private bool isDragging;

	private bool isMult10;

	private bool isMult100;

	private bool isScrollIgnore;

	private RichTextBuilder builder = new RichTextBuilder(60);

	private int maxRemove;

	private int currentRemove;

	private List<TreeNodeRender> highlightedMayNodes = new List<TreeNodeRender>();

	private GameItem lastItem;

	private GameInventory lastInventory;

	private SlotMarker lastSlot;

	private HighlightManager highlightManager = new HighlightManager();

	public override List<KeyCode> keyListeners => new KeyCode[10] { key, rotateCWKey, rotateCCWKey, flipKey, resetKey, scrollMult10Key, scrollMult100Key, scrollIgnoreKey, flipAltKey, secondaryKey }.ToList();

	public KeyCode rotateCWKey => KeyBindHelper.Get("rotate_cw");

	public KeyCode rotateCCWKey => KeyBindHelper.Get("rotate_ccw");

	public KeyCode flipKey => KeyBindHelper.Get("flip_item");

	public KeyCode resetKey => KeyBindHelper.Get("reset_orientation");

	public KeyCode flipAltKey => KeyBindHelper.Get("flip_item_alt");

	public KeyCode secondaryKey => KeyBindHelper.Get("secondary_click");

	public bool IsDraggingItem => isDragging;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		current = this;
	}

	protected override void OnEventPress(ISet<KeyCode> keysPressed)
	{
		if (keysPressed.Contains(scrollMult10Key))
		{
			isMult10 = true;
		}
		if (keysPressed.Contains(scrollMult100Key))
		{
			isMult100 = true;
		}
		if (keysPressed.Contains(scrollIgnoreKey))
		{
			isScrollIgnore = true;
		}
		if (isDragging)
		{
			if (IsDragTargetDestroyed())
			{
				AbortDrag();
				return;
			}
			bool flag = EmporiumOption.Instance != null && EmporiumOption.Instance.swapDragRotateFlip;
			bool num = keysPressed.Contains(rotateCWKey) || (flag && keysPressed.Contains(secondaryKey));
			bool flag2 = keysPressed.Contains(rotateCCWKey);
			bool flag3 = keysPressed.Contains(flipKey) || keysPressed.Contains(flipAltKey) || (!flag && keysPressed.Contains(secondaryKey));
			bool flag4 = keysPressed.Contains(resetKey);
			bool flag5 = num || flag2 || flag3 || flag4;
			if (num)
			{
				currentItem.RotCW();
			}
			if (flag2)
			{
				currentItem.RotCCW();
			}
			if (flag3)
			{
				FlipHeldItem();
			}
			if (flag4)
			{
				while (currentItem.tempShape.orientation != 0)
				{
					currentItem.RotCW();
				}
				if (currentItem.tempShape.flipped)
				{
					currentItem.FlipX();
				}
			}
			if (flag5)
			{
				RefreshDragHighlight();
			}
		}
		else if (keysPressed.Contains(key) && !ItemPinHelper.BlocksClicks)
		{
			lastMouseClickedPosition = Input.mousePosition;
			currentItem = RenderHandler.RaycastElement<GameItemElement>(lastMouseClickedPosition);
			lastMouseClickedKey = keysPressed.First();
		}
	}

	protected override void OnEventRelease(ISet<KeyCode> keysReleased)
	{
		if (keysReleased.Contains(scrollMult10Key))
		{
			isMult10 = false;
		}
		if (keysReleased.Contains(scrollMult100Key))
		{
			isMult100 = false;
		}
		if (keysReleased.Contains(scrollIgnoreKey))
		{
			isScrollIgnore = false;
		}
		if (!keysReleased.Contains(lastMouseClickedKey))
		{
			return;
		}
		if (isDragging && IsDragTargetDestroyed())
		{
			AbortDrag();
			return;
		}
		if (isDragging)
		{
			EndDrag();
		}
		OnEventReset();
	}

	protected override void OnEventMouseMove(Vector3 mousePosition, Vector3 mouseDelta)
	{
		if (lastMouseClickedKey == KeyCode.None || currentItem == null)
		{
			return;
		}
		if (isDragging && IsDragTargetDestroyed())
		{
			AbortDrag();
			return;
		}
		if (!isDragging && (mousePosition - lastMouseClickedPosition).magnitude >= dragThreshold && !StoreUIManager.Instance.hoveringStoreUI && !UIDragHandler.isDraggingAnyUI)
		{
			StartDrag();
		}
		Drag(mouseDelta);
	}

	private void FlipHeldItem()
	{
		if (currentItem.tempShape.orientation % 2 == 0)
		{
			currentItem.FlipX();
		}
		else
		{
			currentItem.FlipY();
		}
	}

	private void RefreshDragHighlight()
	{
		if (lastInventory != null)
		{
			lastSlot = lastInventory.TryInventorySlot(currentItem, currentRemove, currentItem.GetFakeRotatorCenteredPosition(), currentItem.tempShape, currentItem.modifiedState);
			highlightManager.Try(lastSlot, (lastSlot.numTransfer > 0) ? RenderHandler.BackgroundColor.Green : RenderHandler.BackgroundColor.Red);
		}
	}

	protected override void OnEventMouseScroll(Vector2 scrollDelta)
	{
		if (!isDragging)
		{
			return;
		}
		if (IsDragTargetDestroyed())
		{
			AbortDrag();
			return;
		}
		if (!isScrollIgnore && maxRemove <= 1)
		{
			int num = (int)(scrollDelta.x + scrollDelta.y);
			if (num != 0)
			{
				if (EmporiumOption.Instance != null && EmporiumOption.Instance.swapDragRotateFlip)
				{
					FlipHeldItem();
				}
				else if (num > 0)
				{
					currentItem.RotCCW();
				}
				else
				{
					currentItem.RotCW();
				}
				RefreshDragHighlight();
			}
			return;
		}
		bool flag = false;
		if (!isScrollIgnore)
		{
			int num2 = (int)(scrollDelta.x + scrollDelta.y);
			if (isMult10)
			{
				num2 *= 10;
			}
			if (isMult100)
			{
				num2 *= 100;
			}
			if ((isMult10 || isMult100) && currentRemove == 1)
			{
				currentRemove = 0;
			}
			int num3 = currentRemove;
			currentRemove += num2;
			if (currentRemove < 1)
			{
				currentRemove = 1;
			}
			if (currentRemove > maxRemove)
			{
				currentRemove = maxRemove;
			}
			currentItem.UpdateFakeReal(currentRemove);
			if (num3 == currentRemove)
			{
				flag = true;
			}
		}
		if (flag || isScrollIgnore)
		{
			if (RenderHandler.RaycastElement<GameInventory>(Input.mousePosition, currentItem.fakeBackground) is GameGridScrollableInventory gameGridScrollableInventory)
			{
				gameGridScrollableInventory.Scroll(scrollDelta);
				Drag(Vector3.zero);
			}
			else if (RenderHandler.RaycastElement<GameItem>(Input.mousePosition, currentItem.fakeBackground)?.parentInventory is GameGridScrollableInventory gameGridScrollableInventory2)
			{
				gameGridScrollableInventory2.Scroll(scrollDelta);
				Drag(Vector3.zero);
			}
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (!hasFocus && lastMouseClickedKey != 0)
		{
			if (isDragging && IsDragTargetDestroyed())
			{
				AbortDrag();
			}
			else
			{
				OnEventReset();
			}
		}
	}

	protected override bool OnEventUpdate()
	{
		if (isDragging && IsDragTargetDestroyed())
		{
			AbortDrag();
			return false;
		}
		if (lastMouseClickedKey != 0 && !Input.GetKey(lastMouseClickedKey))
		{
			OnEventReset();
			return false;
		}
		if (isDragging)
		{
			Vector2 vector = currentItem.fakeMiddle.parent.rectTransform.InverseTransformPoint(Input.mousePosition);
			float x = vector.x;
			float y = vector.y;
			float num = x;
			float num2 = y;
			(float, float) centeredPosition = currentItem.GetCenteredPosition();
			float item = centeredPosition.Item1;
			float item2 = centeredPosition.Item2;
			float num3 = Mathf.Clamp(dragSnapAnimationSpeed * Time.deltaTime, 0f, 1f);
			x = num * num3 + item * (1f - num3);
			float num4 = num2 * num3 + item2 * (1f - num3);
			num = x;
			num2 = num4;
			currentItem.SetCenteredPosition(num, num2);
		}
		if (isDragging)
		{
			return true;
		}
		return false;
	}

	protected override void OnEventReset()
	{
		lastItem = null;
		lastInventory = null;
		bool flag = isDragging;
		isDragging = false;
		try
		{
			if (flag)
			{
				EndDrag(cancelled: true);
			}
			highlightManager.Clear();
			MachineryTabs.ClearDragHover();
		}
		finally
		{
			currentItem = null;
			lastMouseClickedPosition = Vector3.zero;
			lastMouseClickedKey = KeyCode.None;
			isDragging = false;
			isMult10 = false;
			isMult100 = false;
		}
	}

	private bool IsDragTargetDestroyed()
	{
		if (currentItem != null)
		{
			return currentItem.IsDestroyed();
		}
		return true;
	}

	private void AbortDrag()
	{
		isDragging = false;
		ClearMayHighlights();
		OverlayHandler.current.HideTooltip();
		OnEventReset();
	}

	private void ClearMayHighlights()
	{
		foreach (TreeNodeRender highlightedMayNode in highlightedMayNodes)
		{
			if (highlightedMayNode != null && highlightedMayNode.image != null)
			{
				highlightedMayNode.image.enabled = false;
			}
		}
		highlightedMayNodes.Clear();
	}

	private void StartDrag()
	{
		if (currentItem?.parentInventory == null)
		{
			return;
		}
		currentItem?.contentWindow?.Hide();
		currentItem?.examineWindow?.Hide();
		WindowsHandler.current.visibleWindows.FindAll((PixelWindow w) => GraphUtils.FindChildEqualsIgnoreType<GameItem>(w, currentItem?.parentInventory)).ForEach(delegate(PixelWindow w)
		{
			w.ToFront();
		});
		GameItemElement thisItem = currentItem;
		if (!thisItem.MayRemove())
		{
			return;
		}
		maxRemove = thisItem.MaxNumRemove();
		if (maxRemove < 1)
		{
			return;
		}
		currentRemove = (currentItem.isDebugMenu ? 1 : maxRemove);
		if (Input.GetKey(KeyCode.LeftAlt))
		{
			currentRemove = 1;
		}
		if (Input.GetKey(KeyCode.LeftShift))
		{
			currentRemove = (int)Math.Ceiling((float)maxRemove / 2f);
		}
		thisItem.UpdateFakeReal(currentRemove);
		thisItem.ToggleFakeBackground(enabled: true, UISettings.current.dragOpacity);
		thisItem.tempShape.Set(thisItem.modifiedShape);
		List<GameItem> list = new List<GameItem>();
		foreach (PixelWindow visibleWindow in WindowsHandler.current.visibleWindows)
		{
			list.AddRange(GraphUtils.FindChildrenType(visibleWindow, (GameItem r) => r.MayHaveValidInventorySlot(thisItem) || r.MayStack(thisItem) || (currentItem.MayTarget(r) && currentItem.CanTarget(r))));
		}
		foreach (GameItem item in list)
		{
			if (item != thisItem)
			{
				List<TreeNodeRender> list2 = item.parentInventory?.HighlightSlot(item.parentInventory?.GetIncompleteItemInventorySlot(item), RenderHandler.BackgroundColor.Blue, doReplace: true);
				if (list2 != null)
				{
					highlightedMayNodes.AddRange(list2);
				}
			}
		}
		isDragging = true;
		InputActionManager.current.ResetAfter(this);
		AudioManager.Instance.TryPlayOne(currentItem.soundDragStart, currentItem.soundDragEnd);
	}

	private void Drag(Vector3 mouseDelta)
	{
		if (!isDragging)
		{
			return;
		}
		Vector3 mousePosition = Input.mousePosition;
		currentItem.MovePosition(mouseDelta.x, mouseDelta.y);
		GameInventory gameInventory = null;
		GameItem gameItem = RenderHandler.RaycastElement<GameItem>(mousePosition, currentItem.fakeBackground);
		if (gameItem == currentItem)
		{
			gameItem = null;
		}
		if (gameItem == null)
		{
			gameInventory = RenderHandler.RaycastFindParentElement<GameInventory>(mousePosition, currentItem.fakeBackground);
		}
		if (gameItem != lastItem)
		{
			if (lastInventory != null)
			{
				highlightManager.Clear();
			}
			if (lastItem != null)
			{
				highlightManager.Clear();
				if ((GeneralHelper.MayPlayerInsertInto(lastItem) && lastItem.MayHaveValidInventorySlot(currentItem)) || lastItem.MayStack(currentItem) || (currentItem.MayTarget(lastItem) && currentItem.CanTarget(lastItem)))
				{
					lastItem.parentInventory?.HighlightSlot(lastItem.GetIncompleteSlotInInventory(), RenderHandler.BackgroundColor.Blue, doReplace: true);
				}
			}
			if (gameItem != null && GeneralHelper.MayPlayerInsertInto(gameItem) && gameItem.MayHaveValidInventorySlot(currentItem))
			{
				lastSlot = gameItem.TryFindOneValidInventorySlot(currentItem);
				GameInventory parentInventory = gameItem.parentInventory;
				if (parentInventory != null)
				{
					SlotMarker incompleteSlotInInventory = gameItem.GetIncompleteSlotInInventory();
					SlotMarker slotMarker = lastSlot;
					parentInventory.HighlightSlot(incompleteSlotInInventory, (slotMarker != null && slotMarker.numTransfer > 0) ? RenderHandler.BackgroundColor.Green : RenderHandler.BackgroundColor.Red, doReplace: true);
				}
			}
			else if (gameItem != null && gameItem.MayStack(currentItem))
			{
				lastSlot = gameItem.GetStackSlot(currentItem);
				GameInventory parentInventory2 = gameItem.parentInventory;
				if (parentInventory2 != null)
				{
					SlotMarker incompleteSlotInInventory2 = gameItem.GetIncompleteSlotInInventory();
					SlotMarker slotMarker2 = lastSlot;
					parentInventory2.HighlightSlot(incompleteSlotInInventory2, (slotMarker2 != null && slotMarker2.numTransfer > 0) ? RenderHandler.BackgroundColor.Green : RenderHandler.BackgroundColor.Red, doReplace: true);
				}
			}
			else if (gameItem != null && currentItem.MayTarget(gameItem) && currentItem.CanTarget(gameItem))
			{
				gameItem.parentInventory?.HighlightSlot(gameItem.GetIncompleteSlotInInventory(), RenderHandler.BackgroundColor.Green, doReplace: true);
			}
			else if (gameItem != null)
			{
				lastSlot = gameItem.GetIncompleteSlotInInventory();
				highlightManager.Try(lastSlot, RenderHandler.BackgroundColor.Red);
			}
		}
		if (gameItem == null)
		{
			highlightManager.Clear();
			if (gameInventory != null && GeneralHelper.MayPlayerInsertInto(gameInventory) && gameInventory.MayHaveValidInventorySlot(currentItem))
			{
				lastSlot = gameInventory.TryInventorySlot(currentItem, currentRemove, currentItem.GetFakeRotatorCenteredPosition(), currentItem.tempShape, currentItem.modifiedState);
				if (lastSlot != null)
				{
					highlightManager.Try(lastSlot, (lastSlot.numTransfer > 0) ? RenderHandler.BackgroundColor.Green : RenderHandler.BackgroundColor.Red);
				}
			}
		}
		Vector2 localTranslation = tooltipTranslation + new Vector2(currentItem.tempShape.globalWidth * UISettings.current.gridSize / 2, 0f);
		GameItem lockedModuleBayMachine = MachineHelper.GetLockedModuleBayMachine(currentItem, gameItem, gameInventory);
		string reason;
		GameItem incompatibleDragTarget = MachineDnaManipulator.GetIncompatibleDragTarget(currentItem, gameItem, gameInventory, out reason);
		if (gameItem != null)
		{
			if (GeneralHelper.MayPlayerInsertInto(gameItem) && gameItem.MayHaveValidInventorySlot(currentItem))
			{
				SlotMarker slotMarker3 = gameItem.TryFindOneValidInventorySlot(currentItem);
				OverlayHandler.current.ShowTooltip(mousePosition, localTranslation, RichTextGenerator.InsertItemInOtherItem(currentItem, gameItem, slotMarker3 != null && slotMarker3.numTransfer > 0, builder));
			}
			else if (currentItem.MayTarget(gameItem) && currentItem.CanTarget(gameItem))
			{
				SlotMarker targetSlot = gameItem.TryFindOneValidInventorySlot(currentItem);
				OverlayHandler.current.ShowTooltip(mousePosition, localTranslation, RichTextGenerator.UseItemOnOtherItem(currentItem, gameItem, targetSlot, canInsert: true, builder));
			}
			else if (lockedModuleBayMachine != null)
			{
				OverlayHandler.current.ShowTooltip(mousePosition, localTranslation, MachineHelper.CreateLockedModuleBayDragTooltip(builder, currentItem, lockedModuleBayMachine));
			}
			else if (incompatibleDragTarget != null)
			{
				OverlayHandler.current.ShowTooltip(mousePosition, localTranslation, MachineDnaManipulator.CreateIncompatibleDragTooltip(builder, currentItem, incompatibleDragTarget, reason));
			}
		}
		else if (lockedModuleBayMachine != null)
		{
			OverlayHandler.current.ShowTooltip(mousePosition, localTranslation, MachineHelper.CreateLockedModuleBayDragTooltip(builder, currentItem, lockedModuleBayMachine));
		}
		else if (incompatibleDragTarget != null)
		{
			OverlayHandler.current.ShowTooltip(mousePosition, localTranslation, MachineDnaManipulator.CreateIncompatibleDragTooltip(builder, currentItem, incompatibleDragTarget, reason));
		}
		else
		{
			OverlayHandler.current.HideTooltip();
		}
		if (gameItem == null && gameInventory == null)
		{
			MachineryTabs.UpdateDragHover(mousePosition, currentItem.fakeBackground, currentItem);
		}
		else
		{
			MachineryTabs.ClearDragHover();
		}
		lastItem = gameItem;
		lastInventory = gameInventory;
	}

	private void EndDrag(bool cancelled = false)
	{
		MachineryTabs.ClearDragHover();
		if (!cancelled && lastInventory == null && lastItem == null)
		{
			GameInventory tabInventory = MachineryTabs.FindDropInventoryAt(Input.mousePosition, currentItem.fakeBackground, currentItem);
			if (tabInventory != null && GeneralHelper.MayPlayerInsertInto(tabInventory) && currentItem.parentInventory != tabInventory)
			{
				GraphUtils.TryAcceptAll(tabInventory, currentItem, currentRemove);
				WindowsHandler.current.visibleWindows.FindAll((PixelWindow w) => GraphUtils.FindChildEqualsIgnoreType<GameItem>(w, tabInventory)).ForEach(delegate(PixelWindow w)
				{
					w.ToFront();
				});
			}
		}
		if (lastInventory != null && GeneralHelper.MayPlayerInsertInto(lastInventory))
		{
			if (lastInventory is GameGridScrollableInventory { alwaysStack: not false } && currentItem?.parentInventory == lastInventory)
			{
				lastSlot = lastInventory.TryInventorySlot(currentItem, currentRemove, currentItem.GetFakeRotatorCenteredPosition(), currentItem.tempShape, currentItem.modifiedState);
				SlotMarker slotMarker = lastSlot;
				if (slotMarker != null && slotMarker.IsValid())
				{
					lastSlot?.AcceptUnchecked();
				}
			}
			else
			{
				lastSlot = lastInventory.TryInventorySlot(currentItem, currentRemove, currentItem.GetFakeRotatorCenteredPosition(), currentItem.tempShape, currentItem.modifiedState);
				lastSlot?.TryAcceptOnce();
			}
			WindowsHandler.current.visibleWindows.FindAll((PixelWindow w) => GraphUtils.FindChildEqualsIgnoreType<GameItem>(w, lastInventory)).ForEach(delegate(PixelWindow w)
			{
				w.ToFront();
			});
		}
		if (lastItem != null && GeneralHelper.MayPlayerInsertInto(lastItem) && lastItem.MayHaveValidInventorySlot(currentItem))
		{
			for (int i = 0; i < 40; i++)
			{
				if (currentRemove < 1)
				{
					break;
				}
				lastSlot = lastItem.TryFindOneValidInventorySlot(currentItem);
				if (lastSlot == null || lastSlot.numTransfer < 1)
				{
					break;
				}
				currentRemove -= lastSlot.TryAcceptOnce(currentRemove);
			}
		}
		if (lastItem != null && lastItem.MayStack(currentItem))
		{
			lastSlot = lastItem.GetStackSlot(currentItem);
			lastSlot?.TryAcceptOnce(currentRemove);
		}
		if (lastItem != null && currentItem.MayTarget(lastItem) && currentItem.CanTarget(lastItem))
		{
			ItemBehaviourManager.current.Target(currentItem, lastItem);
			if (currentItem.MayTarget(lastItem) && currentItem.CanTarget(lastItem))
			{
				currentItem.Target(lastItem);
			}
		}
		ClearMayHighlights();
		highlightManager.Clear();
		currentItem.ToggleFakeBackground(enabled: false);
		currentItem.Validate();
		if (currentItem?.parentInventory == null)
		{
			currentItem.Destroy();
		}
		OverlayHandler.current.HideTooltip();
		isDragging = false;
		AudioManager.Instance.TryPlayOne(currentItem.soundDragEnd, currentItem.soundDragStart);
	}

	public bool CustomCancel(bool nonOwnedOnly = false)
	{
		if (!isDragging)
		{
			return false;
		}
		if (nonOwnedOnly)
		{
			if (currentItem != null && !GeneralHelper.IsItemOwned(currentItem))
			{
				OnEventReset();
				return true;
			}
			return false;
		}
		OnEventReset();
		return true;
	}

	public bool Cancel()
	{
		if (isDragging)
		{
			OnEventReset();
			return true;
		}
		return false;
	}

	public GameItem GetCurrentItem()
	{
		return currentItem;
	}
}
