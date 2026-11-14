using System.Collections.Generic;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Duckov.UI;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using Saves;
using SodaCraft.Localizations;
using UnityEngine;

namespace Duckov.Buildings;

public class Showcase : MonoBehaviour
{
	[SerializeField]
	private string id;

	[SerializeField]
	[LocalizationKey("Default")]
	private string displayNameKey;

	[SerializeField]
	private Item item;

	public bool snapToGround = true;

	[SerializeField]
	private Transform[] displayParents = new Transform[0];

	[SerializeField]
	private CinemachineVirtualCamera vcam;

	private Dictionary<Slot, ItemGraphicInfo> graphicsInstances = new Dictionary<Slot, ItemGraphicInfo>();

	private bool loaded;

	private bool dirty;

	public string ID => id;

	public string DisplayNameKey => displayNameKey;

	public string DisplayName => DisplayNameKey.ToPlainText();

	public Item Item => item;

	public SlotCollection Slots => Item.Slots;

	public Inventory Inventory => Item.Inventory;

	public CinemachineVirtualCamera Vcam => vcam;

	private string SaveKey => "Showcase_" + id;

	private void Awake()
	{
		Item.onItemTreeChanged += OnTreeChanged;
		if (Vcam != null)
		{
			Vcam.gameObject.SetActive(value: false);
		}
		Slots.TryInitialize();
		Load().Forget();
	}

	private void OnDestroy()
	{
		Slots.Master.onItemTreeChanged -= OnTreeChanged;
		SavesSystem.OnCollectSaveData -= Save;
	}

	private async UniTask Load()
	{
		Item item = await ItemSavesUtilities.LoadItem(SaveKey);
		if (!(item == null))
		{
			if (item.Slots != null)
			{
				foreach (Slot slot2 in item.Slots)
				{
					if (slot2 == null)
					{
						continue;
					}
					Item content = slot2.Content;
					if (!(content == null))
					{
						string key = slot2.Key;
						Slot slot = Slots.GetSlot(key);
						if (slot == null)
						{
							ItemUtilities.SendToPlayerStorage(slot2.Content);
						}
						else
						{
							slot.Plug(content, out var _);
						}
					}
				}
			}
			if (item.Inventory != null)
			{
				int lastItemPosition = item.Inventory.GetLastItemPosition();
				for (int i = 0; i <= lastItemPosition; i++)
				{
					Item item2 = item.Inventory[i];
					if (!(item2 == null))
					{
						item2.Detach();
						Inventory.AddAndMerge(item2, i);
					}
				}
			}
		}
		SavesSystem.OnCollectSaveData += Save;
		loaded = true;
		RefreshAll();
		item.DestroyTree();
	}

	private void Save()
	{
		if (loaded)
		{
			Item.Save(SaveKey);
		}
	}

	private void OnTreeChanged(Item item)
	{
		SetDirty();
	}

	private void SetDirty()
	{
		dirty = true;
	}

	private void Update()
	{
		if (LevelManager.LevelInited && dirty)
		{
			RefreshAll();
			Save();
		}
	}

	private void RefreshAll()
	{
		dirty = false;
		Transform[] array = displayParents;
		foreach (Transform slotDisplayParent in array)
		{
			RefreshSlot(slotDisplayParent);
		}
	}

	private void RefreshSlot(Transform slotDisplayParent)
	{
		string key = slotDisplayParent.name;
		Slot slot = Slots[key];
		if (slot == null)
		{
			return;
		}
		if (graphicsInstances.TryGetValue(slot, out var value))
		{
			if (value != null)
			{
				Object.Destroy(value.gameObject);
			}
			graphicsInstances[slot] = null;
		}
		Item content = slot.Content;
		if (!(content == null))
		{
			_ = content.ItemGraphic;
			ItemGraphicInfo value2 = ItemGraphicInfo.CreateAGraphic(content, slotDisplayParent, snapToGround, useSpriteIfNoGraphic: false);
			graphicsInstances[slot] = value2;
		}
	}

	[ContextMenu("Show UI")]
	public void ShowUI()
	{
		if (!(View.ActiveView != null))
		{
			ShowcaseView.Show(this);
		}
	}
}
