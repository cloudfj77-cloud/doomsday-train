using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomUIWindowFrame : MonoBehaviour
{
	[Tooltip("Where builder content is instantiated. Should carry a VerticalLayoutGroup (childControlWidth/Height on, childForceExpandHeight off).")]
	public RectTransform contentRoot;

	[Tooltip("Optional title text on the window header.")]
	public TextMeshProUGUI titleText;

	[Tooltip("Optional close button. CustomUIWindow wires it to Close() automatically.")]
	public Button closeButton;

	[Tooltip("Optional drag handler (usually on the header). Toggled by CustomUIBuilder.SetDraggable.")]
	public UIDragHandler dragHandle;
}
