using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransitFrame : MonoBehaviour
{
	[Header("Shell")]
	public RectTransform window;

	public TextMeshProUGUI title;

	public Button closeButton;

	public RectTransform cardsHost;

	public RectTransform trackHost;

	[Header("Travel button")]
	public Button travelButton;

	public Image travelOuter;

	public Image travelInner;

	public TextMeshProUGUI travelLabel;

	public CanvasGroup travelGroup;

	[Header("Paging (more than five destinations)")]
	public Button prevButton;

	public Image prevImage;

	public Button nextButton;

	public Image nextImage;

	[Header("Skin: cards")]
	public Sprite cardPlate;

	public Sprite cardHeaderPlate;

	public Sprite cardOutlineNormal;

	public Sprite cardOutlineSelected;

	public Sprite cardOutlineUnavailable;

	public Sprite station;

	public Sprite connector;

	public Sprite frame2;

	public Sprite scrollTrack;

	public Sprite scrollThumb;

	[Header("Skin: destination pictures (by location id; mods use sprite keys instead)")]
	public List<string> imageIds = new List<string>();

	public List<Sprite> imageSprites = new List<Sprite>();

	[Header("Skin: fonts (editor-time defaults, TMPLocalizer swaps per locale)")]
	public TMP_FontAsset fontLubrifont;

	public TMP_FontAsset fontNotoRegular;

	public Sprite GetImage(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		for (int i = 0; i < imageIds.Count && i < imageSprites.Count; i++)
		{
			if (imageIds[i] == id)
			{
				return imageSprites[i];
			}
		}
		return null;
	}
}
