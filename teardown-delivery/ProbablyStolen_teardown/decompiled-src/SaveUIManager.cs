using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveUIManager : MonoBehaviour
{
	public GameObject SaveSlotPrefab;

	public GameObject scrollViewContent;

	private static List<int> s_cachedSlotIds = null;

	private static readonly Dictionary<int, SavePreviewData> s_cachedPreviews = new Dictionary<int, SavePreviewData>();

	public static void UpdateCacheEntry(int slotId, SavePreviewData data)
	{
		if (s_cachedSlotIds != null && !s_cachedSlotIds.Contains(slotId))
		{
			s_cachedSlotIds.Add(slotId);
		}
		s_cachedPreviews[slotId] = data;
	}

	public static void RemoveCacheEntry(int slotId)
	{
		s_cachedSlotIds?.Remove(slotId);
		s_cachedPreviews.Remove(slotId);
	}

	public static void InvalidateCache()
	{
		s_cachedSlotIds = null;
		s_cachedPreviews.Clear();
	}

	public static List<int> GetOrderedSlotIds()
	{
		EnsureCacheLoaded();
		return (from id in s_cachedSlotIds
			where s_cachedPreviews.ContainsKey(id)
			where true
			orderby s_cachedPreviews[id].lastSaveDateTime descending
			select id).ToList();
	}

	public static SavePreviewData GetPreview(int slotId)
	{
		EnsureCacheLoaded();
		if (!s_cachedPreviews.TryGetValue(slotId, out var value))
		{
			return null;
		}
		return value;
	}

	public void OpenUI()
	{
		Reset();
		base.gameObject.SetActive(value: true);
		EnsureCacheLoaded();
		PopulateSlots();
	}

	public void CloseUI()
	{
		base.gameObject.SetActive(value: false);
		MainMenuUIController.Instance.OnAnyButtonClick();
		MainMenuUIController.Instance.darknessOverlay.gameObject.SetActive(value: false);
	}

	public void Reset()
	{
		for (int num = scrollViewContent.transform.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(scrollViewContent.transform.GetChild(num).gameObject);
		}
	}

	private static void EnsureCacheLoaded()
	{
		if (s_cachedSlotIds != null)
		{
			return;
		}
		s_cachedSlotIds = SaveFiles.IndexLoad("activeSaveSlotId", new List<int>());
		foreach (int s_cachedSlotId in s_cachedSlotIds)
		{
			if (!s_cachedPreviews.ContainsKey(s_cachedSlotId))
			{
				SavePreviewData savePreviewData = SaveFiles.IndexLoad<SavePreviewData>("preview_" + s_cachedSlotId, null);
				if (savePreviewData != null)
				{
					s_cachedPreviews[s_cachedSlotId] = savePreviewData;
				}
			}
		}
	}

	private void PopulateSlots()
	{
		foreach (int item in from id in s_cachedSlotIds
			where s_cachedPreviews.ContainsKey(id)
			where true
			orderby s_cachedPreviews[id].lastSaveDateTime descending
			select id)
		{
			if (!s_cachedPreviews.TryGetValue(item, out var value))
			{
				continue;
			}
			SaveSlotElement component = UnityEngine.Object.Instantiate(SaveSlotPrefab, scrollViewContent.transform).GetComponent<SaveSlotElement>();
			component.slotId = item;
			component.startLocale = value.startLocale;
			component.versionTMP.text = value.version;
			component.dateTMP.text = value.lastSaveDateTime.ToString("dd/MM/yyyy HH:mm:ss");
			component.titleTMP.text = value.startName;
			component.infoTMP.text = value.credit.ToString();
			try
			{
				component.titleTMP.text = value.startName + " (" + LocHelper.GetLocalizedMechanic("mech_day_label", value.dayCount) + ")";
				component.infoTMP.text = LocHelper.GetLocalizedUI("network_ui_cost", value.credit);
				component.icon.sprite = ((IconProvider.Instance != null) ? IconProvider.Instance.GetIcon(value.startType, value.startScenarioId) : null);
				if (component.icon.sprite == null && value.startType == NewGameData.StartType.MODDED)
				{
					StartingScenario startingScenario = StartingScenario.Get(value.startScenarioId);
					if (startingScenario != null && !string.IsNullOrEmpty(startingScenario.iconKey) && CustomUIManager.Instance != null)
					{
						component.icon.sprite = CustomUIManager.Instance.GetSprite(startingScenario.iconKey);
					}
				}
			}
			catch (Exception arg)
			{
				Debug.LogError($"[SaveUIManager] Slot {item} row could not be fully filled, showing it with plain text: {arg}");
			}
		}
	}

	public void DiscoverSaves()
	{
		EnsureCacheLoaded();
		List<int> list = SaveFiles.ScanAndRegisterOrphans();
		Debug.Log("[SaveUIManager] DiscoverSaves registered " + list.Count + " save file(s)");
		if (list.Count > 0)
		{
			Reset();
			PopulateSlots();
		}
	}
}
