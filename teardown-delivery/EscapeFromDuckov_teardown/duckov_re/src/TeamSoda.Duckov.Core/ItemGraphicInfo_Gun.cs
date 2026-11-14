using System.Collections.Generic;
using ItemStatsSystem;
using UnityEngine;

public class ItemGraphicInfo_Gun : ItemGraphicInfo
{
	public HandheldSocketTypes handheldSocket = HandheldSocketTypes.normalHandheld;

	public HandheldAnimationType handAnimationType = HandheldAnimationType.normal;

	public BowAnimation bowAnimation;

	public GameObject loadedVisualObject;

	private Item item;

	protected override void AutoSet()
	{
		base.AutoSet();
		bowAnimation = GetComponentInChildren<BowAnimation>();
	}

	public void SetSockets(List<Transform> _sockets)
	{
		sockets.Clear();
		foreach (Transform _socket in _sockets)
		{
			if (!(_socket == null))
			{
				ItemGraphicSocket itemGraphicSocket = new ItemGraphicSocket
				{
					socketPoint = _socket
				};
				sockets.Add(itemGraphicSocket);
			}
		}
		dicBuilt = false;
	}
}
