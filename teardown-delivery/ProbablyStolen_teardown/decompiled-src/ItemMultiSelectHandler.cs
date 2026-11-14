using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemMultiSelectHandler : EmptyActionHandler, Cancelable
{
	private enum State
	{
		Idle,
		PendingBand,
		Banding,
		Selected,
		PendingGroupDrag,
		GroupDragging
	}

	public static ItemMultiSelectHandler current;

	[SerializeField]
	public float dragThreshold = 10f;

	[SerializeField]
	public KeyCode key = KeyCode.Mouse0;

	[SerializeField]
	public float dragSnapAnimationSpeed = 20f;

	private State state;

	private Vector3 pressPosition;

	private PixelWindow bandWindow;

	private GameInventory bandInventory;

	private readonly List<GameItemElement> selectedItems = new List<GameItemElement>();

	private readonly Dictionary<GameItemElement, Vector2> dragOffsets = new Dictionary<GameItemElement, Vector2>();

	private readonly Dictionary<GameItemElement, GameInventory> selectedHomes = new Dictionary<GameItemElement, GameInventory>();

	private readonly List<TreeNodeRender> selectionHighlights = new List<TreeNodeRender>();

	private TreeNodeRender bandNode;

	private readonly TreeNodeRender[] bandEdges = new TreeNodeRender[4];

	private GameInventory hoverInventory;

	private GameItem hoverItem;

	private readonly List<TreeNodeRender> hoverInventoryNodes = new List<TreeNodeRender>();

	private readonly List<TreeNodeRender> hoverHighlightNodes = new List<TreeNodeRender>();

	private bool precisePlacementActive;

	private readonly List<TreeNodeRender> precisePreviewNodes = new List<TreeNodeRender>();

	public override List<KeyCode> keyListeners => new KeyCode[1] { key }.ToList();

	public bool HasSelection => state != State.Idle;

	public bool IsGroupDragging => state == State.GroupDragging;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		current = this;
	}

	protected override bool OnEventInit()
	{
		if ((object)QuickChooseHandler.current == null || (object)WindowsHandler.current == null)
		{
			return false;
		}
		List<Cancelable> cancelables = QuickChooseHandler.current.GetCancelables();
		if (cancelables.Contains(this))
		{
			return true;
		}
		int num = cancelables.IndexOf(WindowsHandler.current);
		if (num < 0)
		{
			return false;
		}
		cancelables.Insert(num, this);
		return true;
	}

	protected override void OnEventPress(ISet<KeyCode> keysPressed)
	{
		if (!keysPressed.Contains(key) || ItemPinHelper.BlocksClicks)
		{
			return;
		}
		PruneSelection();
		Vector3 mousePosition = Input.mousePosition;
		if (state == State.Selected)
		{
			GameItemElement gameItemElement = RenderHandler.RaycastElement<GameItemElement>(mousePosition);
			bool flag = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
			if (gameItemElement != null && selectedItems.Contains(gameItemElement) && !flag)
			{
				if (selectedItems.Count == 1)
				{
					ClearSelection();
					state = State.Idle;
				}
				else
				{
					pressPosition = mousePosition;
					state = State.PendingGroupDrag;
					InputActionManager.current.ResetAfter(this);
				}
				return;
			}
			ClearSelection();
			state = State.Idle;
		}
		if (state == State.Idle && !StoreUIManager.Instance.hoveringStoreUI && !UIDragHandler.isDraggingAnyUI && RenderHandler.RaycastElement<GameItemElement>(mousePosition) == null && !IsPointerOverDragSurface(mousePosition))
		{
			pressPosition = mousePosition;
			bandInventory = RenderHandler.RaycastFindParentElement<GameInventory>(mousePosition);
			bandWindow = RenderHandler.RaycastFindParentElement<PixelWindow>(mousePosition);
			state = State.PendingBand;
		}
	}

	protected override void OnEventRelease(ISet<KeyCode> keysReleased)
	{
		if (keysReleased.Contains(key))
		{
			PruneSelection();
			switch (state)
			{
			case State.PendingBand:
				bandWindow = null;
				bandInventory = null;
				state = State.Idle;
				break;
			case State.Banding:
				FinalizeBand();
				break;
			case State.PendingGroupDrag:
				ClearSelection();
				state = State.Idle;
				break;
			case State.GroupDragging:
				EndGroupDrag();
				break;
			case State.Selected:
				break;
			}
		}
	}

	protected override void OnEventMouseMove(Vector3 mousePosition, Vector3 mouseDelta)
	{
		PruneSelection();
		if (state == State.PendingBand && (mousePosition - pressPosition).magnitude >= dragThreshold && !StoreUIManager.Instance.hoveringStoreUI && !UIDragHandler.isDraggingAnyUI)
		{
			state = State.Banding;
			InputActionManager.current.ResetAfter(this);
		}
		if (state == State.Banding)
		{
			UpdateBand(mousePosition);
		}
		if (state == State.PendingGroupDrag && (mousePosition - pressPosition).magnitude >= dragThreshold)
		{
			StartGroupDrag();
		}
	}

	protected override bool OnEventUpdate()
	{
		PruneSelection();
		if ((state == State.PendingBand || state == State.Banding || state == State.PendingGroupDrag || state == State.GroupDragging) && !Input.GetKey(key))
		{
			OnEventReset();
			return false;
		}
		if (state == State.GroupDragging)
		{
			MoveGhosts();
			UpdateDropTarget();
		}
		if (selectedItems.Count > 0 && state != State.Banding)
		{
			if (precisePlacementActive)
			{
				ClearSelectionHighlightNodes();
			}
			else
			{
				RefreshSelectionHighlight();
			}
		}
		if (state != State.Banding && state != State.PendingGroupDrag)
		{
			return state == State.GroupDragging;
		}
		return true;
	}

	protected override void OnEventReset()
	{
		if (state == State.GroupDragging)
		{
			RestoreGhosts();
		}
		ClearSelection();
		HideBand();
		ClearDropTarget();
		bandWindow = null;
		bandInventory = null;
		pressPosition = Vector3.zero;
		state = State.Idle;
	}

	public bool Cancel()
	{
		if (state == State.Idle)
		{
			return false;
		}
		OnEventReset();
		return true;
	}

	public bool IsGroupDraggingItem(GameItem item)
	{
		if (state == State.GroupDragging && item is GameItemElement item2)
		{
			return selectedItems.Contains(item2);
		}
		return false;
	}

	private void PruneSelection()
	{
		if (selectedItems.Count == 0)
		{
			return;
		}
		for (int num = selectedItems.Count - 1; num >= 0; num--)
		{
			GameItemElement gameItemElement = selectedItems[num];
			bool flag = gameItemElement?.IsDestroyed() ?? true;
			GameInventory value;
			bool flag2 = !flag && selectedHomes.TryGetValue(gameItemElement, out value) && gameItemElement.parentInventory != value;
			if (flag || flag2)
			{
				selectedItems.RemoveAt(num);
				dragOffsets.Remove(gameItemElement);
				selectedHomes.Remove(gameItemElement);
				if (!flag)
				{
					RestoreLiveGhost(gameItemElement);
				}
			}
		}
		selectionHighlights.RemoveAll((TreeNodeRender n) => n == null);
		if (selectedItems.Count == 0 && (state == State.Selected || state == State.PendingGroupDrag || state == State.GroupDragging))
		{
			state = State.Idle;
			OnEventReset();
		}
	}

	private void RestoreLiveGhost(GameItemElement element)
	{
		if (element != null && !element.IsDestroyed())
		{
			element.fakeBackground.image.raycastTarget = true;
			if (element.fakeMiddle.parent == RenderHandler.current.overlayRoot)
			{
				element.ToggleFakeBackground(enabled: false);
				element.Validate();
			}
		}
	}

	public bool CustomCancel(bool nonOwnedOnly = false)
	{
		if (state == State.Idle)
		{
			return false;
		}
		if (nonOwnedOnly)
		{
			bool result = false;
			for (int num = selectedItems.Count - 1; num >= 0; num--)
			{
				GameItemElement gameItemElement = selectedItems[num];
				if (gameItemElement == null || gameItemElement.IsDestroyed() || !GeneralHelper.IsItemOwned(gameItemElement))
				{
					selectedItems.RemoveAt(num);
					dragOffsets.Remove(gameItemElement);
					selectedHomes.Remove(gameItemElement);
					RestoreLiveGhost(gameItemElement);
					result = true;
				}
			}
			if (selectedItems.Count == 0)
			{
				state = State.Idle;
				OnEventReset();
			}
			return result;
		}
		OnEventReset();
		return true;
	}

	private static bool IsPointerOverDragSurface(Vector2 screenPosition)
	{
		PointerEventData eventData = new PointerEventData(EventSystem.current)
		{
			position = screenPosition
		};
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, list);
		if (list.Count == 0)
		{
			return false;
		}
		GameObject gameObject = list[0].gameObject;
		if (gameObject.GetComponentInParent<UIDragHandler>() != null)
		{
			return true;
		}
		TreeNodeRender component = gameObject.GetComponent<TreeNodeRender>();
		if (component == null || component.eventHandler == null)
		{
			return false;
		}
		if (component.eventHandler.mouseDragFunc.Count <= 0)
		{
			return component.eventHandler.mouseBeginDragFunc.Count > 0;
		}
		return true;
	}

	private void EnsureBandNode()
	{
		if (!(bandNode != null))
		{
			bandNode = TreeNodeRender.Instantiate().SetParent(RenderHandler.current.overlayRoot);
			bandNode.image.enabled = false;
			bandNode.image.raycastTarget = false;
			bandNode.rectTransform.pivot = new Vector2(0f, 1f);
			for (int i = 0; i < bandEdges.Length; i++)
			{
				TreeNodeRender treeNodeRender = TreeNodeRender.Instantiate().SetParent(bandNode);
				treeNodeRender.image.sprite = null;
				treeNodeRender.image.color = new Color32(213, 210, 184, byte.MaxValue);
				treeNodeRender.image.raycastTarget = false;
				treeNodeRender.image.enabled = false;
				bandEdges[i] = treeNodeRender;
			}
		}
	}

	private void UpdateBand(Vector3 mousePosition)
	{
		EnsureBandNode();
		RectTransform rectTransform = RenderHandler.current.overlayRoot.rectTransform;
		Vector2 lhs = rectTransform.InverseTransformPoint(pressPosition);
		Vector2 rhs = rectTransform.InverseTransformPoint(mousePosition);
		Vector2 vector = Vector2.Min(lhs, rhs);
		Vector2 vector2 = Vector2.Max(lhs, rhs);
		Vector2 sizeDelta = vector2 - vector;
		bandNode.rectTransform.SetAsLastSibling();
		bandNode.rectTransform.anchoredPosition = new Vector2(vector.x, vector2.y);
		bandNode.rectTransform.sizeDelta = sizeDelta;
		float num = 1f;
		SetBandEdge(bandEdges[0], new Vector2(0f, 0f), new Vector2(sizeDelta.x, num));
		SetBandEdge(bandEdges[1], new Vector2(0f, 0f - Mathf.Max(0f, sizeDelta.y - num)), new Vector2(sizeDelta.x, num));
		SetBandEdge(bandEdges[2], new Vector2(0f, 0f), new Vector2(num, sizeDelta.y));
		SetBandEdge(bandEdges[3], new Vector2(Mathf.Max(0f, sizeDelta.x - num), 0f), new Vector2(num, sizeDelta.y));
		Vector2 vector3 = Vector2.Min(pressPosition, mousePosition);
		Vector2 vector4 = Vector2.Max(pressPosition, mousePosition);
		selectedItems.Clear();
		selectedItems.AddRange(ComputeBandItems(Rect.MinMaxRect(vector3.x, vector3.y, vector4.x, vector4.y)));
		RefreshSelectionHighlight();
	}

	private static void SetBandEdge(TreeNodeRender edge, Vector2 anchoredPosition, Vector2 size)
	{
		edge.rectTransform.anchoredPosition = anchoredPosition;
		edge.rectTransform.sizeDelta = size;
		edge.image.enabled = true;
	}

	private void HideBand()
	{
		if (bandNode == null)
		{
			return;
		}
		bandNode.image.enabled = false;
		TreeNodeRender[] array = bandEdges;
		foreach (TreeNodeRender treeNodeRender in array)
		{
			if (treeNodeRender != null)
			{
				treeNodeRender.image.enabled = false;
			}
		}
	}

	private static Rect GetElementWorldRect(PixelElement element)
	{
		RectTransform rectTransform = element.handler.rectTransform;
		Vector3 position = rectTransform.position;
		Vector3 lossyScale = rectTransform.lossyScale;
		float num = (float)element.widthPixels * lossyScale.x;
		float num2 = (float)element.heightPixels * lossyScale.y;
		return Rect.MinMaxRect(position.x, position.y - num2, position.x + num, position.y);
	}

	private static bool BandOverlapsItemShape(GameItemElement element, Rect bandRect, Rect clipRect, bool hasClip, out Vector2 sweptCenter)
	{
		sweptCenter = default(Vector2);
		GridShape modifiedShape = element.modifiedShape;
		if (modifiedShape == null)
		{
			return false;
		}
		Vector3 position = element.middle.rectTransform.position;
		Vector3 lossyScale = element.middle.rectTransform.lossyScale;
		float num = (float)UISettings.current.gridSize * lossyScale.x;
		float num2 = (float)UISettings.current.gridSize * lossyScale.y;
		for (int i = 0; i < modifiedShape.height; i++)
		{
			for (int j = 0; j < modifiedShape.width; j++)
			{
				if (modifiedShape.GetLocal(j, i) == 0)
				{
					continue;
				}
				(int, int) tuple = modifiedShape.ForwardTransform(j, i);
				int item = tuple.Item1;
				int item2 = tuple.Item2;
				float num3 = position.x + (float)(item - modifiedShape.minX) * num;
				float num4 = position.y - (float)(item2 - modifiedShape.minY) * num2;
				float num5 = num3 + num;
				float num6 = num4 - num2;
				if (hasClip)
				{
					num3 = Mathf.Max(num3, clipRect.xMin);
					num6 = Mathf.Max(num6, clipRect.yMin);
					num5 = Mathf.Min(num5, clipRect.xMax);
					num4 = Mathf.Min(num4, clipRect.yMax);
					if (num5 <= num3 || num4 <= num6)
					{
						continue;
					}
				}
				if (!(bandRect.xMin >= num5) && !(bandRect.xMax <= num3) && !(bandRect.yMin >= num4) && !(bandRect.yMax <= num6))
				{
					sweptCenter = new Vector2((Mathf.Max(bandRect.xMin, num3) + Mathf.Min(bandRect.xMax, num5)) / 2f, (Mathf.Max(bandRect.yMin, num6) + Mathf.Min(bandRect.yMax, num4)) / 2f);
					return true;
				}
			}
		}
		return false;
	}

	private List<GameItemElement> ComputeBandItems(Rect bandRect)
	{
		List<GameItemElement> list = new List<GameItemElement>();
		List<GameItem> list2 = new List<GameItem>();
		bool flag = false;
		GraphNodeStorage graphNodeStorage = bandWindow;
		if (graphNodeStorage != null)
		{
			list2.AddRange(GraphUtils.FindChildrenType<GameItem>(graphNodeStorage));
		}
		else if (bandInventory != null)
		{
			list2.AddRange(bandInventory.childItems);
		}
		else
		{
			flag = true;
			foreach (PixelWindow visibleWindow in WindowsHandler.current.visibleWindows)
			{
				if (visibleWindow != null)
				{
					list2.AddRange(GraphUtils.FindChildrenType<GameItem>(visibleWindow));
				}
			}
		}
		foreach (GameItem item in list2)
		{
			if (item is GameItemElement gameItemElement && !gameItemElement.IsDestroyed() && gameItemElement.parentInventory != null && !gameItemElement.isDebugMenu && gameItemElement.MayRemove() && gameItemElement.MaxNumRemove() >= 1 && !ItemPinHelper.IsPinned(gameItemElement) && !(gameItemElement.handler == null) && gameItemElement.handler.gameObject.activeInHierarchy)
			{
				Rect clipRect = default(Rect);
				bool hasClip = false;
				if (gameItemElement.parentInventory is PixelElement element)
				{
					clipRect = GetElementWorldRect(element);
					hasClip = clipRect.width > 0f && clipRect.height > 0f;
				}
				if (BandOverlapsItemShape(gameItemElement, bandRect, clipRect, hasClip, out var sweptCenter) && !list.Contains(gameItemElement) && (!flag || RenderHandler.RaycastElement<GameItem>(sweptCenter) == item))
				{
					list.Add(gameItemElement);
				}
			}
		}
		return list;
	}

	private void FinalizeBand()
	{
		HideBand();
		bandWindow = null;
		bandInventory = null;
		if (selectedItems.Count == 0)
		{
			ClearSelection();
			state = State.Idle;
			return;
		}
		state = State.Selected;
		selectedHomes.Clear();
		foreach (GameItemElement selectedItem in selectedItems)
		{
			selectedHomes[selectedItem] = selectedItem.parentInventory;
		}
		RefreshSelectionHighlight();
		if (TutorialUIManager.Instance != null)
		{
			TutorialUIManager.Instance.OnMultiSelect();
		}
	}

	private void ClearSelectionHighlightNodes()
	{
		foreach (TreeNodeRender selectionHighlight in selectionHighlights)
		{
			if (selectionHighlight != null)
			{
				selectionHighlight.image.enabled = false;
			}
		}
		selectionHighlights.Clear();
	}

	private void RefreshSelectionHighlight()
	{
		ClearSelectionHighlightNodes();
		foreach (GameItemElement selectedItem in selectedItems)
		{
			SlotMarker slotMarker = selectedItem.parentInventory?.GetIncompleteItemInventorySlot(selectedItem);
			if (slotMarker != null)
			{
				List<TreeNodeRender> list = selectedItem.parentInventory.HighlightSlot(slotMarker, RenderHandler.BackgroundColor.Blue, doReplace: true);
				if (list != null)
				{
					selectionHighlights.AddRange(list);
				}
			}
		}
	}

	private void ClearSelection()
	{
		ClearSelectionHighlightNodes();
		selectedItems.Clear();
		dragOffsets.Clear();
		selectedHomes.Clear();
	}

	private void StartGroupDrag()
	{
		selectedItems.RemoveAll((GameItemElement e) => e.parentInventory == null || !e.MayRemove() || e.MaxNumRemove() < 1);
		if (selectedItems.Count == 0)
		{
			ClearSelection();
			state = State.Idle;
			return;
		}
		dragOffsets.Clear();
		Vector2 vector = RenderHandler.current.overlayRoot.rectTransform.InverseTransformPoint(Input.mousePosition);
		foreach (GameItemElement selectedItem in selectedItems)
		{
			selectedItem.contentWindow?.Hide();
			selectedItem.examineWindow?.Hide();
			selectedItem.UpdateFakeReal(selectedItem.MaxNumRemove());
			selectedItem.ToggleFakeBackground(enabled: true, UISettings.current.dragOpacity);
			selectedItem.tempShape.Set(selectedItem.modifiedShape);
			selectedItem.fakeBackground.image.raycastTarget = false;
			var (x, y) = selectedItem.GetCenteredPosition();
			dragOffsets[selectedItem] = new Vector2(x, y) - vector;
		}
		state = State.GroupDragging;
		InputActionManager.current.ResetAfter(this);
		AudioManager.Instance.TryPlayOne(selectedItems[0].soundDragStart, selectedItems[0].soundDragEnd);
	}

	private void MoveGhosts()
	{
		Vector2 vector = RenderHandler.current.overlayRoot.rectTransform.InverseTransformPoint(Input.mousePosition);
		float num = Mathf.Clamp(dragSnapAnimationSpeed * Time.deltaTime, 0f, 1f);
		foreach (GameItemElement selectedItem in selectedItems)
		{
			if (dragOffsets.TryGetValue(selectedItem, out var value))
			{
				Vector2 vector2 = vector + value;
				var (num2, num3) = selectedItem.GetCenteredPosition();
				selectedItem.SetCenteredPosition(vector2.x * num + num2 * (1f - num), vector2.y * num + num3 * (1f - num));
			}
		}
	}

	private void UpdateDropTarget()
	{
		Vector3 mousePosition = Input.mousePosition;
		GameItem item = RenderHandler.RaycastElement<GameItem>(mousePosition);
		if (item is GameItemElement item2 && selectedItems.Contains(item2))
		{
			item = null;
		}
		GameInventory inventory = null;
		if (item == null)
		{
			inventory = RenderHandler.RaycastFindParentElement<GameInventory>(mousePosition);
		}
		GameInventory commonHomeInventory = GetCommonHomeInventory();
		bool flag = item == null && inventory != null && inventory.SupportsGroupPlacement && commonHomeInventory != null && (inventory == commonHomeInventory || IsPreciseModifierHeld());
		if (item == hoverItem && inventory == hoverInventory && flag == precisePlacementActive)
		{
			if (flag)
			{
				RefreshPrecisePreview(inventory);
			}
			return;
		}
		foreach (TreeNodeRender hoverHighlightNode in hoverHighlightNodes)
		{
			if (hoverHighlightNode != null)
			{
				hoverHighlightNode.image.enabled = false;
			}
		}
		hoverHighlightNodes.Clear();
		foreach (TreeNodeRender hoverInventoryNode in hoverInventoryNodes)
		{
			if (hoverInventoryNode != null)
			{
				hoverInventoryNode.image.enabled = false;
			}
		}
		hoverInventoryNodes.Clear();
		ClearPrecisePreviewNodes();
		if (flag)
		{
			RefreshPrecisePreview(inventory);
		}
		else if (inventory != null)
		{
			bool flag2 = GeneralHelper.MayPlayerInsertInto(inventory) && selectedItems.Any((GameItemElement s) => inventory.MayHaveValidInventorySlot(s));
			List<TreeNodeRender> list = inventory.HighlightWholeInventory(flag2 ? RenderHandler.BackgroundColor.Green : RenderHandler.BackgroundColor.Red);
			if (list != null)
			{
				hoverInventoryNodes.AddRange(list);
			}
		}
		if (item != null)
		{
			bool flag3 = (GeneralHelper.MayPlayerInsertInto(item) && selectedItems.Any((GameItemElement s) => item.MayHaveValidInventorySlot(s))) || selectedItems.Any((GameItemElement s) => item.MayStack(s)) || selectedItems.Any((GameItemElement s) => s.MayTarget(item) && s.CanTarget(item));
			SlotMarker incompleteSlotInInventory = item.GetIncompleteSlotInInventory();
			List<TreeNodeRender> list2 = incompleteSlotInInventory?.inventory?.HighlightSlot(incompleteSlotInInventory, flag3 ? RenderHandler.BackgroundColor.Green : RenderHandler.BackgroundColor.Red, doReplace: true);
			if (list2 != null)
			{
				hoverHighlightNodes.AddRange(list2);
			}
		}
		hoverItem = item;
		hoverInventory = inventory;
		precisePlacementActive = flag;
	}

	private GameInventory GetCommonHomeInventory()
	{
		GameInventory gameInventory = null;
		foreach (GameItemElement selectedItem in selectedItems)
		{
			if (selectedItem.parentInventory == null)
			{
				return null;
			}
			if (gameInventory == null)
			{
				gameInventory = selectedItem.parentInventory;
			}
			else if (gameInventory != selectedItem.parentInventory)
			{
				return null;
			}
		}
		return gameInventory;
	}

	private bool TryGetPreciseDelta(GameInventory inventory, out int deltaX, out int deltaY)
	{
		deltaX = 0;
		deltaY = 0;
		if (selectedItems.Count == 0)
		{
			return false;
		}
		GameItemElement gameItemElement = selectedItems[0];
		if (gameItemElement.IsDestroyed())
		{
			return false;
		}
		SlotMarker slotMarker = inventory.TryInventorySlot(gameItemElement, gameItemElement.unitCount, gameItemElement.GetFakeRotatorCenteredPosition(), gameItemElement.tempShape, gameItemElement.modifiedState);
		if (slotMarker?.itemGridShape == null)
		{
			return false;
		}
		deltaX = slotMarker.itemGridShape.minX - gameItemElement.modifiedShape.minX;
		deltaY = slotMarker.itemGridShape.minY - gameItemElement.modifiedShape.minY;
		return true;
	}

	private static GridShape TranslatedShape(GameItemElement element, int deltaX, int deltaY)
	{
		GridShape modifiedShape = element.modifiedShape;
		return new GridShapeBuilder(modifiedShape).SetPosition(modifiedShape.minX + deltaX, modifiedShape.minY + deltaY).Build();
	}

	private static bool IsPreciseModifierHeld()
	{
		if (!Input.GetKey(KeyCode.LeftShift))
		{
			return Input.GetKey(KeyCode.RightShift);
		}
		return true;
	}

	private bool IsFormationValid(GameInventory inventory, int deltaX, int deltaY)
	{
		if (selectedItems.Count == 0)
		{
			return false;
		}
		if (inventory.IsInsertLocked())
		{
			return false;
		}
		bool flag = inventory != GetCommonHomeInventory();
		if (flag && !GeneralHelper.MayPlayerInsertInto(inventory))
		{
			return false;
		}
		List<GameItem> ignoredItems = new List<GameItem>(selectedItems);
		foreach (GameItemElement selectedItem in selectedItems)
		{
			if (selectedItem.IsDestroyed() || selectedItem.parentInventory == null)
			{
				return false;
			}
			if (!selectedItem.MayRemove() || selectedItem.MaxNumRemove() < selectedItem.unitCount)
			{
				return false;
			}
			if (flag && !inventory.MayHaveValidInventorySlot(selectedItem))
			{
				return false;
			}
			if (!inventory.CheckShapePlacement(selectedItem, TranslatedShape(selectedItem, deltaX, deltaY), ignoredItems))
			{
				return false;
			}
		}
		return true;
	}

	private void RefreshPrecisePreview(GameInventory inventory)
	{
		ClearPrecisePreviewNodes();
		if (!TryGetPreciseDelta(inventory, out var deltaX, out var deltaY))
		{
			return;
		}
		RenderHandler.BackgroundColor color = (IsFormationValid(inventory, deltaX, deltaY) ? RenderHandler.BackgroundColor.Green : RenderHandler.BackgroundColor.Red);
		foreach (GameItemElement selectedItem in selectedItems)
		{
			SlotMarker slotMarker = new SlotMarker();
			slotMarker.item = selectedItem;
			slotMarker.inventory = inventory;
			slotMarker.itemGridShape = TranslatedShape(selectedItem, deltaX, deltaY);
			List<TreeNodeRender> list = inventory.HighlightSlot(slotMarker, color, doReplace: true);
			if (list != null)
			{
				precisePreviewNodes.AddRange(list);
			}
		}
	}

	private void ClearPrecisePreviewNodes()
	{
		foreach (TreeNodeRender precisePreviewNode in precisePreviewNodes)
		{
			if (precisePreviewNode != null)
			{
				precisePreviewNode.image.enabled = false;
			}
		}
		precisePreviewNodes.Clear();
	}

	private void CommitPrecisePlacement(GameInventory inventory, int deltaX, int deltaY)
	{
		List<GameItemElement> list = new List<GameItemElement>(selectedItems);
		foreach (GameItemElement item in list)
		{
			if (!item.IsDestroyed())
			{
				item.parentInventory?.Expel(item);
			}
		}
		foreach (GameItemElement item2 in list)
		{
			if (!item2.IsDestroyed())
			{
				item2.modifiedShape = new GridShapeBuilder(item2.modifiedShape).SetTransform(TranslatedShape(item2, deltaX, deltaY));
				inventory.UncheckedAccept(item2);
			}
		}
	}

	private void ClearDropTarget()
	{
		foreach (TreeNodeRender hoverHighlightNode in hoverHighlightNodes)
		{
			if (hoverHighlightNode != null)
			{
				hoverHighlightNode.image.enabled = false;
			}
		}
		hoverHighlightNodes.Clear();
		foreach (TreeNodeRender hoverInventoryNode in hoverInventoryNodes)
		{
			if (hoverInventoryNode != null)
			{
				hoverInventoryNode.image.enabled = false;
			}
		}
		hoverInventoryNodes.Clear();
		ClearPrecisePreviewNodes();
		precisePlacementActive = false;
		hoverItem = null;
		hoverInventory = null;
	}

	private void EndGroupDrag()
	{
		Vector3 mousePosition = Input.mousePosition;
		GameItem gameItem = RenderHandler.RaycastElement<GameItem>(mousePosition);
		if (gameItem is GameItemElement item && selectedItems.Contains(item))
		{
			gameItem = null;
		}
		GameInventory targetInventory = null;
		if (gameItem == null)
		{
			targetInventory = RenderHandler.RaycastFindParentElement<GameInventory>(mousePosition);
		}
		bool flag = false;
		if (gameItem == null && targetInventory == null)
		{
			targetInventory = MachineryTabs.FindDropInventoryAt(mousePosition);
			flag = targetInventory != null;
		}
		List<GameItemElement> list = new List<GameItemElement>(selectedItems);
		string soundDragStart = list[0].soundDragStart;
		string soundDragEnd = list[0].soundDragEnd;
		bool flag2 = false;
		if (gameItem == null && targetInventory != null && targetInventory.SupportsGroupPlacement && !flag)
		{
			GameInventory commonHomeInventory = GetCommonHomeInventory();
			if (commonHomeInventory != null && (commonHomeInventory == targetInventory || IsPreciseModifierHeld()))
			{
				if (TryGetPreciseDelta(targetInventory, out var deltaX, out var deltaY) && IsFormationValid(targetInventory, deltaX, deltaY))
				{
					CommitPrecisePlacement(targetInventory, deltaX, deltaY);
				}
				flag2 = true;
			}
		}
		if (!flag2)
		{
			foreach (GameItemElement item2 in list)
			{
				if (item2.IsDestroyed() || item2.parentInventory == null || !item2.MayRemove() || item2.MaxNumRemove() < 1)
				{
					continue;
				}
				if (gameItem is GameItemElement gameItemElement2 && gameItemElement2.IsDestroyed())
				{
					gameItem = null;
				}
				if (gameItem != null && item2 != gameItem)
				{
					if (GeneralHelper.MayPlayerInsertInto(gameItem) && gameItem.MayHaveValidInventorySlot(item2))
					{
						GraphUtils.TryAcceptAll(gameItem, item2);
					}
					else if (gameItem.MayStack(item2))
					{
						gameItem.GetStackSlot(item2)?.TryAcceptOnce();
					}
					else if (item2.MayTarget(gameItem) && item2.CanTarget(gameItem))
					{
						ItemBehaviourManager.current.Target(item2, gameItem);
						if ((!(gameItem is GameItemElement gameItemElement3) || !gameItemElement3.IsDestroyed()) && !item2.IsDestroyed() && item2.MayTarget(gameItem) && item2.CanTarget(gameItem))
						{
							item2.Target(gameItem);
						}
					}
				}
				else if (targetInventory != null && GeneralHelper.MayPlayerInsertInto(targetInventory) && item2.parentInventory != targetInventory)
				{
					GraphUtils.TryAcceptAll(targetInventory, item2);
				}
			}
		}
		RestoreGhosts();
		ClearDropTarget();
		if (targetInventory != null)
		{
			WindowsHandler.current.visibleWindows.FindAll((PixelWindow w) => GraphUtils.FindChildEqualsIgnoreType<GameItem>(w, targetInventory)).ForEach(delegate(PixelWindow w)
			{
				w.ToFront();
			});
		}
		ClearSelection();
		state = State.Idle;
		AudioManager.Instance.TryPlayOne(soundDragEnd, soundDragStart);
	}

	private void RestoreGhosts()
	{
		foreach (GameItemElement selectedItem in selectedItems)
		{
			if (selectedItem != null && !selectedItem.IsDestroyed())
			{
				selectedItem.fakeBackground.image.raycastTarget = true;
				selectedItem.ToggleFakeBackground(enabled: false);
				selectedItem.Validate();
				if (selectedItem.parentInventory == null)
				{
					selectedItem.Destroy();
				}
			}
		}
		dragOffsets.Clear();
	}
}
