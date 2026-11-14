using System.Collections.Generic;
using Duckov.Buildings;
using Duckov.Scenes;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using UnityEngine;

public class ShowcaseAltar : MonoBehaviour
{
	[SerializeField]
	private Showcase showcase;

	[SerializeField]
	private GameObject setActiveWithShowcase;

	public static Dictionary<string, ShowcaseAltar> instances = new Dictionary<string, ShowcaseAltar>();

	public string ID => showcase.ID;

	public ShowcaseAltar GetInstance(string id)
	{
		if (instances.TryGetValue(id, out var value))
		{
			return value;
		}
		return null;
	}

	private void Start()
	{
		if (GetInstance(ID) != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		MultiSceneCore.MoveToActiveWithScene(base.gameObject);
		instances[ID] = this;
		showcase.Item.onSlotContentChanged += OnSlotChanged;
		Refresh();
	}

	private void OnDestroy()
	{
		showcase.Item.onSlotContentChanged -= OnSlotChanged;
	}

	private void OnSlotChanged(Item item, Slot slot)
	{
		Refresh();
	}

	private void Refresh()
	{
		bool status = GetStatus();
		setActiveWithShowcase.SetActive(status);
	}

	private bool GetStatus()
	{
		return showcase.Item.Slots[0].Content != null;
	}
}
