using System;
using System.Collections.Generic;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using UnityEngine;

public class ItemGraphicInfo : MonoBehaviour
{
	[Serializable]
	public struct ItemGraphicSocket
	{
		public Transform socketPoint;

		public GameObject showIfPluged;

		public GameObject hideIfPluged;
	}

	public Transform groundPoint;

	[SerializeField]
	protected List<ItemGraphicSocket> sockets = new List<ItemGraphicSocket>();

	public SpriteRenderer fallbackSprite;

	public Dictionary<string, ItemGraphicSocket> socketsDictionary;

	protected bool dicBuilt;

	private Item itemRefrence;

	private List<ItemGraphicInfo> subGraphics;

	public static ItemGraphicInfo spriteGraphicPfb;

	public Item ItemRefrence => itemRefrence;

	public static ItemGraphicInfo CreateAGraphic(Item item, Transform parent, bool snapGround = false, bool useSpriteIfNoGraphic = true)
	{
		if (item == null)
		{
			return null;
		}
		ItemGraphicInfo itemGraphicInfo = null;
		if (item.ItemGraphic == null)
		{
			if (!useSpriteIfNoGraphic)
			{
				return null;
			}
			itemGraphicInfo = UnityEngine.Object.Instantiate(spriteGraphicPfb);
			itemGraphicInfo.fallbackSprite.sprite = item.Icon;
		}
		else
		{
			itemGraphicInfo = UnityEngine.Object.Instantiate(item.ItemGraphic);
		}
		if (parent != null)
		{
			itemGraphicInfo.transform.SetParent(parent);
			itemGraphicInfo.transform.localPosition = Vector3.zero;
			itemGraphicInfo.transform.localRotation = Quaternion.identity;
			itemGraphicInfo.transform.localScale = Vector3.one;
		}
		itemGraphicInfo.Setup(item);
		if (snapGround)
		{
			itemGraphicInfo.SnapGroundPointToParent();
		}
		return itemGraphicInfo;
	}

	public void SnapGroundPointToParent()
	{
		Transform parent = base.transform.parent;
		if (!(parent == null) && (bool)groundPoint)
		{
			Vector3 vector = parent.InverseTransformPoint(groundPoint.position);
			base.transform.localPosition = -vector;
		}
	}

	private void OnSlotContentChanged(Item item, Slot slot)
	{
		if ((bool)itemRefrence && !itemRefrence.IsBeingDestroyed)
		{
			RefreshSubGraphics();
		}
	}

	private void OnDestroy()
	{
		if ((bool)itemRefrence)
		{
			itemRefrence.onSlotContentChanged -= OnSlotContentChanged;
		}
	}

	public void Setup(Item item)
	{
		itemRefrence = item;
		itemRefrence.onSlotContentChanged -= OnSlotContentChanged;
		itemRefrence.onSlotContentChanged += OnSlotContentChanged;
		RefreshSubGraphics();
	}

	private void RefreshSubGraphics()
	{
		if (!itemRefrence)
		{
			return;
		}
		if (subGraphics != null)
		{
			int num;
			for (num = 0; num < subGraphics.Count; num++)
			{
				ItemGraphicInfo itemGraphicInfo = subGraphics[num];
				subGraphics.RemoveAt(num);
				num--;
				if (itemGraphicInfo != null)
				{
					UnityEngine.Object.Destroy(itemGraphicInfo.gameObject);
				}
			}
		}
		subGraphics = new List<ItemGraphicInfo>();
		if (!dicBuilt || socketsDictionary == null)
		{
			dicBuilt = true;
			socketsDictionary = new Dictionary<string, ItemGraphicSocket>();
			foreach (ItemGraphicSocket socket in sockets)
			{
				socketsDictionary.Add(socket.socketPoint.name, socket);
			}
		}
		foreach (KeyValuePair<string, ItemGraphicSocket> item in socketsDictionary)
		{
			ItemGraphicSocket value = item.Value;
			if ((bool)value.showIfPluged)
			{
				value.showIfPluged.SetActive(value: false);
			}
			if ((bool)value.hideIfPluged)
			{
				value.hideIfPluged.SetActive(value: true);
			}
		}
		if (!(itemRefrence.Slots != null) || itemRefrence.Slots.Count <= 0)
		{
			return;
		}
		foreach (Slot slot in itemRefrence.Slots)
		{
			Item content = slot.Content;
			if (content == null)
			{
				continue;
			}
			string key = slot.Key;
			if (!socketsDictionary.TryGetValue(key, out var value2))
			{
				continue;
			}
			ItemGraphicInfo itemGraphicInfo2 = CreateAGraphic(content, value2.socketPoint, snapGround: false, useSpriteIfNoGraphic: false);
			if ((bool)itemGraphicInfo2)
			{
				if ((bool)value2.showIfPluged)
				{
					value2.showIfPluged.SetActive(value: true);
				}
				if ((bool)value2.hideIfPluged)
				{
					value2.hideIfPluged.SetActive(value: false);
				}
				subGraphics.Add(itemGraphicInfo2);
			}
		}
	}

	protected virtual void AutoSet()
	{
		Transform transform = base.transform.Find("Sockets");
		if (!transform)
		{
			return;
		}
		sockets.Clear();
		foreach (Transform item2 in transform)
		{
			ItemGraphicSocket item = new ItemGraphicSocket
			{
				socketPoint = item2
			};
			Transform transform3 = FindDeepChild(base.transform, "HideIf_" + item2.name);
			if ((bool)transform3)
			{
				item.hideIfPluged = transform3.gameObject;
			}
			Transform transform4 = FindDeepChild(base.transform, "ShowIf_" + item2.name);
			if ((bool)transform4)
			{
				item.showIfPluged = transform4.gameObject;
			}
			sockets.Add(item);
		}
	}

	public Transform FindDeepChild(Transform parent, string childName)
	{
		foreach (Transform item in parent)
		{
			if (item.name == childName)
			{
				return item;
			}
			Transform transform2 = FindDeepChild(item, childName);
			if (transform2 != null)
			{
				return transform2;
			}
		}
		return null;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(base.transform.position, new Vector3(0.75f, 0.75f, 0.75f));
	}
}
