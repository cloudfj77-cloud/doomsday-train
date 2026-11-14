using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomEventHandler : MonoBehaviour
{
	public static bool clearDoubleClickOnNextFrame;

	public float doubleClickTimeThreshold = 0.5f;

	private TreeNodeRender rayCastResult;

	private PointerEventData rayCastEventData;

	private bool hasRayCast;

	private float timeDeltaSeconds = 100000f;

	private PointerEventData.InputButton lastButtonDown;

	private TreeNodeRender lastHandlerClicked;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private void Awake()
	{
		clearDoubleClickOnNextFrame = false;
	}

	private void Start()
	{
	}

	private void Update()
	{
		HandleDoubleClickEvent();
		hasRayCast = false;
	}

	private PointerEventData.InputButton GetButtonDown()
	{
		if (Input.GetMouseButtonDown(1))
		{
			return PointerEventData.InputButton.Right;
		}
		if (Input.GetMouseButtonDown(2))
		{
			return PointerEventData.InputButton.Middle;
		}
		return PointerEventData.InputButton.Left;
	}

	private void UpdateRayCast()
	{
		PointerEventData eventData = new PointerEventData(EventSystem.current)
		{
			position = Input.mousePosition,
			scrollDelta = Input.mouseScrollDelta,
			button = GetButtonDown()
		};
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, list);
		int num = -1;
		TreeNodeRender treeNodeRender = null;
		foreach (RaycastResult item in list)
		{
			TreeNodeRender component = item.gameObject.GetComponent<TreeNodeRender>();
			if ((object)component != null && item.depth > num)
			{
				num = item.depth;
				treeNodeRender = component;
			}
		}
		hasRayCast = true;
		rayCastResult = treeNodeRender;
		rayCastEventData = eventData;
	}

	private TreeNodeRender GetRayCast()
	{
		if (!hasRayCast)
		{
			UpdateRayCast();
		}
		return rayCastResult;
	}

	private void HandleDoubleClickEvent()
	{
		if (clearDoubleClickOnNextFrame)
		{
			timeDeltaSeconds = 100000f;
			lastHandlerClicked = null;
			clearDoubleClickOnNextFrame = false;
		}
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
		{
			if (timeDeltaSeconds <= doubleClickTimeThreshold && GetButtonDown() == lastButtonDown && GetRayCast() == lastHandlerClicked)
			{
				if ((object)GetRayCast() != null)
				{
					GetRayCast().OnPointerDoubleClick(rayCastEventData);
				}
				timeDeltaSeconds = 100000f;
				lastHandlerClicked = null;
			}
			else
			{
				lastButtonDown = GetButtonDown();
				lastHandlerClicked = GetRayCast();
				timeDeltaSeconds = 0f;
			}
		}
		timeDeltaSeconds += Time.deltaTime;
	}

	public static void ClearDoubleClickTarget()
	{
		clearDoubleClickOnNextFrame = true;
	}

	public static int GetNumberInput()
	{
		if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			return 0;
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			return 1;
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			return 2;
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			return 3;
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			return 4;
		}
		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			return 5;
		}
		if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			return 6;
		}
		if (Input.GetKeyDown(KeyCode.Alpha7))
		{
			return 7;
		}
		if (Input.GetKeyDown(KeyCode.Alpha8))
		{
			return 8;
		}
		if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			return 9;
		}
		if (Input.GetKeyDown(KeyCode.Keypad0))
		{
			return 0;
		}
		if (Input.GetKeyDown(KeyCode.Keypad1))
		{
			return 1;
		}
		if (Input.GetKeyDown(KeyCode.Keypad2))
		{
			return 2;
		}
		if (Input.GetKeyDown(KeyCode.Keypad3))
		{
			return 3;
		}
		if (Input.GetKeyDown(KeyCode.Keypad4))
		{
			return 4;
		}
		if (Input.GetKeyDown(KeyCode.Keypad5))
		{
			return 5;
		}
		if (Input.GetKeyDown(KeyCode.Keypad6))
		{
			return 6;
		}
		if (Input.GetKeyDown(KeyCode.Keypad7))
		{
			return 7;
		}
		if (Input.GetKeyDown(KeyCode.Keypad8))
		{
			return 8;
		}
		if (Input.GetKeyDown(KeyCode.Keypad9))
		{
			return 9;
		}
		return -1;
	}
}
