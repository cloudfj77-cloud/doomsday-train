using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewGamePerksFrame : NewGamePanelFrame
{
	[Header("Texts")]
	public TextMeshProUGUI instruction;

	public TextMeshProUGUI availableLabel;

	public TextMeshProUGUI selectedLabel;

	public TextMeshProUGUI pointsCounter;

	public TextMeshProUGUI perksCounter;

	[Header("Hosts (slots are built here at runtime)")]
	public RectTransform availableSlots;

	public RectTransform selectedSlots;

	public Button prevButton;

	public Image prevImage;

	public Button nextButton;

	public Image nextImage;

	[Tooltip("Arrows of the selected row; shown only when more than ten perks are picked.")]
	public Button selectedPrevButton;

	public Image selectedPrevImage;

	public Button selectedNextButton;

	public Image selectedNextImage;

	[Header("Skin: perk slots (64x72, base shadow included)")]
	public Sprite perkSlotNormal;

	public Sprite perkSlotHover;

	[Tooltip("16x18 lock drawn over an incompatible perk's icon.")]
	public Sprite perkSlotLock;

	[Tooltip("The same lock in red, on the tooltip's \"Locked by perk\" line (NewGamePerkTooltip).")]
	public Sprite perkTooltipLock;
}
