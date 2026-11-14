using System.Collections.Generic;
using UnityEngine;

public class ScratcherUIManager : MonoBehaviour
{
	public GameObject ui;

	public GameObject back;

	public GameItem currentActiveScratcherItem;

	public List<ScratchInput> scratchInputs;

	public List<Texture2D> symbolTextures;

	public static ScratcherUIManager Instance { get; private set; }

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

	public void OpenUI(GameItem gameItem)
	{
		ui.SetActive(value: true);
		currentActiveScratcherItem = gameItem;
		PaperUIManager.Instance.audioSourcePickup.Play();
		LotteryHelper.UpdateScratchRadius();
		ScratcherHelper.TryRerollForBetterLuck(gameItem);
		Reset();
		SetPrizeSprite();
		LoadScratch();
		back.SetActive(value: false);
		foreach (ScratchInput scratchInput in scratchInputs)
		{
			scratchInput.gameObject.SetActive(value: true);
		}
	}

	public void CloseUI()
	{
		ui.SetActive(value: false);
		PaperUIManager.Instance.audioSourceDrop.Play();
		currentActiveScratcherItem = null;
		ScratchInput.isScratching = false;
	}

	public void OnScratchComplete(int slot)
	{
		ScratcherHelper.MarkSymbolScratched(currentActiveScratcherItem, slot);
	}

	public void Reset()
	{
		foreach (ScratchInput scratchInput in scratchInputs)
		{
			scratchInput.ResetScratch();
		}
	}

	public void SetPrizeSprite()
	{
		for (int i = 1; i <= 3; i++)
		{
			int @int = currentActiveScratcherItem.GetTagReadonly("SCRATCHER_SYMBOL" + i).GetInt();
			scratchInputs[i - 1].SetTexture(symbolTextures[@int - 1]);
		}
	}

	public void LoadScratch()
	{
		for (int i = 1; i <= 3; i++)
		{
			if (currentActiveScratcherItem.GetTagReadonly("SCRATCHER_STATE" + i).GetInt() == 1)
			{
				scratchInputs[i - 1].RevealAll();
			}
		}
	}

	public void OnTurnButtonClicked()
	{
		if (back.activeSelf)
		{
			back.SetActive(value: false);
			PaperUIManager.Instance.audioSourceDrop.Play();
			{
				foreach (ScratchInput scratchInput in scratchInputs)
				{
					scratchInput.gameObject.SetActive(value: true);
				}
				return;
			}
		}
		back.SetActive(value: true);
		PaperUIManager.Instance.audioSourceDrop.Play();
		foreach (ScratchInput scratchInput2 in scratchInputs)
		{
			scratchInput2.gameObject.SetActive(value: false);
		}
	}
}
