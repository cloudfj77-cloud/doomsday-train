using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameFrame : NewGamePanelFrame
{
	[Header("Header")]
	public Button discoverButton;

	[Header("Scrolling list (plates built here at runtime)")]
	public ScrollRect scroll;

	public RectTransform content;

	public Scrollbar scrollbar;

	[Header("Skin: plate pieces")]
	public Sprite iconRing;

	public Sprite deleteNormal;

	public Sprite deleteHover;

	public Sprite duplicateNormal;

	public Sprite duplicateHover;

	[Header("Dialog (delete confirmation and notices)")]
	public GameObject dialogRoot;

	public TextMeshProUGUI dialogTitle;

	public TextMeshProUGUI dialogText;

	public NewGameCommonButton dialogConfirm;

	public NewGameCommonButton dialogCancel;
}
