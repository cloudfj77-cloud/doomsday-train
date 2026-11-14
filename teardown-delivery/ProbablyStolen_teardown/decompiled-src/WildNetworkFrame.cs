using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WildNetworkFrame : MonoBehaviour
{
	[Header("Dynamic hosts")]
	public RectTransform tabBar;

	public ScrollRect scroll;

	public RectTransform gridContent;

	[Header("Info column")]
	public TextMeshProUGUI infoTitle;

	public TextMeshProUGUI infoSubtitle;

	public TextMeshProUGUI infoDescription;

	public TextMeshProUGUI infoCooldown;

	public TextMeshProUGUI infoMessage;

	public TextMeshProUGUI fundsLabel;

	public TextMeshProUGUI fundsValue;

	public TextMeshProUGUI favorLabel;

	public TextMeshProUGUI favorValue;

	[Header("Use / Unlock button")]
	public Button useButton;

	public Image useOuterBorder;

	public Image useInnerBorder;

	public TextMeshProUGUI useLabel;

	public TextMeshProUGUI usePrice;

	[Header("Skin: tabs (256x42 exports, sliced 4)")]
	public Sprite tabNormal;

	public Sprite tabHover;

	public Sprite tabActive;

	[Header("Skin: cards (exports, sliced 4)")]
	public Sprite cardPlate;

	public Sprite cardHeaderPlate;

	[Header("Skin: slots (84x84 exports)")]
	public Sprite slotFrame;

	public Sprite slotInset;

	public Sprite slotBorderNormal;

	public Sprite slotBorderPurchased;

	public Sprite slotBorderHover;

	public Sprite priceTagNormal;

	public Sprite priceTagUnavailable;

	public Sprite connector;

	[Tooltip("Padlock overlay for demo-locked upgrades.")]
	public Sprite padlock;

	[Header("Skin: window chrome")]
	public Sprite windowFrame;

	public Sprite headerBar;

	public Sprite crossNormal;

	public Sprite crossHover;

	public Sprite scrollTrack;

	public Sprite scrollThumb;

	[Tooltip("6x6 white frame, 2px border, square corners, sliced 2. Drawn by the generator; the Use button borders.")]
	public Sprite frame2Square;

	[Header("Skin: fonts (editor-time defaults, TMPLocalizer swaps per locale)")]
	public TMP_FontAsset fontLubrifont;

	public TMP_FontAsset fontNotoMedium;

	[Header("Skin: upgrade icons")]
	[Tooltip("Every sprite of networkIcons.png; looked up by sprite name, which matches the upgrade id.")]
	public Sprite[] icons;

	public Sprite FindIcon(string id)
	{
		if (icons == null || string.IsNullOrEmpty(id))
		{
			return null;
		}
		Sprite[] array = icons;
		foreach (Sprite sprite in array)
		{
			if (sprite != null && sprite.name == id)
			{
				return sprite;
			}
		}
		return null;
	}
}
