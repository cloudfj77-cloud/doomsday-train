using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreRequestElement : MonoBehaviour
{
	public TextMeshProUGUI title;

	public TextMeshProUGUI description;

	public TextMeshProUGUI duration;

	public TextMeshProUGUI reward;

	public TextMeshProUGUI faction;

	public TextMeshProUGUI requestedItem;

	public TextMeshProUGUI requester;

	public Button acceptButton;

	public StoreRequest storeRequest;

	public Image image;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Init(StoreRequest request)
	{
		title.text = request.title;
		faction.text = StoreReputation.GetFactionNameByID(request.factionId);
		description.text = request.description;
		requester.text = LocHelper.GetLocalizedMechanic("mech_request_from", request.requesterName);
		if (request.useCustomItemName)
		{
			requestedItem.text = request.requiredItemCount + " " + request.requestedItem;
		}
		else
		{
			requestedItem.text = request.requiredItemCount + " " + GeneralHelper.GetDisplayNameFromID(request.requiredItem);
		}
		image.color = MetaHelper.HexToColor(StoreReputation.GetFactionColorByID(request.factionId));
		duration.text = ((request.duration == 1) ? LocHelper.GetLocalizedMechanic("mech_request_duration_1", request.duration) : LocHelper.GetLocalizedMechanic("mech_request_duration_n", request.duration));
		reward.text = "";
		if (request.creditReward != 0)
		{
			reward.text += LocHelper.GetLocalizedMechanic("mech_request_reward_credits", request.creditReward);
		}
		if (request.reputationReward != 0)
		{
			reward.text += LocHelper.GetLocalizedMechanic("mech_request_reward_rep", request.reputationReward, request.GetFactionDisplayName());
		}
		acceptButton.interactable = request.CanFillRequest();
		storeRequest = request;
	}

	public void OnClick()
	{
		Debug.Log("Element Clicked");
		storeRequest.FillRequest();
		PCUIManager.Instance.OnChange();
		PCUIManager.Instance.storePC.RefreshState();
	}
}
