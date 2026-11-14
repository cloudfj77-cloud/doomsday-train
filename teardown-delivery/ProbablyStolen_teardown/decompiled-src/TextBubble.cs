using Febucci.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextBubble : MonoBehaviour
{
	public TextMeshProUGUI textfield;

	public Image backgroundImage;

	public TypewriterByCharacter typewriter;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetColor(Color color)
	{
		backgroundImage.color = color;
	}
}
