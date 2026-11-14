using TMPro;
using UnityEngine;

public class HandnoteUIManager : MonoBehaviour
{
	public GameObject panel;

	public TextMeshProUGUI title;

	public TextMeshProUGUI body;

	public TextMeshProUGUI sign;

	public TMP_FontAsset fontNotebook;

	public TMP_FontAsset fontSundayFriday;

	public static HandnoteUIManager Instance { get; private set; }

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
	}

	private void Update()
	{
	}

	public void OpenPanel(GameItem gameItem)
	{
		title.text = gameItem.GetTagReadonly("HANDNOTE_TITLE").GetString();
		body.text = gameItem.GetTagReadonly("HANDNOTE_BODY").GetString();
		sign.text = gameItem.GetTagReadonly("HANDNOTE_SIGN").GetString();
		PaperUIManager.Instance.audioSourcePickup.Play();
		panel.SetActive(value: true);
	}

	public void ClosePanel()
	{
		PaperUIManager.Instance.audioSourceDrop.Play();
		panel.SetActive(value: false);
	}
}
