using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CombatEndedUIController : MonoBehaviour
{
	public Label labelText;

	public Button button;

	private void Start()
	{
		VisualElement rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
		labelText = rootVisualElement.Q<Label>("info");
		button = rootVisualElement.Q<Button>("continu");
		button.clicked += ButtonClicked;
		rootVisualElement.style.display = DisplayStyle.None;
	}

	private void Update()
	{
	}

	public void Display(string text)
	{
		labelText.text = text;
		GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.Flex;
	}

	public void Hide()
	{
		GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.None;
	}

	public void ButtonClicked()
	{
		Hide();
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
	}
}
