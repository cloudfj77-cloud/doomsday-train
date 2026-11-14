using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RTSController : MonoBehaviour
{
	public Vector3 worldPosition;

	private static Plane plane = new Plane(Vector3.up, 0f);

	public static List<GameObject> teamPawnBuffer = new List<GameObject>();

	public static List<GameObject> selectedObjects = new List<GameObject>();

	public static List<GameObject> previousSelectedObjects = new List<GameObject>();

	public static int orderGiven = 0;

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && IsDeselectOnClick())
		{
			Interactable.DeselectCurrents();
		}
		Input.GetMouseButtonUp(0);
		if (!Input.GetMouseButtonDown(1) || selectedObjects == null || selectedObjects.Count <= 0)
		{
			return;
		}
		UpdatePawnReceivedOrderCount();
		foreach (GameObject item in selectedObjects.ToList())
		{
			if (item.TryGetComponent<IControlable>(out var component))
			{
				component.OnRightClick(GetMousePositionCollider());
			}
		}
	}

	public static Vector3 GetMousePositionGroundLevel()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (plane.Raycast(ray, out var enter))
		{
			return ray.GetPoint(enter);
		}
		return default(Vector3);
	}

	public static Vector3 GetMousePositionCollider()
	{
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hitInfo, 1000f))
		{
			return hitInfo.point;
		}
		return default(Vector3);
	}

	public static void RemoveObjectFromSelections(GameObject gameObject)
	{
		selectedObjects.Remove(gameObject);
	}

	public static void SelectCurrentParty()
	{
	}

	public static void UpdatePawnReceivedOrderCount()
	{
		orderGiven = 0;
		foreach (GameObject item in selectedObjects.ToList())
		{
			if (item.TryGetComponent<IControlable>(out var _))
			{
				orderGiven++;
			}
		}
	}

	public static void CancelSelectedPawnOrder()
	{
	}

	public static void IssueSearchOrderToSelected()
	{
	}

	public static bool IsDeselectOnClick()
	{
		if (Interactable.isHoveringInteractable && Interactable.currentHoveringInteractable.isControlable)
		{
			return true;
		}
		if (Interactable.isHoveringInteractable && !Interactable.currentHoveringInteractable.isControlable)
		{
			return false;
		}
		if (InteractableUI.isHoveringInteractable)
		{
			return false;
		}
		return true;
	}
}
