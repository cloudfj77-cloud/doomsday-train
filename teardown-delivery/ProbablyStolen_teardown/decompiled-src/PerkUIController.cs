using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerkUIController : MonoBehaviour
{
	public GameObject ui;

	public GameObject availablePerks;

	public GameObject selectedPerks;

	public GameObject perkElementPrefab;

	public TextMeshProUGUI pointCount;

	public TextMeshProUGUI slotCounter;

	private int maxPerkCount = 4;

	private int maxPerkPoint = 3;

	private int currentPerkPoint;

	private int currentPerkCount;

	public Color green;

	public Color red;

	public Color orange;

	public Button startButton;

	public GameObject startingPerkTooltip;

	public RectTransform startingPerkTooltipRect;

	public TextMeshProUGUI startingPerkTooltipTitle;

	public TextMeshProUGUI startingPerkTooltipDescription;

	public TextMeshProUGUI startingPerkTooltipCost;

	public TextMeshProUGUI startingPerkTooltipIncompatible;

	public Sprite normalBackground;

	public Sprite disabledBackground;

	public GameObject perkBlocker;

	public bool isFirstRun;

	[NonSerialized]
	public Transform tooltipHost;

	private Transform tooltipHomeParent;

	private int tooltipHomeSiblingIndex;

	private Vector3 tooltipHomeLocalScale;

	private bool tooltipDocked;

	public static PerkUIController Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		LetterboxManager.LayoutChanged += OnLetterboxLayoutChanged;
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			LetterboxManager.LayoutChanged -= OnLetterboxLayoutChanged;
		}
	}

	private void Start()
	{
		if (!NewGameData.Instance.isInMainMenu || availablePerks == null)
		{
			return;
		}
		StartingPerkList.InitStartingPerk();
		foreach (StartingPerk perk in StartingPerkList.Perks)
		{
			StartingPerkElement component = UnityEngine.Object.Instantiate(perkElementPrefab, availablePerks.transform).GetComponent<StartingPerkElement>();
			component.perk = perk;
			component.id = perk.id;
		}
		OnChange();
	}

	private void Update()
	{
		if (StartingPerkElement.showTooltip && startingPerkTooltip != null && startingPerkTooltip.activeSelf)
		{
			Vector3 tooltipPositionSimple = UIHelper.GetTooltipPositionSimple();
			tooltipPositionSimple.z = startingPerkTooltip.transform.position.z;
			startingPerkTooltip.transform.position = tooltipPositionSimple;
		}
	}

	public void OpenUI()
	{
		ui.SetActive(value: true);
		if (pointCount != null)
		{
			pointCount.text = LocHelper.GetLocalizedMainMenu("main_perk_points_used", currentPerkPoint, maxPerkPoint);
		}
		if (slotCounter != null)
		{
			slotCounter.text = LocHelper.GetLocalizedMainMenu("main_perk_count", currentPerkCount, maxPerkCount);
		}
		isFirstRun = SaveFiles.IndexLoad("totalRunCount", 0) == 0;
		if (perkBlocker != null)
		{
			perkBlocker.SetActive(isFirstRun);
		}
	}

	public void CloseUI()
	{
		ui.SetActive(value: false);
		MainMenuUIController.Instance.darknessOverlay.gameObject.SetActive(value: false);
		NewGameSpecializationUI.Open();
		MainMenuUIController.Instance.OnAnyButtonClick();
	}

	public void HideUI()
	{
		ui.SetActive(value: false);
	}

	public void Reset()
	{
	}

	public void SelectPerk(StartingPerkElement perk)
	{
		perk.transform.SetParent(selectedPerks.transform);
		NewGameData.Instance.startingPerks.Add(perk.perk);
		currentPerkCount++;
		currentPerkPoint += perk.perk.cost;
		maxPerkCount += perk.perk.maxSlot;
		OnChange();
	}

	public void DeselectPerk(StartingPerkElement perk)
	{
		perk.transform.SetParent(availablePerks.transform);
		NewGameData.Instance.startingPerks.Remove(perk.perk);
		currentPerkCount--;
		currentPerkPoint -= perk.perk.cost;
		maxPerkCount -= perk.perk.maxSlot;
		OnChange();
	}

	public void OnChange()
	{
		pointCount.text = LocHelper.GetLocalizedMainMenu("main_perk_points_used", currentPerkPoint, maxPerkPoint);
		slotCounter.text = LocHelper.GetLocalizedMainMenu("main_perk_count", currentPerkCount, maxPerkCount);
		if (currentPerkPoint > maxPerkPoint)
		{
			pointCount.color = red;
		}
		else
		{
			pointCount.color = green;
		}
		if (currentPerkCount > maxPerkCount)
		{
			slotCounter.color = red;
		}
		else
		{
			slotCounter.color = green;
		}
		if (CanStart())
		{
			startButton.interactable = true;
		}
		else
		{
			startButton.interactable = false;
		}
		SortPerkContainer(availablePerks);
		foreach (Transform item in availablePerks.transform)
		{
			if (item.TryGetComponent<StartingPerkElement>(out var component))
			{
				component.background.sprite = (component.CanSelect() ? normalBackground : disabledBackground);
			}
		}
	}

	public bool CanStart()
	{
		if (currentPerkCount <= maxPerkCount && currentPerkPoint <= maxPerkPoint)
		{
			return true;
		}
		return false;
	}

	public void ShowTooltip()
	{
		if (!(startingPerkTooltip == null))
		{
			DockTooltip();
			if (tooltipDocked)
			{
				startingPerkTooltip.transform.SetAsLastSibling();
			}
			startingPerkTooltip.SetActive(value: true);
		}
	}

	public void HideTooltip()
	{
		if (!(startingPerkTooltip == null))
		{
			RestoreTooltipFromDock();
			startingPerkTooltip.SetActive(value: false);
		}
	}

	private void DockTooltip()
	{
		if (!tooltipDocked)
		{
			Transform dockRoot = LetterboxManager.DockRoot;
			if (dockRoot == null)
			{
				dockRoot = tooltipHost;
			}
			if (!(dockRoot == null))
			{
				Transform transform = startingPerkTooltip.transform;
				tooltipHomeParent = transform.parent;
				tooltipHomeSiblingIndex = transform.GetSiblingIndex();
				tooltipHomeLocalScale = transform.localScale;
				transform.SetParent(dockRoot, worldPositionStays: true);
				FlattenTooltipZ(transform);
				tooltipDocked = true;
			}
		}
	}

	private void RestoreTooltipFromDock()
	{
		if (tooltipDocked)
		{
			tooltipDocked = false;
			if (!(startingPerkTooltip == null) && !(tooltipHomeParent == null) && startingPerkTooltip.scene.isLoaded)
			{
				Transform obj = startingPerkTooltip.transform;
				obj.SetParent(tooltipHomeParent, worldPositionStays: true);
				obj.SetSiblingIndex(tooltipHomeSiblingIndex);
				obj.localScale = tooltipHomeLocalScale;
				FlattenTooltipZ(obj);
			}
		}
	}

	private static void FlattenTooltipZ(Transform tooltip)
	{
		Vector3 localPosition = tooltip.localPosition;
		localPosition.z = 0f;
		tooltip.localPosition = localPosition;
	}

	private void OnLetterboxLayoutChanged()
	{
		if (tooltipDocked && LetterboxManager.DockCanvas == null)
		{
			StartingPerkElement.showTooltip = false;
			HideTooltip();
		}
	}

	private void SortPerkContainer(GameObject container)
	{
		List<StartingPerkElement> list = new List<StartingPerkElement>();
		foreach (Transform item in container.transform)
		{
			StartingPerkElement component = item.GetComponent<StartingPerkElement>();
			if (component != null)
			{
				list.Add(component);
			}
		}
		list.Sort(delegate(StartingPerkElement a, StartingPerkElement b)
		{
			int num = b.perk.cost.CompareTo(a.perk.cost);
			return (num != 0) ? num : a.id.CompareTo(b.id);
		});
		for (int i = 0; i < list.Count; i++)
		{
			list[i].transform.SetSiblingIndex(i);
		}
	}
}
