using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BargainPanelFrame : TabletSkin
{
	[Header("Shell")]
	public RectTransform panel;

	public RectTransform inner;

	public TextMeshProUGUI header;

	public RectTransform groupsHost;

	public ScrollRect groupsScroll;

	public Scrollbar groupsScrollbar;

	public RectTransform footer;

	public TextMeshProUGUI repHeader;

	public RectTransform repLineHost;
}
