using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeSelectHandler : EmptyActionHandler
{
	public static RecipeSelectHandler current;

	[SerializeField]
	public KeyCode key;

	[NonSerialized]
	public Action<GameItem> callback;

	public override List<KeyCode> keyListeners => new KeyCode[1] { key }.ToList();

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		current = this;
	}

	protected override void OnEventPress(ISet<KeyCode> keysPressed)
	{
		if (keysPressed.Contains(key) && callback != null)
		{
			GameItem gameItem = RenderHandler.RaycastElement<GameItem>(Input.mousePosition);
			if (gameItem != null)
			{
				callback(gameItem);
			}
		}
	}
}
