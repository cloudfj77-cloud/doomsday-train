using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSelectHandler : EmptyActionHandler, Cancelable
{
	public static ItemSelectHandler current;

	[SerializeField]
	public KeyCode key;

	[SerializeField]
	public Vector2 tooltipTranslation = new Vector3(32f, 0f);

	[SerializeField]
	public Vector2 equipCursorTranslation = new Vector2(-12f, -12f);

	public bool auto = true;

	private GameItem currentItem;

	private HighlightManager highlightManager = new HighlightManager();

	private List<TreeNodeRender> highlightedMayNodes = new List<TreeNodeRender>();

	private bool isEquipped;

	private TreeNodeRender equipCursor;

	private readonly List<TreeNodeRender> equipCursorOverlays = new List<TreeNodeRender>();

	private RichTextBuilder builder = new RichTextBuilder(60);

	private GameItem lastItem;

	public override List<KeyCode> keyListeners => new KeyCode[2] { key, secondaryKey }.ToList();

	public KeyCode secondaryKey => KeyBindHelper.Get("secondary_click");

	public bool IsEquipped => isEquipped;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		current = this;
	}

	public GameItem GetEquippedItem()
	{
		if (!isEquipped)
		{
			return null;
		}
		return currentItem;
	}

	public void TrySelectItem(GameItem newItem)
	{
		if (newItem != null && currentItem == null && newItem.MaySelectSlotItem() && newItem.CanSelectSlotItem())
		{
			SelectItem(newItem);
		}
	}

	public void TryEquipItem(GameItem newItem)
	{
		if (newItem != null && GeneralHelper.IsItemEffectivelyOwned(newItem) && newItem.MaySelectSlotItem() && newItem.CanSelectSlotItem())
		{
			if (!base.enabled && StoreUIManager.Instance != null && StoreUIManager.Instance.hoveringStoreUI)
			{
				Enable();
			}
			OnEventReset();
			SelectItem(newItem);
			isEquipped = true;
			ShowEquipCursor(newItem);
			UpdateEquipCursorPosition();
		}
	}

	private void SelectItem(GameItem newItem)
	{
		InputActionManager.current.ResetAfter(this);
		currentItem = newItem;
		RefreshHighlights(newItem);
		AudioManager.Instance.TryPlayOne(currentItem.soundSelect, currentItem.soundDragStart, currentItem.soundDragEnd);
	}

	private void RefreshHighlights(GameItem sourceItem)
	{
		highlightManager.Clear();
		ClearMayHighlights();
		SlotMarker incompleteSlotInInventory = sourceItem.GetIncompleteSlotInInventory();
		highlightManager.Try(incompleteSlotInInventory, RenderHandler.BackgroundColor.Green);
		List<GameItem> list = new List<GameItem>();
		foreach (PixelWindow visibleWindow in WindowsHandler.current.visibleWindows)
		{
			list.AddRange(GraphUtils.FindChildrenType(visibleWindow, (GameItem r) => sourceItem.MayTarget(r)));
		}
		foreach (GameItem item in list)
		{
			List<TreeNodeRender> list2 = item.parentInventory?.HighlightSlot(item.GetIncompleteSlotInInventory(), RenderHandler.BackgroundColor.Blue, doReplace: true);
			if (list2 != null)
			{
				highlightedMayNodes.AddRange(list2);
			}
		}
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

	private void ShowEquipCursor(GameItem item)
	{
		if (equipCursor == null)
		{
			equipCursor = TreeNodeRender.Instantiate().SetParentOverlayRoot();
			equipCursor.gameObject.name = "EquipCursorNode";
			equipCursorOverlays.Clear();
		}
		GameItemElement gameItemElement = item as GameItemElement;
		Sprite sprite = gameItemElement?.background.image.sprite;
		if (!(sprite == null))
		{
			equipCursor.image.sprite = sprite;
			equipCursor.image.SetNativeSize();
			equipCursor.image.color = new Color(1f, 1f, 1f, UISettings.current.dragOpacity);
			equipCursor.image.raycastTarget = false;
			equipCursor.image.enabled = true;
			equipCursor.rectTransform.SetAsLastSibling();
			CopyEquipCursorOverlays(gameItemElement);
		}
	}

	private void CopyEquipCursorOverlays(GameItemElement element)
	{
		int num = 0;
		foreach (TreeNodeRender activeInnateSpriteMod in element.GetActiveInnateSpriteMods())
		{
			if (num >= equipCursorOverlays.Count)
			{
				TreeNodeRender treeNodeRender2 = TreeNodeRender.Instantiate().SetParent(equipCursor);
				treeNodeRender2.gameObject.name = "EquipCursorOverlay";
				treeNodeRender2.image.raycastTarget = false;
				equipCursorOverlays.Add(treeNodeRender2);
			}
			TreeNodeRender treeNodeRender3 = equipCursorOverlays[num];
			num++;
			treeNodeRender3.image.sprite = activeInnateSpriteMod.image.sprite;
			treeNodeRender3.image.SetNativeSize();
			Color color = activeInnateSpriteMod.image.color;
			color.a *= UISettings.current.dragOpacity;
			treeNodeRender3.image.color = color;
			RectTransform rectTransform = activeInnateSpriteMod.rectTransform;
			RectTransform rectTransform2 = treeNodeRender3.rectTransform;
			rectTransform2.anchorMin = rectTransform.anchorMin;
			rectTransform2.anchorMax = rectTransform.anchorMax;
			rectTransform2.pivot = rectTransform.pivot;
			rectTransform2.localScale = rectTransform.localScale;
			rectTransform2.localEulerAngles = rectTransform.localEulerAngles;
			rectTransform2.anchoredPosition = rectTransform.anchoredPosition - element.background.rectTransform.anchoredPosition;
			treeNodeRender3.image.enabled = true;
			treeNodeRender3.gameObject.SetActive(value: true);
		}
		for (int i = num; i < equipCursorOverlays.Count; i++)
		{
			equipCursorOverlays[i].gameObject.SetActive(value: false);
		}
	}

	private void HideEquipCursor()
	{
		if (equipCursor != null && equipCursor.image != null)
		{
			equipCursor.image.enabled = false;
		}
		foreach (TreeNodeRender equipCursorOverlay in equipCursorOverlays)
		{
			if (equipCursorOverlay != null)
			{
				equipCursorOverlay.gameObject.SetActive(value: false);
			}
		}
	}

	private void UpdateEquipCursorPosition()
	{
		if (!(equipCursor == null))
		{
			Vector2 vector = equipCursorTranslation - new Vector2(equipCursor.rectTransform.sizeDelta.x, 0f);
			equipCursor.rectTransform.anchoredPosition = (Vector2)equipCursor.rectTransform.parent.InverseTransformPoint(Input.mousePosition) + vector;
			equipCursor.rectTransform.position = new Vector3(Mathf.RoundToInt(equipCursor.rectTransform.position.x), Mathf.RoundToInt(equipCursor.rectTransform.position.y), equipCursor.rectTransform.position.z);
		}
	}

	private bool ShouldExitEquip()
	{
		if (currentItem == null)
		{
			return true;
		}
		if (currentItem is GameItemElement gameItemElement && gameItemElement.IsDestroyed())
		{
			return true;
		}
		if (!GeneralHelper.IsItemEffectivelyOwned(currentItem))
		{
			return true;
		}
		GameItem gameItem = currentItem;
		if (gameItem.MaySelectSlotItem())
		{
			return !gameItem.CanSelectSlotItem();
		}
		return true;
	}

	private void ContinueEquipAfterUse()
	{
		if (ShouldExitEquip())
		{
			OnEventReset();
			return;
		}
		lastItem = null;
		RefreshHighlights(currentItem);
		ShowEquipCursor(currentItem);
		UpdateEquipCursorPosition();
	}

	public bool Cancel()
	{
		if (isEquipped)
		{
			OnEventReset();
			return true;
		}
		return false;
	}

	protected override void OnEventPress(ISet<KeyCode> keysPressed)
	{
		if (keysPressed.Contains(secondaryKey))
		{
			if (isEquipped)
			{
				OnEventReset();
			}
		}
		else
		{
			if (!keysPressed.Contains(key) || (StoreUIManager.Instance != null && StoreUIManager.Instance.hoveringStoreUI))
			{
				return;
			}
			GameItem gameItem = currentItem;
			GameItem gameItem2 = RenderHandler.RaycastElement<GameItem>(Input.mousePosition);
			GameItem gameItem3 = gameItem2;
			if (auto && gameItem2 != null && currentItem == null && gameItem3.MaySelectSlotItem() && gameItem3.CanSelectSlotItem())
			{
				SelectItem(gameItem2);
				ConsumeClickPress();
			}
			else if (gameItem2 != null && currentItem != null && currentItem != gameItem2)
			{
				ItemBehaviourManager.current.Target(gameItem, gameItem2);
				if (gameItem.MayTarget(gameItem2) && gameItem.CanTarget(gameItem2))
				{
					gameItem.Target(gameItem2);
				}
				ConsumeClickPress();
				if (isEquipped)
				{
					ContinueEquipAfterUse();
				}
				else
				{
					OnEventReset();
				}
			}
			else if (gameItem != null && currentItem == gameItem2 && gameItem.MayActivateSlotItem())
			{
				if (gameItem.MayActivateSlotItem() && gameItem.CanActivateSlotItem())
				{
					gameItem.ActivateSlotItem();
				}
				ConsumeClickPress();
				if (isEquipped)
				{
					ContinueEquipAfterUse();
				}
				else
				{
					OnEventReset();
				}
			}
			else if (currentItem != gameItem2 && !isEquipped)
			{
				OnEventReset();
			}
		}
	}

	private static void ConsumeClickPress()
	{
		if (ItemMouseDoubleClickHandler.current != null)
		{
			ItemMouseDoubleClickHandler.current.ConsumePress();
		}
	}

	protected override bool OnEventUpdate()
	{
		if (isEquipped)
		{
			if (ShouldExitEquip())
			{
				OnEventReset();
				return false;
			}
			UpdateEquipCursorPosition();
			if (StoreUIManager.Instance != null && StoreUIManager.Instance.hoveringStoreUI)
			{
				OverlayHandler.current.HideTooltip();
				return true;
			}
		}
		if (currentItem != null)
		{
			GameItem gameItem = currentItem;
			GameItem gameItem2 = RenderHandler.RaycastElement<GameItem>(Input.mousePosition);
			if (gameItem2 != lastItem)
			{
				if (lastItem != null && gameItem.MayTarget(lastItem))
				{
					lastItem.parentInventory?.HighlightSlot(lastItem.GetIncompleteSlotInInventory(), RenderHandler.BackgroundColor.Blue, doReplace: true);
				}
				if (gameItem2 != null && gameItem.MayTarget(gameItem2))
				{
					gameItem2.parentInventory?.HighlightSlot(gameItem2.GetIncompleteSlotInInventory(), gameItem.CanTarget(gameItem2) ? RenderHandler.BackgroundColor.Green : RenderHandler.BackgroundColor.Red, doReplace: true);
				}
			}
			lastItem = gameItem2;
			if (gameItem2 != null)
			{
				string displayName = gameItem.GetDisplayName(preferCustomName: true);
				string displayName2 = gameItem2.GetDisplayName(preferCustomName: true);
				builder.Clear();
				RichTextBuilder richTextBuilder = ItemBehaviourManager.current.TargetTooltip(gameItem, gameItem2);
				if (richTextBuilder != null)
				{
					builder.Add(richTextBuilder);
				}
				if (gameItem == gameItem2 && gameItem.MayActivateSlotItem() && gameItem.CanActivateSlotItem())
				{
					builder.Add("Use and activate ").Add(displayName, RenderHandler.ColorPalette.None, "", bold: true, italic: false, underline: true);
				}
				else if (gameItem.MayTarget(gameItem2) && gameItem.CanTarget(gameItem2))
				{
					if (TooltipUseHelper.HasUseTooltip(gameItem, gameItem2))
					{
						TooltipUseHelper.HandleUseTooltip(gameItem, gameItem2, builder);
					}
					else
					{
						builder.Add(LocHelper.GetLocalizedMechanic("mech_drag_use_on", "<b>" + displayName + "</b>", "<b><u>" + displayName2 + "</u></b>"));
					}
					AbilityItemHelper.CreateHoverTargetTooltip(builder, currentItem, gameItem2);
				}
				else if (gameItem != gameItem2)
				{
					builder.Add(LocHelper.GetLocalizedMechanic("mech_drag_cant_use_on", displayName, displayName2), RenderHandler.ColorPalette.Red);
				}
				else if (gameItem == gameItem2 && gameItem.MayActivateSlotItem() && !gameItem.CanActivateSlotItem())
				{
					builder.Add("Cannot activate " + displayName + "!", RenderHandler.ColorPalette.Red);
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
			else
			{
				OverlayHandler.current.HideTooltip();
			}
		}
		return currentItem != null;
	}

	protected override void OnEventReset()
	{
		if (isEquipped && currentItem != null)
		{
			AudioManager.Instance.TryPlayOne(currentItem.soundDragEnd, currentItem.soundDragStart);
		}
		highlightManager.Clear();
		if (currentItem != null)
		{
			OverlayHandler.current.HideTooltip();
		}
		currentItem = null;
		ClearMayHighlights();
		isEquipped = false;
		HideEquipCursor();
	}

	public void EndSelect(bool nonOwnedOnly = false)
	{
		if (nonOwnedOnly)
		{
			if (currentItem != null && !GeneralHelper.IsItemEffectivelyOwned(currentItem))
			{
				OnEventReset();
			}
		}
		else
		{
			OnEventReset();
		}
	}
}
