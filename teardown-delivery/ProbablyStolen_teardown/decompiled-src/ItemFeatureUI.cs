using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemFeatureUI : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public TextMeshProUGUI TMP;

	public ItemFeature itemFeature;

	public void OnFeatureUIClick()
	{
		if (itemFeature == null || InspectionUIManager.IsConfrontLocked())
		{
			return;
		}
		Debug.Log("OnFeatureUIClicked");
		if (RTSController.previousSelectedObjects.Count == 0)
		{
			return;
		}
		Debug.Log("OnFeatureUIClicked2");
		Debug.Log(RTSController.previousSelectedObjects[0].name);
		NotebookElement component2;
		if (RTSController.previousSelectedObjects[0].TryGetComponent<NegociationElement>(out var component))
		{
			if (component.IsNegociationTool())
			{
				if (component.GetNegociationToolID().Equals(itemFeature.GetToolNeeded()))
				{
					ExposeFeature();
				}
				else
				{
					OnExposeFeatureBadMatch();
				}
			}
		}
		else if (RTSController.previousSelectedObjects[0].TryGetComponent<NotebookElement>(out component2))
		{
			Debug.Log("OnFeatureUIClicked3");
			if (itemFeature == component2.itemFeature)
			{
				ExposeFeature();
			}
			else
			{
				OnExposeFeatureBadMatch();
			}
		}
	}

	public void ExposeFeature()
	{
		EmporiumAudioPlayer.Instance.PlayMatched();
		if (itemFeature.GetExposeDialog() != null)
		{
			DialogUIManager.Instance.StartDialogue(itemFeature.GetExposeDialog());
		}
		else
		{
			itemFeature.ExposeFeature();
		}
	}

	public void OnExposeFeatureBadMatch()
	{
		EmporiumAudioPlayer.Instance.PlayUnMatched();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (EmporiumRTSController.IsNegociationToolSelected())
		{
			TMP.alpha = 0.75f;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		TMP.alpha = 1f;
	}

	private void Awake()
	{
		TMP = GetComponent<TextMeshProUGUI>();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
