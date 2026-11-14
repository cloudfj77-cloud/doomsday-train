using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class DialogBubbleWidth : MonoBehaviour
{
	public TextMeshProUGUI tmp;

	public LayoutElement layoutElement;

	public float maxWidth = 400f;

	private void LateUpdate()
	{
		float x = tmp.GetPreferredValues(tmp.text, maxWidth, 0f).x;
		layoutElement.preferredWidth = Mathf.Min(x, maxWidth);
	}
}
