using UnityEngine;
using UnityEngine.UI;

public class ChemFinisherUIManager : MonoBehaviour
{
	public GameObject UIPanel;

	public GameObject LayoutGroup;

	public GameObject ChemFinishElementPrefab;

	public GameItem currentItem;

	public static ChemFinisherUIManager Instance { get; private set; }

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

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OpenUI(GameItem chemFinisherMachine)
	{
		UIPanel.gameObject.SetActive(value: true);
		EmporiumAudioPlayer.Instance.audioButton1.Play();
		currentItem = chemFinisherMachine;
		for (int num = LayoutGroup.transform.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(LayoutGroup.transform.GetChild(num).gameObject);
		}
		foreach (ChemFinisherRecipe chemFinisherRecipe in ChemFinisherRecipe.chemFinisherRecipes)
		{
			ChemFinishElement component = Object.Instantiate(ChemFinishElementPrefab, LayoutGroup.transform).GetComponent<ChemFinishElement>();
			component.identifier = chemFinisherRecipe.identifier;
			component.displayTMP.text = chemFinisherRecipe.displayName;
			component.volumeTMP.text = LocHelper.GetLocalizedMechanic("mech_chem_required_volume", ((float)chemFinisherRecipe.requiredVolume / 1000f).ToString("F2"));
			component.dryTMP.text = (chemFinisherRecipe.isRequireDry ? LocHelper.GetLocalizedMechanic("mech_chem_no_wet") : "");
		}
		ScrollRect componentInParent = LayoutGroup.GetComponentInParent<ScrollRect>(includeInactive: true);
		if (componentInParent != null && componentInParent.content == LayoutGroup.transform)
		{
			componentInParent.StopMovement();
			RectTransform obj = (RectTransform)LayoutGroup.transform;
			obj.anchoredPosition = new Vector2(obj.anchoredPosition.x, 0f);
		}
	}

	public void HideUI()
	{
		UIPanel.gameObject.SetActive(value: false);
		EmporiumAudioPlayer.Instance.audioButton1.Play();
	}

	public void OnChemRecipeSelected(string identifier)
	{
		Debug.Log("Selected Chem Recipe: " + identifier);
		currentItem.ModifyTag("SELECTED_CHEM_ID_TAG", delegate(TagState state)
		{
			state.Enable().SetString(identifier);
		});
	}
}
