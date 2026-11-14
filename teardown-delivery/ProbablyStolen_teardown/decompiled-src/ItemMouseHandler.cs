using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemMouseHandler : EmptyActionHandler
{
	public static ItemMouseHandler current;

	[SerializeField]
	public KeyCode key;

	private GameItem currentItem;

	private HighlightManager highlightManager = new HighlightManager();

	private GameItem previousItem;

	public override List<KeyCode> keyListeners => new KeyCode[1] { key }.ToList();

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		current = this;
	}

	protected override void OnEventPress(ISet<KeyCode> keysPressed)
	{
		GameItem gameItem = RenderHandler.RaycastElement<GameItem>(Input.mousePosition);
		currentItem = gameItem;
	}

	protected override void OnEventRelease(ISet<KeyCode> keysPressed)
	{
		currentItem = null;
	}

	protected override void OnEventMouseScroll(Vector2 scrollDelta)
	{
		if (RenderHandler.RaycastElement<GameItem>(Input.mousePosition)?.parentInventory is GameGridScrollableInventory gameGridScrollableInventory)
		{
			gameGridScrollableInventory.Scroll(scrollDelta);
		}
	}

	protected override bool OnEventUpdate()
	{
		GameItem gameItem = RenderHandler.RaycastElement<GameItem>(Input.mousePosition);
		if (gameItem != previousItem)
		{
			highlightManager.Clear();
			SlotMarker slotMarker = gameItem?.GetIncompleteSlotInInventory();
			RenderHandler.BackgroundColor backgroundColor = RenderHandler.BackgroundColor.Red;
			if (slotMarker != null && gameItem.MayRemove() && gameItem.MaxNumRemove() > 0)
			{
				backgroundColor = RenderHandler.BackgroundColor.Green;
			}
			highlightManager.Try(slotMarker, (gameItem == currentItem) ? backgroundColor : RenderHandler.BackgroundColor.White);
		}
		previousItem = gameItem;
		return base.OnEventUpdate();
	}

	protected override void OnEventReset()
	{
		highlightManager.Clear();
		currentItem = null;
	}
}
