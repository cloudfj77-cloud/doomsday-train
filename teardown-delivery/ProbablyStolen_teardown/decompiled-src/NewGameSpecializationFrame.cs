using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewGameSpecializationFrame : NewGamePanelFrame
{
	[Header("Icon strip")]
	public RectTransform stripSlots;

	public Button prevButton;

	public Image prevImage;

	public Button nextButton;

	public Image nextImage;

	[Header("Scenario texts")]
	public TextMeshProUGUI title;

	public new TextMeshProUGUI tag;

	public TextMeshProUGUI description;

	public TextMeshProUGUI startWithLabel;

	public RectTransform bulletsHost;

	public TextMeshProUGUI warning;

	[Header("Skin: slots (64x64)")]
	public Sprite slotNormal;

	public Sprite slotSelected;

	[Header("Skin: scenario icons")]
	[Tooltip("Every sprite of meta_ui/new_game/icons.png; looked up as \"icons_\" + scenario id.")]
	public Sprite[] icons;

	public Sprite FindIcon(string spriteName)
	{
		if (icons == null || string.IsNullOrEmpty(spriteName))
		{
			return null;
		}
		Sprite[] array = icons;
		foreach (Sprite sprite in array)
		{
			if (sprite != null && sprite.name == spriteName)
			{
				return sprite;
			}
		}
		return null;
	}
}
