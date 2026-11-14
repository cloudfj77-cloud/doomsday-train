using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MultiPageUIManager : MonoBehaviour
{
	public GameObject panel;

	private int currentPage;

	public List<GameObject> books;

	private int currentBookIndex;

	public GameObject buttonLeft;

	public GameObject buttonRight;

	public TextMeshProUGUI pageNumberText;

	public static MultiPageUIManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		Reset();
	}

	public void OpenUI(int bookIndex)
	{
		Reset();
		currentBookIndex = bookIndex;
		panel.SetActive(value: true);
		currentPage = 0;
		books[bookIndex].SetActive(value: true);
		ShowPage(currentPage);
		UpdateButtons();
		PaperUIManager.Instance.audioSourcePickup.Play();
	}

	public void CloseUI()
	{
		panel.SetActive(value: false);
		Reset();
		PaperUIManager.Instance.audioSourceDrop.Play();
	}

	public void FlipLeftClicked()
	{
		if (currentPage > 0)
		{
			EmporiumAudioPlayer.Instance.flip1.Play();
			currentPage--;
			ShowPage(currentPage);
			UpdateButtons();
		}
	}

	public void FlipRightClicked()
	{
		if (currentPage < books[currentBookIndex].transform.childCount - 1)
		{
			EmporiumAudioPlayer.Instance.flip1.Play();
			currentPage++;
			ShowPage(currentPage);
			UpdateButtons();
		}
	}

	private void ShowPage(int page)
	{
		Transform transform = books[currentBookIndex].transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(value: false);
		}
		transform.GetChild(page).gameObject.SetActive(value: true);
		int num = page + 1;
		int childCount = transform.childCount;
		pageNumberText.text = LocHelper.GetLocalizedUI("ui_page_indicator", num, childCount);
	}

	private void UpdateButtons()
	{
		buttonLeft.SetActive(currentPage > 0);
		buttonRight.SetActive(currentPage < books[currentBookIndex].transform.childCount - 1);
	}

	public void Reset()
	{
		foreach (GameObject book in books)
		{
			book.SetActive(value: false);
		}
	}
}
