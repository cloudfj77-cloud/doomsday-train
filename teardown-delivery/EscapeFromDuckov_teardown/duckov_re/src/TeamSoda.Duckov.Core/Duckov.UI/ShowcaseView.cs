using Duckov.Buildings;
using Duckov.UI.Animations;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duckov.UI;

public class ShowcaseView : View
{
	[SerializeField]
	private FadeGroup fadeGroup;

	[SerializeField]
	private InventoryDisplay inventoryDisplay;

	[SerializeField]
	private InventoryDisplay inventoryDisplay_PlayerStorage;

	[SerializeField]
	private InventoryDisplay inventoryDisplay_Target;

	[SerializeField]
	private FadeGroup detailsDisplayFadeGroup;

	[SerializeField]
	private ItemDetailsDisplay detailsDisplay;

	[SerializeField]
	private ItemSlotCollectionDisplay slotsDisplay;

	[SerializeField]
	private TextMeshProUGUI nameDisplay;

	private Showcase target;

	protected override void Awake()
	{
		base.Awake();
		ItemUIUtilities.OnSelectionChanged += OnSelectionChanged;
		inventoryDisplay.onDisplayDoubleClicked += OnPlayerItemDoubleClicked;
		inventoryDisplay_PlayerStorage.onDisplayDoubleClicked += OnPlayerItemDoubleClicked;
		slotsDisplay.onElementDoubleClicked += OnTargetSlotDoubleClicked;
		inventoryDisplay_Target.onDisplayDoubleClicked += OnTargetItemDoubleClicked;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		ItemUIUtilities.OnSelectionChanged -= OnSelectionChanged;
		inventoryDisplay.onDisplayDoubleClicked -= OnPlayerItemDoubleClicked;
		inventoryDisplay_PlayerStorage.onDisplayDoubleClicked -= OnPlayerItemDoubleClicked;
		slotsDisplay.onElementDoubleClicked -= OnTargetSlotDoubleClicked;
		inventoryDisplay_Target.onDisplayDoubleClicked -= OnTargetItemDoubleClicked;
	}

	private void OnTargetSlotDoubleClicked(ItemSlotCollectionDisplay display1, SlotDisplay slotDisplay)
	{
		if (slotDisplay == null)
		{
			return;
		}
		Slot slot = slotDisplay.Target;
		if (slot == null)
		{
			return;
		}
		Item content = slot.Content;
		if (!(content == null))
		{
			content.Detach();
			if (!ItemUtilities.SendToPlayerCharacter(content) && !target.Item.TryPlug(content, emptyOnly: true))
			{
				content.Drop(CharacterMainControl.Main, createRigidbody: true);
			}
		}
	}

	private void OnTargetItemDoubleClicked(InventoryDisplay display, InventoryEntry entry, PointerEventData data)
	{
		if (entry == null)
		{
			return;
		}
		Item content = entry.Content;
		if (!(content == null))
		{
			content.Detach();
			if (!ItemUtilities.SendToPlayerCharacter(content) && !target.Item.Inventory.AddAndMerge(content))
			{
				content.Drop(CharacterMainControl.Main, createRigidbody: true);
			}
		}
	}

	private void OnPlayerItemDoubleClicked(InventoryDisplay display, InventoryEntry entry, PointerEventData data)
	{
		if (entry == null)
		{
			return;
		}
		Item content = entry.Content;
		if (content == null || target == null)
		{
			return;
		}
		Item item = target.Item;
		if (!(item == null))
		{
			content.Detach();
			if (!item.TryPlug(content, emptyOnly: true) && (item.Inventory == null || !item.Inventory.AddAndMerge(content)) && !ItemUtilities.SendToPlayerCharacterInventory(content))
			{
				content.Drop(CharacterMainControl.Main, createRigidbody: true);
			}
		}
	}

	private void OnSelectionChanged()
	{
		if (ItemUIUtilities.SelectedItem == null)
		{
			detailsDisplayFadeGroup.Hide();
			return;
		}
		detailsDisplay.Setup(ItemUIUtilities.SelectedItem);
		detailsDisplayFadeGroup.Show();
	}

	protected override void OnOpen()
	{
		base.OnOpen();
		if (!LevelManager.LevelInited || target == null)
		{
			Close();
			return;
		}
		inventoryDisplay.Setup(CharacterMainControl.Main.CharacterItem.Inventory);
		if (PlayerStorage.Inventory == null)
		{
			inventoryDisplay_PlayerStorage.gameObject.SetActive(value: false);
		}
		else
		{
			inventoryDisplay_PlayerStorage.Setup(PlayerStorage.Inventory);
			inventoryDisplay_PlayerStorage.gameObject.SetActive(value: true);
		}
		inventoryDisplay_Target.gameObject.SetActive(target.Inventory != null);
		if (target.Inventory != null)
		{
			inventoryDisplay_Target.Setup(target.Inventory);
		}
		ItemUIUtilities.Select(null);
		detailsDisplayFadeGroup.SkipHide();
		slotsDisplay.Setup(target.Slots.Master);
		nameDisplay.text = target.DisplayName;
		if (target.Vcam != null)
		{
			target.Vcam.gameObject.SetActive(value: true);
		}
		fadeGroup.Show();
		UIInputManager.OnNextPage += OnNextPage;
		UIInputManager.OnPreviousPage += OnPreviousPage;
	}

	protected override void OnClose()
	{
		base.OnClose();
		if (target != null && target.Vcam != null)
		{
			target.Vcam.gameObject.SetActive(value: false);
		}
		fadeGroup.Hide();
		UIInputManager.OnNextPage -= OnNextPage;
		UIInputManager.OnPreviousPage -= OnPreviousPage;
	}

	private void OnPreviousPage(UIInputEventData data)
	{
		if (inventoryDisplay_PlayerStorage.UsePages)
		{
			inventoryDisplay_PlayerStorage.PreviousPage();
		}
	}

	private void OnNextPage(UIInputEventData data)
	{
		if (inventoryDisplay_PlayerStorage.UsePages)
		{
			inventoryDisplay_PlayerStorage.NextPage();
		}
	}

	public static void Show(Showcase target)
	{
		ShowcaseView viewInstance = View.GetViewInstance<ShowcaseView>();
		if (viewInstance == null)
		{
			Debug.Log("Showcase view not found");
			return;
		}
		viewInstance.target = target;
		viewInstance.Open();
	}
}
