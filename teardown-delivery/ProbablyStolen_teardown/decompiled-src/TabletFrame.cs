using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabletFrame : TabletSkin
{
	[Header("Shell")]
	public RectTransform panel;

	public Image art;

	public Image frame;

	public Button closeButton;

	[Header("Section")]
	public RectTransform section;

	public Image sectionHeaderPlate;

	public TextMeshProUGUI sectionHeader;

	public RectTransform infoHost;

	public Image separatorTop;

	public RectTransform body;

	public ScrollRect scroll;

	public RectTransform content;

	public Scrollbar scrollbar;

	public Image scrollHandle;

	public Image arrowsWatermark;

	public Image separatorBottom;

	public RectTransform footer;

	[Header("Buttons")]
	public RectTransform buttonHost;
}
