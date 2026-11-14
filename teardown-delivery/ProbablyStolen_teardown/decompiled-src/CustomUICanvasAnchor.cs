using UnityEngine;

public class CustomUICanvasAnchor : MonoBehaviour
{
	[Tooltip("Layer id custom windows use to target this canvas, e.g. front / paper / rear / meta / menu")]
	public string layerId;

	[Tooltip("Optional: parent for custom windows. Defaults to this object's RectTransform.")]
	public RectTransform customRoot;

	[Header("Game scene integration")]
	[Tooltip("Add a StoreUI component to custom windows created on this layer, tying them into game scene conventions (hover detection, sibling ordering). Leave off on the main menu anchor: StoreUI needs StoreUIManager.")]
	public bool attachStoreUI;

	[Tooltip("Value for StoreUI.isPaperUI on attached windows. Enable on the paper canvas anchor.")]
	public bool isPaperUI;

	public RectTransform Root
	{
		get
		{
			if (!(customRoot != null))
			{
				return base.transform as RectTransform;
			}
			return customRoot;
		}
	}

	private void OnEnable()
	{
		CustomUIManager.RegisterAnchor(this);
	}

	private void OnDisable()
	{
		CustomUIManager.UnregisterAnchor(this);
	}
}
