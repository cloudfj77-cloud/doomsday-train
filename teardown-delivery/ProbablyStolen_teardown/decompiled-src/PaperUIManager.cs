using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaperUIManager : MonoBehaviour
{
	public const string JOE_CARD_UI_ID = "joe_card";

	public GameObject paperUIGroup;

	public AudioSource audioSourcePickup;

	public AudioSource audioSourceDrop;

	public GameObject panel;

	public Image panelImage;

	public TextMeshProUGUI title;

	public TextMeshProUGUI body;

	public TextMeshProUGUI signature;

	public Sprite paperBack1;

	public Sprite paperBack2;

	public GameObject contractPanel;

	public GameObject deedPanel;

	public GameObject introPanel;

	public GameObject bookPanel;

	public TextMeshProUGUI bookTitle;

	public TextMeshProUGUI bookSubtitle;

	public TextMeshProUGUI bookBody;

	public TextMeshProUGUI bookPage;

	public int bookCurrentPage;

	public static GameItem currentBook;

	public Button leftButton;

	public Button rightButton;

	public GameObject indexGroup;

	public List<TextMeshProUGUI> indexElements;

	public GameObject slipPanel;

	public TextMeshProUGUI procurementHeader;

	public TextMeshProUGUI procurementClient;

	public TextMeshProUGUI procurementBody;

	public TextMeshProUGUI procurementDueDate;

	public GameObject generalSlipPanel;

	public TextMeshProUGUI generalSlipTitle;

	public TextMeshProUGUI generalSlipBody;

	public GameObject JoeWildBusinessCard;

	public GameObject cigaretteGuidePanel;

	public GameObject foodStampGuidePanel;

	public GameObject injectorGuidePanel;

	public GameObject wantedPanel;

	public GameObject secCardPanel;

	public GameObject cardBrewer;

	public GameObject cardChem;

	public GameObject cardPharma;

	public GameObject cardMining;

	public GameObject cigaretteColorPanel;

	public GameObject codeBookPanel;

	public GameObject cardRev;

	public GameObject secSlip;

	public GameObject lotteryInfo;

	public GameObject augIdGuide;

	private readonly List<GameObject> closedForSleep = new List<GameObject>();

	public static PaperUIManager Instance { get; private set; }

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
		if (JoeWildBusinessCard != null && JoeWildBusinessCard.TryGetComponent<StoreUI>(out var component) && string.IsNullOrEmpty(component.identifier))
		{
			component.identifier = "joe_card";
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OpenUI(string title, string body, string signature = "", int pageType = 1)
	{
		audioSourcePickup.Play();
		panel.SetActive(value: true);
		switch (pageType)
		{
		case 1:
			panelImage.sprite = paperBack1;
			break;
		case 2:
			panelImage.sprite = paperBack2;
			break;
		}
		this.title.text = title;
		this.body.text = body;
		this.signature.text = signature;
	}

	public void CloseUI()
	{
		audioSourceDrop.Play();
		panel.SetActive(value: false);
	}

	public void OpenContract()
	{
		audioSourcePickup.Play();
		contractPanel.SetActive(value: true);
	}

	public void CloseContract()
	{
		audioSourceDrop.Play();
		contractPanel.SetActive(value: false);
	}

	public void OpenDeed()
	{
		audioSourcePickup.Play();
		deedPanel.SetActive(value: true);
	}

	public void CloseDeed()
	{
		audioSourceDrop.Play();
		deedPanel.SetActive(value: false);
	}

	public void CloseBook()
	{
		audioSourceDrop.Play();
		bookPanel.SetActive(value: false);
		currentBook = null;
	}

	public void OpenBook(GameItem gameItem)
	{
		audioSourcePickup.Play();
		bookPanel.SetActive(value: true);
		currentBook = gameItem;
		BookOnFlipPage();
	}

	public void BookOnFlipPage()
	{
		if (currentBook == null)
		{
			return;
		}
		int @int = currentBook.GetTagReadonly("CURRENT_BOOK_PAGE").GetInt();
		if (@int == -1)
		{
			OpenIndex();
			return;
		}
		CloseIndex();
		StoreBookData bookById = StoreBookData.GetBookById(currentBook.GetTagReadonly("BOOK_ID").GetString());
		bookTitle.text = bookById.title;
		bookSubtitle.text = bookById.subtitle[@int];
		bookBody.text = bookById.body[@int];
		if (BookHelper.CanFlipLeft(currentBook))
		{
			leftButton.gameObject.SetActive(value: true);
		}
		else
		{
			leftButton.gameObject.SetActive(value: false);
		}
		if (BookHelper.CanFlipRight(currentBook))
		{
			rightButton.gameObject.SetActive(value: true);
		}
		else
		{
			rightButton.gameObject.SetActive(value: false);
		}
		bookPage.text = "Page " + (currentBook.GetTagReadonly("CURRENT_BOOK_PAGE").GetInt() + 1);
	}

	public void BookOnFlipPage(int page)
	{
		StoreBookData bookById = StoreBookData.GetBookById(currentBook.GetTagReadonly("BOOK_ID").GetString());
		if (page <= bookById.pageCount)
		{
			currentBook.ModifyTag("CURRENT_BOOK_PAGE", delegate(TagState state)
			{
				state.Enable().SetInt(page);
			});
			BookOnFlipPage();
		}
	}

	public void BookIndexClick()
	{
		currentBook.ModifyTag("CURRENT_BOOK_PAGE", delegate(TagState state)
		{
			state.Enable().SetInt(0);
		});
		BookOnFlipPage();
		audioSourceDrop.Play();
	}

	public void BookLeftClick()
	{
		if (BookHelper.CanFlipLeft(currentBook))
		{
			int currentPage = currentBook.GetTagReadonly("CURRENT_BOOK_PAGE").GetInt();
			currentBook.ModifyTag("CURRENT_BOOK_PAGE", delegate(TagState state)
			{
				state.Enable().SetInt(currentPage - 1);
			});
			if (currentPage - 1 == -1)
			{
				OpenIndex();
			}
			else
			{
				BookOnFlipPage();
			}
			audioSourceDrop.Play();
		}
	}

	public void BookRightClick()
	{
		if (BookHelper.CanFlipRight(currentBook))
		{
			int currentPage = currentBook.GetTagReadonly("CURRENT_BOOK_PAGE").GetInt();
			currentBook.ModifyTag("CURRENT_BOOK_PAGE", delegate(TagState state)
			{
				state.Enable().SetInt(currentPage + 1);
			});
			BookOnFlipPage();
			audioSourceDrop.Play();
		}
	}

	public void OnIndexClick(int page)
	{
		BookOnFlipPage(page);
		audioSourceDrop.Play();
	}

	public void CloseIndex()
	{
		indexGroup.SetActive(value: false);
	}

	public void OpenIndex()
	{
		audioSourceDrop.Play();
		bookSubtitle.text = "";
		bookBody.text = "";
		currentBook.ModifyTag("CURRENT_BOOK_PAGE", delegate(TagState state)
		{
			state.Enable().SetInt(-1);
		});
		StoreBookData bookById = StoreBookData.GetBookById(currentBook.GetTagReadonly("BOOK_ID").GetString());
		bookTitle.text = bookById.title;
		if (BookHelper.CanFlipLeft(currentBook))
		{
			leftButton.gameObject.SetActive(value: true);
		}
		else
		{
			leftButton.gameObject.SetActive(value: false);
		}
		if (BookHelper.CanFlipRight(currentBook))
		{
			rightButton.gameObject.SetActive(value: true);
		}
		else
		{
			rightButton.gameObject.SetActive(value: false);
		}
		indexGroup.SetActive(value: true);
		foreach (TextMeshProUGUI indexElement in indexElements)
		{
			indexElement.gameObject.SetActive(value: false);
		}
		for (int i = 0; i < bookById.pageCount; i++)
		{
			indexElements[i].gameObject.SetActive(value: true);
			indexElements[i].text = "Page " + (i + 1) + ": " + bookById.subtitle[i];
		}
		bookPage.text = "Index Page";
	}

	public void OpenSlipUI(GameItem gameItem)
	{
		audioSourcePickup.Play();
		slipPanel.SetActive(value: true);
		procurementHeader.text = gameItem.GetTagReadonly("PROCUREMENT_HEADER").GetString();
		procurementClient.text = "By " + gameItem.GetTagReadonly("PROCUREMENT_CLIENT_NAME").GetString();
		procurementBody.text = "Order:\n    " + gameItem.GetTagReadonly("PROCUREMENT_ORDER_BODY").GetString() + "\n    \nReward:\n    " + gameItem.GetTagReadonly("PROCUREMENT_FINAL_REWARD").GetInt() + " Credits";
		switch (gameItem.GetTagReadonly("PROCUREMENT_DAY").GetInt())
		{
		case 0:
			procurementDueDate.text = "Due today";
			break;
		case 1:
			procurementDueDate.text = "Due tomorrow";
			break;
		default:
			procurementDueDate.text = "Due in " + gameItem.GetTagReadonly("PROCUREMENT_DAY").GetInt() + " Days";
			break;
		}
	}

	public void CloseSlipUI()
	{
		audioSourceDrop.Play();
		slipPanel.SetActive(value: false);
	}

	public void OpenGeneralSlipUI(GameItem gameItem)
	{
		audioSourcePickup.Play();
		generalSlipPanel.SetActive(value: true);
		generalSlipTitle.text = gameItem.GetTagReadonly("SLIP_TITLE").GetString();
		generalSlipBody.text = gameItem.GetTagReadonly("SLIP_BODY").GetString();
	}

	public void CloseGeneralSlipUI()
	{
		audioSourceDrop.Play();
		generalSlipPanel.SetActive(value: false);
	}

	public void ToggleJoeCard()
	{
		if (JoeWildBusinessCard.activeSelf)
		{
			JoeWildBusinessCard.SetActive(value: false);
			return;
		}
		JoeWildBusinessCard.SetActive(value: true);
		audioSourcePickup.Play();
	}

	public void OnJoeCardClosed()
	{
		if (audioSourceDrop != null && audioSourceDrop.isActiveAndEnabled)
		{
			audioSourceDrop.Play();
		}
	}

	public void OpenCigaretteGuide()
	{
		cigaretteGuidePanel.SetActive(value: true);
		audioSourcePickup.Play();
	}

	public void CloseCigaretteGuide()
	{
		cigaretteGuidePanel.SetActive(value: false);
		audioSourceDrop.Play();
	}

	public void OpenStampGuide()
	{
		foodStampGuidePanel.SetActive(value: true);
		audioSourcePickup.Play();
	}

	public void CloseStampGuide()
	{
		foodStampGuidePanel.SetActive(value: false);
		audioSourceDrop.Play();
	}

	public void OpenInjectorGuide()
	{
		injectorGuidePanel.SetActive(value: true);
		audioSourcePickup.Play();
	}

	public void CloseInjectorGuide()
	{
		injectorGuidePanel.SetActive(value: false);
		audioSourceDrop.Play();
	}

	public void OpenWantedPaper()
	{
		wantedPanel.SetActive(value: true);
		audioSourcePickup.Play();
	}

	public void CloseWantedPaper()
	{
		wantedPanel.SetActive(value: false);
		audioSourceDrop.Play();
	}

	public void OpenSecCard()
	{
		secCardPanel.SetActive(value: true);
		audioSourcePickup.Play();
	}

	public void CloseSecCard()
	{
		secCardPanel.SetActive(value: false);
		audioSourceDrop.Play();
	}

	public void OpenCigaretteColor()
	{
		cigaretteColorPanel.SetActive(value: true);
		audioSourcePickup.Play();
	}

	public void CloseCigaretteColor()
	{
		cigaretteColorPanel.SetActive(value: false);
		audioSourceDrop.Play();
	}

	public void OpenCodeBook()
	{
		codeBookPanel.SetActive(value: true);
		audioSourcePickup.Play();
	}

	public void CloseCodeBook()
	{
		codeBookPanel.SetActive(value: false);
		audioSourceDrop.Play();
	}

	public void OpenBrewerCard()
	{
		cardBrewer.SetActive(value: true);
		audioSourcePickup.Play();
	}

	public void CloseBrewerCard()
	{
		cardBrewer.SetActive(value: false);
		audioSourceDrop.Play();
	}

	public void OpenMiningCard()
	{
		cardMining.SetActive(value: true);
		audioSourcePickup.Play();
	}

	public void CloseMiningCard()
	{
		cardMining.SetActive(value: false);
		audioSourceDrop.Play();
	}

	public void OpenChemCard()
	{
		cardChem.SetActive(value: true);
		audioSourcePickup.Play();
	}

	public void CloseChemCard()
	{
		cardChem.SetActive(value: false);
		audioSourceDrop.Play();
	}

	public void OpenPharmaCard()
	{
		cardPharma.SetActive(value: true);
		audioSourcePickup.Play();
	}

	public void ClosePharmaCard()
	{
		cardPharma.SetActive(value: false);
		audioSourceDrop.Play();
	}

	public void OpenCard(string id, GameItem gameItem = null)
	{
		if (id.Equals("card_rev"))
		{
			cardRev.SetActive(value: true);
			audioSourcePickup.Play();
		}
		if (id.Equals("sec_slip"))
		{
			secSlip.SetActive(value: true);
			SecuritySlip component = secSlip.GetComponent<SecuritySlip>();
			if (component != null && gameItem != null)
			{
				component.Init(gameItem);
			}
			audioSourcePickup.Play();
		}
	}

	public void CloseCard(string id)
	{
		if (id.Equals("card_rev"))
		{
			cardRev.SetActive(value: false);
			audioSourceDrop.Play();
		}
		if (id.Equals("sec_slip"))
		{
			secSlip.SetActive(value: false);
			audioSourceDrop.Play();
		}
	}

	public void CloseAllPaperUI()
	{
		foreach (Transform item in paperUIGroup.transform)
		{
			item.gameObject.SetActive(value: false);
		}
	}

	public void CloseAllPaperUIForSleep()
	{
		closedForSleep.Clear();
		foreach (Transform item in paperUIGroup.transform)
		{
			if (item.gameObject.activeSelf)
			{
				closedForSleep.Add(item.gameObject);
			}
		}
		CloseAllPaperUI();
	}

	public void RestorePaperUIClosedForSleep(bool morning)
	{
		foreach (GameObject item in closedForSleep)
		{
			if (!(item == null) && !item.activeSelf && (!(EndOfDayUIManager.Instance != null) || !(item == EndOfDayUIManager.Instance.UI)) && (!(StoreUIManager.Instance != null) || !(item == StoreUIManager.Instance.backgroundBlocking)) && (!morning || ((!(NotebookUIManager.Instance != null) || !(item == NotebookUIManager.Instance.panelObject)) && (!(NewsUIManager.Instance != null) || !(item == NewsUIManager.Instance.uiGameObject)) && (!(NegociationUIManager.Instance != null) || !(item == NegociationUIManager.Instance.uiObject)))))
			{
				item.SetActive(value: true);
			}
		}
		closedForSleep.Clear();
	}

	public void OpenLotteryInfo()
	{
		lotteryInfo.SetActive(value: true);
		audioSourcePickup.Play();
	}

	public void CloseLotteryInfo()
	{
		lotteryInfo.SetActive(value: false);
		audioSourceDrop.Play();
	}

	public void OpenAugIdGuide()
	{
		if (augIdGuide != null)
		{
			augIdGuide.SetActive(value: true);
			audioSourcePickup.Play();
		}
	}

	public void CloseAugIdGuide()
	{
		if (augIdGuide != null)
		{
			augIdGuide.SetActive(value: false);
			audioSourceDrop.Play();
		}
	}
}
