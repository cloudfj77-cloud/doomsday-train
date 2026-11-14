using UnityEngine;
using UnityEngine.UI;

namespace Duckov.UI;

[CreateAssetMenu]
public class UIPrefabsReference : ScriptableObject
{
	[SerializeField]
	private ItemDisplay itemDisplay;

	[SerializeField]
	private SlotIndicator slotIndicator;

	[SerializeField]
	private SlotDisplay slotDisplay;

	[SerializeField]
	private InventoryEntry inventoryEntry;

	[SerializeField]
	private Button button;

	[SerializeField]
	private ScrollRect scrollRect;

	public ItemDisplay ItemDisplay => itemDisplay;

	public SlotIndicator SlotIndicator => slotIndicator;

	public SlotDisplay SlotDisplay => slotDisplay;

	public InventoryEntry InventoryEntry => inventoryEntry;

	public Button Button => button;

	public ScrollRect ScrollRect => scrollRect;
}
