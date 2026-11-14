using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuickChooseHandler : EmptyActionHandler
{
	public static QuickChooseHandler current;

	[SerializeField]
	public KeyCode cancelKey;

	[SerializeField]
	public KeyCode confirmKey;

	private bool doCancel;

	private bool doConfirm;

	private List<Cancelable> cancelables = new List<Cancelable>();

	private List<Confirmable> confirmables = new List<Confirmable>();

	public override List<KeyCode> keyListeners => new KeyCode[2] { cancelKey, confirmKey }.ToList();

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		current = this;
	}

	public QuickChooseHandler Listen(object c)
	{
		if (c is Cancelable item)
		{
			cancelables.Add(item);
		}
		if (c is Confirmable item2)
		{
			confirmables.Add(item2);
		}
		return this;
	}

	protected override bool OnEventInit()
	{
		Listen(OverlayHandler.current);
		Listen(ItemSelectHandler.current);
		Listen(WindowsHandler.current);
		Listen(ItemMouseDragHandler.current);
		return base.OnEventInit();
	}

	protected override void OnEventPress(ISet<KeyCode> keysPressed)
	{
		if (keysPressed.Contains(cancelKey))
		{
			doCancel = true;
		}
		if (keysPressed.Contains(confirmKey))
		{
			doConfirm = true;
		}
	}

	protected override void OnEventReset()
	{
		doCancel = false;
	}

	protected override bool OnEventUpdate()
	{
		if (doCancel)
		{
			bool result = false;
			if (StoreUIManager.Instance != null)
			{
				result = StoreUIManager.Instance.ResolveEscape();
			}
			else
			{
				foreach (Cancelable cancelable in cancelables)
				{
					if (cancelable.Cancel())
					{
						result = true;
						break;
					}
				}
			}
			doCancel = false;
			doConfirm = false;
			return result;
		}
		if (doConfirm)
		{
			bool result2 = false;
			foreach (Confirmable confirmable in confirmables)
			{
				if (confirmable.Confirm())
				{
					result2 = true;
					break;
				}
			}
			doCancel = false;
			doConfirm = false;
			return result2;
		}
		return false;
	}

	public List<Cancelable> GetCancelables()
	{
		return cancelables;
	}
}
