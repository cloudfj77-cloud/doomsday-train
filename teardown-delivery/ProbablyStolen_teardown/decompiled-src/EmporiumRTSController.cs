using UnityEngine;

public class EmporiumRTSController : MonoBehaviour
{
	public GameObject negociationTarget;

	public static EmporiumRTSController Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	public void OnInvestigationToolSelected()
	{
		negociationTarget.gameObject.SetActive(value: true);
	}

	public void OnInvestigationToolUnselected()
	{
		negociationTarget.gameObject.SetActive(value: false);
	}

	public void OnSelected()
	{
		if (IsNegociationToolSelected())
		{
			OnInvestigationToolSelected();
		}
	}

	public void OnDeselected()
	{
		if (!IsNegociationToolSelected())
		{
			OnInvestigationToolUnselected();
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public static bool IsNegociationToolSelected()
	{
		if (RTSController.selectedObjects == null || RTSController.selectedObjects.Count == 0 || RTSController.selectedObjects[0] == null)
		{
			return false;
		}
		if (RTSController.selectedObjects[0].TryGetComponent<NegociationElement>(out var _))
		{
			return true;
		}
		if (RTSController.selectedObjects[0].TryGetComponent<NotebookElement>(out var _))
		{
			return true;
		}
		return false;
	}

	public static bool IsNegociationToolPreviouslySelected()
	{
		if (RTSController.previousSelectedObjects.Count == 0)
		{
			return false;
		}
		if (RTSController.previousSelectedObjects[0].TryGetComponent<NegociationElement>(out var _))
		{
			return true;
		}
		if (RTSController.selectedObjects[0].TryGetComponent<NotebookElement>(out var _))
		{
			return true;
		}
		return false;
	}

	public static NegociationElement GetNegociationElement()
	{
		if (RTSController.selectedObjects[0].TryGetComponent<NegociationElement>(out var component))
		{
			return component;
		}
		return null;
	}

	public static NegociationElement GetNegociationElementPrevious()
	{
		if (RTSController.previousSelectedObjects[0].TryGetComponent<NegociationElement>(out var component))
		{
			return component;
		}
		return null;
	}

	public static NotebookElement GetNotebookElementPrevious()
	{
		if (RTSController.previousSelectedObjects[0].TryGetComponent<NotebookElement>(out var component))
		{
			return component;
		}
		return null;
	}
}
