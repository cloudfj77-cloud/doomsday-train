using System.Collections.Generic;
using Duckov.UI.Animations;
using Duckov.Utilities;
using ItemStatsSystem.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.UI;

public class StorageDock : View
{
	[SerializeField]
	private FadeGroup fadeGroup;

	[SerializeField]
	private StorageDockEntry entryTemplate;

	[SerializeField]
	private GameObject placeHolder;

	[SerializeField]
	private TextMeshProUGUI pageText;

	[SerializeField]
	private Button btnNextPage;

	[SerializeField]
	private Button btnPrevPage;

	private const int itemsPerPage = 24;

	private int page;

	private PrefabPool<StorageDockEntry> _entryPool;

	public static StorageDock Instance => View.GetViewInstance<StorageDock>();

	private int TotalItemCount
	{
		get
		{
			if (PlayerStorage.IncomingItemBuffer == null)
			{
				return 0;
			}
			return PlayerStorage.IncomingItemBuffer.Count;
		}
	}

	private int MaxPage => TotalItemCount / 24;

	private PrefabPool<StorageDockEntry> EntryPool
	{
		get
		{
			if (_entryPool == null)
			{
				_entryPool = new PrefabPool<StorageDockEntry>(entryTemplate);
			}
			return _entryPool;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		entryTemplate.gameObject.SetActive(value: false);
		btnNextPage.onClick.AddListener(NextPage);
		btnPrevPage.onClick.AddListener(PrevPage);
	}

	private void OnEnable()
	{
		PlayerStorage.OnTakeBufferItem += OnTakeBufferItem;
	}

	private void OnDisable()
	{
		PlayerStorage.OnTakeBufferItem -= OnTakeBufferItem;
	}

	private void OnTakeBufferItem()
	{
		Refresh();
	}

	protected override void OnOpen()
	{
		base.OnOpen();
		if (PlayerStorage.Instance == null)
		{
			Close();
			return;
		}
		fadeGroup.Show();
		Setup();
	}

	protected override void OnClose()
	{
		base.OnClose();
		fadeGroup.Hide();
	}

	private void Setup()
	{
		Refresh();
	}

	private void Refresh()
	{
		EntryPool.ReleaseAll();
		List<ItemTreeData> incomingItemBuffer = PlayerStorage.IncomingItemBuffer;
		int num = page * 24;
		int num2 = num + 24;
		if (num2 > incomingItemBuffer.Count)
		{
			num2 = incomingItemBuffer.Count;
		}
		for (int i = num; i < num2; i++)
		{
			ItemTreeData itemTreeData = incomingItemBuffer[i];
			if (itemTreeData != null)
			{
				EntryPool.Get().Setup(i, itemTreeData);
			}
		}
		placeHolder.gameObject.SetActive(incomingItemBuffer.Count <= 0);
		pageText.text = $"{page + 1}/{MaxPage + 1}";
	}

	public void NextPage()
	{
		GotoPage(page + 1);
	}

	public void PrevPage()
	{
		GotoPage(page - 1);
	}

	public void GotoPage(int page)
	{
		if (page < 0)
		{
			page = 0;
		}
		if (page > MaxPage)
		{
			page = MaxPage;
		}
		this.page = page;
		Refresh();
	}

	internal static void Show()
	{
		if (!(Instance == null))
		{
			Instance.Open();
		}
	}
}
